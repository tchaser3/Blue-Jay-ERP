﻿/* Title:           Vehicles In Yard
 * Date:            4-13-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to enter vehicles in the yard*/

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
using VehicleInYardDLL;
using VehicleMainDLL;
using NewEventLogDLL;
using DataValidationDLL;
using DateSearchDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for VehiclesInYard.xaml
    /// </summary>
    public partial class VehiclesInYard : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehiclesInYardByVehicleIDAndDateRangeDataSet TheFindVehiclesInYardByVehicleIDAndDateRangeDataSet = new FindVehiclesInYardByVehicleIDAndDateRangeDataSet();

        public VehiclesInYard()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void txtEnterVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intLength;


            strValueForValidation = txtEnterVehicleNumber.Text;
            intLength = strValueForValidation.Length;

            if (intLength == 6)
            {
                mitProcess.Focus();
            }
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strVehicleNumber;
            int intRecordsReturned;
            int intVehicleID;
            DateTime datTransactionDate;
            bool blnFatalError = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                strVehicleNumber = txtEnterVehicleNumber.Text;
                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                if(strVehicleNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Was Not Entered");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Number Entered Does Not Exist");
                    return;
                }
                else if (intRecordsReturned == 1)
                {
                    intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheFindVehiclesInYardByVehicleIDAndDateRangeDataSet = TheVehicleInYardClass.FindVehiclesInYardByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                    intRecordsReturned = TheFindVehiclesInYardByVehicleIDAndDateRangeDataSet.FindVehiclesInYardByVehicleIDAndDateRange.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Has Already Been Placed In The Yard");
                        return;
                    }

                    datTransactionDate = DateTime.Now;

                    blnFatalError = TheVehicleInYardClass.InsertVehicleInYard(datTransactionDate, intVehicleID);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Was a Problem, Contact IT");
                        return;
                    }
                }

                txtEnterVehicleNumber.Text = "";

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle In Yard // Process Menu Item " + Ex.Message);

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
