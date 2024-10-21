using System;
using System.Runtime.CompilerServices;
using Helper.Common.ProcessInterop.API;

namespace Helper.Common.ProcessInterop;

public partial class ProcessMemory : IDisposable
{
    /// <summary>
    /// Reads a value of type <typeparamref name="T"/> from a given memory address with optional pointer dereferencing using offsets.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of the value to read (e.g., int, float, IntPtr).</typeparam>
    /// <param name="address">The base memory address to read from.</param>
    /// <param name="offsets">Optional offsets for pointer dereferencing (to navigate through pointer chains).</param>
    /// <returns>The value of type <typeparamref name="T"/> if read successfully, otherwise the default value of <typeparamref name="T"/>.</returns>
    public T Read<T>(IntPtr address, params int[] offsets) where T : unmanaged
    {
        if (!DerefOffsets(address, out IntPtr endAddress, offsets))
            return default;

        return Read<T>(endAddress, out T value)
            ? value
            : default;
    }

    /// <summary>
    /// Reads a pointer (IntPtr) from the specified memory address.
    /// </summary>
    /// <param name="address">The memory address to read from.</param>
    /// <returns>The pointer read from the address, or IntPtr.Zero if the read fails.</returns>
    public IntPtr ReadPointer(IntPtr address)
    {
        return Read<IntPtr>(address);
    }

    /// <summary>
    /// Reads a pointer (IntPtr) from the specified memory address and outputs it through the <paramref name="value"/> parameter.
    /// </summary>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="value">Outputs the read pointer (IntPtr) if successful.</param>
    /// <returns>True if the read was successful, otherwise false.</returns>
    public bool ReadPointer(IntPtr address, out IntPtr value)
    {
        return Read(address, out value);
    }

    /// <summary>
    /// Reads a value of type <typeparamref name="T"/> from the specified memory address and outputs it through the <paramref name="value"/> parameter.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of the value to read (e.g., int, float, IntPtr).</typeparam>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="value">Outputs the value of type <typeparamref name="T"/> if the read is successful.</param>
    /// <returns>True if the read was successful, otherwise false.</returns>
    public bool Read<T>(IntPtr address, out T value) where T : unmanaged
    {
        if (!IsNativePtr<T>())
            return WinAPI.ReadProcessMemory(pHandle, address, out value);

        value = default;

        // Handle pointer reading in an unsafe block for unmanaged memory access.
        unsafe
        {
            fixed (void* ptr = &value)
            {
                Span<byte> bytes = new Span<byte>(ptr, PointerSize);
                return ReadArray(pHandle, bytes);
            }
        }
    }

    /// <summary>
    /// Reads an array of type <typeparamref name="T"/> from the specified memory address.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of elements in the array.</typeparam>
    /// <param name="arrayLength">The number of elements to read.</param>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="offsets">Optional offsets for pointer dereferencing.</param>
    /// <returns>The array of type <typeparamref name="T"/> if read successfully, otherwise an empty array.</returns>
    public T[] ReadArray<T>(int arrayLength, IntPtr address, params int[] offsets) where T : unmanaged
    {
        T[] value = new T[arrayLength];

        if (DerefOffsets(address, out IntPtr endAddress, offsets))
            ReadArray<T>(endAddress, value);

        return value;
    }

    /// <summary>
    /// Reads an array of type <typeparamref name="T"/> from the specified memory address and outputs it.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of elements in the array.</typeparam>
    /// <param name="arrayLength">The number of elements to read.</param>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="array">Outputs the read array.</param>
    /// <returns>True if the array was read successfully, otherwise false.</returns>
    public bool ReadArray<T>(int arrayLength, IntPtr address, out T[] array) where T : unmanaged
    {
        array = new T[arrayLength];
        return ReadArray<T>(address, array);
    }

    /// <summary>
    /// Reads an array of type <typeparamref name="T"/> from the specified memory address into the given <see cref="Span{T}"/>.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of elements in the array.</typeparam>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="array">The span that will hold the read values.</param>
    /// <returns>True if the array was read successfully, otherwise false.</returns>
    public bool ReadArray<T>(IntPtr address, Span<T> array) where T : unmanaged
    {
        if (_disposed)
            throw new InvalidOperationException($"Cannot invoke ReadProcessMemory method on a disposed {nameof(ProcessMemory)} instance");

        return WinAPI.ReadProcessMemory<T>(pHandle, address, array);
    }

    /// <summary>
    /// Reads a string from the specified memory address with optional pointer dereferencing.
    /// </summary>
    /// <param name="maxLength">The maximum length of the string to read.</param>
    /// <param name="stringType">The type of string encoding (e.g., ANSI, UTF-16).</param>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="offsets">Optional offsets for pointer dereferencing.</param>
    /// <returns>The string if read successfully, otherwise an empty string.</returns>
    public string ReadString(int maxLength, StringType stringType, IntPtr address, params int[] offsets)
    {
        return DerefOffsets(address, out IntPtr endAddress, offsets)
            ? ReadString(endAddress, maxLength, stringType)
            : string.Empty;
    }

