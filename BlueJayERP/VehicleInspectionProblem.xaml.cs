/* Title:           Vehicle Inspection Problem
 * Date:            4-10-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to report a vehicle problem*/

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
using InspectionsDLL;
using VehicleProblemsDLL;
using NewEventLogDLL;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleInspectionProblem.xaml
    /// </summary>
    public partial class VehicleInspectionProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();
        FindOpenVehicleMainProblemsByVehicleIDDataSet TheFindOpenVehicleMainProblemsByVehicleIDDataSet = new FindOpenVehicleMainProblemsByVehicleIDDataSet();
        ExistingOpenProblemsDataSet TheExistingOpenProblemsDataSet = new ExistingOpenProblemsDataSet();
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindVehicleInspectionProblemByProblemIDDataSet TheFindVehicleInspectionProblemByProblemIDDataSet = new FindVehicleInspectionProblemByProblemIDDataSet();

        int gintProblemID;
        bool gblnNewWorkOrder;
        bool gblnMultipleOrders;
        int gintMultipleSelectedIndex;
        int gintManagerID;
        int gintFleetEmployeeID;

        public VehicleInspectionProblem()
        {
            InitializeComponent();
        }
        private void SetReadOnlyControls(bool blnValueBoolean)
        {
            cboMultipleProblems.IsEnabled = blnValueBoolean;
            cboSelectEmployee.IsEnabled = blnValueBoolean;
            cboSelectEmployee.IsEnabled = blnValueBoolean;
            txtFleetNotes.IsEnabled = blnValueBoolean;
            txtManagerNotes.IsEnabled = blnValueBoolean;
            txtInspectionNotes.IsEnabled = blnValueBoolean;
        }
        private bool CharacterCheck(string strValueForValidation)
        {
            bool blnItemFailed = false;
            char[] chaWordToTest;
            int intLength;
            int intCounter;
            int intPatternCounter = 0;
            char chaTestingCharacter = '*';

            chaWordToTest = strValueForValidation.ToCharArray();
            intLength = chaWordToTest.Length - 1;

            for (intCounter = 0; intCounter <= intLength; intCounter++)
            {
                if (chaTestingCharacter != chaWordToTest[intCounter])
                {
                    chaTestingCharacter = chaWordToTest[intCounter];
                    intPatternCounter = 0;
                }
                else if(chaTestingCharacter == chaWordToTest[intCounter])
                {
                    intPatternCounter += 1;
                    if(intPatternCounter > 3)
                    {
                        blnItemFailed = true;
                    }
                }
            }


            return blnItemFailed;
        }
        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //this will save the transaction
            string strInspectionNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datTransactionDate = DateTime.Now;
            //int intLength;
            string strManagerNotes;
            string strFleetNotes;
            bool blnThereIsaProblem = false;

            try
            {
                MainWindow.gstrVehicleProblem = txtInspectionNotes.Text;
                blnThereIsaProblem = CharacterCheck(MainWindow.gstrVehicleProblem);
                if(blnThereIsaProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "There Is a Recurring Character that is displaying, Invalid Entry\n";
                }
                strManagerNotes = txtManagerNotes.Text;
                if(strManagerNotes.Length < 20)
                {
                    blnFatalError = true;
                    strErrorMessage += "Manager Notes Entry is not Long Enough\n";
                }
                strFleetNotes = txtFleetNotes.Text;
                if (strFleetNotes.Length < 20)
                {
                    blnFatalError = true;
                    strErrorMessage += "Fleet Notes Entry is not Long Enough\n";
                }
                if (cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Fleet Employee Was Not Selected\n";
                }
                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Manager Was Not Selected\n";
                }
                if (gintMultipleSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Multiple Orders Was Not Selected\n";
                }
                if (gblnNewWorkOrder == true)
                {
                    if (MainWindow.gstrVehicleProblem == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vehicle Problem Was Not Entered\n";
                    }
                }

                strInspectionNotes = txtInspectionNotes.Text;
                if (strInspectionNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Inspection Notes Were Not Entered\n";
                }
                else
                {
                    blnThereIsaProblem = CharacterCheck(strInspectionNotes);
                    if(blnThereIsaProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "There is a recurring character in the Inspection Notes\n";
                    }
                }
                
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strManagerNotes = "MANAGER NOTES: " + strManagerNotes;
                strFleetNotes = "FLEET NOTES: " + strFleetNotes;

                if (gblnNewWorkOrder == true)
                {
                    //iserting into table
                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gstrVehicleProblem);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(datTransactionDate);

                    gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;

                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strManagerNotes, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strFleetNotes, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }              

                blnFatalError = TheInspectionClass.InsertVehicleInspectionProblem(MainWindow.gintVehicleID, MainWindow.gintInspectionID, MainWindow.gstrInspectionType, MainWindow.gstrVehicleProblem, MainWindow.gintOdometerReading, MainWindow.gblnServicable, strInspectionNotes, gintManagerID , gintFleetEmployeeID, strManagerNotes, strFleetNotes, gintProblemID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Vehicle Has Been Updated");

                if (gblnMultipleOrders == true)
                {
                    txtInspectionNotes.Text = "";
                    txtManagerNotes.Text = "";
                    txtFleetNotes.Text = "";
                    cboMultipleProblems.SelectedIndex = 0;
                    SetReadOnlyControls(false);

                }
                else if (gblnMultipleOrders == false)
                {

                    Close();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Inspection Problem // Save Menu Item " + Ex.Message);

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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting up to load the combo box
            gintProblemID = 0;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //HideTextBoxes();

                cboMultipleProblems.Items.Add("Select");
                cboMultipleProblems.Items.Add("Yes");
                cboMultipleProblems.Items.Add("No");
                cboMultipleProblems.SelectedIndex = 0;

                ExistingOpenProblemsDataSet.openordersRow FirstRow = TheExistingOpenProblemsDataSet.openorders.NewopenordersRow();

                FirstRow.ProblemID = -1;
                FirstRow.Problem = "NEW WORK ORDER";

                TheExistingOpenProblemsDataSet.openorders.Rows.Add(FirstRow);

                TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                intNumberOfRecords = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID.Rows.Count - 1;

                MainWindow.TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ExistingOpenProblemsDataSet.openordersRow NewOrderRow = TheExistingOpenProblemsDataSet.openorders.NewopenordersRow();

                        NewOrderRow.Problem = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID[intCounter].Problem;
                        NewOrderRow.ProblemID = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID[intCounter].ProblemID;

                        TheExistingOpenProblemsDataSet.openorders.Rows.Add(NewOrderRow);
                    }
                }

                cboSelectManager.Items.Add("Select Manager");

                intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    string strFirstName = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FirstName;
                    string strLastName = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].LastName;

                    cboSelectManager.Items.Add(strFirstName + " " + strLastName);
                }

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("WAREHOUSE");
                cboSelectEmployee.Items.Add("Select Fleet Employee");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " " + TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName);
                }

                cboSelectEmployee.SelectedIndex = 0;
                cboSelectManager.SelectedIndex = 0;
                dgrWorkOrders.ItemsSource = TheExistingOpenProblemsDataSet.openorders;
                SetReadOnlyControls(false);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Inspection Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
       /* private void HideTextBoxes()
        {
            txtInspectionNotes.Visibility = Visibility.Hidden;
            txtVehicleProblem.Visibility = Visibility.Hidden;
        } */

        private void dgrWorkOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid OpenOrderGrid;
            DataGridRow OpenOrderRow;
            DataGridCell ProblemID;
            string strProblemID;
            int intRecordsReturned;          

            try
            {
                intSelectedIndex = dgrWorkOrders.SelectedIndex;
                //HideTextBoxes();

                if (intSelectedIndex > -1)
                {
                    SetReadOnlyControls(true);
                    OpenOrderGrid = dgrWorkOrders;
                    OpenOrderRow = (DataGridRow)OpenOrderGrid.ItemContainerGenerator.ContainerFromIndex(OpenOrderGrid.SelectedIndex);
                    ProblemID = (DataGridCell)OpenOrderGrid.Columns[0].GetCellContent(OpenOrderRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                   
                    gintProblemID = Convert.ToInt32(strProblemID);

                    if (intSelectedIndex > 0)
                    {
                        TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(gintProblemID);

                        MainWindow.gstrVehicleProblem = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;

                        txtInspectionNotes.Text = MainWindow.gstrVehicleProblem;

                        txtInspectionNotes.IsReadOnly = true;
                    }

                    if (gintProblemID == -1)
                    {
                        cboSelectEmployee.IsEnabled = true;
                        cboSelectManager.IsEnabled = true;
                        txtFleetNotes.Text = "";
                        txtManagerNotes.Text = "";
                        txtInspectionNotes.Text = "";
                        txtInspectionNotes.IsReadOnly = false;
                        gblnNewWorkOrder = true;
                        txtFleetNotes.IsReadOnly = false;
                        txtManagerNotes.IsReadOnly = false;
                    }
                    else if (gintProblemID > -1)
                    {
                        gblnNewWorkOrder = false;

                        TheFindVehicleInspectionProblemByProblemIDDataSet = TheInspectionClass.FindVehicleInspectionProblemByProblemID(gintProblemID);

                        intRecordsReturned = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            gintManagerID = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].ManagerID;
                            gintFleetEmployeeID = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].FleetEmployeeID;
                            txtFleetNotes.Text = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].FleetEmployeeNotes;
                            txtManagerNotes.Text = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].ManagerNotes;

                            LoadComboBoxBoxes(gintManagerID, gintFleetEmployeeID);

                            //txtInspectionNotes.IsReadOnly = true;
                            //txtManagerNotes.IsReadOnly = true;
                            //txtFleetNotes.IsReadOnly = true;
                            //cboSelectEmployee.IsEnabled = false;
                            //cboSelectManager.IsEnabled = false;
                        }
                    }

                    
                    txtInspectionNotes.Visibility = Visibility.Visible;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Inspection Problem // Open Problems Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadComboBoxBoxes(int intManagerID, int intFleetEmployeeID)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].employeeID == gintManagerID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectManager.SelectedIndex = intSelectedIndex;

            intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].EmployeeID == gintFleetEmployeeID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectEmployee.SelectedIndex = intSelectedIndex;
        }
        private void cboMultipleProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gintMultipleSelectedIndex = cboMultipleProblems.SelectedIndex;

            if (gintMultipleSelectedIndex == 1)
                gblnMultipleOrders = true;
            else if (gintMultipleSelectedIndex == 2)
                gblnMultipleOrders = false;
        }

        private void CboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintManagerID = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
            }
        }

        private void CboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintFleetEmployeeID = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].EmployeeID;
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
