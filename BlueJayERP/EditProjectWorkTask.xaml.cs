/* Title:           Edit Project Work Task
 * Date:            8-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This is where the work task ID will be changed */

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
using WorkTaskDLL;
using EmployeeProjectAssignmentDLL;
using ProjectTaskDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditProjectWorkTask.xaml
    /// </summary>
    public partial class EditProjectWorkTask : Window
    {
        //setting up the Classes+
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();

        //setting up the data
        FindWorkTaskByTaskKeywordDataSet TheFindWorktaskByTaskKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindEmployeeProjectAssignmentByTaskIDDataSet TheFindEmployeeProjectAssignmentByTaskIDDataSet = new FindEmployeeProjectAssignmentByTaskIDDataSet();
        OldWorkTasksDataSet TheOldWorkTasksDataSet = new OldWorkTasksDataSet();
        FindProjectTaskByTaskIDDataSet TheFindProjectTaskByTaskIDDataSet = new FindProjectTaskByTaskIDDataSet();

        int gintNewTaskID;

        public EditProjectWorkTask()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
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

        private void txtOldTaskName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strOldTask;
            int intLength;
            int intCounter;
            int intnumberOfRecords;

            try
            {
                strOldTask = txtOldTaskName.Text;
                intLength = strOldTask.Length;

                if(intLength >= 2)
                {
                    TheOldWorkTasksDataSet.worktask.Rows.Clear();

                    TheFindWorktaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strOldTask);

                    intnumberOfRecords = TheFindWorktaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count - 1;

                    if(intnumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("The Work Task Was Not Found ");
                        return;                         
                    }

                    for(intCounter = 0; intCounter <= intnumberOfRecords; intCounter++)
                    {
                        OldWorkTasksDataSet.worktaskRow NewTaskRow = TheOldWorkTasksDataSet.worktask.NewworktaskRow();

                        NewTaskRow.ChangeItems = false;
                        NewTaskRow.WorkTask = TheFindWorktaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask;
                        NewTaskRow.WorkTaskID = TheFindWorktaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTaskID;

                        TheOldWorkTasksDataSet.worktask.Rows.Add(NewTaskRow);
                    }

                    dgrOldTasks.ItemsSource = TheOldWorkTasksDataSet.worktask;
                }
            }
            catch (Exception EX)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project Work Task // Old Task Name Change " + EX.Message);

                TheMessagesClass.ErrorMessage(EX.ToString());
            }
        }

        private void txtNewTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            int intNumberOfRecords;
            string strWorkTask;
            int intLength;

            try
            {
                strWorkTask = txtNewTask.Text;
                intLength = strWorkTask.Length;

                if(intLength > 2)
                {
                    //dgrNewTask.Items.Clear();

                    TheFindWorktaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                    intNumberOfRecords = TheFindWorktaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("The Work Task Entered Does Not Exist");
                        return;
                    }

                    dgrNewTask.ItemsSource = TheFindWorktaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project Work // New Task Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrNewTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TaskID;
            string strTaskID;

            try
            {
                //setting local variable
                if(dgrNewTask.SelectedIndex > -1)
                {
                    dataGrid = dgrNewTask;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TaskID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTaskID = ((TextBlock)TaskID.Content).Text;

                    gintNewTaskID = Convert.ToInt32(strTaskID);
                    mitProcess.IsEnabled = true;
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project Work Task // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            mitProcess.IsEnabled = false;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting variables
            int intTaskCounter;
            int intTaskNumberOfRecords;
            int intTaskID;
            int intEmployeeCounter;
            int intEmployeeNumberOfRecords;
            int intTransactionID;
            bool blnFatalError = false;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intTaskNumberOfRecords = TheOldWorkTasksDataSet.worktask.Rows.Count - 1;

                for(intTaskCounter = 0; intTaskCounter <= intTaskNumberOfRecords; intTaskCounter++)
                {
                    if(TheOldWorkTasksDataSet.worktask[intTaskCounter].ChangeItems == true)
                    {
                        intTaskID = TheOldWorkTasksDataSet.worktask[intTaskCounter].WorkTaskID;

                        TheFindEmployeeProjectAssignmentByTaskIDDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProjectAssignementByTaskID(intTaskID);

                        intEmployeeNumberOfRecords = TheFindEmployeeProjectAssignmentByTaskIDDataSet.FindEmployeeProjectAssignmentByTaskID.Rows.Count - 1;

                        for(intEmployeeCounter = 0; intEmployeeCounter <= intEmployeeNumberOfRecords; intEmployeeCounter++)
                        {
                            intTransactionID = TheFindEmployeeProjectAssignmentByTaskIDDataSet.FindEmployeeProjectAssignmentByTaskID[intEmployeeCounter].TransactionID;

                            blnFatalError = TheEmployeeProjectAssignmentClass.UpdateEmployeeProjectAssignmentTaskID(intTransactionID, gintNewTaskID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }

                        blnFatalError = TheWorkTaskClass.UpdateWorkTaskActive(intTaskID, false);

                        TheFindProjectTaskByTaskIDDataSet = TheProjectTaskClass.FindProjectTaskByTaskID(intTaskID);

                        intEmployeeNumberOfRecords = TheFindProjectTaskByTaskIDDataSet.FindProjectTaskByTaskID.Rows.Count - 1;

                        for(intEmployeeCounter = 0; intEmployeeCounter <= intEmployeeNumberOfRecords; intEmployeeCounter++)
                        {
                            intTransactionID = TheFindProjectTaskByTaskIDDataSet.FindProjectTaskByTaskID[intEmployeeCounter].TransactionID;

                            blnFatalError = TheProjectTaskClass.UpdateProjectTaskID(intTransactionID, intTaskCounter);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }

                }

                TheMessagesClass.InformationMessage("All Tasks Have Been Updated");
                TheOldWorkTasksDataSet.worktask.Rows.Clear();
                dgrOldTasks.ItemsSource = TheOldWorkTasksDataSet.worktask;
                TheFindWorktaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword("llllllllll");
                dgrNewTask.ItemsSource = TheFindWorktaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword;
                txtNewTask.Text = "";
                txtOldTaskName.Text = "";

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project Work Task // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
    }
}
