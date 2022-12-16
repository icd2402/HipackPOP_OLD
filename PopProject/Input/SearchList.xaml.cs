using PopProject.Detail;
using PopProject.UserControl_;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        ObservableCollection<MainModel> searchList;

        public SearchList(ObservableCollection<MainModel> workList)
        {
            InitializeComponent();
            worklist = workList;
            _ = textbox_search.Focus();
        }

        public SearchList(ObservableCollection<MainModel> workList, bool check)
        {
            InitializeComponent();
            changeCheck = check;
            worklist = workList;
            _ = textbox_search.Focus();
        }

        private void KeyBoardCall()
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

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


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

       private void SearchChanged_Text(object sender, TextChangedEventArgs e)
       {
            searchList = new ObservableCollection<MainModel>();

            if (textbox_search.Text.Length == 0)
            {
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

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        private void ScrollViewer_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index;


            if (e.ClickCount == 2)
            {
                if (!changeCheck)
                {
                    ShowDetail(sender);
                }
                else
                {
                    Canvas canvas = (Canvas)sender;
                    MainModel context = (MainModel)canvas.DataContext;
                    index =  worklist.IndexOf(context);
                    Properties.Settings.Default.index = index;
                    Properties.Settings.Default.Save();

                    Close();
                    for (int j = 0; j < searchList.Count; j++)
                    {
                        searchList[j].check = false;
                        searchList[j].icback = "White";
                    }
                }
            }
            else if (e.ClickCount == 1)
            {
                if (sender.GetType().Name.Equals("Canvas"))
                {
                    Canvas canvas = (Canvas)sender;
                    index = ic.Items.IndexOf(canvas.DataContext);
                    Console.WriteLine("i = > " + index);

                    if (searchList[index].check)
                    {
                        searchList[index].check = false;
                        searchList[index].icback = "White";
                        //indexCheck = false;
                    }

                    else
                    {
                        //indexCheck = true;
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

        private void ShowDetail(object sender)
        {
            Canvas canvas = (Canvas)sender;
            MainModel context = (MainModel)canvas.DataContext;

            /* Consumer consumer = new Consumer(context);
             consumer_ = consumer;
             consumer.ShowDialog();
             consumer_ = null;
            */
            string special = "";
            Win_Detail win_detail = new Win_Detail(context, special, worklist, Properties.Settings.Default.detail_check);
            _ = win_detail.ShowDialog();
        }

        private void Keyboard_Call(object sender, MouseButtonEventArgs e)
        {
            KeyBoardCall();
        }

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
    }
}
