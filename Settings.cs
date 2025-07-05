using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SonicXShadowGenerations;

public partial class Settings : UserControl
{
    public bool SonicStart { get; set; }
    public bool SonicLoadless { get; set; }
    public bool GreenHill1 { get; set; }
    public bool GreenHill1_1 { get; set; }
    public bool GreenHill1_2 { get; set; }
    public bool GreenHill1_3 { get; set; }
    public bool GreenHill1_4 { get; set; }
    public bool GreenHill1_5 { get; set; }
    public bool GreenHill2 { get; set; }
    public bool GreenHill2_1 { get; set; }
    public bool GreenHill2_2 { get; set; }
    public bool GreenHill2_3 { get; set; }
    public bool GreenHill2_4 { get; set; }
    public bool GreenHill2_5 { get; set; }
    public bool ChemicalPlant1 { get; set; }
    public bool ChemicalPlant1_1 { get; set; }
    public bool ChemicalPlant1_2 { get; set; }
    public bool ChemicalPlant1_3 { get; set; }
    public bool ChemicalPlant1_4 { get; set; }
    public bool ChemicalPlant1_5 { get; set; }
    public bool ChemicalPlant2 { get; set; }
    public bool ChemicalPlant2_1 { get; set; }
    public bool ChemicalPlant2_2 { get; set; }
    public bool ChemicalPlant2_3 { get; set; }
    public bool ChemicalPlant2_4 { get; set; }
    public bool ChemicalPlant2_5 { get; set; }
    public bool SkySanctuary1 { get; set; }
    public bool SkySanctuary1_1 { get; set; }
    public bool SkySanctuary1_2 { get; set; }
    public bool SkySanctuary1_3 { get; set; }
    public bool SkySanctuary1_4 { get; set; }
    public bool SkySanctuary1_5 { get; set; }
    public bool SkySanctuary2 { get; set; }
    public bool SkySanctuary2_1 { get; set; }
    public bool SkySanctuary2_2 { get; set; }
    public bool SkySanctuary2_3 { get; set; }
    public bool SkySanctuary2_4 { get; set; }
    public bool SkySanctuary2_5 { get; set; }
    public bool MetalSonic { get; set; }
    public bool DeathEgg { get; set; }
    public bool SpeedHighway1 { get; set; }
    public bool SpeedHighway1_1 { get; set; }
    public bool SpeedHighway1_2 { get; set; }
    public bool SpeedHighway1_3 { get; set; }
    public bool SpeedHighway1_4 { get; set; }
    public bool SpeedHighway1_5 { get; set; }
    public bool SpeedHighway2 { get; set; }
    public bool SpeedHighway2_1 { get; set; }
    public bool SpeedHighway2_2 { get; set; }
    public bool SpeedHighway2_3 { get; set; }
    public bool SpeedHighway2_4 { get; set; }
    public bool SpeedHighway2_5 { get; set; }
    public bool CityEscape1 { get; set; }
    public bool CityEscape1_1 { get; set; }
    public bool CityEscape1_2 { get; set; }
    public bool CityEscape1_3 { get; set; }
    public bool CityEscape1_4 { get; set; }
    public bool CityEscape1_5 { get; set; }
    public bool CityEscape2 { get; set; }
    public bool CityEscape2_1 { get; set; }
    public bool CityEscape2_2 { get; set; }
    public bool CityEscape2_3 { get; set; }
    public bool CityEscape2_4 { get; set; }
    public bool CityEscape2_5 { get; set; }
    public bool SeasideHill1 { get; set; }
    public bool SeasideHill1_1 { get; set; }
    public bool SeasideHill1_2 { get; set; }
    public bool SeasideHill1_3 { get; set; }
    public bool SeasideHill1_4 { get; set; }
    public bool SeasideHill1_5 { get; set; }
    public bool SeasideHill2 { get; set; }
    public bool SeasideHill2_1 { get; set; }
    public bool SeasideHill2_2 { get; set; }
    public bool SeasideHill2_3 { get; set; }
    public bool SeasideHill2_4 { get; set; }
    public bool SeasideHill2_5 { get; set; }
    public bool Shadow { get; set; }
    public bool PerfectChaos { get; set; }
    public bool CrisisCity1 { get; set; }
    public bool CrisisCity1_1 { get; set; }
    public bool CrisisCity1_2 { get; set; }
    public bool CrisisCity1_3 { get; set; }
    public bool CrisisCity1_4 { get; set; }
    public bool CrisisCity1_5 { get; set; }
    public bool CrisisCity2 { get; set; }
    public bool CrisisCity2_1 { get; set; }
    public bool CrisisCity2_2 { get; set; }
    public bool CrisisCity2_3 { get; set; }
    public bool CrisisCity2_4 { get; set; }
    public bool CrisisCity2_5 { get; set; }
    public bool RooftopRun1 { get; set; }
    public bool RooftopRun1_1 { get; set; }
    public bool RooftopRun1_2 { get; set; }
    public bool RooftopRun1_3 { get; set; }
    public bool RooftopRun1_4 { get; set; }
    public bool RooftopRun1_5 { get; set; }
    public bool RooftopRun2 { get; set; }
    public bool RooftopRun2_1 { get; set; }
    public bool RooftopRun2_2 { get; set; }
    public bool RooftopRun2_3 { get; set; }
    public bool RooftopRun2_4 { get; set; }
    public bool RooftopRun2_5 { get; set; }
    public bool PlanetWisp1 { get; set; }
    public bool PlanetWisp1_1 { get; set; }
    public bool PlanetWisp1_2 { get; set; }
    public bool PlanetWisp1_3 { get; set; }
    public bool PlanetWisp1_4 { get; set; }
    public bool PlanetWisp1_5 { get; set; }
    public bool PlanetWisp2 { get; set; }
    public bool PlanetWisp2_1 { get; set; }
    public bool PlanetWisp2_2 { get; set; }
    public bool PlanetWisp2_3 { get; set; }
    public bool PlanetWisp2_4 { get; set; }
    public bool PlanetWisp2_5 { get; set; }
    public bool Silver { get; set; }
    public bool EggDragoon { get; set; }
    public bool TimeEater { get; set; }
    public bool ShadowLoadless { get; set; }
    public bool ShadowNewGameStart { get; set; }
    public bool ShadowLevelEnterStart { get; set; }
    public bool ShadowReset { get; set; }
    public bool SpacecolonyArk1 { get; set; }
    public bool SpacecolonyArk1_1 { get; set; }
    public bool SpacecolonyArk1_2 { get; set; }
    public bool SpacecolonyArk1_Hard { get; set; }
    public bool SpacecolonyArk2 { get; set; }
    public bool SpacecolonyArk2_1 { get; set; }
    public bool SpacecolonyArk2_2 { get; set; }
    public bool SpacecolonyArk2_Hard { get; set; }
    public bool RailCanyon1 { get; set; }
    public bool RailCanyon1_1 { get; set; }
    public bool RailCanyon1_2 { get; set; }
    public bool RailCanyon1_Hard { get; set; }
    public bool RailCanyon2 { get; set; }
    public bool RailCanyon2_1 { get; set; }
    public bool RailCanyon2_2 { get; set; }
    public bool RailCanyon2_Hard { get; set; }
    public bool KingdomValley1 { get; set; }
    public bool KingdomValley1_1 { get; set; }
    public bool KingdomValley1_2 { get; set; }
    public bool KingdomValley1_Hard { get; set; }
    public bool KingdomValley2 { get; set; }
    public bool KingdomValley2_1 { get; set; }
    public bool KingdomValley2_2 { get; set; }
    public bool KingdomValley2_Hard { get; set; }
    public bool SunsetHeights1 { get; set; }
    public bool SunsetHeights1_1 { get; set; }
    public bool SunsetHeights1_2 { get; set; }
    public bool SunsetHeights1_Hard { get; set; }
    public bool SunsetHeights2 { get; set; }
    public bool SunsetHeights2_1 { get; set; }
    public bool SunsetHeights2_2 { get; set; }
    public bool SunsetHeights2_Hard { get; set; }
    public bool ChaosIsland1 { get; set; }
    public bool ChaosIsland1_1 { get; set; }
    public bool ChaosIsland1_2 { get; set; }
    public bool ChaosIsland1_Hard { get; set; }
    public bool ChaosIsland2 { get; set; }
    public bool ChaosIsland2_1 { get; set; }
    public bool ChaosIsland2_2 { get; set; }
    public bool ChaosIsland2_Hard { get; set; }
    public bool RadicalHighway1 { get; set; }
    public bool RadicalHighway2 { get; set; }
    public bool ShadowFocusPatch { get; set; }
    public bool Biolizard { get; set; }
    public bool Biolizard_Hard { get; set; }
    public bool MetalOverlord { get; set; }
    public bool MetalOverlord_Hard { get; set; }
    public bool Mephiles { get; set; }
    public bool Mephiles_Hard { get; set; }
    public bool BlackDoom { get; set; }
    public bool Tokio1 { get; set; }
    public bool Tokio1_1 { get; set; }
    public bool Tokio1_2 { get; set; }

