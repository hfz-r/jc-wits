using System.Windows.Controls;
using ESD.JC_RoleMgmt.ViewModels;
using System.Security.Permissions;

namespace ESD.JC_RoleMgmt.Views
{
    /// <summary>
    /// Interaction logic for RoleOperationView.xaml
    /// </summary>
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class RoleOperationView : UserControl
    {
        public RoleOperationView(RoleOperationViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }
    }
}
