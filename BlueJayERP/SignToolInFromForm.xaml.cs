/* Title:           Sign Tool In From Form
 * Date:            3-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This the Sign Tool In when the tool is being signed out */

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
using NewEmployeeDLL;
using KeyWordDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SignToolInFromForm.xaml
    /// </summary>
    public partial class SignToolInFromForm : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        ToolsClass TheToolsClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();

        public SignToolInFromForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(MainWindow.gstrToolID);

                MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;

                txtToolID.Text = MainWindow.gstrToolID;
                txtDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                txtFirstName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].FirstName;
                txtLastName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].LastName;

                MainWindow.gblnToolSignedIn = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Tool In From Form // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitSignInTool_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strHomeOffice;
            bool blnKeyWordNotFound = true;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strHomeOffice = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].HomeOffice;

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(strHomeOffice, MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);

                    if(blnKeyWordNotFound == false)
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

                MainWindow.gblnToolSignedIn = true;

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Tool In From Form // Sign In Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
