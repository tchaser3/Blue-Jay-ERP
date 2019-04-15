/* Title:           Edit Projects
 * Date:            2-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form for editting projects */

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
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditProject.xaml
    /// </summary>
    public partial class EditProject : Window
    {
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public EditProject()
        {
            InitializeComponent();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //this will set the controls
            //setting local variables
            string strAssignedProjectID;
            string strProjectName;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intProjectID;

            try
            {
                strAssignedProjectID = txtAssignedProjectID.Text;
                strProjectName = txtProjectName.Text;
                if (strAssignedProjectID == "")
                {
                    blnFatalError = true;
                        strErrorMessage += "Assigned Project Not Entered\n";
                }
                if (strProjectName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Project Name Not Entered\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intProjectID = Convert.ToInt32(txtProjectID.Text);

                blnFatalError = TheProjectClass.UpdateProjectProject(intProjectID, strAssignedProjectID, strProjectName);

                if (blnFatalError == true)
                    throw new Exception();

                if (blnFatalError == false)
                {
                    txtAssignedProjectID.Text = "";
                    txtEnterProject.Text = "";
                    txtProjectID.Text = "";
                    txtProjectName.Text = "";
                    txtTransactionDate.Text = "";
                    TheMessagesClass.InformationMessage("The Project Has Been Updated");
                    mitSave.IsEnabled = false;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
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

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intRecordsReturned = 0;
            string strProject;
            int intProjectID = 0;
            string strAssignedProjectID = "";
            string strProjectName = "";
            DateTime datTransactionDate = DateTime.Now;
            bool blnNotInteger;
            bool blnItemNotFound = true;

            try
            {
                //data validation
                strProject = txtEnterProject.Text;
                if (strProject == "")
                {
                    TheMessagesClass.ErrorMessage("Project Information was not Entered");
                    return;
                }

                //checking to see if this is an integer
                blnNotInteger = TheDataValidationClass.VerifyIntegerData(strProject);

                if (blnNotInteger == false)
                {
                    intProjectID = Convert.ToInt32(strProject);

                    //checking to see if project exists
                    MainWindow.TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(intProjectID);

                    intRecordsReturned = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID.Rows.Count;

                    if (intRecordsReturned != 0)
                    {
                        blnItemNotFound = false;
                        intProjectID = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectID;
                        strAssignedProjectID = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].AssignedProjectID;
                        strProjectName = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
                        datTransactionDate = MainWindow.TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].TransactionDate;
                    }
                }

                if (blnItemNotFound == true)
                {
                    MainWindow.TheFindProjectsByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProject);

                    intRecordsReturned = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if (intRecordsReturned != 0)
                    {
                        blnItemNotFound = false;
                        intProjectID = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                        strAssignedProjectID = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].AssignedProjectID;
                        strProjectName = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                        datTransactionDate = MainWindow.TheFindProjectsByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].TransactionDate;
                    }
                }

                if (blnItemNotFound == true)
                {
                    MainWindow.TheFindProjectByProjectNameDataSet = TheProjectClass.FindProjectByProjectName(strProject);

                    intRecordsReturned = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName.Rows.Count;

                    if (intRecordsReturned != 0)
                    {
                        blnItemNotFound = false;
                        intProjectID = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName[0].ProjectID;
                        strAssignedProjectID = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName[0].AssignedProjectID;
                        strProjectName = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName[0].ProjectName;
                        datTransactionDate = MainWindow.TheFindProjectByProjectNameDataSet.FindProjectByProjectName[0].TransactionDate;
                    }
                }

                if (blnItemNotFound == true)
                {
                    TheMessagesClass.InformationMessage("Project Not Found");
                    return;
                }

                txtAssignedProjectID.Text = strAssignedProjectID;
                txtProjectID.Text = Convert.ToString(intProjectID);
                txtProjectName.Text = strProjectName;
                txtTransactionDate.Text = Convert.ToString(datTransactionDate);
                mitSave.IsEnabled = true;        

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project // Find Button " + Ex.Message);

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
