/* Title:           Report Body Damage
 * Date:            4-10-18
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the user to report body damage */

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
using VehicleBodyDamageDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ReportBodyDamage.xaml
    /// </summary>
    public partial class ReportBodyDamage : Window
    {
        //setting the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleBodyDamageClass TheVehicleBodyDamageClass = new VehicleBodyDamageClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data
        FindVehicleMainBodyDamageByVehicleIDDataSet TheFindVehicleMainBodyDamageByVehicleIDDataSet = new FindVehicleMainBodyDamageByVehicleIDDataSet();

        public ReportBodyDamage()
        {
            InitializeComponent();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("file://bjc/shares/Documents/WAREHOUSE/WhseTrac%20Manual/index.html");
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bluejay.on.spiceworks.com/portal/tickets");
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the data grid
            //setting local variables
            int intRecordsReturned;

            TheFindVehicleMainBodyDamageByVehicleIDDataSet = TheVehicleBodyDamageClass.FindVehicleMainBodyDamageByVehicleID(MainWindow.gintVehicleID);

            intRecordsReturned = TheFindVehicleMainBodyDamageByVehicleIDDataSet.FindVehicleMainBodyDamageByVehicleID.Rows.Count;

            if (intRecordsReturned == 0)
            {
                TheMessagesClass.InformationMessage("This Will Be The First Reported Body Damage");
            }

            dgrBodyDamage.ItemsSource = TheFindVehicleMainBodyDamageByVehicleIDDataSet.FindVehicleMainBodyDamageByVehicleID;
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //this will save the transaction
            string strDamageReported;
            bool blnFatalError = false;

            try
            {
                //data validation
                strDamageReported = txtEnterBodyDamage.Text;
                if (strDamageReported == "")
                {
                    TheMessagesClass.ErrorMessage("There Was No Body Damage Entered");
                    return;
                }

                blnFatalError = TheVehicleBodyDamageClass.InsertNewVehicleBodyDamage(MainWindow.gintVehicleID, strDamageReported, DateTime.Now, false);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was A Problem, Contact IT");
                    return;
                }

                TheMessagesClass.InformationMessage("Damage Entered");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Body Damage // Save Menu Item " + Ex.Message);

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
