using System;
using System.Reflection;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace LiveSplit.KunaiSpeedTracker
{
    // ReSharper disable once UnusedMember.Global
    public class KunaiSpeedTrackerFactory : IComponentFactory
    {
        public IComponent Create(LiveSplitState state)
        {
            return new KunaiSpeedTrackerComponent(state, ComponentName);
        }

        public string UpdateName => ComponentName;
        public string XMLURL => UpdateURL + "Components/LiveSplit.KunaiSpeedTracker.Updates.xml";
        public string UpdateURL => "https://raw.githubusercontent.com/seanpr96/LiveSplit.KunaiSpeedTracker/master/";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public string ComponentName => "KUNAI Speed Tracker v" + Version;
        public string Description => "Speed tracker for KUNAI";
        public ComponentCategory Category => ComponentCategory.Information;
    }
}
