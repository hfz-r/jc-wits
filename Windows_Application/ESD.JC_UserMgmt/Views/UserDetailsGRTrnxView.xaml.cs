using System.Windows.Controls;
using ESD.JC_UserMgmt.ViewModels;

namespace ESD.JC_UserMgmt.Views
{
    /// <summary>
    /// Interaction logic for UserDetailsGRTrnxView.xaml
    /// </summary>
    public partial class UserDetailsGRTrnxView : UserControl
    {
        public UserDetailsGRTrnxView(UserDetailsGRTrnxViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
