using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Windows;
namespace PopProject.UserControl_
{
    /// <summary>
    /// AuthCodeChange.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AuthCodeChange : Window
    {
        public AuthCodeChange()
        {
            InitializeComponent();
        }

        //변경 버튼 클릭
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //기존 패스워드 일치 시
            if (SettingJsonManager.lockPassword.Equals(password1.Text))
            {
                if (password2.Password != "")
                {
                    SettingJsonManager.lockPassword = password2.Password; //변경 비밀번호 저장
                    SettingJsonManager.SaveJsonFile(); //저장

                    MessageBoxResult result = System.Windows.MessageBox.Show("비밀번호 변경 완료.", "알림", MessageBoxButton.OK);

                    Close();
                }
                else
                {
                    MessageBox_Dialog box = new MessageBox_Dialog("변경 할 비밀번호를 넣어주세요.");
                    _ = box.ShowDialog();
                }
            }
            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("기존 비밀번호가 일치하지 않습니다.");
                _ = box.ShowDialog();
            }
        }

        private bool IsNumeric(string password)
        {
            throw new NotImplementedException();
        }
    }
}
