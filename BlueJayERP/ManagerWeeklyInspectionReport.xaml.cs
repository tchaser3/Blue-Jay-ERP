/* Title:           Manager Weekly Inspection Report
 * Date:            4-9-18
 * Authour:         Terry Holmes
 * 
 * Description:     This form is used to reporting weekly inspection results */

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
using Microsoft.Win32;
using WeeklyInspectionsDLL;
using NewEventLogDLL;
using DataValidationDLL;
using DateSearchDLL;
using WeeklyVehicleCleanliness;
using WeeklyBulkToolInspectionDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ManagerWeeklyInspectionReport.xaml
    /// </summary>
    public partial class ManagerWeeklyInspectionReport : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        WeeklyInspectionClass TheWeeklyInspectionClass = new WeeklyInspectionClass();
        WeeklyVehicleCleanlinessClass TheWeeklyVehicleCleanlinessClass = new WeeklyVehicleCleanlinessClass();
        WeeklyBulkToolInspectionClass TheWeeklyBulkToolInspectionClass = new WeeklyBulkToolInspectionClass();

        FindWeeklyVehicleMainInspectionByDateRangeDataSet TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet = new FindWeeklyVehicleMainInspectionByDateRangeDataSet();
        FindWeeklyVehicleMainInspectionProblemByInspectionIDDataSet TheFindWeeklyVehicleMainInspectionProblemsByInspectionIDDataSet = new FindWeeklyVehicleMainInspectionProblemByInspectionIDDataSet();
        FindWeeklyVehicleCleanlinessByInspectionIDDataSet TheFindWeeklyVehicleCleanlinessByInspectionIDDataSet = new FindWeeklyVehicleCleanlinessByInspectionIDDataSet();
        FindWeeklyBulkToolInspectionByInspectionIDDataSet TheFindWeeklyBulkToolInspectionByInspectionIDDataSet = new FindWeeklyBulkToolInspectionByInspectionIDDataSet();


        //settup created data set
        AuditReportDataSet TheAuditReportDataSet = new AuditReportDataSet();

        public ManagerWeeklyInspectionReport()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strValueForValidation = "";
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            int intInspectionID;
            string strNotes;
            string strCleanlinessNotes;
            string strToolNotes;
            bool blnConesCorrect;
            bool blnSignsCorrect;
            bool blnFirstAidKitCorrect;
            bool blnFireExtinguisherCorrect;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //clearing the data set
                TheAuditReportDataSet.auditreport.Rows.Clear();

                //data validation
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage = "The Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                    datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage = "The End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                    datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
                    datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    PleaseWait.Close();
                    return;
                }

                TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet = TheWeeklyInspectionClass.FindWeeklyVehicleMainInspectionByDateRange(datStartDate, datEndDate);

                intNumberOfRecords = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strNotes = " ";
                        intInspectionID = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange[intCounter].TransactionID;

                        TheFindWeeklyVehicleMainInspectionProblemsByInspectionIDDataSet = TheWeeklyInspectionClass.FindWeeklyVehicleMainInspectionProblemByInspectionID(intInspectionID);

                        TheFindWeeklyVehicleCleanlinessByInspectionIDDataSet = TheWeeklyVehicleCleanlinessClass.FindWeeklyVehicleCleanlinessbyInspectionID(intInspectionID);

                        intRecordsReturned = TheFindWeeklyVehicleCleanlinessByInspectionIDDataSet.FindWeeklyVehicleCleanlinessByInspectionID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            strCleanlinessNotes = TheFindWeeklyVehicleCleanlinessByInspectionIDDataSet.FindWeeklyVehicleCleanlinessByInspectionID[0].CleanlinessNotes;
                        }
                        else
                        {
                            strCleanlinessNotes = "NO NOTES ENTERED";
                        }

                        intRecordsReturned = TheFindWeeklyVehicleMainInspectionProblemsByInspectionIDDataSet.FindWeeklyVehicleMainInspectionProblemByInspectionID.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            strNotes = TheFindWeeklyVehicleMainInspectionProblemsByInspectionIDDataSet.FindWeeklyVehicleMainInspectionProblemByInspectionID[0].VehicleProblem;
                        }

                        TheFindWeeklyBulkToolInspectionByInspectionIDDataSet = TheWeeklyBulkToolInspectionClass.FindWeeklyBulkToolInspectionByInspectionID(intInspectionID);

                        intRecordsReturned = TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            strToolNotes = "NO INSPECTION NOTES WERE FOUND";
                            blnConesCorrect = false;
                            blnSignsCorrect = false;
                            blnFirstAidKitCorrect = false;
                            blnFireExtinguisherCorrect = false;
                        }
                        else
                        {
                            if(TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].IsConesCorrectNull() == true)
                            {
                                blnConesCorrect = false;
                            }
                            else
                            {
                                blnConesCorrect = TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].ConesCorrect;
                            }

                            if(TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].IsSignsCorrectNull() == true)
                            {
                                blnSignsCorrect = false;
                            }
                            else
                            {
                                blnSignsCorrect = TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].SignsCorrect;
                            }

                            if(TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].IsFireExtingisherNull() == true)
                            {
                                blnFireExtinguisherCorrect = false;
                            }
                            else
                            {
                                blnFireExtinguisherCorrect = TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].FireExtingisher;
                            }

                            if(TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].IsFirstAidCorrectNull() == true)
                            {
                                blnFirstAidKitCorrect = false;
                            }
                            else
                            {
                                blnFirstAidKitCorrect = TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].FirstAidCorrect;
                            }

                            strToolNotes = TheFindWeeklyBulkToolInspectionByInspectionIDDataSet.FindWeeklyBulkToolInspectionByInspectionID[0].InspectionNotes;
                        }

                        AuditReportDataSet.auditreportRow NewInspectionRow = TheAuditReportDataSet.auditreport.NewauditreportRow();

                        NewInspectionRow.VehicleNumber = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange[intCounter].VehicleNumber;
                        NewInspectionRow.Date = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange[intCounter].InspectionDate;
                        NewInspectionRow.Findings = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange[intCounter].InspectionStatus;
                        NewInspectionRow.FirstName = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange[intCounter].FirstName;
                        NewInspectionRow.LastName = TheFindWeeklyVehicleMainInspectionbyDateRangeDataSet.FindWeeklyVehicleMainInspectionByDateRange[intCounter].LastName;
                        NewInspectionRow.Notes = strNotes;
                        NewInspectionRow.CleanlinessNotes = strCleanlinessNotes;
                        NewInspectionRow.ToolNotes = strToolNotes;
                        NewInspectionRow.ConesCorrect = blnConesCorrect;
                        NewInspectionRow.SignsCorrect = blnSignsCorrect;
                        NewInspectionRow.FirstAidCorrect = blnFirstAidKitCorrect;
                        NewInspectionRow.FireExtinguisherCorrect = blnFireExtinguisherCorrect;

                        TheAuditReportDataSet.auditreport.Rows.Add(NewInspectionRow);
                    }
                }

                dgrResults.ItemsSource = TheAuditReportDataSet.auditreport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Inspection Report // Generate Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void mitPrint_Click(object sender, RoutedEventArgs e)
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {
                PrintDialog pdProblemReport = new PrintDialog();

                if (pdProblemReport.ShowDialog().Value)
                {
                    FlowDocument fdProjectReport = new FlowDocument();
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdProjectReport.PagePadding = thickness;

                    pdProblemReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheAuditReportDataSet.auditreport.Columns.Count;
                    fdProjectReport.ColumnWidth = 10;
                    fdProjectReport.IsColumnWidthFlexible = false;


                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        ProjectReportTable.Columns.Add(new TableColumn());
                    }
                    ProjectReportTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Audit Vehicle Inspection Report"))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Findings"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Notes"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Cleanliness Notes"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Cones Correct"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Signs Correct"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Aid Correct"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Fire Correct"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Notes"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    newTableRow.Cells[0].ColumnSpan = 1;
                    newTableRow.Cells[1].ColumnSpan = 1;
                    newTableRow.Cells[2].ColumnSpan = 1;
                    newTableRow.Cells[3].ColumnSpan = 1;
                    newTableRow.Cells[4].ColumnSpan = 1;
                    newTableRow.Cells[5].ColumnSpan = 2;
                    newTableRow.Cells[6].ColumnSpan = 1;

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheAuditReportDataSet.auditreport.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheAuditReportDataSet.auditreport[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                            if (intColumnCounter == 5)
                            {
                                newTableRow.Cells[intColumnCounter].ColumnSpan = 2;
                            }

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProblemReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProblemReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProblemReport.PrintableAreaWidth;
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Audit Vehicle Inspection Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Inspection Report // Print Button " + Ex.Message);
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
                intRowNumberOfRecords = TheAuditReportDataSet.auditreport.Rows.Count;
                intColumnNumberOfRecords = TheAuditReportDataSet.auditreport.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheAuditReportDataSet.auditreport.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheAuditReportDataSet.auditreport.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Inspection Report // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
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
