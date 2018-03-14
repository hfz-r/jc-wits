using DataLayer;
using ESD.JC_RoleMgmt.ModelsExt;
using ESD.JC_RoleMgmt.Services;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static ESD.JC_RoleMgmt.Services.RoleServices;

namespace ESD.JC_RoleMgmt.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class RoleOperationViewModel : BindableBase, IConfirmNavigationRequest
    {
        #region Properties

        private ObservableCollection<ModuleAccessCtrlExt> _ModuleList;
        public ObservableCollection<ModuleAccessCtrlExt> ModuleList
        {
            get { return _ModuleList; }
            set { SetProperty(ref _ModuleList, value); }
        }

        private List<ModuleAccessCtrlTransaction> _ModuleTransactionList;
        public List<ModuleAccessCtrlTransaction> ModuleTransactionList
        {
            get { return _ModuleTransactionList; }
            set { SetProperty(ref _ModuleTransactionList, value); }
        }

        private Role _RoleData;
        public Role RoleData
        {
            get { return _RoleData; }
            set { SetProperty(ref _RoleData, value); }
        }

        private string _RoleCodeAlias;
        public string RoleCodeAlias
        {
            get { return _RoleCodeAlias; }
            set
            {
                SetProperty(ref _RoleCodeAlias, value);
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
        private IRoleServices RoleServices;
        private IModuleAccessCtrlServices ModuleAccessCtrlServices;
        private IModuleAccessCtrlTransactionServices ModuleAccessCtrlTransactionServices;
        private InteractionRequest<Confirmation> confirmExitInteractionRequest;
        private DelegateCommand<object> _saveCommand;

        public RoleOperationViewModel(IRoleServices _RoleServices, IModuleAccessCtrlServices _ModuleAccessCtrlServices, IModuleAccessCtrlTransactionServices _ModuleAccessCtrlTransactionServices)
        {
            RoleServices = _RoleServices;
            ModuleAccessCtrlServices = _ModuleAccessCtrlServices;
            ModuleAccessCtrlTransactionServices = _ModuleAccessCtrlTransactionServices; 
             SendState = NormalStateKey;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            CancelCommand = new DelegateCommand(Cancel);
            _saveCommand = new DelegateCommand<object>(Save, CanSave);
            confirmExitInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand SaveCommand
        {
            get { return this._saveCommand; }
        }
        public IInteractionRequest ConfirmExitInteractionRequest
        {
            get { return this.confirmExitInteractionRequest; }
        }

        private void OnLoaded()
        {
            ModuleList = new ObservableCollection<ModuleAccessCtrlExt>();
            var module = ModuleAccessCtrlServices.GetAll();
            if (module.Count() > 0)
            {
                var tempObj = new List<ModuleAccessCtrl>();
                tempObj = ModuleAccessCtrlServices.GetAll().ToList();

                foreach (var item in tempObj)
                {
                    ModuleList.Add(new ModuleAccessCtrlExt
                    {
                        ID = item.ID,
                        Module = item.Module,
                        IsChecked = false
                    });
                }
            }

            var data = this.RoleData;
            if (data.ID != 0)
            {
                ModuleTransactionList = ModuleAccessCtrlTransactionServices.GetModuleAccessCtrlTransaction(data.ID);
                if (ModuleTransactionList != null && ModuleTransactionList.Count > 0)
                {
                    foreach (var item in ModuleTransactionList)
                    {
                        foreach (var tempModule in ModuleList)
                        {
                            if (tempModule.ID == item.ModuleID)
                            {
                                tempModule.IsChecked = item.IsAllow;
                            }
                        }
                    }
                }
            }
        }

        private void Save(object ignored)
        {
            SendState = SavingStateKey;

            var data = this.RoleData;

            if (string.IsNullOrEmpty(data.RoleName))
            {
                this.SendState = NormalStateKey;

                MessageBox.Show("Role Name is required.", "Notification", MessageBoxButton.OK);
                return;
            }

            try
            {
                Response ok = new Response();
                if (data.ID != 0)
                {
                    var updateObj = Update(data);
                    ok = RoleServices.Save(updateObj, "Update");

                    //Update module access control  
                    var updateModuleObj = UpdateModule();
                    foreach (var module in updateModuleObj)
                    {
                        ModuleAccessCtrlTransactionServices.Save(module, "Update");
                    }
                }
                else
                {
                    var newObj = Add(data);
                    ok = RoleServices.Save(newObj, "Save");

                    //Update module access control with newly added role(by ID)
                    var newModuleObj = AddModule(ok.id);
                    foreach (var module in newModuleObj)
                    {
                        ModuleAccessCtrlTransactionServices.Save(module, "Save");
                    }
                }
                       
                if (ok.state)
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

        private Role Add(Role data)
        {
            return new Role
            {
                RoleCode = RoleCodeAlias,
                RoleName = data.RoleName,
                Description = data.Description,
                CreatedOn = DateTime.Now,
                CreatedBy = AuthenticatedUser,
                ModifiedOn = DateTime.Now,
                ModifiedBy = AuthenticatedUser
            };
        }

        private List<ModuleAccessCtrlTransaction> AddModule(long id)
        {
            List<ModuleAccessCtrlTransaction> newObj = new List<ModuleAccessCtrlTransaction>();
            foreach (var item in ModuleList)
            {
                newObj.Add(new ModuleAccessCtrlTransaction
                {
                    ModuleID = item.ID,
                    RoleID = id,
                    IsAllow = item.IsChecked
                });
            }
            return newObj;
        }

        private Role Update(Role data)
        {
            var roleUpdate = RoleServices.GetRole(data.ID);
            if (roleUpdate != null)
            {
                roleUpdate.RoleCode = RoleCodeAlias;
                roleUpdate.RoleName = data.RoleName;
                roleUpdate.Description = !string.IsNullOrEmpty(data.Description) ? data.Description : string.Empty;
                roleUpdate.ModifiedOn = DateTime.Now;
                roleUpdate.ModifiedBy = AuthenticatedUser;
            }
            return roleUpdate;
        }

        private List<ModuleAccessCtrlTransaction> UpdateModule()
        {
            if (ModuleTransactionList != null && ModuleTransactionList.Count > 0)
            {
                foreach(var item in ModuleTransactionList)
                {
                    foreach (var tempModule in ModuleList)
                    {
                        if (tempModule.ID == item.ModuleID)
                        {
                            item.IsAllow = tempModule.IsChecked;
                        }
                    }
                }
            }
            return ModuleTransactionList;
        }        

        private bool CanSave(object ignored)
        {
            return !string.IsNullOrEmpty(RoleCodeAlias);
        }

        private void Cancel()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private long? GetRequestedRoleID(NavigationContext navigationContext)
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
            var roledata = new Role();

            AuthenticatedUser = (!string.IsNullOrEmpty((string)navigationContext.Parameters["AuthenticatedUser"]) ? 
                (string)navigationContext.Parameters["AuthenticatedUser"] : string.Empty);

            var id = GetRequestedRoleID(navigationContext);
            if (id.HasValue)
            {
                var role = RoleServices.GetRole(id.Value);
                if (role != null)
                {
                    if (!string.IsNullOrEmpty(RoleCodeAlias))
                    {
                        IsEnabled = false;
                    }

                    RoleCodeAlias = role.RoleCode;
                    roledata.ID = role.ID;
                    roledata.RoleCode = role.RoleCode;
                    roledata.RoleName = role.RoleName;
                    roledata.Description = role.Description;
                    roledata.ModifiedOn = role.ModifiedOn;
                    roledata.ModifiedBy = role.ModifiedBy;
                }
            }
            else
            {
                IsEnabled = true;
            }

            this.RoleData = roledata;

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }
    }
}
