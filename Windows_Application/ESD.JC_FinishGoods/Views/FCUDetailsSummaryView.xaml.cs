using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FCUDetailsSummaryView.xaml
    /// </summary>
    public partial class FCUDetailsSummaryView : UserControl
    {
        public FCUDetailsSummaryView(FCUDetailsSummaryViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
