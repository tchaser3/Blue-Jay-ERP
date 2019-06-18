/* Title:           Add Vehicle to Employee Table
 * Date:            6-18-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used for adding the vehicles to the employee table */

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
using NewEmployeeDLL;
using VehicleMainDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddVehicleToEmployeeTable.xaml
    /// </summary>
    public partial class AddVehicleToEmployeeTable : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        FindAllEmployeesByLastNameDataSet TheFindAllEmployeebyLastNameDataSet = new FindAllEmployeesByLastNameDataSet();
        ImportVehiclesDataSet TheImportVehiclesDataSet = new ImportVehiclesDataSet();

        public AddVehicleToEmployeeTable()
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
            int intRecordsReturned;
            string strLastName;

            try
            {
                TheImportVehiclesDataSet.importvehicles.Rows.Clear();

                TheFindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();

                intNumberOfRecords = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strLastName = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleNumber;

                    TheFindAllEmployeebyLastNameDataSet = TheEmployeeClass.FindAllEmployeesByLastName(strLastName);

                    intRecordsReturned = TheFindAllEmployeebyLastNameDataSet.FindAllEmployeeByLastName.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        ImportVehiclesDataSet.importvehiclesRow NewVehicleRow = TheImportVehiclesDataSet.importvehicles.NewimportvehiclesRow();

                        NewVehicleRow.AssignedOffice = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].AssignedOffice;
                        NewVehicleRow.CDLRequired = "N/A";
                        NewVehicleRow.DOTStatus = "N/A";
                        NewVehicleRow.DOTStatusID = -1;
                        NewVehicleRow.EmployeeID = -1;
                        NewVehicleRow.GPSStatusID = -1;
                        NewVehicleRow.IMEI = "N/A";
                        NewVehicleRow.LicensePlate = "N/A";
                        NewVehicleRow.MedicalCardRequired = "N/A";
                        NewVehicleRow.OdometerReading = 0;
                        NewVehicleRow.VehicleMake = "N/A";
                        NewVehicleRow.VehicleModel = "N/A";
                        NewVehicleRow.VehicleNumber = strLastName;
                        NewVehicleRow.VINNumber = "N/A";
                        NewVehicleRow.Year = 0;
                        NewVehicleRow.TamperTag = "N/A";
                    
                        TheImportVehiclesDataSet.importvehicles.Rows.Add(NewVehicleRow);

                    }
                }

                dgrResutls.ItemsSource = TheImportVehiclesDataSet.importvehicles;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle To Employee Table // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle To Employee Table // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