    /// <summary>
    /// Reads a string from the specified memory address with optional pointer dereferencing, auto-detecting the string encoding.
    /// </summary>
    /// <param name="maxLength">The maximum length of the string to read.</param>
    /// <param name="address">The memory address to read from.</param>
    /// <param name="offsets">Optional offsets for pointer dereferencing.</param>
    /// <returns>The string if read successfully, otherwise an empty string.</returns>
    public string ReadString(int maxLength, IntPtr address, params int[] offsets)
    {
        return DerefOffsets(address, out IntPtr endAddress, offsets)
            ? ReadString(endAddress, maxLength)
            : string.Empty;
    }

    /// <summary>
    /// Reads a string from the specified memory address with a maximum length, auto-detecting the string encoding.
    /// </summary>
    /// <param name="address">The memory address to read the string from.</param>
    /// <param name="maxLength">The maximum length of the string to read.</param>
    /// <param name="defaultValue">The default value to return if the read fails.</param>
    /// <returns>The string if read successfully, otherwise the <paramref name="defaultValue"/>.</returns>
    public string ReadString(IntPtr address, int maxLength, string defaultValue = "")
    {
        if (_disposed)
            throw new InvalidOperationException($"Cannot invoke ReadProcessMemory method on a disposed {nameof(ProcessMemory)} instance");

        return WinAPI.ReadString(pHandle, address, maxLength, StringType.AutoDetect, out string result, defaultValue)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Reads a string from the specified memory address and outputs it, auto-detecting the string encoding.
    /// </summary>
    /// <param name="address">The memory address to read the string from.</param>
    /// <param name="maxLength">The maximum length of the string to read.</param>
    /// <param name="value">Outputs the read string if successful.</param>
    /// <param name="defaultValue">The default value to return if the read fails.</param>
    /// <returns>True if the string was read successfully, otherwise false.</returns>
    public bool ReadString(IntPtr address, int maxLength, out string value, string defaultValue = "")
    {
        if (_disposed)
            throw new InvalidOperationException($"Cannot invoke ReadProcessMemory method on a disposed {nameof(ProcessMemory)} instance");

        return WinAPI.ReadString(pHandle, address, maxLength, StringType.AutoDetect, out value, defaultValue);
    }

    /// <summary>
    /// Reads a string from the specified memory address with a maximum length, using the specified string encoding type.
    /// </summary>
    /// <param name="address">The memory address to read the string from.</param>
    /// <param name="maxLength">The maximum length of the string to read.</param>
    /// <param name="stringType">The type of string encoding (e.g., ANSI, UTF-16).</param>
    /// <param name="defaultValue">The default value to return if the read fails.</param>
    /// <returns>The string if read successfully, otherwise the <paramref name="defaultValue"/>.</returns>
    public string ReadString(IntPtr address, int maxLength, StringType stringType, string defaultValue = "")
    {
        if (_disposed)
            throw new InvalidOperationException($"Cannot invoke ReadProcessMemory method on a disposed {nameof(ProcessMemory)} instance");

        return WinAPI.ReadString(pHandle, address, maxLength, stringType, out string result, defaultValue)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Reads a string from the specified memory address with a maximum length, using the specified string encoding type, and outputs it.
    /// </summary>
    /// <param name="address">The memory address to read the string from.</param>
    /// <param name="maxLength">The maximum length of the string to read.</param>
    /// <param name="stringType">The type of string encoding (e.g., ANSI, UTF-16).</param>
    /// <param name="result">Outputs the read string if successful.</param>
    /// <param name="defaultValue">The default value to return if the read fails.</param>
    /// <returns>True if the string was read successfully, otherwise false.</returns>
    public bool ReadString(IntPtr address, int maxLength, StringType stringType, out string result, string defaultValue = "")
    {
        if (_disposed)
            throw new InvalidOperationException($"Cannot invoke ReadProcessMemory method on a disposed {nameof(ProcessMemory)} instance");

        return WinAPI.ReadString(pHandle, address, maxLength, stringType, out result, defaultValue);
    }

    /// <summary>
    /// Determines if the type <typeparamref name="T"/> is a native pointer type (e.g., IntPtr or UIntPtr).
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns>True if the type is a native pointer type, otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsNativePtr<T>()
    {
        Type type = typeof(T);
        return type == typeof(IntPtr) || type == typeof(UIntPtr);
    }

    /// <summary>
    /// Dereferences a series of pointer offsets starting from a base memory address.
    /// </summary>
    /// <param name="address">The base memory address to start from.</param>
    /// <param name="offsets">An array of integer offsets to dereference through.</param>
    /// <returns>The final memory address after all dereferencing if successful, otherwise <see cref="IntPtr.Zero"/>.</returns>
    public IntPtr DerefOffsets(IntPtr address, params int[] offsets)
    {
        return DerefOffsets(address, out IntPtr endAddress, offsets)
            ? endAddress
            : default;
    }

    /// <summary>
    /// Dereferences through a series of pointer offsets starting from a base address.
    /// </summary>
    /// <param name="baseAddress">The starting memory address for pointer dereferencing.</param>
    /// <param name="finalAddress">Outputs the final address after dereferencing through all offsets.</param>
    /// <param name="offsets">An array of integer offsets to traverse pointer chains.</param>
    /// <returns>True if the dereferencing was successful, otherwise false.</returns>
    public bool DerefOffsets(IntPtr address, out IntPtr value, params int[] offsets)
    {
        value = address;
        foreach (int offset in offsets)
        {
            if (!ReadPointer(value, out value) || value == IntPtr.Zero)
                return false;
            value += offset;
        }
        return true;
    }
}
