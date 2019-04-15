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
using VehicleBulkToolsDLL;
using NewEventLogDLL;
using DataValidationDLL;
using WeeklyBulkToolInspectionDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for BulkToolsAssignedToVehicle.xaml
    /// </summary>
    public partial class BulkToolsAssignedToVehicle : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WeeklyBulkToolInspectionClass TheWeeklyBulkToolInspectionClass = new WeeklyBulkToolInspectionClass();

        FindVehicleBulkToolByVehicleNumberDataSet TheFindVehicleBulkToolByVehicleNumberDataSet = new FindVehicleBulkToolByVehicleNumberDataSet();
        BulkToolsOnVehicleDataSet TheBulkToolsOnVehicleDataSet = new BulkToolsOnVehicleDataSet();


        public BulkToolsAssignedToVehicle()
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

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intCones = 0;
            int intSignage = 0;
            int intFireExtinguisher = 0;
            int intFirstAidKits = 0;
            string strNotes = "";
            int intMissingTotal;
            bool blnToolsPresent = true;
            bool blnConesCorrect = true;
            bool blnSignsCorrect = true;
            bool blnFirstAidCorrect = true;
            bool blnFireExtinguisherCorrect = true;

            try
            {
                TheFindVehicleBulkToolByVehicleNumberDataSet = TheVehicleBulkToolsClass.FindVehicleBulkToolByVehicleNumber(MainWindow.gstrVehicleNumber);

                strValueForValidation = txtCones.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Number Of Cones is not an Integer\n";                    
                }
                else
                {
                    intCones = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtSignage.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Number Of Signs is not an Integer\n";
                }
                else
                {
                    intSignage = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtFireExtinguisher.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Number Of Fire Extinguishers is not an Integer\n";
                }
                else
                {
                    intFireExtinguisher = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtFirstAidKit.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Number Of First Aid Kits is not an Integer\n";
                }
                else
                {
                    intFirstAidKits = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strNotes += txtNotes.Text + "\n";

                intNumberOfRecords = TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber.Rows.Count - 1;                

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].ToolCategory == "CONE")
                        {
                            if(intCones != TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                            {
                                if(intCones < TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                                {
                                    intMissingTotal = TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity - intCones;

                                    strNotes += "There are " + Convert.ToString(intMissingTotal) + " Missing Cones\n";

                                    blnConesCorrect = false;
                                    
                                    blnToolsPresent = false;
                                }
                            }

                        }
                        else if(TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].ToolCategory == "FIRE EXTINGUISHER")
                        {
                            if (intFireExtinguisher != TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                            {
                                if (intFireExtinguisher < TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                                {
                                    intMissingTotal = TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity - intFireExtinguisher;

                                    strNotes += "There are " + Convert.ToString(intMissingTotal) + " Missing Fire Extinguishers\n";

                                    blnToolsPresent = false;

                                    blnSignsCorrect = false;
                                }
                                else
                                {
                                    intMissingTotal = intFireExtinguisher - TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity;

                                    strNotes += "The Vehicled Has " + Convert.ToString(intMissingTotal) + " Excess Extinguishers\n";

                                    blnToolsPresent = false;

                                    blnSignsCorrect = false;
                                }
                            }
                        }
                        else if (TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].ToolCategory == "FIRST AID KIT")
                        {
                            if (intFirstAidKits != TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                            {
                                if (intFirstAidKits < TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                                {
                                    intMissingTotal = TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity - intFirstAidKits;

                                    strNotes += "There are " + Convert.ToString(intMissingTotal) + " Missing First Aid Kits\n";

                                    blnToolsPresent = false;

                                    blnFirstAidCorrect = false;
                                }
                                else
                                {
                                    intMissingTotal = intFirstAidKits - TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity;

                                    strNotes += "The Vehicled Has " + Convert.ToString(intMissingTotal) + " Excess First Aid Kits\n";

                                    blnToolsPresent = false;

                                    blnFirstAidCorrect = false;
                                }
                            }
                        }
                        else if (TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].ToolCategory == "SIGN")
                        {
                            if (intSignage != TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                            {
                                if (intSignage < TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity)
                                {
                                    intMissingTotal = TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity - intSignage;

                                    strNotes += "There are " + Convert.ToString(intMissingTotal) + " Missing Signs\n";

                                    blnToolsPresent = false;

                                    blnSignsCorrect = false;
                                }
                                else
                                {
                                    intMissingTotal = intSignage - TheFindVehicleBulkToolByVehicleNumberDataSet.FindVehicleBulkToolByVehicleNumber[intCounter].Quantity;

                                    strNotes += "The Vehicled Has " + Convert.ToString(intMissingTotal) + " Excess Signs\n";

                                    blnToolsPresent = false;

                                    blnSignsCorrect = false;
                                }
                            }

                        }
                    }
                }
                else
                {
                    if(intFirstAidKits > 0)
                    {
                        strNotes += "There are " + Convert.ToString(intCones) + " Excess First Aid Kits\n";

                        blnToolsPresent = false;

                        blnFirstAidCorrect = false;
                    }
                    if (intFireExtinguisher > 0)
                    {
                        strNotes += "There are " + Convert.ToString(intCones) + " Excess Fire Extinguisers\n";

                        blnToolsPresent = false;

                        blnFireExtinguisherCorrect = false;
                    }
                    if(intSignage > 0)
                    {
                        strNotes += "There are " + Convert.ToString(intCones) + " Excess Signs\n";

                        blnToolsPresent = false;

                        blnSignsCorrect = false;
                    }
                }

                blnFatalError = TheWeeklyBulkToolInspectionClass.InsertWeeklyBulkVehicleToolInspection(MainWindow.gintInspectionID, MainWindow.gintVehicleID, blnToolsPresent, strNotes.ToUpper(), blnConesCorrect, blnSignsCorrect, blnFirstAidCorrect, blnFireExtinguisherCorrect);

                if (blnFatalError == true)
                    throw new Exception();

                if(blnToolsPresent == false)
                {
                    TheMessagesClass.ErrorMessage("There is a Problem with the Tools\n " + strNotes);
                }

                Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Bulk Tools Assigned To Vehicle // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCones.Text = "0";
            txtFireExtinguisher.Text = "0";
            txtFirstAidKit.Text = "0";
            txtSignage.Text = "0";
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
