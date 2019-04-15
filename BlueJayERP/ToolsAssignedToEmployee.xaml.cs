/* Title:           Tools Assigned To Employee
 * Date:            3-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This will show tools that an employee has signed out.*/

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
using NewToolsDLL;
using Microsoft.Win32;
using BulkToolsDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ToolsAssignedToEmployee.xaml
    /// </summary>
    public partial class ToolsAssignedToEmployee : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolsClass = new ToolsClass();
        BulkToolsClass TheBulkToolsClass = new BulkToolsClass();

        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();
        FindToolsByEmployeeIDDataSet TheFindToolsByEmployeeIDDataSet = new FindToolsByEmployeeIDDataSet();
        EmployeeToolsDataSet TheEmployeeToolsDataSet = new EmployeeToolsDataSet();
        FindBulkToolsCurrentlyAssignedToEmployeeDataSet TheBulkToolsCurrentlyAssignedToEmployeeDataSet = new FindBulkToolsCurrentlyAssignedToEmployeeDataSet();

        public ToolsAssignedToEmployee()
        {
            InitializeComponent();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            TheEmployeeToolsDataSet.employeetools.Rows.Clear();
            dgrTools.ItemsSource = TheEmployeeToolsDataSet.employeetools;
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

                    pdProblemReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheEmployeeToolsDataSet.employeetools.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Signed Out Tools By " + MainWindow.gstrFirstName + " " + MainWindow.gstrLastName))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    
                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Description"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Category"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Notes"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Quantity"))));
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

                    intNumberOfRecords = TheEmployeeToolsDataSet.employeetools.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheEmployeeToolsDataSet.employeetools[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Tools Signed Out");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tools Assinged To Employee // Print Menu Item " + Ex.Message);
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
                intRowNumberOfRecords = TheEmployeeToolsDataSet.employeetools.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeToolsDataSet.employeetools.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeToolsDataSet.employeetools.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeToolsDataSet.employeetools.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tools Assigned To Employee // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            //getting the last name
            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;

            if(intLength > 2)
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                intNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].FirstName + " " + TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].LastName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    TheEmployeeToolsDataSet.employeetools.Rows.Clear();

                    MainWindow.gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].EmployeeID;
                    MainWindow.gstrFirstName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].FirstName;
                    MainWindow.gstrLastName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].LastName;

                    TheFindToolsByEmployeeIDDataSet = TheToolsClass.FindToolsByEmployeeID(MainWindow.gintEmployeeID);

                    intNumberOfRecords = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            EmployeeToolsDataSet.employeetoolsRow NewToolRow = TheEmployeeToolsDataSet.employeetools.NewemployeetoolsRow();

                            NewToolRow.ToolID = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolID;
                            NewToolRow.ToolCategory = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolCategory;
                            NewToolRow.ToolDescription = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolDescription;
                            NewToolRow.ToolNotes = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolNotes;
                            NewToolRow.TransactionDate = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].TransactionDate;
                            NewToolRow.Quantity = 1;

                            TheEmployeeToolsDataSet.employeetools.Rows.Add(NewToolRow);
                        }
                    }

                    TheBulkToolsCurrentlyAssignedToEmployeeDataSet = TheBulkToolsClass.FindBulkToolsCurrentlyAssignedToEmployee(MainWindow.gintEmployeeID);

                    intNumberOfRecords = TheBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            EmployeeToolsDataSet.employeetoolsRow NewToolRow = TheEmployeeToolsDataSet.employeetools.NewemployeetoolsRow();

                            NewToolRow.ToolID = "BULK TOOLS";
                            NewToolRow.ToolCategory = TheBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee[intCounter].ToolCategory;
                            NewToolRow.ToolDescription = "BULK TOOL";
                            NewToolRow.ToolNotes = TheBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee[intCounter].IssueNotes;
                            NewToolRow.TransactionDate = TheBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee[intCounter].SignOutDate;
                            NewToolRow.Quantity = TheBulkToolsCurrentlyAssignedToEmployeeDataSet.FindBulkToolsCurrentlyAssignedToEmployee[intCounter].Quantity;

                            TheEmployeeToolsDataSet.employeetools.Rows.Add(NewToolRow);
                        }
                        
                    }

                    dgrTools.ItemsSource = TheEmployeeToolsDataSet.employeetools;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tools Assigned To Employees // Combo Box Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheEmployeeToolsDataSet.employeetools.Rows.Clear();
            dgrTools.ItemsSource = TheEmployeeToolsDataSet.employeetools;
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
