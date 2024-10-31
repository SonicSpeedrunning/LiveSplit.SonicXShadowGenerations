using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Helper.Common.ProcessInterop.API;

namespace Helper.Common.ProcessInterop;

/// <summary>
/// Represents a collection of modules in an external process.
/// Implements <see cref="IEnumerable{Module}"/> to allow enumeration of modules.
/// </summary>
public readonly struct ProcessModuleCollection : IEnumerable<ProcessModule>
{
    private readonly IntPtr pHandle;
    private readonly bool firstModuleOnly;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessModuleCollection"/> struct.
    /// </summary>
    /// <param name="processHandle">The handle to the target process.</param>
    /// <param name="firstModuleOnly">Whether to only retrieve the first module.</param>
    public ProcessModuleCollection(IntPtr processHandle, bool firstModuleOnly)
    {
        this.pHandle = processHandle;
        this.firstModuleOnly = firstModuleOnly;
    }

    /// <summary>
    /// Gets the module with the specified name.
    /// </summary>
    /// <param name="name">The name of the module to retrieve.</param>
    /// <returns>The <see cref="ProcessModule"/> instance matching the specified name.</returns>
    public ProcessModule this[string name] => this.First(m => m.ModuleName == name);

    /// <summary>
    /// Gets the main module of the process.
    /// </summary>
    public ProcessModule MainModule => this.First();

    /// <summary>
    /// Tries to retrieve a specified module from the process module collection.
    /// If the specified module is not found, returns the main module as the out parameter.
    /// </summary>
    /// <param name="name">The name of the module to search for.</param>
    /// <param name="module">The <see cref="ProcessModule"/> instance found or the main module if not found.</param>
    /// <returns>True if the module was found; otherwise, false.</returns>
    public bool TryGetValue(string name, out ProcessModule module)
    {
        ProcessModule? mod = this.FirstOrDefault(m => m.ModuleName == name);
        module = mod is null ? MainModule : mod;
        return module is not null;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the process modules.
    /// </summary>
    /// <returns>An IEnumerator object to iterate over the modules.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return WinAPI.EnumProcessModules(pHandle, firstModuleOnly).GetEnumerator();
    }

    /// <summary>
    /// Returns a strongly-typed enumerator that iterates through the process modules.
    /// </summary>
    /// <returns>An IEnumerator of ProcessModule to iterate over the modules.</returns>
    IEnumerator<ProcessModule> IEnumerable<ProcessModule>.GetEnumerator()
    {
        return WinAPI.EnumProcessModules(pHandle, firstModuleOnly).GetEnumerator();
    }
}

/// <summary>
/// Represents a module loaded into a process.
/// Provides information about the module, such as its base address, size, and name.
/// </summary>
public record ProcessModule
{
    private readonly IntPtr processHandle;

    /// <summary>
    /// Gets the base address of the module in the process memory.
    /// </summary>
    public IntPtr BaseAddress { get; }

    /// <summary>
    /// Gets the entry point address of the module.
    /// </summary>
    public IntPtr EntryPointAddress { get; }

    /// <summary>
    /// Gets the file name of the module.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the memory size of the module in bytes.
    /// </summary>
    public int ModuleMemorySize { get; }

    /// <summary>
    /// Gets the name of the module.
    /// </summary>
    public string ModuleName { get; }

    /// <summary>
    /// Gets the file version information of the module.
    /// </summary>
    public FileVersionInfo FileVersionInfo => FileVersionInfo.GetVersionInfo(FileName);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current module (its name).</returns>
    public override string ToString()
    {
        return ModuleName ?? base.ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessModule"/> record.
    /// </summary>
    /// <param name="processHandle">Handle to the target process.</param>
    /// <param name="baseAddress">The base address of the module in the process memory.</param>
    /// <param name="entryPointAddress">The entry point address of the module.</param>
    /// <param name="fileName">The file name of the module.</param>
    /// <param name="moduleMemorySize">The memory size of the module in bytes.</param>
    /// <param name="moduleName">The name of the module.</param>
    internal ProcessModule(IntPtr processHandle, IntPtr baseAddress, IntPtr entryPointAddress, string fileName, int moduleMemorySize, string moduleName)
    {
        this.processHandle = processHandle;
        BaseAddress = baseAddress;
        EntryPointAddress = entryPointAddress;
        FileName = fileName;
        ModuleMemorySize = moduleMemorySize;
        ModuleName = moduleName;
    }

    /// <summary>
    /// Gets the collection of symbols / exported functions associated with the module.
    /// </summary>
    /// <returns>A <see cref="SymbolCollection"/> representing the symbols in the module.</returns>
    public SymbolCollection Symbols => new SymbolCollection(processHandle, BaseAddress);


    /// <summary>
    /// Scans the module's memory for the first occurrence of the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for in the module's memory.</param>
    /// <returns>The address of the first occurrence of the pattern, or <see cref="IntPtr.Zero"/> if not found.</returns>
    public IntPtr Scan(ScanPattern pattern)
    {
        return ScanAll(pattern).FirstOrDefault();
    }

    /// <summary>
    /// Scans the module's memory for all occurrences of the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to search for in the module's memory.</param>
    /// <returns>An enumerable collection of addresses where the pattern is found.</returns>
    public IEnumerable<IntPtr> ScanAll(ScanPattern pattern)
    {
        return SignatureScanner.ScanAll(processHandle, pattern, BaseAddress, ModuleMemorySize);
    }
}