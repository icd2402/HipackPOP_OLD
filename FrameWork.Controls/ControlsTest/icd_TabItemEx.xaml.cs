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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrameWork.Controls
{
    /// <summary>
    /// icd_Label.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class icd_TabItemEx : UserControl
    {
        public icd_TabItemEx()
        {
            InitializeComponent();
        }
        public event System.Windows.RoutedEventHandler Closed;
        
        public void Closed_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (Closed != null)
                Closed(this, e);
            MessageBox.Show("Closed");
        }
    }
}
