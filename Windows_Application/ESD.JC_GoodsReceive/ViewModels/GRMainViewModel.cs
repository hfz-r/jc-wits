using DataLayer;
using ESD.JC_GoodsReceive.Services;
using ESD.JC_Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using TDSFramework;
using Microsoft.Win32;
using FileHelpers;
using FileHelpers.ExcelNPOIStorage;
using Microsoft.Office.Interop.Excel;
using ESD.JC_Infrastructure;
using FileHelpers.Events;
using System.Threading;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Configuration;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class GRMainViewModel : BindableBase
    {
        #region Properties

        private ICollectionView _GoodReceives;
        public ICollectionView GoodReceives
        {
            get { return _GoodReceives; }
            set { SetProperty(ref _GoodReceives, value); }
        }

        private ListCollectionView _listBoxOk;
        public ListCollectionView ListBoxOk
        {
            get { return _listBoxOk; }
            set
            {
                SetProperty(ref _listBoxOk, value);
                RaisePropertyChanged("ListBoxOk");
            }
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

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set
            {
                _cellInfo = value;
            }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                if (GoodReceives != null)
                    CollectionViewSource.GetDefaultView(GoodReceives).Refresh();
            }
        }

        private List<bool> _CountChecked = new List<bool>();
        public List<bool> CountChecked
        {
            get { return _CountChecked; }
            set { SetProperty(ref _CountChecked, value); }
        }

        private object _ImportBtn;
        public object ImportBtn
        {
            get { return _ImportBtn; }
            set
            {
                SetProperty(ref _ImportBtn, value);
                RaisePropertyChanged("ImportBtn");
            }
        }

        private bool _isEnableAutoRefresh;
        public bool IsEnableAutoRefresh
        {
            get { return _isEnableAutoRefresh; }
            set
            {
                SetProperty(ref _isEnableAutoRefresh, value);
                RaisePropertyChanged("IsEnableAutoRefresh");

                if (_isEnableAutoRefresh)
                    ExecuteTimer();
                else
                    StopTimer();
            }
        }

        private double _ProgressValue;
        public double ProgressValue
        {
            get
            {
                return _ProgressValue;
            }
            set
            {
                if (_ProgressValue == value)
                    return;
                _ProgressValue = value;

                RaisePropertyChanged("ProgressValue");
            }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return Thread.CurrentPrincipal.Identity.Name;

                return "Unauthorized User";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.IsAuthenticated;
            }
        }


        #endregion

        private const string grDetailsViewName = "GRDetailsView";

        XSSFWorkbook grWorkbook;

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IGRServices grServices;
        private IGRTimerSevices timerServices;

        public ObservableCollection<GoodsReceive> grCollection { get; private set; }
        public ObservableCollection<GoodsReceive> tempCollection { get; private set; }
        public ObservableCollection<CheckedListItem<OkCategory>> OkFilter { get; private set; }

        private DelegateCommand<object> _ImportGRCommand;
        private DelegateCommand<object> _ExportGRCommand;
        private DelegateCommand<object> _PrintLblCommand;
        private DelegateCommand<object> _DeleteCommand;
        private DelegateCommand<object> _IsSelected;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public GRMainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IGRServices grServices, IGRTimerSevices timerServices)
        {
            this.regionManager = regionManager;
            this.grServices = grServices;
            this.timerServices = timerServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<GR_ItemMessageEvent>().Subscribe(ConsumeItemMessage);
            
            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenGRDetailsCommand = new DelegateCommand<GoodsReceive>(OpenGRDetails);
            XOKCommand = new DelegateCommand(OnLoaded);
            OKCommand = new DelegateCommand(OKImport);

            _ImportGRCommand = new DelegateCommand<object>(ImportGR, CanImport);
            _ExportGRCommand = new DelegateCommand<object>(ExportGR, CanExport);
            _PrintLblCommand = new DelegateCommand<object>(PrintLabel, CanDeletePrint);
            _DeleteCommand = new DelegateCommand<object>(Delete, CanDeletePrint);
            _IsSelected = new DelegateCommand<object>(CheckBoxIsSelected);
            _checkedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(true);
            });
            _unCheckedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(false);
            });
            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        #region Commands

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenGRDetailsCommand { get; private set; }
        public ICommand OKCommand { get; private set; }
        public ICommand XOKCommand { get; private set; }
        public ICommand ImportGRCommand
        {
            get { return this._ImportGRCommand; }
        }
        public ICommand ExportGRCommand
        {
            get { return this._ExportGRCommand; }
        }
        public ICommand PrintLblCommand
        {
            get { return this._PrintLblCommand; }
        }
        public ICommand DeleteCommand
        {
            get { return this._DeleteCommand; }
        }
        public ICommand IsSelected
        {
            get { return this._IsSelected; }
        }
        public ICommand CheckedAllCommand
        {
            get { return _checkedAllCommand; }
        }
        public ICommand UnCheckedAllCommand
        {
            get { return _unCheckedAllCommand; }
        }
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        #endregion Commands

        private void OnLoaded()
        {
            ImportBtn = null;

            if (ProgressValue == 0)
                IsEnableAutoRefresh = true;

            tempCollection = new ObservableCollection<GoodsReceive>();
            grCollection = new ObservableCollection<GoodsReceive>();
            foreach (var obj in grServices.GetAll())
            {
                if (obj.QtyReceived == null)
                    obj.Ok = false;
                else
                {
                    if (obj.QtyReceived == obj.Quantity)
                        obj.Ok = true;
                    else if (obj.QtyReceived < obj.Quantity)
                        obj.Ok = null;
                }

                obj.Ok2 = obj.Ok;
                obj.Quantity2 = obj.Quantity;
                obj.QtyReceived2 = obj.QtyReceived;

                obj.IsChecked = false;
                grCollection.Add(obj);
            }
            
            _ImportGRCommand.RaiseCanExecuteChanged();
            _ExportGRCommand.RaiseCanExecuteChanged();

            BindListBox();

            GoodReceives = new ListCollectionView(grCollection);
            GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
        }

        private void BindListBox()
        {
            OkFilter = new ObservableCollection<CheckedListItem<OkCategory>>();
            foreach (var obj in grCollection.GroupBy(ok => ok.Ok).Select(x => new { Key = x.Key }))
            {
                OkFilter.Add(new CheckedListItem<OkCategory>
                {
                    IsChecked = true,
                    Item = new OkCategory
                    {
                        BoolOk = obj.Key,
                        TextOk = obj.Key == null ? " - Partial" : (obj.Key.ToString() == "False") ? " - NOT OK" : " - OK"
                    }
                });
            }

            ListBoxOk = new ListCollectionView(OkFilter);
        }

        private bool Filter(object obj)
        {
            var gr = (GoodsReceive)obj;

            if (gr.ID == 0)
            {
                if (string.IsNullOrEmpty(FilterTextBox))
                    return true;

                return (gr.PurchaseOrder.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                        gr.Material.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                        gr.MaterialShortText.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                int count = OkFilter.Where(chk => chk.IsChecked).Count(ok => ok.Item.BoolOk == gr.Ok);

                if (string.IsNullOrEmpty(FilterTextBox) && count > 0)
                {
                    return true;
                }
                else
                {
                    if (count == 0)
                    {
                        return false;
                    }

                    return (gr.PurchaseOrder.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                            gr.Material.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                            gr.MaterialShortText.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        private void CheckBoxIsSelected(object IsChecked)
        {
            bool chk = (bool)IsChecked;
            if (chk)
                CountChecked.Add(chk);
            else
            {
                if (CountChecked.Where(x => x == true).Count() > 0)
                    CountChecked.Remove(true);
            }

            RaisePropertyChanged("GoodReceives");

            _PrintLblCommand.RaiseCanExecuteChanged();
            _DeleteCommand.RaiseCanExecuteChanged();
        }

        private bool CanImport(object ignored)
        {
            if (ImportBtn != null)
            {
                return false;
            }

            return true;
        }

        private void ImportGR(object ignored)
        {
            try
            {
                var dlg = new OpenFileDialog();
                dlg.DefaultExt = ".xls|.xlsx|.csv";
                dlg.Filter = "Excel documents (*.xls, *.xlsx)|*.xls;*.xlsx|CSV files (*.csv)|*.csv";
                tempCollection = new ObservableCollection<GoodsReceive>();

                if (dlg.ShowDialog() == true)
                {
                    var file = new FileInfo(dlg.FileName);
                    if (file.Extension == ".csv")
                    {
                        ReadCSVFile(file.FullName);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(file.Extension))
                        {
                            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                            {
                                grWorkbook = new XSSFWorkbook(fs);
                            }

                            ReadExcelFile();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ReadExcelFile()
        {
            try
            {
                ISheet sheet = grWorkbook.GetSheet("GR");
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                
                #region Cells Comparison
                while (rows.MoveNext())
                {
                    IRow row = (XSSFRow)rows.Current;
                    if (row.RowNum == 0)
                        continue;
                    if (row.Cells.Any(d => d.CellType == CellType.Error))
                        continue;

                    ImportCLassModel obj = new ImportCLassModel();
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        ICell cell = row.GetCell(i);

                        switch (i)
                        {
                            case 0:
                                if (cell == null)
                                    obj.DocumentDate = null;
                                else
                                    obj.DocumentDate = cell.DateCellValue;
                                break;

                            case 1:
                                if (cell == null)
                                    obj.DocumentDate = null;
                                else
                                    obj.PostingDate = cell.DateCellValue;
                                break;

                            case 2:
                                if (cell == null)
                                    continue;
                                else
                                    obj.PurchaseOrder = SetCellValue(cell);
                                break;

                            case 3:
                                if (cell == null)
                                    obj.Vendor = null;
                                else
                                    obj.Vendor = SetCellValue(cell);
                                break;

                            case 4:
                                if (cell == null)
                                    obj.DeliveryNote = null;
                                else
                                    obj.DeliveryNote = SetCellValue(cell);
                                break;

                            case 5:
                                if (cell == null)
                                    obj.BillOfLading = null;
                                else
                                    obj.BillOfLading = SetCellValue(cell);
                                break;

                            case 6:
                                if (cell == null)
                                    obj.HeaderText = null;
                                else
                                    obj.HeaderText = SetCellValue(cell);
                                break;

                            case 7:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Material = SetCellValue(cell);
                                break;

                            case 8:
                                if (cell == null)
                                    continue;
                                else
                                    obj.MaterialShortText = SetCellValue(cell);
                                break;

                            case 9:
                                if (cell == null)
                                    obj.Ok = null;
                                else
                                    obj.Ok = cell.BooleanCellValue.ToString();
                                break;

                            case 10:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Quantity = Convert.ToDecimal(cell.NumericCellValue);
                                break;

                            case 11:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Eun = SetCellValue(cell);
                                break;

                            case 12:
                                if (cell == null)
                                    obj.MvmtType = null;
                                else
                                    obj.MvmtType = SetCellValue(cell);
                                break;

                            case 13:
                                if (cell == null)
                                    continue;
                                else
                                    obj.StorageLoc = SetCellValue(cell);
                                break;

                            case 14:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Plant = Convert.ToInt32(cell.NumericCellValue);
                                break;

                            case 15:
                                if (cell == null)
                                    continue;
                                else
                                    obj.StorageBin = SetCellValue(cell);
                                break;
                        }
                    }
                    PopulateRecords(obj, tempCollection);
                }
                #endregion Cells Comparison

                if (tempCollection.Count() > 0)
                {
                    ImportBtn = true;
                    IsEnableAutoRefresh = false;

                    _ImportGRCommand.RaiseCanExecuteChanged();
                    _ExportGRCommand.RaiseCanExecuteChanged();

                    GoodReceives = new ListCollectionView(tempCollection);
                    GoodReceives.SortDescriptions.Add(new SortDescription("CreatedOn", ListSortDirection.Descending));

                    CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
                }
                else
                    throw new Exception("There was no matched data detected.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read contents:\n\n" + ex.Message, "Error");
            }
        }

        public string SetCellValue(ICell cell)
        {
            return (cell.CellType.ToString() == "String" ? cell.StringCellValue : cell.NumericCellValue.ToString());
        }

        private void OKImport()
        {
            if (tempCollection.All(x => x.IsChecked == false))
            {
                MessageBox.Show("No changes have been made.");
                OnLoaded();
            }
            else
            {
                this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Confirm to save this?",
                        Title = "Confirm"
                    },
                     c =>
                     {
                         if (c.Confirmed)
                         {
                             if (Save())
                                 OnLoaded();
                         }
                     });
            }
        }

        private void ReadCSVFile(string fullName)
        {
            try
            {
                var engine = new DelimitedFileEngine<ImportCLassModel>(Encoding.UTF8);
                engine.AfterReadRecord += AfterReadCSVEvent;
                engine.Options.Delimiter = ";";

                var records = engine.ReadFile(fullName);
                foreach (ImportCLassModel rec in records)
                {
                    if (string.IsNullOrEmpty(rec.PurchaseOrder))
                        throw new Exception("Some of Purchase Order is null/empty.");
                    if (string.IsNullOrEmpty(rec.Material))
                        throw new Exception("Some of  Material is null/empty.");
                    if (string.IsNullOrEmpty(rec.MaterialShortText))
                        throw new Exception("Some of Material Short Text is null/empty.");
                    if (string.IsNullOrEmpty(rec.Eun))
                        throw new Exception("Some of Eun Order is null/empty.");
                    if (string.IsNullOrEmpty(rec.StorageLoc))
                        throw new Exception("Some of Storage Loc is null/empty.");
                    if (string.IsNullOrEmpty(rec.StorageBin))
                        throw new Exception("Some of Storage Bin is null/empty.");

                    PopulateRecords(rec, tempCollection);
                }

                if (tempCollection.Count() > 0)
                {
                    ImportBtn = true;
                    IsEnableAutoRefresh = false;

                    _ImportGRCommand.RaiseCanExecuteChanged();
                    _ExportGRCommand.RaiseCanExecuteChanged();

                    GoodReceives = new ListCollectionView(tempCollection);
                    GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

                    CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read contents:\n\n" + ex.Message, "Error");
            }
        }

        private void AfterReadCSVEvent(EngineBase engine, AfterReadEventArgs<ImportCLassModel> e)
        {
            if (e.Record.PurchaseOrder.Length > 11)
                throw new Exception("Line " + e.LineNumber + ": PurchaseOrder is too long");

            if (e.Record.Material.Length > 8)
                throw new Exception("Line " + e.LineNumber + ": Material is too long");

            if (e.Record.MaterialShortText.Length > 41)
                throw new Exception("Line " + e.LineNumber + ": MaterialShortText is too long");

            if (e.Record.StorageBin.Length > 6)
                throw new Exception("Line " + e.LineNumber + ": StorageBin is too long");
        }

        private bool Save()
        {
            bool ok = false;
            try
            {
                List<GoodsReceive> toAddList = new List<GoodsReceive>();
                List<GoodsReceive> toUpdateList = new List<GoodsReceive>();

                foreach (GoodsReceive gr in tempCollection)
                {
                    if (gr.IsChecked == true)
                    {
                        var obj = grServices.GetGRBySAPNo(gr.Material);
                        {
                            if (obj != null)
                            {
                                Update(ref obj, gr);
                                toUpdateList.Add(obj);
                            }
                            else
                            {
                                toAddList.Add(gr);
                            }
                        }
                    }
                }

                if (toAddList.Count() > 0)
                    ok = grServices.Save(toAddList, "Save");
                if (toUpdateList.Count() > 0)
                    ok = grServices.Save(toUpdateList, "Update");

                return ok;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Update(ref GoodsReceive obj, GoodsReceive gr)
        {
            obj.Vendor = gr.Vendor;
            obj.Material = gr.Material;
            obj.MaterialShortText = gr.MaterialShortText;
            obj.Ok = gr.Ok;
            obj.Quantity = gr.Quantity;
            obj.StorageLoc = gr.StorageLoc;
            obj.Plant = gr.Plant;
            obj.StorageBin = gr.StorageBin;
            obj.DocumentDate = gr.DocumentDate;
            obj.PostingDate = gr.PostingDate;
            obj.ModifiedOn = DateTime.Now;
            obj.ModifiedBy = AuthenticatedUser;
        }

        private void PopulateRecords(ImportCLassModel rec, ObservableCollection<GoodsReceive> temp)
        {
            if (grCollection.Any(sap => sap.Material == rec.Material) == false)
            {
                temp.Add(new GoodsReceive
                {
                    DocumentDate = rec.DocumentDate,
                    PostingDate = null,
                    PurchaseOrder = rec.PurchaseOrder,
                    Vendor = rec.Vendor ?? string.Empty,
                    DeliveryNote = rec.DeliveryNote ?? string.Empty,
                    BillOfLading = rec.BillOfLading ?? string.Empty,
                    HeaderText = rec.HeaderText ?? string.Empty,
                    Material = rec.Material,
                    MaterialShortText = rec.MaterialShortText,
                    Ok = Convert.ToBoolean(rec.Ok),
                    Ok2 = Convert.ToBoolean(rec.Ok),
                    Quantity = rec.Quantity,
                    Quantity2 = rec.Quantity,
                    Eun = rec.Eun,
                    MvmtType = rec.MvmtType ?? string.Empty,
                    StorageLoc = rec.StorageLoc,
                    Plant = rec.Plant,
                    StorageBin = rec.StorageBin,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = AuthenticatedUser,
                    IsChecked = true
                });
            }
        }

        private bool CanExport(object ignored)
        {
            if (ImportBtn != null)
            {
                return false;
            }

            return true;
        }

        private void ExportGR(object ignored)
        {
            ExcelNPOIStorage storage = new ExcelNPOIStorage(typeof(ImportCLassModel), 0, 0);

            string path = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Export GR");

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (IOException ie)
                {
                    Console.WriteLine("IO Error: " + ie.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("General Error: " + e.Message);
                }
            }

            storage.FileName = path + 
                string.Concat(@"\GoodsReceiveExport_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm", 
                System.Globalization.CultureInfo.InvariantCulture)) + 
                ".xlsx";

            storage.ColumnsHeaders.Add("Document Date");
            storage.ColumnsHeaders.Add("Posting Date");
            storage.ColumnsHeaders.Add("Purchase Order");
            storage.ColumnsHeaders.Add("Vendor");
            storage.ColumnsHeaders.Add("Delivery Note");
            storage.ColumnsHeaders.Add("Bill Of Lading");
            storage.ColumnsHeaders.Add("Header Text");
            storage.ColumnsHeaders.Add("Material");
            storage.ColumnsHeaders.Add("Material Short Text");
            storage.ColumnsHeaders.Add("OK");
            storage.ColumnsHeaders.Add("Quantity");
            storage.ColumnsHeaders.Add("Eun");
            storage.ColumnsHeaders.Add("Mvmt Type");
            storage.ColumnsHeaders.Add("Storage Loc");
            storage.ColumnsHeaders.Add("Plant");
            storage.ColumnsHeaders.Add("Storage Bin");
            storage.ColumnsHeaders.Add("Quantity Received");
            storage.ColumnsHeaders.Add("Transaction Reason");
            storage.ColumnsHeaders.Add("Received By");
            storage.ColumnsHeaders.Add("Received On");

            ObservableCollection<ImportCLassModel> importObj = new ObservableCollection<ImportCLassModel>();
            ImportCLassModel temp = new ImportCLassModel();

            using (var db = new InventoryContext())
            {
                foreach (var rec in grCollection)
                {
                    var grTxn = db.GRTransactions.Where(p => p.GRID == rec.ID);
                    if (grTxn != null && grTxn.Count() > 0)
                    {
                        foreach(var item in grTxn)
                        {
                            temp = new ImportCLassModel()
                            {
                                DocumentDate = rec.DocumentDate,
                                PostingDate = rec.PostingDate,
                                PurchaseOrder = rec.PurchaseOrder,
                                Vendor = rec.Vendor ?? string.Empty,
                                DeliveryNote = item.DeliveryNote ?? string.Empty,
                                BillOfLading = item.BillOfLading ?? string.Empty,
                                HeaderText = rec.HeaderText ?? string.Empty,
                                Material = rec.Material,
                                MaterialShortText = rec.MaterialShortText,
                                Ok = rec.Ok == null || rec.Ok == false ? "Not Complete" : "Completed",
                                Quantity = rec.Quantity,
                                Eun = rec.Eun,
                                MvmtType = rec.MvmtType ?? string.Empty,
                                StorageLoc = rec.StorageLoc,
                                Plant = rec.Plant,
                                StorageBin = rec.StorageBin,
                                QuantityReceived = item.Quantity,
                                ReasonDesc = item.Reason != null ? item.Reason.ReasonDesc : string.Empty,
                                ReceivedBy = item.CreatedBy,
                                ReceivedOn = Convert.ToString(item.CreatedOn),
                            };
                            importObj.Add(temp);
                        }
                    }
                    else
                    {
                        temp = new ImportCLassModel()
                        {
                            DocumentDate = rec.DocumentDate,
                            PostingDate = rec.PostingDate,
                            PurchaseOrder = rec.PurchaseOrder,
                            Vendor = rec.Vendor ?? string.Empty,
                            DeliveryNote = rec.DeliveryNote ?? string.Empty,
                            BillOfLading = rec.BillOfLading ?? string.Empty,
                            HeaderText = rec.HeaderText ?? string.Empty,
                            Material = rec.Material,
                            MaterialShortText = rec.MaterialShortText,
                            Ok = rec.Ok == null || rec.Ok == false ? "Not Complete" : "Completed",
                            Quantity = rec.Quantity,
                            Eun = rec.Eun,
                            MvmtType = rec.MvmtType ?? string.Empty,
                            StorageLoc = rec.StorageLoc,
                            Plant = rec.Plant,
                            StorageBin = rec.StorageBin,
                            QuantityReceived = 0,
                            ReasonDesc = string.Empty,
                            ReceivedBy = string.Empty,
                            ReceivedOn = string.Empty
                        };
                        importObj.Add(temp);
                    }
                }
            }

            if (importObj != null)
            {
                storage.InsertRecords(importObj.ToArray());

                this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Exported successfully. Open file?",
                        Title = "Notification"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? OpenExportedFile(storage.FileName) : "NOT OK!"; });
            }
        }

        private string OpenExportedFile(string fileName)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;

                Workbook wb = excel.Workbooks.Open(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return "OK";
        }

        private void Delete(object ignored)
        {
            if (grCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to delete?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = grCollection.Where(x => x.IsChecked == true).ToList();

                    try
                    {
                        foreach (var item in listObj)
                        {
                            grServices.Delete(item.ID);
                        }
                        OnLoaded();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void PrintLabel(object ignored)
        {
            if (grCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = grCollection.Where(x => x.IsChecked == true).ToList();
                    
                    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    pd.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["GRPrinterName"];

                    try
                    {
                        string fileName = string.Empty;
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        fileName = path + @"\JC-WITS_GR.prn";

                        StreamReader txtReader = new StreamReader(fileName, false);
                        string xTemp = txtReader.ReadToEnd();
                        txtReader.Close();
                        var parts = new Dictionary<int, string>[2];

                        foreach (var item in listObj)
                        {
                            StringBuilder strPallet = new StringBuilder();
                            StringBuilder strPalletTemplate = new StringBuilder();

                            strPallet.Append(string.Empty);
                            strPalletTemplate.Append(string.Empty);
                            strPalletTemplate.Append(xTemp);

                            strPallet = new StringBuilder();
                            strPallet.Append(strPalletTemplate.ToString());
                            strPallet.Replace("<PONO>", item.PurchaseOrder);
                            strPallet.Replace("<SAPNO>", item.Material);
                            strPallet.Replace("<LEGACYNO>", item.Material);
                            strPallet.Replace("<BIN>", item.StorageBin);
                            strPallet.Replace("<QUANTITY>", item.Quantity.ToString("G29") + " " + item.Eun);

                            if (item.Eun == "KG")
                                strPallet.Replace("<QRCODE>", item.Material + ";" + item.Quantity);
                            else
                                strPallet.Replace("<QRCODE>", item.Material);

                            //start EN & MS String Builder
                            for (int x = 0; x < 2; x++)
                            {
                                parts[x] = new Dictionary<int, string>();

                                string input = string.Empty;
                                string lbl = string.Empty;
                                switch (x)
                                {
                                    case 0:
                                        input = item.MaterialShortText;
                                        lbl = "<EN";
                                        break;
                                    case 1:
                                        input = item.MaterialShortText;
                                        lbl = "<MS";
                                        break;
                                }

                                string[] words = !string.IsNullOrEmpty(input) ? input.Split(' ') : new string[] { };

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
                            //end EN & MS String Builder

                            if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                            {
                            }
                        }

                        OnLoaded();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private bool CanDeletePrint(object ignored)
        {
            if (ImportBtn != null)
            {
                return false;
            }

            return (CountChecked == null && CountChecked.Count.Equals(0)) ? false : (CountChecked.Where(x => x == true).Count() > 0);
        }

        private void OpenGRDetails(GoodsReceive gr)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", gr.ID);

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(grDetailsViewName + parameters, UriKind.Relative));
        }

        private void SetIsSelectedProperty(bool isSelected)
        {
            ObservableCollection<GoodsReceive> tempObj = new ObservableCollection<GoodsReceive>();
            if (tempCollection != null && tempCollection.Count() > 0)
                tempObj = tempCollection;
            else
                tempObj = grCollection;

            foreach (var gr in tempObj)
            {
                gr.IsChecked = isSelected;
            }

            switch (isSelected)
            {
                case true:
                    CountChecked.AddRange(Enumerable.Repeat(true, tempObj.Count));
                    break;
                case false:
                    CountChecked = new List<bool>();
                    break;
            }

            _PrintLblCommand.RaiseCanExecuteChanged();
            _DeleteCommand.RaiseCanExecuteChanged();

            GoodReceives = new ListCollectionView(tempObj);
            GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
        }

        public void ExecuteTimer()
        {
            timerServices.StartTimerExecute();
        }

        public void StopTimer()
        {
            timerServices.StopTimerExecute();
        }

        private void ConsumeItemMessage(GRItemMessage msg)
        {
            if (msg == null)
                return;

            if (msg.HasValue)
            {
                ProgressValue = msg.PercentageValue;

                if (msg.State == "Completed")
                {
                    #region refresh grid
                    
                    foreach (var obj in grCollection)
                    {
                        var gr = grServices.GetGR(obj.ID);

                        if (gr != null)
                        {
                            obj.Ok = gr.Ok.GetValueOrDefault();
                            obj.Quantity = gr.Quantity;
                            obj.QtyReceived = gr.QtyReceived;

                            if (obj.QtyReceived == null)
                                obj.Ok = false;
                            else
                            {
                                if (obj.QtyReceived == obj.Quantity)
                                    obj.Ok = true;
                                else if (obj.QtyReceived < obj.Quantity)
                                    obj.Ok = null;
                            }

                            obj.Ok2 = obj.Ok;
                            obj.Quantity2 = obj.Quantity;
                            obj.QtyReceived2 = obj.QtyReceived;

                            if (OkFilter.All(x => x.Item.BoolOk != gr.Ok))
                            {
                                OkFilter.Add(new CheckedListItem<OkCategory>
                                {
                                    IsChecked = true,
                                    Item = new OkCategory
                                    {
                                        BoolOk = gr.Ok,
                                        TextOk = gr.Ok == null ? " - Partial" : (gr.Ok.ToString() == "False") ? " - NOT OK" : " - OK"
                                    }
                                });
                            }
                        }
                    }

                    CollectionViewSource.GetDefaultView(ListBoxOk).Refresh();

                    #endregion refresh grid

                    StopTimer();
                    ExecuteTimer();
                }
            }
        }
    }

    [DelimitedRecord("")]
    public class ImportCLassModel
    {
        [FieldOrder(1)]
        public DateTime? DocumentDate { get; set; }

        [FieldOrder(2)]
        public DateTime? PostingDate { get; set; }

        [FieldOrder(3)]
        public string PurchaseOrder { get; set; }

        [FieldOrder(4)]
        public string Vendor { get; set; }

        [FieldOrder(5)]
        public string DeliveryNote { get; set; }

        [FieldOrder(6)]
        public string BillOfLading { get; set; }

        [FieldOrder(7)]
        public string HeaderText { get; set; }

        [FieldOrder(8)]
        public string Material { get; set; }

        [FieldOrder(9)]
        public string MaterialShortText { get; set; }

        [FieldOrder(10)]
        public string Ok { get; set; }

        [FieldOrder(12)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal Quantity { get; set; }

        [FieldOrder(13)]
        public string Eun { get; set; }

        [FieldOrder(14)]
        public string MvmtType { get; set; }

        [FieldOrder(15)]
        public string StorageLoc { get; set; }

        [FieldOrder(16)]
        public int Plant { get; set; }

        [FieldOrder(17)]
        public string StorageBin { get; set; }

        [FieldOrder(18)]
        public decimal QuantityReceived { get; set; }

        [FieldOrder(19)]
        public string ReasonDesc { get; set; }

        [FieldOrder(20)]
        public string ReceivedBy { get; set; }

        [FieldOrder(21)]
        public string ReceivedOn { get; set; }
    }

    public class OkCategory
    {
        public bool? BoolOk { get; set; }
        public string TextOk { get; set; }
    }
}
