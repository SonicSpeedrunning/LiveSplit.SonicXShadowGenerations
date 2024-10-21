using System;
using System.Collections.Generic;
using System.Linq;

namespace Helper.Common.ProcessInterop;

public partial class ProcessMemory
{
    /// <summary>
    /// Scans the main module of the process for the given pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>The first matching address or IntPtr.Zero if not found.</returns>
    public IntPtr Scan(ScanPattern pattern)
    {
        return Scan(pattern, MainModule);
    }

    /// <summary>
    /// Scans the specified module of the process for the given pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for.</param>
    /// <param name="processModule">The module to scan.</param>
    /// <returns>The first matching address or IntPtr.Zero if not found.</returns>
    public IntPtr Scan(ScanPattern pattern, ProcessModule processModule)
    {
        return Scan(pattern, processModule.BaseAddress, processModule.ModuleMemorySize);
    }

    /// <summary>
    /// Scans a specified memory range for the given pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for.</param>
    /// <param name="baseAddress">The starting address of the memory range.</param>
    /// <param name="size">The size of the memory range.</param>
    /// <returns>The first matching address or IntPtr.Zero if not found.</returns>
    public IntPtr Scan(ScanPattern pattern, IntPtr baseAddress, int size)
    {
        return ScanAll(pattern, baseAddress, size).FirstOrDefault();
    }

    /// <summary>
    /// Scans for all occurrences of the pattern in increments of 0x1000 (page size) for memory that is allocated, reducing unallocated memory overhead.
    /// </summary>
    /// <param name="pattern">The pattern to search for.</param>
    /// <param name="baseAddress">The base memory address to start scanning from.</param>
    /// <param name="size">The size of the memory region to scan.</param>
    /// <returns>An enumerable collection of memory addresses where the pattern was found.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the size is less than or equal to zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the pattern length is larger than the specified memory size.</exception>
    public IEnumerable<IntPtr> ScanAll(ScanPattern pattern, IntPtr baseAddress, int size)
    {
        return SignatureScanner.ScanAll(pHandle, pattern, baseAddress, size);
    }
}