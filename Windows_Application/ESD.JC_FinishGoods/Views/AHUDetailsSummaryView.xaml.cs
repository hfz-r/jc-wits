using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for AHUDetailsSummaryView.xaml
    /// </summary>
    public partial class AHUDetailsSummaryView : UserControl
    {
        public AHUDetailsSummaryView(AHUDetailsSummaryViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
