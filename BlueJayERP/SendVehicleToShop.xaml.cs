/* Title:           Send Vehicle To Shop
 * Date:            7-2-18
 * Author:          Terry Holmes
 * 
 * Description:     This window will send vehicles to the shop */

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
using VehicleProblemsDLL;
using VehiclesInShopDLL;
using VendorsDLL;
using VehicleMainDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SendVehicleToShop.xaml
    /// </summary>
    public partial class SendVehicleToShop : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemPrintClass TheVehicleProblemPrintClass = new VehicleProblemPrintClass();

        //setting up data
        FindOpenVehicleMainProblemsByVehicleIDDataSet TheFindOpenvehicleMainProblemByVehicleIDDataSet = new FindOpenVehicleMainProblemsByVehicleIDDataSet();
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSoredByVendorNameDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehicleInShopByProblemIDDataSet TheFindVehicleInShopbyProblemIDDataSet = new FindVehicleInShopByProblemIDDataSet();        

        public SendVehicleToShop()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            txtVehicleNumber.Text = "";
            TheFindOpenvehicleMainProblemByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(0);

            dgrResults.ItemsSource = TheFindOpenvehicleMainProblemByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

            mitProcess.IsEnabled = false;
            dgrResults.IsEnabled = false;
            txtVehicleNumber.IsEnabled = false;

            //this will load the combo box
            TheFindVendorsSoredByVendorNameDataSet = TheVendorsClass.FindVendorsSortedByVendorName();

            intNumberOfRecords = TheFindVendorsSoredByVendorNameDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

            cboVendors.Items.Clear();
            cboVendors.Items.Add("Select Vendor");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboVendors.Items.Add(TheFindVendorsSoredByVendorNameDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboVendors.SelectedIndex = 0;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            int intRecordsReturned;
            int intTransactionID;

            try
            {
                TheFindVehicleInShopbyProblemIDDataSet = TheVehiclesInShopClass.FindVehicleInShopByProblemID(MainWindow.gintProblemID);

                intRecordsReturned = TheFindVehicleInShopbyProblemIDDataSet.FindVehicleInShopByProblemID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    intTransactionID = TheFindVehicleInShopbyProblemIDDataSet.FindVehicleInShopByProblemID[0].TransactionID;

                    blnFatalError = TheVehiclesInShopClass.UpdateVehicleInShopVendorID(intTransactionID, MainWindow.gintVendorID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if (intRecordsReturned < 1)
                {
                    blnFatalError = TheVehiclesInShopClass.InsertVehicleInShop(MainWindow.gintVehicleID, DateTime.Now, MainWindow.gintVendorID, MainWindow.gstrVehicleProblem, MainWindow.gintProblemID);

                    if (blnFatalError == true)
                        throw new Exception();
                }               

                MessageBoxResult result = MessageBox.Show("Do You Want To Print A Copy of the problem", "Please Choose", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TheVehicleProblemPrintClass.PrintVehicleProblemInfo();
                }

                TheMessagesClass.InformationMessage("Vehicle Has Been Sent To Shop");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Send Vehicle To Shop // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void cboVendors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will set the vendor
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboVendors.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintVendorID = TheFindVendorsSoredByVendorNameDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;

                    txtVehicleNumber.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Send Vehicle To Shop // Vendor Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strVehicleNumber;
            int intLength;
            int intRecordsReturned;
            bool blnVehicleFound;

            strVehicleNumber = txtVehicleNumber.Text;
            intLength = strVehicleNumber.Length;
            blnVehicleFound = false;
            if(intLength == 4)
            {
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    blnVehicleFound = true;
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                }
            }
            else if(intLength == 6)
            {
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    blnVehicleFound = true;
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Not Found");
                    return;
                }
            }
            else if (intLength > 6)
            {
                TheMessagesClass.ErrorMessage("To Many Characters for a Vehicle Number");
                return;
            }

            if (blnVehicleFound == true)
            {
                TheFindOpenvehicleMainProblemByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                dgrResults.ItemsSource = TheFindOpenvehicleMainProblemByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

                dgrResults.IsEnabled = true;
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell VehicleProblem;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                if(dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    VehicleProblem = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    MainWindow.gstrVehicleProblem = ((TextBlock)VehicleProblem.Content).Text;
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    mitProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Send Vehicle To Shop // Grid Selection " + Ex.Message);

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
