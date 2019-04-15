/* Title:           View Task Information
 * Date:            8-2-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form for looking at information on tasks you created */

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
using AssignedTasksDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewTaskInformation.xaml
    /// </summary>
    public partial class ViewTaskInformation : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AssignedTaskClass TheAssignedTaskClass = new AssignedTaskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindAssignedTaskUpdateByTaskIDDataSet TheFindAssignedTaskUpdateByTaskIDDataSet = new FindAssignedTaskUpdateByTaskIDDataSet();
        FindAssignedEmployeeTaskByTaskIDDataSet TheFindAssignedEmployeeTaskByTaskIDDataSet = new FindAssignedEmployeeTaskByTaskIDDataSet();

        public ViewTaskInformation()
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
            try
            {
                UpdateDataSet();

                rdoNo.IsChecked = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Task Information // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrUpdates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void UpdateDataSet()
        {
            TheFindAssignedTaskUpdateByTaskIDDataSet = TheAssignedTaskClass.FindAssignedTaskUpdateByTaskID(MainWindow.gintAssignedTaskID);

            dgrUpdates.ItemsSource = TheFindAssignedTaskUpdateByTaskIDDataSet.FindAssignedTaskUpdateByTaskID;
        }
        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheAssignedTaskClass.UpdateAssignedTask(MainWindow.gintAssignedTaskID, DateTime.Now, false);

                if (blnFatalError == true)
                    throw new Exception();

                UpdateSpecificTasks UpdateSpecificTasks = new UpdateSpecificTasks();
                UpdateSpecificTasks.ShowDialog();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Task Information // Radio Yes Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }
    }
}
