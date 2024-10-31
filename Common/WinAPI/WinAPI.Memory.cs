using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Helper.Common.ProcessInterop.API.Definitions;

namespace Helper.Common.ProcessInterop.API;

internal static partial class WinAPI
{
    /// <summary>
    /// Reads a value of type T from an external process's memory at a given address.
    /// </summary>
    /// <typeparam name="T">The type of value to read (must be unmanaged).</typeparam>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">The memory address to read from in the external process.</param>
    /// <param name="value">The read value output.</param>
    /// <returns>True if the value is successfully read, false otherwise.</returns>
    internal unsafe static bool ReadProcessMemory<T>(IntPtr processHandle, IntPtr address, out T value) where T: unmanaged
    {
        fixed (void* valuePtr = &value)
        {
            Span<byte> valueBuffer = new(valuePtr, Unsafe.SizeOf<T>());
            return ReadProcessMemory(processHandle, address, valueBuffer);
        }
    }

    /// <summary>
    /// Reads memory from an external process into a provided buffer of type T.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer (must be unmanaged).</typeparam>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">The memory address to read from in the external process.</param>
    /// <param name="buffer">The buffer where the memory will be written.</param>
    /// <returns>True if the memory is successfully read, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the buffer is empty.</exception>
    internal static bool ReadProcessMemory<T>(IntPtr processHandle, IntPtr address, Span<T> buffer) where T: unmanaged
    {
        Span<byte> byteBuffer = MemoryMarshal.Cast<T, byte>(buffer);
        return ReadProcessMemory(processHandle, address, byteBuffer);
    }

