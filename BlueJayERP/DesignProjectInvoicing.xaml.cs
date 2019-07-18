/* Title:       Design Project Invoicing
 * Date:        7-15-19
 * Author:      Terry Holmes
 * 
 * Description: This is used to do invoicing for design project */

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
using DataValidationDLL;
using WOVInvoicingDLL;
using DesignPermitsDLL;
using DesignProjectsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DesignProjectInvoicing.xaml
    /// </summary>
    public partial class DesignProjectInvoicing : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        DesignPermitsClass TheDesignPermitsClass = new DesignPermitsClass();
        DesignProjectsClass TheDesignProjectsClass = new DesignProjectsClass();

        //setting up the data
        FindDesignProjectsReadyForInvoicingDataSet TheFindDesignProjectsReadyForInvoicingDataSet = new FindDesignProjectsReadyForInvoicingDataSet();
        FindPermitsByProjectIDDataSet TheFindPermitByProjectIDDataSet = new FindPermitsByProjectIDDataSet();
        DesignProjectsNeedingInvoiceDataSet TheDesignProjectsNeedingInvoiceDataSet = new DesignProjectsNeedingInvoiceDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsByAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();

        int gintCounter;
        int gintNumberOfRecords;

        public DesignProjectInvoicing()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intSecondCounter;
            int intProjectID;
            bool blnItemFound;

            try
            {
                //filling data set
                TheFindDesignProjectsReadyForInvoicingDataSet = TheWOVInvoicingClass.FindDesignProjectsForInvoicing();

                //getting ready for the loop
                intNumberOfRecords = TheFindDesignProjectsReadyForInvoicingDataSet.FindDesignProjectsReadyForInvoicing.Rows.Count - 1;
                TheDesignProjectsNeedingInvoiceDataSet.designprojects.Rows.Clear();
                gintCounter = 0;
                gintNumberOfRecords = 0;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        intProjectID = TheFindDesignProjectsReadyForInvoicingDataSet.FindDesignProjectsReadyForInvoicing[intCounter].ProjectID;

                        if(gintCounter > 0)
                        {
                            for(intSecondCounter = 0; intSecondCounter <= gintNumberOfRecords; intSecondCounter++)
                            {
                                if(intProjectID == TheDesignProjectsNeedingInvoiceDataSet.designprojects[intSecondCounter].ProjectID)
                                {
                                    blnItemFound = true;
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            DesignProjectsNeedingInvoiceDataSet.designprojectsRow NewProjectRow = TheDesignProjectsNeedingInvoiceDataSet.designprojects.NewdesignprojectsRow();

                            NewProjectRow.AssignedOffice = TheFindDesignProjectsReadyForInvoicingDataSet.FindDesignProjectsReadyForInvoicing[intCounter].AssignedOffice;
                            NewProjectRow.AssignedProjectID = TheFindDesignProjectsReadyForInvoicingDataSet.FindDesignProjectsReadyForInvoicing[intCounter].AssignedProjectID;
                            NewProjectRow.ProjectID = intProjectID;
                            NewProjectRow.ProjectName = TheFindDesignProjectsReadyForInvoicingDataSet.FindDesignProjectsReadyForInvoicing[intCounter].ProjectName;
                            NewProjectRow.ProcessProject = false;

                            TheDesignProjectsNeedingInvoiceDataSet.designprojects.Rows.Add(NewProjectRow);
                            gintNumberOfRecords = gintCounter;
                            gintCounter++;
                        }
                    }
                }

                dgrResults.ItemsSource = TheDesignProjectsNeedingInvoiceDataSet.designprojects;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Project Invoicing // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnNeedsToBeProcessed;
            int intProjectID;
            bool blnPoleStick;
            string strAssignedProjectID;
            string strProjectName;
            string strAddress;

            try
            {
                intNumberOfRecords = TheDesignProjectsNeedingInvoiceDataSet.designprojects.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    blnNeedsToBeProcessed = TheDesignProjectsNeedingInvoiceDataSet.designprojects[intCounter].ProcessProject;

                    if(blnNeedsToBeProcessed == true)
                    {
                        strAssignedProjectID = TheDesignProjectsNeedingInvoiceDataSet.designprojects[intCounter].AssignedProjectID;

                        
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Peroject Invoicing // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            bool blnProcessProject;

            intSelectedIndex = dgrResults.SelectedIndex;

            if(intSelectedIndex > -1)
            {
                blnProcessProject = !TheDesignProjectsNeedingInvoiceDataSet.designprojects[intSelectedIndex].ProcessProject;

                TheDesignProjectsNeedingInvoiceDataSet.designprojects[intSelectedIndex].ProcessProject = blnProcessProject;

                dgrResults.ItemsSource = TheDesignProjectsNeedingInvoiceDataSet.designprojects;
            }
        }
    }
}
