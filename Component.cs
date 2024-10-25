using System;
using System.Xml;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.SonicXShadowGenerations.Game;
using LiveSplit.Options;
using Helper.Common.ProcessInterop;
using LiveSplit.SonicXShadowGenerations.Game.Sonic;

namespace LiveSplit.SonicXShadowGenerations
{
    partial class AutosplitterComponent : LogicComponent
    {
        public override string ComponentName => "Sonic X Shadow Generations - Autosplitter";
        private Settings Settings { get; set; } = new Settings();

        private readonly Task autosplitterTask;
        private readonly CancellationTokenSource cancelToken = new CancellationTokenSource();

        public AutosplitterComponent(LiveSplitState state)
        {
            // The main autosplitter Task runs asynchronously 
            autosplitterTask = Task.Run(async () =>
            {
                try
                {
                    await AutosplitterTask(state, cancelToken.Token);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            },
            cancelToken.Token);
        }

        public override void Dispose()
        {
            cancelToken.Cancel();
            autosplitterTask.Wait();
            autosplitterTask?.Dispose();
            Settings.Dispose();
        }

        public override XmlNode GetSettings(XmlDocument document) { return this.Settings.GetSettings(document); }

        public override Control GetSettingsControl(LayoutMode mode) { return this.Settings; }

        public override void SetSettings(XmlNode settings) { this.Settings.SetSettings(settings); }

        private async Task AutosplitterTask(LiveSplitState state, CancellationToken canceltoken)
        {
            string[] gameProcessNames = ["SONIC_GENERATIONS.exe", "SONIC_X_SHADOW_GENERATIONS.exe"];

            // Update interval represents the amount of time between each update cycle. Tipically 16ms for 60hz refresh rate.
            // The default in LiveSplit One is 120hz so no reason to attempt this here as well.
            TimeSpan updateInterval = TimeSpan.FromMilliseconds(1000d / 120);

            // TimerModel, used by the autosplitter to signal the splitting actions.
            TimerModel timer = new TimerModel() { CurrentState = state };

            // Standard stopwatch used to keep track of the time spent for each update cycles.
            Stopwatch clock = new Stopwatch();

            while (!canceltoken.IsCancellationRequested)
            {
                // Hook to the target process
                ProcessMemory process = await ProcessHookAsync(gameProcessNames, canceltoken);

                // Perform memory scanning and look for the addresses we need.
                // As this proces can deliberately throw, it's wrapped in a try/catch block.
                // If memory scanning fails, the autosplitter will wait (to spare a bit of system resources) and retry again.
                IMemory? memory = null;
                while (!canceltoken.IsCancellationRequested && process.IsOpen)
                {
                    try
                    {
                        memory = process.ProcessName switch
                        {
                            "SONIC_GENERATIONS.exe" => new MemorySonic(process),
                            _ => new MemoryShadow(process),
                        };
                        
                        break;
                    }
                    catch
                    {
                        await Task.Delay(1000, canceltoken);
                    }
                }

                // If memory is still null here, we either requested cancellation of the task or the process is not open anymore
                if (memory is null)
                    continue;

                while (!canceltoken.IsCancellationRequested && process.IsOpen)
                {
                    if (memory is null)
                        break;

                    clock.Start();

                    // Splitting logic. Adapted from LiveSplit's source code:
                    // Order of execution:
                    // 1. update() will always be run first. There are no conditions on the execution of this action.
                    // 2. If the timer is currently either running or paused, then the isLoading, gameTime, and reset actions will be run.
                    // 3. If reset does not return true, then the split action will be run.
                    // 4. If the timer is currently not running (and not paused), then the start action will be run.
                    memory.Update(process);

                    // Main logic: the autosplitter checks for time, s and splitting conditions only if it's running
                    // This prevents, for example, automatic resetting when the run is already complete (TimerPhase.Ended)
                    if (timer.CurrentState.CurrentPhase == TimerPhase.Running || timer.CurrentState.CurrentPhase == TimerPhase.Paused)
                    {
                        bool? isLoading = memory.IsLoading(Settings);
                        if (isLoading is not null)
                            state.IsGameTimePaused = isLoading.Value;

                        TimeSpan? gameTime = memory.GameTime(Settings);
                        if (gameTime is not null)
                            timer.CurrentState.SetGameTime(gameTime.Value);

                        if (memory.Reset(Settings))
                            timer.Reset();
                        else if (memory.Split(Settings))
                            timer.Split();
                    }

                    // Start logic: for obvious reasons, this should be checked only if the timer has not started yet
                    if (timer.CurrentState.CurrentPhase == TimerPhase.NotRunning && memory.Start(Settings))
                    {
                        timer.Start();
                        state.IsGameTimePaused = true;

                        bool? isLoading = memory.IsLoading(Settings);
                        if (isLoading is not null)
                            state.IsGameTimePaused = isLoading.Value;
                    }

                    TimeSpan elapsedTicks = clock.Elapsed;
                    clock.Reset();

                    if (elapsedTicks < updateInterval)
                        canceltoken.WaitHandle.WaitOne(updateInterval - elapsedTicks);
                }

                process?.Dispose();
            }
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

        private async Task<ProcessMemory> ProcessHookAsync(string[] exeNames, CancellationToken token)
        {
            while(true)
            {
                try
                {
                    ProcessMemory? process = exeNames.Select(ProcessMemory.HookProcess).FirstOrDefault(p => p is not null);
                    
                    if (process is not null)
                        return process;
                }
                catch { }

                await Task.Delay(1500, token);
                token.ThrowIfCancellationRequested();
            }
        }
    }
}