using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for EunKGDetailsView.xaml
    /// </summary>
    public partial class EunKGDetailsView : UserControl
    {
        public EunKGDetailsView()
        {
            InitializeComponent();
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
