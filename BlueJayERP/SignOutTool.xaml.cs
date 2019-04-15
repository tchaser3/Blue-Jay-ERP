/* Title:           Sign Out Tool
 * Date:            3-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to sign out tools*/

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
using NewToolsDLL;
using DataValidationDLL;
using ToolHistoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SignOutTool.xaml
    /// </summary>
    public partial class SignOutTool : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolsClass TheToolsClass = new ToolsClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();

        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();

        public SignOutTool()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strToolID;
            int intRecordsReturned;
            string strMessage;
            string strCaption;

            try
            {
                strToolID = txtEnterToolID.Text;
                if (strToolID == "")
                {
                    TheMessagesClass.ErrorMessage("Tool ID Was Not Entered");
                    return;
                }

                TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);
                intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;
                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Tool is not Active or Does Not Exist");
                    return;
                }
                if (TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].Available == false)
                {

                    strMessage = "Tool Is Currently Signed Out, Do You Want To Sign Tool In";
                    strCaption = "Please Select";
                    MainWindow.gstrToolID = strToolID;

                    MessageBoxResult result = MessageBox.Show(strMessage, strCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SignToolInFromForm SignToolInFromForm = new SignToolInFromForm();
                        SignToolInFromForm.ShowDialog();

                        if (MainWindow.gblnToolSignedIn == false)
                        {
                            TheMessagesClass.InformationMessage("The Tool Was Not Signed In, the Transaction Will Not Proceed");
                            mitSignOutTool.IsEnabled = false;
                            return;
                        }

                        TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);
                    }
                    else
                    {
                        return;
                    }
                }

                MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
                MainWindow.gblnToolSignedIn = true;
                txtDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Out Tool // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;
            string strFullName;

            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;

            if (intLength > 2)
            {
                cboSelectEmployee.Items.Clear();

                cboSelectEmployee.Items.Add("Select Employee");

                TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                intNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strFullName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].FirstName + " ";
                        strFullName += TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].LastName;

                        cboSelectEmployee.Items.Add(strFullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].EmployeeID;

                if (MainWindow.gblnToolSignedIn == true)
                {
                    mitSignOutTool.IsEnabled = true;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitSignOutTool.IsEnabled = false;
        }

        private void mitSignOutTool_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError;

            try
            {
                blnFatalError = TheToolsClass.UpdateToolSignOut(MainWindow.gintToolKey, MainWindow.gintEmployeeID, false);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheToolHistoryClass.InsertToolHistory(MainWindow.gintToolKey, MainWindow.gintEmployeeID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "TOOL SIGNED OUT");

                if (blnFatalError == true)
                    throw new Exception();

                txtDescription.Text = "";
                txtEnterLastName.Text = "";
                txtEnterToolID.Text = "";
                cboSelectEmployee.Items.Clear();
                TheMessagesClass.InformationMessage("The Tool Has Been Signed Out");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Out Tool // Sign Out Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("file://bjc/shares/Documents/WAREHOUSE/WhseTrac%20Manual/index.html");
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bluejay.on.spiceworks.com/portal/tickets");
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
            MainWindow.BulkToolSignOutWindow.Visibility = Visibility.Visible;
        }
    }
}
