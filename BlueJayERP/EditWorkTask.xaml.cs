/* Title:           Edit Work Task
 * Date:            2-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This the form used to edit work tasks */

using DataValidationDLL;
using NewEventLogDLL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkTaskDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditWorkTask.xaml
    /// </summary>
    public partial class EditWorkTask : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByTaskKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindWorkTaskByTaskIDDataSet TheFindWorkTaskbyTaskIDDataSet = new FindWorkTaskByTaskIDDataSet();

        bool gblnActive;

        public EditWorkTask()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strWorkTask;
            decimal decTaskCost = 1;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;

            try
            {
                strWorkTask = txtWorkTask.Text;
                if(strWorkTask == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task Was Not Entered\n";
                }
                strValueForValidation = txtTotalCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Cost Is Not Numeric\n";
                }
                else
                {
                    decTaskCost = Convert.ToDecimal(strValueForValidation);
                }
                if(cboSelectActive.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Active Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //updating record
                blnFatalError = TheWorkTaskClass.UpdateWorkTask(MainWindow.gintWorkTaskID, strWorkTask, decTaskCost);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkTaskClass.UpdateWorkTaskActive(MainWindow.gintWorkTaskID, gblnActive);

                if (blnFatalError == true)
                    throw new Exception();

                if (blnFatalError == false)
                {
                    txtEnterWorkTask.Text = "";
                    txtTotalCost.Text = "";
                    txtWorkTask.Text = "";
                    txtWorkTaskID.Text = "";
                    cboSelectWorkTask.SelectedIndex = 0;
                    cboSelectActive.SelectedIndex = 0;
                    mitSave.IsEnabled = false;
                    TheMessagesClass.InformationMessage("The Record Has Been Updated");
                }
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Work Task // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitSave.IsEnabled = false;
            cboSelectActive.Items.Add("Select Active");
            cboSelectActive.Items.Add("Yes");
            cboSelectActive.Items.Add("No");
            cboSelectActive.SelectedIndex = 0;
        }

        private void txtEnterWorkTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strWorkTask = txtEnterWorkTask.Text;

                intLength = strWorkTask.Length;

                if(intLength >= 2)
                {
                    cboSelectWorkTask.Items.Clear();
                    cboSelectWorkTask.Items.Add("Select Work Task");

                    TheFindWorkTaskByTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                    intNumberOfRecords = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count - 1;

                    if(intNumberOfRecords == -1)
                    {
                        TheMessagesClass.InformationMessage("The Work Task Entered Does Not Exist");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectWorkTask.Items.Add(TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
                    }

                    cboSelectWorkTask.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Work Task // Enter Work Task Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWorkTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectWorkTask.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintWorkTaskID = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;

                    txtWorkTaskID.Text = Convert.ToString(MainWindow.gintWorkTaskID);
                    txtWorkTask.Text = TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTask;
                    txtTotalCost.Text = Convert.ToString(TheFindWorkTaskByTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].TaskCost);                   

                    mitSave.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Work Task // Combo Box Change Event " + Ex.Message);

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

        private void cboSelectActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSelectActive.SelectedIndex == 1)
            {
                gblnActive = true;
            }
            else if(cboSelectActive.SelectedIndex == 2)
            {
                gblnActive = false;
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
