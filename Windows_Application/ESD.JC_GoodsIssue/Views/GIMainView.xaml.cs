using ESD.JC_GoodsIssue.ViewModels;
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

        public GIMainView(GIMainViewModel viewModel, IEventAggregator EventAggregator)
        {
            InitializeComponent();

            DataContext = viewModel;

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
    }
}
