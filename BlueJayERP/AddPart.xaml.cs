/* Title:           Add Part
 * Date:            3-20-18
 * Author:          Terrance Holmes
 * 
 * Description:     This is where the parts are added */

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
using DataValidationDLL;
using NewPartNumbersDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddPart.xaml
    /// </summary>
    public partial class AddPart : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();

        //setting up the data
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindPartFromMasterPartListByPartNumberDataSet TheFindPartFromMasterPartListByPartNumberDataSet = new FindPartFromMasterPartListByPartNumberDataSet();
        FindPartFromMasterPartListByJDEPartNumberDataSet TheFindPartFromMasterPartLitByJDEPartNumberDataSet = new FindPartFromMasterPartListByJDEPartNumberDataSet();

        public AddPart()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            string strPartNumber;
            string strJDEPartNumber;
            string strDescription;
            string strCharterPart;
            double douPrice = 0;
            int intRecordsReturned;

            try
            {
                //beginning data validation
                strPartNumber = txtPartNumber.Text;
                if (strPartNumber == "")
                {
                    strErrorMessage += "The Part Number Was Not Entered\n";
                    blnFatalError = true;
                }
                strJDEPartNumber = txtJDEPartNumber.Text;
                if (strJDEPartNumber == "")
                {
                    strErrorMessage += "The JDE Part Number Was Not Entered\n";
                    blnFatalError = true;
                }
                strDescription = txtDescription.Text;
                if (strDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Description Was Not Entered\n";
                }
                strCharterPart = txtCharter.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyYesNoData(strCharterPart);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Charter Part in not a Yes or No\n";
                }
                strValueForValidation = txtPrice.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Price is not a Valid Format\n";
                }
                else
                {
                    douPrice = Convert.ToDouble(strValueForValidation);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //checking to see if the part number exists
                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Part Number Exists");
                    return;
                }

                if (strJDEPartNumber != "NOT REQUIRED")
                {
                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strJDEPartNumber);

                    if (intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("JDE Part Number Exists");
                        return;
                    }
                }

                //searching the master parts list
                TheFindPartFromMasterPartListByPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Part Number Exists with the Master Part List Table");
                    return;
                }

                if (strJDEPartNumber != "NOT REQUIRED")
                {
                    TheFindPartFromMasterPartLitByJDEPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByJDEPartNumber(strJDEPartNumber);

                    if (intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("JDE Part Number Exists within the Master Part List");
                        return;
                    }
                }

                //saving the record
                blnFatalError = ThePartNumberClass.InsertPartIntoPartNumbers(-1, strPartNumber, strJDEPartNumber, strDescription, float.Parse(txtPrice.Text));

                if (blnFatalError == true)
                    throw new Exception();

                ClearControls();
                SetControlsReadOnly(true);

            }
           
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Part // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitAdd_Click(object sender, RoutedEventArgs e)
        {
            //setting controls up
            txtJDEPartNumber.Text = "NOT REQUIRED";
            txtCharter.Text = "NO";
            SetControlsReadOnly(false);
            mitSave.IsEnabled = true;
            txtPrice.Text = "0.00";
            txtPartNumber.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load during the event load
            try
            {
                SetControlsReadOnly(true);

                mitSave.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Parts // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtCharter.IsReadOnly = blnValueBoolean;
            txtDescription.IsReadOnly = blnValueBoolean;
            txtJDEPartNumber.IsReadOnly = blnValueBoolean;
            txtPartNumber.IsReadOnly = blnValueBoolean;
            txtPrice.IsReadOnly = blnValueBoolean;
        }
        private void ClearControls()
        {
            txtCharter.Text = "";
            txtDescription.Text = "";
            txtJDEPartNumber.Text = "";
            txtPartNumber.Text = "";
            txtPrice.Text = "";
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
            TheMessagesClass.AddTask();
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
