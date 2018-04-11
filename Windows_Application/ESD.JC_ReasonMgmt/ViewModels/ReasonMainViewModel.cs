using ESD.JC_Infrastructure.Events;
using ESD.JC_ReasonMgmt.Services;
using ESD.JC_ReasonMgmt.ModelsExt;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Input;
using ESD.JC_Infrastructure;
using System.Windows;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_ReasonMgmt.ViewModels
{
    public class ReasonMainViewModel : BindableBase
    {
        private ObservableCollection<ReasonExt> _Reasons;
        public ObservableCollection<ReasonExt> Reasons
        {
            get { return _Reasons; }
            set { SetProperty(ref _Reasons, value); }
        }

        private ReasonExt _SelectedReason;
        public ReasonExt SelectedReason
        {
            get { return _SelectedReason; }
            set { SetProperty(ref _SelectedReason, value); }
        }

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set
            {
                _cellInfo = value;
                if (_cellInfo != null)
                {
                    _deleteReasonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                if (Reasons != null)
                    CollectionViewSource.GetDefaultView(Reasons).Refresh();
            }
        }

        private int _ItemCount;
        public int ItemCount
        {
            get { return _ItemCount; }
            set { SetProperty(ref _ItemCount, value); }
        }

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IReasonServices ReasonServices;
        private DelegateCommand<object> _saveReasonCommand;
        private DelegateCommand<object> _deleteReasonCommand;
        private InteractionRequest<Confirmation> interactionRequest;

        public ReasonMainViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IReasonServices _ReasonServices)
        {
            RegionManager = _RegionManager;
            ReasonServices = _ReasonServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);

            _saveReasonCommand = new DelegateCommand<object>(SaveReason);
            _deleteReasonCommand = new DelegateCommand<object>(DeleteReason, CanDelete);
            interactionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand SaveReasonCommand
        {
            get { return this._saveReasonCommand; }
        }
        public ICommand DeleteReasonCommand
        {
            get { return this._deleteReasonCommand; }
        }
        public IInteractionRequest InteractionRequest
        {
            get { return this.interactionRequest; }
        }

        private void OnLoaded()
        {
            Reasons = new ObservableCollection<ReasonExt>();
            Reasons.CollectionChanged += Reasons_CollectionChanged;

            foreach (var r in ReasonServices.GetAll())
            {
                Reasons.Add(new ReasonExt
                {
                    ID = r.ID,
                    ReasonDesc = r.ReasonDesc,
                    ModifiedOn = r.ModifiedOn,
                    ModifiedBy = r.ModifiedBy
                });
            }

            CollectionViewSource.GetDefaultView(Reasons).Filter = ReasonFilter;

            Reasons = SequencingService.SetCollectionSequence(Reasons);
            RaisePropertyChanged("Reasons");
        }

        private void Reasons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemCount = Reasons.Count;
            SequencingService.SetCollectionSequence(Reasons);
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private bool ReasonFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var reason = (ReasonExt)item;
            if (reason.ID == 0)
                return true;

            return (reason.ReasonDesc.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void SaveReason(object ignored)
        {
            this.interactionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to save this?",
                        Title = "Confirm"
                    },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            if (InitSave())
                                OnLoaded();
                        }
                    });
        }

        private void DeleteReason(object ignored)
        {
            if (SelectedReason == null)
                return;

            if (SelectedReason.ID != 0)
            {
                this.interactionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to remove this?",
                        Title = "Confirm"
                    },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            if (InitDelete(SelectedReason.ID))
                                OnLoaded();
                        }

                    });
            }
            else
            {
                if (!string.IsNullOrEmpty(SelectedReason.ReasonDesc))
                {
                    Reasons.Remove(SelectedReason);
                }
            }
        }

        private bool InitSave()
        {
            bool ok = false;

            List<ReasonExt> toSaveList = new List<ReasonExt>();
            List<ReasonExt> toUpdateList = new List<ReasonExt>();
            foreach (var rsn in Reasons.ToList())
            {
                if (rsn.ID == 0 && !string.IsNullOrEmpty(rsn.ReasonDesc))
                    toSaveList.Add(rsn);
                else if (rsn.ID != 0)
                    toUpdateList.Add(rsn);
            }

            try
            {
                List<Reason> addObj = Add(toSaveList);
                if (addObj.Count() > 0)
                    ok = ReasonServices.Save(addObj, "Save");

                List<Reason> updateObj = Update(toUpdateList);
                if (updateObj.Count() > 0)
                    ok = ReasonServices.Save(updateObj, "Update");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }

        private List<Reason> Add(List<ReasonExt> toSaveList)
        {
            List<Reason> addObj = new List<Reason>();

            foreach (var o in toSaveList)
            {
                var reason = new Reason
                {
                    ReasonDesc = o.ReasonDesc,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = o.ModifiedOn,
                    ModifiedBy = AuthenticatedUser
                };
                addObj.Add(reason);
            }
            return addObj;
        }

        private List<Reason> Update(List<ReasonExt> toUpdateList)
        {
            List<Reason> updateObj = new List<Reason>();

            foreach (var u in toUpdateList)
            {
                var reason = ReasonServices.GetReason(u.ID);
                if (reason != null && reason.ReasonDesc != u.ReasonDesc)
                {
                    reason.ReasonDesc = u.ReasonDesc;
                    reason.ModifiedOn = DateTime.Now;
                    reason.ModifiedBy = AuthenticatedUser;
                }
                updateObj.Add(reason);
            }
            return updateObj;
        }

        private bool CanDelete(object ignored)
        {
            if (SelectedReason == null)
                return false;
            if (CellInfo.Item.ToString().Contains("NewItemPlaceholder"))
                return false;

            return true;
        }

        private bool InitDelete(long ID)
        {
            bool ok = false;

            try
            {
                if (ID != 0)
                {
                    ok = ReasonServices.Delete(ID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }
    }
}
