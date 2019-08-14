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

        FindWOVBillingCodesByBillingIDDataSet TheFindWOVBillingCodesByBillingIDDataSet = new FindWOVBillingCodesByBillingIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

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

        }
    }
}
