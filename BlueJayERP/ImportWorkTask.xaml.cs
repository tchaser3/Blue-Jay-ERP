/* Title:           Import Work Task
 * Date:            7-10-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form it import work tasks */

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

using Excel = Microsoft.Office.Interop.Excel;
using NewEventLogDLL;
using WorkTaskDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportWorkTask.xaml
    /// </summary>
    public partial class ImportWorkTask : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();

        //setting up the data
        ImportWorkTaskDataSet TheImportWorkTaskDataSet = new ImportWorkTaskDataSet();
        FindWorkTaskByWorkTaskDataSet TheFindWorkTaskByWorkTaskDataSet = new FindWorkTaskByWorkTaskDataSet();

        public ImportWorkTask()
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

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            TheImportWorkTaskDataSet.worktask.Rows.Clear();

            dgrResults.ItemsSource = TheImportWorkTaskDataSet.worktask;

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

            string strWorkTask;
            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;

            try
            {
                TheImportWorkTaskDataSet.worktask.Rows.Clear();

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
                    strWorkTask = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);

                    TheFindWorkTaskByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskByWorkTask(strWorkTask);

                    intRecordsReturned = TheFindWorkTaskByWorkTaskDataSet.FindWorkTaskByWorkTask.Rows.Count;

                    if (intRecordsReturned == 0)
                    {

                        ImportWorkTaskDataSet.worktaskRow NewTaskRow = TheImportWorkTaskDataSet.worktask.NewworktaskRow();

                        NewTaskRow.WorkTask = strWorkTask.ToUpper();
                        NewTaskRow.TaskCost = 1;

                        TheImportWorkTaskDataSet.worktask.Rows.Add(NewTaskRow);
                    }
                }

                PleaseWait.Close();
                dgrResults.ItemsSource = TheImportWorkTaskDataSet.worktask;
                mitProcess.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Work Task // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError;
            string strWorkTask;

            try
            {
                intNumberOfRecords = TheImportWorkTaskDataSet.worktask.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strWorkTask = TheImportWorkTaskDataSet.worktask[intCounter].WorkTask;

                    blnFatalError = TheWorkTaskClass.InsertWorkTask(strWorkTask, 1);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("All Records Have Been Imported");

                TheImportWorkTaskDataSet.worktask.Rows.Clear();

                dgrResults.ItemsSource = TheImportWorkTaskDataSet.worktask;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import Work Task // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitProcess.IsEnabled = false;
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
    }
}
