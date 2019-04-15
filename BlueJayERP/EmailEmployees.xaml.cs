/* Title:           Email Employees
 * Date:            8-22-18
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the employees to email any report from the program */

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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EmailEmployees.xaml
    /// </summary>
    public partial class EmailEmployees : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data;
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeEmailAddressDataSet TheFindEmployeeEmailAddressDataSet = new FindEmployeeEmailAddressDataSet();

        public EmailEmployees()
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

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            strLastName = txtLastName.Text;
            intLength = strLastName.Length;
            if(intLength > 2)
            {
                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
                else
                {
                    TheMessagesClass.ErrorMessage("Employee Was Not Found");
                    return;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgrEmployees.ItemsSource = MainWindow.TheEmailListDataSet.employees;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting up local variables
            int intEmployeeID;
            int intSelectedIndex;
            string strFirstName;
            string strLastname;
            string strEmailAddress;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    TheFindEmployeeEmailAddressDataSet = TheEmployeeClass.FindEmployeeEmailAddress(intEmployeeID);

                    strFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    strLastname = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;
                    
                    if(TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].IsEmailAddressNull() == true)
                    {
                        TheMessagesClass.InformationMessage("The Employee Selected does not have an Email Address");
                        return;
                    }

                    strEmailAddress = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].EmailAddress;

                    EmailListDataSet.employeesRow NewEmployeeRow = MainWindow.TheEmailListDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastname;
                    NewEmployeeRow.EmailAddress = strEmailAddress;

                    MainWindow.TheEmailListDataSet.employees.Rows.Add(NewEmployeeRow);

                    dgrEmployees.ItemsSource = MainWindow.TheEmailListDataSet.employees;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Email Emaployees // Combo Box Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitResetEmail_Click(object sender, RoutedEventArgs e)
        {
            txtLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            MainWindow.TheEmailListDataSet.employees.Rows.Clear();
            dgrEmployees.ItemsSource = MainWindow.TheEmailListDataSet.employees;
        }
    }
}
