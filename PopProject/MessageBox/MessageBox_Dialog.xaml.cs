using System;
using System.Windows;
using System.Windows.Input;

namespace PopProject.MessageBox
{
    /// <summary>
    /// MessageBox_Dialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBox_Dialog : Window
    {
        private String Message;
        private int flag = 0;

        public MessageBox_Dialog(String Message)
        {
            InitializeComponent();
            this.Message = Message;
            message_content.Text = Message;
        }

        public MessageBox_Dialog(String Message, int flag)
        {

            InitializeComponent();
            this.Message = Message;
            message_content.Text = Message;
            this.flag = flag;

        }

        private void Close_messagebox(object sender, MouseEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {

            switch (flag)
            {
                case 0:
                    Window.GetWindow(this).Close();
                    break;

                case 1:
                    System.Diagnostics.Process.Start("explorer.exe", "http://packcom.net/Setup/POP_NEW/publish.htm");
                    Window.GetWindow(this).Close();
                    break;
            }
        }
    }
}
