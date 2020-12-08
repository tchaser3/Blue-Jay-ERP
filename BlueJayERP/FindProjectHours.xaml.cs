/* Title:           FindProjectHours
 * Date:            3-5-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to find project hours */

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
using ProjectTaskDLL;
using WorkTaskDLL;
using EmployeeProjectAssignmentDLL;
using Microsoft.Win32;
using DateSearchDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindProjectHours.xaml
    /// </summary>
    public partial class FindProjectHours : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectHoursDataSet TheFindProjectHoursDataSet = new FindProjectHoursDataSet();
        FindProjectWorkTaskDataSet TheFindProjectWorkTaskDataSet = new FindProjectWorkTaskDataSet();
        CompleteProjectInfoDataSet TheCompleteProjectInfoDataSet = new CompleteProjectInfoDataSet();
        FindWorkTaskByWorkTaskDataSet TheFindWorkTaskByWorkTaskDataSet = new FindWorkTaskByWorkTaskDataSet();
        FindSpecificProjectWorkTaskDataSet TheFindSpecificProjectWorkTaskDataSet = new FindSpecificProjectWorkTaskDataSet();
        ProjectWorkSummaryDataSet TheProjectWorkSummaryDataSet = new ProjectWorkSummaryDataSet();

        decimal gdecTotalHours;
        decimal gdecLaborCosts;
        int gintSummaryCounter;
        int gintSummaryUpperLimit;

        public FindProjectHours()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strProjectID;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strWorkTask;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            bool blnItemFound;
            int intFootage;
            int intSummaryCounter;
            decimal decHours;
            decimal decLaborCosts;
            DateTime datStartDate = DateTime.Now;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                strProjectID = txtEnterProjectID.Text;
                TheCompleteProjectInfoDataSet.projectinfo.Rows.Clear();
                TheProjectWorkSummaryDataSet.worksummary.Rows.Clear();
                gintSummaryCounter = 0;
                gintSummaryUpperLimit = 0;

                gdecTotalHours = 0;
                gdecLaborCosts = 0;

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Project Not Found");
                    return;
                }

                MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 1500);

                TheFindProjectHoursDataSet = TheEmployeeProjectAssignmentClass.FindProjectHours(MainWindow.gintProjectID, datStartDate);

                intNumberOfRecords = TheFindProjectHoursDataSet.FindProjectHours.Rows.Count - 1;
                intSecondNumberOfRecords = 0;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        gdecTotalHours += TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;
                        gdecLaborCosts += TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalEmployeeCost;
                        datTransactionDate = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TransactionDate;
                        strWorkTask = TheFindProjectHoursDataSet.FindProjectHours[intCounter].WorkTask;
                        decHours = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;
                        decLaborCosts = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalEmployeeCost;

                        if(intSecondNumberOfRecords > 0)
                        {
                            for(intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                            {
                                if(strWorkTask == TheCompleteProjectInfoDataSet.projectinfo[intSecondCounter].WorkTask)
                                {
                                    if(datTransactionDate == TheCompleteProjectInfoDataSet.projectinfo[intSecondCounter].TransactionDate)
                                    {
                                        TheCompleteProjectInfoDataSet.projectinfo[intSecondCounter].Hours += decHours;
                                        TheCompleteProjectInfoDataSet.projectinfo[intSecondCounter].LaborCosts += decLaborCosts;
                                        blnItemFound = true;
                                    }
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            CompleteProjectInfoDataSet.projectinfoRow NewTaskRow = TheCompleteProjectInfoDataSet.projectinfo.NewprojectinfoRow();

                            NewTaskRow.FootagePieces = 0;
                            NewTaskRow.TransactionDate = datTransactionDate;
                            NewTaskRow.WorkTask = strWorkTask;
                            NewTaskRow.Hours = decHours;
                            NewTaskRow.LaborCosts = decLaborCosts;

                            TheCompleteProjectInfoDataSet.projectinfo.Rows.Add(NewTaskRow);
                            intSecondNumberOfRecords++;
                        }
                        
                    }
                }

                intNumberOfRecords = TheCompleteProjectInfoDataSet.projectinfo.Rows.Count - 1;
                
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intFootage = 0;
                    strWorkTask = TheCompleteProjectInfoDataSet.projectinfo[intCounter].WorkTask;
                    datTransactionDate = TheCompleteProjectInfoDataSet.projectinfo[intCounter].TransactionDate;

                    TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);

                    MainWindow.gintWorkTaskID = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask[0].WorkTaskID;

                    TheFindSpecificProjectWorkTaskDataSet = TheProjectTaskClass.FindSpecificProjectWorkTask(MainWindow.gintProjectID, MainWindow.gintWorkTaskID);

                    intSecondNumberOfRecords = TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask.Rows.Count - 1;

                    for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        if(TheCompleteProjectInfoDataSet.projectinfo[intCounter].FootagePieces == 0)
                        {
                            if (datTransactionDate == TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask[intSecondCounter].TransactionDate)
                            {
                                intFootage = Convert.ToInt32(TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask[intSecondCounter].FootagePieces);

                                TheCompleteProjectInfoDataSet.projectinfo[intCounter].FootagePieces = intFootage;
                            }
                        }
                    }
                }

                intNumberOfRecords = TheCompleteProjectInfoDataSet.projectinfo.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnItemFound = false;

                    strWorkTask = TheCompleteProjectInfoDataSet.projectinfo[intCounter].WorkTask;

                    if(gintSummaryCounter > 0)
                    {
                        for(intSummaryCounter = 0; intSummaryCounter <= gintSummaryUpperLimit; intSummaryCounter++)
                        {
                            if(strWorkTask == TheProjectWorkSummaryDataSet.worksummary[intSummaryCounter].WorkTask)
                            {
                                TheProjectWorkSummaryDataSet.worksummary[intSummaryCounter].FootagePieces += TheCompleteProjectInfoDataSet.projectinfo[intCounter].FootagePieces;
                                TheProjectWorkSummaryDataSet.worksummary[intSummaryCounter].Hours += TheCompleteProjectInfoDataSet.projectinfo[intCounter].Hours;
                                blnItemFound = true;
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        ProjectWorkSummaryDataSet.worksummaryRow NewTaskRow = TheProjectWorkSummaryDataSet.worksummary.NewworksummaryRow();

                        NewTaskRow.FootagePieces = TheCompleteProjectInfoDataSet.projectinfo[intCounter].FootagePieces;
                        NewTaskRow.WorkTask = strWorkTask;
                        NewTaskRow.Hours = TheCompleteProjectInfoDataSet.projectinfo[intCounter].Hours;
                        NewTaskRow.LaborCosts = TheCompleteProjectInfoDataSet.projectinfo[intCounter].LaborCosts;

                        TheProjectWorkSummaryDataSet.worksummary.Rows.Add(NewTaskRow);

                        gintSummaryUpperLimit = gintSummaryCounter;
                        gintSummaryCounter++;
                    }
                }

                dgrResults.ItemsSource = TheProjectWorkSummaryDataSet.worksummary;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
                txtLaborCosts.Text = Convert.ToString(gdecLaborCosts);

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                PleaseWait.Close();

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Project Hours // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
                    intColumns = TheProjectWorkSummaryDataSet.worksummary.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Report For " + txtEnterProjectID.Text))));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Task"))));
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

                    intNumberOfRecords = TheProjectWorkSummaryDataSet.worksummary.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheProjectWorkSummaryDataSet.worksummary[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Project Information Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Project Hours // Print Menu Item " + Ex.Message);
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
                intRowNumberOfRecords = TheProjectWorkSummaryDataSet.worksummary.Rows.Count;
                intColumnNumberOfRecords = TheProjectWorkSummaryDataSet.worksummary.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectWorkSummaryDataSet.worksummary.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectWorkSummaryDataSet.worksummary.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Project Hours // Export to Excel " + ex.Message);

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

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtEnterProjectID.Text = "";
            txtTotalHours.Text = "";
            TheProjectWorkSummaryDataSet.worksummary.Rows.Clear();
            dgrResults.ItemsSource = TheProjectWorkSummaryDataSet.worksummary;
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DgrResults_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

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
