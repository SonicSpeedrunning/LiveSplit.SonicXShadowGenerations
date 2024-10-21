using Helper.Common.Collections;

namespace Helper.Common.MemoryUtils;

/// <summary>
/// A special implementation of <see cref="Watcher{T}"/> that uses an internal Tick value 
/// to lazily update <see cref="Current"/> and <see cref="Old"/> when needed.
/// </summary>
/// <typeparam name="T">The type of the value being watched, constrained to non-nullable types.</typeparam>
public class LazyWatcher<T> : Watcher<T> where T : notnull
{
    /// <summary>
    /// The current tick value used to determine if an update is necessary.
    /// </summary>
    protected uint tick = 0;

    /// <summary>
    /// A reference to a <see cref="MemStateTracker"/> instance used to track ticks.
    /// </summary>
    protected readonly MemStateTracker tickRef;

    /// <summary>
    /// Gets a value indicating whether an update is needed based on the current tick.
    /// </summary>
    protected bool NeedsUpdate => tick != tickRef.CurrentTick;

    /// <summary>
    /// Gets the current value, updating it if the tick has changed.
    /// </summary>
    public override T Current { get { Update(); return base.Current; } protected set => base.Current = value; }

    /// <summary>
    /// Gets the old value, updating it if the tick has changed.
    /// </summary>
    public override T Old { get { Update(); return base.Old; } protected set => base.Old = value; }

    /// <summary>
    /// Gets a value indicating whether the value has changed, updating it if the tick has changed.
    /// </summary>
    public override bool Changed { get { Update(); return base.Changed; } protected set => base.Changed = value; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyWatcher{T}"/> class, 
    /// with a reference to a <see cref="MemStateTracker"/>.
    /// </summary>
    /// <param name="tickRef">The <see cref="MemStateTracker"/> used to track ticks.</param>
    public LazyWatcher(MemStateTracker tickRef)
        : base()
    {
        this.tickRef = tickRef;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyWatcher{T}"/> class 
    /// with a reference to a <see cref="MemStateTracker"/> and an optional callback function.
    /// </summary>
    /// <param name="tickRef">The <see cref="MemStateTracker"/> used to track ticks.</param>
    /// <param name="Func">An optional callback function to set for the watcher.</param>
    public LazyWatcher(MemStateTracker tickRef, Callback Func)
        : base(Func)
    {
        this.tickRef = tickRef;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyWatcher{T}"/> class with specified
    /// initial values for <see cref="Current"/> and <see cref="Old"/>, and a reference to a 
    /// <see cref="MemStateTracker"/>.
    /// </summary>
    /// <param name="tickRef">The <see cref="MemStateTracker"/> used to track ticks.</param>
    /// <param name="current">The initial value to set for the <see cref="Current"/> property.</param>
    /// <param name="old">The initial value to set for the <see cref="Old"/> property.</param>
    /// <param name="Func">An optional callback function to set for the watcher.</param>
    public LazyWatcher(MemStateTracker tickRef, T current, T old, Callback? Func = null)
        : base(current, old, Func)
    {
        this.tickRef = tickRef;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyWatcher{T}"/> class with specified
    /// initial values for <see cref="Current"/> and <see cref="Old"/>, and a reference to a 
    /// <see cref="MemStateTracker"/>.
    /// </summary>
    /// <param name="tickRef">The <see cref="MemStateTracker"/> used to track ticks.</param>
    /// <param name="current">The initial value to set for the <see cref="Current"/> property.</param>
    /// <param name="old">The initial value to set for the <see cref="Old"/> property.</param>
    /// <param name="Func">An optional callback function to set for the watcher.</param>
    public LazyWatcher(MemStateTracker tickRef, T defaultValue, Callback? Func = null)
        : this(tickRef, defaultValue, defaultValue, Func) { }

    /// <summary>
    /// Updates the <see cref="Current"/> and <see cref="Old"/> values only if the current tick
    /// differs from the last recorded tick. If the ticks are the same, no update occurs.
    /// </summary>
    /// <returns>True if an update was performed and successful; otherwise, false.</returns>
    public override bool Update()
    {
        return NeedsUpdate ? ForceUpdate() : false;
    }

    /// <summary>
    /// Forces an update of the <see cref="Current"/> and <see cref="Old"/> values by
    /// setting the internal tick value to the current tick from <see cref="MemStateTracker"/> 
    /// and calling the base update method.
    /// </summary>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public bool ForceUpdate()
    {
        tick = tickRef.CurrentTick;
        return base.Update();
    }
}