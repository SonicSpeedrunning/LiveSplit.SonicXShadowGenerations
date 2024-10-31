using System;
using System.Diagnostics;
using Helper.Common.MemoryUtils;
using Helper.Common.ProcessInterop;
using Helper.Common.ProcessInterop.API;
using LiveSplit.SonicXShadowGenerations.Common;
using LiveSplit.SonicXShadowGenerations.GameEngine;

namespace LiveSplit.SonicXShadowGenerations.Game.Shadow;

/// <summary>
/// Represents the memory management and state tracking for Shadow Generations.
/// Inherits from the abstract Memory class to provide game state data.
/// </summary>
internal class MemoryShadow : Memory
{
    /// <summary>
    /// The version of the game.
    /// </summary>
    public GameVersion Version { get; }

    /// <summary>
    /// The Hedgehog engine instance used for interacting with the game engine.
    /// </summary>
    private HedgehogEngine2 Engine { get; }

    /// <summary>
    /// Watches the loading state of the game.
    /// </summary>
    public LazyWatcher<bool> Is_Loading { get; }

    /// <summary>
    /// Watches the current game mode (e.g., main menu, in-game).
    /// </summary>
    public LazyWatcher<string> GameMode { get; }

    /// <summary>
    /// Watches the status of the HSM (Hierarchical State Machine).
    /// </summary>
    public LazyWatcher<string[]> HsmStatus { get; }

    /// <summary>
    /// Watches the level ID in the game.
    /// </summary>
    public LazyWatcher<LevelID> LevelID { get; }

    /// <summary>
    /// Watches if the player is currently in the final Quick Time Event (QTE).
    /// </summary>
    private LazyWatcher<bool> IsInFinalQTE { get; }

    /// <summary>
    /// Watches the count of QTEs encountered at the final boss.
    /// </summary>
    public LazyWatcher<int> FinalQTECount { get; }

