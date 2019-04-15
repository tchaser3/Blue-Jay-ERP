/* Title:           Add Job Type
 * Date:            2-4-19
 * Author:          Terry Holmes
 * 
 * Description:     This is where the job types are added */

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
using JobTypeDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddJobType.xaml
    /// </summary>
    public partial class AddJobType : Window
    {
        //setting up the classes
        WPFMessagesClass TheMesssagesClass = new WPFMessagesClass();
        JobTypeClass TheJobTypeClass = new JobTypeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        public AddJobType()
        {
            InitializeComponent();
        }
        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.LaunchHelpSite();
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.LaunchEmail();
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.MyOriginatingTask();
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.MyOpenTasks();
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.AddTask();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strJobType;
            bool blnFatalError = false;

            try
            {
                strJobType = txtEnterJobType.Text;
                if(strJobType == "")
                {
                    TheMesssagesClass.ErrorMessage("Job Type Was Not Entered");
                    return;
                }

                blnFatalError = TheJobTypeClass.InsertJobType(strJobType);

                if (blnFatalError == true)
                    throw new Exception();

                TheMesssagesClass.InformationMessage("Job Type Has Been Added");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Job Type // Process Menu Item " + Ex.Message);

                TheMesssagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterJobType.Text = "";
        }
    }
}
