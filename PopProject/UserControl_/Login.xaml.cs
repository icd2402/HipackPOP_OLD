using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.IO;
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

            //인증코드 확인
            //Is_GetCompanyCodeFile();

            ////인증코드 불러 왔으면
            //if (Is_GetCompanyCodeFile())
            //{

            //}
            ////인증코드 로드 실패
            //else
            //{

            //}

            //자동로그인 체크시
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

                //인증코드 저장
                Create_CompanyCodeFile();
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
                case "19": //마닐라
                case "20": //원단
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


        private static string filePath = @"C:\ICD\settings.json";

        //인증코드 파일 읽어오기
        //static bool Is_GetCompanyCodeFile()
        //{
        //    try
        //    {
        //        string loadedSettings = File.ReadAllText(filePath);
        //        JObject jObject = JObject.Parse(loadedSettings);

        //        string login_id = jObject["login_id"].ToString();
        //        string login_password = jObject["login_password"].ToString();
        //        string corp_name = jObject["corp_name"].ToString();
        //        string corp_id = jObject["corp_id"].ToString();
        //        bool login_check = bool.Parse(jObject["login_check"].ToString());
        //        string mercName = jObject["mercName"].ToString();
        //        string SerialName = jObject["SerialName"].ToString();
        //        int processType = Convert.ToInt32(jObject["processType"]);
        //        int hour = Convert.ToInt32(jObject["hour"]);
        //        int minute = Convert.ToInt32(jObject["minute"]);
        //        string url = jObject["url"].ToString();
        //        string saveDate = jObject["saveDate"].ToString();
        //        int pplanKey = Convert.ToInt32(jObject["pplanKey"]);
        //        int count = Convert.ToInt32(jObject["count"]);
        //        string UserID = jObject["UserID"].ToString();
        //        string UserName = jObject["UserName"].ToString();
        //        string signalR = jObject["signalR"].ToString();
        //        string serverIndex = jObject["serverIndex"].ToString();
        //        string pworkkey = jObject["pworkkey"].ToString();
        //        float consumersumcount = float.Parse(jObject["pworkkey"].ToString());
        //        int consumpWorkKey = Convert.ToInt32(jObject["consumpWorkKey"]);
        //        float consumersumerrorcount = float.Parse(jObject["pworkkey"].ToString());
        //        int TypeIndex = Convert.ToInt32(jObject["TypeIndex"]);
        //        int CountType = Convert.ToInt32(jObject["CountType"]);
        //        string settingitem = jObject["settingitem"].ToString();
        //        string meterialinfo = jObject["meterialinfo"].ToString();
        //        int setting_check = Convert.ToInt32(jObject["setting_check"]);
        //        int baudrate = Convert.ToInt32(jObject["baudrate"]);
        //        string SensorRate = jObject["SensorRate"].ToString();
        //        bool RadioChecked = bool.Parse(jObject["RadioChecked"].ToString());
        //        string printItem = jObject["printItem"].ToString();
        //        bool samework = bool.Parse(jObject["samework"].ToString());
        //        bool color_check = bool.Parse(jObject["color_check"].ToString());
        //        bool count_check = bool.Parse(jObject["count_check"].ToString());
        //        bool boxCheck = bool.Parse(jObject["boxCheck"].ToString());
        //        bool searchCheck = bool.Parse(jObject["searchCheck"].ToString());
        //        bool keyinCheck = bool.Parse(jObject["keyinCheck"].ToString());
        //        bool detail_check = bool.Parse(jObject["detail_check"].ToString());
        //        bool option1 = bool.Parse(jObject["option1"].ToString());
        //        float presultcount = float.Parse(jObject["presultcount"].ToString());
        //        bool twoSum = bool.Parse(jObject["twoSum"].ToString());
        //        bool accountCheck = bool.Parse(jObject["accountCheck"].ToString());
        //        bool productionCheck = bool.Parse(jObject["productionCheck"].ToString());
        //        bool scrollvarCheck = bool.Parse(jObject["scrollvarCheck"].ToString());
        //        int index = Convert.ToInt32(jObject["index"]);
        //        string error = jObject["error"].ToString();
        //        string error1 = jObject["error1"].ToString();
        //        bool Plc = bool.Parse(jObject["Plc"].ToString());
        //        bool ChangeCheck = bool.Parse(jObject["ChangeCheck"].ToString());
        //        string ip = jObject["ip"].ToString();
        //        string port = jObject["port"].ToString();
        //        int Gap = Convert.ToInt32(jObject["Gap"]);
        //        int mimi = Convert.ToInt32(jObject["mimi"]);
        //        int jang = Convert.ToInt32(jObject["jang"]);
        //        int pok = Convert.ToInt32(jObject["pok"]);
        //        int go = Convert.ToInt32(jObject["go"]);
        //        int setCount = Convert.ToInt32(jObject["setCount"]);
        //        int boxType = Convert.ToInt32(jObject["boxType"]);
        //        bool plcCheck = bool.Parse(jObject["plcCheck"].ToString());
        //        bool completeValue_checked = bool.Parse(jObject["completeValue_checked"].ToString());
        //        bool checkMerc = bool.Parse(jObject["checkMerc"].ToString());
        //        bool robotchecked = bool.Parse(jObject["robotchecked"].ToString());
        //        string robotIP = jObject["robotIP"].ToString();
        //        string robotPort = jObject["robotPort"].ToString();
        //        string manager = jObject["manager"].ToString();
        //        string authKey = jObject["authKey"].ToString();
        //        string SerialName1 = jObject["SerialName1"].ToString();
        //        int consumCount = Convert.ToInt32(jObject["consumCount"]);

        //        if (!string.IsNullOrWhiteSpace(login_id) && !string.IsNullOrWhiteSpace(login_password))
        //        {
        //            Properties.Settings.Default.login_id = login_id;
        //            Properties.Settings.Default.login_password = login_password;
        //            Properties.Settings.Default.corp_name = corp_name;
        //            Properties.Settings.Default.corp_id = corp_id;
        //            Properties.Settings.Default.login_check = login_check;
        //            Properties.Settings.Default.mercName = mercName;
        //            Properties.Settings.Default.SerialName = SerialName;
        //            Properties.Settings.Default.processType = processType;
        //            Properties.Settings.Default.hour = hour;
        //            Properties.Settings.Default.minute = minute;
        //            Properties.Settings.Default.url = url;
        //            Properties.Settings.Default.saveDate = saveDate;
        //            Properties.Settings.Default.pplanKey = pplanKey;
        //            Properties.Settings.Default.count = count;
        //            Properties.Settings.Default.UserID = UserID;
        //            Properties.Settings.Default.UserName = UserName;
        //            Properties.Settings.Default.signalR = signalR;
        //            Properties.Settings.Default.serverIndex = serverIndex;
        //            Properties.Settings.Default.pworkkey = pworkkey;
        //            Properties.Settings.Default.consumersumcount = consumersumcount;
        //            Properties.Settings.Default.consumpWorkKey = consumpWorkKey;
        //            Properties.Settings.Default.consumersumerrorcount = consumersumerrorcount;
        //            Properties.Settings.Default.TypeIndex = TypeIndex;
        //            Properties.Settings.Default.CountType = CountType;
        //            Properties.Settings.Default.settingitem = settingitem;
        //            Properties.Settings.Default.meterialinfo = meterialinfo;
        //            Properties.Settings.Default.setting_check = setting_check;
        //            Properties.Settings.Default.baudrate = baudrate;
        //            Properties.Settings.Default.SensorRate = SensorRate;
        //            Properties.Settings.Default.RadioChecked = RadioChecked;
        //            Properties.Settings.Default.printItem = printItem;
        //            Properties.Settings.Default.samework = samework;
        //            Properties.Settings.Default.color_check = color_check;
        //            Properties.Settings.Default.count_check = count_check;
        //            Properties.Settings.Default.boxCheck = boxCheck;
        //            Properties.Settings.Default.searchCheck = searchCheck;
        //            Properties.Settings.Default.keyinCheck = keyinCheck;
        //            Properties.Settings.Default.detail_check = detail_check;
        //            Properties.Settings.Default.option1 = option1;
        //            Properties.Settings.Default.presultcount = presultcount;
        //            Properties.Settings.Default.twoSum = twoSum;
        //            Properties.Settings.Default.accountCheck = accountCheck;
        //            Properties.Settings.Default.productionCheck = productionCheck;
        //            Properties.Settings.Default.scrollvarCheck = scrollvarCheck;
        //            Properties.Settings.Default.index = index;
        //            Properties.Settings.Default.error = error;
        //            Properties.Settings.Default.error1 = error1;
        //            Properties.Settings.Default.Plc = Plc;
        //            Properties.Settings.Default.ChangeCheck = ChangeCheck;
        //            Properties.Settings.Default.ip = ip;
        //            Properties.Settings.Default.port = port;
        //            Properties.Settings.Default.Gap = Gap;
        //            Properties.Settings.Default.mimi = mimi;
        //            Properties.Settings.Default.jang = jang;
        //            Properties.Settings.Default.pok = pok;
        //            Properties.Settings.Default.go = go;
        //            Properties.Settings.Default.setCount = setCount;
        //            Properties.Settings.Default.boxType = boxType;
        //            Properties.Settings.Default.plcCheck = plcCheck;
        //            Properties.Settings.Default.completeValue_checked = completeValue_checked;
        //            Properties.Settings.Default.checkMerc = checkMerc;
        //            Properties.Settings.Default.robotchecked = robotchecked;
        //            Properties.Settings.Default.robotIP = robotIP;
        //            Properties.Settings.Default.robotPort = robotPort;
        //            Properties.Settings.Default.manager = manager;
        //            Properties.Settings.Default.authKey = authKey;
        //            Properties.Settings.Default.SerialName1 = SerialName1;
        //            Properties.Settings.Default.consumCount = consumCount;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //    return true;
        //}


        //인증코드를 파일로 저장
        private void Create_CompanyCodeFile()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.login_id) && !string.IsNullOrWhiteSpace(Properties.Settings.Default.login_password))
                {
                    //인증코드 파일경로가 존재하는지 확인 후 경로 생성
                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    //파일 내용
                    var settings = new Dictionary<string, string>
                        {
                            {"login_id", Properties.Settings.Default.login_id},
                            {"login_password", Properties.Settings.Default.login_password},
                            {"corp_name", Properties.Settings.Default.corp_name},
                            {"corp_id", Properties.Settings.Default.corp_id},
                            {"login_check", Properties.Settings.Default.login_check.ToString()},
                            {"mercName", Properties.Settings.Default.mercName},
                            {"SerialName", Properties.Settings.Default.SerialName},
                            {"processType", Properties.Settings.Default.processType.ToString()},
                            {"hour", Properties.Settings.Default.hour.ToString()},
                            {"minute", Properties.Settings.Default.minute.ToString()},
                            {"url", Properties.Settings.Default.url},
                            {"saveDate", Properties.Settings.Default.saveDate},
                            {"pplanKey", Properties.Settings.Default.pplanKey.ToString()},
                            {"count", Properties.Settings.Default.count.ToString()},
                            {"UserID", Properties.Settings.Default.UserID},
                            {"UserName", Properties.Settings.Default.UserName},
                            {"signalR", Properties.Settings.Default.signalR},
                            {"serverIndex", Properties.Settings.Default.serverIndex},
                            {"pworkkey", Properties.Settings.Default.pworkkey},
                            {"consumersumcount", Properties.Settings.Default.consumersumcount.ToString()},
                            {"consumpWorkKey", Properties.Settings.Default.consumpWorkKey.ToString()},
                            {"consumersumerrorcount", Properties.Settings.Default.consumersumerrorcount.ToString()},
                            {"TypeIndex", Properties.Settings.Default.TypeIndex.ToString()},
                            {"CountType", Properties.Settings.Default.CountType.ToString()},
                            {"settingitem", Properties.Settings.Default.settingitem},
                            {"meterialinfo", Properties.Settings.Default.meterialinfo},
                            {"setting_check", Properties.Settings.Default.setting_check.ToString()},
                            {"baudrate", Properties.Settings.Default.baudrate.ToString()},
                            {"SensorRate", Properties.Settings.Default.SensorRate},
                            {"RadioChecked", Properties.Settings.Default.RadioChecked.ToString()},
                            {"printItem", Properties.Settings.Default.printItem},
                            {"samework", Properties.Settings.Default.samework.ToString()},
                            {"color_check", Properties.Settings.Default.color_check.ToString()},
                            {"count_check", Properties.Settings.Default.count_check.ToString()},
                            {"boxCheck", Properties.Settings.Default.boxCheck.ToString()},
                            {"searchCheck", Properties.Settings.Default.searchCheck.ToString()},
                            {"keyinCheck", Properties.Settings.Default.keyinCheck.ToString()},
                            {"detail_check", Properties.Settings.Default.detail_check.ToString()},
                            {"option1", Properties.Settings.Default.option1.ToString()},
                            {"presultcount", Properties.Settings.Default.presultcount.ToString()},
                            {"twoSum", Properties.Settings.Default.twoSum.ToString()},
                            {"accountCheck", Properties.Settings.Default.accountCheck.ToString()},
                            {"productionCheck", Properties.Settings.Default.productionCheck.ToString()},
                            {"scrollvarCheck", Properties.Settings.Default.scrollvarCheck.ToString()},
                            {"index", Properties.Settings.Default.index.ToString()},
                            {"error", Properties.Settings.Default.error},
                            {"error1", Properties.Settings.Default.error1},
                            {"Plc", Properties.Settings.Default.Plc.ToString()},
                            {"ChangeCheck", Properties.Settings.Default.ChangeCheck.ToString()},
                            {"ip", Properties.Settings.Default.ip},
                            {"port", Properties.Settings.Default.port},
                            {"Gap", Properties.Settings.Default.Gap.ToString()},
                            {"mimi", Properties.Settings.Default.mimi.ToString()},
                            {"jang", Properties.Settings.Default.jang.ToString()},
                            {"pok", Properties.Settings.Default.pok.ToString()},
                            {"go", Properties.Settings.Default.go.ToString()},
                            {"setCount", Properties.Settings.Default.setCount.ToString()},
                            {"boxType", Properties.Settings.Default.boxType.ToString()},
                            {"plcCheck", Properties.Settings.Default.plcCheck.ToString()},
                            {"completeValue_checked", Properties.Settings.Default.completeValue_checked.ToString()},
                            {"checkMerc", Properties.Settings.Default.checkMerc.ToString()},
                            {"robotchecked", Properties.Settings.Default.robotchecked.ToString()},
                            {"robotIP", Properties.Settings.Default.robotIP},
                            {"robotPort", Properties.Settings.Default.robotPort},
                            {"manager", Properties.Settings.Default.manager},
                            {"authKey", Properties.Settings.Default.authKey},
                            {"SerialName1", Properties.Settings.Default.SerialName1},
                            {"consumCount", Properties.Settings.Default.consumCount.ToString()}
                        };

                    //인증코드 파일 작성
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

                    //인증코드 파일이 있는지 확인 후 해당 파일을 숨김 처리 (숨김 이후 수정되지 않음)
                    File.WriteAllText(filePath, json);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
