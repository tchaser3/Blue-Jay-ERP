/* Title:           Find Projects
 * Date:            6-6-18
 * Author:          Terry Holmes
 * 
 * Desciption:      This is used to find the projects */

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
using ProjectsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindProjects.xaml
    /// </summary>
    public partial class FindProjects : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectClass TheProjectClass = new ProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectByProjectNameDataSet TheFindProjectByProjectNameDataSet = new FindProjectByProjectNameDataSet();
        
        public FindProjects()
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
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up locak variables
            string strProject;
            int intRecordsReturned;

            strProject = txtEnterProject.Text;

            TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProject);

            intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

            if(intRecordsReturned > 0)
            {
                dgrResults.ItemsSource = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID;
            }
            else
            {
                TheFindProjectByProjectNameDataSet = TheProjectClass.FindProjectByProjectName(strProject);

                dgrResults.ItemsSource = TheFindProjectByProjectNameDataSet.FindProjectByProjectName;
            }
        }

        private void txtEnterProject_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strProjectName;
            int intLength;

            strProjectName = txtEnterProject.Text;
            intLength = strProjectName.Length;

            if(intLength > 3)
            {
                TheFindProjectByProjectNameDataSet = TheProjectClass.FindProjectByProjectName(strProjectName);

                dgrResults.ItemsSource = TheFindProjectByProjectNameDataSet.FindProjectByProjectName;
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
