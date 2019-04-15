/* Title:           Create Tool Category
 * Date:            11-14-18
 * Author:          Terry Holmes
 * 
 * Description:     This windows is for Creating a Tool Category */

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
using ToolCategoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateToolCategory.xaml
    /// </summary>
    public partial class CreateToolCategory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();

        FindToolCategoryByCategoryDataSet TheFindToolCategoryByCategoryDataSet = new FindToolCategoryByCategoryDataSet();

        public CreateToolCategory()
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

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtToolCategory.Text = "";
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            string strToolCategory;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                strToolCategory = txtToolCategory.Text;
                if(strToolCategory == "")
                {
                    TheMessagesClass.ErrorMessage("The Tool Category was not Entered");
                    return;
                }

                TheFindToolCategoryByCategoryDataSet = TheToolCategoryClass.FindToolCategoryByCategory(strToolCategory);

                intRecordsReturned = TheFindToolCategoryByCategoryDataSet.FindToolCategoryByCategory.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("Tool Category Already Exists");
                    return;
                }

                blnFatalError = TheToolCategoryClass.InsertToolCategory(strToolCategory);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Tool Category Has Been Saved");

                txtToolCategory.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Crate Tool Category // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
