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

        private bool _canPrint = true;
        public bool CanPrint
        {
            get { return _canPrint; }
            set { SetProperty(ref _canPrint, value); }
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

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        #endregion Properties

        private const string fcuDetailsViewName = "FGfcuDetailsView";

        XSSFWorkbook fcuWorkbook;

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IFCUServices fcuServices;

        public ObservableCollection<FCU> fcuCollection { get; private set; }
        public ObservableCollection<FCU> tempCollection { get; private set; }

        public DelegateCommand ImportFGCommand;
        public DelegateCommand ExportFGCommand;
        public DelegateCommand PrintLblCommand;
        public DelegateCommand OKCommand;
        public DelegateCommand XOKCommand;

        private DelegateCommand<object> _IsSelected;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;

        public FGfcuViewModel(ICompositeCommands applicationCommands, IRegionManager regionManager, IEventAggregator eventAggregator, IFCUServices fcuServices)
        {
            this.regionManager = regionManager;
            this.fcuServices = fcuServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);
            this.eventAggregator.GetEvent<FilterTextBoxEvent>().Subscribe(InitTextBoxSearch);

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

            ImportFGCommand = new DelegateCommand(Import);
            ExportFGCommand = new DelegateCommand(Export);
            PrintLblCommand = new DelegateCommand(PrintLabel).ObservesCanExecute(() => CanPrint);
            OKCommand = new DelegateCommand(OKImport);
            XOKCommand = new DelegateCommand(OnLoaded);
        }

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

        private void OnLoaded()
        {
            this.eventAggregator.GetEvent<ObjectEvent>().Publish(null);

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

                obj.IsChecked = false;
                fcuCollection.Add(obj);
            }

            FCU = new ListCollectionView(fcuCollection);
            FCU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(FCU).Filter = Filter;
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

            FCU = new ListCollectionView(tempObj);
            FCU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(FCU).Filter = Filter;
        }

        private void Import()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xls|.xlsx";
            dlg.Filter = "Excel documents (*.xls, *.xlsx, *.xlsm)|*.xls;*.xlsx;*.xlsm";

            tempCollection = new ObservableCollection<FCU>();

            if (dlg.ShowDialog() == true)
            {
                try
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Failed.", MessageBoxButton.OK);
                }
            }
        }

        private void ReadExcelFile()
        {
            try
            {
                ISheet sheet = fcuWorkbook.GetSheetAt(1);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

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

                            case 29:
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

                    if (!string.IsNullOrEmpty(obj.SerialNo) ||
                        !string.IsNullOrEmpty(obj.Project))
                        PopulateRecords(obj, tempCollection);
                }
                #endregion Cells Comparison

                if (tempCollection.Count() > 0)
                {
                    this.eventAggregator.GetEvent<ObjectEvent>().Publish(true);

                    FCU = new ListCollectionView(tempCollection);
                    FCU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

                    CollectionViewSource.GetDefaultView(FCU).Filter = Filter;
                }
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
                    Project = rec.Project,
                    UnitTag = rec.UnitTag,
                    PartNo = rec.PartNo,
                    Model = rec.Model,
                    PowerSupply = rec.PowerSupply,
                    FanMotor1 = rec.FanMotor1,
                    FanMotor2 = rec.FanMotor2,
                    Qty = rec.Qty,
                    CoolingCoil = rec.CoolingCoil1 + "R" + "/" + rec.CoolingCoil2 + "H",
                    SalesOrder = rec.SalesOrder,
                    Item = rec.Item,
                    SerialNo = rec.SerialNo,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = AuthenticatedUser,
                    IsChecked = true
                });
            }
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
                    QtyReceived = fcu.QtyReceived.GetValueOrDefault()
                });
            }

            if (importObj != null)
            {
                storage.InsertRecords(importObj.ToArray());
            }
            if (MessageBox.Show("File Export Success. Are you want to open the file?", "Notification", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                if (MessageBox.Show("Are you confirm you want to save this?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (Save())
                        OnLoaded();
                    else
                        throw new Exception("Save Failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import Failed.", MessageBoxButton.OK);
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

        private void PrintLabel()
        {
            if (fcuCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = fcuCollection.Where(x => x.IsChecked == true).ToList();

                    StringBuilder strErrorPallet = new StringBuilder();
                    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    pd.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterPort;

                    try
                    {
                        string fileName = string.Empty;
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        //fileName = path + @"\JC-FG_FCU.prn";
                        fileName = path + @"\JC_test2.prn";

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
                            strPallet.Replace("<Qty>", item.Qty.ToString());
                            strPallet.Replace("<Item>", item.Item.ToString());
                            strPallet.Replace("<SerialNo>", item.SerialNo);

                            if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                            {
                                strErrorPallet.Append(item.SerialNo + ", ");
                            }
                        }

                        if (strErrorPallet.Length > 0)
                        {
                            strErrorPallet.Remove(strErrorPallet.Length - 2, 2);
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

        private bool Filter(object item)
        {
            if (string.IsNullOrEmpty(FilterTextBox))
                return true;

            var fcu = (FCU)item;

            return (fcu.Project.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                    fcu.SerialNo.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));

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
            OKCommand.IsActive = IsActive;
            XOKCommand.IsActive = IsActive;

            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;

        #endregion Composite Buttons
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

        [FieldOrder(14)]
        public string PowerSupply { get; set; }

        [FieldOrder(9)]
        public string FanMotor1 { get; set; }

        [FieldOrder(10)]
        public string FanMotor2 { get; set; }

        [FieldOrder(5)]
        public decimal Qty { get; set; }

        [FieldOrder(11)]
        public string CoolingCoil1 { get; set; }

        [FieldOrder(12)]
        public string CoolingCoil2 { get; set; }

        [FieldOrder(13)]
        public string SalesOrder { get; set; }

        [FieldOrder(6)]
        public decimal Item { get; set; }

        [FieldOrder(7)]
        public string SerialNo { get; set; }

        [FieldOrder(8)]
        public decimal QtyReceived { get; set; }
    }
}
