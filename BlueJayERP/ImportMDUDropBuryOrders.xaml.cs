/* Title:           Import MDU Drop Bury Orders
 * Date:            May 18, 2018
 * Author:          Terry Holmes
 * 
 * Description:     This the form used import work orders */

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
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using CustomersDLL;
using DataValidationDLL;
using DateSearchDLL;
using DropBuryMDUDLL;
using KeyWordDLL;
using WorkOrderDLL;
using WorkTypeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ImportMDUDropBuryOrders.xaml
    /// </summary>
    public partial class ImportMDUDropBuryOrders : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DropBuryMDUClass TheDropBuryMDUClass = new DropBuryMDUClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();

        ImportedDataSet TheImportedDataSet = new ImportedDataSet();
        CustomersDataSet TheCustomersDataSet = new CustomersDataSet();
        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindCustomerAddressDateMatchDataSet TheFindCustomerAddressDateMatchDataSet = new FindCustomerAddressDateMatchDataSet();
        FindCustomerByPhoneNumberDataSet TheFindCustomerByPhoneNumberDataSet = new FindCustomerByPhoneNumberDataSet();
        FindCustomersByAddressIDDataSet TheFindCustomersByAddressIDDataSet = new FindCustomersByAddressIDDataSet();
        FindActiveCustomerByAccountNumberDataSet TheFindActiveCustomerByAccountNumberDataSet = new FindActiveCustomerByAccountNumberDataSet();
        FindCustomerByAccountNumberDataSet TheFindCustomerByAccountNumberDataSet = new FindCustomerByAccountNumberDataSet();
        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderByWorkOrderNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();
        FindAddressByAddressesDataSet TheFindAddressByAddressesDataSet = new FindAddressByAddressesDataSet();
        FindWorkZoneByZoneNameDataSet TheFindWorkZoneByZoneNameDataSet = new FindWorkZoneByZoneNameDataSet();
        FindAllOpenMDUDropOrdersDataSet TheFindAllOpenMDUDropOrdersDataSet = new FindAllOpenMDUDropOrdersDataSet();


        int gintColumnCounter;
        int gintPhoneCounter;
        int gintWorkTypeID;
        string gstrWorkType;

        public ImportMDUDropBuryOrders()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                mitImportJobs.IsEnabled = false;

                TheCustomersDataSet = TheCustomersClass.GetCustomersInfo();

                gintPhoneCounter = TheCustomersDataSet.customers.Rows.Count;

                TheFindWorkTypeSortedDataSet = TheWorkTypeClass.FindWorkTypeSorted();

                intNumberOfRecords = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted.Rows.Count - 1;
                cboSelectWorkType.Items.Add("Select Work Type");

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWorkType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboSelectWorkType.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import MDU Drop Bury Orders // Windows Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboSelectWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will select the work type
            int intSelectedIndex;

            intSelectedIndex = cboSelectWorkType.SelectedIndex - 1;

            if (intSelectedIndex == -1)
            {
                mitImportJobs.IsEnabled = false;
                mitSelectSpreadSheet.IsEnabled = false;
                TheImportedDataSet.importedjobs.Rows.Clear();
                dgrResults.ItemsSource = TheImportedDataSet.importedjobs;
            }
            else
            {
                gintWorkTypeID = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].TypeID;
                gstrWorkType = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].WorkType;
                mitSelectSpreadSheet.IsEnabled = true;
            }
        }

        private void mitSelectSpreadSheet_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            string strInformation = "";
            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            char[] chaInformation;
            int intCharCounter;
            int intCharLength;
            string strCompleteWord = "";
            int intLength;

            try
            {
                TheImportedDataSet.importedjobs.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".csv"; // Default file extension
                dlg.Filter = "Excel (.csv)|*.csv"; // Filter files by extension

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
                intNumberOfRecords = range.Rows.Count - 1;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strInformation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);

                    gintColumnCounter = 0;

                    chaInformation = strInformation.ToCharArray();

                    intCharLength = strInformation.Length - 1;

                    ImportedDataSet.importedjobsRow NewJobRow = TheImportedDataSet.importedjobs.NewimportedjobsRow();

                    NewJobRow.ScheduledDate = DateTime.Now;

                    for (intCharCounter = 0; intCharCounter <= intCharLength; intCharCounter++)
                    {
                        if (chaInformation[intCharCounter] != ',')
                        {
                            strCompleteWord += Convert.ToString(chaInformation[intCharCounter]);
                            strCompleteWord = strCompleteWord.ToUpper();
                        }
                        else
                        {
                            if (gintColumnCounter == 1)
                            {
                                NewJobRow.WorkorderID = strCompleteWord;
                            }
                            else if (gintColumnCounter == 11)
                            {
                                NewJobRow.JobStatus = strCompleteWord;
                            }
                            else if (gintColumnCounter == 29)
                            {
                                strCompleteWord = strCompleteWord.Substring(2);

                                if (strCompleteWord == "MAPLE")
                                {
                                    strCompleteWord = "MAPLE HTS";
                                }

                                NewJobRow.Pool = strCompleteWord;
                            }
                            else if (gintColumnCounter == 57)
                            {
                                NewJobRow.AccountID = strCompleteWord;
                            }
                            else if (gintColumnCounter == 58)
                            {
                                NewJobRow.FirstName = strCompleteWord;
                            }
                            else if (gintColumnCounter == 59)
                            {
                                NewJobRow.LastName = strCompleteWord;
                            }
                            else if (gintColumnCounter == 60)
                            {
                                if (strCompleteWord == "(999) 999-9999")
                                {
                                    strCompleteWord = Convert.ToString(gintPhoneCounter);
                                    gintPhoneCounter++;
                                }
                                else if (strCompleteWord == "")
                                {
                                    strCompleteWord = Convert.ToString(gintPhoneCounter);
                                    gintPhoneCounter++;
                                }
                                else if (strCompleteWord == null)
                                {
                                    strCompleteWord = Convert.ToString(gintPhoneCounter);
                                    gintPhoneCounter++;
                                }

                                NewJobRow.PhoneNumber = strCompleteWord;
                            }
                            else if (gintColumnCounter == 75)
                            {
                                NewJobRow.StreetAddress = strCompleteWord;
                            }
                            else if (gintColumnCounter == 77)
                            {
                                NewJobRow.City = strCompleteWord;
                            }
                            else if (gintColumnCounter == 78)
                            {
                                NewJobRow.State = strCompleteWord;
                            }
                            else if (gintColumnCounter == 79)
                            {
                                intLength = strCompleteWord.Length;

                                if (intColumnRange > 5)
                                {
                                    strCompleteWord = strCompleteWord.Substring(0, 5);
                                }


                                NewJobRow.Zip = strCompleteWord;
                            }

                            strCompleteWord = "";
                            gintColumnCounter++;
                        }
                    }

                    TheImportedDataSet.importedjobs.Rows.Add(NewJobRow);
                }

                CheckOpenOrders();

                PleaseWait.Close();
                dgrResults.ItemsSource = TheImportedDataSet.importedjobs;
                mitImportJobs.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import MDU Drop Bury Orders // Select Spreadsheet Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void CheckOpenOrders()
        {
            int intFirstCounter;
            int intFirstNumberOfRecords;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            bool blnItemNotFound;
            DateTime datStartDate;
            DateTime datEndDate = DateTime.Now;
            string strWorkOrderNumber;

            try
            {
                datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
                datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
                datStartDate = TheDateSearchClass.SubtractingDays(datEndDate, 365);

                MainWindow.TheCancelledOrdersDataSet.cancelledorders.Rows.Clear();

                TheFindAllOpenMDUDropOrdersDataSet = TheDropBuryMDUClass.FindAllOpenMDUDropOrders(datStartDate, datEndDate, gstrWorkType);

                intFirstNumberOfRecords = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders.Rows.Count - 1;

                intSecondNumberOfRecords = TheImportedDataSet.importedjobs.Rows.Count - 1;

                for(intFirstCounter = 0; intFirstCounter <= intFirstNumberOfRecords; intFirstCounter++)
                {
                    blnItemNotFound = true;
                    strWorkOrderNumber = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].WorkOrderNumber;

                    for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        if(strWorkOrderNumber == TheImportedDataSet.importedjobs[intSecondCounter].WorkorderID)
                        {
                            blnItemNotFound = false;
                        }
                    }

                    if(blnItemNotFound == true)
                    {
                        CancelledOrdersDataSet.cancelledordersRow CancelledOrder = MainWindow.TheCancelledOrdersDataSet.cancelledorders.NewcancelledordersRow();

                        CancelledOrder.City = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].City;
                        CancelledOrder.DateEntered = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].DateEntered;
                        CancelledOrder.DateReceived = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].DateReceived;
                        CancelledOrder.DateScheduled = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].DateScheduled;
                        CancelledOrder.FirstName = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].FirstName;
                        CancelledOrder.LastName = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].LastName;
                        CancelledOrder.StreetAddress = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].StreetAddress;
                        CancelledOrder.WorkOrderID = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].WorkOrderID;
                        CancelledOrder.WorkOrderNumber = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].WorkOrderNumber;
                        CancelledOrder.WorkOrderStatus = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].WorkOrderStatus;
                        CancelledOrder.WorkType = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].WorkType;
                        CancelledOrder.ZoneLocation = TheFindAllOpenMDUDropOrdersDataSet.FindAllOpenMDUDropOrders[intFirstCounter].ZoneLocation;

                        MainWindow.TheCancelledOrdersDataSet.cancelledorders.Rows.Add(CancelledOrder);

                    }
                }

                if(MainWindow.TheCancelledOrdersDataSet.cancelledorders.Rows.Count > 0)
                {
                    CancelledOrdersWindow CancelledOrdersWindow = new CancelledOrdersWindow();
                    CancelledOrdersWindow.Show();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Import MDU Drop Bury ORders // Check Open Orders " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
