/* Title:           Remove Employees From Vehicles
 * Date:            6-1-18
 * Author:          Terry Holmes
 * 
 * Description:     This will remove inactive employees from vehicles if they are missed */

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
using VehicleMainDLL;
using NewEventLogDLL;
using VehicleAssignmentDLL;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for RemoveEmployeesFromVehicles.xaml
    /// </summary>
    public partial class RemoveEmployeesFromVehicles : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EventLogClass TheEventLogclass = new EventLogClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindVehicleMainByDeactiveEmployeesDataSet TheFindVehicleMainDeactiveEmployeesDataSet = new FindVehicleMainByDeactiveEmployeesDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();

        public RemoveEmployeesFromVehicles()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the grid
            TheFindVehicleMainDeactiveEmployeesDataSet = TheVehicleMainClass.FindVehicleMainByDeactiveEmployees();

            dgrResults.ItemsSource = TheFindVehicleMainDeactiveEmployeesDataSet.FindVehicleMainByDeactiveEmployees;

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
        }

        private void mitRemoveEmployees_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;
            string strHomeOffice;
            int intVehicleID;
            bool blnFatalError;
            int intTransactionID;

            try
            {
                intNumberOfRecords = TheFindVehicleMainDeactiveEmployeesDataSet.FindVehicleMainByDeactiveEmployees.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = TheFindVehicleMainDeactiveEmployeesDataSet.FindVehicleMainByDeactiveEmployees[intCounter].VehicleID;
                    strHomeOffice = TheFindVehicleMainDeactiveEmployeesDataSet.FindVehicleMainByDeactiveEmployees[intCounter].AssignedOffice;

                    intWarehouseID = FindWarehouseID(strHomeOffice);

                    TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(intVehicleID);

                    intTransactionID = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].TransactionID;
                    
                    blnFatalError = TheVehicleMainClass.UpdateVehicleMainEmployeeID(intVehicleID, intWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleAssignmentClass.UpdateCurrentVehicleAssignment(intTransactionID, false);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, intWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("Employees Have Been Removed from Vehicles");

                TheFindVehicleMainDeactiveEmployeesDataSet = TheVehicleMainClass.FindVehicleMainByDeactiveEmployees();

                dgrResults.ItemsSource = TheFindVehicleMainDeactiveEmployeesDataSet.FindVehicleMainByDeactiveEmployees;
            }
            catch (Exception Ex)
            {
                TheEventLogclass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Remove Employees From Vehicles // Remove Employess " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private int FindWarehouseID(string strHomeOffice)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID = 0;

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(strHomeOffice == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                {
                    intWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                }
            }

            return intWarehouseID;
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
