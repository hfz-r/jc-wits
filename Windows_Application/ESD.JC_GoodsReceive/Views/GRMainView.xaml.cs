using ESD.JC_GoodsReceive.ViewModels;
using ESD.JC_Infrastructure;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRMainView.xaml
    /// </summary>
    public partial class GRMainView : UserControl
    {
        private GRMainViewModel viewModel;

        public GRMainView(GRMainViewModel _viewModel)
        {
            InitializeComponent();

            DataContext = _viewModel;
            viewModel = _viewModel;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        private void btnOkFilter_Click(object sender, RoutedEventArgs e)
        {
            okPopupSelection.IsOpen = true;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<OkCategory> item in viewModel.OkFilter)
            {
                item.IsChecked = true;
            }
        }

        private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<OkCategory> item in viewModel.OkFilter)
            {
                item.IsChecked = false;
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(viewModel.GoodReceives).Refresh();
        }
    }
}