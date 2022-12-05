using System.Windows;
using System.Windows.Input;

namespace PopProject.Detail
{
    /// <summary>
    /// IP_Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IP_Setting : Window
    {
        public IP_Setting()
        {
            InitializeComponent();
            ipaddress.Text = Properties.Settings.Default.robotIP;
            PortNum.Text = Properties.Settings.Default.robotPort;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.robotIP = ipaddress.Text.ToString();
            Properties.Settings.Default.robotPort = PortNum.Text.ToString();
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
