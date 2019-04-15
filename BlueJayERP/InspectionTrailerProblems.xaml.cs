/* Title:           Inspection Trailer Problems
 * Date:            11-9-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to ether select or add a new trailer problem */

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
using TrailerProblemDLL;
using TrailerProblemUpdateDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for InspectionTrailerProblems.xaml
    /// </summary>
    public partial class InspectionTrailerProblems : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        TrailerProblemUpdateClass TheTrailerProblemUpdateClass = new TrailerProblemUpdateClass();

        FindOpenTrailerProblemsByTrailerIDDataSet TheFindOpenTrailerProblemsByTrailerIDDataSet = new FindOpenTrailerProblemsByTrailerIDDataSet();
        ExistingTrailerProblemsDataSet TheExistingTrailerProblemsDataSet = new ExistingTrailerProblemsDataSet();
        FindTrailerProblemByDateMatchDataSet TheFindTrailerProblemByDateMatchDataSet = new FindTrailerProblemByDateMatchDataSet();

        int gintWarehouseEmployeeID;

        public InspectionTrailerProblems()
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

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            string strTrailerProblem;
            string strErrorMessage = "";
            int intTrailerProblemLength;

            try
            {
                if(cboMultipleProblems.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Multiple Problems Not Selected\n";
                }
                if(cboNewProblem.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "New Problem Was Not Selected\n";
                }
                strTrailerProblem = txtProblem.Text;
                intTrailerProblemLength = strTrailerProblem.Length;
                if(intTrailerProblemLength < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Entered Is Not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(cboNewProblem.SelectedIndex == 1)
                {
                    blnFatalError = TheTrailerProblemClass.InsertTrailerProblem(MainWindow.gintTrailerID, datTransactionDate, gintWarehouseEmployeeID, strTrailerProblem, 1001);

                    if(blnFatalError == true)
                    {
                        throw new Exception();
                    }

                    TheFindTrailerProblemByDateMatchDataSet = TheTrailerProblemClass.FindTrailerProblemByDateMatch(datTransactionDate);

                    MainWindow.gintProblemID = TheFindTrailerProblemByDateMatchDataSet.FindTrailerProblemByDateMatch[0].ProblemID;
                }

                if(MainWindow.gintProblemID == -1)
                {
                    TheMessagesClass.ErrorMessage("A Problem was not Selected");
                    return;
                }

                blnFatalError = TheTrailerProblemUpdateClass.InsertTrailerProblemUpdate(MainWindow.gintProblemID, gintWarehouseEmployeeID, strTrailerProblem);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                TheMessagesClass.InformationMessage("Problem Was Entered");

                if(cboMultipleProblems.SelectedIndex == 2)
                {
                    Close();
                }

                MainWindow.gstrInspectionProblem += strTrailerProblem + "\n";

                MainWindow.gintProblemID = 0;
                txtProblem.Text = "";

                TheFindOpenTrailerProblemsByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

                dgrResults.ItemsSource = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID;

                cboNewProblem.IsEnabled = true;
                cboNewProblem.SelectedIndex = 0;

            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Communications // Inspection Trailer Problem // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

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
            //setting up variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //setting up controls
                cboMultipleProblems.Items.Add("Select");
                cboMultipleProblems.Items.Add("Yes");
                cboMultipleProblems.Items.Add("No");
                cboMultipleProblems.SelectedIndex = 0;

                cboNewProblem.Items.Add("Select");
                cboNewProblem.Items.Add("Yes");
                cboNewProblem.Items.Add("No");
                cboNewProblem.SelectedIndex = 0;

                dgrResults.IsEnabled = false;
                MainWindow.gintProblemID = -1;

                TheFindOpenTrailerProblemsByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

                intNumberOfRecords = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID.Rows.Count - 1;

                gintWarehouseEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                MainWindow.gstrInspectionProblem = "";

                if (intNumberOfRecords < 0)
                {
                    cboNewProblem.SelectedIndex = 1;
                    cboNewProblem.IsEnabled = false;
                    dgrResults.IsEnabled = false;
                }
                else
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ExistingTrailerProblemsDataSet.existingtrailerproblemsRow NewProblemRow = TheExistingTrailerProblemsDataSet.existingtrailerproblems.NewexistingtrailerproblemsRow();

                        NewProblemRow.ProblemID = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].ProblemID;
                        NewProblemRow.ProblemReported = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].ProblemReported;
                        NewProblemRow.TransactionDate = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].TransactionDate;

                        TheExistingTrailerProblemsDataSet.existingtrailerproblems.Rows.Add(NewProblemRow);
                    }
                }

                dgrResults.ItemsSource = TheExistingTrailerProblemsDataSet.existingtrailerproblems;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Inspection Trailer Problems // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
           
        }

        private void cboNewProblem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboNewProblem.SelectedIndex == 1)
            {
                dgrResults.IsEnabled = false;
            }
            else if(cboNewProblem.SelectedIndex == 2)
            {
                dgrResults.IsEnabled = true;
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TrailerProblem;
            string strTrailerProblem;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if(intSelectedIndex > -1)
                {
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TrailerProblem = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    strTrailerProblem = ((TextBlock)TrailerProblem.Content).Text;
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    txtProblem.Text = strTrailerProblem; 
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Inspection Trailer Problems // Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
