/* Title:           Add Project
 * Date:            2-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to add projects */

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
    /// Interaction logic for AddProject.xaml
    /// </summary>
    public partial class AddProject : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the classes
        FindProjectByProjectNameDataSet TheFindProjectByProjectNameDataSet = new FindProjectByProjectNameDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();

        public AddProject()
        {
            InitializeComponent();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            string strAssignedProjectID;
            string strProjectName;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            bool blnDIDExists;
            bool blnNameExists;

            try
            {
                //beginning data validation
                strAssignedProjectID = txtAssignedProjectID.Text;
                strProjectName = txtProjectName.Text;
                blnDIDExists = false;
                blnNameExists = false;

                if (strAssignedProjectID == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Assigned Project ID Was Not Entered\n";
                }
                if (strProjectName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Name Was Not Entered\n";
                }
                else
                {
                    TheFindProjectByProjectNameDataSet = TheProjectClass.FindProjectByProjectName(strProjectName);

                    intRecordsReturned = TheFindProjectByProjectNameDataSet.FindProjectByProjectName.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnNameExists = true;
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strAssignedProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    blnDIDExists = true;
                }


                if ((blnDIDExists == true) && (blnNameExists == true))
                {
                    TheMessagesClass.ErrorMessage("This Project Currently Exists");
                    return;
                }

                blnFatalError = TheProjectClass.InsertProject(strAssignedProjectID, strProjectName);
                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Project Has Been Saved");
                txtAssignedProjectID.Text = "";
                txtProjectName.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtAssignedProjectID.Text = "";
            txtAssignedProjectID.Text = "";
            MainWindow.FindExistingProjects.Visibility = Visibility.Hidden;
            this.Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtAssignedProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strProjectID;
            int intLength;

            try
            {
                strProjectID = txtAssignedProjectID.Text;

                intLength = strProjectID.Length;

                if (intLength > 4)
                {
                    MainWindow.TheFindProjectsByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Add Project Add New Project Assigned Project ID Text Change Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void txtProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intLength;
            string strProjectName;

            try
            {
                strProjectName = txtProjectName.Text;

                intLength = strProjectName.Length;

                if (intLength > 5)
                {
                    MainWindow.TheFindProjectByProjectNameDataSet = TheProjectClass.FindProjectByProjectName(strProjectName);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project // Project Name Text Change Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
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
