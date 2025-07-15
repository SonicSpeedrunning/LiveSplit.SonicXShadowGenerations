﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JHelper.Common.MemoryUtils;
using JHelper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.GameEngine;

/// <summary>
/// The HedgehogEngine2 class provides memory interaction for games based on the Hedgehog Engine 2.
/// This class has been tailored for Shadow Generations, enabling real-time memory analysis
/// and caching of essential game objects like GameManager, Boss instances, and game modes.
/// </summary>
internal class HedgehogEngine2
{
    /// <summary>
    /// Pointer to the main base address of the game manager instance.
    /// </summary>
    private readonly IntPtr pGameManager; // Points to an instance of hh:game::GameManager

    /// <summary>
    /// Address of the game's routine responsible for pausing the game when the main window goes out of focus
    /// </summary>
    internal IntPtr HWndAddress { get; }

    /// <summary>
    /// Runtime Type Information instance used to look up and identify in-memory types.
    /// </summary>
    internal RTTI RTTI { get; }

    /// <summary>
    /// Dictionary that stores cached addresses for various game services.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _services = [];

    /// <summary>
    /// Dictionary that stores cached addresses for different in-game objects.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _objects = [];

    /// <summary>
    /// Dictionary that stores cached addresses for application extensions.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _extensions = [];

    /// <summary>
    /// Pointer to the current running instance of app::game::GameMode.
    /// </summary>
    internal IntPtr GameMode { get; private set; } = default;
    
    /// <summary>
    /// Initializes a new instance of the HedgehogEngine2 class and locates
    /// the pointer to the main game manager in memory.
    /// </summary>
    /// <param name="process">The target process</param>
    public HedgehogEngine2(ProcessMemory process)
    {
        // Perform memory scan to find GameManager base address.
        pGameManager = process
            .MainModule
            .ScanAll(new ScanPattern(7, "48 89 41 18 48 89 2D") { OnFound = addr => addr + 0x4 + process.Read<int>(addr) })
            .First();

        HWndAddress = process
            .MainModule
            .ScanAll(new ScanPattern(0, "?? 4A C0 E8 03"))
            .First();

        // Initialize RTTI for type information.
        RTTI = new RTTI(process);
    }

    /// <summary>
    /// Resets all cached memory addresses, forcing the next update to re-cache them.
    /// </summary>
    private void ResetCache()
    {
        GameMode = IntPtr.Zero;
        _services.Clear();
        _objects.Clear();
        _extensions.Clear();
    }

