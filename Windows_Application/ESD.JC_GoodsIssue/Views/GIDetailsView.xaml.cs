using System.Windows.Controls;
using ESD.JC_GoodsIssue.ViewModels;

namespace ESD.JC_GoodsIssue.Views
{
    /// <summary>
    /// Interaction logic for GIDetailsView.xaml
    /// </summary>
    public partial class GIDetailsView : UserControl
    {
        public GIDetailsView(GIDetailsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
