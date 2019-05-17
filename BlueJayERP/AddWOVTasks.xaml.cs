/* Title:           Add WOV Tasks
 * Date:            5-16-19
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to add wov tasks*/

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
using NewEmployeeDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddWOVTasks.xaml
    /// </summary>
    public partial class AddWOVTasks : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WOVInvoicingClass TheWOVInvoicingClass = new WOVInvoicingClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindWOVTaskByOfficeIDandDescriptionDataSet TheFindWOVTaskByOfficeIDandDescriptionDataSet = new FindWOVTaskByOfficeIDandDescriptionDataSet();

        public AddWOVTasks()
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
            int intCounter;
            int intNumberOfRecords;

            cboSelectOffice.Items.Clear();
            cboSelectOffice.Items.Add("Select Office");

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectOffice.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectOffice.SelectedIndex = 0;

            txtTaskDescription.Text = "";
            txtTaskPrice.Text = "";
        }

        private void CboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strValueForValidation;
            decimal decTaskPrice = 0;
            string strTaskDescription;

            try
            {
                if(cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Has Not Been Selected\n";
                }
                strTaskDescription = txtTaskDescription.Text;
                if(strTaskDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Description was not Entered\n";
                }
                strValueForValidation = txtTaskPrice.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Price is not Numberic\n";
                }
                else
                {
                    decTaskPrice = Convert.ToDecimal(strValueForValidation);
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheWOVInvoicingClass.InsertWOVTask(strTaskDescription, MainWindow.gintWarehouseID, decTaskPrice);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Task Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add WOV Tasks // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
