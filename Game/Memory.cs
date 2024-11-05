using System;
using JHelper.Common.Collections;
using JHelper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.Game;

internal abstract class Memory
{
    /// <summary>
    /// Simple counter used for lazily update the watchers
    /// </summary>
    protected MemStateTracker StateTracker { get; }

    /// <summary>
    /// Determines if the game has transitioned from the title screen and should start LiveSplit's timer.
    /// </summary>
    /// <returns>True if the autosplitter timer should be started; otherwise, false.</returns>
    internal abstract bool Start(Settings settings);

    /// <summary>
    /// Determines if a split should occur based on game conditions.
    /// </summary>
    /// <returns>True if a split should occur; otherwise, false.</returns>
    internal abstract bool Split(Settings settings);

    /// <summary>
    /// Determines if the autosplitter timer should reset.
    /// </summary>
    /// <returns>True if the autospltter should reset; otherwise, false.</returns>
    internal abstract bool Reset(Settings settings);

    /// <summary>
    /// Determines if the game is currently in a loading state.
    /// </summary>
    /// <returns>True if the game is loading; otherwise, false.</returns>
    internal abstract bool? IsLoading(Settings settings);

    /// <summary>
    /// Sets the autosplitter's game time.
    /// </summary>
    /// <returns>The game time as a nullable TimeSpan.</returns>
    internal abstract TimeSpan? GameTime(Settings settings);

    /// <summary>
    /// Updates the memory state of the game
    /// </summary>
    /// <param name="process">The current <see cref="ProcessMemory"/> instance.</param>
    internal abstract void Update(ProcessMemory process, Settings settings);

    public Memory()
    {
        StateTracker = new MemStateTracker();
    }
}
