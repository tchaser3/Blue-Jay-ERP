/* Title:           Add Vehicle Bulk Tools
 * Date:            5-29-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the way to add bulk tools to a vehicle */

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
using ToolCategoryDLL;
using VehicleBulkToolsDLL;
using NewEventLogDLL;
using DataValidationDLL;
using VehicleMainDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddVehicleBulkTools.xaml
    /// </summary>
    public partial class AddVehicleBulkTools : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();

        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindVehicleBulkToolDataSet TheFindVehicleBulkToolDataSet = new FindVehicleBulkToolDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();

        public AddVehicleBulkTools()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ClearVehicleControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //loading the combo box
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count - 1;
                cboSelectToolCategory.Items.Clear();
                cboSelectToolCategory.Items.Add("Select Tool Category");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectToolCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboSelectToolCategory.SelectedIndex = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle Bulk Tools // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectToolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectToolCategory.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                    MainWindow.gstrToolCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;
                }
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle Bulk Tools // Changing Combo Box Index " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            string strVehicleNumber;
            int intQuantity = 0;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            int intRecordsReturned;
            string strErrorMessage = "";

            try
            {
                //data validation
                strVehicleNumber = txtVehicleNumber.Text;
                if (strVehicleNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage = "Vehicle Number Was Not Addd\n";
                }
                else
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Vehicle Number Entered was not Found\n";
                    }
                    else
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                }
                if(cboSelectToolCategory.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Tool Category Was Not Selected\n";
                }
                strValueForValidation = txtQuantity.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Quantity is not an Integer\n";
                }
                else
                {
                    intQuantity = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //checking to see if the tool has been put in
                TheFindVehicleBulkToolDataSet = TheVehicleBulkToolsClass.FindVehicleBulkTool(strVehicleNumber, MainWindow.gstrToolCategory);

                intRecordsReturned = TheFindVehicleBulkToolDataSet.FindVehicleBulkTool.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The Vehicle Has This Tool Assgined Already, Please\nAdjust The Count on Edit Vehicle Bulk Tool");

                    return;
                }

                blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(MainWindow.gintVehicleID, MainWindow.gintCategoryID, intQuantity);

                if (blnFatalError == true)
                    throw new Exception();

                ClearVehicleControls();

                TheMessagesClass.InformationMessage("The Tool Has Been Entered");

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle Bulk Tools // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ClearVehicleControls()
        {
            txtQuantity.Text = "";
            txtVehicleNumber.Text = "";
            cboSelectToolCategory.SelectedIndex = 0;
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
