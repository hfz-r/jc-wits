using ESD.JC_UserMgmt.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ESD.JC_UserMgmt.Views
{
    /// <summary>
    /// Interaction logic for UserMainView.xaml
    /// </summary>
    public partial class UserMainView : UserControl
    {
        public UserMainView(UserMainViewViewModel viewModel)
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
