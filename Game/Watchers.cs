using System;
using System.Linq;
using System.Buffers;
using Helper.Common.Collections;
using Helper.Common.MemoryUtils;
using Helper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.Game;

internal class Watchers
{
    private MemStateTracker _stateTracker;

    public LazyWatcher<bool> IsLoading { get; }


    // Cached addresses
    private IntPtr Address_Application { get; set; }
    private IntPtr Address_ApplicationSequence { get; set; }
    private IntPtr Address_GameMode { get; set; }
    private IntPtr Address_GameModeExtension { get; set; }

    // Useful cached values
    private int GameModeExtensionCount { get; set; }


    public Watchers(ProcessMemory process, Memory memory)
    {
        _stateTracker = new();

        IsLoading = new LazyWatcher<bool>(_stateTracker, (_, _) =>
        {
            return false;
        });
    }

    public void Update(ProcessMemory process, Memory memory)
    {
        _stateTracker.Tick();

        // Reset the cached addresses
        Address_Application = default;
        Address_ApplicationSequence = default;
        Address_GameMode = default;
        Address_GameModeExtension = default;
        // And reset the cached values as well
        GameModeExtensionCount = default;

        if (!process.ReadPointer(memory.BaseEngineAddress, out IntPtr ptr) || ptr == IntPtr.Zero)
            return;
        Address_Application = ptr;

        if (!process.Read<byte>(Address_Application + memory.Offset_Application + 0x8, out byte applicationSequenceCount) || applicationSequenceCount == 0)
            return;
        
        long[] rent = ArrayPool<long>.Shared.Rent(applicationSequenceCount);

        try
        {
            if (!process.ReadArray<long>(Address_Application + memory.Offset_Application, rent.AsSpan(0, applicationSequenceCount)))
                return;

            Address_ApplicationSequence = rent
                .Take(applicationSequenceCount)
                .Select(item => 
                {
                    if (!process.ReadPointer((IntPtr)item, out IntPtr instance) || instance == IntPtr.Zero)
                        return IntPtr.Zero;

                    if (!memory.RTTILookup(process, instance, out string name) || name != "ApplicationSequenceExtension@game@app@@")
                        return IntPtr.Zero;

                    return instance;
                })
                .FirstOrDefault(item => item != IntPtr.Zero);

            if (Address_ApplicationSequence == IntPtr.Zero)
                return;
        }
        finally
        {
            ArrayPool<long>.Shared.Return(rent);
        }

        if (!process.ReadPointer(Address_ApplicationSequence + memory.Offset_GameMode, out ptr))
            return;
        Address_GameMode = ptr;

        if (!process.ReadPointer(Address_GameMode + memory.Offset_GameModeExtension, out ptr))
            return;
        Address_GameModeExtension = ptr;

        if (!process.Read<byte>(Address_GameMode + memory.Offset_GameModeExtension + 0x8, out byte gameModeExtensionCount))
            return;
        GameModeExtensionCount = gameModeExtensionCount;

        rent = ArrayPool<long>.Shared.Rent(GameModeExtensionCount);
        try
        {
            if (!process.ReadArray(Address_GameModeExtension, rent.AsSpan(0, GameModeExtensionCount)))
                return;


        }
        finally
        {
            ArrayPool<long>.Shared.Return(rent);
        }
    }
}
