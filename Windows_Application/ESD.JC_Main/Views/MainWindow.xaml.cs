using ESD.JC_Infrastructure;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows;

namespace ESD.JC_Main.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string userMgmtModuleName = "UserMgmtModule";
        private static Uri mainViewUri = new Uri("UserMainView", UriKind.Relative);

        IModuleManager ModuleManager;
        IRegionManager RegionManager;

        public MainWindow(IModuleManager _ModuleManager, IRegionManager _RegionManager)
        {
            InitializeComponent();

            ModuleManager = _ModuleManager;
            RegionManager = _RegionManager;

            this.ModuleManager.LoadModuleCompleted += (s, e) =>
            {
                if (e.ModuleInfo.ModuleName == userMgmtModuleName)
                {
                    this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
                }
            };
        }
    }
}
