using System;
using System.Windows.Controls;
using DataLayer;
using ESD.JC_GoodsReceive.ViewModels;
using Prism.Regions;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRDetailsTransactionView.xaml
    /// </summary>
    public partial class GRDetailsTransactionView : UserControl
    {
        public GRDetailsTransactionView(GRDetailsTransactionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
