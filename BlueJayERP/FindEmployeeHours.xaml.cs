﻿/* Title:           Find Employee Hours
 * Date:            2-19-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the find employee hours */

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
using DataValidationDLL;
using DateSearchDLL;
using NewEmployeeDLL;
using System.IO;
using System.Configuration;
using Microsoft.Win32;
using System.Net;
using EmployeeProductivityStatsDLL;
using ProjectsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindEmployeeHours.xaml
    /// </summary>
    public partial class FindEmployeeHours : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        ProjectClass TheProjectClass = new ProjectClass();

        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeProjectTaskStatsDataSet TheFindEmployeeProjectTasksDataSet = new FindEmployeeProjectTaskStatsDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        EmployeeProjectTaskDataSet TheEmployeeProjectTaskDataSet = new EmployeeProjectTaskDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        decimal gdecTotalHours;

        public FindEmployeeHours()
        {
            InitializeComponent();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtEnterEndDate.Text = "";
            cboSelectEmployee.Items.Clear();
            txtEnterLastName.Text = "";
            txtEnterStartDate.Text = "";
            txtTotalHours.Text = "";
            

            this.Visibility = Visibility.Hidden;
        }

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            int intLength;
            string strLastName;
            int intCounter;
            int intNumberOfRecords;

            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;

            if(intLength > 2)
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employeed");
                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mitGenerateReport.IsEnabled = false;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                mitGenerateReport.IsEnabled = true;
            }
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strProjectID;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();
            gdecTotalHours = 0;
            TheEmployeeProjectTaskDataSet.projecttask.Rows.Clear();

            try
            {
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "An Employee Has Not Been Selected\n";
                }
                strValueForValidation = txtEnterStartDate.Text;
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

                blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                    return;
                }

                TheFindEmployeeProjectTasksDataSet = TheEmployeeProductivityStatsClass.FindEmployeeProjectTaskStats(MainWindow.gintEmployeeID, gdatStartDate, gdatEndDate);
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                intNumberOfRecords = TheFindEmployeeProjectTasksDataSet.FindEmployeeProjectTaskStats.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strProjectID = TheFindEmployeeProjectTasksDataSet.FindEmployeeProjectTaskStats[intCounter].AssignedProjectID;
                        TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                        EmployeeProjectTaskDataSet.projecttaskRow NewProjectTaskRow = TheEmployeeProjectTaskDataSet.projecttask.NewprojecttaskRow();

                        NewProjectTaskRow.AssignedProjectID = strProjectID;
                        NewProjectTaskRow.HomeOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice;
                        NewProjectTaskRow.ProjectName = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                        NewProjectTaskRow.TotalFootagePieces = TheFindEmployeeProjectTasksDataSet.FindEmployeeProjectTaskStats[intCounter].TotalFootage;
                        NewProjectTaskRow.TotalHours = TheFindEmployeeProjectTasksDataSet.FindEmployeeProjectTaskStats[intCounter].TaskTotalHours;
                        NewProjectTaskRow.WorkTask = TheFindEmployeeProjectTasksDataSet.FindEmployeeProjectTaskStats[intCounter].WorkTask;
                        gdecTotalHours += TheFindEmployeeProjectTasksDataSet.FindEmployeeProjectTaskStats[intCounter].TaskTotalHours;

                        TheEmployeeProjectTaskDataSet.projecttask.Rows.Add(NewProjectTaskRow);
                    }
                }


                dgrResults.ItemsSource = TheEmployeeProjectTaskDataSet.projecttask;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Generate Report Menu Item " + Ex.Message);

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
                    intColumns = TheEmployeeProjectTaskDataSet.projecttask.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Employee Labor Report for " + cboSelectEmployee.SelectedItem.ToString()))));    
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date Range " + Convert.ToString(gdatStartDate) + " Thru " + Convert.ToString(gdatEndDate)))));
                    newTableRow.Cells[0].FontSize = 18;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Name"))));
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

                    intNumberOfRecords = TheEmployeeProjectTaskDataSet.projecttask.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheEmployeeProjectTaskDataSet.projecttask[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Employee Labor Report");
                    intCurrentRow = 0;

                } 
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Print Button " + Ex.Message);
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
                intRowNumberOfRecords = TheEmployeeProjectTaskDataSet.projecttask.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeProjectTaskDataSet.projecttask.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProjectTaskDataSet.projecttask.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeProjectTaskDataSet.projecttask.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
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
