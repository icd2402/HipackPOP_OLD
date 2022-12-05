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
            ip.Text = Properties.Settings.Default.ip;
            port.Text = Properties.Settings.Default.port;

        }

        private void Done(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.ip = ip.Text.Trim();
            Properties.Settings.Default.port = port.Text.Trim();
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
