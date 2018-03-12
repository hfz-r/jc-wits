using ESD.JC_Infrastructure.Events;
using ESD.JC_LocationMgmt.Services;
using ESD.JC_LocationMgmt.ModelsExt;
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

namespace ESD.JC_LocationMgmt.ViewModels
{
    public class LocationMainViewViewModel : BindableBase
    {
        private ObservableCollection<LocationExt> _Countries;
        public ObservableCollection<LocationExt> Countries
        {
            get { return _Countries; }
            set { SetProperty(ref _Countries, value); }
        }

        private LocationExt _SelectedLocation;
        public LocationExt SelectedLocation
        {
            get { return _SelectedLocation; }
            set { SetProperty(ref _SelectedLocation, value); }
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
                    _deleteLocationCommand.RaiseCanExecuteChanged();
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
        private ILocationServices LocationServices;
        private DelegateCommand<object> _saveLocationCommand;
        private DelegateCommand<object> _deleteLocationCommand;
        private InteractionRequest<Confirmation> interactionRequest;

        public LocationMainViewViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, ILocationServices _LocationServices)
        {
            RegionManager = _RegionManager;
            LocationServices = _LocationServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);

            _saveLocationCommand = new DelegateCommand<object>(SaveLocation);
            _deleteLocationCommand = new DelegateCommand<object>(DeleteLocation, CanDelete);
            interactionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand SaveLocationCommand
        {
            get { return this._saveLocationCommand; }
        }
        public ICommand DeleteLocationCommand
        {
            get { return this._deleteLocationCommand; }
        }
        public IInteractionRequest InteractionRequest
        {
            get { return this.interactionRequest; }
        }

        private void OnLoaded()
        {
            Countries = new ObservableCollection<LocationExt>();
            Countries.CollectionChanged += Countries_CollectionChanged;

            foreach (var r in LocationServices.GetAll())
            {
                Countries.Add(new LocationExt
                {
                    ID = r.ID,
                    LocationDesc = r.LocationDesc,
                    ModifiedOn = r.ModifiedOn,
                    ModifiedBy = r.ModifiedBy
                });
            }

            CollectionViewSource.GetDefaultView(Countries).Filter = LocationFilter;

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

        private bool LocationFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var country = (LocationExt)item;
            if (country.ID == 0)
                return true;

            return (country.LocationDesc.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void SaveLocation(object ignored)
        {
            this.interactionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to save this?",
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

        private void DeleteLocation(object ignored)
        {
            if (SelectedLocation == null)
                return;

            if (SelectedLocation.ID != 0)
            {
                this.interactionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to remove this?",
                        Title = "Confirm"
                    },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            if (InitDelete(SelectedLocation.ID))
                                OnLoaded();
                        }

                    });
            }
            else
            {
                Countries.Remove(SelectedLocation);
            }
        }

        private bool InitSave()
        {
            bool ok = false;

            List<LocationExt> toSaveList = new List<LocationExt>();
            List<LocationExt> toUpdateList = new List<LocationExt>();
            foreach (var rsn in Countries.ToList())
            {
                if (rsn.ID == 0 && !string.IsNullOrEmpty(rsn.LocationDesc))
                    toSaveList.Add(rsn);
                else if (rsn.ID != 0)
                    toUpdateList.Add(rsn);
            }

            try
            {
                List<Location> addObj = Add(toSaveList);
                if (addObj.Count() > 0)
                    ok = LocationServices.Save(addObj, "Save");

                List<Location> updateObj = Update(toUpdateList);
                if (updateObj.Count() > 0)
                    ok = LocationServices.Save(updateObj, "Update");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }

        private List<Location> Add(List<LocationExt> toSaveList)
        {
            List<Location> addObj = new List<Location>();

            foreach (var o in toSaveList)
            {
                var country = new Location
                {
                    LocationDesc = o.LocationDesc,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = o.ModifiedOn,
                    ModifiedBy = AuthenticatedUser
                };
                addObj.Add(country);
            }
            return addObj;
        }

        private List<Location> Update(List<LocationExt> toUpdateList)
        {
            List<Location> updateObj = new List<Location>();

            foreach (var u in toUpdateList)
            {
                var country = LocationServices.GetLocation(u.ID);
                if (country != null && country.LocationDesc != u.LocationDesc)
                {
                    country.LocationDesc = u.LocationDesc;
                    country.ModifiedOn = DateTime.Now;
                    country.ModifiedBy = AuthenticatedUser;
                }
                updateObj.Add(country);
            }
            return updateObj;
        }

        private bool CanDelete(object ignored)
        {
            if (SelectedLocation == null)
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
                    ok = LocationServices.Delete(ID);
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
