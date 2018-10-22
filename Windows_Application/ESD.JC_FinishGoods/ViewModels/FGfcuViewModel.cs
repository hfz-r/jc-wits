using DataLayer;
using ESD.JC_Infrastructure.Controls;
using ESD.JC_FinishGoods.Services;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ESD.JC_Infrastructure.Events;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Linq;
using FileHelpers.ExcelNPOIStorage;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using FileHelpers;
using Microsoft.Office.Interop.Excel;
using ESD.JC_Infrastructure;
using System.Text;
using System.Drawing.Printing;
using System.Reflection;
using TDSFramework;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FGfcuViewModel : BindableBase, IActiveAware
    {
        #region Properties

        public string ViewName
        {
            get { return "Fan Coil Unit (FCU)"; }
        }

        private ICollectionView _FCU;
        public ICollectionView FCU
        {
            get { return _FCU; }
            set { SetProperty(ref _FCU, value); }
        }

        private ListCollectionView _listBoxFcuStatus;
        public ListCollectionView ListBoxFcuStatus
        {
            get { return _listBoxFcuStatus; }
            set
            {
                SetProperty(ref _listBoxFcuStatus, value);
                RaisePropertyChanged("ListBoxFcuStatus");
            }
        }

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set { SetProperty(ref _cellInfo, value); }
        }

        private List<bool> _CountChecked = new List<bool>();
        public List<bool> CountChecked
        {
            get { return _CountChecked; }
            set { SetProperty(ref _CountChecked, value); }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                if (FCU != null)
                    CollectionViewSource.GetDefaultView(FCU).Refresh();
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

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        private DateTime _currDateTime;
        public DateTime currDateTime
        {
            get { return _currDateTime; }
            set { SetProperty(ref _currDateTime, value); }
        }

        public bool? IsImportBtnEnabled { get; private set; }

        #endregion Properties

        private const string fcuDetailsViewName = "FGfcuDetailsView";

        XSSFWorkbook fcuWorkbook;

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IFCUServices fcuServices;
        private IFCUTimerSevices timerServices;

        public ObservableCollection<FCU> fcuCollection { get; private set; }
        public ObservableCollection<FCU> tempCollection { get; private set; }
        public ObservableCollection<CheckedListItem<FCUStatusCategory>> FcuStatusFilter { get; private set; }

        public DelegateCommand ImportFGCommand;
        public DelegateCommand ExportFGCommand;
        public DelegateCommand PrintLblCommand;
        public DelegateCommand DeleteFGCommand;
        public DelegateCommand OKCommand;
        public DelegateCommand XOKCommand;

        private DelegateCommand<object> _IsSelected;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;

        public FGfcuViewModel(ICompositeCommands applicationCommands, IRegionManager regionManager, IEventAggregator eventAggregator, IFCUServices fcuServices, IFCUTimerSevices timerServices)
        {
            this.regionManager = regionManager;
            this.fcuServices = fcuServices;
            this.timerServices = timerServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);
            this.eventAggregator.GetEvent<FilterTextBoxEvent>().Subscribe(InitTextBoxSearch);
            this.eventAggregator.GetEvent<FCU_ItemMessageEvent>().Subscribe(ConsumeItemMessage);

            IsEnableAutoRefresh = true;
            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenFCUDetailsCommand = new DelegateCommand<FCU>(OpenFCUDetails);
            _IsSelected = new DelegateCommand<object>(CheckBoxIsSelected);
            _checkedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(true);
            });
            _unCheckedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(false);
            });

            ImportFGCommand = new DelegateCommand(Import, CanImport);
            ExportFGCommand = new DelegateCommand(Export, CanExport);
            PrintLblCommand = new DelegateCommand(PrintLabel, CanPrint);
            DeleteFGCommand = new DelegateCommand(Delete, CanDelete);
            OKCommand = new DelegateCommand(OKImport);
            XOKCommand = new DelegateCommand(OnLoaded);
        }

        #region Commands

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenFCUDetailsCommand { get; private set; }
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

        #endregion

        private void OnLoaded()
        {
            tempCollection = new ObservableCollection<FCU>();
            fcuCollection = new ObservableCollection<FCU>();
            foreach (var obj in fcuServices.GetAll(false))
            {
                if (obj.QtyReceived == null)
                    obj.ShipStatus = false;
                else
                {
                    if (obj.QtyReceived == obj.Qty)
                        obj.ShipStatus = true;
                    else if (obj.QtyReceived < obj.Qty)
                        obj.ShipStatus = null;
                }

                obj.ShipStatus2 = obj.ShipStatus;
                obj.Qty2 = obj.Qty;
                obj.QtyReceived2 = obj.QtyReceived;

                obj.IsChecked = false;
                fcuCollection.Add(obj);
            }

            IsImportBtnEnabled = null;
            ImportFGCommand.RaiseCanExecuteChanged();
            ExportFGCommand.RaiseCanExecuteChanged();
            this.eventAggregator.GetEvent<ObjectEvent>().Publish(IsImportBtnEnabled);

            BindListBox();

            FCU = new ListCollectionView(fcuCollection);
            FCU.SortDescriptions.Add(new SortDescription("CreatedOn", ListSortDirection.Descending));
            FCU.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(FCU).Filter = Filter;
        }

        private void BindListBox()
        {
            FcuStatusFilter = new ObservableCollection<CheckedListItem<FCUStatusCategory>>();
            foreach (var obj in fcuCollection.GroupBy(ok => ok.ShipStatus).Select(x => new { Key = x.Key }))
            {
                FcuStatusFilter.Add(new CheckedListItem<FCUStatusCategory>
                {
                    IsChecked = true,
                    Item = new FCUStatusCategory
                    {
                        BoolFcuStatus = obj.Key,
                        TextFcuStatus = obj.Key == null ? " - Partial" : (obj.Key.ToString() == "False") ? " - NOT OK" : " - OK"
                    }
                });
            }

            ListBoxFcuStatus = new ListCollectionView(FcuStatusFilter);
        }

        private bool Filter(object item)
        {
            var fcu = (FCU)item;
            if (fcu.ID == 1)
            {
                if (string.IsNullOrEmpty(FilterTextBox))
                    return true;

                return (fcu.Project.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                        fcu.SerialNo.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                int count = FcuStatusFilter.Where(chk => chk.IsChecked).Count(ok => ok.Item.BoolFcuStatus == fcu.ShipStatus);

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

                    return (fcu.Project.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                            fcu.SerialNo.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private void InitTextBoxSearch(string searchString)
        {
            FilterTextBox = searchString;
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

            RaisePropertyChanged("FCU");

            PrintLblCommand.RaiseCanExecuteChanged();
            DeleteFGCommand.RaiseCanExecuteChanged();
        }

        private void SetIsSelectedProperty(bool isSelected)
        {
            ObservableCollection<FCU> tempObj = new ObservableCollection<FCU>();
            if (tempCollection != null && tempCollection.Count() > 0)
                tempObj = tempCollection;
            else
                tempObj = fcuCollection;

            foreach (var fcu in tempObj)
            {
                fcu.IsChecked = isSelected;
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

            PrintLblCommand.RaiseCanExecuteChanged();
            DeleteFGCommand.RaiseCanExecuteChanged();

            FCU = new ListCollectionView(tempObj);
            FCU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(FCU).Filter = Filter;
        }

        private bool CanImport()
        {
            if (IsImportBtnEnabled != null)
            {
                return false;
            }

            return true;
        }

        private void Import()
        {
            try
            {
                var dlg = new OpenFileDialog();
                dlg.DefaultExt = ".xls|.xlsx";
                dlg.Filter = "Excel documents (*.xls, *.xlsx, *.xlsm)|*.xls;*.xlsx;*.xlsm";

                tempCollection = new ObservableCollection<FCU>();

                if (dlg.ShowDialog() == true)
                {
                    var file = new FileInfo(dlg.FileName);
                    if (!string.IsNullOrEmpty(file.Extension))
                    {
                        using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                        {
                            fcuWorkbook = new XSSFWorkbook(fs);
                        }

                        ReadExcelFile();
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
                ISheet sheet = fcuWorkbook.GetSheetAt(1);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                currDateTime = DateTime.Now;

                #region Cells Comparison
                while (rows.MoveNext())
                {
                    IRow row = (XSSFRow)rows.Current;
                    if (row.RowNum == 0)
                        continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                        continue;
                    if (row.Cells.Any(d => d.CellType == CellType.Error))
                        continue;

                    FCUImportCLassModel obj = new FCUImportCLassModel();
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        ICell cell = row.GetCell(i);

                        switch (i)
                        {
                            case 0:
                                if (cell == null)
                                    continue;
                                else
                                    obj.IDSeq = SetCellValue(cell);
                                break;

                            case 1:
                                if (cell == null)
                                    continue;
                                else
                                    obj.SerialNo = SetCellValue(cell);
                                break;

                            case 2:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Project = SetCellValue(cell);
                                break;

                            case 3:
                                if (cell == null)
                                    continue;
                                else
                                    obj.UnitTag = SetCellValue(cell);
                                break;

                            case 6:
                                if (cell == null)
                                    continue;
                                else
                                    obj.SalesOrder = SetCellValue(cell);
                                break;

                            case 7:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Item = (cell.CellType.ToString() == "String" ? Convert.ToDecimal(cell.StringCellValue) : Convert.ToDecimal(cell.NumericCellValue)); ;
                                break;

                            case 8:
                                if (cell == null)
                                    continue;
                                else
                                    obj.PartNo = SetCellValue(cell);
                                break;

                            case 9:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Model = SetCellValue(cell);
                                break;

                            case 17:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.FanMotor1 = SetFormulaCellValue(cell);
                                else
                                    obj.FanMotor1 = SetCellValue(cell);
                                break;

                            case 18:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.FanMotor2 = SetFormulaCellValue(cell);
                                else
                                    obj.FanMotor2 = SetCellValue(cell);
                                break;

                            case 19:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.Qty = cell.CachedFormulaResultType.ToString() == "String" ? Convert.ToDecimal(cell.StringCellValue) : Convert.ToDecimal(cell.NumericCellValue);
                                else
                                    obj.Qty = (cell.CellType.ToString() == "String" ? Convert.ToDecimal(cell.StringCellValue) : Convert.ToDecimal(cell.NumericCellValue));
                                break;

                            case 23:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.CoolingCoil1 = SetFormulaCellValue(cell);
                                else
                                    obj.CoolingCoil1 = SetCellValue(cell);
                                break;

                            case 24:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.CoolingCoil2 = SetFormulaCellValue(cell);
                                else
                                    obj.CoolingCoil2 = SetCellValue(cell);
                                break;

                            case 28:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.PowerSupply = SetFormulaCellValue(cell);
                                else
                                    obj.PowerSupply = SetCellValue(cell);
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(obj.SerialNo) && obj.SerialNo != "0")
                        PopulateRecords(obj, tempCollection);
                }
                #endregion Cells Comparison

                if (tempCollection.Count() > 0)
                {
                    IsImportBtnEnabled = true;
                    ImportFGCommand.RaiseCanExecuteChanged();
                    ExportFGCommand.RaiseCanExecuteChanged();
                    this.eventAggregator.GetEvent<ObjectEvent>().Publish(IsImportBtnEnabled);

                    FCU = new ListCollectionView(tempCollection);
                    FCU.SortDescriptions.Clear();
                    FCU.SortDescriptions.Add(new SortDescription("IDSeq", ListSortDirection.Ascending));

                    CollectionViewSource.GetDefaultView(FCU).Filter = Filter;
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

        public string SetFormulaCellValue(ICell cell)
        {
            return cell.CachedFormulaResultType.ToString() == "String" ? cell.StringCellValue : cell.NumericCellValue.ToString();
        }

        private void PopulateRecords(FCUImportCLassModel rec, ObservableCollection<FCU> temp)
        {
            if (fcuCollection.Any(x => x.SerialNo == rec.SerialNo) == false)
            {
                temp.Add(new FCU
                {
                    ID = Convert.ToInt64(rec.IDSeq),
                    Project = rec.Project,
                    UnitTag = rec.UnitTag,
                    PartNo = rec.PartNo,
                    Model = rec.Model,
                    PowerSupply = rec.PowerSupply,
                    FanMotor1 = rec.FanMotor1,
                    FanMotor2 = rec.FanMotor2,
                    Qty = rec.Qty,
                    Qty2 = rec.Qty,
                    CoolingCoil = rec.CoolingCoil1 + "R" + "/" + rec.CoolingCoil2 + "H",
                    SalesOrder = rec.SalesOrder,
                    Item = rec.Item,
                    SerialNo = rec.SerialNo,
                    CreatedOn = currDateTime,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = currDateTime,
                    ModifiedBy = AuthenticatedUser,
                    IsChecked = true,
                    ShipStatus = false,
                    ShipStatus2 = false
                });
            }
        }

        private bool CanExport()
        {
            if (IsImportBtnEnabled != null)
            {
                return false;
            }

            return true;
        }

        private void Export()
        {
            ExcelNPOIStorage storage = new ExcelNPOIStorage(typeof(FCUImportCLassModel), 0, 0);

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Export FG");

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
                string.Concat(@"\FCUExport_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm",
                System.Globalization.CultureInfo.InvariantCulture)) +
                ".xlsx";

            storage.ColumnsHeaders.Add("Project");
            storage.ColumnsHeaders.Add("Unit Tag");
            storage.ColumnsHeaders.Add("Part No.");
            storage.ColumnsHeaders.Add("Model");
            storage.ColumnsHeaders.Add("Quantity");
            storage.ColumnsHeaders.Add("Item");
            storage.ColumnsHeaders.Add("Serial No.");
            storage.ColumnsHeaders.Add("Quantity Received");
            storage.ColumnsHeaders.Add("Shipped By");
            storage.ColumnsHeaders.Add("Shipped On");

            ObservableCollection<FCUImportCLassModel> importObj = new ObservableCollection<FCUImportCLassModel>();
            foreach (var fcu in fcuCollection)
            {
                importObj.Add(new FCUImportCLassModel
                {
                    Project = fcu.Project,
                    UnitTag = fcu.UnitTag,
                    PartNo = fcu.PartNo,
                    Model = fcu.Model,
                    Qty = fcu.Qty,
                    Item = fcu.Item,
                    SerialNo = fcu.SerialNo,
                    QtyReceived = fcu.QtyReceived.GetValueOrDefault(),
                    ShippedBy = fcu.CreatedBy,
                    ShippedOn = Convert.ToString(fcu.CreatedOn)
                });
            }

            if (importObj != null)
            {
                storage.InsertRecords(importObj.ToArray());
            }
            if (MessageBox.Show("Exported successfully. Open file?", "Notification", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenExportedFile(storage.FileName);
            }
        }

        private void OpenExportedFile(string fileName)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;

                Workbook wb = excel.Workbooks.Open(fileName);
            }
            catch
            {
                throw new Exception();
            }
        }

        private void OKImport()
        {
            try
            {
                if (tempCollection.All(x => x.IsChecked == false))
                {
                    MessageBox.Show("No changes have been made.");
                    OnLoaded();
                }
                else
                {
                    if (MessageBox.Show("Confirm to save this?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (Save())
                            OnLoaded();
                        else
                            throw new Exception("Save failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import failed.", MessageBoxButton.OK);
            }
        }

        private bool Save()
        {
            bool ok = false;
            try
            {
                List<FCU> toAddList = new List<FCU>();
                List<FCU> toUpdateList = new List<FCU>();

                foreach (FCU fcu in tempCollection)
                {
                    if (fcu.IsChecked == true)
                    {
                        var obj = fcuServices.GetFCUBySerialNo(fcu.SerialNo);
                        {
                            if (obj != null)
                            {
                                Update(ref obj, fcu);
                                toUpdateList.Add(obj);
                            }
                            else
                            {
                                toAddList.Add(fcu);
                            }
                        }
                    }
                }

                if (toAddList.Count() > 0)
                    ok = fcuServices.Save(toAddList, "Save");
                if (toUpdateList.Count() > 0)
                    ok = fcuServices.Save(toUpdateList, "Update");

                return ok;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Update(ref FCU obj, FCU fcu)
        {
            obj.Project = fcu.Project;
            obj.UnitTag = fcu.UnitTag;
            obj.PartNo = fcu.PartNo;
            obj.Model = fcu.Model;
            obj.PowerSupply = fcu.PowerSupply;
            obj.FanMotor1 = fcu.FanMotor1;
            obj.FanMotor2 = fcu.FanMotor2;
            obj.Qty = fcu.Qty;
            obj.CoolingCoil = fcu.CoolingCoil;
            obj.SalesOrder = fcu.SalesOrder;
            obj.Item = fcu.Item;
            obj.ModifiedOn = DateTime.Now;
            obj.ModifiedBy = AuthenticatedUser;
        }

        private bool CanDelete()
        {
            if (IsImportBtnEnabled != null)
            {
                return false;
            }

            return (CountChecked == null && CountChecked.Count.Equals(0)) ? false : (CountChecked.Where(x => x == true).Count() > 0);
        }

        private void Delete()
        {
            if (fcuCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to delete?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = fcuCollection.Where(x => x.IsChecked == true).ToList();

                    try
                    {
                        foreach (var item in listObj)
                        {
                            fcuServices.Delete(item.ID);
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

        private bool CanPrint()
        {
            if (IsImportBtnEnabled != null)
            {
                return false;
            }

            return (CountChecked == null && CountChecked.Count.Equals(0)) ? false : (CountChecked.Where(x => x == true).Count() > 0);
        }

        private void PrintLabel()
        {
            if (fcuCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = fcuCollection.Where(x => x.IsChecked == true).ToList();
                    
                    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    pd.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterPort;

                    try
                    {
                        string fileName = string.Empty;
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        fileName = path + @"\JC-FG_FCU.prn";

                        StreamReader txtReader = new StreamReader(fileName, Encoding.Default, false);
                        string xTemp = txtReader.ReadToEnd();
                        txtReader.Close();

                        foreach (var item in listObj)
                        {
                            StringBuilder strPallet = new StringBuilder();
                            StringBuilder strPalletTemplate = new StringBuilder();

                            strPallet.Append(string.Empty);
                            strPalletTemplate.Append(string.Empty);
                            strPalletTemplate.Append(xTemp);

                            strPallet = new StringBuilder();
                            strPallet.Append(strPalletTemplate.ToString());
                            strPallet.Replace("<Project>", item.Project);
                            strPallet.Replace("<UnitTag>", item.UnitTag);
                            strPallet.Replace("<PartNo>", item.PartNo);
                            strPallet.Replace("<Model>", item.Model);
                            strPallet.Replace("<PowerSupply>", item.PowerSupply);
                            strPallet.Replace("<FanMotor1>", item.FanMotor1);
                            strPallet.Replace("<FanMotor2>", item.FanMotor2);
                            strPallet.Replace("<CoolingCoil>", item.CoolingCoil);
                            strPallet.Replace("<HeatingCoil>", item.HeatingCoil);
                            strPallet.Replace("<Heater>", item.Heater);
                            strPallet.Replace("<SalesOrder>", item.SalesOrder);
                            strPallet.Replace("<Qty>", item.Qty.ToString("G29"));
                            strPallet.Replace("<Item>", item.Item.ToString());
                            strPallet.Replace("<SerialNo>", item.SerialNo);

                            for (int i = 0; i < Properties.Settings.Default.PrintCount; i++)
                            {
                                if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                                {
                                }
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

        private void OpenFCUDetails(FCU fcu)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", fcu.ID);

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(fcuDetailsViewName + parameters, UriKind.Relative));
        }

        #region Composite Buttons

        bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        private void OnIsActiveChanged()
        {
            ImportFGCommand.IsActive = IsActive;
            ExportFGCommand.IsActive = IsActive;
            PrintLblCommand.IsActive = IsActive;
            DeleteFGCommand.IsActive = IsActive;
            OKCommand.IsActive = IsActive;
            XOKCommand.IsActive = IsActive;

            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;

        #endregion Composite Buttons

        #region Timer

        private void ExecuteTimer()
        {
            timerServices.StartTimerExecute();
        }

        private void StopTimer()
        {
            timerServices.StopTimerExecute();
        }

        private void ConsumeItemMessage(FCUItemMessage msg)
        {
            if (msg == null)
                return;

            if (msg.HasValue)
            {
                ProgressValue = msg.PercentageValue;

                if (msg.State == "Completed")
                {
                    #region refresh grid

                    if (fcuCollection != null && fcuCollection.Count > 0)
                    {
                        foreach (var obj in fcuCollection)
                        {
                            var fcu = fcuServices.GetFCU(obj.ID);
                            obj.ShipStatus = fcu.ShipStatus;
                            obj.Qty = fcu.Qty;
                            obj.QtyReceived = fcu.QtyReceived;

                            if (obj.QtyReceived == null)
                                obj.ShipStatus = false;
                            else
                            {
                                if (obj.QtyReceived == obj.Qty)
                                    obj.ShipStatus = true;
                                else if (obj.QtyReceived < obj.Qty)
                                    obj.ShipStatus = null;
                            }

                            obj.ShipStatus2 = obj.ShipStatus;
                            obj.Qty2 = obj.Qty;
                            obj.QtyReceived2 = obj.QtyReceived;

                            if (FcuStatusFilter.All(x => x.Item.BoolFcuStatus != fcu.ShipStatus))
                            {
                                FcuStatusFilter.Add(new CheckedListItem<FCUStatusCategory>
                                {
                                    IsChecked = true,
                                    Item = new FCUStatusCategory
                                    {
                                        BoolFcuStatus = fcu.ShipStatus,
                                        TextFcuStatus = fcu.ShipStatus == null ? " - Partial" : (fcu.ShipStatus.ToString() == "False") ? " - NOT OK" : " - OK"
                                    }
                                });
                            }
                        }
                    }

                    if(ListBoxFcuStatus != null)
                        CollectionViewSource.GetDefaultView(ListBoxFcuStatus).Refresh();

                    #endregion refresh grid

                    StopTimer();
                    ExecuteTimer();
                }
            }
        }

        #endregion Timer
    }

    [DelimitedRecord("")]
    public class FCUImportCLassModel
    {
        [FieldOrder(1)]
        public string Project { get; set; }

        [FieldOrder(2)]
        public string UnitTag { get; set; }

        [FieldOrder(3)]
        public string PartNo { get; set; }

        [FieldOrder(4)]
        public string Model { get; set; }

        [FieldOrder(16)]
        public string PowerSupply { get; set; }

        [FieldOrder(11)]
        public string FanMotor1 { get; set; }

        [FieldOrder(12)]
        public string FanMotor2 { get; set; }

        [FieldOrder(5)]
        public decimal Qty { get; set; }

        [FieldOrder(13)]
        public string CoolingCoil1 { get; set; }

        [FieldOrder(14)]
        public string CoolingCoil2 { get; set; }

        [FieldOrder(15)]
        public string SalesOrder { get; set; }

        [FieldOrder(6)]
        public decimal Item { get; set; }

        [FieldOrder(7)]
        public string SerialNo { get; set; }
    
        [FieldOrder(8)]
        public decimal QtyReceived { get; set; }

        [FieldOrder(17)]
        public string IDSeq { get; set; }

        [FieldOrder(9)]
        public string ShippedBy { get; set; }

        [FieldOrder(10)]
        public string ShippedOn { get; set; }
    }

    public class FCUStatusCategory
    {
        public bool? BoolFcuStatus { get; set; }
        public string TextFcuStatus { get; set; }
    }
}
