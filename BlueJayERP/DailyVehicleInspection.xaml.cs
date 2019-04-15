/* Title:           Daily Vehicle Inspection
 * Date:            4-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used for Daily Vehicle Inspections */

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
using InspectionsDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using VehicleHistoryDLL;
using VehicleProblemsDLL;
using VehicleMainDLL;
using DataValidationDLL;
using VehicleStatusDLL;
using VehicleAssignmentDLL;
using VehicleBulkToolsDLL;
using VehiclesInShopDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DailyVehicleInspection.xaml
    /// </summary>
    public partial class DailyVehicleInspection : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        InspectionsClass TheInspectionsClass = new InspectionsClass();
        VehicleHistoryClass TheVehicleHistoryClass = new VehicleHistoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();

        //Setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindDailyVehicleMainInspectionByDateMatchDataSet TheFindDailyVehicleMainInspectionByDateMatchDataSet = new FindDailyVehicleMainInspectionByDateMatchDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();
        FindVehicleMainInShopByVehicleIDDataSet TheFindVehicleMainInShopByVehicleIDDataSet = new FindVehicleMainInShopByVehicleIDDataSet();

        string gstrVehicleStatus;

        public DailyVehicleInspection()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will process the information
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intRecordsReturned;

            try
            {
                //data validation
                MainWindow.gstrVehicleNumber =txtEnterVehicleNumber.Text;
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);
                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;
                if(intRecordsReturned == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Number Not Found\n";
                }
                else
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(MainWindow.gintVehicleID);

                    if(TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].LastName == "WAREHOUSE")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vehicle is Assigned to a Warehouse and\nMust Be Assigned to a Technician\n";
                    }
                }
                
                strValueForValidation = cboEmployee.SelectedItem.ToString();
                if (strValueForValidation == "Select Employee")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
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

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                MainWindow.gdatTransactionDate = DateTime.Now;

                blnFatalError = TheInspectionsClass.InsertDailyVehicleInspection(MainWindow.gintVehicleID, MainWindow.gdatTransactionDate, MainWindow.gintEmployeeID, MainWindow.gstrInspectionStatus, MainWindow.gintOdometerReading);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                blnFatalError = TheVehicleHistoryClass.InsertVehicleHistory(MainWindow.gintVehicleID, MainWindow.gintEmployeeID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                blnFatalError = TheVehicleMainClass.UpdateVehicleMainEmployeeID(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact ID");
                    return;
                }


                TheFindDailyVehicleMainInspectionByDateMatchDataSet = TheInspectionsClass.FindDailyVehicleMainInspectionByDateMatch(MainWindow.gdatTransactionDate);

                MainWindow.gintInspectionID = TheFindDailyVehicleMainInspectionByDateMatchDataSet.FindDailyVehicleMainInspectionByDateMatch[0].TransactionID;

                TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, gstrVehicleStatus, DateTime.Now);

                if (rdoPassedServiceRequired.IsChecked == true)
                {

                    VehicleInspectionProblem VehicleInspectionProblem = new VehicleInspectionProblem();
                    VehicleInspectionProblem.ShowDialog();
                }

                txtEnterVehicleNumber.Text = "";
                cboEmployee.Items.Clear();
                txtEnterLastName.Text = "";
                txtOdometerReading.Text = "";
                rdoPassed.IsChecked = false;
                rdoPassedServiceRequired.IsChecked = false;
                cboBodyDamageReported.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Daily Vehicle Inspection // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspectioin // txtEnterLastName_TextChanged " + Ex.Message);

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

        private void cboBodyDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboBodyDamageReported.SelectedIndex;

            if (intSelectedIndex == 1)
            {
                ReportBodyDamage ReportBodyDamage = new ReportBodyDamage();
                ReportBodyDamage.ShowDialog();
            }
        }

        private void rdoPassed_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrInspectionStatus = "PASSED";
            gstrVehicleStatus = "NO PROBLEM";
        }

        private void rdoPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnServicable = true;
            MainWindow.gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            gstrVehicleStatus = "NEEDS WORK";
        }
        
        private void txtEnterVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intBJCLength;
            int intRecordsReturned;

            //data validation
            strValueForValidation = txtEnterVehicleNumber.Text;
            intBJCLength = strValueForValidation.Length;
            ControlsEnabled(true);

            if (intBJCLength >= 4)
            {
                MainWindow.gstrVehicleNumber = strValueForValidation;

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheFindVehicleMainInShopByVehicleIDDataSet = TheVehiclesInShopClass.FindVehicleMainInShopByVehicleID(MainWindow.gintVehicleID);

                    intRecordsReturned = TheFindVehicleMainInShopByVehicleIDDataSet.FindVehicleMainInShopByVehicleID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Is In Shop, Inspection Cannot Be Processed");
                        ControlsEnabled(false);
                        return;
                    }
                }
                else if (intBJCLength >= 6)
                {
                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }

                }
            }
        }
        private void ControlsEnabled(bool ValueBoolean)
        {
            txtEnterLastName.IsEnabled = ValueBoolean;
            txtOdometerReading.IsEnabled = ValueBoolean;
            cboBodyDamageReported.IsEnabled = ValueBoolean;
            cboEmployee.IsEnabled = ValueBoolean;
            rdoPassed.IsEnabled = ValueBoolean;
            rdoPassedServiceRequired.IsEnabled = ValueBoolean;
            mitProcess.IsEnabled = ValueBoolean;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitProcess.IsEnabled = false;

            MainWindow.gstrInspectionType = "DAILY";

            cboBodyDamageReported.Items.Add("Select Answer");
            cboBodyDamageReported.Items.Add("YES");
            cboBodyDamageReported.Items.Add("NO");
            cboBodyDamageReported.SelectedIndex = 0;
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
