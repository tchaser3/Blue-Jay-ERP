/* Title:           Add Vehicle
 * Date:            4-11-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is to add a vehicle*/

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
    /// Interaction logic for AddVehicle.xaml
    /// </summary>
    public partial class AddVehicle : Window
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

        //setting variables
        bool gblnCDLRequired;
        bool gblnMedicalCardRequired;
        int gintDOTStatusID;
        int gintGPSStatusID;
        //int gintVehicleID;
        //int gintVehicleInfoID;
        //bool gblnRecordEdit;
        string gstrDOTStatus;
        string gstrGPSStatus;
        //string gstrIMEI;
       // int gintTamperTag;

        //creating global variables
        string gstrAssignedOffice;
        int gintEmployeeID;

        public AddVehicle()
        {
            InitializeComponent();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle // Load Vehicle Info Combo " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the form
            try
            {
                LoadWarehouseComboBox();

                LoadVehicleInfoCombo();

                SetControlsReadOnly(true);

                mitSave.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle // Window Loaded " + Ex.Message);

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
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtVehicleNumber.IsReadOnly = blnValueBoolean;
            txtEmployeeID.IsReadOnly = blnValueBoolean;
            txtLicensePlate.IsReadOnly = blnValueBoolean;
            txtNotes.IsReadOnly = blnValueBoolean;
            txtOilChangeDate.IsReadOnly = blnValueBoolean;
            txtOilChangeOdometer.IsReadOnly = blnValueBoolean;
            txtVehicleMake.IsReadOnly = blnValueBoolean;
            txtVehicleModel.IsReadOnly = blnValueBoolean;
            txtVehicleYear.IsReadOnly = blnValueBoolean;
            txtVINNumber.IsReadOnly = blnValueBoolean;
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitAdd_Click(object sender, RoutedEventArgs e)
        {
            int intVehicleID;

            SetControlsReadOnly(false);

            intVehicleID = 0;

            txtVehicleID.Text = Convert.ToString(intVehicleID);

            mitSave.IsEnabled = true;
            mitAdd.IsEnabled = false;

            txtNotes.Text = "NO NOTES PROVIDED";
        }


        private void ResetControls()
        {
            cboWarehouse.SelectedIndex = 0;
            txtVehicleNumber.Text = "";
            txtEmployeeID.Text = "";
            txtLicensePlate.Text = "";
            txtNotes.Text = "";
            txtOilChangeDate.Text = "";
            txtOilChangeOdometer.Text = "";
            txtVehicleID.Text = "";
            txtVehicleMake.Text = "";
            txtVehicleModel.Text = "";
            txtVehicleYear.Text = "";
            txtVINNumber.Text = "";
            mitAdd.IsEnabled = true;
            mitSave.IsEnabled = false;
            txtIMEI.Text = "";
            txtTamperTag.Text = "";
            cboCDLRequired.SelectedIndex = 0;
            cboDOTStatus.SelectedIndex = 0;
            cboGPSPlugStatus.SelectedIndex = 0;
            cboMedicalCardRequired.SelectedIndex = 0;
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            gstrAssignedOffice = cboWarehouse.SelectedItem.ToString();

            if (gstrAssignedOffice != "Select Warehouse")
            {
                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if (gstrAssignedOffice == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        gintEmployeeID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                        txtEmployeeID.Text = Convert.ToString(gintEmployeeID);
                    }
                }
            }
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            int intVehicleID;
            string strVehicleNumber;
            int intVehicleYear = 0;
            string strVehicleMake;
            string strVehicleModel;
            string strLicensePlate;
            string strVINNumber;
            int intOilChangeOdometer = 0;
            DateTime datOilChangeDate = DateTime.Now;
            string strNotes;
            int intRecordsReturned;
            string strIMEI;
            int intTamperTag = 0;

            try
            {
                //beginning data validation
                if (cboWarehouse.SelectedItem.ToString() == "Select Warehouse")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Assigned Office Was Not Selected\n";
                }
                strVehicleNumber = txtVehicleNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTextData(strVehicleNumber);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Number Was Not An Integer\n";
                }
                
                strValueForValidation = txtVehicleYear.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Year is not an Integer\n";
                }
                else
                {
                    intVehicleYear = Convert.ToInt32(strValueForValidation);
                }
                strVehicleMake = txtVehicleMake.Text;
                if (strVehicleMake == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Vehicle Make Was Not Entered\n";
                }
                strVehicleModel = txtVehicleModel.Text;
                if (strVehicleModel == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Vehicle Model Was Not Entered\n";
                }
                strLicensePlate = txtLicensePlate.Text;
                if (strLicensePlate == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The License Plate Was Not Entered\n";
                }
                strVINNumber = txtVINNumber.Text;
                if (strVINNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The VIN Number Was Not Entered\n";
                }
                strValueForValidation = txtOilChangeOdometer.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Oil Change Odometer Is Not An Integer\n";
                }
                else
                {
                    intOilChangeOdometer = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtOilChangeDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Oil Change Date is not a Date\n";
                }
                else
                {
                    datOilChangeDate = Convert.ToDateTime(strValueForValidation);
                }
                strNotes = txtNotes.Text;
                if (strNotes == "")
                {
                    strNotes = "NO NOTES PROVIDED";
                }
                strIMEI = txtIMEI.Text;
                if(strIMEI == "")
                {
                    strErrorMessage += "The IMEI was not Entered\n";
                }
                strValueForValidation = txtTamperTag.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Tamper Tag Is Not An Integer\n";
                }
                else
                {
                    intTamperTag = Convert.ToInt32(strValueForValidation);
                }
                if(cboCDLRequired.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "CDL Required Not Selected\n";
                }
                if(cboDOTStatus.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "DOT Status Not Selected\n";
                }
                if(cboGPSPlugStatus.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "GPS Plug Status Not Selected\n";
                }
                if(cboMedicalCardRequired.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Medical Card Required Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned != 0)
                {
                    TheMessagesClass.ErrorMessage("There Is An Active Vehicle With The Same Vehicle Number");
                    return;
                }

                intVehicleID = Convert.ToInt32(txtVehicleID.Text);

                blnFatalError = TheVehicleMainClass.InsertVehicleMain(intVehicleID, strVehicleNumber, intVehicleYear, strVehicleMake, strVehicleModel, strLicensePlate, strVINNumber, intOilChangeOdometer, datOilChangeDate, gintEmployeeID, strNotes, gstrAssignedOffice);

                if (blnFatalError == true)
                {
                    throw new Exception();                   
                }
                else
                {
                    SetControlsReadOnly(true);
                    ResetControls();
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                blnFatalError = TheVehicleInfoClass.InsertVehicleInfo(intVehicleID, gblnCDLRequired, gblnMedicalCardRequired, gintDOTStatusID, gintGPSStatusID, strIMEI, intTamperTag);

                if (blnFatalError == true)
                    throw new Exception();

                 TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                blnFatalError = TheVehicleStatusClass.InsertVehicleStatus(intVehicleID, "AVAILABLE", DateTime.Now);

                blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, gintEmployeeID);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Vehicle // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

        private void cboMedicalCardRequired_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboMedicalCardRequired.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnMedicalCardRequired = true;
            else if (intSelectedIndex == 2)
                gblnMedicalCardRequired = false;
        }

        private void cboDOTStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboDOTStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDOTStatusID = TheFindDOTStatusSortedDataSet.FindDOTStatusSorted[intSelectedIndex].DOTStatusID;

            gstrDOTStatus = cboDOTStatus.SelectedItem.ToString();
        }

        private void cboGPSPlugStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboGPSPlugStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintGPSStatusID = TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted[intSelectedIndex].GPSStatusID;

            gstrGPSStatus = cboGPSPlugStatus.SelectedItem.ToString();
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
