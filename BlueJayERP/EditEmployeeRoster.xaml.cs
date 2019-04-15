/* Title:           Edit Employee From Roster
 * Date:            10-17-18
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the user to edit an employee */

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
using DepartmentDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditEmployeeRoster.xaml
    /// </summary>
    public partial class EditEmployeeRoster : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindSortedEmployeeGroupDataSet TheFindSortedEmployeeGroupDataSet = new FindSortedEmployeeGroupDataSet();
        
        bool gblnActive;
        string gstrEmployeeGroup;
        string gstrEmployeeType;
        string gstrSalaryType;
        string gstrDepartment;
        int gintManagerID;
        

        public EditEmployeeRoster()
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
            LoadActiveComboBox();
            LoadGroupComboBox();
            LoadWarehouseComboBox();
            LoadEmployeeType();
            LoadPayType();
            LoadDepartmentComboBox();
            LoadManagerComboBox();
            LoadControls();
        }
        private void LoadControls()
        {
            //setting local variables
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found, Contact IT");
                    Close();
                }

                txtEmployeeID.Text = Convert.ToString(MainWindow.gintEmployeeID);
                txtFirstName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                txtLastName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                txtPhoneNumber.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PhoneNumber;
                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEmailAddressNull() == false)
                {
                    txtEmailAddress.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                }
                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsPayIDNull() == false)
                {
                    txtPayID.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PayID);
                }
                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsStartDateNull() == false)
                {
                    txtStartDate.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].StartDate);
                }
                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].IsEndDateNull() == false)
                {
                    txtTerminationDate.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EndDate);
                }

                if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].Active == true)
                {
                    cboActive.SelectedIndex = 1;
                }

                intNumberOfRecords = TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeGroup == TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup[intCounter].GroupName)
                    {
                        cboSelectGroup.SelectedIndex = intCounter + 1;
                    }
                }

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        cboWarehouse.SelectedIndex = intCounter + 1;
                    }
                }

                if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].SalaryType == "EXEMPT")
                    cboSelectSalaryType.SelectedIndex = 1;
                else if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].SalaryType == "NON-EXEMPT")
                    cboSelectSalaryType.SelectedIndex = 2;

                if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeType == "EMPLOYEE")
                    cboEmployeeType.SelectedIndex = 1;

                intNumberOfRecords = MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].Department == MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department)
                    {
                        cboSelectDepartment.SelectedIndex = intCounter + 1;
                    }
                }

                intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].ManagerID == MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].employeeID)
                    {
                        cboSelectManager.SelectedIndex = intCounter + 1;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee Roster // Load Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void LoadManagerComboBox()
        {
            int intCounter;
            int intNumberOfRecords;

            MainWindow.TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

            intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

            cboSelectManager.Items.Add("Select Manager");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectManager.Items.Add(MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
            }

            cboSelectManager.SelectedIndex = 0;
        }
        private void LoadDepartmentComboBox()
        {
            int intCounter;
            int intNumberOfRecords;

            MainWindow.TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            cboSelectDepartment.Items.Add("Select Department");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectDepartment.Items.Add(MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboSelectDepartment.SelectedIndex = 0;
        }
        private void LoadPayType()
        {
            cboSelectSalaryType.Items.Add("Select Salary Type");
            cboSelectSalaryType.Items.Add("EXEMPT");
            cboSelectSalaryType.Items.Add("NON-EXEMPT");
            cboSelectSalaryType.SelectedIndex = 0;
        }
        private void LoadEmployeeType()
        {
            cboEmployeeType.Items.Add("Select Employee Type");
            cboEmployeeType.Items.Add("EMPLOYEE");
            cboEmployeeType.Items.Add("CONTRACTOR");
            cboEmployeeType.SelectedIndex = 0;
        } 
        private void LoadWarehouseComboBox()
        {
            int intCounter;
            int intNumberOfRecords;

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            cboWarehouse.Items.Add("Select Warehouse");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboWarehouse.SelectedIndex = 0;
        }
        private void LoadGroupComboBox()
        {
            int intCounter;
            int intNumberOfRecords;

            TheFindSortedEmployeeGroupDataSet = TheEmployeeClass.FindSortedEmpoyeeGroup();

            intNumberOfRecords = TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup.Rows.Count - 1;

            cboSelectGroup.Items.Add("Select Group");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectGroup.Items.Add(TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup[intCounter].GroupName);
            }

            cboSelectGroup.SelectedIndex = 0;
        }
        private void LoadActiveComboBox()
        {
            cboActive.Items.Add("Select Active");
            cboActive.Items.Add("Yes");
            cboActive.Items.Add("No");

            cboActive.SelectedIndex = 0;
        }

        private void cboActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboActive.SelectedIndex == 1)
                gblnActive = true;
            else if (cboActive.SelectedIndex == 2)
                gblnActive = false;
        }

        private void cboSelectGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectGroup.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gstrEmployeeGroup = TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup[intSelectedIndex].GroupName;
            }
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
               MainWindow.gstrHomeOffice   = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;
            }
        }

        private void cboEmployeeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboEmployeeType.SelectedIndex > 0)
                gstrEmployeeType = cboEmployeeType.SelectedItem.ToString();
        }

        private void cboSelectSalaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectSalaryType.SelectedIndex > 0)
                gstrSalaryType = cboSelectSalaryType.SelectedItem.ToString();
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gstrDepartment = MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].Department;
            }
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

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strFirstName;
            string strLastName;
            string strEmailAddress;
            int intPayID = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strPhoneNumber;

            try
            {
                //data validation
                strFirstName = txtFirstName.Text;
                if(strFirstName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The First Name Was not Entered\n";
                }
                strLastName = txtLastName.Text;
                if(strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Last Name Was not Entered\n";
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Phone Number is not the Correct Format\n";
                }
                strEmailAddress = txtEmailAddress.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyEmailAddress(strEmailAddress);
                if(blnThereIsAProblem == true)
                {
                    strEmailAddress = "";
                }
                strValueForValidation = txtPayID.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Pay ID is not an Integer\n";
                }
                else
                {
                    intPayID = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtTerminationDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(cboActive.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Active As Not Selected\n";
                }
                else if(cboActive.SelectedIndex == 2)
                {
                    datEndDate = DateTime.Now;
                }
                if(cboSelectGroup.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Group Was Not Selected\n";
                }
                if(cboWarehouse.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Home Office was not Selected\n";
                }
                if(cboEmployeeType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Type Was Not Selected\n";
                }
                if(cboSelectSalaryType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Salary Type Was not Selected\n";
                }
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was not Selected\n";
                }
                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager Was not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheEmployeeClass.UpdateEmployee(MainWindow.gintEmployeeID, strFirstName, strLastName,  strPhoneNumber, gblnActive, gstrEmployeeGroup, MainWindow.gstrHomeOffice, gstrEmployeeType, strEmailAddress, gstrSalaryType, gstrDepartment, gintManagerID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeePayInformation(MainWindow.gintEmployeeID, gstrDepartment, gstrSalaryType, gintManagerID, intPayID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeStartDate(MainWindow.gintEmployeeID, datStartDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeEndDate(MainWindow.gintEmployeeID, datEndDate);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Employee Has Been Updated");

                Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee Roster // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
