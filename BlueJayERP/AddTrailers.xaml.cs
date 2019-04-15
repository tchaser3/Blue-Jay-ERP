/* Titile:          Add Trailers
 * Date:            9-17-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the add trailers */

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
using TrailersDLL;
using TrailerCategoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddTrailers.xaml
    /// </summary>
    public partial class AddTrailers : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        TrailerCategoryClass TheTrailerCategoryClass = new TrailerCategoryClass();

        //setting up the data
        FindTrailerByTrailerNumberDataSet TheFindTrailerByTrailerNumberDataSet = new FindTrailerByTrailerNumberDataSet();
        FindSortedTrailerCategoryDataSet TheFindSortedTrailerCategoryDataSet = new FindSortedTrailerCategoryDataSet();

        int gintCategoryID;

        public AddTrailers()
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
            MainWindow.SendEmailWindow.Visibility = Visibility.Visible;
        }

        private void mitMyOriginatingTasks_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MyOriginatingTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitMyOpenTasks_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateAssignTasksWindow.Visibility = Visibility.Visible;
        }

        private void mitAssignTask_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.AssignTasksWindow.Visibility = Visibility.Visible;
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
        private void LoadCategoryCombo()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindSortedTrailerCategoryDataSet = TheTrailerCategoryClass.FindSortedTrailerCategory();

                intNumberOfRecords = TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory.Rows.Count - 1;
                cboTrailerCategory.Items.Clear();
                cboTrailerCategory.Items.Add("Select Trailer Type");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboTrailerCategory.Items.Add(TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory[intCounter].TrailerCategory);
                }

                cboTrailerCategory.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Trailers // Load Category Combo " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadWarehouseCombo()
        {
            int intCounter;
            int intNumberOfRecords;

            cboAssignedWarehouse.Items.Clear();
            cboAssignedWarehouse.Items.Add("Select Warehouse");
            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboAssignedWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboAssignedWarehouse.SelectedIndex = 0;
        }
        private void ClearTextBoxes()
        {
            txtDescription.Text = "";
            txtLicensePlate.Text = "";
            txtTrailerNotes.Text = "";
            txtTrailerNumber.Text = "";
            txtVINNUmber.Text = "";
        }
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadCategoryCombo();
            LoadWarehouseCombo();
            ClearTextBoxes();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategoryCombo();
            LoadWarehouseCombo();
            ClearTextBoxes();
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strTrailerNumber;
            string strVINNumber;
            string strTrailerDescription;
            string strLicensePlate;
            string strTrailerNotes;
            int intRecordsReturned;
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                strTrailerNumber = txtTrailerNumber.Text;
                if(strTrailerNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Trailer Number Was Not Entered\n";
                }
                else
                {
                    TheFindTrailerByTrailerNumberDataSet = TheTrailersClass.FindTrailerByTrailerNumber(strTrailerNumber);

                    intRecordsReturned = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Trailer Number is Already Used\n";
                    }
                }
                if(cboTrailerCategory.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Trailer Type Was Not Selected\n";
                }
                strVINNumber = txtVINNUmber.Text;
                if(strVINNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "VIN Number Was Not Entered\n";
                }
                strTrailerDescription = txtDescription.Text;
                if(strTrailerDescription == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Trailer Description Was Not Entered\n";
                }
                strLicensePlate = txtLicensePlate.Text;
                if(strLicensePlate == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The License Plate Was Not Entered\n";
                }
                if(cboAssignedWarehouse.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Trailer Was Not Assigned a Warehouse\n";
                }
                strTrailerNotes = txtTrailerNotes.Text;
                if(strTrailerNotes == "")
                {
                    strTrailerNotes = "NO NOTES ENTERED";
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //inserting the trailer
                blnFatalError = TheTrailersClass.InsertTrailer(strTrailerNumber, gintCategoryID, strVINNumber, strTrailerDescription, strLicensePlate, MainWindow.gintWarehouseID, MainWindow.gintWarehouseID, strTrailerNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Trailer has been Added");

                LoadCategoryCombo();
                LoadWarehouseCombo();
                ClearTextBoxes();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Trailers // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboTrailerCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboTrailerCategory.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintCategoryID = TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory[intSelectedIndex].CategoryID;
            }
        }

        private void cboAssignedWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboAssignedWarehouse.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }
    }
}
