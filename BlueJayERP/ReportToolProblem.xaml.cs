/* Title:           Report Tool Problem
 * Date:            7-19-18
 * Author:          Terry Holmes
 * 
 * Description:     This the window to report tools */

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
using ToolProblemDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ReportToolProblem.xaml
    /// </summary>
    public partial class ReportToolProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolClass = new ToolsClass();
        ToolProblemClass TheToolProblemClass = new ToolProblemClass();

        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindToolProblemByDateMatchDataSet TheFindToolProblemByDateMatchDataSet = new FindToolProblemByDateMatchDataSet();
        ImportDocumentsDataSet TheImportDocumentsDataSet = new ImportDocumentsDataSet();

        //setting global variables
        string gstrDocumentType;
        bool gblnIsRepairable;
        bool gblnIsClosed;
        bool gblnDeactivateTool;
        int gintTransactionID;
        string gstrToolID;
        string gstrWarehouseStatement;

        public ReportToolProblem()
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
            TheMessagesClass.CloseTheProgram();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetContols();
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitProcessTool_Click(object sender, RoutedEventArgs e)
        {
            DateTime datTransactionDate;
            DateTime datRecordDate;
            long intResult;
            string strTransactionName;
            string strNewLocation = "";
            int intCounter;
            int intNumberOfRecords;
            string strDocumentType;
            string strDocumentPath;
            bool blnFatalError = false;

            try
            {
                blnFatalError = PerformDataValidation();

                if(dgrDocument.Items.Count == 0)
                {
                    TheMessagesClass.ErrorMessage("There Are No Documents Assigned to the Problem");
                    return;
                }

                //loading the problem
                datRecordDate = DateTime.Now;

                blnFatalError = TheToolProblemClass.InsertToolProblem(MainWindow.gintToolKey, datRecordDate, MainWindow.gintEmployeeID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, gstrWarehouseStatement, gblnIsRepairable, gblnIsClosed);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindToolProblemByDateMatchDataSet = TheToolProblemClass.FindToolProblemByDateMatch(datRecordDate);

                MainWindow.gintProblemID = TheFindToolProblemByDateMatchDataSet.FindToolProblemByDateMatch[0].ProblemID;

                blnFatalError = TheToolProblemClass.InsertToolProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, gstrWarehouseStatement);

                if (blnFatalError == true)
                    throw new Exception();

                intNumberOfRecords = TheImportDocumentsDataSet.documents.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strDocumentType = TheImportDocumentsDataSet.documents[intCounter].DocumentType.ToUpper();
                    strDocumentPath = TheImportDocumentsDataSet.documents[intCounter].DocumentPath;

                    if(strDocumentType == "DOCUMENT")
                    {
                        datTransactionDate = DateTime.Now;

                        intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                        strTransactionName = Convert.ToString(intResult) + Convert.ToString(intCounter);

                        strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\ToolProblemFiles\\" + strTransactionName + ".pdf";
                    }
                    if (strDocumentType == "PICTURE")
                    {
                        datTransactionDate = DateTime.Now;

                        intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                        strTransactionName = Convert.ToString(intResult) + Convert.ToString(intCounter);

                        strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\ToolProblemFiles\\" + strTransactionName + ".jpg";
                    }

                    System.IO.File.Copy(strDocumentPath, strNewLocation);

                    blnFatalError = TheToolProblemClass.InsertToolProblemDocument(MainWindow.gintProblemID, strDocumentType, strNewLocation);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                if(gblnDeactivateTool == true)
                {
                    blnFatalError = TheToolClass.UpdateToolActive(MainWindow.gintToolKey, false);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Tool Problem Has Been Reported");

                ResetContols();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Process Tool " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting the local varialbes
            int intRecordsReturned;

            gstrToolID = txtEnterToolID.Text;

            //getting the tool information
            TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(gstrToolID);

            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

            TheImportDocumentsDataSet.documents.Rows.Clear();

            dgrDocument.ItemsSource = TheImportDocumentsDataSet.documents;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("The Tool Was Not Found");
                return;
            }
            else if(intRecordsReturned > 0)
            {
                txtCost.Text = Convert.ToString(TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCost);
                txtToolDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
            }

        }
        private void ResetContols()
        {
            txtCost.Text = "";
            txtEnterToolID.Text = "";
            txtToolDescription.Text = "";
            cboSelectEmployee.Items.Clear();
            mitProcessTool.IsEnabled = false;
            mitRemoveDocument.IsEnabled = false;
            cboRepairable.SelectedIndex = 0;
            cboClosed.SelectedIndex = 0;
            cboDocumentType.SelectedIndex = 0;
            TheImportDocumentsDataSet.documents.Rows.Clear();
            dgrDocument.ItemsSource = TheImportDocumentsDataSet.documents;
            txtLastName.Text = "";
            txtWarehouseStatement.Text = "";
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting varialbes
            int intCounter;
            int intNumberOfRecords;
            string strLastName;
            int intLength;

            try
            {
                strLastName = txtLastName.Text;

                intLength = strLastName.Length;

                if (intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
               
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Last Name Text Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboClosed.Items.Add("Select");
            cboClosed.Items.Add("Yes");
            cboClosed.Items.Add("No");

            cboRepairable.Items.Add("Select");
            cboRepairable.Items.Add("Yes");
            cboRepairable.Items.Add("No");

            cboDocumentType.Items.Add("Select");
            cboDocumentType.Items.Add("Document");
            cboDocumentType.Items.Add("Picture");

            ResetContols();

        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problems // Combo Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError;
            string strDocumentPath = "";
            string strDocumentType;
            bool blnRightFormat = false;

            try
            {
                blnFatalError = PerformDataValidation();

                if (blnFatalError == true)
                {
                    return;
                }

                strDocumentType = cboDocumentType.SelectedItem.ToString();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    strDocumentPath = dlg.FileName.ToUpper();
                }
                else
                {
                    return;
                }

                if(strDocumentPath.Contains("JPG"))
                {
                    if(gstrDocumentType == "PICTURE")
                    {
                        blnRightFormat = true;
                    }
                }
                else if(strDocumentPath.Contains("PDF"))
                {
                    if (gstrDocumentType == "DOCUMENT")
                    {
                        blnRightFormat = true;
                    }
                }

                if(blnRightFormat == false)
                {
                    TheMessagesClass.ErrorMessage("File Type is not Correct");

                    return;
                }

                ImportDocumentsDataSet.documentsRow NewDocumentRow = TheImportDocumentsDataSet.documents.NewdocumentsRow();

                NewDocumentRow.DocumentPath = strDocumentPath;
                NewDocumentRow.DocumentType = strDocumentType;

                TheImportDocumentsDataSet.documents.Rows.Add(NewDocumentRow);

                dgrDocument.ItemsSource = TheImportDocumentsDataSet.documents;

                mitProcessTool.IsEnabled = true;
                mitRemoveDocument.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Add Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private bool PerformDataValidation()
        {
            bool blnFatalError = false;
            string strToolID = "";
            int intSelectedIndex = 0;
            string strErrorMessage = "";

            strToolID = txtEnterToolID.Text;
            if(strToolID == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Tool ID Has Not Been Entered\n";
            }
            intSelectedIndex = cboSelectEmployee.SelectedIndex;
            if(intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Employee Was Not Selected\n";
            }
            gstrWarehouseStatement = txtWarehouseStatement.Text;
            if(gstrWarehouseStatement == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Warehouse Statement Has Not Been Added\n";
            }
            intSelectedIndex = cboRepairable.SelectedIndex;
            if(intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Repairable Was Not Selected\n";
            }
            intSelectedIndex = cboClosed.SelectedIndex;
            if(intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Closed Was not Selected\n";
            }
            intSelectedIndex = cboDocumentType.SelectedIndex;
            if(intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Document Type Was Not Selected\n";
            }
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
            }

            return blnFatalError;
        }

        private void cboRepairable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRepairable.SelectedIndex == 1)
                gblnIsRepairable = true;
            else if (cboRepairable.SelectedIndex == 2)
            {
                gblnIsRepairable = false;

                MessageBoxResult result = MessageBox.Show("Is Tool Being Retired", "Thank You", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    gblnDeactivateTool = true;
                }
                else
                {
                    gblnDeactivateTool = false;
                }
            }
                
        }

        private void cboClosed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboClosed.SelectedIndex == 1)
                gblnIsClosed = true;
            else if (cboClosed.SelectedIndex == 2)
                gblnIsClosed = false;
        }

        private void mitRemoveDocument_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            //gblnRecordDeleted = true;

            try
            {
                dgrDocument.SelectedIndex = -1;

                intNumberOfRecords = TheImportDocumentsDataSet.documents.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (TheImportDocumentsDataSet.documents[intCounter].TransactionID == gintTransactionID)
                    {
                        TheImportDocumentsDataSet.documents[intCounter].Delete();
                        intCounter -= 1;
                        intNumberOfRecords -= 1;
                        dgrDocument.SelectedIndex = -1;

                    }
                }

                dgrDocument.ItemsSource = TheImportDocumentsDataSet.documents;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Menu Item Remove Document " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrDocument.SelectedIndex;

                if (intSelectedIndex > -1)
                {
                    
                    //setting local variable
                    dataGrid = dgrDocument;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strTransactionID = ((TextBlock)TransactionID.Content).Text;
                
                    //find the record
                    gintTransactionID = Convert.ToInt32(strTransactionID);
                   
                }


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboDocumentType.SelectedIndex > 0)
                gstrDocumentType = cboDocumentType.SelectedItem.ToString().ToUpper();
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
