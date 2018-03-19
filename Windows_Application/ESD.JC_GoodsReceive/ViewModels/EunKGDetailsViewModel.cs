using System;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using ESD.JC_GoodsReceive.Notifications;
using ESD.JC_GoodsReceive.ModelsExt;
using Prism.Commands;
using System.Windows.Input;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using ESD.JC_GoodsReceive.Services;
using DataLayer;
using System.ComponentModel;
using System.Windows;
using ESD.JC_Infrastructure.Controls;
using System.Text;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using TDSFramework;
using System.Threading;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class EunKGDetailsViewModel : BindableBase, IInteractionRequestAware
    {
        #region Properties

        private ObjectSelectionNotification notification;
        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                if (value is ObjectSelectionNotification)
                {
                    this.notification = value as ObjectSelectionNotification;
                    RaisePropertyChanged("Notification");
                }
            }
        }

        private ICollectionView _collectionView;
        public ICollectionView CollectionView
        {
            get { return _collectionView; }
            set { SetProperty(ref _collectionView, value); }
        }

        private ObservableCollection<EunKGExt> _EunKGs;
        public ObservableCollection<EunKGExt> EunKGs
        {
            get { return _EunKGs; }
            set
            {
                SetProperty(ref _EunKGs, value);
                RaisePropertyChanged("Notification");
            }
        }

        private EunKGExt _SelectedItem;
        public EunKGExt SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
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
                    _deleteCommand.RaiseCanExecuteChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                if (CollectionView != null)
                    CollectionViewSource.GetDefaultView(CollectionView).Refresh();
            }
        }

        private List<bool> _CountChecked = new List<bool>();
        public List<bool> CountChecked
        {
            get { return _CountChecked; }
            set { SetProperty(ref _CountChecked, value); }
        }

        private int _ItemCount;
        public int ItemCount
        {
            get { return _ItemCount; }
            set { SetProperty(ref _ItemCount, value); }
        }

        private decimal _maximumValue = 0;
        public decimal MaximumValue
        {
            get { return _maximumValue; }
            set { SetProperty(ref _maximumValue, value); }
        }

        public Action FinishInteraction { get; set; }

        public string State { get; private set; }

        #endregion

        #region Initializer & Constructor

        ProgressDialogHelper hlp;
        BackgroundWorker worker;

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IEunKGServices EunKGServices;

        private DelegateCommand _onloadedCommand;
        private DelegateCommand<object> _saveCommand;
        private DelegateCommand<object> _deleteCommand;
        private DelegateCommand<object> _IsSelected;
        private DelegateCommand<object> _PrintLblCommand;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;
        private InteractionRequest<Confirmation> interactionRequest;

        public EunKGDetailsViewModel(IEventAggregator EventAggregator, IRegionManager RegionManager, IEunKGServices EunKGServices)
        {
            this.RegionManager = RegionManager;
            this.EunKGServices = EunKGServices;
            this.EventAggregator = EventAggregator;

            State = string.Empty;

            hlp = new ProgressDialogHelper(EventAggregator);
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;

            _onloadedCommand = new DelegateCommand(OnLoaded);
            _saveCommand = new DelegateCommand<object>(SaveCommand, CanSave);
            _deleteCommand = new DelegateCommand<object>(DeleteCommand, CanDelete);
            _IsSelected = new DelegateCommand<object>(CheckBoxIsSelected);
            _PrintLblCommand = new DelegateCommand<object>(PrintLabel, CanPrint);
            _checkedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(true);
            });
            _unCheckedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(false);
            });
            _backCommand = new DelegateCommand(BackCommand);
            interactionRequest = new InteractionRequest<Confirmation>();
        }

        public ICommand OnLoadedCommand
        {
            get { return this._onloadedCommand; }
        }
        public ICommand SaveEunKGCommand
        {
            get { return this._saveCommand; }
        }
        public ICommand DeleteEunKGCommand
        {
            get { return this._deleteCommand; }
        }
        public ICommand IsSelected
        {
            get { return this._IsSelected; }
        }
        public ICommand PrintLblCommand
        {
            get { return this._PrintLblCommand; }
        }
        public ICommand CheckedAllCommand
        {
            get { return _checkedAllCommand; }
        }
        public ICommand UnCheckedAllCommand
        {
            get { return _unCheckedAllCommand; }
        }
        public ICommand _backCommand
        {
            get; private set;
        }
        public IInteractionRequest InteractionRequest
        {
            get { return this.interactionRequest; }
        }

        #endregion

        private void OnLoaded()
        {
            if (notification.ParentItem != null)
            {
                var obj = notification.ParentItem;
                MaximumValue = obj.Quantity - obj.QtyReceived.GetValueOrDefault();
            }

            EunKGs = new ObservableCollection<EunKGExt>();
            EunKGs.CollectionChanged += EunKGs_CollectionChanged;

            var tempObj = new List<EunKG>();
            if (!string.IsNullOrEmpty(State) && State == "RefreshGrid")
                tempObj = EunKGServices.GetAll(true).Where(y => y.GRID == notification.ParentItem.ID).ToList();
            else
                tempObj = notification.Items.ToList();

            foreach (var item in tempObj)
            {
                EunKGs.Add(new EunKGExt
                {
                    ID = item.ID,
                    GRID = item.GRID,
                    PO = item.GoodsReceive.PurchaseOrder,
                    SAPNO = item.GoodsReceive.Material,
                    EN = item.GoodsReceive.MaterialShortText,
                    EUN = item.GoodsReceive.Eun,
                    Qty = item.Quantity,
                    BIN = item.GoodsReceive.StorageBin,
                    ModifiedOn = item.ModifiedOn,
                    ModifiedBy = item.ModifiedBy,
                    IsChecked = false
                });
            }

            EunKGs = SequencingService.SetCollectionSequence(EunKGs);
            RaisePropertyChanged("EunKGs");

            CollectionView = new ListCollectionView(EunKGs);
            CollectionViewSource.GetDefaultView(CollectionView).Filter = EunKGFilter;
        }

        private void EunKGs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemCount = EunKGs.Count;
            SequencingService.SequenceParent = notification.ParentItem;
            SequencingService.SetCollectionSequence(EunKGs);
        }

        private bool EunKGFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterTextBox))
                return true;

            var kg = (EunKGExt)item;
            if (kg.ID == 0)
                return true;

            return (kg.ModifiedBy.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
        }

        private void CheckBoxIsSelected(object IsChecked)
        {
            if (SelectedItem == null)
                return;
            if (CellInfo.Item.ToString().Contains("NewItemPlaceholder"))
                return;

            bool chk = (bool)IsChecked;
            if (chk)
                CountChecked.Add(chk);
            else
            {
                if (CountChecked.Where(x => x == true).Count() > 0)
                    CountChecked.Remove(true);
            }

            _PrintLblCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged("CollectionView");
        }

        private bool CanSave(object ignored)
        {
            return EunKGs != null && (EunKGs.Sum(x => x.Qty) > MaximumValue) == false;
        }

        private void SaveCommand(object obj)
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
                            if (EunKGs.Sum(x => x.Qty) > MaximumValue)
                            {
                                MessageBox.Show("Quantity Should Not More Than " + MaximumValue + "!", "Failed To Save", MessageBoxButton.OK);
                                return;
                            }

                            if (InitSave())
                            {
                                State = "RefreshGrid";
                                OnLoaded();
                            }
                        }
                    });

        }

        private bool CanDelete(object ignored)
        {
            if (SelectedItem == null)
                return false;
            if (CellInfo.Item.ToString().Contains("NewItemPlaceholder"))
                return false;

            return true;
        }

        private void DeleteCommand(object obj)
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.ID != 0)
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
                            if (InitDelete(SelectedItem.ID))
                            {
                                State = "RefreshGrid";
                                OnLoaded();
                            }
                        }

                    });
            }
            else
            {
                EunKGs.Remove(SelectedItem);
            }
        }

        private void BackCommand()
        {
            if (this.notification != null)
            {
                if (!string.IsNullOrEmpty(State) && State == "RefreshGrid")
                    this.notification.Confirmed = true;
                else
                    this.notification.Confirmed = false;
            }

            this.FinishInteraction();
        }

        private bool InitSave()
        {
            bool ok = false;

            List<EunKGExt> toSaveList = new List<EunKGExt>();
            List<EunKGExt> toUpdateList = new List<EunKGExt>();
            foreach (var kg in EunKGs)
            {
                if (kg.ID == 0 && kg.Qty > 0)
                    toSaveList.Add(kg);
                else if (kg.ID != 0)
                    toUpdateList.Add(kg);
            }

            try
            {
                List<EunKG> addObj = Add(toSaveList);
                if (addObj.Count() > 0)
                    ok = EunKGServices.Save(addObj, "Save");

                List<EunKG> updateObj = Update(toUpdateList);
                if (updateObj.Count() > 0)
                    ok = EunKGServices.Save(updateObj, "Update");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }

        private List<EunKG> Add(List<EunKGExt> toSaveList)
        {
            List<EunKG> addObj = new List<EunKG>();

            foreach (var o in toSaveList)
            {
                var eunKG = new EunKG
                {
                    GRID = o.GRID,
                    Eun = o.EUN,
                    Quantity = o.Qty,
                    CreatedOn = DateTime.Now,
                    CreatedBy = notification.AuthenticatedUser,
                    ModifiedOn = o.ModifiedOn,
                    ModifiedBy = notification.AuthenticatedUser
                };
                addObj.Add(eunKG);
            }
            return addObj;
        }

        private List<EunKG> Update(List<EunKGExt> toUpdateList)
        {
            List<EunKG> updateObj = new List<EunKG>();

            foreach (var u in toUpdateList)
            {
                var eunKG = EunKGServices.GetEunKG(u.ID);
                if (eunKG != null && eunKG.Quantity != u.Qty)
                {
                    eunKG.Quantity = u.Qty;
                    eunKG.ModifiedOn = DateTime.Now;
                    eunKG.ModifiedBy = notification.AuthenticatedUser;
                }
                updateObj.Add(eunKG);
            }
            return updateObj;
        }

        private bool InitDelete(long ID)
        {
            bool ok = false;

            try
            {
                if (ID != 0)
                {
                    ok = EunKGServices.Delete(ID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            return ok;
        }

        private void SetIsSelectedProperty(bool isSelected)
        {
            foreach (var kg in EunKGs)
            {
                kg.IsChecked = isSelected;
            }

            switch (isSelected)
            {
                case true:
                    CountChecked.AddRange(Enumerable.Repeat(true, EunKGs.Count));
                    break;
                case false:
                    CountChecked = new List<bool>();
                    break;
            }

            _PrintLblCommand.RaiseCanExecuteChanged();

            CollectionView = new ListCollectionView(EunKGs);
            CollectionViewSource.GetDefaultView(CollectionView).Filter = EunKGFilter;
        }

        private bool CanPrint(object ignored)
        {
            if (EunKGs != null && EunKGs.Any(w => w.IsChecked == true && w.ID == 0))
                return false;

            return (CountChecked == null && CountChecked.Count.Equals(0)) ? false : (CountChecked.Where(x => x == true).Count() > 0);
        }

        private void PrintLabel(object ignored)
        {
            if (EunKGs.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    hlp.SetDialogDescription("Printing...");

                    worker.DoWork += delegate (object s, DoWorkEventArgs args)
                    {
                        StringBuilder strErrorPallet = new StringBuilder();
                        System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                        pd.PrinterSettings = new PrinterSettings();
                        pd.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterPort;

                        try
                        {
                            string fileName = string.Empty;
                            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            fileName = path + @"\JC-WITS_GR_Label.prn";

                            StreamReader txtReader = new StreamReader(fileName, false);
                            string xTemp = txtReader.ReadToEnd();
                            txtReader.Close();
                            var parts = new Dictionary<int, string>[2];

                            var listObj = EunKGs.Where(x => x.IsChecked == true).ToList();
                            foreach (var item in listObj)
                            {
                                StringBuilder strPallet = new StringBuilder();
                                StringBuilder strPalletTemplate = new StringBuilder();

                                strPallet.Append(string.Empty);
                                strPalletTemplate.Append(string.Empty);
                                strPalletTemplate.Append(xTemp);

                                strPallet = new StringBuilder();
                                strPallet.Append(strPalletTemplate.ToString());
                                strPallet.Replace("<SAPNO>", item.SAPNO);
                                strPallet.Replace("<LEGACYNO>", item.SAPNO);
                                strPallet.Replace("<BIN>", item.BIN);
                                strPallet.Replace("<QTY>", item.Qty.ToString());
                                strPallet.Replace("<BUN>", item.EUN);
                                strPallet.Replace("<QRCODE>", item.SAPNO + ";" + item.Qty.ToString());

                                for (int x = 0; x < 2; x++)
                                {
                                    parts[x] = new Dictionary<int, string>();

                                    string input = string.Empty;
                                    string lbl = string.Empty;
                                    switch (x)
                                    {
                                        case 0:
                                            input = item.EN;
                                            lbl = "<EN";
                                            break;
                                        case 1:
                                            input = item.EN;
                                            lbl = "<MS";
                                            break;
                                    }

                                    string[] words = input.Split(' ');

                                    string part = string.Empty;
                                    int partCounter = 0;
                                    foreach (var word in words)
                                    {
                                        if (part.Length + word.Length < 33)
                                        {
                                            part += string.IsNullOrEmpty(part) ? word : " " + word;
                                        }
                                        else
                                        {
                                            parts[x].Add(partCounter, part);
                                            part = word;
                                            partCounter++;
                                        }
                                    }
                                    parts[x].Add(partCounter, part);

                                    if (partCounter == 0)
                                        parts[x].Add((partCounter + 1), string.Empty);

                                    foreach (var i in parts[x])
                                    {
                                        string index = lbl + (i.Key + 1).ToString() + ">";
                                        strPallet.Replace(index, i.Value);
                                    }
                                }

                                if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                                {
                                    strErrorPallet.Append(item.GRID + ", ");
                                }

                                if (worker.CancellationPending)
                                {
                                    args.Cancel = true;
                                    return;
                                }

                                hlp.Initialize();
                            }

                            if (strErrorPallet.Length > 0)
                            {
                                strErrorPallet.Remove(strErrorPallet.Length - 2, 2);
                            }

                            State = "RefreshGrid";
                            OnLoaded();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };

                    worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
                    {
                        hlp.pgdialog.Close();
                    };

                    worker.RunWorkerAsync();
                    hlp.pgdialog.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
