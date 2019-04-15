/* Title:           Warehouse Inventory Stats
 * Date:            10-11-18
 * Author:          Terrance Holmes
 * 
 * Description:     This is the window that will do stuff */

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
using DateSearchDLL;
using InventoryDLL;
using InventoryStatsDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using NewPartNumbersDLL;
using Microsoft.Win32;
using excel = Microsoft.Office.Interop;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for WarehouseInventoryStats.xaml
    /// </summary>
    public partial class WarehouseInventoryStats : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        InventoryStatsClass TheInventoryStatsClass = new InventoryStatsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();

        //setting up the data
        FindWarehouseInventoryDataSet TheFindWarehouseInventoryDataSet = new FindWarehouseInventoryDataSet();
        FindInventoryIssueStatsDataSet TheFindInventoryIssueStatsDataSet = new FindInventoryIssueStatsDataSet();
        FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        CalculatedInventoryStatsDataSet TheCalculatedInventoryStatsDataSet = new CalculatedInventoryStatsDataSet();
        CalculatedInventoryStatsDataSet TheFinalCalculatedInventoryStatsDataSet = new CalculatedInventoryStatsDataSet();

        public WarehouseInventoryStats()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBox();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadComboBox();
        }
        private void LoadComboBox()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectWarehouse.Items.Clear();
                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();
                cboSelectWarehouse.Items.Add("Select Warehouse");
                intNumberOfRecords = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;

                TheFinalCalculatedInventoryStatsDataSet.inventorystats.Rows.Clear();

                dgrResults.ItemsSource = TheFinalCalculatedInventoryStatsDataSet.inventorystats;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Warehouse Inventory Stats // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intWarehouseID;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            DateTime datTodaysDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intPartID;
            int intLoop;
            string strPartNumber;
            double douAverage;
            double douSTDev;
            int intCount;
            bool blnDoNotCopy;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    TheCalculatedInventoryStatsDataSet.inventorystats.Rows.Clear();
                    TheFinalCalculatedInventoryStatsDataSet.inventorystats.Rows.Clear();

                    intWarehouseID = TheFindPartsWarehouseDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                    TheFindWarehouseInventoryDataSet = TheInventoryClass.FindWarehouseInventory(intWarehouseID);

                    intNumberOfRecords = TheFindWarehouseInventoryDataSet.FindWarehouseInventory.Rows.Count - 1;

                    datTodaysDate = TheDateSearchClass.RemoveTime(datTodaysDate);


                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        datStartDate = TheDateSearchClass.SubtractingDays(datTodaysDate, 90);
                        datEndDate = TheDateSearchClass.AddingDays(datStartDate, 30);
                        intLoop = 1;

                        strPartNumber = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].PartNumber;

                        TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                        intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;

                        CalculatedInventoryStatsDataSet.inventorystatsRow NewInventoryRow = TheCalculatedInventoryStatsDataSet.inventorystats.NewinventorystatsRow();

                        NewInventoryRow.PartID = intPartID;
                        NewInventoryRow.JDEPartNumber = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].JDEPartNumber;
                        NewInventoryRow.PartDescription = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intCounter].PartDescription;
                        NewInventoryRow.PartNumber = strPartNumber;

                        while (datStartDate < datTodaysDate)
                        {
                            TheFindInventoryIssueStatsDataSet = TheInventoryStatsClass.FindInventoryIssueStats(intPartID, intWarehouseID, datStartDate, datEndDate);

                            intRecordsReturned = TheFindInventoryIssueStatsDataSet.FindInventoryIssueStats.Rows.Count;

                            if (intRecordsReturned == 0)
                            {
                                douAverage = 0;
                                douSTDev = 0;
                                intCount = 0;
                            }
                            else
                            {
                                if (TheFindInventoryIssueStatsDataSet.FindInventoryIssueStats[0].IsAverageIssuedNull() == true)
                                {
                                    douAverage = 0;
                                }
                                else
                                {
                                    douAverage = TheFindInventoryIssueStatsDataSet.FindInventoryIssueStats[0].AverageIssued;
                                }
                                if (TheFindInventoryIssueStatsDataSet.FindInventoryIssueStats[0].IsIssuedSTDEVNull() == true)
                                {
                                    douSTDev = 0;
                                }
                                else
                                {
                                    douSTDev = TheFindInventoryIssueStatsDataSet.FindInventoryIssueStats[0].IssuedSTDEV;
                                }

                                intCount = TheFindInventoryIssueStatsDataSet.FindInventoryIssueStats[0].TotalIssued;
                            }

                            if (intLoop == 1)
                            {
                                NewInventoryRow.PeriodOneAvg = douAverage;
                                NewInventoryRow.PeriodOneCount = intCount;
                                NewInventoryRow.PeriodOneSD = douSTDev;
                                NewInventoryRow.PeriodOneStarDate = datStartDate;
                            }
                            else if (intLoop == 2)
                            {
                                NewInventoryRow.PeriodTwoAvg = douAverage;
                                NewInventoryRow.PeriodTwoCount = intCount;
                                NewInventoryRow.PeriodTwoSD = douSTDev;
                                NewInventoryRow.PeriodTwoStartDate = datStartDate;
                            }
                            else if (intLoop == 3)
                            {
                                NewInventoryRow.PeriodThreeAvg = douAverage;
                                NewInventoryRow.PeriodThreeCount = intCount;
                                NewInventoryRow.PeriodThreeSD = douSTDev;
                                NewInventoryRow.PeriodThreeStartDate = datStartDate;
                            }

                            datStartDate = datEndDate;
                            datEndDate = TheDateSearchClass.AddingDays(datEndDate, 30);
                            intLoop++;
                        }

                        TheCalculatedInventoryStatsDataSet.inventorystats.Rows.Add(NewInventoryRow);
                    }

                    intNumberOfRecords = TheCalculatedInventoryStatsDataSet.inventorystats.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnDoNotCopy = false;

                        if (TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodOneAvg == 0)
                        {
                            if (TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodTwoAvg == 0)
                            {
                                if (TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodThreeAvg == 0)
                                {
                                    blnDoNotCopy = true;
                                }
                            }
                        }

                        if (blnDoNotCopy == false)
                        {
                            CalculatedInventoryStatsDataSet.inventorystatsRow NewStatsRow = TheFinalCalculatedInventoryStatsDataSet.inventorystats.NewinventorystatsRow();

                            NewStatsRow.JDEPartNumber = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].JDEPartNumber;
                            NewStatsRow.PartDescription = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PartDescription;
                            NewStatsRow.PartID = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PartID;
                            NewStatsRow.PartNumber = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PartNumber;
                            NewStatsRow.PeriodOneAvg = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodOneAvg;
                            NewStatsRow.PeriodOneCount = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodOneCount;
                            NewStatsRow.PeriodOneSD = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodOneSD;
                            NewStatsRow.PeriodThreeAvg = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodThreeAvg;
                            NewStatsRow.PeriodThreeCount = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodThreeCount;
                            NewStatsRow.PeriodThreeSD = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodThreeSD;
                            NewStatsRow.PeriodTwoAvg = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodTwoAvg;
                            NewStatsRow.PeriodTwoCount = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodTwoCount;
                            NewStatsRow.PeriodTwoSD = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodTwoSD;
                            NewStatsRow.PeriodOneStarDate = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodOneStarDate;
                            NewStatsRow.PeriodThreeStartDate = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodThreeStartDate;
                            NewStatsRow.PeriodTwoStartDate = TheCalculatedInventoryStatsDataSet.inventorystats[intCounter].PeriodTwoStartDate;

                            TheFinalCalculatedInventoryStatsDataSet.inventorystats.Rows.Add(NewStatsRow);
                        }
                    }

                    dgrResults.ItemsSource = TheFinalCalculatedInventoryStatsDataSet.inventorystats;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Inventory Stats // Main Window // Combo Box Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
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
                intRowNumberOfRecords = TheFinalCalculatedInventoryStatsDataSet.inventorystats.Rows.Count;
                intColumnNumberOfRecords = TheFinalCalculatedInventoryStatsDataSet.inventorystats.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFinalCalculatedInventoryStatsDataSet.inventorystats.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFinalCalculatedInventoryStatsDataSet.inventorystats.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Warehouse Inventory Stats // Export to Excel " + ex.Message);

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
