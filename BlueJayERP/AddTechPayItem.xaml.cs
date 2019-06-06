/* Title:           Add Tech Pay Item
 * Date:            5-21-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add an item */

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
using TechPayDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddTechPayItem.xaml
    /// </summary>
    public partial class AddTechPayItem : Window
    {
        //setting local classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TechPayClass TheTechPayClass = new TechPayClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindTechPayItemByCodeDataSet TheFindTechPayItemByCodeDataSet = new FindTechPayItemByCodeDataSet();
        FindTechPayItemByDescriptionDataSet TheFindTechPayItemByDescriptionDataSet = new FindTechPayItemByDescriptionDataSet();

        public AddTechPayItem()
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
            txtJobDescription.Text = "";
            txtTechPayCode.Text = "";
            txtTechPayPrice.Text = "";
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strTechPayCode;
            string strJobDescription;
            string strValueForValidation;
            string strErrorMessage = "";
            decimal decTechPayPrice = 0;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                //data validation
                strTechPayCode = txtTechPayCode.Text;
                if(strTechPayCode == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tech Pay Code Was Not Entered\n";
                }
                else
                {
                    TheFindTechPayItemByCodeDataSet = TheTechPayClass.FindTechPayItemByCode(strTechPayCode);

                    intRecordsReturned = TheFindTechPayItemByCodeDataSet.FindTechPayItemByCode.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Tech Pay Code Has Been Entered Already\n";
                    }
                }
                strJobDescription = txtJobDescription.Text;
                if(strJobDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Job Description was not Entered\n";
                }
                else
                {
                    TheFindTechPayItemByDescriptionDataSet = TheTechPayClass.FindTechPayItemByDescription(strJobDescription);

                    intRecordsReturned = TheFindTechPayItemByDescriptionDataSet.FindTechPayItemByDescription.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Job Description Has Been Entered Already\n";
                    }
                }
                strValueForValidation = txtTechPayPrice.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tech Pay Price is not Numeric\n";
                }
                else
                {
                    decTechPayPrice = Convert.ToDecimal(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheTechPayClass.InsertTechPayItem(strTechPayCode, strJobDescription, decTechPayPrice);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Tech Pay Item Has Been Entered");

                ResetControls();
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Tech Pay Item // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
