/* Title:           DOT Audit Report
 * Date:            4-3-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used for DOT Audits */

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
using Excel = Microsoft.Office.Interop.Excel;
using NewEventLogDLL;
using DateSearchDLL;
using VehicleMainDLL;
using InspectionsDLL;
using VehicleProblemsDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DOTAuditReport.xaml
    /// </summary>
    public partial class DOTAuditReport : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        VehicleProblemClass TheVehicleProblemsClass = new VehicleProblemClass();
        InspectionsClass TheInspectionsClass = new InspectionsClass();

        //setting up the data
        DOTAuditDataSet TheDOTAuditDataSet = new DOTAuditDataSet();
        DOTAuditDataSet TheSortedDOTAuditDataSete = new DOTAuditDataSet();
        EventLogClass TheEventLogClass = new EventLogClass();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleBVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindActiveVehicleMainSortedDataSet ThefindActiveVehicleMainSortedDataSet = new FindActiveVehicleMainSortedDataSet();
        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet = new FindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet();
        DOTVehicleInspectionDataSet TheDOTVehicleInspectionDataSet = new DOTVehicleInspectionDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;

        //setting up variables
        //int gintColumnCounter;
        PrintDialog pdProblemReport;

        public DOTAuditReport()
        {
            InitializeComponent();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitImportExcel_Click(object sender, RoutedEventArgs e)
        {
            bool blnIsNotInteger = false;
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            string strInformation = "";
            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            string strVehicleNumber;
            int intVehicleID;
            int intSecondCounter;
            int intSecondNumberOfRecords;

            try
            {
                TheDOTAuditDataSet.dotaudit.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
               // dlg.DefaultExt = ".csv"; // Default file extension
                //dlg.Filter = "Excel (.csv)|*.csv"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count - 1;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strInformation = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2);

                    strInformation = strInformation.Substring(4, 4);
                    blnIsNotInteger = TheDataValidationClass.VerifyIntegerData(strInformation);
                    if (blnIsNotInteger == false)
                    {
                        DOTAuditDataSet.dotauditRow NewVehicleRow = TheDOTAuditDataSet.dotaudit.NewdotauditRow();

                        strVehicleNumber = strInformation;
                        TheFindActiveVehicleBVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);
                        intVehicleID = TheFindActiveVehicleBVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                        NewVehicleRow.VehicleID = intVehicleID;
                        NewVehicleRow.VehicleNumber = strVehicleNumber;
                        NewVehicleRow.VIN = TheFindActiveVehicleBVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VINNumber;

                        TheDOTAuditDataSet.dotaudit.Rows.Add(NewVehicleRow);
                    }

                }

                ThefindActiveVehicleMainSortedDataSet = TheVehicleMainClass.FindActiveVehicleMainSorted();

                intNumberOfRecords = ThefindActiveVehicleMainSortedDataSet.FindActiveVehicleMainSorted.Rows.Count - 1;

                intSecondNumberOfRecords = TheDOTAuditDataSet.dotaudit.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = ThefindActiveVehicleMainSortedDataSet.FindActiveVehicleMainSorted[intCounter].VehicleID;

                    for (intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        if (intVehicleID == TheDOTAuditDataSet.dotaudit[intSecondCounter].VehicleID)
                        {
                            DOTAuditDataSet.dotauditRow NewVehicleRow = TheSortedDOTAuditDataSete.dotaudit.NewdotauditRow();

                            NewVehicleRow.VehicleNumber = TheDOTAuditDataSet.dotaudit[intSecondCounter].VehicleNumber;
                            NewVehicleRow.VehicleID = intVehicleID;
                            NewVehicleRow.VIN = TheDOTAuditDataSet.dotaudit[intSecondCounter].VIN;

                            TheSortedDOTAuditDataSete.dotaudit.Rows.Add(NewVehicleRow);
                        }
                    }
                }
                
                PleaseWait.Close();
                dgrResults.ItemsSource = TheSortedDOTAuditDataSete.dotaudit;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // DOT Audit Report // Import Excel Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
}

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID;
            DateTime datTransactionDate;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            string strInspectStatus;
            string strVehicleProblem;
            string strInspectionNotes;
            int intThirdCounter;
            int intThirdNumberOfRecords;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                gdatEndDate = DateTime.Now;

                gdatEndDate = TheDateSearchClass.RemoveTime(gdatEndDate);

                gdatStartDate = TheDateSearchClass.SubtractingDays(gdatEndDate, 100);

                intNumberOfRecords = TheSortedDOTAuditDataSete.dotaudit.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = TheSortedDOTAuditDataSete.dotaudit[intCounter].VehicleID;

                    TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = TheInspectionsClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, gdatStartDate, gdatEndDate);

                    intSecondNumberOfRecords = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count - 1;

                    for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        strInspectStatus = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intSecondCounter].InspectionStatus;
                        datTransactionDate = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intSecondCounter].InspectionDate;
                        datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);
                        strVehicleProblem = "";
                        strInspectionNotes = "";

                        if(strInspectStatus == "PASSED")
                        {
                            strVehicleProblem = "NO PROBLEM REPORTED";
                            strInspectionNotes = "NO PROBLEM REPORTED";
                        }
                        else
                        {
                            TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet = TheInspectionsClass.FindDailyVehicleInspectionProblemByVehicleIDAndDateRange(intVehicleID, datTransactionDate, datTransactionDate.AddDays(1));

                            intThirdNumberOfRecords = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange.Rows.Count - 1;

                            for(intThirdCounter = 0; intThirdCounter <= intThirdNumberOfRecords; intThirdCounter++)
                            {
                                if (TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].VehicleProblem == null)
                                {
                                    strVehicleProblem = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].InspectionNotes + "\n";
                                }
                                else
                                {
                                    strVehicleProblem = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].VehicleProblem + "\n";
                                }

                                strInspectionNotes = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].InspectionNotes + "\n";
                            }
                        }


                        DOTVehicleInspectionDataSet.inspectionresultsRow NewInspectionRow = TheDOTVehicleInspectionDataSet.inspectionresults.NewinspectionresultsRow();

                        NewInspectionRow.VehicleNumber = TheSortedDOTAuditDataSete.dotaudit[intCounter].VehicleNumber;
                        NewInspectionRow.InspectionNotes = strInspectionNotes;
                        NewInspectionRow.InspectionStatus = strInspectStatus;
                        NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intSecondCounter].OdometerReading;
                        NewInspectionRow.TransactionDate = datTransactionDate;
                        NewInspectionRow.VehicleProblem = strVehicleProblem;

                        TheDOTVehicleInspectionDataSet.inspectionresults.Rows.Add(NewInspectionRow);
                    }
                }

                dgrResults.ItemsSource = TheDOTVehicleInspectionDataSet.inspectionresults;
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // DOT Audit Report // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void mitPrint_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID;
            DateTime datTransactionDate;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            string strInspectStatus;
            string strVehicleProblem;
            string strInspectionNotes;
            int intThirdCounter;
            int intThirdNumberOfRecords;


            pdProblemReport = new PrintDialog();

            if (pdProblemReport.ShowDialog().Value)
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                try
                {
                    gdatEndDate = DateTime.Now;

                    gdatEndDate = TheDateSearchClass.RemoveTime(gdatEndDate);

                    gdatStartDate = TheDateSearchClass.SubtractingDays(gdatEndDate, 200);

                    intNumberOfRecords = TheSortedDOTAuditDataSete.dotaudit.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        TheDOTVehicleInspectionDataSet.inspectionresults.Rows.Clear();

                        intVehicleID = TheSortedDOTAuditDataSete.dotaudit[intCounter].VehicleID;

                        TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = TheInspectionsClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, gdatStartDate, gdatEndDate);

                        intSecondNumberOfRecords = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count - 1;

                        for (intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                        {
                            strInspectStatus = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intSecondCounter].InspectionStatus;
                            datTransactionDate = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intSecondCounter].InspectionDate;
                            datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);
                            strVehicleProblem = "";
                            strInspectionNotes = "";

                            if (strInspectStatus == "PASSED")
                            {
                                strVehicleProblem = "NO PROBLEM REPORTED";
                                strInspectionNotes = "NO PROBLEM REPORTED";
                            }
                            else
                            {
                                TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet = TheInspectionsClass.FindDailyVehicleInspectionProblemByVehicleIDAndDateRange(intVehicleID, datTransactionDate, datTransactionDate.AddDays(1));

                                intThirdNumberOfRecords = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange.Rows.Count - 1;

                                for (intThirdCounter = 0; intThirdCounter <= intThirdNumberOfRecords; intThirdCounter++)
                                {
                                    if (TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].VehicleProblem == null)
                                    {
                                        strVehicleProblem = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].InspectionNotes + "\n";
                                    }
                                    else
                                    {
                                        strVehicleProblem = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].VehicleProblem + "\n";
                                    }

                                    strInspectionNotes = TheFindDailyVehicleInspectionProblemByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionProblemsByVehicleIDAndDateRange[intThirdCounter].InspectionNotes + "\n";
                                }
                            }


                            DOTVehicleInspectionDataSet.inspectionresultsRow NewInspectionRow = TheDOTVehicleInspectionDataSet.inspectionresults.NewinspectionresultsRow();

                            NewInspectionRow.VehicleNumber = TheSortedDOTAuditDataSete.dotaudit[intCounter].VehicleNumber;
                            NewInspectionRow.InspectionNotes = strInspectionNotes;
                            NewInspectionRow.InspectionStatus = strInspectStatus;
                            NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intSecondCounter].OdometerReading;
                            NewInspectionRow.TransactionDate = datTransactionDate;
                            NewInspectionRow.VehicleProblem = strVehicleProblem;

                            TheDOTVehicleInspectionDataSet.inspectionresults.Rows.Add(NewInspectionRow);
                        }

                        PrintReports();
                    }

                    dgrResults.ItemsSource = TheDOTVehicleInspectionDataSet.inspectionresults;
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // DOT Audit Report // Process Menu Item " + Ex.Message);

                    TheMessagesClass.ErrorMessage(Ex.ToString());
                }

                PleaseWait.Close();
            }
            
        }
        private void PrintReports()
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {

               
                    FlowDocument fdProjectReport = new FlowDocument();
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdProjectReport.PagePadding = thickness;

                    pdProblemReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheDOTVehicleInspectionDataSet.inspectionresults.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Production Report For " + "Vehicle"))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("BJC Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Inspection Results"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Problem"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Inspection Notes"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Odometer Reading"))));
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

                    intNumberOfRecords = TheDOTVehicleInspectionDataSet.inspectionresults.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheDOTVehicleInspectionDataSet.inspectionresults[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Project Production Report");
                    intCurrentRow = 0;

                
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // DOT Audit Report // Print Button " + Ex.Message);
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
