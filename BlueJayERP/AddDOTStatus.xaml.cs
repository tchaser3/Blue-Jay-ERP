/* Title:           Add DOT Status
 * Date:            4-25-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to enter DOT Statuses */

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
using VehicleInfoDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddDOTStatus.xaml
    /// </summary>
    public partial class AddDOTStatus : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindDOTStatusByStatusDataSet TheFindDOTStatusByStatusDataSet = new FindDOTStatusByStatusDataSet();

        public AddDOTStatus()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtDOTStatus.Text = "";
            Visibility = Visibility.Hidden;
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDOTStatus;
            bool blnFatalError;
            int intRecordsReturned;

            try
            {
                strDOTStatus = txtDOTStatus.Text;
                if (strDOTStatus == "")
                {
                    TheMessagesClass.ErrorMessage("The DOT Status Was Not Entered");
                    return;
                }

                TheFindDOTStatusByStatusDataSet = TheVehicleInfoClass.FindDOTStatusByStatus(strDOTStatus);

                intRecordsReturned = TheFindDOTStatusByStatusDataSet.FindDOTStatusByStatus.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("DOT Status Already Exists");
                    return;
                }

                blnFatalError = TheVehicleInfoClass.InsertDOTStatus(strDOTStatus);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a problem, Contact ID");
                    return;
                }

                txtDOTStatus.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add DOT Status // Save Menu " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
