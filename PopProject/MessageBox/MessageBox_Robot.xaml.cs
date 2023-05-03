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

namespace PopProject.MessageBox
{
    /// <summary>
    /// MessageBox_Robot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBox_Robot : Window
    {
        private String Message;


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
