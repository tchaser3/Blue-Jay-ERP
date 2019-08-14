/* Title:           Design Employee Productivity
 * Date:            7-10-19
 * Author:          Terry Holmes
 * 
 * Description:     This is the where the design employee productivity will be shown */

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
using DesignProductivityDLL;
using DataValidationDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DesignEmployeeProductivity.xaml
    /// </summary>
    public partial class DesignEmployeeProductivity : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DesignProductivityClass TheDesignProductivityClass = new DesignProductivityClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindDesignDepartmentProductivityByDateRangeDataSet TheFindDesignDepartmentProductivityByDateRangeDataSet = new FindDesignDepartmentProductivityByDateRangeDataSet();
        FindDesignEmployeeProductivityByDateRangeDataSet TheFindDesignEmployeeProductivityByDataRangeDataSet = new FindDesignEmployeeProductivityByDateRangeDataSet();
        FindDesignTotalDepartmentProductivityDataSet TheFindDesignTotalDepartmentProductivityDataSet = new FindDesignTotalDepartmentProductivityDataSet();
        FindDesignTotalEmployeeProductivityHoursDataSet TheFindDesignTotalEmployeeProductivityHoursDataSet = new FindDesignTotalEmployeeProductivityHoursDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        //setting global Variables
        string gstrReportType;
        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public DesignEmployeeProductivity()
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
            //setting local variables
            DateTime datStartDate = Convert.ToDateTime("01/01/1900");
            
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            txtEnterLastName.Text = "";
            txtEnterLastName.Visibility = Visibility.Hidden;
            lblEnterLastName.Visibility = Visibility.Hidden;
            cboSelectReportType.Items.Clear();
            cboSelectReportType.Items.Add("Select Report Type");
            cboSelectReportType.Items.Add("Design Department Productivity");
            cboSelectReportType.Items.Add("Design Employee Productivity");
            cboSelectReportType.Items.Add("Design Department Total Productivity");
            cboSelectReportType.SelectedIndex = 0;
            txtStartDate.Text = "";
            txtEndDate.Text = "";

            TheFindDesignTotalDepartmentProductivityDataSet = TheDesignProductivityClass.FindDesignTotalDepartmentProductivity(datStartDate, datStartDate);

            dgrResults.ItemsSource = TheFindDesignTotalDepartmentProductivityDataSet.FindDesignTotalDepartmentProductivity;
        }

        private void CboSelectReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectReportType.SelectedIndex == 1)
            {
                gstrReportType = "DEPARTMENT";
                lblEnterLastName.Visibility = Visibility.Hidden;
                txtEnterLastName.Visibility = Visibility.Hidden;
                lblSelectEmployee.Visibility = Visibility.Hidden;
                cboSelectEmployee.Visibility = Visibility.Hidden;
            }
            else if (cboSelectReportType.SelectedIndex == 2)
            {
                gstrReportType = "EMPLOYEE";
                lblEnterLastName.Visibility = Visibility.Visible;
                txtEnterLastName.Visibility = Visibility.Visible;
                lblSelectEmployee.Visibility = Visibility.Visible;
                cboSelectEmployee.Visibility = Visibility.Visible;
            }
            else if (cboSelectReportType.SelectedIndex == 3)
            {
                gstrReportType = "TOTAL";
                lblEnterLastName.Visibility = Visibility.Hidden;
                txtEnterLastName.Visibility = Visibility.Hidden;
                lblSelectEmployee.Visibility = Visibility.Hidden;
                cboSelectEmployee.Visibility = Visibility.Hidden;
            }
            
        }

        private void MitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            try
            {
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The end Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(gstrReportType == "EMPLOYEE")
                {
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gstrReportType == "DEPARTMENT")
                {
                    TheFindDesignDepartmentProductivityByDateRangeDataSet = TheDesignProductivityClass.FindDesignDepartmentProductivityByDateRange(gdatStartDate, gdatEndDate);

                    dgrResults.ItemsSource = TheFindDesignDepartmentProductivityByDateRangeDataSet.FindDesignDepartmentProductivityByDateRange;
                }
                else if(gstrReportType == "EMPLOYEE")
                {
                    TheFindDesignEmployeeProductivityByDataRangeDataSet = TheDesignProductivityClass.FindDesignEmployeeProductivityByDateRange(MainWindow.gintEmployeeID, gdatStartDate, gdatEndDate);

                    dgrResults.ItemsSource = TheFindDesignEmployeeProductivityByDataRangeDataSet.FindDesignEmployeeProductivityByDateRange;
                }
                else if(gstrReportType == "TOTAL")
                {
                    TheFindDesignTotalDepartmentProductivityDataSet = TheDesignProductivityClass.FindDesignTotalDepartmentProductivity(gdatStartDate, gdatEndDate);

                    dgrResults.ItemsSource = TheFindDesignTotalDepartmentProductivityDataSet.FindDesignTotalDepartmentProductivity;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Employee Productivity // Generate Report Button " + Ex.Message);

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

                if(intLength > 3)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Employee Productivity // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex = 0;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Employee Productivity // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (gstrReportType == "DEPARTMENT")
            {
                ExportDepartmentToExcel();
            }
            else if (gstrReportType == "EMPLOYEE")
            {
                ExportEmployeeToExcel();
            }
            else if (gstrReportType == "TOTAL")
            {
                ExportTotalToExcel();
            }
        }
        private void ExportDepartmentToExcel()
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
                intRowNumberOfRecords = TheFindDesignDepartmentProductivityByDateRangeDataSet.FindDesignDepartmentProductivityByDateRange.Rows.Count;
                intColumnNumberOfRecords = TheFindDesignDepartmentProductivityByDateRangeDataSet.FindDesignDepartmentProductivityByDateRange.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindDesignDepartmentProductivityByDateRangeDataSet.FindDesignDepartmentProductivityByDateRange.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindDesignDepartmentProductivityByDateRangeDataSet.FindDesignDepartmentProductivityByDateRange.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Employee Productivity // Export Department to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportEmployeeToExcel()
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
                intRowNumberOfRecords = TheFindDesignEmployeeProductivityByDataRangeDataSet.FindDesignEmployeeProductivityByDateRange.Rows.Count;
                intColumnNumberOfRecords = TheFindDesignEmployeeProductivityByDataRangeDataSet.FindDesignEmployeeProductivityByDateRange.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindDesignEmployeeProductivityByDataRangeDataSet.FindDesignEmployeeProductivityByDateRange.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindDesignEmployeeProductivityByDataRangeDataSet.FindDesignEmployeeProductivityByDateRange.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Employee Productivity // Export Employee to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportTotalToExcel()
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
                intRowNumberOfRecords = TheFindDesignTotalDepartmentProductivityDataSet.FindDesignTotalDepartmentProductivity.Rows.Count;
                intColumnNumberOfRecords = TheFindDesignTotalDepartmentProductivityDataSet.FindDesignTotalDepartmentProductivity.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindDesignTotalDepartmentProductivityDataSet.FindDesignTotalDepartmentProductivity.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindDesignTotalDepartmentProductivityDataSet.FindDesignTotalDepartmentProductivity.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Design Employee Productivity // Export Total to Excel " + ex.Message);

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
