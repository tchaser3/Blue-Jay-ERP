/* Title:           My Originating Tasks
 * Date:            8-2-18
 * Author:          Terry Holmes
 * 
 * Description:     This the window that will show all the tasks that you have created over a date range */

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
using AssignedTasksDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for MyOriginatingTasks.xaml
    /// </summary>
    public partial class MyOriginatingTasks : Window
    {
        //Setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AssignedTaskClass TheAssignedTaskClass = new AssignedTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindAssignedTasksByOriginatingEmployeeIDDataSet TheFindAssignedTasksbyOriginatingEmployeeIDDataSet = new FindAssignedTasksByOriginatingEmployeeIDDataSet();
        OriginatingTasksDataSet TheOriginatingTaskDataSet = new OriginatingTasksDataSet();

        public MyOriginatingTasks()
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
            
        }

        private void dgrTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TaskID;
            string strTaskID;

            try
            {
                if (dgrTasks.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrTasks;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TaskID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTaskID = ((TextBlock)TaskID.Content).Text;

                    //find the record
                    MainWindow.gintAssignedTaskID = Convert.ToInt32(strTaskID);

                    ViewTaskInformation ViewTaskInformation = new ViewTaskInformation();
                    ViewTaskInformation.ShowDialog();

                    LoadDataSet();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // My Originating Tasks // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadDataSet();            
        }
        private void LoadDataSet()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheOriginatingTaskDataSet.tasks.Rows.Clear();

                TheFindAssignedTasksbyOriginatingEmployeeIDDataSet = TheAssignedTaskClass.FindAssignedTaskByOriginatingEmployeeID(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                intNumberOfRecords = TheFindAssignedTasksbyOriginatingEmployeeIDDataSet.FindAssignedTasksByOriginatingEmployeeID.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    OriginatingTasksDataSet.tasksRow NewTaskRow = TheOriginatingTaskDataSet.tasks.NewtasksRow();

                    NewTaskRow.AssignedDate = TheFindAssignedTasksbyOriginatingEmployeeIDDataSet.FindAssignedTasksByOriginatingEmployeeID[intCounter].AssignedDate;
                    NewTaskRow.Task = TheFindAssignedTasksbyOriginatingEmployeeIDDataSet.FindAssignedTasksByOriginatingEmployeeID[intCounter].MessageSubject;
                    NewTaskRow.TaskComplete = TheFindAssignedTasksbyOriginatingEmployeeIDDataSet.FindAssignedTasksByOriginatingEmployeeID[intCounter].TaskComplete;
                    NewTaskRow.TaskID = TheFindAssignedTasksbyOriginatingEmployeeIDDataSet.FindAssignedTasksByOriginatingEmployeeID[intCounter].TransactionID;

                    TheOriginatingTaskDataSet.tasks.Rows.Add(NewTaskRow);
                }

                dgrTasks.ItemsSource = TheOriginatingTaskDataSet.tasks;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // My Originating Tasks // Load Data Set " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }
    }
}
