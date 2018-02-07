using System.Windows.Controls;
using ESD.JC_GoodsReceive.ViewModels;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRDetailsView.xaml
    /// </summary>
    public partial class GRDetailsView : UserControl
    {
        public GRDetailsView(GRDetailsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
