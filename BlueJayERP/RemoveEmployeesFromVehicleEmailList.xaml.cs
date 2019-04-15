/* Title:           Remove Employee From Vehicle Email List
 * Date:            9-5-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window for removing employees from the email list */

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
using VehicleExceptionEmailDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for RemoveEmployeesFromVehicleEmailList.xaml
    /// </summary>
    public partial class RemoveEmployeesFromVehicleEmailList : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleExceptionEmailClass TheVehicleExceptionEmailClass = new VehicleExceptionEmailClass();

        FindSortedVehicleEmailListDataSet TheFindSortedVehicleEmailListDataSet = new FindSortedVehicleEmailListDataSet();

        public RemoveEmployeesFromVehicleEmailList()
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
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MyOriginatingTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateAssignTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.AssignTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;

            TheFindSortedVehicleEmailListDataSet = TheVehicleExceptionEmailClass.FindSortedVehicleEmailList();

            dgrResults.ItemsSource = TheFindSortedVehicleEmailListDataSet.FindSortedVehicleEmailList;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindSortedVehicleEmailListDataSet = TheVehicleExceptionEmailClass.FindSortedVehicleEmailList();

            dgrResults.ItemsSource = TheFindSortedVehicleEmailListDataSet.FindSortedVehicleEmailList;
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell EmployeeID;
            string strEmployeeID;
            bool blnFatalError = false;
            int intEmployeeID;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if (intSelectedIndex > -1)
                {

                    const string message = "Are You Sure You Want To Remove This Employee From the List?";
                    const string caption = "Remove Employee";
                    MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                   

                    if (result == MessageBoxResult.Yes)
                    {
                        //setting local variable
                        dataGrid = dgrResults;
                        selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                        EmployeeID = (DataGridCell)dataGrid.Columns[1].GetCellContent(selectedRow).Parent;
                        strEmployeeID = ((TextBlock)EmployeeID.Content).Text;

                        intEmployeeID = Convert.ToInt32(strEmployeeID);

                        blnFatalError = TheVehicleExceptionEmailClass.RemoveVehicleInspectionEmail(intEmployeeID);

                        if (blnFatalError == true)
                            throw new Exception();

                        TheMessagesClass.InformationMessage("Employee Was Removed");

                        TheFindSortedVehicleEmailListDataSet = TheVehicleExceptionEmailClass.FindSortedVehicleEmailList();

                        dgrResults.ItemsSource = TheFindSortedVehicleEmailListDataSet.FindSortedVehicleEmailList;
                    }
                }
               
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Remove Employees From Vehicle Email List // Grid Selection " + Ex.Message);
            }
        }
    }
}
