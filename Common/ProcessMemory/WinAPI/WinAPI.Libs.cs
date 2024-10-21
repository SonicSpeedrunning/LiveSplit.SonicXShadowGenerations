namespace Helper.Common.ProcessInterop.API.Definitions;

/// <summary>
/// A static class that contains constant strings representing the names of important system libraries.
/// </summary>
internal static class Libs
{
    /// <summary>
    /// The name of the Windows Kernel32 library, which contains core functions for Windows applications.
    /// </summary>
    internal const string Kernel32 = "kernel32.dll";

    /// <summary>
    /// The name of the Psapi library, which provides functions for process and system performance information.
    /// </summary>
    internal const string Psapi = "psapi.dll";

    /// <summary>
    /// The name of the Ntdll library, which contains NT kernel functions and is used for low-level operations.
    /// </summary>
    internal const string Ntdll = "ntdll.dll";
}
