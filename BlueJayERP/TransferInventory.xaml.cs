/* Title:           Transfer Inventory
 * Date:            3-5-18
 * Author:          Terrance Holmes
 * 
 * Description:     This is the window to transfer Inventory*/

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
using InventoryDLL;
using CharterInventoryDLL;
using NewPartNumbersDLL;
using DataValidationDLL;
using NewEmployeeDLL;
using KeyWordDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for TransferInventory.xaml
    /// </summary>
    public partial class TransferInventory : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        CharterInventoryClass TheCharterInventoryClass = new CharterInventoryClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();

        //setting up global variables
        int gintSendingWarehouseID;
        int gintReceivingWarehouseID;
        int gintQuantity;
        bool gblnSendJHInventory;
        bool gblnReceiveJHInventory;
        int gintSendingTransactionID;
        int gintSendingQuantity;

        //setting up the data sets
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindCharterWarehouseInventoryForPartDataSet TheFindCharterWarehouseInventoryForPartDataSet = new FindCharterWarehouseInventoryForPartDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();
        
        public TransferInventory()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
            this.Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the controls
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.TheFindPartsWarehousesDataSet = TheEmployeeClass.FindPartsWarehouses();

                cboReceivingWarehouse.Items.Add("Selecting Receiving Warehouse");
                cboSendingWarehouse.Items.Add("Select Sending Warehouse");

                intNumberOfRecords = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboReceivingWarehouse.Items.Add(MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                    cboSendingWarehouse.Items.Add(MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboReceivingWarehouse.SelectedIndex = 0;
                cboSendingWarehouse.SelectedIndex = 0;
                cboReceivingWarehouse.IsEnabled = false;
                mitTransferInventory.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Transfer Inventory // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSendingWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strWarehouse;
            bool blnKeyWordNotFound;

            intSelectedIndex = cboSendingWarehouse.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintSendingWarehouseID = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                strWarehouse = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].FirstName;

                blnKeyWordNotFound = TheKeyWordClass.FindKeyWord("JH", strWarehouse);

                if(strWarehouse == "TRAINING")
                {
                    blnKeyWordNotFound = false;
                }

                if(blnKeyWordNotFound == false)
                {
                    gblnSendJHInventory = true;
                }
                else
                {
                    gblnSendJHInventory = false;
                }
            }
        }

        private void cboReceivingWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strWarehouse;
            bool blnKeyWordNotFound = true;

            intSelectedIndex = cboReceivingWarehouse.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintReceivingWarehouseID = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                strWarehouse = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].FirstName;

                blnKeyWordNotFound = TheKeyWordClass.FindKeyWord("JH", strWarehouse);

                if (strWarehouse == "TRAINING")
                {
                    blnKeyWordNotFound = false;
                }

                if (blnKeyWordNotFound == false)
                {
                    gblnReceiveJHInventory = true;
                }
                else
                {
                    gblnReceiveJHInventory = false;
                }

                mitTransferInventory.IsEnabled = true;
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intSelectedIndex;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = cboSendingWarehouse.SelectedIndex;

                if(intSelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Sending Warehouse Not Selected\n";
                }
                MainWindow.gstrPartNumber = txtEnterPartNumber.Text;
                if(MainWindow.gstrPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Number Was Not Entered\n";
                }
                strValueForValidation = txtEnterQuantity.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Quantity is not an Integer\n";
                }
                else
                {
                    gintQuantity = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(MainWindow.gstrPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.InformationMessage("Part Number Was Not Found");
                    return;
                }
                else
                {
                    MainWindow.gintPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                }

                if(gblnSendJHInventory == true)
                {
                    TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(MainWindow.gintPartID, gintSendingWarehouseID);

                    intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.InformationMessage("The Part in this Warehouse Does Not Exist");
                        return;
                    }
                    else if(gintQuantity > TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity)
                    {
                        TheMessagesClass.ErrorMessage("Quantity Transfered Cannot Be Greater that Warehouse Quantity");
                        return;
                    }
                    else
                    {
                        txtPartDescription.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;
                        gintSendingTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;
                        gintSendingQuantity = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity;
                    }
                }

                if (gblnSendJHInventory == false)
                {
                    TheFindCharterWarehouseInventoryForPartDataSet = TheCharterInventoryClass.FindCharterWarehouseInventoryForPart(MainWindow.gintPartID, gintSendingWarehouseID);

                    intRecordsReturned = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.InformationMessage("The Part in this Warehouse Does Not Exist");
                        return;
                    }
                    else if(gintQuantity > TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].Quantity)
                    {
                        TheMessagesClass.ErrorMessage("Quantity Transfered Cannot Be Greater that Warehouse Quantity");
                        return;
                    }
                    else
                    {
                        txtPartDescription.Text = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].PartDescription;
                        gintSendingTransactionID = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].TransactionID;
                        gintSendingQuantity = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].Quantity;
                    }
                }

                cboReceivingWarehouse.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Transfer Inventory // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if the number is changed, this will go into effect
            cboReceivingWarehouse.IsEnabled = false;
            mitTransferInventory.IsEnabled = false;
            cboReceivingWarehouse.SelectedIndex = 0;            
        }

        private void mitTransferInventory_Click(object sender, RoutedEventArgs e)
        {
            int intTransactionID;
            int intQuantity;
            int intRecordsReturned;
            bool blnFatalError;

            try
            {
                if (gblnReceiveJHInventory == true)
                {
                    TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(MainWindow.gintPartID, gintReceivingWarehouseID);

                    intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        blnFatalError = TheInventoryClass.InsertInventoryPart(MainWindow.gintPartID, gintQuantity, gintReceivingWarehouseID);

                        if(blnFatalError == true)
                            throw new Exception();
                    }
                    else
                    {
                        intTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;
                        intQuantity = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity;

                        intQuantity += gintQuantity;

                        blnFatalError = TheInventoryClass.UpdateInventoryPart(intTransactionID, intQuantity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    if(gblnSendJHInventory == true)
                    {
                        gintSendingQuantity -= gintQuantity;
                        blnFatalError = TheInventoryClass.UpdateInventoryPart(gintSendingTransactionID, gintSendingQuantity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else if(gblnSendJHInventory == false)
                    {
                        gintSendingQuantity -= gintQuantity;
                        blnFatalError = TheCharterInventoryClass.UpdateCharterInventory(gintSendingTransactionID, gintSendingQuantity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
                else if (gblnReceiveJHInventory == false)
                {
                    TheFindCharterWarehouseInventoryForPartDataSet = TheCharterInventoryClass.FindCharterWarehouseInventoryForPart(MainWindow.gintPartID, gintReceivingWarehouseID);

                    intRecordsReturned = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        blnFatalError = TheCharterInventoryClass.InsertCharterInventory(MainWindow.gintPartID, gintQuantity, gintReceivingWarehouseID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else
                    {
                        intTransactionID = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].TransactionID;
                        intQuantity = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].Quantity;

                        intQuantity += gintQuantity;

                        blnFatalError = TheCharterInventoryClass.UpdateCharterInventory(intTransactionID, intQuantity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    if (gblnSendJHInventory == true)
                    {
                        gintSendingQuantity -= gintQuantity;
                        blnFatalError = TheInventoryClass.UpdateInventoryPart(gintSendingTransactionID, gintSendingQuantity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else if (gblnSendJHInventory == false)
                    {
                        gintSendingQuantity -= gintQuantity;
                        blnFatalError = TheCharterInventoryClass.UpdateCharterInventory(gintSendingTransactionID, gintSendingQuantity);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Part Has Been Transferred");
                ResetForm();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Transfer Inventory // Transfer Inventory Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }
        private void ResetForm()
        {
            cboReceivingWarehouse.SelectedIndex = 0;
            cboSendingWarehouse.SelectedIndex = 0;
            txtEnterPartNumber.Text = "";
            txtEnterQuantity.Text = "";
            txtPartDescription.Text = "";
            mitTransferInventory.IsEnabled = false;
            cboReceivingWarehouse.IsEnabled = false;
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
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
