/* Title:           Assign Trailer
 * Date:            9-20-18
 * Author:          Terrance Holmes
 * 
 * Description:     This is used to assign trailers */

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
using TrailerHistoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AssignTrailer.xaml
    /// </summary>
    public partial class AssignTrailer : Window
    {
        //setting classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        TrailerHistoryClass TheTrailerHistoryClass = new TrailerHistoryClass();

        FindTrailerByTrailerNumberDataSet TheFindTrailerByTrailerNumberDataSet = new FindTrailerByTrailerNumberDataSet();
        FindTrailerByEmployeeIDDataSet TheFindTrailerByEmployeeIDDataSet = new FindTrailerByEmployeeIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        bool gblnAvailable;
        string gstrHistoryNotes;

        public AssignTrailer()
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

        private void txtTrailerNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intLength;
            string strTrailerNumber;
            int intRecordsReturned;

            strTrailerNumber = txtTrailerNumber.Text;
            intLength = strTrailerNumber.Length;
            if (intLength == 4)
            {
                TheFindTrailerByTrailerNumberDataSet = TheTrailersClass.FindTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    txtFirstName.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].FirstName;
                    txtLastName.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].LastName;
                    MainWindow.gintTrailerID = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerID;
                    txtEnterLastName.IsReadOnly = false;
                }
            }
            else if(intLength == 6)
            {
                TheFindTrailerByTrailerNumberDataSet = TheTrailersClass.FindTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    txtFirstName.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].FirstName;
                    txtLastName.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].LastName;
                    MainWindow.gintTrailerID = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerID;
                    txtEnterLastName.IsReadOnly = false;
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Trailer Not Found");
                }
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            strLastName = txtEnterLastName.Text;
            intLength = strLastName.Length;
            if(intLength > 2)
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter<= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                    cboSelectEmployee.IsEnabled = true;
                }
                else
                {
                    TheMessagesClass.InformationMessage("Employee Not Found");
                    return;
                }
            }
        }
        private void ResetControls()
        {
            cboSelectEmployee.Items.Clear();
            txtEnterLastName.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtTrailerNumber.Text = "";
            mitProcess.IsEnabled = false;
            cboSelectEmployee.IsEnabled = false;
            txtEnterLastName.IsReadOnly = true;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;
            
            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                mitProcess.IsEnabled = false;

                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                if (TheComboEmployeeDataSet.employees[intSelectedIndex].LastName != "WAREHOUSE")
                {
                    TheFindTrailerByEmployeeIDDataSet = TheTrailersClass.FindTrailerByEmployeeID(MainWindow.gintEmployeeID);

                    intRecordsReturned = TheFindTrailerByEmployeeIDDataSet.FindTrailerByEmployeeID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Is Assigned to Trailer Number " + TheFindTrailerByEmployeeIDDataSet.FindTrailerByEmployeeID[0].TrailerNumber);
                        return;
                    }
                    else
                    {
                        gblnAvailable = false;
                        gstrHistoryNotes = "EMPLOYEE ASSIGNED TO TRAILER";
                    }
                }
                else
                {
                    gblnAvailable = true;
                    gstrHistoryNotes = "TRAILER RETURNED TO WAREHOUSE";
                }

                mitProcess.IsEnabled = true;

            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheTrailersClass.UpdateTrailerEmployeeAndAvailability(MainWindow.gintTrailerID, MainWindow.gintEmployeeID, gblnAvailable);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheTrailerHistoryClass.InsertTrailerHistory(MainWindow.gintTrailerID, MainWindow.gintEmployeeID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, gstrHistoryNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Trailer has been Assigned");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Assign Trailer // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
