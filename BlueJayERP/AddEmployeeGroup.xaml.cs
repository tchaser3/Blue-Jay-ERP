/* Title:           Add Employee Group
 * Date:            2-9-128
 * Author:          Terry Holmes
 * 
 * Description:     This window is for adding employee groups */

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


namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddEmployeeGroup.xaml
    /// </summary>
    public partial class AddEmployeeGroup : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindEmployeeGroupByGroupNameDataSet TheFindEmployeeGroupByGroupNameDataSet = new FindEmployeeGroupByGroupNameDataSet();

        public AddEmployeeGroup()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            string strGroupName;
            int intRecordsReturned;
            bool blnFatalError = false;

            //data validation
            strGroupName = txtGroupName.Text;
            if (strGroupName == "")
            {
                TheMessagesClass.ErrorMessage("Group Name Was Not Entered");
                return;
            }

            //checking to see if the group name exists
            TheFindEmployeeGroupByGroupNameDataSet = TheEmployeeClass.FindEmployeeGroupByName(strGroupName);

            intRecordsReturned = TheFindEmployeeGroupByGroupNameDataSet.FindEmployeeGroupByGroupName.Rows.Count;

            if (intRecordsReturned != 0)
            {
                TheMessagesClass.ErrorMessage("Group Name Already Exists");
                return;
            }

            blnFatalError = TheEmployeeClass.CreateEmployeeGroup(strGroupName);

            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("There Was A Problem, Contact IT");
                return;
            }
            else
            {
                txtGroupName.Text = "";
                TheMessagesClass.InformationMessage("Group Name Was Saved");
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
