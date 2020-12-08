/* title:           Add Department
 * Date:            10-3-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the windows that will allow the department to be added */

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
using DepartmentDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddDepartment.xaml
    /// </summary>
    public partial class AddDepartment : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindDepartmentByNameDataSet TheFindDepartmentByNameDataSet = new FindDepartmentByNameDataSet();

        public AddDepartment()
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
            TheMessagesClass.LaunchEmail();
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtDepartment.Text = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDepartment;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                strDepartment = txtDepartment.Text;
                if(strDepartment == "")
                {
                    TheMessagesClass.ErrorMessage("Department Was Not Entered");
                    return;
                }

                TheFindDepartmentByNameDataSet = TheDepartmentClass.FindDepartmentByName(strDepartment);

                intRecordsReturned = TheFindDepartmentByNameDataSet.FindDepartmentByName.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Department was Already Added");
                    return;
                }

                blnFatalError = TheDepartmentClass.InsertDepartment(strDepartment , true);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Department Was Added");

                txtDepartment.Text = "";
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Department // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
