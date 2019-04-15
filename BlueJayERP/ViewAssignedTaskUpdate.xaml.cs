/* Title:           View Assigned Task Update
 * Date:            8-3-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to see assigned taks */

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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewAssignedTaskUpdate.xaml
    /// </summary>
    public partial class ViewAssignedTaskUpdate : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AssignedTaskClass TheAssignedTaskClass = new AssignedTaskClass();

        FindAssignedTaskUpdateByTransactionIDDataSet TheFindAssignedTaskUpdateByTransactionIDDataSet = new FindAssignedTaskUpdateByTransactionIDDataSet();

        public ViewAssignedTaskUpdate()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindAssignedTaskUpdateByTransactionIDDataSet = TheAssignedTaskClass.FindAssignedTaskUpdateByTransactionID(MainWindow.gintTransactionID);

            txtDate.Text = Convert.ToString(TheFindAssignedTaskUpdateByTransactionIDDataSet.FindAssignedTaskUpdateByTransactionID[0].TransactionDate);
            txtEmployee.Text = TheFindAssignedTaskUpdateByTransactionIDDataSet.FindAssignedTaskUpdateByTransactionID[0].FirstName + " " + TheFindAssignedTaskUpdateByTransactionIDDataSet.FindAssignedTaskUpdateByTransactionID[0].LastName;
            txtTaskSubjext.Text = TheFindAssignedTaskUpdateByTransactionIDDataSet.FindAssignedTaskUpdateByTransactionID[0].MessageSubject;
            txtTaskUpdate.Text = TheFindAssignedTaskUpdateByTransactionIDDataSet.FindAssignedTaskUpdateByTransactionID[0].UpdateNotes;
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
    }
}
