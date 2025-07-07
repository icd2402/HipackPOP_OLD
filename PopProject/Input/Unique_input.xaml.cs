using Newtonsoft.Json.Linq;
using PopProject.input;
using PopProject.Util;
using System;
using System.Windows;
using System.Windows.Input;

namespace PopProject.Input
{
    public partial class Unique_input : Window
    {
        private readonly InputModel InputModel;
        public Unique_input(InputModel inputModel)
        {
            InitializeComponent();
            _ = unique_text.Focus();
            InputModel = inputModel;
            PopSearchResultMemo();

            if (!SettingJsonManager.keyinCheck)
            {
                KeyBoxarCall();
            }

        }

        private void PopSearchResultMemo()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopSearchResultMemo]",
                new string[] { "@pworkkey" },
                new object[] { InputModel.pworkKey },
                0
                );

            if (returnData.결과 == 1)
            {
                string pMemo = "";
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);

                if (Data_Jarry != null)
                {
                    foreach (JObject result in Data_Jarry)
                    {
                        pMemo = result["pMemo"].ToString();
                    }
                }

                unique_text.Text = pMemo;
            }
        }

        //키보드 호출
        private void KeyBoxarCall()
        {
            ////64비트 운영 체제에서 사용되는 키보드 온스크린 실행 파일 경로
            //string path64 = Path.Combine(Directory.GetDirectories
            //    (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");

            ////32비트 운영 체제에서 사용되는 키보드 온스크린 실행 파일 경로
            //string path32 = @"C:\windows\osk.exe";

            //// 현재 운영 체제가 64비트인 경우 path64를, 아닌 경우 path32를 선택
            //string path = Environment.Is64BitOperatingSystem ? path64 : path32;

            //if (File.Exists(path32))
            //{
            //    _ = Process.Start(path32);
            //}
            try
            {
                var path64 = System.IO.Path.Combine(System.IO.Directory.GetDirectories(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
                var path32 = @"C:\windows\system32\osk.exe";
                var path = (Environment.Is64BitOperatingSystem) ? path64 : path32;
                if (System.IO.File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        //저장 버튼
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            string pMemo = unique_text.Text;
            PopInputResultMemo(pMemo);
        }

        //특이사항 저장
        private void PopInputResultMemo(string pMemo)
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopInputResultMemo]",
                new string[] { "@pworkkey", "@pMemo" },
                new object[] { InputModel.pworkKey, pMemo },
                1
                );

            if (returnData.결과 == 1)
            {
                Close();
            }
        }

        //나가기 버튼
        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //드래그
        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //키보드 호출 버튼
        private void Keyboard_call(object sender, MouseButtonEventArgs e)
        {
            KeyBoxarCall();
            _ = unique_text.Focus();
        }
    }
}
