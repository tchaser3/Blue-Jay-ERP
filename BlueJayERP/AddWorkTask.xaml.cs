/* Title:           Add Work Task
 * Date:            2-9-18
 * Author:          Terry Holmes
 * 
 * Description:     This is where a work task will be added */

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
using WorkTaskDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddWorkTask.xaml
    /// </summary>
    public partial class AddWorkTask : Window
    {
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindWorkTaskByWorkTaskDataSet TheFindWorkTaskByWorkTaskDataSet = new FindWorkTaskByWorkTaskDataSet();

        public AddWorkTask()
        {
            InitializeComponent();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            string strErrorMessage = "";
            string strWorkTask;
            decimal decTotalCost = 0;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intRecordsReturn;

            try
            {
                strWorkTask = txtWorkTask.Text;
                if(strWorkTask == "")
                {
                    strErrorMessage += "The Work Task Was Not Entered\n";
                    blnFatalError = true;
                }
                strValueForValidation = txtTotalCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Total Cost was not a Decimal\n";
                }
                else
                {
                    decTotalCost = Convert.ToDecimal(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);

                intRecordsReturn = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask.Rows.Count;

                if(intRecordsReturn > 0)
                {
                    TheMessagesClass.InformationMessage("The Work Task Entered Already Exists");
                    return;
                }

                blnFatalError = TheWorkTaskClass.InsertWorkTask(strWorkTask, decTotalCost);

                if (blnFatalError == true)
                    throw new Exception();

                if(blnFatalError == false)
                {
                    txtWorkTask.Text = "";
                    txtTotalCost.Text = "";
                    TheMessagesClass.InformationMessage("The Work Task Has Been Saved");
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Work Task // Save Button " + Ex.Message);

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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
