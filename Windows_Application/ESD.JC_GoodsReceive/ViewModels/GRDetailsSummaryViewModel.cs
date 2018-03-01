﻿using DataLayer;
using ESD.JC_GoodsReceive.Services;
using ESD.JC_Infrastructure;
using Prism.Mvvm;
using Prism.Regions;
using System;
using Microsoft.Practices.Unity;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using System.Collections.ObjectModel;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class GRDetailsSummaryViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Transaction Summary"; }
        }

        private ObservableCollection<GRTransaction> _summryCollection;
        public ObservableCollection<GRTransaction> summryCollection
        {
            get { return _summryCollection; }
            set
            {
                SetProperty(ref _summryCollection, value);
                if (summryCollection.Count() > 0)
                {
                    var grObj = summryCollection.Select(x => x.GoodsReceive).FirstOrDefault();
                    RemainingQty = grObj.Quantity - grObj.QtyReceived.GetValueOrDefault();
                }

                RaisePropertyChanged("summryCollection");
            }
        }

        private decimal _RemainingQty;
        public decimal RemainingQty
        {
            get { return _RemainingQty; }
            set
            {
                SetProperty(ref _RemainingQty, value);
                RaisePropertyChanged("RemainingQty");
            }
        }

        private IUnityContainer Container;
        private IRegionManager RegionManager;
        private IGRTransactionServices GRTransactionServices;

        public GRDetailsSummaryViewModel(IUnityContainer _Container, IRegionManager _RegionManager, IGRTransactionServices _GRTransactionServices)
        {
            Container = _Container;
            RegionManager = _RegionManager;
            GRTransactionServices = _GRTransactionServices;
        }
    }
}
