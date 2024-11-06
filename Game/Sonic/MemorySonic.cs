using System;
using System.Linq;
using JHelper.Common.MemoryUtils;
using JHelper.Common.ProcessInterop;

namespace LiveSplit.SonicXShadowGenerations.Game.Sonic;

/// <summary>
/// Represents the memory management and state tracking for Sonic Generations.
/// Inherits from the abstract Memory class to provide game state data.
/// </summary>
internal class MemorySonic : Memory
{
    /// <summary>
    /// The version of the game.
    /// </summary>
    public GameVersion Version { get; }

    /// <summary>
    /// The base memory address used to monitor the loading state.
    /// </summary>
    private readonly IntPtr baseLoading;

    /// <summary>
    /// The base memory address used to track the current level information.
    /// </summary>
    private readonly IntPtr baseLevel;

    /// <summary>
    /// Watcher for monitoring the game's loading state.
    /// </summary>
    public LazyWatcher<bool> Is_Loading { get; }

    /// <summary>
    /// Watcher for monitoring the current level ID of the game.
    /// </summary>
    public LazyWatcher<LevelID> LevelID { get; }

    /// <summary>
    /// Watcher for monitoring the level completion state.
    /// </summary>
    public LazyWatcher<bool> LevelCompletion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySonic"/> class and sets up memory watchers.
    /// </summary>
    /// <param name="process">The process memory instance for Sonic Generations.</param>
    /// <exception cref="Exception">Thrown if memory addresses cannot be found for tracking.</exception>
    public MemorySonic(ProcessMemory process)
        : base()
    {
        // Set the version based on the process's main module memory size
        Version = process.MainModule.ModuleMemorySize switch
        {
            0x18182000 => GameVersion.v1_1_0_0,
            0x16951000 => GameVersion.v1_1_0_1,
            _  => GameVersion.Unknown,
        };

        // Find the base address for loading state in memory
        baseLoading = process.MainModule.ScanAll(new MemoryScanPattern(3, "48 8B 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 45 AF") { OnFound = (addr) => addr + 0x4 + process.Read<int>(addr) }).First();

        // Find the base address for level data in memory.
        IntPtr level = process
            .MainModule
            .ScanAll(new MemoryScanPattern(3, "48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 8B 0D"))
            .First(addr =>
            {
                IntPtr ptr = addr + 12;
                ptr = ptr + 0x4 + process.Read<int>(ptr);
                return process.ReadString(ptr, 10) == "Trial_Ver1";
            });
        baseLevel = level + 0x4 + process.Read<int>(level);

        // Initialize watchers for game state tracking.

        LevelID = new LazyWatcher<LevelID>(StateTracker, Sonic.LevelID.GreenHill_Act1, (current, _) =>
        {
            Span<byte> lvl = stackalloc byte[2];

            // Retrieve current level ID by reading from memory.
            if (!process.ReadPointer(baseLevel, out IntPtr ptr)
                || !process.ReadPointer(ptr + 0xAC, out ptr)
                || !process.ReadArray(ptr + 0xC8, lvl))
                return current;

            // Map level data to corresponding level IDs.
            return lvl[0] switch
            {
                0 => lvl[1] switch
                {
                    1 => Sonic.LevelID.GreenHill_Act1_Challenge1,
                    2 => Sonic.LevelID.GreenHill_Act1_Challenge2,
                    3 => Sonic.LevelID.GreenHill_Act1_Challenge3,
                    4 => Sonic.LevelID.GreenHill_Act1_Challenge4,
                    5 => Sonic.LevelID.GreenHill_Act1_Challenge5,
                    _ => Sonic.LevelID.GreenHill_Act1
                },
                1 => lvl[1] switch
                {
                    1 => Sonic.LevelID.GreenHill_Act2_Challenge1,
                    2 => Sonic.LevelID.GreenHill_Act2_Challenge2,
                    3 => Sonic.LevelID.GreenHill_Act2_Challenge3,
                    4 => Sonic.LevelID.GreenHill_Act2_Challenge4,
                    5 => Sonic.LevelID.GreenHill_Act2_Challenge5,
                    _ => Sonic.LevelID.GreenHill_Act2
                },
                2 => lvl[1] switch
                {
                    1 => Sonic.LevelID.ChemicalPlant_Act1_Challenge1,
                    2 => Sonic.LevelID.ChemicalPlant_Act1_Challenge2,
                    3 => Sonic.LevelID.ChemicalPlant_Act1_Challenge3,
                    4 => Sonic.LevelID.ChemicalPlant_Act1_Challenge4,
                    5 => Sonic.LevelID.ChemicalPlant_Act1_Challenge5,
                    _ => Sonic.LevelID.ChemicalPlant_Act1
                },
                3 => lvl[1] switch
                {
                    1 => Sonic.LevelID.ChemicalPlant_Act2_Challenge1,
                    2 => Sonic.LevelID.ChemicalPlant_Act2_Challenge2,
                    3 => Sonic.LevelID.ChemicalPlant_Act2_Challenge3,
                    4 => Sonic.LevelID.ChemicalPlant_Act2_Challenge4,
                    5 => Sonic.LevelID.ChemicalPlant_Act2_Challenge5,
                    _ => Sonic.LevelID.ChemicalPlant_Act2
                },
                4 => lvl[1] switch
                {
                    1 => Sonic.LevelID.SkySanctuary_Act1_Challenge1,
                    2 => Sonic.LevelID.SkySanctuary_Act1_Challenge2,
                    3 => Sonic.LevelID.SkySanctuary_Act1_Challenge3,
                    4 => Sonic.LevelID.SkySanctuary_Act1_Challenge4,
                    5 => Sonic.LevelID.SkySanctuary_Act1_Challenge5,
                    _ => Sonic.LevelID.SkySanctuary_Act1
                },
                5 => lvl[1] switch
                {
                    1 => Sonic.LevelID.SkySanctuary_Act2_Challenge1,
                    2 => Sonic.LevelID.SkySanctuary_Act2_Challenge2,
                    3 => Sonic.LevelID.SkySanctuary_Act2_Challenge3,
                    4 => Sonic.LevelID.SkySanctuary_Act2_Challenge4,
                    5 => Sonic.LevelID.SkySanctuary_Act2_Challenge5,
                    _ => Sonic.LevelID.SkySanctuary_Act2
                },
                6 => lvl[1] switch
                {
                    1 => Sonic.LevelID.SpeedHighway_Act1_Challenge1,
                    2 => Sonic.LevelID.SpeedHighway_Act1_Challenge2,
                    3 => Sonic.LevelID.SpeedHighway_Act1_Challenge3,
                    4 => Sonic.LevelID.SpeedHighway_Act1_Challenge4,
                    5 => Sonic.LevelID.SpeedHighway_Act1_Challenge5,
                    _ => Sonic.LevelID.SpeedHighway_Act1,
                },
                7 => lvl[1] switch
                {
                    1 => Sonic.LevelID.SpeedHighway_Act2_Challenge1,
                    2 => Sonic.LevelID.SpeedHighway_Act2_Challenge2,
                    3 => Sonic.LevelID.SpeedHighway_Act2_Challenge3,
                    4 => Sonic.LevelID.SpeedHighway_Act2_Challenge4,
                    5 => Sonic.LevelID.SpeedHighway_Act2_Challenge5,
                    _ => Sonic.LevelID.SpeedHighway_Act2,
                },
                8 => lvl[1] switch
                {
                    1 => Sonic.LevelID.CityEscape_Act1_Challenge1,
                    2 => Sonic.LevelID.CityEscape_Act1_Challenge2,
                    3 => Sonic.LevelID.CityEscape_Act1_Challenge3,
                    4 => Sonic.LevelID.CityEscape_Act1_Challenge4,
                    5 => Sonic.LevelID.CityEscape_Act1_Challenge5,
                    _ => Sonic.LevelID.CityEscape_Act1,
                },
                9 => lvl[1] switch
                {
                    1 => Sonic.LevelID.CityEscape_Act2_Challenge1,
                    2 => Sonic.LevelID.CityEscape_Act2_Challenge2,
                    3 => Sonic.LevelID.CityEscape_Act2_Challenge3,
                    4 => Sonic.LevelID.CityEscape_Act2_Challenge4,
                    5 => Sonic.LevelID.CityEscape_Act2_Challenge5,
                    _ => Sonic.LevelID.CityEscape_Act2,
                },
                10 => lvl[1] switch
                {
                    1 => Sonic.LevelID.SeasideHill_Act1_Challenge1,
                    2 => Sonic.LevelID.SeasideHill_Act1_Challenge2,
                    3 => Sonic.LevelID.SeasideHill_Act1_Challenge3,
                    4 => Sonic.LevelID.SeasideHill_Act1_Challenge4,
                    5 => Sonic.LevelID.SeasideHill_Act1_Challenge5,
                    _ => Sonic.LevelID.SeasideHill_Act1,
                },
                11 => lvl[1] switch
                {
                    1 => Sonic.LevelID.SeasideHill_Act2_Challenge1,
                    2 => Sonic.LevelID.SeasideHill_Act2_Challenge2,
                    3 => Sonic.LevelID.SeasideHill_Act2_Challenge3,
                    4 => Sonic.LevelID.SeasideHill_Act2_Challenge4,
                    5 => Sonic.LevelID.SeasideHill_Act2_Challenge5,
                    _ => Sonic.LevelID.SeasideHill_Act2,
                },
                12 => lvl[1] switch
                {
                    1 => Sonic.LevelID.CrisisCity_Act1_Challenge1,
                    2 => Sonic.LevelID.CrisisCity_Act1_Challenge2,
                    3 => Sonic.LevelID.CrisisCity_Act1_Challenge3,
                    4 => Sonic.LevelID.CrisisCity_Act1_Challenge4,
                    5 => Sonic.LevelID.CrisisCity_Act1_Challenge5,
                    _ => Sonic.LevelID.CrisisCity_Act1,
                },
                13 => lvl[1] switch
                {
                    1 => Sonic.LevelID.CrisisCity_Act2_Challenge1,
                    2 => Sonic.LevelID.CrisisCity_Act2_Challenge2,
                    3 => Sonic.LevelID.CrisisCity_Act2_Challenge3,
                    4 => Sonic.LevelID.CrisisCity_Act2_Challenge4,
                    5 => Sonic.LevelID.CrisisCity_Act2_Challenge5,
                    _ => Sonic.LevelID.CrisisCity_Act2,
                },
                14 => lvl[1] switch
                {
                    1 => Sonic.LevelID.RooftopRun_Act1_Challenge1,
                    2 => Sonic.LevelID.RooftopRun_Act1_Challenge2,
                    3 => Sonic.LevelID.RooftopRun_Act1_Challenge3,
                    4 => Sonic.LevelID.RooftopRun_Act1_Challenge4,
                    5 => Sonic.LevelID.RooftopRun_Act1_Challenge5,
                    _ => Sonic.LevelID.RooftopRun_Act1,
                },
                15 => lvl[1] switch
                {
                    1 => Sonic.LevelID.RooftopRun_Act2_Challenge1,
                    2 => Sonic.LevelID.RooftopRun_Act2_Challenge2,
                    3 => Sonic.LevelID.RooftopRun_Act2_Challenge3,
                    4 => Sonic.LevelID.RooftopRun_Act2_Challenge4,
                    5 => Sonic.LevelID.RooftopRun_Act2_Challenge5,
                    _ => Sonic.LevelID.RooftopRun_Act2,
                },
                16 => lvl[1] switch
                {
                    1 => Sonic.LevelID.PlanetWisp_Act1_Challenge1,
                    2 => Sonic.LevelID.PlanetWisp_Act1_Challenge2,
                    3 => Sonic.LevelID.PlanetWisp_Act1_Challenge3,
                    4 => Sonic.LevelID.PlanetWisp_Act1_Challenge4,
                    5 => Sonic.LevelID.PlanetWisp_Act1_Challenge5,
                    _ => Sonic.LevelID.PlanetWisp_Act1,
                },
                17 => lvl[1] switch
                {
                    1 => Sonic.LevelID.PlanetWisp_Act2_Challenge1,
                    2 => Sonic.LevelID.PlanetWisp_Act2_Challenge2,
                    3 => Sonic.LevelID.PlanetWisp_Act2_Challenge3,
                    4 => Sonic.LevelID.PlanetWisp_Act2_Challenge4,
                    5 => Sonic.LevelID.PlanetWisp_Act2_Challenge5,
                    _ => Sonic.LevelID.PlanetWisp_Act2,
                },
                20 => Sonic.LevelID.VsMetalSonic,
                21 => Sonic.LevelID.VsShadow,
                22 => Sonic.LevelID.VsSilver,
                23 => Sonic.LevelID.DeathEgg,
                24 => Sonic.LevelID.PerfectChaos,
                25 => Sonic.LevelID.EggDragoon,
                26 => Sonic.LevelID.TimeEater,
                27 => Sonic.LevelID.WhiteWorld,
                32 => Sonic.LevelID.MainMenu, // Also used for Time Eater cutscene
                _ => current,
            };
        });

        Is_Loading = new LazyWatcher<bool>(StateTracker, false, (current, _) =>
        {
            // Check if the main menu is active.
            if (LevelID.Current == Sonic.LevelID.MainMenu)
                return false;

            // Read loading state from memory.
            if (!process.ReadPointer(baseLoading, out IntPtr ptr)
                || !process.Read(ptr + 0xC, out byte val))
                return current;

            // Return true if loading byte is non-zero.
            return val != 0;
        });

        LevelCompletion = new LazyWatcher<bool>(StateTracker, false, (current, _) =>
        {
            if (!process.ReadPointer(baseLevel, out IntPtr ptr)
                || !process.ReadPointer(ptr + 0xAC, out ptr)
                || !process.Read(ptr + 0xC4, out byte val))
                return false;

            // Return true if level is marked as completed.
            return val == 2;
        });
    }

