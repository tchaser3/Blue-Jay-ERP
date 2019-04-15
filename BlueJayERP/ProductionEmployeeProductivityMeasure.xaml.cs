/* Title:           Production Employee Productivity Measure
 * Date:            3-11-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used for finding employee productivity measure */

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
using DataValidationDLL;
using EmployeeProductivityStatsDLL;
using NewEmployeeDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ProductionEmployeeProductivityMeasure.xaml
    /// </summary>
    public partial class ProductionEmployeeProductivityMeasure : Window
    {
        //setting class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        FindProductionEmployeesProductivityMeasureDataSet TheFindProductionEmployeesProductivityMeasureDataSet = new FindProductionEmployeesProductivityMeasureDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        ProductivityMeassureDataSet TheProductivityMeassureDataSet = new ProductivityMeassureDataSet();

        public ProductionEmployeeProductivityMeasure()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            txtEndDate.Text = "";
            txtStartDate.Text = "";

            TheProductivityMeassureDataSet.productivitymeasure.Rows.Clear();

            dgrResults.ItemsSource = TheProductivityMeassureDataSet.productivitymeasure;
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strFullName;
            int intManagerID;
            decimal decProductionValue;

            try
            {
                TheProductivityMeassureDataSet.productivitymeasure.Rows.Clear();

                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatStartDate = Convert.ToDateTime(strValueForValidation);
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
                    MainWindow.gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(MainWindow.gdatStartDate, MainWindow.gdatEndDate);
                    
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                TheFindProductionEmployeesProductivityMeasureDataSet = TheEmployeeProductivityStatsClass.FindProductionEmployeesProductivityMeasure(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        intManagerID = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure[intCounter].ManagerID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                        strFullName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                        strFullName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                        decProductionValue = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure[intCounter].TotalProductivity;
                        decProductionValue = decProductionValue / 1000;
                        decProductionValue = Math.Round(decProductionValue, 2);

                        ProductivityMeassureDataSet.productivitymeasureRow NewProductionRow = TheProductivityMeassureDataSet.productivitymeasure.NewproductivitymeasureRow();

                        NewProductionRow.EmployeeID = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure[intCounter].EmployeeID;
                        NewProductionRow.FirstName = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure[intCounter].FirstName;
                        NewProductionRow.HomeOffice = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure[intCounter].HomeOffice;
                        NewProductionRow.LastName = TheFindProductionEmployeesProductivityMeasureDataSet.FindProductionEmployeesProductivityMeasure[intCounter].LastName;
                        NewProductionRow.Manager = strFullName;
                        NewProductionRow.TotalProductivity = decProductionValue;

                        TheProductivityMeassureDataSet.productivitymeasure.Rows.Add(NewProductionRow);
                    }
                }

                dgrResults.ItemsSource = TheProductivityMeassureDataSet.productivitymeasure;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Production Employee Productivity Measure // Generate Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell EmployeeID;
            string strEmployeeID;

            try
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

                    IndividualProductionEmployeeProductivity IndividualProductionEmployeeProductivity = new IndividualProductionEmployeeProductivity();
                    IndividualProductionEmployeeProductivity.ShowDialog();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Production Employee Productivity Measure // Grid Selection " + Ex.Message);

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
                intRowNumberOfRecords = TheProductivityMeassureDataSet.productivitymeasure.Rows.Count;
                intColumnNumberOfRecords = TheProductivityMeassureDataSet.productivitymeasure.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProductivityMeassureDataSet.productivitymeasure.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProductivityMeassureDataSet.productivitymeasure.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Production Employee Productivity Measure // Export to Excel " + ex.Message);

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
