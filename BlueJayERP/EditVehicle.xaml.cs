/* Title:           Edit Vehicle
 * Date:            4-20-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to edit vehicles*/

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
using VehicleMainDLL;
using NewEventLogDLL;
using DataValidationDLL;
using VehicleStatusDLL;
using VehicleAssignmentDLL;
using NewEmployeeDLL;
using VehicleInfoDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditVehicle.xaml
    /// </summary>
    public partial class EditVehicle : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindDOTStatusSortedDataSet TheFindDOTStatusSortedDataSet = new FindDOTStatusSortedDataSet();
        FindGPSPlugStatusSortedDataSet TheFindGPSPlugStatusSortedDataSet = new FindGPSPlugStatusSortedDataSet();
        FindVehicleInfoByIMEIDataSet TheFindVehicleInfoByIMEIDataSet = new FindVehicleInfoByIMEIDataSet();
        FindVehicleInfoByVehicleNumberDataSet TheFindVehicleInfoByVehicleNumberDataSet = new FindVehicleInfoByVehicleNumberDataSet();
        
        //setting variables
        bool gblnCDLRequired;
        bool gblnMedicalCardRequired;
        int gintDOTStatusID;
        int gintGPSStatusID;
        string gstrDOTStatus;
        string gstrGPSStatus;
        bool gblnInfoNotEntered;
        int gintVehicleInfoID;

        //creating global variables
        string gstrAssignedOffice;

        public EditVehicle()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            ResetControls();
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void cboGPSPlugStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboGPSPlugStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintGPSStatusID = TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted[intSelectedIndex].GPSStatusID;

            gstrGPSStatus = cboGPSPlugStatus.SelectedItem.ToString();
        }

        private void cboDOTStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboDOTStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDOTStatusID = TheFindDOTStatusSortedDataSet.FindDOTStatusSorted[intSelectedIndex].DOTStatusID;

            gstrDOTStatus = cboDOTStatus.SelectedItem.ToString();
        }

        private void cboMedicalCardRequired_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboMedicalCardRequired.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnMedicalCardRequired = true;
            else if (intSelectedIndex == 2)
                gblnMedicalCardRequired = false;
        }

        private void cboCDLRequired_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboCDLRequired.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnCDLRequired = true;
            else if (intSelectedIndex == 2)
                gblnCDLRequired = false;
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gstrAssignedOffice = cboWarehouse.SelectedItem.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWarehouseComboBox();

            LoadVehicleInfoCombo();
        }
        private void LoadVehicleInfoCombo()
        {
            //this will set up the form for use
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //loading the CDL Required Combo Box
                cboCDLRequired.Items.Add("Select");
                cboCDLRequired.Items.Add("Yes");
                cboCDLRequired.Items.Add("No");
                cboCDLRequired.SelectedIndex = 0;

                cboMedicalCardRequired.Items.Add("Select");
                cboMedicalCardRequired.Items.Add("Yes");
                cboMedicalCardRequired.Items.Add("No");
                cboMedicalCardRequired.SelectedIndex = 0;

                TheFindDOTStatusSortedDataSet = TheVehicleInfoClass.FindDOTStatusSorted();
                cboDOTStatus.Items.Add("Select");

                intNumberOfRecords = TheFindDOTStatusSortedDataSet.FindDOTStatusSorted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboDOTStatus.Items.Add(TheFindDOTStatusSortedDataSet.FindDOTStatusSorted[intCounter].DOTStatus);
                }

                TheFindGPSPlugStatusSortedDataSet = TheVehicleInfoClass.FindGPSPlugStatusSorted();

                cboGPSPlugStatus.Items.Add("Select");

                intNumberOfRecords = TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboGPSPlugStatus.Items.Add(TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted[intCounter].GPSStatus);
                }

                cboDOTStatus.SelectedIndex = 0;
                cboGPSPlugStatus.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Vehicle // Load Vehicle Info Combo " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
        private void LoadWarehouseComboBox()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            //this will load the combo box
            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            cboWarehouse.Items.Add("Select Warehouse");

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboWarehouse.SelectedIndex = 0;
        }
        private void ResetControls()
        {
            cboWarehouse.SelectedIndex = 0;
            txtVehicleNumber.Text = "";
            txtLicensePlate.Text = "";
            txtNotes.Text = "";
            txtOilChangeDate.Text = "";
            txtOilChangeOdometer.Text = "";
            txtVehicleID.Text = "";
            txtVehicleMake.Text = "";
            txtVehicleModel.Text = "";
            txtVehicleYear.Text = "";
            txtVINNumber.Text = "";
            txtIMEI.Text = "";
            txtTamperTag.Text = "";
            cboCDLRequired.SelectedIndex = 0;
            cboDOTStatus.SelectedIndex = 0;
            cboGPSPlugStatus.SelectedIndex = 0;
            cboMedicalCardRequired.SelectedIndex = 0;
            txtEnterVehicleNumber.Text = "";
            mitSave.IsEnabled = false;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strVehicleNumber;
            int intRecordsReturned;
            int intTamperTag;
            string strIMEI;
            string strDOTStatus;
            string strGPSStatus;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;
                if(strVehicleNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Not Entered");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Not Found");
                    return;
                }

                TheFindVehicleInfoByVehicleNumberDataSet = TheVehicleInfoClass.FindVehicleInfoByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    intTamperTag = 0;
                    strIMEI = "NOT ENTERED";
                    cboDOTStatus.SelectedIndex = 0;
                    cboGPSPlugStatus.SelectedIndex = 0;
                    cboCDLRequired.SelectedIndex = 0;
                    cboMedicalCardRequired.SelectedIndex = 0;
                    gblnInfoNotEntered = true;
                }
                else
                {
                    intTamperTag = TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].TamperTag;
                    strIMEI = TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].IMEI;
                    strDOTStatus = TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].DOTStatus;
                    strGPSStatus = TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].GPSStatus;
                    gblnInfoNotEntered = false;
                    gintVehicleInfoID = TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].VehicleInfoID;

                    FindDOTStatus(strDOTStatus);

                    FindGPSStatus(strGPSStatus);

                    if(TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].CDLRequired == true)
                    {
                        cboCDLRequired.SelectedIndex = 1;
                    }
                    else
                    {
                        cboCDLRequired.SelectedIndex = 2;
                    }
                    if(TheFindVehicleInfoByVehicleNumberDataSet.FindVehicleInfoByVehicleNumber[0].MedicalCardRequired == true)
                    {
                        cboMedicalCardRequired.SelectedIndex = 1;
                    }
                    else
                    {
                        cboMedicalCardRequired.SelectedIndex = 2;
                    }

                }

                SetAssignedOffice(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].AssignedOffice);

                txtIMEI.Text = strIMEI;
                txtLicensePlate.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].LicensePlate;
                txtNotes.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].Notes;
                txtOilChangeDate.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].OilChangeDate);
                txtOilChangeOdometer.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].OilChangeOdometer);
                txtTamperTag.Text = Convert.ToString(intTamperTag);
                txtVehicleID.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID);
                txtVehicleMake.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleMake;
                txtVehicleModel.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleModel;
                txtVehicleNumber.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleNumber;
                txtVehicleYear.Text = Convert.ToString(TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleYear);
                txtVINNumber.Text = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VINNumber;

                mitSave.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Vehicle // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FindGPSStatus(string strGPSStatus)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = cboGPSPlugStatus.Items.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboGPSPlugStatus.SelectedIndex = intCounter;

                    if (strGPSStatus == cboGPSPlugStatus.SelectedItem.ToString())
                    {
                        intSelectedIndex = intCounter;
                    }
                }

                cboGPSPlugStatus.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Vehicle // Find GPS Status " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FindDOTStatus(string strDOTStatus)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = cboDOTStatus.Items.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboDOTStatus.SelectedIndex= intCounter;

                    if (strDOTStatus == cboDOTStatus.SelectedItem.ToString())
                    {
                        intSelectedIndex = intCounter;
                    }
                }

                cboDOTStatus.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Vehicle // Find DOT Status " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetAssignedOffice(string strAssignedOffice)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = cboWarehouse.Items.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWarehouse.SelectedIndex = intCounter;

                    if (strAssignedOffice == cboWarehouse.SelectedItem.ToString())
                    {
                        intSelectedIndex = intCounter;
                    }
                }

                cboWarehouse.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Vehicle // Set Assigned Office " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            int intVehicleID;
            string strLicensePlate;
            int intOdometerReading = 0;
            DateTime datOilChangeDate = DateTime.Now;
            string strVINNumber;
            string strNotes;
            string strIMEI;
            int intTamperTag = 0;

            try
            {
                intVehicleID = Convert.ToInt32(txtVehicleID.Text);
                strLicensePlate = txtLicensePlate.Text;
                if(strLicensePlate == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The License Plate Was Not Entered\n";
                }
                strValueForValidation = txtOilChangeOdometer.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Odometer was not an Integer\n";
                }
                else
                {
                    intOdometerReading = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtOilChangeDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Odometer Date is not a Date\n";
                }
                else
                {
                    datOilChangeDate = Convert.ToDateTime(strValueForValidation);
                }
                strVINNumber = txtVINNumber.Text;
                if(strVINNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "VIN Number Was Not Entered\n";
                }
                strNotes = txtNotes.Text;
                if(strNotes == "")
                {
                    strNotes = "NO NOTES ENTERED";
                }
                if(cboWarehouse.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Assigned Office Was Not Selected\n";
                }
                if(cboCDLRequired.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "CDL Required Was Not Selected\n";
                }
                if(cboMedicalCardRequired.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Medical Card Required Was Not Selected\n";
                }
                if(cboDOTStatus.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "DOT Status was not Selected\n";
                }
                if(cboGPSPlugStatus.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Plug Status Was Not Selected\n";
                }
                strIMEI = txtIMEI.Text;
                if(strIMEI == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "IMEI Was not Entered\n";
                }
                strValueForValidation = txtTamperTag.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Tamper Tag Is Not An Integer\n";
                }
                else
                {
                    intTamperTag = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheVehicleMainClass.UpdateVehicleMainEdit(intVehicleID, strLicensePlate, intOdometerReading, datOilChangeDate, strVINNumber, strNotes, true, gstrAssignedOffice);

                if (blnFatalError == true)
                    throw new Exception();

                if(gblnInfoNotEntered == false)
                {
                    blnFatalError = TheVehicleInfoClass.UpdateVehicleInfoStatus(gintVehicleInfoID, gblnCDLRequired, gblnMedicalCardRequired, gintDOTStatusID, gintGPSStatusID, DateTime.Now);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else
                {
                    blnFatalError = TheVehicleInfoClass.InsertVehicleInfo(intVehicleID, gblnCDLRequired, gblnMedicalCardRequired, gintDOTStatusID, gintGPSStatusID, strIMEI, intTamperTag);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("Vehicle Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue ERP // Edit Vehicle // Save Menu Item " + Ex.Message);

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
