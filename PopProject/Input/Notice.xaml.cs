using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using PopProject.Detail;
using PopProject.input;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
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

namespace PopProject.Input
{
    /// <summary>
    /// Notice.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Notice : Window
    {
        private Canvas previousClickedCanvas = null;

        public Notice()
        {
            InitializeComponent();

            NoticeList();
        }

        private void NoticeList()
        {
            Console.WriteLine("[PopNoticeList] 조회: " + DateTimeManager.GetSelectedDateStr());
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopNoticeSearch]",
               new string[] { "@corp_name" },
               new string[] { SettingJsonManager.corp_name }, 
               0
               );
            if (returnData.결과 == 1)
            {
            }
        }


        //리스트 Canvas클릭 이벤트 처리
        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;

            // 이전에 클릭한 캔버스가 있으면 해당 캔버스의 배경색과 테두리를 원래 색상으로 변경
            if (previousClickedCanvas != null)
            {
                previousClickedCanvas.Background = previousClickedCanvas.Tag as Brush;
                Border previousBorder = VisualTreeHelper.GetParent(previousClickedCanvas) as Border;

                if (previousBorder != null)
                {
                    previousBorder.BorderThickness = new Thickness(0); // 이전 캔버스의 테두리를 기본값으로 변경
                }
            }

            canvas.Tag = canvas.Background; // 클릭한 캔버스의 원래 배경색을 저장
            canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86AEE3"));// 클릭한 캔버스 배경색 변경


            // 현재 클릭한 캔버스를 이전에 클릭한 캔버스로 설정
            previousClickedCanvas = canvas;


            //더블클릭시 상세창
            if (e.ClickCount == 2)
            {
            }
        }
    }
}
