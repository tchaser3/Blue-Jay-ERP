/* Ttile:           Find Multiple Employee Production
 * Date:            3-9-18
 * Author:          Terry Holmes
 * 
 * Description:     This form will find production for multiple employees */

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
using NewEmployeeDLL;
using DataValidationDLL;
using Microsoft.Win32;
using DateSearchDLL;
using EmployeeProductivityStatsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindMultipleEmployeeProduction.xaml
    /// </summary>
    public partial class FindMultipleEmployeeProduction : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();

        //setting up the data;
        FindEmployeeTaskStatsDataSet TheFindEmployeeTaskStatsDataSet;
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDataSet = new FindEmployeeByEmployeeIDDataSet();
        MultipleEmployeeDataSet TheMultipleEmployeeDataSet = new MultipleEmployeeDataSet();
        EmployeesSummaryDataSet TheEmployeeSummaryDataSet = new EmployeesSummaryDataSet();

        //setting up global variables
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        decimal gdecTotalHours;

        public FindMultipleEmployeeProduction()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetWindow();
            Visibility = Visibility.Hidden;
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            int intTaskCounter;
            int intTaskNumberOfRecords;
            int intEmployeeID;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intRecordsReturned = TheMultipleEmployeeDataSet.employees.Rows.Count;
                TheEmployeeSummaryDataSet.employeetasks.Rows.Clear();
                gdecTotalHours = 0;

                if(intRecordsReturned < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employees have not been Selected\n";
                }
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                    gdatEndDate = TheDataSearchClass.AddingDays(gdatEndDate, 1);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                    return;
                }

                //first loop
                intNumberOfRecords = TheMultipleEmployeeDataSet.employees.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheMultipleEmployeeDataSet.employees[intCounter].EmployeeID;

                    TheFindEmployeeTaskStatsDataSet = TheEmployeeProductivityStatsClass.FindEmployeeTaskStats(intEmployeeID, gdatStartDate, gdatEndDate);

                    TheFindEmployeeByEmployeeIDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                    intTaskNumberOfRecords = TheFindEmployeeTaskStatsDataSet.FindEmployeeTaskStats.Rows.Count - 1;

                    for(intTaskCounter = 0; intTaskCounter <= intTaskNumberOfRecords; intTaskCounter++)
                    {
                        EmployeesSummaryDataSet.employeetasksRow NewTaskRow = TheEmployeeSummaryDataSet.employeetasks.NewemployeetasksRow();

                        NewTaskRow.EmployeeID = intEmployeeID;
                        NewTaskRow.FirstName = TheFindEmployeeByEmployeeIDataSet.FindEmployeeByEmployeeID[0].FirstName;
                        NewTaskRow.HomeOffice = TheFindEmployeeByEmployeeIDataSet.FindEmployeeByEmployeeID[0].HomeOffice;
                        NewTaskRow.LastName = TheFindEmployeeByEmployeeIDataSet.FindEmployeeByEmployeeID[0].LastName;
                        NewTaskRow.TotalFootagePieces = TheFindEmployeeTaskStatsDataSet.FindEmployeeTaskStats[intTaskCounter].TotalFootage;
                        NewTaskRow.TotalHours = TheFindEmployeeTaskStatsDataSet.FindEmployeeTaskStats[intTaskCounter].TaskTotalHours;
                        NewTaskRow.WorkTask = TheFindEmployeeTaskStatsDataSet.FindEmployeeTaskStats[intTaskCounter].WorkTask;

                        gdecTotalHours += TheFindEmployeeTaskStatsDataSet.FindEmployeeTaskStats[intTaskCounter].TaskTotalHours;

                        TheEmployeeSummaryDataSet.employeetasks.Rows.Add(NewTaskRow);
                    }
                }

                dgrResults.ItemsSource = TheEmployeeSummaryDataSet.employeetasks;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Multiple Employee Production // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
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

                    pdProblemReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheEmployeeSummaryDataSet.employeetasks.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Multiple Employee Production Report Between " + Convert.ToString(gdatStartDate) + " And " + Convert.ToString(gdatEndDate)))));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Employee ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Home Office"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Task"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Footage/Pieces"))));
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

                    intNumberOfRecords = TheEmployeeSummaryDataSet.employeetasks.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheEmployeeSummaryDataSet.employeetasks[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Multiple Employee Production Report");
                    intCurrentRow = 0;

                }
                
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Multiple Employee Hours // Print Button " + Ex.Message);
            }
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
                intRowNumberOfRecords = TheEmployeeSummaryDataSet.employeetasks.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeSummaryDataSet.employeetasks.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeSummaryDataSet.employeetasks.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeSummaryDataSet.employeetasks.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Multiple Employee Hours // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
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
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);

                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            
        }


        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MultipleEmployeeDataSet.employeesRow NewEmployeeRow = TheMultipleEmployeeDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.EmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    NewEmployeeRow.FirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    NewEmployeeRow.LastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;

                    TheMultipleEmployeeDataSet.employees.Rows.Add(NewEmployeeRow);

                    dgrResults.ItemsSource = TheMultipleEmployeeDataSet.employees;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Multiple Employee Production // Combo Box Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitResetWindow_Click(object sender, RoutedEventArgs e)
        {
            ResetWindow();
        }
        public void ResetWindow()
        {
            txtEndDate.Text = "";
            txtStartDate.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            TheMultipleEmployeeDataSet.employees.Rows.Clear();
            dgrResults.ItemsSource = TheMultipleEmployeeDataSet.employees;
            txtTotalHours.Text = "";
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
            ResetWindow();
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
