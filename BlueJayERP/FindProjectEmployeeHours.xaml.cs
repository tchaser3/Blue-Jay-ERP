/* Title:           Find Project Employee Hours
 * Date:            3-8-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to find project employee hours */

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
using ProjectsDLL;
using EmployeeProjectAssignmentDLL;
using ProjectTaskDLL;
using WorkTaskDLL;
using DateSearchDLL;
using Microsoft.Win32;
using NewEmployeeDLL;
using EmployeeCrewAssignmentDLL;
using EmployeeDateEntryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindProjectEmployeeHours.xaml
    /// </summary>
    public partial class FindProjectEmployeeHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectHoursDataSet TheFindProjectHoursDataSet = new FindProjectHoursDataSet();
        EmployeeCrewDataSet TheEmployeeCrewDataSet = new EmployeeCrewDataSet();
        FindAllEmployeesByLastNameDataSet TheFindAllEmployeesByLastNameDataSet = new FindAllEmployeesByLastNameDataSet();
        FindProjectWorkTaskDataSet TheFindProjectWorkTaskDataSet = new FindProjectWorkTaskDataSet();
        FindWorkTaskByWorkTaskDataSet TheFindWorkTaskByWorkTaskDataSet = new FindWorkTaskByWorkTaskDataSet();
        FindEmployeeCrewAssignmentByDateRangeDataSet TheFindEmployeeCrewAssignmentByDateRange = new FindEmployeeCrewAssignmentByDateRangeDataSet();
        ProjectDailyTaskDataSet TheProjectDailyTaskDataSet = new ProjectDailyTaskDataSet();
        FindCrewProductivityForATaskDataSet TheFindCrewProductivityForATaskDataSet = new FindCrewProductivityForATaskDataSet();
        FindProjectTaskHoursByAssignedProjectIDDataSet TheFindProjectTaskHoursByAssignedProjectIDDataSet = new FindProjectTaskHoursByAssignedProjectIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindWorkTaskByTaskIDDataSet TheFindWorkTaskByTaskIDDataSet = new FindWorkTaskByTaskIDDataSet();
        ProjectEmployeeHoursDataSet TheProjectEmployeeHoursDataSet = new ProjectEmployeeHoursDataSet();

        decimal gdecTotalHours;
        int gintTaskCounter;
        int gintTaskUIpperLimit;

        public FindProjectEmployeeHours()
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
                intRowNumberOfRecords = TheEmployeeCrewDataSet.employeework.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeCrewDataSet.employeework.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeCrewDataSet.employeework.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeCrewDataSet.employeework.Rows[intRowCounter][intColumnCounter].ToString();

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
                    intColumns = TheEmployeeCrewDataSet.employeework.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Production Report For " + txtEnterProjectID.Text))));
                    newTableRow.Cells[0].FontSize = 25;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Crew"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Task"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Footage/Pieces"))));
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

                    intNumberOfRecords = TheEmployeeCrewDataSet.employeework.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheEmployeeCrewDataSet.employeework[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Project Production Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Print Button " + Ex.Message);
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            TheEmployeeCrewDataSet.employeework.Rows.Clear();
            txtEnterProjectID.Text = "";
            txtTotalHours.Text = "";
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string strProjectID;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            string strFirstName;
            string strLastName;
            DateTime datTransactionDate;
            string strWorkTask;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            bool blnItemFound;
            int intTaskCounter;
            DateTime datStartDate = DateTime.Now;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                TheProjectDailyTaskDataSet.projecttask.Rows.Clear();
                TheProjectEmployeeHoursDataSet.employees.Rows.Clear();
                strProjectID = txtEnterProjectID.Text;
                if(strProjectID == "")
                {
                    TheMessagesClass.ErrorMessage("Project ID Was Not Entered");
                    PleaseWait.Close();
                    return;
                }

                gdecTotalHours = 0;
                TheEmployeeCrewDataSet.employeework.Rows.Clear();

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.InformationMessage("Project Was Not Found");
                    PleaseWait.Close();
                    return;
                }

                MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                TheFindProjectTaskHoursByAssignedProjectIDDataSet = TheProjectTaskClass.FindProjectTaskHoursByAssignedProjectID(strProjectID);

                datStartDate = TheDataSearchClass.SubtractingDays(datStartDate, 1200);

                TheFindProjectHoursDataSet = TheEmployeeProjectAssignmentClass.FindProjectHours(MainWindow.gintProjectID, datStartDate);

                intNumberOfRecords = TheFindProjectHoursDataSet.FindProjectHours.Rows.Count - 1;

                intSecondNumberOfRecords = 0;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strFirstName = TheFindProjectHoursDataSet.FindProjectHours[intCounter].FirstName;
                    strLastName = TheFindProjectHoursDataSet.FindProjectHours[intCounter].LastName;
                    strWorkTask = TheFindProjectHoursDataSet.FindProjectHours[intCounter].WorkTask;
                    datTransactionDate = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TransactionDate;
                    blnItemFound = false;

                    gdecTotalHours += TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;

                    if(intSecondNumberOfRecords > 0)
                    {
                        for(intSecondCounter = 0; intSecondCounter <  intSecondNumberOfRecords; intSecondCounter++)
                        {
                            if(datTransactionDate == TheEmployeeCrewDataSet.employeework[intSecondCounter].TransactionDate)
                            {
                                if(strFirstName == TheEmployeeCrewDataSet.employeework[intSecondCounter].FirstName)
                                {
                                    if(strLastName == TheEmployeeCrewDataSet.employeework[intSecondCounter].LastName)
                                    {
                                        if(strWorkTask == TheEmployeeCrewDataSet.employeework[intSecondCounter].WorkTask)
                                        {
                                            blnItemFound = true;
                                            TheEmployeeCrewDataSet.employeework[intSecondCounter].Hours += TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        EmployeeCrewDataSet.employeeworkRow NewEmployeeRow = TheEmployeeCrewDataSet.employeework.NewemployeeworkRow();

                        NewEmployeeRow.AssignedProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].AssignedProjectID;
                        NewEmployeeRow.Crew = "";
                        NewEmployeeRow.FirstName = strFirstName;
                        NewEmployeeRow.FootagePieces = 0;
                        NewEmployeeRow.Hours = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;
                        NewEmployeeRow.LastName = strLastName;
                        NewEmployeeRow.ProjectName = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                        NewEmployeeRow.TransactionDate = datTransactionDate;
                        NewEmployeeRow.WorkTask = strWorkTask;

                        TheEmployeeCrewDataSet.employeework.Rows.Add(NewEmployeeRow);
                        intSecondNumberOfRecords++;
                    }
                }

                intNumberOfRecords = intSecondNumberOfRecords - 1;
                
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strFirstName = TheEmployeeCrewDataSet.employeework[intCounter].FirstName;
                    strLastName = TheEmployeeCrewDataSet.employeework[intCounter].LastName;
                    strWorkTask = TheEmployeeCrewDataSet.employeework[intCounter].WorkTask;
                    datTransactionDate = TheEmployeeCrewDataSet.employeework[intCounter].TransactionDate;

                    TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);

                    MainWindow.gintWorkTaskID = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask[0].WorkTaskID;

                    MainWindow.gintEmployeeID = FindEmployeeID(strFirstName, strLastName);

                    if (MainWindow.gintEmployeeID == -1)
                        throw new Exception();

                    TheFindProjectWorkTaskDataSet = TheProjectTaskClass.FindProjectWorkTask(MainWindow.gintProjectID, MainWindow.gintEmployeeID, MainWindow.gintWorkTaskID, datTransactionDate, datTransactionDate);

                    intSecondNumberOfRecords = TheFindProjectWorkTaskDataSet.FindProjectWorkTask.Rows.Count - 1;

                    for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        TheEmployeeCrewDataSet.employeework[intCounter].FootagePieces += TheFindProjectWorkTaskDataSet.FindProjectWorkTask[intSecondCounter].FootagePieces;
                    }

                    TheFindEmployeeCrewAssignmentByDateRange = TheEmployeeCrewAssignmentClass.FindEmployeeCrewAssignmentByDateRange(MainWindow.gintEmployeeID, datTransactionDate, datTransactionDate);

                    intSecondNumberOfRecords = TheFindEmployeeCrewAssignmentByDateRange.FindEmployeeCrewAssignmentByDateRange.Rows.Count - 1;

                    for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        if(strProjectID == TheFindEmployeeCrewAssignmentByDateRange.FindEmployeeCrewAssignmentByDateRange[intSecondCounter].AssignedProjectID)
                        {
                            TheEmployeeCrewDataSet.employeework[intCounter].Crew = TheFindEmployeeCrewAssignmentByDateRange.FindEmployeeCrewAssignmentByDateRange[intSecondCounter].CrewID;
                        }
                    }
                }


                intNumberOfRecords = TheEmployeeCrewDataSet.employeework.Rows.Count - 1;
                gintTaskCounter = 0;
                gintTaskUIpperLimit = 0;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnItemFound = false;
                    strWorkTask = TheEmployeeCrewDataSet.employeework[intCounter].WorkTask;
                    datTransactionDate = TheEmployeeCrewDataSet.employeework[intCounter].TransactionDate;

                    if(gintTaskCounter > 0)
                    {
                        for(intTaskCounter = 0; intTaskCounter <= gintTaskUIpperLimit; intTaskCounter++)
                        {
                            if(datTransactionDate == TheProjectDailyTaskDataSet.projecttask[intTaskCounter].TransactionDate)
                            {
                                if(strWorkTask == TheProjectDailyTaskDataSet.projecttask[intTaskCounter].WorkTask)
                                {
                                    TheProjectDailyTaskDataSet.projecttask[intTaskCounter].Hours += TheEmployeeCrewDataSet.employeework[intCounter].Hours;
                                    blnItemFound = true;

                                    if(TheProjectDailyTaskDataSet.projecttask[intTaskCounter].Footage == 0)
                                    {
                                        TheProjectDailyTaskDataSet.projecttask[intTaskCounter].Footage = TheEmployeeCrewDataSet.employeework[intCounter].FootagePieces;
                                    }
                                }
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        ProjectDailyTaskDataSet.projecttaskRow NewTaskRow = TheProjectDailyTaskDataSet.projecttask.NewprojecttaskRow();

                        NewTaskRow.Crew = TheEmployeeCrewDataSet.employeework[intCounter].Crew;
                        NewTaskRow.Footage = TheEmployeeCrewDataSet.employeework[intCounter].FootagePieces;
                        NewTaskRow.Hours = TheEmployeeCrewDataSet.employeework[intCounter].Hours;
                        NewTaskRow.TransactionDate = TheEmployeeCrewDataSet.employeework[intCounter].TransactionDate;
                        NewTaskRow.WorkTask = TheEmployeeCrewDataSet.employeework[intCounter].WorkTask;

                        TheProjectDailyTaskDataSet.projecttask.Rows.Add(NewTaskRow);
                        gintTaskUIpperLimit = gintTaskCounter;
                        gintTaskCounter++;
                    }
                }
                
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);

                intNumberOfRecords = TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    MainWindow.gintWorkTaskID = TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID[intCounter].TaskID;
                    MainWindow.gintEmployeeID = TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID[intCounter].EmployeeID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                    TheFindWorkTaskByTaskIDDataSet = TheWorkTaskClass.FindWorkTaskByWorkTaskID(MainWindow.gintWorkTaskID);

                    ProjectEmployeeHoursDataSet.employeesRow NewEmployeeRow = TheProjectEmployeeHoursDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.FirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    NewEmployeeRow.LastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                    NewEmployeeRow.TotalHours = TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID[intCounter].EmployeeTotalHours;
                    NewEmployeeRow.TransactionDate = TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID[intCounter].TransactionDate;
                    NewEmployeeRow.WorkTask = TheFindWorkTaskByTaskIDDataSet.FindWorkTaskByWorkTaskID[0].WorkTask;

                    TheProjectEmployeeHoursDataSet.employees.Rows.Add(NewEmployeeRow);
                }

                dgrResults.ItemsSource = TheProjectDailyTaskDataSet.projecttask;
                dgrEmployees.ItemsSource = TheProjectEmployeeHoursDataSet.employees;
                bool blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Find Project Employee Hours // Generate Report Menu Item ");

            }
            catch(Exception Ex)
            {
                PleaseWait.Close();

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Project Employee Hours // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
        private int FindEmployeeID(string strFirstName, string strLastName)
        {
            int intEmployeeID = 0;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindAllEmployeesByLastNameDataSet = TheEmployeeClass.FindAllEmployeesByLastName(strLastName);

                intNumberOfRecords = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (strFirstName == TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].FirstName)
                    {
                        intEmployeeID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].EmployeeID;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Project Employee Hours //  Find Employee ID " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                intEmployeeID = -1;
            }

            return intEmployeeID;
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void dgrEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell WorkTask;
            DataGridCell TransactionDate;
            string strWorkTask;
            string strTransactionDate;
            DateTime datTransactionDate;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                if (dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    WorkTask = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    TransactionDate = (DataGridCell)dataGrid.Columns[1].GetCellContent(selectedRow).Parent;
                    strWorkTask = ((TextBlock)WorkTask.Content).Text;
                    strTransactionDate = ((TextBlock)TransactionDate.Content).Text;
                    datTransactionDate = Convert.ToDateTime(strTransactionDate);
                    datTransactionDate = TheDataSearchClass.RemoveTime(datTransactionDate);

                    TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);

                    MainWindow.gintWorkTaskID = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask[0].WorkTaskID;

                    TheFindCrewProductivityForATaskDataSet = TheEmployeeCrewAssignmentClass.FindCrewProductivityForATask(MainWindow.gintProjectID, MainWindow.gintWorkTaskID, datTransactionDate, datTransactionDate);

                    intNumberOfRecords = TheFindCrewProductivityForATaskDataSet.FindCrewProductivtyForATask.Rows.Count - 1;

                    TheProjectEmployeeHoursDataSet.employees.Rows.Clear();

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        MainWindow.gintWorkTaskID = TheFindCrewProductivityForATaskDataSet.FindCrewProductivtyForATask[intCounter].TaskID;
                        MainWindow.gintEmployeeID = TheFindCrewProductivityForATaskDataSet.FindCrewProductivtyForATask[intCounter].EmployeeID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                        TheFindWorkTaskByTaskIDDataSet = TheWorkTaskClass.FindWorkTaskByWorkTaskID(MainWindow.gintWorkTaskID);

                        ProjectEmployeeHoursDataSet.employeesRow NewEmployeeRow = TheProjectEmployeeHoursDataSet.employees.NewemployeesRow();

                        NewEmployeeRow.FirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                        NewEmployeeRow.LastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                        NewEmployeeRow.TotalHours = TheFindCrewProductivityForATaskDataSet.FindCrewProductivtyForATask[intCounter].EmployeeTotalHours;
                        NewEmployeeRow.TransactionDate = TheFindCrewProductivityForATaskDataSet.FindCrewProductivtyForATask[intCounter].TransactionDate;
                        NewEmployeeRow.WorkTask = TheFindWorkTaskByTaskIDDataSet.FindWorkTaskByWorkTaskID[0].WorkTask;

                        TheProjectEmployeeHoursDataSet.employees.Rows.Add(NewEmployeeRow);
                    }

                    dgrEmployees.ItemsSource = TheProjectEmployeeHoursDataSet.employees;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Send Vehicle To Shop // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtEnterProjectID.Text = "";
            txtTotalHours.Text = "";
            TheProjectDailyTaskDataSet.projecttask.Rows.Clear();
            dgrResults.ItemsSource = TheProjectDailyTaskDataSet.projecttask;
            TheFindCrewProductivityForATaskDataSet = TheEmployeeCrewAssignmentClass.FindCrewProductivityForATask(0,0,DateTime.Now, DateTime.Now);
            dgrEmployees.ItemsSource = TheFindCrewProductivityForATaskDataSet.FindCrewProductivtyForATask;
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