    /// <summary>
    /// Updates the current state of the memory trackers each game tick.
    /// </summary>
    /// <param name="process">The process memory instance.</param>
    /// <param name="settings">The settings controlling level tracking and splitting.</param>
    internal override void Update(ProcessMemory process, Settings settings)
    {
        StateTracker.Tick();
    }

    /// <summary>
    /// Determines if the run has started, initializing tracking based on settings.
    /// </summary>
    /// <param name="settings">The settings used to initialize tracking.</param>
    /// <returns>True if the autosplitter timer should be started; otherwise, false.</returns>
    internal override bool Start(Settings settings)
    {
        return settings.SonicStart && LevelID.Old == Sonic.LevelID.MainMenu && LevelID.Current == Sonic.LevelID.GreenHill_Act1;
    }

    /// <summary>
    /// Determines if a split should occur based on the level ID and completion state.
    /// </summary>
    /// <param name="settings">The settings controlling level-specific splits.</param>
    /// <returns>True if a split should occur; otherwise, false.</returns>
    internal override bool Split(Settings settings)
    {
        // Special split condition for the Time Eater level.
        if (LevelID.Old == Sonic.LevelID.TimeEater)
            return settings.TimeEater && !LevelCompletion.Old && LevelCompletion.Current;

        // Regular split conditions based on completion of specific levels.
        return !LevelCompletion.Current && LevelCompletion.Old && (LevelID.Old switch
        {
            Sonic.LevelID.GreenHill_Act1 => settings.GreenHill1,
            Sonic.LevelID.GreenHill_Act1_Challenge1 => settings.GreenHill1_1,
            Sonic.LevelID.GreenHill_Act1_Challenge2 => settings.GreenHill1_2,
            Sonic.LevelID.GreenHill_Act1_Challenge3 => settings.GreenHill1_3,
            Sonic.LevelID.GreenHill_Act1_Challenge4 => settings.GreenHill1_4,
            Sonic.LevelID.GreenHill_Act1_Challenge5 => settings.GreenHill1_5,
            Sonic.LevelID.GreenHill_Act2 => settings.GreenHill2,
            Sonic.LevelID.GreenHill_Act2_Challenge1 => settings.GreenHill2_1,
            Sonic.LevelID.GreenHill_Act2_Challenge2 => settings.GreenHill2_2,
            Sonic.LevelID.GreenHill_Act2_Challenge3 => settings.GreenHill2_3,
            Sonic.LevelID.GreenHill_Act2_Challenge4 => settings.GreenHill2_4,
            Sonic.LevelID.GreenHill_Act2_Challenge5 => settings.GreenHill2_5,
            Sonic.LevelID.ChemicalPlant_Act1 => settings.ChemicalPlant1,
            Sonic.LevelID.ChemicalPlant_Act1_Challenge1 => settings.ChemicalPlant1_1,
            Sonic.LevelID.ChemicalPlant_Act1_Challenge2 => settings.ChemicalPlant1_2,
            Sonic.LevelID.ChemicalPlant_Act1_Challenge3 => settings.ChemicalPlant1_3,
            Sonic.LevelID.ChemicalPlant_Act1_Challenge4 => settings.ChemicalPlant1_4,
            Sonic.LevelID.ChemicalPlant_Act1_Challenge5 => settings.ChemicalPlant1_5,
            Sonic.LevelID.ChemicalPlant_Act2 => settings.ChemicalPlant2,
            Sonic.LevelID.ChemicalPlant_Act2_Challenge1 => settings.ChemicalPlant2_1,
            Sonic.LevelID.ChemicalPlant_Act2_Challenge2 => settings.ChemicalPlant2_2,
            Sonic.LevelID.ChemicalPlant_Act2_Challenge3 => settings.ChemicalPlant2_3,
            Sonic.LevelID.ChemicalPlant_Act2_Challenge4 => settings.ChemicalPlant2_4,
            Sonic.LevelID.ChemicalPlant_Act2_Challenge5 => settings.ChemicalPlant2_5,
            Sonic.LevelID.SkySanctuary_Act1 => settings.SkySanctuary1,
            Sonic.LevelID.SkySanctuary_Act1_Challenge1 => settings.SkySanctuary1_1,
            Sonic.LevelID.SkySanctuary_Act1_Challenge2 => settings.SkySanctuary1_2,
            Sonic.LevelID.SkySanctuary_Act1_Challenge3 => settings.SkySanctuary1_3,
            Sonic.LevelID.SkySanctuary_Act1_Challenge4 => settings.SkySanctuary1_4,
            Sonic.LevelID.SkySanctuary_Act1_Challenge5 => settings.SkySanctuary1_5,
            Sonic.LevelID.SkySanctuary_Act2 => settings.SkySanctuary2,
            Sonic.LevelID.SkySanctuary_Act2_Challenge1 => settings.SkySanctuary2_1,
            Sonic.LevelID.SkySanctuary_Act2_Challenge2 => settings.SkySanctuary2_2,
            Sonic.LevelID.SkySanctuary_Act2_Challenge3 => settings.SkySanctuary2_3,
            Sonic.LevelID.SkySanctuary_Act2_Challenge4 => settings.SkySanctuary2_4,
            Sonic.LevelID.SkySanctuary_Act2_Challenge5 => settings.SkySanctuary2_5,
            Sonic.LevelID.VsMetalSonic => settings.MetalSonic,
            Sonic.LevelID.DeathEgg => settings.DeathEgg,
            Sonic.LevelID.SpeedHighway_Act1 => settings.SpeedHighway1,
            Sonic.LevelID.SpeedHighway_Act1_Challenge1 => settings.SpeedHighway1_1,
            Sonic.LevelID.SpeedHighway_Act1_Challenge2 => settings.SpeedHighway1_2,
            Sonic.LevelID.SpeedHighway_Act1_Challenge3 => settings.SpeedHighway1_3,
            Sonic.LevelID.SpeedHighway_Act1_Challenge4 => settings.SpeedHighway1_4,
            Sonic.LevelID.SpeedHighway_Act1_Challenge5 => settings.SpeedHighway1_5,
            Sonic.LevelID.SpeedHighway_Act2 => settings.SpeedHighway2,
            Sonic.LevelID.SpeedHighway_Act2_Challenge1 => settings.SpeedHighway2_1,
            Sonic.LevelID.SpeedHighway_Act2_Challenge2 => settings.SpeedHighway2_2,
            Sonic.LevelID.SpeedHighway_Act2_Challenge3 => settings.SpeedHighway2_3,
            Sonic.LevelID.SpeedHighway_Act2_Challenge4 => settings.SpeedHighway2_4,
            Sonic.LevelID.SpeedHighway_Act2_Challenge5 => settings.SpeedHighway2_5,
            Sonic.LevelID.CityEscape_Act1 => settings.CityEscape1,
            Sonic.LevelID.CityEscape_Act1_Challenge1 => settings.CityEscape1_1,
            Sonic.LevelID.CityEscape_Act1_Challenge2 => settings.CityEscape1_2,
            Sonic.LevelID.CityEscape_Act1_Challenge3 => settings.CityEscape1_3,
            Sonic.LevelID.CityEscape_Act1_Challenge4 => settings.CityEscape1_4,
            Sonic.LevelID.CityEscape_Act1_Challenge5 => settings.CityEscape1_5,
            Sonic.LevelID.CityEscape_Act2 => settings.CityEscape2,
            Sonic.LevelID.CityEscape_Act2_Challenge1 => settings.CityEscape2_1,
            Sonic.LevelID.CityEscape_Act2_Challenge2 => settings.CityEscape2_2,
            Sonic.LevelID.CityEscape_Act2_Challenge3 => settings.CityEscape2_3,
            Sonic.LevelID.CityEscape_Act2_Challenge4 => settings.CityEscape2_4,
            Sonic.LevelID.CityEscape_Act2_Challenge5 => settings.CityEscape2_5,
            Sonic.LevelID.SeasideHill_Act1 => settings.SeasideHill1,
            Sonic.LevelID.SeasideHill_Act1_Challenge1 => settings.SeasideHill1_1,
            Sonic.LevelID.SeasideHill_Act1_Challenge2 => settings.SeasideHill1_2,
            Sonic.LevelID.SeasideHill_Act1_Challenge3 => settings.SeasideHill1_3,
            Sonic.LevelID.SeasideHill_Act1_Challenge4 => settings.SeasideHill1_4,
            Sonic.LevelID.SeasideHill_Act1_Challenge5 => settings.SeasideHill1_5,
            Sonic.LevelID.SeasideHill_Act2 => settings.SeasideHill2,
            Sonic.LevelID.SeasideHill_Act2_Challenge1 => settings.SeasideHill2_1,
            Sonic.LevelID.SeasideHill_Act2_Challenge2 => settings.SeasideHill2_2,
            Sonic.LevelID.SeasideHill_Act2_Challenge3 => settings.SeasideHill2_3,
            Sonic.LevelID.SeasideHill_Act2_Challenge4 => settings.SeasideHill2_4,
            Sonic.LevelID.SeasideHill_Act2_Challenge5 => settings.SeasideHill2_5,
            Sonic.LevelID.VsShadow => settings.Shadow,
            Sonic.LevelID.PerfectChaos => settings.PerfectChaos,
            Sonic.LevelID.CrisisCity_Act1 => settings.CrisisCity1,
            Sonic.LevelID.CrisisCity_Act1_Challenge1 => settings.CrisisCity1_1,
            Sonic.LevelID.CrisisCity_Act1_Challenge2 => settings.CrisisCity1_2,
            Sonic.LevelID.CrisisCity_Act1_Challenge3 => settings.CrisisCity1_3,
            Sonic.LevelID.CrisisCity_Act1_Challenge4 => settings.CrisisCity1_4,
            Sonic.LevelID.CrisisCity_Act1_Challenge5 => settings.CrisisCity1_5,
            Sonic.LevelID.CrisisCity_Act2 => settings.CrisisCity1,
            Sonic.LevelID.CrisisCity_Act2_Challenge1 => settings.CrisisCity2_1,
            Sonic.LevelID.CrisisCity_Act2_Challenge2 => settings.CrisisCity2_2,
            Sonic.LevelID.CrisisCity_Act2_Challenge3 => settings.CrisisCity2_3,
            Sonic.LevelID.CrisisCity_Act2_Challenge4 => settings.CrisisCity2_4,
            Sonic.LevelID.CrisisCity_Act2_Challenge5 => settings.CrisisCity2_5,
            Sonic.LevelID.RooftopRun_Act1 => settings.RooftopRun1,
            Sonic.LevelID.RooftopRun_Act1_Challenge1 => settings.RooftopRun1_1,
            Sonic.LevelID.RooftopRun_Act1_Challenge2 => settings.RooftopRun1_2,
            Sonic.LevelID.RooftopRun_Act1_Challenge3 => settings.RooftopRun1_3,
            Sonic.LevelID.RooftopRun_Act1_Challenge4 => settings.RooftopRun1_4,
            Sonic.LevelID.RooftopRun_Act1_Challenge5 => settings.RooftopRun1_5,
            Sonic.LevelID.RooftopRun_Act2 => settings.RooftopRun2,
            Sonic.LevelID.RooftopRun_Act2_Challenge1 => settings.RooftopRun2_1,
            Sonic.LevelID.RooftopRun_Act2_Challenge2 => settings.RooftopRun2_2,
            Sonic.LevelID.RooftopRun_Act2_Challenge3 => settings.RooftopRun2_3,
            Sonic.LevelID.RooftopRun_Act2_Challenge4 => settings.RooftopRun2_4,
            Sonic.LevelID.RooftopRun_Act2_Challenge5 => settings.RooftopRun2_5,
            Sonic.LevelID.PlanetWisp_Act1 => settings.PlanetWisp1,
            Sonic.LevelID.PlanetWisp_Act1_Challenge1 => settings.PlanetWisp1_1,
            Sonic.LevelID.PlanetWisp_Act1_Challenge2 => settings.PlanetWisp1_2,
            Sonic.LevelID.PlanetWisp_Act1_Challenge3 => settings.PlanetWisp1_3,
            Sonic.LevelID.PlanetWisp_Act1_Challenge4 => settings.PlanetWisp1_4,
            Sonic.LevelID.PlanetWisp_Act1_Challenge5 => settings.PlanetWisp1_5,
            Sonic.LevelID.PlanetWisp_Act2 => settings.PlanetWisp2,
            Sonic.LevelID.PlanetWisp_Act2_Challenge1 => settings.PlanetWisp2_1,
            Sonic.LevelID.PlanetWisp_Act2_Challenge2 => settings.PlanetWisp2_2,
            Sonic.LevelID.PlanetWisp_Act2_Challenge3 => settings.PlanetWisp2_3,
            Sonic.LevelID.PlanetWisp_Act2_Challenge4 => settings.PlanetWisp2_4,
            Sonic.LevelID.PlanetWisp_Act2_Challenge5 => settings.PlanetWisp2_5,
            Sonic.LevelID.EggDragoon => settings.EggDragoon,
            _ => false,
        });
    }

    /// <summary>
    /// Determines if the game is currently loading.
    /// </summary>
    /// <returns>True if the game is loading; otherwise, false.</returns>
    internal override bool? IsLoading(Settings settings) => settings.SonicLoadless && Is_Loading.Current;

    /// <summary>
    /// Determines if the autosplitter timer should reset.
    /// </summary>
    /// <returns>True if the autospltter should reset; otherwise, false.</returns>
    internal override bool Reset(Settings settings) => false;

    /// <summary>
    /// Sets the autosplitter's game time.
    /// </summary>
    /// <returns>The game time as a nullable TimeSpan.</returns>
    internal override TimeSpan? GameTime(Settings settings) => null;
}
