/* Title:           Find Available Tools
 * Date:            6-7-18
 * Author:          Terry Holmes
 * 
 * Description:     This is to see the tool availablility */

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
using NewToolsDLL;
using NewEventLogDLL;
using ToolCategoryDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for FindAvailableTools.xaml
    /// </summary>
    public partial class FindAvailableTools : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolsClass TheToolsClass = new ToolsClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();

        FindAvailableActiveToolsDataSet TheFindAvailableActiveToolsDataSet = new FindAvailableActiveToolsDataSet();
        FindAvailableActiveToolsByCategoryDataSet ThefindAvailableActiveToolsByCategoryDataSet = new FindAvailableActiveToolsByCategoryDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();

        public FindAvailableTools()
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

        private void mitCloseApplication_Click(object sender, RoutedEventArgs e)
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
            int intCounter;
            int intNumberOfRecords;

            TheFindAvailableActiveToolsDataSet = TheToolsClass.FindAvailableActiveTools();

            dgrResults.ItemsSource = TheFindAvailableActiveToolsDataSet.FindAvailableActiveTools;

            TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

            intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count - 1;

            cboSelectCategory.Items.Add("Select Tool Category");
            cboSelectCategory.Items.Add("All Tool Categories");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectCategory.Items.Add(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory);
            }

            cboSelectCategory.SelectedIndex = 0;
        }

        private void cboSelectCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectCategory.SelectedIndex - 2;

                if(intSelectedIndex == -1)
                {
                    TheFindAvailableActiveToolsDataSet = TheToolsClass.FindAvailableActiveTools();

                    dgrResults.ItemsSource = TheFindAvailableActiveToolsDataSet.FindAvailableActiveTools;
                }
                else if(intSelectedIndex > -1)
                {
                    ThefindAvailableActiveToolsByCategoryDataSet = TheToolsClass.FindAvailableActiveToolsByCategory(TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intSelectedIndex].ToolCategory);

                    dgrResults.ItemsSource = ThefindAvailableActiveToolsByCategoryDataSet.FindAvailableActiveToolsByCategory;
                }
             }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Find Available Tools // cbo Select Category Selection Changed " + Ex.Message);

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
