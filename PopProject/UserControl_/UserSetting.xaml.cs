using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Net.Json;
using System.Windows;
using System.Windows.Input;

namespace PopProject.UserControl_
{
    /// <summary>
    /// UserSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserSetting : Window
    {
        public UserSetting()
        {
            InitializeComponent();

            selUser();
        }

        private void selUser()
        {
            string corp_id = SettingJsonManager.corp_id;

            JsonObjectCollection json = new JsonObjectCollection
                {
                    new JsonStringValue("corp_id", corp_id)

                };

            HttpClient_util httpClient_Util = new HttpClient_util("Pop_SelUser");
            string result = httpClient_Util.SendHttp(json.ToString());
            httpClient_Util = null;
            JObject jo = JObject.Parse(result);
            string jo_result = jo.SelectToken("result").ToString();
            Console.WriteLine("jo_result : " + jo_result);

            switch (jo_result)
            {
               

                case "1":
                    _ = new LogData("로그인");
                    string jo_data = jo.SelectToken("data").ToString();
                    JArray jarr = JArray.Parse(jo_data);
                    Console.WriteLine("jo_data : " + jo_data);
                    List<UserModel> userlist = new List<UserModel>();
                    for(int i = 0; i < jarr.Count; i++)
                    {
                        UserModel user = new UserModel();
                        user.login_id = jarr[i]["login_id"].ToString();
                        user.login_password = jarr[i]["login_password"].ToString();
                        user.corp_name = jarr[i]["corp_name"].ToString();
                        user.corp_id = jarr[i]["corp_id"].ToString();
                        user.url = jarr[i]["url"].ToString();
                        user.signalR = jarr[i]["signalR"].ToString();
                        user.serverIndex = jarr[i]["serverIndex"].ToString();
                        user.manager = jarr[i]["manager"].ToString();
                        user.user_name = jarr[i]["user_name"].ToString();
                        user.login_time = jarr[i]["login_time"].ToString();

                        userlist.Add(user);
                    }


                    ic.ItemsSource = userlist;

                    break;
                default:
                    break;
            }

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Exit_window(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void User_Add(object sender, RoutedEventArgs e)
        {

        }
    }
}
