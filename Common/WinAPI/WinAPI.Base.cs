using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Security;
using Helper.Common.ProcessInterop.API.Definitions;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Helper.Common.ProcessInterop.API;

internal static partial class WinAPI
{
    /// <summary>
    /// Opens a handle to a process by its name.
    /// </summary>
    /// <param name="name">The name of the process to search for.</param>
    /// <param name="processId">The ID of the process if found; otherwise, 0.</param>
    /// <param name="pHandle">The handle to the process if found; otherwise, IntPtr.Zero.</param>
    /// <returns>True if the process is found and the handle is opened; otherwise, false.</returns>
    [SkipLocalsInit]
    internal static bool OpenProcessHandleByName(string name, out int processId, out IntPtr pHandle)
    {
        const int processIdsArraySize = 512; // The maximum number of process IDs to retrieve (512 is a good estimate)
        int[]? processIds = null;            // Array to hold the process IDs

        try
        {
            // Rent an array from the shared pool to hold the process IDs
            processIds = ArrayPool<int>.Shared.Rent(processIdsArraySize);
            int sizeNeeded;

            unsafe
            {
                fixed (int* pProcessIds = processIds)
                {
                    // Get the array of process IDs
                    if (!EnumProcesses(pProcessIds, processIdsArraySize * sizeof(int), out sizeNeeded))
                    {
                        // If the enumeration fails, set outputs to default and return false
                        processId = 0;
                        pHandle = IntPtr.Zero;
                        return false;
                    }
                }
            }
            
            int numProcesses = sizeNeeded / sizeof(int);

            // Iterate over each process ID and yield the process name
            for (int i = 0; i < numProcesses; i++)
            {
                int localProcessId = processIds[i];

                if (!OpenProcess(localProcessId, out IntPtr localHandle))
                    continue;   // If unable to open, skip to the next process

                // If we found the right process, return
                if (IsOpen(localHandle) && GetProcessName(localHandle) == name)
                {
                    // If a match is found, set the outputs and return true
                    processId = localProcessId;
                    pHandle = localHandle;
                    return true;
                }

                // If we didn't find the right process, close the handle to avoid leaks
                CloseProcessHandle(localHandle);
            }
        }
        finally
        {
            // Clean up: Return the rented array back to the pool to avoid memory leaks
            if (processIds is not null)
                ArrayPool<int>.Shared.Return(processIds);
        }

        // If no matching process is found, set outputs to default and return false
        processId = 0;
        pHandle = IntPtr.Zero;
        return false;

        // Importing necessary methods from the Windows API
        [DllImport(Libs.Psapi)]
        [SuppressUnmanagedCodeSecurity]
        static unsafe extern bool EnumProcesses(int* lpidProcess, int cb, out int lpcbNeeded);
    }

    /// <summary>
    /// Retrieves the name of the process associated with the specified handle.
    /// </summary>
    /// <param name="pHandle">A handle to the process whose name is to be retrieved.</param>
    /// <returns>
    /// The name of the process, or an empty string if the name cannot be retrieved.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the process handle is invalid (i.e., IntPtr.Zero).
    /// </exception>
    [SkipLocalsInit]
    internal static string GetProcessName(IntPtr pHandle)
    {
        if (pHandle == IntPtr.Zero)
            throw new InvalidOperationException("Invalid process handle.");

        const int BUFFER_LENGTH = 256;
        Span<char> nameBuffer = stackalloc char[BUFFER_LENGTH];

        unsafe
        {
            fixed (char* pNameBuffer = nameBuffer)
            {
                uint size = GetModuleBaseNameW(pHandle, IntPtr.Zero, pNameBuffer, BUFFER_LENGTH);
                return size == 0 ? string.Empty : Marshal.PtrToStringUni((IntPtr)pNameBuffer);
            }
        }
        
        // Import the GetModuleBaseNameW function from the psapi.dll
        [DllImport(Libs.Psapi)]
        [SuppressUnmanagedCodeSecurity]
        static unsafe extern uint GetModuleBaseNameW(IntPtr hProcess, IntPtr hModule, char* lpBaseName, int nSize);
    }

