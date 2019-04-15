/* Title:           Add GPS STatus
 * Date:            4-24-18
 * Author:          Terry Holmes
 * 
 * Description:     this is used to add a gps status */

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
    /// Interaction logic for AddGPSStatus.xaml
    /// </summary>
    public partial class AddGPSStatus : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindGPSStatusByStatusDataSet TheFindGPSStatusByStatusDataSet = new FindGPSStatusByStatusDataSet();

        public AddGPSStatus()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            // this will save the status
            string strGPSStatus;
            bool blnFatalError = false;
            int intRecordsReturned = 0;

            try
            {
                strGPSStatus = txtGPSStatus.Text;
                if (strGPSStatus == "")
                {
                    TheMessagesClass.ErrorMessage("The GPS Status Was Not Entered");
                    return;
                }

                TheFindGPSStatusByStatusDataSet = TheVehicleInfoClass.FindGPSStatusByStatus(strGPSStatus);

                intRecordsReturned = TheFindGPSStatusByStatusDataSet.FindGPSStatusByStatus.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The GPS Status Already Exists");
                    return;
                }

                blnFatalError = TheVehicleInfoClass.InsertGPSPlugStatus(strGPSStatus);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact IT");
                    return;
                }

                txtGPSStatus.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add GPS Status // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtGPSStatus.Text = "";
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
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
