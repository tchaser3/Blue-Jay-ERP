/* Title:           Retired IT Assets
 * Date:            1-24-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show all retired assets */

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
using ItAssetsDLL;
using Microsoft.Win32;
using DateSearchDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for RetiredITAssets.xaml
    /// </summary>
    public partial class RetiredITAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();
        DateSearchClass TheDateSearchClas = new DateSearchClass();

        FindRetiredITAssetsDataSet TheFindRetiredAssetsDataSet = new FindRetiredITAssetsDataSet();

        public RetiredITAssets()
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

        private void MitExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

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
                intRowNumberOfRecords = TheFindRetiredAssetsDataSet.FindRetiredITAssets.Rows.Count;
                intColumnNumberOfRecords = TheFindRetiredAssetsDataSet.FindRetiredITAssets.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindRetiredAssetsDataSet.FindRetiredITAssets.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindRetiredAssetsDataSet.FindRetiredITAssets.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Retired IT Assets // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
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
            txtEndDate.Text = "";
            txtStartDate.Text = "";

            TheFindRetiredAssetsDataSet = TheITAssetsClass.FindRetiredITAssets(DateTime.Now, DateTime.Now);

            dgrResults.ItemsSource = TheFindRetiredAssetsDataSet.FindRetiredITAssets;
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;

            strValueForValidation = txtStartDate.Text;
            blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
            if(blnThereIsAProblem == true)
            {
                blnFatalError = true;
                strErrorMessage += "Start Date is not a Date\n";
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
                strErrorMessage += "End Date is not a Date\n";
            }
            else
            {
                MainWindow.gdatEndDate = Convert.ToDateTime(strValueForValidation);

                MainWindow.gdatEndDate = TheDateSearchClas.AddingDays(MainWindow.gdatEndDate, 1);
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
                    TheMessagesClass.ErrorMessage("The Starting Date is after the Ending Date");
                    return;
                }
            }

            TheFindRetiredAssetsDataSet = TheITAssetsClass.FindRetiredITAssets(MainWindow.gdatStartDate, MainWindow.gdatEndDate);

            dgrResults.ItemsSource = TheFindRetiredAssetsDataSet.FindRetiredITAssets;

        }
    }
}
