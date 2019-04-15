/* Title:           Add Employee To Email List
 * Date:            9-5-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window to add employees to email list */

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
using VehicleExceptionEmailDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddEmployeeToVehicleEmailList.xaml
    /// </summary>
    public partial class AddEmployeeToVehicleEmailList : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleExceptionEmailClass TheVehicleExceptionEmailClass = new VehicleExceptionEmailClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeEmailAddressDataSet TheFindEmployeeEmailAddressDataSet = new FindEmployeeEmailAddressDataSet();
        FindVehicleEmailListByEmployeeIDDataSet TheFindVehicleEmailListByEmployeeIDDataSet = new FindVehicleEmailListByEmployeeIDDataSet();

        string gstrEmailAddress;

        public AddEmployeeToVehicleEmailList()
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
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                TheFindVehicleEmailListByEmployeeIDDataSet = TheVehicleExceptionEmailClass.FindVehicleEmailListByEmployeeID(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindVehicleEmailListByEmployeeIDDataSet.FindVehicleEmailListByEmployeeID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Is Already Part of Email List");
                    return;
                }

                blnFatalError = TheVehicleExceptionEmailClass.InsertVehicleInspectionEmail(MainWindow.gintEmployeeID, gstrEmailAddress);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Employee Has Been Added");

                ClearControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Employee To Vehicle Email List // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up variables
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;

            if(intLength > 2)
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                TheFindEmployeeEmailAddressDataSet = TheEmployeeClass.FindEmployeeEmailAddress(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    gstrEmailAddress = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].EmailAddress;
                    txtEmailAddress.Text = gstrEmailAddress;
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Employee Does Not Have a Email Address");
                }
            }
        }
        private void ClearControls()
        {
            txtEmailAddress.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
        }
        
        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ClearControls();
        }
    }
}
