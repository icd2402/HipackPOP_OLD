using PopProject.Detail;
using PopProject.Util;
using PopProject.UserControl_;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PopProject.Input
{
    /// <summary>
    /// SearchList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SearchList : Window
    {
        private readonly ObservableCollection<MainModel> worklist;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        private bool changeCheck = false;
        private bool CNS_Barcode_Send = false; //true이면 바코드 읽으면 CNS로 데이터 전송

        ObservableCollection<MainModel> searchList;

        //메인화면에서 검색창 팝업
        public SearchList(ObservableCollection<MainModel> workList)
        {
            InitializeComponent();
            worklist = workList;
            _ = textbox_search.Focus();
        }

        //생산계획 순서변경 창에서 검색창 팝업
        public SearchList(ObservableCollection<MainModel> workList, bool check)
        {
            InitializeComponent();
            changeCheck = check;
            worklist = workList;
            _ = textbox_search.Focus();
        }
        
        //검색 텍스트 입력
        private void SearchChanged_Text(object sender, TextChangedEventArgs e)
        {
            searchList = new ObservableCollection<MainModel>();

            if (textbox_search.Text.Length == 0)
            {
                //검색리스트에서 선택한 항복 복구
                for (int j = 0; j < worklist.Count; j++)
                {
                    if (worklist[j].icback.Equals("#6799FF"))
                    {
                        worklist[j].check = false;
                        worklist[j].icback = "White";
                    }
                }

                searchList.Clear();
            }
            else
            {
                for (int i = 0; i < worklist.Count; i++)
                {
                    worklist[i].pSize = Regex.Replace(worklist[i].pSize, @"\s", "");

                    if (worklist[i].corpName.ToUpper().Contains(textbox_search.Text.ToUpper().Trim())
                        || worklist[i].prodName.ToUpper().Contains(textbox_search.Text.ToUpper().Trim())
                        || worklist[i].pSize.ToString().Trim().Contains(textbox_search.Text.ToString().Trim())
                        || worklist[i].customSName.ToUpper().Contains(textbox_search.Text.ToUpper().Trim()))
                    {
                        searchList.Add(worklist[i]);
                    }
                }
            }
            ic.ItemsSource = searchList;
        }

        //품목 클릭
        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index;

            //더블 클릭
            if (e.ClickCount == 2)
            {
                //생산계획 변경창에서 더블클릭 시
                if (!changeCheck)
                {
                    ShowDetail(sender);
                }
                else
                {
                    Canvas canvas = (Canvas)sender;
                    MainModel context = (MainModel)canvas.DataContext;
                    index =  worklist.IndexOf(context);
                    SettingJsonManager.index = index;
                    SettingJsonManager.SaveJsonFile();

                    Close();

                    for (int j = 0; j < searchList.Count; j++)
                    {
                        searchList[j].check = false;
                        searchList[j].icback = "White";
                    }
                }
            }
            //한번 클릭
            else if (e.ClickCount == 1)
            {
                if (sender.GetType().Name.Equals("Canvas"))
                {
                    Canvas canvas = (Canvas)sender;

                    // Canvas의 데이터 컨텍스트로부터 해당 항목의 인덱스를 찾음
                    index = ic.Items.IndexOf(canvas.DataContext);
                    Console.WriteLine("i = > " + index);

                    var canvass = sender as Canvas;
                    var selectedItem = canvass.DataContext;


                    // 해당 항목의 선택 상태를 변경하고 배경색을 변경
                    if (searchList[index].check)
                    {
                        searchList[index].check = false;
                        searchList[index].icback = "White";
                    }
                    else
                    {
                        searchList[index].check = true;
                        searchList[index].icback = "#6799FF";
                    }

                    for (int j = 0; j < searchList.Count; j++)
                    {
                        if (j != index)
                        {
                            if (searchList[j].icback.Equals("#6799FF"))
                            {
                                searchList[j].check = false;
                                searchList[j].icback = "White";
                            }
                        }
                    }
                }
            }
        }


        //상세창 팝업
        private void ShowDetail(object sender)
        {
            Canvas canvas = (Canvas)sender;
            MainModel context = (MainModel)canvas.DataContext;
            string special = "";
            Win_Detail win_detail = new Win_Detail(context, special, worklist, SettingJsonManager.detail_check, CNS_Barcode_Send);
            _ = win_detail.ShowDialog();
        }

        //창에서 마우스 다운
        private void ScrollViewer_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        //창에서 마우스 업
        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }

        //창에서 마우스 위치 확인
        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        //키보드 호출
        private void keyboard_down(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D353D"));
        }

        //키보드 호출
        private void keyboard_up(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#485563"));

            KeyBoardCall();
        }

        //키보드 호출
        private void KeyBoardCall()
        {

            var path64 = System.IO.Path.Combine(System.IO.Directory.GetDirectories(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
            var path32 = @"C:\windows\system32\osk.exe";
            var path = (Environment.Is64BitOperatingSystem) ? path64 : path32;
            if (System.IO.File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        //드래그
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //나가기 버튼
        private void exit(object sender, MouseButtonEventArgs e)
        {
            if (searchList != null)
            {
                for (int j = 0; j < searchList.Count; j++)
                {
                    searchList[j].check = false;
                    searchList[j].icback = "White";
                }
            }
            Close();
        }

        //Esc 키
        private void Main_keydownEvent(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
            switch (e.Key.ToString())
            {
                case "Escape":
                    Close();
                    for (int j = 0; j < searchList.Count; j++)
                    {
                        searchList[j].check = false;
                        searchList[j].icback = "White";
                    }
                    break;
                default:
                    break;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;  // 이벤트가 다른 곳으로 전파되지 않도록 처리

            ScrollViewer scrollViewer = sender as ScrollViewer;
            double newOffset = scrollViewer.VerticalOffset + e.Delta;
            scrollViewer.ScrollToVerticalOffset(newOffset);
        }
    }
}
