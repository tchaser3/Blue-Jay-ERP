/* Title:           Retire IT Assets
 * Date:            1-24-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to retire an IT Asset */

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
using ItAssetsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for RetireITAsset.xaml
    /// </summary>
    public partial class RetireITAsset : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();

        //setting data set
        FindITAssetBySerialNumberDataSet TheFindITAssetBySerialNumberDataSet = new FindITAssetBySerialNumberDataSet();

        //setting global id
        int gintItemID;

        public RetireITAsset()
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

        private void MitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strReason;
            DateTime datCloseDate = DateTime.Now;
            bool blnFatalError = false;

            try
            {
                strReason = txtReason.Text;
                if(strReason == "")
                {
                    TheMessagesClass.ErrorMessage("Retirement Reason was not Entered");
                    return;
                }

                blnFatalError = TheITAssetsClass.RetireITAssets(gintItemID, datCloseDate, strReason);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The IT Assets has been Retired");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Retire IT Asset // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
            txtEnterSerialNumber.Text = "";
            txtItem.Text = "";
            txtItemID.Text = "";
            txtManufacturer.Text = "";
            txtModel.Text = "";
            txtReason.Text = "";
            txtHomeOffice.Text = "";
            gintItemID = 0;
            mitProcess.IsEnabled = false;
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            string strSerialNumber;
            int intRecordsReturned;

            try
            {
                strSerialNumber = txtEnterSerialNumber.Text;
                if(strSerialNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Serial Number Was Not Entered");
                    return;
                }

                TheFindITAssetBySerialNumberDataSet = TheITAssetsClass.FindITAssetBySerialNumber(strSerialNumber);
                intRecordsReturned = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber.Rows.Count;
                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("The Serial Number was not Found");
                    return;
                }

                //loading controls
                gintItemID = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber[0].ItemID;
                txtItemID.Text = Convert.ToString(gintItemID);
                txtItem.Text = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber[0].Item;
                txtManufacturer.Text = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber[0].Manufacturer;
                txtModel.Text = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber[0].Model;
                txtHomeOffice.Text = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber[0].HomeOffice;
                mitProcess.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Retire IT Assets // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
