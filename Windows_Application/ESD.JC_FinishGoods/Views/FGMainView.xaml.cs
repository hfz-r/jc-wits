using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;
using System.Windows;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGMainView.xaml
    /// </summary>
    public partial class FGMainView : UserControl
    {
        public FGMainView(FGMainViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
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
