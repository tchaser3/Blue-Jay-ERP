/* Title:           Vehicle Exception Report
 * Date:            4-18-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the report that will run for the vehicle exception report */

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
using VehiclesInShopDLL;
using InspectionsDLL;
using VehicleAssignmentDLL;
using DateSearchDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleExceptionReport.xaml
    /// </summary>
    public partial class VehicleExceptionReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        VehiclesInShopClass TheVehicleInShopClass = new VehiclesInShopClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSete = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        VehicleExceptionDataSet TheVehicleExceptionDataSet = new VehicleExceptionDataSet();
        FindVehiclesInYardByVehicleIDAndDateRangeDataSet TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = new FindVehiclesInYardByVehicleIDAndDateRangeDataSet();
        FindVehicleMainInShopByVehicleIDDataSet TheFindVehicleMainInShopByVehicleIDDataSet = new FindVehicleMainInShopByVehicleIDDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindcurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();

        public VehicleExceptionReport()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
                intRowNumberOfRecords = TheVehicleExceptionDataSet.vehicleexception.Rows.Count;
                intColumnNumberOfRecords = TheVehicleExceptionDataSet.vehicleexception.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleExceptionDataSet.vehicleexception.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleExceptionDataSet.vehicleexception.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void mitPrint_Click(object sender, RoutedEventArgs e)
        {
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                PrintDialog pdCancelledReport = new PrintDialog();

                if (pdCancelledReport.ShowDialog().Value)
                {
                    FlowDocument fdCancelledLines = new FlowDocument();
                    Thickness thickness = new Thickness(100, 50, 50, 50);
                    fdCancelledLines.PagePadding = thickness;

                    //Set Up Table Columns
                    Table cancelledTable = new Table();
                    fdCancelledLines.Blocks.Add(cancelledTable);
                    cancelledTable.CellSpacing = 0;
                    intColumns = TheVehicleExceptionDataSet.vehicleexception.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        cancelledTable.Columns.Add(new TableColumn());
                    }
                    cancelledTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Exception Report"))));
                    newTableRow.Cells[0].FontSize = 20;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Assigned Office"))));

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheVehicleExceptionDataSet.vehicleexception.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheVehicleExceptionDataSet.vehicleexception[intReportRowCounter][intColumnCounter].ToString()))));


                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                        }
                    }

                    //Set up page and print
                    fdCancelledLines.ColumnWidth = pdCancelledReport.PrintableAreaWidth;
                    fdCancelledLines.PageHeight = pdCancelledReport.PrintableAreaHeight;
                    fdCancelledLines.PageWidth = pdCancelledReport.PrintableAreaWidth;
                    pdCancelledReport.PrintDocument(((IDocumentPaginatorSource)fdCancelledLines).DocumentPaginator, "Vehicle Exception Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Print Menu Item " + Ex.Message);
            }

            PleaseWait.Close();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            TheVehicleExceptionDataSet.vehicleexception.Rows.Clear();
            dgrResults.ItemsSource = TheVehicleExceptionDataSet.vehicleexception;
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitEmailReport_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strMessage = "";
            string strHeader = "Vehicle Exception Report - Do Not Reply";

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheVehicleExceptionDataSet.vehicleexception.Rows.Count - 1;

                strMessage = "<h1>Vehicle Exception Report<h1>";
                strMessage += "<table>";
                strMessage += "<tr>";
                strMessage += "<td><b>Vehicle Number</b></td>";
                strMessage += "<td><b>First Name</b></td>";
                strMessage += "<td><b>Last Name</b></td>";
                strMessage += "<td><b>Home Office</b></td>";
                strMessage += "</tr>";
                strMessage += "<p>               </p>";

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strMessage += "<tr>";
                    strMessage += "<td>" + TheVehicleExceptionDataSet.vehicleexception[intCounter].VehicleNumber + "</td>";
                    strMessage += "<td>" + TheVehicleExceptionDataSet.vehicleexception[intCounter].FirstName + "</td>";
                    strMessage += "<td>" + TheVehicleExceptionDataSet.vehicleexception[intCounter].LastName + "</td>";
                    strMessage += "<td>" + TheVehicleExceptionDataSet.vehicleexception[intCounter].AssignedOffice + "</td>";
                    strMessage += "</tr>";
                }

                strMessage += "<table>";

                TheSendEmailClass.VehicleReports(strHeader, strMessage);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP //Vehicle Exception Report // Email Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheVehicleExceptionDataSet.vehicleexception.Rows.Clear();
            dgrResults.ItemsSource = TheVehicleExceptionDataSet.vehicleexception;
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID;
            int intRecordsReturned;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate;
            string strFirstNamed = "";
            string strLastName = "";

            try
            {
                TheVehicleExceptionDataSet.vehicleexception.Rows.Clear();

                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);

                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                TheFindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();

                intNumberOfRecords = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleID;
                    
                    TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSete = TheInspectionClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                    intRecordsReturned = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSete.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = TheVehicleInYardClass.FindVehiclesInYardByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                        intRecordsReturned = TheFindVehicleInYardByVehicleIDAndDateRangeDataSet.FindVehiclesInYardByVehicleIDAndDateRange.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                           TheFindVehicleMainInShopByVehicleIDDataSet = TheVehicleInShopClass.FindVehicleMainInShopByVehicleID(intVehicleID);

                            intRecordsReturned = TheFindVehicleMainInShopByVehicleIDDataSet.FindVehicleMainInShopByVehicleID.Rows.Count;

                            if(intRecordsReturned == 0)
                            {
                                TheFindcurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(intVehicleID);

                                intRecordsReturned = TheFindcurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID.Rows.Count;

                                if(intRecordsReturned == 0)
                                {
                                    strLastName = "NOT ASSIGNED";
                                    strFirstNamed = "NOT ASSIGNED";
                                }
                                else
                                {
                                    strLastName = TheFindcurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].LastName;
                                    strFirstNamed = TheFindcurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].FirstName;
                                }

                                VehicleExceptionDataSet.vehicleexceptionRow NewVehicleRow = TheVehicleExceptionDataSet.vehicleexception.NewvehicleexceptionRow();

                                NewVehicleRow.AssignedOffice = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].AssignedOffice;
                                NewVehicleRow.FirstName = strFirstNamed;
                                NewVehicleRow.LastName = strLastName;
                                NewVehicleRow.VehicleID = intVehicleID;
                                NewVehicleRow.VehicleNumber = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleNumber;

                                TheVehicleExceptionDataSet.vehicleexception.Rows.Add(NewVehicleRow);
                            }
                        }
                    }
                }

                dgrResults.ItemsSource = TheVehicleExceptionDataSet.vehicleexception;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Exception Report // Generate The Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
