/* Title:           Open Tool Problems
 * Date:            7-19-18
 * Author:          Terrance Holmes
 * 
 * Description:     This form will show all open problems */

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
using ToolProblemDLL;
using NewToolsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for OpenToolProblems.xaml
    /// </summary>
    public partial class OpenToolProblems : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolProblemClass TheToolProblemClass = new ToolProblemClass();
        ToolsClass TheToolsClass = new ToolsClass();

        //data
        FindOpenToolProblemsDataSet TheFindOpenToolProblemsDataSet = new FindOpenToolProblemsDataSet();
        
        public OpenToolProblems()
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
            TheFindOpenToolProblemsDataSet = TheToolProblemClass.FindOpenToolProblems();

            dgrOpenProblems.ItemsSource = TheFindOpenToolProblemsDataSet.FindOpenToolProblems;
        }

        private void dgrOpenProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;
            DataGridCell ToolID;
            DataGridCell ToolDescription;

            try
            {
                //setting local variable
                if(dgrOpenProblems.SelectedIndex > -1)
                {
                    dataGrid = dgrOpenProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    ToolID = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    MainWindow.gstrToolID = ((TextBlock)ToolID.Content).Text;
                    ToolDescription = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    MainWindow.gstrToolDescription = ((TextBlock)ToolDescription.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    ToolProblemInformation ToolProblemInformation = new ToolProblemInformation();
                    ToolProblemInformation.ShowDialog();
                }               
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Open Tool Problems // Grid Selection " + Ex.Message);

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
