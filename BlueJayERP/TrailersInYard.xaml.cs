/* Title:           Trailers In yard
 * Date:            9-24-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to enter Trailers in The Yard */

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
using TrailersInYardDLL;
using TrailersDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for TrailersInYard.xaml
    /// </summary>
    public partial class TrailersInYard : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailersInYardClass TheTrailersInYardClass = new TrailersInYardClass();
        TrailersClass TheTrailersClass = new TrailersClass();

        //setting up the data
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();

        public TrailersInYard()
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
            TheMessagesClass.MyOpenTasks();
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
            txtEnterTrailerNumber.Text = "";
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up variables
            string strTrailerNumber;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                strTrailerNumber = txtEnterTrailerNumber.Text;
                if(strTrailerNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Trailer Number Not Found");
                    return;
                }

                TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Trailer Was Not Found");
                    return;
                }

                MainWindow.gintTrailerID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerID;

                blnFatalError = TheTrailersInYardClass.InsertTrailerInYard(MainWindow.gintTrailerID);

                if (blnFatalError == true)
                    throw new Exception();

                txtEnterTrailerNumber.Text = "";
                txtEnterTrailerNumber.Focus();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Trailers In Yard // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
