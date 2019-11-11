/* Title:           Update Vehicle Problem
 * Date:            6-28-18
 * Author:          Terry Holmes
 * 
 * Description:     This is where a work order can be updated */

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
using VehiclesInShopDLL;
using VehicleProblemsDLL;
using VendorsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateVehicleProblem.xaml
    /// </summary>
    public partial class UpdateVehicleProblem : Window
    {
        //setting classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        VehicleProblemClass TheVehicleProblemsClass = new VehicleProblemClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        VehicleProblemPrintClass TheVehicleProblemPrintClass = new VehicleProblemPrintClass();

        //setting data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActivehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindOpenVehicleMainProblemsByVehicleIDDataSet TheFindOpenVehicleMainProblemsByVehicleIDDataSet = new FindOpenVehicleMainProblemsByVehicleIDDataSet();
        FindVehiclesInShopByVehicleIDDataSet TheFindVehiclesInShopByVehlcleIDDataSet = new FindVehiclesInShopByVehicleIDDataSet();
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorNameDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindVehicleInShopByProblemIDDataSet TheFindVehicleInShopByProblemIDDataSet = new FindVehicleInShopByProblemIDDataSet();

        //setting variables
        bool gblnVehicleInShop;
        bool gblnOrderselected;
        bool gblnWorkComplete;
        string gstrProblemStatus;

        public UpdateVehicleProblem()
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
            //setting local variables
            string strVehicleNumber;
            int intLength;
            int intRecordsReturned;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;
                MainWindow.gstrVehicleNumber = strVehicleNumber;
                intLength = strVehicleNumber.Length;
                if(intLength == 4)
                {
                    TheFindActivehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActivehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActivehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemsClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                        dgrOpenProblems.ItemsSource = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

                        TheFindVehiclesInShopByVehlcleIDDataSet = TheVehiclesInShopClass.FindVehiclesInShopByVehicleID(MainWindow.gintVehicleID);

                        intRecordsReturned = TheFindVehiclesInShopByVehlcleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            txtVehicleInyard.Text = "YES With " + TheFindVehiclesInShopByVehlcleIDDataSet.FindVehiclesInShopByVehicleID[0].VendorName;
                        }
                        else
                        {
                            txtVehicleInyard.Text = "Not In Shop";
                        }
                    }
                }
                else if (intLength == 6)
                {
                    TheFindActivehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActivehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActivehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemsClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                        dgrOpenProblems.ItemsSource = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;

                        TheFindVehiclesInShopByVehlcleIDDataSet = TheVehiclesInShopClass.FindVehiclesInShopByVehicleID(MainWindow.gintVehicleID);

                        intRecordsReturned = TheFindVehiclesInShopByVehlcleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            txtVehicleInyard.Text = "YES With " + TheFindVehiclesInShopByVehlcleIDDataSet.FindVehiclesInShopByVehicleID[0].VendorName;
                        }
                        else
                        {
                            txtVehicleInyard.Text = "Not In Shop";
                        }
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }
                }
                else if(intLength > 6)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Entered has to Many Characters");
                    return;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Vehicle Problem // Enter Vehicle Textbox " + Ex.Message);

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
                    MainWindow.gintVendorID = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;
                }
            }
            catch(Exception EX)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Vehicle Problem // Select Vendor Combo Box " + EX.Message);

                TheMessagesClass.ErrorMessage(EX.ToString());
            }
        }
        private void ResetControls()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            //setting controls
            txtEnterVehicleNumber.Text = "";
            txtVehicleInyard.Text = "";
            txtVehicleUpdate.Text = "";
            gblnOrderselected = false;
            cboWorkComplete.SelectedIndex = 0;
            cboVehicleInShop.SelectedIndex = 0;
            btnLoadDocuments.IsEnabled = false;
            

            //setting combo box
            cboSelectVendor.Items.Clear();

            TheFindVendorsSortedByVendorNameDataSet = TheVendorsClass.FindVendorsSortedByVendorName();

            intNumberOfRecords = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

            cboSelectVendor.Items.Add("Select Vendor");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectVendor.Items.Add(TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboSelectVendor.SelectedIndex = 0;
            mitUpdateProblem.IsEnabled = false;

            //clearing grid
            TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemsClass.FindOpenVehicleMainProblemsByVehicleID(0);
            dgrOpenProblems.ItemsSource = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboWorkComplete.Items.Add("Select Answer");
            cboWorkComplete.Items.Add("Yes");
            cboWorkComplete.Items.Add("No");
            cboVehicleInShop.Items.Add("SelectAnswer");
            cboVehicleInShop.Items.Add("Yes");
            cboVehicleInShop.Items.Add("No");
            ResetControls();

        }

        private void mitUpdateProblem_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strVehicleUpdate = "";
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;
            //bool blnItemFound = false;
            int intTransactionID;

            try
            {
                strVehicleUpdate = txtVehicleUpdate.Text;
                if (strVehicleUpdate == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Update Was Not Entered\n";
                }
                else
                {
                    if(strVehicleUpdate.Length < 20)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vehicle Update WAs Not Long Enough\n";
                    }
                }
                if(gblnVehicleInShop == true)
                {
                    if(cboSelectVendor.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vendor Was Not Selected\n";
                    }
                }
                if(cboVehicleInShop.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage =  "Vehicle In Shop Was Not Selected\n";
                }
                if(cboWorkComplete.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage = "Work Complete Was Not Selected\n";
                }
                if(gblnOrderselected == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Problem Was Not Selected\n";
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheVehicleProblemsClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strVehicleUpdate, datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                if(gblnWorkComplete == false)
                {
                    if (gblnVehicleInShop == true)
                    {
                        TheFindVehicleInShopByProblemIDDataSet = TheVehiclesInShopClass.FindVehicleInShopByProblemID(MainWindow.gintProblemID);

                        intRecordsReturned = TheFindVehicleInShopByProblemIDDataSet.FindVehicleInShopByProblemID.Rows.Count;

                        if(intRecordsReturned < 1)
                        {
                            blnFatalError = TheVehiclesInShopClass.InsertVehicleInShop(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gintVendorID, MainWindow.gstrVehicleProblem, MainWindow.gintProblemID);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheVehicleProblemsClass.ChangeVehicleProblemStatus(MainWindow.gintProblemID, "WIP");

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        else
                        {
                            if(TheFindVehicleInShopByProblemIDDataSet.FindVehicleInShopByProblemID[0].VendorID == MainWindow.gintVendorID)
                            {
                                
                            }
                            else
                            {
                                blnFatalError = TheVehicleProblemsClass.UpdateVehicleProblemVendorID(MainWindow.gintProblemID, MainWindow.gintVendorID);

                                if (blnFatalError == true)
                                    throw new Exception();

                                intTransactionID = TheFindVehicleInShopByProblemIDDataSet.FindVehicleInShopByProblemID[0].TransactionID;

                                blnFatalError = TheVehiclesInShopClass.UpdateVehicleInShopVendorID(intTransactionID, MainWindow.gintVendorID);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }
                        }

                    }
                    else
                    {
                        intRecordsReturned = TheFindVehiclesInShopByVehlcleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

                        blnFatalError = TheVehicleProblemsClass.ChangeVehicleProblemStatus(MainWindow.gintProblemID, "NEED WORK");

                        if (blnFatalError == true)
                            throw new Exception();

                        if (intRecordsReturned > 0)
                        {
                            blnFatalError = TheVehiclesInShopClass.RemoveVehicleInShop(MainWindow.gintVehicleID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }  
                else if(gblnWorkComplete == true)
                {
                    gstrProblemStatus = "AWAITING INVOICE";

                    blnFatalError = TheVehicleProblemsClass.ChangeVehicleProblemStatus(MainWindow.gintProblemID, gstrProblemStatus);

                    if (blnFatalError == true)
                        throw new Exception();
                   
                    blnFatalError = TheVehiclesInShopClass.RemoveVehicleInShop(MainWindow.gintVehicleID);

                    if (blnFatalError == true)
                        throw new Exception();

                }

                MessageBoxResult result = MessageBox.Show("Do You Want To Print A Copy of the problem", "Please Choose", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TheVehicleProblemPrintClass.PrintVehicleProblemInfo();
                }

                TheMessagesClass.InformationMessage("The Vehicle Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Vehicle Problem // Update Problem Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrOpenProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;
            DataGridCell Problem;

            try
            {
                if (dgrOpenProblems.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrOpenProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    Problem = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    MainWindow.gstrVehicleProblem = ((TextBlock)Problem.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);
                    mitUpdateProblem.IsEnabled = true;

                    gblnOrderselected = true;
                    btnLoadDocuments.IsEnabled = true;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Update Vehicle Problem // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboVehicleInShop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboVehicleInShop.SelectedIndex == 1)
            {
                gblnVehicleInShop = true;
            }
            else
            {
                gblnVehicleInShop = false;
            }
        }

        private void cboWorkComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboWorkComplete.SelectedIndex == 1)
            {
                gblnWorkComplete = true;
            }
            else
            {
                gblnWorkComplete = false;
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void BtnLoadDocuments_Click(object sender, RoutedEventArgs e)
        {
            LoadVehicleProblemDocuments LoadVehicleProblemDocuments = new LoadVehicleProblemDocuments();
            LoadVehicleProblemDocuments.ShowDialog();
        }
    }
}
