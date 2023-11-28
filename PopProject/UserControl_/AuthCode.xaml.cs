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
        public AuthCode()
        {
            InitializeComponent();
            
        }

        private void AuthCodeSave(object sender, RoutedEventArgs e)
        {
            if (authCode.Password.Equals("icd4662402"))
            {
                SettingJsonManager.authKey = "";
                SettingJsonManager.SaveJsonFile();
            }
        
            Close();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void KeyDown_event(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "F1":
                    SettingJsonManager.authKey = "";
                    SettingJsonManager.SaveJsonFile();
                    Close();
                    break;
            }
         
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
