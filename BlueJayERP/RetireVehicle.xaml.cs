/* Title:           Retire Vehicle
 * Date:            4-24-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to retire a vehicle*/

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
using VehicleMainDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for RetireVehicle.xaml
    /// </summary>
    public partial class RetireVehicle : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        public RetireVehicle()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtEnterVehicleNumber.Text = "";
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
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

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strVehicleNumber;
            int intRecordReturns;
            int intVehicleID;
            bool blnFatalError = false;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;
                if(strVehicleNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Was Not Entered");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordReturns = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordReturns == 0)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Was Not Found");
                    return;
                }

                intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                blnFatalError = TheVehicleMainClass.UpdateVehicleMainActive(intVehicleID, false);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Vehicle Has Been Retired");

                txtEnterVehicleNumber.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Retive Vehicle // Process Number " + Ex.Message);

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
