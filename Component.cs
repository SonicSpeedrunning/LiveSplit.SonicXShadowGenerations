using System.Xml;
using System.Windows.Forms;
using System.Threading.Tasks;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.SonicXShadowGenerations;

internal partial class AutosplitterComponent : LogicComponent
{
    private Settings Settings { get; set; } = new Settings();

    public AutosplitterComponent(LiveSplitState state)
    {
        autosplitterTask = Task.Run(() =>
        {
            try
            {
                AutosplitterLogic(state, cancelToken.Token);
            }
            catch { }
        });
    }

    public override void Dispose()
    {
        cancelToken.Cancel();
        autosplitterTask?.Wait();
        autosplitterTask?.Dispose();
        Settings?.Dispose();
    }

    public override XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);
    public override Control GetSettingsControl(LayoutMode mode) => Settings;
    public override void SetSettings(XmlNode settings) => Settings.SetSettings(settings);
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
}