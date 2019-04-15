/* Title:           Vehicle Problem Print Class
 * Date:            7-16-18
 * Author:          Terry Holmes
 * 
 * Description:     This is used to for printing a work order */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using NewEventLogDLL;
using VehicleMainDLL;
using VehicleProblemsDLL;
using DateSearchDLL;
using System.Windows.Media;

namespace BlueJayERP
{
    class VehicleProblemPrintClass
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehicleMainProblemUpdateByProblemIDDataSet TheFindVehicleMainProblemUpdateByProblemIDDataSet = new FindVehicleMainProblemUpdateByProblemIDDataSet();
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        PrintProblemUpdateDataSet ThePrintProblemUpdateDataSet = new PrintProblemUpdateDataSet();

        public bool PrintVehicleProblemInfo()
        {
            //setting local controls
            bool blnFatalEDrror = false;
            int intCounter;
            int intNumberOfRecords;
            string strVehicleNumber;
            string strProblem;
            int intCurrentRow = 0;
            int intColumns;
            
            try
            {
                PrintDialog pdVehicleHProblemHistory = new PrintDialog();

                TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(MainWindow.gintProblemID);
                strVehicleNumber = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].VehicleNumber;
                strProblem = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;
                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);
                TheFindVehicleMainProblemUpdateByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemUpdateByProblemID(MainWindow.gintProblemID);
                ThePrintProblemUpdateDataSet.problemupdate.Rows.Clear();

                intNumberOfRecords = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    PrintProblemUpdateDataSet.problemupdateRow NewUpdateRow = ThePrintProblemUpdateDataSet.problemupdate.NewproblemupdateRow();

                    NewUpdateRow.TransactionDate = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].TransactionDate;
                    NewUpdateRow.FirstName = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].FirstName;
                    NewUpdateRow.LastName = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].LastName;
                    NewUpdateRow.Update = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].ProblemUpdate;

                    ThePrintProblemUpdateDataSet.problemupdate.Rows.Add(NewUpdateRow);
                }

                if (pdVehicleHProblemHistory.ShowDialog().Value)
                {
                    FlowDocument fdAcceptLetter = new FlowDocument();
                    Paragraph Title = new Paragraph(new Run("BLUE JAY COMMUNICATIONS, INC"));
                    Title.FontSize = 20;
                    Title.TextAlignment = TextAlignment.Center;
                    Title.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(Title);
                    Paragraph Title2 = new Paragraph(new Run("7500 Associates Avenue"));
                    Title2.FontSize = 16;
                    Title2.LineHeight = 1;
                    Title2.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title2);
                    Paragraph Title3 = new Paragraph(new Run("Brooklyn, OH 44144"));
                    Title3.FontSize = 16;
                    Title3.LineHeight = 1;
                    Title3.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title3);
                    fdAcceptLetter.ColumnWidth = 900;
                    Paragraph Space1 = new Paragraph(new Run());
                    Space1.LineHeight = 1;
                    fdAcceptLetter.Blocks.Add(Space1);
                    Paragraph Title4 = new Paragraph(new Run(""));
                    Title4.TextDecorations = TextDecorations.Underline;
                    Title4.FontSize = 16;
                    Title4.LineHeight = 1;
                    Title4.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title4);
                    Paragraph Title5 = new Paragraph(new Run("Problem Number " + Convert.ToString(MainWindow.gintProblemID) + " For Vehicle " + strVehicleNumber));
                    Title5.TextDecorations = TextDecorations.Underline;
                    Title5.FontSize = 16;
                    Title5.LineHeight = 1;
                    Title5.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title5);
                    Paragraph Title6 = new Paragraph(new Run("For The Problem Of " + strProblem));
                    Title6.TextDecorations = TextDecorations.Underline;
                    Title6.FontSize = 16;
                    Title6.LineHeight = 1;
                    Title6.TextAlignment = TextAlignment.Center;
                    fdAcceptLetter.Blocks.Add(Title6);
                    Paragraph Space2 = new Paragraph(new Run());
                    Space1.LineHeight = 2;
                    fdAcceptLetter.Blocks.Add(Space2);

                    //getting the customer information                 
                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdAcceptLetter.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = ThePrintProblemUpdateDataSet.problemupdate.Columns.Count;
                    fdAcceptLetter.ColumnWidth = 10;
                    fdAcceptLetter.IsColumnWidthFlexible = false;


                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        ProjectReportTable.Columns.Add(new TableColumn());
                    }
                    ProjectReportTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Nasme"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Problem"))));

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = ThePrintProblemUpdateDataSet.problemupdate.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(ThePrintProblemUpdateDataSet.problemupdate[intReportRowCounter][intColumnCounter].ToString()))));

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
                    fdAcceptLetter.ColumnWidth = pdVehicleHProblemHistory.PrintableAreaWidth;
                    fdAcceptLetter.PageHeight = pdVehicleHProblemHistory.PrintableAreaHeight;
                    fdAcceptLetter.PageWidth = pdVehicleHProblemHistory.PrintableAreaWidth;
                    intCurrentRow = 0;

                    pdVehicleHProblemHistory.PrintDocument(((IDocumentPaginatorSource)fdAcceptLetter).DocumentPaginator, "Blue Jay Communications Acceptance");

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Problem Print Class // Create MDU Drop Acceptance Letter " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalEDrror = true;
            }

            return blnFatalEDrror;
        }
    }
}
