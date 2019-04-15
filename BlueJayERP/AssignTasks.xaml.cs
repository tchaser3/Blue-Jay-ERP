/* Title:           Assign Tasks
 * Date:            7-23-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to assign a task */

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
using DataValidationDLL;
using AssignedTasksDLL;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignTasks.xaml
    /// </summary>
    public partial class AssignTasks : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValiationClass = new DataValidationClass();
        AssignedTaskClass TheAssignTaskClass = new AssignedTaskClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        AssignEmployeesDataSet TheAssignEmployeesDataSet = new AssignEmployeesDataSet();
        FindAssignedTasksByDateMatchDataSet TheFindAssignedTaskByDateMatchDataSet = new FindAssignedTasksByDateMatchDataSet();
        FindEmployeeEmailAddressDataSet TheFindEmployeeEmailAddressDataSet = new FindEmployeeEmailAddressDataSet();
        
        public AssignTasks()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will load the combo box
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            string strLastName;

            strLastName = txtLastName.Text;
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");

            intLength = strLastName.Length;

            if(intLength > 2)
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }
        private void ResetControls()
        {
            txtLastName.Text = "";
            txtMessage.Text = "";
            txtSubject.Text = "";
            cboSelectEmployee.Items.Clear();
            TheAssignEmployeesDataSet.assignemployees.Rows.Clear();
            mitProcess.IsEnabled = false;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            string strLastName;
            string strFirstName;
            string strEmailAddress = "";
            int intSelectedIndex;
            int intEmployeeID;
            bool blnRecordFound = false;
            bool blnSendEmail = true;
            int intRecordsReturned;
            bool blnNotAnEmail;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    blnRecordFound = CheckForExistingEmployee(intEmployeeID);

                    if(blnRecordFound == true)
                    {
                        TheMessagesClass.ErrorMessage("Employee Has Already Been Added");

                        return;
                    }

                    strFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    strLastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;

                    TheFindEmployeeEmailAddressDataSet = TheEmployeeClass.FindEmployeeEmailAddress(intEmployeeID);

                    intRecordsReturned = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnSendEmail = false;
                    }
                    else if(intRecordsReturned > 0)
                    {
                        if(TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].IsEmailAddressNull() == false)
                        {
                            strEmailAddress = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].EmailAddress;

                            blnNotAnEmail = TheDataValiationClass.VerifyEmailAddress(strEmailAddress);

                            if(blnNotAnEmail == true)
                            {
                                blnSendEmail = false;
                            }
                            else if(blnNotAnEmail == false)
                            {
                                blnSendEmail = true;
                            }
                        }
                        else
                        {
                            blnSendEmail = false;
                        }
                        
                    }

                    AssignEmployeesDataSet.assignemployeesRow NewEmployeeRow = TheAssignEmployeesDataSet.assignemployees.NewassignemployeesRow();

                    NewEmployeeRow.EmailAddress = strEmailAddress;
                    NewEmployeeRow.EmployeeID = intEmployeeID;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.SendEmail = blnSendEmail;

                    TheAssignEmployeesDataSet.assignemployees.Rows.Add(NewEmployeeRow);

                    TheMessagesClass.InformationMessage("Employee Added"); 
                    txtLastName.Text = "";
                    cboSelectEmployee.Items.Clear();

                    mitProcess.IsEnabled = true;
                    
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Tasks // Combo Box Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private bool CheckForExistingEmployee(int intEmployeeID)
        {
            bool blnRecordFound = false;
            int intCounter;
            int intNumberOfRecords;

            intNumberOfRecords = TheAssignEmployeesDataSet.assignemployees.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if (intEmployeeID == TheAssignEmployeesDataSet.assignemployees[intCounter].EmployeeID)
                {
                    blnRecordFound = true;
                }
            }
            
            return blnRecordFound;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            string strMessageSubject = "A Task Has Been Assigned For You - ";
            string strSubject = "";
            string strMessageText;
            string strErrorMessage = "";
            int intAssignedEmployeeID;
            int intOriginatingEmployeeID;
            bool blnEmailSuccess;
            string strServerMessage = "";
            DateTime datTransactionDate = DateTime.Now;
            int intTaskID;

            try
            {
                if(txtSubject.Text == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Subject Was Not Entered\n";
                }
                else
                {
                    strMessageSubject += txtSubject.Text + " - Do Not Reply";
                    strSubject = txtSubject.Text;
                }
                strServerMessage = txtMessage.Text;
                if(strServerMessage == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Message is Blank\n";
                }

                strMessageText = "<h3>" + strMessageSubject + "</h3>" + "<p>" + strServerMessage + "</p>";

                intNumberOfRecords = TheAssignEmployeesDataSet.assignemployees.Rows.Count - 1;

                intOriginatingEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                blnFatalError = TheAssignTaskClass.InsertAssignedTask(intOriginatingEmployeeID, datTransactionDate, strSubject, strServerMessage);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindAssignedTaskByDateMatchDataSet = TheAssignTaskClass.FindAssignedTaskByDateMatch(datTransactionDate);

                intTaskID = TheFindAssignedTaskByDateMatchDataSet.FindAssignedTasksByDateMatch[0].TransactionID;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intAssignedEmployeeID = TheAssignEmployeesDataSet.assignemployees[intCounter].EmployeeID;

                    blnFatalError = TheAssignTaskClass.InsertAssignedEmployeeTasks(intAssignedEmployeeID, intTaskID);

                    if (blnFatalError == true)
                        throw new Exception();

                    if (TheAssignEmployeesDataSet.assignemployees[intCounter].SendEmail == true)
                    {
                        blnEmailSuccess = TheSendEmailClass.SendEmail(TheAssignEmployeesDataSet.assignemployees[intCounter].EmailAddress, strMessageSubject, strMessageText);

                        if (blnEmailSuccess == false)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Tasks Have Been Assigned");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Tasks // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }
    }
}
