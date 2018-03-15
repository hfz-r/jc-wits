using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Regions;
using ESD.JC_Infrastructure;

namespace ESD.JC_CountryMgmt.Views
{
    /// <summary>
    /// Interaction logic for CountryMgmtNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("04")]
    public partial class CountryMgmtNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("CountryMainView", UriKind.Relative);

        public IRegionManager regionManager;

        public CountryMgmtNavigationItemView(IRegionManager _regionManager)
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
            this.NavigateToCountryMgmtRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToCountryMgmtRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
