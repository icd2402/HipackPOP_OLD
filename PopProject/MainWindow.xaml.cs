
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
                LogManager.Error("로그인 실패.", MethodBase.GetCurrentMethod().DeclaringType);
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
                    DragMove();
                }
            }
            catch (Exception h)
            {
                string message = h.Message;
            }
        }

        //화면켜짐 신호 송신    // 확인
        //private void invokeMercName(string mercName, string v)
        //{
        //    try
        //    {
        //        if (SettingJsonManager.corp_name.Equals("광성수출포장") || 
        //            SettingJsonManager.corp_name.Equals("대세물산") ||
        //            SettingJsonManager.corp_name.Equals("유창포장"))
        //        {
        //            popService(mercName, v);
        //        }
        //        else
        //        {
        //            SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);

        //        }
        //    }
        //    catch (Exception )
        //    {
        //    }
        //}

        //신규 서비스 Http 셋팅
        //private static HttpClient client = new HttpClient();
        //async void popService(string mercName, string str)
        //{
        //    //주소
        //    if (SettingJsonManager.corp_name.Equals("광성수출포장"))
        //    {
        //        Url = "http://ksbox.iptime.org:18888/pop/gathering"; //광성
        //    }
        //    else if (SettingJsonManager.corp_name.Equals("대세물산"))
        //    {
        //        Url = "http://dsbox.iptime.org:18888/pop/gathering"; //대세
        //    }
        //    else if (SettingJsonManager.corp_name.Equals("유창포장"))
        //    {
        //        Url = "http://211.253.37.232:18888/pop/gathering"; //유창
        //    }
        //    else if (SettingJsonManager.corp_name.Equals("금화"))
        //    {
        //        Url = "http://khbox.iptime.org:18888/pop/gathering"; //금화
        //    }
        //    else if (SettingJsonManager.corp_name.Equals("광성수출포장테스트") ||
        //            SettingJsonManager.corp_name.Equals("대세물산테스트") ||
        //            SettingJsonManager.corp_name.Equals("유창포장테스트") ||
        //            SettingJsonManager.corp_name.Equals("금화테스트"))
        //    {
        //        Url = "http://localhost:18888/pop/gathering"; //테스트용 주소(개인 DeskTop)
        //    }

        //    string CompanyCode = SettingJsonManager.corp_id; //업체코드
        //    string MercName = mercName; //설비명
        //    string Data = str.ToString(); //데이터

        //    await workCnt(Url, CompanyCode, MercName, Data);
        //}

        //신규 서비스 Http Get
        //async Task workCnt(string url, string companyCode, string mercName, string data)
        //{
        //    try
        //    {
        //        // 파라미터를 포함한 URL 생성
        //        string urlWithParameters = $"{url}/{companyCode}/{mercName}/{data}";
        //        // HTTP GET 요청 보내기
        //        HttpResponseMessage response = await client.GetAsync(urlWithParameters);

        //        if (response.IsSuccessStatusCode == false)
        //        {
        //            log.Error("서비스로 데이터 전송 실패");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

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
