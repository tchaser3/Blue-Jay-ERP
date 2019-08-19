/* Title:           Employee Hours Punched
 * Date:            8-147-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show all the employee punched reports */

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
using EmployeePunchedHoursDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeHoursPunched.xaml
    /// </summary>
    public partial class EmployeeHoursPunched : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindEmployeePunchesByManagerIDDataSet TheFindEmployeePunchesByManagerIDDataSet = new FindEmployeePunchesByManagerIDDataSet();
        FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();

        public EmployeeHoursPunched()
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
            //setting up variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                txtEndDate.Text = "";
                txtStartDate.Text = "";

                TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

                intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;
                cboSelectManager.Items.Clear();
                cboSelectManager.Items.Add("Select Manager");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectManager.Items.Add(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
                }

                cboSelectManager.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Hours Punched // Reset Controls");

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intManagerID = 0;
            int intSelectedIndex = 0;
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            if(cboSelectManager.SelectedIndex > 0)
            {
                try
                {
                    strValueForValidation = txtStartDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Start Date is not a Date\n";
                    }
                    else
                    {
                        datStartDate = Convert.ToDateTime(strValueForValidation);
                    }
                    strValueForValidation = txtEndDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The End Date is not a Date\n";
                    }
                    else
                    {
                        datEndDate = Convert.ToDateTime(strValueForValidation);
                    }
                    if (cboSelectManager.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Manager Was Not Selected\n";
                    }
                    else
                    {
                        intSelectedIndex = cboSelectManager.SelectedIndex - 1;
                        intManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
                    }
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }
                    else
                    {
                        blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);
                        if (blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The Start Date is after the End Date\n");
                            return;
                        }
                    }

                    TheFindEmployeePunchesByManagerIDDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchesByManagerID(intManagerID, datStartDate, datEndDate);

                    dgrResults.ItemsSource = TheFindEmployeePunchesByManagerIDDataSet.FindEmployeePunchesByManagerID;
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Hours Punched // Combo Box Selection " + Ex.Message);

                    TheMessagesClass.ErrorMessage(Ex.ToString());
                }
            }
        }

        private void MitExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheFindEmployeePunchesByManagerIDDataSet.FindEmployeePunchesByManagerID.Rows.Count;
                intColumnNumberOfRecords = TheFindEmployeePunchesByManagerIDDataSet.FindEmployeePunchesByManagerID.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindEmployeePunchesByManagerIDDataSet.FindEmployeePunchesByManagerID.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindEmployeePunchesByManagerIDDataSet.FindEmployeePunchesByManagerID.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Hours Punched // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
