using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;
using System.Windows;
using System.Windows.Data;
using ESD.JC_Infrastructure;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGahuView.xaml
    /// </summary>
    public partial class FGahuView : UserControl
    {
        private FGahuViewModel viewModel;

        public FGahuView(FGahuViewModel _viewModel)
        {
            InitializeComponent();

            DataContext = _viewModel;
            viewModel = _viewModel;
        }

        private void btnAhuStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ahuStatusPopupSelection.IsOpen = true;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<AHUStatusCategory> item in viewModel.AhuStatusFilter)
            {
                item.IsChecked = true;
            }
        }

        private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<AHUStatusCategory> item in viewModel.AhuStatusFilter)
            {
                item.IsChecked = false;
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(viewModel.AHU).Refresh();
        }
    }
}
