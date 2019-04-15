/* Title:           View Vehicle Problem
 * Date:            1-9-19
 * Author:          Terry Holmes
 * 
 * Description:     This window will show vehicle problems over a date range */

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
using DateSearchDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewVehicleProblem.xaml
    /// </summary>
    public partial class ViewVehicleProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindAllVehicleMainProblemsByDateRangeDataSet TheFindAllVehicleProblemsByDateRangeDataSet = new FindAllVehicleMainProblemsByDateRangeDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        //setting global variables
        bool gblnVehicleFound;

        public ViewVehicleProblem()
        {
            InitializeComponent();
        }

        private void TxtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strVehicleNumber;
            int intLength;
            int intRecordsReturned;

            strVehicleNumber = txtVehicleNumber.Text;
            intLength = strVehicleNumber.Length;
            gblnVehicleFound = false;

            if((intLength == 4) || (intLength == 6))
            {
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    gblnVehicleFound = true;
                }
                else if(intRecordsReturned == 0)
                {
                    if(intLength == 6)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }
                }
            }
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
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtVehicleNumber.Text = "";

            TheFindAllVehicleProblemsByDateRangeDataSet = TheVehicleProblemClass.FindAllVehicleMainProblemsByDateRange(-1, DateTime.Now, DateTime.Now);

            dgrResults.ItemsSource = TheFindAllVehicleProblemsByDateRangeDataSet.FindAllVehicleMainProblemsByDateRange;

            gblnVehicleFound = false;
        }

        private void MitGetVehicleProblems_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strValueForValidation;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                //performing data validation
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Start Date Not A Date\n";
                }
                else
                {
                    datStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "End Date Not A Date\n";
                }
                else
                {
                    datEndDate = Convert.ToDateTime(strValueForValidation);
                    datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
                }
                if(gblnVehicleFound == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                TheFindAllVehicleProblemsByDateRangeDataSet = TheVehicleProblemClass.FindAllVehicleMainProblemsByDateRange(MainWindow.gintVehicleID, datStartDate, datEndDate);

                dgrResults.ItemsSource = TheFindAllVehicleProblemsByDateRangeDataSet.FindAllVehicleMainProblemsByDateRange;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Vehicle Problem // Get Vehicle Problems " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                if (dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    ViewSelectedVehicleProblem ViewSelectedVehicleProblem = new ViewSelectedVehicleProblem();
                    ViewSelectedVehicleProblem.ShowDialog();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Vehicle Problem // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
