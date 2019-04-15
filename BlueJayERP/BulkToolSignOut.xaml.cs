/* Title:           Bulk Tool Sign Out
 * Date:            11-15-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the bulk tool sign out */

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
using BulkToolsDLL;
using ToolCategoryDLL;
using DataValidationDLL;
using BulkToolHistoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for BulkToolSignOut.xaml
    /// </summary>
    public partial class BulkToolSignOut : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        BulkToolsClass TheBulkToolsClass = new BulkToolsClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        BulkToolHistoryClass TheBulkToolHistoryClass = new BulkToolHistoryClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindToolCategoryByKeyWordDataSet TheFindToolCategoryByKeyWordDataSet = new FindToolCategoryByKeyWordDataSet();
        FindBulkToolsByDateMatchDataSet TheFindBulkToolByDateMatchDataSet = new FindBulkToolsByDateMatchDataSet();

        //setting global variables
        int gintWarehouseEmployeeID;

        public BulkToolSignOut()
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

        private void mitSignOutBulkTool_Click(object sender, RoutedEventArgs e)
        {
            string strIssueNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strValueForValidation;
            int intQuantity = 0;
            bool blnThereIsAProblem = false;
            DateTime datTransactionDate = DateTime.Now;
            int intBulkToolID;

            try
            {
                if (cboSelectEmployee.SelectedIndex <= 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Was Not Selected\n";
                }
                if (cboSelectCategory.SelectedIndex <= 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Tool Category Was Not Selected\n";
                }
                strValueForValidation = txtQuantity.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Quantity Entered is not an Integer\n";
                }
                else
                {
                    intQuantity = Convert.ToInt32(strValueForValidation);
                }
                strIssueNotes = txtIssueNotes.Text;
                if (strIssueNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "No Notes Were Entered\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheBulkToolsClass.InsertBulkTools(MainWindow.gintEmployeeID, MainWindow.gintCategoryID, intQuantity, datTransactionDate, gintWarehouseEmployeeID, strIssueNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindBulkToolByDateMatchDataSet = TheBulkToolsClass.FindBulkToolsByDateMatch(datTransactionDate);

                intBulkToolID = TheFindBulkToolByDateMatchDataSet.FindBulkToolsByDateMatch[0].TransactionID;

                blnFatalError = TheBulkToolHistoryClass.InsertBulkToolHistory(MainWindow.gintEmployeeID, gintWarehouseEmployeeID, intQuantity, intBulkToolID, strIssueNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Tools Are Signed Out");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Bulk Tool Sign Out // Sign Out Bulk Tool Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }            
        }
        private void ResetControls()
        {
            gintWarehouseEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
            txtEnterLastName.Text = "";
            txtEnterToolCategory.Text = "";
            txtIssueNotes.Text = "";
            cboSelectCategory.Items.Clear();
            cboSelectEmployee.Items.Clear();
            txtQuantity.Text = "";
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;
            if(intLength > 2)
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void txtEnterToolCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strToolCategory;
            int intCounter;
            int intNumberOfRecords;

            cboSelectCategory.Items.Clear();
            cboSelectCategory.Items.Add("Select Tool Category");

            strToolCategory = txtEnterToolCategory.Text;

            TheFindToolCategoryByKeyWordDataSet = TheToolCategoryClass.FindToolCategoryByKeyWord(strToolCategory);

            intNumberOfRecords = TheFindToolCategoryByKeyWordDataSet.FindToolCategoryByKeyWord.Rows.Count - 1;

            if(intNumberOfRecords < 0)
            {
                TheMessagesClass.ErrorMessage("Tool Category Not Found");
                return;
            }

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectCategory.Items.Add(TheFindToolCategoryByKeyWordDataSet.FindToolCategoryByKeyWord[intCounter].ToolCategory);
            }

            cboSelectCategory.SelectedIndex = 0;
        }

        private void cboSelectCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectCategory.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintCategoryID = TheFindToolCategoryByKeyWordDataSet.FindToolCategoryByKeyWord[intSelectedIndex].CategoryID;
            }
        }
    }
}
