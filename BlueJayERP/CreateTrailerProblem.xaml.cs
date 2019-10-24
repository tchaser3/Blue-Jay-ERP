/* Title:           Create Trailer Problem
 * Date:            9-25-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create a trailer problem */

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
using TrailersDLL;
using TrailerProblemDLL;
using TrailerProblemUpdateDLL;
using NewEventLogDLL;
using DataValidationDLL;
using VendorsDLL;
using NewEmployeeDLL;
using TrailerProblemDocumentationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateTrailerProblem.xaml
    /// </summary>
    public partial class CreateTrailerProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        TrailerProblemUpdateClass TheTrailerProblemUpdateClass = new TrailerProblemUpdateClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        TrailerProblemDocumentationClass TheTrailerProblemDocumentationClass = new TrailerProblemDocumentationClass();

        //setting up the data sets
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorsNameDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindTrailerProblemByDateMatchDataSet TheFindTrailerProblemByDateMatchDataSet = new FindTrailerProblemByDateMatchDataSet();

        public CreateTrailerProblem()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            //setting up the combo box
            cboSelectVendor.Items.Clear();
            cboSelectVendor.Items.Add("Select Vendor");

            TheFindVendorsSortedByVendorsNameDataSet = TheVendorsClass.FindVendorsSortedByVendorName();

            intNumberOfRecords = TheFindVendorsSortedByVendorsNameDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectVendor.Items.Add(TheFindVendorsSortedByVendorsNameDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboSelectVendor.SelectedIndex = 0;

            txtEnterTrailerNumber.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtLicensePlate.Text = "";
            txtProblemReported.Text = "";
            txtTrailerDescription.Text = "";
            txtTrailerLocation.Text = "";
            txtTrailerNotes.Text = "";

            btnCheckOpenProblems.IsEnabled = false;
            btnProcess.IsEnabled = false;
            btnAddDocuments.IsEnabled = false;
            cboSelectVendor.IsEnabled = false;
        }

        private void TxtEnterTrailerNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strTrailerNumber;
            int intLength;
            int intRecordsReturned;

            try
            {
                strTrailerNumber = txtEnterTrailerNumber.Text;
                intLength = strTrailerNumber.Length;

                if(intLength == 4)
                {
                    TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strTrailerNumber);

                    intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        LoadTrailerInformation();
                    }
                }
                else if(intLength == 6)
                {
                    TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strTrailerNumber);

                    intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Trailer Was Not Found");
                        return;
                    }
                }

                cboSelectVendor.IsEnabled = true;
                btnCheckOpenProblems.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create Trailer Problem // Enter Trailer Number Textbox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadTrailerInformation()
        {
            int intOfficeID;

            try
            {
                txtFirstName.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].FirstName;
                txtLastName.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].LastName;
                txtLicensePlate.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].LicensePlate;
                intOfficeID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].LocationID;
                MainWindow.gintTrailerID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerID;

                //getting the location
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intOfficeID);

                txtTrailerLocation.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].FirstName;
                txtTrailerDescription.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerDescription;
                txtTrailerNotes.Text = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerNotes;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Create Trailer Problem // Load Trailer Information " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void BtnCheckOpenProblems_Click(object sender, RoutedEventArgs e)
        {
            OpenTrailerProblemsByTrailerID OpenTrailerProblemsByTrailerID = new OpenTrailerProblemsByTrailerID();
            OpenTrailerProblemsByTrailerID.ShowDialog();
        }

        private void CboSelectVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectVendor.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintVendorID = TheFindVendorsSortedByVendorsNameDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;

                btnProcess.IsEnabled = true;
            }
            else
            {
                btnProcess.IsEnabled = false;
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            DateTime datTransactionDate = DateTime.Now;
            string strProblemReported;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intEmployeeID;

            try
            {
                if(cboSelectVendor.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Vendor was not Selected\n";
                }
                strProblemReported = txtProblemReported.Text;
                if(strProblemReported.Length < 20)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Reported is not Long Enough\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                blnFatalError = TheTrailerProblemClass.InsertTrailerProblem(MainWindow.gintTrailerID, datTransactionDate, intEmployeeID, strProblemReported, MainWindow.gintVendorID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindTrailerProblemByDateMatchDataSet = TheTrailerProblemClass.FindTrailerProblemByDateMatch(datTransactionDate);

                MainWindow.gintProblemID = TheFindTrailerProblemByDateMatchDataSet.FindTrailerProblemByDateMatch[0].ProblemID;

                blnFatalError = TheTrailerProblemUpdateClass.InsertTrailerProblemUpdate(MainWindow.gintProblemID, intEmployeeID, strProblemReported);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Problem Has Been Create");

                btnAddDocuments.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Created Trailer Problem // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void BtnAddDocuments_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datTransactionDate = DateTime.Now;
            string strDocumentPath = "";
            string strDocumentType;
            bool blnRightFormat = false;
            long intResult;
            string strNewLocation = "";
            string strTransactionName = "";
            bool blnFatalError;
            string strVehicleNumber;
            string strProblem;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name                

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();
                

                // Process open file dialog box results
                if (result == true)
                {
                    

                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            // Open document
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();
                            strTransactionName = "";

                            strDocumentType = System.IO.Path.GetExtension(dlg.FileNames[intCounter]).ToUpper();

                            datTransactionDate = DateTime.Now;

                            intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second +intCounter;
                            strTransactionName += Convert.ToString(intResult);

                            strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\VehicleProblemFiles\\Trailer" + strTransactionName + strDocumentType;

                            System.IO.File.Copy(strDocumentPath, strNewLocation);

                            blnFatalError = TheTrailerProblemDocumentationClass.InsertTrailerProblemDocumentation(MainWindow.gintTrailerID, MainWindow.gintProblemID, strNewLocation);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }
                
                

                TheMessagesClass.InformationMessage("Documents Have Been Added");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Add Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
