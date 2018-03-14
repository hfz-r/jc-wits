using ESD.JC_GoodsIssue.ViewModels;
using System.Windows.Controls;

namespace ESD.JC_GoodsIssue.Views
{
    /// <summary>
    /// Interaction logic for GIDetailsSummaryView.xaml
    /// </summary>
    public partial class GIDetailsSummaryView : UserControl
    {
        public GIDetailsSummaryView(GIDetailsSummaryViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
