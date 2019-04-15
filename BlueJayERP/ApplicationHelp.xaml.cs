/* Title:           Application Help
 * Date:            3-29-18
 * Author:          Terry Holmes
 * 
 * Description:     This the help form */

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

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for ApplicationHelp.xaml
    /// </summary>
    public partial class ApplicationHelp : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        
        public ApplicationHelp()
        {
            InitializeComponent();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //myWebBrowser.Navigate(new Uri("file://bjc/shares/Documents/WAREHOUSE/WhseTrac%20Manual/index.html"));

                System.Diagnostics.Process.Start("file://bjc/shares/Documents/WAREHOUSE/WhseTrac%20Manual/index.html");
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        
        }

        private void mitExit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
