using System.Windows;
using Launchpad.Module;
using Member.Data;
using Member.Data.Interfaces;
using Member.Module;
using Member.UI.ViewModels;
using Member.UI.Views;
using Prism.Ioc;
using Prism.Modularity;
using PsaDruck.Module;

namespace FfwGeraetewart
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return new MainWindow();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(LaunchpadModule));
            moduleCatalog.AddModule(typeof(MemberModule));
            moduleCatalog.AddModule(typeof(PsaDruckModule));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            /*
             * RegisterTypes
             * This function is used to register any app dependencies. For example, there might be an interface to
             * read customer data from a persistent store of some kind and the implementation of it is to use a
             * database of some kind. It might look something like this:
             */

            containerRegistry.RegisterSingleton<IMemberRepository, MemberRepository>();
            containerRegistry.RegisterSingleton<IPsaRepository, PsaRepository>();

            containerRegistry.RegisterDialog<MemberDialogView, MemberDialogViewModel>();
            containerRegistry.RegisterDialog<MemberDeleteDialogView, MemberDeleteDialogViewModel>();
        }
    }
}