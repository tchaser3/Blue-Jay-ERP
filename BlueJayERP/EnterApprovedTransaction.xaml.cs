﻿/* Title:           Enter Approved Transactions
 * Date:            7-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the information to be put in the program */


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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EnterApprovedTransaction.xaml
    /// </summary>
    public partial class EnterApprovedTransaction : Window
    {
        //creating the classes
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
        //WorkTaskStatsClass TheWorkTaskStatsClass = new WorkTaskStatsClass();
        ProductivityDataEntryClass TheProductivityDataEntryClass = new ProductivityDataEntryClass();

        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();
        ProjectWorkCompletedDataSet TheEmployeeWorkCompleteDataSet = new ProjectWorkCompletedDataSet();
        ProjectWorkCompletedDataSet TheProjectWorkCompletedDataSet = new ProjectWorkCompletedDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        //FindWorkTaskStatsByTaskIDDataSet TheFindWorkTaskStatsByTaskIDDataSet = new FindWorkTaskStatsByTaskIDDataSet();
        FindProductivityDataEntryByDateDataSet TheFindProductivityDataEntryByDateDataSet = new FindProductivityDataEntryByDateDataSet();

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

        public EnterApprovedTransaction()
        {
            InitializeComponent();
        }
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            bool blnFatalError = false;
            int intEmployeeID;
            decimal decHours;


            try
            {
                if (gblnProjectFound == false)
                {
                    strProjectID = txtEnterProjectID.Text;

                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Project Not Found, A Valid Project Must Be Entered");
                        return;
                    }
                    else
                    {
                        txtProjectName.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                        MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                        MainWindow.gstrAssignedProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].AssignedProjectID;
                        gblnProjectFound = true;
                        intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                        decHours = Convert.ToDecimal(txtEnterHours.Text);

                        blnFatalError = TheProductivityDataEntryClass.InsertProductivityDataEntry(intEmployeeID, MainWindow.gintProjectID, datTransactionDate, decHours, 0, 0);

                        if (blnFatalError == true)
                            throw new Exception();

                        TheFindProductivityDataEntryByDateDataSet = TheProductivityDataEntryClass.FindProductivityDataEntryByDate(datTransactionDate);

                        gintDataEntryTransactionID = TheFindProductivityDataEntryByDateDataSet.FindProductivtyDataEntryByDate[0].TransactionID;

                        gintEmployeeCounter = 0;
                        gintTaskCounter = 0;
                    }
                }

                strLastName = txtEnterLastLame.Text;
                intLength = strLastName.Length;

                if (intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                    intNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count - 1;

                    if (intNumberOfRecords == -1)
                    {
                        TheMessagesClass.InformationMessage("Employee Not Found");
                        return;
                    }
                    else
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].FirstName + " " + TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].LastName);
                        }
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transaction // Enter Last Name Text Change Event " + Ex.Message);
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

        private void mitCheckProject_Click(object sender, RoutedEventArgs e)
        {
            string strProjectID;
            int intRecordsReturned;

            strProjectID = txtEnterProjectID.Text;

            TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

            intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

            if (intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("Project Not Found, A Valid Project Must Be Entered");
                txtEnterLastLame.Text = "";
                return;
            }
            else
            {
                txtProjectName.Text = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                MainWindow.gstrAssignedProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].AssignedProjectID;
                gblnProjectFound = true;
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
                    MainWindow.gstrFirstName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].FirstName;
                    MainWindow.gstrLastName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].LastName;
                    MainWindow.gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].EmployeeID;
                    btnAddEmployee.IsEnabled = true;

                    if (gblnCrewIDSet == false)
                    {
                        txtCrewID.Text = MainWindow.gstrLastName;
                        gblnCrewIDSet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transaction // cboSelectEmployee Event" + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheDataValidationClass.VerifyDoubleData(txtEnterHours.Text);
                if (blnFatalError == true)
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transactions // Add Employee Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

            try
            {
                mitRemoveEmployee.IsEnabled = false;
                mitRemoveTransactions.IsEnabled = true;

                intNumberOfRecords = TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Count - 1;

                if (intNumberOfRecords == -1)
                {
                    TheMessagesClass.ErrorMessage("There Are No Employees Assigned to this Project");
                    return;
                }

                strValueForValidation = txtEnterHours.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
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
                if (blnThereIsAProblem == true)
                {
                    strErrorMessage += "The Footage or Pieces is not an Integer\n";
                    blnFatalError = true;
                }
                else
                {
                    intFootagePieces = Convert.ToInt32(strValueForValidation);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if (gblnHoursEntered == true)
                {
                    gdecHours = 0;
                }
                
                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    ProjectWorkCompletedDataSet.workcompletedRow NewWorkRow = TheProjectWorkCompletedDataSet.workcompleted.NewworkcompletedRow();

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
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transaction // Add Hours " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectTask.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWorkTaskID = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;
                MainWindow.gstrWorkTask = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTask;
            }
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datTransactionDate;
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
                if (blnFatalError == true)
                {
                    strErrorMessage += "The Date is not a Date\n";
                    blnFatalError = true;
                }
                if (txtCrewID.Text == "")
                {
                    strErrorMessage += "Crew ID Was Not Entered\n";
                    blnFatalError = true;
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                datTransactionDate = Convert.ToDateTime(txtTransactionDate.Text);
                strCrewID = txtCrewID.Text;

                intNumberOfRecords = TheProjectWorkCompletedDataSet.workcompleted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
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

                ResetForm();

                TheMessagesClass.InformationMessage("The Project Information Has Been Saved");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transaction // Process Menu Item " + Ex.Message);

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

            if (intNumberOfRecords == -1)
            {
                TheMessagesClass.ErrorMessage("The Task Was Not Found");
                return;
            }

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
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

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gdecHours = TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].Hours;

                    if (TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].TransactionID == gintTransactionID)
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transaction // Menu Item Remove Employee " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if (intSelectedIndex > -1)
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transactions // Grid Selection " + Ex.Message);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Enter Approved Transactions // Menu Item Remove Transaction " + Ex.Message);

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

        private void txtEnterProjectID_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //gblnProjectFound = false;
                //btnAddEmployee.IsEnabled = false;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Project Labor // Enter Project ID Text Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
