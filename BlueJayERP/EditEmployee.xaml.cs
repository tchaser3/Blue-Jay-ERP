/* Title:           Edit Employee
 * Date:            2-28-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is where you edit an employee */

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
using NewEmployeeDLL;
using DataValidationDLL;
using DepartmentDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditEmployee.xaml
    /// </summary>
    public partial class EditEmployee : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();

        //setting up the data
        FindAllEmployeesByLastNameDataSet TheFindAllEmployeesByLastNameDataSet = new FindAllEmployeesByLastNameDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeID = new FindEmployeeByEmployeeIDDataSet();
        EmployeeGroupDataSet TheEmployeeGroupDataSet;

        //setting global variables
        bool gblnActive;
        string gstrHomeOffice;
        string gstrEmployeeType;
        string gstrGroup;
        string gstrDepartment;
        string gstrSalaryType;
        int gintManagerID;

        public EditEmployee()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will call the like search
            string strLastName;
            int intNumberOfRecords;
            int intCounter;
            int intLength;

            try
            {
                strLastName = txtEnterLastName.Text;

                intLength = strLastName.Length;
                
                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheFindAllEmployeesByLastNameDataSet = TheEmployeeClass.FindAllEmployeesByLastName(strLastName);

                    intNumberOfRecords = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName.Rows.Count - 1;

                    if(intNumberOfRecords == -1)
                    {
                        TheMessagesClass.ErrorMessage("Employees Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].FirstName + " " + TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].LastName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee // Enter Last Name Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            bool blnActive;
            string strEmployeeType;
            string strHomeOffice;
            string strGroup;
            int intComboSelectedIndex = 0;
            string strSalaryType;
            string strDepartment;
            int intManagerID;
            int intPayID;


            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    cboSelectActive.IsEnabled = true;
                    cboSelectEmployeetype.IsEnabled = true;
                    cboSelectHomeOffice.IsEnabled = true;
                    cboSelectGroup.IsEnabled = true;

                    MainWindow.gintEmployeeID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].EmployeeID;

                    TheFindEmployeeByEmployeeID = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                    txtEmployeeID.Text = Convert.ToString(MainWindow.gintEmployeeID);
                    txtFirstName.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].FirstName;
                    txtLastName.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].LastName;
                    txtPhoneNumber.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].PhoneNumber;
                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsStartDateNull() == true)
                    {
                        txtStartDate.Text = "Enter a Start Date";
                    }
                    else
                    {
                        txtStartDate.Text = Convert.ToString(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].StartDate);
                    }                    
                    if(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].IsEndDateNull() == true)
                    {
                        txtEndDate.Text = "Enter A End Date";
                    }
                    else
                    {
                        txtEndDate.Text = Convert.ToString(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EndDate);
                    }
                    

                    blnActive = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].Active;
                    strEmployeeType = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].EmployeeType;
                    strHomeOffice = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].HomeOffice;
                    strGroup = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].EmployeeGroup;

                    if(TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].IsPayIDNull() == true)
                    {
                        intPayID = -1;
                    }
                    else
                    {
                        intPayID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].PayID;
                    }                    

                    txtPayID.Text = Convert.ToString(intPayID);

                    if(TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].IsEmailAddressNull() == false)
                    {
                        txtEmailAddress.Text = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].EmailAddress;
                    }
                    else
                    {
                        txtEmailAddress.Text = "";
                    }
                    if (TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].IsDepartmentNull() == true)
                    {
                        strDepartment = "";

                        cboSelectDepartment.SelectedIndex = 0;
                    }
                    else
                    {
                        strDepartment = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].Department;

                        for(intCounter = 0; intCounter <= cboSelectDepartment.Items.Count - 1; intCounter++)
                        {
                            if(strDepartment == cboSelectDepartment.SelectedItem.ToString())
                            {
                                cboSelectDepartment.SelectedIndex = intCounter;
                            }
                        }
                    }

                    if (TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].IsManagerIDNull() == true)
                    {
                        intManagerID = 0;

                        cboSelectManager.SelectedIndex = 0;
                    }
                    else if (TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].ManagerID == 0)
                    {
                        intManagerID = 0;

                        cboSelectManager.SelectedIndex = 0;
                    }
                    else
                    {
                        intManagerID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].ManagerID;

                        intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            if (intManagerID == MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].employeeID)
                            {
                                cboSelectManager.SelectedIndex = intCounter + 1;
                            }
                        }
                    }

                    if(TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].IsSalaryTypeNull() == true)
                    {
                        strSalaryType = "";

                        cboSelectSalaryType.SelectedIndex = 0;
                    }
                    else
                    {
                        strSalaryType = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].SalaryType;

                        for(intCounter = 0; intCounter < cboSelectSalaryType.Items.Count; intCounter++)
                        {
                            cboSelectSalaryType.SelectedIndex = intCounter;

                            if (strSalaryType == cboSelectSalaryType.SelectedItem.ToString())
                            {
                                break;
                            }
                        }
                    }
                    if(TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].IsDepartmentNull() == true)
                    {
                        cboSelectDepartment.SelectedIndex = 0;
                    }
                    else
                    {
                        strDepartment = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intSelectedIndex].Department;

                        for(intCounter = 0; intCounter < cboSelectDepartment.Items.Count; intCounter++)
                        {
                            cboSelectDepartment.SelectedIndex = intCounter;

                            if (strDepartment == cboSelectDepartment.SelectedItem.ToString()) 
                            {
                                break;
                            }
                        }
                    }

                    if (blnActive == true)
                        cboSelectActive.SelectedIndex = 1;
                    else
                        cboSelectActive.SelectedIndex = 2;

                    intNumberOfRecords = cboSelectGroup.Items.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectGroup.SelectedIndex = intCounter;

                        if (cboSelectGroup.SelectedItem.ToString() == strGroup)
                        {
                            intComboSelectedIndex = intCounter;
                        }
                    }

                    cboSelectGroup.SelectedIndex = intComboSelectedIndex;

                    intNumberOfRecords = cboSelectHomeOffice.Items.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectHomeOffice.SelectedIndex = intCounter;

                        if (cboSelectHomeOffice.SelectedItem.ToString() == strHomeOffice)
                        {
                            intComboSelectedIndex = intCounter;
                        }
                    }

                    cboSelectHomeOffice.SelectedIndex = intComboSelectedIndex;

                    intNumberOfRecords = cboSelectEmployeetype.Items.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployeetype.SelectedIndex = intCounter;

                        if (cboSelectEmployeetype.SelectedItem.ToString() == strEmployeeType)
                        {
                            intComboSelectedIndex = intCounter;
                        }
                    }

                    cboSelectEmployeetype.SelectedIndex = intComboSelectedIndex;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboSelectActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strActive;

            if(cboSelectActive.SelectedIndex > 0)
            {
                strActive = cboSelectActive.SelectedItem.ToString();

                if (strActive == "YES")

                    gblnActive = true;
                else
                    gblnActive = false;
            }
        }

        private void cboSelectGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectGroup.SelectedIndex > 0)
                gstrGroup = cboSelectGroup.SelectedItem.ToString();
        }

        private void cboSelectHomeOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectHomeOffice.SelectedIndex > 0)
                gstrHomeOffice = cboSelectHomeOffice.SelectedItem.ToString();
        }

        private void cboSelectEmployeetype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectEmployeetype.SelectedIndex > 0)
                gstrEmployeeType = cboSelectEmployeetype.SelectedItem.ToString();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetControls();
            this.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        
        private void ResetControls()
        {
            //local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {

                txtEmployeeID.Text = "";
                txtEnterLastName.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtPhoneNumber.Text = "";
                txtEmailAddress.Text = "";
                txtPayID.Text = "";
                txtEndDate.Text = "";
                txtStartDate.Text = "";
                cboSelectActive.Items.Clear();
                cboSelectEmployeetype.Items.Clear();
                cboSelectDepartment.Items.Clear();
                cboSelectEmployee.Items.Clear();
                cboSelectGroup.Items.Clear();
                cboSelectManager.Items.Clear();
                cboSelectSalaryType.Items.Clear();

                cboSelectActive.Items.Add("Select Active");
                cboSelectActive.Items.Add("YES");
                cboSelectActive.Items.Add("NO");
                cboSelectEmployeetype.Items.Add("Select Type");
                cboSelectEmployeetype.Items.Add("EMPLOYEE");
                cboSelectEmployeetype.Items.Add("CONTRACTOR");
                cboSelectHomeOffice.Items.Add("Select Warehouse");
                cboSelectGroup.Items.Add("Select Group");
                cboSelectSalaryType.Items.Add("Select Salary Type");
                cboSelectSalaryType.Items.Add("EXEMPT");
                cboSelectSalaryType.Items.Add("NON-EXEMPT");

                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectHomeOffice.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectHomeOffice.SelectedIndex = 0;

                TheEmployeeGroupDataSet = TheEmployeeClass.GetEmployeeGroupInfo();

                intNumberOfRecords = TheEmployeeGroupDataSet.employeegroup.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectGroup.Items.Add(TheEmployeeGroupDataSet.employeegroup[intCounter].GroupName);
                }

                cboSelectDepartment.Items.Add("Select Department");

                MainWindow.TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

                intNumberOfRecords = MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(MainWindow.TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
                }

               
                cboSelectManager.Items.Add("Select Manager");

                MainWindow.TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

                intNumberOfRecords = MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectManager.Items.Add(MainWindow.TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
                }

                cboSelectGroup.SelectedIndex = 0;
                cboSelectHomeOffice.SelectedIndex = 0;
                cboSelectEmployeetype.SelectedIndex = 0;
                cboSelectSalaryType.SelectedIndex = 0;
                cboSelectDepartment.SelectedIndex = 0;
                cboSelectManager.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }


        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intSelectedIndex;
            string strPhoneNumber;
            string strEmailAddress;
            int intPayID = 0;
            string strValueforValidation;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            
            try
            {
                intSelectedIndex = cboSelectActive.SelectedIndex;
                if(intSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Active Was Not Selected\n";
                }
                intSelectedIndex = cboSelectEmployee.SelectedIndex;
                if (intSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                intSelectedIndex = cboSelectEmployeetype.SelectedIndex;
                if (intSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Type Was Not Selected\n";
                }
                intSelectedIndex = cboSelectGroup.SelectedIndex;
                if (intSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Group Was Not Selected\n";
                }
                intSelectedIndex = cboSelectHomeOffice.SelectedIndex;
                if (intSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Home Office Was Not Selected\n";
                }
                MainWindow.gstrFirstName = txtFirstName.Text;
                if(MainWindow.gstrFirstName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "First Name Was Not Entered\n";
                }
                MainWindow.gstrLastName = txtLastName.Text;
                if(MainWindow.gstrLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Last Name Was Not Selected\n";
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Phone Number is not the Correct Format\n";
                }
                strValueforValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueforValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Start Date is not a Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueforValidation);
                }
                strValueforValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueforValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "End Date is not a Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueforValidation);
                }
                if (cboSelectSalaryType.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Salary Type Was Not Selected\n";
                }
                if(cboSelectDepartment.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Department Was Not Selected\n";
                }
                if(cboSelectManager.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Manager was not Selected\n";
                }
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerRange(txtPayID.Text);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Pay ID was not Entered\n";
                }
                else
                {
                    intPayID = Convert.ToInt32(txtPayID.Text);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strEmailAddress = txtEmailAddress.Text;
                if (strEmailAddress == "")
                {
                    strEmailAddress = " ";
                }

                MainWindow.gintEmployeeID = Convert.ToInt32(txtEmployeeID.Text);

                blnFatalError = TheEmployeeClass.UpdateEmployee(MainWindow.gintEmployeeID, MainWindow.gstrFirstName, MainWindow.gstrLastName, strPhoneNumber,  gblnActive, gstrGroup, gstrHomeOffice, gstrEmployeeType, strEmailAddress, gstrSalaryType, gstrDepartment, gintManagerID);

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

                ResetControls();

                TheMessagesClass.InformationMessage("Employee Updated");
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Employee // Save Menu Item " + Ex.Message);

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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
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
