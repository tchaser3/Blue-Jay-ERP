/* Title:           Update Work Task Productivity Value
 * Date:            3-12-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to udpate the Work Task Productivity Value */

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
using WorkTaskDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateWorkTaskProductivityValue.xaml
    /// </summary>
    public partial class UpdateWorkTaskProductivityValue : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();

        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();

        public UpdateWorkTaskProductivityValue()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
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
            txtEnterTask.Text = "";
            txtProductionValue.Text = "";
            txtWorkTaskID.Text = "";
            cboSelectTask.Items.Clear();
            btnProcess.IsEnabled = false;
        }

        private void TxtEnterTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            strWorkTask = txtEnterTask.Text;
            intLength = strWorkTask.Length;
            if(intLength > 2)
            {
                TheFindWorkTaskKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);

                intNumberOfRecords = TheFindWorkTaskKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Task Not Found");
                    return;
                }

                cboSelectTask.Items.Clear();
                cboSelectTask.Items.Add("Select Work Task");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectTask.Items.Add(TheFindWorkTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
                }

                cboSelectTask.SelectedIndex = 0;
            }
        }

        private void CboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting up the variables
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectTask.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintWorkTaskID = TheFindWorkTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;
                    txtWorkTaskID.Text = Convert.ToString(MainWindow.gintWorkTaskID);
                    if(TheFindWorkTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].IsProductionValueNull() == true)
                    {
                        txtProductionValue.Text = "0";
                    }
                    else
                    {
                        txtProductionValue.Text = Convert.ToString(TheFindWorkTaskKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].ProductionValue);
                    }

                    btnProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // uPdate Work Task Productivity Value // Combo Box Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            decimal decProductivityValue = 0;

            try
            {
                strValueForValidation = txtProductionValue.Text;
                blnFatalError = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Productivity Value is not Numeric");
                    return;
                }

                decProductivityValue = Convert.ToDecimal(strValueForValidation);

                blnFatalError = TheWorkTaskClass.UpdateWorkTaskProductivityRate(MainWindow.gintWorkTaskID, decProductivityValue);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Task Value Has Been Updated");

                ResetControls();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Work Task Productivity Value // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
