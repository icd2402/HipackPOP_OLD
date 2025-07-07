using System;
using System.Windows;
using System.Windows.Input;

namespace PopProject.MessageBox
{
    /// <summary>
    /// MessageBox_Robot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBox_Robot : Window
    {
#pragma warning disable CS0169 // 'MessageBox_Robot.Message' 필드가 사용되지 않았습니다.
        private String Message;
#pragma warning restore CS0169 // 'MessageBox_Robot.Message' 필드가 사용되지 않았습니다.


        public MessageBox_Robot()
        {
            InitializeComponent();
        }

        private void Messagebox_button1(object sender, MouseButtonEventArgs e) //현재 데이터 저장
        {

        }

        private void Messagebox_button2(object sender, MouseButtonEventArgs e) //전송예정 데이터 저장 
        {


        }

        private void Close_messagebox(object sender, MouseEventArgs e) // X버튼 닫기
        {
            Window.GetWindow(this).Close();
        }

        private void Messagebox_close(object sender, MouseButtonEventArgs e) //닫기 버튼
        {
            Window.GetWindow(this).Close();
        }
    }
}
