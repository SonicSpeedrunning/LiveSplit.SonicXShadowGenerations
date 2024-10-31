using System;
using Helper.Common.ProcessInterop.API;

namespace Helper.Common.ProcessInterop;

public partial class ProcessMemory : IDisposable
{
    /// <summary>
    /// Writes a pointer (IntPtr) to the specified memory address.
    /// </summary>
    /// <param name="address">The memory address to write to.</param>
    /// <param name="value">The pointer value to write.</param>
    /// <returns>True if the write was successful, otherwise false.</returns>
    public bool WritePointer(IntPtr address, IntPtr value)
    {
        return Write(address, value);
    }

    /// <summary>
    /// Writes a value of type <typeparamref name="T"/> to the specified memory address.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of the value to write (e.g., int, float, IntPtr).</typeparam>
    /// <param name="address">The memory address to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>True if the write was successful, otherwise false.</returns>
    public bool Write<T>(IntPtr address, T value) where T : unmanaged
    {
        if (!IsNativePtr<T>())
            return WinAPI.WriteProcessMemory(pHandle, address, value);

        // Handle pointer writing in an unsafe block for unmanaged memory access.
        unsafe
        {
            ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(&value, PointerSize);
            return WriteArray<byte>(address, bytes);
        }
    }

    /// <summary>
    /// Writes an array of type <typeparamref name="T"/> to the specified memory address.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of elements in the array.</typeparam>
    /// <param name="array">The array of values to write.</param>
    /// <param name="address">The memory address to write to.</param>
    /// <param name="offsets">Optional offsets for pointer dereferencing.</param>
    /// <returns>True if the array was written successfully, otherwise false.</returns>
    public bool WriteArray<T>(IntPtr address, T[] array, params int[] offsets) where T : unmanaged
    {
        if (DerefOffsets(address, out IntPtr endAddress, offsets))
            return WriteArray(endAddress, array);
        return false;
    }

    /// <summary>
    /// Writes an array of type <typeparamref name="T"/> to the specified memory address.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of elements in the array.</typeparam>
    /// <param name="address">The memory address to write to.</param>
    /// <param name="array">The span that holds the values to write.</param>
    /// <returns>True if the array was written successfully, otherwise false.</returns>
    public bool WriteArray<T>(IntPtr address, ReadOnlySpan<T> array) where T : unmanaged
    {
        if (_disposed)
            throw new InvalidOperationException($"Cannot invoke WriteProcessMemory method on a disposed {nameof(ProcessMemory)} instance");

        return WinAPI.WriteProcessMemory<T>(pHandle, address, array);
    }

    /// <summary>
    /// Writes a string into an external process's memory at a given address.
    /// </summary>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">Memory address where the string will be written in the external process.</param>
    /// <param name="value">The string to write.</param>
    /// <param name="stringType">Specifies whether the string is ASCII or Unicode.</param>
    /// <returns>Returns true if the string is successfully written, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is empty.</exception>
    public bool WriteString(IntPtr address, string value, StringType stringType)
    {
        return WinAPI.WriteString(pHandle, address, value, stringType);
    }
}
