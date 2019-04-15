/* Title:           Cycle Counts
 * Date:            3-19-18
 * Author:          Terry Holmes
 * 
 * Description:     Users can print tickets and also change inventory counts */

using DataValidationDLL;
using InventoryDLL;
using KeyWordDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using NewPartNumbersDLL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for CycleCounts.xaml
    /// </summary>
    public partial class CycleCounts : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        Random myRandomNumber = new Random();

        //setting up the data
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();

        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();
        FindWarehouseInventoryDataSet TheFindWarehouseInventoryDataSet = new FindWarehouseInventoryDataSet();
        CycleCountTicketsDataSet TheCycleCountTicketsDataSet = new CycleCountTicketsDataSet();

        int[] gintNumberUsed = new int[11];
        int gintArrayCounter;
        int gintTransactionID;

        public CycleCounts()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetControls();
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
                MainWindow.TheFindPartsWarehousesDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses.Rows.Count - 1;

                cboSelectWarehouse.Items.Add("Select Warehouse");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
                mitPrintCycleCountTickets.IsEnabled = false;
                btnSearch.IsEnabled = false;
                btnUpdate.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Cycle Counts // Wndows Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strWarehouseName;
            bool blnKeyWordNotFound;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                mitPrintCycleCountTickets.IsEnabled = true;
                btnSearch.IsEnabled = true;

                strWarehouseName = cboSelectWarehouse.SelectedItem.ToString();
                blnKeyWordNotFound = TheKeyWordClass.FindKeyWord("JH", strWarehouseName);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strPartNumber;
            bool blnIsNotInteger = false;
            int intRecordsReturned;
            bool blnItemFound = false;


            strPartNumber = txtEnterPartNumber.Text;
            if(strPartNumber == "")
            {
                TheMessagesClass.ErrorMessage("Part Number Was Not Added");
                return;
            }

            blnIsNotInteger = TheDataValidationClass.VerifyIntegerData(strPartNumber);
            if(blnIsNotInteger == false)
            {
                MainWindow.gintPartID = Convert.ToInt32(strPartNumber);

                TheFindPartByPartIDDataSet = ThePartNumberClass.FindPartByPartID(MainWindow.gintPartID);

                intRecordsReturned = TheFindPartByPartIDDataSet.FindPartByPartID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    blnItemFound = true;
                }
            }
            if(blnItemFound == false)
            {
                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    blnItemFound = true;
                    MainWindow.gintPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                }

                if(blnItemFound == false)
                {
                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnItemFound = true;
                        MainWindow.gintPartID = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID;
                    }
                }
            }

            if(blnItemFound == false)
            {
                TheMessagesClass.ErrorMessage("Part Information Does Not Exist");
                return;
            }

            blnItemFound = false;

            TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(MainWindow.gintPartID, MainWindow.gintWarehouseID);

            intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("No Information for this Part, You Cannoot Change The Count");
                return;
            }
            else
            {
                txtCurrentCount.Text = Convert.ToString(TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity);
                gintTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;
            }
        

            btnUpdate.IsEnabled = true;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            string strValueForValidation;
            int intQuantity;

            try
            {
                strValueForValidation = txtNewCount.Text;
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("New Count is not an Integer");
                    return;
                }

                intQuantity = Convert.ToInt32(strValueForValidation);

                
                blnFatalError = TheInventoryClass.UpdateInventoryPart(gintTransactionID, intQuantity);

                if (blnFatalError == true)
                    throw new Exception();
                

                TheMessagesClass.InformationMessage("The Part Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Cycle Count // Update Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void ResetControls()
        {
            txtCurrentCount.Text = "";
            txtEnterPartNumber.Text = "";
            txtNewCount.Text = "";
            cboSelectWarehouse.SelectedIndex = 0;
            mitPrintCycleCountTickets.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnSearch.IsEnabled = false;
        }

        private void mitPrintCycleCountTickets_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intRandomNumber = 0;
            int intNumberOfRecords;
            int intArrayCounter;
            bool blnDublicateNumber;
            bool blnItemFound;

            try
            {
                for (intArrayCounter = 0; intArrayCounter <= 10; intArrayCounter++)
                {
                    gintNumberUsed[intArrayCounter] = -1;
                }

                gintArrayCounter = 0;

                TheCycleCountTicketsDataSet.tickets.Rows.Clear();

                
                TheFindWarehouseInventoryDataSet = TheInventoryClass.FindWarehouseInventory(MainWindow.gintWarehouseID);

                intNumberOfRecords = TheFindWarehouseInventoryDataSet.FindWarehouseInventory.Rows.Count;

                for (intCounter = 0; intCounter < 10; intCounter++)
                {
                    blnDublicateNumber = true;

                    while (blnDublicateNumber == true)
                    {
                        blnItemFound = false;

                        intRandomNumber = myRandomNumber.Next(0, intNumberOfRecords);

                        for (intArrayCounter = 0; intArrayCounter <= 10; intArrayCounter++)
                        {
                            if (intRandomNumber == gintNumberUsed[intArrayCounter])
                            {
                                blnItemFound = true;
                            }
                        }

                        if (blnItemFound == false)
                        {
                                blnDublicateNumber = false;
                        }
                    }

                    gintNumberUsed[gintArrayCounter] = intRandomNumber;
                    gintArrayCounter++;

                    CycleCountTicketsDataSet.ticketsRow NewPartRow = TheCycleCountTicketsDataSet.tickets.NewticketsRow();

                    NewPartRow.Description = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intRandomNumber].PartDescription;
                    NewPartRow.JDEPartNumber = "";

                    TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intRandomNumber].PartNumber);

                    NewPartRow.PartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                    NewPartRow.PartNumber = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intRandomNumber].PartNumber;
                    NewPartRow.Quantity = "";

                    TheCycleCountTicketsDataSet.tickets.Rows.Add(NewPartRow);

                }
                

                PrintTickets();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Cycle Count // Print Cycle Count Tickets " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void PrintTickets()
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
                    intColumns = TheCycleCountTicketsDataSet.tickets.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Cycle Count Ticket For Warehouse " + cboSelectWarehouse.SelectedItem.ToString()))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);


                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Part ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Part Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("JDE Part Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Description"))));
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

                    intNumberOfRecords = TheCycleCountTicketsDataSet.tickets.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheCycleCountTicketsDataSet.tickets[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Cycle Count Ticket");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Cycle Count // Print Tickets " + Ex.Message);
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
