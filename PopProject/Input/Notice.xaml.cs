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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace PopProject.Input
{
    /// <summary>
    /// Notice.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Notice : Window
    {
        private Canvas previousClickedCanvas = null;
        private List<UpdateInfor_Model> Programe_ItemList = new List<UpdateInfor_Model>();
        public ObservableCollection<UpdateInfor_Model> NoticeList;
        string ServerIndex;

        public Notice()
        {
            InitializeComponent();
            ServerIndexSearch(); //index 조회
            NoticeListSearch(); //공지사항 조회
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
            canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D0DAE3")); // 클릭한 캔버스 배경색 변경

            // 현재 클릭한 캔버스를 이전에 클릭한 캔버스로 설정
            previousClickedCanvas = canvas;

            // Tag 속성에서 Notice 값을 가져와서 NoticeData.Text에 설정
            if (canvas.DataContext is UpdateInfor_Model model)
            {
                NoticeData.Text = model.Notice;
            }
        }

        //업체별 인덱스 번호 부여
        private void ServerIndexSearch()
        {
            var serverIndexMap = new Dictionary<string, string>
            {
                { "ojbox", "1" }, //오주
                { "miraebox", "2" }, //미래
                { "hnbox", "3" }, //하나
                { "ksbox", "4" }, //광성수출포장
                { "shbox", "5" }, //선호
                { "ycbox", "6" }, //유창
                { "kmbox", "7" }, //광무
                { "dybox", "8" }, //대양
                { "swbox", "9" }, //신우
                { "nrbox", "10" }, //누리
                { "wrbox", "11" }, //우리
                { "hsbox", "12" }, //해성(양산)
                { "erbox", "13" }, //유램
                { "cwbox", "14" }, //채움팩
                { "sjbox", "15" }, //성주
                { "jobox", "16" }, //지온
                { "dubox", "17" }, //두성
                { "dsbox", "18" }, //대세
                { "gybox", "19" }, //구룡
                { "hssbox", "20" }, //혜성(울산)
                { "sungjibox", "21" }, //성지
                { "khbox", "22" }, //금화
                { "opbox", "23" }, //원팩
                { "ksboxs", "24" }, //광성포장산업
                { "npbox", "25" }, //내포수출포장
                { "kmsbox", "26" } //금성포장공업
            };

            // 서버 인덱스를 매핑된 값으로 설정
            ServerIndex = serverIndexMap.ContainsKey(SettingJsonManager.serverIndex)? serverIndexMap[SettingJsonManager.serverIndex] : "0"; // 기본값 설정, 필요에 따라 변경
        }


        public void NoticeListSearch()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://hipackcom.co.kr/Notice_select");
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8;";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    string status = response.StatusCode.ToString();

                    if (status == "OK")
                    {
                        NoticeList = new ObservableCollection<UpdateInfor_Model>();
                        NoticeList.Clear();

                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                        {
                            var text = JObject.Parse(reader.ReadToEnd());
                            var list = (JArray)text["data"];

                            if (list != null)
                            {
                                // 리스트를 업데이트 날짜 내림차순으로 정렬
                                foreach (var result in list.OrderByDescending(x => (DateTime)x["UPDATE_DATE"]))
                                {
                                    var model = new UpdateInfor_Model
                                    {
                                        Program_ID = (int)result["PROGRAM_ID"], //프로젝트 구분 1:하이팩, 2:POP, 3:MES, 4.ERP
                                        Version_Number = (string)result["VERSION_NUMBER"], //프로그램 버전
                                        Update_Date = (string)result["UPDATE_DATE"] == "0000-00-00" ? DateTime.Now.Date : DateTime.Parse((string)result["UPDATE_DATE"]), //업데이트 날짜
                                        Notice_No = (int)result["NOTICE_NO"], //업데이트 번호
                                        Notice = (string)result["NOTICE"], //내용
                                        Apply_Server = (string)result["APPLY_SERVER"] //해당 업체
                                    };

                                    // Program_ID가 2인 경우 (1:하이팩, 2:POP, 3:MES, 4.ERP)
                                    if (model.Program_ID == 2)
                                    {
                                        // Apply_Server 값을 ','로 분리하여 서버 목록을 가져옴
                                        var applyServers = model.Apply_Server.Split(',');

                                        // 서버 인덱스가 applyServers 목록에 포함되어 있거나 Apply_Server 값이 "0"인 경우
                                        if (applyServers.Contains(ServerIndex) || model.Apply_Server == "0")
                                        {
                                            NoticeList.Add(model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ic.ItemsSource = NoticeList;

                if (NoticeList.Count.Equals(0))
                {
                    tb1.Visibility = Visibility.Visible;
                }
                else
                {
                    Dispatcher.InvokeAsync(() => SelectFirstItem(), DispatcherPriority.Loaded);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SelectFirstItem()
        {
            // 첫 번째 Canvas 요소를 찾아 클릭 이벤트를 트리거
            var firstItemContainer = ic.ItemContainerGenerator.ContainerFromIndex(0) as ContentPresenter;
            if (firstItemContainer != null)
            {
                var firstCanvas = FindVisualChild<Canvas>(firstItemContainer);
                if (firstCanvas != null)
                {
                    Canvas_PreviewMouseDown(firstCanvas, null);
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                    return (T)child;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }

    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                //return dateTime.ToString("yyyy년 MM월 dd일", culture);
                return dateTime.ToString("MM월 dd일", culture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class indexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
