using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FCUDetailsTransactionView.xaml
    /// </summary>
    public partial class FCUDetailsTransactionView : UserControl
    {
        public FCUDetailsTransactionView(FCUDetailsTransactionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
