using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ItAssetsDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using Excel = Microsoft.Office.Interop.Excel;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportITAssets.xaml
    /// </summary>
    public partial class ImportITAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up data
        FindITAssetBySerialNumberDataSet TheFindITAssetBySerialNumberDataSet = new FindITAssetBySerialNumberDataSet();
        AssetsForImportDataSet TheAssetsForImportDataSet = new AssetsForImportDataSet();

        public ImportITAssets()
        {
            InitializeComponent();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchEmail();
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.MyOriginatingTask();
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.MyOpenTasks();
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.AddTask();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
                TheAssetsForImportDataSet.assets.Rows.Clear();

                dgrResults.ItemsSource = TheAssetsForImportDataSet.assets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import IT Assets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void MitImportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strItem;
            string strManufacturer;
            string strModel;
            string strSerialNumber;
            int intQuantity;

            try
            {
                TheAssetsForImportDataSet.assets.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strItem = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);
                    strManufacturer = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);
                    strModel = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2);
                    strSerialNumber = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2);
                    intQuantity = Convert.ToInt32(Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2));

                    if(strSerialNumber == null)
                    {
                        strSerialNumber = "";
                    }
                    if(strModel == null)
                    {
                        strModel = "";
                    }
                    if(strManufacturer == null)
                    {
                        strManufacturer = "";
                    }

                    strItem = strItem.ToUpper();
                    strManufacturer = strManufacturer.ToUpper();
                    strModel = strModel.ToUpper();
                    strSerialNumber = strSerialNumber.ToUpper();

                    AssetsForImportDataSet.assetsRow NewAssetRow = TheAssetsForImportDataSet.assets.NewassetsRow();

                    NewAssetRow.Item = strItem;
                    NewAssetRow.ItemValue = 0;
                    NewAssetRow.Manufacturer = strManufacturer;
                    NewAssetRow.Model = strModel;
                    NewAssetRow.Quantity = intQuantity;
                    NewAssetRow.SerialNumber = strSerialNumber;
                    NewAssetRow.Updates = "";
                    NewAssetRow.WarehouseID = MainWindow.gintWarehouseID;

                    TheAssetsForImportDataSet.assets.Rows.Add(NewAssetRow);
                }

                PleaseWait.Close();
                dgrResults.ItemsSource = TheAssetsForImportDataSet.assets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import IT Assets // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            string strItem;
            string strManufacture;
            string strModel;
            string strSerialNumber;
            int intQuantity;
            decimal decValue = 0;
            string strUpgrades = "";

            try
            {
                intNumberOfRecords = TheAssetsForImportDataSet.assets.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strItem = TheAssetsForImportDataSet.assets[intCounter].Item;
                    strManufacture = TheAssetsForImportDataSet.assets[intCounter].Manufacturer;
                    strModel = TheAssetsForImportDataSet.assets[intCounter].Model;
                    strSerialNumber = TheAssetsForImportDataSet.assets[intCounter].SerialNumber;
                    intQuantity = TheAssetsForImportDataSet.assets[intCounter].Quantity;

                    blnFatalError = TheITAssetsClass.InsertITAsset(strItem, strManufacture, strModel, strSerialNumber, intQuantity, decValue, strUpgrades, MainWindow.gintWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Files Have Been Imported");
            }   
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import IT Assets // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
         }
    }
}
