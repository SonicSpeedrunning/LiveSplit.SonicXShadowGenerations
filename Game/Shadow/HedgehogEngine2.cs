using System;
using System.Buffers;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Runtime Type Information instance for looking up in-memory types.
    /// </summary>
    internal RTTI RTTI { get; }

    // Cached addresses for frequently accessed game objects.
    internal IntPtr GameManager { get; private set; } = default; // An instance of hh::game::GameManager
    internal IntPtr BossPerfectBlackDoomFinal { get; private set; } = default; // An instance of app::BossPerfectBlackDoomFinal
    internal IntPtr EventQTEInput { get; private set; } = default; // An instance of app::evt::EventQTEInput
    internal IntPtr MyApplication { get; private set; } = default; // An instance of app::MyApplication
    internal IntPtr ApplicationSequenceExtension { get; private set; } = default; // An instance of app::game::ApplicationSequenceExtension
    internal IntPtr GameMode { get; private set; } = default; // The current running instance of app::game::GameMode
    internal IntPtr GameModeHsmExtension { get; private set; } = default; // The current running instance of app::game::GameModeHsmExtension
    internal int GameModeExtensionCount { get; private set; } = default; // The number of currently running instances of type app::game::GameModeExtension

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
    /// Resets all cached memory addresses, forcing the next update to re-cache.
    /// </summary>
    private void ResetCache()
    {
        GameManager = IntPtr.Zero;
        BossPerfectBlackDoomFinal = IntPtr.Zero;
        EventQTEInput = IntPtr.Zero;
        MyApplication = IntPtr.Zero;
        ApplicationSequenceExtension = IntPtr.Zero;
        GameMode = IntPtr.Zero;
        GameModeHsmExtension = IntPtr.Zero;
        GameModeExtensionCount = 0;
    }

    /// <summary>
    /// Updates the cached memory addresses for important game objects by reading from
    /// the process memory. This method should be called periodically to refresh the cached values.
    /// </summary>
    /// <param name="process">The target process memory handler.</param>
    public void Update(ProcessMemory process)
    {
        long[] rent;

        // Step 1: Reset the cached addresses at the beginning of each update cycle.
        ResetCache();

        // Step 2: Attempt to read the main GameManager pointer.
        if (!process.ReadPointer(pGameManager, out IntPtr addr))
            return; // Return early if GameManager address is invalid.
        GameManager = addr;

        // Step 3: Read pointer to MyApplication; skip if invalid.
        if (process.ReadPointer(GameManager + 0x350, out addr))
        {
            MyApplication = addr;

            // Retrieve ApplicationSequenceExtension if valid.
            if (process.Read(MyApplication + 0x88, out ArrayAndSize array) && array.size != 0 && array.size < 64)
            {
                rent = ArrayPool<long>.Shared.Rent(array.size);
                Span<long> span = rent.AsSpan(0, array.size);

                // Read array of pointers for ApplicationSequenceExtension
                if (process.ReadArray(array.Entries, span))
                {
                    foreach (var entry in span)
                    {
                        // Identify ApplicationSequenceExtension using RTTI lookup.
                        if (RTTI.Lookup((IntPtr)entry, out string value))
                        {
                            if (value == "ApplicationSequenceExtension")
                                ApplicationSequenceExtension = (IntPtr)entry;
                        }

                        if (ApplicationSequenceExtension.IsNotZero())
                            break;
                    }
                }
                ArrayPool<long>.Shared.Return(rent);

                // If ApplicationSequenceExtension is found, locate GameMode.
                if (ApplicationSequenceExtension.IsNotZero())
                {
                    // Try to read the pointer for GameMode instance.
                    if (process.ReadPointer(ApplicationSequenceExtension + 0x78, out addr))
                    {
                        GameMode = addr;

                        // Read current instance of GameModeExtension.
                        if (process.Read(GameMode + 0xB0, out array) && array.size != 0 && array.size < 128)
                        {
                            GameModeExtensionCount = array.size;
                            rent = ArrayPool<long>.Shared.Rent(GameModeExtensionCount);
                            span = rent.AsSpan(0, GameModeExtensionCount);

                            // Retrieve array of extensions, looking for GameModeHsmExtension.
                            if (process.ReadArray(array.Entries, span))
                            {
                                foreach (var entry in span)
                                {
                                    if (RTTI.Lookup((IntPtr)entry, out string value))
                                    {
                                        if (value == "GameModeHsmExtension")
                                            GameModeHsmExtension = (IntPtr)entry;
                                    }

                                    if (GameModeHsmExtension.IsNotZero())
                                        break;
                                }
                            }
                            ArrayPool<long>.Shared.Return(rent);
                        }
                    }
                }
            }
        }

        // Step 4: Check if in final boss fight by verifying level ID.
        // This check is necessary in order to optimize memory by avoiding
        // scanning the game objects array when not necessary.
        Span<byte> levelID = stackalloc byte[6];
        Span<byte> seq = stackalloc byte[6] { (byte)'w', (byte)'1', (byte)'4', (byte)'b', (byte)'1', (byte)'0' };
        if (process.ReadArray(ApplicationSequenceExtension + 0xA0, levelID) && levelID.SequenceEqual(seq))
        {
            // Read BossPerfectBlackDoomFinal instance for final boss.
            if (process.Read(GameManager + 0x130, out ArrayAndSize array) && array.size != 0 && array.size < 4096)
            {
                rent = ArrayPool<long>.Shared.Rent(array.size);
                Span<long> span = rent.AsSpan(0, array.size);

                // Search for boss-related objects.
                if (process.ReadArray(array.Entries, span))
                {
                    foreach (var entry in span)
                    {
                        if (RTTI.Lookup((IntPtr)entry, out string value))
                        {
                            if (value == "BossPerfectBlackDoomFinal")
                                BossPerfectBlackDoomFinal = (IntPtr)entry;
                            else if (value == "EventQTEInput")
                                EventQTEInput = (IntPtr)entry;
                        }

                        // Break if both critical objects are found.
                        if (BossPerfectBlackDoomFinal.IsNotZero() && EventQTEInput.IsNotZero())
                            break;
                    }
                }
                ArrayPool<long>.Shared.Return(rent);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    private readonly struct ArrayAndSize
    {
        [FieldOffset(0x0)] private readonly long _instance;
        [FieldOffset(0x8)] public readonly int size;

        public IntPtr Entries => (IntPtr)_instance;
    }
}
