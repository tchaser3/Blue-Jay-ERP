/* Title:           Project Material
 * Date:            4-23-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the project material report */

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
using ProjectsDLL;
using InventoryDLL;
using IssuedPartsDLL;
using DataValidationDLL;
using NewEmployeeDLL;
using Microsoft.Win32;
using NewPartNumbersDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ProjectMaterial.xaml
    /// </summary>
    public partial class ProjectMaterial : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        IssuedPartsClass TheIssuePartsClass = new IssuedPartsClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();

        //setting up the data sets
        ProjectMaterialReportDataSet TheProjectMaterialReportDataSet = new ProjectMaterialReportDataSet();
        FindProjectByProjectNameDataSet TheFindProjectByProjectNameDataSet = new FindProjectByProjectNameDataSet();
        FindIssuedPartsByProjectIDDataSet TheFindIssuedPartsByProjectIDDataSet = new FindIssuedPartsByProjectIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();

        //setting global variables
        int gintReportCounter;
        int gintReportUpperLimit;
        decimal gdecTotalCost;

        public ProjectMaterial()
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

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitPrint_Click(object sender, RoutedEventArgs e)
        {
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                PrintDialog pdCancelledReport = new PrintDialog();

                if (pdCancelledReport.ShowDialog().Value)
                {
                    FlowDocument fdCancelledLines = new FlowDocument();
                    Thickness thickness = new Thickness(100, 50, 50, 50);
                    fdCancelledLines.PagePadding = thickness;

                    //Set Up Table Columns
                    Table cancelledTable = new Table();
                    fdCancelledLines.Blocks.Add(cancelledTable);
                    cancelledTable.CellSpacing = 0;
                    intColumns = TheProjectMaterialReportDataSet.projectmaterial.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        cancelledTable.Columns.Add(new TableColumn());
                    }
                    cancelledTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Material Report"))));
                    newTableRow.Cells[0].FontSize = 20;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Cost $" + txtProjectCost.Text))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Part ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Part Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("JDE Part Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Description"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Issued"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Warehouse"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Cost"))));

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheProjectMaterialReportDataSet.projectmaterial.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheProjectMaterialReportDataSet.projectmaterial[intReportRowCounter][intColumnCounter].ToString()))));


                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                        }
                    }

                    //Set up page and print
                    fdCancelledLines.ColumnWidth = pdCancelledReport.PrintableAreaWidth;
                    fdCancelledLines.PageHeight = pdCancelledReport.PrintableAreaHeight;
                    fdCancelledLines.PageWidth = pdCancelledReport.PrintableAreaWidth;
                    pdCancelledReport.PrintDocument(((IDocumentPaginatorSource)fdCancelledLines).DocumentPaginator, "Project Material Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Project Material // Print Menu Item " + Ex.Message);
            }

            PleaseWait.Close();
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
                intRowNumberOfRecords = TheProjectMaterialReportDataSet.projectmaterial.Rows.Count;
                intColumnNumberOfRecords = TheProjectMaterialReportDataSet.projectmaterial.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] =TheProjectMaterialReportDataSet.projectmaterial.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectMaterialReportDataSet.projectmaterial.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Project Material // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMaterialDataSet();
        }
        private void LoadMaterialDataSet()
        {
            //setting local variables
            //int intRecordsReturned;
            //bool blnItemNotFound = true;
            int intCounter;
            int intNumberOfRecords;
            int intReportCounter;
            bool blnItemFound;
            int intPartID;
            int intQuantity;
            int intWarehouseID;
            decimal decCost;
            decimal decTotalCost;

            try
            {
                TheProjectMaterialReportDataSet.projectmaterial.Rows.Clear();

                gintReportCounter = 0;
                gintReportUpperLimit = 0;

                TheFindIssuedPartsByProjectIDDataSet = TheIssuePartsClass.FindIssuedPartsByProjectID(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        blnItemFound = false;
                        intPartID = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].PartID;
                        intQuantity = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].Quantity;

                        if (gintReportCounter > 0)
                        {
                            for (intReportCounter = 0; intReportCounter <= gintReportUpperLimit; intReportCounter++)
                            {
                                if (intPartID == TheProjectMaterialReportDataSet.projectmaterial[intReportCounter].PartID)
                                {
                                    TheProjectMaterialReportDataSet.projectmaterial[intReportCounter].Issued += intQuantity;
                                    blnItemFound = true;
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            ProjectMaterialReportDataSet.projectmaterialRow NewPartRow = TheProjectMaterialReportDataSet.projectmaterial.NewprojectmaterialRow();

                            NewPartRow.PartDescription = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].PartDescription;
                            NewPartRow.Issued = intQuantity;
                            NewPartRow.JDEPartNumber = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].JDEPartNumber;
                            NewPartRow.PartID = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].PartID;
                            NewPartRow.PartNumber = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].PartNumber;
                            NewPartRow.Cost = 0;
                            intWarehouseID = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].WarehouseID;
                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);
                            NewPartRow.Warehouse = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;

                            TheProjectMaterialReportDataSet.projectmaterial.Rows.Add(NewPartRow);
                            gintReportUpperLimit = gintReportCounter;
                            gintReportCounter++;
                        }
                    }
                }

                gdecTotalCost = 0;
                intNumberOfRecords = TheProjectMaterialReportDataSet.projectmaterial.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intQuantity = TheProjectMaterialReportDataSet.projectmaterial[intCounter].Issued;
                    intPartID = TheProjectMaterialReportDataSet.projectmaterial[intCounter].PartID;

                    TheFindPartByPartIDDataSet = ThePartNumberClass.FindPartByPartID(intPartID);

                    decCost = Convert.ToDecimal(TheFindPartByPartIDDataSet.FindPartByPartID[0].Price);

                    decTotalCost = intQuantity * decCost;

                    decTotalCost = Math.Round(decTotalCost, 2);

                    TheProjectMaterialReportDataSet.projectmaterial[intCounter].Cost = decTotalCost;
                    gdecTotalCost += decTotalCost;
                }

                txtProjectCost.Text = Convert.ToString(gdecTotalCost);
                dgrResults.ItemsSource = TheProjectMaterialReportDataSet.projectmaterial;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Inventory Reports // Project Report // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadMaterialDataSet();
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
