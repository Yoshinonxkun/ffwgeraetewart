using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Launchpad.UI.ViewModels
{
    public class LaunchpadViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public LaunchpadViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        public DelegateCommand<string> NavigateCommand { get; }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("MainRegion", navigatePath);
        }
    }
}