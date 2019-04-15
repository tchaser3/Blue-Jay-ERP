/* Title:           View Selected Vehicle problem
 * Date:            1-10-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to view a selected problem */

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
using VehicleProblemsDLL;
using VehicleProblemDocumentationDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ViewSelectedVehicleProblem.xaml
    /// </summary>
    public partial class ViewSelectedVehicleProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        VehicleProblemDocumentClass TheVehicleProblemDocumentClass = new VehicleProblemDocumentClass();

        //setting up data
        FindVehicleMainProblemUpdateByProblemIDDataSet TheFindVehicleMainProblemUpdateByProblemIDDataSet = new FindVehicleMainProblemUpdateByProblemIDDataSet();
        FindVehicleProblemDocumentationByProblemIDDataSet TheFindVehicleProblemDocumentationByProblemID = new FindVehicleProblemDocumentationByProblemIDDataSet();
        FindVehicleInvoiceByInvoiceIDDataSet TheFindVehicleByInvoiceIDDataSet = new FindVehicleInvoiceByInvoiceIDDataSet();        
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        SelectedProblemUpdatesDataSet TheSelectedProblemUpdatesDataSet = new SelectedProblemUpdatesDataSet();
        SelectedProblemDocumentationDataSet TheSelectedProblemDocumentationDataSet = new SelectedProblemDocumentationDataSet();

        //setting up Global variables
        int gintInvoiceID;

        public ViewSelectedVehicleProblem()
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
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            
            try
            {
                TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(MainWindow.gintProblemID);

                txtProblemID.Text = Convert.ToString(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].ProblemID);
                txtProblem.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;
                txtTransactionDate.Text = Convert.ToString(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].TransactionDAte);
                txtVendor.Text = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].VendorName;

                if(TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].IsInvoiceIDNull() == false)
                {
                    gintInvoiceID = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].InvoiceID;
                }
                else
                {
                    gintInvoiceID = -1;
                }

                TheFindVehicleMainProblemUpdateByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemUpdateByProblemID(MainWindow.gintProblemID);

                intNumberOfRecords = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    SelectedProblemUpdatesDataSet.selectedproblemupdatesRow NewUpdateRow = TheSelectedProblemUpdatesDataSet.selectedproblemupdates.NewselectedproblemupdatesRow();

                    NewUpdateRow.Date = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].TransactionDate;
                    NewUpdateRow.FirstName = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].FirstName;
                    NewUpdateRow.LastName = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].LastName;
                    NewUpdateRow.Updates = TheFindVehicleMainProblemUpdateByProblemIDDataSet.FindVehicleMainProblemUpdateByProblemID[intCounter].ProblemUpdate;

                    TheSelectedProblemUpdatesDataSet.selectedproblemupdates.Rows.Add(NewUpdateRow);
                }

                dgrProblemUpdates.ItemsSource = TheSelectedProblemUpdatesDataSet.selectedproblemupdates;

                TheFindVehicleProblemDocumentationByProblemID = TheVehicleProblemDocumentClass.FindVehicleProblemDocumentationByProblemID(MainWindow.gintProblemID);

                intNumberOfRecords = TheFindVehicleProblemDocumentationByProblemID.FindVehicleProblemDocumentationByProblemID.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        SelectedProblemDocumentationDataSet.problemdocumentationRow NewDocumentRow = TheSelectedProblemDocumentationDataSet.problemdocumentation.NewproblemdocumentationRow();

                        NewDocumentRow.Date = TheFindVehicleProblemDocumentationByProblemID.FindVehicleProblemDocumentationByProblemID[intCounter].TransactionDate;
                        NewDocumentRow.DocumentType = TheFindVehicleProblemDocumentationByProblemID.FindVehicleProblemDocumentationByProblemID[intCounter].DocumentType;
                        NewDocumentRow.DocumentPath = TheFindVehicleProblemDocumentationByProblemID.FindVehicleProblemDocumentationByProblemID[intCounter].DocumentPath;

                        TheSelectedProblemDocumentationDataSet.problemdocumentation.Rows.Add(NewDocumentRow);
                    }
                }

                TheFindVehicleByInvoiceIDDataSet = TheVehicleProblemDocumentClass.FindVehicleInvoiceByInvoiceID(gintInvoiceID);

                intNumberOfRecords = TheFindVehicleByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        SelectedProblemDocumentationDataSet.problemdocumentationRow NewDocumentRow = TheSelectedProblemDocumentationDataSet.problemdocumentation.NewproblemdocumentationRow();

                        NewDocumentRow.Date = TheFindVehicleByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID[intCounter].InvoiceDate;
                        NewDocumentRow.DocumentType = "INVOICE";
                        NewDocumentRow.DocumentPath = TheFindVehicleByInvoiceIDDataSet.FindVehicleInvoiceByInvoiceID[intCounter].InvoicePath;

                        TheSelectedProblemDocumentationDataSet.problemdocumentation.Rows.Add(NewDocumentRow);
                    }                    
                }

                dgrProblemDocumentation.ItemsSource = TheSelectedProblemDocumentationDataSet.problemdocumentation;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Selected Vehicle Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void DgrProblemDocumentation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrProblemDocumentation.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrProblemDocumentation;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // View Selected Vehicle Problem // Vehicle Documentation Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
