/* Title:           Add Billing Code to Project
 * Date:            7-23-19
 * Author:          Terry Holmes
 * 
 * Description:     This is where we would add the codes */

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
using DesignProjectsDLL;
using WOVInvoicingDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddBillingCodeToProject.xaml
    /// </summary>
    public partial class AddBillingCodeToProject : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DesignProjectsClass TheDesignProjectsClass = new DesignProjectsClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();

        //setting up the data
        FindSortedWOVBillingCodesDataSet TheFindSortedWOVBillingCodes = new FindSortedWOVBillingCodesDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsByAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();

        int gintBillingID;

        public AddBillingCodeToProject()
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
            //setting up the variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //loading billing codes
                TheFindSortedWOVBillingCodes = TheWOVInvoicingClass.FindSortedWOVBillingCodes();

                intNumberOfRecords = TheFindSortedWOVBillingCodes.FindSortedWOVBillingCodes.Rows.Count - 1;
                cboSelectBillingCode.Items.Add("Select Billing Code");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectBillingCode.Items.Add(TheFindSortedWOVBillingCodes.FindSortedWOVBillingCodes[intCounter].BillingDescription);
                }

                cboSelectBillingCode.SelectedIndex = 0;

                TheFindDesignProjectsByAssignedProjectIDDataSet = TheDesignProjectsClass.FindDesignProjectsByAssignedProjectID(MainWindow.gstrAssignedProjectID);

                txtAddress.Text = TheFindDesignProjectsByAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectAddress;
                txtJobType.Text = TheFindDesignProjectsByAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].JobType;
                txtProjectID.Text = MainWindow.gstrAssignedProjectID;
                txtProjectName.Text = TheFindDesignProjectsByAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectName;
                MainWindow.gintProjectID = TheFindDesignProjectsByAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Adding Billing Codes to Project // Window Loaded Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectBillingCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectBillingCode.SelectedIndex - 1;

            if (intSelectedIndex > -1)
               gintBillingID = TheFindSortedWOVBillingCodes.FindSortedWOVBillingCodes[intSelectedIndex].BillingID;
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                if(cboSelectBillingCode.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("The Billing Ciode was not Selected");
                    return;
                }

                blnFatalError = TheDesignProjectsClass.UpdateDesignProjectBillingID(MainWindow.gintProjectID, gintBillingID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Billing Code Has Been Entered");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Billing Codes To Project // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
