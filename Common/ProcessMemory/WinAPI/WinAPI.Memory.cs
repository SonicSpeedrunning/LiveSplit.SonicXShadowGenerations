using System;
using System.Buffers;
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
    public unsafe static bool ReadProcessMemory<T>(IntPtr processHandle, IntPtr address, out T value) where T: unmanaged
    {
        fixed (void* valuePtr = &value)
        {
            Span<byte> valueBuffer = new(valuePtr, Marshal.SizeOf<T>());
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
    public static bool ReadProcessMemory<T>(IntPtr processHandle, IntPtr address, Span<T> buffer) where T: unmanaged
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
    public static bool ReadProcessMemory(IntPtr processHandle, IntPtr address, Span<byte> buffer)
    {
        if (processHandle == IntPtr.Zero)
            throw new InvalidOperationException("Invalid process handle.");

        int size = buffer.Length;

        if (size == 0)
            throw new ArgumentException("Buffer cannot be empty.");

        unsafe
        {
            fixed (byte* pBuf = buffer)
                //return ReadProcessMemory(processHandle, address, pBuf, size, out int bytesRead) && bytesRead == size;
                return NtReadVirtualMemory(processHandle, address, pBuf, size, out int bytesRead) == 0 && bytesRead == size;
        }

        // Import the ReadProcessMemory function from kernel32.dll.
        // It is used to read memory from the external process into a local buffer.
        //[DllImport("kernel32.dll")]
        //static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        [DllImport(Libs.Ntdll)]
        [SuppressUnmanagedCodeSecurity]
        static unsafe extern int NtReadVirtualMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer, int dwSize, out int lpNumberOfBytesRead);
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
    public static unsafe bool ReadString(IntPtr processHandle, IntPtr address, int maxLength, StringType stringType, out string result, string defaultValue = "")
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