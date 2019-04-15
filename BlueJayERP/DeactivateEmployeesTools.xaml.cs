/* Title:           Deactivated Employees Tools
 * Date:            8-22-18
 * Author:          Terry Holmes
 * 
 * Description:     This will show all deactivated employees Tools */

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
using NewToolsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DeactivateEmployeesTools.xaml
    /// </summary>
    public partial class DeactivateEmployeesTools : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolsClass = new ToolsClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up Data
        FindDeactivatedEmployeesDataSet TheFindDeactivatedEmployeesDataSet = new FindDeactivatedEmployeesDataSet();
        FindToolsByEmployeeIDDataSet TheFindToolsByEmployeeIDDataSet = new FindToolsByEmployeeIDDataSet();
        CurrentToolsDataSet TheCurrentToolsDataSet = new CurrentToolsDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        //setting global variables
        string gstrHeader;
        string gstrMessage;

        public DeactivateEmployeesTools()
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

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadGrid();
        }
        private void LoadGrid()
        {
            int intEmployeeCounter;
            int intEmployeeNumberOfRecords;
            int intToolCounter;
            int intToolNumberOfRecords;
            int intEmployeeID;
            int intCurrentLocation;

            try
            {
                TheCurrentToolsDataSet.currenttools.Rows.Clear();

                TheFindDeactivatedEmployeesDataSet = TheEmployeeClass.FindDeactivatedEmployees();

                intEmployeeNumberOfRecords = TheFindDeactivatedEmployeesDataSet.FindDeactivatedEmployees.Rows.Count - 1;

                for(intEmployeeCounter = 0; intEmployeeCounter <= intEmployeeNumberOfRecords; intEmployeeCounter++)
                {
                    intEmployeeID = TheFindDeactivatedEmployeesDataSet.FindDeactivatedEmployees[intEmployeeCounter].EmployeeID;

                    TheFindToolsByEmployeeIDDataSet = TheToolsClass.FindToolsByEmployeeID(intEmployeeID);

                    intToolNumberOfRecords = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID.Rows.Count - 1;

                    if(intToolNumberOfRecords > -1)
                    {
                        for(intToolCounter = 0; intToolCounter <= intToolNumberOfRecords; intToolCounter++)
                        {
                            intCurrentLocation = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].CurrentLocation;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intCurrentLocation);

                            CurrentToolsDataSet.currenttoolsRow NewToolRow = TheCurrentToolsDataSet.currenttools.NewcurrenttoolsRow();

                            NewToolRow.AssignedOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                            NewToolRow.FirstName = TheFindDeactivatedEmployeesDataSet.FindDeactivatedEmployees[intEmployeeCounter].FirstName;
                            NewToolRow.LastName = TheFindDeactivatedEmployeesDataSet.FindDeactivatedEmployees[intEmployeeCounter].LastName;
                            NewToolRow.ToolID = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].ToolID;
                            NewToolRow.Description = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].ToolDescription;
                            NewToolRow.TransactionDate = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].TransactionDate;

                            TheCurrentToolsDataSet.currenttools.Rows.Add(NewToolRow);
                        }
                    }
                }

                dgrTools.ItemsSource = TheCurrentToolsDataSet.currenttools;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Deactivate Employee Tools // Load Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitEmailReport_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress;

            try
            {
                EmailEmployees EmailEmployees = new EmailEmployees();
                EmailEmployees.ShowDialog();

                CreateReport();

                intNumberOfRecords = MainWindow.TheEmailListDataSet.employees.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strEmailAddress = MainWindow.TheEmailListDataSet.employees[intCounter].EmailAddress;

                    TheSendEmailClass.SendEmail(strEmailAddress, gstrHeader, gstrMessage);
                }

                TheMessagesClass.InformationMessage("Email Sent");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Deactivate Employee Tools // Email Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void CreateReport()
        {
            int intCounter;
            int intNumberOfRecords;
            gstrMessage = "";
            gstrHeader = "Tools Assigned to Dactivated Employees Report - Do Not Reply";

            try
            {
                intNumberOfRecords = TheCurrentToolsDataSet.currenttools.Rows.Count - 1;

                gstrMessage = "<h1>Tools Still Assigned To Deactivated Employees Report</h1>";
                gstrMessage += "<h3>Report Prepared By " + MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].FirstName + " " + MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].LastName + "</h3>";
                gstrMessage += "<table>";
                gstrMessage += "<tr>";
                gstrMessage += "<td><b>Tool ID</b></td>";
                gstrMessage += "<td><b>Tool Description</b></td>";
                gstrMessage += "<td><b>First Name</b></td>";
                gstrMessage += "<td><b>Last Name</b></td>";
                gstrMessage += "<td><b>Date</b></td>";
                gstrMessage += "<td><b>Assigned Office</b></td>";
                gstrMessage += "</tr>";
                gstrMessage += "<p>               </p>";

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gstrMessage += "<tr>";
                    gstrMessage += "<td>" + TheCurrentToolsDataSet.currenttools[intCounter].ToolID + "</td>";
                    gstrMessage += "<td>" + TheCurrentToolsDataSet.currenttools[intCounter].Description + "</td>";
                    gstrMessage += "<td>" + TheCurrentToolsDataSet.currenttools[intCounter].FirstName + "</td>";
                    gstrMessage += "<td>" + TheCurrentToolsDataSet.currenttools[intCounter].LastName + "</td>";
                    gstrMessage += "<td>" + Convert.ToString(TheCurrentToolsDataSet.currenttools[intCounter].TransactionDate) + "</td>";
                    gstrMessage += "<td>" + TheCurrentToolsDataSet.currenttools[intCounter].AssignedOffice + "</td>";
                    gstrMessage += "</tr>";
                }

                gstrMessage += "<table>";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Automate Vehicle Reports // Email Vehicle Exception Report " + Ex.Message);

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
