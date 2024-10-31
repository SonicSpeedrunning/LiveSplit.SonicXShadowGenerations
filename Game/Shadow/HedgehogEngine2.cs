using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helper.Common.ProcessInterop;
using LiveSplit.SonicXShadowGenerations.Common;

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
    internal IntPtr hWndAddress { get; }

    /// <summary>
    /// Runtime Type Information instance used to look up and identify in-memory types.
    /// </summary>
    internal RTTI RTTI { get; }

    /// <summary>
    /// Dictionary that stores cached addresses for various game services.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _services = new();

    /// <summary>
    /// Dictionary that stores cached addresses for different in-game objects.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _objects = new();

    /// <summary>
    /// Dictionary that stores cached addresses for various application extensions.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _extensions = new();

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
            .ScanAll(new MemoryScanPattern(7, "48 89 41 18 48 89 2D") { OnFound = addr => addr + 0x4 + process.Read<int>(addr) })
            .First();

        hWndAddress = process
            .MainModule
            .ScanAll(new MemoryScanPattern(0, "?? 4A C0 E8 03"))
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
        long[] rent;

        // Step 1: Reset the cached addresses at the beginning of each update cycle.
        ResetCache();

        // Step 2: Attempt to read the main GameManager pointer.
        if (!process.ReadPointer(pGameManager, out IntPtr _gameManager))
            return; // Return early if GameManager address is invalid.

        if (!process.Read(_gameManager, out GameManager gameManager))
            return;

        // Scan the game services
        if (gameManager.noOfGameServices > 0 && gameManager.noOfGameServices < 1024)
        {
            rent = ArrayPool<long>.Shared.Rent(gameManager.noOfGameServices);
            Span<long> span = rent.AsSpan(0, gameManager.noOfGameServices);

            if (process.ReadArray(gameManager.GameServices, span))
            {
                foreach (var entry in span)
                {
                    if (RTTI.Lookup((IntPtr)entry, out string value))
                        _services[value] = (IntPtr)entry;
                }
            }
            ArrayPool<long>.Shared.Return(rent);
        }

        // Scan game application extensions.
        if (process.Read(gameManager.GameApplication, out GameApplication gameApplication)
            && gameApplication.noOfApplicationExtensions > 0
            && gameApplication.noOfApplicationExtensions < 64)
        {
            rent = ArrayPool<long>.Shared.Rent(gameApplication.noOfApplicationExtensions);
            Span<long> span = rent.AsSpan(0, gameApplication.noOfApplicationExtensions);

            IntPtr ase = IntPtr.Zero;

            // Read array of pointers for ApplicationSequenceExtension
            if (process.ReadArray(gameApplication.ApplicationExtensions, span))
            {
                foreach (var entry in span)
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
            ArrayPool<long>.Shared.Return(rent);

            // If ApplicationSequenceExtension is found, locate GameMode.
            if (ase.IsNotZero())
            {
                // Try to read the pointer for GameMode instance.
                if (process.Read(ase, out ApplicationSequenceExtension extension))
                {
                    GameMode = extension.GameMode;

                    // Read current instance of GameModeExtension.
                    if (process.Read(GameMode, out GameMode gameMode) && gameMode.noOfExtensions > 0 && gameMode.noOfExtensions < 128)
                    {
                        rent = ArrayPool<long>.Shared.Rent(gameMode.noOfExtensions);
                        span = rent.AsSpan(0, gameMode.noOfExtensions);

                        // Retrieve array of extensions
                        if (process.ReadArray(gameMode.Extensions, span))
                        {
                            foreach (var entry in span)
                            {
                                if (RTTI.Lookup((IntPtr)entry, out string value))
                                    _extensions[value] = (IntPtr)entry;
                            }
                        }

                        ArrayPool<long>.Shared.Return(rent);
                    }
                }
            }
        }

        // Scan the game objects.
        if (gameManager.noOfGameObjects > 0 && gameManager.noOfGameObjects < 2048)
        {
            rent = ArrayPool<long>.Shared.Rent(gameManager.noOfGameObjects);
            Span<long> span = rent.AsSpan(0, gameManager.noOfGameObjects);

            if (process.ReadArray(gameManager.GameObjects, span))
            {
                foreach (var entry in span)
                {
                    if (RTTI.Lookup((IntPtr)entry, out string value))
                    {
                        // We are excluding elements starting with "Obj" because essentially useless
                        if (value.Length > 2 && value[0] == 'O' && value[1] == 'b' && value[2] == 'j')
                            continue;

                        _objects[value] = (IntPtr)entry;
                    }
                }
            }

            ArrayPool<long>.Shared.Return(rent);
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
