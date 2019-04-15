/* Title:           Productivity Data Entry Reports
 * Date:            6-4-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to track the productivity data entry */

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
using ProjectsDLL;
using ProductivityDataEntryDLL;
using DataValidationDLL;
using DateSearchDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ProductivityDataEntryReports.xaml
    /// </summary>
    public partial class ProductivityDataEntryReports : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProductivityDataEntryClass TheProductivityDataEntryClass = new ProductivityDataEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        ComboEmployeeDataSet TheComboBoxEmployeeDataSet = new ComboEmployeeDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProductivityDataEntryByDateRangeDataSet TheFindProductivityDataEntryByDateRangeDataSet = new FindProductivityDataEntryByDateRangeDataSet();
        FindProductivityDataEntryByEmployeeIDDataSet TheFindProductivityDataEntryByEmployeeIDDataSet = new FindProductivityDataEntryByEmployeeIDDataSet();
        FindProductivityDataEntryByProjectIDDataSet TheFindProductivtyDataEntryByProjectIDdataSet = new FindProductivityDataEntryByProjectIDDataSet();
        ProductivityDataEntryDataSet TheProductivityDataEntryDataSet = new ProductivityDataEntryDataSet();

        string gstrReportType;
        int gintEmployeeID;

        public ProductivityDataEntryReports()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the report combo box
            cboReportType.Items.Add("Select Report Type");
            cboReportType.Items.Add("Date Range Report");
            cboReportType.Items.Add("Employee Report");
            cboReportType.Items.Add("Project Report");

            cboReportType.SelectedIndex = 0;

            ResetControls();
        }
        private void ResetControls()
        {
            TheProductivityDataEntryDataSet.dataentry.Rows.Clear();
            dgrResults.ItemsSource = TheProductivityDataEntryDataSet.dataentry;
            lblDataEntered.Visibility = Visibility.Hidden;
            lblEndDate.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            lblStartDate.Visibility = Visibility.Hidden;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            cboSelectEmployee.Items.Clear();
            txtEndDate.Text = "";
            txtEndDate.Visibility = Visibility.Hidden;
            txtEnteredData.Text = "";
            txtEnteredData.Visibility = Visibility.Hidden;
            txtStartDate.Text = "";
            txtStartDate.Visibility = Visibility.Hidden;
        }
        private void DateControlsVisible()
        {
            txtEndDate.Visibility = Visibility.Visible;
            lblEndDate.Visibility = Visibility.Visible;
            txtStartDate.Visibility = Visibility.Visible;
            lblStartDate.Visibility = Visibility.Visible;
        }

        private void txtEnteredData_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up the variables
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                strLastName = txtEnteredData.Text;

                intLength = strLastName.Length;

                if ((intLength > 2) && (gstrReportType == "EMPLOYEE"))
                {
                    TheComboBoxEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboBoxEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboBoxEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "blue Jay ERP // Productivity Data Entry Reports // Enter Data Text Changed Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboReportType.SelectedIndex;


            ResetControls();

            if (intSelectedIndex == 1)
            {
                gstrReportType = "DATE RANGE";
                DateControlsVisible();
            }
            else if (intSelectedIndex == 2)
            {
                gstrReportType = "EMPLOYEE";
                DateControlsVisible();
                lblDataEntered.Visibility = Visibility.Visible;
                lblDataEntered.Content = "Enter Last Name";
                txtEnteredData.Visibility = Visibility;
                lblSelectEmployee.Visibility = Visibility.Visible;
                cboSelectEmployee.Visibility = Visibility.Visible;
            }
            else if (intSelectedIndex == 3)
            {
                gstrReportType = "PROJECT";
                lblDataEntered.Visibility = Visibility.Visible;
                lblDataEntered.Content = "Enter Project ID";
                txtEnteredData.Visibility = Visibility;
            }
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsaProblem = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            string strProjectID;
            string strErrorMessage = "";
            string strProjectName;
            int intProjectID;

            try
            {
                TheProductivityDataEntryDataSet.dataentry.Rows.Clear();

                if((gstrReportType == "DATE RANGE") || (gstrReportType == "EMPLOYEE"))
                {
                    strValueForValidation = txtStartDate.Text;
                    blnThereIsaProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if(blnThereIsaProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Start Date is not a Date\n";
                    }
                    else
                    {
                        datStartDate = Convert.ToDateTime(strValueForValidation);
                        datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                    }
                    strValueForValidation = txtEndDate.Text;
                    blnThereIsaProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnThereIsaProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The End Date is not a Date\n";
                    }
                    else
                    {
                        datEndDate = Convert.ToDateTime(strValueForValidation);
                        datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
                        datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
                    }
                    if(gstrReportType == "EMPLOYEE")
                    {
                        if(cboSelectEmployee.SelectedIndex < 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Employee Was Not Selected\n";
                        }
                    }
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }
                    else
                    {
                        blnFatalError = TheDataValidationClass.verifyDateRange(datStartDate, datEndDate);
                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The Start Date is After the End Date");
                            return;
                        }
                    }

                    if(gstrReportType == "DATE RANGE")
                    {
                        TheFindProductivityDataEntryByDateRangeDataSet = TheProductivityDataEntryClass.FindProductivityDataEntbyDateRange(datStartDate, datEndDate);

                        intNumberOfRecords = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange.Rows.Count - 1;

                        if(intNumberOfRecords < 0)
                        {
                            TheMessagesClass.InformationMessage("No Records Found");
                            return;
                        }
                        else
                        {
                            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                ProductivityDataEntryDataSet.dataentryRow NewTransactionRow = TheProductivityDataEntryDataSet.dataentry.NewdataentryRow();

                                NewTransactionRow.EntryDate = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].EntryDate;
                                NewTransactionRow.FirstName = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].FirstName;
                                NewTransactionRow.LastName = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].LastName;
                                NewTransactionRow.ProjectID = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].AssignedProjectID;
                                NewTransactionRow.ProjectName = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].ProjectName;
                                NewTransactionRow.TransactionID = TheFindProductivityDataEntryByDateRangeDataSet.FindProductivityDataEntryByDateRange[intCounter].TransactionID;

                                TheProductivityDataEntryDataSet.dataentry.Rows.Add(NewTransactionRow);
                            }
                        }
                    }
                    else if(gstrReportType == "EMPLOYEE")
                    {
                        TheFindProductivityDataEntryByEmployeeIDDataSet = TheProductivityDataEntryClass.FindProductivityDataEntryByEmployeeID(gintEmployeeID, datStartDate, datEndDate);

                        intNumberOfRecords = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID.Rows.Count - 1;

                        if (intNumberOfRecords < 0)
                        {
                            TheMessagesClass.InformationMessage("No Records Found");
                            return;
                        }
                        else
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                ProductivityDataEntryDataSet.dataentryRow NewTransactionRow = TheProductivityDataEntryDataSet.dataentry.NewdataentryRow();

                                NewTransactionRow.EntryDate = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].EntryDate;
                                NewTransactionRow.FirstName = TheComboBoxEmployeeDataSet.employees[cboSelectEmployee.SelectedIndex - 1].FirstName;
                                NewTransactionRow.LastName = TheComboBoxEmployeeDataSet.employees[cboSelectEmployee.SelectedIndex - 1].LastName;
                                NewTransactionRow.ProjectID = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].AssignedProjectID;
                                NewTransactionRow.ProjectName = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].ProjectName;
                                NewTransactionRow.TransactionID = TheFindProductivityDataEntryByEmployeeIDDataSet.FindProductivityDataEntryByEmployeeID[intCounter].TransactionID;

                                TheProductivityDataEntryDataSet.dataentry.Rows.Add(NewTransactionRow);
                            }
                        }
                    }
                }
                else if (gstrReportType == "PROJECT")
                {
                    strProjectID = txtEnteredData.Text;
                    if(strProjectID == "")
                    {
                        TheMessagesClass.ErrorMessage("The Project Was Not Entered");
                        return;
                    }

                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProjectID);

                    intNumberOfRecords = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("The Project Was Not Found");
                        return;
                    }

                    strProjectName = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectName;
                    intProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                    TheFindProductivtyDataEntryByProjectIDdataSet = TheProductivityDataEntryClass.FindProductivityDataEntryByProjectID(intProjectID);

                    intNumberOfRecords = TheFindProductivtyDataEntryByProjectIDdataSet.FindProductivityDataEntryByProjectID.Rows.Count - 1;

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.InformationMessage("No Records Found");
                        return;
                    }
                    else
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            ProductivityDataEntryDataSet.dataentryRow NewTransactionRow = TheProductivityDataEntryDataSet.dataentry.NewdataentryRow();

                            NewTransactionRow.EntryDate = TheFindProductivtyDataEntryByProjectIDdataSet.FindProductivityDataEntryByProjectID[intCounter].EntryDate;
                            NewTransactionRow.FirstName = TheFindProductivtyDataEntryByProjectIDdataSet.FindProductivityDataEntryByProjectID[intCounter].FirstName;
                            NewTransactionRow.LastName = TheFindProductivtyDataEntryByProjectIDdataSet.FindProductivityDataEntryByProjectID[intCounter].LastName;
                            NewTransactionRow.ProjectID = strProjectID;
                            NewTransactionRow.ProjectName = strProjectName;
                            NewTransactionRow.TransactionID = TheFindProductivtyDataEntryByProjectIDdataSet.FindProductivityDataEntryByProjectID[intCounter].TransactionID;

                            TheProductivityDataEntryDataSet.dataentry.Rows.Add(NewTransactionRow);
                        }
                    }
                }

                dgrResults.ItemsSource = TheProductivityDataEntryDataSet.dataentry;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Productivity Date Entry Report // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboBoxEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
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
                intRowNumberOfRecords = TheProductivityDataEntryDataSet.dataentry.Rows.Count;
                intColumnNumberOfRecords = TheProductivityDataEntryDataSet.dataentry.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProductivityDataEntryDataSet.dataentry.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProductivityDataEntryDataSet.dataentry.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Weekly Inspection Report // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
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
                    intColumns = TheProductivityDataEntryDataSet.dataentry.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Productivity Data Entry Report"))));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    newTableRow.Cells[0].ColumnSpan = 1;
                    newTableRow.Cells[1].ColumnSpan = 1;
                    newTableRow.Cells[2].ColumnSpan = 1;
                    newTableRow.Cells[3].ColumnSpan = 1;
                    newTableRow.Cells[4].ColumnSpan = 1;
                    newTableRow.Cells[5].ColumnSpan = 1;
                    

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheProductivityDataEntryDataSet.dataentry.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheProductivityDataEntryDataSet.dataentry[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                            if (intColumnCounter == 5)
                            {
                                newTableRow.Cells[intColumnCounter].ColumnSpan = 2;
                            }

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProblemReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProblemReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProblemReport.PrintableAreaWidth;
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Productivity Date Entry Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Productivity Data Entry Reports // Print Button " + Ex.Message);
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
