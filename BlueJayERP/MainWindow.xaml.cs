/* Title:           Blue Jay ERP System
 * Date:            2-8-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the ERP Program */

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewEmployeeDLL;
using NewEventLogDLL;
using ProjectsDLL;
using EmployeeLaborRateDLL;
using WorkTaskDLL;
using System.Windows.Threading;
using VehicleExceptionEmailDLL;
using DateSearchDLL;
using VehicleProblemsDLL;
using DropBuryMDUDLL;
using VehicleBulkToolsDLL;
using AssignedTasksDLL;
using DepartmentDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the class file
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeclass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeLaborRateClass TheEmployeeLaborRateClass = new EmployeeLaborRateClass();
        VehicleExceptionEmailClass TheVehicleExceptionEmailClass = new VehicleExceptionEmailClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        AutomatedVehicleReports TheAutomatedVehicleReportsClass = new AutomatedVehicleReports();
        AssignedTaskClass TheAssignedTaskClass = new AssignedTaskClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();

        //setting up data
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        public static FindPartsWarehousesDataSet TheFindPartsWarehousesDataSet = new FindPartsWarehousesDataSet();
        public static FindSortedEmployeeGroupDataSet TheFindSortedEmployeeGroupDataSet = new FindSortedEmployeeGroupDataSet();
        public static VerifyEmployeeDataSet TheExistingEmployeeDataSet = new VerifyEmployeeDataSet();
        public static FindProjectByAssignedProjectIDDataSet TheFindProjectsByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        public static FindProjectByProjectNameDataSet TheFindProjectByProjectNameDataSet = new FindProjectByProjectNameDataSet();
        public static FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindActiveEmployeesDataSet TheFindActiveEmployeeDataSet = new FindActiveEmployeesDataSet();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        public static FindSortedWorkTaskDataSet TheFindSortedWorkTaskDataSet = new FindSortedWorkTaskDataSet();
        public static VehicleExceptionEmailDataSet TheVehicleExceptionEmailDataSet = new VehicleExceptionEmailDataSet();
        public static FindOpenVehicleMainProblemsByVehicleIDDataSet TheFindOpenVehicleMainProblemsByVehicleIDDataSet = new FindOpenVehicleMainProblemsByVehicleIDDataSet();
        public static CancelledOrdersDataSet TheCancelledOrdersDataSet = new CancelledOrdersDataSet();
        public static FindVehicleBulkToolByTransactionIDDataSet TheFindVehicleBulkByTransactionIDDataSet = new FindVehicleBulkToolByTransactionIDDataSet();
        public static EmailListDataSet TheEmailListDataSet = new EmailListDataSet();
        public static FindAssignedTasksByAssignedEmployeeIDDataSet TheFindAssignedTasksByAssignedEmployeeIDDataSet = new FindAssignedTasksByAssignedEmployeeIDDataSet();
        public static FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();
        public static FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        public static DesignProjectInvoicingReportDataSet TheDesignProjectInvoicingReportDataSet = new DesignProjectInvoicingReportDataSet();

        //setting up variables
        public static bool gblnKeepNewEmployee;
        public static int gintEmployeeID;
        public static int gintWorkTaskID;
        public static string gstrFirstName;
        public static string gstrLastName;
        public static int gintProjectID;
        public static string gstrAssignedProjectID;
        public static string gstrWorkTask;
        public static DateTime gdatStartDate;
        public static DateTime gdatEndDate;
        public static int gintPartID;
        public static string gstrPartNumber;
        public static string gstrJDEPartNumber;
        public static int gintToolKey;
        public static string gstrToolID;
        public static bool gblnToolSignedIn;
        public static bool gblnToolSignedOut;
        public static int gintWarehouseID;
        public static string gstrHomeOffice;
        public static int gintTransactionID;
        public static int gintVehicleID;
        public static string gstrVehicleNumber;
        public static string gstrInspectionType;
        public static string gstrInspectionStatus;
        public static bool gblnServicable;
        public static DateTime gdatTransactionDate;
        public static int gintInspectionID;
        public static int gintOdometerReading;
        public static string gstrVehicleProblem;
        public static string gstrCleanlinessNotes;
        public static bool gblnReportRan;
        public static bool gblnWorkOrderSelected;
        public static int gintCategoryID;
        public static string gstrToolCategory;
        public static int gintProblemID;
        public static int gintVendorID;
        public static string gstrToolDescription;
        public static int gintAssignedTaskID;
        int gintNumberOfTasks;
        public static bool gblnLoggedIn;
        public static int gintTrailerID;
        public static string gstrInspectionProblem;
        public static bool gblnExistingTrailerProblem;
        public static int gintItemID;
        public static int gintPhoneID;
        
        //setting up windows
        public static AddEmployee AddNewEmployee = new AddEmployee();
        public static AddEmployeeGroup AddNewEmployeeGroup = new AddEmployeeGroup();
        public static AddWorkTask AddNewWorkTask = new AddWorkTask();
        public static EditWorkTask EditExisingWorkTask = new EditWorkTask();
        public static AddProject AddNewProject = new AddProject();
        public static EditProject EditExistingProject = new EditProject();
        public static ExistingProjects FindExistingProjects = new ExistingProjects();
        public static AddProjectLabor AddNewProjectLabor = new AddProjectLabor();
        public static FindEmployeeHours FindAnEmployeeHours = new FindEmployeeHours();
        public static FindEmployeeCrewAssignments FindingEmployeeCrewAssignments = new FindEmployeeCrewAssignments();
        public static FindLaborHours FindingLaborHours = new FindLaborHours();
        public static EditEmployee EditAnEmployee = new EditEmployee();
        public static TransferInventory TransferExistingInventory = new TransferInventory();
        public static FindProjectHours FindingProjectHours = new FindProjectHours();
        public static ShopHoursAnalysis ComputingShopHoursAnalysis = new ShopHoursAnalysis();
        public static FindProjectEmployeeHours TheFindProjectEmployeeHours = new FindProjectEmployeeHours();
        public static FindMultipleEmployeeProduction FindMultipleEmployeeProductionWindow = new FindMultipleEmployeeProduction();
        public static ToolSignIn ToolSignInWindow = new ToolSignIn();
        public static SignOutTool SignOutToolWindow = new SignOutTool();
        public static ToolHistory ToolHistoryWindow = new ToolHistory();
        public static ToolsAssignedToEmployee ToolsAssignedToEmployeeWindow = new ToolsAssignedToEmployee();
        public static TerminateEmployee TerminateEmployeeWindow = new TerminateEmployee();
        public static EditProjectLabor EditProjectLaborWindow = new EditProjectLabor();
        public static CycleCounts CycleCountsWindow = new CycleCounts();
        public static AddPart AddPartWindow = new AddPart();
        public static EditParts EditPartsWindow = new EditParts();
        public static AddPartFromMasterList AddPartFromMasterListWindow = new AddPartFromMasterList();
        public static ProjectDateSearch ProjectDateSearchWindow = new ProjectDateSearch();
        public static EmployeeCrewSummary EmployeeCrewSummaryWindow = new EmployeeCrewSummary();
        public static ApplicationHelp ApplicationHelpWindow = new ApplicationHelp();
        public static DOTAuditReport DOTAuditReportWindow = new DOTAuditReport();
        public static ManagerWeeklyInspectionReport ManagerWeeklyInspectioinReportWindow = new ManagerWeeklyInspectionReport();
        public static ManagerWeeklyAuditDataEntry ManagerWeeklyAuditDataEntryWindow = new ManagerWeeklyAuditDataEntry();
        public static AddVehicle AddVehicleWindow = new AddVehicle();
        public static AssignVehicle AssignVehicleWindow = new AssignVehicle();
        public static DailyVehicleInspection DailyVehicleInspectionWindow = new DailyVehicleInspection();
        public static VehiclesInYard VehiclesInYardWindow = new VehiclesInYard();
        public static VehicleRoster VehicleRosterWindow = new VehicleRoster();
        public static ImportVehicles ImportVehicleWindow = new ImportVehicles();
        public static VehicleHistoryReport VehicleHistoryReportWindow = new VehicleHistoryReport();
        public static DailyVehicleInspectionReport DailyVehicleInspectionReportWindow = new DailyVehicleInspectionReport();
        public static VehicleExceptionReport VehicleExceptionReportWindow = new VehicleExceptionReport();
        public static EditVehicle EditVehicleWindow = new EditVehicle();
        public static TotalProjectInformation TotalProjectInformationWindow = new TotalProjectInformation();
        public static RetireVehicle RetireVehicleWindow = new RetireVehicle();
        public static VehicleInYardReport VehicleInYardReportWindow = new VehicleInYardReport();
        public static AddDOTStatus AddDOTStatusWindow = new AddDOTStatus();
        public static AddGPSStatus AddGPSStatusWindow = new AddGPSStatus();
        public static PreventiveMaintenance PreventiveMaintenanceWindow = new PreventiveMaintenance();
        public static ImportMDUDropBuryOrders ImportMDUDropBuryOrdersWindow = new ImportMDUDropBuryOrders();
        public static AddVehicleBulkTools AddVehicleBulkToolsWindow = new AddVehicleBulkTools();
        public static SelectVehicleBulkTool SelectVehicleBulkToolWindow = new SelectVehicleBulkTool();
        public static RemoveEmployeesFromVehicles RemoveEmployeesFromVehiclesWindow = new RemoveEmployeesFromVehicles();
        public static ProductivityDataEntryReports ProductivityDataEntryReportsWindow = new ProductivityDataEntryReports();
        public static FindProjects FindProjectsWindow = new FindProjects();
        public static FindAvailableTools FindAvailableToolsWindow = new FindAvailableTools();
        public static CreateTool CreateToolWindow = new CreateTool();
        public static EditTools EditToolsWindow = new EditTools();
        public static NewVehicleProblem NewVehicleProblemWindow = new NewVehicleProblem();
        public static UpdateVehicleProblem UpdateVehicleProblemWindow = new UpdateVehicleProblem();
        public static AvailableVehicles AvailableVehiclesWindow = new AvailableVehicles();
        public static SendVehicleToShop SendVehicleToShopWindow = new SendVehicleToShop();
        public static OpenVehicleProblems OpenVehicleProblemsWindow = new OpenVehicleProblems();
        public static VehiclesInShop VehiclesInShopWindow = new VehiclesInShop();
        public static ViewWorkTaskStatistics ViewWorkTaskStatisticsWindow = new ViewWorkTaskStatistics();
        public static ImportWorkTask ImportWorkTaskWindow = new ImportWorkTask();
        public static EnterApprovedTransaction EnterApprovedTransactionWindow = new EnterApprovedTransaction();
        public static VehicleProblemHistory VehicleProblemHistoryWindow = new VehicleProblemHistory();
        public static ReportToolProblem ReportToolProblemWindow = new ReportToolProblem();
        public static OpenToolProblems OpenToolProblemsWindow = new OpenToolProblems();
        public static BrokenToolReport BrokenToolReportWindow = new BrokenToolReport();
        public static AssignTasks AssignTasksWindow = new AssignTasks();
        public static UpdateAssignedTasks UpdateAssignTasksWindow = new UpdateAssignedTasks();
        public static MyOriginatingTasks MyOriginatingTasksWindow = new MyOriginatingTasks();
        public static EditProjectWorkTask EditProjectWorkTaskWindow = new EditProjectWorkTask();
        public static HistoricalVehicleExceptionReport HistoricalVehicleExceptionReportWindow = new HistoricalVehicleExceptionReport();
        public static AuditReportEmployeeAssignment AuditReportEmployeeAssignmentWindow = new AuditReportEmployeeAssignment();
        public static CurrentToolLocation CurrentToolLocationWindow = new CurrentToolLocation();
        public static DeactivateEmployeesTools DeactivateEmployeeToolsWindow = new DeactivateEmployeesTools();
        public static SendEmail SendEmailWindow = new SendEmail();
        public static AddEmployeeToVehicleEmailList AddEmployeeToVehicleEmailListWindow = new AddEmployeeToVehicleEmailList();
        public static RemoveEmployeesFromVehicleEmailList RemoveEmployeesFromVehiclesEmailListWindow = new RemoveEmployeesFromVehicleEmailList();
        public static AddTrailers AddTrailersWindow = new AddTrailers();
        public static AddTrailerCategory AddTrailerCategoryWindow = new AddTrailerCategory();
        public static ImportTrailers ImportTrailersWindow = new ImportTrailers();
        public static EditTrailer EditTrailerWindow = new EditTrailer();
        public static AssignTrailer AssignTrailerWindow = new AssignTrailer();
        public static TrailerRoster TrailerRosterWindow = new TrailerRoster();
        public static TrailersInYard TrailersInYardWindow = new TrailersInYard();
        public static DailyTrailerInspection DailyTrailerInspectionWindow = new DailyTrailerInspection();
        public static CompareEmployeeCrews CompareEmployeeCrewsWindow = new CompareEmployeeCrews();
        public static AddDepartment AddDepartmentWindow = new AddDepartment();
        public static WarehouseInventoryStats WarehouseInventoryStatsWindow = new WarehouseInventoryStats();
        public static ImportEmployeeHours ImportEmployeeHoursWindow = new ImportEmployeeHours();
        public static EmployeePunchedVSProductionHours EmployeePunchedVSProductionHoursWindow = new EmployeePunchedVSProductionHours();
        public static EmployeeRoster EmployeeRosterWindow = new EmployeeRoster();
        public static ImportEmployeePunches ImportEmployeePunchesWindow = new ImportEmployeePunches();
        public static ViewEmployeePunches ViewEmployeePunchesWindow = new ViewEmployeePunches();
        public static DailyTrailerInspectionReport DailyTrailerInspectionReportWindow = new DailyTrailerInspectionReport();
        public static CreateToolCategory CreateToolCategoryWindow = new CreateToolCategory();
        public static BulkToolSignOut BulkToolSignOutWindow = new BulkToolSignOut();
        public static SelectBulkToolForSignIn SelectBulkToolForSignInWindow = new SelectBulkToolForSignIn();
        public static ManagerProductivityPunchedReport ManagerProductivityPunchedReportWindow = new ManagerProductivityPunchedReport();
        public static EmployeeLaborRate EmployeeLaborRateWindow = new EmployeeLaborRate();
        public static ImportITAssets ImportITAssetsWindow = new ImportITAssets();
        public static SelectITAsset SelectITAssetWindow = new SelectITAsset();
        public static InvoiceVehicleProblems InvoiceVehicleProblemsWindow = new InvoiceVehicleProblems();
        public static VehicleProblemInvoiceReport VehicleProblemInvoiceReportWindow = new VehicleProblemInvoiceReport();
        public static ViewVehicleProblem ViewVehicleProblemWindow = new ViewVehicleProblem();
        public static VehicleInvoicesByDateRange VehicleINvoicesByDateRangeWindow = new VehicleInvoicesByDateRange();
        public static AddITAssets AddITAssetsWindow = new AddITAssets();
        public static RetireITAsset RetireITAssetWindow = new RetireITAsset();
        public static CurrentITAssets CurrentITAssetsWindow = new CurrentITAssets();
        public static RetiredITAssets RetiredITAssetsWindow = new RetiredITAssets();
        public static AddJobType AddJobTypeWindow = new AddJobType();
        public static ImportVendors ImportVendorsWindow = new ImportVendors();
        public static ManagerWeeklyVehicleCount ManagerWeeklyVehicleCountWindow = new ManagerWeeklyVehicleCount();
        public static ProductionEmployeeProductivityMeasure ProductionEmployeeProductivityMeasureWindow = new ProductionEmployeeProductivityMeasure();
        public static UpdateWorkTaskProductivityValue UpdateWorkTaskProductivityValueWindow = new UpdateWorkTaskProductivityValue();
        public static PhoneList PhoneListWindow = new PhoneList();
        public static AssignPhoneExtension AssignPhoneExtensionWindow = new AssignPhoneExtension();
        public static AssignCellPhones AssignCellPhonesWindow = new AssignCellPhones();
        public static ProjectMaterial ProjectMaterialWindow = new ProjectMaterial();
        public static AddWOVBillingCodes AddWOVBillingCodesWindow = new AddWOVBillingCodes();
        public static EditWOVBillingCodes EditWOVBillingCodesWindow = new EditWOVBillingCodes();
        public static AddWOVTasks AddWOVTasksWindow = new AddWOVTasks();
        public static EditWOVTask EditWOVTaskWindow = new EditWOVTask();
        public static AddTechPayItem AddTechPayItemWindow = new AddTechPayItem();
        public static AddVehicleToEmployeeTable AddVehicleToEmployeeTableWindow = new AddVehicleToEmployeeTable();
        public static OpenCellPhoneReport OpenCellPhoneReportWindow = new OpenCellPhoneReport();
        public static DesignEmployeeProductivity DesignEmployeeProductivityWindow = new DesignEmployeeProductivity();
        public static DesignProjectInvoicing DesignProjectInvoicingWindow = new DesignProjectInvoicing();

        FindEmployeeProductivityFootage FindEmployeeProductivityFootageWindow = new FindEmployeeProductivityFootage();

        DispatcherTimer MyTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            gblnLoggedIn = false;
            UpdateLogon();
        }

        private void mitSignOut_Click(object sender, RoutedEventArgs e)
        {
            gblnLoggedIn = false;

            ResetMenu();

            CloseAllForms();

            UpdateLogon();
        }
        private void UpdateLogon()
        {
            int intRecordsReturned;

            try
            {

                EmployeeLogon EmployeeLogon = new EmployeeLogon();
                EmployeeLogon.ShowDialog();

                intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

                if(intRecordsReturned > 0)
                {

                    SecurityForMenu();

                    TheFindPartsWarehousesDataSet = TheEmployeeclass.FindPartsWarehouses();

                    TheFindWarehousesDataSet = TheEmployeeclass.FindWarehouses();

                    TheFindSortedEmployeeGroupDataSet = TheEmployeeclass.FindSortedEmpoyeeGroup();
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Main Window // Grid Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void CloseAllForms()
        {
            AddNewEmployee.Visibility = Visibility.Hidden;
            AddNewEmployeeGroup.Visibility = Visibility.Hidden;
            AddNewWorkTask.Visibility = Visibility.Hidden;
            EditExisingWorkTask.Visibility = Visibility.Hidden;
            AddNewProject.Visibility = Visibility.Hidden;
            EditExistingProject.Visibility = Visibility.Hidden;
            FindExistingProjects.Visibility = Visibility.Hidden;
            AddNewProjectLabor.Visibility = Visibility.Hidden;
            FindAnEmployeeHours.Visibility = Visibility.Hidden;
            FindingEmployeeCrewAssignments.Visibility = Visibility.Hidden;
            FindingLaborHours.Visibility = Visibility.Hidden;
            EditAnEmployee.Visibility = Visibility.Hidden;
            TransferExistingInventory.Visibility = Visibility.Hidden;
            FindingProjectHours.Visibility = Visibility.Hidden;
            ComputingShopHoursAnalysis.Visibility = Visibility.Hidden;
            TheFindProjectEmployeeHours.Visibility = Visibility.Hidden;
            FindMultipleEmployeeProductionWindow.Visibility = Visibility.Hidden;
            ToolSignInWindow.Visibility = Visibility.Hidden;
            SignOutToolWindow.Visibility = Visibility.Hidden;
            ToolHistoryWindow.Visibility = Visibility.Hidden;
            ToolsAssignedToEmployeeWindow.Visibility = Visibility.Hidden;
            TerminateEmployeeWindow.Visibility = Visibility.Hidden;
            EditProjectLaborWindow.Visibility = Visibility.Hidden;
            CycleCountsWindow.Visibility = Visibility.Hidden;
            AddPartWindow.Visibility = Visibility.Hidden;
            EditPartsWindow.Visibility = Visibility.Hidden;
            AddPartFromMasterListWindow.Visibility = Visibility.Hidden;
            ProjectDateSearchWindow.Visibility = Visibility.Hidden;
            EmployeeCrewSummaryWindow.Visibility = Visibility.Hidden;
            ApplicationHelpWindow.Visibility = Visibility.Hidden;
            DOTAuditReportWindow.Visibility = Visibility.Hidden;
            ManagerWeeklyInspectioinReportWindow.Visibility = Visibility.Hidden;
            ManagerWeeklyAuditDataEntryWindow.Visibility = Visibility.Hidden;
            AddVehicleWindow.Visibility = Visibility.Hidden;
            AssignVehicleWindow.Visibility = Visibility.Hidden;
            DailyVehicleInspectionWindow.Visibility = Visibility.Hidden;
            VehiclesInYardWindow.Visibility = Visibility.Hidden;
            VehicleRosterWindow.Visibility = Visibility.Hidden;
            ImportVehicleWindow.Visibility = Visibility.Hidden;
            VehicleHistoryReportWindow.Visibility = Visibility.Hidden;
            DailyVehicleInspectionReportWindow.Visibility = Visibility.Hidden;
            VehicleExceptionReportWindow.Visibility = Visibility.Hidden;
            EditVehicleWindow.Visibility = Visibility.Hidden;
            TotalProjectInformationWindow.Visibility = Visibility.Hidden;
            RetireVehicleWindow.Visibility = Visibility.Hidden;
            VehicleInYardReportWindow.Visibility = Visibility.Hidden;
            AddDOTStatusWindow.Visibility = Visibility.Hidden;
            AddGPSStatusWindow.Visibility = Visibility.Hidden;
            PreventiveMaintenanceWindow.Visibility = Visibility.Hidden;
            ImportMDUDropBuryOrdersWindow.Visibility = Visibility.Hidden;
            AddVehicleBulkToolsWindow.Visibility = Visibility.Hidden;
            SelectVehicleBulkToolWindow.Visibility = Visibility.Hidden;
            RemoveEmployeesFromVehiclesWindow.Visibility = Visibility.Hidden;
            ProductivityDataEntryReportsWindow.Visibility = Visibility.Hidden;
            FindProjectsWindow.Visibility = Visibility.Hidden;
            FindAvailableToolsWindow.Visibility = Visibility.Hidden;
            CreateToolWindow.Visibility = Visibility.Hidden;
            EditToolsWindow.Visibility = Visibility.Hidden;
            NewVehicleProblemWindow.Visibility = Visibility.Hidden;
            UpdateVehicleProblemWindow.Visibility = Visibility.Hidden;
            AvailableVehiclesWindow.Visibility = Visibility.Hidden;
            SendVehicleToShopWindow.Visibility = Visibility.Hidden;
            OpenVehicleProblemsWindow.Visibility = Visibility.Hidden;
            VehiclesInShopWindow.Visibility = Visibility.Hidden;
            ViewWorkTaskStatisticsWindow.Visibility = Visibility.Hidden;
            ImportWorkTaskWindow.Visibility = Visibility.Hidden;
            EnterApprovedTransactionWindow.Visibility = Visibility.Hidden;
            VehicleProblemHistoryWindow.Visibility = Visibility.Hidden;
            ReportToolProblemWindow.Visibility = Visibility.Hidden;
            OpenToolProblemsWindow.Visibility = Visibility.Hidden;
            BrokenToolReportWindow.Visibility = Visibility.Hidden;
            AssignTasksWindow.Visibility = Visibility.Hidden;
            UpdateAssignTasksWindow.Visibility = Visibility.Hidden;
            MyOriginatingTasksWindow.Visibility = Visibility.Hidden;
            EditProjectWorkTaskWindow.Visibility = Visibility.Hidden;
            HistoricalVehicleExceptionReportWindow.Visibility = Visibility.Hidden;
            AuditReportEmployeeAssignmentWindow.Visibility = Visibility.Hidden;
            CurrentToolLocationWindow.Visibility = Visibility.Hidden;
            DeactivateEmployeeToolsWindow.Visibility = Visibility.Hidden;
            SendEmailWindow.Visibility = Visibility.Hidden;
            AddEmployeeToVehicleEmailListWindow.Visibility = Visibility.Hidden;
            RemoveEmployeesFromVehiclesEmailListWindow.Visibility = Visibility.Hidden;
            AddTrailersWindow.Visibility = Visibility.Hidden;
            AddTrailerCategoryWindow.Visibility = Visibility.Hidden;
            ImportTrailersWindow.Visibility = Visibility.Hidden;
            EditTrailerWindow.Visibility = Visibility.Hidden;
            AssignTrailerWindow.Visibility = Visibility.Hidden;
            TrailerRosterWindow.Visibility = Visibility.Hidden;
            TrailersInYardWindow.Visibility = Visibility.Hidden;
            DailyTrailerInspectionWindow.Visibility = Visibility.Hidden;
            CompareEmployeeCrewsWindow.Visibility = Visibility.Hidden;
            FindEmployeeProductivityFootageWindow.Visibility = Visibility.Hidden;
            AddDepartmentWindow.Visibility = Visibility.Hidden;
            WarehouseInventoryStatsWindow.Visibility = Visibility.Hidden;
            ImportEmployeeHoursWindow.Visibility = Visibility.Hidden;
            EmployeePunchedVSProductionHoursWindow.Visibility = Visibility.Hidden;
            EmployeeRosterWindow.Visibility = Visibility.Hidden;
            ImportEmployeePunchesWindow.Visibility = Visibility.Hidden;
            ViewEmployeePunchesWindow.Visibility = Visibility.Hidden;
            DailyTrailerInspectionWindow.Visibility = Visibility.Hidden;
            CreateToolCategoryWindow.Visibility = Visibility.Hidden;
            BulkToolSignOutWindow.Visibility = Visibility.Hidden;
            SelectBulkToolForSignInWindow.Visibility = Visibility.Hidden;
            ManagerProductivityPunchedReportWindow.Visibility = Visibility.Hidden;
            EmployeeLaborRateWindow.Visibility = Visibility.Hidden;
            ImportITAssetsWindow.Visibility = Visibility.Hidden;
            SelectITAssetWindow.Visibility = Visibility.Hidden;
            InvoiceVehicleProblemsWindow.Visibility = Visibility.Hidden;
            VehicleProblemInvoiceReportWindow.Visibility = Visibility.Hidden;
            ViewVehicleProblemWindow.Visibility = Visibility.Hidden;
            VehicleINvoicesByDateRangeWindow.Visibility = Visibility.Hidden;
            AddITAssetsWindow.Visibility = Visibility.Hidden;
            RetireITAssetWindow.Visibility = Visibility.Hidden;
            CurrentITAssetsWindow.Visibility = Visibility.Hidden;
            RetiredITAssetsWindow.Visibility = Visibility.Hidden;
            AddJobTypeWindow.Visibility = Visibility.Hidden;
            ImportVendorsWindow.Visibility = Visibility.Hidden;
            ManagerWeeklyVehicleCountWindow.Visibility = Visibility.Hidden;
            ProductionEmployeeProductivityMeasureWindow.Visibility = Visibility.Hidden;
            UpdateWorkTaskProductivityValueWindow.Visibility = Visibility.Hidden;
            PhoneListWindow.Visibility = Visibility.Hidden;
            AssignPhoneExtensionWindow.Visibility = Visibility.Hidden;
            AssignCellPhonesWindow.Visibility = Visibility.Hidden;
            ProjectMaterialWindow.Visibility = Visibility.Hidden;
            AddWOVBillingCodesWindow.Visibility = Visibility.Hidden;
            EditWOVBillingCodesWindow.Visibility = Visibility.Hidden;
            AddWOVTasksWindow.Visibility = Visibility.Hidden;
            EditWOVTaskWindow.Visibility = Visibility.Hidden;
            AddTechPayItemWindow.Visibility = Visibility.Hidden;
            AddVehicleToEmployeeTableWindow.Visibility = Visibility.Hidden;
            OpenCellPhoneReportWindow.Visibility = Visibility.Hidden;
            DesignEmployeeProductivityWindow.Visibility = Visibility.Hidden;
            DesignProjectInvoicingWindow.Visibility = Visibility.Hidden;
        }

        private void mitAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddNewEmployee.Visibility = Visibility.Visible;
        }
        private void SecurityForMenu()
        {
            if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "USERS")
            {
                mitAdministration.Visibility = Visibility.Hidden;
                mitEmployees.Visibility = Visibility.Hidden;
                mitProjects.Visibility = Visibility.Hidden;
                mitMDUDropBury.Visibility = Visibility.Hidden;
                mitInventory.Visibility = Visibility.Hidden;
                mitAssets.Visibility = Visibility.Hidden;
                mitToolSignIn.Visibility = Visibility.Hidden;
                mitToolSignOut.Visibility = Visibility.Hidden;
                mitToolHistory.Visibility = Visibility.Hidden;
                mitCreateTool.Visibility = Visibility.Hidden;
                mitEditTool.Visibility = Visibility.Hidden;
                mitToolAvailability.Visibility = Visibility.Hidden;
                mitVehicleToolAssignment.Visibility = Visibility.Hidden;
                mitInspections.Visibility = Visibility.Hidden;
                mitEditTrailer.Visibility = Visibility.Hidden;
                mitEditVehicle.Visibility = Visibility.Hidden;
                mitAssignTrailer.Visibility = Visibility.Hidden;
                mitTrailersInYard.Visibility = Visibility.Hidden;
                mitViewEmployeePunches.Visibility = Visibility.Hidden;
                mitManagerProductivityPunched.Visibility = Visibility.Hidden;
                mitImportITAssets.Visibility = Visibility.Hidden;
                mitChangeITAssetLocation.Visibility = Visibility.Hidden;
                mitDesignEmployeeProductivity.Visibility = Visibility.Hidden;
                mitDesignInvoicing.Visibility = Visibility.Hidden;
            }
            else if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "WAREHOUSE")
            {
                mitAdministration.Visibility = Visibility.Hidden;
                mitEmployees.Visibility = Visibility.Hidden;
                mitProjects.Visibility = Visibility.Hidden;
                mitMDUDropBury.Visibility = Visibility.Hidden;
                mitAssets.Visibility = Visibility.Hidden;
                mitDOTAuditReport.Visibility = Visibility.Hidden;
                mitViewEmployeePunches.Visibility = Visibility.Hidden;
                mitViewEmployeePunches.Visibility = Visibility.Hidden;
                mitImportITAssets.Visibility = Visibility.Hidden;
                mitChangeITAssetLocation.Visibility = Visibility.Hidden;
                mitDesignEmployeeProductivity.Visibility = Visibility.Hidden;
                mitDesignInvoicing.Visibility = Visibility.Hidden;
            }
            else if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "MANAGERS")
            {
                mitAdministration.Visibility = Visibility.Hidden;
                mitAssets.Visibility = Visibility.Hidden;
                mitTransferInventory.Visibility = Visibility.Hidden;
                mitToolSignIn.Visibility = Visibility.Hidden;
                mitToolSignOut.Visibility = Visibility.Hidden;
                mitEditTrailer.Visibility = Visibility.Hidden;
                mitEditVehicle.Visibility = Visibility.Hidden;
                mitCreateTool.Visibility = Visibility.Hidden;
                mitEditTool.Visibility = Visibility.Hidden;
                mitAssignTrailer.Visibility = Visibility.Hidden;
                mitTrailersInYard.Visibility = Visibility.Hidden;
                mitImportITAssets.Visibility = Visibility.Hidden;
                mitChangeITAssetLocation.Visibility = Visibility.Hidden;
            }
            else if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "OFFICE")
            {
                mitAdministration.Visibility = Visibility.Hidden;
                mitAssets.Visibility = Visibility.Hidden;
                mitTransferInventory.Visibility = Visibility.Hidden;
                mitShopHoursAnalysis.Visibility = Visibility.Hidden;
                mitToolSignIn.Visibility = Visibility.Hidden;
                mitToolSignOut.Visibility = Visibility.Hidden;
                mitCycleCount.Visibility = Visibility.Hidden;
                mitDOTAuditReport.Visibility = Visibility.Hidden;
                mitEditTrailer.Visibility = Visibility.Hidden;
                mitEditVehicle.Visibility = Visibility.Hidden;
                mitCreateTool.Visibility = Visibility.Hidden;
                mitEditTool.Visibility = Visibility.Hidden;
                mitAssignTrailer.Visibility = Visibility.Hidden;
                mitTrailersInYard.Visibility = Visibility.Hidden;
                mitViewEmployeePunches.Visibility = Visibility.Hidden;
                mitManagerProductivityPunched.Visibility = Visibility.Hidden;
                mitImportITAssets.Visibility = Visibility.Hidden;
                mitChangeITAssetLocation.Visibility = Visibility.Hidden;
            }
            else if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "SUPER USER")
            {
                mitAdministration.Visibility = Visibility.Hidden;
                mitShopHoursAnalysis.Visibility = Visibility.Hidden;
                mitImportITAssets.Visibility = Visibility.Hidden;
                mitChangeITAssetLocation.Visibility = Visibility.Hidden;
            }
            else if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "IT")
            {

            }
            else if (TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "ADMIN")
            {

            }
            else
            {
                mitAddEmployee.Visibility = Visibility.Hidden;
                mitAddEmployeeGroups.Visibility = Visibility.Hidden;
                mitAddEmployeeGroups.Visibility = Visibility.Hidden;
                mitAddProject.Visibility = Visibility.Hidden;
                mitAddProjectLabor.Visibility = Visibility.Hidden;
                mitAdministration.Visibility = Visibility.Hidden;
                mitAddWorkTask.Visibility = Visibility.Hidden;
                mitEditEmployee.Visibility = Visibility.Hidden;
                mitEditProject.Visibility = Visibility.Hidden;
                mitEditProjectLabor.Visibility = Visibility.Hidden;
                mitEmployees.Visibility = Visibility.Hidden;
                mitEmployeLaborRate.Visibility = Visibility.Hidden;
                mitFindEmployeeCrewAssignment.Visibility = Visibility.Hidden;
                mitFindEmployeeHoursOverDateRange.Visibility = Visibility.Hidden;
                mitFindLaborHoursByDateRange.Visibility = Visibility.Hidden;
                mitFindProject.Visibility = Visibility.Hidden;
                mitFindProjectHours.Visibility = Visibility.Hidden;
                mitProductivityReportForTasks.Visibility = Visibility.Hidden;
                mitMDUDropBury.Visibility = Visibility.Hidden;
                mitProjectDateSearch.Visibility = Visibility.Hidden;
                mitProjectReports.Visibility = Visibility.Hidden;
                mitProjects.Visibility = Visibility.Hidden;
                mitTerminateEmployee.Visibility = Visibility.Hidden;
                mitInventory.Visibility = Visibility.Hidden;
                mitVehicles.Visibility = Visibility.Hidden;
                mitTools.Visibility = Visibility.Hidden;
                mitTrailers.Visibility = Visibility.Hidden;
                mitAssets.Visibility = Visibility.Hidden;
                mitInformationTechnology.Visibility = Visibility.Hidden;
                mitHelp.Visibility = Visibility.Hidden;
                mitInspections.Visibility = Visibility.Hidden;
                mitManagerProductivityPunched.Visibility = Visibility.Hidden;
                mitImportITAssets.Visibility = Visibility.Hidden;
                mitChangeITAssetLocation.Visibility = Visibility.Hidden;
                DesignEmployeeProductivityWindow.Visibility = Visibility.Hidden;
            }
        }
        private void ResetMenu()
        {
            mitAddEmployee.Visibility = Visibility.Visible;
            mitAddEmployeeGroups.Visibility = Visibility.Visible;
            mitAddEmployeeGroups.Visibility = Visibility.Visible;
            mitAddProject.Visibility = Visibility.Visible;
            mitAddProjectLabor.Visibility = Visibility.Visible;
            mitAdministration.Visibility = Visibility.Visible;
            mitAddWorkTask.Visibility = Visibility.Visible;
            mitEditEmployee.Visibility = Visibility.Visible;
            mitEditProject.Visibility = Visibility.Visible;
            mitEditProjectLabor.Visibility = Visibility.Visible;
            mitEmployees.Visibility = Visibility.Visible;
            mitEmployeLaborRate.Visibility = Visibility.Visible;
            mitFindEmployeeCrewAssignment.Visibility = Visibility.Visible;
            mitFindEmployeeHoursOverDateRange.Visibility = Visibility.Visible;
            mitFindLaborHoursByDateRange.Visibility = Visibility.Visible;
            mitFindProject.Visibility = Visibility.Visible;
            mitFindProjectHours.Visibility = Visibility.Visible;
            mitProductivityReportForTasks.Visibility = Visibility.Visible;
            mitMDUDropBury.Visibility = Visibility.Visible;
            mitProjectDateSearch.Visibility = Visibility.Visible;
            mitProjectReports.Visibility = Visibility.Visible;
            mitProjects.Visibility = Visibility.Visible;
            mitTerminateEmployee.Visibility = Visibility.Visible;
            mitInventory.Visibility = Visibility.Visible;
            mitVehicles.Visibility = Visibility.Visible;
            mitTools.Visibility = Visibility.Visible;
            mitTrailers.Visibility = Visibility.Visible;
            mitAssets.Visibility = Visibility.Visible;
            mitInformationTechnology.Visibility = Visibility.Visible;
            mitHelp.Visibility = Visibility.Visible;
            mitTransferInventory.Visibility = Visibility.Visible;
            mitShopHoursAnalysis.Visibility = Visibility.Visible;
            mitToolSignIn.Visibility = Visibility.Visible;
            mitToolSignOut.Visibility = Visibility.Visible;
            mitToolHistory.Visibility = Visibility.Visible;
            mitCreateTool.Visibility = Visibility.Visible;
            mitEditTool.Visibility = Visibility.Visible;
            mitToolAvailability.Visibility = Visibility.Visible;
            mitVehicleToolAssignment.Visibility = Visibility.Visible;
            mitCycleCount.Visibility = Visibility.Visible;
            mitInspections.Visibility = Visibility.Visible;
            mitDOTAuditReport.Visibility = Visibility.Visible;
            mitEditVehicle.Visibility = Visibility.Visible;
            mitEditTrailer.Visibility = Visibility.Visible;
            mitAssignTrailer.Visibility = Visibility.Visible;
            mitTrailersInYard.Visibility = Visibility.Visible;
            mitViewEmployeePunches.Visibility = Visibility.Visible;
            mitManagerProductivityPunched.Visibility = Visibility.Visible;
            mitImportITAssets.Visibility = Visibility.Visible;
            mitChangeITAssetLocation.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mitAddEmployeeGroups_Click(object sender, RoutedEventArgs e)
        {
            AddNewEmployeeGroup.Visibility = Visibility.Visible;
        }

        private void mitAddWorkTask_Click(object sender, RoutedEventArgs e)
        {
            AddNewWorkTask.Visibility = Visibility.Visible;
        }

        private void mitEditWorkTask_Click(object sender, RoutedEventArgs e)
        {
            EditExisingWorkTask.Visibility = Visibility.Visible;
        }

        private void mitAddProject_Click(object sender, RoutedEventArgs e)
        {
            FindExistingProjects.Visibility = Visibility.Visible;
            AddNewProject.Visibility = Visibility.Visible;
        }

        private void mitEditProject_Click(object sender, RoutedEventArgs e)
        {
            EditExistingProject.Visibility = Visibility.Visible;
            FindExistingProjects.Visibility = Visibility.Visible;
        }

        private void mitAddProjectLabor_Click(object sender, RoutedEventArgs e)
        {
            AddNewProjectLabor.Visibility = Visibility.Visible;
        }

        private void mitFindEmployeeHoursOverDateRange_Click(object sender, RoutedEventArgs e)
        {
            FindAnEmployeeHours.Visibility = Visibility.Visible;
        }

        private void mitFindEmployeeCrewAssignment_Click(object sender, RoutedEventArgs e)
        {
            FindingEmployeeCrewAssignments.Visibility = Visibility.Visible;
        }

        private void mitFindLaborHoursByDateRange_Click(object sender, RoutedEventArgs e)
        {
            FindingLaborHours.Visibility = Visibility.Visible;
        }

        private void mitEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            EditAnEmployee.Visibility = Visibility.Visible;
        }

        private void mitTransferInventory_Click(object sender, RoutedEventArgs e)
        {
            TransferExistingInventory.Visibility = Visibility.Visible;
        }

        private void mitFindProjectHours_Click(object sender, RoutedEventArgs e)
        {
            FindingProjectHours.Visibility = Visibility.Visible;
        }

        private void mitShopHoursAnalysis_Click(object sender, RoutedEventArgs e)
        {
            ComputingShopHoursAnalysis.Visibility = Visibility.Visible;
        }

        private void mitFindProjectEmployeeHours_Click(object sender, RoutedEventArgs e)
        {
            TheFindProjectEmployeeHours.Visibility = Visibility.Visible;
        }

        private void mitFindMultipeEmployeeProduction_Click(object sender, RoutedEventArgs e)
        {
            FindMultipleEmployeeProductionWindow.Visibility = Visibility.Visible;
        }

        private void mitToolSignIn_Click(object sender, RoutedEventArgs e)
        {
            ToolSignInWindow.Visibility = Visibility.Visible;
        }

        private void mitToolSignOut_Click(object sender, RoutedEventArgs e)
        {
            SignOutToolWindow.Visibility = Visibility.Visible;
        }

        private void mitToolHistory_Click(object sender, RoutedEventArgs e)
        {
            ToolHistoryWindow.Visibility = Visibility.Visible;
        }

        private void mitToolsAssignedToEmploye_Click(object sender, RoutedEventArgs e)
        {
            ToolsAssignedToEmployeeWindow.Visibility = Visibility.Visible;
        }

        private void mitTerminateEmployee_Click(object sender, RoutedEventArgs e)
        {
            TerminateEmployeeWindow.Visibility = Visibility.Visible;
        }

        private void mitEditProjectLabor_Click(object sender, RoutedEventArgs e)
        {
            EditProjectLaborWindow.Visibility = Visibility.Visible;
        }

        private void mitCycleCount_Click(object sender, RoutedEventArgs e)
        {
            CycleCountsWindow.Visibility = Visibility.Visible;
        }

        private void mitAddPart_Click(object sender, RoutedEventArgs e)
        {
            AddPartWindow.Visibility = Visibility.Visible;
        }

        private void mitEditPart_Click(object sender, RoutedEventArgs e)
        {
            EditPartsWindow.Visibility = Visibility.Visible;
        }

        private void mitAddPartFromMasterList_Click(object sender, RoutedEventArgs e)
        {
            AddPartFromMasterListWindow.Visibility = Visibility.Visible;
        }

        private void mitProjectDateSearch_Click(object sender, RoutedEventArgs e)
        {
            ProjectDateSearchWindow.Visibility = Visibility.Visible;
        }

        private void mitVehicleToolAssignment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mitEmployeeCrewSummary_Click(object sender, RoutedEventArgs e)
        {
            EmployeeCrewSummaryWindow.Visibility = Visibility.Visible;
        }

        private void mitHelp_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void mitDOTAuditReport_Click(object sender, RoutedEventArgs e)
        {
            DOTAuditReportWindow.Visibility = Visibility.Visible;
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitManagerWeeklyInspectionReport_Click(object sender, RoutedEventArgs e)
        {
            ManagerWeeklyInspectioinReportWindow.Visibility = Visibility.Visible;
        }

        private void mitManagerWeeklyDataEntry_Click(object sender, RoutedEventArgs e)
        {
            ManagerWeeklyAuditDataEntryWindow.Visibility = Visibility.Visible;
        }

        private void mitAddVehicle_Click(object sender, RoutedEventArgs e)
        {
            AddVehicleWindow.Visibility = Visibility.Visible;
        }

        private void mitAssignVehicle_Click(object sender, RoutedEventArgs e)
        {
            AssignVehicleWindow.Visibility = Visibility.Visible;
        }

        private void mitInspections_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void mitVehiclesInYard_Click(object sender, RoutedEventArgs e)
        {
            VehiclesInYardWindow.Visibility = Visibility.Visible;
        }

        private void mitVehicleRoster_Click(object sender, RoutedEventArgs e)
        {
            VehicleRosterWindow.Visibility = Visibility.Visible;
        }

        private void mitVehicleHistoryReport_Click(object sender, RoutedEventArgs e)
        {
            VehicleHistoryReportWindow.Visibility = Visibility.Visible;
        }

        private void mitImportVehicles_Click(object sender, RoutedEventArgs e)
        {
            ImportVehicleWindow.Visibility = Visibility.Visible;
        }

        private void mitDailyVehicleInspectionReport_Click(object sender, RoutedEventArgs e)
        {
            DailyVehicleInspectionReportWindow.Visibility = Visibility.Visible;
        }

        private void mitDailyVehicleInspection_Click(object sender, RoutedEventArgs e)
        {
            DailyVehicleInspectionWindow.Visibility = Visibility.Visible;
        }

        private void mitVehicleExceptionReport_Click(object sender, RoutedEventArgs e)
        {
            VehicleExceptionReportWindow.Visibility = Visibility.Visible;
        }

        private void mitEditVehicle_Click(object sender, RoutedEventArgs e)
        {
            EditVehicleWindow.Visibility = Visibility.Visible;
        }

        private void mitProjectLaborMaterial_Click(object sender, RoutedEventArgs e)
        {
            TotalProjectInformationWindow.Visibility = Visibility.Visible;
        }

        private void mitRetireVehice_Click(object sender, RoutedEventArgs e)
        {
            RetireVehicleWindow.Visibility = Visibility.Visible;
        }

        private void mitVehicleInYardReport_Click(object sender, RoutedEventArgs e)
        {
            VehicleInYardReportWindow.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gblnReportRan = false;

            gintNumberOfTasks = 0;

            MyTimer.Tick += new EventHandler(BeginTheProcess);
            MyTimer.Interval = new TimeSpan(0, 0, 10);
            MyTimer.Start();
        }
        private void BeginTheProcess(object sender, EventArgs e)
        {
            int intHours;
            int intMinutes;
            int intNumberOfRecords;

            DateTime datTodaysDate = DateTime.Now;
            
            intHours = datTodaysDate.Hour;
            intMinutes = datTodaysDate.Minute;

            if((intHours >= 1) && (intHours < 4))
            {
                Application.Current.Shutdown();
            }
            if(intHours < 6)
            {
                TheMessagesClass.ErrorMessage("The Program Will Not Be Available until 6:30 am");
                Application.Current.Shutdown();
            }
            else if ((intHours == 6) && (intMinutes < 30))
            {
                TheMessagesClass.ErrorMessage("The Program Will Not Be Available until 6:30 am");
                Application.Current.Shutdown();
            }

            if(gblnLoggedIn == true)
            {
                TheFindAssignedTasksByAssignedEmployeeIDDataSet = TheAssignedTaskClass.FindAssignedTasksByAssignedEmployeeID(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                intNumberOfRecords = TheFindAssignedTasksByAssignedEmployeeIDDataSet.FindAssignedTasksByAssignedEmployeeID.Rows.Count;

                if (intNumberOfRecords > gintNumberOfTasks)
                {
                    gintNumberOfTasks = intNumberOfRecords;
                    UpdateAssignTasksWindow.Visibility = Visibility.Visible;
                }
            }            
        }

        private void mitAddDOTStatus_Click(object sender, RoutedEventArgs e)
        {
            AddDOTStatusWindow.Visibility = Visibility.Visible;
        }


        private void mitAddGPSStatus_Click(object sender, RoutedEventArgs e)
        {
            AddGPSStatusWindow.Visibility = Visibility.Visible;
        }

        private void mitPreventiveMaintenance_Click(object sender, RoutedEventArgs e)
        {
            PreventiveMaintenanceWindow.Visibility = Visibility.Visible;
        }

        private void mitImportMDUDropBuryOrders_Click(object sender, RoutedEventArgs e)
        {
            ImportMDUDropBuryOrdersWindow.Visibility = Visibility.Visible;
        }

        private void mitAddBulkToolsToVehicle_Click(object sender, RoutedEventArgs e)
        {
            AddVehicleBulkToolsWindow.Visibility = Visibility.Visible;
        }

        private void mitEditVehicleBulkTool_Click(object sender, RoutedEventArgs e)
        {
            SelectVehicleBulkToolWindow.Visibility = Visibility.Visible;
        }

        private void mitRemoveEmployeesFromVehicles_Click(object sender, RoutedEventArgs e)
        {
            RemoveEmployeesFromVehiclesWindow.Visibility = Visibility.Visible;
        }

        private void mitProductivityDataEntryReports_Click(object sender, RoutedEventArgs e)
        {
            ProductivityDataEntryReportsWindow.Visibility = Visibility.Visible;
        }

        private void mitFindProject_Click(object sender, RoutedEventArgs e)
        {
            FindProjectsWindow.Visibility = new Visibility();
        }

        private void mitToolAvailability_Click(object sender, RoutedEventArgs e)
        {
            FindAvailableToolsWindow.Visibility = Visibility.Visible;
        }

        private void mitCreateTool_Click(object sender, RoutedEventArgs e)
        {
            CreateToolWindow.Visibility = Visibility.Visible;
        }

        private void mitReportITProblem_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitEditTool_Click(object sender, RoutedEventArgs e)
        {
            EditToolsWindow.Visibility = Visibility.Visible;
        }

        private void mitNeeVehicleProblem_Click(object sender, RoutedEventArgs e)
        {
            NewVehicleProblemWindow.Visibility = Visibility.Visible;
        }

        private void mitUpdateVehicleProblem_Click(object sender, RoutedEventArgs e)
        {
            UpdateVehicleProblemWindow.Visibility = Visibility.Visible;
        }

        private void mitAvailableVehicles_Click(object sender, RoutedEventArgs e)
        {
            AvailableVehiclesWindow.Visibility = Visibility.Visible;
        }

        private void mitSendVehicleToShop_Click(object sender, RoutedEventArgs e)
        {
            SendVehicleToShopWindow.Visibility = Visibility.Visible;
        }

        private void mitOpenVehicleProblems_Click(object sender, RoutedEventArgs e)
        {
            OpenVehicleProblemsWindow.Visibility = Visibility.Visible;
        }

        private void mitVehiclesInShop_Click(object sender, RoutedEventArgs e)
        {
            VehiclesInShopWindow.Visibility = Visibility.Visible;
        }

        private void mitViewWorkTaskStatistics_Click(object sender, RoutedEventArgs e)
        {
            ViewWorkTaskStatisticsWindow.Visibility = Visibility.Visible;
        }

        private void mitImportWorkTask_Click(object sender, RoutedEventArgs e)
        {
            ImportWorkTaskWindow.Visibility = Visibility.Visible;
        }

        private void mitEnterTaskHours_Click(object sender, RoutedEventArgs e)
        {
            EnterApprovedTransactionWindow.Visibility = Visibility.Visible;
        }

        private void mitVehicleProblemHistory_Click(object sender, RoutedEventArgs e)
        {
            VehicleProblemHistoryWindow.Visibility = Visibility.Visible;
        }

        private void mitReportToolProblem_Click(object sender, RoutedEventArgs e)
        {
            ReportToolProblemWindow.Visibility = Visibility.Visible;
        }

        private void mitOpenToolProblems_Click(object sender, RoutedEventArgs e)
        {
            OpenToolProblemsWindow.Visibility = Visibility.Visible;
        }

        private void mitBrokenToolReport_Click(object sender, RoutedEventArgs e)
        {
            BrokenToolReportWindow.Visibility = Visibility.Visible;
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            AssignTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            UpdateAssignTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            MyOriginatingTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitProjectWorkTask_Click(object sender, RoutedEventArgs e)
        {
            EditProjectWorkTaskWindow.Visibility = Visibility.Visible;
        }

        private void mitHistoricalVehicleException_Click(object sender, RoutedEventArgs e)
        {
            HistoricalVehicleExceptionReportWindow.Visibility = Visibility.Visible;
        }

        private void mitAuditReportEmployeeAssignment_Click(object sender, RoutedEventArgs e)
        {
            AuditReportEmployeeAssignmentWindow.Visibility = Visibility.Visible;
        }

        private void mitCurrentToolLocation_Click(object sender, RoutedEventArgs e)
        {
            CurrentToolLocationWindow.Visibility = Visibility.Visible;
        }

        private void mitToolsAssignedToDeactivatedEmployees_Click(object sender, RoutedEventArgs e)
        {
            DeactivateEmployeeToolsWindow.Visibility = Visibility.Visible;
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            SendEmailWindow.Visibility = Visibility.Visible;
        }

        private void mitAddEmployeeToVehicleEmailList_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeToVehicleEmailListWindow.Visibility = Visibility.Visible;
        }

        private void mitRemoveEmployeesFromVehicleEmailList_Click(object sender, RoutedEventArgs e)
        {
            RemoveEmployeesFromVehiclesEmailListWindow.Visibility = Visibility.Visible;
        }

        private void mitAddTrailers_Click(object sender, RoutedEventArgs e)
        {
            AddTrailersWindow.Visibility = Visibility.Visible;
        }

        private void mitAddTrailerCategory_Click(object sender, RoutedEventArgs e)
        {
            AddTrailerCategoryWindow.Visibility = Visibility.Visible;         
        }

        private void mitImportTrailers_Click(object sender, RoutedEventArgs e)
        {
            ImportTrailersWindow.Visibility = Visibility.Visible;
        }

        private void mitEditTrailer_Click(object sender, RoutedEventArgs e)
        {
            EditTrailerWindow.Visibility = Visibility.Visible;
        }

        private void mitAssignTrailer_Click(object sender, RoutedEventArgs e)
        {
            AssignTrailerWindow.Visibility = Visibility.Visible;
        }

        private void mitTrailerRoster_Click(object sender, RoutedEventArgs e)
        {
            TrailerRosterWindow.Visibility = Visibility.Visible;
        }

        private void mitTrailersInYard_Click(object sender, RoutedEventArgs e)
        {
            TrailersInYardWindow.Visibility = Visibility.Visible;
        }

        private void mitDailyTrailerInspection_Click(object sender, RoutedEventArgs e)
        {
            DailyTrailerInspectionWindow.Visibility = Visibility.Visible;
        }

        private void mitCompareEmployeeCrews_Click(object sender, RoutedEventArgs e)
        {
            CompareEmployeeCrewsWindow.Visibility = Visibility.Visible;
        }

        private void mitFindEmployeeProductivityByDateRange_Click(object sender, RoutedEventArgs e)
        {
            FindEmployeeProductivityFootageWindow.Visibility = Visibility.Visible;
        }

        private void mitAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            AddDepartmentWindow.Visibility = Visibility.Visible;
        }

        private void mitWarehouseInventoryStats_Click(object sender, RoutedEventArgs e)
        {
            WarehouseInventoryStatsWindow.Visibility = Visibility.Visible;
        }

        private void mitImportEmployeeHours_Click(object sender, RoutedEventArgs e)
        {
            ImportEmployeeHoursWindow.Visibility = Visibility.Visible;
        }

        private void mitEmployeePunchedVsProductionHours_Click(object sender, RoutedEventArgs e)
        {
            EmployeePunchedVSProductionHoursWindow.Visibility = Visibility.Visible;
        }

        private void mitEmployeeRoster_Click(object sender, RoutedEventArgs e)
        {
            EmployeeRosterWindow.Visibility = Visibility.Visible;
        }

        private void mitImportEmployeePunches_Click(object sender, RoutedEventArgs e)
        {
            ImportEmployeePunchesWindow.Visibility = Visibility.Visible;
        }

        private void mitViewEmployeePunches_Click(object sender, RoutedEventArgs e)
        {
            ViewEmployeePunchesWindow.Visibility = Visibility.Visible;
        }

        private void mitDailyTrailerInspectionReport_Click(object sender, RoutedEventArgs e)
        {
            DailyTrailerInspectionReportWindow.Visibility = Visibility.Visible;  
        }

        private void mitCreateToolCategory_Click(object sender, RoutedEventArgs e)
        {
            CreateToolCategoryWindow.Visibility = Visibility.Visible;
        }

        private void mitBulkToolSignOut_Click(object sender, RoutedEventArgs e)
        {
            BulkToolSignOutWindow.Visibility = Visibility.Visible;
        }

        private void mitBulkToolSignIn_Click(object sender, RoutedEventArgs e)
        {
            SelectBulkToolForSignInWindow.Visibility = Visibility.Visible;
        }

        private void mitManagerProductivityPunched_Click(object sender, RoutedEventArgs e)
        {
            ManagerProductivityPunchedReportWindow.Visibility = Visibility.Visible;
        }

        private void MitEmployeLaborRate_Click(object sender, RoutedEventArgs e)
        {
            EmployeeLaborRateWindow.Visibility = Visibility.Visible;
        }

        private void MitImportITAssets_Click(object sender, RoutedEventArgs e)
        {
            ImportITAssetsWindow.Visibility = Visibility.Visible;
        }

        private void MitChangeITAssetLocation_Click(object sender, RoutedEventArgs e)
        {
            SelectITAssetWindow.Visibility = Visibility.Visible;
        }

        private void MitInvoiceVehicelProblem_Click(object sender, RoutedEventArgs e)
        {
            InvoiceVehicleProblemsWindow.Visibility = Visibility.Visible;
        }

        private void MitInvoicedVehicleProblems_Click(object sender, RoutedEventArgs e)
        {
            VehicleProblemInvoiceReportWindow.Visibility = Visibility.Visible;
        }

        private void MitVehicleInvoicedByDateRange_Click(object sender, RoutedEventArgs e)
        {
            VehicleINvoicesByDateRangeWindow.Visibility = Visibility.Visible;
        }

        private void MitViewVehicleProblem_Click(object sender, RoutedEventArgs e)
        {
            ViewVehicleProblemWindow.Visibility = Visibility.Visible;
        }

        private void MitAddITAsset_Click(object sender, RoutedEventArgs e)
        {
            AddITAssetsWindow.Visibility = Visibility.Visible;
        }

        private void MitRetireITAsset_Click(object sender, RoutedEventArgs e)
        {
            RetireITAssetWindow.Visibility = Visibility.Visible;
        }

        private void MitCurrentITAssets_Click(object sender, RoutedEventArgs e)
        {
            CurrentITAssetsWindow.Visibility = Visibility.Visible;
        }

        private void MitRetiredITAssets_Click(object sender, RoutedEventArgs e)
        {
            RetiredITAssetsWindow.Visibility = Visibility.Visible;
        }

        private void MitJobType_Click(object sender, RoutedEventArgs e)
        {
            AddJobTypeWindow.Visibility = Visibility.Visible;
        }

        private void MitAddVendor_Click(object sender, RoutedEventArgs e)
        {
            ImportVendorsWindow.Visibility = Visibility.Visible;
        }

        private void MitManagerWeeklyVehicleCountReport_Click(object sender, RoutedEventArgs e)
        {
            ManagerWeeklyVehicleCountWindow.Visibility = Visibility.Visible;
        }

        private void MitProductionEmployeeProductivityMeasure_Click(object sender, RoutedEventArgs e)
        {
            ProductionEmployeeProductivityMeasureWindow.Visibility = Visibility.Visible;
        }

        private void MitUpdateWorkTaskProductionValue_Click(object sender, RoutedEventArgs e)
        {
            UpdateWorkTaskProductivityValueWindow.Visibility = Visibility.Visible;
        }

        private void MitPhoneList_Click(object sender, RoutedEventArgs e)
        {
            PhoneListWindow.Visibility = Visibility.Visible;
        }

        private void MitAssignPhoneExtension_Click(object sender, RoutedEventArgs e)
        {
            AssignPhoneExtensionWindow.Visibility = Visibility.Visible;
        }

        private void MitAssignCellPhone_Click(object sender, RoutedEventArgs e)
        {
            AssignCellPhonesWindow.Visibility = Visibility.Visible;
        }

        private void MitAddWOVBillingCodes_Click(object sender, RoutedEventArgs e)
        {
            AddWOVBillingCodesWindow.Visibility = Visibility.Visible;
        }

        private void MitEditWOVBillingCodes_Click(object sender, RoutedEventArgs e)
        {
            EditWOVBillingCodesWindow.Visibility = Visibility.Visible;
        }

        private void MitAddWOVTasks_Click(object sender, RoutedEventArgs e)
        {
            AddWOVTasksWindow.Visibility = Visibility.Visible;
        }

        private void MitEditWOVTasks_Click(object sender, RoutedEventArgs e)
        {
            EditWOVTaskWindow.Visibility = Visibility.Visible;
        }

        private void MitAddTechPayItem_Click(object sender, RoutedEventArgs e)
        {
            AddTechPayItemWindow.Visibility = Visibility.Visible;
        }

        private void MitAddNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            AddVehicleWindow.Visibility = Visibility.Visible;
        }

        private void MitAddVehicleToEmployeeTable_Click(object sender, RoutedEventArgs e)
        {
            AddVehicleToEmployeeTableWindow.Visibility = Visibility.Visible;
        }

        private void MitOpenCellPhoneReport_Click(object sender, RoutedEventArgs e)
        {
            OpenCellPhoneReportWindow.Visibility = Visibility.Visible;
        }

        private void MitDesignEmployeeProductivity_Click(object sender, RoutedEventArgs e)
        {
            DesignEmployeeProductivityWindow.Visibility = Visibility.Visible;
        }

        private void MitDesignProjectInvoicing_Click(object sender, RoutedEventArgs e)
        {
            DesignProjectInvoicingWindow.Visibility = Visibility.Visible;
        }
    }
}
