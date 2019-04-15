/* Title:           Preventive Maintenanced
 * Date:            4-26-18
 * Author:          Terry Holmes
 * 
 * Description:     this window is used to put in the preventive maintenance  */

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
using VehicleProblemsDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for PreventiveMaintenance.xaml
    /// </summary>
    public partial class PreventiveMaintenance : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleProblemPrintClass TheVehicleProblemPrintClass = new VehicleProblemPrintClass();

        //setting the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();

        //setting global variables
        bool gblnOilChangeComplete;
        int gintOldOdometerReading;

        public PreventiveMaintenance()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboSelectCompletion.Items.Add("Select Completion");
            cboSelectCompletion.Items.Add("YES");
            cboSelectCompletion.Items.Add("NO");
            cboSelectCompletion.SelectedIndex = 0;

            SetControlsReadOnly(true);

            mitProccess.IsEnabled = false;

            MainWindow.gblnWorkOrderSelected = false;
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtOilChangeOdometer.IsReadOnly = blnValueBoolean;
            txtOilChangeDate.IsReadOnly = blnValueBoolean;
        }

        private void btnFindVehicle_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strVehicleNumber;
            int intRecordsReturned;

            strVehicleNumber = txtEnterVehicleNumber.Text;
            mitProccess.IsEnabled = false;
            cboSelectCompletion.SelectedIndex = 0;

            if(strVehicleNumber == "")
            {
                TheMessagesClass.ErrorMessage("Vehicle Number Was Not Entered");
                return;
            }

            TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

            intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

            if (intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("Vehicle Number Was Not Found");
                return;
            }
            
            MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

            MainWindow.TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

            intRecordsReturned = MainWindow.TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID.Rows.Count;

            if (intRecordsReturned > 0)
            {
                SelectExistingWorkOrder SelectExistingWorkOrder = new SelectExistingWorkOrder();
                SelectExistingWorkOrder.ShowDialog();
            }

            txtOilChangeDate.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].OilChangeDate);
            txtOilChangeOdometer.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].OilChangeOdometer);
            gintOldOdometerReading = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].OilChangeOdometer;
            txtVehicleMake.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleMake;
            txtVehicleModel.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleModel;
            txtVehicleYear.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleYear);

            SetControlsReadOnly(false);
        }
        private void ResetControls()
        {
            txtEnterVehicleNumber.Text = "";
            txtOilChangeDate.Text = "";
            txtOilChangeOdometer.Text = "";
            txtVehicleMake.Text = "";
            txtVehicleModel.Text = "";
            txtVehicleYear.Text = "";
            mitProccess.IsEnabled = false;
            cboSelectCompletion.SelectedIndex = 0;
        }

        private void cboSelectCompletion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // setting local variables
            int intSelectedIndex;
            
            intSelectedIndex = cboSelectCompletion.SelectedIndex;

            if(intSelectedIndex == 1)
            {
                mitProccess.IsEnabled = true;
                gblnOilChangeComplete = true;
            }
            else if(intSelectedIndex == 2)
            {
                mitProccess.IsEnabled = true;
                gblnOilChangeComplete = false;
            }
            else
            {
                mitProccess.IsEnabled = false;
            }

        }

        private void mitProccess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datOilChangeDate = DateTime.Now;
            int intOdometerReading = 0;
            float fltInvoiceTotal = 0;
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datTransactionDate = DateTime.Now;

            try
            {
                strValueForValidation = txtOilChangeDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Odometer Date is not a Date\n";
                }
                else
                {
                    datOilChangeDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtOilChangeOdometer.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Odometer Reading is not an Integer\n";
                }
                else
                {
                    intOdometerReading = Convert.ToInt32(strValueForValidation);

                    if (gintOldOdometerReading + 3000 >= intOdometerReading)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Odometer Reading is less than 3000 Miles Since Last Oil Change\n";
                    }
                }
                
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(MainWindow.gblnWorkOrderSelected == false)
                {
                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, datTransactionDate, "PREVENTIVE MAINTENANCE");

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(datTransactionDate);

                    MainWindow.gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;
                }

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "PREVENTIVIE MAINTENANCE PERFORMED", datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheVehicleMainClass.UpdateMainOilChangeInformation(MainWindow.gintVehicleID, intOdometerReading, datOilChangeDate);

                if (blnFatalError == true)
                    throw new Exception();

                if (gblnOilChangeComplete == true)
                {
                    blnFatalError = TheVehicleProblemClass.ChangeVehicleProblemStatus(MainWindow.gintProblemID, "AWAITING INVOICE");

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleProblemClass.UpdateVehiclePRoblemCost(MainWindow.gintProblemID, fltInvoiceTotal);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Record Has Been Saved");

                MessageBoxResult result = MessageBox.Show("Do You Want To Print A Copy of the problem", "Please Choose", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TheVehicleProblemPrintClass.PrintVehicleProblemInfo();
                }

                ResetControls();
                SetControlsReadOnly(true);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Preventive Maintenance // Process Menu Item " + Ex.Message);

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
