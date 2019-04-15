/* Title:           Manager Weeily Audit Data Entry
 * Date:            4-10-18
 * Author:          Terry Holmes
 * 
 * Description:     This is where the Audit and Weekly Inspections are going */

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
using WeeklyInspectionsDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using VehicleMainDLL;
using DataValidationDLL;
using VehicleStatusDLL;
using WeeklyVehicleCleanliness;
using VehiclesInShopDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ManagerWeeklyAuditDataEntry.xaml
    /// </summary>
    public partial class ManagerWeeklyAuditDataEntry : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WeeklyInspectionClass TheWeeklyInspectionClass = new WeeklyInspectionClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();
        WeeklyVehicleCleanlinessClass TheWeeklyVehicleCleanlinessClass = new WeeklyVehicleCleanlinessClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        

        //setting up the data
        FindWeeklyVehicleInspectionIDDataSet TheFindWeeklyVehicleInspectionIDDataSet = new FindWeeklyVehicleInspectionIDDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        bool gblnVehicleCleanliness;

        public ManagerWeeklyAuditDataEntry()
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

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtEnterBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intBJCLength;
            int intRecordsReturned;

            //data validation
            strValueForValidation = txtEnterBJCNumber.Text;
            intBJCLength = strValueForValidation.Length;

            if (intBJCLength >= 4)
            {
                MainWindow.gstrVehicleNumber = strValueForValidation;

                TheFindVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                intRecordsReturned = TheFindVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    MainWindow.gintVehicleID = TheFindVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                   
                }
                else if(intBJCLength >= 6)
                {
                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }
                    
                }
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strLastName;
            int intLength;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                cboEmployee.Items.Clear();
                cboEmployee.Items.Add("Select Employee");

                if (intLength >= 3)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }
                    }
                    else
                    {
                        TheMessagesClass.InformationMessage("No Employees Found");
                    }

                    cboEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Audit Data Entry // txtEnterLastName_TextChanged " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            string strFullName;

            intSelectedIndex = cboEmployee.SelectedIndex;

            if (intSelectedIndex > 0)
            {
                strFullName = cboEmployee.SelectedItem.ToString();

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (strFullName == TheComboEmployeeDataSet.employees[intCounter].FullName)
                    {
                        MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                        mitProcess.IsEnabled = true;
                        break;
                    }
                }
            }
            else
            {
                mitProcess.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitProcess.IsEnabled = false;

            MainWindow.gstrInspectionType = "WEEKLY";

            cboBodyDamageReported.Items.Add("Select Answer");
            cboBodyDamageReported.Items.Add("YES");
            cboBodyDamageReported.Items.Add("NO");
            cboBodyDamageReported.SelectedIndex = 0;

            cboVehicleCleanliness.Items.Add("Select Cleanliness");
            cboVehicleCleanliness.Items.Add("Cleanliness Problems");
            cboVehicleCleanliness.Items.Add("No Cleanliness Problems");
            cboVehicleCleanliness.SelectedIndex = 0;
        }

        private void cboBodyDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;

            intSelectedIndex = cboBodyDamageReported.SelectedIndex;

            if(intSelectedIndex > 0)
            {

                MainWindow.gstrVehicleNumber = txtEnterBJCNumber.Text;

                TheFindVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                intRecordsReturned = TheFindVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Not Found");
                    return;
                }

                if (intSelectedIndex == 1)
                {
                    ReportBodyDamage ReportBodyDamage = new ReportBodyDamage();
                    ReportBodyDamage.ShowDialog();
                }
            }

        }

        private void rdoPassed_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrInspectionStatus = "NO PROBLEM";
        }

        private void rdoPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnServicable = true;
            MainWindow.gstrInspectionStatus = "PASSED SERVICE REQUIRED";
        }

        private void rdoFailed_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnServicable = false;
            MainWindow.gstrInspectionStatus = "NEEDS WORK";
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will process the information
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strVehicleNumber;
            int intRecordsReturned;

            try
            {
                strVehicleNumber = txtEnterBJCNumber.Text;
                TheFindVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);
                intRecordsReturned = TheFindVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;
                if(intRecordsReturned == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Number Entered Was Not Found\n";
                }
                else
                {
                    MainWindow.gintVehicleID = TheFindVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                }
                strValueForValidation = cboEmployee.SelectedItem.ToString();
                if (strValueForValidation == "Select Employee")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (rdoFailed.IsChecked == false)
                {
                    if (rdoPassed.IsChecked == false)
                    {
                        if (rdoPassedServiceRequired.IsChecked == false)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Passed, Passed Service Required, or Failure Was Not Checked\n";
                        }
                    }
                }
                strValueForValidation = txtOdometerReading.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Odometer Reading is not an Integer\n";
                }
                else
                {
                    MainWindow.gintOdometerReading = Convert.ToInt32(strValueForValidation);
                }
                if(cboVehicleCleanliness.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Cleanliness Was Not Selected\n";
                }

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                
                MainWindow.gdatTransactionDate = DateTime.Now;

                blnFatalError = TheWeeklyInspectionClass.InsertWeeklyVehicleInspection(MainWindow.gintVehicleID, MainWindow.gdatTransactionDate, MainWindow.gintEmployeeID, MainWindow.gstrInspectionStatus, MainWindow.gintOdometerReading);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact ID");
                    return;
                }

                TheFindWeeklyVehicleInspectionIDDataSet = TheWeeklyInspectionClass.FindWeelyVehicleInspectionID(MainWindow.gintVehicleID, MainWindow.gintEmployeeID, MainWindow.gintOdometerReading, MainWindow.gdatTransactionDate);

                MainWindow.gintInspectionID = TheFindWeeklyVehicleInspectionIDDataSet.FindWeeklyVehicleInspectionID[0].TransactionID;

                blnFatalError = TheWeeklyVehicleCleanlinessClass.InsertWeeklyVehicleCleanliness(MainWindow.gintInspectionID, MainWindow.gintVehicleID, gblnVehicleCleanliness, MainWindow.gstrCleanlinessNotes);

                if (blnFatalError == true)
                    throw new Exception();

                if ((rdoFailed.IsChecked == true) || (rdoPassedServiceRequired.IsChecked == true))
                {

                    VehicleInspectionProblem VehicleInspectionProblem = new VehicleInspectionProblem();
                    VehicleInspectionProblem.ShowDialog();
                }

                if (rdoFailed.IsChecked == true)
                {
                    PleaseWait PleaseWait = new PleaseWait();
                    PleaseWait.Show();

                    TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "DOWN", DateTime.Now);

                    TheSendEmailClass.EmailMessage(MainWindow.gstrVehicleNumber, MainWindow.gstrVehicleProblem);

                    PleaseWait.Close();
                }

                BulkToolsAssignedToVehicle BulkToolsAssignedToVehicle = new BulkToolsAssignedToVehicle();
                BulkToolsAssignedToVehicle.ShowDialog();

                txtEnterBJCNumber.Text = "";
                cboEmployee.Items.Clear();
                txtEnterLastName.Text = "";
                txtOdometerReading.Text = "";
                rdoFailed.IsChecked = false;
                rdoPassed.IsChecked = false;
                rdoPassedServiceRequired.IsChecked = false;
                cboBodyDamageReported.SelectedIndex = 0;
                cboVehicleCleanliness.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Audit Data Entry // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboVehicleCleanliness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboVehicleCleanliness.SelectedIndex == 1)
            {
                gblnVehicleCleanliness = true;
                VehicleCleanliness VehicleCleanliness = new VehicleCleanliness();
                VehicleCleanliness.ShowDialog();
            }
            else
            {
                MainWindow.gstrCleanlinessNotes = "NO NOTES WERE ENTERED";
                gblnVehicleCleanliness = false;
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
