using ESD.JC_GoodsIssue.ViewModels;
using System.Windows.Controls;

namespace ESD.JC_GoodsIssue.Views
{
    /// <summary>
    /// Interaction logic for GIDetailsTransactionView.xaml
    /// </summary>
    public partial class GIDetailsTransactionView : UserControl
    {
        public GIDetailsTransactionView(GIDetailsTransactionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
