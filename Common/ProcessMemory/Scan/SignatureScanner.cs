using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Helper.Common.ProcessInterop.API;

namespace Helper.Common.ProcessInterop;

/// <summary>
/// Static class to scan memory for a specific signature pattern within a given byte array.
/// Useful for identifying particular byte sequences in memory.
/// </summary>
public static class SignatureScanner
{
    /// <summary>
    /// Scans the memory of a process for all instances of a specific signature pattern.
    /// </summary>
    /// <param name="pHandle">Handle to the process whose memory is being scanned.</param>
    /// <param name="pattern">The signature pattern to look for in memory.</param>
    /// <param name="baseAddress">Base address of the memory region to scan.</param>
    /// <param name="size">Size of the memory region to scan.</param>
    /// <returns>An enumerable collection of memory addresses where the pattern was found.</returns>
    public static IEnumerable<IntPtr> ScanAll(IntPtr pHandle, ScanPattern pattern, IntPtr baseAddress, int size)
    {
        // Ensure the size of the memory region is greater than 0
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");

        // Ensure the memory region is not smaller than the pattern itself
        if (pattern.Pattern.Length > size)
            throw new InvalidOperationException("The memory region to scan in cannot be shorter than the signature pattern.");

        // This sigscan essentially works by reading one memory page (0x1000 bytes)
        // at a time and looking for the signature in each page. We create a buffer
        // sligthly larger than 0x1000 bytes (0x1000 + the length of the signature - 1)
        // in order to keep the very first bytes as the tail of the previous memory page.
        // This allows to scan across the memory page boundaries.

        int signatureLength = pattern.Pattern.Length - 1;   // In reality we want the length - 1, as we need it as an indexer
        nint address = baseAddress;                         // This will get updated while the scanner advances through the memory pages
        nint endAddress = baseAddress + size;               // Final address we want the scanner to stop
        bool lastPageSuccess = false;                       // For (small) performance gains we can save some processing power in certain situations if reading a memory page fails
        const int PAGE_SIZE = 0x1000;                       // The size of a memory opage (0x1000 bytes)

        // Rent a buffer from ArrayPool to avoid allocating a new one on each iteration
        byte[] buffer = ArrayPool<byte>.Shared.Rent(PAGE_SIZE + signatureLength);

        try
        {
            // Iterate over memory pages in 0x1000 (page size) increments
            while (address < endAddress)
            {
                // Calculate the next page boundary
                nint pageBoundary = (address & ~0xFFF) + 0x1000;
                nint end = pageBoundary < endAddress ? pageBoundary : endAddress;
                int length = (int)(end - address);

                // If the previous page read was successful, move the last few bytes to the start of the buffer
                if (lastPageSuccess)
                    buffer.AsSpan(PAGE_SIZE, signatureLength).CopyTo(buffer.AsSpan(0, signatureLength));

                if (WinAPI.ReadProcessMemory(pHandle, address, buffer.AsSpan(signatureLength, length)))
                {
                    IEnumerable<int> scan = lastPageSuccess
                        ? ScanAll(pattern, buffer, 0, length + signatureLength)
                        : ScanAll(pattern, buffer, signatureLength, length);

                    foreach (int value in scan)
                    {
                        // Calculate the exact address of the match in the process memory
                        IntPtr foundAddress = lastPageSuccess ?
                            address + value - signatureLength
                            : address + value;

                        // Handle cases where a callback (OnFound) is defined
                        yield return pattern is MemoryScanPattern memoryPattern
                            ? memoryPattern.OnFound is not null ? memoryPattern.OnFound(foundAddress) : foundAddress
                            : foundAddress;
                    }

                    lastPageSuccess = true; // Set success flag for the current page
                }

                address = end; // Move to the next page
            }
        }
        finally
        {
            // Return the rented buffer to the pool to avoid memory leaks
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Scans the entire data buffer for a single signature pattern and returns all found offsets.
    /// </summary>
    /// <param name="pattern">The signature pattern to search for.</param>
    /// <param name="data">The byte array to search in.</param>
    /// <returns>An enumerable of matching offsets where the pattern is found.</returns>
    public static IEnumerable<int> ScanAll(ScanPattern pattern, byte[] data)
    {
        if (pattern.Pattern.Length > data.Length)
            throw new InvalidOperationException("The buffer to scan in cannot be shorter than the signature pattern.");

        return ScanAll(pattern, data, 0, data.Length);
    }

    /// <summary>
    /// Scans the data buffer for a single signature pattern and returns all found offsets.
    /// </summary>
    /// <param name="pattern">The signature pattern to search for.</param>
    /// <param name="data">The byte array to search in.</param>
    /// <param name="startIndex">The starting index in the data buffer.</param>
    /// <param name="noOfElements">The number of elements (bytes) to scan from the start index.</param>
    /// <returns>An enumerable of matching offsets where the pattern is found.</returns>
    private static IEnumerable<int> ScanAll(ScanPattern pattern, byte[] data, int startIndex, int noOfElements)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "The start index must be greater than or equal to zero.");

        if (pattern.Pattern.Length > noOfElements)
            throw new InvalidOperationException("The buffer to scan in cannot be shorter than the signature pattern.");

        using (ScanEnumerator scanner = new ScanEnumerator(pattern, data, startIndex, noOfElements))
        {
            while (scanner.MoveNext())
                yield return scanner.Current + pattern.Offset;
        }
    }

