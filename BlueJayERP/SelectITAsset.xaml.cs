/* Title:           Select IT Asset
 * Date:            12-6-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used for selecting an IT Asset for editing */

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
using ItAssetsDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SelectITAsset.xaml
    /// </summary>
    public partial class SelectITAsset : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ITAssetsClass TheITAssetsClass = new ITAssetsClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up data
        FindITAssetBySerialNumberDataSet TheFindITAssetBySerialNumberDataSet = new FindITAssetBySerialNumberDataSet();
        FindITAssetsByItemDataSet TheFindITAssetsByItemDataSet = new FindITAssetsByItemDataSet();

        public SelectITAsset()
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterItem.Text = "";

            TheFindITAssetBySerialNumberDataSet = TheITAssetsClass.FindITAssetBySerialNumber("fuck you asshole");

            dgrResults.ItemsSource = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void TxtEnterItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strSerialNumber;
            int intLength;
            int intRecordsReturned;

            strSerialNumber = txtEnterItem.Text;

            intLength = strSerialNumber.Length;

            if(intLength > 4)
            {
                TheFindITAssetBySerialNumberDataSet = TheITAssetsClass.FindITAssetBySerialNumber(strSerialNumber);

                intRecordsReturned = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheFindITAssetsByItemDataSet = TheITAssetsClass.FindITAssetsByItem(strSerialNumber);

                    intRecordsReturned = TheFindITAssetsByItemDataSet.FindITAssetByItem.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("No Records Found");
                        return;
                    }

                    dgrResults.ItemsSource = TheFindITAssetsByItemDataSet.FindITAssetByItem;
                }
                else
                {
                    dgrResults.ItemsSource = TheFindITAssetBySerialNumberDataSet.FindITAssetBySerialNumber;
                }
            }
        }

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ItemID;
            string strItemID;

            try
            {
                if (dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ItemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strItemID = ((TextBlock)ItemID.Content).Text;

                    MainWindow.gintItemID = Convert.ToInt32(strItemID);

                    ChangeITAssetLocation ChangeITAssetLocation = new ChangeITAssetLocation();
                    ChangeITAssetLocation.ShowDialog();

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Select IT Asset // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
