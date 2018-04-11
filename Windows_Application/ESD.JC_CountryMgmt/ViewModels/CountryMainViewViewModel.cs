using ESD.JC_Infrastructure.Events;
using ESD.JC_CountryMgmt.Services;
using ESD.JC_CountryMgmt.ModelsExt;
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

namespace ESD.JC_CountryMgmt.ViewModels
{
    public class CountryMainViewViewModel : BindableBase
    {
        private ObservableCollection<CountryExt> _Countries;
        public ObservableCollection<CountryExt> Countries
        {
            get { return _Countries; }
            set { SetProperty(ref _Countries, value); }
        }

        private CountryExt _SelectedCountry;
        public CountryExt SelectedCountry
        {
            get { return _SelectedCountry; }
            set { SetProperty(ref _SelectedCountry, value); }
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
                    _deleteCountryCommand.RaiseCanExecuteChanged();
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
                if (Countries != null)
                    CollectionViewSource.GetDefaultView(Countries).Refresh();
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
        private ICountryServices CountryServices;
        private DelegateCommand<object> _saveCountryCommand;
        private DelegateCommand<object> _deleteCountryCommand;
        private InteractionRequest<Confirmation> interactionRequest;

        public CountryMainViewViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, ICountryServices _CountryServices)
        {
            RegionManager = _RegionManager;
            CountryServices = _CountryServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);

            _saveCountryCommand = new DelegateCommand<object>(SaveCountry);
            _deleteCountryCommand = new DelegateCommand<object>(DeleteCountry, CanDelete);
            interactionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand SaveCountryCommand
        {
            get { return this._saveCountryCommand; }
        }
        public ICommand DeleteCountryCommand
        {
            get { return this._deleteCountryCommand; }
        }
        public IInteractionRequest InteractionRequest
        {
            get { return this.interactionRequest; }
        }

        private void OnLoaded()
        {
            Countries = new ObservableCollection<CountryExt>();
            Countries.CollectionChanged += Countries_CollectionChanged;

            foreach (var r in CountryServices.GetAll())
            {
                Countries.Add(new CountryExt
                {
                    ID = r.ID,
                    CountryDesc = r.CountryDesc,
                    ModifiedOn = r.ModifiedOn,
                    ModifiedBy = r.ModifiedBy
                });
            }

            CollectionViewSource.GetDefaultView(Countries).Filter = CountryFilter;

            Countries = SequencingService.SetCollectionSequence(Countries);
            RaisePropertyChanged("Countries");
        }

        private void Countries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemCount = Countries.Count;
            SequencingService.SetCollectionSequence(Countries);
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private bool CountryFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var country = (CountryExt)item;
            if (country.ID == 0)
                return true;

            return (country.CountryDesc.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void SaveCountry(object ignored)
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

        private void DeleteCountry(object ignored)
        {
            if (SelectedCountry == null)
                return;

            if (SelectedCountry.ID != 0)
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
                            if (InitDelete(SelectedCountry.ID))
                                OnLoaded();
                        }

                    });
            }
            else
            {
                if (!string.IsNullOrEmpty(SelectedCountry.CountryDesc))
                {
                    Countries.Remove(SelectedCountry);
                }
            }
        }

        private bool InitSave()
        {
            bool ok = false;

            List<CountryExt> toSaveList = new List<CountryExt>();
            List<CountryExt> toUpdateList = new List<CountryExt>();
            foreach (var rsn in Countries.ToList())
            {
                if (rsn.ID == 0 && !string.IsNullOrEmpty(rsn.CountryDesc))
                    toSaveList.Add(rsn);
                else if (rsn.ID != 0)
                    toUpdateList.Add(rsn);
            }

            try
            {
                List<Country> addObj = Add(toSaveList);
                if (addObj.Count() > 0)
                    ok = CountryServices.Save(addObj, "Save");

                List<Country> updateObj = Update(toUpdateList);
                if (updateObj.Count() > 0)
                    ok = CountryServices.Save(updateObj, "Update");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }

        private List<Country> Add(List<CountryExt> toSaveList)
        {
            List<Country> addObj = new List<Country>();

            foreach (var o in toSaveList)
            {
                var country = new Country
                {
                    CountryDesc = o.CountryDesc,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = o.ModifiedOn,
                    ModifiedBy = AuthenticatedUser
                };
                addObj.Add(country);
            }
            return addObj;
        }

        private List<Country> Update(List<CountryExt> toUpdateList)
        {
            List<Country> updateObj = new List<Country>();

            foreach (var u in toUpdateList)
            {
                var country = CountryServices.GetCountry(u.ID);
                if (country != null && country.CountryDesc != u.CountryDesc)
                {
                    country.CountryDesc = u.CountryDesc;
                    country.ModifiedOn = DateTime.Now;
                    country.ModifiedBy = AuthenticatedUser;
                }
                updateObj.Add(country);
            }
            return updateObj;
        }

        private bool CanDelete(object ignored)
        {
            if (SelectedCountry == null)
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
                    ok = CountryServices.Delete(ID);
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
