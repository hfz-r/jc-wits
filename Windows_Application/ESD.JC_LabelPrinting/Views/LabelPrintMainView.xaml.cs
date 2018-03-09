using System.Windows.Controls;
using ESD.JC_LabelPrinting.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_LabelPrinting.Views
{
    /// <summary>
    /// Interaction logic for LabelPrintMainView.xaml
    /// </summary>
    public partial class LabelPrintMainView : UserControl
    {
        public LabelPrintMainView(LabelPrintMainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
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

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //FocusExtensions.SetFocusOnActiveElementInScope(MainControl);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control ctl in GridLP.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
            }
        }
    }
}
