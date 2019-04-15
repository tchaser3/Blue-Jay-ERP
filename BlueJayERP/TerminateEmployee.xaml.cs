/* Title:           Terminate Employee
 * Date:            3-15-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to print off all assets assigned to an employee and deactivate their ID */

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
using VehicleMainDLL;
using NewToolsDLL;
using VehicleAssignmentDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for TerminateEmployee.xaml
    /// </summary>
    public partial class TerminateEmployee : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        ToolsClass TheToolsClass = new ToolsClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();

        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();
        FindToolsByEmployeeIDDataSet TheFindToolsByEmployeeIDDataSet = new FindToolsByEmployeeIDDataSet();
        FindVehicleAssignedByEmployeeIDDataSet TheFindVehicleAssigedByEmployeeIDDataSet = new FindVehicleAssignedByEmployeeIDDataSet();
        EmployeeToolsDataSet TheEmployeeToolsDataSet = new EmployeeToolsDataSet();
        VehicleToolsDataSet TheVehicleToolsDataSet = new VehicleToolsDataSet();

        int gintVehicleEmployeeID;

        PrintDialog pdProblemReport;

        public TerminateEmployee()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
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

                TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                intNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count - 1;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Is Not Active or Does Not Exist");
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
            int intSecondNumberOfRecords;
            int intSecondCounter;
            int intBJCNumber;
            int intToolCounter;
            int intToolNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    TheEmployeeToolsDataSet.employeetools.Rows.Clear();
                    TheVehicleToolsDataSet.vehicletools.Rows.Clear();

                    MainWindow.gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].EmployeeID;
                    MainWindow.gstrFirstName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].FirstName;
                    MainWindow.gstrLastName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].LastName;

                    TheFindToolsByEmployeeIDDataSet = TheToolsClass.FindToolsByEmployeeID(MainWindow.gintEmployeeID);

                    intNumberOfRecords = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        EmployeeToolsDataSet.employeetoolsRow NewToolRow = TheEmployeeToolsDataSet.employeetools.NewemployeetoolsRow();

                        NewToolRow.ToolCategory = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolCategory;
                        NewToolRow.ToolDescription = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolDescription;
                        NewToolRow.ToolNotes = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolNotes;
                        NewToolRow.ToolID = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].ToolID;
                        NewToolRow.TransactionDate = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intCounter].TransactionDate;

                        TheEmployeeToolsDataSet.employeetools.Rows.Add(NewToolRow);
                    }

                    dgrTools.ItemsSource = TheEmployeeToolsDataSet.employeetools;

                    //checking for a vehicle
                    TheFindVehicleAssigedByEmployeeIDDataSet = TheVehicleMainClass.FindVehicleAssignedByEmployeeID(MainWindow.gintEmployeeID);

                    intNumberOfRecords = TheFindVehicleAssigedByEmployeeIDDataSet.FindVehicleAssignedByEmployeeID.Rows.Count - 1;

                    if (intNumberOfRecords == 0)
                    {
                        txtVehicleAssignment.Text = Convert.ToString(TheFindVehicleAssigedByEmployeeIDDataSet.FindVehicleAssignedByEmployeeID[0].BJCNumber);
                    }
                    else if(intNumberOfRecords > 0)
                    {
                        txtVehicleAssignment.Text = "Multiple";
                    }
                    else
                    {
                        txtVehicleAssignment.Text = "None";
                    }

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intBJCNumber = TheFindVehicleAssigedByEmployeeIDDataSet.FindVehicleAssignedByEmployeeID[intCounter].BJCNumber;

                            TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(Convert.ToString(intBJCNumber));

                            intSecondNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count - 1;

                            for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                            {
                                gintVehicleEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSecondCounter].EmployeeID;

                                TheFindToolsByEmployeeIDDataSet = TheToolsClass.FindToolsByEmployeeID(gintVehicleEmployeeID);

                                intToolNumberOfRecords = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID.Rows.Count - 1;

                                if(intToolNumberOfRecords > -1)
                                {
                                    for(intToolCounter = 0; intToolCounter <= intToolNumberOfRecords; intToolCounter++)
                                    {
                                        VehicleToolsDataSet.vehicletoolsRow NewToolRow = TheVehicleToolsDataSet.vehicletools.NewvehicletoolsRow();

                                        NewToolRow.BJCNumber = intBJCNumber;
                                        NewToolRow.ToolCategory = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].ToolCategory;
                                        NewToolRow.ToolDescription = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].ToolDescription;
                                        NewToolRow.ToolID = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].ToolID;
                                        NewToolRow.ToolNotes = TheFindToolsByEmployeeIDDataSet.FindToolsByEmployeeID[intToolCounter].ToolNotes;

                                        TheVehicleToolsDataSet.vehicletools.Rows.Add(NewToolRow);
                                    }
                                    
                                }
                            }
                        }
                    }

                    dgrVehicleTools.ItemsSource = TheVehicleToolsDataSet.vehicletools;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Terminate Employee // Combo box Change Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitTerminateEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            pdProblemReport = new PrintDialog();
            PrintEmployeeTools();
            PrintVehicleTools();

            blnFatalError = TheEmployeeClass.DeactivateEmployee(MainWindow.gintEmployeeID);

            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("Contact IT");
            }
            else
            {
                txtEnterLastName.Text = "";
                txtVehicleAssignment.Text = "";
                cboSelectEmployee.Items.Clear();
                TheEmployeeToolsDataSet.employeetools.Rows.Clear();
                TheVehicleToolsDataSet.vehicletools.Rows.Clear();
                dgrVehicleTools.ItemsSource = TheVehicleToolsDataSet.vehicletools;
                dgrTools.ItemsSource = TheEmployeeToolsDataSet.employeetools;
                TheMessagesClass.InformationMessage("The Employee is no Longer Active");
            }

        }
        private void PrintEmployeeTools()
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tools Assigned To Employee"))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(MainWindow.gstrFirstName + " " + MainWindow.gstrLastName))));
                    newTableRow.Cells[0].FontSize = 18;
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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Employee Tools Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Terminate Employee // Print Employee Tools " + Ex.Message);
            }
        }
        private void PrintVehicleTools()
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {
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
                    intColumns = TheVehicleToolsDataSet.vehicletools.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tools Assigned To Vehicles Assigned To "))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(MainWindow.gstrFirstName + " " + MainWindow.gstrLastName))));
                    newTableRow.Cells[0].FontSize = 18;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Description"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Category"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool Notes"))));
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

                    intNumberOfRecords = TheVehicleToolsDataSet.vehicletools.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheVehicleToolsDataSet.vehicletools[intReportRowCounter][intColumnCounter].ToString()))));

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
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Vehicle Tools Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Print Button " + Ex.Message);
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
