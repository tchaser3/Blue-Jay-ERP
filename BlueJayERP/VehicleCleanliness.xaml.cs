/* Title:           Vehicle Cleanliness
 * Date:            4-20-18
 * Author:          Terry Holmes
 * 
 * Description:     This where the vehicle cleanliness will be added */

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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleCleanliness.xaml
    /// </summary>
    public partial class VehicleCleanliness : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public VehicleCleanliness()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrCleanlinessNotes = txtNotes.Text;

            if(MainWindow.gstrCleanlinessNotes == "")
            {
                TheMessagesClass.ErrorMessage("No Notes Entered");
                return;
            }

            Close();
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
