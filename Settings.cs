using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SonicXShadowGenerations;

public partial class Settings : UserControl
{
    // General
    public bool WFocus { get; set; }
    public bool StoryStart { get; set; }
    public bool ArcadeStart { get; set; }
    public bool Arcade1_1 { get; set; }
    public bool BossRushStart { get; set; }

    // Skills
    public bool Skill_Cyloop { get; set; }
    public bool Skill_PhantomRush { get; set; }
    public bool Skill_AirTrick { get; set; }
    public bool Skill_StompAttack { get; set; }
    public bool Skill_SonicBoom { get; set; }
    public bool Skill_AutoCombo { get; set; }
    public bool Skill_WildRush { get; set; }
    public bool Skill_QuickCyloop { get; set; }
    public bool Skill_HomingShot { get; set; }
    public bool Skill_SpinSlash { get; set; }
    public bool Skill_LoopKick { get; set; }
    public bool Skill_RecoverySmash { get; set; }

    // Story - Kronos
    public bool Kronos_Ninja { get; set; }
    public bool Kronos_Door { get; set; }
    public bool Kronos_Amy { get; set; }
    public bool Kronos_GigantosFirst { get; set; }
    public bool Kronos_Tombstones { get; set; }
    public bool Kronos_GigantosStart { get; set; }
    public bool Kronos_SuperSonic { get; set; }
    public bool Island_Kronos_story { get; set; }
    public bool w1_1_story { get; set; }
    public bool w1_2_story { get; set; }
    public bool w1_3_story { get; set; }
    public bool w1_4_story { get; set; }
    public bool w1_5_story { get; set; }
    public bool w1_6_story { get; set; }
    public bool w1_7_story { get; set; }
    public bool Kronos_BlueCE { get; set; }
    public bool Kronos_RedCE { get; set; }
    public bool Kronos_YellowCE { get; set; }
    public bool Kronos_WhiteCE { get; set; }
    public bool Kronos_GreenCE { get; set; }
    public bool Kronos_CyanCE { get; set; }
    public bool Island_Kronos_fishing { get; set; }

    // Story - Ares
    public bool Ares_Knuckles { get; set; }
    public bool Ares_WyvernFirst { get; set; }
    public bool Ares_Water { get; set; }
    public bool Ares_Crane { get; set; }
    public bool Ares_GreenCE { get; set; }
    public bool Ares_CyanCE { get; set; }
    public bool Ares_WyvernStart { get; set; }
    public bool Ares_WyvernRun { get; set; }
    public bool Ares_SuperSonic { get; set; }
    public bool Island_Ares_story { get; set; }
    public bool w2_1_story { get; set; }
    public bool w2_2_story { get; set; }
    public bool w2_3_story { get; set; }
    public bool w2_4_story { get; set; }
    public bool w2_5_story { get; set; }
    public bool w2_6_story { get; set; }
    public bool w2_7_story { get; set; }
    public bool Ares_BlueCE { get; set; }
    public bool Ares_RedCE { get; set; }
    public bool Ares_YellowCE { get; set; }
    public bool Ares_WhiteCE { get; set; }
    public bool Island_Ares_fishing { get; set; }
    
