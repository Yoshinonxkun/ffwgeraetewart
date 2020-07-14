using Member.UI.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Member.Module
{
    public class MemberModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(MemberView));
        }
    }
}