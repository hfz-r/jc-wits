using DataLayer.Repositories;
using ESD.JC_Main.Views;
using ESD.JC_Main.LoginServices;
using ESD.JC_Main.ViewModels;
using ESD.JC_RoleMgmt;
using ESD.JC_RoleMgmt.Views;
using ESD.JC_UserMgmt;
using ESD.JC_UserMgmt.Views;
using ESD.JC_ReasonMgmt;
using ESD.JC_ReasonMgmt.Views;
using ESD.JC_GoodsReceive;
using ESD.JC_GoodsReceive.Views;
using ESD.JC_LabelPrinting;
using ESD.JC_LabelPrinting.Views;
using ESD.JC_GoodsIssue;
using ESD.JC_GoodsIssue.Views;
using ESD.JC_FinishGoods;
using ESD.JC_FinishGoods.Views;
using ESD.JC_CountryMgmt;
using ESD.JC_CountryMgmt.Views;
using ESD.JC_LocationMgmt;
using ESD.JC_LocationMgmt.Views;
using ESD.JC_Infrastructure.AccessControl;
using System.Windows;
using System.Threading;
using Prism.Unity;
using Prism.Mvvm;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using ESD.JC_UserMgmt.ViewModels;

namespace ESD.JC_Main
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Closed += delegate (object s, System.EventArgs e)
            {
                if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    Application.Current.MainWindow = (Window)this.Shell;
                    Application.Current.MainWindow.Show();
                }
            };

            loginWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(RoleMgmtModule));
            moduleCatalog.AddModule(typeof(UserMgmtModule));
            moduleCatalog.AddModule(typeof(LocationMgmtModule));
            moduleCatalog.AddModule(typeof(CountryMgmtModule));
            moduleCatalog.AddModule(typeof(ReasonMgmtModule));
            moduleCatalog.AddModule(typeof(LabelPrintingModule));
            moduleCatalog.AddModule(typeof(GRModule));
            moduleCatalog.AddModule(typeof(GIModule));
            moduleCatalog.AddModule(typeof(FGModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IModuleAccessCtrlTransactionRepository, ModuleAccessCtrlTransactionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAuthenticationService, AuthenticationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IModuleAccessServices, ModuleAccessServices>(new ContainerControlledLifetimeManager());
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<LoginWindow>(() => Container.TryResolve<LoginWindowViewModel>());
            ViewModelLocationProvider.Register<MainWindow>(() => Container.TryResolve<MainWindowViewModel>());
            ViewModelLocationProvider.Register<RoleMgmtNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<UserMgmtNavigationItemView>(() => Container.TryResolve<UserMainViewModel>());
            ViewModelLocationProvider.Register<LocationMgmtNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<CountryMgmtNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<ReasonMgmtNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<LabelPrintNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<GRNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<GINavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
            ViewModelLocationProvider.Register<FGNavigationItemView>(() => Container.TryResolve<AuthorizationViewModel>());
        }
    }
}
