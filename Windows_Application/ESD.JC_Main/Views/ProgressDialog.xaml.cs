using System;
using System.Windows;

namespace ESD.JC_Main.Views
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }
        
        public int ProgressValue
        {
            set
            {
                this.ProgressBar.Value = value;
            }
        }
    }
}
