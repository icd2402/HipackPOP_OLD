using PopProject.KeyBoard;
using PopProject.UserControl_;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace PopProject.Input
{
    /// <summary>
    /// Plc_Pop.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Plc_Pop : Window
    {
        string[] psize;
        MainModel mainModel;
        Socket mainSock;
        Win_KeyBoard win_KeyBoard;
        public Plc_Pop(MainModel mainModel)
        {
            InitializeComponent();
            this.mainModel = mainModel;
            SetPlcData();
        }

        private void SetPlcData()
        {
            거래처.Text = mainModel.customSName;
            품명.Text = mainModel.prodName;
            psize = mainModel.pSize.Split('*');
            작업지시.Text = Properties.Settings.Default.setCount.ToString();
            장.Text = psize[0] == null ? "" : psize[0].Trim();
            폭.Text = psize[1] == null ? "" : psize[1].Trim();
            고.Text = psize[2] == null ? "" : psize[2].Trim();
            갭.Text = Properties.Settings.Default.Gap.ToString();
            미미.Text = Properties.Settings.Default.mimi.ToString();
            ip.Text = Properties.Settings.Default.ip + " / " + Properties.Settings.Default.port;

        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void exit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Properties.Settings.Default.plcCheck = false;
            Properties.Settings.Default.Save();
            Close();
        }

        private void ip_input(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Ip_input ip_Input = new Ip_input();
            ip_Input.ShowDialog();
            ip.Text = Properties.Settings.Default.ip + " / " + Properties.Settings.Default.port;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "F1":
                    Ip_input ip_Input = new Ip_input();
                    ip_Input.ShowDialog();
                    ip.Text = Properties.Settings.Default.ip + " / " + Properties.Settings.Default.port;
                    break;
                default:
                    break;
            }
        }

        private void Done(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int a = combo.SelectedIndex;
            int boxType = 1;
            switch (a)
            {
                case 0:
                    boxType = 1;
                    break;
                case 1:
                    boxType = 3;
                    break;
                case 2:
                    boxType = 5;
                    break;
                case 3:
                    boxType = 6;
                    break;
                case 4:
                    boxType = 7;
                    break;
            }

            Properties.Settings.Default.Gap = int.Parse(갭.Text.ToString());
            Properties.Settings.Default.mimi = int.Parse(미미.Text.ToString());
            Properties.Settings.Default.jang = int.Parse(장.Text.ToString());
            Properties.Settings.Default.pok = int.Parse(폭.Text.ToString());
            Properties.Settings.Default.go = int.Parse(고.Text.ToString());
            Properties.Settings.Default.setCount = int.Parse(작업지시.Text.ToString());
            Properties.Settings.Default.boxType = int.Parse(boxType.ToString());
            Properties.Settings.Default.plcCheck = true;
            Properties.Settings.Default.Save();
            Close();

        }

        private void Input_value(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 10,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }
    }
}
