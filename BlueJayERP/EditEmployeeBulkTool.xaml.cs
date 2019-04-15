/* Title:           Edit Employee Bulk Tool
 * Date:            11-16-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to edit or sign in the bulk tools */

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
using BulkToolsDLL;
using DataValidationDLL;
using BulkToolHistoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditEmployeeBulkTool.xaml
    /// </summary>
    public partial class EditEmployeeBulkTool : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        BulkToolsClass TheBulkToolsClass = new BulkToolsClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        BulkToolHistoryClass TheBulkToolHistoryClass = new BulkToolHistoryClass();

        FindBulkToolsByTransactionIDDataSet TheFindBulkToolbyTransactionIDDataSet = new FindBulkToolsByTransactionIDDataSet();

        bool gblnSignIn;
        bool gblnAdjustQuantity;
        int gintWarehouseEmployeeID;

        public EditEmployeeBulkTool()
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
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //loading up the controls
            TheFindBulkToolbyTransactionIDDataSet = TheBulkToolsClass.FindBulkToolsByTransactionID(MainWindow.gintTransactionID);

            txtCurrentQuantity.Text = Convert.ToString(TheFindBulkToolbyTransactionIDDataSet.FindBulkToolsByTransactionID[0].Quantity);
            txtSignOutDate.Text = Convert.ToString(TheFindBulkToolbyTransactionIDDataSet.FindBulkToolsByTransactionID[0].SignOutDate);
            txtToolCategory.Text = TheFindBulkToolbyTransactionIDDataSet.FindBulkToolsByTransactionID[0].ToolCategory;
            txtIssueNotes.Text = TheFindBulkToolbyTransactionIDDataSet.FindBulkToolsByTransactionID[0].IssueNotes;
            txtTransactionID.Text = Convert.ToString(TheFindBulkToolbyTransactionIDDataSet.FindBulkToolsByTransactionID[0].TransactionID);
            txtNewQuantity.Visibility = Visibility.Hidden;
            lblNewQuantity.Visibility = Visibility.Hidden;
            gintWarehouseEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
        }

        private void rdoSignIn_Checked(object sender, RoutedEventArgs e)
        {
            txtNewQuantity.Visibility = Visibility.Hidden;
            lblNewQuantity.Visibility = Visibility.Hidden;
            gblnAdjustQuantity = false;
            gblnSignIn = true;
        }

        private void rdoAdjustQuantity_Checked(object sender, RoutedEventArgs e)
        {
            txtNewQuantity.Visibility = Visibility.Visible;
            lblNewQuantity.Visibility = Visibility.Visible;
            gblnAdjustQuantity = true;
            gblnSignIn = false;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            int intQuantity;
            int intTotalQuantity;
            DateTime datSignInDate = DateTime.Now;

            try
            {
                intTotalQuantity = Convert.ToInt32(txtCurrentQuantity.Text);

                if(gblnSignIn == true)
                {
                    blnFatalError = TheBulkToolsClass.SignInBulkTool(MainWindow.gintTransactionID, gintWarehouseEmployeeID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheBulkToolHistoryClass.InsertBulkToolHistory(MainWindow.gintEmployeeID, gintWarehouseEmployeeID, intTotalQuantity, MainWindow.gintTransactionID, "BULK TOOLS SIGNED IN");

                    if (blnFatalError == true)
                        throw new Exception();

                }
                else if(gblnAdjustQuantity == true)
                {
                    strValueForValidation = txtNewQuantity.Text;
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Quantity Entered is not an Integer");
                        return;
                    }

                    intQuantity = Convert.ToInt32(strValueForValidation);

                    if(intQuantity > intTotalQuantity)
                    {
                        TheMessagesClass.ErrorMessage("The Quantity Returned is greater than the Total Quantity");
                        return;
                    }

                    intTotalQuantity = intTotalQuantity - intQuantity;

                    blnFatalError = TheBulkToolsClass.UpdateBulkToolsQuantity(MainWindow.gintTransactionID, intTotalQuantity);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheBulkToolHistoryClass.InsertBulkToolHistory(MainWindow.gintEmployeeID, gintWarehouseEmployeeID, intTotalQuantity, MainWindow.gintTransactionID, "THE QUANTITY HAS BEEN ADJUSTED");

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Bulk Tool Has Been Updated");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee Bulk Tool // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
