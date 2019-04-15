/* Title:           Edit Parts
 * Date:            3-20-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window for editing a part */

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
    /// Interaction logic for EditParts.xaml
    /// </summary>
    public partial class EditParts : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();

        //setting up data
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();

        public EditParts()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
            Visibility = Visibility.Hidden;
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strPartNumber;
            int intPartID;
            string strDescription;
            string strJDEPartNumber;
            double douPrice = 0;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intRecordCounter;
            bool blnRecordMatch;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //data validation
                strPartNumber = txtPartNumber.Text;
                intPartID = Convert.ToInt32(txtPartID.Text);
                if (strPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Number Was Not Entered\n";
                }
                strJDEPartNumber = txtJDEPartNumber.Text;
                if (strJDEPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "JDE Part Number Was Not Entered\n";
                }
                strDescription = txtDescription.Text;
                if (strDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Description Was Not Entered\n";
                }
                strValueForValidation = txtPrice.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Price Entered in not Numeric\n";
                }
                else
                {
                    douPrice = Convert.ToDouble(strValueForValidation);
                }
                if (blnFatalError == true)
                {
                    PleaseWait.Close();
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //validation to see if the part number is used with another row
                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    blnRecordMatch = false;

                    for (intRecordCounter = 0; intRecordCounter <= intRecordsReturned - 1; intRecordCounter++)
                    {
                        if (strPartNumber == TheFindPartByPartNumberDataSet.FindPartByPartNumber[intRecordCounter].PartNumber)
                        {
                            if (intPartID != TheFindPartByPartNumberDataSet.FindPartByPartNumber[intRecordCounter].PartID)
                            {
                                blnRecordMatch = true;
                            }
                        }
                    }

                    if (blnRecordMatch == true)
                    {
                        TheMessagesClass.ErrorMessage("Part Number Exists with a Different Part ID");
                        return;
                    }

                }


                if ((strJDEPartNumber != "NOT REQUIRED") && (strJDEPartNumber != "NOT PROVIDED"))
                {
                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strJDEPartNumber);

                    intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnRecordMatch = false;

                        for (intRecordCounter = 0; intRecordCounter <= intRecordsReturned - 1; intRecordCounter++)
                        {
                            if (strJDEPartNumber == TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[intRecordCounter].JDEPartNumber)
                            {
                                if (intPartID != TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[intRecordCounter].PartID)
                                {
                                    blnRecordMatch = true;
                                }
                            }
                        }

                        if (blnRecordMatch == true)
                        {
                            TheMessagesClass.ErrorMessage("JDE Part Number Exists with a Different Part ID");
                            return;
                        }

                    }
                }

                //doing the loop
                blnFatalError = ThePartNumberClass.UpdatePartInformation(intPartID, strJDEPartNumber, strDescription, true, float.Parse(txtPrice.Text));

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Part Has Been Updated");

                ClearControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Parts // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load during the form loading
            txtEnterPartNumber.Focus();

            mitSave.IsEnabled = false;
        }
        private void ClearControls()
        {
            txtDescription.Text = "";
            txtEnterPartNumber.Text = "";
            txtJDEPartNumber.Text = "";
            txtPartID.Text = "";
            txtPartNumber.Text = "";
            txtPrice.Text = "";
        }

        private void btnFindPartNumber_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strPartNumber;
            int intRecordsReturned;

            try
            {
                //data validation
                strPartNumber = txtEnterPartNumber.Text;
                if (strPartNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Part Number Not Entered");
                    return;
                }

                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheFindPartByJDEPartNumberDataSet= ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.InformationMessage("Part Number Was Not Found");

                        ClearControls();
                    }
                    else if (intRecordsReturned > 0)
                    {
                        txtDescription.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription;
                        txtJDEPartNumber.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].JDEPartNumber;
                        txtPartID.Text = Convert.ToString(TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID);
                        txtPartNumber.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartNumber;
                        txtPrice.Text = Convert.ToString(TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].Price);
                        mitSave.IsEnabled = true;
                    }
                }
                else if (intRecordsReturned > 0)
                {
                    txtDescription.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;
                    txtJDEPartNumber.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].JDEPartNumber;
                    txtPartID.Text = Convert.ToString(TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID);
                    txtPartNumber.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartNumber;
                    txtPrice.Text = Convert.ToString(TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].Price);
                    mitSave.IsEnabled = true;
                }


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Parts // Find Part Number Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
