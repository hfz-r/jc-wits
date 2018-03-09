using Prism.Unity;
using Prism.Mvvm;
using Prism.Modularity;
using DataLayer.Repositories;
using ESD.JC_Main.Views;
using ESD.JC_Main.LoginServices;
using ESD.JC_Main.ViewModels;
using ESD.JC_RoleMgmt;
using ESD.JC_UserMgmt;
using ESD.JC_ReasonMgmt;
using ESD.JC_GoodsReceive;
using ESD.JC_LabelPrinting;
using System.Windows;
using System.Threading;
using Microsoft.Practices.Unity;
using ESD.JC_GoodsIssue;
using ESD.JC_FinishGoods;

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
            moduleCatalog.AddModule(typeof(ReasonMgmtModule));
            moduleCatalog.AddModule(typeof(GRModule));
            moduleCatalog.AddModule(typeof(LabelPrintingModule));
            moduleCatalog.AddModule(typeof(GIModule));
            moduleCatalog.AddModule(typeof(FGModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAuthenticationService, AuthenticationService>(new ContainerControlledLifetimeManager());
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<LoginWindow>(() => Container.TryResolve<LoginWindowViewModel>());
            ViewModelLocationProvider.Register<MainWindow>(() => Container.TryResolve<MainWindowViewModel>());
        }
    }
}
