using ESD.JC_Infrastructure;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_GoodsIssue.Views
{
    /// <summary>
    /// Interaction logic for GINavigationItemView.xaml
    /// </summary>
    [ViewSortHint("06")]
    public partial class GINavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("GIMainView", UriKind.Relative);

        public IRegionManager regionManager;

        public GINavigationItemView(IRegionManager _regionManager)
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
            this.NavigateToGIRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToGIRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
