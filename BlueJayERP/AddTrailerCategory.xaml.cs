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
using TrailerCategoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for AddTrailerCategory.xaml
    /// </summary>
    public partial class AddTrailerCategory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailerCategoryClass TheTrailerCategoryClass = new TrailerCategoryClass();

        //setting up the data
        FindTrailerCategoryByCategoryDataSet TheFindTrailerCategoryByCategoryDataSet = new FindTrailerCategoryByCategoryDataSet();

        public AddTrailerCategory()
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

        private void mitSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intRecordsReturned;
            string strCategory;
            bool blnFatalError = false;

            try
            {
                strCategory = txtCategory.Text;
                if(strCategory == "")
                {
                    TheMessagesClass.ErrorMessage("Trailer Category was not Entered");
                    return;
                }

                TheFindTrailerCategoryByCategoryDataSet = TheTrailerCategoryClass.FindTrailerCategoryByCategory(strCategory);

                intRecordsReturned = TheFindTrailerCategoryByCategoryDataSet.FindTrailerCategoryByCategory.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Trailer Category Exists");
                    return;
                }

                blnFatalError = TheTrailerCategoryClass.InsertTrailerCategory(strCategory);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Trailer Category Has Been Added");

                txtCategory.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Add Trailer Category // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtCategory.Text = "";
        }
    }
}
