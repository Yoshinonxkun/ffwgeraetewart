using Launchpad.UI.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Launchpad.Module
{
    public class LaunchpadModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(LaunchpadView));
        }
    }
}