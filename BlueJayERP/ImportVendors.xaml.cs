/* Title:           Import Vendors
 * Date:            2-6-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import the vendors */

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
using VendorsDLL;
using DataValidationDLL;

using Excel = Microsoft.Office.Interop.Excel;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportVendors.xaml
    /// </summary>
    public partial class ImportVendors : Window
    {
        //set up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up data
        VendorImportDataSet TheVendorImportDataSet = new VendorImportDataSet();
        FindVendorByVendorNameDataSet TheFindVendorByVendorNameDataSet = new FindVendorByVendorNameDataSet();

        public ImportVendors()
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
            mitProcess.IsEnabled = false;
            TheVendorImportDataSet.vendorimport.Rows.Clear();
            dgrResults.ItemsSource = TheVendorImportDataSet.vendorimport;
        }

        private void MitImportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strVendorName;
            string strContactName = "";
            string strPhoneNumber = "";
            int intRecordsReturned;

            try
            {
                TheVendorImportDataSet.vendorimport.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {

                    strVendorName = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);
                    strVendorName = strVendorName.ToUpper();

                    TheFindVendorByVendorNameDataSet = TheVendorsClass.FindVendorByVendorName(strVendorName);

                    intRecordsReturned = TheFindVendorByVendorNameDataSet.FindVendorByVendorName.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        strContactName = (range.Cells[intCounter, 2] as Excel.Range).Value2.ToUpper();
                        strPhoneNumber = (range.Cells[intCounter, 3] as Excel.Range).Value2.ToUpper();

                        VendorImportDataSet.vendorimportRow NewVendorRow = TheVendorImportDataSet.vendorimport.NewvendorimportRow();

                        NewVendorRow.ContactName = strContactName;
                        NewVendorRow.PhoneNumber = strPhoneNumber;
                        NewVendorRow.VendorName = strVendorName;

                        TheVendorImportDataSet.vendorimport.Rows.Add(NewVendorRow);
                      
                    }

                }

                PleaseWait.Close();
                dgrResults.ItemsSource = TheVendorImportDataSet.vendorimport;
                mitProcess.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Vendors // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strVendorName;
            string strContactName;
            string strPhoneNumber;
            bool blnFatalError;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intNumberOfRecords = TheVendorImportDataSet.vendorimport.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strVendorName = TheVendorImportDataSet.vendorimport[intCounter].VendorName;
                    strContactName = TheVendorImportDataSet.vendorimport[intCounter].ContactName;
                    strPhoneNumber = TheVendorImportDataSet.vendorimport[intCounter].PhoneNumber;

                    blnFatalError = TheVendorsClass.InsertNewVendor(strVendorName, strContactName, strPhoneNumber);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Vendors Have Been Added");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
