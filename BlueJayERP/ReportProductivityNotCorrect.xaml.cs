/* Title:       Report Productivity Not Correct
 * Date:        1-3-20
 * Author:      Terry Holmes
 * 
 * Description: This is used to report when an employee doesn't do the productivity correctly */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using EmployeeProjectAssignmentDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ReportProductivityNotCorrect.xaml
    /// </summary>
    public partial class ReportProductivityNotCorrect : Window
    {
        //setting up the classes
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        int gintEmployeeID;

        public ReportProductivityNotCorrect()
        {
            InitializeComponent();
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
            //setting up the variables
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Wasn't Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Productivity Not Correct // Last Name Textbox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                btnProcess.IsEnabled = true;
            }
        }
        private void ResetControls()
        {
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            btnProcess.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    TheMessagesClass.ErrorMessage("The Employee Wasn't Selected");
                    return;
                }

                blnFatalError = TheEmployeeProjectAssignmentClass.InsertProductivityNotCorrect(gintEmployeeID, DateTime.Now);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Employee Has Been Reported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Productivity Not Correct // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
    }
}
