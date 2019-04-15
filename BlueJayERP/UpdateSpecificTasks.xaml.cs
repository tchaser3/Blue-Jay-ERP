/* Title:           Update Specific Task
 * Date:            7-25-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form that will show all the updates for a task and also reassignment of a task to another user */

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
using AssignedTasksDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateSpecificTasks.xaml
    /// </summary>
    public partial class UpdateSpecificTasks : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        AssignedTaskClass TheAssignedTaskClass = new AssignedTaskClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindAssignedTasksByTaskIDDataSet TheFindAssignedTasksByTaskIDDataSet = new FindAssignedTasksByTaskIDDataSet();
        FindAssignedTaskUpdateByTaskIDDataSet TheFindAssignedTaskUpdateByTaskIDDataSet = new FindAssignedTaskUpdateByTaskIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindAssignedEmployeeTaskByEmployeeIDTaskIDDataSet TheFindAssignedEmployeeTaskByEmployeeIDTaskIDDataSet = new FindAssignedEmployeeTaskByEmployeeIDTaskIDDataSet();
        AssignEmployeesDataSet TheAssigneEmployeesDataSet = new AssignEmployeesDataSet();
        FindOpenEmployeeAssignedTasksByTaskIDDataSet TheFindOpenEmployeeAssignedTasksByTaskIDDataSet = new FindOpenEmployeeAssignedTasksByTaskIDDataSet();
        FindEmployeeEmailAddressDataSet TheFindEmployeeEmailAddressDataSet = new FindEmployeeEmailAddressDataSet();

        bool gblnComplete;
        bool gblnReassignTask;

        public UpdateSpecificTasks()
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

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the data sets
            TheFindAssignedTasksByTaskIDDataSet = TheAssignedTaskClass.FindAssignedTasksByTaskID(MainWindow.gintAssignedTaskID);

            TheFindAssignedTaskUpdateByTaskIDDataSet = TheAssignedTaskClass.FindAssignedTaskUpdateByTaskID(MainWindow.gintAssignedTaskID);

            dgrTaskUpdates.ItemsSource = TheFindAssignedTaskUpdateByTaskIDDataSet.FindAssignedTaskUpdateByTaskID;

            txtOriginatingEmployee.Text = TheFindAssignedTasksByTaskIDDataSet.FindAssignedTaskByTaskID[0].FirstName + " " + TheFindAssignedTasksByTaskIDDataSet.FindAssignedTaskByTaskID[0].LastName;

            txtSubject.Text = TheFindAssignedTasksByTaskIDDataSet.FindAssignedTaskByTaskID[0].MessageSubject;

            txtMessage.Text = TheFindAssignedTasksByTaskIDDataSet.FindAssignedTaskByTaskID[0].MessageText;

            cboAssignTaskToNewEmployee.Items.Add("Select Answer");
            cboAssignTaskToNewEmployee.Items.Add("Yes");
            cboAssignTaskToNewEmployee.Items.Add("No");
            cboAssignTaskToNewEmployee.SelectedIndex = 0;
            rdoNo.IsChecked = true;
            rdoYes.IsChecked = false;

            SetEmployeeControlsVisible(false);

            TheAssigneEmployeesDataSet.assignemployees.Rows.Clear();
        }
        private void SetEmployeeControlsVisible(bool blnValueBoolean)
        {
            txtEnterLastName.IsEnabled = blnValueBoolean;
            cboSelectEmployee.IsEnabled = blnValueBoolean;

        }

        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            gblnComplete = true;
        }

        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnComplete = false;
        }

        private void cboAssignTaskToNewEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboAssignTaskToNewEmployee.SelectedIndex == 1)
            {
                gblnReassignTask = true;
                SetEmployeeControlsVisible(true);
            }
            else
            {
                gblnReassignTask = false;
                SetEmployeeControlsVisible(false);
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;

                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

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
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Specific Task // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strEmailAddress = "";
            int intEmployeeID;
            int intRecordsReturned;
            bool blnSendEmail = false;
            bool blnNotAnEmail = false;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    AssignEmployeesDataSet.assignemployeesRow NewEmployeeRow = TheAssigneEmployeesDataSet.assignemployees.NewassignemployeesRow();

                    NewEmployeeRow.EmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    NewEmployeeRow.FirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    NewEmployeeRow.LastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;

                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    TheFindEmployeeEmailAddressDataSet = TheEmployeeClass.FindEmployeeEmailAddress(intEmployeeID);

                    intRecordsReturned = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        blnSendEmail = false;
                    }
                    else if (intRecordsReturned > 0)
                    {
                        if (TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].IsEmailAddressNull() == false)
                        {
                            strEmailAddress = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].EmailAddress;

                            blnNotAnEmail = TheDataValidationClass.VerifyEmailAddress(strEmailAddress);

                            if (blnNotAnEmail == true)
                            {
                                blnSendEmail = false;
                            }
                            else if (blnNotAnEmail == false)
                            {
                                blnSendEmail = true;
                            }
                        }
                        else
                        {
                            blnSendEmail = false;
                        }

                    }


                    NewEmployeeRow.EmailAddress = strEmailAddress;
                    NewEmployeeRow.SendEmail = blnSendEmail;

                    /*TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID);

                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeGroup == "USERS")
                    {
                        NewEmployeeRow.SendEmail = false;
                    }
                    else
                    {
                        NewEmployeeRow.SendEmail = true;
                    }*/

                    TheAssigneEmployeesDataSet.assignemployees.Rows.Add(NewEmployeeRow);

                    TheMessagesClass.InformationMessage("Employee Added");
                    txtEnterLastName.Text = "";
                }

                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Specific Task // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strUpdate;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strMessageToEmail;
            string strEmailAddress;
            string strHeader = "A New Task Has Been Assigned To You - ";
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;

            try
            {
                strUpdate = txtTaskUpdate.Text;
                if(strUpdate == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Update Was Not Entered\n";
                }
                if(gblnReassignTask == true)
                {
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Employee Was Not Selected\n";
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheAssignedTaskClass.InsertAssignedTaskUpdate(MainWindow.gintAssignedTaskID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strUpdate);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader += txtSubject.Text + " - Do Not Reply";
                strMessageToEmail = "<h3>" + strHeader + "</H3>" + "<p>" + txtMessage.Text + "</p>";

                if (gblnReassignTask == true)
                {
                    intNumberOfRecords = TheAssigneEmployeesDataSet.assignemployees.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheAssigneEmployeesDataSet.assignemployees[intCounter].EmployeeID;

                        TheFindAssignedEmployeeTaskByEmployeeIDTaskIDDataSet = TheAssignedTaskClass.FindAssignedEmployeeTaskByEmployeeIDTaskID(MainWindow.gintAssignedTaskID, intEmployeeID);

                        intRecordsReturned = TheFindAssignedEmployeeTaskByEmployeeIDTaskIDDataSet.FindAssignedEmployeeTaskByEmployeeIDTaskID.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            blnFatalError = TheAssignedTaskClass.InsertAssignedEmployeeTasks(intEmployeeID, MainWindow.gintAssignedTaskID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }

                        if (TheAssigneEmployeesDataSet.assignemployees[intCounter].SendEmail == true)
                        {
                            strEmailAddress = TheAssigneEmployeesDataSet.assignemployees[intCounter].EmailAddress;

                            blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessageToEmail));

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                if(gblnComplete == true)
                {
                    blnFatalError = TheAssignedTaskClass.UpdateAssignedEmployeeTask(MainWindow.gintAssignedTaskID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, gblnComplete);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindOpenEmployeeAssignedTasksByTaskIDDataSet = TheAssignedTaskClass.FindOpenEmployeeAssignedTaskByTaskID(MainWindow.gintAssignedTaskID);

                    intRecordsReturned = TheFindOpenEmployeeAssignedTasksByTaskIDDataSet.FindOpenAssignedEmployeeTasksByTaskID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = TheAssignedTaskClass.UpdateAssignedTask(MainWindow.gintAssignedTaskID, DateTime.Now, true);


                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Tasks have been Updated");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Specific Tasks // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }
    }
}
