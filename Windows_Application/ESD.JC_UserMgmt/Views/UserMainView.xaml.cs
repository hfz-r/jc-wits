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
        UserMainViewModel m_ViewModel;

        public UserMainView(UserMainViewModel viewModel)
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

        private void OnMainGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = (UserMainViewModel)this.DataContext;
        }

        //private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    var aa = ((PasswordBox)sender).Password;
        //}
    }
}
