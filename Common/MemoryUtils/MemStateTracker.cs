using System;

namespace Helper.Common.Collections;

/// <summary>
/// A simple utility class that acts as a counter, incrementing by 1 each time 
/// <see cref="Tick"/> is called. When the counter reaches <see cref="uint.MaxValue"/>, 
/// it wraps around and resets to 0.
/// <br />
/// This class is primarily used within caching mechanisms or lazy evaluation scenarios 
/// where it is necessary to track changes or updates over time, such as in the 
/// <see cref="LazyWatcher"/> class.
/// </summary>
public sealed class MemStateTracker
{
    /// <summary>
    /// The current tick, stored as an unsigned 32-bit integer.
    /// </summary>
    public uint CurrentTick { get; private set; } = uint.MinValue;

    /// <summary>
    /// Occurs when the tick count is incremented.
    /// </summary>
    public event EventHandler? OnTick;


    /// <summary>
    /// Initializes a new instance of the <see cref="MemStateTracker"/> class, 
    /// setting the initial tick value to 0.
    /// </summary>
    public MemStateTracker()
    { }

    /// <summary>
    /// Increments the current tick count by 1. If the current value equals <see cref="uint.MaxValue"/>, 
    /// the counter wraps around and resets to 0.
    /// </summary>
    public void Tick()
    {
        if (CurrentTick == uint.MaxValue)
            CurrentTick = uint.MinValue;
        else
            CurrentTick++;

        OnTick?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets the tick count to 0 manually.
    /// </summary>
    public void Reset()
    {
        CurrentTick = default;
    }
}
