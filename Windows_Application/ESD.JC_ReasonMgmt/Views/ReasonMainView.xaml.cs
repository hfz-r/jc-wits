using System.Windows.Controls;
using ESD.JC_ReasonMgmt.ViewModels;
using ESD.JC_ReasonMgmt.ModelsExt;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace ESD.JC_ReasonMgmt.Views
{
    /// <summary>
    /// Interaction logic for ReasonMainView.xaml
    /// </summary>
    public partial class ReasonMainView : UserControl
    {
        private ReasonMainViewViewModel m_ViewModel;

        public ReasonMainView(ReasonMainViewViewModel viewModel)
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
            m_ViewModel = (ReasonMainViewViewModel)this.DataContext;
        }
    }
}
