/* Title:           Vehicle History Report
 * Date:            4-17-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the vehicle instory report */

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
using VehicleHistoryDLL;
using DataValidationDLL;
using DateSearchDLL;
using NewEmployeeDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleHistoryReport.xaml
    /// </summary>
    public partial class VehicleHistoryReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleHistoryClass TheVehicleHistoryClass = new VehicleHistoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehicleHistoryByDateRangeDataSet TheFindVehicleHistoryByDateRangeDataSet = new FindVehicleHistoryByDateRangeDataSet();
        FindVehicleHistoryByEmployeeIDAndDateRangeDataSet TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet = new FindVehicleHistoryByEmployeeIDAndDateRangeDataSet();
        FindVehicleHistoryByVehicleIDAndDateRangeDataSet TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet = new FindVehicleHistoryByVehicleIDAndDateRangeDataSet();
        VehicleHistoryReportDataSet TheVehicleHistoryReportDataSet = new VehicleHistoryReportDataSet();

        int gintEmployeeID;
        int gintVehicleID;
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        string gstrReportType;

        public VehicleHistoryReport()
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

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strVehicleNumber;
            int intRecordsReturned;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                TheVehicleHistoryReportDataSet.historyreport.Rows.Clear();
                
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Starting Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Ending Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Starting Date is After the Ending Date");
                        return;
                    }
                }


                if (gstrReportType == "DATE RANGE")
                {
                    TheFindVehicleHistoryByDateRangeDataSet = TheVehicleHistoryClass.FindVehicleHistoryByDateRange(gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindVehicleHistoryByDateRangeDataSet.FindVehicleHistoryByDateRange.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        VehicleHistoryReportDataSet.historyreportRow NewHistoryRow = TheVehicleHistoryReportDataSet.historyreport.NewhistoryreportRow();

                        NewHistoryRow.VehicleNumber = TheFindVehicleHistoryByDateRangeDataSet.FindVehicleHistoryByDateRange[intCounter].VehicleNumber;
                        NewHistoryRow.FirstName = TheFindVehicleHistoryByDateRangeDataSet.FindVehicleHistoryByDateRange[intCounter].FirstName;
                        NewHistoryRow.LastName = TheFindVehicleHistoryByDateRangeDataSet.FindVehicleHistoryByDateRange[intCounter].LastName;
                        NewHistoryRow.TransactionDate = TheFindVehicleHistoryByDateRangeDataSet.FindVehicleHistoryByDateRange[intCounter].TransactionDate;

                        TheVehicleHistoryReportDataSet.historyreport.Rows.Add(NewHistoryRow);
                    }
                }
                if (gstrReportType == "VEHICLE NUMBER")
                {
                    strVehicleNumber = txtSearchInfo.Text;
                    if(strVehicleNumber == "")
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Number Not Entered");
                        return;
                    }
                    else
                    {
                        TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                        intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            TheMessagesClass.ErrorMessage("Vehicle Not Found");
                            return;
                        }
                        else
                        {
                            gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                        }
                    }

                    TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet = TheVehicleHistoryClass.FindVehicleHistoryByVehicleIDAndDateRange(gintVehicleID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet.FindVehicleHistoryByVehicleIDAndDateRange.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        VehicleHistoryReportDataSet.historyreportRow NewHistoryRow = TheVehicleHistoryReportDataSet.historyreport.NewhistoryreportRow();

                        NewHistoryRow.VehicleNumber = TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet.FindVehicleHistoryByVehicleIDAndDateRange[intCounter].VehicleNumber;
                        NewHistoryRow.FirstName = TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet.FindVehicleHistoryByVehicleIDAndDateRange[intCounter].FirstName;
                        NewHistoryRow.LastName = TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet.FindVehicleHistoryByVehicleIDAndDateRange[intCounter].LastName;
                        NewHistoryRow.TransactionDate = TheFindVehicleHistoryByVehicleIDAndDateRangeDataSet.FindVehicleHistoryByVehicleIDAndDateRange[intCounter].TransactionDate;

                        TheVehicleHistoryReportDataSet.historyreport.Rows.Add(NewHistoryRow);
                    }
                }
                if (gstrReportType == "EMPLOYEE")
                {
                    TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet = TheVehicleHistoryClass.FindVehicleHistoryByEmployeeIDAndDateRange(gintEmployeeID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet.FindVehicleHistoryByEmployeeIDAndDateRange.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        VehicleHistoryReportDataSet.historyreportRow NewHistoryRow = TheVehicleHistoryReportDataSet.historyreport.NewhistoryreportRow();

                        NewHistoryRow.VehicleNumber = TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet.FindVehicleHistoryByEmployeeIDAndDateRange[intCounter].VehicleNumber;
                        NewHistoryRow.FirstName = TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet.FindVehicleHistoryByEmployeeIDAndDateRange[intCounter].FirstName;
                        NewHistoryRow.LastName = TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet.FindVehicleHistoryByEmployeeIDAndDateRange[intCounter].LastName;
                        NewHistoryRow.TransactionDate = TheFindVehicleHistoryByEmployeeIDAndDateRangeDataSet.FindVehicleHistoryByEmployeeIDAndDateRange[intCounter].TransactionDate;

                        TheVehicleHistoryReportDataSet.historyreport.Rows.Add(NewHistoryRow);
                    }
                }

                dgrResults.ItemsSource = TheVehicleHistoryReportDataSet.historyreport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle History Report // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void rdoDateRange_Checked(object sender, RoutedEventArgs e)
        {
            gstrReportType = "DATE RANGE";
            ResetControls();
        }

        private void rdoVehicleNumber_Checked(object sender, RoutedEventArgs e)
        {
            gstrReportType = "VEHICLE NUMBER";
            ResetControls();
            txtSearchInfo.Visibility = Visibility.Visible;
            lblSearchInfo.Visibility = Visibility.Visible;
            lblSearchInfo.Content = "Enter Vehicle Number";
        }

        private void rdoEmployee_Checked(object sender, RoutedEventArgs e)
        {
            gstrReportType = "EMPLOYEE";
            ResetControls();
            cboSelectEmployee.Visibility = Visibility.Visible;
            txtSearchInfo.Visibility = Visibility.Visible;
            lblSearchInfo.Visibility = Visibility.Visible;
            lblSelectEmployee.Visibility = Visibility.Visible;
            lblSearchInfo.Content = "Enter Last Name";
        }
        private void ResetControls()
        {
            cboSelectEmployee.Visibility = Visibility.Hidden;
            txtSearchInfo.Visibility = Visibility.Hidden;
            lblSearchInfo.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rdoDateRange.IsChecked = true;
        }

        private void txtSearchInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            string strSearchValue;
            
            strSearchValue = txtSearchInfo.Text;
            intLength = strSearchValue.Length;

            if (gstrReportType == "EMPLOYEE")
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strSearchValue);

                cboSelectEmployee.Items.Clear();

                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }
                }
                else
                {
                    TheMessagesClass.InformationMessage("Employee Not Found");
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting up for the loop
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;
            string strFullName;

            intSelectedIndex = cboSelectEmployee.SelectedIndex;

            if (intSelectedIndex > 0)
            {
                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                strFullName = cboSelectEmployee.SelectedItem.ToString();

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (strFullName == TheComboEmployeeDataSet.employees[intCounter].FullName)
                    {
                        gintEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                    }
                }
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
                    intColumns = TheVehicleHistoryReportDataSet.historyreport.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        cancelledTable.Columns.Add(new TableColumn());
                    }
                    cancelledTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle History Report"))));
                    newTableRow.Cells[0].FontSize = 20;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("TransactionID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("BJC Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));


                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheVehicleHistoryReportDataSet.historyreport.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheVehicleHistoryReportDataSet.historyreport[intReportRowCounter][intColumnCounter].ToString()))));


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
                    pdCancelledReport.PrintDocument(((IDocumentPaginatorSource)fdCancelledLines).DocumentPaginator, "Vehicle History Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle History Report // Print Menu Item " + Ex.Message);
            }

            PleaseWait.Close();
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
                intRowNumberOfRecords = TheVehicleHistoryReportDataSet.historyreport.Rows.Count;
                intColumnNumberOfRecords = TheVehicleHistoryReportDataSet.historyreport.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleHistoryReportDataSet.historyreport.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleHistoryReportDataSet.historyreport.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
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
