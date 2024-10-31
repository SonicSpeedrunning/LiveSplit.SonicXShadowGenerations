using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helper.Common.ProcessInterop.API;

namespace Helper.Common.ProcessInterop;

/// <summary>
/// Represents a collection of symbols (function names and addresses) from an external process module.
/// </summary>
public readonly struct SymbolCollection : IEnumerable<Symbol>
{
    private readonly IntPtr pHandle;
    private readonly IntPtr moduleBaseAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolCollection"/> struct.
    /// </summary>
    /// <param name="pHandle">The handle to the external process.</param>
    /// <param name="moduleBaseAddress">The base address of the module in memory.</param>
    /// <param name="moduleMemorySize">The size of the module's memory.</param>
    public SymbolCollection(IntPtr pHandle, IntPtr moduleBaseAddress)
    {
        this.pHandle = pHandle;
        this.moduleBaseAddress = moduleBaseAddress;
    }

    /// <summary>
    /// Gets the memory address of the symbol with the specified name.
    /// </summary>
    /// <param name="name">The name of the symbol (e.g., function) to retrieve.</param>
    /// <returns>The memory address of the symbol associated with the specified name.</returns>
    public IntPtr this[string name] => this.First(m => m.Name == name).Address;

    /// <summary>
    /// Attempts to retrieve the memory address of a symbol by name.
    /// </summary>
    /// <param name="name">The name of the symbol to search for.</param>
    /// <param name="symbolAddress">When the method returns, contains the memory address of the symbol, if found.</param>
    /// <returns><c>true</c> if the symbol was found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(string name, out IntPtr symbolAddress)
    {
        symbolAddress = this.FirstOrDefault(m => m.Name == name).Address;
        return symbolAddress != default;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the symbols in the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the symbols.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return WinAPI.EnumerateFunctions(pHandle, moduleBaseAddress).GetEnumerator();
    }

    /// <summary>
    /// Returns a strongly-typed enumerator that iterates through the symbols in the collection.
    /// </summary>
    /// <returns>An enumerator of <see cref="Symbol"/> to iterate through the symbols.</returns>
    IEnumerator<Symbol> IEnumerable<Symbol>.GetEnumerator()
    {
        return WinAPI.EnumerateFunctions(pHandle, moduleBaseAddress).GetEnumerator();
    }
}

/// <summary>
/// Represents a symbol, which includes its name and memory address.
/// Used to represent exported functions or other symbols in a process module.
/// </summary>
public readonly record struct Symbol
{
    /// <summary>
    /// Gets the name of the symbol (e.g., function name).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the memory address of the symbol.
    /// </summary>
    public IntPtr Address { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Symbol"/> record.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="address">The memory address of the symbol.</param>
    public Symbol(string name, IntPtr address)
    {
        Name = name;
        Address = address;
    }
}