    public Settings()
    {
        InitializeComponent();
        autosplitterVersion.Text = "Autosplitter version: v" + Assembly.GetExecutingAssembly().GetName().Version;

        // General settings
        chkSonicStart.DataBindings.Add("Checked", this, "SonicStart", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSonicLoadless.DataBindings.Add("Checked", this, "SonicLoadless", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill1.DataBindings.Add("Checked", this, "GreenHill1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill1_1.DataBindings.Add("Checked", this, "GreenHill1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill1_2.DataBindings.Add("Checked", this, "GreenHill1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill1_3.DataBindings.Add("Checked", this, "GreenHill1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill1_4.DataBindings.Add("Checked", this, "GreenHill1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill1_5.DataBindings.Add("Checked", this, "GreenHill1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill2.DataBindings.Add("Checked", this, "GreenHill2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill2_1.DataBindings.Add("Checked", this, "GreenHill2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill2_2.DataBindings.Add("Checked", this, "GreenHill2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill2_3.DataBindings.Add("Checked", this, "GreenHill2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill2_4.DataBindings.Add("Checked", this, "GreenHill2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkGreenHill2_5.DataBindings.Add("Checked", this, "GreenHill2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant1.DataBindings.Add("Checked", this, "ChemicalPlant1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant1_1.DataBindings.Add("Checked", this, "ChemicalPlant1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant1_2.DataBindings.Add("Checked", this, "ChemicalPlant1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant1_3.DataBindings.Add("Checked", this, "ChemicalPlant1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant1_4.DataBindings.Add("Checked", this, "ChemicalPlant1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant1_5.DataBindings.Add("Checked", this, "ChemicalPlant1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant2.DataBindings.Add("Checked", this, "ChemicalPlant2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant2_1.DataBindings.Add("Checked", this, "ChemicalPlant2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant2_2.DataBindings.Add("Checked", this, "ChemicalPlant2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant2_3.DataBindings.Add("Checked", this, "ChemicalPlant2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant2_4.DataBindings.Add("Checked", this, "ChemicalPlant2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChemicalPlant2_5.DataBindings.Add("Checked", this, "ChemicalPlant2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary1.DataBindings.Add("Checked", this, "SkySanctuary1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary1_1.DataBindings.Add("Checked", this, "SkySanctuary1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary1_2.DataBindings.Add("Checked", this, "SkySanctuary1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary1_3.DataBindings.Add("Checked", this, "SkySanctuary1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary1_4.DataBindings.Add("Checked", this, "SkySanctuary1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary1_5.DataBindings.Add("Checked", this, "SkySanctuary1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary2.DataBindings.Add("Checked", this, "SkySanctuary2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary2_1.DataBindings.Add("Checked", this, "SkySanctuary2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary2_2.DataBindings.Add("Checked", this, "SkySanctuary2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary2_3.DataBindings.Add("Checked", this, "SkySanctuary2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary2_4.DataBindings.Add("Checked", this, "SkySanctuary2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSkySanctuary2_5.DataBindings.Add("Checked", this, "SkySanctuary2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkMetalSonic.DataBindings.Add("Checked", this, "MetalSonic", false, DataSourceUpdateMode.OnPropertyChanged);
        chkDeathEgg.DataBindings.Add("Checked", this, "DeathEgg", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway1.DataBindings.Add("Checked", this, "SpeedHighway1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway1_1.DataBindings.Add("Checked", this, "SpeedHighway1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway1_2.DataBindings.Add("Checked", this, "SpeedHighway1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway1_3.DataBindings.Add("Checked", this, "SpeedHighway1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway1_4.DataBindings.Add("Checked", this, "SpeedHighway1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway1_5.DataBindings.Add("Checked", this, "SpeedHighway1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway2.DataBindings.Add("Checked", this, "SpeedHighway2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway2_1.DataBindings.Add("Checked", this, "SpeedHighway2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway2_2.DataBindings.Add("Checked", this, "SpeedHighway2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway2_3.DataBindings.Add("Checked", this, "SpeedHighway2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway2_4.DataBindings.Add("Checked", this, "SpeedHighway2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpeedHighway2_5.DataBindings.Add("Checked", this, "SpeedHighway2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape1.DataBindings.Add("Checked", this, "CityEscape1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape1_1.DataBindings.Add("Checked", this, "CityEscape1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape1_2.DataBindings.Add("Checked", this, "CityEscape1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape1_3.DataBindings.Add("Checked", this, "CityEscape1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape1_4.DataBindings.Add("Checked", this, "CityEscape1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape1_5.DataBindings.Add("Checked", this, "CityEscape1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape2.DataBindings.Add("Checked", this, "CityEscape2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape2_1.DataBindings.Add("Checked", this, "CityEscape2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape2_2.DataBindings.Add("Checked", this, "CityEscape2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape2_3.DataBindings.Add("Checked", this, "CityEscape2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape2_4.DataBindings.Add("Checked", this, "CityEscape2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCityEscape2_5.DataBindings.Add("Checked", this, "CityEscape2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill1.DataBindings.Add("Checked", this, "SeasideHill1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill1_1.DataBindings.Add("Checked", this, "SeasideHill1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill1_2.DataBindings.Add("Checked", this, "SeasideHill1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill1_3.DataBindings.Add("Checked", this, "SeasideHill1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill1_4.DataBindings.Add("Checked", this, "SeasideHill1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill1_5.DataBindings.Add("Checked", this, "SeasideHill1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill2.DataBindings.Add("Checked", this, "SeasideHill2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill2_1.DataBindings.Add("Checked", this, "SeasideHill2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill2_2.DataBindings.Add("Checked", this, "SeasideHill2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill2_3.DataBindings.Add("Checked", this, "SeasideHill2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill2_4.DataBindings.Add("Checked", this, "SeasideHill2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeasideHill2_5.DataBindings.Add("Checked", this, "SeasideHill2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShadow.DataBindings.Add("Checked", this, "Shadow", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPerfectChaos.DataBindings.Add("Checked", this, "PerfectChaos", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity1.DataBindings.Add("Checked", this, "CrisisCity1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity1_1.DataBindings.Add("Checked", this, "CrisisCity1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity1_2.DataBindings.Add("Checked", this, "CrisisCity1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity1_3.DataBindings.Add("Checked", this, "CrisisCity1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity1_4.DataBindings.Add("Checked", this, "CrisisCity1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity1_5.DataBindings.Add("Checked", this, "CrisisCity1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity2.DataBindings.Add("Checked", this, "CrisisCity2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity2_1.DataBindings.Add("Checked", this, "CrisisCity2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity2_2.DataBindings.Add("Checked", this, "CrisisCity2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity2_3.DataBindings.Add("Checked", this, "CrisisCity2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity2_4.DataBindings.Add("Checked", this, "CrisisCity2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCrisisCity2_5.DataBindings.Add("Checked", this, "CrisisCity2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun1.DataBindings.Add("Checked", this, "RooftopRun1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun1_1.DataBindings.Add("Checked", this, "RooftopRun1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun1_2.DataBindings.Add("Checked", this, "RooftopRun1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun1_3.DataBindings.Add("Checked", this, "RooftopRun1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun1_4.DataBindings.Add("Checked", this, "RooftopRun1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun1_5.DataBindings.Add("Checked", this, "RooftopRun1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun2.DataBindings.Add("Checked", this, "RooftopRun2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun2_1.DataBindings.Add("Checked", this, "RooftopRun2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun2_2.DataBindings.Add("Checked", this, "RooftopRun2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun2_3.DataBindings.Add("Checked", this, "RooftopRun2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun2_4.DataBindings.Add("Checked", this, "RooftopRun2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRooftopRun2_5.DataBindings.Add("Checked", this, "RooftopRun2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp1.DataBindings.Add("Checked", this, "PlanetWisp1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp1_1.DataBindings.Add("Checked", this, "PlanetWisp1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp1_2.DataBindings.Add("Checked", this, "PlanetWisp1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp1_3.DataBindings.Add("Checked", this, "PlanetWisp1_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp1_4.DataBindings.Add("Checked", this, "PlanetWisp1_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp1_5.DataBindings.Add("Checked", this, "PlanetWisp1_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp2.DataBindings.Add("Checked", this, "PlanetWisp2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp2_1.DataBindings.Add("Checked", this, "PlanetWisp2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp2_2.DataBindings.Add("Checked", this, "PlanetWisp2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp2_3.DataBindings.Add("Checked", this, "PlanetWisp2_3", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp2_4.DataBindings.Add("Checked", this, "PlanetWisp2_4", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlanetWisp2_5.DataBindings.Add("Checked", this, "PlanetWisp2_5", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSilver.DataBindings.Add("Checked", this, "Silver", false, DataSourceUpdateMode.OnPropertyChanged);
        chkEggDragoon.DataBindings.Add("Checked", this, "EggDragoon", false, DataSourceUpdateMode.OnPropertyChanged);
        chkTimeEater.DataBindings.Add("Checked", this, "TimeEater", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShadowLoadless.DataBindings.Add("Checked", this, "ShadowLoadless", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk1.DataBindings.Add("Checked", this, "SpaceColonyArk1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk1_1.DataBindings.Add("Checked", this, "SpaceColonyArk1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk1_2.DataBindings.Add("Checked", this, "SpaceColonyArk1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk1_Hard.DataBindings.Add("Checked", this, "SpaceColonyArk1_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk2.DataBindings.Add("Checked", this, "SpaceColonyArk2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk2_1.DataBindings.Add("Checked", this, "SpaceColonyArk2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk2_2.DataBindings.Add("Checked", this, "SpaceColonyArk2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSpaceColonyArk2_Hard.DataBindings.Add("Checked", this, "SpaceColonyArk2_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon1.DataBindings.Add("Checked", this, "RailCanyon1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon1_1.DataBindings.Add("Checked", this, "RailCanyon1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon1_2.DataBindings.Add("Checked", this, "RailCanyon1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon1_Hard.DataBindings.Add("Checked", this, "RailCanyon1_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon2.DataBindings.Add("Checked", this, "RailCanyon2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon2_1.DataBindings.Add("Checked", this, "RailCanyon2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon2_2.DataBindings.Add("Checked", this, "RailCanyon2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRailCanyon2_Hard.DataBindings.Add("Checked", this, "RailCanyon2_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley1.DataBindings.Add("Checked", this, "KingdomValley1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley1_1.DataBindings.Add("Checked", this, "KingdomValley1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley1_2.DataBindings.Add("Checked", this, "KingdomValley1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley1_Hard.DataBindings.Add("Checked", this, "KingdomValley1_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley2.DataBindings.Add("Checked", this, "KingdomValley2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley2_1.DataBindings.Add("Checked", this, "KingdomValley2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley2_2.DataBindings.Add("Checked", this, "KingdomValley2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkKingdomValley2_Hard.DataBindings.Add("Checked", this, "KingdomValley2_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights1.DataBindings.Add("Checked", this, "SunsetHeights1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights1_1.DataBindings.Add("Checked", this, "SunsetHeights1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights1_2.DataBindings.Add("Checked", this, "SunsetHeights1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights1_Hard.DataBindings.Add("Checked", this, "SunsetHeights1_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights2.DataBindings.Add("Checked", this, "SunsetHeights2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights2_1.DataBindings.Add("Checked", this, "SunsetHeights2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights2_2.DataBindings.Add("Checked", this, "SunsetHeights2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSunsetHeights2_Hard.DataBindings.Add("Checked", this, "SunsetHeights2_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland1.DataBindings.Add("Checked", this, "ChaosIsland1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland1_1.DataBindings.Add("Checked", this, "ChaosIsland1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland1_2.DataBindings.Add("Checked", this, "ChaosIsland1_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland1_Hard.DataBindings.Add("Checked", this, "ChaosIsland1_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland2.DataBindings.Add("Checked", this, "ChaosIsland2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland2_1.DataBindings.Add("Checked", this, "ChaosIsland2_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland2_2.DataBindings.Add("Checked", this, "ChaosIsland2_2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkChaosIsland2_Hard.DataBindings.Add("Checked", this, "ChaosIsland2_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRadicalHighway1.DataBindings.Add("Checked", this, "RadicalHighway1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRadicalHighway2.DataBindings.Add("Checked", this, "RadicalHighway2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShadowNewGameStart.DataBindings.Add("Checked", this, "ShadowNewGameStart", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShadowLevelEnterStart.DataBindings.Add("Checked", this, "ShadowLevelEnterStart", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShadowReset.DataBindings.Add("Checked", this, "ShadowReset", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShadowFocusPatch.DataBindings.Add("Checked", this, "ShadowFocusPatch", false, DataSourceUpdateMode.OnPropertyChanged);
        chkBiolizard.DataBindings.Add("Checked", this, "Biolizard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkBiolizard_Hard.DataBindings.Add("Checked", this, "Biolizard_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkMetalOverlord.DataBindings.Add("Checked", this, "MetalOverlord", false, DataSourceUpdateMode.OnPropertyChanged);
        chkMetalOverlord_Hard.DataBindings.Add("Checked", this, "MetalOverlord_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkMephiles.DataBindings.Add("Checked", this, "Mephiles", false, DataSourceUpdateMode.OnPropertyChanged);
        chkMephiles_Hard.DataBindings.Add("Checked", this, "Mephiles_Hard", false, DataSourceUpdateMode.OnPropertyChanged);
        chkBlackDoom.DataBindings.Add("Checked", this, "BlackDoom", false, DataSourceUpdateMode.OnPropertyChanged);
        chkTokio1.DataBindings.Add("Checked", this, "Tokio1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkTokio1_1.DataBindings.Add("Checked", this, "Tokio1_1", false, DataSourceUpdateMode.OnPropertyChanged);
        chkTokio1_2.DataBindings.Add("Checked", this, "Tokio1_2", false, DataSourceUpdateMode.OnPropertyChanged);


        // Default Values
        SonicLoadless = true;
        SonicStart = true;
        GreenHill1 = GreenHill1_1 = GreenHill1_2 = GreenHill1_3 = GreenHill1_4 = GreenHill1_5 = true;
        GreenHill2 = GreenHill2_1 = GreenHill2_2 = GreenHill2_3 = GreenHill2_4 = GreenHill2_5 = true;
        ChemicalPlant1 = ChemicalPlant1_1 = ChemicalPlant1_2 = ChemicalPlant1_3 = ChemicalPlant1_4 = ChemicalPlant1_5 = true;
        ChemicalPlant2 = ChemicalPlant2_1 = ChemicalPlant2_2 = ChemicalPlant2_3 = ChemicalPlant2_4 = ChemicalPlant2_5 = true;
        SkySanctuary1 = SkySanctuary1_1 = SkySanctuary1_2 = SkySanctuary1_3 = SkySanctuary1_4 = SkySanctuary1_5 = true;
        SkySanctuary2 = SkySanctuary2_1 = SkySanctuary2_2 = SkySanctuary2_3 = SkySanctuary2_4 = SkySanctuary2_5 = true;
        MetalSonic = true;
        DeathEgg = true;
        SpeedHighway1 = SpeedHighway1_1 = SpeedHighway1_2 = SpeedHighway1_3 = SpeedHighway1_4 = SpeedHighway1_5 = true;
        SpeedHighway2 = SpeedHighway2_1 = SpeedHighway2_2 = SpeedHighway2_3 = SpeedHighway2_4 = SpeedHighway2_5 = true;
        CityEscape1 = CityEscape1_1 = CityEscape1_2 = CityEscape1_3 = CityEscape1_4 = CityEscape1_5 = true;
        CityEscape2 = CityEscape2_1 = CityEscape2_2 = CityEscape2_3 = CityEscape2_4 = CityEscape2_5 = true;
        SeasideHill1 = SeasideHill1_1 = SeasideHill1_2 = SeasideHill1_3 = SeasideHill1_4 = SeasideHill1_5 = true;
        SeasideHill2 = SeasideHill2_1 = SeasideHill2_2 = SeasideHill2_3 = SeasideHill2_4 = SeasideHill2_5 = true;
        Shadow = true;
        PerfectChaos = true;
        CrisisCity1 = CrisisCity1_1 = CrisisCity1_2 = CrisisCity1_3 = CrisisCity1_4 = CrisisCity1_5 = true;
        CrisisCity2 = CrisisCity2_1 = CrisisCity2_2 = CrisisCity2_3 = CrisisCity2_4 = CrisisCity2_5 = true;
        RooftopRun1 = RooftopRun1_1 = RooftopRun1_2 = RooftopRun1_3 = RooftopRun1_4 = RooftopRun1_5 = true;
        RooftopRun2 = RooftopRun2_1 = RooftopRun2_2 = RooftopRun2_3 = RooftopRun2_4 = RooftopRun2_5 = true;
        PlanetWisp1 = PlanetWisp1_1 = PlanetWisp1_2 = PlanetWisp1_3 = PlanetWisp1_4 = PlanetWisp1_5 = true;
        PlanetWisp2 = PlanetWisp2_1 = PlanetWisp2_2 = PlanetWisp2_3 = PlanetWisp2_4 = PlanetWisp2_5 = true;
        Silver = true;
        EggDragoon = true;
        TimeEater = true;
        ShadowLoadless = true;
        SpacecolonyArk1 = SpacecolonyArk1_1 = SpacecolonyArk1_2 = SpacecolonyArk1_Hard = true;
        SpacecolonyArk2 = SpacecolonyArk2_1 = SpacecolonyArk2_2 = SpacecolonyArk2_Hard = true;
        RailCanyon1 = RailCanyon1_1 = RailCanyon1_2 = RailCanyon1_Hard = true;
        RailCanyon2 = RailCanyon2_1 = RailCanyon2_2 = RailCanyon2_Hard = true;
        KingdomValley1 = KingdomValley1_1 = KingdomValley1_2 = KingdomValley1_Hard = true;
        KingdomValley2 = KingdomValley2_1 = KingdomValley2_2 = KingdomValley2_Hard = true;
        SunsetHeights1 = SunsetHeights1_1 = SunsetHeights1_2 = SunsetHeights1_Hard = true;
        SunsetHeights2 = SunsetHeights2_1 = SunsetHeights2_2 = SunsetHeights2_Hard = true;
        ChaosIsland1 = ChaosIsland1_1 = ChaosIsland1_2 = ChaosIsland1_Hard = true;
        ChaosIsland2 = ChaosIsland2_1 = ChaosIsland2_2 = ChaosIsland2_Hard = true;
        RadicalHighway1 = RadicalHighway2 = true;
        ShadowNewGameStart = ShadowReset = ShadowLevelEnterStart = true;
        Biolizard = Biolizard_Hard = true;
        MetalOverlord = MetalOverlord_Hard = true;
        Mephiles = Mephiles_Hard = true;
        BlackDoom = true;
        Tokio1 = Tokio1_1 = Tokio1_2 = true;
        ShadowFocusPatch = false;
    }

    public XmlNode GetSettings(XmlDocument doc)
    {
        XmlElement settingsNode = doc.CreateElement("Settings");
        settingsNode.AppendChild(ToElement(doc, "SonicStart", SonicStart));
        settingsNode.AppendChild(ToElement(doc, "SonicLoadless", SonicLoadless));
        settingsNode.AppendChild(ToElement(doc, "GreenHill1", GreenHill1));
        settingsNode.AppendChild(ToElement(doc, "GreenHill1_1", GreenHill1_1));
        settingsNode.AppendChild(ToElement(doc, "GreenHill1_2", GreenHill1_2));
        settingsNode.AppendChild(ToElement(doc, "GreenHill1_3", GreenHill1_3));
        settingsNode.AppendChild(ToElement(doc, "GreenHill1_4", GreenHill1_4));
        settingsNode.AppendChild(ToElement(doc, "GreenHill1_5", GreenHill1_5));
        settingsNode.AppendChild(ToElement(doc, "GreenHill2", GreenHill2));
        settingsNode.AppendChild(ToElement(doc, "GreenHill2_1", GreenHill2_1));
        settingsNode.AppendChild(ToElement(doc, "GreenHill2_2", GreenHill2_2));
        settingsNode.AppendChild(ToElement(doc, "GreenHill2_3", GreenHill2_3));
        settingsNode.AppendChild(ToElement(doc, "GreenHill2_4", GreenHill2_4));
        settingsNode.AppendChild(ToElement(doc, "GreenHill2_5", GreenHill2_5));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant1", ChemicalPlant1));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant1_1", ChemicalPlant1_1));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant1_2", ChemicalPlant1_2));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant1_3", ChemicalPlant1_3));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant1_4", ChemicalPlant1_4));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant1_5", ChemicalPlant1_5));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant2", ChemicalPlant2));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant2_1", ChemicalPlant2_1));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant2_2", ChemicalPlant2_2));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant2_3", ChemicalPlant2_3));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant2_4", ChemicalPlant2_4));
        settingsNode.AppendChild(ToElement(doc, "ChemicalPlant2_5", ChemicalPlant2_5));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary1", SkySanctuary1));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary1_1", SkySanctuary1_1));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary1_2", SkySanctuary1_2));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary1_3", SkySanctuary1_3));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary1_4", SkySanctuary1_4));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary1_5", SkySanctuary1_5));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary2", SkySanctuary2));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary2_1", SkySanctuary2_1));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary2_2", SkySanctuary2_2));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary2_3", SkySanctuary2_3));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary2_4", SkySanctuary2_4));
        settingsNode.AppendChild(ToElement(doc, "SkySanctuary2_5", SkySanctuary2_5));
        settingsNode.AppendChild(ToElement(doc, "MetalSonic", MetalSonic));
        settingsNode.AppendChild(ToElement(doc, "DeathEgg", DeathEgg));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway1", SpeedHighway1));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway1_1", SpeedHighway1_1));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway1_2", SpeedHighway1_2));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway1_3", SpeedHighway1_3));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway1_4", SpeedHighway1_4));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway1_5", SpeedHighway1_5));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway2", SpeedHighway2));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway2_1", SpeedHighway2_1));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway2_2", SpeedHighway2_2));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway2_3", SpeedHighway2_3));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway2_4", SpeedHighway2_4));
        settingsNode.AppendChild(ToElement(doc, "SpeedHighway2_5", SpeedHighway2_5));
        settingsNode.AppendChild(ToElement(doc, "CityEscape1", CityEscape1));
        settingsNode.AppendChild(ToElement(doc, "CityEscape1_1", CityEscape1_1));
        settingsNode.AppendChild(ToElement(doc, "CityEscape1_2", CityEscape1_2));
        settingsNode.AppendChild(ToElement(doc, "CityEscape1_3", CityEscape1_3));
        settingsNode.AppendChild(ToElement(doc, "CityEscape1_4", CityEscape1_4));
        settingsNode.AppendChild(ToElement(doc, "CityEscape1_5", CityEscape1_5));
        settingsNode.AppendChild(ToElement(doc, "CityEscape2", CityEscape2));
        settingsNode.AppendChild(ToElement(doc, "CityEscape2_1", CityEscape2_1));
        settingsNode.AppendChild(ToElement(doc, "CityEscape2_2", CityEscape2_2));
        settingsNode.AppendChild(ToElement(doc, "CityEscape2_3", CityEscape2_3));
        settingsNode.AppendChild(ToElement(doc, "CityEscape2_4", CityEscape2_4));
        settingsNode.AppendChild(ToElement(doc, "CityEscape2_5", CityEscape2_5));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill1", SeasideHill1));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill1_1", SeasideHill1_1));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill1_2", SeasideHill1_2));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill1_3", SeasideHill1_3));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill1_4", SeasideHill1_4));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill1_5", SeasideHill1_5));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill2", SeasideHill2));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill2_1", SeasideHill2_1));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill2_2", SeasideHill2_2));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill2_3", SeasideHill2_3));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill2_4", SeasideHill2_4));
        settingsNode.AppendChild(ToElement(doc, "SeasideHill2_5", SeasideHill2_5));
        settingsNode.AppendChild(ToElement(doc, "Shadow", Shadow));
        settingsNode.AppendChild(ToElement(doc, "PerfectChaos", PerfectChaos));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity1", CrisisCity1));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity1_1", CrisisCity1_1));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity1_2", CrisisCity1_2));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity1_3", CrisisCity1_3));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity1_4", CrisisCity1_4));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity1_5", CrisisCity1_5));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity2", CrisisCity2));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity2_1", CrisisCity2_1));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity2_2", CrisisCity2_2));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity2_3", CrisisCity2_3));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity2_4", CrisisCity2_4));
        settingsNode.AppendChild(ToElement(doc, "CrisisCity2_5", CrisisCity2_5));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun1", RooftopRun1));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun1_1", RooftopRun1_1));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun1_2", RooftopRun1_2));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun1_3", RooftopRun1_3));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun1_4", RooftopRun1_4));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun1_5", RooftopRun1_5));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun2", RooftopRun2));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun2_1", RooftopRun2_1));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun2_2", RooftopRun2_2));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun2_3", RooftopRun2_3));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun2_4", RooftopRun2_4));
        settingsNode.AppendChild(ToElement(doc, "RooftopRun2_5", RooftopRun2_5));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp1", PlanetWisp1));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp1_1", PlanetWisp1_1));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp1_2", PlanetWisp1_2));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp1_3", PlanetWisp1_3));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp1_4", PlanetWisp1_4));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp1_5", PlanetWisp1_5));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp2", PlanetWisp2));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp2_1", PlanetWisp2_1));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp2_2", PlanetWisp2_2));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp2_3", PlanetWisp2_3));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp2_4", PlanetWisp2_4));
        settingsNode.AppendChild(ToElement(doc, "PlanetWisp2_5", PlanetWisp2_5));
        settingsNode.AppendChild(ToElement(doc, "Silver", Silver));
        settingsNode.AppendChild(ToElement(doc, "EggDragoon", EggDragoon));
        settingsNode.AppendChild(ToElement(doc, "TimeEater", TimeEater));
        settingsNode.AppendChild(ToElement(doc, "ShadowLoadless", ShadowLoadless));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk1", SpacecolonyArk1));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk1_1", SpacecolonyArk1_1));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk1_2", SpacecolonyArk1_2));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk1_Hard", SpacecolonyArk1_Hard));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk2", SpacecolonyArk2));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk2_1", SpacecolonyArk2_1));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk2_2", SpacecolonyArk2_2));
        settingsNode.AppendChild(ToElement(doc, "SpacecolonyArk2_Hard", SpacecolonyArk2_Hard));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon1", RailCanyon1));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon1_1", RailCanyon1_1));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon1_2", RailCanyon1_2));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon1_Hard", RailCanyon1_Hard));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon2", RailCanyon2));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon2_1", RailCanyon2_1));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon2_2", RailCanyon2_2));
        settingsNode.AppendChild(ToElement(doc, "RailCanyon2_Hard", RailCanyon2_Hard));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley1", KingdomValley1));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley1_1", KingdomValley1_1));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley1_2", KingdomValley1_2));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley1_Hard", KingdomValley1_Hard));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley2", KingdomValley2));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley2_1", KingdomValley2_1));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley2_2", KingdomValley2_2));
        settingsNode.AppendChild(ToElement(doc, "KingdomValley2_Hard", KingdomValley2_Hard));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights1", SunsetHeights1));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights1_1", SunsetHeights1_1));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights1_2", SunsetHeights1_2));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights1_Hard", SunsetHeights1_Hard));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights2", SunsetHeights2));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights2_1", SunsetHeights2_1));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights2_2", SunsetHeights2_2));
        settingsNode.AppendChild(ToElement(doc, "SunsetHeights2_Hard", SunsetHeights2_Hard));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland1", ChaosIsland1));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland1_1", ChaosIsland1_1));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland1_2", ChaosIsland1_2));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland1_Hard", ChaosIsland1_Hard));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland2", ChaosIsland2));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland2_1", ChaosIsland2_1));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland2_2", ChaosIsland2_2));
        settingsNode.AppendChild(ToElement(doc, "ChaosIsland2_Hard", ChaosIsland2_Hard));
        settingsNode.AppendChild(ToElement(doc, "RadicalHighway1", RadicalHighway1));
        settingsNode.AppendChild(ToElement(doc, "RadicalHighway2", RadicalHighway2));
        settingsNode.AppendChild(ToElement(doc, "ShadowNewGameStart", ShadowNewGameStart));
        settingsNode.AppendChild(ToElement(doc, "ShadowLevelEnterStart", ShadowLevelEnterStart));
        settingsNode.AppendChild(ToElement(doc, "ShadowReset", ShadowReset));
        settingsNode.AppendChild(ToElement(doc, "ShadowFocusPatch", ShadowFocusPatch));
        settingsNode.AppendChild(ToElement(doc, "Biolizard", Biolizard));
        settingsNode.AppendChild(ToElement(doc, "Biolizard_Hard", Biolizard_Hard));
        settingsNode.AppendChild(ToElement(doc, "MetalOverlord", MetalOverlord));
        settingsNode.AppendChild(ToElement(doc, "MetalOverlord_Hard", MetalOverlord_Hard));
        settingsNode.AppendChild(ToElement(doc, "Mephiles", Mephiles));
        settingsNode.AppendChild(ToElement(doc, "Mephiles_Hard", Mephiles_Hard));
        settingsNode.AppendChild(ToElement(doc, "BlackDoom", BlackDoom));
        settingsNode.AppendChild(ToElement(doc, "Tokio1", Tokio1));
        settingsNode.AppendChild(ToElement(doc, "Tokio1_1", Tokio1_1));
        settingsNode.AppendChild(ToElement(doc, "Tokio1_2", Tokio1_2));
        return settingsNode;
    }

    public void SetSettings(XmlNode settings)
    {
        SonicStart = ParseBool(settings, "SonicStart", true);
        SonicLoadless = ParseBool(settings, "SonicLoadless", true);
        GreenHill1 = ParseBool(settings, "GreenHill1", true);
        GreenHill1_1 = ParseBool(settings, "GreenHill1_1", true);
        GreenHill1_2 = ParseBool(settings, "GreenHill1_2", true);
        GreenHill1_3 = ParseBool(settings, "GreenHill1_3", true);
        GreenHill1_4 = ParseBool(settings, "GreenHill1_4", true);
        GreenHill1_5 = ParseBool(settings, "GreenHill1_5", true);
        GreenHill2 = ParseBool(settings, "GreenHill2", true);
        GreenHill2_1 = ParseBool(settings, "GreenHill2_1", true);
        GreenHill2_2 = ParseBool(settings, "GreenHill2_2", true);
        GreenHill2_3 = ParseBool(settings, "GreenHill2_3", true);
        GreenHill2_4 = ParseBool(settings, "GreenHill2_4", true);
        GreenHill2_5 = ParseBool(settings, "GreenHill2_5", true);
        ChemicalPlant1 = ParseBool(settings, "ChemicalPlant1", true);
        ChemicalPlant1_1 = ParseBool(settings, "ChemicalPlant1_1", true);
        ChemicalPlant1_2 = ParseBool(settings, "ChemicalPlant1_2", true);
        ChemicalPlant1_3 = ParseBool(settings, "ChemicalPlant1_3", true);
        ChemicalPlant1_4 = ParseBool(settings, "ChemicalPlant1_4", true);
        ChemicalPlant1_5 = ParseBool(settings, "ChemicalPlant1_5", true);
        ChemicalPlant2 = ParseBool(settings, "ChemicalPlant2", true);
        ChemicalPlant2_1 = ParseBool(settings, "ChemicalPlant2_1", true);
        ChemicalPlant2_2 = ParseBool(settings, "ChemicalPlant2_2", true);
        ChemicalPlant2_3 = ParseBool(settings, "ChemicalPlant2_3", true);
        ChemicalPlant2_4 = ParseBool(settings, "ChemicalPlant2_4", true);
        ChemicalPlant2_5 = ParseBool(settings, "ChemicalPlant2_5", true);
        SkySanctuary1 = ParseBool(settings, "SkySanctuary1", true);
        SkySanctuary1_1 = ParseBool(settings, "SkySanctuary1_1", true);
        SkySanctuary1_2 = ParseBool(settings, "SkySanctuary1_2", true);
        SkySanctuary1_3 = ParseBool(settings, "SkySanctuary1_3", true);
        SkySanctuary1_4 = ParseBool(settings, "SkySanctuary1_4", true);
        SkySanctuary1_5 = ParseBool(settings, "SkySanctuary1_5", true);
        SkySanctuary2 = ParseBool(settings, "SkySanctuary2", true);
        SkySanctuary2_1 = ParseBool(settings, "SkySanctuary2_1", true);
        SkySanctuary2_2 = ParseBool(settings, "SkySanctuary2_2", true);
        SkySanctuary2_3 = ParseBool(settings, "SkySanctuary2_3", true);
        SkySanctuary2_4 = ParseBool(settings, "SkySanctuary2_4", true);
        SkySanctuary2_5 = ParseBool(settings, "SkySanctuary2_5", true);
        MetalSonic = ParseBool(settings, "MetalSonic", true);
        DeathEgg = ParseBool(settings, "DeathEgg", true);
        SpeedHighway1 = ParseBool(settings, "SpeedHighway1", true);
        SpeedHighway1_1 = ParseBool(settings, "SpeedHighway1_1", true);
        SpeedHighway1_2 = ParseBool(settings, "SpeedHighway1_2", true);
        SpeedHighway1_3 = ParseBool(settings, "SpeedHighway1_3", true);
        SpeedHighway1_4 = ParseBool(settings, "SpeedHighway1_4", true);
        SpeedHighway1_5 = ParseBool(settings, "SpeedHighway1_5", true);
        SpeedHighway2 = ParseBool(settings, "SpeedHighway2", true);
        SpeedHighway2_1 = ParseBool(settings, "SpeedHighway2_1", true);
        SpeedHighway2_2 = ParseBool(settings, "SpeedHighway2_2", true);
        SpeedHighway2_3 = ParseBool(settings, "SpeedHighway2_3", true);
        SpeedHighway2_4 = ParseBool(settings, "SpeedHighway2_4", true);
        SpeedHighway2_5 = ParseBool(settings, "SpeedHighway2_5", true);
        CityEscape1 = ParseBool(settings, "CityEscape1", true);
        CityEscape1_1 = ParseBool(settings, "CityEscape1_1", true);
        CityEscape1_2 = ParseBool(settings, "CityEscape1_2", true);
        CityEscape1_3 = ParseBool(settings, "CityEscape1_3", true);
        CityEscape1_4 = ParseBool(settings, "CityEscape1_4", true);
        CityEscape1_5 = ParseBool(settings, "CityEscape1_5", true);
        CityEscape2 = ParseBool(settings, "CityEscape2", true);
        CityEscape2_1 = ParseBool(settings, "CityEscape2_1", true);
        CityEscape2_2 = ParseBool(settings, "CityEscape2_2", true);
        CityEscape2_3 = ParseBool(settings, "CityEscape2_3", true);
        CityEscape2_4 = ParseBool(settings, "CityEscape2_4", true);
        CityEscape2_5 = ParseBool(settings, "CityEscape2_5", true);
        SeasideHill1 = ParseBool(settings, "SeasideHill1", true);
        SeasideHill1_1 = ParseBool(settings, "SeasideHill1_1", true);
        SeasideHill1_2 = ParseBool(settings, "SeasideHill1_2", true);
        SeasideHill1_3 = ParseBool(settings, "SeasideHill1_3", true);
        SeasideHill1_4 = ParseBool(settings, "SeasideHill1_4", true);
        SeasideHill1_5 = ParseBool(settings, "SeasideHill1_5", true);
        SeasideHill2 = ParseBool(settings, "SeasideHill2", true);
        SeasideHill2_1 = ParseBool(settings, "SeasideHill2_1", true);
        SeasideHill2_2 = ParseBool(settings, "SeasideHill2_2", true);
        SeasideHill2_3 = ParseBool(settings, "SeasideHill2_3", true);
        SeasideHill2_4 = ParseBool(settings, "SeasideHill2_4", true);
        SeasideHill2_5 = ParseBool(settings, "SeasideHill2_5", true);
        Shadow = ParseBool(settings, "Shadow", true);
        PerfectChaos = ParseBool(settings, "PerfectChaos", true);
        CrisisCity1 = ParseBool(settings, "CrisisCity1", true);
        CrisisCity1_1 = ParseBool(settings, "CrisisCity1_1", true);
        CrisisCity1_2 = ParseBool(settings, "CrisisCity1_2", true);
        CrisisCity1_3 = ParseBool(settings, "CrisisCity1_3", true);
        CrisisCity1_4 = ParseBool(settings, "CrisisCity1_4", true);
        CrisisCity1_5 = ParseBool(settings, "CrisisCity1_5", true);
        CrisisCity2 = ParseBool(settings, "CrisisCity2", true);
        CrisisCity2_1 = ParseBool(settings, "CrisisCity2_1", true);
        CrisisCity2_2 = ParseBool(settings, "CrisisCity2_2", true);
        CrisisCity2_3 = ParseBool(settings, "CrisisCity2_3", true);
        CrisisCity2_4 = ParseBool(settings, "CrisisCity2_4", true);
        CrisisCity2_5 = ParseBool(settings, "CrisisCity2_5", true);
        RooftopRun1 = ParseBool(settings, "RooftopRun1", true);
        RooftopRun1_1 = ParseBool(settings, "RooftopRun1_1", true);
        RooftopRun1_2 = ParseBool(settings, "RooftopRun1_2", true);
        RooftopRun1_3 = ParseBool(settings, "RooftopRun1_3", true);
        RooftopRun1_4 = ParseBool(settings, "RooftopRun1_4", true);
        RooftopRun1_5 = ParseBool(settings, "RooftopRun1_5", true);
        RooftopRun2 = ParseBool(settings, "RooftopRun2", true);
        RooftopRun2_1 = ParseBool(settings, "RooftopRun2_1", true);
        RooftopRun2_2 = ParseBool(settings, "RooftopRun2_2", true);
        RooftopRun2_3 = ParseBool(settings, "RooftopRun2_3", true);
        RooftopRun2_4 = ParseBool(settings, "RooftopRun2_4", true);
        RooftopRun2_5 = ParseBool(settings, "RooftopRun2_5", true);
        PlanetWisp1 = ParseBool(settings, "PlanetWisp1", true);
        PlanetWisp1_1 = ParseBool(settings, "PlanetWisp1_1", true);
        PlanetWisp1_2 = ParseBool(settings, "PlanetWisp1_2", true);
        PlanetWisp1_3 = ParseBool(settings, "PlanetWisp1_3", true);
        PlanetWisp1_4 = ParseBool(settings, "PlanetWisp1_4", true);
        PlanetWisp1_5 = ParseBool(settings, "PlanetWisp1_5", true);
        PlanetWisp2 = ParseBool(settings, "PlanetWisp2", true);
        PlanetWisp2_1 = ParseBool(settings, "PlanetWisp2_1", true);
        PlanetWisp2_2 = ParseBool(settings, "PlanetWisp2_2", true);
        PlanetWisp2_3 = ParseBool(settings, "PlanetWisp2_3", true);
        PlanetWisp2_4 = ParseBool(settings, "PlanetWisp2_4", true);
        PlanetWisp2_5 = ParseBool(settings, "PlanetWisp2_5", true);
        EggDragoon = ParseBool(settings, "EggDragoon", true);
        TimeEater = ParseBool(settings, "TimeEater", true);
        ShadowLoadless = ParseBool(settings, "ShadowLoadless", true);
        SpacecolonyArk1 = ParseBool(settings, "SpacecolonyArk1", true);
        SpacecolonyArk1_1 = ParseBool(settings, "SpacecolonyArk1_1", true);
        SpacecolonyArk1_2 = ParseBool(settings, "SpacecolonyArk1_2", true);
        SpacecolonyArk1_Hard = ParseBool(settings, "SpacecolonyArk1_Hard", true);
        SpacecolonyArk2 = ParseBool(settings, "SpacecolonyArk2", true);
        SpacecolonyArk2_1 = ParseBool(settings, "SpacecolonyArk2_1", true);
        SpacecolonyArk2_2 = ParseBool(settings, "SpacecolonyArk2_2", true);
        SpacecolonyArk2_Hard = ParseBool(settings, "SpacecolonyArk2_Hard", true);
        RailCanyon1 = ParseBool(settings, "RailCanyon1", true);
        RailCanyon1_1 = ParseBool(settings, "RailCanyon1_1", true);
        RailCanyon1_2 = ParseBool(settings, "RailCanyon1_2", true);
        RailCanyon1_Hard = ParseBool(settings, "RailCanyon1_Hard", true);
        RailCanyon2 = ParseBool(settings, "RailCanyon2", true);
        RailCanyon2_1 = ParseBool(settings, "RailCanyon2_1", true);
        RailCanyon2_2 = ParseBool(settings, "RailCanyon2_2", true);
        RailCanyon2_Hard = ParseBool(settings, "RailCanyon2_Hard", true);
        KingdomValley1 = ParseBool(settings, "KingdomValley1", true);
        KingdomValley1_1 = ParseBool(settings, "KingdomValley1_1", true);
        KingdomValley1_2 = ParseBool(settings, "KingdomValley1_2", true);
        KingdomValley1_Hard = ParseBool(settings, "KingdomValley1_Hard", true);
        KingdomValley2 = ParseBool(settings, "KingdomValley2", true);
        KingdomValley2_1 = ParseBool(settings, "KingdomValley2_1", true);
        KingdomValley2_2 = ParseBool(settings, "KingdomValley2_2", true);
        KingdomValley2_Hard = ParseBool(settings, "KingdomValley2_Hard", true);
        SunsetHeights1 = ParseBool(settings, "SunsetHeights1", true);
        SunsetHeights1_1 = ParseBool(settings, "SunsetHeights1_1", true);
        SunsetHeights1_2 = ParseBool(settings, "SunsetHeights1_2", true);
        SunsetHeights1_Hard = ParseBool(settings, "SunsetHeights1_Hard", true);
        SunsetHeights2 = ParseBool(settings, "SunsetHeights2", true);
        SunsetHeights2_1 = ParseBool(settings, "SunsetHeights2_1", true);
        SunsetHeights2_2 = ParseBool(settings, "SunsetHeights2_2", true);
        SunsetHeights2_Hard = ParseBool(settings, "SunsetHeights2_Hard", true);
        ChaosIsland1 = ParseBool(settings, "ChaosIsland1", true);
        ChaosIsland1_1 = ParseBool(settings, "ChaosIsland1_1", true);
        ChaosIsland1_2 = ParseBool(settings, "ChaosIsland1_2", true);
        ChaosIsland1_Hard = ParseBool(settings, "ChaosIsland1_Hard", true);
        ChaosIsland2 = ParseBool(settings, "ChaosIsland2", true);
        ChaosIsland2_1 = ParseBool(settings, "ChaosIsland2_1", true);
        ChaosIsland2_2 = ParseBool(settings, "ChaosIsland2_2", true);
        ChaosIsland2_Hard = ParseBool(settings, "ChaosIsland2_Hard", true);
        RadicalHighway1 = ParseBool(settings, "RadicalHighway1", true);
        RadicalHighway2 = ParseBool(settings, "RadicalHighway2", true);
        ShadowNewGameStart = ParseBool(settings, "ShadowNewGameStart", true);
        ShadowLevelEnterStart = ParseBool(settings, "ShadowLevelEnterStart", true);
        ShadowReset = ParseBool(settings, "ShadowReset", true);
        ShadowFocusPatch = ParseBool(settings, "ShadowFocusPatch", true);
        Biolizard = ParseBool(settings, "Biolizard", true);
        Biolizard_Hard = ParseBool(settings, "Biolizard_Hard", true);
        MetalOverlord = ParseBool(settings, "MetalOverlord", true);
        MetalOverlord_Hard = ParseBool(settings, "MetalOverlord_Hard", true);
        Mephiles = ParseBool(settings, "Mephiles", true);
        Mephiles_Hard = ParseBool(settings, "Mephiles_Hard", true);
        BlackDoom = ParseBool(settings, "BlackDoom", true);
        Tokio1 = ParseBool(settings, "Tokio1", true);
        Tokio1_1 = ParseBool(settings, "Tokio1_1", true);
        Tokio1_2 = ParseBool(settings, "Tokio1_2", true);
    }

    static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
    {
        return settings[setting] != null ? (bool.TryParse(settings[setting].InnerText, out bool val) ? val : default_) : default_;
    }

    static XmlElement ToElement<T>(XmlDocument document, string name, T value)
    {
        XmlElement str = document.CreateElement(name);
        str.InnerText = value!.ToString();
        return str;
    }

    private void ClassicButton_Click(object sender, EventArgs e)
    {
        chkSonicLoadless.Checked = true;
        chkSonicStart.Checked = true;
        chkGreenHill1.Checked = chkGreenHill1_1.Checked = chkGreenHill1_2.Checked = chkGreenHill1_3.Checked = chkGreenHill1_4.Checked = chkGreenHill1_5.Checked = true;
        chkGreenHill2.Checked = chkGreenHill2_1.Checked = chkGreenHill2_2.Checked = chkGreenHill2_3.Checked = chkGreenHill2_4.Checked = chkGreenHill2_5.Checked = true;
        chkChemicalPlant1.Checked = chkChemicalPlant1_1.Checked = chkChemicalPlant1_2.Checked = chkChemicalPlant1_3.Checked = chkChemicalPlant1_4.Checked = chkChemicalPlant1_5.Checked = true;
        chkChemicalPlant2.Checked = chkChemicalPlant2_1.Checked = chkChemicalPlant2_2.Checked = chkChemicalPlant2_3.Checked = chkChemicalPlant2_4.Checked = chkChemicalPlant2_5.Checked = true;
        chkSkySanctuary1.Checked = chkSkySanctuary1_1.Checked = chkSkySanctuary1_2.Checked = chkSkySanctuary1_3.Checked = chkSkySanctuary1_4.Checked = chkSkySanctuary1_5.Checked = true;
        chkSkySanctuary2.Checked = chkSkySanctuary2_1.Checked = chkSkySanctuary2_2.Checked = chkSkySanctuary2_3.Checked = chkSkySanctuary2_4.Checked = chkSkySanctuary2_5.Checked = true;
        chkMetalSonic.Checked = true;
        chkDeathEgg.Checked = true;
    }

    private void AdventureButton_Click(object sender, EventArgs e)
    {
        chkSpeedHighway1.Checked = chkSpeedHighway1_1.Checked = chkSpeedHighway1_2.Checked = chkSpeedHighway1_3.Checked = chkSpeedHighway1_4.Checked = chkSpeedHighway1_5.Checked = true;
        chkSpeedHighway2.Checked = chkSpeedHighway2_1.Checked = chkSpeedHighway2_2.Checked = chkSpeedHighway2_3.Checked = chkSpeedHighway2_4.Checked = chkSpeedHighway2_5.Checked = true;
        chkCityEscape1.Checked = chkCityEscape1_1.Checked = chkCityEscape1_2.Checked = chkCityEscape1_3.Checked = chkCityEscape1_4.Checked = chkCityEscape1_5.Checked = true;
        chkCityEscape2.Checked = chkCityEscape2_1.Checked = chkCityEscape2_2.Checked = chkCityEscape2_3.Checked = chkCityEscape2_4.Checked = chkCityEscape2_5.Checked = true;
        chkSeasideHill1.Checked = chkSeasideHill1_1.Checked = chkSeasideHill1_2.Checked = chkSeasideHill1_3.Checked = chkSeasideHill1_4.Checked = chkSeasideHill1_5.Checked = true;
        chkSeasideHill2.Checked = chkSeasideHill2_1.Checked = chkSeasideHill2_2.Checked = chkSeasideHill2_3.Checked = chkSeasideHill2_4.Checked = chkSeasideHill2_5.Checked = true;
        chkShadow.Checked = true;
        chkPerfectChaos.Checked = true;
    }

    private void ModernButton_Click(object sender, EventArgs e)
    {
        chkCrisisCity1.Checked = chkCrisisCity1_1.Checked = chkCrisisCity1_2.Checked = chkCrisisCity1_3.Checked = chkCrisisCity1_4.Checked = chkCrisisCity1_5.Checked = true;
        chkCrisisCity2.Checked = chkCrisisCity2_1.Checked = chkCrisisCity2_2.Checked = chkCrisisCity2_3.Checked = chkCrisisCity2_4.Checked = chkCrisisCity2_5.Checked = true;
        chkRooftopRun1.Checked = chkRooftopRun1_1.Checked = chkRooftopRun1_2.Checked = chkRooftopRun1_3.Checked = chkRooftopRun1_4.Checked = chkRooftopRun1_5.Checked = true;
        chkRooftopRun2.Checked = chkRooftopRun2_1.Checked = chkRooftopRun2_2.Checked = chkRooftopRun2_3.Checked = chkRooftopRun2_4.Checked = chkRooftopRun2_5.Checked = true;
        chkPlanetWisp1.Checked = chkPlanetWisp1_1.Checked = chkPlanetWisp1_2.Checked = chkPlanetWisp1_3.Checked = chkPlanetWisp1_4.Checked = chkPlanetWisp1_5.Checked = true;
        chkPlanetWisp2.Checked = chkPlanetWisp2_1.Checked = chkPlanetWisp2_2.Checked = chkPlanetWisp2_3.Checked = chkPlanetWisp2_4.Checked = chkPlanetWisp2_5.Checked = true;
        chkSilver.Checked = true;
        chkEggDragoon.Checked = true;
        chkTimeEater.Checked = true;
    }

    private void DiscordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        System.Diagnostics.Process.Start("https://discord.gg/M6CyCfC6M7");
    }

    private void ShadowMiscButton_Click(object sender, EventArgs e)
    {
        chkShadowNewGameStart.Checked = chkShadowReset.Checked = chkShadowLevelEnterStart.Checked = true;
        chkShadowFocusPatch.Checked = false;
        chkShadowLoadless.Checked = true;
        chkBiolizard.Checked = chkBiolizard_Hard.Checked = true;
        chkMetalOverlord.Checked = chkMetalOverlord_Hard.Checked = true;
        chkMephiles.Checked = chkMephiles_Hard.Checked = true;
        chkBlackDoom.Checked = true;
        chkTokio1.Checked = chkTokio1_1.Checked = chkTokio1_2.Checked = true;
    }

    private void ShadowButton_Click(object sender, EventArgs e)
    {
        chkSpaceColonyArk1.Checked = chkSpaceColonyArk1_1.Checked = chkSpaceColonyArk1_2.Checked = chkSpaceColonyArk1_Hard.Checked = true;
        chkSpaceColonyArk2.Checked = chkSpaceColonyArk2_1.Checked = chkSpaceColonyArk2_2.Checked = chkSpaceColonyArk2_Hard.Checked = true;
        chkRailCanyon1.Checked = chkRailCanyon1_1.Checked = chkRailCanyon1_2.Checked = chkRailCanyon1_Hard.Checked = true;
        chkRailCanyon2.Checked = chkRailCanyon2_1.Checked = chkRailCanyon2_2.Checked = chkRailCanyon2_Hard.Checked = true;
        chkKingdomValley1.Checked = chkKingdomValley1_1.Checked = chkKingdomValley1_2.Checked = chkKingdomValley1_Hard.Checked = true;
        chkKingdomValley2.Checked = chkKingdomValley2_1.Checked = chkKingdomValley2_2.Checked = chkKingdomValley2_Hard.Checked = true;
        chkSunsetHeights1.Checked = chkSunsetHeights1_1.Checked = chkSunsetHeights1_2.Checked = chkSunsetHeights1_Hard.Checked = true;
        chkSunsetHeights2.Checked = chkSunsetHeights2_1.Checked = chkSunsetHeights2_2.Checked = chkSunsetHeights2_Hard.Checked = true;
        chkChaosIsland1.Checked = chkChaosIsland1_1.Checked = chkChaosIsland1_2.Checked = chkChaosIsland1_Hard.Checked = true;
        chkChaosIsland2.Checked = chkChaosIsland2_1.Checked = chkChaosIsland2_2.Checked = chkChaosIsland2_Hard.Checked = true;
        chkRadicalHighway1.Checked = chkRadicalHighway2.Checked = true;
    }
}