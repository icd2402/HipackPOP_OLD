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
using System.IO;
using System.Diagnostics;
using PopProject.Util;
using PopProject.input;
using Newtonsoft.Json.Linq;
using Path = System.IO.Path;
using PopProject.UserControl_;
using PopProject.Detail;

namespace PopProject.MessageBox
{
    /// <summary>
    /// PrintMemoBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PrintMemoBox : Window
    {
        MainModel mainModel;
        DetailModel detailitem;

        public PrintMemoBox(MainModel main, DetailModel detailitem)
        {
            InitializeComponent();

            mainModel = main;
            this.detailitem = detailitem;
        }


        //저장 버튼
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            //string pMemo = unique_text.Text;
        }

        //키보드 호출 버튼
        private void Keyboard_call(object sender, MouseButtonEventArgs e)
        {
            KeyBoxarCall();
            _ = unique_text.Focus();
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
    }
}
