using ESD.JC_FinishGoods.ViewModels;
using System.Windows.Controls;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for AHUDetailsTransactionView.xaml
    /// </summary>
    public partial class AHUDetailsTransactionView : UserControl
    {
        public AHUDetailsTransactionView(AHUDetailsTransactionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
