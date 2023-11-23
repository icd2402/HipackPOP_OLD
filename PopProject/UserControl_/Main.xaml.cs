using Newtonsoft.Json.Linq;
using PopProject.Consumer_;
using PopProject.Detail;
using PopProject.input;
using PopProject.Input;
using PopProject.MerList;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using System.Net.Http;

namespace PopProject.UserControl_
{
    /// <summary>
    /// Main.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Main : UserControl
    {

        int wdIGCOunt = 0;
        int arduconnCount = 0;
        private bool sortChecked = true;
        private DispatcherTimer timer;
        public static SerialPort arduSerial = new SerialPort();
        public static SerialPort arduSerial1 = new SerialPort();
        public static Win_input win_Input;
        public static Robot_Setting robot_Setting = null;
        public static Robot_Setting2 robot_Setting2 = null;
        public ObservableCollection<MainModel> workList;
        public ObservableCollection<MainModel> workList1;
        public ObservableCollection<MainModel> MachineCheck;
        public static Consumer consumer_;
        public static MainModel production = null;
        private int minuite = 0;
        private int minuite1 = 0;
        public static int str;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        public static bool passCheck = true; //넘기기
        public static DateTime dt;
        private bool CheckDateValue = true;
        private bool DateCheck = false;
        private bool CNS_Barcode_Send = false; //true이면 바코드 읽으면 CNS로 데이터 전송
        private bool oderLast_Sort = false;
        bool SignalR_Tick = false;
        private string SignalR_State = ""; //시그널R 상태
        public ILog log = LogManager.GetLogger(typeof(Main)); //로그
        string Url; //서비스 주소
        private bool rawData = false;
        private double LastConunt;

        public Main()
        {
            try
            {
                InitializeComponent();
                production = null;

                // 버전 번호를 텍스트 블록에 할당
                //string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //versionTextBlock.Text += version;

                //Log4net
                XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));

                //왼쪽상단 업체명
                corp_name.Text = Properties.Settings.Default.corp_name;
                string metrialitem = Properties.Settings.Default.meterialinfo;

                //중앙 설비명 텍스트
                string merName = Properties.Settings.Default.mercName;
                tbMercName.Text = merName;

                // 설비명 변경 Lock / Unlock 설정
                if (!Properties.Settings.Default.checkMerc)
                {
                    btnLogout.Background = Brushes.Green;
                    merc_lock.Text = "UNLOCK";
                }
                else
                {
                    merc_lock.Text = "LOCK";
                    btnLogout.Background = Brushes.Red;
                }

                try
                {
                    SignalRAdministrator conn = new SignalRAdministrator();
                    conn.Setconnection(true);
                }
                catch (Exception)
                {
                }

                

                //날짜 변경 버튼 활성화 (대세물산)
                if (Properties.Settings.Default.corp_name.Equals("대세물산") || 
                    Properties.Settings.Default.corp_name.Equals("대세물산테스트") || 
                    Properties.Settings.Default.corp_name.Equals("유창포장") || 
                    Properties.Settings.Default.corp_name.Equals("유창포장테스트") ||
                    Properties.Settings.Default.corp_name.Equals("광성수출포장") ||
                    Properties.Settings.Default.corp_name.Equals("광성수출포장테스트"))
                {
                    txtDate2.Visibility = Visibility.Visible;
                    txtDate3.Visibility = Visibility.Visible;
                }

                //오주포장 넘기기 정보 받기위한 SiganlR recive
                SignalRAdministrator.허브.On<string, string>("returnCount", recive);

                //관리자 계정이 아닐때만 현황판으로 켜짐 신호 보냄 
                if (!Properties.Settings.Default.manager.Equals("1"))
                {
                    InvokeMercName(Properties.Settings.Default.mercName, "A"); //설비명, 켜짐신호
                }

                Properties.Settings.Default.consumersumcount = 0;
                Properties.Settings.Default.Save();

                SetSetting(metrialitem); //작업정보 설장 값 저장
                ArduConn(); //아두이노 통신연결

                //신우,우리,누리,혜성,성주 //통수 설비일때 텍스트 변경
                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                {
                    //통수 설비일때
                    if (Properties.Settings.Default.CountType != 0)
                    {
                        work.Text = "박스수";
                        work.FontSize = 16;
                        box.Visibility = Visibility.Visible;
                        Canvas.SetTop(work, 1); //박스수 텍스트
                        Canvas.SetTop(box, 20); //작업수 텍스트
                    }
                }

                //옵션에서 스크롤바 사용 유/무 확인
                if (Properties.Settings.Default.scrollvarCheck)
                {
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    sv.PreviewMouseLeftButtonDown -= ScrollViewer_PreviewMouseLeftButtonDown;
                    sv.PreviewMouseLeftButtonUp -= Sv_PreviewMouseLeftButtonUp;
                    sv.PreviewMouseMove -= Sv_PreviewMouseMove;
                }
                else
                {
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    sv.PreviewMouseLeftButtonDown += ScrollViewer_PreviewMouseLeftButtonDown;
                    sv.PreviewMouseLeftButtonUp += Sv_PreviewMouseLeftButtonUp;
                    sv.PreviewMouseMove += Sv_PreviewMouseMove;
                }

                //설비점검 버튼 
                if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트") || Properties.Settings.Default.corp_name.Equals("광성수출포장") || Properties.Settings.Default.corp_name.Equals("광성수출포장테스트"))
                {
                    TextBlock tb계획순서변경 = this.계획순서변경;
                    tb계획순서변경.Margin = new Thickness(850, 10, 0, 0);

                    TextBlock tb설비점검 = this.설비점검;
                    tb설비점검.Margin = new Thickness(850, 60, 0, 0);

                    설비점검.Visibility = Visibility.Visible;
                }
                else
                {
                }

                //인터넷 연결 확인
                bool connected = NetworkInterface.GetIsNetworkAvailable();
                if (connected)
                {


                    TimerSet(); //시계
                    //TimerSet2(); //미사용
                    TimerSet3(); //SignalR 통신 모니터링(5초)
                    //TimerSet4(); //SignalR 통신 로그(1분)
                }
                else
                {
                    MessageBox_Dialog dialog = new MessageBox_Dialog("인터넷 상태를 확인해 주세요");
                    _ = dialog.ShowDialog();
                    log.Error("인터넷 연결 실패");

                }
            }
            catch (Exception e)
            {
                log.Error("메인화면 업로드 실패 : " + e.Message);
            }
        }

        //넘기기 신호 받는용
        private void recive(string arg1, string arg2) //설비명, 데이터
        {
            MainModel model = new MainModel();
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    //오주포장
                    switch (arg1)
                    {
                        case "인라인3호기":
                        case "인라인2호기":
                        case "후렉소1호기":

                            Console.WriteLine("넘기기 받은 신호" + arg1 + " : " + arg2);

                            //설비명 일치시
                            if (arg1.Equals(Properties.Settings.Default.mercName))
                            {
                                string special = "";
                                string[] Split_arg2 = arg2.Split('^');
                                if (Split_arg2.Length > 1 && Split_arg2[1].Equals("@"))
                                {
                                    for (int i = 0; i < workList.Count; i++)
                                    {
                                        if (workList[i].orderKey.ToString().Equals(Split_arg2[0]))
                                        {
                                            model = workList[i];
                                            special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                                            break;
                                        }
                                    }

                                    //넘기기 데이터로 디테일창 팝업
                                    if (passCheck)
                                    {
                                        try
                                        {
                                            can.Visibility = Visibility.Visible; //화면 어둡게 처리
                                            Win_Detail win_detail = new Win_Detail(model, special, workList, Properties.Settings.Default.detail_check, false); //모델,참고사항,workList,듀얼모니터,바코드
                                            win_detail.ShowDialog();

                                            // selectWorkList();
                                            // resultSearchDate();
                                            win_Input = null;
                                            win_detail = null;
                                        }
                                        catch (Exception e1)
                                        {
                                            log.Error("디테일 창 팝업 실패(넘기기) : " + e1.Message);
                                            log.Error("디테일 창 팝업 실패(넘기기) : " + e1.StackTrace);
                                        }

                                        //생산데이터 있으면 아두이노한테 현재값 요청
                                        try
                                        {
                                            if (production != null)
                                            {
                                                arduSerial.Write("B");
                                            }
                                        }
                                        catch (Exception)
                                        {

                                        }
                                        can.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                            break;

                        case "후렉소":
                            break;

                        default:
                            break;

                    }
                }), DispatcherPriority.ApplicationIdle);
            }
            catch (Exception)
            {
            }
        }

        //아두이노 시리얼 연결
        public void ArduConn()
        {
            try
            {
                //오픈이면
                if (arduSerial.IsOpen)
                {
                    Thread serialClose;
                    serialClose = new System.Threading.Thread(new System.Threading.ThreadStart(serialDisConnect));//닫았다 연결 시도
                    serialClose.IsBackground = true;
                    serialClose.Start();
                }
                else//닫혔으면 재연결 시도
                {
                    arduSerial.Encoding = Encoding.GetEncoding(65001);
                    arduSerial.PortName = Properties.Settings.Default.SerialName;
                    arduSerial.BaudRate = Properties.Settings.Default.baudrate;
                    arduSerial.DataReceived += ArduReReceive;

                    arduSerial.Open();
                    arduSerial.Write(Properties.Settings.Default.SensorRate);
                    btrSerial.Background = Brushes.Green;
                }

                //통신 끊겼으면 재연결 시도
                if (!arduSerial.IsOpen)
                {
                    arduSerial.Encoding = Encoding.GetEncoding(65001);
                    arduSerial.PortName = Properties.Settings.Default.SerialName;
                    arduSerial.BaudRate = Properties.Settings.Default.baudrate;
                    arduSerial.DataReceived += ArduReReceive;

                    arduSerial.Open();
                    arduSerial.Write(Properties.Settings.Default.SensorRate);
                    btrSerial.Background = Brushes.Green;
                }
            }
            catch (Exception e)
            {
                btrSerial.Background = Brushes.Red;
                log.Error("아두이노 연결 오류 : " + e.Message);
            }
        }

        // 아두이노 시리얼포트 닫기았다 오픈
        private void serialDisConnect()
        {
            try
            {
                arduSerial.DataReceived -= ArduReReceive;
                arduSerial.DtrEnable = false;
                arduSerial.RtsEnable = false;
                arduSerial.ReadExisting(); // 시리얼 읽기, 비우기
                arduSerial.DiscardInBuffer(); //수신 버퍼 삭제
                arduSerial.DiscardOutBuffer(); //송신 버퍼 삭제
                arduSerial.Close(); // 시리얼 포트 닫기
            }
            catch
            {
            }
            finally
            {
                arduSerial.Encoding = Encoding.GetEncoding(65001);
                arduSerial.PortName = Properties.Settings.Default.SerialName;
                arduSerial.BaudRate = Properties.Settings.Default.baudrate;
                arduSerial.DataReceived += ArduReReceive;

                arduSerial.Open();
                arduSerial.Write(Properties.Settings.Default.SensorRate);
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    btrSerial.Background = Brushes.Green;
                }));
            }
        }

        

        //로그 기록(구)
        private void LogWrite(string str)
        {
            // 현재 작업 디렉토리와 로그 파일 및 디렉토리 경로 지정
            string DirPath = Environment.CurrentDirectory + @"\Log";
            string FilePath = DirPath + "\\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string temp;

            // 작업 디렉토리와 파일 정보 가져오기
            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                // 디렉토리가 없으면 생성
                if (!di.Exists) Directory.CreateDirectory(DirPath);

                // 파일이 없으면 생성하고 로그 기록
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                // 파일이 있으면 로그 기록 추가
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }


        //아두이노 카운터값 수신 -> 카운터값 가공 -> 현황판으로 전송
        public void ArduReReceive(object sender, SerialDataReceivedEventArgs e)
        {
            float pCnt; //통수
            try
            {
                str = Math.Abs(int.Parse(arduSerial.ReadLine().Trim())); //아두이노 카운터값 공백 제거 후 절댓값으로 변경
                Console.WriteLine("아두이노 값(Main) : " + str); //실시간 값 확인용

                //생산중일때(생산정보 있을때)
                if (production != null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        //생산정보값이 정상일때
                        if (production != null)
                        {
                            //설정에서 이합철 체크시 통수값 * 2
                            pCnt = Properties.Settings.Default.twoSum ? production.pCnt * 2 : production.pCnt;
                        }
                        else
                        {
                            pCnt = 1;
                        }

                        //SaveDate가 비어있지 않고 현재 날짜와 다르면
                        if (!Properties.Settings.Default.saveDate.Equals("") && !Properties.Settings.Default.saveDate.Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                        {
                            //계획키가 일치할때
                            if (production.pplanKey == Properties.Settings.Default.pplanKey)
                            {
                                str = Properties.Settings.Default.count + str;
                            }
                        }

                        //매수일때
                        if (Properties.Settings.Default.CountType == 0)
                        {
                            production.sumPworkCount = str;
                            production.presultCount = str;
                            production.presultCount1 = str;

                            //인풋창이 팝업되어있으면
                            if (win_Input != null)
                            {
                                win_Input.inputModel.count = str;
                            }

                            InvokeCount(str, Properties.Settings.Default.mercName); //현황판으로 카운터 전송
                            Console.WriteLine("현황판으로 카운터(매수) 전송 값(Main) : " + Properties.Settings.Default.mercName + ", " + str); //실시간 값 확인용

                            if (Properties.Settings.Default.corp_name.Equals("광성수출포장") && rawData)
                            {
                                //RawData 저장(업체코드,설비명,데이터,오더키,수주키,제품ID)
                                new LocalDBManager().SetRawData(Properties.Settings.Default.corp_id, Properties.Settings.Default.mercName, str.ToString(), production.orderKey, production.prodID);
                            }
                        }
                        else //통수일때
                        {
                            //신우,우리,누리,성주 (이합철)
                            if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                            {
                                //설정에서 이합철 체크시 통수값 * 2 
                                pCnt = Properties.Settings.Default.twoSum ? (production.pCnt * 2) : 1;
                            }

                            //아두이노 카운터값 있으면
                            if (str != 0)
                            {
                                production.sumPworkCount = Convert.ToInt32(Math.Floor(str * pCnt)); //카운터값 * 통수
                               
                                //상자수 사용 체크시
                                if (Properties.Settings.Default.boxCheck)
                                {
                                    //다시 통수값 나누어줌
                                    if (pCnt >= 1)
                                    {
                                        production.sumPworkCount /= (float)pCnt;
                                    }
                                }
                                production.presultCount = production.sumPworkCount;
                                production.presultCount1 = production.sumPworkCount;

                                //인풋창 팝업시
                                if (win_Input != null)
                                {
                                    win_Input.inputModel.count = production.presultCount;
                                }

                                InvokeCount(Convert.ToInt32(production.presultCount), Properties.Settings.Default.mercName); //현황판으로 카운터 전송
                                Console.WriteLine("현황판으로 카운터(통수) 전송 값(Main) : " + Properties.Settings.Default.mercName + ", " + Convert.ToInt32(production.presultCount)); //실시간 값 확인용

                                if(Properties.Settings.Default.corp_name.Equals("광성수출포장") && rawData)
                                {
                                    //RawData 저장(업체코드,설비명,데이터,오더키,수주키,제품ID)
                                    new LocalDBManager().SetRawData(Properties.Settings.Default.corp_id, Properties.Settings.Default.mercName, production.presultCount.ToString(), production.orderKey, production.prodID);
                                }
                            }
                        }
                    }));
                }
            }
            catch (Exception e1)
            {
            }
        }

        //SignalR 연결상태 로그
        private void TimerSet4()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60)
            };
            timer.Tick += new EventHandler(Timer_Tick4);
            timer.Start();
        }

        //SignalR 연결상태 로그
        private void Timer_Tick4(object sender, EventArgs e)
        {
            SignalRAdministrator conn = new SignalRAdministrator();
            SignalR_State = conn.GetState().ToString();
            //log.Info("SignalR 상태(1분) : " + SignalR_State);
        }

        //SignalR 연결상태 모니터링용
        private void TimerSet3()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += new EventHandler(SignalR_Com_Status);
            timer.Start();
        }

        //SignalR 연결상태 모니터링용
        private void SignalR_Com_Status(object sender, EventArgs e)
        {
            SignalRAdministrator conn = new SignalRAdministrator();
            //Console.WriteLine("SignalR 연결상태: " + conn.GetState());

            //상태에 따른 글자 변환
            SignalR_State = conn.GetState().ToString();
            SState.Foreground = SignalR_State.Equals("Connected") ? Brushes.Green : Brushes.Black;
        }

        //미사용
        private void TimerSet2()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(Timer_Tick2);
            timer.Start();
        }

        //미사용
        private void Timer_Tick2(object sender, EventArgs e)
        {
        }

        // 1초 타이머 실행
        private void TimerSet()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }


        private bool isEventTriggered = false; //리스트 조회
        bool 설비점검중 = false;
        //시계 타이머
        private void Timer_Tick(object sender, EventArgs e)
        {
            var clock = DateTime.Now; //현재 시간

            //POP날짜와 현재 날짜가 일치할때에 현재시간 갱신
            if (CheckDateValue && !dt.ToString("D").Equals(DateTime.Now.ToString("D")))
            {
                dt = DateTime.Now;
            }
            
            string datePart = dt.ToString("yyyy년 MM월 dd일 dddd"); //날짜
            string datetime = string.Format("{0:D2}시 {1:D2}분 {2:D2}초", clock.Hour, clock.Minute, clock.Second);//시간
            txtDate.Text = datePart;  //날짜
            txtClock.Text = datetime; //시간

            //프로그램 실행 1초 후 리스트 조회 (한번만)
            if (!isEventTriggered)
            {
                if (win_Input == null || robot_Setting == null)
                {
                    SelectWorkList(); //생산 계획 리스트 조회
                }
                ResultSearchDate(); //생산 결과 리스트 조회
            }

            isEventTriggered = true;

            //지정된 시간에 컴퓨터를 종료
            if (dt.Hour == Properties.Settings.Default.hour)
            {
                if (dt.Minute == Properties.Settings.Default.minute)
                {
                    if (dt.Second == 0)
                    {
                        _ = Process.Start("shutdown.exe", "-s");
                    }
                }
            }

            //대세,광성은 바코드 동작 x
            if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트") || Properties.Settings.Default.corp_name.Equals("광성수출포장") || Properties.Settings.Default.corp_name.Equals("광성수출포장테스트"))
            {
            }
            else
            {
                //바코드
                _ = tbBarcode.Focus();
                tbBarcode.SelectAll();
            }

            //1분마다 실행
            if (minuite1 == 60)
            {
                bool connected = NetworkInterface.GetIsNetworkAvailable(); //네트워크 연결 여부 확인
                if (connected)
                {
                    //관리자 계정이 아닐때만 현황판으로 켜짐 신호 보냄 
                    if (!Properties.Settings.Default.manager.Equals("1"))
                    {
                        InvokeMercName(Properties.Settings.Default.mercName, "A"); //켜짐
                    }
                }
                minuite1 = 0;
            }

            //5분마다 실행
            if (minuite == 30)
            {
                bool connected = NetworkInterface.GetIsNetworkAvailable(); //네트워크 연결 여부 확인
                if (!arduSerial.IsOpen) //아두이노 오픈 안되어 있으면 다시 오픈 요청
                {
                    arduSerial.DataReceived -= ArduReReceive;
                    ArduConn();
                }

                if (connected) //네트워크 연결되어 있으면
                {
                    if (production != null && production.presultCount != 0)
                    {
                        InvokeCount(Convert.ToInt32(production.presultCount), Properties.Settings.Default.mercName); //현황판으로 카운터 전송
                    }

                    if (win_Input == null || robot_Setting == null)
                    {
                        if (!Properties.Settings.Default.corp_name.Equals("유창포장"))
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                    }
                    minuite = 0;
                }
            }

            //유창포장은 설비별로 다른 시간대에 계획/결과 리스트 조회하도록 함
            if (Properties.Settings.Default.corp_name.Equals("유창포장") || Properties.Settings.Default.corp_name.Equals("유창포장테스트"))
            {
                switch (Properties.Settings.Default.processType)
                {
                    case 1:
                        if (dt.Minute == 0 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 2:
                        if (dt.Minute == 3 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }

                        break;

                    case 3:
                        if (dt.Minute == 6 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 4:
                        if (dt.Minute == 9 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 5:
                        if (dt.Minute == 12 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 6:
                        if (dt.Minute == 15 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 7:
                        if (dt.Minute == 18 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 8:
                        if (dt.Minute == 21 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 9:
                        if (dt.Minute == 24 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 10:
                        if (dt.Minute == 27 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 11:
                        if (dt.Minute == 30 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 12:
                        if (dt.Minute == 33 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 13:
                        if (dt.Minute == 36 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 14:
                        if (dt.Minute == 39 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 15:
                        if (dt.Minute == 42 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 16:
                        if (dt.Minute == 45 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 17:
                        if (dt.Minute == 48 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    case 18:

                        if (dt.Minute == 51 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }

                        break;

                    case 19:
                        if (dt.Minute == 54 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }

                        break;

                    case 20:
                        if (dt.Minute == 57 && dt.Second == 0)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        break;

                    default:
                        break;
                }
            }
            minuite += 1;
            minuite1 += 1;
            // Console.WriteLine("minuite - " + minuite);
        }

        // POP 프로그램 켜짐/꺼짐 상태 송신
        private void InvokeMercName(string mercName, string v)
        {
            try
            {
                if (Properties.Settings.Default.corp_name.Equals("광성수출포장") || Properties.Settings.Default.corp_name.Equals("대세물산"))
                {
                    popService(mercName, v);
                }
                else 
                {
                    SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v); //invokeCount, DB명, 설비명, 문자
                }
            }
            catch (Exception)
            {
            }

        }

        // 현황판으로 카운터 전송
        public void InvokeCount(int str, string mercName)
        {
            try
            {
                if (Properties.Settings.Default.corp_name.Equals("광성수출포장") || Properties.Settings.Default.corp_name.Equals("광성수출포장테스트") || Properties.Settings.Default.corp_name.Equals("대세물산"))
                {
                    popService(mercName, str.ToString());
                }
                else
                {
                _ = SignalRAdministrator.허브.Invoke("invokeCount", Properties.Settings.Default.corp_id, mercName, str + ""); //invokeCount, DB명, 설비명, 문자(카운터값)
                }
            }
            catch (Exception)
            {
            }
        }

        //신규 서비스 Http 셋팅
        private static HttpClient client = new HttpClient();
        async Task popService(string mercName, string str)
        {
            //주소
            if (Properties.Settings.Default.corp_name.Equals("광성수출포장"))
            {
                Url = "http://ksbox.iptime.org:18888/pop/gathering"; //광성
            }
            else if (Properties.Settings.Default.corp_name.Equals("대세물산"))
            {
                Url = "http://dsbox.iptime.org:18888/pop/gathering"; //대세
            }
            else if (Properties.Settings.Default.corp_name.Equals("광성수출포장테스트") ||
                    Properties.Settings.Default.corp_name.Equals("대세물산테스트"))
            {
                Url = "http://localhost:18888/pop/gathering"; //테스트용 주소(개인 DeskTop)
            }

            string CompanyCode = Properties.Settings.Default.corp_id; //업체코드
            string MercName = mercName; //설비명
            string Data = str.ToString(); //데이터

            await workCnt(Url, CompanyCode, MercName, Data);
        }

        //신규 서비스 Http Get
        async Task workCnt(string url, string companyCode, string mercName, string data)
        {
            try
            {
                // 파라미터를 포함한 URL 생성
                string urlWithParameters = $"{url}/{companyCode}/{mercName}/{data}";
                // HTTP GET 요청 보내기
                HttpResponseMessage response = await client.GetAsync(urlWithParameters);

                if(response.IsSuccessStatusCode == false)
                {
                    log.Error("서비스로 데이터 전송 실패");
                }

                Console.WriteLine("서비스로 데이터 전송 값(Main) : " + urlWithParameters);
            }
            catch (Exception)
            {
            }
        }

        //자동 로그인 해제
        private void Serial_click(object sender, RoutedEventArgs e)
        {
            AuthCode auth = new AuthCode();
            auth.ShowDialog();
        }

        //원격연결 버튼
        private void Remote_click(object sender, MouseButtonEventArgs e)
        {
            _ = Process.Start("http://2007.asw.co.kr/client/index.jsp?cpcode=icd&autostart=true");
            MainModel production = null;
        }

        //날짜 클릭
        private void TxtDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dt = DateTime.Now;
            txtDate.Text = dt.ToString("D");
            if (dt.ToString("D").Equals(DateTime.Now.ToString("D")))
            {
                CheckDateValue = true;
            }
            else
            {
                CheckDateValue = false;
            }

            SelectWorkList(); //계획 조회
            ResultSearchDate(); //결과 조회
        }

                  

         //if
         

        //설비명 클릭
        private void Mer_click(object sender, MouseButtonEventArgs e)
        {
            // UNLOCK 일때
            if (!Properties.Settings.Default.checkMerc)
            {
                string before = Properties.Settings.Default.mercName; //현재 설비명 before에 담기
                MercList mercList = new MercList(); //리스트 가져오기
                _ = mercList.ShowDialog(); //설비 리스트 팝업
                tbMercName.Text = Properties.Settings.Default.mercName;

                if (!before.Equals(Properties.Settings.Default.mercName)) //설비명이 변경되면
                {
                    //관리자 계정이 아닐때만 실행 (혼선 방지)
                    if (!Properties.Settings.Default.manager.Equals("1"))
                    {
                        InvokeMercName(before, "B"); //변경전 설비명, 꺼짐신호
                        InvokeMercName(Properties.Settings.Default.mercName, "A"); //변경된 설비명, 켜짐신호
                        Properties.Settings.Default.error = "";
                        Properties.Settings.Default.error1 = "";
                        Properties.Settings.Default.Save();
                    }

                    log.Info("설비 변경됨 => " + Properties.Settings.Default.mercName);

                }
                else //설비명 변경 안했으면
                {
                    //관리자 계정이 아닐때만 현황판으로 켜짐 신호 보냄 (혼선 방지)
                    if (!Properties.Settings.Default.manager.Equals("1"))
                    {
                        InvokeMercName(Properties.Settings.Default.mercName, "A"); //기존 설비명, 켜짐신호
                    }
                }

                // 상단 바 계획 글자 옵션
                if (Properties.Settings.Default.CountType != 0) //통수일때 
                {
                    work.Text = "박스수";
                    work.FontSize = 16;
                    box.Visibility = Visibility.Visible;
                    Canvas.SetTop(work, 1);
                    Canvas.SetTop(box, 20);
                }
                else //매수 일떄
                {
                    work.Text = "계획수";
                    box.Visibility = Visibility.Collapsed;
                    Canvas.SetTop(work, 10);
                }

                SelectWorkList(); //계획 조회
                ResultSearchDate(); //결과 조회
            }
        }

        //계획 리스트 조회
        public void SelectWorkList()
        {
            wdIGCOunt = 0;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[PopWorkList]",
               new string[] { "@pplanDate" },
               new string[] { dt.ToString("yyyy-MM-dd")}, //날짜 변경용
               //new string[] { DateTime.Now.ToString("yyyy-MM-dd") }, //날짜 고정
               0
               );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    Visibility newProd;
                    workList = new ObservableCollection<MainModel>(); //메인 화면
                    workList1 = new ObservableCollection<MainModel>(); //순서변경 화면
                    foreach (JObject result in Data_Jarry)
                    {
                        double pplanNo = double.Parse(result["pplanNo"].ToString()) == 0 ? double.MaxValue : double.Parse(result["pplanNo"].ToString());
                        newProd = result["newProd"].Equals("1") ? Visibility.Visible : Visibility.Hidden;
                        MainModel listitem = new MainModel
                        {
                            pCnt = float.Parse(result["pCnt"].ToString()), //통수
                            pplanNo = pplanNo
                        };

                        if (result["boxType"].ToString() != null)
                        {
                            listitem.boxType = result["boxType"].ToString(); //상자 형식
                        }

                        if (result["PalletHeight"].ToString() != null)
                        {
                            listitem.PalletHeight = result["PalletHeight"].ToString(); //파레트 높이
                        }

                        if (result["PalletCount"].ToString() != null)
                        {
                            listitem.PalletCount = result["PalletCount"].ToString(); //파레트 적재매수
                        }

                        if (result["givWD"].ToString() != null)
                        {
                            listitem.givWD = result["givWD"].ToString(); //원단 발주
                        }

                        if (result["givSC"].ToString() != null)
                        {
                            listitem.givSC = result["givSC"].ToString(); //마닐라 발주
                        }


                        switch (Properties.Settings.Default.corp_name) //업체명따른 처래처 품명 텍스트 변경
                        {

                            case "오주포장":
                            case "대양포장":
                                run수주일자.Text = "[수주일자]";
                                run납품일자.Text = "[납품일자]";
                                run거래처.Text = "거래처";
                                run품명.Text = "품명";

                                //div.Text = "[수주날짜] [납품일자] 거래처/품명";
                                Canvas.SetLeft(div, 85);
                                listitem.visibility = Visibility.Visible;  //ScrollViewer 수주날짜
                                listitem.visibility1 = Visibility.Visible; //ScrollViewer 납품일자
                                listitem.left = 250;
                                break;

                            case "신우수출포장":
                            case "신우포장테스트":
                                run수주일자.Text = "[수주일자]";
                                run납품일자.Text = "";
                                run거래처.Text = "거래처";
                                run품명.Text = "품명";

                                //div.Text = "[수주날짜] 거래처/품명";
                                Canvas.SetLeft(div, 133);
                                listitem.visibility = Visibility.Visible;    //ScrollViewer 수주날짜
                                listitem.visibility1 = Visibility.Collapsed; //ScrollViewer 납품일자
                                listitem.left = 160;
                                break;

                            //기본 업체
                            default:
                                run수주일자.Text = "";
                                run납품일자.Text = "";
                                run거래처.Text = "거래처";
                                run품명.Text = "품명";

                                //div.Text = "거래처/품명";
                                Canvas.SetLeft(div, 160);
                                listitem.visibility = Visibility.Collapsed;  //ScrollViewer 수주날짜
                                listitem.visibility1 = Visibility.Collapsed; //ScrollViewer 납품일자
                                listitem.left = 70;
                                break;
                        }

                        //세팅에서 마닐라 정보 미체크시 (기본)
                        if (Properties.Settings.Default.meterialinfo.Equals(""))
                        {
                            info.Text = "장 * 폭 * 고";
                            listitem.SCBJCount = result["WDScore"].ToString(); //스코아
                            listitem.scbjcountColor = Brushes.Blue;
                            string[] split = result["WDScore"].ToString().Split('*');
                            if (split.Length > 0 && split.Length != 1)
                            {
                                if (!split[0].Equals(split[2]))
                                {
                                    listitem.scbjcountColor = Brushes.Red;
                                }
                            }
                            else
                            {
                                split = result["WDScore"].ToString().Split('+');
                                if (split.Length == 4)
                                {
                                    listitem.scbjcountColor = Brushes.Red;
                                }
                            }
                            listitem.SCIGCount = "";     //마닐라 입고 수량
                            listitem.SCmetrialName = "";
                            listitem.mLmW = result["pSize"].ToString(); //제품규격
                        }
                        else//세팅에서 마닐라 정보 체크시
                        {
                            info.Text = "마닐라정보";
                            listitem.mLmW = result["SCmW"].ToString() + "*" + result["SCmL"].ToString();

                            if (listitem.givSC.Equals("2"))
                            {
                                listitem.SCBJCount = "대체량   " + string.Format("{0:n0}", result["SCBJCount"]);
                                listitem.SCIGCount = "";
                            }
                            else
                            {
                                listitem.SCBJCount = "발주   " + string.Format("{0:n0}", result["SCBJCount"]);
                                listitem.SCIGCount = "입고   " + string.Format("{0:n0}", result["SCIGCount"]);

                                if (result["SCIGCount"].ToString().Equals("0") && !result["SCBJCount"].ToString().Equals("0"))
                                {

                                    listitem.SCIGCount = result["SCIGCount"].ToString();
                                }
                            }
                            listitem.SCmetrialName = result["SCmetrialName"].ToString();
                        }

                        listitem.pAccount = !result["pAccount"].ToString().Equals("0") ? "(" + result["pAccount"].ToString() + "차)" : "";  //생산 차수
                        listitem.ProcessMemo = result["ProcessMemo"].ToString(); //공정 비고
                        listitem.ProcessTypeName = result["ProcessTypeName"].ToString();
                        listitem.pplanMemo = result["pplanMemo"].ToString();
                        listitem.SpecialMemo = result["SpecialMemo"].ToString();
                        listitem.pplanMemo = result["pplanMemo"].ToString();
                        listitem.SpecialMemo = result["SpecialMemo"].ToString();

                        if (result["orderDate"].ToString() != null) //계획 날짜
                        {
                            listitem.orderDate = "[" + result["orderDate"].ToString().Replace("-", "/").Substring(2, 8) + "]";
                        }

                        if (listitem.givWD.Equals("2"))
                        {
                            listitem.WDBJCount = "대체량   " + string.Format("{0:n0}", result["WDBJCount"]);
                            listitem.WDIGCount = "";
                        }
                        else
                        {
                            listitem.WDBJCount = "발주   " + string.Format("{0:n0}", result["WDBJCount"]);
                            listitem.WDIGCount = "입고   " + string.Format("{0:n0}", result["WDIGCount"]);
                            if (result["WDIGCount"].ToString().Equals("0") && !result["WDBJCount"].ToString().Equals("0"))
                            {
                                try
                                {
                                    if (result["metCustomSname"] != null)
                                    {
                                        listitem.WDIGCount = result["metCustomSname"].ToString();
                                    }

                                }
                                catch (Exception e)
                                {
                                    listitem.WDIGCount = "입고   " + string.Format("{0:n0}", result["WDIGCount"]);
                                }
                            }
                        }

                        //품명 길이가 넘어갈 경우 두줄 표현 (오주 요청)
                        string prodNames = result["prodName"].ToString();
                        if (prodNames.Length > 32)
                        {
                            listitem.prodName = prodNames.Substring(0, 32); //첫번째 줄
                            listitem.prodName2 = prodNames.Substring(32); //두번째 줄
                            listitem.Canvas = "35"; //높이 조절
                        }
                        //안넘어 가면 한줄 표현 (기본)
                        else
                        {
                            listitem.Canvas = "43"; //높이 조절
                            listitem.prodName = result["prodName"].ToString(); //품명
                        }

                        listitem.pplanKey = int.Parse(result["pplanKey"].ToString());
                        listitem.orderKey = result["orderKey"].ToString();
                        listitem.ProcessType = int.Parse(result["ProcessType"].ToString());
                        listitem.pplanDate = "[" + result["pplanDate"].ToString().Replace("-", "/").Substring(2, 8) + "]";
                        listitem.prodName = result["prodName"].ToString();
                        listitem.customSName = result["customSName"].ToString();
                        listitem.paperGor = result["paperGor"].ToString() + " 골";
                        listitem.orderWorkNo = int.Parse(result["orderWorkNo"].ToString());
                        listitem.mercName = result["mercName"].ToString();
                        listitem.urgency = result["urgency"].ToString();
                        listitem.aL = result["aL"].ToString();
                        listitem.aW = result["aW"].ToString();
                        listitem.aLaW = result["WDmL"].ToString() + "*" + result["WDmW"].ToString();

                        listitem.ColorType = result["ColorType"].ToString().Replace("|", "");
                        listitem.ConnetingType = result["ConnetingType"].ToString();
                        listitem.PrintNo = result["PrintNo"].ToString();
                        listitem.Printposition = result["Printposition"].ToString();
                        listitem.PushingNo = result["PushingNo"].ToString();
                        listitem.Pushingposition = result["Pushingposition"].ToString();

                        if (result["corpName"] != null)
                        {
                            listitem.corpName = result["corpName"].ToString();
                        }
                        listitem.handChk = result["handChk"].ToString();
                        listitem.PackType = result["PackType"].ToString();
                        listitem.filmNo = result["filmNo"].ToString();
                        listitem.CoatingType = result["CoatingType"].ToString();
                        listitem.PalletType = result["PalletType"].ToString();
                        listitem.PackCount = result["PackCount"].ToString();
                        listitem.pplanCount = int.Parse(result["pplanCount"].ToString());

                        //신우,우리,누리
                        if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("우리포장산업테스트") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("누리팩테스트"))
                        {
                            ss.Visibility = Visibility.Visible; //집계창 표시
                        }
                        else
                        {
                            Grid.SetRowSpan(workListBorder, 2); //WorkList Grid.Row3까지 확장 
                            ss.Visibility = Visibility.Collapsed; //집계창 표시해제
                        }

                        //신우,우리,누리,혜성,성주
                        if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                        {
                            //매수
                            if (Properties.Settings.Default.CountType == 0)
                            {
                                listitem.boxvisibility = Visibility.Collapsed;
                            }
                            else//통수
                            {
                                listitem.pplanboxCount = (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                            }
                        }
                        else
                        {
                            listitem.boxvisibility = Visibility.Collapsed;
                        }

                        //옵션>상자수 사용
                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.pplanCount = Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("성주포장")
                                ? int.Parse(result["pplanCount"].ToString())
                                : (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                        }

                        listitem.presultCount = float.Parse(result["presultCount"].ToString());

                        //옵션>상자수 사용
                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.presultCount /= listitem.pCnt;
                            listitem.presultCount1 /= listitem.pCnt;
                        }
                        else
                        {
                            //통수
                            if (Properties.Settings.Default.CountType == 1)
                            {
                                //신우,우리,누리,성주
                                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                                {
                                    listitem.presultCount = float.Parse(result["presultCount"].ToString()) / listitem.pCnt;
                                    listitem.presultCount1 = float.Parse(result["presultCount"].ToString()) / listitem.pCnt;
                                }
                                else//그외 업체
                                {
                                    listitem.presultCount = float.Parse(result["presultCount"].ToString());
                                    listitem.presultCount1 = float.Parse(result["presultCount"].ToString());
                                }
                            }
                            else//매수
                            {
                                listitem.presultCount = float.Parse(result["presultCount"].ToString());
                                listitem.presultCount1 = float.Parse(result["presultCount"].ToString());
                            }

                        }

                        listitem.pSize = result["pSize"].ToString();
                        listitem.orderCount = int.Parse(result["orderCount"].ToString());
                        listitem.orderLast = result["orderLast"].ToString();

                        if (result["orderLast"].ToString() != null)
                        {
                            listitem.orderLast = "[" + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8) + "]";
                        }

                        listitem.prodID = result["prodID"].ToString();
                        listitem.newProd = newProd;
                        listitem.eDatetime = "";
                        listitem.sDatetime = "E";
                        listitem.subOb = new List<SubModel>();
                        listitem.resultList = new ObservableCollection<InputModel>();
                        listitem.bMember = bool.Parse(result["bMember"].ToString());
                        listitem.workMemo = result["workMemo"].ToString();

                        //오주,대양
                        if (!Properties.Settings.Default.corp_name.Equals("오주포장") && !Properties.Settings.Default.corp_name.Equals("대양포장") && !Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                        {
                            listitem.icback = listitem.bMember ? "#AFE1FF" : "White"; //실수요자
                            listitem.pStateVisi = Visibility.Collapsed;
                        }
                        else //그외 업체
                        {
                            listitem.pStateVisi = Visibility.Visible;
                            listitem.icback = result["WDIGCount"].ToString().Equals("0") || result["WDIGCount"].ToString().Equals("0.0") ? "White" : "#AFE1FF";
                            if (result["WDIGCount"].ToString().Equals("0") || result["WDIGCount"].ToString().Equals("0.0"))
                            {
                                wdIGCOunt++;
                            }
                            if (listitem.givWD.Equals("2"))
                            {
                                listitem.icback = "#AFE1FF";
                            }
                        }

                        if (result["pState"].ToString() != null)
                        {
                            listitem.pState = result["pState"].ToString();
                            switch (listitem.pState)
                            {
                                case "0":
                                    listitem.pStateString = "정상";
                                    listitem.pStateSColor = "BLUE";
                                    break;
                                case "1":
                                    listitem.pStateString = "보류";
                                    listitem.pStateSColor = "GREEN";
                                    break;
                                case "2":
                                    listitem.pStateString = "지급";
                                    listitem.pStateSColor = "BLUE";
                                    break;
                                case "3":
                                    listitem.pStateString = "초지급";
                                    listitem.pStateSColor = "BLUE";
                                    break;
                                case "4":
                                    listitem.pStateString = "상차";
                                    listitem.pStateSColor = "BLUE";
                                    break;
                            }
                        }

                        //오주포장
                        if (Properties.Settings.Default.mercName.Equals("인라인2호기") || Properties.Settings.Default.mercName.Equals("인라인3호기"))
                        {
                            if (listitem.ProcessType == 1 && listitem.ProcessType == 7 || listitem.ProcessType == 8) //인쇄(1), 철접합(7), 풀접합(8)
                            {
                                //작업정보 설정값
                                string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                                for (int i = 0; i < setting.Length; i++)
                                {
                                    string item = "";
                                    switch (setting[i])
                                    {
                                        case "1":
                                            item = result["filmNo"].ToString();
                                            break;
                                        case "2":
                                            item = result["CoatingType"].ToString();
                                            break;
                                        case "3":
                                            item = result["PrintNo"].ToString();
                                            break;
                                        case "4":
                                            item = result["Printposition"].ToString();
                                            break;
                                        case "5":
                                            item = result["PushingNo"].ToString();
                                            break;
                                        case "6":
                                            item = result["Pushingposition"].ToString();
                                            break;
                                        case "7":
                                            item = Properties.Settings.Default.color_check
                                                   ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ")
                                                   : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
                                            break;
                                        case "8":
                                            item = result["ConnetingType"].ToString();
                                            break;
                                        case "9":
                                            item = result["handChk"].ToString();
                                            break;
                                        case "10":
                                            item = result["PackCount"].ToString();
                                            break;
                                        case "11":
                                            item = result["PackType"].ToString();
                                            break;
                                        case "12":
                                            item = result["workMemo"].ToString();
                                            break;
                                        case "13":
                                            item = result["pplanMemo"].ToString();
                                            break;
                                        case "14":
                                            item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                                   ? "-"
                                                   : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                            break;
                                        case "15":
                                            item = result["PalletType"].ToString();
                                            break;
                                        case "16":
                                            item = result["SpecialMemo"].ToString();
                                            break;

                                        default:
                                            break;
                                    }

                                    //작업정보 표현
                                    if (i == 0)
                                    {
                                        listitem.item1 = item.Replace("\r\n", ""); //첫째줄
                                    }
                                    else if (i == 1)
                                    {
                                        listitem.item2 = item.Replace("\r\n", ""); //둘째줄
                                    }
                                    else if (i == 2)
                                    {
                                        listitem.item3 = item.Replace("\r\n", ""); //셋째줄
                                    }
                                    else if (i == 3)
                                    {
                                        listitem.item4 = item.Replace("\r\n", ""); //넷째줄
                                    }

                                }

                                //orderKey와 orderWorkNo일치 값 찾기
                                foreach (JObject resultSub in Data_Jarry.Where((x) => (listitem.orderKey + listitem.orderWorkNo).Equals(x["orderKey"].ToString() + x["orderWorkNo"].ToString())))
                                {
                                    SubModel subItem = new SubModel();

                                    subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();              //공정 순번
                                    subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();    //공정명
                                    subItem.processType = int.Parse(resultSub["ProcessType"].ToString()); //공정ID
                                    subItem.orderKey = resultSub["orderKey"].ToString();                  //오더키
                                    subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());       //계획량

                                    //결과값 있을때 공정 아이콘 검정으로 처리
                                    if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                                    {
                                        subItem.stateColor = Brushes.Black; //배경
                                        subItem.stateFontColor = Brushes.White; //글자
                                        subItem.presultCount = int.Parse(resultSub["presultCount"].ToString()); //결과수량

                                        //불량수량 있으면 결과값에 - 불량수량
                                        if (!resultSub["perrCount"].ToString().Equals(""))
                                        {
                                            subItem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                                        }
                                    }
                                    //결과값 없을때 공정 아이콘 흰색 처리
                                    else
                                    {
                                        subItem.stateColor = Brushes.White;
                                        subItem.stateFontColor = Brushes.Black;
                                    }

                                    listitem.subOb.Add(subItem);
                                }

                                //공정 순번으로 오름차순 정렬
                                listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                if (listitem.subOb.Count == 2)
                                {
                                    //공정명에 톰슨이 없거나 "2"라는 글자가 없을 경우에만 리스트 등록 
                                    if (listitem.subOb.Where((x) => x.ProcessTypeName.ToString().Equals("톰슨") || x.ProcessTypeName.ToString().Contains("2")).Count() == 0)
                                    {
                                        workList.Add(listitem);
                                    }
                                }
                            }
                        }
                        //오주포장 (풀접합)
                        else if (Properties.Settings.Default.mercName.Equals("풀접합") && Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (listitem.ProcessType == 8 || listitem.ProcessType == 7 || listitem.ProcessType == 5) //7:철접합  8: 풀접합
                            {
                                //작업정보 설정값
                                string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                                for (int i = 0; i < setting.Length; i++)
                                {
                                    string item = "";
                                    switch (setting[i])
                                    {
                                        case "1":
                                            item = result["filmNo"].ToString();
                                            break;
                                        case "2":
                                            item = result["CoatingType"].ToString();
                                            break;
                                        case "3":
                                            item = result["PrintNo"].ToString();
                                            break;
                                        case "4":
                                            item = result["Printposition"].ToString();
                                            break;
                                        case "5":
                                            item = result["PushingNo"].ToString();
                                            break;
                                        case "6":
                                            item = result["Pushingposition"].ToString();
                                            break;
                                        case "7":
                                            item = Properties.Settings.Default.color_check
                                                   ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ")
                                                   : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
                                            break;
                                        case "8":
                                            item = result["ConnetingType"].ToString();
                                            break;
                                        case "9":
                                            item = result["handChk"].ToString();
                                            break;
                                        case "10":
                                            item = result["PackCount"].ToString();
                                            break;
                                        case "11":
                                            item = result["PackType"].ToString();
                                            break;
                                        case "12":
                                            item = result["workMemo"].ToString();
                                            break;
                                        case "13":
                                            item = result["pplanMemo"].ToString();
                                            break;
                                        case "14":
                                            item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                               ? "-"
                                               : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                            break;
                                        case "15":
                                            item = result["PalletType"].ToString();
                                            break;
                                        case "16":
                                            item = result["SpecialMemo"].ToString();
                                            break;

                                        default:
                                            break;
                                    }

                                    //작업정보 표현
                                    if (i == 0)
                                    {
                                        listitem.item1 = item.Replace("\r\n", ""); //첫째줄
                                    }
                                    else if (i == 1)
                                    {
                                        listitem.item2 = item.Replace("\r\n", ""); //둘째줄
                                    }
                                    else if (i == 2)
                                    {
                                        listitem.item3 = item.Replace("\r\n", ""); //셋째줄
                                    }
                                    else if (i == 3)
                                    {
                                        listitem.item4 = item.Replace("\r\n", ""); //넷째줄
                                    }
                                }

                                foreach (JObject resultSub in Data_Jarry)
                                {
                                    SubModel subItem = new SubModel();
                                    if (listitem.orderKey.Equals(resultSub["orderKey"].ToString()))
                                    {
                                        if (listitem.pplanKey.Equals(resultSub["pplanKey"].ToString()))
                                        {

                                        }
                                    }

                                    //orderKey와 orderWorkNo일치 값 찾기
                                    if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                    {
                                        subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();              //공정 순번
                                        subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();    //공정명
                                        subItem.processType = int.Parse(resultSub["ProcessType"].ToString()); //공정ID
                                        subItem.orderKey = resultSub["orderKey"].ToString();                  //오더키
                                        subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());       //계획량

                                        //결과값 있을때 공정 아이콘 검정으로 처리
                                        if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                                        {
                                            subItem.stateColor = Brushes.Black;
                                            subItem.stateFontColor = Brushes.White;
                                            subItem.presultCount = int.Parse(resultSub["presultCount"].ToString());

                                            if (!resultSub["perrCount"].ToString().Equals(""))
                                            {
                                                subItem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                                            }
                                        }
                                        //결과값 없을때 공정 아이콘 흰색으로 처리
                                        else
                                        {
                                            subItem.stateColor = Brushes.White;
                                            subItem.stateFontColor = Brushes.Black;
                                        }
                                        listitem.subOb.Add(subItem);
                                    }
                                }
                                listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                workList.Add(listitem);
                            }
                        }
                        //오주포장 (베가)
                        else if (Properties.Settings.Default.mercName.Equals("베가") && Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (listitem.ProcessType == 8) //풀접합
                            {
                                string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                                for (int i = 0; i < setting.Length; i++)
                                {
                                    string item = "";
                                    switch (setting[i])
                                    {
                                        case "1":
                                            item = result["filmNo"].ToString();
                                            break;
                                        case "2":
                                            item = result["CoatingType"].ToString();
                                            break;
                                        case "3":
                                            item = result["PrintNo"].ToString();
                                            break;
                                        case "4":
                                            item = result["Printposition"].ToString();
                                            break;
                                        case "5":
                                            item = result["PushingNo"].ToString();
                                            break;
                                        case "6":
                                            item = result["Pushingposition"].ToString();
                                            break;
                                        case "7":
                                            item = Properties.Settings.Default.color_check
                                                ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ")
                                                : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
                                            break;
                                        case "8":
                                            item = result["ConnetingType"].ToString();
                                            break;
                                        case "9":
                                            item = result["handChk"].ToString();
                                            break;
                                        case "10":
                                            item = result["PackCount"].ToString();
                                            break;
                                        case "11":
                                            item = result["PackType"].ToString();
                                            break;
                                        case "12":
                                            item = result["workMemo"].ToString();
                                            break;
                                        case "13":
                                            item = result["pplanMemo"].ToString();
                                            break;
                                        case "14":
                                            item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                               ? "-"
                                               : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                            break;
                                        case "15":
                                            item = result["PalletType"].ToString();
                                            break;
                                        case "16":
                                            item = result["SpecialMemo"].ToString();
                                            break;

                                        default:
                                            break;
                                    }

                                    //작업정보 표현
                                    if (i == 0)
                                    {
                                        listitem.item1 = item.Replace("\r\n", ""); //첫째줄
                                    }
                                    else if (i == 1)
                                    {
                                        listitem.item2 = item.Replace("\r\n", ""); //둘째줄
                                    }
                                    else if (i == 2)
                                    {
                                        listitem.item3 = item.Replace("\r\n", ""); //셋째줄
                                    }
                                    else if (i == 3)
                                    {
                                        listitem.item4 = item.Replace("\r\n", ""); //넷째줄
                                    }
                                }

                                foreach (JObject resultSub in Data_Jarry)
                                {
                                    SubModel subItem = new SubModel();
                                    if (listitem.orderKey.Equals(resultSub["orderKey"].ToString()))
                                    {
                                        if (listitem.pplanKey.Equals(resultSub["pplanKey"].ToString()))
                                        {
                                        }
                                    }

                                    //orderKey와 orderWorkNo일치 값 찾기
                                    if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                    {
                                        subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();              //공정 순번
                                        subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();    //공정명
                                        subItem.processType = int.Parse(resultSub["ProcessType"].ToString()); //공정ID
                                        subItem.orderKey = resultSub["orderKey"].ToString();                  //오더키
                                        subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());       //계획량

                                        //결과값 있을때 공정 아이콘 검정으로 처리
                                        if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                                        {
                                            subItem.stateColor = Brushes.Black;
                                            subItem.stateFontColor = Brushes.White;
                                            subItem.presultCount = int.Parse(resultSub["presultCount"].ToString());

                                            if (!resultSub["perrCount"].ToString().Equals(""))
                                            {
                                                subItem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                                            }
                                        }
                                        //결과값 없을때 공정 아이콘 흰색으로 처리
                                        else
                                        {
                                            subItem.stateColor = Brushes.White;
                                            subItem.stateFontColor = Brushes.Black;
                                        }

                                        listitem.subOb.Add(subItem);
                                    }
                                }

                                listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                workList.Add(listitem);
                            }

                        }
                        //오주포장 (2공장인쇄기)
                        else if (Properties.Settings.Default.mercName.Equals("2공장인쇄기") && Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (listitem.ProcessType == 27) //2공장 인쇄기
                            {
                                if (listitem.ProcessType == Properties.Settings.Default.processType)
                                {
                                    //작업정보 설정값
                                    string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                                    for (int i = 0; i < setting.Length; i++)
                                    {
                                        string item = "";
                                        switch (setting[i])
                                        {
                                            case "1":
                                                item = result["filmNo"].ToString();
                                                break;
                                            case "2":
                                                item = result["CoatingType"].ToString();
                                                break;
                                            case "3":
                                                item = result["PrintNo"].ToString();
                                                break;
                                            case "4":
                                                item = result["Printposition"].ToString();
                                                break;
                                            case "5":
                                                item = result["PushingNo"].ToString();
                                                break;
                                            case "6":
                                                item = result["Pushingposition"].ToString();
                                                break;
                                            case "7":
                                                item = Properties.Settings.Default.color_check ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ") : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
                                                break;
                                            case "8":
                                                item = result["ConnetingType"].ToString();
                                                break;
                                            case "9":
                                                item = result["handChk"].ToString();
                                                break;
                                            case "10":
                                                item = result["PackCount"].ToString();
                                                break;
                                            case "11":
                                                item = result["PackType"].ToString();
                                                break;
                                            case "12":
                                                item = result["workMemo"].ToString();
                                                break;
                                            case "13":
                                                item = result["pplanMemo"].ToString();
                                                break;
                                            case "14":
                                                item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                                ? "-"
                                                : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                                break;
                                            case "15":
                                                item = result["PalletType"].ToString();
                                                break;
                                            case "16":
                                                item = result["SpecialMemo"].ToString();
                                                break;

                                            default:
                                                break;
                                        }

                                        //작업정보 표현
                                        if (i == 0)
                                        {
                                            listitem.item1 = item.Replace("\r\n", ""); //첫째줄
                                        }
                                        else if (i == 1)
                                        {
                                            listitem.item2 = item.Replace("\r\n", ""); //둘째줄
                                        }
                                        else if (i == 2)
                                        {
                                            listitem.item3 = item.Replace("\r\n", ""); //셋째줄
                                        }
                                        else if (i == 3)
                                        {
                                            listitem.item4 = item.Replace("\r\n", ""); //넷째줄
                                        }
                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {
                                        SubModel subItem = new SubModel();
                                        //Console.WriteLine(resultSub["pplanKey"].ToString());
                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();             //공정 순번
                                            subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();   //공정명
                                            subItem.processType = int.Parse(resultSub["ProcessType"].ToString());//공정ID
                                            subItem.orderKey = resultSub["orderKey"].ToString();                 //오더키
                                            subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());      //계획량

                                            //결과값 있을때 공정 아이콘 검정으로 처리
                                            if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                                            {
                                                subItem.stateColor = Brushes.Black;
                                                subItem.stateFontColor = Brushes.White;
                                                subItem.presultCount = int.Parse(resultSub["presultCount"].ToString());
                                                if (!resultSub["perrCount"].ToString().Equals(""))
                                                {
                                                    subItem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                                                }
                                            }
                                            //결과값 없을때 공정 아이콘 흰색으로 처리
                                            else
                                            {
                                                subItem.stateColor = Brushes.White;
                                                subItem.stateFontColor = Brushes.Black;
                                            }
                                            listitem.subOb.Add(subItem);
                                        }
                                    }
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                    workList.Add(listitem);
                                }
                            }
                        }

                        //일반 공정
                        else
                        {
                            //공정명 없을때
                            if (listitem.mercName.Equals(""))
                            {
                                //설비 번호로 구별
                                if (listitem.ProcessType == Properties.Settings.Default.processType)
                                {
                                    //작업정보 설정값
                                    string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                                    for (int i = 0; i < setting.Length; i++)
                                    {
                                        string item = "";
                                        switch (setting[i])
                                        {
                                            case "1":
                                                item = result["filmNo"].ToString();
                                                break;
                                            case "2":
                                                item = result["CoatingType"].ToString();
                                                break;
                                            case "3":
                                                item = result["PrintNo"].ToString();
                                                break;
                                            case "4":
                                                item = result["Printposition"].ToString();
                                                break;
                                            case "5":
                                                item = result["PushingNo"].ToString();
                                                break;
                                            case "6":
                                                item = result["Pushingposition"].ToString();
                                                break;
                                            case "7":
                                                item = Properties.Settings.Default.color_check ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ") : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
                                                break;
                                            case "8":
                                                item = result["ConnetingType"].ToString();
                                                break;
                                            case "9":
                                                item = result["handChk"].ToString();
                                                break;
                                            case "10":
                                                item = result["PackCount"].ToString();
                                                break;
                                            case "11":
                                                item = result["PackType"].ToString();
                                                break;
                                            case "12":
                                                item = result["workMemo"].ToString();
                                                break;
                                            case "13":
                                                item = result["pplanMemo"].ToString();
                                                break;
                                            case "14":
                                                item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                                ? "-"
                                                : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                                break;
                                            case "15":
                                                item = result["PalletType"].ToString();
                                                break;
                                            case "16":
                                                item = result["SpecialMemo"].ToString();
                                                break;

                                            default:
                                                break;
                                        }

                                        //작업정보 표현
                                        if (i == 0)
                                        {
                                            listitem.item1 = item.Replace("\r\n", ""); //첫째줄
                                        }
                                        else if (i == 1)
                                        {
                                            listitem.item2 = item.Replace("\r\n", ""); //둘째줄
                                        }
                                        else if (i == 2)
                                        {
                                            listitem.item3 = item.Replace("\r\n", ""); //셋째줄
                                        }
                                        else if (i == 3)
                                        {
                                            listitem.item4 = item.Replace("\r\n", ""); //넷째줄
                                        }
                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {
                                        SubModel subItem = new SubModel();
                                        //Console.WriteLine(resultSub["pplanKey"].ToString());
                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();              //공정 순번
                                            subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();    //공정명
                                            subItem.processType = int.Parse(resultSub["ProcessType"].ToString()); //공정ID
                                            subItem.orderKey = resultSub["orderKey"].ToString();                  //오더키
                                            subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());       //계획량

                                            //결과값 있을때 공정 아이콘 검정으로 처리
                                            if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                                            {
                                                subItem.stateColor = Brushes.Black;
                                                subItem.stateFontColor = Brushes.White;
                                                subItem.presultCount = int.Parse(resultSub["presultCount"].ToString());
                                                if (!resultSub["perrCount"].ToString().Equals(""))
                                                {
                                                    subItem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                                                }
                                            }
                                            //결과값 없을때 공정 아이콘 흰색으로 처리
                                            else
                                            {
                                                subItem.stateColor = Brushes.White;
                                                subItem.stateFontColor = Brushes.Black;
                                            }
                                            listitem.subOb.Add(subItem);
                                        }
                                    }

                                    listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                    workList.Add(listitem);
                                }
                            }
                            //공정명 있을때
                            else
                            {
                                //공정명으로 구분
                                if (listitem.mercName.Equals(Properties.Settings.Default.mercName))
                                {
                                    //작업정보 설정값
                                    string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                                    for (int i = 0; i < setting.Length; i++)
                                    {
                                        string item = "";
                                        switch (setting[i])
                                        {
                                            case "1":
                                                item = result["filmNo"].ToString();
                                                break;
                                            case "2":
                                                item = result["CoatingType"].ToString();
                                                break;
                                            case "3":
                                                item = result["PrintNo"].ToString();
                                                break;
                                            case "4":
                                                item = result["Printposition"].ToString();
                                                break;
                                            case "5":
                                                item = result["PushingNo"].ToString();
                                                break;
                                            case "6":
                                                item = result["Pushingposition"].ToString();
                                                break;
                                            case "7":
                                                item = result["ColorType"].ToString().Replace("|", "");
                                                break;
                                            case "8":
                                                item = result["ConnetingType"].ToString();
                                                break;
                                            case "9":
                                                item = result["handChk"].ToString();
                                                break;
                                            case "10":
                                                item = result["PackCount"].ToString();
                                                break;
                                            case "11":
                                                item = result["PackType"].ToString();
                                                break;
                                            case "12":
                                                item = result["workMemo"].ToString();
                                                break;
                                            case "13":
                                                item = result["pplanMemo"].ToString();
                                                break;
                                            case "14":
                                                item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                               ? "-"
                                               : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                                break;
                                            case "15":
                                                item = result["PalletType"].ToString();
                                                break;
                                            case "16":
                                                item = result["SpecialMemo"].ToString();
                                                break;

                                            default:
                                                break;
                                        }

                                        //작업정보 표현
                                        if (i == 0)
                                        {
                                            listitem.item1 = item.Replace("\r\n", ""); //첫째줄
                                        }
                                        else if (i == 1)
                                        {
                                            listitem.item2 = item.Replace("\r\n", ""); //둘째줄
                                        }
                                        else if (i == 2)
                                        {
                                            listitem.item3 = item.Replace("\r\n", ""); //셋째줄
                                        }
                                        else if (i == 3)
                                        {
                                            listitem.item4 = item.Replace("\r\n", ""); //넷째줄
                                        }

                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {
                                        SubModel subitem = new SubModel();

                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subitem.ProcessSeq = resultSub["ProcessSeq"].ToString();              //공정 순번
                                            subitem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();    //공정명
                                            subitem.processType = int.Parse(resultSub["ProcessType"].ToString()); //공정ID
                                            subitem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());       //오더키
                                            subitem.orderKey = resultSub["orderKey"].ToString();                  //계획량

                                            //결과값 있을때 공정 아이콘 검정으로 처리
                                            if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                                            {
                                                subitem.stateColor = Brushes.Black;
                                                subitem.stateFontColor = Brushes.White;
                                                subitem.presultCount = int.Parse(resultSub["presultCount"].ToString());

                                                if (!resultSub["perrCount"].ToString().Equals(""))
                                                {
                                                    subitem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                                                }
                                            }
                                            //결과값 없을때 공정 아이콘 흰색으로 처리
                                            else
                                            {
                                                subitem.stateColor = Brushes.White;
                                                subitem.stateFontColor = Brushes.Black;
                                            }

                                            listitem.subOb.Add(subitem);
                                        }
                                    }
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                    workList.Add(listitem);
                                }
                            }
                        }
                    }
                    workList1 = workList;
                    ic.ItemsSource = workList;
                }
                else
                {
                    //MessageBox_Dialog box = new MessageBox_Dialog("PopWorkList 조회 오류!!");
                    //_ = box.ShowDialog();
                }
                
                log.Info(Properties.Settings.Default.mercName + " 생산계획 리스트 조회 완료");
            }
            else
            {
                log.Error("생산계획 조회 오류");
            }
            실적현황조회();
        }

        //신우,우리,누리 사용중
        private void 실적현황조회()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel 실적returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[spSel425processinfo_workProcess]",
               new string[] { "@출력일", "@출력일2", "@설비명" },
               new string[] { DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), Properties.Settings.Default.mercName },
               0
               );
            if (실적returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(실적returnData.리턴값);
                if (Data_Jarry != null)
                {
                    List<실적Model> 실적List = new List<실적Model>();

                    foreach (JObject result in Data_Jarry)
                    {
                        실적Model 실적Item = new 실적Model();

                        실적Item.pworkKey = Convert.ToInt32(result["pworkKey"].ToString());
                        실적Item.ProcessSeq = Convert.ToInt32(result["ProcessSeq"].ToString());
                        실적Item.ProcessType = Convert.ToInt32(result["ProcessType"].ToString());
                        실적Item.ProcessName = result["ProcessName"].ToString();
                        실적Item.customID = Convert.ToInt32(result["customID"].ToString());
                        실적Item.customSName = result["customSName"].ToString();
                        실적Item.prodID = Convert.ToInt32(result["prodID"].ToString());
                        실적Item.prodName = result["prodName"].ToString();
                        실적Item.pplanCount = Convert.ToInt32(result["pplanCount"].ToString());
                        실적Item.pworkCount = Convert.ToInt32(result["pworkCount"].ToString());
                        실적Item.perrCount = Convert.ToInt32(result["perrCount"].ToString());
                        실적Item.sDatetime = result["sDatetime"].ToString();
                        실적Item.eDatetime = result["eDatetime"].ToString();
                        실적Item.workingTimeValue = Convert.ToInt32(result["workingTime"].ToString());
                        실적Item.workingTime = String.Format("{0}분", result["workingTime"].ToString());
                        실적Item.machineName = result["mercName"].ToString();
                        실적Item.errMemo = result["workerName"].ToString();
                        실적Item.pMemo = result["pMemo"].ToString();
                        Double pCnt = 1.0;
                        Double.TryParse(result["pCnt"].ToString(), out pCnt);
                        실적Item.pCnt = pCnt;
                        실적Item.workM2 = Convert.ToDouble(result["workM2"].ToString());

                        실적List.Add(실적Item);
                    }
                    p1.Text = workList.Sum(x => x.pplanCount).ToString("N0"); //계획량 (매수)
                    p2.Text = workList.Sum(x => x.pplanCount / x.pCnt).ToString("N0"); //계획통 (통수)

                    if (실적List.Count() >= 0)
                    {
                        p3.Text = 실적List.Sum((x) => x.pworkCount).ToString("N0"); //생산량 (매수)
                        p4.Text = 실적List.Sum((x) => x.pworkCount / x.pCnt).ToString("N0"); //생산통 (통수)
                        p5.Text = 실적List.Sum((x) => x.workM2).ToString("N0"); //생산면적
                        p6.Text = 실적List.Sum((x) => x.perrCount).ToString("N0"); //불량수 (매수)
                        p7.Text = 실적List.Sum((x) => x.perrCount / x.pCnt).ToString("N0"); //불량통 (통수)
                        p8.Text = 실적List.Sum((x) => x.workingTimeValue).ToString("D0") + "분"; //생산시간 

                        if (실적List.Sum((x) => x.pworkCount) == 0) //진행율
                        {
                            p9.Text = "0%";
                        }
                        else
                        {
                            Double 진척률 = (실적List.Sum((x) => x.pworkCount) / workList.Sum(x => x.pplanCount)) * 100.0;
                            p9.Text = 진척률.ToString("N2") + "%";
                        }
                    }
                }
            }
            else
            {
                log.Error("실적현황 조회 오류");
            }
        }

        //생산결과 조회
        public void ResultSearchDate()
        {
            production = null;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopResultSearchDate]",
                new string[] { "@pworkDate" },
                //new string[] { DateTime.Now.ToString("yyyy-MM-dd") }, //날짜 고정
                new string[] { dt.ToString("yyyy-MM-dd") },  //날짜 변경용
            0
        );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    ObservableCollection<MainModel> tempOb = new ObservableCollection<MainModel>();
                    foreach (MainModel workItem in workList)
                    {
                        foreach (JObject result in Data_Jarry)
                        {
                            _ = new InputModel();
                            if (workItem.pplanKey.ToString().Equals(result["pplanKey"].ToString()))
                            {
                                if (result["mercName"].ToString().Equals(Properties.Settings.Default.mercName))
                                {
                                    workItem.presultCount = float.Parse(result["pworkCount"].ToString());   //생산결과 수량
                                    workItem.sumPworkCount += float.Parse(result["pworkCount"].ToString()); //생산결과 총 수량
                                    workItem.sDatetime = result["sDatetime"].ToString();

                                    if (!result["perrorCount"].ToString().Equals(""))
                                    {
                                        workItem.perrorCount += int.Parse(result["perrorCount"].ToString());
                                    }

                                    workItem.imagePath = "/img/bn_01.png"; //작업중 버튼

                                    //완료시간 있으면
                                    if (!result["eDatetime"].ToString().Equals(""))
                                    {
                                        workItem.eDatetime = result["eDatetime"].ToString();
                                    }
                                    else//완료시간 없으면
                                    {
                                        //생산중이면
                                        if (production != null)
                                        {
                                            if (!production.pplanKey.ToString().Equals(workItem.pplanKey.ToString()))
                                            {
                                                production = workItem;
                                            }
                                            else
                                            {
                                                float tempSum = production.sumPworkCount;
                                                float tempPresult = production.presultCount;
                                                production = workItem;
                                                production.sumPworkCount = tempSum;
                                                production.presultCount = tempPresult;
                                            }
                                        }
                                        else
                                        {
                                            production = workItem;

                                            // DB Load
                                            if (rawData)
                                            {
                                                DataTable temp = new LocalDBManager().GetRawData("Last");
                                                
                                                // 설비명 동일 && 제품 orderKey 동일 && 메세지 타입은 N
                                                if(temp.Rows[0]["MercName"].ToString().Equals(Properties.Settings.Default.mercName)
                                                    && temp.Rows[0]["OrderKey"].ToString().Equals(production.orderKey)
                                                    && temp.Rows[0]["MsgType"].ToString().Equals("N"))
                                                {
                                                    production.sumPworkCount = Convert.ToInt32(temp.Rows[0]["Msg"]) + production.sumPworkCount;
                                                    production.presultCount = Convert.ToInt32(temp.Rows[0]["Msg"]);
                                                    // 계속해서 아두이노로 들어오는 카운터 값에 + 방법 연구 필요
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    tempOb.Add(workItem);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < workList.Count; i++)
                    {
                        if (Properties.Settings.Default.boxCheck)
                        {
                            workList[i].sumPworkCount = workList[i].sumPworkCount / workList[i].pCnt;
                        }

                        if (Properties.Settings.Default.completeValue_checked)
                        {

                            if (workList[i].eDatetime.Length > 5)
                            {
                                workList.Remove(workList[i]);
                            }
                        }
                    }

                    //동시작업 설정 체크시
                    if (!Properties.Settings.Default.samework)
                    {
                        foreach (MainModel item in tempOb)
                        {
                            if (!Properties.Settings.Default.mercName.Equals(item.mercName))
                            {
                                if (!item.imagePath.Equals("/img/bn_01.png"))
                                {
                                    _ = workList.Remove(item);
                                }
                            }
                        }
                    }
                    txtNoPlan.Visibility = workList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    SortWorkList();
                }
            }
            else
            {
                log.Error("생산결과 조회 오류");
            }
            실적현황조회();
            실수요자진행현황조회();
        }


        //실수요자 생산계획 조회
        private void 실수요자진행현황조회()
        {
            string pplanKeyArray = string.Empty;

            foreach (var item in workList.Where((x) => x.bMember).GroupBy((x) => x.pplanKey))
            {
                if (!string.IsNullOrEmpty(pplanKeyArray))
                {
                    pplanKeyArray += ",";
                }
                pplanKeyArray += item.FirstOrDefault().pplanKey;
            }


            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               Properties.Settings.Default.serverIndex,
              "[PopResultSearchMemberOrderArray]",
               new string[] { "@pplanKeyArray" },
               new object[] { pplanKeyArray },
               0
               );

            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    foreach (JObject result in Data_Jarry)
                    {
                        workList.Where((x) => x.pplanKey.ToString() == result["pplanKey"].ToString()).FirstOrDefault().imagePath = "/img/bn_01.png"; //작업중 버튼

                        production = workList.Where((x) => x.pplanKey.ToString() == result["pplanKey"].ToString()).FirstOrDefault();
                    }
                }
            }
            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("실수요자진행현황조회 조회 오류!!");
                _ = box.ShowDialog();
                log.Error("실수요자 진행현황 조회 오류");
            }
        }


        //옵션>생산리스트 상단/하단 정렬
        private void SortedList()
        {
            SortDescription sort = new SortDescription("sDatetime", ListSortDirection.Ascending); //시작시간
            SortDescription sort10 = new SortDescription("customSName", ListSortDirection.Ascending);

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(workList);

            //거래처 ㄱㄴㄷ 정렬
            if (!sortChecked)
            {
                run수주일자.Foreground = Brushes.Black;
                run납품일자.Foreground = Brushes.Black;
                run거래처.Foreground = Brushes.Blue;
                run품명.Foreground = Brushes.Blue;

                collectionView.Filter = null;
                collectionView.SortDescriptions.Clear();
                collectionView.SortDescriptions.Add(sort);
                collectionView.SortDescriptions.Add(sort10);
                collectionView.Refresh();

                if (Properties.Settings.Default.corp_name.Equals("오주포장") || Properties.Settings.Default.corp_name.Equals("대양포장") || Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                {
                    oderLast_Sort = true; //수주일자 정렬
                }
            }
            else//일반 정렬(상/하단)
            {
                SortWorkList();
            }
        }

        //옵션>생산리스트 상단/하단 정렬
        private void SortWorkList()
        {
            if (sortChecked)
            {
                run수주일자.Foreground = Brushes.Black;
                run납품일자.Foreground = Brushes.Black;
                run거래처.Foreground = Brushes.Black;
                run품명.Foreground = Brushes.Black;

                SortDescription sort = new SortDescription("sDatetime", ListSortDirection.Ascending); //시작시간
                SortDescription sort1 = new SortDescription("pplanNo", ListSortDirection.Ascending);  //계획번호
                SortDescription sort2 = new SortDescription("pplanKey", ListSortDirection.Ascending); //계획키
                SortDescription sort3 = new SortDescription("urgency", ListSortDirection.Descending); //긴급
                SortDescription sort4 = new SortDescription("pAccount", ListSortDirection.Ascending); //생산차수
                _ = new SortDescription("WDIGCount", ListSortDirection.Descending); //입고 수량
                SortDescription sort6 = new SortDescription("icback", ListSortDirection.Ascending); //실수요자
                SortDescription sort7 = new SortDescription("orderDate", ListSortDirection.Ascending);//수주일자
                SortDescription sort8 = new SortDescription("pState", ListSortDirection.Descending);  //상태
                SortDescription sort9 = new SortDescription("orderLast", ListSortDirection.Ascending);  //납품일자
                bool sortCheck = Properties.Settings.Default.RadioChecked;

                ICollectionView collectionView = CollectionViewSource.GetDefaultView(workList);

                collectionView.Filter = null;
                collectionView.SortDescriptions.Clear();

                //하단정렬
                if (sortCheck)
                {
                    sort = new SortDescription("eDatetime", ListSortDirection.Ascending);
                    collectionView.SortDescriptions.Add(sort);
                    sort = new SortDescription("sDatetime", ListSortDirection.Ascending);
                    collectionView.SortDescriptions.Add(sort);
                    collectionView.SortDescriptions.Add(sort8);
                    //오주,대양
                    if (Properties.Settings.Default.corp_name.Equals("오주포장") || Properties.Settings.Default.corp_name.Equals("대양포장") || Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                    {
                        //367124
                        collectionView.SortDescriptions.Add(sort3);
                        collectionView.SortDescriptions.Add(sort6);
                        collectionView.SortDescriptions.Add(sort1);
                        collectionView.SortDescriptions.Add(sort7);
                        collectionView.SortDescriptions.Add(sort2);
                        collectionView.SortDescriptions.Add(sort4);
                    }
                    else//그외 업체
                    {
                        collectionView.SortDescriptions.Add(sort3);
                        collectionView.SortDescriptions.Add(sort1);
                        collectionView.SortDescriptions.Add(sort2);
                        collectionView.SortDescriptions.Add(sort4);
                    }
                    collectionView.Refresh();
                }
                else//상단정렬
                {
                    collectionView.SortDescriptions.Add(sort);  //시작시간
                    collectionView.SortDescriptions.Add(sort8); //상태
                    if (Properties.Settings.Default.corp_name.Equals("오주포장") || Properties.Settings.Default.corp_name.Equals("대양포장") || Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                    {
                        collectionView.SortDescriptions.Add(sort3); //긴급
                        collectionView.SortDescriptions.Add(sort6); //실수요자(파랑바탕)
                        collectionView.SortDescriptions.Add(sort1); //계획번호
                        collectionView.SortDescriptions.Add(sort7); //수주일자
                        collectionView.SortDescriptions.Add(sort2); //계획키
                        collectionView.SortDescriptions.Add(sort4); //생산차수
                    }
                    //그외 업체
                    else
                    {
                        collectionView.SortDescriptions.Add(sort3); //긴급
                        collectionView.SortDescriptions.Add(sort1); //계획번호
                        collectionView.SortDescriptions.Add(sort2); //계획키
                        collectionView.SortDescriptions.Add(sort4); //생산차수
                    }
                    collectionView.Refresh();
                }

                List<MainModel> lists = collectionView.Cast<MainModel>().ToList();

                for (int i = 0; i < lists.Count; i++)
                {
                    workList1[i] = lists[i];
                }
            }
            else
            {
                SortedList();
            }
            int count = 0;
            for (int i = 0; i < workList.Count; i++)
            {
                if (workList[i].icback.Equals("#AFE1FF"))
                {

                    count = count + 1;
                }
            }
            if (Properties.Settings.Default.corp_name.Equals("오주포장"))
            {
                tbNo.FontSize = 15;
                tbNo.Text = workList.Count.ToString() + "/\n" + count;
            }
            else
            {
                tbNo.Text = workList.Count.ToString();
            }
        }

        

        // 프로그램 종료 버튼 클릭
        private void Program_exit(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.consumersumcount = 0;
            Properties.Settings.Default.Save();

            //관리자 계정이 아닐때만 현황판으로 꺼짐 신호 보냄 
            if (!Properties.Settings.Default.manager.Equals("1"))
            {
                InvokeMercName(Properties.Settings.Default.mercName, "B"); //설비명, 꺼짐신호
            }

            //인증코드 저장(로그인 정보)
            try
            {
                string filePath = @"C:\ICD\settings.json";

                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.login_id) && !string.IsNullOrWhiteSpace(Properties.Settings.Default.login_password))
                {
                    //인증코드 파일경로가 존재하는지 확인 후 경로 생성
                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    //파일 내용
                    var settings = new Dictionary<string, string>
                    {
                        {"login_id", Properties.Settings.Default.login_id},
                        {"login_password", Properties.Settings.Default.login_password},
                        {"corp_name", Properties.Settings.Default.corp_name},
                        {"corp_id", Properties.Settings.Default.corp_id},
                        {"login_check", Properties.Settings.Default.login_check.ToString()},
                        {"mercName", Properties.Settings.Default.mercName},
                        {"SerialName", Properties.Settings.Default.SerialName},
                        {"processType", Properties.Settings.Default.processType.ToString()},
                        {"hour", Properties.Settings.Default.hour.ToString()},
                        {"minute", Properties.Settings.Default.minute.ToString()},
                        {"url", Properties.Settings.Default.url},
                        {"saveDate", Properties.Settings.Default.saveDate},
                        {"pplanKey", Properties.Settings.Default.pplanKey.ToString()},
                        {"count", Properties.Settings.Default.count.ToString()},
                        {"UserID", Properties.Settings.Default.UserID},
                        {"UserName", Properties.Settings.Default.UserName},
                        {"signalR", Properties.Settings.Default.signalR},
                        {"serverIndex", Properties.Settings.Default.serverIndex},
                        {"pworkkey", Properties.Settings.Default.pworkkey},
                        {"consumersumcount", Properties.Settings.Default.consumersumcount.ToString()},
                        {"consumpWorkKey", Properties.Settings.Default.consumpWorkKey.ToString()},
                        {"consumersumerrorcount", Properties.Settings.Default.consumersumerrorcount.ToString()},
                        {"TypeIndex", Properties.Settings.Default.TypeIndex.ToString()},
                        {"CountType", Properties.Settings.Default.CountType.ToString()},
                        {"settingitem", Properties.Settings.Default.settingitem},
                        {"meterialinfo", Properties.Settings.Default.meterialinfo},
                        {"setting_check", Properties.Settings.Default.setting_check.ToString()},
                        {"baudrate", Properties.Settings.Default.baudrate.ToString()},
                        {"SensorRate", Properties.Settings.Default.SensorRate},
                        {"RadioChecked", Properties.Settings.Default.RadioChecked.ToString()},
                        {"printItem", Properties.Settings.Default.printItem},
                        {"samework", Properties.Settings.Default.samework.ToString()},
                        {"color_check", Properties.Settings.Default.color_check.ToString()},
                        {"count_check", Properties.Settings.Default.count_check.ToString()},
                        {"boxCheck", Properties.Settings.Default.boxCheck.ToString()},
                        {"searchCheck", Properties.Settings.Default.searchCheck.ToString()},
                        {"keyinCheck", Properties.Settings.Default.keyinCheck.ToString()},
                        {"detail_check", Properties.Settings.Default.detail_check.ToString()},
                        {"option1", Properties.Settings.Default.option1.ToString()},
                        {"presultcount", Properties.Settings.Default.presultcount.ToString()},
                        {"twoSum", Properties.Settings.Default.twoSum.ToString()},
                        {"accountCheck", Properties.Settings.Default.accountCheck.ToString()},
                        {"productionCheck", Properties.Settings.Default.productionCheck.ToString()},
                        {"scrollvarCheck", Properties.Settings.Default.scrollvarCheck.ToString()},
                        {"index", Properties.Settings.Default.index.ToString()},
                        {"error", Properties.Settings.Default.error},
                        {"error1", Properties.Settings.Default.error1},
                        {"Plc", Properties.Settings.Default.Plc.ToString()},
                        {"ChangeCheck", Properties.Settings.Default.ChangeCheck.ToString()},
                        {"ip", Properties.Settings.Default.ip},
                        {"port", Properties.Settings.Default.port},
                        {"Gap", Properties.Settings.Default.Gap.ToString()},
                        {"mimi", Properties.Settings.Default.mimi.ToString()},
                        {"jang", Properties.Settings.Default.jang.ToString()},
                        {"pok", Properties.Settings.Default.pok.ToString()},
                        {"go", Properties.Settings.Default.go.ToString()},
                        {"setCount", Properties.Settings.Default.setCount.ToString()},
                        {"boxType", Properties.Settings.Default.boxType.ToString()},
                        {"plcCheck", Properties.Settings.Default.plcCheck.ToString()},
                        {"completeValue_checked", Properties.Settings.Default.completeValue_checked.ToString()},
                        {"checkMerc", Properties.Settings.Default.checkMerc.ToString()},
                        {"robotchecked", Properties.Settings.Default.robotchecked.ToString()},
                        {"robotIP", Properties.Settings.Default.robotIP},
                        {"robotPort", Properties.Settings.Default.robotPort},
                        {"manager", Properties.Settings.Default.manager},
                        {"authKey", Properties.Settings.Default.authKey},
                        {"SerialName1", Properties.Settings.Default.SerialName1},
                        {"consumCount", Properties.Settings.Default.consumCount.ToString()}
                    };

                    //인증코드 파일 작성
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

                    //인증코드 파일이 있는지 확인 후 해당 파일을 숨김 처리 (숨김 이후 수정되지 않음)
                    File.WriteAllText(filePath, json);
                    log.Info("인증코드 파일 저장 성공");
                }
            }
            catch (Exception ex)
            {
                log.Error("인증코드 파일 저장 실패");
            }
            //프로그램 종료
            log.Info("프로그램 종료");
            Environment.Exit(0);
        }

        // 상태 버튼 클릭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool check = true;
                if (sender.GetType().Name.Equals("Image"))
                {
                    can.Visibility = Visibility.Visible;
                    Image img = (Image)sender;
                    MainModel model = (MainModel)img.DataContext;

                    //실수요자 일때
                    if (model.bMember)
                    {
                        //대기 버튼 진입
                        if (model.imagePath.Equals("/img/bn_02.png"))
                        {
                            check = true; //이월 버튼 활성화
                            model.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            //관리자 로그인 아닐때 키 생성
                            if (!Properties.Settings.Default.manager.Equals("1"))
                            {
                                //아두이노 연결 안되어있으면 무시
                                if (!Properties.Settings.Default.SerialName.Equals(""))
                                {
                                    arduSerial.Write("A");//아두이노 카운터 리셋
                                }

                                //실수요자 창 팝업
                                PopResultStart(model);
                            }
                        }
                        //작업중 버튼 진입
                        else if (model.imagePath.Equals("/img/bn_01.png"))
                        {
                            check = false; //이월 버튼 비활성화
                        }
                        //완료 버튼 진입
                        else if (model.imagePath.Equals("/img/bn_03.png"))
                        {
                            check = true; //이월 버튼 활성화
                            arduSerial.Write("A");//아두이노 카운터 리셋
                        }

                        e.Handled = false;
                        DSConsumer dSConsumer = new DSConsumer(model, check);
                        dSConsumer.ShowDialog(); //실수요자 리스트 창 팝업

                        //창 종료 후 
                        InvokeCount(Properties.Settings.Default.mercName, "E"); //현황판으로 생산종료 송신
                        SelectWorkList();   //생산계획 조회
                        ResultSearchDate(); //생산결과 조회
                    }
                    //일반생산
                    else
                    {
                        win_Input = new Win_input(model, workList, false);
                        _ = win_Input.ShowDialog(); //인풋창 팝업

                        //팝업창 종료 후

                        //
                        if (Properties.Settings.Default.presultcount != 0)
                        {
                            model.presultCount = Properties.Settings.Default.presultcount;
                        }

                        //대세, 구룡일때
                        if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }

                        Properties.Settings.Default.presultcount = 0;
                        Properties.Settings.Default.Save();
                    }

                    //유창
                    if (Properties.Settings.Default.corp_name.Equals("유창포장") || Properties.Settings.Default.corp_name.Equals("유창포장테스트"))
                    {
                        //계획건수 20개 미만일때
                        if (workList.Count < 20)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                    }
                    win_Input = null;

                    if (production != null)
                    {
                        arduSerial.Write("B");
                    }

                    e.Handled = false;
                    can.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception e1)
            {
                e.Handled = false;
                can.Visibility = Visibility.Collapsed;
            }
        }

        //실수요자 생산 시작
        private int PopResultStart(MainModel model)
        {

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopResultStart]",
                new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey" },
                new object[] {
                DateTime.Now.ToString("yyyy-MM-dd"),
                model.orderKey,
                model.pplanKey,
                model.ProcessType,
                Properties.Settings.Default.mercName,
                MainWindow.userID,
                MainWindow.userName,
                "",
                "",
                ""},
                1

                );

            if (returnData.결과 == 1)
            {
                // mainModel.pworkKey = int.Parse(returnData.리턴값);
                if (model.imagePath.Equals("/img/bn_03.png")) //완료버튼
                {
                    model.ConsumWorkKey = returnData.리턴값;
                }
                Properties.Settings.Default.pworkkey = returnData.리턴값;
                Properties.Settings.Default.Save();
                //Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                model.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                InvokeCount(Properties.Settings.Default.mercName, "S");
                Properties.Settings.Default.count = 0;
                Properties.Settings.Default.saveDate = "";
                Properties.Settings.Default.pplanKey = 0;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;
        }

        private void InvokeCount(string mercName, string v) //설비명,카운터
        {
            try
            {
                if (Properties.Settings.Default.corp_name.Equals("광성수출포장"))
                {
                    popService(mercName, v);
                }
                else
                {
                    SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v); //invokeCount, DB명, 설비명, 문자
                }
            }
            catch (Exception e)
            {
            }
        }

        //계획리스트 클릭
        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //듀얼상세화면 체크시
            if (Properties.Settings.Default.detail_check)
            {
                //더블클릭시 디테일 창 호출
                if (e.ClickCount == 2)
                {
                    ShowDetail(sender);
                }
            }
            else 
            {
                //더블클릭시 디테일 창 호출
                if (e.ClickCount == 2)
                {
                    ShowDetail(sender);
                }
            }
        }

        //디테일 창 팝업
        private void ShowDetail(object sender)
        {
            try
            {
                if (!sender.GetType().Name.Equals("Image"))
                {
                    Canvas canvas = (Canvas)sender;
                    MainModel context = (MainModel)canvas.DataContext;
                    can.Visibility = Visibility.Visible;

                    string special = "";
                    for (int i = 0; i < workList.Count; i++)
                    {
                        if (context.orderKey.Equals(workList[i].orderKey.ToString()))
                        {
                            special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                        }
                    }

                    try
                    {
                        CNS_Barcode_Send = false;
                        Win_Detail win_detail = new Win_Detail(context, special, workList, Properties.Settings.Default.detail_check, false);
                        _ = win_detail.ShowDialog();

                        if (Properties.Settings.Default.corp_name.Equals("유창포장"))
                        {
                            if (workList.Count < 30)
                            {
                                SelectWorkList();
                                ResultSearchDate();
                            }
                        }

                        win_Input = null;
                        win_detail = null;
                    }
                    catch (Exception e1)
                    {
                        log.Error("디테일 창 팝업 실패 : " + e1.Message);
                    }

                    try
                    {
                        if (production != null)
                        {
                            arduSerial.Write("B");
                        }
                    }
                    catch (Exception)
                    {

                    }

                    can.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception e)
            {
                log.Error("디테일 창 팝업 실패 : " + e.Message);
            }
        }

        int Barcode_Scan = 0;
        //바코드 읽기
        private void TbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            Barcode_Scan++;

            MainModel tempItem = null;

            CNS_Barcode_Send = false;
            string code = tbBarcode.Text; //바코드 값 저장 
            _ = int.TryParse(code, out int pCode); //바코드 값  정수로 변환


            //바코드 정보 있으면
            if (pCode > 0)
            {
                //workList.orderKey와 pCode값과 같은지 검사
                if (workList.Count(x => x.orderKey == pCode.ToString()) > 0)
                {
                    tempItem = workList.FirstOrDefault(x => x.orderKey == pCode.ToString());
                }
            }

            //일치하는 계획이 없으면 실행
            if (Barcode_Scan == 11 && tempItem == null)
            {
                Barcode_Scan = 0;

                MessageBox_Dialog box = new MessageBox_Dialog("일치하는 생산 계획이 없습니다.");
                _ = box.ShowDialog();
            }

            //일치하는 계획건 찾으면 실행
            if (tempItem != null)
            {
                Barcode_Scan = 0;

                tbBarcode.Text = "";
                can.Visibility = Visibility.Visible;
                string special = "";
                for (int i = 0; i < workList.Count; i++)
                {
                    if (tempItem.orderKey.Equals(workList[i].orderKey.ToString()))
                    {
                        special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                    }
                }

                try
                {
                    //PLC 사용시 
                    CNS_Barcode_Send = Properties.Settings.Default.Plc ? true : false;

                    Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, Properties.Settings.Default.detail_check, CNS_Barcode_Send);
                    _ = win_Detail.ShowDialog();
                    win_Input = null;
                    win_Detail = null;
                }
                catch (Exception e1)
                {
                    log.Error("디테일 창 팝업 실패 : " + e1.Message);
                    log.Error("디테일 창 팝업 실패 : " + e1.StackTrace);
                }

                //생산중이면
                if (production != null)
                {
                    try
                    {
                        arduSerial.Write("B"); //아두이노 현재카운터 요청
                    }
                    catch (Exception ex)
                    {
                        string exception = ex.Message;
                    }

                }
                can.Visibility = Visibility.Collapsed;
            }

            e.Handled = false;
        }

        private void BarcodeCheck()
        {
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

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        private void btnRawData_click(object sender, RoutedEventArgs e)
        {
            if (rawData)
            {
                rawData = false;
                tbRawData.Text = "N";
            }
            else
            {
                rawData = true;
                tbRawData.Text = "Y";
            }
        }

        private void Logout_click(object sender, RoutedEventArgs e)
        {
            setLock(); //설비명 LOCK /UNLOCK 버튼

            //Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Save();
            //Application.Current.Shutdown();
            //_ = Process.Start(Application.ResourceAssembly.Location);
        }

        //설비명 LOCK /UNLOCK
        private void setLock()
        {
            if (Properties.Settings.Default.checkMerc)
            {
                btnLogout.Background = Brushes.Green;
                merc_lock.Text = "UNLOCK";
                Properties.Settings.Default.checkMerc = false;
            }
            else
            {
                merc_lock.Text = "LOCK";
                btnLogout.Background = Brushes.Red;
                Properties.Settings.Default.checkMerc = true;
            }
            Properties.Settings.Default.Save();
            log.Info("설비 => " + merc_lock.Text);
        }

        //설정 버튼
        private void Setting(object sender, MouseButtonEventArgs e)
        {
            log.Info("설정창 열기");
            can.Visibility = Visibility.Visible;
            Win_Setting win_Setting = new Win_Setting();
            _ = win_Setting.ShowDialog();
            string metrialitem = Properties.Settings.Default.meterialinfo;

            SetSetting(metrialitem);
            arduSerial.DataReceived -= ArduReReceive;

            ArduConn();
            // ArduConn1();

            //스크롤바 사용 유/무
            sv.VerticalScrollBarVisibility = Properties.Settings.Default.scrollvarCheck ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            if (Properties.Settings.Default.scrollvarCheck)
            {
                sv.PreviewMouseLeftButtonDown -= ScrollViewer_PreviewMouseLeftButtonDown;
                sv.PreviewMouseLeftButtonUp -= Sv_PreviewMouseLeftButtonUp;
                sv.PreviewMouseMove -= Sv_PreviewMouseMove;
            }
            else
            {
                sv.PreviewMouseLeftButtonDown += ScrollViewer_PreviewMouseLeftButtonDown;
                sv.PreviewMouseLeftButtonUp += Sv_PreviewMouseLeftButtonUp;
                sv.PreviewMouseMove += Sv_PreviewMouseMove;
            }

            if (Properties.Settings.Default.searchCheck)
            {
                SelectWorkList(); // 생산 계획 리스트 조회
                ResultSearchDate();
            }

            Properties.Settings.Default.searchCheck = false;
            Properties.Settings.Default.Save();
            can.Visibility = Visibility.Collapsed;
        }

        private string ExtractKorean(string beforestr)
        {
            //beforestr = "#ffffff";
            string str = Regex.Replace(beforestr, "#(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})", "");
            return str;
        }

        private void SetSetting(string metrialitem)
        {
            switch (metrialitem)
            {
                case "19":
                case "20":
                    arduSerial.Close();
                    Content = null;
                    timer.Stop();

                    MainWindow.main = null;
                    if (MainWindow.main1 == null)
                    {
                        MainWindow.main1 = new Main1();
                    }
                    Content = MainWindow.main1;
                    break;

                default:

                    //info.Text = "원단정보";
                    //  Canvas.SetLeft(info, 1000);    
                    break;
            }
        }

        private void Change_workList(object sender, MouseButtonEventArgs e)
        {
            ChangeWorkList ch = new ChangeWorkList(workList1);
            _ = ch.ShowDialog();

            foreach (MainModel item in workList)
            {
                item.check = false;
                item.icback = "White";
            }

            if (!Properties.Settings.Default.ChangeCheck)
            {
                SelectWorkList(); // 생산 계획 리스트 조회
                ResultSearchDate();
            }

            Properties.Settings.Default.ChangeCheck = false;
            Properties.Settings.Default.index = 0;
            Properties.Settings.Default.Save();
        }

        private void TxtTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Time_pick.Time_pick time_Pick = new Time_pick.Time_pick();
            _ = time_Pick.ShowDialog();
        }

        private void Main_keydownEvent(object sender, KeyEventArgs e)
        {
            // Console.WriteLine(e.Key.ToString());
            switch (e.Key.ToString())
            {
                case "F1":
                    SearchList search = new SearchList(workList);
                    _ = search.ShowDialog();
                    break;
                case "F2":
                    Ip_input ip_Input = new Ip_input();
                    ip_Input.ShowDialog();
                    break;
                case "F8":
                    if (Properties.Settings.Default.manager.Equals("0"))
                    {
                        MessageBox_Dialog msg = new MessageBox_Dialog("관리자 계정이 아닙니다.");
                        msg.ShowDialog();
                    }
                    else
                    {
                        UserSetting user = new UserSetting();
                        user.ShowDialog();

                    }
                    break;
                case "F12":
                    //Properties.Settings.Default.Reset();
                    //Properties.Settings.Default.Save();
                    //Application.Current.Shutdown();
                    //_ = Process.Start(Application.ResourceAssembly.Location);

                    break;
                default:
                    break;
            }
        }

        private void Search_(object sender, MouseButtonEventArgs e)
        {
            SearchList search = new SearchList(workList);
            _ = search.ShowDialog();

            ResultSearchDate();
        }

        private void Refresh(object sender, MouseButtonEventArgs e)
        {
            SelectWorkList();
            ResultSearchDate();
        }

        //설정>업로드
        private void upload_setting()
        {
            string macAddress = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[Pop_machinfo_update]",
                new string[] { "@설비명", "@작업정보", "@상자수", "@동시작업", "@이합설비", "@상자수계산", "@맥주소" },
                new object[] {
                    Properties.Settings.Default.mercName,
                    Properties.Settings.Default.settingitem,
                    Properties.Settings.Default.boxCheck,
                    Properties.Settings.Default.samework,
                    Properties.Settings.Default.twoSum,
                    Properties.Settings.Default.option1,
                    macAddress
                },
                1
                );
            if (returnData.결과 == 1)
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("업로드 성공");
                msg.ShowDialog();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장 실패");
                msg.ShowDialog();
            }
        }

        private void Pop_noty(object sender, MouseButtonEventArgs e)
        {
            Pop_noty pop_Noty = new Pop_noty();
            pop_Noty.ShowDialog();
        }

        //정렬
        private void sort(object sender, MouseButtonEventArgs e)
        {
            //납품일자 기준 정렬
            if (oderLast_Sort)
            {
                run수주일자.Foreground = Brushes.Black;
                run납품일자.Foreground = Brushes.Blue;
                run거래처.Foreground = Brushes.Black;
                run품명.Foreground = Brushes.Black;

                SortDescription sort = new SortDescription("sDatetime", ListSortDirection.Ascending); //시작시간
                SortDescription sort1 = new SortDescription("pplanNo", ListSortDirection.Ascending);  //계획번호
                SortDescription sort2 = new SortDescription("pplanKey", ListSortDirection.Ascending); //계획키
                SortDescription sort3 = new SortDescription("urgency", ListSortDirection.Descending); //긴급
                SortDescription sort4 = new SortDescription("pAccount", ListSortDirection.Ascending); //생산차수
                _ = new SortDescription("WDIGCount", ListSortDirection.Descending); //입고 수량
                SortDescription sort6 = new SortDescription("icback", ListSortDirection.Ascending); //실수요자
                SortDescription sort7 = new SortDescription("orderDate", ListSortDirection.Ascending);//수주일자
                SortDescription sort8 = new SortDescription("pState", ListSortDirection.Descending);  //상태
                SortDescription sort9 = new SortDescription("orderLast", ListSortDirection.Ascending);  //납품일자
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(workList);

                collectionView.Filter = null;
                collectionView.SortDescriptions.Clear();
                collectionView.SortDescriptions.Add(sort);  //시작시간
                collectionView.SortDescriptions.Add(sort9); //납품일자
                collectionView.SortDescriptions.Add(sort8); //상태
                collectionView.SortDescriptions.Add(sort3); //긴급
                collectionView.SortDescriptions.Add(sort6); //실수요자(파랑바탕)
                collectionView.SortDescriptions.Add(sort1); //계획번호
                collectionView.SortDescriptions.Add(sort7); //수주일자
                collectionView.SortDescriptions.Add(sort2); //계획키
                collectionView.SortDescriptions.Add(sort4); //생산차수
                collectionView.Refresh();
                oderLast_Sort = false; //수주일자 정렬
            }
            else 
            {
                //거래처 ㄱㄴㄷ 정렬 (클릭시 이벤트)
                if (sortChecked)
                {
                    sortChecked = false;
                }
                //일반 정렬
                else
                {
                    sortChecked = true;
                }

                SortedList();
            }
        }

        //날짜 뒤로 이동
        private void Date_Click1(object sender, MouseButtonEventArgs e)
        {
            dt = dt.AddDays(-1);
            txtDate.Text = dt.ToString("D");
            if (dt.ToString("D").Equals(DateTime.Now.ToString("D")))
            {
                CheckDateValue = true;
            }
            else
            {
                CheckDateValue = false;
            }

            SelectWorkList(); //계획 조회
            ResultSearchDate(); //결과 조회
        }

        //날짜 앞으로 이동
        private void Date_Click2(object sender, MouseButtonEventArgs e)
        {
            dt = dt.AddDays(1);
            txtDate.Text = dt.ToString("D");
            if (dt.ToString("D").Equals(DateTime.Now.ToString("D")))
            {
                CheckDateValue = true;
            }
            else
            {
                CheckDateValue = false;
            }

            SelectWorkList(); //계획 조회
            ResultSearchDate(); //결과 조회
        }

        //설비점검 클릭
        private void Machine_Check(object sender, MouseButtonEventArgs e)
        {
            MachineCheck machineCheck = new MachineCheck();
            //machineCheck.Owner = this;
            machineCheck.Show();
        }

        //상태버튼 클릭시 SignalR 재연결
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("SignalR 서버에 재연결 합니다.");
                dialog.ShowDialog();
                log.Info("SignalR 재연결 요청");
                
                SignalRAdministrator conn = new SignalRAdministrator();
                conn.Setconnection(false);
                conn.Setconnection(true);
            }
            catch (Exception ex)
            {
                log.Error("SignalR 재연결 요청 실패: " + ex.Message);
            }
        }

        //SIgnalR 재연결 
        public void TimerSetting() 
        {
            try
            {
                DispatcherTimer Timer = new DispatcherTimer();
                Timer.Interval = new TimeSpan(0, 30, 0);
                Timer.Tick += Timer_Ticks;
                Timer.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void Timer_Ticks(object sender, EventArgs e)
        {
        }
    }



    public class StrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string str = "";

            if (value != null)
            {
                if (value.Equals("1"))
                {
                    str = "긴급";
                }
            }

            return str;

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







