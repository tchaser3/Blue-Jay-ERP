/* Title:           Edit Trailer
 * Date:            9-20-18
 * Author:          Terry Holmes
 * 
 * Description:     This window is used to edit a trailer */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using TrailerCategoryDLL;
using TrailersDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for EditTrailer.xaml
    /// </summary>
    public partial class EditTrailer : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailerCategoryClass TheTrailerCategoryClass = new TrailerCategoryClass();
        TrailersClass TheTrailersClass = new TrailersClass();

        //setting up the data
        FindTrailerByTrailerNumberDataSet TheFindTrailerByTrailerNumberDataSet = new FindTrailerByTrailerNumberDataSet();
        FindSortedTrailerCategoryDataSet TheFindSortedTrailerCategoryDataSet = new FindSortedTrailerCategoryDataSet();

        //setting up variables
        int gintCategoryID;
        bool gblnActive;

        public EditTrailer()
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
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            //resetting combo boxes
            cboLocation.Items.Clear();
            cboLocation.Items.Add("Select Warehouse");

            MainWindow.TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboLocation.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboLocation.SelectedIndex = 0;

            cboTrailerCategory.Items.Clear();
            cboTrailerCategory.Items.Add("Select Trailer Type");

            TheFindSortedTrailerCategoryDataSet = TheTrailerCategoryClass.FindSortedTrailerCategory();

            intNumberOfRecords = TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboTrailerCategory.Items.Add(TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory[intCounter].TrailerCategory);
            }

            cboActive.Items.Clear();
            cboActive.Items.Add("Select Active");
            cboActive.Items.Add("Yes");
            cboActive.Items.Add("No");
            cboActive.SelectedIndex = 0;
            cboTrailerCategory.SelectedIndex = 0;

            //clearing text boxes
            txtEnterTrailerNumber.Text = "";
            txtLicensePlate.Text = "";
            txtTrailerDescription.Text = "";
            txtTrailerNotes.Text = "";
            txtVINNumber.Text = "";
            txtTrailerID.Text = "";
            txtEnterTrailerNumber.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
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

        private void cboLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboLocation.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void cboActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboActive.SelectedIndex == 1)
                gblnActive = true;
            else if (cboActive.SelectedIndex == 2)
                gblnActive = false;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting variables
            string strTrailerNumber;
            int intRecordsReturned;
            string strTrailerCategory;
            int intLocationID;
            bool blnActive;
            int intCounter;
            int intNumberOfRecords;
            
            try
            {
                strTrailerNumber = txtEnterTrailerNumber.Text;
                if(strTrailerNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Trailer Number Was Not Entered");
                    return;
                }

                TheFindTrailerByTrailerNumberDataSet = TheTrailersClass.FindTrailerByTrailerNumber(strTrailerNumber);

                intRecordsReturned = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Trailer Was Not Found");
                    return;
                }

                txtTrailerID.Text = Convert.ToString(TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerID);
                txtTrailerDescription.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerDescription;
                strTrailerCategory = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerCategory;

                intNumberOfRecords = TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(strTrailerCategory == TheFindSortedTrailerCategoryDataSet.FindSortedTrailerCategory[intCounter].TrailerCategory)
                    {
                        cboTrailerCategory.SelectedIndex = intCounter + 1;
                    }
                }

                txtVINNumber.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].VINNumber;
                txtLicensePlate.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].LicensePlate;
                txtTrailerNotes.Text = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerNotes;

                intLocationID = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].LocationID;

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(intLocationID == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID)
                    {
                        cboLocation.SelectedIndex = intCounter + 1;
                    }
                }

                blnActive = TheFindTrailerByTrailerNumberDataSet.FindTrailerByTrailerNumber[0].TrailerActive;

                if (blnActive == true)
                    cboActive.SelectedIndex = 1;
                else if (blnActive == false)
                    cboActive.SelectedIndex = 2;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Trailer // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            string strTrailerNotes;
            int intTrailerID;
            string strErrorMessage = "";

            try
            {
                intTrailerID = Convert.ToInt32(txtTrailerID.Text);

                strTrailerNotes = txtTrailerNotes.Text;

                if(strTrailerNotes == "")
                {
                    strTrailerNotes = "NO NOTES ENTERED";
                }

                if (cboTrailerCategory.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Trailer Type Was Not Selected\n";
                }
                if (cboLocation.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Trailer Location Was Not Selected\n";
                }
                if (cboActive.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Trailer Active Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheTrailersClass.UpdateTrailerActive(intTrailerID, gblnActive);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheTrailersClass.UpdateTrailerLocation(intTrailerID, MainWindow.gintWarehouseID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheTrailersClass.UpdateTrailerNotes(intTrailerID, strTrailerNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Trailer Has Been Updatede");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Edit Trailer // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
