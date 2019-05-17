/* Title:           Total Project Information
 * Date:            4-23-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to find the material and labor for a project */

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
using InventoryDLL;
using IssuedPartsDLL;
using ReceivePartsDLL;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for TotalProjectInformation.xaml
    /// </summary>
    public partial class TotalProjectInformation : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        IssuedPartsClass TheIssuedPartsClass = new IssuedPartsClass();
        ReceivePartsClass TheReceivePartsClass = new ReceivePartsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        
        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectHoursDataSet TheFindProjectHoursDataSet = new FindProjectHoursDataSet();
        FindProjectWorkTaskDataSet TheFindProjectWorkTaskDataSet = new FindProjectWorkTaskDataSet();
        CompleteProjectInfoDataSet TheCompleteProjectInfoDataSet = new CompleteProjectInfoDataSet();
        FindWorkTaskByWorkTaskDataSet TheFindWorkTaskByWorkTaskDataSet = new FindWorkTaskByWorkTaskDataSet();
        FindSpecificProjectWorkTaskDataSet TheFindSpecificProjectWorkTaskDataSet = new FindSpecificProjectWorkTaskDataSet();       

        decimal gdecTotalHours;       

        public TotalProjectInformation()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ProjectMaterialWindow.Visibility = Visibility.Hidden;
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
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
            decimal decHours;

            try
            {
                strProjectID = txtEnterProjectID.Text;
                TheCompleteProjectInfoDataSet.projectinfo.Rows.Clear();
                MainWindow.ProjectMaterialWindow.Visibility = Visibility.Hidden;

                gdecTotalHours = 0;

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Project Not Found");
                    return;
                }

                MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                TheFindProjectHoursDataSet = TheEmployeeProjectAssignmentClass.FindProjectHours(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindProjectHoursDataSet.FindProjectHours.Rows.Count - 1;
                intSecondNumberOfRecords = 0;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        decHours = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;
                        gdecTotalHours += decHours;
                        datTransactionDate = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TransactionDate;
                        strWorkTask = TheFindProjectHoursDataSet.FindProjectHours[intCounter].WorkTask;

                        if (intSecondNumberOfRecords > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondCounter++)
                            {
                                if (strWorkTask == TheCompleteProjectInfoDataSet.projectinfo[intSecondCounter].WorkTask)
                                {
                                    if (datTransactionDate == TheCompleteProjectInfoDataSet.projectinfo[intSecondCounter].TransactionDate)
                                    {
                                        blnItemFound = true;
                                    }
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            CompleteProjectInfoDataSet.projectinfoRow NewTaskRow = TheCompleteProjectInfoDataSet.projectinfo.NewprojectinfoRow();

                            NewTaskRow.FootagePieces = 0;
                            NewTaskRow.ProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].AssignedProjectID;
                            NewTaskRow.ProjectName = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                            NewTaskRow.TransactionDate = datTransactionDate;
                            NewTaskRow.WorkTask = strWorkTask;
                            NewTaskRow.Hours = decHours;

                            TheCompleteProjectInfoDataSet.projectinfo.Rows.Add(NewTaskRow);
                            intSecondNumberOfRecords++;
                        }

                    }
                }

                intNumberOfRecords = TheCompleteProjectInfoDataSet.projectinfo.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intFootage = 0;
                    strWorkTask = TheCompleteProjectInfoDataSet.projectinfo[intCounter].WorkTask;
                    datTransactionDate = TheCompleteProjectInfoDataSet.projectinfo[intCounter].TransactionDate;

                    TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);

                    MainWindow.gintWorkTaskID = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask[0].WorkTaskID;

                    TheFindSpecificProjectWorkTaskDataSet = TheProjectTaskClass.FindSpecificProjectWorkTask(MainWindow.gintProjectID, MainWindow.gintWorkTaskID);

                    intSecondNumberOfRecords = TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask.Rows.Count - 1;

                    for (intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        if (datTransactionDate == TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask[intSecondCounter].TransactionDate)
                        {
                            intFootage += Convert.ToInt32(TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask[intSecondCounter].FootagePieces);
                        }
                    }

                    TheCompleteProjectInfoDataSet.projectinfo[intCounter].FootagePieces = intFootage;
                }

                dgrResults.ItemsSource = TheCompleteProjectInfoDataSet.projectinfo;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);

                MainWindow.ProjectMaterialWindow.Visibility = Visibility.Visible;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Total Project Information // Generate Report Menu Item " + Ex.Message);

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
                intRowNumberOfRecords = TheCompleteProjectInfoDataSet.projectinfo.Rows.Count;
                intColumnNumberOfRecords = TheCompleteProjectInfoDataSet.projectinfo.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompleteProjectInfoDataSet.projectinfo.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompleteProjectInfoDataSet.projectinfo.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Total Project Information // Export to Excel " + ex.Message);

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
                    intColumns = TheCompleteProjectInfoDataSet.projectinfo.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        cancelledTable.Columns.Add(new TableColumn());
                    }
                    cancelledTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Labor Report For Project " + txtEnterProjectID.Text))));
                    newTableRow.Cells[0].FontSize = 20;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Labor Hours " + txtTotalHours.Text))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Task"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Footage/Pieces"))));

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheCompleteProjectInfoDataSet.projectinfo.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheCompleteProjectInfoDataSet.projectinfo[intReportRowCounter][intColumnCounter].ToString()))));


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
                    pdCancelledReport.PrintDocument(((IDocumentPaginatorSource)fdCancelledLines).DocumentPaginator, "Project Labor Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Total Project Report // Print Menu Item " + Ex.Message);
            }

            PleaseWait.Close();
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            TheCompleteProjectInfoDataSet.projectinfo.Rows.Clear();
            dgrResults.ItemsSource = TheCompleteProjectInfoDataSet.projectinfo;

            txtEnterProjectID.Text = "";
            txtTotalHours.Text = "";
        }
    }
}
