using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PopProject.Util
{
    /// <summary>
    /// Clsuc_Cover.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Clsuc_Cover : UserControl
    {
        public Clsuc_Cover()
        {
            InitializeComponent();

        }
        public void ReContent(double value)
        {
            ItemPanel.Margin = new Thickness(0, Cls_Variable.Dbl_ReportPageTop, 0, 0);
        }
    }
}
