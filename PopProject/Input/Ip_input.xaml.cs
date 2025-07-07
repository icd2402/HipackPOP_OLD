using PopProject.Util;
using System.Windows;
using System.Windows.Input;

namespace PopProject.Input
{
    /// <summary>
    /// Ip_input.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Ip_input : Window
    {

        public Ip_input()
        {
            InitializeComponent();
            ip.Text = SettingJsonManager.ip;
            port.Text = SettingJsonManager.port;

        }

        private void Done(object sender, MouseButtonEventArgs e)
        {
            SettingJsonManager.ip = ip.Text.Trim();
            SettingJsonManager.port = port.Text.Trim();
            SettingJsonManager.SaveJsonFile();
            Close();
        }
    }
}
