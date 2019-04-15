/* Title:           Trailer Body Damage
 * Date:            11-5-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used for Trailer Body Damage */


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
using TrailerBodyDamageDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for TrailerBodyDamage.xaml
    /// </summary>
    public partial class TrailerBodyDamage : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        TrailerBodyDamageClass TheTrailerBodyDamageClass = new TrailerBodyDamageClass();
        EventLogClass TheEventLogclass = new EventLogClass();

        FindTrailerBodyDamageByTrailerIDDataSet TheFindTrailerBodyDamageByTrailerIDDataSet = new FindTrailerBodyDamageByTrailerIDDataSet();

        public TrailerBodyDamage()
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
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;

            TheFindTrailerBodyDamageByTrailerIDDataSet = TheTrailerBodyDamageClass.FindTrailerBodyDamageByTrailerID(MainWindow.gintTrailerID);

            intRecordsReturned = TheFindTrailerBodyDamageByTrailerIDDataSet.FindTrailerBodyDamageByTrailerID.Rows.Count;

            cboDamageReported.Items.Add("Select Yes or No");
            cboDamageReported.Items.Add("Yes");
            cboDamageReported.Items.Add("No");
            mitProcess.IsEnabled = false;

            if (intRecordsReturned == 0)
            {
                cboDamageReported.SelectedIndex = 2;
                cboDamageReported.IsEnabled = false;
            }
            else if(intRecordsReturned > 0)
            {
                cboDamageReported.IsEnabled = true;
                cboDamageReported.SelectedIndex = 0;
            }

            dgrResults.ItemsSource = TheFindTrailerBodyDamageByTrailerIDDataSet.FindTrailerBodyDamageByTrailerID;
           
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strBodyProblem;
            bool blnFatalError = false;
            int intLength;

            try
            {
                strBodyProblem = txtReportedDamage.Text;
                intLength = strBodyProblem.Length;

                if(intLength < 10)
                {
                    TheMessagesClass.ErrorMessage("Reported Dmage is to short");
                    return;
                }

                blnFatalError = TheTrailerBodyDamageClass.InsertTrailerBodyDamage(MainWindow.gintTrailerID, strBodyProblem);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Reported Damage ");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogclass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Trailer Body Damage // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboDamageReported.SelectedIndex == 1)
            {
                TheMessagesClass.InformationMessage("Thank You, The Window Will Close");
                Close();
            }
            else if(cboDamageReported.SelectedIndex == 2)
            {
                mitProcess.IsEnabled = true;
            }
            else
            {
                mitProcess.IsEnabled = false;
            }
        }
    }
}
