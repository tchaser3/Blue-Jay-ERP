/* Title:           Update Assigned Tasks
 * Date:            7-27-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form for the employee to update tasks assigned to them */

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
using AssignedTasksDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateAssignedTasks.xaml
    /// </summary>
    public partial class UpdateAssignedTasks : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AssignedTaskClass TheAssignedTaskClass = new AssignedTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //FindAssignedTasksByAssignedEmployeeIDDataSet TheFindAssignedTasksByAssignedEmployeeIDDataSet = new FindAssignedTasksByAssignedEmployeeIDDataSet();

        public UpdateAssignedTasks()
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
            LoadGrid();
        }
        private void LoadGrid()
        {
            
        }

        private void dgrOpenTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TaskID;
            string strTaskID;

            try
            {
                if(dgrOpenTasks.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrOpenTasks;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TaskID = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    strTaskID = ((TextBlock)TaskID.Content).Text;

                    //find the record
                    MainWindow.gintAssignedTaskID = Convert.ToInt32(strTaskID);

                    UpdateSpecificTasks UpdateSpecificTasks = new UpdateSpecificTasks();
                    UpdateSpecificTasks.ShowDialog();

                    MainWindow.TheFindAssignedTasksByAssignedEmployeeIDDataSet = TheAssignedTaskClass.FindAssignedTasksByAssignedEmployeeID(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                    dgrOpenTasks.ItemsSource = MainWindow.TheFindAssignedTasksByAssignedEmployeeIDDataSet.FindAssignedTasksByAssignedEmployeeID;
                }
               
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Assigned Tasks // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainWindow.TheFindAssignedTasksByAssignedEmployeeIDDataSet = TheAssignedTaskClass.FindAssignedTasksByAssignedEmployeeID(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

            dgrOpenTasks.ItemsSource = MainWindow.TheFindAssignedTasksByAssignedEmployeeIDDataSet.FindAssignedTasksByAssignedEmployeeID;
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //setting local variables
            int intEmployeeID;

            intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

            MainWindow.TheFindAssignedTasksByAssignedEmployeeIDDataSet = TheAssignedTaskClass.FindAssignedTasksByAssignedEmployeeID(intEmployeeID);

            dgrOpenTasks.ItemsSource = MainWindow.TheFindAssignedTasksByAssignedEmployeeIDDataSet.FindAssignedTasksByAssignedEmployeeID;
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }
    }
}
