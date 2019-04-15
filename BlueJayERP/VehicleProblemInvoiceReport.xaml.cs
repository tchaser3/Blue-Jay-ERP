/* Title:           Vehicle Problem Invoice Report
 * Date:            1-3-19
 * Name:            Terrance Holmes
 * 
 * Description:     this is where a report for invoiced vehicle problems will be shown */

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
using VehicleMainDLL;
using VehicleProblemDocumentationDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleProblemInvoiceReport.xaml
    /// </summary>
    public partial class VehicleProblemInvoiceReport : Window
    {
        //setting the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemDocumentClass TheVehicleProblemDocumentClass = new VehicleProblemDocumentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehicleInvoiceByVehicleIDDataSet TheFindVehicleInvoiceByVehicleIDDataSet = new FindVehicleInvoiceByVehicleIDDataSet();

        public VehicleProblemInvoiceReport()
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
            txtVehicleNumber.Text = "";
            TheFindVehicleInvoiceByVehicleIDDataSet = TheVehicleProblemDocumentClass.FindVehicleInvoiceByVehicleID(-1, DateTime.Now, DateTime.Now);

            dgrResults.ItemsSource = TheFindVehicleInvoiceByVehicleIDDataSet.FindVehicleInvoicesByVehicleID;

            mitGetVehicleInvoiceInformation.IsEnabled = false;
            mitGenerateReport.IsEnabled = false;
        }

        private void TxtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intLength;
            int intRecordsReturned;

            try
            {
                MainWindow.gstrVehicleNumber = txtVehicleNumber.Text;
                intLength = MainWindow.gstrVehicleNumber.Length;

                if ((intLength == 4) || (intLength == 6))
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        if(intLength == 6)
                        {
                            TheMessagesClass.ErrorMessage("Vehicle Number Not Found");
                        }

                        return;
                    }

                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    mitGetVehicleInvoiceInformation.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Problem Invoice Report // Vehicle Number Text Changed Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[6].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    if (strDocumentPath != "NO INVOICE ATTACHED")
                    {
                        System.Diagnostics.Process.Start(strDocumentPath);
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Problem Invoice Report // Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitGetVehicleInvoiceInformation_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;

            //data validation
            strValueForValidation = txtStartDate.Text;
            blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
            if (blnThereIsAProblem == true)
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
            if(blnThereIsAProblem == true)
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
                    TheMessagesClass.ErrorMessage("The End Date is after the Start Date");
                    return;
                }
            }

            TheFindVehicleInvoiceByVehicleIDDataSet = TheVehicleProblemDocumentClass.FindVehicleInvoiceByVehicleID(MainWindow.gintVehicleID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

            dgrResults.ItemsSource = TheFindVehicleInvoiceByVehicleIDDataSet.FindVehicleInvoicesByVehicleID;

            mitGenerateReport.IsEnabled = true;

            blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Blue Jay ERP // Vehicle Problem Invoice Report");

            if (blnFatalError == true)
                throw new Exception();
        }

        private void MitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            SpecificVehicleInvoiceReport SpecificVehicleInvoiceReport = new SpecificVehicleInvoiceReport();
            SpecificVehicleInvoiceReport.ShowDialog();
        }
    }
}
