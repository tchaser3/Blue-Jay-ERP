/* Title:           Add Project Labor
 * Date:            2-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the add Project labor form */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using ProjectsDLL;
using EmployeeCrewAssignmentDLL;
using EmployeeLaborRateDLL;
using EmployeeProjectAssignmentDLL;
using ProjectTaskDLL;
using WorkTaskDLL;
using DataValidationDLL;
using WorkTaskStatsDLL;
using ProductivityDataEntryDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddProjectLabor.xaml
    /// </summary>
    public partial class AddProjectLabor : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        EmployeeLaborRateClass TheEmployeeLaborRateClass = new EmployeeLaborRateClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WorkTaskStatsClass TheWorkTaskStatsClass = new WorkTaskStatsClass();
        ProductivityDataEntryClass TheProductivityDataEntryClass = new ProductivityDataEntryClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //setting up the data
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        ProjectWorkCompletedDataSet TheEmployeeWorkCompleteDataSet = new ProjectWorkCompletedDataSet();
        ProjectWorkCompletedDataSet TheProjectWorkCompletedDataSet = new ProjectWorkCompletedDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindWorkTaskStatsByTaskIDDataSet TheFindWorkTaskStatsByTaskIDDataSet = new FindWorkTaskStatsByTaskIDDataSet();
        FindProductivityDataEntryByDateDataSet TheFindProductivityDataEntryByDateDataSet = new FindProductivityDataEntryByDateDataSet();
        FindEmployeeHoursOverDateRangeDataSet TheFindEmployeeHoursOverADateRangeDataSet = new FindEmployeeHoursOverDateRangeDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindEmployeeByLastNameEndDateDataSet TheFindEmployeeByLastNameEndDateDataSet = new FindEmployeeByLastNameEndDateDataSet();

        //setting global variables
        bool gblnProjectFound;
        decimal gdecTotalHours;
        bool gblnCrewIDSet;
        bool gblnHoursEntered;
        bool gblnRecordDeleted;
        decimal gdecHours;
        int gintTransactionID;
        int gintDataEntryTransactionID;
        int gintEmployeeCounter;
        int gintTaskCounter;

        public AddProjectLabor()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
            this.Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void txtEnterLastLame_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strProjectID;
            int intRecordsReturned;
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate = DateTime.Now;
            DateTime datTodaysDate = DateTime.Now;
            bool blnFatalError = false;
            int intEmployeeID;
            decimal decHours;
            DateTime datEndDate;


            try
            {
                datEndDate = TheDateSearchClass.SubtractingDays(datTodaysDate, 21);

                blnFatalError = TheDataValidationClass.VerifyDateData(txtTransactionDate.Text);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is not a Date");
                    return;
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(txtTransactionDate.Text);
                }
                if(datTransactionDate > datTodaysDate)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is in the Future");
                    return;
                }

                if (gblnProjectFound == false)
                {
                    strProjectID = txtEnterProjectID.Text;

                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strProjectID);

                        intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            TheMessagesClass.ErrorMessage("Project Not Found, A Valid Project Must Be Entered");
                            return;
                        }
                        else
                        {
                            MainWindow.gintProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                        }
                    }
                    else
                    {
                        MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    }

                    TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(MainWindow.gintProjectID);

                    MainWindow.gstrAssignedProjectID = strProjectID;
                    
                    txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
                    
                    gblnProjectFound = true;
                    intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                    decHours = Convert.ToDecimal(txtEnterHours.Text);

                    blnFatalError = TheProductivityDataEntryClass.InsertProductivityDataEntry(intEmployeeID, MainWindow.gintProjectID, datTodaysDate, decHours, 0, 0);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindProductivityDataEntryByDateDataSet = TheProductivityDataEntryClass.FindProductivityDataEntryByDate(datTodaysDate);

                    gintDataEntryTransactionID = TheFindProductivityDataEntryByDateDataSet.FindProductivtyDataEntryByDate[0].TransactionID;

                    gintEmployeeCounter = 0;
                    gintTaskCounter = 0;
                    
                }

                strLastName = txtEnterLastLame.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheFindEmployeeByLastNameEndDateDataSet = TheEmployeeClass.FindEmployeeByLastNameEndDate(strLastName, datEndDate);

                    intNumberOfRecords = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate.Rows.Count - 1;

                    if(intNumberOfRecords == -1)
                    {
                        TheMessagesClass.InformationMessage("Employee Not Found");
                        return;
                    }
                    else
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intCounter].FirstName + " " + TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intCounter].LastName);
                        }
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Enter Last Name Text Change Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables

            gblnRecordDeleted = false;
            gblnProjectFound = false;
            btnAddEmployee.IsEnabled = false;
            btnAddHours.IsEnabled = false;
            gdecTotalHours = 0;
            gblnCrewIDSet = false;
            
        }

        private void txtEnterProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                gblnProjectFound = false;
                btnAddEmployee.IsEnabled = false;
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Enter Project ID Text Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitCheckProject_Click(object sender, RoutedEventArgs e)
        {
            string strProjectID;
            int intRecordsReturned;

            if (gblnProjectFound == false)
            {
                strProjectID = txtEnterProjectID.Text;

                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Project Not Found, A Valid Project Must Be Entered");
                        return;
                    }
                    else
                    {
                        MainWindow.gintProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                    }
                }
                else
                {
                    MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                }

                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(MainWindow.gintProjectID);

                MainWindow.gstrAssignedProjectID = strProjectID;

                txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;
                mitRemoveTransactions.IsEnabled = false;
                mitRemoveEmployee.IsEnabled = true;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gstrFirstName = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intSelectedIndex].FirstName;
                    MainWindow.gstrLastName = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intSelectedIndex].LastName;
                    MainWindow.gintEmployeeID = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intSelectedIndex].EmployeeID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);
                    
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeType == "CONTRACTOR")
                    {
                        TheMessagesClass.ErrorMessage("You Have Selected a Contractor, Please Select Again");
                        cboSelectEmployee.SelectedIndex = 0;
                        txtEnterLastLame.Focus();
                        return;
                    }

                    btnAddEmployee.IsEnabled = true;

                    if(gblnCrewIDSet == false)
                    {
                        txtCrewID.Text = MainWindow.gstrLastName;
                        gblnCrewIDSet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // cboSelectEmployee Event" + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheDataValidationClass.VerifyDoubleData(txtEnterHours.Text);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Hours is not Numeric");
                    return;
                }

                gdecHours = Convert.ToDecimal(txtEnterHours.Text);

                //adding the record
                ProjectWorkCompletedDataSet.workcompletedRow NewWorkRow = TheEmployeeWorkCompleteDataSet.workcompleted.NewworkcompletedRow();

                NewWorkRow.EmployeeID = MainWindow.gintEmployeeID;
                NewWorkRow.FirstName = MainWindow.gstrFirstName;
                NewWorkRow.LastName = MainWindow.gstrLastName;
                NewWorkRow.ProjectID = MainWindow.gintProjectID;
                NewWorkRow.AssignedProjectID = MainWindow.gstrAssignedProjectID;
                NewWorkRow.TaskID = 0;
                NewWorkRow.WorkTask = "";
                NewWorkRow.Hours = gdecHours;
                NewWorkRow.FootagePieces = 0;

                TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Add(NewWorkRow);

                dgrResults.ItemsSource = TheEmployeeWorkCompleteDataSet.workcompleted;

                txtEnterLastLame.Text = "";
                btnAddHours.IsEnabled = true;
                txtEnterFootage.Text = "0";
                gdecTotalHours += gdecHours;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
                gblnHoursEntered = false;
                txtEnterLastLame.Focus();
                gintEmployeeCounter++;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Add Employee Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private double CalculatePropability(double douMean, double douHoursEntered)
        {
            double douPropability = 0;
            double douFirstCalcuation;
            double douSecondCalculation;
            double douThirdCalculation = 1;
            int intCounter;
            int intFactorial;

            try
            {


                douFirstCalcuation = Math.Pow(2.71836, douMean * -1);
                douSecondCalculation = Math.Pow(douMean, douHoursEntered);
                douThirdCalculation = douHoursEntered;

                intFactorial = Convert.ToInt32(douHoursEntered);


                for (intCounter = 1; intCounter <= intFactorial; intCounter++)
                {
                    douThirdCalculation = douThirdCalculation * intCounter;
                }

                douPropability = (douFirstCalcuation * douSecondCalculation) / douThirdCalculation;
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Calculate Propability " + Ex.Message);
            }

            return douPropability;
        }
        private bool CheckEmployeeTotalHours(int intEmployeeID, decimal decHoursEntered)
        {
            bool blnOverHours = false;
            int intCounter;
            int intNumberOfRecords;
            decimal decTotalHours = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                datStartDate = Convert.ToDateTime(txtTransactionDate.Text);
                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = datStartDate;

                TheFindEmployeeHoursOverADateRangeDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeHoursOverDateRange(intEmployeeID, datStartDate, datEndDate);

                decTotalHours = decHoursEntered;

                intNumberOfRecords = TheFindEmployeeHoursOverADateRangeDataSet.FindEmployeeHoursOverDateRange.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        decTotalHours += TheFindEmployeeHoursOverADateRangeDataSet.FindEmployeeHoursOverDateRange[intCounter].TotalHours;
                    }
                }

                if (decTotalHours > 16)
                {
                    blnOverHours = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Check Employee Total Hours " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnOverHours = true;
            }

            return blnOverHours;
        }
        private void btnAddHours_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intFootagePieces = 0;
            int intRecordsReturned = 0;
            double douProbability;
            double douMean;
            double douHoursEntered;
            bool blnOverHours;

            try
            {
                mitRemoveEmployee.IsEnabled = false;
                mitRemoveTransactions.IsEnabled = true;

                intNumberOfRecords = TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Count - 1;

                if(intNumberOfRecords == -1)
                {
                    TheMessagesClass.ErrorMessage("There Are No Employees Assigned to this Project");
                    return;
                }
                if(cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task was not Selected\n";
                }
                strValueForValidation = txtEnterHours.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    strErrorMessage += "Total Hours is not Numeric\n";
                    blnFatalError = true;
                }
                else
                {
                    gdecHours = Convert.ToDecimal(strValueForValidation);
                }
                strValueForValidation = txtEnterFootage.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    strErrorMessage += "The Footage or Pieces is not an Integer\n";
                    blnFatalError = true;
                }
                else
                {
                    intFootagePieces = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtTransactionDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date is not a Date\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnHoursEntered == true)
                {
                    gdecHours = 0;
                }

                TheFindWorkTaskStatsByTaskIDDataSet = TheWorkTaskStatsClass.FindWorkTaskStatsByTaskID(MainWindow.gintWorkTaskID);

                intRecordsReturned = TheFindWorkTaskStatsByTaskIDDataSet.FindWorkTaskStatsByWorkTaskID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    douMean = Convert.ToDouble(TheFindWorkTaskStatsByTaskIDDataSet.FindWorkTaskStatsByWorkTaskID[0].TaskMean);
                    douHoursEntered = Convert.ToDouble(gdecHours);

                    douProbability = CalculatePropability(douMean, douHoursEntered);


                    if (douProbability < .0001)
                    {
                        TheMessagesClass.ErrorMessage("The Hours Entered Are Outside Expected Range\nPlease Return Return Sheet To Manager");
                        return;
                    }
                }

                
                
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnOverHours = CheckEmployeeTotalHours(TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].EmployeeID, gdecHours);

                    if(blnOverHours == true)
                    {
                        TheMessagesClass.ErrorMessage(TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].FirstName + " " + TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].LastName + " Has Worked Over 16 Hours\nPlease Return Sheet To Manager");

                        return;
                    }

                    ProjectWorkCompletedDataSet.workcompletedRow NewWorkRow =TheProjectWorkCompletedDataSet.workcompleted.NewworkcompletedRow();

                    NewWorkRow.AssignedProjectID = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].AssignedProjectID;
                    NewWorkRow.EmployeeID = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].EmployeeID;
                    NewWorkRow.FirstName = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].FirstName;
                    NewWorkRow.LastName = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].LastName;
                    NewWorkRow.ProjectID = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].ProjectID;
                    NewWorkRow.TaskID = MainWindow.gintWorkTaskID;
                    NewWorkRow.WorkTask = MainWindow.gstrWorkTask;
                    NewWorkRow.Hours = gdecHours;
                    NewWorkRow.FootagePieces = intFootagePieces;

                    TheProjectWorkCompletedDataSet.workcompleted.Rows.Add(NewWorkRow);
                    gblnHoursEntered = true;

                    txtEnterFootage.Text = "";
                    txtEnterTask.Text = "";

                    txtEnterTask.Focus();
                }


                gintTaskCounter++;
                dgrResults.ItemsSource = TheProjectWorkCompletedDataSet.workcompleted;
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Add Hours " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectTask.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWorkTaskID = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;
                MainWindow.gstrWorkTask = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTask;

                if(MainWindow.gintProjectID == 104330)
                {
                    if(MainWindow.gintWorkTaskID != 1230)
                    {
                        TheMessagesClass.ErrorMessage("You Must Use BJC1 - NON-PRODUCTIVE TIME");
                        cboSelectTask.SelectedIndex = 0;
                        return;
                    }
                }
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datTransactionDate;
            DateTime datTodaysDate = DateTime.Now;
            int intEmployeeID;
            int intProjectID;
            int intWorkTaskID;
            decimal decTotalHours;
            int intFootagePieces;
            string strCrewID;
            string strErrorMessage = "";

            try
            {
                mitRemoveTransactions.IsEnabled = true;
                mitRemoveEmployee.IsEnabled = false;

                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(txtTransactionDate.Text);
                if(blnFatalError == true)
                {
                    strErrorMessage += "The Date is not a Date\n";
                    blnFatalError = true;
                }
                if(txtCrewID.Text == "")
                {
                    strErrorMessage += "Crew ID Was Not Entered\n";
                    blnFatalError = true;
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                datTransactionDate = Convert.ToDateTime(txtTransactionDate.Text);

                if(datTransactionDate > datTodaysDate)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is in the Future");
                    return;
                }
                strCrewID = txtCrewID.Text;

                intNumberOfRecords = TheProjectWorkCompletedDataSet.workcompleted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheProjectWorkCompletedDataSet.workcompleted[intCounter].EmployeeID;
                    intProjectID = TheProjectWorkCompletedDataSet.workcompleted[intCounter].ProjectID;
                    intWorkTaskID = TheProjectWorkCompletedDataSet.workcompleted[intCounter].TaskID;
                    decTotalHours = TheProjectWorkCompletedDataSet.workcompleted[intCounter].Hours;
                    intFootagePieces = TheProjectWorkCompletedDataSet.workcompleted[intCounter].FootagePieces;

                    //first insert
                    blnFatalError = TheEmployeeProjectAssignmentClass.InsertEmployeeProjectAssignment(intEmployeeID, intProjectID, intWorkTaskID, datTransactionDate, decTotalHours);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProjectTaskClass.InsertProjectTask(intProjectID, intEmployeeID, intWorkTaskID, Convert.ToDecimal(intFootagePieces), datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheEmployeeCrewAssignmentClass.InsertEmployeeCrewAssignment(strCrewID, intEmployeeID, intProjectID, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProductivityDataEntryClass.UpdateProductivityDataEntryHoursTasks(gintDataEntryTransactionID, gintEmployeeCounter, gintTaskCounter);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Add Project Labor // Project Labor Has Been Added " + txtEnterProjectID.Text);

                if (blnFatalError == true)
                    throw new Exception();

                ResetForm();

                TheMessagesClass.InformationMessage("The Project Information Has Been Saved");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ResetForm()
        {
            txtCrewID.Text = "";
            txtEnterFootage.Text = "";
            txtEnterHours.Text = "";
            txtEnterLastLame.Text = "";
            txtEnterProjectID.Text = "";
            txtProjectName.Text = "";
            txtTotalHours.Text = "";
            cboSelectEmployee.Items.Clear();
            cboSelectTask.Items.Clear();
            TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Clear();
            TheProjectWorkCompletedDataSet.workcompleted.Rows.Clear();
            dgrResults.ItemsSource = TheEmployeeWorkCompleteDataSet.workcompleted;
            gblnCrewIDSet = false;
            gblnProjectFound = false;
            gdecTotalHours = 0;
            mitRemoveEmployee.IsEnabled = true;
            mitRemoveTransactions.IsEnabled = false;
            txtEnterTask.Text = "";
            gblnHoursEntered = false;
            mitRemoveTransactions.IsEnabled = false;
        }

        private void mitResetWindow_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void txtEnterTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intCounter;
            int intNumberOfRecords;

            strWorkTask = txtEnterTask.Text;
            TheFindWorkTaskByKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);
            cboSelectTask.Items.Clear();
            cboSelectTask.Items.Add("Select Task");

            intNumberOfRecords = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count - 1;

            if(intNumberOfRecords == -1)
            {
                TheMessagesClass.ErrorMessage("The Task Was Not Found");
                return;
            }

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectTask.Items.Add(TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
            }

            cboSelectTask.SelectedIndex = 0;
        }

        private void mitRemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            //gblnRecordDeleted = true;

            try
            {
                dgrResults.SelectedIndex = -1;

                intNumberOfRecords = TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gdecHours = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].Hours;

                    if(TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].TransactionID == gintTransactionID)
                    {
                        TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].Delete();
                        intCounter -= 1;
                        intNumberOfRecords -= 1;
                        gdecTotalHours -= gdecHours;
                        gblnRecordDeleted = false;
                        dgrResults.SelectedIndex = -1;
                        
                    }
                }

                dgrResults.ItemsSource = TheEmployeeWorkCompleteDataSet.workcompleted;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Menu Item Remove Employee " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrResults_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if(intSelectedIndex > -1)
                {
                    if (gblnRecordDeleted == false)
                    {
                        //setting local variable
                        dataGrid = dgrResults;
                        selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                        TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                        strTransactionID = ((TextBlock)TransactionID.Content).Text;
                        gblnRecordDeleted = true;

                        //find the record
                        gintTransactionID = Convert.ToInt32(strTransactionID);
                    }
                    else
                    {
                        gblnRecordDeleted = false;
                    }
                }
                

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitRemoveTransactions_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            gblnRecordDeleted = true;

            try
            {
                dgrResults.SelectedIndex = -1;

                intNumberOfRecords = TheProjectWorkCompletedDataSet.workcompleted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gdecHours = TheProjectWorkCompletedDataSet.workcompleted[intCounter].Hours;

                    if (TheProjectWorkCompletedDataSet.workcompleted[intCounter].TransactionID == gintTransactionID)
                    {
                        TheProjectWorkCompletedDataSet.workcompleted[intCounter].Delete();
                        intCounter -= 1;
                        intNumberOfRecords -= 1;
                        gdecTotalHours -= gdecHours;
                        gblnRecordDeleted = false;
                        dgrResults.SelectedIndex = -1;
                    }
                }

                dgrResults.ItemsSource = TheProjectWorkCompletedDataSet.workcompleted;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Menu Item Remove Transaction " + Ex.Message);

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
