/* Title:           Find Employee Crew Assignments
 * Date:            2-21-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to find employee crew assignments */

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
using DataValidationDLL;
using DateSearchDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using EmployeeCrewAssignmentDLL;
using EmployeeProjectAssignmentDLL;
using ProjectTaskDLL;
using WorkTaskDLL;
using ProjectsDLL;
using Microsoft.Win32;
using EmployeeProductivityStatsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindEmployeeCrewAssignments.xaml
    /// </summary>
    public partial class FindEmployeeCrewAssignments : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeeProductiivityStatsClass TheEmployeeProductiivityStatsClass = new EmployeeProductiivityStatsClass();

        FindEmployeeCrewsByDateRangeDataSet TheFindEmployeeCrewsByDateRangeDataSet = new FindEmployeeCrewsByDateRangeDataSet();
        FindEmployeeCrewByCrewIDDataSet TheFindEmployeeCrewByCrewIDDataSet = new FindEmployeeCrewByCrewIDDataSet();
        CrewHoursDataSet TheCrewHoursDataSet = new CrewHoursDataSet();
        FindEmployeeCrewAssignmentComboBoxDataSet TheFindEmployeeCrewAssignmentComboBoxDataSet = new FindEmployeeCrewAssignmentComboBoxDataSet();
        FindEmployeeProjectHourDateRangeSummaryDataSet TheFindEmployeeProjectHourDateRangeSummaryDataSet = new FindEmployeeProjectHourDateRangeSummaryDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        decimal gdecTotalHours;
        string gstrCrewID;
        bool gblnLoadComplete;

        public FindEmployeeCrewAssignments()
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
                intRowNumberOfRecords = TheCrewHoursDataSet.crewhours.Rows.Count;
                intColumnNumberOfRecords = TheCrewHoursDataSet.crewhours.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCrewHoursDataSet.crewhours.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCrewHoursDataSet.crewhours.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Reports // Vehicle Problem Report // Export to Excel " + ex.Message);

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
                    intColumns = TheCrewHoursDataSet.crewhours.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Employee Labor Report for Crew ID " + gstrCrewID))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date Range " + Convert.ToString(gdatStartDate) + " Thru " + Convert.ToString(gdatEndDate)))));
                    newTableRow.Cells[0].FontSize = 18;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Hours Of  " + Convert.ToString(gdecTotalHours)))));
                    newTableRow.Cells[0].FontSize = 18;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));         
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Home Office"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Hours"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    //newTableRow.Cells[0].ColumnSpan = 1;
                    //newTableRow.Cells[1].ColumnSpan = 1;
                    //newTableRow.Cells[2].ColumnSpan = 1;
                    //newTableRow.Cells[3].ColumnSpan = 2;
                    //newTableRow.Cells[4].ColumnSpan = 1;

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheCrewHoursDataSet.crewhours.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheCrewHoursDataSet.crewhours[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                            //if (intColumnCounter == 3)
                            //{
                            //newTableRow.Cells[intColumnCounter].ColumnSpan = 2;
                            //}

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProblemReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProblemReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProblemReport.PrintableAreaWidth;
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Crew Labor Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Crew Assignment // Print Button " + Ex.Message);
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitFindCrews_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                gblnLoadComplete = false;
                cboSelectCrew.Items.Clear();
                cboSelectCrew.Items.Add("Select Crew");
                

                strValueForValidation = txtEnterStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Start Date is not a Start Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEnterEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "End Date is not a Start Date\n";
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
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is After The End Date");
                        return;
                    }
                }

                TheFindEmployeeCrewAssignmentComboBoxDataSet = TheEmployeeCrewAssignmentClass.FindEmployeeCrewAssignmentComboBox(gdatStartDate, gdatEndDate);

                intNumberOfRecords = TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectCrew.Items.Add(TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox[intCounter].CrewID);
                    }
                }

                cboSelectCrew.SelectedIndex = 0;
                gblnLoadComplete = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Crew Assignments // Find Crews Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectCrew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intProjectID;
            

            if(gblnLoadComplete == true)
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                try
                {
                    intSelectedIndex = cboSelectCrew.SelectedIndex;
                    gdecTotalHours = 0;
                    TheCrewHoursDataSet.crewhours.Rows.Clear();

                    if (gblnLoadComplete == true)
                    {
                        gstrCrewID = cboSelectCrew.SelectedItem.ToString();

                        TheFindEmployeeCrewByCrewIDDataSet = TheEmployeeCrewAssignmentClass.FindEmployeeCrewByCrewID(gstrCrewID, gdatStartDate, gdatEndDate);

                        intNumberOfRecords = TheFindEmployeeCrewByCrewIDDataSet.FindEmployeeCrewByCrewID.Rows.Count - 1;

                        if (intNumberOfRecords == -1)
                        {
                            TheMessagesClass.ErrorMessage("There is a Problem with Crew Assignments");
                            PleaseWait.Close();
                            return;
                        }
                        
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheFindEmployeeCrewByCrewIDDataSet.FindEmployeeCrewByCrewID[intCounter].EmployeeID;
                            intProjectID = TheFindEmployeeCrewByCrewIDDataSet.FindEmployeeCrewByCrewID[intCounter].ProjectID;

                            TheFindEmployeeProjectHourDateRangeSummaryDataSet = TheEmployeeProductiivityStatsClass.FindEmployeeProjectHourDateRangeSummary(intEmployeeID, intProjectID, gdatStartDate, gdatEndDate);

                            CrewHoursDataSet.crewhoursRow NewHoursRow = TheCrewHoursDataSet.crewhours.NewcrewhoursRow();

                            NewHoursRow.AssignedProjectID = TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].AssignedProjectID;
                            NewHoursRow.FirstName = TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].FirstName;
                            NewHoursRow.LastName = TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].LastName;
                            NewHoursRow.ProjectName = TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].ProjectName;
                            NewHoursRow.Hours = TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].TotalHours;
                            gdecTotalHours += TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].TotalHours;
                            NewHoursRow.HomeOffice = TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].HomeOffice;

                            TheCrewHoursDataSet.crewhours.Rows.Add(NewHoursRow);
                        }

                       

                        dgrResults.ItemsSource = TheCrewHoursDataSet.crewhours;

                        txtTotalHours.Text = Convert.ToString(gdecTotalHours);
                    }
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Crew Assignments // Combo Box Change Event " + Ex.Message);

                    TheMessagesClass.ErrorMessage(Ex.ToString());
                }

                PleaseWait.Close();
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
