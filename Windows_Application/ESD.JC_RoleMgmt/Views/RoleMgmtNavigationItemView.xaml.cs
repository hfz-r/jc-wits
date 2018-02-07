using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Regions;
using ESD.JC_Infrastructure;

namespace ESD.JC_RoleMgmt.Views
{
    /// <summary>
    /// Interaction logic for RoleMgmtNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("01")]
    public partial class RoleMgmtNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("RoleMainView", UriKind.Relative);

        public IRegionManager regionManager;

        public RoleMgmtNavigationItemView(IRegionManager _regionManager)
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
            this.NavigateToRoleMgmtRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToRoleMgmtRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
