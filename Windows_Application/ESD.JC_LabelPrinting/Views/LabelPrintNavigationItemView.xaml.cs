using ESD.JC_Infrastructure;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_LabelPrinting.Views
{
    /// <summary>
    /// Interaction logic for LabelPrintNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("04")]
    public partial class LabelPrintNavigationItemView : UserControl
    {
        private static Uri mainViewUri = new Uri("LabelPrintMainView", UriKind.Relative);
        //private static Uri mainViewUri = new Uri("LblPrntView", UriKind.Relative);

        public IRegionManager regionManager;

        public LabelPrintNavigationItemView(IRegionManager _regionManager)
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
            this.NavigateToLabelPrintRadioButton.IsChecked = (uri == mainViewUri);
        }

        private void NavigateToLabelPrintRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, mainViewUri);
        }
    }
}
