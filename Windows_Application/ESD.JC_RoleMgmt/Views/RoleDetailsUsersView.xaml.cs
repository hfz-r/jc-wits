using System.Windows.Controls;
using ESD.JC_RoleMgmt.ViewModels;

namespace ESD.JC_RoleMgmt.Views
{
    /// <summary>
    /// Interaction logic for RoleDetailsUsersView.xaml
    /// </summary>
    public partial class RoleDetailsUsersView : UserControl
    {
        public RoleDetailsUsersView(RoleDetailsUsersViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
