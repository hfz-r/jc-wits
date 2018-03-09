using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGahuView.xaml
    /// </summary>
    public partial class FGahuView : UserControl
    {
        public FGahuView(FGahuViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
