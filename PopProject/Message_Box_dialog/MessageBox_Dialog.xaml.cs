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
        public MessageBox_Dialog(String Message)
        {
            InitializeComponent();
            this.Message = Message;
            message_content.Text = Message;
        }
          
        private void Close_messagebox(object sender, MouseEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {

            Window.GetWindow(this).Close();
        }
    }
}
