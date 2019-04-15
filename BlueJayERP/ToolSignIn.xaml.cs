/* Title:           Tool Sign In
 * Date:            3-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to sign in tools*/

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
using NewToolsDLL;
using ToolHistoryDLL;
using NewEventLogDLL;
using KeyWordDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ToolSignIn.xaml
    /// </summary>
    public partial class ToolSignIn : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();

        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();

        public ToolSignIn()
        {
            InitializeComponent();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strToolID;
            int intRecordsReturned;

            strToolID = txtEnterToolID.Text;
            if (strToolID == "")
            {
                TheMessagesClass.ErrorMessage("The Tool ID Was Not Entered");
                return;
            }

            TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

            if (intRecordsReturned == 0)
            {
                TheMessagesClass.InformationMessage("Tool ID Was Not Found");
                return;
            }

            if (TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].Available == true)
            {
                TheMessagesClass.InformationMessage("Tool Is Already Signed In");
                return;
            }

            MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
            txtDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
            txtFirstName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].FirstName;
            txtLastName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].LastName;
            mitSignInTool.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitSignInTool.IsEnabled = false;
        }

        private void mitSignInTool_Click(object sender, RoutedEventArgs e)
        {
            //setting local variable
            bool blnFatalError;
            string strHomeOffice;
            int intNumberOfRecords;
            int intCounter;
            bool blnKeyWordNotFound = true;

            try
            {
                strHomeOffice = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].HomeOffice;

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(strHomeOffice, MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);

                    if (blnKeyWordNotFound == false)
                    {
                        MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                    }
                }

                blnFatalError = TheToolsClass.UpdateToolSignOut(MainWindow.gintToolKey, MainWindow.gintWarehouseID, true);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheToolHistoryClass.InsertToolHistory(MainWindow.gintToolKey, MainWindow.gintWarehouseID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "TOOL SIGNED IN");

                if (blnFatalError == true)
                    throw new Exception();

                txtDescription.Text = "";
                txtEnterToolID.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                mitSignInTool.IsEnabled = false;
                TheMessagesClass.InformationMessage("The Tool Has Been Signed In");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign In Tool // Tool Sign In Menu Item " + Ex.Message);

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

        private void mitSignInBulkTool_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SelectBulkToolForSignInWindow.Visibility = Visibility.Visible;
        }
    }
}
