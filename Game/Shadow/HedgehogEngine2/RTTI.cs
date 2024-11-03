using System;
using System.Collections.Generic;
using System.Linq;
using Helper.Common.ProcessInterop;
using Helper.Common.ProcessInterop.API;

namespace LiveSplit.SonicXShadowGenerations.GameEngine;

/// <summary>
/// A class used to perform RTTI (Run-Time Type Information) lookups
/// for game objects in a specific process's memory space. This class 
/// identifies object types by analyzing RTTI descriptors in memory 
/// based on the object's vtable address.
/// </summary>
/// <remarks>
/// Initializes an instance of the RTTI class with a given process.
/// </remarks>
/// <param name="process">The process memory interface for reading data.</param>
internal class RTTI(ProcessMemory process)
{
    /// <summary>
    /// Represents the process memory interface used for reading data 
    /// from a target process.
    /// </summary>
    private readonly ProcessMemory process = process;

    /// <summary>
    /// Base address of the main module in the target process.
    /// </summary>
    private readonly nint mainModuleBase = process.MainModule.BaseAddress;

    /// <summary>
    /// Size of the main module in the target process.
    /// </summary>
    private readonly int mainModuleSize = process.MainModule.ModuleMemorySize;

    /// <summary>
    /// Determines if the target process is 64-bit or 32-bit.
    /// </summary>
    private readonly bool is64Bit = process.Is64Bit;

    /// <summary>
    /// A cache for storing previously identified RTTI type names, 
    /// mapped by memory offset, to reduce redundant lookups.
    /// </summary>
    private readonly Dictionary<IntPtr, string> cache = [];

    /// <summary>
    /// Looks up the RTTI (Run-Time Type Information) name for an object
    /// located at the given instance address.
    /// </summary>
    /// <param name="instanceAddress">The memory address of the object instance.</param>
    /// <param name="value">Outputs the RTTI type name if found, otherwise an empty string.</param>
    /// <returns>True if the lookup succeeds; otherwise, false.</returns>
    public bool Lookup(IntPtr instanceAddress, out string value)
    {
        if (instanceAddress == IntPtr.Zero || !process.ReadPointer(instanceAddress, out IntPtr vtable))
        {
            value = string.Empty;
            return false;
        }

        // Ensure the vtable address is within bounds and non-zero
        if (vtable == IntPtr.Zero || vtable < mainModuleBase || vtable > mainModuleBase + mainModuleSize)
        {
            value = string.Empty;
            return false;
        }

        // Check if the type name for this offset is already cached
        if (cache.TryGetValue(vtable, out value))
            return true;

        // Adjust pointer calculations and offsets based on bitness
        IntPtr rttiDescriptorAddress = vtable - (is64Bit ? 0x8 : 0x4);

        if (!process.ReadPointer(rttiDescriptorAddress, out IntPtr addr))
            return false;

        // Offset to the RTTI type name location depends on process architecture
        int typeNameOffset = is64Bit ? 0xC : 0x4;
        if (!process.Read(addr + typeNameOffset, out int val))
            return false;

        // Calculate the final address for reading the RTTI type name string
        IntPtr typeNameAddress = mainModuleBase + val + 0x4 + (is64Bit ? 0x10 : 0x8);
        if (!process.ReadString(typeNameAddress, 128, StringType.ASCII, out string finalValue))
            return false;

        // Parse the type name and cache it
        value = finalValue.Contains('@') ? finalValue.Split('@')[0] : finalValue;
        cache[vtable] = value;
        return true;
    }
}
