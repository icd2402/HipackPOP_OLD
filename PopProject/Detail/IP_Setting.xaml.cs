using System.Windows;
using System.Windows.Input;
using PopProject.Util;

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
            ipaddress.Text = SettingJsonManager.robotIP;
            PortNum.Text = SettingJsonManager.robotPort;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            SettingJsonManager.robotIP = ipaddress.Text.ToString();
            SettingJsonManager.robotPort = PortNum.Text.ToString();
            SettingJsonManager.SaveJsonFile();
            Close();
        }
    }
}
