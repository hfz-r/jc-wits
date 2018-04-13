using ESD.JC_GoodsIssue.ViewModels;
using ESD.JC_GoodsReceive.ViewModels;
using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ESD.JC_GoodsIssue.Views
{
    /// <summary>
    /// Interaction logic for GIMainView.xaml
    /// </summary>
    public partial class GIMainView : UserControl
    {
        private IEventAggregator EventAggregator;
        private GIMainViewModel viewModel;

        public GIMainView(GIMainViewModel _viewModel, IEventAggregator EventAggregator)
        {
            InitializeComponent();

            DataContext = viewModel;

            DataContext = _viewModel;
            viewModel = _viewModel;

            this.EventAggregator = EventAggregator;
            this.EventAggregator.GetEvent<CollectionViewSourceEvent>().Publish((CollectionViewSource)(this.Resources["TrnxEntries"]));
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
    }
}