    /// <summary>
    /// Updates the cached memory addresses for important game objects by reading from
    /// the process memory. This method should be called periodically to refresh the cached values.
    /// </summary>
    /// <param name="process">The target process memory handler.</param>
    [SkipLocalsInit]
    public void Update(ProcessMemory process)
    {
        // Step 1: Reset the cached addresses at the beginning of each update cycle.
        ResetCache();

        // Step 2: Attempt to read the main GameManager pointer.
        if (!process.ReadPointer(pGameManager, out IntPtr _gameManager))
            return; // Return early if GameManager address is invalid.

        IntPtr gameObjects;
        int noOfGameObjects;

        IntPtr gameServices;
        int noOfGameServices;

        IntPtr gameApplication;
        
        // This is a very large structure so we rather use an ArrayPool than a struct
        using (ArrayRental<byte> rent = new ArrayRental<byte>(0x358))
        {
            if (!process.ReadArray(_gameManager, rent.Span))
                return;

            unsafe
            {
                fixed (byte* ptr = rent.Span)
                {
                    gameObjects = (IntPtr)(*(long*)(ptr + 0x130));
                    noOfGameObjects = *(short*)(ptr + 0x138);
                    gameServices = (IntPtr)(*(long*)(ptr + 0x150));
                    noOfGameServices = *(short*)(ptr + 0x158);
                    gameApplication = (IntPtr)(*(long*)(ptr + 0x350));
                }
            }
        }


        // Scan the game services
        if (noOfGameServices > 0 && noOfGameServices < 1024)
        {
            using (ArrayRental<long> rent = new(noOfGameServices))
            {
                if (process.ReadArray(gameServices, rent.Span))
                {
                    foreach (var entry in rent.Span)
                    {
                        if (RTTI.Lookup((IntPtr)entry, out string value))
                            _services[value] = (IntPtr)entry;
                    }
                }
            }
        }

        // Scan game application extensions.
        if (process.Read(gameApplication, out GameApplication ggameApplication)
            && ggameApplication.noOfApplicationExtensions > 0
            && ggameApplication.noOfApplicationExtensions < 64)
        {
            IntPtr ase = IntPtr.Zero;

            using (ArrayRental<long> rent = new(ggameApplication.noOfApplicationExtensions))
            {
                // Read array of pointers for ApplicationSequenceExtension
                if (process.ReadArray(ggameApplication.ApplicationExtensions, rent.Span))
                {
                    foreach (var entry in rent.Span)
                    {
                        // Identify ApplicationSequenceExtension using RTTI lookup.
                        if (RTTI.Lookup((IntPtr)entry, out string value))
                        {
                            if (value == "ApplicationSequenceExtension")
                            {
                                ase = (IntPtr)entry;
                                break;
                            }
                        }
                    }
                }
            }

            // If ApplicationSequenceExtension is found, locate GameMode.
            if (ase != IntPtr.Zero)
            {
                // Try to read the pointer for GameMode instance.
                if (process.Read(ase, out ApplicationSequenceExtension extension))
                {
                    GameMode = extension.GameMode;

                    // Read current instance of GameModeExtension.
                    if (process.Read(extension.GameMode, out GameMode gameMode) && gameMode.noOfExtensions > 0 && gameMode.noOfExtensions < 128)
                    {
                        using (ArrayRental<long> rent = gameMode.noOfExtensions < 25 ? new(stackalloc long[gameMode.noOfExtensions]) : new(gameMode.noOfExtensions))
                        {
                            // Retrieve array of extensions
                            if (process.ReadArray(gameMode.Extensions, rent.Span))
                            {
                                foreach (var entry in rent.Span)
                                {
                                    if (RTTI.Lookup((IntPtr)entry, out string value))
                                        _extensions[value] = (IntPtr)entry;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Scan the game objects.
        if (noOfGameObjects > 0 && noOfGameObjects < 2048)
        {
            using (ArrayRental<long> rent = new(noOfGameObjects))
            {
                if (process.ReadArray(gameObjects, rent.Span))
                {
                    foreach (var entry in rent.Span)
                    {
                        if (RTTI.Lookup((IntPtr)entry, out string value))
                        {
                            // We are excluding elements starting with "Obj"
                            if (value.Length > 2 && value[0] == 'O' && value[1] == 'b' && value[2] == 'j')
                                continue;

                            _objects[value] = (IntPtr)entry;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retrieves a cached instance pointer of a game service by name.
    /// </summary>
    /// <param name="name">The name of the service to retrieve.</param>
    /// <param name="instance">The instance pointer of the service, if found.</param>
    /// <returns>True if the service is found; otherwise, false.</returns>
    public bool GetService(string name, out IntPtr instance)
    {
        return _services.TryGetValue(name, out instance);
    }

    /// <summary>
    /// Retrieves a cached instance pointer of a game object by name.
    /// </summary>
    /// <param name="name">The name of the object to retrieve.</param>
    /// <param name="instance">The instance pointer of the object, if found.</param>
    /// <returns>True if the object is found; otherwise, false.</returns>
    public bool GetObject(string name, out IntPtr instance)
    {
        return _objects.TryGetValue(name, out instance);
    }

    /// <summary>
    /// Retrieves a cached instance pointer of an application extension by name.
    /// </summary>
    /// <param name="name">The name of the extension to retrieve.</param>
    /// <param name="instance">The instance pointer of the extension, if found.</param>
    /// <returns>True if the extension is found; otherwise, false.</returns>
    public bool GetExtension(string name, out IntPtr instance)
    {
        return _extensions.TryGetValue(name, out instance);
    }
}