    /// <summary>
    /// Constructor initializing game engine, version, and various game state watchers.
    /// </summary>
    /// <param name="process">The current game process.</param>
    public MemoryShadow(ProcessMemory process)
        : base()
    {
        Engine = new HedgehogEngine2(process);

        // Determine the game version based on module memory size.
        // Currently unused in the autosplitter, might be needed in the future.
        Version = process.MainModule.ModuleMemorySize switch
        {
            0x1CA2A000 => GameVersion.v1_1_0_0,
            _ => GameVersion.Unknown
        };

        // Initialize LazyWatchers for observing game properties

        GameMode = new LazyWatcher<string>(StateTracker, "GameModeTitle", (current, _) =>
        {
            // Look up the game mode using RTTI
            return Engine.RTTI.Lookup(Engine.GameMode, out string gm) ? gm : current;
        });

        HsmStatus = new LazyWatcher<string[]>(StateTracker, [string.Empty, string.Empty, string.Empty, string.Empty], (current, _) =>
        {
            string[] ret = new string[4]; // Array to hold status strings
            Span<long> buf = stackalloc long[2]; // Buffer to read memory

            // Check for valid game mode and read status from memory
            if (!Engine.GetExtension("GameModeHsmExtension", out IntPtr instance)
                || !process.ReadArray(instance + 0x38, buf))
            {
                // Return the current status if read fails
                ret[0] = current[0];
                ret[1] = current[1];
                ret[2] = current[2];
                ret[3] = current[3];
                return ret;
            }

            // Determine the number of details to read (max 4)
            int no_of_details = (int)buf[1] & 0xF;
            if (no_of_details > 4)
                no_of_details = 4;

            Span<long> details = stackalloc long[no_of_details];
            // Read the details from memory
            if (!process.ReadArray((IntPtr)buf[0], details))
            {
                // Return the current status if read fails
                ret[0] = current[0];
                ret[1] = current[1];
                ret[2] = current[2];
                ret[3] = current[3];
                return ret;
            }

            // Look up each detail and store it in the return array
            for (int i = 0; i < no_of_details; i++)
            {
                if (Engine.RTTI.Lookup((IntPtr)details[i], out string value))
                    ret[i] = value;
            }

            for (int i = no_of_details; i < 4; i++)
                ret[i] = string.Empty;

            return ret;
        });

        Is_Loading = new LazyWatcher<bool>(StateTracker, false, (current, _) =>
        {
            // Determine if the game is currently loading based on game mode and status
            if (GameMode.Current == "GameModeOpening")
                return false;

            if (!Engine.GetExtension("GameModeHsmExtension", out IntPtr _))
                return true;

            return HsmStatus.Current[1] == "Build"
                || HsmStatus.Current[1] == "Quit"
                || HsmStatus.Current[2] == "TransitStage";
        });

        LevelID = new LazyWatcher<LevelID>(StateTracker, Shadow.LevelID.MainMenu, (current, _) =>
        {
            // Read the current level ID from memory
            if (GameMode.Current == "GameModeTitle")
                return Shadow.LevelID.MainMenu;

            if (!Engine.GetService("LevelInfo", out IntPtr plevelInfo)
                || !process.Read(plevelInfo, out LevelInfo levelInfo)
                || levelInfo.StageData.IsZero()
                || !process.Read(levelInfo.StageData, out StageData stageData)
                || stageData.Name.IsZero()
                || !process.ReadString(stageData.Name, 6, StringType.ASCII, out string id))
                return current;

            /*
                if (Engine.LevelInfo.IsZero()
                || !process.Read(Engine.LevelInfo, out LevelInfo levelInfo)
                || levelInfo.StageData.IsZero()
                || !process.Read(levelInfo.StageData, out StageData stageData)
                || stageData.Name.IsZero()
                || !process.ReadString(stageData.Name, 6, StringType.ASCII, out string id))
                return current;
            */

            // Map the read ID to the corresponding LevelID enum
            return id switch
            {
                "w09a10" => Shadow.LevelID.WhiteWorld,
                "w01a11" => Shadow.LevelID.SpaceColonyArk1,
                "w01c11" => Shadow.LevelID.SpaceColonyArk1_Challenge1,
                "w01c12" => Shadow.LevelID.SpaceColonyArk1_Challenge2,
                "w01h11" => Shadow.LevelID.SpaceColonyArk1_ChallengeHard,
                "w01a20" => Shadow.LevelID.SpaceColonyArk2,
                "w01c21" => Shadow.LevelID.SpaceColonyArk2_Challenge1,
                "w01c22" => Shadow.LevelID.SpaceColonyArk2_Challenge2,
                "w01h21" => Shadow.LevelID.SpaceColonyArk2_ChallengeHard,
                "w02a10" => Shadow.LevelID.RailCanyon1,
                "w02c11" => Shadow.LevelID.RailCanyon1_Challenge1,
                "w02c12" => Shadow.LevelID.RailCanyon1_Challenge2,
                "w02h11" => Shadow.LevelID.RailCanyon1_ChallengeHard,
                "w02a20" => Shadow.LevelID.RailCanyon2,
                "w02c21" => Shadow.LevelID.RailCanyon2_Challenge1,
                "w02c22" => Shadow.LevelID.RailCanyon2_Challenge2,
                "w02h21" => Shadow.LevelID.RailCanyon2_ChallengeHard,
                "w11b10" => Shadow.LevelID.Biolizard,
                "w11b11" => Shadow.LevelID.BiolizardHard,
                "w03a10" => Shadow.LevelID.KingdomValley1,
                "w03c11" => Shadow.LevelID.KingdomValley1_Challenge1,
                "w03c12" => Shadow.LevelID.KingdomValley1_Challenge2,
                "w03h11" => Shadow.LevelID.KingdomValley1_ChallengeHard,
                "w03a20" => Shadow.LevelID.KingdomValley2,
                "w03c21" => Shadow.LevelID.KingdomValley2_Challenge1,
                "w03c22" => Shadow.LevelID.KingdomValley2_Challenge2,
                "w03h21" => Shadow.LevelID.KingdomValley2_ChallengeHard,
                "w04a10" => Shadow.LevelID.SunsetHeights1,
                "w04c11" => Shadow.LevelID.SunsetHeights1_Challenge1,
                "w04c12" => Shadow.LevelID.SunsetHeights1_Challenge2,
                "w04h11" => Shadow.LevelID.SunsetHeights1_ChallengeHard,
                "w04a20" => Shadow.LevelID.SunsetHeights2,
                "w04c21" => Shadow.LevelID.SunsetHeights2_Challenge1,
                "w04c22" => Shadow.LevelID.SunsetHeights2_Challenge2,
                "w04h21" => Shadow.LevelID.SunsetHeights2_ChallengeHard,
                "w12b10" => Shadow.LevelID.MetalOverlord,
                "w12b11" => Shadow.LevelID.MetalOverlordHard,
                "w05a10" => Shadow.LevelID.ChaosIsland1,
                "w05c11" => Shadow.LevelID.ChaosIsland1_Challenge1,
                "w05c12" => Shadow.LevelID.ChaosIsland1_Challenge2,
                "w05h11" => Shadow.LevelID.ChaosIsland1_ChallengeHard,
                "w05a20" => Shadow.LevelID.ChaosIsland2,
                "w05c21" => Shadow.LevelID.ChaosIsland2_Challenge1,
                "w05c22" => Shadow.LevelID.ChaosIsland2_Challenge2,
                "w05h21" => Shadow.LevelID.ChaosIsland2_ChallengeHard,
                "w13b10" => Shadow.LevelID.Mephiles,
                "w13b11" => Shadow.LevelID.MephilesHard,
                "w06a10" => Shadow.LevelID.RadicalHighway1,
                "w06a20" => Shadow.LevelID.RadicalHighway2,
                "w14b10" => Shadow.LevelID.BlackDoom,
                _ => Shadow.LevelID.Unknown,
            };
        });

        IsInFinalQTE = new LazyWatcher<bool>(StateTracker, false, (_, _) =>
        {
            // Check if the player is in the final QTE based on the current level and events
            return LevelID.Current == Shadow.LevelID.BlackDoom
                && Engine.GetObject("EventQTEInput", out IntPtr _)
                && Engine.GetObject("BossPerfectBlackDoomFinal", out IntPtr _);
        });

        FinalQTECount = new LazyWatcher<int>(StateTracker, 0, (current, _) =>
        {
            if (LevelID.Current != Shadow.LevelID.BlackDoom || current == 2)
                return 0;

            if (IsInFinalQTE.Old && !IsInFinalQTE.Current)
                return current + 1;

            return current;
        });
    }

