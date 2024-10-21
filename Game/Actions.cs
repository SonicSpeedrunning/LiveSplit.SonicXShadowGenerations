using System;

namespace LiveSplit.SonicXShadowGenerations.Game
{
    internal static class Actions
    {
        internal static bool Start(Watchers watchers, Settings settings)
        {
            return false;
        }

        internal static bool Split(Watchers watchers, Settings settings)
        {
            return false;
        }

        internal static bool Reset(Watchers watchers, Settings settings)
        {
            return false;
        }

        internal static bool? IsLoading(Watchers watchers, Settings settings)
        {
            return watchers.IsLoading.Current;
        }

        internal static TimeSpan? GameTime(Watchers watchers, Settings settings, Memory memory)
        {
            return null;
        }
    }
}
