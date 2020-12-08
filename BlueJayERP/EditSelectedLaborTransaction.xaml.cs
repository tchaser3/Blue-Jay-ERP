/* Title:           Edit Selected Labor Transaction
 * Date:            3-19-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to edit the selected transaction */

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
using EmployeeProjectAssignmentDLL;
using DataValidationDLL;
using ProjectTaskDLL;
using System.Reflection.Emit;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditSelectedLaborTransaction.xaml
    /// </summary>
    public partial class EditSelectedLaborTransaction : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindEmployeeLaborByTransactionIDDataSet TheFindEmployeeLaborByTransactionIDDataSet = new FindEmployeeLaborByTransactionIDDataSet();
        FindProjectWorkTaskDataSet TheFindProjectWorkTaskDataSet = new FindProjectWorkTaskDataSet();

        int gintWorkTaskTransactionID;

        public EditSelectedLaborTransaction()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intFootage = 0;
            decimal decHours = 0;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            string strEmployeeFullName;
            string strFullName;
            string strHours;
            string strHeader;
            string strMessage;

            try
            {
                strFullName = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].FirstName + " ";
                strFullName += MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].LastName;

                strEmployeeFullName = txtFirstName.Text + " ";
                strEmployeeFullName += txtLastName.Text;                

                strValueForValidation = txtHours.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Hours is not Numeric\n";
                }
                else
                {
                    decHours = Convert.ToDecimal(strValueForValidation);
                }
                strValueForValidation = txtFootage.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Footage/Pieces is not an Integer\n";
                }
                else
                {
                    intFootage = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strHours = txtHours.Text;

                blnFatalError = TheEmployeeProjectAssignmentClass.UpdateEmployeeLaborHours(MainWindow.gintTransactionID, decHours);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProjectTaskClass.UpdateProjectTaskFootage(gintWorkTaskTransactionID, intFootage);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = "There Has Been An Edit To Hours";

                strMessage = "<h1>" + strHeader + "</h1>";
                strMessage += "<h3>" + strFullName + " Has Changed " + strEmployeeFullName + " To " + strHours + "</h3>";

                TheSendEmailClass.SendEmail("jstary@bluejaycommunications.com", strHeader, strMessage);

                TheSendEmailClass.SendEmail("jhoulihan@bluejaycommunications.com", strHeader, strMessage);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Selected Labor Transactions " + strMessage);

                TheMessagesClass.InformationMessage("The Record Has Been Updated");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Selected Labor Transactions // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intEmployeeID;
            int intProjectID;
            int intWorkTaskID;
            DateTime datTransactionDate;

            try
            {
                TheFindEmployeeLaborByTransactionIDDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeLaborByTransactionID(MainWindow.gintTransactionID);

                txtAssignedProjectID.Text = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].AssignedProjectID;
                txtDate.Text = Convert.ToString(TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].TransactionDate);
                txtFirstName.Text = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].FirstName;
                txtLastName.Text = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].LastName;
                txtProjectName.Text = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].ProjectName;
                txtWorkTask.Text = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].WorkTask;
                txtHours.Text = Convert.ToString(TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].TotalHours);

                //getting ready for the the footage
                intEmployeeID = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].EmployeeID;
                intProjectID = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].ProjectID;
                intWorkTaskID = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].WorkTaskID;
                datTransactionDate = TheFindEmployeeLaborByTransactionIDDataSet.FindEmployeeLaborByTransactionID[0].TransactionDate;

                TheFindProjectWorkTaskDataSet = TheProjectTaskClass.FindProjectWorkTask(intProjectID, intEmployeeID, intWorkTaskID, datTransactionDate, datTransactionDate);

                txtFootage.Text = Convert.ToString(TheFindProjectWorkTaskDataSet.FindProjectWorkTask[0].FootagePieces);

                gintWorkTaskTransactionID = TheFindProjectWorkTaskDataSet.FindProjectWorkTask[0].TransactionID;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Selected Labor Transaction // Window Loaded " + Ex.Message);

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
