using System;
using System.Runtime.CompilerServices;

namespace LiveSplit.SonicXShadowGenerations.Common;

/// <summary>
/// Custom extension methods used in this autosplitter
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Checks if a provided IntPtr value is equal to IntPtr.Zero
    /// </summary>
    /// <param name="value">The IntPtr value to check</param>
    /// <returns>True if the value is IntPtr.Zero, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this IntPtr value)
    {
        return value == IntPtr.Zero;
    }

    /// <summary>
    /// Checks if a provided IntPtr value is not equal to IntPtr.Zero
    /// </summary>
    /// <param name="value">The IntPtr value to check</param>
    /// <returns>True if the value is not IntPtr.Zero, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotZero(this IntPtr value)
    {
        return !IsZero(value);
    }
}
