using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGahuView.xaml
    /// </summary>
    public partial class FGahuView : UserControl
    {
        private bool expanded = false;

        public FGahuView(FGahuViewModel viewModel)
        {
            InitializeComponent();
            AHU_Loaded();

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

        private void AHU_Loaded()
        {
            sidePanel.BeginStoryboard((Storyboard)this.Resources["expandStoryBoard"]);
            sidePanel.BeginStoryboard((Storyboard)this.Resources["collapseStoryBoard"]);
        }
    }
}