    /// <summary>
    /// Updates the memory state of the game
    /// </summary>
    /// <param name="process">The current <see cref="ProcessMemory"/> instance.</param>
    internal override void Update(ProcessMemory process, Settings settings)
    {
        ApplyHWNDpatch(process, settings);  // Applies or removes patches
        StateTracker.Tick();                // Update the state tracker
        Engine.Update(process);             // Update the engine with the current process memory
    }

    /// <summary>
    /// Applies or removes the game patch to control focus behavior.
    /// </summary>
    /// <param name="process">The current process memory instance.</param>
    /// <param name="settings">The settings specifying whether to apply the patch.</param>
    private void ApplyHWNDpatch(ProcessMemory process, Settings settings)
    {
        IntPtr address = Engine.hWndAddress;
        bool setting = settings.ShadowFocusPatch;

        if (process.Read<byte>(address, out byte val))
        {
            // If the game is unpatched and we want the patch to be applied
            if (val == 0x75 && setting)
                process.Write<byte>(Engine.hWndAddress, 0xEB); // Apply patch

            // If the game is patched and we want the patch to be removed
            else if (val == 0xEB && !setting)
                process.Write<byte>(Engine.hWndAddress, 0x75); // Remove patch
        }
    }

    /// <summary>
    /// Determines if the game is currently loading.
    /// </summary>
    /// <returns>True if the game is loading; otherwise, false.</returns>
    internal override bool? IsLoading(Settings settings)
    {
        return settings.ShadowLoadless && Is_Loading.Current;
    }

    /// <summary>
    /// Determines if the game has transitioned from the title screen to the opening cutscene.
    /// </summary>
    /// <returns>True if the autosplitter timer should be started; otherwise, false.</returns>
    internal override bool Start(Settings settings)
    {
        return settings.ShadowStart && GameMode.Old == "GameModeTitle" && GameMode.Current == "GameModeOpening";
    }

