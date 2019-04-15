/* Title:           Edit Vehicle Bulk Tool
 * Date:            5-31-18
 * Author:          Terry Holmes
 * 
 * Description:     This is how bulk tools for a vehice will be updated */

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
using DataValidationDLL;
using VehicleBulkToolsDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditVehicleBulkTool.xaml
    /// </summary>
    public partial class EditVehicleBulkTool : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        public EditVehicleBulkTool()
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
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //loading controls
            txtToolCategory.Text = MainWindow.TheFindVehicleBulkByTransactionIDDataSet.FindVehicleBulkToolByTransactionID[0].ToolCategory;
            txtQuantity.Text = Convert.ToString(MainWindow.TheFindVehicleBulkByTransactionIDDataSet.FindVehicleBulkToolByTransactionID[0].Quantity);
        }

        private void mitUpdateTools_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strValueForValidation;
            int intQuantity = 0;
            int intTransactionID;

            try
            {
                strValueForValidation = txtQuantity.Text;
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Quantity is not an Integer");
                    return;
                }
                else
                {
                    intQuantity = Convert.ToInt32(strValueForValidation);
                    if(intQuantity < 0)
                    {
                        TheMessagesClass.ErrorMessage("There Cannot Be a Negative Quantity");
                        return;
                    }
                }

                intTransactionID = MainWindow.TheFindVehicleBulkByTransactionIDDataSet.FindVehicleBulkToolByTransactionID[0].TransactionID;

                blnFatalError = TheVehicleBulkToolsClass.UpdateVehicleBulkTools(intTransactionID, intQuantity);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Tool Value has been Updated");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Vehicle Bulk Tool // Update Tool Menu Item " + Ex.Message);

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
