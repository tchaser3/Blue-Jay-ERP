/* Title:           Edit WOV Billing Codes
 * Date:            5-16-19
 * Author:          Terry Holmes
 * 
 * Description:     This is the window for editing wov billing codes */

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
using WOVInvoicingDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditWOVBillingCodes.xaml
    /// </summary>
    public partial class EditWOVBillingCodes : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindWOVBillingCodesByBillingCodesDataSet TheFindWOVBillingCodesByBillingCodesDataSet = new FindWOVBillingCodesByBillingCodesDataSet();

        //setting global variables
        string gstrBillingCode;
        int gintBillingID;

        public EditWOVBillingCodes()
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
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtBillingDescription.Text = "";
            txtBillingID.Text = "";
            txtEnterBillingCode.Text = "";
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;

            gstrBillingCode = txtEnterBillingCode.Text;
            if(gstrBillingCode == "")
            {
                TheMessagesClass.ErrorMessage("The Billing Code Was Not Entered");
                return;
            }

            TheFindWOVBillingCodesByBillingCodesDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByBillingCodes(gstrBillingCode);

            intRecordsReturned = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode.Rows.Count;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("The Billing Code Was Not Found");
                return;
            }

            txtBillingDescription.Text = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode[0].BillingDescription;
            txtBillingID.Text = Convert.ToString(TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode[0].BillingID);
            gintBillingID = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode[0].BillingID;
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError;
            string strBillingDescription;

            try
            {
                strBillingDescription = txtBillingDescription.Text;
                if (strBillingDescription == "")
                {
                    TheMessagesClass.ErrorMessage("The Billing Description was not Entered");
                    return;
                }

                blnFatalError = TheWOVInvoicingClass.UpdateWOVBillingCodeDescription(gintBillingID, strBillingDescription);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The WOV Billing Code has been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit WOV Billing Code // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
    }
}
