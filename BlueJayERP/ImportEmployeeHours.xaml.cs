/* Title:           Import Employee Hours
 * Date:            10-15-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to import employee hours */

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
using EmployeePunchedHoursDLL;
using Excel = Microsoft.Office.Interop.Excel;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportEmployeeHours.xaml
    /// </summary>
    public partial class ImportEmployeeHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        ImportHoursDataSet TheImportHoursDataSet = new ImportHoursDataSet();
        FindEmployeeByPayIDDataSet TheFindEmployeeByPayIDDataSet = new FindEmployeeByPayIDDataSet();

        public ImportEmployeeHours()
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

        private void mitImportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intPayID = 0;
            decimal decHours = 0;
            string strValueForValidation;
            string strEmployeeName;
            bool blnNextRecord;
            bool blnFatalError;

            try
            {
                TheImportHoursDataSet.employees.Rows.Clear();

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
                    blnNextRecord = true;
                    strEmployeeName = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);
                    if(strEmployeeName == "")
                    {
                        blnNextRecord = false;
                    }
                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if(blnFatalError == true)
                    {
                        blnNextRecord = false;
                    }
                    else
                    {
                        intPayID = Convert.ToInt32(strValueForValidation);
                    }
                    strValueForValidation = Convert.ToString((range.Cells[intCounter, 3] as Excel.Range).Value2);
                    blnFatalError = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                    if (blnFatalError == true)
                    {
                        blnNextRecord = false;
                    }
                    else
                    {
                        decHours = Convert.ToDecimal(strValueForValidation);
                    }

                    if(blnNextRecord == true)
                    {
                        TheFindEmployeeByPayIDDataSet = TheEmployeeClass.FindEmployeeByPayID(intPayID);

                        ImportHoursDataSet.employeesRow NewEmployeeRow = TheImportHoursDataSet.employees.NewemployeesRow();

                        NewEmployeeRow.EmployeeID = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].EmployeeID;
                        NewEmployeeRow.FirstName = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].FirstName;
                        NewEmployeeRow.LastName = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID[0].LastName;
                        NewEmployeeRow.PayID = intPayID;
                        NewEmployeeRow.Hours = decHours;

                        TheImportHoursDataSet.employees.Rows.Add(NewEmployeeRow);
                    }
                    
                }

                PleaseWait.Close();
                dgrResults.ItemsSource = TheImportHoursDataSet.employees;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Import Employee Hours // Import Excel Menu Item ");

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Employee Hours // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void mitProcessHours_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intEmployeeID;
            int intPayID;
            decimal decHours;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                blnFatalError = TheDataValidationClass.VerifyDateData(txtEnterPayPeriod.Text);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Pay Period Date is not a Date");
                    return;
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(txtEnterPayPeriod.Text);
                }

                intNumberOfRecords = TheImportHoursDataSet.employees.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheImportHoursDataSet.employees[intCounter].EmployeeID;
                    intPayID = TheImportHoursDataSet.employees[intCounter].PayID;
                    decHours = TheImportHoursDataSet.employees[intCounter].Hours;

                    blnFatalError = TheEmployeePunchedHoursClass.InsertEmployeePunchedHours(datTransactionDate, intEmployeeID, intPayID, decHours);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Import Employee Hours // Process Hours Menu Item ");

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Hours Have Been Imported");
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Employee Hours // Process Hours Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TheImportHoursDataSet.employees.Rows.Clear();

            dgrResults.ItemsSource = TheImportHoursDataSet.employees;
        }
    }
}
