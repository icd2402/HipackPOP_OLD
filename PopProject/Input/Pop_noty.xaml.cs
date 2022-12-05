using System.Windows;
using System.Windows.Input;

namespace PopProject.Input
{
    /// <summary>
    /// Pop_noty.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Pop_noty : Window
    {
        public Pop_noty()
        {
            InitializeComponent();
        }

        private void Main_keydownEvent(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "Escape":
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}
