using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helper.Common.ProcessInterop.API;

namespace Helper.Common.ProcessInterop;

/// <summary>
/// Represents a collection memory pages in an external process. 
/// Provides an enumerable interface to iterate over the memory regions of a process.
/// </summary>
public readonly struct MemoryPagesSnapshot : IEnumerable<MemoryPage>
{
    private readonly IntPtr pHandle;
    private readonly bool includeMapped;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryPagesSnapshot"/> struct.
    /// </summary>
    /// <param name="processHandle">Handle to the target process.</param>
    /// <param name="includeMapped">Whether to include mapped memory regions (e.g., file-mapped memory) in the snapshot.</param>
    public MemoryPagesSnapshot(IntPtr processHandle, bool includeMapped = false)
    {
        this.pHandle = processHandle;
        this.includeMapped = includeMapped;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the memory pages.
    /// </summary>
    /// <returns>An IEnumerator object to iterate over memory pages.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return WinAPI.MemoryPages(pHandle, includeMapped).GetEnumerator();
    }

    /// <summary>
    /// Returns a strongly-typed enumerator that iterates through the memory pages.
    /// </summary>
    /// <returns>An IEnumerator of MemoryPage to iterate over memory pages.</returns>
    IEnumerator<MemoryPage> IEnumerable<MemoryPage>.GetEnumerator()
    {
        return WinAPI.MemoryPages(pHandle, includeMapped).GetEnumerator();
    }
}

/// <summary>
/// Represents a single memory page (or region) in the process memory.
/// Contains metadata about the memory region and allows scanning it for patterns.
/// </summary>
[SkipLocalsInit]
public record MemoryPage
{
    private readonly IntPtr pHandle;

    /// <summary>
    /// The base address of the memory page.
    /// </summary>
    public readonly IntPtr BaseAddress;

    /// <summary>
    /// The allocation base address (start address) where the memory page was allocated.
    /// </summary>
    public readonly IntPtr AllocationBase;

    /// <summary>
    /// The memory protection level applied when the memory was allocated.
    /// </summary>
    public readonly MemoryProtection AllocationProtect;

    /// <summary>
    /// The size of the memory region.
    /// </summary>
    public readonly nint RegionSize; // Using nint for RegionSize

    /// <summary>
    /// The current state of the memory (committed, free, reserved).
    /// </summary>
    public readonly MemoryState State;

    /// <summary>
    /// The current protection level of the memory (e.g., read, write).
    /// </summary>
    public readonly MemoryProtection Protect;

    /// <summary>
    /// The type of the memory region (private, mapped, or image).
    /// </summary>
    public readonly MemoryType Type;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryPage"/> record.
    /// </summary>
    /// <param name="pHandle">Handle to the target process.</param>
    /// <param name="baseAddress">The base address of the memory page.</param>
    /// <param name="allocationBase">The base address where the memory was allocated.</param>
    /// <param name="allocationProtect">The protection level when the memory was allocated.</param>
    /// <param name="regionSize">The size of the memory region.</param>
    /// <param name="state">The state of the memory (committed, reserved, free).</param>
    /// <param name="protect">The current protection of the memory region.</param>
    /// <param name="type">The type of the memory (private, mapped, or image).</param>
    public MemoryPage(IntPtr pHandle,
        IntPtr baseAddress,
        IntPtr allocationBase,
        MemoryProtection allocationProtect,
        nint regionSize,
        MemoryState state,
        MemoryProtection protect,
        MemoryType type)
    {
        this.pHandle = pHandle;
        BaseAddress = baseAddress;
        AllocationBase = allocationBase;
        AllocationProtect = allocationProtect;
        RegionSize = regionSize;
        State = state;
        Protect = protect; Type = type;
        Type = type;
    }

    /// <summary>
    /// Scans the memory page for the first occurrence of the given pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for in the memory page.</param>
    /// <returns>The address of the first occurrence of the pattern, or <see cref="IntPtr.Zero"/> if not found.</returns>
    public IntPtr Scan(ScanPattern pattern)
    {
        return ScanAll(pattern).FirstOrDefault();
    }

    /// <summary>
    /// Scans the memory page for all occurrences of the given pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for in the memory page.</param>
    /// <returns>An enumerable collection of addresses where the pattern is found.</returns>
    public IEnumerable<IntPtr> ScanAll(ScanPattern pattern)
    {
        return SignatureScanner.ScanAll(pHandle, pattern, BaseAddress, (int)RegionSize);
    }
}

/// <summary>
/// Memory protection options as defined by the Windows API.
/// </summary>
[Flags]
public enum MemoryProtection : uint
{
    PAGE_NOACCESS = 0x01,           // Cannot be accessed.
    PAGE_READONLY = 0x02,           // Can be read, but not written or executed.
    PAGE_READWRITE = 0x04,          // Can be read and written, but not executed.
    PAGE_WRITECOPY = 0x08,          // Copy-on-write. Read access allowed, but writes result in a private copy of the page.
    PAGE_EXECUTE = 0x10,            // Can be executed, but not read or written.
    PAGE_EXECUTE_READ = 0x20,       // Can be executed and read, but not written.
    PAGE_EXECUTE_READWRITE = 0x40,  // Can be executed, read, and written.
    PAGE_EXECUTE_WRITECOPY = 0x80,  // Can be executed and read, but writes are copy-on-write.
    PAGE_GUARD = 0x100,             // Pages in this region are guarded, raising an exception on access.
    PAGE_NOCACHE = 0x200,           // Pages in this region are not cached.
    PAGE_WRITECOMBINE = 0x400,      // Pages in this region allow write combining, for devices with a write buffer.
}

/// <summary>
/// Represents the current state of memory pages.
/// </summary>
[Flags]
public enum MemoryState : uint
{
    MEM_COMMIT = 0x1000,    // The memory is committed and usable.
    MEM_FREE = 0x10000,     // The memory is free and not allocated.
    MEM_RESERVE = 0x2000    // The memory is reserved but not yet committed.
}

/// <summary>
/// Represents the type of memory pages.
/// </summary>
[Flags]
public enum MemoryType : uint
{
    MEM_PRIVATE = 0x20000,  // The memory is private and not shared with other processes.
    MEM_MAPPED = 0x40000,   // The memory is mapped, typically associated with a file mapping
    MEM_IMAGE = 0x1000000   // The memory is an image, typically loaded from an executable or DLL.
}