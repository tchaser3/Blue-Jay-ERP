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
using DataValidationDLL;
using PhonesDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for PhoneList.xaml
    /// </summary>
    public partial class PhoneList : Window
    {
        // setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValiationClass = new DataValidationClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindSortedPhoneListByExtensionDataSet TheFindSortedPhoneListByExtensionsDataSet = new FindSortedPhoneListByExtensionDataSet();
        FindSortedExtensionsByLastNameDataSet TheFindSortedExtensionsByLastNameDataSet = new FindSortedExtensionsByLastNameDataSet();
        FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        FindPhoneByExtensionDataSet TheFindPhoneByExtensionDataSet = new FindPhoneByExtensionDataSet();
        FindPhoneExtensionsByLocationDataSet TheFindPhoneExtensionByLocationDataSet = new FindPhoneExtensionsByLocationDataSet();
        NewPhoneListDataSet TheNewPhoneListDataSet = new NewPhoneListDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        bool gblnLocations;

        public PhoneList()
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
            PhoneListByExtension();
            cboReportType.Items.Clear();
            cboReportType.Items.Add("Select Phone List");
            cboReportType.Items.Add("Phone List By Extension");
            cboReportType.Items.Add("Phone List By Last Name");
            cboReportType.Items.Add("Phone List For Location");
            cboReportType.Items.Add("Find Number By Last Name");
            cboReportType.Items.Add("Find Employee By Extension");
            cboReportType.SelectedIndex = 0;
            gblnLocations = false;
        }
        private void PhoneListByExtension()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;
            string strPhoneNumber = "";

            try
            {
                TheFindSortedPhoneListByExtensionsDataSet = ThePhonesClass.FindSortedPhoneListByExtension();

                TheNewPhoneListDataSet.phonelist.Rows.Clear();

                intNumberOfRecords = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intWarehouseID = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].WarehouseID;
                    if(TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].IsPhoneNumberNull() == true)
                    {
                        strPhoneNumber = "";
                    }
                    else
                    {
                        strPhoneNumber = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].PhoneNumber;
                    }

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);

                    NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                    NewPhone.DIDNumber = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].DirectNumber;
                    NewPhone.Extension = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].Extension;
                    NewPhone.FirstName = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].FirstName;
                    NewPhone.LastName = TheFindSortedPhoneListByExtensionsDataSet.FindSortedPhoneListByExtension[intCounter].LastName;
                    NewPhone.Location = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    NewPhone.CellPhone = strPhoneNumber;

                    TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);
                }

                dgrResults.ItemsSource = TheNewPhoneListDataSet.phonelist;
                txtEnterExtension.Text = "";
                txtEnterExtension.Visibility = Visibility.Hidden;
                lblEnterExtension.Visibility = Visibility.Hidden;
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Visibility = Visibility.Hidden;
                lblSelectLocation.Visibility = Visibility.Hidden;
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Phone List // Phone List By Extension " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboReportType.SelectedIndex;

            if(intSelectedIndex == 1)
            {
                PhoneListByExtension();
            }
            else if(intSelectedIndex == 2)
            {
                PhoneListByLastName();
            }
            else if(intSelectedIndex == 3)
            {
                LocationPhoneList();
            }
            else if(intSelectedIndex == 4)
            {
                PhoneExtensionByLastName();
            }
            else if(intSelectedIndex == 5)
            {
                PhonesByExtension();
            }
        }
        private void PhonesByExtension()
        {
            gblnLocations = false;
            lblEnterExtension.Visibility = Visibility.Visible;
            lblEnterExtension.Content = "Phone Ext";
            txtEnterExtension.Visibility = Visibility.Visible;
            cboSelectLocation.Items.Clear();
            lblSelectLocation.Content = "Select Employee";
            lblSelectLocation.Visibility = Visibility.Hidden;
            cboSelectLocation.Visibility = Visibility.Hidden;
        }
        private void PhoneExtensionByLastName()
        {
            gblnLocations = false;
            lblEnterExtension.Visibility = Visibility.Visible;
            lblEnterExtension.Content = "Last Name";
            txtEnterExtension.Visibility = Visibility.Visible;
            cboSelectLocation.Items.Clear();
            lblSelectLocation.Content = "Select Employee";
            lblSelectLocation.Visibility = Visibility.Hidden;
            cboSelectLocation.Visibility = Visibility.Hidden;
        }
        private void LocationPhoneList()
        {
            //this will load up the combo box for the report
            int intCounter;
            int intNumberOfRecords;

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
            cboSelectLocation.Visibility = Visibility.Visible;
            lblSelectLocation.Visibility = Visibility.Visible;

            cboSelectLocation.Items.Clear();
            gblnLocations = true;
            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;
            cboSelectLocation.Items.Add("Select Location");
            lblSelectLocation.Content = "Select Location";
            TheNewPhoneListDataSet.phonelist.Rows.Clear();
            dgrResults.ItemsSource = TheNewPhoneListDataSet.phonelist;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectLocation.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboSelectLocation.SelectedIndex = 0;
        }
        private void PhoneListByLastName()
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;
            string strPhoneNumber;

            try
            {
                TheFindSortedExtensionsByLastNameDataSet = ThePhonesClass.FindSortedExtensionsByLastName();
                TheNewPhoneListDataSet.phonelist.Rows.Clear();

                intNumberOfRecords = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intWarehouseID = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].WarehouseID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);

                    if(TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].IsPhoneNumberNull() == true)
                    {
                        strPhoneNumber = "";
                    }
                    else
                    {
                        strPhoneNumber = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].PhoneNumber;
                    }

                    

                    NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                    NewPhone.DIDNumber = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].DirectNumber;
                    NewPhone.Extension = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].Extension;
                    NewPhone.FirstName = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].FirstName;
                    NewPhone.LastName = TheFindSortedExtensionsByLastNameDataSet.FindSortedExtensionsByLastName[intCounter].LastName;
                    NewPhone.Location = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    NewPhone.CellPhone = strPhoneNumber;

                    TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);
                }

                dgrResults.ItemsSource = TheNewPhoneListDataSet.phonelist;
                txtEnterExtension.Text = "";
                txtEnterExtension.Visibility = Visibility.Hidden;
                lblEnterExtension.Visibility = Visibility.Hidden;
                cboSelectLocation.Items.Clear();
                cboSelectLocation.Visibility = Visibility.Hidden;
                lblSelectLocation.Visibility = Visibility.Hidden;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Phone List // Phone List by Last Name " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void CboSelectLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intWarehouseID;
            string strWarehouse;
            int intCounter;
            int intNumberOfRecords;
            string strPhoneNumber;

            intSelectedIndex = cboSelectLocation.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                if(gblnLocations == true)
                {
                    intWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
                    strWarehouse = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;
                    TheNewPhoneListDataSet.phonelist.Rows.Clear();

                    TheFindPhoneExtensionByLocationDataSet = ThePhonesClass.FindPhoneExtensionByLocation(intWarehouseID);

                    intNumberOfRecords = TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            if(TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation[intCounter].IsPhoneNumberNull() == true)
                            {
                                strPhoneNumber = "";
                            }
                            else
                            {
                                strPhoneNumber = TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation[intCounter].PhoneNumber;
                            }

                            NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                            NewPhone.DIDNumber = TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation[intCounter].DirectNumber;
                            NewPhone.Extension = TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation[intCounter].Extension;
                            NewPhone.FirstName = TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation[intCounter].FirstName;
                            NewPhone.LastName = TheFindPhoneExtensionByLocationDataSet.FindPhoneExtensionsByLocation[intCounter].LastName;
                            NewPhone.Location = strWarehouse;
                            NewPhone.CellPhone = strPhoneNumber;

                            TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);
                        }
                    }
                }

                dgrResults.ItemsSource = TheNewPhoneListDataSet.phonelist;

            }
        }

        private void TxtEnterExtension_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strEnteredInformation;
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            bool blnFatalError = false;
            int intEmployeeID;
            int intRecordsReturned;
            int intSecondCounter;
            string strFirstName;
            string strLastName;
            int intExtension;
            int intWarehouseID;
            string strPhoneNumber;

            try
            {
                strEnteredInformation = txtEnterExtension.Text;
                intLength = strEnteredInformation.Length;
                TheNewPhoneListDataSet.phonelist.Rows.Clear();
                if(intLength == 3)
                {
                    TheComboEmployeeDataSet.employees.Rows.Clear();

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strEnteredInformation);


                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                            strFirstName = TheComboEmployeeDataSet.employees[intCounter].FirstName;
                            strLastName = TheComboEmployeeDataSet.employees[intCounter].LastName;

                            TheFindPhoneExtensionByEmployeeIDDataSet = ThePhonesClass.FindPhoneExtensionByEmployeeID(intEmployeeID);

                            intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count - 1;

                            if(intRecordsReturned > -1)
                            {
                                for(intSecondCounter = 0; intSecondCounter <= intRecordsReturned; intSecondCounter++)
                                {
                                    if(TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].IsPhoneNumberNull() == true)
                                    {
                                        strPhoneNumber = "";
                                    }
                                    else
                                    {
                                        strPhoneNumber = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].PhoneNumber;
                                    }

                                    NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                                    NewPhone.DIDNumber = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].DirectNumber;
                                    NewPhone.Extension = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].Extension;
                                    NewPhone.FirstName = strFirstName;
                                    NewPhone.LastName = strLastName;
                                    NewPhone.Location = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].AssignedOffice;
                                    NewPhone.CellPhone = strPhoneNumber;

                                    TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);

                                }
                            }
                        }
                    }
                }
                else if (intLength == 4)
                {
                    blnFatalError = TheDataValiationClass.VerifyIntegerData(strEnteredInformation);
                    if(blnFatalError == false)
                    {
                        intExtension = Convert.ToInt32(strEnteredInformation);

                        TheFindPhoneByExtensionDataSet = ThePhonesClass.FindPhoneByExtension(intExtension);

                        intRecordsReturned = TheFindPhoneByExtensionDataSet.FindPhoneByExtension.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            TheMessagesClass.ErrorMessage("The Extension Was Not Found");
                            return;
                        }

                        intWarehouseID = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].WarehouseID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);

                        if(TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].IsPhoneNumberNull() == true)
                        {
                            strPhoneNumber = "";
                        }
                        else
                        {
                            strPhoneNumber = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].PhoneNumber;
                        }

                        NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                        NewPhone.DIDNumber = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].DirectNumber;
                        NewPhone.Extension = intExtension;
                        NewPhone.FirstName = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].FirstName;
                        NewPhone.LastName = TheFindPhoneByExtensionDataSet.FindPhoneByExtension[0].LastName;
                        NewPhone.Location = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                        NewPhone.CellPhone = strPhoneNumber;

                        TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);
                    }
                    else
                    {
                        TheComboEmployeeDataSet.employees.Rows.Clear();

                        TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strEnteredInformation);


                        intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                        if (intNumberOfRecords > -1)
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                intEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                                strFirstName = TheComboEmployeeDataSet.employees[intCounter].FirstName;
                                strLastName = TheComboEmployeeDataSet.employees[intCounter].LastName;

                                TheFindPhoneExtensionByEmployeeIDDataSet = ThePhonesClass.FindPhoneExtensionByEmployeeID(intEmployeeID);

                                intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count - 1;

                                if (intRecordsReturned > -1)
                                {
                                    for (intSecondCounter = 0; intSecondCounter <= intRecordsReturned; intSecondCounter++)
                                    {
                                        if (TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].IsPhoneNumberNull() == true)
                                        {
                                            strPhoneNumber = "";
                                        }
                                        else
                                        {
                                            strPhoneNumber = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].PhoneNumber;
                                        }

                                        NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                                        NewPhone.DIDNumber = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].DirectNumber;
                                        NewPhone.Extension = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].Extension;
                                        NewPhone.FirstName = strFirstName;
                                        NewPhone.LastName = strLastName;
                                        NewPhone.CellPhone = strPhoneNumber;
                                        NewPhone.Location = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].AssignedOffice;

                                        TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);

                                    }
                                }
                            }
                        }
                    }
                }
                else if(intLength > 5)
                {
                    TheComboEmployeeDataSet.employees.Rows.Clear();

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strEnteredInformation);


                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                            strFirstName = TheComboEmployeeDataSet.employees[intCounter].FirstName;
                            strLastName = TheComboEmployeeDataSet.employees[intCounter].LastName;

                            TheFindPhoneExtensionByEmployeeIDDataSet = ThePhonesClass.FindPhoneExtensionByEmployeeID(intEmployeeID);

                            intRecordsReturned = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID.Rows.Count - 1;

                            if (intRecordsReturned > -1)
                            {
                                for (intSecondCounter = 0; intSecondCounter <= intRecordsReturned; intSecondCounter++)
                                {
                                    if (TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].IsPhoneNumberNull() == true)
                                    {
                                        strPhoneNumber = "";
                                    }
                                    else
                                    {
                                        strPhoneNumber = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].PhoneNumber;
                                    }

                                    NewPhoneListDataSet.phonelistRow NewPhone = TheNewPhoneListDataSet.phonelist.NewphonelistRow();

                                    NewPhone.DIDNumber = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].DirectNumber;
                                    NewPhone.Extension = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].Extension;
                                    NewPhone.FirstName = strFirstName;
                                    NewPhone.LastName = strLastName;
                                    NewPhone.CellPhone = strPhoneNumber;
                                    NewPhone.Location = TheFindPhoneExtensionByEmployeeIDDataSet.FindPhoneExtensionByEmployeeID[intSecondCounter].AssignedOffice;

                                    TheNewPhoneListDataSet.phonelist.Rows.Add(NewPhone);

                                }
                            }
                        }
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("The Employee Entered was not found");
                        return;
                    }
                }

                dgrResults.ItemsSource = TheNewPhoneListDataSet.phonelist;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Phone List // Text Box Changed Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void MitExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheNewPhoneListDataSet.phonelist.Rows.Count;
                intColumnNumberOfRecords = TheNewPhoneListDataSet.phonelist.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheNewPhoneListDataSet.phonelist.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheNewPhoneListDataSet.phonelist.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Phone List // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void MitPrint_Click(object sender, RoutedEventArgs e)
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {
                PrintDialog pdProblemReport = new PrintDialog();


                if (pdProblemReport.ShowDialog().Value)
                {
                    FlowDocument fdProjectReport = new FlowDocument();
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdProjectReport.PagePadding = thickness;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheNewPhoneListDataSet.phonelist.Columns.Count;
                    fdProjectReport.ColumnWidth = 10;
                    fdProjectReport.IsColumnWidthFlexible = false;


                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        ProjectReportTable.Columns.Add(new TableColumn());
                    }
                    ProjectReportTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Blue Jay Employee Phone List"))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                                       
                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Extension"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("DID Number"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Office"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    //newTableRow.Cells[0].ColumnSpan = 1;
                    //newTableRow.Cells[1].ColumnSpan = 1;
                    //newTableRow.Cells[2].ColumnSpan = 1;
                    //newTableRow.Cells[3].ColumnSpan = 2;
                    //newTableRow.Cells[4].ColumnSpan = 1;

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheNewPhoneListDataSet.phonelist.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheNewPhoneListDataSet.phonelist[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                            //if (intColumnCounter == 3)
                            //{
                            //newTableRow.Cells[intColumnCounter].ColumnSpan = 2;
                            //}

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProblemReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProblemReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProblemReport.PrintableAreaWidth;
                    pdProblemReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Phone List");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Phone List // Print Menu Item " + Ex.Message);
            }
        }
    }
}
