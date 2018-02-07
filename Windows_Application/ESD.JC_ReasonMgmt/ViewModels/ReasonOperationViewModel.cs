using DataLayer;
using ESD.JC_ReasonMgmt.Services;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace ESD.JC_ReasonMgmt.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ReasonOperationViewModel : BindableBase, IConfirmNavigationRequest
    {
        #region Properties

        private Reason _ReasonData;
        public Reason ReasonData
        {
            get { return _ReasonData; }
            set { SetProperty(ref _ReasonData, value); }
        }

        private string _ReasonDescAlias;
        public string ReasonDescAlias
        {
            get { return _ReasonDescAlias; }
            set
            {
                SetProperty(ref _ReasonDescAlias, value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                SetProperty(ref _IsEnabled, value);
                RaisePropertyChanged("IsEnabled");
            }
        }

        private string _SendState;
        public string SendState
        {
            get { return _SendState; }
            set { SetProperty(ref _SendState, value); }
        }

        private string _AuthenticatedUser;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set
            {
                SetProperty(ref _AuthenticatedUser, value);
                RaisePropertyChanged("AuthenticatedUser");
            }
        }

        #endregion

        private const string NormalStateKey = "Normal";
        private const string SavingStateKey = "Saving";
        private const string SavedStateKey = "Saved";

        private IRegionNavigationJournal navigationJournal;
        private IReasonServices ReasonServices;
        private InteractionRequest<Confirmation> confirmExitInteractionRequest;
        private DelegateCommand<object> _saveCommand;

        public ReasonOperationViewModel(IReasonServices _ReasonServices)
        {
            ReasonServices = _ReasonServices;
            SendState = NormalStateKey;

            CancelCommand = new DelegateCommand(Cancel);
            _saveCommand = new DelegateCommand<object>(Save, CanSave);
            confirmExitInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public ICommand CancelCommand { get; private set; }
        public ICommand SaveCommand
        {
            get { return this._saveCommand; }
        }
        public IInteractionRequest ConfirmExitInteractionRequest
        {
            get { return this.confirmExitInteractionRequest; }
        }

        private void Save(object ignored)
        {
            SendState = SavingStateKey;

            var data = this.ReasonData;

            try
            {
                var reason = ReasonServices.GetReason(data.ID);
                if (reason != null)
                {
                    reason.ReasonDesc = ReasonDescAlias;
                    reason.ModifiedOn = DateTime.Now;
                    reason.ModifiedBy = AuthenticatedUser;
                }
                else
                {
                    reason = new Reason();
                    reason.ReasonDesc = ReasonDescAlias;
                    reason.CreatedOn = DateTime.Now;
                    reason.CreatedBy = AuthenticatedUser;
                    reason.ModifiedOn = DateTime.Now;
                    reason.ModifiedBy = AuthenticatedUser;
                }

                if (ReasonServices.Save(reason))
                {
                    this.SendState = SavedStateKey;
                    if (this.navigationJournal != null)
                    {
                        this.navigationJournal.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);

                this.SendState = NormalStateKey;
            }
        }

        private bool CanSave(object ignored)
        {
            return !string.IsNullOrEmpty(ReasonDescAlias);
        }

        private void Cancel()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private long? GetRequestedReasonID(NavigationContext navigationContext)
        {
            var uid = navigationContext.Parameters["ID"];            
            if (uid != null)
            {
                return long.Parse(uid.ToString());
            }

            return null;
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (this.SendState == NormalStateKey)
            {
                this.confirmExitInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you sure you want to navigate away from this window?",
                        Title = "Confirm"
                    },
                    c => { continuationCallback(c.Confirmed); });
            }
            else
            {
                continuationCallback(true);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var reasondata = new Reason();

            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ?
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedReasonID(navigationContext);
            if (id.HasValue)
            {
                var reason = ReasonServices.GetReason(id.Value);
                if (reason != null)
                {
                    if (!string.IsNullOrEmpty(ReasonDescAlias))
                    {
                        IsEnabled = false;
                    }

                    ReasonDescAlias = reason.ReasonDesc;
                    reasondata.ID = reason.ID;
                    reasondata.ReasonDesc = reason.ReasonDesc;
                    reasondata.ModifiedOn = reason.ModifiedOn;
                    reasondata.ModifiedBy = reason.ModifiedBy;
                }
            }
            else
            {
                IsEnabled = true;
            }

            this.ReasonData = reasondata;

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
