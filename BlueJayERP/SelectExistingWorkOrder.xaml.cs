/* Title:           Select Existing Work Order
 * Date:            6-5-18
 * Author:          Terry Holmes
 * 
 * Description:     This is for selecting an existing work order*/

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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SelectExistingWorkOrder.xaml
    /// </summary>
    public partial class SelectExistingWorkOrder : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        
        public SelectExistingWorkOrder()
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
            MainWindow.gblnWorkOrderSelected = false;
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intRecordsReturned;

            MainWindow.gblnWorkOrderSelected = false;
            
            MainWindow.TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

            intRecordsReturned = MainWindow.TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID.Rows.Count;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.InformationMessage("No Work Order Open");
                Close();
            }

            dgrResults.ItemsSource = MainWindow.TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID;
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                //setting local variable
                dataGrid = dgrResults;
                selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                strProblemID = ((TextBlock)ProblemID.Content).Text;

                //find the record
                MainWindow.gintProblemID = Convert.ToInt32(strProblemID);
                MainWindow.gblnWorkOrderSelected = true;
                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Select Existing Work Order // Grid Selection " + Ex.Message);

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
