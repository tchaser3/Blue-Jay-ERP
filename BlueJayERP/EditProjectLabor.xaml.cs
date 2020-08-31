/* Title:           Edit Project Labor
 * Date:            3-16-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used for editting a project labor */

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
using DataValidationDLL;
using EmployeeProjectAssignmentDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditProjectLabor.xaml
    /// </summary>
    public partial class EditProjectLabor : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up th data
        FindLaborHoursByDateRangeDataSet TheFindLaborHoursByDateRangeDataSet = new FindLaborHoursByDateRangeDataSet();

        public EditProjectLabor()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtEnterStartDate.Text = "";
            txtEnterEndDate.Text = "";
            TheFindLaborHoursByDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindLaborHoursByDateRange(DateTime.Now, DateTime.Now);
            dgvResults.ItemsSource = TheFindLaborHoursByDateRangeDataSet.FindLaborHoursByDateRange;
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitFindTransactions_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";

            strValueForValidation = txtEnterStartDate.Text;
            blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
            if(blnThereIsAProblem == true)
            {
                blnFatalError = true;
                strErrorMessage += "The Start Date is not a Date\n";
            }
            else
            {
                MainWindow.gdatStartDate = Convert.ToDateTime(strValueForValidation);
            }
            strValueForValidation = txtEnterEndDate.Text;
            blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
            if (blnThereIsAProblem == true)
            {
                blnFatalError = true;
                strErrorMessage += "The End Date is not a Date\n";
            }
            else
            {
                MainWindow.gdatEndDate = Convert.ToDateTime(strValueForValidation);
            }
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }
            else
            {
                blnFatalError = TheDataValidationClass.verifyDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                    return;
                }
            }

            TheFindLaborHoursByDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindLaborHoursByDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

            dgvResults.ItemsSource = TheFindLaborHoursByDateRangeDataSet.FindLaborHoursByDateRange;
        }

        private void dgvResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;

            try
            {
                if(dgvResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgvResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTransactionID = ((TextBlock)TransactionID.Content).Text;
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Has Selected A Transaction to Be Editted");

                    //find the record
                    MainWindow.gintTransactionID = Convert.ToInt32(strTransactionID);

                    EditSelectedLaborTransaction EditSelectedLaborTransaction = new EditSelectedLaborTransaction();
                    EditSelectedLaborTransaction.ShowDialog();

                    TheFindLaborHoursByDateRangeDataSet = TheEmployeeProjectAssignmentClass.FindLaborHoursByDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    dgvResults.ItemsSource = TheFindLaborHoursByDateRangeDataSet.FindLaborHoursByDateRange;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Project Labor // Grid Selection " + Ex.Message);

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
