/* Title:           Add Employee
 * Date:            2-9+-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window to Add New Employee */

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
using DataValidationDLL;
using DepartmentDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();

        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();

        //setting variables
        string gstrSalaryType;
        string gstrDepartment;
        int gintManagerID;

        public AddEmployee()
        {
            InitializeComponent();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            SetControlsReadOnly(true);
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            cboEmployeeType.SelectedIndex = 0;
            cboSelectGroup.SelectedIndex = 0;
            cboWarehouse.SelectedIndex = 0;
            this.Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Add Employees // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtFirstName.IsReadOnly = blnValueBoolean;
            txtLastName.IsReadOnly = blnValueBoolean;
            txtPhoneNumber.IsReadOnly = blnValueBoolean;
            mitSave.IsEnabled = !(blnValueBoolean);
            txtEmailAddress.IsReadOnly = blnValueBoolean;

            if (blnValueBoolean == false)
            {
                txtFirstName.Background = Brushes.White;
                txtLastName.Background = Brushes.White;
                txtPhoneNumber.Background = Brushes.White;
                txtEmailAddress.Background = Brushes.White;
            }
            else
            {
                txtFirstName.Background = Brushes.LightGray;
                txtLastName.Background = Brushes.LightGray;
                txtPhoneNumber.Background = Brushes.LightGray;
                txtEmailAddress.Background = Brushes.LightGray;
            }
        }
        private void ClearControls()
        {
            //setting local variables;
            int intNumberOfRecords;
            int intCounter;


            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhoneNumber.Text = "";
            txtEmailAddress.Text = "";
            SetControlsReadOnly(true);

            MainWindow.TheFindSortedEmployeeGroupDataSet = TheEmployeeClass.FindSortedEmpoyeeGroup();

            intNumberOfRecords = MainWindow.TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup.Rows.Count - 1;

            cboSelectGroup.Items.Clear();
            cboSelectGroup.Items.Add("Select Group");

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectGroup.Items.Add(MainWindow.TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup[intCounter].GroupName);
            }

            cboWarehouse.Items.Clear();
            cboWarehouse.Items.Add("Select Warehouse");

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectDepartment.Items.Clear();
            cboSelectDepartment.Items.Add("Select Department");

            MainWindow.TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboSelectManager.Items.Clear();
            cboSelectManager.Items.Add("Select Manager");

            MainWindow.TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

            intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
            }

            cboEmployeeType.Items.Clear();
            cboEmployeeType.Items.Add("Select Type");
            cboEmployeeType.Items.Add("EMPLOYEE");
            cboEmployeeType.Items.Add("CONTRACTOR");

            cboSelectSalaryType.Items.Clear();
            cboSelectSalaryType.Items.Add("Select Salary Type");
            cboSelectSalaryType.Items.Add("EXEMPT");
            cboSelectSalaryType.Items.Add("NON-EXEMPT");

            cboSelectGroup.SelectedIndex = 0;
            cboWarehouse.SelectedIndex = 0;
            cboEmployeeType.SelectedIndex = 0;
            cboSelectSalaryType.SelectedIndex = 0;
            cboSelectDepartment.SelectedIndex = 0;
            cboSelectManager.SelectedIndex = 0;
            SetControlsReadOnly(true);
        }

        private void mitAdd_Click(object sender, RoutedEventArgs e)
        {
            int intEmployeeID;

            SetControlsReadOnly(false);

            //this will load the controls
            SetControlsReadOnly(false);

            intEmployeeID = TheEmployeeClass.CreateEmployeeID();

            txtEmployeeID.Text = Convert.ToString(intEmployeeID);
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //this will add an employee
            //setting local variables
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strFirstName;
            string strLastName;
            string strPhoneNumber;
            string strGroup;
            string strHomeOffice;
            string strEmployeeType;
            int intRecordsReturned;
            bool blnThereIsAProblem = false;
            string strEmailAddress = "NO EMAIL ADDRESS";
            int intPayID = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate;
            string strEndDate = "12/31/2999";

            try
            {
                datEndDate = Convert.ToDateTime(strEndDate);
                strEmployeeType = cboEmployeeType.SelectedItem.ToString();
                if (strEmployeeType == "Select Type")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Type Was Not Selected\n";
                }
                strGroup = cboSelectGroup.SelectedItem.ToString();
                if (strGroup == "Select Group")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Group Was Not Selected\n";
                }
                strHomeOffice = cboWarehouse.SelectedItem.ToString();
                if (strHomeOffice == "Select Warehouse")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Warehouse Was Not Selected\n";
                }
                strFirstName = txtFirstName.Text;
                if (strFirstName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The First Name Was Not Entered\n";
                }
                strLastName = txtLastName.Text;
                if (strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Last Name Was Not Entered\n";
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Phone Number Was Not Entered\n";
                }
                if(txtEmailAddress.Text != "")
                {
                    strEmailAddress = txtEmailAddress.Text.ToLower();
                    blnThereIsAProblem = TheDataValidationClass.VerifyEmailAddress(strEmailAddress);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Email Address is not the correct format\n";
                    }
                }
                else
                {
                    strEmailAddress = "";
                }
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(txtPayID.Text);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Pay ID is not an Integer\n";
                }
                else
                {
                    intPayID = Convert.ToInt32(txtPayID.Text);
                }
                if (cboSelectSalaryType.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Salary Type Was Not Selected\n";
                }
                if (cboSelectDepartment.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Department Was Not Selected\n";
                }
                if (cboSelectManager.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Manager was not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //checking to see if the employee exists
                MainWindow.TheExistingEmployeeDataSet = TheEmployeeClass.VerifyEmployee(strFirstName, strLastName);
                intRecordsReturned = MainWindow.TheExistingEmployeeDataSet.VerifyEmployee.Rows.Count;

                MainWindow.gblnKeepNewEmployee = true;

                if (intRecordsReturned > 0)
                {
                    ExistingEmployees ExistingEmployees = new ExistingEmployees();
                    ExistingEmployees.ShowDialog();
                }

                if (MainWindow.gblnKeepNewEmployee == false)
                {
                    return;
                }

                MainWindow.gintEmployeeID = Convert.ToInt32(txtEmployeeID.Text);

                blnFatalError = TheEmployeeClass.InsertEmployee(MainWindow.gintEmployeeID, strFirstName, strLastName, strPhoneNumber, true, strGroup, strHomeOffice, strEmployeeType, strEmailAddress, gstrSalaryType, gstrDepartment, gintManagerID, intPayID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeStartDate(MainWindow.gintEmployeeID, datStartDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeEndDate(MainWindow.gintEmployeeID, datEndDate);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Employee Has Been Added");

                ClearControls();
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Employees // Add Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.TheFindSortedEmployeeGroupDataSet = TheEmployeeClass.FindSortedEmpoyeeGroup();
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

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ClearControls();
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintManagerID = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSelectDepartment.SelectedIndex > 0)
            {
                gstrDepartment = cboSelectDepartment.SelectedItem.ToString();
            }
        }

        private void cboSelectSalaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSelectSalaryType.SelectedIndex > 0)
            {
                gstrSalaryType = cboSelectSalaryType.SelectedItem.ToString();
            }
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