    /// <summary>
    /// Opens a handle to the specified process id.
    /// </summary>
    /// <param name="processId">The unique id of the process to hook to.</param>
    /// <returns>A handle to the target process if the function succeeds; otherwise, IntPtr.Zero</returns>
    internal static bool OpenProcess(int processId, out IntPtr processHandle)
    {
        // Constants that define the process access flags for reading and querying the process.
        const uint PROCESS_VM_READ = 0x0010;             // Grants read access to the process's memory
        const uint PROCESS_VM_WRITE = 0x0020;            // Grants write access to the process's memory
        const uint PROCESS_VM_OPERATION = 0x0008;        // Required to perform an operation on the address space of a process (including WriteProcessMemory)
        const uint PROCESS_QUERY_INFORMATION = 0x0400;   // Grants the ability to query information about the process
        const uint SYNCHRONIZE = 0x00100000;             // Allows to use wait functions, eg. WaitForSingleObject

        // Open the process with the required permissions. The function fails if the returned handle is zero.
        processHandle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION | SYNCHRONIZE, 0, processId);
        return processHandle != IntPtr.Zero;

        // The OpenProcess function is imported from kernel32.dll.
        // It is used to open a handle to the external process with the specified access rights.
        [DllImport(Libs.Kernel32)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);
    }

    /// <summary>
    /// Closes a specified process handle, freeing the relative unmanaged resources.
    /// </summary>
    internal static bool CloseProcessHandle(IntPtr processHandle)
    {
        return CloseHandle(processHandle) != 0;

        // The CloseHandle function is imported from kernel32.dll.
        // It is used to close the handle to the external process when we're done with it.
        // As the handle is an unmanaged resource, it is important to close the handle in order to prevent leaking of resources.
        [DllImport(Libs.Kernel32)]
        [SuppressUnmanagedCodeSecurity]
        static extern int CloseHandle(IntPtr hObject);
    }

    /// <summary>
    /// Checks if the specified process is still running.
    /// </summary>
    /// <param name="handle">Handle to the process.</param>
    /// <returns>True if the process is still running, false if it has exited.</returns>
    internal static bool IsOpen(IntPtr handle)
    {
        // Constants used by WaitForSingleObject
        const uint WAIT_TIMEOUT = 0x00000102;

        // We pass a 0 ms timeout to check the process state, so it returns immediately without blocking
        return WaitForSingleObject(handle, 0) == WAIT_TIMEOUT;

        // Importing WaitForSingleObject from kernel32.dll
        [DllImport(Libs.Kernel32)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
    }

    /// <summary>
    /// Determines if the given process is 64-bit.
    /// </summary>
    /// <param name="hProcess">The handle to the process.</param>
    /// <returns>True if the process is 64-bit, false if it is 32-bit.</returns>
    internal static bool Is64Bit(IntPtr hProcess)
    {
        if (hProcess == IntPtr.Zero)
            throw new InvalidOperationException("Invalid process handle.");

        // Check if the operating system is 64-bit
        // No processes are 64-bit on a 32-bit OS
        if (!Environment.Is64BitOperatingSystem)
            return false;

        Version osVersion = Environment.OSVersion.Version;
        
        if (osVersion.Major > 10 || (osVersion.Major == 10 && osVersion.Build >= 10586))
        {
            return IsWow64Process2(hProcess, out ushort processMachine, out _) != 0
                && processMachine != 0x014C  // x86
                && processMachine != 0x01C0  // ARM
                && processMachine != 0x01C4; // ARMv7

            [DllImport(Libs.Kernel32)]
            [SuppressUnmanagedCodeSecurity]
            static extern int IsWow64Process2(IntPtr hProcess, out ushort pProcessMachine, out ushort pNativeMachine);
        }
        else
        {
            // Check if the process is running under WOW64.
            // If the system call fails, we return false by default, although this result
            // is inconsequential as it generally assumes the presence of invalid handle.
            return IsWow64Process(hProcess, out int iswow64) != 0 && iswow64 == 0;

            [DllImport(Libs.Kernel32)]
            [SuppressUnmanagedCodeSecurity]
            static extern int IsWow64Process(IntPtr hProcess, out int wow64Process);
        }
    }
}