    /// <summary>
    /// Scans the data buffer for multiple signature patterns and returns all found offsets.
    /// </summary>
    /// <param name="patterns">An array of signature patterns to search for.</param>
    /// <param name="data">The byte array to search in.</param>
    /// <param name="startIndex">The starting index in the data buffer.</param>
    /// <param name="noOfElements">The number of elements (bytes) to scan from the start index.</param>
    /// <returns>An enumerable of matching offsets where any pattern is found.</returns>
    public static IEnumerable<int> ScanAll(ScanPattern[] patterns, byte[] data, int startIndex, int noOfElements)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "The start index must be greater than or equal to zero.");

        if (patterns.Any(pattern => pattern.Pattern.Length > noOfElements))
            throw new InvalidOperationException("The buffer to scan in cannot be shorter than the signature pattern.");

        foreach (ScanPattern pattern in patterns)
        {
            using (ScanEnumerator scanner = new ScanEnumerator(pattern, data, startIndex, noOfElements))
            {
                while (scanner.MoveNext())
                    yield return scanner.Current + pattern.Offset;
            }
        }
    }

    /// <summary>
    /// Enumerator that scans through a byte buffer and finds occurrences of a specific scan pattern.
    /// </summary>
    private struct ScanEnumerator : IEnumerator<int>
    {
        public int Current { get; private set; }    // The current position in the buffer (index of the found pattern)
        object IEnumerator.Current => Current;      // Explicit implementation for the non-generic IEnumerator

        private readonly int startCursor; // Start index for scanning
        private int currentCursor;        // The current scan position in the buffer
        private readonly int endCursor;   // The limit for the enumerator to prevent scanning beyond the valid range

        private readonly ScanPattern scanPattern;   // The pattern we are scanning for
        private readonly byte[] haystack;           // The byte buffer where the scan occurs

        /// <summary>
        /// Initializes the ScanEnumerator with the signature pattern and the data buffer.
        /// </summary>
        /// <param name="signature">The pattern to search for.</param>
        /// <param name="data">The byte array to scan.</param>
        public ScanEnumerator(ScanPattern signature, byte[] data)
            : this(signature, data, data.Length) { }

        /// <summary>
        /// Initializes the ScanEnumerator with the signature pattern and the data buffer, limited by 'noOfElements'.
        /// </summary>
        /// <param name="signature">The pattern to search for.</param>
        /// <param name="data">The byte array to scan.</param>
        /// <param name="noOfElements">The number of elements (bytes) to scan from the start of the buffer.</param>
        public ScanEnumerator(ScanPattern signature, byte[] data, int noOfElements)
            : this(signature, data, 0, noOfElements) { }

        /// <summary>
        /// Initializes the ScanEnumerator with the signature pattern, data buffer, start index, and number of elements to scan.
        /// </summary>
        /// <param name="signature">The pattern to search for.</param>
        /// <param name="data">The byte array to scan.</param>
        /// <param name="startIndex">The starting index in the byte array.</param>
        /// <param name="noOfElements">The number of elements (bytes) to scan from the start index.</param>
        public ScanEnumerator(ScanPattern signature, byte[] data, int startIndex, int noOfElements)
        {
            if (startIndex + noOfElements > data.Length)
                throw new IndexOutOfRangeException("Tried to perform a scan outside the bounds of the provided data array");

            haystack = data;
            scanPattern = signature;
            startCursor = startIndex;
            currentCursor = startIndex;
            endCursor = startIndex + noOfElements - signature.Pattern.Length + 1;
        }

        /// <summary>
        /// Advances the enumerator to the next matching position in the buffer.
        /// </summary>
        /// <returns>True if the next element is successfully found; otherwise, false.</returns>
        public unsafe bool MoveNext()
        {
            int signatureLength = scanPattern.Pattern.Length;

            // Defining locals as they can be faster
            int currentCursor = this.currentCursor;
            int endCursor = this.endCursor;

            // We access memory directly for performance
            fixed (byte* pBuffer = haystack, pPattern = scanPattern.Pattern, pMask = scanPattern.Mask, pSkipOffsets = scanPattern.SkipOffsets)
            {
                while (currentCursor < endCursor)
                {
                    int i = 0;

                    // Compare 8 bytes at a time
                    while (i + 8 <= signatureLength)
                    {
                        if ((*(ulong*)(pBuffer + currentCursor + i) & *(ulong*)(pMask + i)) != *(ulong*)(pPattern + i))
                            goto end;
                        i += 8;
                    }

                    // Compare 4 bytes at a time
                    while (i + 4 <= signatureLength)
                    {
                        if ((*(uint*)(pBuffer + currentCursor + i) & *(uint*)(pMask + i)) != *(uint*)(pPattern + i))
                            goto end;
                        i += 4;
                    }

                    // Compare 2 bytes at a time
                    while (i + 2 <= signatureLength)
                    {
                        if ((*(ushort*)(pBuffer + currentCursor + i) & *(ushort*)(pMask + i)) != *(ushort*)(pPattern + i))
                            goto end;
                        i += 2;
                    }

                    // Compare remaining bytes one by one
                    while (i < signatureLength)
                    {
                        if ((*(pBuffer + currentCursor + i) & *(pMask + i)) != *(pPattern + i))
                            goto end;
                        i += 1;
                    }

                    // If we find a match, set current to the current cursor position and return true
                    Current = currentCursor - startCursor;
                    currentCursor += 1; // Move cursor to next byte
                    this.currentCursor = currentCursor;
                    return true;

                end:
                    // Skip over unnecessary comparisons using the skip offset table
                    currentCursor += *(pSkipOffsets + *(pBuffer + currentCursor + signatureLength - 1));
                }
            }

            this.currentCursor = currentCursor;
            return false;
        }

        /// <summary>
        /// Resets the enumerator to its initial position, which is before the first element.
        /// </summary>
        public void Reset()
        {
            currentCursor = startCursor;
        }

        /// <summary>
        /// Releases any resources used by the enumerator.
        /// </summary>
        public void Dispose() { } // No unmanaged resources to release in this implementation
    }
}

