/* Title:           Historical Vehicle Exception Report 
 * Date:            8-14-18
 * Author:          Terry Holmes
 * 
 * Description:     This will show the historical inspections */

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
using NewEventLogDLL;
using VehicleMainDLL;
using VehicleInYardDLL;
using InspectionsDLL;
using DateSearchDLL;
using Microsoft.Win32;


namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for HistoricalVehicleExceptionReport.xaml
    /// </summary>
    public partial class HistoricalVehicleExceptionReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleInYardClass TheVehiclesInYardClass = new VehicleInYardClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSete = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        HistoricalExceptionsDataSet TheHistoricalExceptionDataSet = new HistoricalExceptionsDataSet();
        FindVehiclesInYardByVehicleIDAndDateRangeDataSet TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = new FindVehiclesInYardByVehicleIDAndDateRangeDataSet();
        FindVehicleMainForAssignedOfficeDataSet TheFindVehicleMainForAssignedOfficeDataSet = new FindVehicleMainForAssignedOfficeDataSet();

        public HistoricalVehicleExceptionReport()
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

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mitExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheHistoricalExceptionDataSet.vehicleexception.Rows.Count;
                intColumnNumberOfRecords = TheHistoricalExceptionDataSet.vehicleexception.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheHistoricalExceptionDataSet.vehicleexception.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheHistoricalExceptionDataSet.vehicleexception.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Histor Vehicle Exception Report // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            DateTime datStartDate;
            DateTime datEndDated;
            DateTime datLimitDate;
            string strAssignedOffice;
            int intVehicleID;
            bool blnItemFound;

            try
            {
                TheHistoricalExceptionDataSet.vehicleexception.Rows.Clear();

                strAssignedOffice = txtOfficeName.Text;
                if(strAssignedOffice == "")
                {
                    TheMessagesClass.ErrorMessage("Assigned Office Was Not Entered");
                    return;
                }

                TheFindVehicleMainForAssignedOfficeDataSet = TheVehicleMainClass.FindVehicleMainForAssignedOffice(strAssignedOffice);

                intNumberOfRecords = TheFindVehicleMainForAssignedOfficeDataSet.FindVehicleMainForAssignedOffice.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("The Assigned Office Does Not Exist");
                    return;
                }

                datLimitDate = DateTime.Now;
                datLimitDate = TheDateSearchClass.RemoveTime(datLimitDate);
                datStartDate = TheDateSearchClass.SubtractingDays(datLimitDate, 90);

                while(datStartDate < datLimitDate)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;

                        datEndDated = TheDateSearchClass.AddingDays(datStartDate, 1);

                        intVehicleID = TheFindVehicleMainForAssignedOfficeDataSet.FindVehicleMainForAssignedOffice[intCounter].VehicleID;

                        TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSete = TheInspectionClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDated);

                        intRecordsReturned = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSete.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            blnItemFound = true;
                        }
                        else
                        {
                            TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = TheVehiclesInYardClass.FindVehiclesInYardByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDated);

                            intRecordsReturned = TheFindVehicleInYardByVehicleIDAndDateRangeDataSet.FindVehiclesInYardByVehicleIDAndDateRange.Rows.Count;

                            if(intRecordsReturned > 0)
                            {
                                blnItemFound = true;
                            }
                        }

                        if(blnItemFound == false)
                        {
                            HistoricalExceptionsDataSet.vehicleexceptionRow NewVehicleRow = TheHistoricalExceptionDataSet.vehicleexception.NewvehicleexceptionRow();

                            NewVehicleRow.AssignedOffice = strAssignedOffice;
                            NewVehicleRow.FirstName = TheFindVehicleMainForAssignedOfficeDataSet.FindVehicleMainForAssignedOffice[intCounter].FirstName;
                            NewVehicleRow.LastName = TheFindVehicleMainForAssignedOfficeDataSet.FindVehicleMainForAssignedOffice[intCounter].LastName;
                            NewVehicleRow.InspectionDate = datStartDate;
                            NewVehicleRow.VehicleID = intVehicleID;
                            NewVehicleRow.VehicleNumber = TheFindVehicleMainForAssignedOfficeDataSet.FindVehicleMainForAssignedOffice[intCounter].VehicleNumber;

                            TheHistoricalExceptionDataSet.vehicleexception.Rows.Add(NewVehicleRow);
                        }                            
                    }

                    datStartDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                    if(datStartDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        datStartDate = TheDateSearchClass.AddingDays(datStartDate, 2);
                    }
                    else if(datStartDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        datStartDate = TheDateSearchClass.AddingDays(datStartDate, 1);
                    }
                }

                dgrResults.ItemsSource = TheHistoricalExceptionDataSet.vehicleexception;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Historical Vehicle Exception Report // Generate Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
    }
}
