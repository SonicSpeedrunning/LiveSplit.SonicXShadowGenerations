using System;
using System.Buffers;
using System.Linq;
using Helper.Common.Collections;
using Helper.Common.MemoryUtils;
using Helper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.Game;

internal class MemoryShadow : IMemory
{
    // General game stuff
    public Shadow.GameVersion Version { get; }

    // Important addresses and offsets
    public IntPtr BaseEngineAddress { get; }


    // Cached addresses
    private IntPtr Address_Application { get; set; } = IntPtr.Zero;
    private IntPtr Address_ApplicationSequence { get; set; } = IntPtr.Zero;
    private IntPtr Address_GameMode { get; set; } = IntPtr.Zero;
    private IntPtr Address_GameModeExtension { get; set; } = IntPtr.Zero;
    private IntPtr Address_GameModeHsmExtension { get; set; } = IntPtr.Zero;


    public int Offset_Application { get; }
    public int Offset_GameMode { get; }
    public int Offset_GameModeExtension { get; }
    private int GameModeExtensionCount { get; set; }


    private MemStateTracker _stateTracker;
    public LazyWatcher<bool> Is_Loading { get; }


    public MemoryShadow(ProcessMemory process)
    {
        Version = process.MainModule.ModuleMemorySize switch
        {
            0x1CA2A000 => Shadow.GameVersion.v1_1_0_0,
            _ => Shadow.GameVersion.Unknown
        };

        BaseEngineAddress = process.Scan(new MemoryScanPattern(1, "E8 ???????? 4C 8B 40 28")
        {
            OnFound = (addr) =>
            {
                IntPtr tempAddr = addr + process.Read<int>(addr) + 0x4 + 0x3;
                tempAddr += process.Read<int>(tempAddr) + 0x4;
                return tempAddr;
            }
        });

        Offset_Application = 0x88;
        Offset_GameMode = 0x78;
        Offset_GameModeExtension = 0xB0;

        _stateTracker = new MemStateTracker();

        Is_Loading = new LazyWatcher<bool>(_stateTracker, false, (current, _) =>
        {
            if (RTTILookup(process, Address_GameMode, out string gm) && gm == "GameModeOpening@game@app@@")
                return false;

            if (GameModeExtensionCount == 0 || Address_GameModeHsmExtension == IntPtr.Zero)
                return true;

            Span<long> buf = stackalloc long[2];

            if (!process.ReadArray(Address_GameModeHsmExtension + 0x60, buf))
                return current;

            if (process.ReadPointer((IntPtr)buf[0] + 0x20, out IntPtr ptr)
                && process.ReadString(ptr, 128, out string detail)
                && (detail == "Build" || detail == "Quit"))
                return true;

            if (process.ReadPointer((IntPtr)buf[1] + 0x20, out ptr)
                && process.ReadString(ptr, 128, out detail)
                && detail == "TransitStage")
                return true;

            return false;
        });
    }

    internal override void Update(ProcessMemory process)
    {
        _stateTracker.Tick();

        // Reset the cached addresses
        Address_Application = default;
        Address_ApplicationSequence = default;
        Address_GameMode = default;
        Address_GameModeExtension = default;
        Address_GameModeHsmExtension = default;

        // And reset the cached values as well
        GameModeExtensionCount = default;

        if (!process.ReadPointer(BaseEngineAddress, out IntPtr ptr) || ptr == IntPtr.Zero)
            return;

        if (!process.ReadPointer(ptr + Offset_Application, out IntPtr ptr2))
            return;

        Address_Application = ptr2;

        if (!process.Read<byte>(ptr + Offset_Application + 0x8, out byte applicationSequenceCount) || applicationSequenceCount == 0 || applicationSequenceCount > 25)
            return;

        long[] rent = ArrayPool<long>.Shared.Rent(applicationSequenceCount);

        try
        {
            if (!process.ReadArray<long>(Address_Application, rent.AsSpan(0, applicationSequenceCount)))
                return;

            Address_ApplicationSequence = rent
                .Take(applicationSequenceCount)
                .Select(item =>
                {
                    if (!RTTILookup(process, (IntPtr)item, out string name) || name != "ApplicationSequenceExtension@game@app@@")
                        return IntPtr.Zero;

                    return (IntPtr)item;
                })
                .FirstOrDefault(item => item != IntPtr.Zero);

            if (Address_ApplicationSequence == IntPtr.Zero)
                return;
        }
        finally
        {
            ArrayPool<long>.Shared.Return(rent);
        }

        if (!process.ReadPointer(Address_ApplicationSequence + Offset_GameMode, out ptr))
            return;
        Address_GameMode = ptr;

        if (!process.ReadPointer(Address_GameMode + Offset_GameModeExtension, out ptr))
            return;
        Address_GameModeExtension = ptr;

        if (!process.Read<byte>(Address_GameMode + Offset_GameModeExtension + 0x8, out byte gameModeExtensionCount)
            || gameModeExtensionCount == 0 || gameModeExtensionCount > 128)
            return;
        GameModeExtensionCount = gameModeExtensionCount;

        rent = ArrayPool<long>.Shared.Rent(GameModeExtensionCount);
        try
        {
            if (!process.ReadArray(Address_GameModeExtension, rent.AsSpan(0, GameModeExtensionCount)))
                return;

            foreach (var item in rent.AsSpan(0, GameModeExtensionCount))
            {
                if (RTTILookup(process, (IntPtr)item, out string value))
                {
                    if (value == "GameModeHsmExtension@game@app@@")
                        Address_GameModeHsmExtension = (IntPtr)item;
                }

                if (Address_GameModeHsmExtension != IntPtr.Zero)
                    break;
            }
        }
        finally
        {
            ArrayPool<long>.Shared.Return(rent);
        }
    }

    internal override bool? IsLoading(Settings settings)
    {
        return settings.ShadowLoadless && Is_Loading.Current;
    }

    /// <summary>
    /// Recovers the RTTI name
    /// </summary>
    private bool RTTILookup(ProcessMemory process, IntPtr instanceAddress, out string value)
    {
        if (instanceAddress == IntPtr.Zero
            || !process.ReadPointer(instanceAddress, out IntPtr addr)
            || !process.ReadPointer(addr - 0x8, out addr)
            || !process.Read<int>(addr += 0xC, out int val))
        {
            value = string.Empty;
            return false;
        }

        return process.ReadString(process.MainModule.BaseAddress + val + 0x10 + 0x4, 128, out value);
    }

    internal override bool Start(Settings settings) => false;
    internal override bool Split(Settings settings) => false;
    internal override bool Reset(Settings settings) => false;
    internal override TimeSpan? GameTime(Settings settings) => null;
}
