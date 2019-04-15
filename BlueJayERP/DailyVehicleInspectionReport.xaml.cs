/* Title:           Daily Vehicle Inspection Report
 * Date:            4-18-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is where the daily vehicle inspection report is completed */

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
using DateSearchDLL;
using InspectionsDLL;
using NewEventLogDLL;
using VehicleMainDLL;
using DataValidationDLL;
using NewEmployeeDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DailyVehicleInspectionReport.xaml
    /// </summary>
    public partial class DailyVehicleInspectionReport : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindDailyVehicleInspectionByDateRangeDataSet TheFindDailyVehicleInspectionByDateRangeDataSet = new FindDailyVehicleInspectionByDateRangeDataSet();
        FindVehicleInspectionProblemsByInspectionIDDataSet TheFindVehicleInspectionProblemByInsepctionIDDataSet = new FindVehicleInspectionProblemsByInspectionIDDataSet();
        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindDailyVehicleInspectionsByEmployeeIDAndDateRangeDataSet TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet = new FindDailyVehicleInspectionsByEmployeeIDAndDateRangeDataSet();
        DailyVehicleInspectionReportDataSet TheDailyVehicleInspectionReportDataSet = new DailyVehicleInspectionReportDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        int gintEmployeeID;
        int gintVehicleID;
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        string gstrReportType;

        public DailyVehicleInspectionReport()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Clear();
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtSearchInfo.Text = "";
            cboSelectEmployee.Items.Clear();
            dgrResults.ItemsSource = TheDailyVehicleInspectionReportDataSet.dailyinspection;
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }
        private void TodaysInspections()
        {
            //setting local varaibles
            DateTime datTodaysDate = DateTime.Now;
            DateTime datLimitDate;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intInspectionID;

            try
            {
                TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Clear();
                rdoDateRange.IsChecked = true;
                datTodaysDate = TheDataSearchClass.RemoveTime(datTodaysDate);
                datLimitDate = TheDataSearchClass.AddingDays(datTodaysDate, 1);

                TheFindDailyVehicleInspectionByDateRangeDataSet = TheInspectionClass.FindDailyVehicleInspectionByDateRange(datTodaysDate, datLimitDate);

                intNumberOfRecords = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        DailyVehicleInspectionReportDataSet.dailyinspectionRow NewInspectionRow = TheDailyVehicleInspectionReportDataSet.dailyinspection.NewdailyinspectionRow();

                        intInspectionID = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].TransactionID;

                        TheFindVehicleInspectionProblemByInsepctionIDDataSet = TheInspectionClass.FindVehicleInspectionProblemsbyInspectionID(intInspectionID);

                        intRecordsReturned = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID.Rows.Count;

                        if (intRecordsReturned == 0)
                        {
                            NewInspectionRow.InspectionNotes = "NO NOTES REPORTED";
                        }
                        else
                        {
                            NewInspectionRow.InspectionNotes = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID[0].InspectionNotes;
                        }

                        NewInspectionRow.FirstName = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].FirstName;
                        NewInspectionRow.InspectionDate = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].InspectionDate;
                        NewInspectionRow.InspectionID = intInspectionID;
                        NewInspectionRow.InspectionStatus = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].InspectionStatus;
                        NewInspectionRow.LastName = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].LastName;
                        NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].OdometerReading;
                        NewInspectionRow.VehicleID = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].VehicleID;
                        NewInspectionRow.VehicleNumber = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].VehicleNumber;
                        NewInspectionRow.HomeOffice = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].AssignedOffice;

                        TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Add(NewInspectionRow);
                    }
                }

                ResetControls();
                dgrResults.ItemsSource = TheDailyVehicleInspectionReportDataSet.dailyinspection;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Todays Inspection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TodaysInspections();
        }

        private void mitGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strValueForValidation;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;
            int intInspectionID;
            int intRecordsReturned;
            string strVehicleNumber;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Clear();

                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Start Date is not a Date\n";
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
                    strErrorMessage += "End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                    gdatEndDate = TheDataSearchClass.RemoveTime(gdatEndDate);
                    gdatEndDate = TheDataSearchClass.AddingDays(gdatEndDate, 1);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    PleaseWait.Close();
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("Start Date is After the End Date");
                        PleaseWait.Close();
                        return;
                    }
                }

                if(gstrReportType == "DATE RANGE")
                {
                    TheFindDailyVehicleInspectionByDateRangeDataSet = TheInspectionClass.FindDailyVehicleInspectionByDateRange(gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            DailyVehicleInspectionReportDataSet.dailyinspectionRow NewInspectionRow = TheDailyVehicleInspectionReportDataSet.dailyinspection.NewdailyinspectionRow();

                            intInspectionID = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].TransactionID;

                            TheFindVehicleInspectionProblemByInsepctionIDDataSet = TheInspectionClass.FindVehicleInspectionProblemsbyInspectionID(intInspectionID);

                            intRecordsReturned = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID.Rows.Count;

                            if (intRecordsReturned == 0)
                            {
                                NewInspectionRow.InspectionNotes = "NO NOTES REPORTED";
                            }
                            else
                            {
                                NewInspectionRow.InspectionNotes = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID[0].InspectionNotes;
                            }

                            NewInspectionRow.FirstName = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].FirstName;
                            NewInspectionRow.InspectionDate = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].InspectionDate;
                            NewInspectionRow.InspectionID = intInspectionID;
                            NewInspectionRow.InspectionStatus = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].InspectionStatus;
                            NewInspectionRow.LastName = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].LastName;
                            NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].OdometerReading;
                            NewInspectionRow.VehicleID = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].VehicleID;
                            NewInspectionRow.VehicleNumber = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].VehicleNumber;
                            NewInspectionRow.HomeOffice = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].AssignedOffice;

                            TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Add(NewInspectionRow);
                        }
                    }
                }
                else if(gstrReportType == "EMPLOYEE")
                {
                    TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet = TheInspectionClass.FindDailyVehicleInspectionByEmployeeIDAndDateRange(gintEmployeeID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            DailyVehicleInspectionReportDataSet.dailyinspectionRow NewInspectionRow = TheDailyVehicleInspectionReportDataSet.dailyinspection.NewdailyinspectionRow();

                            intInspectionID = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].TransactionID;

                            TheFindVehicleInspectionProblemByInsepctionIDDataSet = TheInspectionClass.FindVehicleInspectionProblemsbyInspectionID(intInspectionID);

                            intRecordsReturned = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID.Rows.Count;

                            if (intRecordsReturned == 0)
                            {
                                NewInspectionRow.InspectionNotes = "NO NOTES REPORTED";
                            }
                            else
                            {
                                NewInspectionRow.InspectionNotes = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID[0].InspectionNotes;
                            }

                            NewInspectionRow.FirstName = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].FirstName;
                            NewInspectionRow.InspectionDate = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].InspectionDate;
                            NewInspectionRow.InspectionID = intInspectionID;
                            NewInspectionRow.InspectionStatus = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].InspectionStatus;
                            NewInspectionRow.LastName = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].LastName;
                            NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].OdometerReading;
                            NewInspectionRow.VehicleID = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].VehicleID;
                            NewInspectionRow.VehicleNumber = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].VehicleNumber;
                            NewInspectionRow.HomeOffice = TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet.FindDailyVehicleInspectionsByEmployeeIDAndDateRange[intCounter].AssignedOffice;

                            TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Add(NewInspectionRow);
                        }
                    }

                }
                else if(gstrReportType == "VEHICLE NUMBER")
                {
                    strVehicleNumber = txtSearchInfo.Text;
                    if(strVehicleNumber == "")
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Number Not Entered");
                        PleaseWait.Close();
                        return;
                    }

                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Numbered Does Not Exist");
                        PleaseWait.Close();
                        return;
                    }
                    else
                    {
                        gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }

                    TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = TheInspectionClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(gintVehicleID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            DailyVehicleInspectionReportDataSet.dailyinspectionRow NewInspectionRow = TheDailyVehicleInspectionReportDataSet.dailyinspection.NewdailyinspectionRow();

                            intInspectionID = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].TransactionID;

                            TheFindVehicleInspectionProblemByInsepctionIDDataSet = TheInspectionClass.FindVehicleInspectionProblemsbyInspectionID(intInspectionID);

                            intRecordsReturned = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID.Rows.Count;

                            if (intRecordsReturned == 0)
                            {
                                NewInspectionRow.InspectionNotes = "NO NOTES REPORTED";
                            }
                            else
                            {
                                NewInspectionRow.InspectionNotes = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID[0].InspectionNotes;
                            }

                            NewInspectionRow.FirstName = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].FirstName;
                            NewInspectionRow.InspectionDate = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].InspectionDate;
                            NewInspectionRow.InspectionID = intInspectionID;
                            NewInspectionRow.InspectionStatus = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].InspectionStatus;
                            NewInspectionRow.LastName = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].LastName;
                            NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].OdometerReading;
                            NewInspectionRow.VehicleID = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].VehicleID;
                            NewInspectionRow.VehicleNumber = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].VehicleNumber;
                            NewInspectionRow.HomeOffice = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange[intCounter].AssignedOffice;

                            TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Add(NewInspectionRow);
                        }
                    }
                }

                dgrResults.ItemsSource = TheDailyVehicleInspectionReportDataSet.dailyinspection;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Generate Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void rdoDateRange_Checked(object sender, RoutedEventArgs e)
        {
            gstrReportType = "DATE RANGE";
            ResetControls();
        }

        private void rdoVehicleNumber_Checked(object sender, RoutedEventArgs e)
        {
            gstrReportType = "VEHICLE NUMBER";
            ResetControls();
            txtSearchInfo.Visibility = Visibility.Visible;
            lblSearchInfo.Visibility = Visibility.Visible;
            lblSearchInfo.Content = "Enter Vehicle Number";
        }

        private void rdoEmployee_Checked(object sender, RoutedEventArgs e)
        {
            gstrReportType = "EMPLOYEE";
            ResetControls();
            cboSelectEmployee.Visibility = Visibility.Visible;
            txtSearchInfo.Visibility = Visibility.Visible;
            lblSearchInfo.Visibility = Visibility.Visible;
            lblSelectEmployee.Visibility = Visibility.Visible;
            lblSearchInfo.Content = "Enter Last Name";
        }
        private void ResetControls()
        {
            cboSelectEmployee.Visibility = Visibility.Hidden;
            txtSearchInfo.Visibility = Visibility.Hidden;
            lblSearchInfo.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
        }

        private void txtSearchInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            string strSearchValue;

            strSearchValue = txtSearchInfo.Text;
            intLength = strSearchValue.Length;

            if (gstrReportType == "EMPLOYEE")
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strSearchValue);

                cboSelectEmployee.Items.Clear();

                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }
                }
                else
                {
                    TheMessagesClass.InformationMessage("Employee Not Found");
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting up for the loop
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;
            string strFullName;

            intSelectedIndex = cboSelectEmployee.SelectedIndex;

            if (intSelectedIndex > 0)
            {
                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                strFullName = cboSelectEmployee.SelectedItem.ToString();

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (strFullName == TheComboEmployeeDataSet.employees[intCounter].FullName)
                    {
                        gintEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                    }
                }
            }
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
                    intColumns = TheDailyVehicleInspectionReportDataSet.dailyinspection.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        cancelledTable.Columns.Add(new TableColumn());
                    }
                    cancelledTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Daily Vehicle Inspection Report"))));
                    newTableRow.Cells[0].FontSize = 20;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Inspection ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Vehicle Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Odometer"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Status"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Notes"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Home Office"))));


                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        cancelledTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = cancelledTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheDailyVehicleInspectionReportDataSet.dailyinspection[intReportRowCounter][intColumnCounter].ToString()))));


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
                    pdCancelledReport.PrintDocument(((IDocumentPaginatorSource)fdCancelledLines).DocumentPaginator, "Daily Vehicle Inspection Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Print Menu Item " + Ex.Message);
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
                intRowNumberOfRecords = TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Count;
                intColumnNumberOfRecords = TheDailyVehicleInspectionReportDataSet.dailyinspection.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheDailyVehicleInspectionReportDataSet.dailyinspection.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void mitEmailReport_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strMessage = "<h1>Daily Vehicle Inspection Notes</h1>";
            string strHeader = "Daily Vehicle Inspection Report - Do Not Reply";

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                strMessage += "<table>";
                strMessage += "<tr>";
                strMessage += "<td><b>Vehicle Number</b></td>";
                strMessage += "<td><b>First Name</b></td>";
                strMessage += "<td><b>Last Name</b></td>";
                strMessage += "<td><b>Home Office</b></td>";
                strMessage += "<td><b>Odometer</b></td>";
                strMessage += "<td><b>Inspection Status</b></td>";
                strMessage += "<td><b>Inspection Notes</b></td>";
                strMessage += "</tr>";
                strMessage += "<p>               </p>";

                TodaysInspections();

                intNumberOfRecords = TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strMessage += "<tr>";
                    strMessage += "<td>" + TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].VehicleNumber + "</td>";
                    strMessage += "<td>" + TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].FirstName + "</td>";
                    strMessage += "<td>" + TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].LastName + "</td>";
                    strMessage += "<td>" + TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].HomeOffice + "</td>";
                    strMessage += "<td>" + Convert.ToString(TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].OdometerReading) + "</td>";
                    strMessage += "<td>" + TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].InspectionStatus + "</td>";
                    strMessage += "<td>" + TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].InspectionNotes + "</td>";
                    strMessage += "</tr>";

                }

                strMessage += "</table>";

                TheSendEmailClass.VehicleReports(strHeader, strMessage);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Vehicle Inspection Report // Email Report Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
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
