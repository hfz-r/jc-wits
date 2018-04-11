using System.Windows.Controls;
using ESD.JC_RoleMgmt.ViewModels;

namespace ESD.JC_RoleMgmt.Views
{
    /// <summary>
    /// Interaction logic for RoleDetailsModuleView.xaml
    /// </summary>
    public partial class RoleDetailsModuleView : UserControl
    {
        public RoleDetailsModuleView(RoleDetailsModuleViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
