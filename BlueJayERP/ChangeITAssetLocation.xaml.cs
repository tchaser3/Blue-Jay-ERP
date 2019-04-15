/* Title:           Change IT Asset Location
 * Date:            12-5-2018
 * Author:          Terry Holmes
 * 
 * Description:     This window will allow the user to change the location of an IT Asset */

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
    /// Interaction logic for ChangeITAssetLocation.xaml
    /// </summary>
    public partial class ChangeITAssetLocation : Window
    {
        //setting up class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();

        //setting up data
        FindITAssetsByItemIDDataSet TheFindITAssetsByItemIDDataSet = new FindITAssetsByItemIDDataSet();

        public ChangeITAssetLocation()
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
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                cboSelectWarehouse.Items.Add("Select Warehouse");

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;

                TheFindITAssetsByItemIDDataSet = TheITAssetsClass.FindITAssetsByItemID(MainWindow.gintItemID);

                txtHomeOffice.Text = TheFindITAssetsByItemIDDataSet.FindITAssetByItemID[0].HomeOffice;
                txtItem.Text = TheFindITAssetsByItemIDDataSet.FindITAssetByItemID[0].Item;
                txtItemID.Text = Convert.ToString(TheFindITAssetsByItemIDDataSet.FindITAssetByItemID[0].ItemID);
                txtManufacturer.Text = TheFindITAssetsByItemIDDataSet.FindITAssetByItemID[0].Manufacturer;
                txtModel.Text = TheFindITAssetsByItemIDDataSet.FindITAssetByItemID[0].Model;
                txtSerialNumber.Text = TheFindITAssetsByItemIDDataSet.FindITAssetByItemID[0].SerialNumber;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Change IT Asset Location // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectIndex].EmployeeID;
            }
        }

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                if(cboSelectWarehouse.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("A New Location Was Not Selected");
                    return;
                }

                blnFatalError = TheITAssetsClass.UpdateITAssetLocation(MainWindow.gintItemID, MainWindow.gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Asset Location has been Updated");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Change IT Asset Location // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
