/* Title:           Assign Cell Phones
 * Date:            4-18-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to assign the cell phones */

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
using PhonesDLL;
using NewEventLogDLL;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignCellPhones.xaml
    /// </summary>
    public partial class AssignCellPhones : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastFourDataSet = new FindCellPhoneByLastFourDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public AssignCellPhones()
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
            txtCellPhoneNumber.Text = "";
            txtEnterLastFour.Text = "";
            txtEnterLastName.Text =  "";
            cboSelectEmployee.Items.Clear();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtEnterLastFour_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intLength;
            bool blnFatalError = false;
            int intRecordsReturned;

            strValueForValidation = txtEnterLastFour.Text;
            intLength = strValueForValidation.Length;
            if(intLength == 4)
            {
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Information Entered is not an Integer");
                    return;
                }
                
                TheFindCellPhoneByLastFourDataSet = ThePhoneClass.FindCellPhoneByLastFour(strValueForValidation);

                intRecordsReturned = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Cell Phone Not Found");
                    return;
                }

                txtCellPhoneNumber.Text = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneNumber;
                txtCurrentAssignment.Text = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].FirstName + " " + TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].LastName;
                MainWindow.gintPhoneID = TheFindCellPhoneByLastFourDataSet.FindCellPhoneByLastFour[0].PhoneID;
            }
        }

        private void TxtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
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
                cboSelectEmployee.Items.Add("Select Emnployee");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }
    }
}
