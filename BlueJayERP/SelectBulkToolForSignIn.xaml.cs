/* Title:           Select Bulk Tool Sign In
 * Date:            11-16-18
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the usesr to select the bulk Tool for sign in */

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
using BulkToolsDLL;
using NewEmployeeDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for SelectBulkToolForSignIn.xaml
    /// </summary>
    public partial class SelectBulkToolForSignIn : Window
    {
        //setting up the clases
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        BulkToolsClass TheBulkToolsClass = new BulkToolsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //Setting up combo box
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindBulkToolsCurrentlyAssignedToEmployeeDataSet TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet = new FindBulkToolsCurrentlyAssignedToEmployeeDataSet();
        
        public SelectBulkToolForSignIn()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ResetControls()
        {
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet = TheBulkToolsClass.FindBulkToolsCurrentlyAssignedToEmployee(-1);
            dgrResults.ItemsSource = TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;

            if(intLength > 2)
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet = TheBulkToolsClass.FindBulkToolsCurrentlyAssignedToEmployee(MainWindow.gintEmployeeID);

                dgrResults.ItemsSource = TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee;
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;

            try
            {
                if (dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTransactionID = ((TextBlock)TransactionID.Content).Text;

                    MainWindow.gintTransactionID = Convert.ToInt32(strTransactionID);

                    EditEmployeeBulkTool EditEmployeeBulkTool = new EditEmployeeBulkTool();
                    EditEmployeeBulkTool.ShowDialog();

                    TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet = TheBulkToolsClass.FindBulkToolsCurrentlyAssignedToEmployee(MainWindow.gintEmployeeID);

                    dgrResults.ItemsSource = TheFindBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee;

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Select Bulk Tool For Sign In // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
