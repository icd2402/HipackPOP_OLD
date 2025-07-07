using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Net.Json;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PopProject.UserControl_
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            _ = Keyboard.Focus(textbox_login_id);

            //  세팅 정보 존재 Check
            bool fileExit = SettingJsonManager.CheckJsonFile();
            LogManager.Info("JsonSettings 파일 존재 유무 확인: " + fileExit.ToString(), MethodBase.GetCurrentMethod().DeclaringType);

            // 세팅 정보 있으면
            if (fileExit)
            {
                //인증코드 포함 세팅 정보 Load
                SettingJsonManager.LoadJsonFile();

                // 자동로그인 체크
                login_check.IsChecked = SettingJsonManager.login_check == true ? true : false;

                // ID 와 PW Setting 에 있으면 입력
                textbox_login_id.Text = string.IsNullOrWhiteSpace(SettingJsonManager.login_id) ? "" : SettingJsonManager.login_id;
                textbox_login_password.Password = string.IsNullOrWhiteSpace(SettingJsonManager.login_password) ? "" : SettingJsonManager.login_password;

                LogManager.Info("자동 로그인 체크 및 ID/PW 정보 Load", MethodBase.GetCurrentMethod().DeclaringType);
            }
            else
            {
                // settings.json 파일 새로 생성 및 초기화
                SettingJsonManager.SaveJsonFile();
                // settings.json Load
                SettingJsonManager.LoadJsonFile();
            }

            // 자동 로그인 체크 true && ID/PW 빈칸 아닌 경우
            if (login_check.IsChecked == true && textbox_login_id.Text != "" && textbox_login_password.Password.Length != 0)
            {
                Login_();
                LogManager.Info("자동 로그인", MethodBase.GetCurrentMethod().DeclaringType);
            }
            else
            {
                SettingJsonManager.login_check = false;
                login_check.IsChecked = false;
            }
        }

        //로그인 마우스 올렸을때
        private void Login_button_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            login_button.Background = Brushes.Bisque;
        }

        //로그인 마우스 올렸을때
        private void Exit_button_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Exit_button.Background = Brushes.Bisque;
        }

        private void Login_button_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            login_button.Background = new SolidColorBrush(Color.FromRgb(244, 148, 64));
        }

        private void Exit_button_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Exit_button.Background = new SolidColorBrush(Color.FromRgb(244, 148, 64));
        }

        private void Login_button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //  Console.WriteLine("마우스 클릭");
            Cls_Variable.SharedValue = true;

            if (textbox_login_id.Text.Equals(""))
            {
                Grid.Effect = new BlurEffect();
                can.Visibility = Visibility.Visible;
                MessageBox_Dialog dialog = new MessageBox_Dialog("아이디를 입력 하지 않으셨습니다.");
                _ = dialog.ShowDialog();
                can.Visibility = Visibility.Collapsed;
                Grid.Effect = null;
                return;
            }

            if (textbox_login_password.Password.Length == 0)
            {
                Grid.Effect = new BlurEffect();
                can.Visibility = Visibility.Visible;
                MessageBox_Dialog dialog = new MessageBox_Dialog("패스워드를 입력 하지 않으셨습니다.");
                _ = dialog.ShowDialog();
                can.Visibility = Visibility.Collapsed;
                Grid.Effect = null;
                return;
            }
            Login_();
        }

        //로그 폴더 생성
        //public void 로그폴더생성()
        //{
        //    string folderPath = @"C:\POP_LOG";

        //    if (!Directory.Exists(folderPath))
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(folderPath);
        //            Console.WriteLine("폴더가 생성되었습니다.");
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }
        //    else
        //    {
        //    }
        //}

        public void Login_()
        {
            bool connected = NetworkInterface.GetIsNetworkAvailable();
            if (connected)
            {
                string id = textbox_login_id.Text;
                string password = textbox_login_password.Password;

                JsonObjectCollection json = new JsonObjectCollection
                {
                    new JsonStringValue("id", id),
                    new JsonStringValue("password", password)
                };

                Console.WriteLine("json : " + json.ToString());
                HttpClient_util httpClient_Util = new HttpClient_util("Pop_login");
                string result = httpClient_Util.SendHttp(json.ToString());
                httpClient_Util = null;
                JObject jo = JObject.Parse(result); // 에러파트
                string jo_result = jo.SelectToken("result").ToString();
                Console.WriteLine("jo_result : " + jo_result);

                switch (jo_result)
                {
                    // 아이디가 없는 경우
                    case "0":
                        MessageBox_Dialog dialog = new MessageBox_Dialog("아이디가 존재 하지 않습니다.");
                        _ = dialog.ShowDialog();
                        break;

                    // 아이디는 있는 경우
                    case "1":
                        string jo_data = jo.SelectToken("data").ToString();
                        JArray jarr = JArray.Parse(jo_data);
                        Console.WriteLine("jo_data : " + jo_data);
                        string login_password = jarr[0]["login_password"].ToString();
                        string login_id = jarr[0]["login_id"].ToString();
                        string corp_name = jarr[0]["corp_name"].ToString();
                        string corp_id = jarr[0]["corp_id"].ToString();
                        string url = jarr[0]["url"].ToString();
                        string signalR = jarr[0]["signalR"].ToString();
                        string serverIndex = jarr[0]["serverIndex"].ToString();
                        string manager = jarr[0]["manager"].ToString();

                        // 패스워드 OK = 정상로그인
                        if (login_password.Equals(password))
                        {
                            SettingJsonManager.login_id = login_id;
                            SettingJsonManager.login_password = login_password;
                            SettingJsonManager.corp_name = corp_name;
                            SettingJsonManager.corp_id = corp_id;
                            SettingJsonManager.url = url;   // 상자 이미지 폴더 위치
                            SettingJsonManager.signalR = signalR;
                            SettingJsonManager.serverIndex = serverIndex;
                            SettingJsonManager.manager = manager;
                            SettingJsonManager.login_check = login_check.IsChecked.GetValueOrDefault();
                            SettingJsonManager.authKey = corp_id;

                            string metrialitem = SettingJsonManager.meterialinfo;
                            Console.WriteLine("metrialitem -> " + metrialitem);
                            SetSetting(metrialitem);

                            json = new JsonObjectCollection
                                {
                                     new JsonStringValue("login_time", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")),
                                     new JsonStringValue("login_id", SettingJsonManager.login_id)

                                };
                            Console.WriteLine("json : " + json.ToString());
                            httpClient_Util = new HttpClient_util("login_time_update");
                            result = httpClient_Util.SendHttp(json.ToString());

                            LogManager.Info("로그인 정보 정상", MethodBase.GetCurrentMethod().DeclaringType);
                        }
                        // 패스워드 틀린 경우
                        else
                        {
                            //SettingJsonManager.login_id = "";
                            //SettingJsonManager.login_password = "";
                            //SettingJsonManager.corp_name = "";
                            //SettingJsonManager.corp_id = "";

                            dialog = new MessageBox_Dialog("패스워드를 확인해 주세요");
                            _ = dialog.ShowDialog();
                        }
                        break;

                    default:
                        break;
                }

                //인증코드 저장
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                LogManager.Error("인터넷 연결상태 불량", MethodBase.GetCurrentMethod().DeclaringType);
                MessageBox_Dialog dialog = new MessageBox_Dialog("인터넷 연결 상태를 확인해 주세요");
                _ = dialog.ShowDialog();
            }
        }

        private void SetSetting(string metrialitem)
        {
            //마닐라,원단 or 기본 화면 확면 구분
            switch (metrialitem)
            {
                case "19": //마닐라
                case "20": //원단
                    if (SettingJsonManager.authKey.Equals(SettingJsonManager.corp_id))
                    {
                        _ = new LogData("로그인");
                        Main1 main1 = new Main1();
                        Content = main1;
                    }
                    else
                    {
                        if (SettingJsonManager.authKey.Equals(""))
                        {
                            SettingJsonManager.authKey = SettingJsonManager.corp_id;
                            SettingJsonManager.SaveJsonFile();
                        }
                        else
                        {
                            MessageBox_Dialog msg = new MessageBox_Dialog("타업체 계정으로 로그인 할 수 없습니다.");
                            msg.ShowDialog();
                        }
                    }
                    break;

                default://기본화면
                    //DB명 일치시
                    if (SettingJsonManager.authKey.Equals(SettingJsonManager.corp_id))
                    {
                        _ = new LogData("로그인");
                        Main main = new Main();
                        Content = main;
                    }
                    else //DB명 불일치
                    {
                        if (SettingJsonManager.authKey.Equals(""))
                        {
                            SettingJsonManager.authKey = SettingJsonManager.corp_id;
                            SettingJsonManager.SaveJsonFile();
                        }
                        else
                        {
                            MessageBox_Dialog msg = new MessageBox_Dialog("타업체 계정으로 로그인 할 수 없습니다.");
                            msg.ShowDialog();
                        }
                    }
                    break;
            }
        }

        private void Textbox_login_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_();
            }
        }

        private void Login_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Exit_button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Login_keydownEvent(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "F12":
                    AuthCode auth = new AuthCode();
                    auth.ShowDialog();

                    break;
            }
        }
    }
}
