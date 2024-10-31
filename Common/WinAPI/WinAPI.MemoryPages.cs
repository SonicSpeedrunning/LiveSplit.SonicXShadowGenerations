using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Helper.Common.ProcessInterop.API.Definitions;

namespace Helper.Common.ProcessInterop.API;

internal static partial class WinAPI
{
    /// <summary>
    /// Enumerates memory pages of the target process.
    /// </summary>
    /// <param name="pHandle">Handle to the target process.</param>
    /// <param name="includeMapped">Whether to include mapped memory regions in the enumeration.</param>
    /// <returns>An enumerable collection of <see cref="MemoryPage"/> objects representing memory pages.</returns>
    [SkipLocalsInit]
    internal static IEnumerable<MemoryPage> MemoryPages(IntPtr pHandle, bool includeMapped = false)
    {
        // If the process handle is invalid, throw an exception.
        if (pHandle == IntPtr.Zero)
            throw new InvalidOperationException("Invalid process handle.");

        // Minimum memory address to start scanning (typically the first addressable memory region).
        nint min = 0x10000;

        // Maximum memory address to scan up to.
        // For 64-bit processes, this is 0x00007FFFFFFEFFFF; for 32-bit, it's 0x7FFEFFFF.
        nint max = (nint)(Environment.Is64BitOperatingSystem && Is64Bit(pHandle) ? 0x00007FFFFFFEFFFF : 0x7FFEFFFF);

        nint address = min;

        // Size of the MemoryBasicInformation structure (used in VirtualQueryEx).
        int memInfoSize = Unsafe.SizeOf<MemoryBasicInformation>();

        while (address < max && VirtualQueryEx(pHandle, address, out MemoryBasicInformation memInfo, memInfoSize) != 0)
        {
            if (memInfo.RegionSize == 0)
                break;

            // Move to the next memory region after the current one.
            address = memInfo.BaseAddress + memInfo.RegionSize;

            // Skip memory regions that are not committed (MEM_COMMIT means the region is allocated and ready for use).
            if ((memInfo.State & MemoryState.MEM_COMMIT) == 0)
                continue;

            // Skip guarded pages to avoid exceptions on access.
            if ((memInfo.Protect & MemoryProtection.PAGE_GUARD) != 0)
                continue;

            // Optionally skip memory-mapped regions if includeMapped is false.
            if (!includeMapped && (memInfo.Type & MemoryType.MEM_MAPPED) != 0)
                continue;

            yield return new MemoryPage(pHandle, memInfo.BaseAddress, memInfo.AllocationBase, memInfo.AllocationProtect, memInfo.RegionSize, memInfo.State, memInfo.Protect, memInfo.Type);
        }

        [DllImport(Libs.Kernel32)]
        [SuppressUnmanagedCodeSecurity]
        static extern nint VirtualQueryEx(IntPtr hProcess, nint lpAddress, out MemoryBasicInformation lpBuffer, nint dwLength);
    }
}

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MemoryBasicInformation
{
    public readonly IntPtr BaseAddress;
    public readonly IntPtr AllocationBase;
    public readonly MemoryProtection AllocationProtect;
    public readonly nint RegionSize; // Using nint for RegionSize
    public readonly MemoryState State;
    public readonly MemoryProtection Protect;
    public readonly MemoryType Type;
}