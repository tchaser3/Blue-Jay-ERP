/* Title:           Project Costing
 * Date:            11-13-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to cost the project */

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
using ProjectsDLL;
using ProjectCostingDLL;
using Microsoft.Win32;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ProjectCosting.xaml
    /// </summary>
    public partial class ProjectCosting : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProjectCostingClass TheProjectCostingClass = new ProjectCostingClass();

        //setting up data
        FindProjectHoursCostingDataSet TheFindProjectHoursCostingDataSet = new FindProjectHoursCostingDataSet();
        FindProjectMaterialCostingDataSet TheFindProjectMaterialCostingDataSet = new FindProjectMaterialCostingDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        ProjectMaterialCostingDataSet TheProjectMaterialCostingDataSet = new ProjectMaterialCostingDataSet();

        decimal gdecTotalHours;
        decimal gdecTotalCost;

        public ProjectCosting()
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
            txtEnterAssignedProjectID.Text = "";
            txtTotalHOurs.Text = "";
            txtTotalMaterialCost.Text = "";
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strAssignedpProjectID;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            double douPrice;
            double douTotalPrice;

            try
            {
                TheProjectMaterialCostingDataSet.projectmateriacosting.Rows.Clear();
                gdecTotalCost = 0;
                gdecTotalHours = 0;
                strAssignedpProjectID = txtEnterAssignedProjectID.Text;
                if(strAssignedpProjectID == "")
                {
                    TheMessagesClass.ErrorMessage("The Assigned Project ID Was Not Entered");
                    return;
                }

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strAssignedpProjectID);

                intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Project Not Found");
                    return;
                }

                TheFindProjectHoursCostingDataSet = TheProjectCostingClass.FindProjectHoursCosting(strAssignedpProjectID);

                intNumberOfRecords = TheFindProjectHoursCostingDataSet.FindProjectHoursCosting.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        gdecTotalHours += TheFindProjectHoursCostingDataSet.FindProjectHoursCosting[intCounter].TotalHours;
                    }
                }

                txtTotalHOurs.Text = Convert.ToString(gdecTotalHours);

                dgrHours.ItemsSource = TheFindProjectHoursCostingDataSet.FindProjectHoursCosting;

                TheFindProjectMaterialCostingDataSet = TheProjectCostingClass.FindProjectMaterialCosting(strAssignedpProjectID);

                intNumberOfRecords = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        douPrice = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting[intCounter].Price;
                        douPrice = Math.Round(douPrice, 2);

                        douTotalPrice = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting[intCounter].PartPrice;
                        douTotalPrice = Math.Round(douTotalPrice, 2);

                        gdecTotalCost += Convert.ToDecimal(douTotalPrice);

                        ProjectMaterialCostingDataSet.projectmateriacostingRow NewPartCost = TheProjectMaterialCostingDataSet.projectmateriacosting.NewprojectmateriacostingRow();

                        NewPartCost.PartID = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting[intCounter].PartID;
                        NewPartCost.PartNumber = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting[intCounter].PartNumber;
                        NewPartCost.PartDescription = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting[intCounter].PartDescription;
                        NewPartCost.TotalQuantity = TheFindProjectMaterialCostingDataSet.FindProjectMaterialCosting[intCounter].TotalQuantity;
                        NewPartCost.TotalCost = Convert.ToDecimal(douTotalPrice);
                        NewPartCost.Cost = Convert.ToDecimal(douPrice);

                        //adding the row
                        TheProjectMaterialCostingDataSet.projectmateriacosting.Rows.Add(NewPartCost);
                    }
                }

                txtTotalMaterialCost.Text = Convert.ToString(gdecTotalCost);

                dgrMaterials.ItemsSource = TheProjectMaterialCostingDataSet.projectmateriacosting;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Project Costing // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;


            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            Microsoft.Office.Interop.Excel._Worksheet secondworksheet = null;

            try
            {
                worksheet = workbook.ActiveSheet;

                worksheet.Name = "Project Hours";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheFindProjectHoursCostingDataSet.FindProjectHoursCosting.Rows.Count;
                intColumnNumberOfRecords = TheFindProjectHoursCostingDataSet.FindProjectHoursCosting.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindProjectHoursCostingDataSet.FindProjectHoursCosting.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }
                
                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindProjectHoursCostingDataSet.FindProjectHoursCosting.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                workbook.Worksheets.Add(worksheet);

                secondworksheet = workbook.ActiveSheet;
                
                secondworksheet.Name = "Project Material";

                cellRowIndex = 1;
                cellColumnIndex = 1;
                intRowNumberOfRecords = TheProjectMaterialCostingDataSet.projectmateriacosting.Rows.Count;
                intColumnNumberOfRecords = TheProjectMaterialCostingDataSet.projectmateriacosting.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    secondworksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectMaterialCostingDataSet.projectmateriacosting.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        secondworksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectMaterialCostingDataSet.projectmateriacosting.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                workbook.Worksheets.Add(secondworksheet);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Employee Hours // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
