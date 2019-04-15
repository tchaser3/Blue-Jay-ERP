/* Title:           Vehicle Problem Notes
 * Date:            7-5-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form that is used to see the Vehicle Problem Notes */

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
using VehicleProblemsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleProblemNotes.xaml
    /// </summary>
    public partial class VehicleProblemNotes : Window
    {
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        VehicleProblemClass theVehicleProblemClass = new VehicleProblemClass();

        FindVehicleProblemUpdateByProblemIDDataSet TheFindVehicleProblemUpdateByProblemIDDataSet = new FindVehicleProblemUpdateByProblemIDDataSet();

        public VehicleProblemNotes()
        {
            InitializeComponent();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.LaunchHelpSite();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindVehicleProblemUpdateByProblemIDDataSet = theVehicleProblemClass.FindVehicleProblemUpdateByProblemID(MainWindow.gintProblemID);

            dgrResults.ItemsSource = TheFindVehicleProblemUpdateByProblemIDDataSet.FindVehicleProblemUpdateByProblemID;
        }

        private void mitEmail_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.LaunchEmail();
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.MyOriginatingTask();
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.MyOpenTasks();
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.AddTask();
        }
    }
}
