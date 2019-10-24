/* Title:           Open Trailer Problems for a Trailer
 * Date:            9-25-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show open trailer problems for a trailer */

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
using TrailerProblemDLL;

namespace BlueJayERP
{
    /// <summary>
    /// Interaction logic for OpenTrailerProblemsByTrailerID.xaml
    /// </summary>
    public partial class OpenTrailerProblemsByTrailerID : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();

        //setting up the data set   
        FindOpenTrailerProblemsByTrailerIDDataSet TheFindOpenTrailerProblemsByTrailerIDDataSet = new FindOpenTrailerProblemsByTrailerIDDataSet();

        public OpenTrailerProblemsByTrailerID()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MitClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindOpenTrailerProblemsByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

            dgrResults.ItemsSource = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID;
        }
    }
}
