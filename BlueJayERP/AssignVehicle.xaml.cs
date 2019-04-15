/* Title:           Assign Vehicle
 * Date:            4-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form where you can assign vehicles*/

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
using VehicleAssignmentDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignVehicle.xaml
    /// </summary>
    public partial class AssignVehicle : Window
    {
        //setting up the classes
        //setting up the clases
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindCurrentVehicleAssignmentByEmployeeIDDataSet TheFindCurrentVehicleAssignmentByEmployeeIDDataSet = new FindCurrentVehicleAssignmentByEmployeeIDDataSet();

        public AssignVehicle()
        {
            InitializeComponent();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void txtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting variables
            string strVehicleNumber;
            bool blnFatalError = false;
            int intRecordsReturned;
            int intLength;

            strVehicleNumber = txtVehicleNumber.Text;

            intLength = strVehicleNumber.Length;

            if (intLength >= 4)
            {
                blnFatalError = TheDataValidationClass.VerifyTextData(strVehicleNumber);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Number Entered Is not Integer");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(MainWindow.gintVehicleID);

                    intRecordsReturned = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        txtCurrentFirstName.Text = "Not Assigned";
                        txtCurrentLastName.Text = "Not Assigned";
                    }
                    else
                    {
                        txtCurrentFirstName.Text = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].FirstName;
                        txtCurrentLastName.Text = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].LastName;
                    }

                    cboSelectEmployee.IsEnabled = true;
                }
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will load the combo box
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            string strLastName;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                if (intLength >= 3)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                    }
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Vehicle // Enter Last Name Text Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strFullName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex;

                if (intSelectedIndex > 0)
                {
                    strFullName = cboSelectEmployee.SelectedItem.ToString();

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (strFullName == TheComboEmployeeDataSet.employees[intCounter].FullName)
                        {
                            MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                            break;
                        }
                    }

                    mitProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Vehicle // cbo Select Employee Selection Changed Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboSelectEmployee.IsEnabled = true;
            mitProcess.IsEnabled = true;
        }
        private void ResetControls()
        {
            txtVehicleNumber.Text = "";
            txtCurrentFirstName.Text = "";
            txtCurrentLastName.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.IsEnabled = false;
            mitProcess.IsEnabled = false;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //data validation
            string strValueForValidation;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intTransactionID;

            try
            {
                MainWindow.gstrVehicleNumber = txtVehicleNumber.Text;

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehice Number Not Valid\n";
                }
                else
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                }
                
                strValueForValidation = cboSelectEmployee.SelectedItem.ToString();
                if (strValueForValidation == "Select Employee")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindCurrentVehicleAssignmentByEmployeeIDDataSet = TheVehicleAssignmentClass.FindCurrentVehicleAssignmentByEmployeeID(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    if(TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID[0].LastName != "WAREHOUSE")
                    {
                        TheMessagesClass.ErrorMessage("Employee Is Assigned To Vehicle Number " + TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID[0].VehicleNumber);
                        return;
                    }
                }

                blnFatalError = TheVehicleMainClass.UpdateVehicleMainEmployeeID(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();


                intRecordsReturned = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID.Rows.Count;

                if (intRecordsReturned == 1)
                {
                    intTransactionID = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].TransactionID;

                    blnFatalError = TheVehicleAssignmentClass.UpdateCurrentVehicleAssignment(intTransactionID, false);

                    if (blnFatalError == true)
                    {
                        throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Record Has Been Saved");
                ResetControls();
                txtVehicleNumber.Focus();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Vehicle // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
