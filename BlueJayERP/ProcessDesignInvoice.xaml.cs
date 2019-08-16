/* Title:           Process Design Invoice
 * Date:            8-5-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to process the invoices */

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
using WOVInvoicingDLL;
using Microsoft.Win32;
using DesignProjectsDLL;
using TechPayDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ProcessDesignInvoice.xaml
    /// </summary>
    public partial class ProcessDesignInvoice : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        DesignProjectsClass TheDesignProjectClass = new DesignProjectsClass();
        TechPayClass TheTechPayClass = new TechPayClass();

        FindWOVBillingCodesByBillingIDDataSet TheFindWOVBillingCodesByBillingIDDataSet = new FindWOVBillingCodesByBillingIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindWOVInvoicingByTransactionDateDataSet TheFindWOVInvoicingByTransactionDateDataSet = new FindWOVInvoicingByTransactionDateDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsByAssignedProjectID = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindTechPayItemByCodeDataSet TheFindTechPayItemByCodeDataSet = new FindTechPayItemByCodeDataSet();

        public ProcessDesignInvoice()
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
            decimal decInvoiceTotal = 0;

            try
            {
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintWarehouseID);

                txtBillingLocation.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;

                if (txtBillingLocation.Text == "CLEVELAND")
                {
                    dgrResults.ItemsSource = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice;

                    intNumberOfRecords = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        decInvoiceTotal += MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].TotalProjectCharge;
                    }
                }
                else if (txtBillingLocation.Text == "MILWAUKEE")
                {
                    dgrResults.ItemsSource = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice;

                    intNumberOfRecords = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        decInvoiceTotal += MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].ProjectTotal;
                    }
                }

                TheFindWOVBillingCodesByBillingIDDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByBillingID(MainWindow.gintBillingID);

                txtBillingType.Text = TheFindWOVBillingCodesByBillingIDDataSet.FindWOVBillingCodesByBillingID[0].BillingCode;
                txtInvoicedAmount.Text = Convert.ToString(decInvoiceTotal);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Process Design Invoice // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }

        private void MitProcessInvoice_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName == "CLEVELAND")
                {
                    blnFatalError = ProcessClevelandInvoice();

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName == "MILWAUKEE")
                {
                    blnFatalError = ProcessWisconsinInvoice();

                    if (blnFatalError == true)
                        throw new Exception();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Process Design Invoice // Process Invoice Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }            
        }
        private bool ProcessWisconsinInvoice()
        {
            bool blnFatalError = false;
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strAssignedProjectID;
            int intProjectID;
            int intEmployeeID;
            int intProjectIdentifictionID;
            string strWOVDescription;
            decimal decTotalQuantity = 0;
            int intWOVTaskID = 0;
            int intTransactionID;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;


            try
            {
                intNumberOfRecords = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Count - 1;

                for (intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //getting ready to insert into tables
                    datTransactionDate = DateTime.Now;
                    strAssignedProjectID = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].DockID;
                    TheFindDesignProjectsByAssignedProjectID = TheDesignProjectClass.FindDesignProjectsByAssignedProjectID(strAssignedProjectID);
                    intProjectID = TheFindDesignProjectsByAssignedProjectID.FindDesignProjectsByAssignedProjectID[0].ProjectID;
                    intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                    intTransactionID = TheFindDesignProjectsByAssignedProjectID.FindDesignProjectsByAssignedProjectID[0].TransactionID;

                    blnFatalError = TheWOVInvoicingClass.InsertWOVInvoice(datTransactionDate, intProjectID, intEmployeeID, MainWindow.gintBillingID);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindWOVInvoicingByTransactionDateDataSet = TheWOVInvoicingClass.FindWOVInvoicingByTransactionDate(datTransactionDate, intProjectID);

                    intProjectIdentifictionID = TheFindWOVInvoicingByTransactionDateDataSet.FindWOVInvoicingByTransactionDate[0].ProjectIdentificationID;

                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].WOV1 != 0)
                    {
                        strWOVDescription = "WOV1";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].WOV1;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].WOV2 != 0)
                    {
                        strWOVDescription = "WOV2";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].WOV2;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].PP1 != 0)
                    {
                        strWOVDescription = "PP1";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].PP1;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].PP2 != 0)
                    {
                        strWOVDescription = "PP2";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].PP2;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].UF != 0)
                    {
                        strWOVDescription = "UF";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].UF;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].HA != 0)
                    {
                        strWOVDescription = "HA";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].HA;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].MC05 != 0)
                    {
                        strWOVDescription = "MC05";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].MC05;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].PermitCost != 0)
                    {
                        strWOVDescription = "NS004";
                        decTotalQuantity = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice[intCounter].PermitCost;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }

                    blnFatalError = TheWOVInvoicingClass.InsertWOVInvoicingItems(intProjectID, intProjectIdentifictionID, intEmployeeID, intWOVTaskID, Convert.ToInt32(decTotalQuantity));

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheDesignProjectClass.CloseDesignProject(intTransactionID, DateTime.Now);

                    if (blnFatalError == true)
                        throw new Exception();

                }

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "Projects Invoiced";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows.Count;
                intColumnNumberOfRecords = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = MainWindow.TheWisconsinDesignProjectInvoicingDataSet.wisconsindesigninvoice.Rows[intRowCounter][intColumnCounter].ToString();

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
                MessageBox.Show("Projects Invoiced");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Process Design Invoice // Process Wisconsin Invoice " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool ProcessClevelandInvoice()
        {
            bool blnFatalError = false;
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate;
            string strAssignedProjectID;
            int intProjectID;
            int intEmployeeID;
            int intProjectIdentifictionID;
            string strWOVDescription;
            decimal decTotalQuantity = 0;
            int intWOVTaskID = 0;
            int intTransactionID;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                intNumberOfRecords = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Count - 1;

                for(intCounter = 1; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //getting ready to insert into tables
                    datTransactionDate = DateTime.Now;
                    strAssignedProjectID = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].DockID;
                    TheFindDesignProjectsByAssignedProjectID = TheDesignProjectClass.FindDesignProjectsByAssignedProjectID(strAssignedProjectID);
                    intProjectID = TheFindDesignProjectsByAssignedProjectID.FindDesignProjectsByAssignedProjectID[0].ProjectID;
                    intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                    intTransactionID = TheFindDesignProjectsByAssignedProjectID.FindDesignProjectsByAssignedProjectID[0].TransactionID;

                    blnFatalError = TheWOVInvoicingClass.InsertWOVInvoice(datTransactionDate, intProjectID, intEmployeeID, MainWindow.gintBillingID);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindWOVInvoicingByTransactionDateDataSet = TheWOVInvoicingClass.FindWOVInvoicingByTransactionDate(datTransactionDate, intProjectID);

                    intProjectIdentifictionID = TheFindWOVInvoicingByTransactionDateDataSet.FindWOVInvoicingByTransactionDate[0].ProjectIdentificationID;

                    if(MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].WOV1 != 0)
                    {
                        strWOVDescription = "WOV1";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].WOV1;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].WOV2 != 0)
                    {
                        strWOVDescription = "WOV2";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].WOV2;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].WOV3 != 0)
                    {
                        strWOVDescription = "WOV3";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].WOV3;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].PP1 != 0)
                    {
                        strWOVDescription = "PP1";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].PP1;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].PP2 != 0)
                    {
                        strWOVDescription = "PP2";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].PP2;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].FE01 != 0)
                    {
                        strWOVDescription = "FE01";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].FE01;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].UG != 0)
                    {
                        strWOVDescription = "UG";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].UG;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].HA != 0)
                    {
                        strWOVDescription = "HA";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].HA;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].MC05 != 0)
                    {
                        strWOVDescription = "MC05";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].MC05;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].MC06 != 0)
                    {
                        strWOVDescription = "MC06";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].MC06;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;
                    }
                    if (MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].NS004 != 0)
                    {
                        strWOVDescription = "NS004";
                        decTotalQuantity = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice[intCounter].NS004;

                        TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strWOVDescription);
                        intWOVTaskID = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode[0].TechPayID;                        
                    }

                    blnFatalError = TheWOVInvoicingClass.InsertWOVInvoicingItems(intProjectID, intProjectIdentifictionID, intEmployeeID, intWOVTaskID, Convert.ToInt32(decTotalQuantity));

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheDesignProjectClass.CloseDesignProject(intTransactionID, DateTime.Now);

                    if (blnFatalError == true)
                        throw new Exception();

                }

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "Projects Invoiced";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows.Count;
                intColumnNumberOfRecords = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = MainWindow.TheClevelandDesignProjectInvoicingDataSet.clevelanddesigninvoice.Rows[intRowCounter][intColumnCounter].ToString();

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
                MessageBox.Show("Projects Invoiced");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Process Design Invoice // Process Cleveland Invoice " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
    }
}
