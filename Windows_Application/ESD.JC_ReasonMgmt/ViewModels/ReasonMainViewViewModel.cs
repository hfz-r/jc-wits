using DataLayer;
using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using ESD.JC_ReasonMgmt.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ESD.JC_ReasonMgmt.ViewModels
{
    public class ReasonMainViewViewModel : BindableBase
    {
        private ICollectionView _Reasons;
        public ICollectionView Reasons
        {
            get { return _Reasons; }
            set { SetProperty(ref _Reasons, value); }
        }

        private Reason _SelectedReason;
        public Reason SelectedReason
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
                    _editReasonCommand.RaiseCanExecuteChanged();
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

        private string _InteractionResultMessage;
        public string InteractionResultMessage
        {
            get { return _InteractionResultMessage; }
            set
            {
                SetProperty(ref _InteractionResultMessage, value);
                this.RaisePropertyChanged("InteractionResultMessage");
            }
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

        private const string reasonDetailsViewName = "ReasonDetailsView";
        private const string reasonOperationViewName = "ReasonOperationView";

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IReasonServices ReasonServices;
        private DelegateCommand<object> _addReasonCommand;
        private DelegateCommand<object> _editReasonCommand;
        private DelegateCommand<object> _deleteReasonCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public ReasonMainViewViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IReasonServices _ReasonServices)
        {
            RegionManager = _RegionManager;
            ReasonServices = _ReasonServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenReasonDetailsCommand = new DelegateCommand<Reason>(OpenReasonDetails);

            _addReasonCommand = new DelegateCommand<object>(AddReason);
            _editReasonCommand = new DelegateCommand<object>(EditReason, CanEdit);
            _deleteReasonCommand = new DelegateCommand<object>(DeleteReason, CanDelete);
            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenReasonDetailsCommand { get; private set; }
        public ICommand AddReasonCommand
        {
            get { return this._addReasonCommand; }
        }
        public ICommand EditReasonCommand
        {
            get { return this._editReasonCommand; }
        }
        public ICommand DeleteReasonCommand
        {
            get { return this._deleteReasonCommand; }
        }
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        private void OnLoaded()
        {
            Reasons = new ListCollectionView(ReasonServices.GetAll());

            CollectionViewSource.GetDefaultView(Reasons).Filter = ReasonFilter;
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private bool ReasonFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var reason = (Reason)item;

            return (reason.ReasonDesc.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void AddReason(object ignored)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);

            RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(reasonOperationViewName + parameters, UriKind.Relative));
        }

        private void EditReason(object ignored)
        {
            if (SelectedReason == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedReason.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(reasonOperationViewName + parameters, UriKind.Relative));
        }

        private bool CanEdit(object ignored)
        {
            return SelectedReason != null;
        }

        private void DeleteReason(object ignored)
        {
            if (SelectedReason == null)
                return;

            this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to remove this?",
                        Title = "Confirm"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? InitDelete(SelectedReason.ID) : "NOT OK!"; });

            OnLoaded();
        }

        private bool CanDelete(object ignored)
        {
            if (SelectedReason == null)
                return false;

            return true;
        }

        private string InitDelete(long? ID)
        {
            try
            {
                if (ID.HasValue)
                {
                    ReasonServices.Delete(ID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return "OK!";
        }

        private void OpenReasonDetails(Reason reason)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedReason.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(reasonDetailsViewName + parameters, UriKind.Relative));
        }
    }
}
