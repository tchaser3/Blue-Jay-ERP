/* Title:           Automated Vehicle Reports
 * Date:            4-25-18
 * Author:          Terry Holmes
 * 
 * Description:     This the autommated report for emailing */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DateSearchDLL;
using InspectionsDLL;
using NewEventLogDLL;
using VehicleMainDLL;
using DataValidationDLL;
using NewEmployeeDLL;
using VehicleInYardDLL;
using VehiclesInShopDLL;
using VehicleAssignmentDLL;

namespace BlueJayERP
{
    class AutomatedVehicleReports
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        VehiclesInShopClass TheVehicleInShopClass = new VehiclesInShopClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindDailyVehicleInspectionByDateRangeDataSet TheFindDailyVehicleInspectionByDateRangeDataSet = new FindDailyVehicleInspectionByDateRangeDataSet();
        FindVehicleInspectionProblemsByInspectionIDDataSet TheFindVehicleInspectionProblemByInsepctionIDDataSet = new FindVehicleInspectionProblemsByInspectionIDDataSet();
        FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = new FindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet();
        FindDailyVehicleInspectionsByEmployeeIDAndDateRangeDataSet TheFindDailyVehicleInspectionByEmployeeIDAndDateRangeDataSet = new FindDailyVehicleInspectionsByEmployeeIDAndDateRangeDataSet();
        DailyVehicleInspectionReportDataSet TheDailyVehicleInspectionReportDataSet = new DailyVehicleInspectionReportDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveVehicleMainDataSet TheFindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        VehicleExceptionDataSet TheVehicleExceptionDataSet = new VehicleExceptionDataSet();
        FindVehiclesInYardByVehicleIDAndDateRangeDataSet TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = new FindVehiclesInYardByVehicleIDAndDateRangeDataSet();
        FindVehiclesInShopByVehicleIDDataSet TheFindVehicleInShopByVehicleIDDataSet = new FindVehiclesInShopByVehicleIDDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindcurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();

        
        public void RunAutomatedReports(DateTime datStartDate)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = RunDailyVehicleInspectionReport(datStartDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = EmailDailyVehicleInspectionReport();

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = RunVehicleExceptionReport(datStartDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = EmailVehicleExceptionReport();

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch(Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private bool EmailVehicleExceptionReport()
        {
            bool blnFatalError = false;

            int intCounter;
            int intNumberOfRecords;
            string strMessage = "";
            string strHeader = "Vehicle Exception Report";

            try
            {
                intNumberOfRecords = TheVehicleExceptionDataSet.vehicleexception.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strMessage += TheVehicleExceptionDataSet.vehicleexception[intCounter].VehicleNumber + "\t\t\t";
                    strMessage += TheVehicleExceptionDataSet.vehicleexception[intCounter].FirstName + " ";
                    strMessage += TheVehicleExceptionDataSet.vehicleexception[intCounter].LastName + "\t\t\t\t";
                    strMessage += TheVehicleExceptionDataSet.vehicleexception[intCounter].AssignedOffice + "\n";
                }

                TheSendEmailClass.VehicleReports(strHeader, strMessage);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Automate Vehicle Reports // Email Vehicle Exception Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            return blnFatalError;
        }
        private bool RunVehicleExceptionReport(DateTime datStartDate)
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID;
            int intRecordsReturned;
            DateTime datEndDate;
            string strFirstNamed = "";
            string strLastName = "";

            try
            {
                TheVehicleExceptionDataSet.vehicleexception.Rows.Clear();

                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                TheFindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();

                intNumberOfRecords = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleID;

                    TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet = TheInspectionClass.FindDailyVehicleInspectionByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                    intRecordsReturned = TheFindDailyVehicleInspectionByVehicleIDAndDateRangeDataSet.FindDailyVehicleInspectionsByVehicleIDAndDateRange.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheFindVehicleInYardByVehicleIDAndDateRangeDataSet = TheVehicleInYardClass.FindVehiclesInYardByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                        intRecordsReturned = TheFindVehicleInYardByVehicleIDAndDateRangeDataSet.FindVehiclesInYardByVehicleIDAndDateRange.Rows.Count;

                        if (intRecordsReturned == 0)
                        {
                            TheFindVehicleInShopByVehicleIDDataSet = TheVehicleInShopClass.FindVehiclesInShopByVehicleID(intVehicleID);

                            intRecordsReturned = TheFindVehicleInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

                            if (intRecordsReturned == 0)
                            {
                                TheFindcurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(intVehicleID);

                                intRecordsReturned = TheFindcurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID.Rows.Count;

                                if (intRecordsReturned == 0)
                                {
                                    strLastName = "NOT ASSIGNED";
                                    strFirstNamed = "NOT ASSIGNED";
                                }
                                else
                                {
                                    strLastName = TheFindcurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].LastName;
                                    strFirstNamed = TheFindcurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].FirstName;
                                }

                                VehicleExceptionDataSet.vehicleexceptionRow NewVehicleRow = TheVehicleExceptionDataSet.vehicleexception.NewvehicleexceptionRow();

                                NewVehicleRow.AssignedOffice = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].AssignedOffice;
                                NewVehicleRow.FirstName = strFirstNamed;
                                NewVehicleRow.LastName = strLastName;
                                NewVehicleRow.VehicleID = intVehicleID;
                                NewVehicleRow.VehicleNumber = TheFindActiveVehicleMainDataSet.FindActiveVehicleMain[intCounter].VehicleNumber;

                                TheVehicleExceptionDataSet.vehicleexception.Rows.Add(NewVehicleRow);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Automate Vehicle Reports // Run Vehicle Exception Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool RunDailyVehicleInspectionReport(DateTime datStartDate)
        {
            //setting local varaibles
             DateTime datLimitDate;
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intInspectionID;
            bool blnFatalError = false;

            try
            {
                TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Clear();
                datLimitDate = TheDataSearchClass.AddingDays(datStartDate, 1);

                TheFindDailyVehicleInspectionByDateRangeDataSet = TheInspectionClass.FindDailyVehicleInspectionByDateRange(datStartDate, datLimitDate);

                intNumberOfRecords = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        DailyVehicleInspectionReportDataSet.dailyinspectionRow NewInspectionRow = TheDailyVehicleInspectionReportDataSet.dailyinspection.NewdailyinspectionRow();

                        intInspectionID = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].TransactionID;

                        TheFindVehicleInspectionProblemByInsepctionIDDataSet = TheInspectionClass.FindVehicleInspectionProblemsbyInspectionID(intInspectionID);

                        intRecordsReturned = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID.Rows.Count;

                        if (intRecordsReturned == 0)
                        {
                            NewInspectionRow.InspectionNotes = "NO NOTES REPORTED";
                        }
                        else
                        {
                            NewInspectionRow.InspectionNotes = TheFindVehicleInspectionProblemByInsepctionIDDataSet.FindVehicleInspectionProblemsByInspectionID[0].InspectionNotes;
                        }

                        NewInspectionRow.FirstName = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].FirstName;
                        NewInspectionRow.InspectionDate = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].InspectionDate;
                        NewInspectionRow.InspectionID = intInspectionID;
                        NewInspectionRow.InspectionStatus = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].InspectionStatus;
                        NewInspectionRow.LastName = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].LastName;
                        NewInspectionRow.OdometerReading = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].OdometerReading;
                        NewInspectionRow.VehicleID = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].VehicleID;
                        NewInspectionRow.VehicleNumber = TheFindDailyVehicleInspectionByDateRangeDataSet.FindDailyVehicleInspectionByDateRange[intCounter].VehicleNumber;

                        TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Add(NewInspectionRow);
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Automated Vehicle Reports // Run Daily Vehicle Inspection Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool EmailDailyVehicleInspectionReport()
        {
            int intCounter;
            int intNumberOfRecords;
            string strMessage = "";
            string strHeader = "Daily Vehicle Inspection Report";
            bool blnFatalError = false;

            try
            {
                intNumberOfRecords = TheDailyVehicleInspectionReportDataSet.dailyinspection.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strMessage += TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].VehicleNumber + "\t";
                    strMessage += TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].FirstName + " ";
                    strMessage += TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].LastName + "\t\t\t";
                    strMessage += Convert.ToString(TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].OdometerReading) + "\t\t";
                    strMessage += TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].InspectionStatus + "\t";
                    strMessage += TheDailyVehicleInspectionReportDataSet.dailyinspection[intCounter].InspectionNotes + "\n";
                }

                TheSendEmailClass.VehicleReports(strHeader, strMessage);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Automated Vehicle Reports  // Email Daily Vehicle Inspection Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
    }
}
