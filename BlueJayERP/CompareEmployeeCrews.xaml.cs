/* Title:           Compare Employee Crews
 * Date:            9-27-18
 * Author:          Terry Holmes
 * 
 * Description:     This form will compare the Employee Crews */

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
using NewEmployeeDLL;
using EmployeeCrewAssignmentDLL;
using DataValidationDLL;
using DateSearchDLL;
using EmployeeProductivityStatsDLL;
using ProjectTaskDLL;
using EmployeeProjectAssignmentDLL;
using WorkTaskDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for CompareEmployeeCrews.xaml
    /// </summary>
    public partial class CompareEmployeeCrews : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();

        //setting up the data
        FindEmployeeCrewAssignmentComboBoxDataSet TheFindEmployeeCrewAssignmentComboBoxDataSet = new FindEmployeeCrewAssignmentComboBoxDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindDetailedEmployeeCrewAssignmentByCrewIDDataSet TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet = new FindDetailedEmployeeCrewAssignmentByCrewIDDataSet();
        CompareCrewEmployeesDataSet TheCompareCrewEmployeesDataSet = new CompareCrewEmployeesDataSet();
        FindEmployeeProjectHoursOverDateRangeDataSet TheFindEmployeeProjectHoursOverDateRangeDataSet = new FindEmployeeProjectHoursOverDateRangeDataSet();
        FindWorkTaskByWorkTaskDataSet TheFindWorkTaskByWorkTaskDataSet = new FindWorkTaskByWorkTaskDataSet();
        CompareCrewTasksDataSet TheCompareCrewTasksDataSet = new CompareCrewTasksDataSet();
        FindEmployeeTaskHoursDataSet TheFindEmployeeTaskHoursDataSet = new FindEmployeeTaskHoursDataSet();
        FindEmployeeProjectTaskFootageDataSet TheFindEmployeeProjectTaskFootageDataSet = new FindEmployeeProjectTaskFootageDataSet();
        FindEmployeeProjectTaskStatsDataSet TheFindEmployeeProjectTaskStatsDataSet = new FindEmployeeProjectTaskStatsDataSet();
        CrewMembersDataSet TheCrewMembersDataSet = new CrewMembersDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        int gintEmployeeCounter;
        int gintEmployeeUpperLimit;
        int gintTaskCounter;
        int gintTaskUpperLimit;

        public CompareEmployeeCrews()
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

        private void mitFindCrews_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //data validation
                TheCompareCrewEmployeesDataSet.employees.Rows.Clear();
                TheCrewMembersDataSet.crewmembers.Rows.Clear();
                strValueForValidation = txtStartDate.Text;
                dgrResults.ItemsSource = TheCompareCrewEmployeesDataSet.employees;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    strErrorMessage += "The Start Date is not a Date\n";
                    blnFatalError = true;
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                //data validation
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    strErrorMessage += "The End Date is not a Date\n";
                    blnFatalError = true;
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is After the End Date");
                        return;
                    }
                }

                cboSelectCrew.Items.Clear();
                cboSelectCrew.Items.Add("Select Crew");

                TheFindEmployeeCrewAssignmentComboBoxDataSet = TheEmployeeCrewAssignmentClass.FindEmployeeCrewAssignmentComboBox(gdatStartDate, gdatEndDate);

                intNumberOfRecords = TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectCrew.Items.Add(TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox[intCounter].CrewID);
                    }
                }

                cboSelectCrew.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Compare Employee Crews // Find Crews Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboSelectCrew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strCrewID;
            int intEmployeeCounter;
            int intEmployeeID;
            int intProjectID;
            int intCounter;
            int intNumberOfRecords;
            bool blnItemFound;
            bool blnCrewMemberFound;


            try
            {
                intSelectedIndex = cboSelectCrew.SelectedIndex - 1;
                TheCompareCrewTasksDataSet.worktasks.Rows.Clear();
                cboSelectTask.Items.Clear();
                cboSelectTask.Items.Add("Select Work Task");

                if(intSelectedIndex > -1)
                {
                    strCrewID = TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox[intSelectedIndex].CrewID;

                    TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet= TheEmployeeCrewAssignmentClass.FindDetailedEmployeeCrewAssignmentByCrewID(strCrewID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID.Rows.Count - 1;
                    gintEmployeeCounter = TheCompareCrewEmployeesDataSet.employees.Rows.Count;
                    gintEmployeeUpperLimit = gintEmployeeCounter - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].EmployeeID;
                            intProjectID = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].ProjectID;
                            blnItemFound = false;
                            blnCrewMemberFound = false;

                            if (gintEmployeeCounter > 0)
                            {
                                for (intEmployeeCounter = 0; intEmployeeCounter <= gintEmployeeUpperLimit; intEmployeeCounter++)
                                {
                                    if (intEmployeeID == TheCompareCrewEmployeesDataSet.employees[intEmployeeCounter].EmployeeID)
                                    {
                                        blnCrewMemberFound = true;

                                        if (intProjectID == TheCompareCrewEmployeesDataSet.employees[intEmployeeCounter].ProjectID)
                                        {
                                                blnItemFound = true;
                                        }
                                    }
                                }
                            }

                            if(blnItemFound == false)
                            {
                                CompareCrewEmployeesDataSet.employeesRow NewEmployeeRow = TheCompareCrewEmployeesDataSet.employees.NewemployeesRow();

                                NewEmployeeRow.AssignedProjectID = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].AssignedProjectID;
                                NewEmployeeRow.Date = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].TransactionDate;
                                NewEmployeeRow.EmployeeID = intEmployeeID;
                                NewEmployeeRow.FirstName = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].FirstName;
                                NewEmployeeRow.HomeOffice = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].HomeOffice;
                                NewEmployeeRow.LastName = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].LastName;
                                NewEmployeeRow.ProjectID = intProjectID;
                                NewEmployeeRow.CrewID = strCrewID;

                                TheCompareCrewEmployeesDataSet.employees.Rows.Add(NewEmployeeRow);
                                gintEmployeeUpperLimit = gintEmployeeCounter;
                                gintEmployeeCounter++;
                            }

                            if(blnCrewMemberFound == false)
                            {
                                CrewMembersDataSet.crewmembersRow NewMemberRow = TheCrewMembersDataSet.crewmembers.NewcrewmembersRow();

                                NewMemberRow.CrewID = strCrewID;
                                NewMemberRow.EmployeeID = intEmployeeID;
                                NewMemberRow.FirstName = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].FirstName;
                                NewMemberRow.LastName = TheFindDetailedEmployeeCrewAssignmentByCrewIDDataSet.FindDetailedEmployeeCrewAssignmentByCrewID[intCounter].LastName;

                                TheCrewMembersDataSet.crewmembers.Rows.Add(NewMemberRow);
                            }
                            
                        }
                    }

                    LoadTaskComboBox();

                    dgrResults.ItemsSource = TheCrewMembersDataSet.crewmembers;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Compare Employee Crews // Select Crew Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadTaskComboBox()
        {
            //setting local variables;
            int intFirstCounter;
            int intFirstNumberOfRecords;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            int intTaskCounter;
            bool blnItemFound;
            int intEmployeeID;
            int intProjectID;
            int intTaskID;
            string strWorkTask;

            try
            {
                intFirstNumberOfRecords = TheCompareCrewEmployeesDataSet.employees.Rows.Count - 1;
                gintTaskCounter = 0;
                gintTaskUpperLimit = 0;

                for (intFirstCounter = 0; intFirstCounter <= intFirstNumberOfRecords; intFirstCounter++)
                {
                    intEmployeeID = TheCompareCrewEmployeesDataSet.employees[intFirstCounter].EmployeeID;
                    intProjectID = TheCompareCrewEmployeesDataSet.employees[intFirstCounter].ProjectID;

                    TheFindEmployeeProjectHoursOverDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProjectHoursOverDateRange(intEmployeeID, intProjectID, gdatStartDate, gdatEndDate);

                    intSecondNumberOfRecords = TheFindEmployeeProjectHoursOverDateRangeDataSet.FindEmployeeProjectHoursOverDateRange.Rows.Count - 1;

                    if(intSecondNumberOfRecords > -1)
                    {
                        for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                        {
                            blnItemFound = false;
                            strWorkTask = TheFindEmployeeProjectHoursOverDateRangeDataSet.FindEmployeeProjectHoursOverDateRange[intSecondCounter].WorkTask;

                            TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);
                            intTaskID = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask[0].WorkTaskID;

                            if(gintTaskCounter > 0)
                            {
                                for(intTaskCounter = 0; intTaskCounter <= gintTaskUpperLimit; intTaskCounter++)
                                {
                                    if(intTaskID == TheCompareCrewTasksDataSet.worktasks[intTaskCounter].WorkTaskID)
                                    {
                                        blnItemFound = true;
                                    }
                                }
                            }

                            if(blnItemFound == false)
                            {
                                CompareCrewTasksDataSet.worktasksRow NewTaskRow = TheCompareCrewTasksDataSet.worktasks.NewworktasksRow();

                                NewTaskRow.WorkTask = strWorkTask;
                                NewTaskRow.WorkTaskID = intTaskID;

                                TheCompareCrewTasksDataSet.worktasks.Rows.Add(NewTaskRow);
                                cboSelectTask.Items.Add(strWorkTask);
                                gintTaskUpperLimit = gintTaskCounter;
                                gintTaskCounter++;
                            }
                            
                        }
                    }
                }

                cboSelectTask.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Compare Employee Crews // Load Task Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
           
        }
        private void mitResetWindow_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
        private void ClearControls()
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            TheCompareCrewEmployeesDataSet.employees.Rows.Clear();
            dgrResults.ItemsSource = TheCompareCrewEmployeesDataSet.employees;
            TheCompareCrewTasksDataSet.worktasks.Rows.Clear();
            cboSelectTask.Items.Clear();
            cboSelectCrew.Items.Clear();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ClearControls();
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting variables
            int intFirstCounter;
            int intFirstNumberOfRecords;
            int intSecondNumberOfRecords;
            int intSelectedIndex;
            int intEmployeeID;
            int intProjectID;

            try
            {
                intSelectedIndex = cboSelectTask.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintWorkTaskID = TheCompareCrewTasksDataSet.worktasks[intSelectedIndex].WorkTaskID;
                    MainWindow.gstrWorkTask = TheCompareCrewTasksDataSet.worktasks[intSelectedIndex].WorkTask;

                    intFirstNumberOfRecords = TheCompareCrewEmployeesDataSet.employees.Rows.Count - 1;

                    for(intFirstCounter = 0; intFirstCounter <= intFirstNumberOfRecords; intFirstCounter++)
                    {
                        intEmployeeID = TheCompareCrewEmployeesDataSet.employees[intFirstCounter].EmployeeID;
                        intProjectID = TheCompareCrewEmployeesDataSet.employees[intFirstCounter].ProjectID;

                        TheFindEmployeeProjectTaskStatsDataSet = TheEmployeeProductivityStatsClass.FindEmployeeProjectTaskStats(intEmployeeID, gdatStartDate, gdatEndDate);

                        intSecondNumberOfRecords = TheFindEmployeeProjectTaskStatsDataSet.FindEmployeeProjectTaskStats.Rows.Count - 1;
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Compare Employee Crews // Select Task Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
