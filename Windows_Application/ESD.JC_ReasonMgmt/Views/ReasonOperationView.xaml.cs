using System.Windows.Controls;
using ESD.JC_ReasonMgmt.ViewModels;
using System.Security.Permissions;

namespace ESD.JC_ReasonMgmt.Views
{
    /// <summary>
    /// Interaction logic for ReasonOperationView.xaml
    /// </summary>
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class ReasonOperationView : UserControl
    {
        public ReasonOperationView(ReasonOperationViewModel viewModel)
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
