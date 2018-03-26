using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGfcuView.xaml
    /// </summary>
    public partial class FGfcuView : UserControl
    {
        private bool expanded = false;

        public FGfcuView(FGfcuViewModel viewModel)
        {
            InitializeComponent();
            FCU_Loaded();

            DataContext = viewModel;
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

        private void FCU_Loaded()
        {
            sidePanel.BeginStoryboard((Storyboard)this.Resources["expandStoryBoard"]);
            sidePanel.BeginStoryboard((Storyboard)this.Resources["collapseStoryBoard"]);
        }
    }
}
