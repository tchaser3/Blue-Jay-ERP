/* Title:           Existing Projects Window
 * Date:            2-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This window will show existing projects */

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
using System.Windows.Threading;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ExistingProjects.xaml
    /// </summary>
    public partial class ExistingProjects : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        DispatcherTimer MyTimer = new DispatcherTimer();

        public ExistingProjects()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void BeginTheProcess(object sender, EventArgs e)
        {
            //setting local variables
            int intRecordsReturned;

            intRecordsReturned = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

            if (intRecordsReturned > 0)
            {
                dgrProjects.ItemsSource = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID;
            }
            else
            {
                intRecordsReturned = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    dgrProjects.ItemsSource = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName;
                }
                else
                {
                    intRecordsReturned = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        dgrProjects.ItemsSource = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID;
                    }
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MyTimer.Tick += new EventHandler(BeginTheProcess);
                MyTimer.Interval = new TimeSpan(0, 0, 1);
                MyTimer.Start();


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Existing Projects // Grid Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
