using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;
using System.Windows;
using System.Windows.Data;
using ESD.JC_Infrastructure;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGfcuView.xaml
    /// </summary>
    public partial class FGfcuView : UserControl
    {
        private FGfcuViewModel viewModel;

        public FGfcuView(FGfcuViewModel _viewModel)
        {
            InitializeComponent();

            DataContext = _viewModel;
            viewModel = _viewModel;
        }

        private void btnFcuStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            fcuStatusPopupSelection.IsOpen = true;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<FCUStatusCategory> item in viewModel.FcuStatusFilter)
            {
                item.IsChecked = true;
            }
        }

        private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<FCUStatusCategory> item in viewModel.FcuStatusFilter)
            {
                item.IsChecked = false;
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(viewModel.FCU).Refresh();
        }
    }
}
