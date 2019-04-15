/* Title:           New Vehicle Problem
 * Date:            6-28-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to create a new problem */

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
using VehiclesInShopDLL;
using VendorsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for NewVehicleProblem.xaml
    /// </summary>
    public partial class NewVehicleProblem : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        VendorsClass TheVendersClass = new VendorsClass();
        VehicleProblemPrintClass TheVehicleProblemPrintClass = new VehicleProblemPrintClass();

        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindOpenVehicleMainProblemsByVehicleIDDataSet TheFindOpenVehicleMainProblemsByVehicleIDDataSet = new FindOpenVehicleMainProblemsByVehicleIDDataSet();
        FindVendorsSortedByVendorNameDataSet TheFindVendorSortedDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();

        bool gblnInShop;

        public NewVehicleProblem()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtEnterVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intLength;
            string strVehicleNumber;
            int intRecordsReturned;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;
                intLength = strVehicleNumber.Length;
                if(intLength == 4)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                        dgrOpenProblems.ItemsSource = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

                        mitProcessProblem.IsEnabled = true;
                    }
                }
                if (intLength == 6)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                        dgrOpenProblems.ItemsSource = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

                        mitProcessProblem.IsEnabled = true;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Number Not Found");
                        return;
                    }
                }
                if(intLength > 6)
                {
                    TheMessagesClass.ErrorMessage("To Many Characters for a Vehicle Number");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // New Vehicle Problem // Enter Vehicle Number Text Change Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            txtAddedNotes.Text = "";
            txtEnterVehicleNumber.Text = "";
            txtProblem.Text = "";
            cboSelectVendor.Items.Clear();

            TheFindVendorSortedDataSet = TheVendersClass.FindVendorsSortedByVendorName();

            intNumberOfRecords = TheFindVendorSortedDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

            cboSelectVendor.Items.Add("Select Vendor");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectVendor.Items.Add(TheFindVendorSortedDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboSelectVendor.SelectedIndex = 0;

            lblSelectVendor.Visibility = Visibility.Hidden;
            cboSelectVendor.Visibility = Visibility.Hidden;

            TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(0);

            dgrOpenProblems.ItemsSource = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

            mitProcessProblem.IsEnabled = false;
        }

        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            gblnInShop = true;
            lblSelectVendor.Visibility = Visibility.Visible;
            cboSelectVendor.Visibility = Visibility.Visible;
        }

        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnInShop = false;
            lblSelectVendor.Visibility = Visibility.Hidden;
            cboSelectVendor.Visibility = Visibility.Hidden;
        }

        private void mitResetForm_Click(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void dgrOpenProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                if (dgrOpenProblems.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrOpenProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(MainWindow.gintProblemID);

                    txtProblem.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;

                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Vehicle Dashboard // Vehicle In Shop // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectVendor.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintVendorID = TheFindVendorSortedDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // New Vehicle Problem // Select Vendor Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitProcessProblem_Click(object sender, RoutedEventArgs e)
        {
            string strVehicleProblem = "";
            string strVehicleNotes = "";
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            string strErrorMessage = "";

            try
            {
                strVehicleProblem = txtProblem.Text;
                if(strVehicleProblem == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehice Problem was Not Entered\n";
                }
                strVehicleNotes = txtAddedNotes.Text;
                if(strVehicleNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Problem Notes were not Entered\n";
                }
                if(gblnInShop == true)
                {
                    if(cboSelectVendor.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vendor was not Selected\n";
                    }
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, datTransactionDate, strVehicleProblem);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(datTransactionDate);

                MainWindow.gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strVehicleNotes, datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                if(gblnInShop == true)
                {
                    blnFatalError = TheVehiclesInShopClass.InsertVehicleInShop(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gintVendorID, strVehicleProblem, MainWindow.gintProblemID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Problem Has Been reported");

                MessageBoxResult result = MessageBox.Show("Do You Want To Print A Copy of the problem", "Please Choose", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TheVehicleProblemPrintClass.PrintVehicleProblemInfo();
                }

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // New Vehicle Problem // Process Problem Menu Item " + Ex.Message);
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
