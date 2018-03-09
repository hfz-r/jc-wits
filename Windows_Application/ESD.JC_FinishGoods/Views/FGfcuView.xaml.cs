using System.Windows.Controls;
using ESD.JC_FinishGoods.ViewModels;

namespace ESD.JC_FinishGoods.Views
{
    /// <summary>
    /// Interaction logic for FGfcuView.xaml
    /// </summary>
    public partial class FGfcuView : UserControl
    {
        public FGfcuView(FGfcuViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
