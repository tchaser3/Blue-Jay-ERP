/* Title:           Employee Labor Rate
 * Date:            11-28-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to either add or edit an employee labor rate */

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
using EmployeeLaborRateDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeLaborRate.xaml
    /// </summary>
    public partial class EmployeeLaborRate : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeLaborRateClass TheEmployeeLaborRateClass = new EmployeeLaborRateClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeLaborRateDataSet TheFindEmployeeLaborRateDataSet = new FindEmployeeLaborRateDataSet();

        bool gblnNewRecord;

        public EmployeeLaborRate()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
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
        private void ResetControls()
        {
            txtLaborRate.Text = "";
            cboSelectEmployee.Items.Clear();
            txtLastName.Text = "";
        }

        private void TxtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Labor Rate // txtLastName Text Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    TheFindEmployeeLaborRateDataSet = TheEmployeeLaborRateClass.FindEmployeeLaborRate(MainWindow.gintEmployeeID);

                    intRecordsReturned = TheFindEmployeeLaborRateDataSet.FindEmployeeLaborRate.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        gblnNewRecord = true;
                        txtLaborRate.Text = "None Found";
                    }
                    else if(intRecordsReturned > 0)
                    {
                        gblnNewRecord = false;
                        txtLaborRate.Text = Convert.ToString(TheFindEmployeeLaborRateDataSet.FindEmployeeLaborRate[0].PayRate);
                    }
                        
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Labor Rate // Combo Selection Changed Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting up variables
            decimal decPayRate = 0;
            string strValueForValidation;
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                strValueForValidation = txtLaborRate.Text;
                blnFatalError = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnFatalError == true)
                {
                    strErrorMessage += "The Pay Rate is not Numeric\n";
                }
                else
                {
                    decPayRate = Convert.ToDecimal(strValueForValidation);

                    if(decPayRate < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Pay Rate is less than 1\n";
                    }
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnNewRecord == true)
                {
                    blnFatalError = TheEmployeeLaborRateClass.InsertEmployeeLaborRate(MainWindow.gintEmployeeID, decPayRate);
                }
                else if(gblnNewRecord == false)
                {
                    blnFatalError = TheEmployeeLaborRateClass.UpdateEmployeeLaborRate(MainWindow.gintEmployeeID, decPayRate);
                }

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Employee Has Been Updated");

                ResetControls();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Labor Rate // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
