using System.Windows.Controls;
using ESD.JC_LocationMgmt.ViewModels;
using System.Windows;

namespace ESD.JC_LocationMgmt.Views
{
    /// <summary>
    /// Interaction logic for LocationMainView.xaml
    /// </summary>
    public partial class LocationMainView : UserControl
    {
        private LocationMainViewViewModel m_ViewModel;

        public LocationMainView(LocationMainViewViewModel viewModel)
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

        /// <summary>
        /// Gets the view model from the data Context and assigns it to a member variable.
        /// </summary>
        private void OnMainGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = (LocationMainViewViewModel)this.DataContext;
        }
    }
}
