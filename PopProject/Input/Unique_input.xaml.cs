using Newtonsoft.Json.Linq;
using PopProject.input;
using PopProject.Util;
using System;
using System.Diagnostics;
using System.IO;
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

            if (!Properties.Settings.Default.keyinCheck)
            {
                KeyBoxarCall();
            }

        }

        private void PopSearchResultMemo()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
            string path64 = Path.Combine(Directory.GetDirectories
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
            string path32 = @"C:\windows\osk.exe";
            string path = Environment.Is64BitOperatingSystem ? path64 : path32;

            if (File.Exists(path32))
            {
                _ = Process.Start(path32);
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
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
