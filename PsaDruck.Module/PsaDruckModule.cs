using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PsaDruck.UI.Views;

namespace PsaDruck.Module
{
    public class PsaDruckModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(PsaDruckView));
        }
    }
}