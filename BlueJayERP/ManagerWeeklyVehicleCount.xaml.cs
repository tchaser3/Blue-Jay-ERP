/* Title:           Manager Weekly Vehicle Count
 * Date:            3-6-19
 * Author:          Terry Holmes
 * 
 * Description:     This is the window for the manager weekly vehicle count */

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
using WeeklyInspectionsDLL;
using NewEventLogDLL;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ManagerWeeklyVehicleCount.xaml
    /// </summary>
    public partial class ManagerWeeklyVehicleCount : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WeeklyInspectionClass TheWeeklyInspectionClass = new WeeklyInspectionClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindWeeklyVehicleInspectionsCountDataSet TheFindWeeklyVehicleInspectionsCountDataSet = new FindWeeklyVehicleInspectionsCountDataSet();
        ManagerVehicleCountDataSet TheManagerVehicleCountDataSet = new ManagerVehicleCountDataSet();

        int gintEmployeeCounter;
        int gintEmployeeNumberOfRecords;

        public ManagerWeeklyVehicleCount()
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
            TheManagerVehicleCountDataSet.vehiclecount.Rows.Clear();
            dgrResults.ItemsSource = TheManagerVehicleCountDataSet.vehiclecount;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        { 
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsNoProblem = false;
            bool blnItemFound;
            int intEmployeeID;
            int intEmployeeCounter;

            try
            {
                TheManagerVehicleCountDataSet.vehiclecount.Rows.Clear();
                strValueForValidation = txtStartDate.Text;
                blnThereIsNoProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsNoProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnFatalError == true)
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

                TheFindWeeklyVehicleInspectionsCountDataSet = TheWeeklyInspectionClass.FindWeeklyVehicleInspectionCount(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindWeeklyVehicleInspectionsCountDataSet.FindWeeklyVehicleInspectionsCount.Rows.Count - 1;
                gintEmployeeCounter = 0;
                gintEmployeeNumberOfRecords = 0;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        intEmployeeID = TheFindWeeklyVehicleInspectionsCountDataSet.FindWeeklyVehicleInspectionsCount[intCounter].EmployeeID;
                        
                        if(gintEmployeeCounter > 0)
                        {
                            for(intEmployeeCounter = 0; intEmployeeCounter <= gintEmployeeNumberOfRecords; intEmployeeCounter++)
                            {
                                if(intEmployeeID == TheManagerVehicleCountDataSet.vehiclecount[intEmployeeCounter].EmployeeID)
                                {
                                    TheManagerVehicleCountDataSet.vehiclecount[intEmployeeCounter].VehiclesInspected++;
                                    blnItemFound = true;
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            ManagerVehicleCountDataSet.vehiclecountRow NewEmployee = TheManagerVehicleCountDataSet.vehiclecount.NewvehiclecountRow();

                            NewEmployee.EmployeeID = intEmployeeID;
                            NewEmployee.FirstName = TheFindWeeklyVehicleInspectionsCountDataSet.FindWeeklyVehicleInspectionsCount[intCounter].FirstName;
                            NewEmployee.LastName = TheFindWeeklyVehicleInspectionsCountDataSet.FindWeeklyVehicleInspectionsCount[intCounter].LastName;
                            NewEmployee.VehiclesInspected = 1;

                            TheManagerVehicleCountDataSet.vehiclecount.Rows.Add(NewEmployee);
                            gintEmployeeNumberOfRecords = gintEmployeeCounter;
                            gintEmployeeCounter++;
                        }
                    }
                }

                LoadChart();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Vehicle Count // Generate Report Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void LoadChart()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                Chart chart = this.FindName("MyWinformChart") as Chart;
                chart.Legends.Clear();
                Series VehicleCount = chart.Series["VehicleCount"];
                chart.Legends.Add(new Legend("VehicleCount"));

                VehicleCount.Points.Clear();
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart.ChartAreas[0].AxisX.Interval = 15;

                intNumberOfRecords = TheManagerVehicleCountDataSet.vehiclecount.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    VehicleCount.Points.Add(TheManagerVehicleCountDataSet.vehiclecount[intCounter].VehiclesInspected);


                    chart.ChartAreas[0].AxisX.CustomLabels.Add(intCounter + .5, intCounter + 1.5, TheManagerVehicleCountDataSet.vehiclecount[intCounter].LastName, 1, LabelMarkStyle.None);
                }
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Vehicle Count // Load Chart " + Ex.Message);

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
                intRowNumberOfRecords = TheManagerVehicleCountDataSet.vehiclecount.Rows.Count;
                intColumnNumberOfRecords = TheManagerVehicleCountDataSet.vehiclecount.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheManagerVehicleCountDataSet.vehiclecount.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheManagerVehicleCountDataSet.vehiclecount.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Vehicle Count // Export to Excel " + ex.Message);

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