    /// <summary>
    /// Determines if a split should occur based on game conditions.
    /// </summary>
    /// <returns>True if a split should occur; otherwise, false.</returns>
    internal override bool Split(Settings settings)
    {
        if (LevelID.Old == Shadow.LevelID.BlackDoom)
            return settings.BlackDoom && FinalQTECount.Old == 2 && FinalQTECount.Current == 0;

        if ((HsmStatus.Old[1] == "ChallengeResult" || HsmStatus.Old[1] == "Result") && HsmStatus.Current[1] == "Build")
        {
            return LevelID.Old switch
            {
                Shadow.LevelID.SpaceColonyArk1 => settings.SpacecolonyArk1,
                Shadow.LevelID.SpaceColonyArk1_Challenge1 => settings.SpacecolonyArk1_1,
                Shadow.LevelID.SpaceColonyArk1_Challenge2 => settings.SpacecolonyArk1_2,
                Shadow.LevelID.SpaceColonyArk1_ChallengeHard => settings.SpacecolonyArk1_Hard,
                Shadow.LevelID.SpaceColonyArk2 => settings.SpacecolonyArk2,
                Shadow.LevelID.SpaceColonyArk2_Challenge1 => settings.SpacecolonyArk2_1,
                Shadow.LevelID.SpaceColonyArk2_Challenge2 => settings.SpacecolonyArk2_2,
                Shadow.LevelID.SpaceColonyArk2_ChallengeHard => settings.SpacecolonyArk2_Hard,
                Shadow.LevelID.RailCanyon1 => settings.RailCanyon1,
                Shadow.LevelID.RailCanyon1_Challenge1 => settings.RailCanyon1_1,
                Shadow.LevelID.RailCanyon1_Challenge2 => settings.RailCanyon1_2,
                Shadow.LevelID.RailCanyon1_ChallengeHard => settings.RailCanyon1_Hard,
                Shadow.LevelID.RailCanyon2 => settings.RailCanyon2,
                Shadow.LevelID.RailCanyon2_Challenge1 => settings.RailCanyon2_1,
                Shadow.LevelID.RailCanyon2_Challenge2 => settings.RailCanyon2_2,
                Shadow.LevelID.RailCanyon2_ChallengeHard => settings.RailCanyon2_Hard,
                Shadow.LevelID.KingdomValley1 => settings.KingdomValley1,
                Shadow.LevelID.KingdomValley1_Challenge1 => settings.KingdomValley1_1,
                Shadow.LevelID.KingdomValley1_Challenge2 => settings.KingdomValley1_2,
                Shadow.LevelID.KingdomValley1_ChallengeHard => settings.KingdomValley1_Hard,
                Shadow.LevelID.KingdomValley2 => settings.KingdomValley2,
                Shadow.LevelID.KingdomValley2_Challenge1 => settings.KingdomValley2_1,
                Shadow.LevelID.KingdomValley2_Challenge2 => settings.KingdomValley2_2,
                Shadow.LevelID.KingdomValley2_ChallengeHard => settings.KingdomValley2_Hard,
                Shadow.LevelID.SunsetHeights1 => settings.SunsetHeights1,
                Shadow.LevelID.SunsetHeights1_Challenge1 => settings.SunsetHeights1_1,
                Shadow.LevelID.SunsetHeights1_Challenge2 => settings.SunsetHeights1_2,
                Shadow.LevelID.SunsetHeights1_ChallengeHard => settings.SunsetHeights1_Hard,
                Shadow.LevelID.SunsetHeights2 => settings.SunsetHeights2,
                Shadow.LevelID.SunsetHeights2_Challenge1 => settings.SunsetHeights2_1,
                Shadow.LevelID.SunsetHeights2_Challenge2 => settings.SunsetHeights2_2,
                Shadow.LevelID.SunsetHeights2_ChallengeHard => settings.SunsetHeights2_Hard,
                Shadow.LevelID.ChaosIsland1 => settings.ChaosIsland1,
                Shadow.LevelID.ChaosIsland1_Challenge1 => settings.ChaosIsland1_1,
                Shadow.LevelID.ChaosIsland1_Challenge2 => settings.ChaosIsland1_2,
                Shadow.LevelID.ChaosIsland1_ChallengeHard => settings.ChaosIsland1_Hard,
                Shadow.LevelID.ChaosIsland2 => settings.ChaosIsland2,
                Shadow.LevelID.ChaosIsland2_Challenge1 => settings.ChaosIsland2_1,
                Shadow.LevelID.ChaosIsland2_Challenge2 => settings.ChaosIsland2_2,
                Shadow.LevelID.ChaosIsland2_ChallengeHard => settings.ChaosIsland2_Hard,
                Shadow.LevelID.RadicalHighway1 => settings.RadicalHighway1,
                Shadow.LevelID.RadicalHighway2 => settings.RadicalHighway2,
                Shadow.LevelID.Biolizard => settings.Biolizard,
                Shadow.LevelID.BiolizardHard => settings.Biolizard_Hard,
                Shadow.LevelID.MetalOverlord => settings.MetalOverlord,
                Shadow.LevelID.MetalOverlordHard => settings.MetalOverlord_Hard,
                Shadow.LevelID.Mephiles => settings.Mephiles,
                Shadow.LevelID.MephilesHard => settings.Mephiles_Hard,
                _ => false,
            };
        }
        return false;
    }

    /// <summary>
    /// Determines if the autosplitter timer should reset.
    /// </summary>
    /// <returns>True if the autospltter should reset; otherwise, false.</returns>
    internal override bool Reset(Settings settings)
    {
        return settings.ShadowReset && GameMode.Old == "GameModeTitle" && GameMode.Current == "GameModeOpening";
    }

    /// <summary>
    /// Sets the autosplitter's game time.
    /// </summary>
    /// <returns>The game time as a nullable TimeSpan.</returns>
    internal override TimeSpan? GameTime(Settings settings) => null;
}