    public Settings()
    {
        InitializeComponent();
        autosplitterVersion.Text = "Autosplitter version: v" + Assembly.GetExecutingAssembly().GetName().Version;

        // General settings
        chkFocus.DataBindings.Add("Checked", this, "WFocus", false, DataSourceUpdateMode.OnPropertyChanged);
        chkStoryStart.DataBindings.Add("Checked", this, "StoryStart", false, DataSourceUpdateMode.OnPropertyChanged);
        chkArcadeStart.DataBindings.Add("Checked", this, "ArcadeStart", false, DataSourceUpdateMode.OnPropertyChanged);
        chkBossRushStart.DataBindings.Add("Checked", this, "BossRushStart", false, DataSourceUpdateMode.OnPropertyChanged);
        chkArcade1_1.DataBindings.Add("Checked", this, "Arcade1_1", false, DataSourceUpdateMode.OnPropertyChanged);

        // Default Values
        WFocus = false;
        StoryStart = ArcadeStart = Arcade1_1 = true;
        BossRushStart = true;
        
     
        //Skills
        Skill_Cyloop = Skill_AirTrick = Skill_PhantomRush = Skill_StompAttack = Skill_AutoCombo = Skill_HomingShot = Skill_LoopKick = Skill_QuickCyloop = Skill_RecoverySmash = Skill_SonicBoom = Skill_SpinSlash = Skill_WildRush = false;

        // Kronos
        Kronos_Ninja = Kronos_Door = Kronos_Amy = Kronos_GigantosFirst = Kronos_Tombstones = false;
        Island_Kronos_story = Island_Kronos_fishing = true;
        w1_1_story = w1_2_story = w1_3_story = w1_4_story = w1_5_story = w1_6_story = w1_7_story = false;
        Kronos_BlueCE = Kronos_RedCE = Kronos_YellowCE = Kronos_WhiteCE = Kronos_GreenCE = Kronos_CyanCE  = Kronos_GigantosStart = Kronos_SuperSonic = false;

        // Ares
        Ares_Knuckles = Ares_WyvernFirst = Ares_Water = Ares_Crane = false;
        Ares_WyvernRun = Ares_WyvernStart = Ares_SuperSonic = false;
        Island_Ares_story = Island_Ares_fishing = true;
        w2_1_story = w2_2_story = w2_3_story = w2_4_story = w2_5_story = w2_6_story = w2_7_story = false;
        Ares_GreenCE = Ares_CyanCE = Ares_BlueCE = Ares_RedCE = Ares_YellowCE = Ares_YellowCE = false;
    }

    public XmlNode GetSettings(XmlDocument doc)
    {
        XmlElement settingsNode = doc.CreateElement("Settings");
        settingsNode.AppendChild(ToElement(doc, "WFocus", WFocus));
        settingsNode.AppendChild(ToElement(doc, "StoryStart", StoryStart));
        settingsNode.AppendChild(ToElement(doc, "ArcadeStart", ArcadeStart));
        settingsNode.AppendChild(ToElement(doc, "BossRushStart", BossRushStart));
        return settingsNode;
    }

    public void SetSettings(XmlNode settings)
    {
        WFocus = ParseBool(settings, "WFocus", false);
        StoryStart = ParseBool(settings, "StoryStart", true);
        ArcadeStart = ParseBool(settings, "ArcadeStart", true);
        BossRushStart = ParseBool(settings, "BossRushStart", true);
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

    private void ChkArcadeStart_CheckedChanged(object sender, EventArgs e)
    {
        chkArcade1_1.Enabled = chkArcadeStart.Checked;
    }

    private void KronosButton_Click(object sender, EventArgs e)
    {
        chkKronos_Ninja.Checked = false;
        chkKronos_Door.Checked = false;
        chkKronos_Amy.Checked = false;
        chkKronos_GigantosFirst.Checked = false;
        chkKronos_Tombstones.Checked = false;
        chkKronos_GigantosStart.Checked = false;
        chk50_story.Checked = true;
        chk0_story.Checked = false;
        chk1_story.Checked = false;
        chk2_story.Checked = false;
        chk3_story.Checked = false;
        chk4_story.Checked = false;
        chk5_story.Checked = false;
        chk6_story.Checked = false;
        chkKronos_BlueCE.Checked = false;
        chkKronos_RedCE.Checked = false;
        chkKronos_YellowCE.Checked = false;
        chkKronos_WhiteCE.Checked = false;
        chkKronos_GreenCE.Checked = false;
        chkKronos_CyanCE.Checked = false;
        chk50_fishing.Checked = true;
    }

    private void DiscordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        System.Diagnostics.Process.Start("https://discord.gg/M6CyCfC6M7");
    }
}