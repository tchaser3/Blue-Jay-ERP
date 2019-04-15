/* Title:           Select Vehicle Bulk Tool
 * Date:            5-30-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is for selecting the tool for editing*/

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
using VehicleBulkToolsDLL;
using VehicleMainDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SelectVehicleBulkTool.xaml
    /// </summary>
    public partial class SelectVehicleBulkTool : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        FindAllBulkToolsForVehicleDataSet TheFindAllBulkToolsForVehicleDataSet = new FindAllBulkToolsForVehicleDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        string gstrVehicleNumber;

        public SelectVehicleBulkTool()
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
            txtVehicleNumber.Text = "";
            TheFindAllBulkToolsForVehicleDataSet = TheVehicleBulkToolsClass.FindAllBulkToolsForVehicle("NO TOOLS");
            dgrResults.ItemsSource = TheFindAllBulkToolsForVehicleDataSet.FindAllBulkToolsForVehicle;
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;

            gstrVehicleNumber = txtVehicleNumber.Text;

            TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(gstrVehicleNumber);

            intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("The Vehicle Number Entered does not Exist");
                return;
            }

            TheFindAllBulkToolsForVehicleDataSet = TheVehicleBulkToolsClass.FindAllBulkToolsForVehicle(gstrVehicleNumber);

            dgrResults.ItemsSource = TheFindAllBulkToolsForVehicleDataSet.FindAllBulkToolsForVehicle;
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;
            int intSelectedIndex;

            try
            {
                //setting local variable
                intSelectedIndex = dgrResults.SelectedIndex;
                if(intSelectedIndex > -1)
                {
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTransactionID = ((TextBlock)TransactionID.Content).Text;

                    //find the record
                    MainWindow.gintTransactionID = Convert.ToInt32(strTransactionID);
                    MainWindow.TheFindVehicleBulkByTransactionIDDataSet = TheVehicleBulkToolsClass.FindVehicleBulkToolByTransactionID(MainWindow.gintTransactionID);

                    EditVehicleBulkTool EditVehicleBulkTool = new EditVehicleBulkTool();
                    EditVehicleBulkTool.ShowDialog();

                    TheFindAllBulkToolsForVehicleDataSet = TheVehicleBulkToolsClass.FindAllBulkToolsForVehicle(gstrVehicleNumber);

                    dgrResults.ItemsSource = TheFindAllBulkToolsForVehicleDataSet.FindAllBulkToolsForVehicle;
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Labor Hours // Grid Selection " + Ex.Message);

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
