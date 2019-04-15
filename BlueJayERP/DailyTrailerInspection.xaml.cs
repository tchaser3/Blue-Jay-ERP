/* Title:           Daily Trailer Inspection
 * Date:            9-24-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to enter daily trailer inspections */

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
using TrailersDLL;
using DailyTrailerInspectionDLL;
using TrailerHistoryDLL;
using TrailerProblemDLL;
using TrailerProblemUpdateDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for DailyTrailerInspection.xaml
    /// </summary>
    public partial class DailyTrailerInspection : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailersClass TheTrailerClass = new TrailersClass();
        DailyTrailerInspectionClass TheDailyTrailerInspectionClass = new DailyTrailerInspectionClass();
        TrailerHistoryClass TheTrailerHistoryClass = new TrailerHistoryClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        TrailerProblemUpdateClass TheTrailerProblemUpdateClass = new TrailerProblemUpdateClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindTrailerByTrailerNumberDataSet TheFindTrailerByTrailerNumberDataSet = new FindTrailerByTrailerNumberDataSet();
        FindDailyTrailerInspectionByDateMatchDataSet TheFindDailyTrailerInspectionByDateMatchDataSet = new FindDailyTrailerInspectionByDateMatchDataSet();
        FindTrailerProblemByDateMatchDataSet TheFindTrailerProblemByDateMatchDataSet = new FindTrailerProblemByDateMatchDataSet();

        string gstrInspectionStatus;
        bool gblnProblemReported;

        public DailyTrailerInspection()
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

        private void mitClose_Click(object sender, RoutedEventArgs e)
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

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastname;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            strLastname = txtEnterLastName.Text;
            intLength = strLastname.Length;

            if(intLength > 2)
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastname);

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
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Trailer Inspection // Select Employee Combo Box event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            cboDamageReported.Items.Clear();
            cboDamageReported.Items.Add("Select Damage Reported");
            cboDamageReported.Items.Add("YES");
            cboDamageReported.Items.Add("NO");
            cboDamageReported.SelectedIndex = 0;
            cboSelectEmployee.Items.Clear();
            rdoPassed.IsChecked = true;
            rdoFailed.IsChecked = false;
            rdoPassedServiceRequired.IsChecked = false;
            txtEnterTrailerNumber.Text = "";
            txtEnterLastName.Text = "";
            cboDamageReported.IsEnabled = false;
            cboSelectEmployee.IsEnabled = false;
            MainWindow.gintTrailerID = -1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void rdoPassed_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED";
            gblnProblemReported = false;
        }

        private void rdoPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            gblnProblemReported = true;
        }

        private void rdoFailed_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "FAILED";
            gblnProblemReported = true;
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            string strErrorMessage = "";

            try
            {
                if(MainWindow.gintTrailerID == -1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Trailer Was Not Entered\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Was Not Selected\n";
                }
                if(cboDamageReported.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Damage Reported Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                if(gblnProblemReported == false)
                {
                    MainWindow.gstrInspectionProblem = "NO PROBLEMS REPORTED";
                }
                else
                {
                    InspectionTrailerProblems InspectionTrailerProblems = new InspectionTrailerProblems();
                    InspectionTrailerProblems.ShowDialog();
                }

                blnFatalError = TheDailyTrailerInspectionClass.InsertDailyTrailerInspection(MainWindow.gintTrailerID, MainWindow.gintEmployeeID, datTransactionDate, gstrInspectionStatus, MainWindow.gstrInspectionProblem);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheTrailerHistoryClass.InsertTrailerHistory(MainWindow.gintTrailerID, MainWindow.gintEmployeeID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "TRAILER SIGNED OUT");


                if (blnFatalError == true)
                    throw new Exception();


                if(gstrInspectionStatus == "FAILED")
                {
                    PleaseWait pleaseWait = new PleaseWait();
                    pleaseWait.Show();

                    TheSendEmailClass.TrailerEmailMessage(txtEnterTrailerNumber.Text, MainWindow.gstrInspectionProblem);

                    pleaseWait.Close();
                }       

                TheMessagesClass.InformationMessage("The Trailer Inspection Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Daily Trailer Inspection // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

            }
        }

        private void cboDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mitProcess.IsEnabled = true;

            if(cboDamageReported.SelectedIndex == 1)
            {
                TrailerBodyDamage TrailerBodyDamage = new TrailerBodyDamage();
                TrailerBodyDamage.ShowDialog();
            }
            if(cboDamageReported.SelectedIndex < 1)
            {
                mitProcess.IsEnabled = false;
            }
            
        }

        private void txtEnterTrailerNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strTrailerNumber;
            int intLength;
            int intRecordsReturned;

            strTrailerNumber = txtEnterTrailerNumber.Text;
            intLength = strTrailerNumber.Length;
            cboDamageReported.IsEnabled = false;
            cboSelectEmployee.IsEnabled = false;

            if((intLength == 4) || (intLength >= 6))
            {
                TheFindTrailerByTrailerNumberDataSet = TheTrailerClass.FindTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Trailer Not Found");
                    return;
                }

                MainWindow.gintTrailerID = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerID;
                cboDamageReported.IsEnabled = true;
                cboSelectEmployee.IsEnabled = true;
            }

        }
    }
}
