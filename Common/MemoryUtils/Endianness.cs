using System;
using System.Linq;

namespace Helper.Common.MemoryUtils;

/// <summary>
/// The <see cref="Endian"/> class provides methods for switching the byte order (endianness) of unmanaged types.
/// </summary>
public static class Endian
{
    /// <summary>
    /// Converts the byte order (endianness) of an unmanaged type based on the desired endianness.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to convert.</typeparam>
    /// <param name="value">The value whose byte order will be converted.</param>
    /// <param name="endian">The desired endianness (either <see cref="Endianness.Little"/> or <see cref="Endianness.Big"/>).</param>
    /// <returns>The value with its byte order converted to the specified endianness.</returns>
    public static T FromEndian<T>(this T value, Endianness endian) where T : unmanaged
    {
        bool isLittleEndian = BitConverter.IsLittleEndian;

        // If the system is little-endian and desired endianness is also little, return the value as-is
        if (isLittleEndian && endian == Endianness.Little)
            return value;

        // If the system is big-endian and the desired endianness is also big-endian, return the value as-is.
        if (!isLittleEndian && endian == Endianness.Big)
            return value;

        // Otherwise, swap the byte order.
        return SwapEndianness(value);
    }

    /// <summary>
    /// Swaps the byte order (endianness) of an unmanaged type.
    /// </summary>
    /// <typeparam name="T">The unmanaged type whose byte order will be swapped.</typeparam>
    /// <param name="value">The value whose byte order will be swapped.</param>
    /// <returns>The value with its byte order swapped.</returns>
    public unsafe static T SwapEndianness<T>(this T value) where T : unmanaged
    {
        int size = sizeof(T);

        // If the size of the type is 1 byte, no swapping is needed (1 byte has no endianness).
        if (size == 1)
            return value;

        // Allocate a buffer on the stack to store the byte representation of the value.
        Span<byte> buffer = stackalloc byte[size];

        // Create a span over the bytes of the value.
        Span<byte> val = new Span<byte>(&value, size);

        // Copy the value's bytes to the buffer.
        val.CopyTo(buffer);

        // Reverse the bytes to swap the endianness.
        buffer.Reverse();

        // Convert the reversed bytes back to the original type and return the result.
        fixed (void* pBuf = buffer)
            return *(T*)pBuf;
    }

    // Overloads for common unmanaged types
    public static short FromEndian(this short value, Endianness endian) => value.FromEndian<short>(endian);
    public static ushort FromEndian(this ushort value, Endianness endian) => value.FromEndian<ushort>(endian);
    public static int FromEndian(this int value, Endianness endian) => value.FromEndian<int>(endian);
    public static uint FromEndian(this uint value, Endianness endian) => value.FromEndian<uint>(endian);
    public static long FromEndian(this long value, Endianness endian) => value.FromEndian<long>(endian);
    public static ulong FromEndian(this ulong value, Endianness endian) => value.FromEndian<ulong>(endian);
    public static float FromEndian(this float value, Endianness endian) => value.FromEndian<float>(endian);
    public static double FromEndian(this double value, Endianness endian) => value.FromEndian<double>(endian);

    /// <summary>
    /// Converts the byte order (endianness) of each element in a span of unmanaged types based on the desired endianness.
    /// </summary>
    /// <typeparam name="T">The unmanaged type contained within the span.</typeparam>
    /// <param name="values">The span of values whose byte order will be converted.</param>
    /// <param name="endian">The desired endianness (either <see cref="Endianness.Little"/> or <see cref="Endianness.Big"/>).</param>
    public static void FromEndian<T>(Span<T> value, Endianness endian) where T : unmanaged
    {
        bool isLittleEndian = BitConverter.IsLittleEndian;

        // If the system's endianness matches the desired endianness, no conversion is needed.
        if (isLittleEndian && endian == Endianness.Little)
            return;

        // If the system is big-endian and desired endianness is big, return as-is
        if (!isLittleEndian && endian == Endianness.Big)
            return;

        // Swap the byte order of each element in the span.
        for (int i = 0; i < value.Length; i++)
            value[i] = value[i].SwapEndianness();
    }
}

/// <summary>
/// Defines the possible endianness types: Little-endian or Big-endian.
/// </summary>
public enum Endianness
{
    Little,
    Big,
}