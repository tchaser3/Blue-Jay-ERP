/* Title:           Assign Phone Extension
 * Date:            4-15-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to assign a phone extension */

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
using DataValidationDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using PhonesDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignPhoneExtension.xaml
    /// </summary>
    public partial class AssignPhoneExtension : Window
    {
        //setting classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PhonesClass ThePhoneClass = new PhonesClass();

        //setting up the data
        FindPhoneByExtensionDataSet TheFindPhoneByExtensionDataSet = new FindPhoneByExtensionDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public AssignPhoneExtension()
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
            txtEnterExtension.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            mitProcess.IsEnabled = false;
        }

        private void TxtEnterExtension_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            int intLength;
            int intRecordsReturned;
            int intExtension;

            try
            {
                strValueForValidation = txtEnterExtension.Text;
                intLength = strValueForValidation.Length;
                if (intLength == 4)
                {
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Value Entered is not an Integer");
                        return;
                    }

                    intExtension = Convert.ToInt32(strValueForValidation);

                    TheFindPhoneByExtensionDataSet = ThePhoneClass.FindPhoneByExtension(intExtension);

                    intRecordsReturned = TheFindPhoneByExtensionDataSet.FindPhoneByExtension.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Extension Entered Does Not Exist");
                        return;
                    }

                    MainWindow.gintTransactionID = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].TransactionID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Phone Extension // Extension Text box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void TxtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if(intNumberOfRecords == -1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Phone Extension // Last Name Text Box Change Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }

            mitProcess.IsEnabled = true;
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            bool blnFatalError = false;

            try
            {
                if(txtEnterExtension.Text =="")
                {
                    TheMessagesClass.ErrorMessage("The Extension Was Not Entered");
                    return;
                }

                blnFatalError = ThePhoneClass.UpdateEmployeePhone(MainWindow.gintTransactionID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Extension has been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Phone Extension // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
