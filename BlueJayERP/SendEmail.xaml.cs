/* Title:           Send Email
 * Date:            8-23-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window to send a system email */

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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SendEmail.xaml
    /// </summary>
    public partial class SendEmail : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        public SendEmail()
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
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ClearControls();
        }
        private void ClearControls()
        {
            txtMessage.Text = "";
            txtSubject.Text = "";
        }

        private void mitSendEmail_Click(object sender, RoutedEventArgs e)
        {
            string strHeader;
            string strMessage;
            string strFinalMessage;
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress;

            strHeader = txtSubject.Text;
            if(strHeader == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Subject Was Not Entered\n";
            }
            strMessage = txtMessage.Text;
            if(strMessage == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Message Was Not Entered\n";
            }
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            EmailEmployees EmailEmployees = new EmailEmployees();
            EmailEmployees.ShowDialog();

            strHeader += " - DO NOT REPLY";

            strFinalMessage = "<h1>" + strHeader + "</h1>";
            strFinalMessage += "<h3>From " + MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].FirstName + " " + MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].LastName + "</h3>";
            strFinalMessage += "<p>" + strMessage + "</p>";

            intNumberOfRecords = MainWindow.TheEmailListDataSet.employees.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                strEmailAddress = MainWindow.TheEmailListDataSet.employees[intCounter].EmailAddress;

                TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strFinalMessage);
            }

            TheMessagesClass.InformationMessage("Email Sent");

            ClearControls();
        }
    }
}
