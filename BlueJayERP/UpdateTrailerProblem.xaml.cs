/* Title:           Update Trailer Problem
 * Date:            9-30-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to update a trailer problem */

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
using TrailersDLL;
using TrailerProblemDLL;
using TrailerProblemUpdateDLL;
using TrailerProblemDocumentationDLL;
using NewEventLogDLL;
using VendorsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateTrailerProblem.xaml
    /// </summary>
    public partial class UpdateTrailerProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        TrailerProblemUpdateClass TheTrailerProblemUpdateClass = new TrailerProblemUpdateClass();
        TrailerProblemDocumentationClass TheTrailerProblemDocumentationClass = new TrailerProblemDocumentationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VendorsClass TheVendorsClass = new VendorsClass();

        //setting up the data sets
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();
        FindOpenTrailerProblemsByTrailerIDDataSet TheFindOpenTrailerProblemByTrailerIDDataSet = new FindOpenTrailerProblemsByTrailerIDDataSet();

        public UpdateTrailerProblem()
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
            TheMessagesClass.AddTask();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEmployeeName.Text = "";
            txtEnterTrailerNumber.Text = "";
            txtTrailerDescription.Text = "";
            txtTrailerID.Text = "";
            txtTrailerNotes.Text = "";
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strTrailerNumber;
            int intRecordsReturned;
            string strFullName;

            try
            {
                strTrailerNumber = txtEnterTrailerNumber.Text;
                if(strTrailerNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Trailer Number Was Not Entered");
                    return;
                }

                TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Trailer Not Found");
                    return;
                }

                MainWindow.gintTrailerID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerID;

                txtTrailerDescription.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerDescription;
                txtTrailerID.Text = Convert.ToString(MainWindow.gintTrailerID);
                txtTrailerNotes.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerNotes;

                strFullName = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].FirstName + " ";
                strFullName += TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].LastName;
                txtEmployeeName.Text = strFullName;

                TheFindOpenTrailerProblemByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

                dgrResults.ItemsSource = TheFindOpenTrailerProblemByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Trailer Problem // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
