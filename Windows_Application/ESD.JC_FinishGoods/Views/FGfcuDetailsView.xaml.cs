using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGfcuDetailsView.xaml
    /// </summary>
    public partial class FGfcuDetailsView : UserControl
    {
        public FGfcuDetailsView(FGfcuDetailsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
