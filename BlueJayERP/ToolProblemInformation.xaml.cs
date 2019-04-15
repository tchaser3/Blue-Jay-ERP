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
using ToolProblemDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ToolProblemInformation.xaml
    /// </summary>
    public partial class ToolProblemInformation : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolProblemClass TheToolProblemClass = new ToolProblemClass();

        FindToolProblemUpdateByProblemIDDataSet TheFindToolProblemUpdateByPRoblemIDDataSet = new FindToolProblemUpdateByProblemIDDataSet();
        FindToolProblemDocumentByProblemIDDataSet TheFindToolProblemDocumentByProblemIDDataSet = new FindToolProblemDocumentByProblemIDDataSet();

        int gintUpdateTotalRecords;
        int gintUpdatetCurrentRecord;
        int gintDocumentTotalRecords;
        int gintDocumentCurrrentRecord;

        public ToolProblemInformation()
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
            string strFullName;

            try
            {
                btnBack.IsEnabled = false;
                btnNext.IsEnabled = false;
                txtToolDescription.Text = MainWindow.gstrToolDescription;
                txtToolID.Text = MainWindow.gstrToolID;

                TheFindToolProblemUpdateByPRoblemIDDataSet = TheToolProblemClass.FindToolProblemUpdateByProblemID(MainWindow.gintProblemID);

                gintUpdateTotalRecords = TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID.Rows.Count - 1;
                gintUpdatetCurrentRecord = 0;

                if(gintUpdateTotalRecords > 0)
                {
                    btnNext.IsEnabled = true;
                }

                strFullName = TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[0].FirstName + " " + TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[0].LastName;

                txtEmployee.Text = strFullName;
                txtTransactionDate.Text = Convert.ToString(TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[0].TransactionDate);
                txtNotes.Text = TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[0].UpdateNotes;

                LoadPictureControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tool Problem Information // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadPictureControls()
        {
            int intCounter;
            int intNumberOfRecords;
            string strDocumentType;
            bool blnImageLoaded = false;

            try
            {
                TheFindToolProblemDocumentByProblemIDDataSet = TheToolProblemClass.FindToolProblemDocumentByProblemID(MainWindow.gintProblemID);

                intNumberOfRecords = TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID.Rows.Count - 1;
                gintDocumentTotalRecords = intNumberOfRecords;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strDocumentType = TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[intCounter].DocumentType;

                    if(strDocumentType == "DOCUMENT")
                    {
                        System.Diagnostics.Process.Start(TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[intCounter].DocumentPath);
                    }
                    else if(strDocumentType == "PICTURE")
                    {
                        if(blnImageLoaded == false)
                        {
                            gintDocumentCurrrentRecord = intCounter;
                            ImageSourceConverter c = new ImageSourceConverter();
                            picToolProblem.Source = (ImageSource)c.ConvertFromString(TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[intCounter].DocumentPath);
                            blnImageLoaded = true;    
                        }
                    }
                }
                if (gintDocumentCurrrentRecord == gintDocumentTotalRecords)
                {
                    btnPictureBack.IsEnabled = false;
                    btnPictureNext.IsEnabled = false;
                }
                else if ((gintDocumentTotalRecords > 0) && (blnImageLoaded == true))
                {
                    btnPictureNext.IsEnabled = true;
                    btnPictureBack.IsEnabled = false;
                }                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tool Problem Information // Load Picture Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnPictureNext_Click(object sender, RoutedEventArgs e)
        {
            bool blnIsImage = false;

            try
            {
                while (blnIsImage == false)
                {
                    gintDocumentCurrrentRecord++;

                    if (TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[gintDocumentCurrrentRecord].DocumentType == "PICTURE")
                    {
                        ImageSourceConverter c = new ImageSourceConverter();
                        picToolProblem.Source = (ImageSource)c.ConvertFromString(TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[gintDocumentCurrrentRecord].DocumentPath);
                        btnPictureBack.IsEnabled = true;
                        blnIsImage = true;                        
                    }

                    if (gintDocumentCurrrentRecord == gintDocumentTotalRecords)
                    {
                        btnPictureNext.IsEnabled = false;
                        blnIsImage = true;
                    }

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tool Problem Information // Picture Next Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }                 
        }

        private void btnPictureBack_Click(object sender, RoutedEventArgs e)
        {
            bool blnIsImage = false;

            try
            {
                while (blnIsImage == false)
                {
                    gintDocumentCurrrentRecord--;

                    if (TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[gintDocumentCurrrentRecord].DocumentType == "PICTURE")
                    {
                        ImageSourceConverter c = new ImageSourceConverter();
                        picToolProblem.Source = (ImageSource)c.ConvertFromString(TheFindToolProblemDocumentByProblemIDDataSet.FindToolProblemDocumentByProblemID[gintDocumentCurrrentRecord].DocumentPath);
                        btnPictureNext.IsEnabled = true;
                        blnIsImage = true;                        
                    }

                    if (gintDocumentCurrrentRecord == 0)
                    {
                        btnPictureBack.IsEnabled = false;
                        blnIsImage = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Tool Problem Information // Picture Back Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //this will load up the update controls
            gintUpdatetCurrentRecord++;
            btnBack.IsEnabled = true;

            txtTransactionDate.Text = Convert.ToString(TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[gintUpdatetCurrentRecord].TransactionDate);
            txtNotes.Text = TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[gintUpdatetCurrentRecord].UpdateNotes;

            if(gintUpdatetCurrentRecord == gintUpdateTotalRecords)
            {
                btnNext.IsEnabled = false;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            gintUpdatetCurrentRecord--;
            btnNext.IsEnabled = true;

            txtTransactionDate.Text = Convert.ToString(TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[gintUpdatetCurrentRecord].TransactionDate);
            txtNotes.Text = TheFindToolProblemUpdateByPRoblemIDDataSet.FindToolProblemUpdateByProblemID[gintUpdatetCurrentRecord].UpdateNotes;

            if (gintUpdatetCurrentRecord == 0)
            {
                btnBack.IsEnabled = false;
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