    /// <summary>
    /// Reads memory from an external process into a byte buffer.
    /// </summary>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">The memory address to read from in the external process.</param>
    /// <param name="buffer">The buffer where the memory will be written.</param>
    /// <returns>True if the memory is successfully read, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the buffer is empty.</exception>
    internal static bool ReadProcessMemory(IntPtr processHandle, IntPtr address, Span<byte> buffer)
    {
        if (processHandle == IntPtr.Zero)
            throw new InvalidOperationException("Invalid process handle.");

        nint size = buffer.Length;

        if (size == 0)
            throw new ArgumentException("Buffer cannot be empty.");

        unsafe
        {
            fixed (byte* pBuf = buffer)
                return ReadProcessMemory(processHandle, address, pBuf, size, out nint bytesRead) != 0 && bytesRead == size;
        }

        // Import the ReadProcessMemory function from kernel32.dll.
        // It is used to read memory from the external process into a local buffer.
        [DllImport(Libs.Kernel32)]
        [SuppressUnmanagedCodeSecurity]
        static unsafe extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer, nint dwSize, out nint lpNumberOfBytesRead);
    }

    /// <summary>
    /// Reads a string from an external process's memory at the given address.
    /// </summary>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">Memory address where the string is stored in the external process.</param>
    /// <param name="maxLength">Maximum length of the string to read.</param>
    /// <param name="stringType">Specifies whether the string is ASCII, Unicode, or should be auto-detected.</param>
    /// <param name="result">The decoded string result, or the default string if reading fails.</param>
    /// <param name="defaultValue">Optional default string to return if the read fails or the length is zero.</param>
    /// <returns>Returns true if the string is successfully read and decoded, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when maxLength is less than 0.</exception>
    [SkipLocalsInit]
    internal static unsafe bool ReadString(IntPtr processHandle, IntPtr address, int maxLength, StringType stringType, out string result, string defaultValue = "")
    {
        if (maxLength < 0)
            throw new ArgumentException("String length cannot be lower than 0.");

        if (maxLength == 0)
        {
            result = defaultValue;
            return true;
        }

        const int StackAllocThreshold = 256;

        // We don't know the string type beforehand, so we allocate for UTF-16 (2 bytes per character)
        byte[]? rentedBuffer = null;
        Span<byte> stringBuffer = maxLength <= StackAllocThreshold
            ? stackalloc byte[maxLength * 2]
            : (rentedBuffer = ArrayPool<byte>.Shared.Rent(maxLength * 2));

        try
        {
            if (!ReadProcessMemory(processHandle, address, stringBuffer))
            {
                result = defaultValue;
                return false;
            }

            result = DecodeString(stringBuffer, stringType);
            return true;
        }
        finally
        {
            if (rentedBuffer is not null)
                ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }

    /// <summary>
    /// Decodes a byte buffer into a string based on the specified string type (ASCII or Unicode).
    /// </summary>
    /// <param name="buffer">The buffer containing the raw bytes of the string.</param>
    /// <param name="stringType">The encoding type to use for decoding (ASCII, Unicode, or AutoDetect).</param>
    /// <returns>The decoded string result.</returns>
    private static string DecodeString(ReadOnlySpan<byte> buffer, StringType stringType)
    {
        if (stringType == StringType.AutoDetect)
            stringType = DetectStringType(buffer);

        return stringType switch
        {
            StringType.Unicode => DecodeUnicodeString(buffer),
            StringType.ASCII => DecodeASCIIString(buffer),
            _ => throw new NotImplementedException("Invalid string type.")
        };
    }

    /// <summary>
    /// Attempts to detect whether a string is Unicode or ASCII based on the byte pattern.
    /// </summary>
    /// <param name="buffer">The buffer containing the raw bytes of the string.</param>
    /// <returns>Returns the detected string type (Unicode or ASCII).</returns>
    private static StringType DetectStringType(ReadOnlySpan<byte> buffer)
    {
        return buffer is [> 0, 0, > 0, 0, ..] ? StringType.Unicode : StringType.ASCII;
    }

    /// <summary>
    /// Decodes a byte buffer into a Unicode string (UTF-16).
    /// </summary>
    /// <param name="stringBuffer">The buffer containing the raw bytes of the Unicode string.</param>
    /// <returns>Returns the decoded Unicode string result.</returns>
    private static string DecodeUnicodeString(ReadOnlySpan<byte> stringBuffer)
    {
        ReadOnlySpan<char> charSpan = MemoryMarshal.Cast<byte, char>(stringBuffer);
        int nullTerminatorIndex = charSpan.IndexOf('\0');

        if (nullTerminatorIndex == -1)
            nullTerminatorIndex = charSpan.Length;

        unsafe
        {
            fixed (char* charPtr = charSpan)
                return new string(charPtr, 0, nullTerminatorIndex);
        }
    }

    /// <summary>
    /// Decodes a byte buffer into an ASCII string.
    /// </summary>
    /// <param name="stringBuffer">The buffer containing the raw bytes of the ASCII string.</param>
    /// <returns>Returns the decoded ASCII string result.</returns>
    private static string DecodeASCIIString(ReadOnlySpan<byte> stringBuffer)
    {
        int nullTerminatorIndex = stringBuffer.IndexOf((byte)0);

        if (nullTerminatorIndex == -1)
            nullTerminatorIndex = stringBuffer.Length;

        unsafe
        {
            fixed (byte* bytePtr = stringBuffer)
                return new string((sbyte*)bytePtr, 0, nullTerminatorIndex);
        }
    }

    /// <summary>
    /// Writes a value of type T into an external process's memory at a given address.
    /// </summary>
    /// <typeparam name="T">The type of value to write (must be unmanaged).</typeparam>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">The memory address to write to in the external process.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>True if the value is successfully written, false otherwise.</returns>
    internal static unsafe bool WriteProcessMemory<T>(IntPtr processHandle, IntPtr address, T value) where T : unmanaged
    {
        ReadOnlySpan<byte> valueBuffer = new ReadOnlySpan<byte>(&value, Unsafe.SizeOf<T>());
        return WriteProcessMemory(processHandle, address, valueBuffer);
    }

    /// <summary>
    /// Writes a buffer of type T into an external process's memory at a given address.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer (must be unmanaged).</typeparam>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">The memory address to write to in the external process.</param>
    /// <param name="buffer">The buffer to write from.</param>
    /// <returns>True if the buffer is successfully written, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the buffer is empty.</exception>
    internal static bool WriteProcessMemory<T>(IntPtr processHandle, IntPtr address, ReadOnlySpan<T> buffer) where T : unmanaged
    {
        ReadOnlySpan<byte> byteBuffer = MemoryMarshal.Cast<T, byte>(buffer);
        return WriteProcessMemory(processHandle, address, byteBuffer);
    }

    /// <summary>
    /// Writes a byte buffer into an external process's memory at a given address.
    /// </summary>
    /// <param name="processHandle">Handle to the external process.</param>
    /// <param name="address">The memory address to write to in the external process.</param>
    /// <param name="buffer">The buffer containing bytes to write.</param>
    /// <returns>True if the buffer is successfully written, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the buffer is empty.</exception>
    internal static unsafe bool WriteProcessMemory(IntPtr processHandle, IntPtr address, ReadOnlySpan<byte> buffer)
    {
        if (processHandle == IntPtr.Zero)
            throw new InvalidOperationException("Invalid process handle.");

        int size = buffer.Length;
        if (size == 0)
            throw new ArgumentException("Tried to write an empty data buffer.");

        fixed (byte* pBuf = buffer)
            return WriteProcessMemory(processHandle, address, pBuf, size, out nint bytesWritten) != 0 && bytesWritten == size;

        // Import the WriteProcessMemory function from kernel32.dll.
        // It is used to write memory to the external process from a local buffer.
        [DllImport(Libs.Kernel32)]
        [SuppressUnmanagedCodeSecurity]
        static unsafe extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer, nint dwSize, out nint lpNumberOfBytesWritten);
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
    public static bool WriteString(IntPtr processHandle, IntPtr address, string value, StringType stringType)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("String cannot be empty.");

        ReadOnlySpan<byte> stringBytes = stringType switch
        {
            StringType.Unicode => MemoryMarshal.AsBytes(value.AsSpan()),
            StringType.ASCII => System.Text.Encoding.ASCII.GetBytes(value),
            _ => throw new NotImplementedException($"Unsupported string type. Note that {StringType.AutoDetect} is not supported for write uperations.")
        };

        return WriteProcessMemory(processHandle, address, stringBytes);
    }
}

/// <summary>
/// Enum representing the types of strings that can be read.
/// </summary>
public enum StringType
{
    /// <summary>
    /// Represents an ASCII encoded string (1 byte per character).
    /// </summary>
    ASCII,

    /// <summary>
    /// Represents a Unicode (UTF-16) encoded string (2 bytes per character).
    /// </summary>
    Unicode,

    /// <summary>
    /// Automatically detects whether the string is ASCII or Unicode.
    /// </summary>
    AutoDetect
}