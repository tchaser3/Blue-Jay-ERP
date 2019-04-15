/* Title:           Find Employee Hours From Labor
 * Date:            2-27-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to how a specific employee hours */

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
using NewEventLogDLL;
using Microsoft.Win32;
using EmployeeProductivityStatsDLL;
using NewEmployeeDLL;
using EmployeeProjectAssignmentDLL;
using ProjectsDLL;
using WorkTaskDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindEmployeeHoursFromLabor.xaml
    /// </summary>
    public partial class FindEmployeeHoursFromLabor : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        ProjectClass TheProjectClass = new ProjectClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();

        EmployeeProjectSummaryDataSet TheEmployeeProjectSummaryDataSet = new EmployeeProjectSummaryDataSet();
        FindEmployeeTaskHoursByEmployeeIDDataSet TheFindEmployeeTaskHoursByEmployeeIDDataSet = new FindEmployeeTaskHoursByEmployeeIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindProjectTaskEmployeesStatsDataSet TheFindProjectTaskEmployeeStatsDataSet = new FindProjectTaskEmployeesStatsDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindWorkTaskByTaskIDDataSet TheFindWorkTaskByTaskIDDataSet = new FindWorkTaskByTaskIDDataSet();

        decimal gdecTotalHours;
        int gintSummaryCounter;
        int gintSummaryNumberOfRecords;

        public FindEmployeeHoursFromLabor()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                    intColumns = TheEmployeeProjectSummaryDataSet.projectsummary.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Employee Labor Report for " + MainWindow.gstrFirstName + " " + MainWindow.gstrLastName))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date Range " + Convert.ToString(MainWindow.gdatStartDate) + " Thru " + Convert.ToString(MainWindow.gdatEndDate)))));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Assigned Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Workt Task ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Task"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Hours"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Footage/Pieces"))));
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

                    intNumberOfRecords = TheEmployeeProjectSummaryDataSet.projectsummary.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheEmployeeProjectSummaryDataSet.projectsummary[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Employee Labor Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours From Labor // Print Menu Item " + Ex.Message);
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
                intRowNumberOfRecords = TheEmployeeProjectSummaryDataSet.projectsummary.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeProjectSummaryDataSet.projectsummary.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProjectSummaryDataSet.projectsummary.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProjectSummaryDataSet.projectsummary.Rows[intRowCounter][intColumnCounter].ToString();

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intProjectID = 0;
            int intTaskID = 0;
            decimal decTotalFootage = 0;
            string strWorkTask = "";
            string strAssignedProjectID = "";
            bool blnItemFound;
            int intSummaryCounter;
            decimal decHours;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            
            
            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            gdecTotalHours = 0;

            try
            {
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                MainWindow.gstrFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                MainWindow.gstrLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                TheFindEmployeeTaskHoursByEmployeeIDDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeTaskHoursByEmployeeID(MainWindow.gintEmployeeID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindEmployeeTaskHoursByEmployeeIDDataSet.FindEmployeeTaskHoursByEmployeeID.Rows.Count - 1;

                gintSummaryCounter = 0;
                gintSummaryNumberOfRecords = 0;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gdecTotalHours += TheFindEmployeeTaskHoursByEmployeeIDDataSet.FindEmployeeTaskHoursByEmployeeID[intCounter].EmployeeTotalHours;
                    intProjectID = TheFindEmployeeTaskHoursByEmployeeIDDataSet.FindEmployeeTaskHoursByEmployeeID[intCounter].ProjectID;
                    intTaskID = TheFindEmployeeTaskHoursByEmployeeIDDataSet.FindEmployeeTaskHoursByEmployeeID[intCounter].TaskID;
                    decHours = TheFindEmployeeTaskHoursByEmployeeIDDataSet.FindEmployeeTaskHoursByEmployeeID[intCounter].EmployeeTotalHours;
                    blnItemFound = false;
                    TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(intProjectID);
                    strAssignedProjectID = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].AssignedProjectID;
                    TheFindWorkTaskByTaskIDDataSet = TheWorkTaskClass.FindWorkTaskByWorkTaskID(intTaskID);
                    strWorkTask = TheFindWorkTaskByTaskIDDataSet.FindWorkTaskByWorkTaskID[0].WorkTask;

                    if (gintSummaryCounter > 0)
                    {
                        for(intSummaryCounter = 0; intSummaryCounter <= gintSummaryNumberOfRecords; intSummaryCounter++)
                        {
                            if (strAssignedProjectID == TheEmployeeProjectSummaryDataSet.projectsummary[intSummaryCounter].AssignedProjectID)
                            {
                                if(strWorkTask == TheEmployeeProjectSummaryDataSet.projectsummary[intSummaryCounter].WorkTask)
                                {
                                    TheEmployeeProjectSummaryDataSet.projectsummary[intSummaryCounter].TotalHours += decHours;
                                    blnItemFound = true;
                                }
                            }
                        }
                    }

                    if (blnItemFound == false)
                    {
                        EmployeeProjectSummaryDataSet.projectsummaryRow NewTaskRow = TheEmployeeProjectSummaryDataSet.projectsummary.NewprojectsummaryRow();

                        NewTaskRow.AssignedProjectID = strAssignedProjectID;
                        NewTaskRow.TotalFootagePieces = decTotalFootage;
                        NewTaskRow.TotalHours = decHours;
                        NewTaskRow.WorkTask = strWorkTask;
                        NewTaskRow.TaskID = intTaskID;
                        NewTaskRow.ProjectID = intProjectID;

                        TheEmployeeProjectSummaryDataSet.projectsummary.Rows.Add(NewTaskRow);
                        gintSummaryNumberOfRecords = gintSummaryCounter;
                        gintSummaryCounter++;
                    }   

                }

                intNumberOfRecords = TheEmployeeProjectSummaryDataSet.projectsummary.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intProjectID = TheEmployeeProjectSummaryDataSet.projectsummary[intCounter].ProjectID;
                    intTaskID = TheEmployeeProjectSummaryDataSet.projectsummary[intCounter].TaskID;
                    decTotalFootage = 0;

                    TheFindProjectTaskEmployeeStatsDataSet = TheEmployeeProductivityStatsClass.FindProjectTaskEmployeeStats(MainWindow.gintEmployeeID, intProjectID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    intSecondNumberOfRecords = TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats.Rows.Count - 1;

                    if(intSecondNumberOfRecords > -1)
                    {
                        for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                        {
                            if(intTaskID == TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats[intSecondCounter].WorkTaskID)
                            {
                                decTotalFootage += TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats[intSecondCounter].FootagePieces;
                            }
                        }
                    }

                    TheEmployeeProjectSummaryDataSet.projectsummary[intCounter].TotalFootagePieces = decTotalFootage;

                }

                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
                dgrResults.ItemsSource = TheEmployeeProjectSummaryDataSet.projectsummary;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Generate Report Menu Item " + Ex.Message);

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

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
