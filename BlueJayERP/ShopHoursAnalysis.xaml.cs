/* Title:           Shop Hours Analysis
 * Date:            3-6-18
 * Author:          Terry Holmes
 * 
 * Description:     This will show information regarding employees in the shop */

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
using ProjectsDLL;
using EmployeeProjectAssignmentDLL;
using NewEventLogDLL;
using DateSearchDLL;
using EmployeeProductivityStatsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ShopHoursAnalysis.xaml
    /// </summary>
    public partial class ShopHoursAnalysis : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectClass TheProjectClass = new ProjectClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();

        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectHoursDataSet TheFindProjectHoursDataSet = new FindProjectHoursDataSet();
        ShopViolatorDataSet TheShopViolatorDataSet = new ShopViolatorDataSet();
        FindProjectStatsDataSet TheFindProjectStatsDataSet = new FindProjectStatsDataSet();

        //setting variables
        decimal gdecTotalHours;
        decimal gdecMean;
        decimal gdecStandDeviation;
        decimal gdecVariance;
        decimal gdecUpperBound;
        decimal gdecProjectHours;
        decimal gdecProjectCost;

        public ShopHoursAnalysis()
        {
            InitializeComponent();
        }

        private void mitExportToExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mitPrint_Click(object sender, RoutedEventArgs e)
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

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheShopViolatorDataSet.violator.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Employees In Shop Hours"))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Average Hours In Shop " + Convert.ToString(gdecMean) + " With an Upper Bound Of " + Convert.ToString(gdecUpperBound)))));
                    newTableRow.Cells[0].FontSize = 18;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Projected Hours for the Year " + Convert.ToString(gdecProjectHours) + " With an Projected Cost of " + Convert.ToString(gdecProjectCost)))));
                    newTableRow.Cells[0].FontSize = 18;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Reported Times Above Upper Bound"))));
                    newTableRow.Cells[0].FontSize = 18;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Hours"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    //newTableRow.Cells[0].ColumnSpan = 1;
                    //newTableRow.Cells[1].ColumnSpan = 1;
                    //newTableRow.Cells[2].ColumnSpan = 1;
                    //newTableRow.Cells[3].ColumnSpan = 2;
                    //newTableRow.Cells[4].ColumnSpan = 1;

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheShopViolatorDataSet.violator.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheShopViolatorDataSet.violator[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Hours in the Shop");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Project Hours // Print Menu Item " + Ex.Message);
            }
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting up the local variables
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            DateTime datTransactionDate = DateTime.Now;

            try
            {
                datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);

                datTransactionDate = TheDateSearchClass.SubtractingDays(datTransactionDate, 31);

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID("SHOP");

                intProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                TheFindProjectHoursDataSet = TheEmployeeProjectAssignmentClass.FindProjectHours(intProjectID, datTransactionDate);

                TheFindProjectStatsDataSet = TheEmployeeProductivityStatsClass.FindProjectStats(intProjectID);

                intNumberOfRecords = TheFindProjectHoursDataSet.FindProjectHours.Rows.Count - 1;

                gdecMean = TheFindProjectStatsDataSet.FindProjectStats[0].AveHours;
                gdecStandDeviation = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].HoursSTDev);
                gdecVariance = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].HoursVariance);
                gdecTotalHours = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].TotalHours);

              
                gdecMean = Math.Round(gdecMean, 4);

                txtAverageHours.Text = Convert.ToString(gdecMean);
                
                gdecVariance = Math.Round(gdecVariance, 4);
                gdecStandDeviation = Math.Round(gdecStandDeviation, 4);
                
                gdecUpperBound = gdecMean + gdecStandDeviation;

                txtUpperBound.Text = Convert.ToString(gdecUpperBound);

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindProjectHoursDataSet.FindProjectHours[intCounter].TransactionDate > datTransactionDate)
                    {
                        if (TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours > gdecUpperBound)
                        {
                            ShopViolatorDataSet.violatorRow NewViolatorRow = TheShopViolatorDataSet.violator.NewviolatorRow();

                            NewViolatorRow.FirstName = TheFindProjectHoursDataSet.FindProjectHours[intCounter].FirstName;
                            NewViolatorRow.LastName = TheFindProjectHoursDataSet.FindProjectHours[intCounter].LastName;
                            NewViolatorRow.Hours = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TotalHours;
                            NewViolatorRow.TransactionDate = TheFindProjectHoursDataSet.FindProjectHours[intCounter].TransactionDate;
                            NewViolatorRow.HomeOffice = TheFindProjectHoursDataSet.FindProjectHours[intCounter].HomeOffice;

                            TheShopViolatorDataSet.violator.Rows.Add(NewViolatorRow);
                        }
                    }
                }

                dgrResults.ItemsSource = TheShopViolatorDataSet.violator;

                gdecProjectHours = gdecMean * 5 * 52;
                txtProjectHours.Text = Convert.ToString(gdecProjectHours);
                gdecProjectCost = gdecProjectHours * 12;
                txtProjectedCost.Text = Convert.ToString(gdecProjectCost);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Shop Hours Analysis // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
