/* Title:           Add IT Assets
 * Date:            1-23-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add an it asset */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using DataValidationDLL;
using ItAssetsDLL;
using EmployeeDateEntryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddITAssets.xaml
    /// </summary>
    public partial class AddITAssets : Window
    {
        //setting up the classes
        WPFMessagesClass TheMesssagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();

        

        public AddITAssets()
        {
            InitializeComponent();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.LaunchHelpSite();
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.LaunchEmail();
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.MyOriginatingTask();
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.MyOpenTasks();
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.AddTask();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMesssagesClass.CloseTheProgram();
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

            txtCost.Text = "0";
            txtItem.Text = "";
            txtManufacturer.Text = "";
            txtModel.Text = "";
            txtQuantity.Text = "1";
            txtSerialNumber.Text = "";
            txtUpgrades.Text = "";

            cboSelectWarehouse.Items.Clear();
            cboSelectWarehouse.Items.Add("Select Office");

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectWarehouse.SelectedIndex = 0;
        }

        private void CboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strItem;
            string strManufacturer;
            string strModel;
            string strSerialNumber;
            string strUpgrades;
            string strValueForValue;
            int intQuantity = 0;
            decimal decItemValue = 0;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;

            try
            {
                //data validation
                strItem = txtItem.Text;
                if(strItem == "")
                {
                    strItem = "";
                }
                strManufacturer = txtManufacturer.Text;
                if(strManufacturer == "")
                {
                    strManufacturer = "";
                }
                strModel = txtModel.Text;
                if(strModel == "")
                {
                    strModel = "";
                }
                strSerialNumber = txtSerialNumber.Text;
                if(strSerialNumber == "")
                {
                    strSerialNumber = "";
                }
                strValueForValue = txtQuantity.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValue);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Quantity is not an Integer\n";
                }
                else
                {
                    intQuantity = Convert.ToInt32(strValueForValue);
                }
                strValueForValue = txtCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValue);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The cost is not numeric\n";
                }
                else
                {
                    decItemValue = Convert.ToDecimal(strValueForValue);
                }
                strUpgrades = txtUpgrades.Text;
                if(strUpgrades == "")
                {
                    strUpgrades = "";
                }
                if(cboSelectWarehouse.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMesssagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheITAssetsClass.InsertITAsset(strItem, strManufacturer, strModel, strSerialNumber, intQuantity, decItemValue, strUpgrades, MainWindow.gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMesssagesClass.InformationMessage("The Asset has been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add IT Assets // Process Menu Item " + Ex.Message);

                TheMesssagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
