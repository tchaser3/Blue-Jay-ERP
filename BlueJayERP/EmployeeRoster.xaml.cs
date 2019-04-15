/* Title:           Employee Roster
 * Date:            10-17-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the employee Roster for Blue Jay */

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
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeRoster.xaml
    /// </summary>
    public partial class EmployeeRoster : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindActiveEmployeesDataSet TheFindActiveEmployeesDataSet;
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        EmployeeRosterDataSet TheEmployeeRosterDataSet = new EmployeeRosterDataSet();

        public EmployeeRoster()
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
            LoadDataSet();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadDataSet();
        }
        private void LoadDataSet()
        {
            int intCounter;
            int intNumberOfRecords;
            int intManagerID;

            try
            {
                TheEmployeeRosterDataSet.employees.Rows.Clear();

                TheFindActiveEmployeesDataSet = TheEmployeeClass.FindActiveEmployees();

                intNumberOfRecords = TheFindActiveEmployeesDataSet.FindActiveEmployees.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intManagerID = TheFindActiveEmployeesDataSet.FindActiveEmployees[intCounter].ManagerID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                    EmployeeRosterDataSet.employeesRow NewEmployeeRow = TheEmployeeRosterDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.Department = TheFindActiveEmployeesDataSet.FindActiveEmployees[intCounter].Department;
                    NewEmployeeRow.EmployeeID = TheFindActiveEmployeesDataSet.FindActiveEmployees[intCounter].EmployeeID;
                    NewEmployeeRow.FirstName = TheFindActiveEmployeesDataSet.FindActiveEmployees[intCounter].FirstName;
                    NewEmployeeRow.HomeOffice = TheFindActiveEmployeesDataSet.FindActiveEmployees[intCounter].HomeOffice;
                    NewEmployeeRow.LastName = TheFindActiveEmployeesDataSet.FindActiveEmployees[intCounter].LastName;
                    NewEmployeeRow.ManagerFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    NewEmployeeRow.ManagerLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                    TheEmployeeRosterDataSet.employees.Rows.Add(NewEmployeeRow);
                }

                dgrResults.ItemsSource = TheEmployeeRosterDataSet.employees;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Roster // Employee Roster Load Data Set " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell EmployeeID;
            string strEmployeeID;

            try
            {
                if((MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "ADMIN") || (MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "IT"))
                {
                    if (dgrResults.SelectedIndex > -1)
                    {
                        //setting local variable
                        dataGrid = dgrResults;
                        selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                        EmployeeID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                        strEmployeeID = ((TextBlock)EmployeeID.Content).Text;

                        //find the record
                        MainWindow.gintEmployeeID = Convert.ToInt32(strEmployeeID);

                        EditEmployeeRoster EditEmployeeRoster = new EditEmployeeRoster();
                        EditEmployeeRoster.ShowDialog();

                        LoadDataSet();
                    }
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicles In Shop // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
                intRowNumberOfRecords = TheEmployeeRosterDataSet.employees.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeRosterDataSet.employees.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeRosterDataSet.employees.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeRosterDataSet.employees.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Roster // Export to Excel " + ex.Message);

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
