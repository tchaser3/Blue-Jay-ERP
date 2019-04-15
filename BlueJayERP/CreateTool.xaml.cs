/* Title:           Create Tool
 * Date:            6-12-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create tools */

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
using NewToolsDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;
using ToolCategoryDLL;
using ToolIDDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateTool.xaml
    /// </summary>
    public partial class CreateTool : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolsClass TheToolsClass = new ToolsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();
        ToolIDClass TheToolIDClass = new ToolIDClass();

        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();
        FindToolIDByCategoryDataSet TheFindToolIDByCategoryDataSet = new FindToolIDByCategoryDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIdDataSet = new FindActiveToolByToolIDDataSet();

        int gintCategoryID;
        int gintWarehouseID;
        int gintTransactionID;
        bool gblnNewToolCategory;

        public CreateTool()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                cboSelectWarehouse.Items.Add("Select Warehouse");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;

                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count - 1;

                cboToolCategory.Items.Add("Select Tool Category");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboToolCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
                }

                cboToolCategory.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create Tool // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboToolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strToolCategory;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = cboToolCategory.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintCategoryID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].CategoryID;
                    strToolCategory = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory;

                    TheFindToolIDByCategoryDataSet = TheToolIDClass.FindToolIDByCategory(strToolCategory);

                    intRecordsReturned = TheFindToolIDByCategoryDataSet.FindToolIDByCategory.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        txtToolID.Text = TheFindToolIDByCategoryDataSet.FindToolIDByCategory[0].ToolID;
                        gintTransactionID = TheFindToolIDByCategoryDataSet.FindToolIDByCategory[0].TransactionID;
                        gblnNewToolCategory = false;
                    }
                    else
                    {
                        txtToolID.Text = "None Found";
                        TheMessagesClass.InformationMessage("There is not a Tool ID for this Tool in the Database, Please Look at Old Tool Spreadsheet");
                        gblnNewToolCategory = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create Tool // Tool Category Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }

        }

        private void mitCreateTool_Click(object sender, RoutedEventArgs e)
        {
            //creating local variables
            int intSelectedIndex;
            string strValueForValidation;
            string strPartNumber;
            string strDescription;
            string strToolID;
            decimal decToolCost = 0;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                //data validation
                intSelectedIndex = cboSelectWarehouse.SelectedIndex;
                if(intSelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Warehouse Was Not Selected\n";
                }
                intSelectedIndex = cboToolCategory.SelectedIndex;
                if (intSelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Category Was Not Selected\n";
                }
                strToolID = txtToolID.Text;
                if(strToolID == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Tool ID Was Not Entered\n";
                }
                else
                {
                    TheFindActiveToolByToolIdDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                    intRecordsReturned = TheFindActiveToolByToolIdDataSet.FindActiveToolByToolID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "There is Already an Active Tool With This ID\n";
                    }
                }
                strPartNumber = txtPartNumber.Text;
                if(strPartNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Part Number Was Not Added\n";
                }
                strDescription = txtDescription.Text;
                if(strDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Description Was Not Added\n";
                }
                strValueForValidation = txtPartCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tool Cost is not Numeric\n";
                }
                else
                {
                    decToolCost = Convert.ToDecimal(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnNewToolCategory == true)
                {
                    blnFatalError = TheToolIDClass.InsertNewToolIDForToolType(gintCategoryID, strToolID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(gblnNewToolCategory == false)
                {
                    blnFatalError = TheToolIDClass.UpdateToolID(gintTransactionID, strToolID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheToolsClass.InsertTools(strToolID, gintWarehouseID, strPartNumber, gintCategoryID, strDescription, decToolCost, gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                ResetControls();

                TheMessagesClass.InformationMessage("The Tool Has Been Created");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create Tool // Create Tool Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ResetControls()
        {
            cboSelectWarehouse.SelectedIndex = 0;
            cboToolCategory.SelectedIndex = 0;
            txtDescription.Text = "";
            txtPartCost.Text = "";
            txtPartNumber.Text = "";
            txtToolID.Text = "";
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
