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
using DesignProjectUpdateDLL;

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
        DesignProjectUpdateClass TheDesignProjectUpdateClass = new DesignProjectUpdateClass();

        //setting up the data
        FindDesignProjectsReadyForInvoicingDataSet TheFindDesignProjectsReadyForInvoicingDataSet = new FindDesignProjectsReadyForInvoicingDataSet();
        FindPermitsByProjectIDDataSet TheFindPermitByProjectIDDataSet = new FindPermitsByProjectIDDataSet();
        DesignProjectsNeedingInvoiceDataSet TheDesignProjectsNeedingInvoiceDataSet = new DesignProjectsNeedingInvoiceDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsByAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindDesignProjectsForInvoicingByOfficeBillingCodeDataSet ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet = new FindDesignProjectsForInvoicingByOfficeBillingCodeDataSet();
        FindSortedWOVBillingCodesDataSet TheFindSortedWOVBillingCodesDataSet = new FindSortedWOVBillingCodesDataSet();
        FindProjectTechPayItemsTotalsByProjectIDDataSet TheFindProjectTechPayItemsTotalsByProjectIDDataSet = new FindProjectTechPayItemsTotalsByProjectIDDataSet();
        FindDesignPermitCostsDataSet TheFindDesignPermitCostsDataSet = new FindDesignPermitCostsDataSet();
        FindWOVTaskByOfficeIDandDescriptionDataSet TheFindWOVTaskByOfficeIDAndDescriptionDataSet = new FindWOVTaskByOfficeIDandDescriptionDataSet();
        FindSortedWOVTasksByOfficeIDDataSet TheFindSortedWOVTasksByOfficeIDDataSet = new FindSortedWOVTasksByOfficeIDDataSet();
        FindDesignProjectUpdatesByProjectIDDataSet TheFindDesignProjectUpdatesByProjectIDDataSet = new FindDesignProjectUpdatesByProjectIDDataSet();

        int gintCounter;
        int gintNumberOfRecords;
        int gintReportCounter;
        int gintReportNumberOfRecords;
        bool gblnDisplayInvoice;

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
        private bool ProjectSelected(int intProjectID)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnIsSelected = false;

            intNumberOfRecords = TheDesignProjectsNeedingInvoiceDataSet.designprojects.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(intProjectID == TheDesignProjectsNeedingInvoiceDataSet.designprojects[intCounter].ProjectID)
                {
                    blnIsSelected = TheDesignProjectsNeedingInvoiceDataSet.designprojects[intCounter].ProcessProject;
                }
            }

            return blnIsSelected;
        }
        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intOfficeCounter;
            int intOfficeNumberOfRecords;
            int intBillingCounter;
            int intBillingNumberOfRecords;
            string strWarehouse;
            bool blnFatalError = false;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
                TheFindSortedWOVBillingCodesDataSet = TheWOVInvoicingClass.FindSortedWOVBillingCodes();

                intOfficeNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;
                intBillingNumberOfRecords = TheFindSortedWOVBillingCodesDataSet.FindSortedWOVBillingCodes.Rows.Count - 1;

                //Office Loop
                for (intOfficeCounter = 0; intOfficeCounter <= intOfficeNumberOfRecords; intOfficeCounter++)
                {
                    //loading office variables
                    MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intOfficeCounter].EmployeeID;
                    strWarehouse = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intOfficeCounter].FirstName;

                    for(intBillingCounter = 0; intBillingCounter <= intBillingNumberOfRecords; intBillingCounter++)
                    {
                        MainWindow.gintBillingID = TheFindSortedWOVBillingCodesDataSet.FindSortedWOVBillingCodes[intBillingCounter].BillingID;
                        gblnDisplayInvoice = false;

                        if(strWarehouse == "CLEVELAND")
                        {
                            blnFatalError = LoadClevelandInvoice(MainWindow.gintWarehouseID, MainWindow.gintBillingID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        else if(strWarehouse == "MILWAUKEE")
                        {
                            MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Clear();

                            blnFatalError = LoadWisconsinInvoice(MainWindow.gintWarehouseID, MainWindow.gintBillingID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }

                        if(gblnDisplayInvoice == true)
                        {
                            //adding the permit costs
                            blnFatalError = AddPermitValues(strWarehouse);

                            if (blnFatalError == true)
                                throw new Exception();

                            ProcessDesignInvoice ProcessDesignInvoice = new ProcessDesignInvoice();
                            ProcessDesignInvoice.ShowDialog();
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
        private bool AddPermitValues(string strWarehouse)
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            decimal decTotalCost;

            try
            {
                TheFindDesignPermitCostsDataSet = TheDesignPermitsClass.FindDesignPermitCosts(MainWindow.gintProjectID);

                intRecordsReturned = TheFindDesignPermitCostsDataSet.FindDesignPermitCosts.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    decTotalCost = TheFindDesignPermitCostsDataSet.FindDesignPermitCosts[0].TotalCost;

                    if(strWarehouse == "CLEVELAND")
                    {
                        intNumberOfRecords = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Count - 1;

                        if(intNumberOfRecords > -1)
                        {
                            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                if(MainWindow.gstrAssignedProjectID == MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].DockID)
                                {
                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].NS004 = decTotalCost;
                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].TotalProjectCharge += decTotalCost;
                                }
                            }
                        }
                    }
                    if (strWarehouse == "MILWAUKEE")
                    {
                        intNumberOfRecords = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Count - 1;

                        if (intNumberOfRecords > -1)
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                if (MainWindow.gstrAssignedProjectID == MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].DockID)
                                {
                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].ProjectTotal = decTotalCost;
                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].ProjectTotal += decTotalCost;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Project Invoicing // Add Permit Value " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool LoadClevelandInvoice(int intOfficeID, int intBillingID)
        {
            bool blnFatalError = false;
            int intProjectCounter;
            int intProjectNumberOfRecords;
            int intPayCounter;
            int intPayNumberOfRecords;
            bool blnIsSelected;
            int intReportCounter;
            bool blnItemFound;
            string strProjectName;
            string strProjectAddress;
            int intTotalQuantity;
            decimal decPrice;
            decimal decTotalPrice;
            string strWOVTaskDescription;
            DateTime datClosingDate;

            try
            {
                ResetClevelandDesignInvoice();
                ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet = TheWOVInvoicingClass.FindDesignProjectsForInvoicingByOfficeBillingCode(intOfficeID, intBillingID);

                intProjectNumberOfRecords = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode.Rows.Count - 1;

                if(intProjectNumberOfRecords > -1)
                {
                    for(intProjectCounter = 0; intProjectCounter <= intProjectNumberOfRecords; intProjectCounter++)
                    {
                        

                        MainWindow.gintProjectID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectID;
                        MainWindow.gstrAssignedProjectID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].AssignedProjectID;
                        strProjectAddress = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectAddress;
                        strProjectName = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectName;

                        blnIsSelected = ProjectSelected(MainWindow.gintProjectID);

                        if(blnIsSelected == true)
                        {
                            gblnDisplayInvoice = true;

                            TheFindProjectTechPayItemsTotalsByProjectIDDataSet = TheTechPayClass.FindProjectTechPayItemsTotalsByProjectID(MainWindow.gintProjectID);

                            intPayNumberOfRecords = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID.Rows.Count - 1;

                            if(intPayNumberOfRecords > -1)
                            {
                                for(intPayCounter = 0; intPayCounter <= intPayNumberOfRecords; intPayCounter++)
                                {
                                    blnItemFound = false;
                                    intTotalQuantity = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                    strWOVTaskDescription = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode;

                                    TheFindWOVTaskByOfficeIDAndDescriptionDataSet = TheWOVInvoicingClass.FindWOVTaskByOfficeIDAndDescription(MainWindow.gintWarehouseID, strWOVTaskDescription);

                                    decPrice = TheFindWOVTaskByOfficeIDAndDescriptionDataSet.FindWOVTaskByOfficeIDAndDescription[0].Price;

                                    decTotalPrice = decPrice * Convert.ToDecimal(intTotalQuantity);
                                    
                                    if(gintReportCounter > 0)
                                    {
                                        for(intReportCounter = 1; intReportCounter <= gintReportNumberOfRecords; intReportCounter++)
                                        {
                                            if (MainWindow.gstrAssignedProjectID == MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].DockID)
                                            {
                                                blnItemFound = true;

                                                if(TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV1")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].WOV1 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV2")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].WOV2 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV3")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].WOV3 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP1")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].PP1 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP2")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].PP2 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "FE01")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].FE01 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "UG")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].UG += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "HA")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].HA += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MC05")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].MC05 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MC06")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].MC06 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "NS004")
                                                {
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].NS004 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intReportCounter].TotalProjectCharge += decTotalPrice;
                                                }
                                            }
                                        }
                                    }

                                    if(blnItemFound == false)
                                    {
                                        ClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoiceRow NewProjectRow = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.NewclevelanddesigninvoiceRow();

                                        NewProjectRow.BusinessAddress = strProjectAddress;
                                        NewProjectRow.BusinessName = strProjectName;
                                        NewProjectRow.DockID = MainWindow.gstrAssignedProjectID;

                                        //getting the last update date
                                        datClosingDate = FindClosingDate(MainWindow.gintProjectID);
                                        NewProjectRow.Date = datClosingDate;
                                        NewProjectRow.TotalProjectCharge = decTotalPrice;
                                        NewProjectRow.NS004 = 0;

                                        if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV1")
                                        {
                                            NewProjectRow.WOV1 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV2")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV3")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP1")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP2")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "FE01")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }                                        
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "UG")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "HA")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MC05")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.MC06 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MC06")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.WOV3 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.FE01 = 0;
                                            NewProjectRow.UG = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                            NewProjectRow.MC06 = Convert.ToDecimal(intTotalQuantity); ;
                                        }
                                        

                                        MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Add(NewProjectRow);

                                        gintReportNumberOfRecords = gintReportCounter + 1;
                                        gintReportCounter++;

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Project Invoicing // Load Cleveland Invoice " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool LoadWisconsinInvoice(int intOfficeID, int intBillingID)
        {
            bool blnFatalError = false;
            int intProjectCounter;
            int intProjectNumberOfRecords;
            int intPayCounter;
            int intPayNumberOfRecords;
            bool blnIsSelected;
            int intReportCounter;
            bool blnItemFound;
            string strProjectName;
            string strProjectAddress;
            int intTotalQuantity;
            decimal decPrice;
            decimal decTotalPrice;
            string strWOVTaskDescription;
            DateTime datClosingDate;

            try
            {
                ResetMilwaukeeDesignInvoice();
                ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet = TheWOVInvoicingClass.FindDesignProjectsForInvoicingByOfficeBillingCode(intOfficeID, intBillingID);

                intProjectNumberOfRecords = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode.Rows.Count - 1;

                if (intProjectNumberOfRecords > -1)
                {
                    for (intProjectCounter = 0; intProjectCounter <= intProjectNumberOfRecords; intProjectCounter++)
                    {


                        MainWindow.gintProjectID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectID;
                        MainWindow.gstrAssignedProjectID = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].AssignedProjectID;
                        strProjectAddress = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectAddress;
                        strProjectName = ThefindDesignProjectsForInvoicingByOfficeBillingCodeDataSet.FindDesignProjectsForInvoicingByOfficeBillingCode[intProjectCounter].ProjectName;

                        blnIsSelected = ProjectSelected(MainWindow.gintProjectID);

                        if (blnIsSelected == true)
                        {
                            gblnDisplayInvoice = true;

                            TheFindProjectTechPayItemsTotalsByProjectIDDataSet = TheTechPayClass.FindProjectTechPayItemsTotalsByProjectID(MainWindow.gintProjectID);

                            intPayNumberOfRecords = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID.Rows.Count - 1;

                            if (intPayNumberOfRecords > -1)
                            {
                                for (intPayCounter = 0; intPayCounter <= intPayNumberOfRecords; intPayCounter++)
                                {
                                    blnItemFound = false;
                                    intTotalQuantity = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TotalQuantity;
                                    strWOVTaskDescription = TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode;

                                    TheFindWOVTaskByOfficeIDAndDescriptionDataSet = TheWOVInvoicingClass.FindWOVTaskByOfficeIDAndDescription(MainWindow.gintWarehouseID, strWOVTaskDescription);

                                    decPrice = TheFindWOVTaskByOfficeIDAndDescriptionDataSet.FindWOVTaskByOfficeIDAndDescription[0].Price;

                                    decTotalPrice = decPrice * Convert.ToDecimal(intTotalQuantity);

                                    if (gintReportCounter > 0)
                                    {
                                        for (intReportCounter = 1; intReportCounter <= gintReportNumberOfRecords; intReportCounter++)
                                        {
                                            if (MainWindow.gstrAssignedProjectID == MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].DockID)
                                            {
                                                blnItemFound = true;

                                                if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV1")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].WOV1 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV2")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].WOV2 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP1")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].PP1 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP2")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].PP2 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "FE01")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].UF += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "HA")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].HA += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                                else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MC05")
                                                {
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].MC05 += Convert.ToDecimal(intTotalQuantity);
                                                    MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intReportCounter].ProjectTotal += decTotalPrice;
                                                }
                                            }
                                        }
                                    }

                                    if (blnItemFound == false)
                                    {
                                        WisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoiceRow NewProjectRow = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.NewwisconsindesigninvoiceRow();

                                        NewProjectRow.BusinessAddress = strProjectAddress;
                                        NewProjectRow.BusinessName = strProjectName;
                                        NewProjectRow.DockID = MainWindow.gstrAssignedProjectID;

                                        //getting the last update date
                                        datClosingDate = FindClosingDate(MainWindow.gintProjectID);
                                        NewProjectRow.Date = datClosingDate;
                                        NewProjectRow.ProjectTotal = decTotalPrice;
                                        NewProjectRow.PermitType = "";
                                        NewProjectRow.PermitCost = 0;

                                        if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV1")
                                        {
                                            NewProjectRow.WOV1 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.UF = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "WOV2")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.UF = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP1")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.PP1 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.UF = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "PP2")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.UF = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "UF")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.UF = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "HA")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.UF = 0;
                                            NewProjectRow.HA = Convert.ToDecimal(intTotalQuantity);
                                            NewProjectRow.MC05 = 0;
                                        }
                                        else if (TheFindProjectTechPayItemsTotalsByProjectIDDataSet.FindProjectTechPayItemsTotalsByProjectID[intPayCounter].TechPayCode == "MC05")
                                        {
                                            NewProjectRow.WOV1 = 0;
                                            NewProjectRow.WOV2 = 0;
                                            NewProjectRow.PP1 = 0;
                                            NewProjectRow.PP2 = 0;
                                            NewProjectRow.UF = 0;
                                            NewProjectRow.HA = 0;
                                            NewProjectRow.MC05 = Convert.ToDecimal(intTotalQuantity);
                                        }

                                        MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Add(NewProjectRow);

                                        gintReportNumberOfRecords = gintReportCounter + 1;
                                        gintReportCounter++;

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Project Invoicing // Load Wisconsin Invoice " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private DateTime FindClosingDate(int intProjectID)
        {
            //setting local variables
            DateTime datClosingDate = DateTime.Now;
            int intNumberOfReocrds;

            TheFindDesignProjectUpdatesByProjectIDDataSet = TheDesignProjectUpdateClass.FindDesignProjectUpdatesByProjectID(intProjectID);

            intNumberOfReocrds = TheFindDesignProjectUpdatesByProjectIDDataSet.FindDesignProjectUpdatesByProjectID.Rows.Count - 1;

            datClosingDate = TheFindDesignProjectUpdatesByProjectIDDataSet.FindDesignProjectUpdatesByProjectID[intNumberOfReocrds].TransactionDate;

            return datClosingDate;
        }
        private bool ResetClevelandDesignInvoice()
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                TheFindSortedWOVTasksByOfficeIDDataSet = TheWOVInvoicingClass.FindSortedWOVTasksByOfficeID(100000);
                gintReportNumberOfRecords = 0;
                gintReportCounter = 0;

                intNumberOfRecords = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID.Rows.Count - 1;

                MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Clear();

                ClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoiceRow NewBillingRow = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.NewclevelanddesigninvoiceRow();

                NewBillingRow.BusinessAddress = "PRICING";
                NewBillingRow.BusinessName = "";
                NewBillingRow.Date = DateTime.Now;
                NewBillingRow.DockID = "";
                NewBillingRow.TotalProjectCharge = 0;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "WOV1")
                    {
                        NewBillingRow.WOV1 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "WOV2")
                    {
                        NewBillingRow.WOV2 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "WOV3")
                    {
                        NewBillingRow.WOV3 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "PP1")
                    {
                        NewBillingRow.PP1 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "PP2")
                    {
                        NewBillingRow.PP2 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "FE01")
                    {
                        NewBillingRow.FE01 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "UG")
                    {
                        NewBillingRow.UG = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "HA")
                    {
                        NewBillingRow.HA = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "MC05")
                    {
                        NewBillingRow.MC05 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "MC06")
                    {
                        NewBillingRow.MC06 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "NS004")
                    {
                        NewBillingRow.NS004 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                }

                MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Add(NewBillingRow);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Project Invoicing // Reset Cleveland Design Invoice " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;

        }
        private bool ResetMilwaukeeDesignInvoice()
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                TheFindSortedWOVTasksByOfficeIDDataSet = TheWOVInvoicingClass.FindSortedWOVTasksByOfficeID(1343);
                gintReportNumberOfRecords = 0;
                gintReportCounter = 0;

                intNumberOfRecords = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID.Rows.Count - 1;

                MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Clear();

                WisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoiceRow NewBillingRow = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.NewwisconsindesigninvoiceRow();

                NewBillingRow.BusinessAddress = "PRICING";
                NewBillingRow.BusinessName = "";
                NewBillingRow.Date = DateTime.Now;
                NewBillingRow.DockID = "";
                NewBillingRow.ProjectTotal = 0;
                NewBillingRow.PermitType = "";

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "WOV1")
                    {
                        NewBillingRow.WOV1 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "WOV2")
                    {
                        NewBillingRow.WOV2 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "PP1")
                    {
                        NewBillingRow.PP1 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "PP2")
                    {
                        NewBillingRow.PP2 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "UF")
                    {
                        NewBillingRow.UF = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "HA")
                    {
                        NewBillingRow.HA = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                    else if (TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].WOVTaskDescription == "MC05")
                    {
                        NewBillingRow.MC05 = TheFindSortedWOVTasksByOfficeIDDataSet.FindSortedWOVTasksByOfficeID[intCounter].Price;
                    }
                }

                MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Add(NewBillingRow);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Project Invoicing // Reset Wisconsin Design Invoice " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;

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
