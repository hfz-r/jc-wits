using System;
using System.Windows.Controls;
using DataLayer;
using ESD.JC_GoodsReceive.ViewModels;
using Prism.Regions;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRDetailsSummaryView.xaml
    /// </summary>
    public partial class GRDetailsSummaryView : UserControl
    {
        public GRDetailsSummaryView(GRDetailsSummaryViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
