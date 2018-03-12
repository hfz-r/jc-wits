using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGahuDetailsView.xaml
    /// </summary>
    public partial class FGahuDetailsView : UserControl
    {
        public FGahuDetailsView(FGahuDetailsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
