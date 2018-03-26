using ESD.JC_GoodsReceive.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRMainView.xaml
    /// </summary>
    public partial class GRMainView : UserControl
    {
        private bool expanded = false;

        public GRMainView(GRMainViewModel viewModel)
        {
            InitializeComponent();
            GR_Loaded();

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

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Handle single leftbutton mouse clicks
            if (e.ClickCount < 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (expanded == false)
                    sidePanel.BeginStoryboard((Storyboard)this.Resources["expandStoryBoard"]);
                else
                    sidePanel.BeginStoryboard((Storyboard)this.Resources["collapseStoryBoard"]);

                expanded = !expanded;
            }
        }

        private void GR_Loaded()
        {
            sidePanel.BeginStoryboard((Storyboard)this.Resources["expandStoryBoard"]);
            sidePanel.BeginStoryboard((Storyboard)this.Resources["collapseStoryBoard"]);
        }
    }
}