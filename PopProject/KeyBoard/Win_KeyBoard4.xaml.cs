using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PopProject.input;
using PopProject.Util;

namespace PopProject.KeyBoard
{
    /// <summary>
    /// Win_KeyBoard3.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Win_KeyBoard4 : Window
    {
        private float pCnt;
        private string ProdID = "";

        public Win_KeyBoard4(string prodID)
        {
            InitializeComponent();
            this.ProdID = prodID;
            txtbox.Text = "";
            txtbox.Focus();
            txtbox.PreviewKeyDown += txtbox_PreviewKeyDown;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Name.Equals("Button"))
            {
                Button button = (Button)sender;
                switch (button.Name)
                {
                    case "key_13": //지우기

                        if (txtbox.Text.Length == 0)
                        {
                            txtbox.Text = "";
                        }
                        else
                        {
                            txtbox.Text = txtbox.Text.Remove(txtbox.Text.Length - 1);
                            if (txtbox.Text.Length == 0)
                            {
                                txtbox.Text = "";
                            }
                        }
                        break;

                    case "key_14": //저장
                        holeValueUpdate();
                        Close();
                        break;

                    case "key_15": //닫기
                        Close();
                        break;

                    default:
                        break;
                }
            }
        }

        private void txtbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                holeValueUpdate();
                Close();
            }
        }

        private void holeValueUpdate()
        {
            Console.WriteLine(this.ProdID + " / " + txtbox.Text + " / " + DateTimeManager.GetSelectedStr());

            ReturnModel returnData = ServiceModel.GetData(
                        SettingJsonManager.serverIndex,
                        "[holeRegistaUpdate]",
                        new string[] { "@prodID", "@hValue", "@UpDatetime" },
                        new object[] { this.ProdID, txtbox.Text, DateTimeManager.GetSelectedStr() },
                        0
                        );
            if (returnData.결과 == 1)
            {
                Console.WriteLine("holeRegistaUpdate 성공 : " + this.ProdID + " / " + txtbox.Text + " / " + DateTimeManager.GetSelectedStr());
            }
            else
            {
                Console.WriteLine("holeRegistaUpdate 실패");
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //키보드 팝업위치 커스터마이징
        private void Keyboard_loaded(object sender, RoutedEventArgs e)
        {
            System.Drawing.Rectangle rec = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
            Left = rec.Left + 600;
            Top = rec.Top + 250;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Normal;

        }
    }
}
