/* Title:           Employee Crew Summary
 * Date:            3-22-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to provide a summary of an employee Crew */

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
using EmployeeProductivityStatsDLL;
using DataValidationDLL;
using DateSearchDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using EmployeeCrewAssignmentDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EmployeeCrewSummary.xaml
    /// </summary>
    public partial class EmployeeCrewSummary : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
          
        FindEmployeeCrewByCrewIDDataSet TheFindEmployeeCrewByCrewIDDataSet = new FindEmployeeCrewByCrewIDDataSet();
        ProjectWorkCompletedDataSet TheProjectWorkCompleteDataSet = new ProjectWorkCompletedDataSet();
        CrewSummaryDataSet TheCrewSummaryDataSet = new CrewSummaryDataSet();
        FindEmployeeCrewAssignmentComboBoxDataSet TheFindEmployeeCrewAssignmentComboBoxDataSet = new FindEmployeeCrewAssignmentComboBoxDataSet();
        FindEmployeeProjectHourDateRangeSummaryDataSet TheFindEmployeeProjectHourDateRangeSummaryDataSet = new FindEmployeeProjectHourDateRangeSummaryDataSet();
        FindProjectTaskEmployeesStatsDataSet TheFindProjectTaskEmployeeStatsDataSet = new FindProjectTaskEmployeesStatsDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        decimal gdecTotalHours;
        string gstrCrewID;
        int gintTaskCounter;
        int gintTaskUpperLimit;

        public EmployeeCrewSummary()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitFindCrews_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            int intCounter;
            int intNumberOfRecords;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                strValueForValidation = txtEnterStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                txtTotalHours.Text = "";
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEnterEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                cboSelectCrew.Items.Clear();
                cboSelectCrew.Items.Add("Select Crew ID");

                TheFindEmployeeCrewAssignmentComboBoxDataSet = TheEmployeeCrewAssignmentClass.FindEmployeeCrewAssignmentComboBox(gdatStartDate, gdatEndDate);

                intNumberOfRecords = TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectCrew.Items.Add(TheFindEmployeeCrewAssignmentComboBoxDataSet.FindEmployeeCrewAssignmentComboBox[intCounter].CrewID);
                    }
                }

                cboSelectCrew.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Crew Assignments // Find Crews Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
            Visibility = Visibility.Hidden;
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
                    intColumns = TheCrewSummaryDataSet.crewsummary.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Crew Summary Report For Date Range Between " + Convert.ToString(gdatStartDate) + " And " + Convert.ToString(gdatEndDate)))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Hours Of  " + Convert.ToString(gdecTotalHours)))));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Crew ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Home Office"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Task"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Footage/Pieces"))));
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

                    intNumberOfRecords = TheCrewSummaryDataSet.crewsummary.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheCrewSummaryDataSet.crewsummary[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Crew Production Summary Report");
                    intCurrentRow = 0;
                    
                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Crew Assignment // Print Button " + Ex.Message);
            }
        }
        private void ResetForm()
        {
            txtEnterEndDate.Text = "";
            txtEnterStartDate.Text = "";
            txtTotalHours.Text = "";
            cboSelectCrew.Items.Clear();
            TheCrewSummaryDataSet.crewsummary.Rows.Clear();
           
        }
        private void mitExportToExcel_Click(object sender, RoutedEventArgs e)
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
                intRowNumberOfRecords = TheCrewSummaryDataSet.crewsummary.Rows.Count;
                intColumnNumberOfRecords = TheCrewSummaryDataSet.crewsummary.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCrewSummaryDataSet.crewsummary.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCrewSummaryDataSet.crewsummary.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Crew Summary // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
       
        private void cboSelectCrew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intTaskCounter;
            bool blnItemFound;
            string strProjectID;
            string strWorkTask;
            int intEmployeeID;
            int intProjectID;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            
            try
            {
                intSelectedIndex = cboSelectCrew.SelectedIndex;
                TheCrewSummaryDataSet.crewsummary.Rows.Clear();
                gdecTotalHours = 0;

                if(intSelectedIndex > 0)
                {
                    gstrCrewID = cboSelectCrew.SelectedItem.ToString();

                    TheFindEmployeeCrewByCrewIDDataSet = TheEmployeeCrewAssignmentClass.FindEmployeeCrewByCrewID(gstrCrewID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindEmployeeCrewByCrewIDDataSet.FindEmployeeCrewByCrewID.Rows.Count - 1;
                    gintTaskCounter = 0;
                    gintTaskUpperLimit = 0;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheFindEmployeeCrewByCrewIDDataSet.FindEmployeeCrewByCrewID[intCounter].EmployeeID;
                            intProjectID = TheFindEmployeeCrewByCrewIDDataSet.FindEmployeeCrewByCrewID[intCounter].ProjectID;

                            TheFindEmployeeProjectHourDateRangeSummaryDataSet = TheEmployeeProductivityStatsClass.FindEmployeeProjectHourDateRangeSummary(intEmployeeID, intProjectID, gdatStartDate, gdatEndDate);

                            gdecTotalHours += TheFindEmployeeProjectHourDateRangeSummaryDataSet.FindEmployeeProjectHourDateRangeSummary[0].TotalHours;

                            TheFindProjectTaskEmployeeStatsDataSet = TheEmployeeProductivityStatsClass.FindProjectTaskEmployeeStats(intEmployeeID, intProjectID, gdatStartDate, gdatEndDate);

                            intSecondNumberOfRecords = TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats.Rows.Count - 1;

                            for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                            {
                                blnItemFound = false;
                                strProjectID = TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats[intSecondCounter].AssignedProjectID;
                                strWorkTask = TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats[intSecondCounter].WorkTask;

                                if (gintTaskCounter > 0)
                                {
                                    for (intTaskCounter = 0; intTaskCounter <= gintTaskUpperLimit; intTaskCounter++)
                                    {
                                        if(strProjectID == TheCrewSummaryDataSet.crewsummary[intTaskCounter].AssignedProjectID)
                                        {
                                            if(strWorkTask == TheCrewSummaryDataSet.crewsummary[intTaskCounter].WorkTask)
                                            {
                                                blnItemFound = true;
                                            }
                                        }
                                    }
                                }

                                if(blnItemFound == false)
                                {
                                    CrewSummaryDataSet.crewsummaryRow NewCrewRow = TheCrewSummaryDataSet.crewsummary.NewcrewsummaryRow();

                                    NewCrewRow.AssignedProjectID = strProjectID;
                                    NewCrewRow.CrewID = gstrCrewID;
                                    NewCrewRow.FootagePieces = TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats[intSecondCounter].FootagePieces;
                                    NewCrewRow.HomeOffice = TheFindProjectTaskEmployeeStatsDataSet.FindProjectTaskEmployeeStats[intSecondCounter].HomeOffice;
                                    NewCrewRow.WorkTask = strWorkTask;

                                    TheCrewSummaryDataSet.crewsummary.Rows.Add(NewCrewRow);
                                    gintTaskUpperLimit = gintTaskCounter;
                                    gintTaskCounter++;
                                }
                            }

                            
                        }
                    }

                    dgrResults.ItemsSource = TheCrewSummaryDataSet.crewsummary;

                    txtTotalHours.Text = Convert.ToString(gdecTotalHours);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Employee Crew Summary // Select Crew Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.Message);
            }

        }

        private void mitResetForm_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtEnterStartDate.Focus();
        }


        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetForm();
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
