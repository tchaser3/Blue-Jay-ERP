/* Title:           Daily Trailer Inspection Report
 * Date:            11-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form that will allow the user to see the Daily Trailer Inspection Report */

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
using NewEmployeeDLL;
using DateSearchDLL;
using DataValidationDLL;
using DailyTrailerInspectionDLL;
using TrailersDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DailyTrailerInspectionReport.xaml
    /// </summary>
    public partial class DailyTrailerInspectionReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogclass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DailyTrailerInspectionClass TheDailyTrailerInspectionClass = new DailyTrailerInspectionClass();
        TrailersClass TheTrailersClass = new TrailersClass();

        FindDailyTrailerInspectionByDateRangeDataSet TheFindDailyTrailerInspectionByDateRangeDataSet = new FindDailyTrailerInspectionByDateRangeDataSet();
        FindDailyTrailerInspectionByEmployeeIDDataSet TheFindDailyTrailerInspectionByEmployeeIDDataSet = new FindDailyTrailerInspectionByEmployeeIDDataSet();
        FindDailyTrailerInspectionByTrailerIDDataSet TheFindDailyTrailerInspectionByTrailerIDDataSet = new FindDailyTrailerInspectionByTrailerIDDataSet();
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        InspectedTrailersDataSet TheInspectedTrailersDataSet = new InspectedTrailersDataSet();

        //setting global variables
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        bool gblnDateRange;
        bool gblnTrailerNumber;
        bool gblnEmployee;
        string gstrTrailerNumber;

        public DailyTrailerInspectionReport()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }
        private void ResetControls()
        {
            cboSelectEmployee.Items.Clear();
            txtEndDate.Text = "";
            txtEnterName.Text = "";
            txtStartDate.Text = "";
            rdoDateRange.IsChecked = true;
            txtEnterName.Visibility = Visibility.Hidden;
            lblEnterName.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            gblnDateRange = true;
            gblnEmployee = false;
            gblnTrailerNumber = false;
            TheInspectedTrailersDataSet.inspectedtrailers.Rows.Clear();
            dgrResults.ItemsSource = TheInspectedTrailersDataSet.inspectedtrailers;
            MainWindow.gintTrailerID = -1;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strErrorMessage = "";

            try
            {
                TheInspectedTrailersDataSet.inspectedtrailers.Rows.Clear();

                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
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
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }

                if(gblnDateRange == true)
                {
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    TheFindDailyTrailerInspectionByDateRangeDataSet = TheDailyTrailerInspectionClass.FindDailyTrailerInspectionByDateRange(gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            InspectedTrailersDataSet.inspectedtrailersRow NewTrailerRow = TheInspectedTrailersDataSet.inspectedtrailers.NewinspectedtrailersRow();

                            NewTrailerRow.FirstName = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].FirstName;
                            NewTrailerRow.InspectionDate = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].InspectionDate;
                            NewTrailerRow.InspectionNotes = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].InspectionNotes;
                            NewTrailerRow.InspectionStatus = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].InspectionStatus;
                            NewTrailerRow.LastName = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].LastName;
                            NewTrailerRow.TrailerNumber = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].TrailerNumber;
                            NewTrailerRow.HomeOffice = TheFindDailyTrailerInspectionByDateRangeDataSet.FindDailyTrailerInspectionByDateRange[intCounter].HomeOffice;

                            TheInspectedTrailersDataSet.inspectedtrailers.Rows.Add(NewTrailerRow);
                        }
                    }
                }
                else if(gblnTrailerNumber == true)
                {
                    if(MainWindow.gintTrailerID == -1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Trailer Number Was Not Entered or Found\n";
                    }
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    TheFindDailyTrailerInspectionByTrailerIDDataSet = TheDailyTrailerInspectionClass.FindDailyTrailerInspectionByTrailerID(MainWindow.gintTrailerID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            InspectedTrailersDataSet.inspectedtrailersRow NewTrailerRow = TheInspectedTrailersDataSet.inspectedtrailers.NewinspectedtrailersRow();

                            NewTrailerRow.FirstName = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID[intCounter].FirstName;
                            NewTrailerRow.InspectionDate = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID[intCounter].InspectionDate;
                            NewTrailerRow.InspectionNotes = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID[intCounter].InspectionNotes;
                            NewTrailerRow.InspectionStatus = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID[intCounter].InspectionStatus;
                            NewTrailerRow.LastName = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID[intCounter].LastName;
                            NewTrailerRow.TrailerNumber = gstrTrailerNumber;
                            NewTrailerRow.HomeOffice = TheFindDailyTrailerInspectionByTrailerIDDataSet.FindDailyTrailerInspectionByTrailerID[intCounter].HomeOffice;

                            TheInspectedTrailersDataSet.inspectedtrailers.Rows.Add(NewTrailerRow);
                        }
                    }
                }
                else if(gblnEmployee == true)
                {
                    if(cboSelectEmployee.SelectedIndex == 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    TheFindDailyTrailerInspectionByEmployeeIDDataSet = TheDailyTrailerInspectionClass.FindDailyTrailerInspectionByEmployeeID(MainWindow.gintEmployeeID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDailyTrailerInspectionByEmployeeIDDataSet.FindDailyTrailerInspectionByEmployeeID.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            InspectedTrailersDataSet.inspectedtrailersRow NewTrailerRow = TheInspectedTrailersDataSet.inspectedtrailers.NewinspectedtrailersRow();

                            NewTrailerRow.FirstName = MainWindow.gstrFirstName;
                            NewTrailerRow.InspectionDate = TheFindDailyTrailerInspectionByEmployeeIDDataSet.FindDailyTrailerInspectionByEmployeeID[intCounter].InspectionDate;
                            NewTrailerRow.InspectionNotes = TheFindDailyTrailerInspectionByEmployeeIDDataSet.FindDailyTrailerInspectionByEmployeeID[intCounter].InspectionNotes;
                            NewTrailerRow.InspectionStatus = TheFindDailyTrailerInspectionByEmployeeIDDataSet.FindDailyTrailerInspectionByEmployeeID[intCounter].InspectionStatus;
                            NewTrailerRow.LastName = MainWindow.gstrLastName;
                            NewTrailerRow.HomeOffice = MainWindow.gstrHomeOffice;
                            NewTrailerRow.TrailerNumber = TheFindDailyTrailerInspectionByEmployeeIDDataSet.FindDailyTrailerInspectionByEmployeeID[intCounter].TrailerNumber;

                            TheInspectedTrailersDataSet.inspectedtrailers.Rows.Add(NewTrailerRow);
                        }
                    }
                }

                dgrResults.ItemsSource = TheInspectedTrailersDataSet.inspectedtrailers;
            }
            catch (Exception Ex)
            {
                TheEventLogclass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Trailer Inspection Report // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
                intRowNumberOfRecords = TheInspectedTrailersDataSet.inspectedtrailers.Rows.Count;
                intColumnNumberOfRecords = TheInspectedTrailersDataSet.inspectedtrailers.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheInspectedTrailersDataSet.inspectedtrailers.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheInspectedTrailersDataSet.inspectedtrailers.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogclass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Trailer Inspection Report // Export to Excel " + ex.Message);

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
                    intColumns = TheInspectedTrailersDataSet.inspectedtrailers.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        cancelledTable.Columns.Add(new TableColumn());
                    }
                    cancelledTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Daily Trailer Inspection Report"))));
                    newTableRow.Cells[0].FontSize = 20;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Inspection Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Trailer Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Home Office"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Inspection Status"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Inspection Notes"))));
                    


                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheInspectedTrailersDataSet.inspectedtrailers.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheInspectedTrailersDataSet.inspectedtrailers[intReportRowCounter][intColumnCounter].ToString()))));


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
                    pdCancelledReport.PrintDocument(((IDocumentPaginatorSource)fdCancelledLines).DocumentPaginator, "Daily Trailer Inspection Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogclass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Trailer Inspection Report // Print Menu Item " + Ex.Message);
            }

            PleaseWait.Close();
        }

        private void rdoDateRange_Checked(object sender, RoutedEventArgs e)
        {
            gblnDateRange = true;
            gblnEmployee = false;
            gblnTrailerNumber = false;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            txtEnterName.Visibility = Visibility.Hidden;
            lblEnterName.Visibility = Visibility.Hidden;
        }

        private void rdoTrailerNumber_Checked(object sender, RoutedEventArgs e)
        {
            gblnTrailerNumber = true;
            gblnEmployee = false;
            gblnDateRange = false;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            txtEnterName.Visibility = Visibility.Visible;
            lblEnterName.Visibility = Visibility.Visible;
        }

        private void rdoEmployee_Checked(object sender, RoutedEventArgs e)
        {
            cboSelectEmployee.Items.Clear();
            gblnTrailerNumber = false;
            gblnEmployee = true;
            gblnDateRange = false;
            lblSelectEmployee.Visibility = Visibility.Visible;
            cboSelectEmployee.Visibility = Visibility.Visible;
            txtEnterName.Visibility = Visibility.Visible;
            lblEnterName.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void txtEnterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strEnteredData;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strEnteredData = txtEnterName.Text;
                intLength = strEnteredData.Length;

                if (gblnTrailerNumber == true)
                {
                    if ((intLength == 4) || (intLength >= 6))
                    {
                        TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strEnteredData);

                        intNumberOfRecords = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                        if (intNumberOfRecords < 1)
                        {
                            TheMessagesClass.ErrorMessage("Trailer Was Not Found");
                            return;
                        }

                        MainWindow.gintTrailerID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerID;
                        gstrTrailerNumber = strEnteredData;
                    }
                }
                else if (gblnEmployee == true)
                {
                    if (intLength > 2)
                    {
                        cboSelectEmployee.Items.Clear();
                        cboSelectEmployee.Items.Add("Select Employee");
                        TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strEnteredData);

                        intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                        if (intNumberOfRecords < 0)
                        {
                            TheMessagesClass.ErrorMessage("Employee Not Found");
                            return;
                        }

                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboSelectEmployee.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogclass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Trailer Inspection Report // Enter Name Text Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variable
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                MainWindow.gstrFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                MainWindow.gstrLastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;
            }
        }
    }
}
