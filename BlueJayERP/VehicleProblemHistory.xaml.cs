/* Title:           Vehicle Problem History
 * Date:            7-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the file to show all repair work done on a vehicle */

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
using VehicleMainDLL;
using VehicleProblemsDLL;
using DateSearchDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleProblemHistory.xaml
    /// </summary>
    public partial class VehicleProblemHistory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        VehicleProblemPrintClass TheVehiclePrintProblemClass = new VehicleProblemPrintClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindAllVehicleMainProblemsByVehicleIDDataSet TheFindAllVehicleMainProblemsByVehicleIDDataSet = new FindAllVehicleMainProblemsByVehicleIDDataSet();
        VehicleProblemDataSet TheVehicleProblemDataSet = new VehicleProblemDataSet();
        FindVehicleMainProblemUpdateByProblemIDDataSet TheFindVehicleMainProblemUpdateByProblemIDDataSet = new FindVehicleMainProblemUpdateByProblemIDDataSet();
        ProblemNotesDataSet TheProblemNotesDataSet = new ProblemNotesDataSet();
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        
        public VehicleProblemHistory()
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
            TheVehicleProblemDataSet.vehicleproblems.Rows.Clear();
            TheProblemNotesDataSet.problemnotes.Rows.Clear();
            txtVehicleNumber.Text = "";
            dgrProblemNotes.ItemsSource = TheProblemNotesDataSet.problemnotes;
            dgrProblems.ItemsSource = TheVehicleProblemDataSet.vehicleproblems;
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void txtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up the variables
            string strVehicleNumber;
            int intLenght;
            int intRecordsReturned;
            bool blnItemFound = false;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strVehicleNumber = txtVehicleNumber.Text;
                intLenght = strVehicleNumber.Length;

                TheVehicleProblemDataSet.vehicleproblems.Rows.Clear();
                TheProblemNotesDataSet.problemnotes.Rows.Clear();

                if (intLenght == 4)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnItemFound = true;
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                }
                else if (intLenght == 6)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnItemFound = true;
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }
                }
                else if (intLenght > 6)
                {
                    TheMessagesClass.ErrorMessage("Too Many Characters for a Vehicle Number");
                    return;
                }

                if (blnItemFound == true)
                {
                    TheFindAllVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindAllVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                    intNumberOfRecords = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        VehicleProblemDataSet.vehicleproblemsRow NewProblemRow = TheVehicleProblemDataSet.vehicleproblems.NewvehicleproblemsRow();

                        NewProblemRow.Problem = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].Problem;
                        NewProblemRow.ProblemID = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].ProblemID;
                        NewProblemRow.Solved = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].ProblemSolved;
                        NewProblemRow.TransactionDate = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].TransactionDAte;

                        TheVehicleProblemDataSet.vehicleproblems.Rows.Add(NewProblemRow);
                    }

                    dgrProblems.ItemsSource = TheVehicleProblemDataSet.vehicleproblems;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Problem History // Text Box Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }

        private void dgrProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                if (dgrProblems.SelectedIndex > -1)
                {
                    TheProblemNotesDataSet.problemnotes.Rows.Clear();

                    //setting local variable
                    dataGrid = dgrProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;

                    //find the record
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    TheFindVehicleMainProblemUpdateByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemUpdateByProblemID(MainWindow.gintProblemID);

                    intNumberOfRecords = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ProblemNotesDataSet.problemnotesRow NewNotesProblem = TheProblemNotesDataSet.problemnotes.NewproblemnotesRow();

                        NewNotesProblem.TransactionDate = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].TransactionDate;
                        NewNotesProblem.ProblemNotes = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].ProblemUpdate;

                        TheProblemNotesDataSet.problemnotes.Rows.Add(NewNotesProblem);
                    }

                    dgrProblemNotes.ItemsSource = TheProblemNotesDataSet.problemnotes;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Vehicle Dashboard // Vehicle In Shop // Problems Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitPRint_Click(object sender, RoutedEventArgs e)
        {
            TheVehiclePrintProblemClass.PrintVehicleProblemInfo();
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
