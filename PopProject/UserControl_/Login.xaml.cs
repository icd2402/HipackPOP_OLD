using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Net.Json;
using System.Net.NetworkInformation;
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
            if (Properties.Settings.Default.login_check)
            {
                login_check.IsChecked = true;
                textbox_login_id.Text = Properties.Settings.Default.login_id;
                textbox_login_password.Password = Properties.Settings.Default.login_password;
                Login_();
            }
            else
            {
                login_check.IsChecked = false;
                textbox_login_id.Text = "";
                textbox_login_password.Password = "";
            }
        }
        //로그인 마우스 올렸을때
        private void Login_button_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            login_button.Background = Brushes.Bisque;
        }

        private void Login_button_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            login_button.Background = new SolidColorBrush(Color.FromRgb(244, 148, 64));
        }

        private void Login_button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //  Console.WriteLine("마우스 클릭");
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
                JObject jo = JObject.Parse(result);
                string jo_result = jo.SelectToken("result").ToString();
                Console.WriteLine("jo_result : " + jo_result);

                switch (jo_result)
                {
                    case "0":
                        MessageBox_Dialog dialog = new MessageBox_Dialog("아이디가 존재 하지 않습니다.");
                        _ = dialog.ShowDialog();
                        break;

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

                        if (login_password.Equals(password))
                        {
                            Properties.Settings.Default.login_id = login_id;
                            Properties.Settings.Default.login_password = login_password;
                            Properties.Settings.Default.corp_name = corp_name;
                            Properties.Settings.Default.corp_id = corp_id;
                            Properties.Settings.Default.url = url;
                            Properties.Settings.Default.signalR = signalR;
                            Properties.Settings.Default.serverIndex = serverIndex;
                            Properties.Settings.Default.manager = manager;
                            Properties.Settings.Default.Save();

                            if (login_check.IsChecked.GetValueOrDefault())
                            {
                                Properties.Settings.Default.login_check = true;
                                Properties.Settings.Default.Save();
                            }

                            else
                            {
                                Properties.Settings.Default.login_check = false;
                                Properties.Settings.Default.Save();
                            }

                            string metrialitem = Properties.Settings.Default.meterialinfo;
                            Console.WriteLine("metrialitem -> " + metrialitem);
                            SetSetting(metrialitem);


                            json = new JsonObjectCollection
                                {
                                     new JsonStringValue("login_time", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")),
                                     new JsonStringValue("login_id", Properties.Settings.Default.login_id)
                                     
                                };
                            Console.WriteLine("json : " + json.ToString());
                            httpClient_Util = new HttpClient_util("login_time_update");
                            result = httpClient_Util.SendHttp(json.ToString());
                        }
                        else
                        {
                            Properties.Settings.Default.login_id = "";
                            Properties.Settings.Default.login_password = "";
                            Properties.Settings.Default.corp_name = "";
                            Properties.Settings.Default.corp_id = "";
                            Properties.Settings.Default.Save();
                            dialog = new MessageBox_Dialog("패스워드를 확인해 주세요");
                            _ = dialog.ShowDialog();
                        }
                        break;

                    default:
                        break;
                }
            }
            else
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("인터넷 연결 상태를 확인해 주세요");
                _ = dialog.ShowDialog();
            }
        }

        private void SetSetting(string metrialitem)
        {
            //마닐라,원단 or 기본 화면 확면 구분
            switch (metrialitem)
            {
                case "12": //마닐라
                case "13": //원단
                    if (Properties.Settings.Default.authKey.Equals(Properties.Settings.Default.corp_id))
                    {
                        _ = new LogData("로그인");
                        Main1 main1 = new Main1();
                        Content = main1;
                    }
                    else
                    {
                        if (Properties.Settings.Default.authKey.Equals(""))
                        {
                            Properties.Settings.Default.authKey = Properties.Settings.Default.corp_id;
                            Properties.Settings.Default.Save();
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
                    if (Properties.Settings.Default.authKey.Equals(Properties.Settings.Default.corp_id))
                    {
                        _ = new LogData("로그인");
                        Main main = new Main();
                        Content = main;
                    }
                    else //DB명 불일치
                    {
                        if (Properties.Settings.Default.authKey.Equals(""))
                        {
                            Properties.Settings.Default.authKey = Properties.Settings.Default.corp_id;
                            Properties.Settings.Default.Save();
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
