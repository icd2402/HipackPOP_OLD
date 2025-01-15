
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;

namespace PopProject
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Main main;
        public static Main1 main1;
        private Mutex mutex = null;
        public static string userID = "0";
        public static string userName = "0";
        string Url; //서비스 주소

        public MainWindow()
        {
            InitializeComponent();

            LogManager.Info("----------------------------Program Start------------------------------", MethodBase.GetCurrentMethod().DeclaringType);
            this.Loaded += MainWindow_Loaded;

            try
            {
                //POP프로그램이 이미 실행중인지 확인
                Duplicate_execution("PopProject");

                // 아두이노 셋팅은 SettingJsonManager 내에서 초기화 내용에 적용
            }
            catch (Exception e)
            {
                LogManager.Error("메인화면 업로드 실패 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 윈도우의 상단 위치를 0으로 설정
            this.Top = 0;
        }
        //POP프로그램이 이미 실행중인지 확인
        private void Duplicate_execution(string mutexName)
        {
            //프로그램 중복실행 체크
            try
            {
                mutex = new Mutex(false, mutexName);

                if (mutex.WaitOne(0, false))
                {
                    LogManager.Info("POP 프로그램 중복체크 완료", MethodBase.GetCurrentMethod().DeclaringType);
                    
                    // 로긴 확인 및 MainCP Load
                    MainCP.Content = new Login();

                    //윈도우 전체화면 옵션
                    WindowState = SettingJsonManager.windowState ? WindowState.Maximized : WindowState.Normal;
                }
                else
                {
                    LogManager.Error("POP 프로그램 중복 사용으로 실행 실패", MethodBase.GetCurrentMethod().DeclaringType);
                    _ = System.Windows.MessageBox.Show("프로그램이 이미 실행 중입니다!", "알림2", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception e)
            {
                LogManager.Error("로그인 실패.\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
                _ = System.Windows.MessageBox.Show("로그인 실패.", "알림1", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
        }

        private void MainCP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (Cls_Variable.SharedValue==false)
                    {
                        DragMove();
                    }
                }
            }
            catch (Exception h)
            {
                string message = h.Message;
            }
            Cls_Variable.SharedValue = false;
        }

        //키보드 이벤트
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "Escape":
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }
        private void Win_closed(object sender, EventArgs e)
        {
            //관리자 계정이 아닐때만 현황판으로 꺼짐 신호 보냄 
            if (!SettingJsonManager.manager.Equals("1"))
            {
                CountManager.SendMsg(SettingJsonManager.mercName, "B");
            }

            LogManager.Info("프로그램 종료", MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
