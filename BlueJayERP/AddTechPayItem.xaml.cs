/* Title:           Add Tech Pay Item
 * Date:            5-21-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to add an item */

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
    /// Interaction logic for AddTechPayItem.xaml
    /// </summary>
    public partial class AddTechPayItem : Window
    {
        //setting local classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public AddTechPayItem()
        {
            InitializeComponent();
        }
    }
}
