/* Title:           Invoice Vehicle Problems
 * Date:            12-19-18
 * Author:          Terry Holmes
 * 
 * Description:     This where a vehicle will get invoiced */

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
using VehicleMainDLL;
using VehicleProblemDocumentationDLL;
using VehicleProblemsDLL;
using DataValidationDLL;
using VendorsDLL;
using EmployeeDateEntryDLL;
using VehiclesInShopDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for InvoiceVehicleProblems.xaml
    /// </summary>
    public partial class InvoiceVehicleProblems : Window
    {
        //setting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        VehicleProblemDocumentClass TheVehiclePRoblemDocumentationClass = new VehicleProblemDocumentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VendorsClass TheVendorsClass = new VendorsClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();

        //setting up the data
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorNameDataSet = new FindVendorsSortedByVendorNameDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehicleMainProblemReadyForInvoicingDataSet TheFindVehicleMainProblemReadyForInvoicingDataSet = new FindVehicleMainProblemReadyForInvoicingDataSet();
        OpenVehicleProblemsDataSet TheOpenVehicleProblemsDataSet = new OpenVehicleProblemsDataSet();
        FindVehicleInvoiceByInvoiceNumberDataSet TheFindVehicleInvoiceByInvoiceNumberDataSet = new FindVehicleInvoiceByInvoiceNumberDataSet();
        FindVehicleMainInShopByVehicleIDDataSet TheFindVehicleMainInShopByVehicleIDDataSet = new FindVehicleMainInShopByVehicleIDDataSet();

        //Setting global variables
        string gstrInvoicePath;
        bool gblnInvoiceAttached;

        public InvoiceVehicleProblems()
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
            int intCounter;
            int intNumberOfRecords;

            txtAmount.Text = "";
            txtInvoiceNumber.Text = "";
            txtVehicleNumber.Text = "";
            txtProblemResolution.Text = "";
            txtInvoicePath.Text = "";

            cboVendor.Items.Clear();
            cboVendor.Items.Add("Select Vendor");

            TheFindVendorsSortedByVendorNameDataSet = TheVendorsClass.FindVendorsSortedByVendorName();
            intNumberOfRecords = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboVendor.Items.Add(TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intCounter].VendorName);
            }

            cboVendor.SelectedIndex = 0;

            TheOpenVehicleProblemsDataSet.openvehicleproblem.Rows.Clear();

            dgrResults.ItemsSource = TheOpenVehicleProblemsDataSet.openvehicleproblem;

            cboAttachInvoice.Items.Clear();
            cboAttachInvoice.Items.Add("Select Answer");
            cboAttachInvoice.Items.Add("Yes");
            cboAttachInvoice.Items.Add("No");
            cboAttachInvoice.SelectedIndex = 0;

            cboAttachInvoice.IsEnabled = false;
            cboVendor.IsEnabled = false;
            mitProcessInvoice.IsEnabled = false;
        }
        private bool FindVehicle(string strVehicleNumber)
        {
            bool blnItemFound = false;
            int intRecordsReturned;

            TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

            intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

            if(intRecordsReturned > 0)
            {
                blnItemFound = true;
                MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
            }

            return blnItemFound;
        }
        private void TxtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strVehicleNumber;
            int intLength;
            bool blnItemFound = false;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;

            try
            {
                strVehicleNumber = txtVehicleNumber.Text;
                intLength = strVehicleNumber.Length;

                if (intLength == 4)
                {
                    blnItemFound = FindVehicle(strVehicleNumber);
                }
                else if (intLength == 6)
                {
                    blnItemFound = FindVehicle(strVehicleNumber);
                    if (blnItemFound == false)
                    {
                        TheMessagesClass.InformationMessage("Vehicle Number Not Found");
                        return;
                    }
                }

                if (blnItemFound == true)
                {
                    TheFindVehicleMainInShopByVehicleIDDataSet = TheVehiclesInShopClass.FindVehicleMainInShopByVehicleID(MainWindow.gintVehicleID);

                    intRecordsReturned = TheFindVehicleMainInShopByVehicleIDDataSet.FindVehicleMainInShopByVehicleID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("THE VEHICLE IS STILL IN THE SHOP");
                        ResetControls();
                        return;
                    }

                    TheOpenVehicleProblemsDataSet.openvehicleproblem.Rows.Clear();

                    TheFindVehicleMainProblemReadyForInvoicingDataSet = TheVehicleProblemClass.FindVehicleMainProblemReadyForInvoicing(MainWindow.gintVehicleID);

                    intNumberOfRecords = TheFindVehicleMainProblemReadyForInvoicingDataSet.FindVehicleMainProblemReadyForInvoicing.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            OpenVehicleProblemsDataSet.openvehicleproblemRow NewProblemRow = TheOpenVehicleProblemsDataSet.openvehicleproblem.NewopenvehicleproblemRow();

                            NewProblemRow.Problem = TheFindVehicleMainProblemReadyForInvoicingDataSet.FindVehicleMainProblemReadyForInvoicing[intCounter].Problem;
                            NewProblemRow.ProblemDate = TheFindVehicleMainProblemReadyForInvoicingDataSet.FindVehicleMainProblemReadyForInvoicing[intCounter].TransactionDAte;
                            NewProblemRow.ProblemID = TheFindVehicleMainProblemReadyForInvoicingDataSet.FindVehicleMainProblemReadyForInvoicing[intCounter].ProblemID;
                            NewProblemRow.Status = TheFindVehicleMainProblemReadyForInvoicingDataSet.FindVehicleMainProblemReadyForInvoicing[intCounter].ProblemStatus;
                            NewProblemRow.Vendor = TheFindVehicleMainProblemReadyForInvoicingDataSet.FindVehicleMainProblemReadyForInvoicing[intCounter].VendorName;
                            NewProblemRow.Selected = false;

                            TheOpenVehicleProblemsDataSet.openvehicleproblem.Rows.Add(NewProblemRow);
                        }
                    }

                    dgrResults.ItemsSource = TheOpenVehicleProblemsDataSet.openvehicleproblem;
                    cboVendor.IsEnabled = true;
                    cboAttachInvoice.IsEnabled = true;
                    mitProcessInvoice.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Invoice Vehicle Problems // Vehicle Number Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboVendor.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintVendorID = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName[intSelectedIndex].VendorID;
            }
        }

        private void CboAttachInvoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }

        private void MitProcessInvoice_Click(object sender, RoutedEventArgs e)
        {
            string strNewLocation;
            bool blnFatalError = false;
            DateTime datTransactionDate;
            long intResult;
            string strTransactionName = "";
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            int intCounter;
            int intNumberOfRecords;
            bool blnItemSelected = false;
            string strInvoiceNumber;
            string strValueForValidation;
            decimal decInvoiceAmount = 0;
            int intInvoiceID;
            int intProblemID;
            string strProblemResolution;

            try
            {
                strInvoiceNumber = txtInvoiceNumber.Text;
                if(strInvoiceNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Invoice Number is Not Entered\n";
                }
                if(cboVendor.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vendor not Selected\n";
                }
                strValueForValidation = txtAmount.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Invoice Amount was not Entered\n";
                }
                else
                {
                    decInvoiceAmount = Convert.ToDecimal(strValueForValidation);
                }
                if(cboAttachInvoice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Attaching an Invoice was not Selected\n";
                }
                intNumberOfRecords = TheOpenVehicleProblemsDataSet.openvehicleproblem.Rows.Count - 1;
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheOpenVehicleProblemsDataSet.openvehicleproblem[intCounter].Selected == true)
                    {
                        blnItemSelected = true;
                    }
                }
                strProblemResolution = txtProblemResolution.Text;
                if(strProblemResolution == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Problem Resolution was not Entered\n";
                }
                if(blnItemSelected == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "A Problem Was Not Selected\n";
                }
                if(gblnInvoiceAttached == true)
                {
                    if(decInvoiceAmount == 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "No Amount was Added with an Invoice Attaced\n";
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }


                datTransactionDate = DateTime.Now;

                if (gblnInvoiceAttached == true)
                {

                    intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                    strTransactionName += Convert.ToString(intResult);
                                       
                    strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\VehicleProblemFiles\\" + strTransactionName + ".pdf";
                                       
                    System.IO.File.Copy(gstrInvoicePath, strNewLocation);
                                       
                }
                else
                {
                    strNewLocation = gstrInvoicePath;
                }

                blnFatalError = TheVehiclePRoblemDocumentationClass.InsertVehicleInvoice(strInvoiceNumber, datTransactionDate, MainWindow.gintVehicleID, decInvoiceAmount, strNewLocation, MainWindow.gintVendorID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindVehicleInvoiceByInvoiceNumberDataSet = TheVehiclePRoblemDocumentationClass.FindVehicleInvoiceByInvoiceNumber(strInvoiceNumber, MainWindow.gintVendorID, MainWindow.gintVehicleID, datTransactionDate);

                intInvoiceID = TheFindVehicleInvoiceByInvoiceNumberDataSet.FindVehicleInvoiceByInvoiceNumber[0].InvoiceID;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intProblemID = TheOpenVehicleProblemsDataSet.openvehicleproblem[intCounter].ProblemID;

                    if(TheOpenVehicleProblemsDataSet.openvehicleproblem[intCounter].Selected == true)
                    {
                        blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemResolution(intProblemID, datTransactionDate, strProblemResolution, intInvoiceID);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemSolved(intProblemID, true);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheVehicleProblemClass.ChangeVehicleProblemStatus(intProblemID, "CLOSED");

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

               

                TheMessagesClass.InformationMessage("The Problems have been Updated and Closed");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Invoice Vehicle Problems // Process Invoice Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboAttachInvoice_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cboAttachInvoice.SelectedIndex > 0)
                {
                    if (cboAttachInvoice.SelectedIndex == 2)
                    {
                        gstrInvoicePath = "NO INVOICE ATTACHED";
                        gblnInvoiceAttached = false;
                    }
                    else if (cboAttachInvoice.SelectedIndex == 1)
                    {
                        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                        dlg.FileName = "Document"; // Default file name

                        // Show open file dialog box
                        Nullable<bool> result = dlg.ShowDialog();

                        // Process open file dialog box results
                        if (result == true)
                        {
                            // Open document
                            gstrInvoicePath = dlg.FileName.ToUpper();
                        }
                        else
                        {
                            return;
                        }

                        gblnInvoiceAttached = true;
                    }

                    txtInvoicePath.Text = gstrInvoicePath;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Invoice Vehicle Problems // CBO Attach Invoice Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if(intSelectedIndex > -1)
                {
                    if (TheOpenVehicleProblemsDataSet.openvehicleproblem[intSelectedIndex].Selected == false)
                    {
                        TheOpenVehicleProblemsDataSet.openvehicleproblem[intSelectedIndex].Selected = true;
                    }
                    else if(TheOpenVehicleProblemsDataSet.openvehicleproblem[intSelectedIndex].Selected == true)
                    {
                        TheOpenVehicleProblemsDataSet.openvehicleproblem[intSelectedIndex].Selected = false;
                    }

                    dgrResults.ItemsSource = TheOpenVehicleProblemsDataSet.openvehicleproblem;
                    dgrResults.UnselectAll();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Invoice Vehicle Problems // Data Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
                
        }
    }
}
