/* Title:           Audit Report Employee Assignment
 * Date:            8-20-18
 * Author:          Terry Holmes
 * 
 * Date:            this windows will all the user to add employees */


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
using NewEmployeeDLL;
using NewEventLogDLL;
using AuditReportEmailListDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AuditReportEmployeeAssignment.xaml
    /// </summary>
    public partial class AuditReportEmployeeAssignment : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        AuditReportEmailListClass TheAuditReportEmailListClass = new AuditReportEmailListClass();

        //setting up the data
        FindAuditReportEmployeeByEmployeeIDDataSet TheFindAuditReportEmployeeByEmployeeIDDataSet = new FindAuditReportEmployeeByEmployeeIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        //setting up variables
        int gintEmployeeID;
        string gstrEmailAddress;

        public AuditReportEmployeeAssignment()
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

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtLastName.Text;
               
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboSelectEmployee.SelectedIndex = 0;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Audit Report Employee Assignment // Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.Message);
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strFirstName;
            string strLastName;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    strFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName.Substring(0, 1).ToLower(); ;
                    strLastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName.ToLower();
                    gstrEmailAddress = strFirstName + strLastName + "@bluejaycommunications.com";
                    txtEmailAddress.Text = gstrEmailAddress;
                    mitProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Audit Report Employee Assignment // Combo Box Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TheFindAuditReportEmployeeByEmployeeIDDataSet = TheAuditReportEmailListClass.FindAuditReportEmployeeByEmployeeID(gintEmployeeID);

                if(TheFindAuditReportEmployeeByEmployeeIDDataSet.FindAuditReportEmployeeByEmployeeID.Rows.Count > 0)
                {
                    TheMessagesClass.InformationMessage("Employee Has Already Been Assigned");
                    return;
                }

                bool blnFatalError = TheAuditReportEmailListClass.InsertAuditReportEmailListEmployee(gintEmployeeID, gstrEmailAddress);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Employee Has Been Added");

                ResetForm();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Audit Report Employee Assignment // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ResetForm()
        {
            mitProcess.IsEnabled = false;
            txtEmailAddress.Text = "";
            txtLastName.Text = "";
            cboSelectEmployee.Items.Clear();
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetForm();
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
