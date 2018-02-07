using ESD.JC_Infrastructure;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_UserMgmt.Views
{
    /// <summary>
    /// Interaction logic for UserMgmtNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("02")]
    public partial class UserMgmtNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("UserMainView", UriKind.Relative);

        public IRegionManager regionManager;

        public UserMgmtNavigationItemView(IRegionManager _regionManager)
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
            this.NavigateToUserMgmtRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToUserMgmtRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
