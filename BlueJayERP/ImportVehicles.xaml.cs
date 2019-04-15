/* Title:           Import Vehicles
 * Date:            4-17-18
 * Author:          Terrance Holmes
 * 
 * Description:     This is used to import the vehicles for a specific spreadsheet */

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
using Excel = Microsoft.Office.Interop.Excel;
using VehicleMainDLL;
using VehicleInfoDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using KeyWordDLL;
using VehicleAssignmentDLL;
using VehicleBulkToolsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportVehicles.xaml
    /// </summary>
    public partial class ImportVehicles : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();

        FindActiveVehicleMainShortVehicleNumberDataSet TheFindActiveVehicleMainShortVehicleNumberDataSet = new FindActiveVehicleMainShortVehicleNumberDataSet();
        FindDOTStatusByStatusDataSet TheFindDOTStatusByStatusDataSet = new FindDOTStatusByStatusDataSet();
        FindGPSStatusByStatusDataSet TheFindGPSStatusByStatusDataSet = new FindGPSStatusByStatusDataSet();
        ImportVehiclesDataSet TheImportVehiclesDataSet = new ImportVehiclesDataSet();

        public ImportVehicles()
        {
            InitializeComponent();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitImportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strVehicleNumber;
            int intRecordsReturned;
            string strAssignedOffice;
            string strIMEI;
            int intGPSStatusID;
            int intDOTStatusID;
            string strDOTStatus;
            string strGPSStatus;

            try
            {
                TheImportVehiclesDataSet.importvehicles.Rows.Clear();

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

                    strVehicleNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);

                    TheFindActiveVehicleMainShortVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainShortVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainShortVehicleNumberDataSet.FindActiveVehicleMainShortVehicleNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        strAssignedOffice = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();
                        strDOTStatus = Convert.ToString((range.Cells[intCounter, 13] as Excel.Range).Value2).ToUpper();
                        strIMEI = Convert.ToString((range.Cells[intCounter, 11] as Excel.Range).Value2).ToUpper();

                        MainWindow.gintEmployeeID = FindEmployeeID(strAssignedOffice);

                        if(MainWindow.gintEmployeeID < 1)
                        {
                            TheMessagesClass.ErrorMessage("Problems Find Assigned Office, Check the Spelling");

                            return;
                        }

                        TheFindDOTStatusByStatusDataSet = TheVehicleInfoClass.FindDOTStatusByStatus(strDOTStatus);

                        intRecordsReturned = TheFindDOTStatusByStatusDataSet.FindDOTStatusByStatus.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            TheMessagesClass.ErrorMessage("DOT Status Was Not Found, Check Spelling");

                            return;
                        }

                        intDOTStatusID = TheFindDOTStatusByStatusDataSet.FindDOTStatusByStatus[0].DOTStatusID;

                        if(strIMEI.Length > 12)
                        {
                            strGPSStatus = "IN USE";
                        }
                        else
                        {
                            strGPSStatus = "NOT IN USE";
                        }

                        TheFindGPSStatusByStatusDataSet = TheVehicleInfoClass.FindGPSStatusByStatus(strGPSStatus);

                        intGPSStatusID = TheFindGPSStatusByStatusDataSet.FindGPSStatusByStatus[0].GPSStatusID;

                        ImportVehiclesDataSet.importvehiclesRow NewVehicleRow = TheImportVehiclesDataSet.importvehicles.NewimportvehiclesRow();

                        NewVehicleRow.AssignedOffice = strAssignedOffice;
                        NewVehicleRow.EmployeeID = MainWindow.gintEmployeeID;
                        NewVehicleRow.CDLRequired = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.DOTStatus = strDOTStatus;
                        NewVehicleRow.IMEI = strIMEI;
                        NewVehicleRow.MedicalCardRequired = Convert.ToString((range.Cells[intCounter, 10] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.OdometerReading = Convert.ToInt32(Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2));
                        NewVehicleRow.TamperTag = Convert.ToString((range.Cells[intCounter, 12] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.VehicleMake = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.VehicleModel = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.VehicleNumber = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.VINNumber = Convert.ToString((range.Cells[intCounter, 7] as Excel.Range).Value2).ToUpper();
                        NewVehicleRow.Year = Convert.ToInt32(Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2));
                        NewVehicleRow.DOTStatusID = intDOTStatusID;
                        NewVehicleRow.GPSStatusID = intGPSStatusID;
                        NewVehicleRow.LicensePlate = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();

                        TheImportVehiclesDataSet.importvehicles.Rows.Add(NewVehicleRow);
                    }
            
                }

                PleaseWait.Close();
                dgrResults.ItemsSource = TheImportVehiclesDataSet.importvehicles;
                mitProcess.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Vehicles // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitProcess.IsEnabled = false;

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
        }
        private int FindEmployeeID(string strAssignedOffice)
        {
            int intEmployeeID = 0;
            int intCounter;
            int intNumberOfRecords;
            bool blnKeyWordNotFound = true;

            try
            {
                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(strAssignedOffice, MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);

                    if(blnKeyWordNotFound == false)
                    {
                        intEmployeeID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Vehicles // Find Employee ID " + Ex.Message);

                intEmployeeID = -1;
            }

            return intEmployeeID;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strVehicleNumber;
            int intYear;
            string strMake;
            string strModel;
            int intOdometerReading;
            string strVINNumber;
            string strAssignedOffice;
            int intEmployeeID;
            string strCDLRequired;
            bool blnCDLRequired = false;
            string strMedicalCardRequired;
            bool blnMedicalCardRequired = false;
            string strIMEI;
            string strTamperTag;
            int intDOTStatusID;
            int intGPSStatusID;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate = DateTime.Now;
            string strLicensePlate;
            bool blnFatalError;
            int intVehicleID;
            int intToolCounter;

            try
            {
                intNumberOfRecords = TheImportVehiclesDataSet.importvehicles.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strVehicleNumber = TheImportVehiclesDataSet.importvehicles[intCounter].VehicleNumber;
                    intYear = TheImportVehiclesDataSet.importvehicles[intCounter].Year;
                    strMake = TheImportVehiclesDataSet.importvehicles[intCounter].VehicleMake;
                    strModel = TheImportVehiclesDataSet.importvehicles[intCounter].VehicleModel;
                    intOdometerReading = TheImportVehiclesDataSet.importvehicles[intCounter].OdometerReading;
                    strVINNumber = TheImportVehiclesDataSet.importvehicles[intCounter].VINNumber;
                    strAssignedOffice = TheImportVehiclesDataSet.importvehicles[intCounter].AssignedOffice;
                    intEmployeeID = TheImportVehiclesDataSet.importvehicles[intCounter].EmployeeID;
                    strCDLRequired = TheImportVehiclesDataSet.importvehicles[intCounter].CDLRequired;

                    if(strCDLRequired == "YES")
                    {
                        blnCDLRequired = true;
                    }
                    else
                    {
                        blnCDLRequired = false;
                    }

                    strMedicalCardRequired = TheImportVehiclesDataSet.importvehicles[intCounter].MedicalCardRequired;

                    if(strMedicalCardRequired == "YES")
                    {
                        blnMedicalCardRequired = true;
                    }
                    else
                    {
                        blnMedicalCardRequired = false;
                    }

                    strIMEI = TheImportVehiclesDataSet.importvehicles[intCounter].IMEI;
                    strTamperTag = TheImportVehiclesDataSet.importvehicles[intCounter].TamperTag;
                    intDOTStatusID = TheImportVehiclesDataSet.importvehicles[intCounter].DOTStatusID;
                    intGPSStatusID = TheImportVehiclesDataSet.importvehicles[intCounter].GPSStatusID;
                    strLicensePlate = TheImportVehiclesDataSet.importvehicles[intCounter].LicensePlate;

                    blnFatalError = TheVehicleMainClass.InsertVehicleMain(0, strVehicleNumber, intYear, strMake, strModel, strLicensePlate, strVINNumber, intOdometerReading, datTransactionDate, intEmployeeID, "NO NOTES REPORTED", strAssignedOffice);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindActiveVehicleMainShortVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainShortVehicleNumber(strVehicleNumber);

                    intVehicleID = TheFindActiveVehicleMainShortVehicleNumberDataSet.FindActiveVehicleMainShortVehicleNumber[0].VehicleID;


                    blnFatalError = TheVehicleInfoClass.InsertVehicleInfo(intVehicleID, blnCDLRequired, blnMedicalCardRequired, intDOTStatusID, intGPSStatusID, strIMEI, Convert.ToInt32(strTamperTag));

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, intEmployeeID);

                    if (blnFatalError == true)
                        throw new Exception();

                    for (intToolCounter = 0; intToolCounter < 4; intToolCounter++)
                    {
                        if (intToolCounter == 0)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, 1009, 0);
                        else if (intToolCounter == 1)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, 1035, 0);
                        else if (intToolCounter == 2)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, 1066, 1);
                        else if (intToolCounter == 3)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, 1067, 1);
                    }

                }

                TheMessagesClass.InformationMessage("Vehicles Imported");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Vehicles // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchEmail();
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.MyOpenTasks();
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.MyOriginatingTask();
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.AddTask();
        }
    }
}
