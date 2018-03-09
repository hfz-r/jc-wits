using ESD.JC_FinishGoods.ViewModels;
using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Controls;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("07")]
    public partial class FGNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("FGMainView", UriKind.Relative);

        ICompositeCommands applicationCommands;

        private IUnityContainer container;
        public IRegionManager regionManager;

        public FGNavigationItemView(IRegionManager _regionManager, IUnityContainer _container, ICompositeCommands _applicationCommands)
        {
            applicationCommands = _applicationCommands;

            container = _container;
            regionManager = _regionManager;

            InitializeComponent();
            InitializeRegion();
        }

        public void InitializeRegion()
        {
            IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            this.UpdateNavigationButtonState(e.Uri);
        }

        private void UpdateNavigationButtonState(Uri uri)
        {
            this.NavigateToFGRadioButton.IsChecked = (uri == mainViewUri);
            if (uri != mainViewUri)
            {
                CloseViewRequested(this, EventArgs.Empty);
            }
        }

        private void NavigateToFGRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }

        private void FGNavigationItemView_CloseViewRequested(object sender, EventArgs e)
        {
            FGahuViewModel ahuViewModel = container.Resolve<FGahuViewModel>();
            FGfcuViewModel fcuViewModel = container.Resolve<FGfcuViewModel>();

            applicationCommands.ImportFGCommand.UnregisterCommand(ahuViewModel.ImportFGCommand);
            applicationCommands.ExportFGCommand.UnregisterCommand(ahuViewModel.ExportFGCommand);
            applicationCommands.PrintLblCommand.UnregisterCommand(ahuViewModel.PrintLblCommand);
            applicationCommands.OKCommand.UnregisterCommand(ahuViewModel.OKCommand);
            applicationCommands.XOKCommand.UnregisterCommand(ahuViewModel.XOKCommand);

            applicationCommands.ImportFGCommand.UnregisterCommand(fcuViewModel.ImportFGCommand);
            applicationCommands.ExportFGCommand.UnregisterCommand(fcuViewModel.ExportFGCommand);
            applicationCommands.PrintLblCommand.UnregisterCommand(fcuViewModel.PrintLblCommand);
            applicationCommands.OKCommand.UnregisterCommand(fcuViewModel.OKCommand);
            applicationCommands.XOKCommand.UnregisterCommand(fcuViewModel.XOKCommand);
        }

        public event EventHandler CloseViewRequested = delegate { };
    }
}