/// <summary>
/// Represents a memory scan pattern, defined by a byte signature and an optional offset. 
/// The pattern can include wildcards (??) for variable bytes.
/// </summary>
public class ScanPattern
{
    internal byte[] Pattern { get; }    // Array of parsed pattern bytes
    internal byte[] Mask { get; }       // Array indicating which bytes are fixed and which are wildcards
    internal byte[] SkipOffsets { get; } // Offset skip table
    internal int Offset { get; }        // Offset to apply when pattern is found

    /// <summary>
    /// Initializes a new instance of the <see cref="ScanPattern"/> class by parsing the signature string.
    /// </summary>
    /// <param name="signature">The signature string (e.g., "AA BB ??").</param>
    /// <exception cref="FormatException">Thrown when the signature format is invalid.</exception>
    public ScanPattern(string signature)
        : this(0, signature) { }

    // Regex to validate the signature format. Accepts two hex characters or wildcards (??) with optional spaces.
    private static readonly Regex regex = new Regex(@"^(([0-9A-Fa-f?]{2})\s*)*$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="ScanPattern"/> class by parsing the signature string.
    /// </summary>
    /// <param name="offset">Offset to be added to the found address.</param>
    /// <param name="signature">The signature string (e.g., "AA BB ??").</param>
    /// <exception cref="FormatException">Thrown when the signature format is invalid.</exception>
    public ScanPattern(int offset, string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new FormatException("Signature cannot be empty.");

        if (!regex.IsMatch(signature))
            throw new FormatException("Invalid signature format. Use 'XX XX ??' or 'XX XX ?X'.");

        // Remove spaces and ensure even number of hex digits
        signature = signature.Replace(" ", string.Empty);
        if ((signature.Length & 1) != 0)
            throw new FormatException("Invalid signature format. Each byte should have exactly two hex digits.");

        int length = signature.Length / 2;

        if (length > 255)
            throw new NotSupportedException("Sigature cannot be longer than 255 bytes.");

        byte[] _pattern = new byte[length];
        byte[] _mask = new byte[length];

        int index = 0;
        for (int i = 0; i < signature.Length; i += 2)
        {
            byte a = (byte)HexCharToDec(signature[i]);
            byte b = (byte)HexCharToDec(signature[i + 1]);

            byte patternByte = (byte)((a << 4) | (b & 0x0F));
            byte maskByte = (byte)((a != 0x10 ? 1 : 0) * 0xF0 | ((b != 0x10 ? 1 : 0) * 0x0F));

            _pattern[index] = (byte)(patternByte & maskByte);
            _mask[index] = maskByte;
            index += 1;
        }

        byte[] skipoffsets = new byte[256];
        byte unknown = 0;

        {
            int end = length - 1;

            for (int i = 0; i < end; i++)
            {
                byte @byte = _pattern[i];
                byte mask = _mask[i];

                if (mask == 0xFF)
                    skipoffsets[@byte] = (byte)(end - i);
                else
                    unknown = (byte)(end - i);
            }

            if (unknown == 0)
                unknown = (byte)length;

            for (int i = 0; i < 256; i++)
            {
                if (unknown < skipoffsets[i] || skipoffsets[i] == 0)
                    skipoffsets[i] = unknown;
            }
        }

        Pattern = _pattern;
        Mask = _mask;
        Offset = offset;
        SkipOffsets = skipoffsets;
    }

    /// <summary>
    /// Converts a hexadecimal character to its decimal representation.
    /// </summary>
    /// <param name="hex">The hexadecimal character.</param>
    /// <returns>The decimal value of the hexadecimal character.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int HexCharToDec(char hex)
    {
        return
            hex == '?' ? 0x10 :
            hex >= '0' && hex <= '9' ? hex - '0' :
            hex >= 'A' && hex <= 'F' ? hex - 'A' + 10 :
            hex - 'a' + 10;
    }
}

/// <summary>
/// Represents a memory scan pattern that extends the base <see cref="ScanPattern"/> class.
/// This class is meant to be used when scanning the memory of a target.
/// It includes an optional callback that can be invoked when a pattern is found during a scan.
/// </summary>
public class MemoryScanPattern : ScanPattern
{
    /// <summary>
    /// A delegate that defines the callback signature to be used when the pattern is found.
    /// The callback returns an <see cref="IntPtr"/> representing the found address.
    /// </summary>
    /// <param name="addr">The address of the found pattern in the memory.</param>
    /// <returns>An <see cref="IntPtr"/> value that could be used to handle the found address.</returns>
    public OnFoundCallback? OnFound { get; set; }

    /// <summary>
    /// Optional callback function that gets invoked when the pattern is found.
    /// This allows custom handling or further processing when a match is detected.
    /// </summary>
    public delegate IntPtr OnFoundCallback(IntPtr addr);

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryScanPattern"/> class with the specified signature.
    /// </summary>
    /// <param name="signature">The memory signature to scan for, represented as a string (e.g., "AA BB ??").</param>
    public MemoryScanPattern(string signature)
        : base(signature) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryScanPattern"/> class with the specified signature and offset.
    /// </summary>
    /// <param name="offset">The offset to be added to the found address when a pattern is matched.</param>
    /// <param name="signature">The memory signature to scan for, represented as a string (e.g., "AA BB ??").</param>
    public MemoryScanPattern(int offset, string signature)
        : base(offset, signature) { }
}