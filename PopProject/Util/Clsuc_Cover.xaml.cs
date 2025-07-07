using System.Windows;
using System.Windows.Controls;

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
