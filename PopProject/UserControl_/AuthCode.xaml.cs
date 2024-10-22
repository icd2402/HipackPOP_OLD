using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PopProject.Util;

namespace PopProject.UserControl_
{
    /// <summary>
    /// AuthCode.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AuthCode : Window
    {
        public int PasswordCorrect { get; private set; }

        public AuthCode()
        {
            InitializeComponent();
            DefaultSetting();
            authCode.Focus();
            authCode.PreviewKeyDown += authKey_KeyDown;

            Keyboard_call();

        }

        private void Keyboard_call()
        {
            var path64 = System.IO.Path.Combine(System.IO.Directory.GetDirectories(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
            var path32 = @"C:\windows\system32\osk.exe";
            var path = (Environment.Is64BitOperatingSystem) ? path64 : path32;
            if (System.IO.File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void DefaultSetting()
        {
            if (SettingJsonManager.lockPassword.Equals(""))
            {
                SettingJsonManager.lockPassword = "0000";
                SettingJsonManager.SaveJsonFile();
            }
        }

        //확인버튼
        private void AuthCodeSave(object sender, RoutedEventArgs e)
        {
            PasswordCheck();
            Close();
        }

        //키보드 이벤트
        private void authKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordCheck();
                Close();
            }
        }

        private void PasswordCheck()
        {
            if (authCode.Password.Equals(SettingJsonManager.lockPassword))
            {
                PasswordCorrect = 1; //비밀번호 일치
            }
            else
            {
                PasswordCorrect = 2; //비밀번호 불일치
            }

        }

        //드래그
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    DragMove();
                }
                catch (InvalidOperationException)
                {
                    // 예외를 처리하지 않고 무시합니다.
                }
            }
        }

        //닫기
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            PasswordCorrect = 0;
            Close();
        }

        //비밀번호 변경
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthCodeChange authCodeChange = new AuthCodeChange();
            authCodeChange.Owner = this; // this는 AuthCode 창을 의미합니다.
            authCodeChange.ShowDialog();
        }
    }
}
