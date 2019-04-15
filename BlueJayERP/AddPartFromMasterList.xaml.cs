/* Title:           Add Part From Master List
 * Date:            3-20-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to add parts from the master list */

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
using NewPartNumbersDLL;
using NewEventLogDLL;
using InventoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddPartFromMasterList.xaml
    /// </summary>
    public partial class AddPartFromMasterList : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventlogClass = new EventLogClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        InventoryClass TheInventoryClass = new InventoryClass();

        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindPartFromMasterPartListByJDEPartNumberDataSet TheFindPartFromMasterPartListByJDEPartNumberDataSet = new FindPartFromMasterPartListByJDEPartNumberDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartFromMasterPartListByPartNumberDataSet TheFindPartFromMasterPartListByPartNumberDataSet = new FindPartFromMasterPartListByPartNumberDataSet();

        public AddPartFromMasterList()
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
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                //data validation
                strPartNumber = txtEnterPartNumber.Text;
                if (strPartNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Part Number Was Not Entered");
                    return;
                }

                //checking to see if the part number exists within the part table
                //checking JDE Part Number
                TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The Part Currently Exists Within The Part Number Table");
                    return;
                }

                //checking part number
                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The Part Currently Exists Within The Part Number Table");
                    return;
                }


                TheFindPartFromMasterPartListByJDEPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByJDEPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheFindPartFromMasterPartListByPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Part Number Entered is not in the Master Part List");
                        return;
                    }
                }

                blnFatalError = ThePartNumberClass.InsertPartIntoPartNumbers(Convert.ToInt32(txtPartID.Text), txtPartNumber.Text, txtJDEPartNumber.Text, txtDescription.Text, float.Parse(txtPrice.Text));

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Part Number Has Been Added");

                ClearControls();
                
            }
            catch (Exception Ex)
            {
                TheEventlogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Part From Master List // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
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
                    TheMessagesClass.ErrorMessage("Part Number Was Not Entered");
                    return;
                }

                //checking to see if the part number exists within the part table
                //checking JDE Part Number
               TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The Part Currently Exists Within The Part Number Table");
                    return;
                }

                //checking part number
                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The Part Currently Exists Within The Part Number Table");
                    return;
                }


                TheFindPartFromMasterPartListByJDEPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByJDEPartNumber(strPartNumber);

                intRecordsReturned = TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheFindPartFromMasterPartListByPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Part Number Entered is not in the Master Part List");
                        return;
                    }
                    else
                    {
                        txtDescription.Text = TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber[0].PartDescription;
                        txtJDEPartNumber.Text =TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber[0].JDEPartNumber;
                        txtPartID.Text = Convert.ToString(TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber[0].PartID);
                        txtPartNumber.Text = TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber[0].PartNumber;
                        txtPrice.Text = Convert.ToString(TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber[0].Price);
                    }
                }
                else
                {
                    txtDescription.Text = TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber[0].PartDescription;
                    txtJDEPartNumber.Text = TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber[0].JDEPartNumber;
                    txtPartID.Text = Convert.ToString(TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber[0].PartID);
                    txtPartNumber.Text = TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber[0].PartNumber;
                    txtPrice.Text = Convert.ToString(TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber[0].Price);
                }
            }
            catch (Exception Ex)
            {
                TheEventlogClass.InsertEventLogEntry(DateTime.Now, "Add Parts Part From Master List Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
