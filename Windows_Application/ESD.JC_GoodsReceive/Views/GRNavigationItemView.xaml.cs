using ESD.JC_Infrastructure;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("07")]
    public partial class GRNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("GRMainView", UriKind.Relative);

        public IRegionManager regionManager;

        public GRNavigationItemView(IRegionManager _regionManager)
        {
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
            this.NavigateToGRRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToGRRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
