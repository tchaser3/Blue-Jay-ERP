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
using Microsoft.Win32;

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
            int intLenght;
            int intRecordsReturned;
            bool blnItemFound = false;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.gstrVehicleNumber = txtVehicleNumber.Text;
                intLenght = MainWindow.gstrVehicleNumber.Length;

                TheVehicleProblemDataSet.vehicleproblems.Rows.Clear();
                TheProblemNotesDataSet.problemnotes.Rows.Clear();

                if (intLenght == 4)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnItemFound = true;
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                }
                else if (intLenght == 6)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

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
                        NewProblemRow.TransactionDate = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].TransactionDate;

                        if(TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].IsProblemResolutionNull() == true)
                        {
                            NewProblemRow.ProblemResolution = "";
                        }
                        else
                        {
                            NewProblemRow.ProblemResolution = TheFindAllVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID[intCounter].ProblemResolution;
                        }                        

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

        private void mitPrintProblemList_Click(object sender, RoutedEventArgs e)
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {
                PrintDialog pdProblemReport = new PrintDialog();


                if (pdProblemReport.ShowDialog().Value)
                {
                    FlowDocument fdProjectReport = new FlowDocument();
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdProjectReport.PagePadding = thickness;

                    pdProblemReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns =TheVehicleProblemDataSet.vehicleproblems.Columns.Count;
                    fdProjectReport.ColumnWidth = 10;
                    fdProjectReport.IsColumnWidthFlexible = false;


                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        ProjectReportTable.Columns.Add(new TableColumn());
                    }
                    ProjectReportTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Problem List For " + MainWindow.gstrVehicleNumber))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Problem ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Problem"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Solved"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Problem Resolution"))));

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheVehicleProblemDataSet.vehicleproblems.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheVehicleProblemDataSet.vehicleproblems[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                            //if (intColumnCounter == 3)
                            //{
                            //newTableRow.Cells[intColumnCounter].ColumnSpan = 2;
                            //}

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProblemReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProblemReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProblemReport.PrintableAreaWidth;
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Vehicle Invoice Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Problem History// Print Problem List For Vehicle " + Ex.Message);
            }
        }

        private void mitExportProblemList_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheVehicleProblemDataSet.vehicleproblems.Rows.Count;
                intColumnNumberOfRecords = TheVehicleProblemDataSet.vehicleproblems.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleProblemDataSet.vehicleproblems.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheVehicleProblemDataSet.vehicleproblems.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Problem History // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
