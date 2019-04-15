/* Title:           Import Trailers
 * Date:            9-19-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to import trailers */

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
using TrailersDLL;
using TrailerCategoryDLL;
using NewEmployeeDLL;
using Excel = Microsoft.Office.Interop.Excel;


namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportTrailers.xaml
    /// </summary>
    public partial class ImportTrailers : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        TrailerCategoryClass TheTrailerCategoryClass = new TrailerCategoryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindTrailerCategoryByCategoryDataSet TheFindTrailerCategoryByCategoryDataSet = new FindTrailerCategoryByCategoryDataSet();
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();
        NewTrailerDataSet TheNewTrailerDataSet = new NewTrailerDataSet();

        string gstrTrailerNumber;
        string gstrAssignedOffice;
        string gstrTrailerDescription = "";
        string gstrTrailerCategory = "";
        int gintCategoryID = 0;
        string gstrLicensePlate = "";
        int gintLocationID = 0;
        string gstrVINNumber = "";

        public ImportTrailers()
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

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError;
            
            try
            {
                intNumberOfRecords = TheNewTrailerDataSet.newtrailers.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gstrTrailerNumber = TheNewTrailerDataSet.newtrailers[intCounter].TrailerNumber;
                    gintCategoryID = TheNewTrailerDataSet.newtrailers[intCounter].CategoryID;
                    gstrVINNumber = TheNewTrailerDataSet.newtrailers[intCounter].VINNumber;
                    gstrTrailerDescription = TheNewTrailerDataSet.newtrailers[intCounter].TrailerDescription;
                    gstrLicensePlate = TheNewTrailerDataSet.newtrailers[intCounter].LicensePlate;
                    gintLocationID = TheNewTrailerDataSet.newtrailers[intCounter].LocationID;

                    blnFatalError = TheTrailersClass.InsertTrailer(gstrTrailerNumber, gintCategoryID, gstrVINNumber, gstrTrailerDescription, gstrLicensePlate, gintLocationID, gintLocationID, "NO NOTES REPORTED");

                    if (blnFatalError == true)
                        throw new Exception();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Trailers // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitImportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            

            try
            {
                TheNewTrailerDataSet.newtrailers.Rows.Clear();

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

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {

                    gstrTrailerNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);

                    gstrTrailerNumber = gstrTrailerNumber.Substring(4);

                    TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(gstrTrailerNumber);

                    intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        gstrAssignedOffice = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                        gstrTrailerDescription = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2).ToUpper();
                        gstrTrailerCategory = Convert.ToString((range.Cells[intCounter, 4] as Excel.Range).Value2).ToUpper();
                        gstrLicensePlate = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                        gstrVINNumber = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();

                        gintLocationID = FindLocationID(gstrAssignedOffice);

                        TheFindTrailerCategoryByCategoryDataSet = TheTrailerCategoryClass.FindTrailerCategoryByCategory(gstrTrailerCategory);

                        gintCategoryID = TheFindTrailerCategoryByCategoryDataSet.FindTrailerCategoryByCategory[0].CategoryID;

                        NewTrailerDataSet.newtrailersRow NewTrailerRow = TheNewTrailerDataSet.newtrailers.NewnewtrailersRow();

                        NewTrailerRow.CategoryID = gintCategoryID;
                        NewTrailerRow.LicensePlate = gstrLicensePlate;
                        NewTrailerRow.Location = gstrAssignedOffice;
                        NewTrailerRow.LocationID = gintLocationID;
                        NewTrailerRow.TrailerCategory =gstrTrailerCategory;
                        NewTrailerRow.TrailerDescription = gstrTrailerDescription;
                        NewTrailerRow.TrailerNumber = gstrTrailerNumber;
                        NewTrailerRow.VINNumber = gstrVINNumber;

                        TheNewTrailerDataSet.newtrailers.Rows.Add(NewTrailerRow);
                       
                    }

                }

                PleaseWait.Close();
                dgrTrailers.ItemsSource = TheNewTrailerDataSet.newtrailers;
                mitSave.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Trailers // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private int FindLocationID(string strAssignedOffice)
        {
            int intLocationID = 0;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(strAssignedOffice == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        intLocationID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Trailers // Find Location ID " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            return intLocationID;
        }
        private void ResetControls()
        {
            mitSave.IsEnabled = false;

            TheNewTrailerDataSet.newtrailers.Rows.Clear();

            dgrTrailers.ItemsSource = TheNewTrailerDataSet.newtrailers;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
    }
}
