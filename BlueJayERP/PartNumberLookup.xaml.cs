/* Title:           Part Number Lookup
 * Date:            10-24-20192
 * Author:          Terry Holmes
 * 
 * Description:     This is used to find information about a part number */

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
using DataValidationDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for PartNumberLookup.xaml
    /// </summary>
    public partial class PartNumberLookup : Window
    {
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting the data set
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByPartIDDataSet TheFindPartByPartIDataSet = new FindPartByPartIDDataSet();


        public PartNumberLookup()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterPartNumber.Text = "";
            txtJDEPartNumber.Text = "";
            txtPartDescription.Text = "";
            txtPartID.Text = "";
            txtPartNumber.Text = "";
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //this will load up the controls and search for the part number
            string strPartNumber;
            int intRecordsReturned;
            bool blnFatalError = false;
            int intPartID = 0;
            bool blnItemFound = false;

            try
            {
                strPartNumber = txtEnterPartNumber.Text;
                if(strPartNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Part Number Not Entered");
                    return;
                }

                blnFatalError = TheDataValidationClass.VerifyIntegerData(strPartNumber);

                if(blnFatalError == false)
                {
                    intPartID = Convert.ToInt32(strPartNumber);

                    TheFindPartByPartIDataSet = ThePartNumberClass.FindPartByPartID(intPartID);

                    intRecordsReturned = TheFindPartByPartIDataSet.FindPartByPartID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnItemFound = true;

                        //loading the controls
                        txtJDEPartNumber.Text = TheFindPartByPartIDataSet.FindPartByPartID[0].JDEPartNumber;
                        txtPartDescription.Text = TheFindPartByPartIDataSet.FindPartByPartID[0].PartDescription;
                        txtPartID.Text = Convert.ToString(intPartID);
                        txtPartNumber.Text = TheFindPartByPartIDataSet.FindPartByPartID[0].PartNumber;
                        blnItemFound = true;
                    }
                }
                
                if(blnItemFound == false)
                {
                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        txtJDEPartNumber.Text = strPartNumber;
                        txtPartDescription.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription;
                        txtPartID.Text = Convert.ToString(TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID);
                        txtPartNumber.Text = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartNumber;
                    }
                    else
                    {
                        TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                        intRecordsReturned = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            txtJDEPartNumber.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].JDEPartNumber;
                            txtPartDescription.Text = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;
                            txtPartID.Text = Convert.ToString(TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID);
                            txtPartNumber.Text = strPartNumber;
                        }
                        else
                        {
                            TheMessagesClass.ErrorMessage("Part Number Not Found");
                            return;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Part Number Lookup // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
