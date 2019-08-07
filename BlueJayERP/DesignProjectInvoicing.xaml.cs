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
using NewEmployeeDLL;
using TechPayDLL;

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
        DesignProjectsClass TheProjectsClass = new DesignProjectsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        TechPayClass TheTechPayClass = new TechPayClass();

        //setting up the data
        FindDesignProjectsReadyForInvoicingDataSet TheFindDesignProjectsReadyForInvoicingDataSet = new FindDesignProjectsReadyForInvoicingDataSet();
        FindPermitsByProjectIDDataSet TheFindPermitByProjectIDDataSet = new FindPermitsByProjectIDDataSet();
        DesignProjectsNeedingInvoiceDataSet TheDesignProjectsNeedingInvoiceDataSet = new DesignProjectsNeedingInvoiceDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsByAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindDesignProjectsForInvoicingByOfficeBillingCodeDataSet ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet = new FindDesignProjectsForInvoicingByOfficeBillingCodeDataSet();
        FindSortedWOVBillingCodesDataSet TheFindSortedWOVBillingCodesDataSet = new FindSortedWOVBillingCodesDataSet();
        FindProjectTechPayItemsTotalsByProjectIDDataSet TheFindProjectTechPayItemsTotalsByProjectIDDataSet = new FindProjectTechPayItemsTotalsByProjectIDDataSet();
        FindDesignPermitCostsDataSet TheFindDesignPermitCostsDataSet = new FindDesignPermitCostsDataSet();

        int gintCounter;
        int gintNumberOfRecords;
        int gintReportCounter;
        int gintReportNumberOfRecords;

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
            string strAssignedProjectID;

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
                        strAssignedProjectID = TheFindDesignProjectsReadyForInvoicingDataSet.FindDesignProjectsReadyForInvoicing[intCounter].AssignedProjectID;

                        TheFindDesignProjectsByAssignedProjectIDDataSet = TheDesignProjectsClass.FindDesignProjectsByAssignedProjectID(strAssignedProjectID);

                        intProjectID = TheFindDesignProjectsByAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].ProjectID;

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
            int intOfficeCounter;
            int intOfficeNumberOfRecords;
            int intBillingCounter;
            int intBillingNumberOfRecords;
            int intOfficeID;
            int intBillingID;
            int intProjectCounter;
            int intProjectNumberOfRecords;
            bool blnProjectSelected;
            int intProjectID;
            int intPayCounter;
            int intPayNumberOfRecords;
            bool blnItemFound;
            int intReportCounter;
            int intPermitCounter;
            int intPermitNumberOfRecords;
            int intRecordsReturned;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
                TheFindSortedWOVBillingCodesDataSet = TheWOVInvoicingClass.FindSortedWOVBillingCodes();

                intOfficeNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;
                intBillingNumberOfRecords = TheFindSortedWOVBillingCodesDataSet.FindSortedWOVBillingCodes.Rows.Count - 1;

                //Office Loop
                for(intOfficeCounter = 0; intOfficeCounter <= intOfficeNumberOfRecords; intOfficeCounter++)
                {
                    //loading office variables
                    intOfficeID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intOfficeCounter].EmployeeID;

                    //second loop
                    for(intBillingCounter = 0; intBillingCounter <= intBillingNumberOfRecords; intBillingCounter++)
                    {
                        intBillingID = TheFindSortedWOVBillingCodesDataSet.FindSortedWOVBillingCodes[intBillingCounter].BillingID;
                        MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing.Rows.Clear();

                        ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet = TheWOVInvoicingClass.FindDesignProjectsForInvoicingByOfficeBillingCode(intOfficeID, intBillingID);
                        gintReportCounter = 0;
                        gintReportNumberOfRecords = 0;

                        intProjectNumberOfRecords = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode.Rows.Count - 1;

                        if (intProjectNumberOfRecords > -1)
                        {
                            for (intProjectCounter = 0; intProjectCounter <= intProjectNumberOfRecords; intProjectCounter++)
                            {
                                intProjectID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectID;
                               
                                blnProjectSelected = ProjectSelected(intProjectID);
                                
                                if(blnProjectSelected == true)
                                {
                                    TheFindProjectTechPayItemsTotalsByProjectIDDataSet = TheTechPayClass.FindProjectTechPayItemsTotalsByProjectID(intProjectID);
                                    
                                    intPayNumberOfRecords = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID.Rows.Count - 1;

                                    if(intPayNumberOfRecords > -1)
                                    {
                                        for (intPayCounter = 0; intPayCounter <= intPayNumberOfRecords; intPayCounter++)
                                        {
                                            blnItemFound = false;

                                            if(gintReportCounter > 0)
                                            {
                                                for (intReportCounter = 0; intReportCounter <= gintReportNumberOfRecords; intReportCounter++)
                                                {
                                                    if (intProjectID == MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].ProjectID)
                                                    {
                                                        if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV1")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].WOV1 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }
                                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV2")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].WOV2 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }
                                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV3")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].WOV3 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }
                                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MCO5")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].MCO5 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }
                                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP1")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].P1 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }
                                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP2")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].P2 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }
                                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "UG")
                                                        {
                                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intReportCounter].UG = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                        }

                                                        blnItemFound = true;

                                                    }
                                                }
                                            }

                                            if (blnItemFound == false)
                                            {
                                                DesignProjectInvoicingReportDataSet.designprojectinvoicingRow NewProjectRow = MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing.NewdesignprojectinvoicingRow();

                                                NewProjectRow.BusinessAddress = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectAddress;
                                                NewProjectRow.BusinessName = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectName;
                                                NewProjectRow.DateOfWork = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].DateReceived;
                                                NewProjectRow.DockID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].AssignedProjectID;
                                                NewProjectRow.PermitCost = 0;
                                                NewProjectRow.PermitPrice = 0;
                                                NewProjectRow.PermitType = "";
                                                NewProjectRow.ProjectID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectID;
                                                NewProjectRow.TotalAmount = 0;


                                                if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV1")
                                                {
                                                    NewProjectRow.WOV1 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                    NewProjectRow.WOV2 = 0;
                                                    NewProjectRow.WOV3 = 0;
                                                    NewProjectRow.MCO5 = 0;
                                                    NewProjectRow.P1 = 0;
                                                    NewProjectRow.P2 = 0;
                                                    NewProjectRow.UG = 0;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV2")
                                                {
                                                    NewProjectRow.WOV1 = 0;
                                                    NewProjectRow.WOV2 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                    NewProjectRow.WOV3 = 0;
                                                    NewProjectRow.MCO5 = 0;
                                                    NewProjectRow.P1 = 0;
                                                    NewProjectRow.P2 = 0;
                                                    NewProjectRow.UG = 0;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV3")
                                                {
                                                    NewProjectRow.WOV1 = 0;
                                                    NewProjectRow.WOV2 = 0;
                                                    NewProjectRow.WOV3 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                    NewProjectRow.MCO5 = 0;
                                                    NewProjectRow.P1 = 0;
                                                    NewProjectRow.P2 = 0;
                                                    NewProjectRow.UG = 0;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MCO5")
                                                {
                                                    NewProjectRow.WOV1 = 0;
                                                    NewProjectRow.WOV2 = 0;
                                                    NewProjectRow.WOV3 = 0;
                                                    NewProjectRow.MCO5 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                    NewProjectRow.P1 = 0;
                                                    NewProjectRow.P2 = 0;
                                                    NewProjectRow.UG = 0;

                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP1")
                                                {
                                                    NewProjectRow.WOV1 = 0;
                                                    NewProjectRow.WOV2 = 0;
                                                    NewProjectRow.WOV3 = 0;
                                                    NewProjectRow.MCO5 = 0;
                                                    NewProjectRow.P1 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                    NewProjectRow.P2 = 0;
                                                    NewProjectRow.UG = 0;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP2")
                                                {
                                                    NewProjectRow.WOV1 = 0;
                                                    NewProjectRow.WOV2 = 0;
                                                    NewProjectRow.WOV3 = 0;
                                                    NewProjectRow.MCO5 = 0;
                                                    NewProjectRow.P1 = 0;
                                                    NewProjectRow.P2 = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                    NewProjectRow.UG = 0;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "UG")
                                                {
                                                    NewProjectRow.WOV1 = 0;
                                                    NewProjectRow.WOV2 = 0;
                                                    NewProjectRow.WOV3 = 0;
                                                    NewProjectRow.MCO5 = 0;
                                                    NewProjectRow.P1 = 0;
                                                    NewProjectRow.P2 = 0;
                                                    NewProjectRow.UG = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                                }

                                                MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing.Rows.Add(NewProjectRow);
                                                gintReportNumberOfRecords = gintReportCounter;
                                                gintReportCounter++;
                                            }

                                        }
                                    }

                                    //counter to find permit costs
                                    intPermitNumberOfRecords = MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing.Rows.Count - 1;

                                    for(intPermitCounter = 0; intPermitCounter <= intPermitNumberOfRecords; intPermitCounter++)
                                    {
                                        intProjectID = MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intPermitCounter].ProjectID;

                                        TheFindDesignPermitCostsDataSet = TheDesignPermitsClass.FindDesignPermitCosts(intProjectID);

                                        intRecordsReturned = TheFindDesignPermitCostsDataSet.FindDesignPermitCosts.Rows.Count;

                                        if(intRecordsReturned > 0)
                                        {
                                            MainWindow.TheDesignProjectInvoicingReportDataSet.designprojectinvoicing[intPermitCounter].PermitCost = TheFindDesignPermitCostsDataSet.FindDesignPermitCosts[0].TotalCost;
                                        }
                                    }

                                    ProcessDesignInvoice ProcessDesignInvoice = new ProcessDesignInvoice();
                                    ProcessDesignInvoice.ShowDialog();
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Peroject Invoicing // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private bool ProjectSelected(int intProjectID)
        {
            bool blnProjectSelected = false;
            int intCounter;
            int intNumberOfRecords;

            intNumberOfRecords = TheDesignProjectsNeedingInvoiceDataSet.designprojects.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if (intProjectID == TheDesignProjectsNeedingInvoiceDataSet.designprojects[intCounter].ProjectID)
                {
                    blnProjectSelected = TheDesignProjectsNeedingInvoiceDataSet.designprojects[intCounter].ProcessProject;
                }
            }

            return blnProjectSelected;
        }
        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            bool blnProcessProject;
            
            intSelectedIndex = dgrResults.SelectedIndex;

            if(intSelectedIndex > -1)
            {
                blnProcessProject = !TheDesignProjectsNeedingInvoiceDataSet.designprojects[intSelectedIndex].ProcessProject;
                MainWindow.gstrAssignedProjectID = TheDesignProjectsNeedingInvoiceDataSet.designprojects[intSelectedIndex].AssignedProjectID;

                TheDesignProjectsNeedingInvoiceDataSet.designprojects[intSelectedIndex].ProcessProject = blnProcessProject;

                dgrResults.ItemsSource = TheDesignProjectsNeedingInvoiceDataSet.designprojects;

                TheFindDesignProjectsByAssignedProjectIDDataSet = TheDesignProjectsClass.FindDesignProjectsByAssignedProjectID(MainWindow.gstrAssignedProjectID);

                if(TheFindDesignProjectsByAssignedProjectIDDataSet.FindDesignProjectsByAssignedProjectID[0].IsBillingIDNull() == true)
                {
                    AddBillingCodeToProject AddBillingCodeToProject = new AddBillingCodeToProject();
                    AddBillingCodeToProject.ShowDialog();
                }
            }
        }
    }
}
