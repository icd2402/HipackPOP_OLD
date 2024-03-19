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

    public partial class Win_KeyBoard3 : Window
    {
        private float pCnt;
        private string ProdID = "";

        public Win_KeyBoard3(string prodID)
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
                    case "key_1": //1
                        txtbox.Text = (txtbox.Text.Equals("")) ? "1" : txtbox.Text + "1";
                        break;

                    case "key_2": //2
                        txtbox.Text = (txtbox.Text.Equals("")) ? "2" : txtbox.Text + "2";
                        break;

                    case "key_3": //3
                        txtbox.Text = (txtbox.Text.Equals("")) ? "3" : txtbox.Text + "3";
                        break;

                    case "key_4": //4
                        txtbox.Text = (txtbox.Text.Equals("")) ? "4" : txtbox.Text + "4";
                        break;

                    case "key_5": //5
                        txtbox.Text = (txtbox.Text.Equals("")) ? "5" : txtbox.Text + "5";
                        break;

                    case "key_6": //6
                        txtbox.Text = (txtbox.Text.Equals("")) ? "6" : txtbox.Text + "6";
                        break;

                    case "key_7": //7
                        txtbox.Text = (txtbox.Text.Equals("")) ? "7" : txtbox.Text + "7";
                        break;

                    case "key_8": //8
                        txtbox.Text = (txtbox.Text.Equals("")) ? "8" : txtbox.Text + "8";
                        break;

                    case "key_9": //9

                        txtbox.Text = (txtbox.Text.Equals("")) ? "9" : txtbox.Text + "9";
                        break;

                    case "key_10": //0
                        txtbox.Text = (txtbox.Text.Equals("")) ? "0" : txtbox.Text + "0";
                        break;

                    case "key_11": //.
                        if (txtbox.Text.Length > 0)
                        {
                            txtbox.Text += ".";
                        }
                        break;

                    //case "key_11": //+

                    //    // 현재 텍스트 상자의 값이 "+"로 시작하면
                    //    if (txtbox.Text.StartsWith("+"))
                    //    {
                    //        txtbox.Text = txtbox.Text.Remove(0, 1);
                    //    }
                    //    // 현재 텍스트 상자의 값이 "-"로 시작하면
                    //    if (txtbox.Text.StartsWith("-"))
                    //    {
                    //        txtbox.Text = txtbox.Text.Remove(0, 1);
                    //        txtbox.Text = "+" + txtbox.Text;
                    //    }
                    //    // 현재 텍스트 상자의 값이 "+"로 시작하지 않으면
                    //    if (!txtbox.Text.StartsWith("+"))
                    //    {
                    //        txtbox.Text = "+" + txtbox.Text;
                    //    }
                    //    break;

                    case "key_12": //-
                        // 현재 텍스트 상자의 값이 "-"로 시작하지 않으면
                        if (!txtbox.Text.StartsWith("-"))
                        {
                            // "-"를 텍스트 상자의 값 앞에 추가합니다.
                            txtbox.Text = "-" + txtbox.Text;
                        }
                        else if (txtbox.Text.StartsWith("-"))
                        {
                            txtbox.Text = txtbox.Text.Remove(0, 1);
                        }

                        //if (txtbox.Text.StartsWith("+"))
                        //{
                        //    txtbox.Text = txtbox.Text.Remove(0, 1);
                        //    txtbox.Text = "-" + txtbox.Text;
                        //}
                        
                        break;

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

                        // 현재 텍스트 상자의 값이 "-" 또느 "+"로 시작하지 않으면
                        if (!txtbox.Text.StartsWith("-") && !txtbox.Text.StartsWith("+"))
                        {
                            txtbox.Text = "+" + txtbox.Text;
                        }
                        rValueUpdate();
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
                if (!txtbox.Text.StartsWith("-") && !txtbox.Text.StartsWith("+"))
                {
                    txtbox.Text = "+" + txtbox.Text;
                }
                rValueUpdate();
                Close();
            }
        }

        private void rValueUpdate()
        {
            Console.WriteLine(this.ProdID + " / " + txtbox.Text + " / " + DateTimeManager.GetSelectedStr());

            ReturnModel returnData = ServiceModel.GetData(
                        SettingJsonManager.serverIndex,
                        "[rRegistaUpdate]",
                        new string[] { "@prodID", "@rValue", "@UpDatetime" },
                        new object[] { this.ProdID, txtbox.Text, DateTimeManager.GetSelectedStr() },
                        0
                        );
            if (returnData.결과 == 1)
            {
                Console.WriteLine("rRegistaUpdate 성공 : " + this.ProdID + " / " + txtbox.Text + " / " + DateTimeManager.GetSelectedStr());
            }
            else
            {
                Console.WriteLine("rRegistaUpdate 실패");
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
