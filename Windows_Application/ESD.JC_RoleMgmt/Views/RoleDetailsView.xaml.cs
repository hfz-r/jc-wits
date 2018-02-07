using ESD.JC_RoleMgmt.ViewModels;
using System.Windows.Controls;

namespace ESD.JC_RoleMgmt.Views
{
    /// <summary>
    /// Interaction logic for RoleDetailsView.xaml
    /// </summary>
    public partial class RoleDetailsView : UserControl
    {
        public RoleDetailsView(RoleDetailsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
