/* Title:           Employee Punched Vs Production Hours
 * Date:            10-15-18
 * Author:          Terry HOlmes
 * 
 * Description:     This form will show the report of production hours vs punched hours */

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
using EmployeeProjectAssignmentDLL;
using EmployeePunchedHoursDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using DataValidationDLL;
using Microsoft.Win32;
using EmployeeDateEntryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeePunchedVSProductionHours.xaml
    /// </summary>
    public partial class EmployeePunchedVSProductionHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        EmployeeProductionPunchDataSet TheEmployeeProductionPunchDataSet = new EmployeeProductionPunchDataSet();
        FindActiveNonExemptEmployeesByPayDateDataSet TheFindActiveNoExemptEmployeesDataSet = new FindActiveNonExemptEmployeesByPayDateDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindEmployeeProductionHoursOverPayPeriodDataSet TheFindEmployeeProductionHoursOverPayPeriodDataSet = new FindEmployeeProductionHoursOverPayPeriodDataSet();
        FindEmployeePunchedHoursDataSet TheFindEmployeePunchedHoursDataSet = new FindEmployeePunchedHoursDataSet();

        //setting global date variables

        public EmployeePunchedVSProductionHours()
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
            TheMessagesClass.MyOpenTasks();
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

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intManagerID;
            string strFirstName;
            string strLastName;
            string strManagerFirstName;
            string strManagerLastName;
            int intRecordReturned;
            bool blnFatalError = false;
            decimal decHoursPunched;
            decimal decProductiveHours;

            try
            {
                TheEmployeeProductionPunchDataSet.employees.Rows.Clear();

                blnFatalError = TheDataValidationClass.VerifyDateData(txtEnterPayPeriod.Text);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is not a Date");
                    return;
                }
                else
                {
                    MainWindow.gdatEndDate = Convert.ToDateTime(txtEnterPayPeriod.Text);
                    MainWindow.gdatStartDate = TheDateSearchClass.SubtractingDays(MainWindow.gdatEndDate, 6);
                }

                TheFindActiveNoExemptEmployeesDataSet = TheEmployeeClass.FindActiveNonExemptEmployeesByPayDate(MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //getting employee info
                    intEmployeeID = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].EmployeeID;
                    strFirstName = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].FirstName;
                    strLastName = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].LastName;
                    intManagerID = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].ManagerID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                    strManagerFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    strManagerLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                    //getting employee punched hours
                    TheFindEmployeePunchedHoursDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHours(intEmployeeID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    intRecordReturned = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours.Rows.Count;

                    if(intRecordReturned == 0)
                    {
                        decHoursPunched = 0;
                    }
                    else
                    {
                        decHoursPunched = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours[0].PunchedHours;
                    }

                    //getting production hours
                    TheFindEmployeeProductionHoursOverPayPeriodDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProductionHoursOverPayPeriodDataSet(intEmployeeID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    intRecordReturned = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod.Rows.Count;

                    if(intRecordReturned == 0)
                    {
                        decProductiveHours = 0;
                    }
                    else
                    {
                        decProductiveHours = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod[0].ProductionHours;
                    }

                    //loading the dataset
                    EmployeeProductionPunchDataSet.employeesRow NewEmployeeRow = TheEmployeeProductionPunchDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.HomeOffice = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].HomeOffice;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.ManagerFirstName = strManagerFirstName;
                    NewEmployeeRow.ManagerLastName = strManagerLastName;
                    NewEmployeeRow.ProductionHours = decProductiveHours;
                    NewEmployeeRow.PunchedHours = decHoursPunched;
                    NewEmployeeRow.HourVariance = decProductiveHours - decHoursPunched;

                    TheEmployeeProductionPunchDataSet.employees.Rows.Add(NewEmployeeRow);
                }

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Employee Punched VS Production Hours // Generate Report Menu Item");

                if (blnFatalError == true)
                    throw new Exception();

                dgrResults.ItemsSource = TheEmployeeProductionPunchDataSet.employees;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Punched VS Production Hours // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExportToExcel_Click(object sender, RoutedEventArgs e)
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
                intRowNumberOfRecords = TheEmployeeProductionPunchDataSet.employees.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeProductionPunchDataSet.employees.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProductionPunchDataSet.employees.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProductionPunchDataSet.employees.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Multiple Employee Hours // Export to Excel " + ex.Message);

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
