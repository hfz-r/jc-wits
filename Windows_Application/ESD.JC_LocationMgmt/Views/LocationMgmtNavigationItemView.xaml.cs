using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Regions;
using ESD.JC_Infrastructure;

namespace ESD.JC_LocationMgmt.Views
{
    /// <summary>
    /// Interaction logic for LocationMgmtNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("03")]
    public partial class LocationMgmtNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("LocationMainView", UriKind.Relative);

        public IRegionManager regionManager;

        public LocationMgmtNavigationItemView(IRegionManager _regionManager)
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
            this.NavigateToLocationMgmtRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToLocationMgmtRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
