/* Title:           Vehicle Roster
 * Date:            4-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the vehicle roster*/

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
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehicleRoster.xaml
    /// </summary>
    public partial class VehicleRoster : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        ActiveVehicleDataSet TheActiveVehicleDataSet = new ActiveVehicleDataSet();

        public VehicleRoster()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();

                intNumberOfRecords = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    ActiveVehicleDataSet.activevehiclesRow NewVehicleRow = TheActiveVehicleDataSet.activevehicles.NewactivevehiclesRow();

                    NewVehicleRow.AssignedOffice = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].AssignedOffice;
                    NewVehicleRow.DOTStatus = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].DOTStatus;
                    NewVehicleRow.FirstName = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].FirstName;
                    NewVehicleRow.IMEI = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].IMEI;
                    NewVehicleRow.LastName = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].LastName;
                    NewVehicleRow.LicensePlate = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].LicensePlate;
                    NewVehicleRow.VehicleMake = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleMake;
                    NewVehicleRow.VehicleModel = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleModel;
                    NewVehicleRow.VehicleNumber = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleNumber;
                    NewVehicleRow.VehicleYear = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleYear;

                    TheActiveVehicleDataSet.activevehicles.Rows.Add(NewVehicleRow);
                }

                dgrResults.ItemsSource = TheActiveVehicleDataSet.activevehicles;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Roster // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("file://bjc/shares/Documents/WAREHOUSE/WhseTrac%20Manual/index.html");
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bluejay.on.spiceworks.com/portal/tickets");
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
                    intColumns = TheActiveVehicleDataSet.activevehicles.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Roster"))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Year"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Make"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Model"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Licnse Plate"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("DOT Status"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("IMEI"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Assigned Office"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 12;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheActiveVehicleDataSet.activevehicles.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheActiveVehicleDataSet.activevehicles[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 10;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProblemReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProblemReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProblemReport.PrintableAreaWidth;
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Vehicle Roster");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Roster // Print Menu Item " + Ex.Message);
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

                worksheet.Name = "ExportedFromDatGrid";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheActiveVehicleDataSet.activevehicles.Rows.Count - 1;
                intColumnNumberOfRecords = TheActiveVehicleDataSet.activevehicles.Columns.Count - 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter <= intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter <= intColumnNumberOfRecords; intColumnCounter++)
                    {
                        // Excel index starts from 1,1. As first Row would have the Column headers, adding a condition check. 
                        if (cellRowIndex == 1)
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = TheActiveVehicleDataSet.activevehicles.Columns[intColumnCounter].ColumnName;
                        }
                        else
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = TheActiveVehicleDataSet.activevehicles.Rows[intRowCounter][intColumnCounter].ToString();
                        }
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Roster // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
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
