using System;
using System.Reflection;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.SonicXShadowGenerations;

[assembly: ComponentFactory(typeof(AutosplitterComponentFactory))]
namespace LiveSplit.SonicXShadowGenerations
{
    public class AutosplitterComponentFactory : IComponentFactory
    {
        public string ComponentName => "Sonic X Shadow Generations - Autosplitter";
        public string Description => "Automatic splitting and Game Time calculation";
        public ComponentCategory Category => ComponentCategory.Control;
        public string UpdateName => this.ComponentName;
        public string UpdateURL => "https://raw.githubusercontent.com/SonicSpeedrunning/LiveSplit.SonicXShadowGenerations/master/";
        public string XMLURL => this.UpdateURL + "Components/update.LiveSplit.SonicXShadowGenerations.xml";
        public IComponent Create(LiveSplitState state) => new AutosplitterComponent(state);

        public Version Version
        {
            get
            {
                _version ??= Assembly.GetExecutingAssembly().GetName().Version;
                return _version;
            }
        }
        public Version? _version = null;
    }
}