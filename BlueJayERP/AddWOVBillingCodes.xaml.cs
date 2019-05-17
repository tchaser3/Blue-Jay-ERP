/* Title:           Add WOV Billing Codes
 * Date:            5-15-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add the billing codes */

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
    /// Interaction logic for AddWOVBillingCodes.xaml
    /// </summary>
    public partial class AddWOVBillingCodes : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindWOVBillingCodeByDescriptionDataSet TheFindWOVBillingCodeByDescriptionDataSet = new FindWOVBillingCodeByDescriptionDataSet();
        FindWOVBillingCodesByBillingCodesDataSet TheFindWOVBillingCodesByBillingCodesDataSet = new FindWOVBillingCodesByBillingCodesDataSet();

        public AddWOVBillingCodes()
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
            txtBillingCode.Text = "";
            txtBillingDescription.Text = "";
        }

        private void MitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strBillingCode;
            string strBillingDescription;
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intRecordsReturned;

            strBillingCode = txtBillingCode.Text;
            if(strBillingCode == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Billing Code was not Entered\n";
            }
            strBillingDescription = txtBillingDescription.Text;
            if(strBillingDescription == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Billing Description was not Entered\n";
            }
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            TheFindWOVBillingCodesByBillingCodesDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByBillingCodes(strBillingCode);

            intRecordsReturned = TheFindWOVBillingCodesByBillingCodesDataSet.FindWOVBillingCodeByBillingCode.Rows.Count;

            if(intRecordsReturned > 0)
            {
                TheMessagesClass.ErrorMessage("The Billing Code Has Already Been Entered");
                return;
            }

            TheFindWOVBillingCodeByDescriptionDataSet = TheWOVInvoicingClass.FindWOVBillingCodesByDescription(strBillingDescription);

            intRecordsReturned = TheFindWOVBillingCodeByDescriptionDataSet.FindWOVBillingCodeByDescription.Rows.Count;

            if (intRecordsReturned > 0)
            {
                return;
            }

            try
            {
                blnFatalError = TheWOVInvoicingClass.InsertWOVBillingCodes(strBillingDescription, strBillingCode);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Billing Code has been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add WOV Billing Codes // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
