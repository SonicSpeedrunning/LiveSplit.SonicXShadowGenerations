using System;
using Helper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.Game;

internal abstract class IMemory
{
    internal abstract void Update(ProcessMemory process);
    internal abstract bool Start(Settings settings);
    internal abstract bool Split(Settings settings);
    internal abstract bool Reset(Settings settings);
    internal abstract bool? IsLoading(Settings settings);
    internal abstract TimeSpan? GameTime(Settings settings);
}
