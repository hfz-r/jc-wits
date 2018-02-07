using System.Windows.Controls;
using ESD.JC_RoleMgmt.ViewModels;
using System.Windows;

namespace ESD.JC_RoleMgmt.Views
{
    /// <summary>
    /// Interaction logic for RoleMainView.xaml
    /// </summary>
    public partial class RoleMainView : UserControl
    {
        public RoleMainView(RoleMainViewViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void ToolBar_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
