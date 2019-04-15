/* Title:           Edit Tools
 * Date:            6-20-18
 * Author:          Terry Holmes
 * 
 * Description:     This how to edit a tool */

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
using NewToolsDLL;
using ToolCategoryDLL;
using DataValidationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditTools.xaml
    /// </summary>
    public partial class EditTools : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindActiveToolByToolIDDataSet TheFindToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        FindToolCategoryByCategoryIDDataSet TheFindToolCategoryByCategoryIDDataSet = new FindToolCategoryByCategoryIDDataSet();

        int gintWarehouseID;

        public EditTools()
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
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the combo box
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                cboWarehouse.Items.Add("Select Warehouse");

                for(intCounter =0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboWarehouse.SelectedIndex = 0;
                cboWarehouse.IsEnabled = false;
                mitRetireTool.IsEnabled = false;
                mitSaveChanges.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ResetControls()
        {
            cboWarehouse.SelectedIndex = 0;
            cboWarehouse.IsEnabled = false;
            txtEnterToolID.Text = "";
            txtPartCost.Text = "";
            txtPartDescription.Text = "";
            txtPartNumber.Text = "";
            txtToolCategory.Text = "";
            mitRetireTool.IsEnabled = false;
            mitSaveChanges.IsEnabled = false;
            txtToolNotes.Text = "";
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //this will find the tool
            string strToolID;
            int intRecordsReturned;
            int intWarehouseID;

            try
            {
                strToolID = txtEnterToolID.Text;
                if (strToolID == "")
                {
                    TheMessagesClass.ErrorMessage("Tool ID Not Entered");
                    return;
                }

                TheFindToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                intRecordsReturned = TheFindToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("No Tools Found");
                    return;
                }

                txtPartCost.Text = Convert.ToString(TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCost);
                txtPartDescription.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                txtPartNumber.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].PartNumber;
                txtToolCategory.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCategory;
                txtToolNotes.Text = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolNotes;
                MainWindow.gintToolKey = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;

                intWarehouseID = TheFindToolByToolIDDataSet.FindActiveToolByToolID[0].CurrentLocation;

                cboWarehouse.IsEnabled = true;
                mitRetireTool.IsEnabled = true;
                mitSaveChanges.IsEnabled = true;

                FindWarehouse(intWarehouseID);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void FindWarehouse(int intWarehouseID)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                if(cboWarehouse.SelectedIndex > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (intWarehouseID == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                        {
                            intSelectedIndex = intCounter + 1;
                        }
                    }

                    cboWarehouse.SelectedIndex = intSelectedIndex;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Find Warehouses " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboWarehouse.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
                
        }

        private void mitRetireTool_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                const string message = "Are you sure that you would like to Retire This Tool?";
                const string caption = "Are You Sure";
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    blnFatalError = TheToolsClass.UpdateToolActive(MainWindow.gintToolKey, false);

                    if (blnFatalError == true)
                        throw new Exception();

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Retire Tool Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strPartNumber;
            string strPartDescription;
            string strNotes;
            string strValueForValidation;
            decimal decToolCost = 0;

            try
            {
                strPartNumber = txtPartNumber.Text;
                if(strPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Number Was Not Entered\n";
                }
                strPartDescription = txtPartDescription.Text;
                strValueForValidation = txtPartCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Cost is not Numeric\n";
                }
                else
                {
                    decToolCost = Convert.ToDecimal(strValueForValidation);
                }
                if(strPartDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Part Description Was Not Entered\n";
                }
                strNotes = txtToolNotes.Text;
                if(strNotes == "")
                {
                    strNotes = "NO NOTES ENTERED";
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheToolsClass.UpdateToolInfo(MainWindow.gintToolKey, strPartNumber, strPartDescription, gintWarehouseID, decToolCost, strNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Tool Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Tools // Save Changes Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

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
