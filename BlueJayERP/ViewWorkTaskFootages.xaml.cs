/* Title:           View Work Task Footages
 * Date:            9-4-19
 * Author:          Terry HOlmes
 * 
 * Description:     This is used to see the footages */

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
using ProjectTaskDLL;
using NewEventLogDLL;
using WorkTaskDLL;
using DataValidationDLL;
using DesignProductivityDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewWorkTaskFootages.xaml
    /// </summary>
    public partial class ViewWorkTaskFootages : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DesignProductivityClass TheDesignProductivityClass = new DesignProductivityClass();

        FindProjectTasksForFootageDataSet TheFindProjectTasksForFootageDataSet = new FindProjectTasksForFootageDataSet();
        FindSortedWorkTaskDataSet TheFindSortedWorkTaskDataSet = new FindSortedWorkTaskDataSet();
        ProjectTaskTotalsDataSet TheProjectTaskTotalsDataSet = new ProjectTaskTotalsDataSet();

        bool gblnAllTasks;

        public ViewWorkTaskFootages()
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
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            TheFindSortedWorkTaskDataSet = TheWorkTaskClass.FindSortedWorkTask();

            intNumberOfRecords = TheFindSortedWorkTaskDataSet.FindSortedWorkTask.Rows.Count - 1;
            cboWorkTask.Items.Clear();
            cboWorkTask.Items.Add("Select Work Task");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboWorkTask.Items.Add(TheFindSortedWorkTaskDataSet.FindSortedWorkTask[intCounter].WorkTask);
            }

            cboWorkTask.SelectedIndex = 0;

            txtEndDate.Text = "";
            txtStartDate.Text = "";
            TheProjectTaskTotalsDataSet.projecttasktotals.Rows.Clear();
            dgrResults.ItemsSource = TheProjectTaskTotalsDataSet.projecttasktotals;
            lblWorkTask.Visibility = Visibility.Hidden;
            cboWorkTask.Visibility = Visibility.Hidden;
            cboSelectReport.Items.Clear();
            cboSelectReport.Items.Add("Select Report Type");
            cboSelectReport.Items.Add("All Work Tasks");
            cboSelectReport.Items.Add("One Work Task");
            cboSelectReport.SelectedIndex = 0;
            gblnAllTasks = true;
        }

        private void MitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intTaskCounter;
            int intTaskNumberOfRecords;
            int intProjectTaskCounter;
            int intProjectTaskNumberOfRecords;
            decimal decTotalFootages;
            int intWorkTaskID;
            string strWorkTask;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatStartDate = Convert.ToDateTime(strValueForValidation);
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
                    MainWindow.gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                TheProjectTaskTotalsDataSet.projecttasktotals.Rows.Clear();

                if (gblnAllTasks == true)
                {
                    TheFindSortedWorkTaskDataSet = TheWorkTaskClass.FindSortedWorkTask();

                    intTaskNumberOfRecords = TheFindSortedWorkTaskDataSet.FindSortedWorkTask.Rows.Count - 1;

                    for (intTaskCounter = 0; intTaskCounter <= intTaskNumberOfRecords; intTaskCounter++)
                    {
                        intWorkTaskID = TheFindSortedWorkTaskDataSet.FindSortedWorkTask[intTaskCounter].WorkTaskID;
                        strWorkTask = TheFindSortedWorkTaskDataSet.FindSortedWorkTask[intTaskCounter].WorkTask;

                        TheFindProjectTasksForFootageDataSet = TheProjectTaskClass.FindProjectTasksForFootage(intWorkTaskID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                        intProjectTaskNumberOfRecords = TheFindProjectTasksForFootageDataSet.FindProjectTasksForFootage.Rows.Count - 1;

                        if (intProjectTaskNumberOfRecords > -1)
                        {
                            decTotalFootages = 0;

                            for (intProjectTaskCounter = 0; intProjectTaskCounter <= intProjectTaskNumberOfRecords; intProjectTaskCounter++)
                            {
                                decTotalFootages += TheFindProjectTasksForFootageDataSet.FindProjectTasksForFootage[intProjectTaskCounter].FootagePieces;
                            }

                            ProjectTaskTotalsDataSet.projecttasktotalsRow NewTaskRow = TheProjectTaskTotalsDataSet.projecttasktotals.NewprojecttasktotalsRow();

                            NewTaskRow.WorkTaskID = intWorkTaskID;
                            NewTaskRow.WorkTask = strWorkTask;
                            NewTaskRow.FootagePieces = decTotalFootages;

                            TheProjectTaskTotalsDataSet.projecttasktotals.Rows.Add(NewTaskRow);
                        }

                    }
                }
                else if (gblnAllTasks == false)
                {
                    TheFindProjectTasksForFootageDataSet = TheProjectTaskClass.FindProjectTasksForFootage(MainWindow.gintWorkTaskID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    intProjectTaskNumberOfRecords = TheFindProjectTasksForFootageDataSet.FindProjectTasksForFootage.Rows.Count - 1;

                    if (intProjectTaskNumberOfRecords > -1)
                    {
                        decTotalFootages = 0;

                        for (intProjectTaskCounter = 0; intProjectTaskCounter <= intProjectTaskNumberOfRecords; intProjectTaskCounter++)
                        {
                            decTotalFootages += TheFindProjectTasksForFootageDataSet.FindProjectTasksForFootage[intProjectTaskCounter].FootagePieces;
                        }

                        ProjectTaskTotalsDataSet.projecttasktotalsRow NewTaskRow = TheProjectTaskTotalsDataSet.projecttasktotals.NewprojecttasktotalsRow();

                        NewTaskRow.WorkTaskID = MainWindow.gintWorkTaskID;
                        NewTaskRow.WorkTask = MainWindow.gstrWorkTask;
                        NewTaskRow.FootagePieces = decTotalFootages;

                        TheProjectTaskTotalsDataSet.projecttasktotals.Rows.Add(NewTaskRow);
                    }
                }
                

                dgrResults.ItemsSource = TheProjectTaskTotalsDataSet.projecttasktotals;

                PleaseWait.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Work Task Footages // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectReport.SelectedIndex == 1)
            {
                gblnAllTasks = true;
                lblWorkTask.Visibility = Visibility.Hidden;
                cboWorkTask.Visibility = Visibility.Hidden;
            }                
            else if (cboSelectReport.SelectedIndex == 2)
            {
                gblnAllTasks = false;
                lblWorkTask.Visibility = Visibility.Visible;
                cboWorkTask.Visibility = Visibility.Visible;
            } 
        }

        private void CboWorkTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboWorkTask.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWorkTaskID = TheFindSortedWorkTaskDataSet.FindSortedWorkTask[intSelectedIndex].WorkTaskID;
                MainWindow.gstrWorkTask = TheFindSortedWorkTaskDataSet.FindSortedWorkTask[intSelectedIndex].WorkTask;
            }
        }

        private void MitExportToExcel_Click(object sender, RoutedEventArgs e)
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
                intRowNumberOfRecords = TheProjectTaskTotalsDataSet.projecttasktotals.Rows.Count;
                intColumnNumberOfRecords = TheProjectTaskTotalsDataSet.projecttasktotals.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectTaskTotalsDataSet.projecttasktotals.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectTaskTotalsDataSet.projecttasktotals.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Work Task Footages // Export Department to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
