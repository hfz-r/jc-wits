using ESD.JC_ReasonMgmt.ViewModels;
using System.Windows.Controls;

namespace ESD.JC_ReasonMgmt.Views
{
    /// <summary>
    /// Interaction logic for ReasonDetailsView.xaml
    /// </summary>
    public partial class ReasonDetailsView : UserControl
    {
        public ReasonDetailsView(ReasonDetailsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
