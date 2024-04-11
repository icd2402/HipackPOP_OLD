using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PopProject.input;
using PopProject.Util;

namespace PopProject.KeyBoard
{
    /// <summary>
    /// Win_KeyBoard.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Win_KeyBoard : Window
    {
        private readonly TextBox txtbox;
        private readonly InputModel inputContext = null;
        private readonly errorModel errorContext = null;
        private errorModel errorModel;
        private float pCnt;

        public Win_KeyBoard(errorModel errorModel)
        {
            InitializeComponent();
            this.errorModel = errorModel;
        }
        public Win_KeyBoard(TextBox txtbox)
        {
            InitializeComponent();
            this.txtbox = txtbox;
            this.txtbox.Text = 0 + "";
            option1.Text = "";
        }
        public Win_KeyBoard(TextBox txtbox, object context,InputModel inputmodel)
        {

            InitializeComponent();
            this.txtbox = txtbox;
            this.txtbox.Text = 0 + "";
            pCnt = inputmodel.pCnt;
            //this.errorModel = context;
            option1.Text = SettingJsonManager.option1 ? "상자수가 자동계산됩니다." : "";
            //Console.WriteLine("context" + context.ToString());

            if (context.ToString().Equals("PopProject.input.errorModel"))
            {
                errorContext = (errorModel)context;
            }

            else
            {
                inputContext = (InputModel)context;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Name.Equals("Button"))
            {
                Button button = (Button)sender;
                switch (button.Name)
                {
                    case "key_1":

                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "1";
                        }
                        else
                        {
                            txtbox.Text += "1";
                        }
                        break;

                    case "key_2":
                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "2";
                        }

                        else
                        {
                            txtbox.Text += "2";
                        }

                        break;
                    case "key_3":
                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "3";
                        }

                        else
                        {
                            txtbox.Text += "3";
                        }

                        break;

                    case "key_4":

                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "4";
                        }

                        else
                        {
                            txtbox.Text += "4";
                        }

                        break;

                    case "key_5":

                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "5";
                        }

                        else
                        {
                            txtbox.Text += "5";
                        }

                        break;

                    case "key_6":
                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "6";
                        }

                        else
                        {
                            txtbox.Text += "6";
                        }
                        break;

                    case "key_7":
                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "7";
                        }

                        else

                        {
                            txtbox.Text += "7";
                        }

                        break;

                    case "key_8":
                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "8";
                        }

                        else
                        {
                            txtbox.Text += "8";
                        }

                        break;

                    case "key_9":

                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "9";
                        }

                        else
                        {
                            txtbox.Text += "9";
                        }
                        break;

                    case "key_10":
                        if (txtbox.Text.Equals("0"))
                        {
                            txtbox.Text = "0";
                        }
                        else
                        {
                            txtbox.Text += "0";
                        }
                        break;

                    case "key_11":

                        if (txtbox.Text.Length == 0)
                        {
                            txtbox.Text = 0 + "";
                        }

                        else

                        {
                            txtbox.Text = txtbox.Text.Remove(txtbox.Text.Length - 1);
                            if (txtbox.Text.Length == 0)
                            {
                                txtbox.Text = 0 + "";
                            }
                        }

                        break;

                    case "key_12":

                        if (inputContext != null)
                        {
                            inputContext.pworkCount = SettingJsonManager.option1 ? int.Parse(txtbox.Text) * inputContext.pCnt : int.Parse(txtbox.Text);
                            inputContext.count = SettingJsonManager.option1 ? int.Parse(txtbox.Text) * inputContext.pCnt : int.Parse(txtbox.Text);
                            LogManager.Info("키보드 생산 값: " + inputContext.count, MethodBase.GetCurrentMethod().DeclaringType);

                        }

                        else if (errorContext != null)
                        {
                            errorContext.perrCount = SettingJsonManager.option1 ? int.Parse(txtbox.Text) * pCnt : int.Parse(txtbox.Text);
                            LogManager.Info("키보드 불량 값: " + errorContext.perrCount, MethodBase.GetCurrentMethod().DeclaringType);
                        }

                        Close();
                        break;
                    default:
                        break;
                }

            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //키보드 팝업위치 커스터마이징
        private void Keyboard_loaded(object sender, RoutedEventArgs e)
        {
            //오주포장 2,3호기는 메인/서브모니터를 반대로 사용중이라 듀얼 화면 체크시에도 메인화면에 키보드를 팝업되도록 수정됨
            if (SettingJsonManager.corp_name.Equals("오주포장"))
            {
                if (SettingJsonManager.mercName.Equals("인라인2호기") || SettingJsonManager.mercName.Equals("인라인3호기"))
                {
                    System.Drawing.Rectangle rec = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;//메인화면
                    Left = rec.Left + 40;
                    Top = rec.Top + 380;
                    WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    WindowState = WindowState.Normal;
                }
                else
                {
                    //설정>듀얼모니터 사용 체크시
                    if (SettingJsonManager.detail_check)
                    {
                        //실제로 듀얼 모니터일때
                        if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
                        {
                            System.Drawing.Rectangle rec = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;
                            Left = rec.Left + 40;
                            Top = rec.Top + 380;
                            WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            WindowState = WindowState.Normal;
                        }
                        else
                        {
                            WindowState = WindowState.Normal;
                        }
                    }
                }
            }
            else //기본
            {
                //설정>듀얼모니터 사용 체크시
                if (SettingJsonManager.detail_check)
                {
                    //실제로 듀얼 모니터일때
                    if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
                    {
                        System.Drawing.Rectangle rec = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;
                        Left = rec.Left + 40;
                        Top = rec.Top + 380;
                        WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        WindowState = WindowState.Normal;
                    }
                    else
                    {
                        WindowState = WindowState.Normal;
                    }
                }
            }
        }
    }
}
