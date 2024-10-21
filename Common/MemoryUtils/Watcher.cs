namespace Helper.Common.MemoryUtils;

/// <summary>
/// Abstract base class representing a watcher that keeps track of a current and an old value.
/// </summary>
public abstract class Watcher
{
    /// <summary>
    /// Gets or sets a value indicating whether the watcher is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Indicates whether the value has changed in the last update.
    /// </summary>
    public virtual bool Changed { get; protected set; } = default;

    /// <summary>
    /// Updates the current value and determines if it has changed.
    /// </summary>
    /// <returns>True if the value was updated; otherwise, false.</returns>
    public abstract bool Update();

    /// <summary>
    /// Resets the current and old values of the watcher.
    /// </summary>
    public abstract void Reset();
}

/// <summary>
/// A generic container for a pair of values, stored as <see cref="Current"/> and <see cref="Old"/>, 
/// for comparisons and other evaluations inside the logic of an auto splitter.
/// </summary>
/// <typeparam name="T">The type of the values being watched.</typeparam>
public class Watcher<T> : Watcher where T : notnull
{
    /// <summary>
    /// Gets the current value of type <typeparamref name="T"/>.
    /// </summary>
    public virtual T Current { get; protected set; }

    /// <summary>
    /// Gets the old value of type <typeparamref name="T"/>.
    /// </summary>
    public virtual T Old { get; protected set; }

    // Delegate type for the callback that gets invoked to update the watcher data
    public delegate T Callback(T current, T old);

    /// <summary>
    /// An optional function that retrieves the current value.
    /// </summary>
    protected Callback? Func { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Watcher{T}"/> class with default values.
    /// </summary>
    public Watcher()
    {
        Current = default!;
        Old = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Watcher{T}"/> class and sets a function 
    /// to automatically get the value for <see cref="Current"/> when calling <see cref="Update()"/>.
    /// </summary>
    /// <param name="Func">The function to invoke to get the current value.</param>
    public Watcher(Callback Func)
        : this()
    {
        this.Func = Func;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Watcher{T}"/> class with specified
    /// current and old values, and an optional callback function.
    /// </summary>
    /// <param name="current">The initial value to set for the <see cref="Current"/> property.</param>
    /// <param name="old">The initial value to set for the <see cref="Old"/> property.</param>
    /// <param name="Func">An optional callback function used to update the <see cref="Current"/> value.</param>
    public Watcher(T current, T old, Callback? Func = null)
    {
        Current = current;
        Old = old;

        if (Func is not null)
            this.Func = Func;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Watcher{T}"/> class with specified
    /// default values for <see cref="Current"/> and <see cref="Old"/> properties,
    /// along with an optional callback function.
    /// </summary>
    /// <param name="defaultValue">The default value to set for both the <see cref="Current"/> 
    /// and <see cref="Old"/> properties.</param>
    /// <param name="Func">An optional callback function to be invoked during updates.</param>
    public Watcher(T defaultValue, Callback? Func = null)
        : this(defaultValue, defaultValue, Func) { }

    /// <summary>
    /// Moves the current value to the old value and runs a previously defined <see cref="Func"/> 
    /// to get the new value for <see cref="Current"/>.
    /// </summary>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public override bool Update()
    {
        Changed = false;

        if (!Enabled)
            return false;

        if (Func is null)
            return false;

        UpdateInternal(Func.Invoke(Current, Old));
        return true;
    }

    /// <summary>
    /// Moves the current value to the old value and manually sets a new value for <see cref="Current"/>.
    /// </summary>
    /// <param name="newValue">The new value to set as the current value.</param>
    public void Update(T newValue)
    {
        Changed = false;

        if (!Enabled)
            return;

        UpdateInternal(newValue);
    }

    /// <summary>
    /// Updates the internal values of the watcher. Moves <see cref="Current"/> to <see cref="Old"/> 
    /// and sets <see cref="Current"/> to a new value.
    /// </summary>
    /// <param name="newValue">The new value to set as the current value.</param>
    private void UpdateInternal(T newValue)
    {
        if (Current is not null)
            Old = Current;

        Current = newValue;

        Changed = Old is null || !Old.Equals(Current);
    }

    /// <summary>
    /// Resets the values stored in the current <see cref="Watcher{T}"/> instance, including the 
    /// associated <see cref="Func"/> if defined.
    /// </summary>
    public override void Reset()
    {
        Current = default!;
        Old = default!;
        Changed = default;
        Func = null;
    }

    /// <summary>
    /// Sets a new <see cref="Func"/> to be invoked when calling <see cref="Update()"/>.
    /// </summary>
    /// <param name="func">The function to set.</param>
    public void SetFunc(Callback func)
    {
        Func = func;
    }
}