/* Title:           Current IT Assets
 * Date:            1-24-19
 * Author:          Terry Holmes
 * 
 * Description:     This is the Asset Reports */

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
using ItAssetsDLL;
using NewEventLogDLL;
using Microsoft.Win32;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for CurrentITAssets.xaml
    /// </summary>
    public partial class CurrentITAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindActiveITAssetsDataSet TheFindActiveITAssetsDataSet = new FindActiveITAssetsDataSet();
        FindCurrentITAssetsByOfficeDataSet TheFindCurrentITAssetsByOfficeDataSet = new FindCurrentITAssetsByOfficeDataSet();
        ReportedITAssetsDataSet TheReportedITAssetsDataSet = new ReportedITAssetsDataSet();

        public CurrentITAssets()
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;
            string strReportType;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;
                cboSelectReportType.Items.Clear();
                cboSelectReportType.Items.Add("Select Report");
                cboSelectReportType.Items.Add("All IT Assets");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strReportType = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName + " Assets";
                    cboSelectReportType.Items.Add(strReportType);
                }

                cboSelectReportType.SelectedIndex = 0;
                LoadAllAssets();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Current IT Assets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadAllAssets()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheReportedITAssetsDataSet.itassets.Rows.Clear();
                TheFindActiveITAssetsDataSet = TheITAssetsClass.FindActiveITAssets();
                intNumberOfRecords = TheFindActiveITAssetsDataSet.FindActiveITAssets.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ReportedITAssetsDataSet.itassetsRow NewAssetRow = TheReportedITAssetsDataSet.itassets.NewitassetsRow();

                        NewAssetRow.Item = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].Item;
                        NewAssetRow.ItemID = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].ItemID;
                        NewAssetRow.ItemQuantity = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].ItemQuantity;
                        NewAssetRow.Manufacturer = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].Manufacturer;
                        NewAssetRow.Model = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].Model;
                        NewAssetRow.Office = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].FirstName;
                        NewAssetRow.SerialNumber = TheFindActiveITAssetsDataSet.FindActiveITAssets[intCounter].SerialNumber;

                        TheReportedITAssetsDataSet.itassets.Rows.Add(NewAssetRow);
                    }
                }

                dgrResults.ItemsSource = TheReportedITAssetsDataSet.itassets;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Current IT Assets // Load All Assets " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheReportedITAssetsDataSet.itassets.Rows.Count;
                intColumnNumberOfRecords = TheReportedITAssetsDataSet.itassets.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheReportedITAssetsDataSet.itassets.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheReportedITAssetsDataSet.itassets.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                PleaseWait.Close();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Current IT Assets // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void CboSelectReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strOffice;

            try
            {
                intSelectedIndex = cboSelectReportType.SelectedIndex;

                if (intSelectedIndex > -1)
                {
                    if(intSelectedIndex == 1)
                    {
                        LoadAllAssets();
                    }
                    else if(intSelectedIndex > 1)
                    {
                        strOffice = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex - 2].FirstName;

                        LoadOfficeITAssets(strOffice);
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Current IT Assets // Combo Box Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadOfficeITAssets(string strOffice)
        {
            int intCounter;
            int intNumberOfRecords;

            TheFindCurrentITAssetsByOfficeDataSet = TheITAssetsClass.FindCurrentITAssetsByOffice(strOffice);
            TheReportedITAssetsDataSet.itassets.Rows.Clear();
            intNumberOfRecords = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice.Rows.Count- 1;

            if (intNumberOfRecords > -1)
            {
                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    ReportedITAssetsDataSet.itassetsRow NewAssetRow = TheReportedITAssetsDataSet.itassets.NewitassetsRow();

                    NewAssetRow.Item = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice[intCounter].Item;
                    NewAssetRow.ItemID = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice[intCounter].ItemID;
                    NewAssetRow.ItemQuantity = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice[intCounter].ItemQuantity;
                    NewAssetRow.Manufacturer = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice[intCounter].Manufacturer;
                    NewAssetRow.Model = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice[intCounter].Model;
                    NewAssetRow.Office = strOffice;
                    NewAssetRow.SerialNumber = TheFindCurrentITAssetsByOfficeDataSet.FindCurrentITAssetsByOffice[intCounter].SerialNumber;

                    TheReportedITAssetsDataSet.itassets.Rows.Add(NewAssetRow);
                }
            }

            dgrResults.ItemsSource = TheReportedITAssetsDataSet.itassets;
        }
    }
}
