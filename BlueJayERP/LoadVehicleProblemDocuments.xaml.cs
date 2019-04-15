/* Title:           Load View Vehicle Problem Documents
 * Date:            12-17-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the window that you can view or load documents */

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
using VehicleProblemsDLL;
using VehicleProblemDocumentationDLL;
using NewEventLogDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for LoadVehicleProblemDocuments.xaml
    /// </summary>
    public partial class LoadVehicleProblemDocuments : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleProblemDocumentClass TheVehicleProblemDocumentClass = new VehicleProblemDocumentClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();

        //setting up the data
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        FindVehicleProblemDocumentationByProblemIDDataSet TheFindVehicleProblemDocumentationByProblemIDDataSet = new FindVehicleProblemDocumentationByProblemIDDataSet();

        string gstrDocumentType;

        public LoadVehicleProblemDocuments()
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
            try
            {
                TheFindVehicleProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(MainWindow.gintProblemID);
                txtVehicleProblem.Text = TheFindVehicleProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;
                txtVehicleNumber.Text = TheFindVehicleProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].VehicleNumber;

                TheFindVehicleProblemDocumentationByProblemIDDataSet = TheVehicleProblemDocumentClass.FindVehicleProblemDocumentationByProblemID(MainWindow.gintProblemID);

                dgrResults.ItemsSource = TheFindVehicleProblemDocumentationByProblemIDDataSet.FindVehicleProblemDocumentationByProblemID;

                cboSelectDocumentType.Items.Add("Select Document Type");
                cboSelectDocumentType.Items.Add("DOCUMENT");
                cboSelectDocumentType.Items.Add("PICTURE");
                cboSelectDocumentType.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Load View Vehicle Problem Documentation // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void BtnUploadDocument_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datTransactionDate = DateTime.Now;
            string strDocumentPath = "";
            string strDocumentType;
            bool blnRightFormat = false;
            long intResult;
            string strNewLocation = "";
            string strTransactionName;
            bool blnFatalError;
            string strVehicleNumber;
            string strProblem;

            try
            {
                strDocumentType = cboSelectDocumentType.SelectedItem.ToString();
                strVehicleNumber = TheFindVehicleProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].VehicleNumber;
                strProblem = TheFindVehicleProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;
                strTransactionName = strVehicleNumber + " " + strProblem + " ";

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    strDocumentPath = dlg.FileName.ToUpper();
                }
                else
                {
                    return;
                }

                if (strDocumentPath.Contains("JPG"))
                {
                    if (gstrDocumentType == "PICTURE")
                    {
                        blnRightFormat = true;
                    }
                }
                else if (strDocumentPath.Contains("PDF"))
                {
                    if (gstrDocumentType == "DOCUMENT")
                    {
                        blnRightFormat = true;
                    }
                }

                if (blnRightFormat == false)
                {
                    TheMessagesClass.ErrorMessage("File Type is not Correct");

                    return;
                }

                if (strDocumentType == "DOCUMENT")
                {
                    datTransactionDate = DateTime.Now;

                    intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                    strTransactionName += Convert.ToString(intResult);

                    strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\VehicleProblemFiles\\" + strTransactionName + ".pdf";
                }
                if (strDocumentType == "PICTURE")
                {
                    datTransactionDate = DateTime.Now;

                    intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                    strTransactionName += Convert.ToString(intResult);

                    strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\VehicleProblemFiles\\" + strTransactionName + ".jpg";
                }

                System.IO.File.Copy(strDocumentPath, strNewLocation);

                blnFatalError = TheVehicleProblemDocumentClass.InsertVehicleProblemDocumentation(datTransactionDate, MainWindow.gintVehicleID, MainWindow.gintProblemID, strDocumentType, strNewLocation);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindVehicleProblemDocumentationByProblemIDDataSet = TheVehicleProblemDocumentClass.FindVehicleProblemDocumentationByProblemID(MainWindow.gintProblemID);

                dgrResults.ItemsSource = TheFindVehicleProblemDocumentationByProblemIDDataSet.FindVehicleProblemDocumentationByProblemID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Add Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void CboSelectDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectDocumentType.SelectedIndex > 0)
                gstrDocumentType = cboSelectDocumentType.SelectedItem.ToString().ToUpper();
        }

        private void DgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrResults.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[5].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Load Vehicle Problem Document // Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
