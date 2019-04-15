/* Title:           Manager Productivity Punched Report
 * Date:            11-23-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window that will show the Manager Productivty Punched Report */

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
using EmployeeProjectAssignmentDLL;
using EmployeePunchedHoursDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using DataValidationDLL;
using Microsoft.Win32;
using System.Windows.Forms.DataVisualization.Charting;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ManagerProductivityPunchedReport.xaml
    /// </summary>
    public partial class ManagerProductivityPunchedReport : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        EmployeeProductionPunchDataSet TheEmployeeProductionPunchDataSet = new EmployeeProductionPunchDataSet();
        FindActiveNonExemptEmployeesByPayDateDataSet TheFindActiveNoExemptEmployeesDataSet = new FindActiveNonExemptEmployeesByPayDateDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindEmployeeProductionHoursOverPayPeriodDataSet TheFindEmployeeProductionHoursOverPayPeriodDataSet = new FindEmployeeProductionHoursOverPayPeriodDataSet();
        FindEmployeePunchedHoursDataSet TheFindEmployeePunchedHoursDataSet = new FindEmployeePunchedHoursDataSet();
        ProductionManagerStatsDataSet TheProductionManagerStatsDataSet = new ProductionManagerStatsDataSet();
        ProductionManagerStatsDataSet TheFinalProductionManagerStatsDataSet = new ProductionManagerStatsDataSet();
        string[] gstrLastName = new string[20];

        int gintManagerCounter;
        int gintManagerUpperLimit;
        decimal gdecTotalEntries;
        decimal gdecTotalAcceptable;

        public ManagerProductivityPunchedReport()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove(); 
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterPayPeriod.Text = "";
            TheProductionManagerStatsDataSet.productionmanager.Rows.Clear();
            TheEmployeeProductionPunchDataSet.employees.Rows.Clear();
        }
        private void mitProcessPayPeriod_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            int intManagerID;
            string strFirstName;
            string strLastName;
            string strManagerFirstName;
            string strManagerLastName;
            int intRecordReturned;
            bool blnFatalError = false;
            decimal decHoursPunched;
            decimal decProductiveHours;
            int intManagerCounter;
            bool blnItemFound;
            decimal decVariance;
            int intTotalNumber;
            int intSelectedIndex;
            int intTotalNumberNeeded;
            decimal decTopPercentage;
            decimal decWorkingPercentage;
            int intArrayCounter;
            int intArrayNumberOfRecords = -1;

            try
            {
                TheEmployeeProductionPunchDataSet.employees.Rows.Clear();
                TheProductionManagerStatsDataSet.productionmanager.Rows.Clear();
                TheFinalProductionManagerStatsDataSet.productionmanager.Rows.Clear();

                blnFatalError = TheDataValidationClass.VerifyDateData(txtEnterPayPeriod.Text);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is not a Date");
                    return;
                }
                else
                {
                    MainWindow.gdatEndDate = Convert.ToDateTime(txtEnterPayPeriod.Text);
                    MainWindow.gdatStartDate = TheDateSearchClass.SubtractingDays(MainWindow.gdatEndDate, 6);
                }

                TheFindActiveNoExemptEmployeesDataSet = TheEmployeeClass.FindActiveNonExemptEmployeesByPayDate(MainWindow.gdatEndDate);

                intNumberOfRecords = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //getting employee info
                    intEmployeeID = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].EmployeeID;
                    strFirstName = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].FirstName;
                    strLastName = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].LastName;
                    intManagerID = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].ManagerID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                    strManagerFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    strManagerLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                    //getting employee punched hours
                    TheFindEmployeePunchedHoursDataSet = TheEmployeePunchedHoursClass.FindEmployeePunchedHours(intEmployeeID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    intRecordReturned = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours.Rows.Count;

                    if (intRecordReturned == 0)
                    {
                        decHoursPunched = 0;
                    }
                    else
                    {
                        decHoursPunched = TheFindEmployeePunchedHoursDataSet.FindEmployeePunchedHours[0].PunchedHours;
                    }

                    //getting production hours
                    TheFindEmployeeProductionHoursOverPayPeriodDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeProductionHoursOverPayPeriodDataSet(intEmployeeID, MainWindow.gdatStartDate, MainWindow.gdatEndDate);

                    intRecordReturned = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod.Rows.Count;

                    if (intRecordReturned == 0)
                    {
                        decProductiveHours = 0;
                    }
                    else
                    {
                        decProductiveHours = TheFindEmployeeProductionHoursOverPayPeriodDataSet.FindEmployeeProductionHoursOverPayPeriod[0].ProductionHours;
                    }

                    //loading the dataset
                    EmployeeProductionPunchDataSet.employeesRow NewEmployeeRow = TheEmployeeProductionPunchDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.HomeOffice = TheFindActiveNoExemptEmployeesDataSet.FindActiveNonExemptEmployeesByPayDate[intCounter].HomeOffice;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.ManagerFirstName = strManagerFirstName;
                    NewEmployeeRow.ManagerLastName = strManagerLastName;
                    NewEmployeeRow.ProductionHours = decProductiveHours;
                    NewEmployeeRow.PunchedHours = decHoursPunched;
                    NewEmployeeRow.HourVariance = decProductiveHours - decHoursPunched;

                    TheEmployeeProductionPunchDataSet.employees.Rows.Add(NewEmployeeRow);
                }

                intNumberOfRecords = TheEmployeeProductionPunchDataSet.employees.Rows.Count - 1;
                gintManagerCounter = 0;
                gdecTotalAcceptable = 0;
                gdecTotalEntries = 0;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strManagerFirstName = TheEmployeeProductionPunchDataSet.employees[intCounter].ManagerFirstName;
                    strManagerLastName = TheEmployeeProductionPunchDataSet.employees[intCounter].ManagerLastName;
                    decVariance = TheEmployeeProductionPunchDataSet.employees[intCounter].HourVariance;
                    blnItemFound = false;
                    intArrayNumberOfRecords = -1;

                    if(gintManagerCounter > 0)
                    {
                        for(intManagerCounter = 0; intManagerCounter <= gintManagerUpperLimit; intManagerCounter++)
                        {
                            if(strManagerFirstName == TheProductionManagerStatsDataSet.productionmanager[intManagerCounter].FirstName)
                            {
                                if(strManagerLastName == TheProductionManagerStatsDataSet.productionmanager[intManagerCounter].LastName)
                                {
                                    blnItemFound = true;

                                    if(decVariance > - 6)
                                    {
                                        TheProductionManagerStatsDataSet.productionmanager[intManagerCounter].ZeroToFive++;
                                        gdecTotalEntries++;
                                        gdecTotalAcceptable++;
                                    }
                                    else if((decVariance < -5) && (decVariance > -11))
                                    {
                                        TheProductionManagerStatsDataSet.productionmanager[intManagerCounter].SixToTen++;
                                        gdecTotalEntries++;
                                    }
                                    else if(decVariance < - 10)
                                    {
                                        TheProductionManagerStatsDataSet.productionmanager[intManagerCounter].Above10++;
                                        gdecTotalEntries++;
                                    }
                                }
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        ProductionManagerStatsDataSet.productionmanagerRow NewManagerRow = TheProductionManagerStatsDataSet.productionmanager.NewproductionmanagerRow();

                        NewManagerRow.FirstName = strManagerFirstName;
                        NewManagerRow.LastName = strManagerLastName;
                        NewManagerRow.PercentInTolerance = 0;

                        if (Convert.ToDouble(decVariance) > -5.01)
                        {
                            NewManagerRow.ZeroToFive = 1;
                            NewManagerRow.SixToTen = 0;
                            NewManagerRow.Above10 = 0;
                            gdecTotalEntries++;
                            gdecTotalAcceptable++;
                        }
                        else if ((Convert.ToDouble(decVariance) < -5) && (Convert.ToDouble(decVariance) > -10.01))
                        {
                            NewManagerRow.ZeroToFive = 0;
                            NewManagerRow.SixToTen = 1;
                            NewManagerRow.Above10 = 0;
                            gdecTotalEntries++;
                        }
                        else if (Convert.ToDouble(decVariance) < -10)
                        {
                            NewManagerRow.ZeroToFive = 0;
                            NewManagerRow.SixToTen = 0;
                            NewManagerRow.Above10 = 1;
                            gdecTotalEntries++;
                        }

                        TheProductionManagerStatsDataSet.productionmanager.Rows.Add(NewManagerRow);
                        gintManagerUpperLimit = gintManagerCounter;
                        gintManagerCounter++;
                    }
                }

                intNumberOfRecords = TheProductionManagerStatsDataSet.productionmanager.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intTotalNumber = TheProductionManagerStatsDataSet.productionmanager[intCounter].ZeroToFive;
                    intTotalNumber += TheProductionManagerStatsDataSet.productionmanager[intCounter].SixToTen;
                    intTotalNumber += TheProductionManagerStatsDataSet.productionmanager[intCounter].Above10;

                    TheProductionManagerStatsDataSet.productionmanager[intCounter].PercentInTolerance = (Convert.ToDecimal(TheProductionManagerStatsDataSet.productionmanager[intCounter].ZeroToFive) / intTotalNumber) * 100;

                    TheProductionManagerStatsDataSet.productionmanager[intCounter].PercentInTolerance = Math.Round(TheProductionManagerStatsDataSet.productionmanager[intCounter].PercentInTolerance, 2);
                }

                intTotalNumberNeeded = 0;
                decTopPercentage = 1000;
                decWorkingPercentage = 0;
                strLastName = "";
                intSelectedIndex = -1;

                while(intTotalNumberNeeded <= intNumberOfRecords)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (TheProductionManagerStatsDataSet.productionmanager[intCounter].PercentInTolerance <= decTopPercentage)
                        {
                            if(TheProductionManagerStatsDataSet.productionmanager[intCounter].PercentInTolerance >= decWorkingPercentage)
                            {
                                blnItemFound = false;

                                if(intArrayNumberOfRecords > -1)
                                {
                                    for(intArrayCounter = 0; intArrayCounter <= intArrayNumberOfRecords; intArrayCounter++)
                                    {
                                        if(TheProductionManagerStatsDataSet.productionmanager[intCounter].LastName == gstrLastName[intArrayCounter])
                                        {
                                            blnItemFound = true;
                                        }

                                    }
                                }


                                if (blnItemFound == false)
                                {
                                    decWorkingPercentage = TheProductionManagerStatsDataSet.productionmanager[intCounter].PercentInTolerance;
                                    intSelectedIndex = intCounter;
                                }
                            }
                        }
                    }

                    ProductionManagerStatsDataSet.productionmanagerRow NewManagerRow = TheFinalProductionManagerStatsDataSet.productionmanager.NewproductionmanagerRow();

                    NewManagerRow.Above10 = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].Above10;
                    NewManagerRow.FirstName = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].FirstName;
                    NewManagerRow.LastName = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].LastName;
                    NewManagerRow.PercentInTolerance = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].PercentInTolerance;
                    NewManagerRow.SixToTen = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].SixToTen;
                    NewManagerRow.ZeroToFive = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].ZeroToFive;

                    TheFinalProductionManagerStatsDataSet.productionmanager.Rows.Add(NewManagerRow);
                    intTotalNumberNeeded++;
                    decTopPercentage = decWorkingPercentage;
                    decWorkingPercentage = 0;
                    intArrayNumberOfRecords++;
                    gstrLastName[intArrayNumberOfRecords] = TheProductionManagerStatsDataSet.productionmanager[intSelectedIndex].LastName;
                }


                dgrResults.ItemsSource = TheFinalProductionManagerStatsDataSet.productionmanager;
                txtCompanyPercentage.Text = Convert.ToString((Math.Round(((gdecTotalAcceptable / gdecTotalEntries) * 100), 2)));

                intNumberOfRecords = TheFinalProductionManagerStatsDataSet.productionmanager.Rows.Count - 1;

                //CustomLabel strManager
                Chart chart = this.FindName("MyWinformChart") as Chart;
                chart.Legends.Clear();
                Series ZeroToFive = chart.Series["ZeroToFive"];
                Series SixToTen = chart.Series["SixToTen"];
                Series Above10 = chart.Series["Above10"];
                chart.Legends.Add(new Legend("ZeroToFive"));
                //chart.Legends.Add(new Legend("SixToTen"));
                //chart.Legends.Add(new Legend("Above10"));

                ZeroToFive.Points.Clear();
                SixToTen.Points.Clear();
                Above10.Points.Clear();
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart.ChartAreas[0].AxisX.Interval = 15;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    ZeroToFive.Points.Add(TheFinalProductionManagerStatsDataSet.productionmanager[intCounter].ZeroToFive);
                    SixToTen.Points.Add(TheFinalProductionManagerStatsDataSet.productionmanager[intCounter].SixToTen);
                    Above10.Points.Add(TheFinalProductionManagerStatsDataSet.productionmanager[intCounter].Above10);

                    chart.ChartAreas[0].AxisX.CustomLabels.Add(intCounter + .5, intCounter + 1.5, TheFinalProductionManagerStatsDataSet.productionmanager[intCounter].LastName, 1, LabelMarkStyle.None);
                }

                //chart.Series["ZeroToFive"].XValueMember = "Key";
                //chart.Series["ZeroToFive"].YValueMembers = "Value";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Manager Productivity Punched Report // Process Pay Period Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

    }
}
