/* Title:           Sign Out IT Asset
 * Date:            9-17-19
 * Author:          Terry Holmes
 * 
 * Description:     This will allow an asset to be signed out */

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
using NewEmployeeDLL;
using ItAssetsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SignOutITAssets.xaml
    /// </summary>
    public partial class SignOutITAssets : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ITAssetsClass TheITAssetClass = new ITAssetsClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data
        FindITAssetBySerialNumberDataSet TheFindITAssetBySerialNumber = new FindITAssetBySerialNumberDataSet();
        FindITAssetsByItemDataSet TheFindITAssetsByItemDataSet = new FindITAssetsByItemDataSet();
        FindITAssetAssignmentByItemIDDataSet TheFindITAssetAssignmentByItemIDDataSet = new FindITAssetAssignmentByItemIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        //setting global variables
        int gintItemID;
        int gintAssigningEmployeeID;
        string gstrFullName;

        public SignOutITAssets()
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
            txtEnterItem.Text = "";
            txtEnterLastName.Text = "";
            txtHomeOffice.Text = "";
            txtItem.Text = "";
            txtManufacturer.Text = "";
            txtModel.Text = "";
            txtSerialNumber.Text = "";
            cboSelectEmployee.Items.Clear();
            mitProcess.IsEnabled = false;

            gintAssigningEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;
            string strItem;            

            try
            {
                strItem = txtEnterItem.Text;
                if(strItem == "")
                {
                    TheMessagesClass.ErrorMessage("The Item Or Serial Number Were\nNot Entered");
                    return;
                }

                TheFindITAssetBySerialNumber = TheITAssetClass.FindITAssetBySerialNumber(strItem);

                intRecordsReturned = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber.Rows.Count;

                if(intRecordsReturned == 1)
                {
                    txtHomeOffice.Text = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber[0].HomeOffice;
                    txtItem.Text = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber[0].Item;
                    txtManufacturer.Text = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber[0].Manufacturer;
                    txtModel.Text = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber[0].Model;
                    txtSerialNumber.Text = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber[0].SerialNumber;
                    gintItemID = TheFindITAssetBySerialNumber.FindITAssetBySerialNumber[0].ItemID;
                }
                else
                {
                    TheFindITAssetsByItemDataSet = TheITAssetClass.FindITAssetsByItem(strItem);

                    intRecordsReturned = TheFindITAssetsByItemDataSet.FindITAssetByItem.Rows.Count;

                    if(intRecordsReturned == 1)
                    {
                        txtHomeOffice.Text = TheFindITAssetsByItemDataSet.FindITAssetByItem[0].HomeOffice;
                        txtItem.Text = TheFindITAssetsByItemDataSet.FindITAssetByItem[0].Item;
                        txtManufacturer.Text = TheFindITAssetsByItemDataSet.FindITAssetByItem[0].Manufacturer;
                        txtModel.Text = TheFindITAssetsByItemDataSet.FindITAssetByItem[0].Model;
                        txtSerialNumber.Text = TheFindITAssetsByItemDataSet.FindITAssetByItem[0].SerialNumber;
                        gintItemID = TheFindITAssetsByItemDataSet.FindITAssetByItem[0].ItemID;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Asset not Found or Be More specific");
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Out IT Asset // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void TxtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboSelectEmployee.SelectedIndex = 0;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Out IT Asset // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                gstrFullName = TheComboEmployeeDataSet.employees[intSelectedIndex].FullName;

                mitProcess.IsEnabled = true;
            }
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            int intRecordsReturned;
            DateTime datTransactionDate = DateTime.Now;
            string strTransactionNotes;
            int intTransactionID;

            try
            {
                strTransactionNotes = "ITEM ASSIGNED TO " + gstrFullName;

                TheFindITAssetAssignmentByItemIDDataSet = TheITAssetClass.FindITAssetAssignmentByItemID(gintItemID);

                intRecordsReturned = TheFindITAssetAssignmentByItemIDDataSet.FindITAssetAssignmentByItemID.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    blnFatalError = TheITAssetClass.InsertITAssetAssignment(gintItemID, datTransactionDate, MainWindow.gintEmployeeID, strTransactionNotes);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(intRecordsReturned == 1)
                {
                    intTransactionID = TheFindITAssetAssignmentByItemIDDataSet.FindITAssetAssignmentByItemID[0].TransactionID;

                    blnFatalError = TheITAssetClass.UpdateITAssetAssignment(intTransactionID, datTransactionDate, MainWindow.gintEmployeeID, strTransactionNotes);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else
                {
                    TheMessagesClass.ErrorMessage("To Many Items were found, Please contact IT");

                    throw new Exception();
                }                

                blnFatalError = TheITAssetClass.InsertITAssetAssignmentHistory(gintItemID, MainWindow.gintEmployeeID, gintAssigningEmployeeID, strTransactionNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Asset Has Been Assigned");

                ResetControls();
            }    
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Sign Out IT Assets // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
