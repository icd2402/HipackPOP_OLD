using Newtonsoft.Json.Linq;
using PopProject.Consumer_;
using PopProject.Detail;
using PopProject.input;
using PopProject.Input;
using PopProject.MerList;
using PopProject.MessageBox;
using PopProject.Util;
using System;
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
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Data;

namespace PopProject.UserControl_
{
    /// <summary>
    /// Main.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Main1 : UserControl
    {
        // int pageCheck = 0;
        int wdIGCOunt = 0;
        private DispatcherTimer timer;
        public bool backgroundCheck = true;
        public static SerialPort arduSerial = new SerialPort();
        public static Win_input win_Input;
        public ObservableCollection<MainModel> workList = null;
        public ObservableCollection<MainModel> workList1 = null;
        public static Robot_Setting robot_Setting = null;
        public static Robot_Setting2 robot_Setting2 = null;
        // 새로운 BackgroundWorker 생성
        private readonly BackgroundWorker bw = new BackgroundWorker();
        // 중간 취소를 지원할 경우 설정값

        public static Consumer consumer_;
        public static DSConsumer dSConsumer;
        public static MainModel production = null;
        private int minuite = 0;
        public static int str;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        private bool CNS_Barcode_Send = false; //true이면 바코드 읽으면 CNS로 데이터 전송
        private SearchList searchListWindow;
        bool tempLock = false;
        bool settingPopupCheck = false;
        private Canvas previousClickedCanvas = null;
        private bool isTextVisible = false; //날짜 변경버튼 
        private bool oderLast_Sort = false;
        private bool sortChecked = true;
        private string plusDate = "";

        public Main1()
        {
            InitializeComponent();

            production = null;
            string metrialitem = SettingJsonManager.meterialinfo;
            SetSetting(metrialitem);
            string merName = SettingJsonManager.mercName;
            tbMercName.Text = merName;


            // 설비명 변경 Lock / Unlock 설정
            if (!SettingJsonManager.checkMerc)
            {
                unlock_img.Visibility = Visibility.Visible;
                lock_img.Visibility = Visibility.Collapsed;
                //btnLogout.Background = Brushes.Green;
                merc_lock.Text = "UNLOCK";
            }
            else
            {
                unlock_img.Visibility = Visibility.Collapsed;
                lock_img.Visibility = Visibility.Visible;
                //btnLogout.Background = Brushes.Red;
                merc_lock.Text = "LOCK";
            }

            try
            {
                SignalRAdministrator conn = new SignalRAdministrator();

                conn.Setconnection(true);
            }
            catch (Exception)
            {

            }
            SettingJsonManager.consumersumcount = 0;
            SettingJsonManager.SaveJsonFile();

            corp_name.Text = SettingJsonManager.corp_name;
            ArduConn();
            sv.VerticalScrollBarVisibility = SettingJsonManager.scrollvarCheck ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;

            if (SettingJsonManager.scrollvarCheck)
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
            //setTbPintNo();
            //인터넷 연결 확인
            bool connected = NetworkInterface.GetIsNetworkAvailable();

            if (connected)
            {
                // 최초 생산계획 및 생산결과 조회
                if (win_Input == null || robot_Setting == null)
                {
                    SelectWorkList(); //생산 계획 리스트 조회
                }
                ResultSearchDate(); //생산 결과 리스트 조회

                Timer_1s();         // 1초 - 시계, 바코드, 종료시간 체크
                Timer_60s();        // 60초(1분) - POP 켜짐 신호 전송
                Timer_300s();       // 300초(5분) - 계획/결과 갱신, 아두이노 연결 확인, 카운팅 값 전송

            }
            else
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("인터넷 상태를 확인해 주세요");
                _ = dialog.ShowDialog();
                LogManager.Error("인터넷 연결 실패", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            }

        }
        
        private void Timer_1s()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(Timer_1s_Tick);
            timer.Start();
        }
        private void Timer_1s_Tick(object sender, EventArgs e)
        {

            //공지사항 팝업
            if (settingPopupCheck == false && !SettingJsonManager.noticePopup && tempLock == false)
            {
                Notice_PopUp();
                tempLock = true;
            }

            txtDate.Text = DateTimeManager.GetSelectedDate().ToString("yyyy년 MM월 dd일 dddd");
            txtClock.Text = DateTimeManager.GetSelectedDT().ToString("HH시 mm분 ss초");

            //지정된 시간에 컴퓨터를 종료
            if (DateTime.Now.Hour == SettingJsonManager.hour && DateTime.Now.Minute == SettingJsonManager.minute)
            {
                _ = Process.Start("shutdown.exe", "-s");
            }

            //검색창이 팝업일때만 실행
            if (searchListWindow != null && searchListWindow.IsVisible)
            {
                SearchList_Pointer();
                Console.WriteLine("검색창 팝업됨");
            }

            //대세,광성은 바코드 동작 x
            if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트") || SettingJsonManager.corp_name.Equals("광성수출포장") || SettingJsonManager.corp_name.Equals("광성수출포장테스트"))
            {
            }
            else
            {
                if (Properties.Settings.Default.PopUpPosition_M == false)
                {
                    //바코드
                    _ = tbBarcode.Focus();
                    tbBarcode.SelectAll();
                }
            }

        }

        // 타이머 60초(1분) - POP 켜짐 신호 전송
        private void Timer_60s()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60)
            };
            timer.Tick += new EventHandler(Timer_60s_Tick);
            timer.Start();
        }

        // 현황판에 POP 켜짐 신호 전송
        private void Timer_60s_Tick(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                //관리자 계정이 아닐때만 켜짐 신호 전송
                if (!SettingJsonManager.manager.Equals("1"))
                {
                    CountManager.SendMsg(SettingJsonManager.mercName, "A"); // A - 켜짐
                }
            }
        }

        // 타이머 300초(5분) - 계획/결과 갱신, 아두이노 연결 확인, 카운팅 값 전송
        private void Timer_300s()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(300)
            };
            timer.Tick += new EventHandler(Timer_300s_Tick);
            timer.Start();
        }

        // 계획 및 결과 갱신, 아두이노 연결 확인, 카운팅 값 전송
        private void Timer_300s_Tick(object sender, EventArgs e)
        {
            bool connected = NetworkInterface.GetIsNetworkAvailable(); //네트워크 연결 여부 확인
            if (!arduSerial.IsOpen) //아두이노 오픈 안되어 있으면 다시 오픈 요청
            {
                arduSerial.DataReceived -= ArduReReceive;
                ArduConn();//아두이노 통신연결(5분 타이머)
            }

            if (connected) //네트워크 연결되어 있으면
            {
                //생산중일때
                if (production != null)
                {
                    //실수요자 생산건
                    if (production.bMember)
                    {
                        if (production.presultCount != 0)
                        {
                            //현황판으로 카운터 전송
                            CountManager.SendMsg(SettingJsonManager.mercName, production.presultCount.ToString());
                        }
                    }
                    else //일반 생산건
                    {
                        if (production.sumPworkCount != 0)
                        {
                            //현황판으로 카운터 전송
                            CountManager.SendMsg(SettingJsonManager.mercName, production.sumPworkCount.ToString());
                        }

                    }
                }

                //실수요자 리스트 팝업 안됐을때만 갱신
                if (dSConsumer == null && win_Input == null)
                {
                    SelectWorkList();
                    ResultSearchDate();
                    Console.WriteLine("계획/결과 조회(5분 타이머)");
                }
            }
        }

        //계획,결과 조회
        private void UpdateWorkPlanNResult()
        {
            SelectWorkList();
            ResultSearchDate();
        }

        private void ArduConn()
        {
            try
            {
                //통신 연결되어 있으면
                if (arduSerial.IsOpen)
                {
                    Thread serialClose;
                    serialClose = new System.Threading.Thread(new System.Threading.ThreadStart(serialDisConnect));//닫았다 연결 시도
                    serialClose.IsBackground = true;
                    serialClose.Start();
                }
                //통신 연결 끊겨있으면
                else if (!arduSerial.IsOpen)
                {
                    LogManager.Info("시리얼 끊겼으면 재시도", MethodBase.GetCurrentMethod().DeclaringType);

                    arduSerial.Encoding = Encoding.GetEncoding(65001);
                    arduSerial.PortName = SettingJsonManager.SerialName;
                    arduSerial.BaudRate = SettingJsonManager.baudrate;
                    arduSerial.DataReceived += ArduReReceive;

                    arduSerial.Open();
                    //arduSerial.Write(SettingJsonManager.SensorRate);
                    btrSerial.Background = Brushes.Green;

                    if (arduSerial.IsOpen)
                    {
                        LogManager.Info("시리얼 연결 완료", MethodBase.GetCurrentMethod().DeclaringType);
                    }
                }
            }
            catch (Exception e)
            {
                btrSerial.Background = Brushes.Red;
                LogManager.Error("아두이노 시리얼 연결 오류 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        // 아두이노 시리얼포트 닫기았다 오픈
        private void serialDisConnect()
        {
            try
            {
                LogManager.Info("시리얼 연결 끊기", MethodBase.GetCurrentMethod().DeclaringType);
                arduSerial.DataReceived -= ArduReReceive;
                arduSerial.DtrEnable = false;
                arduSerial.RtsEnable = false;
                arduSerial.ReadExisting(); // 시리얼 읽기, 비우기
                arduSerial.DiscardInBuffer(); //수신 버퍼 삭제
                arduSerial.DiscardOutBuffer(); //송신 버퍼 삭제
                arduSerial.Close(); // 시리얼 포트 닫기
            }
            catch (Exception e)
            {
                LogManager.Error("아두이노 재연결 오류 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
            finally
            {
                arduSerial.Encoding = Encoding.GetEncoding(65001);
                arduSerial.PortName = SettingJsonManager.SerialName;
                arduSerial.BaudRate = SettingJsonManager.baudrate;
                arduSerial.DataReceived += ArduReReceive;

                arduSerial.Open();
                arduSerial.Write(SettingJsonManager.SensorRate);
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    btrSerial.Background = Brushes.Green;
                }));

                if (arduSerial.IsOpen)
                {
                    LogManager.Info("시리얼 재연결 완료", MethodBase.GetCurrentMethod().DeclaringType);
                }
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
                            pCnt = SettingJsonManager.twoSum ? production.pCnt * 2 : production.pCnt;
                        }
                        else
                        {
                            pCnt = 1;
                        }

                        //SaveDate가 비어있지 않고 현재 날짜와 다르면
                        if (!SettingJsonManager.saveDate.Equals("") && !SettingJsonManager.saveDate.Equals(DateTimeManager.GetNowDateStr()))
                        {
                            //계획키가 일치할때
                            if (production.pplanKey == SettingJsonManager.pplanKey)
                            {
                                str = SettingJsonManager.count + str;
                            }
                        }

                        //매수일때
                        if (SettingJsonManager.CountType == 0)
                        {
                            production.sumPworkCount = str;
                            production.presultCount = str;
                            production.presultCount1 = str;

                            //인풋창이 팝업되어있으면
                            if (win_Input != null)
                            {
                                win_Input.inputModel.count = str;
                            }

                            CountManager.SendMsg(SettingJsonManager.mercName, str.ToString()); //현황판으로 카운터 전송
                            //LogManager.Info("출력 값(매수) : " + str.ToString() + SettingJsonManager.boxCheck, MethodBase.GetCurrentMethod().DeclaringType);
                            Console.WriteLine("현황판으로 카운터(매수) 전송 값(Main) : " + SettingJsonManager.mercName + ", " + str); //실시간 값 확인용

                            if (SettingJsonManager.corp_name.Equals("광성수출포장") && SettingJsonManager.dbUse)
                            {
                                //RawData 저장(업체코드,설비명,데이터,오더키,수주키,제품ID)
                                new LocalDBManager().SetRawData(SettingJsonManager.corp_id, SettingJsonManager.mercName, str.ToString(), production.orderKey, production.prodID);
                            }
                        }
                        else //통수일때
                        {
                            //신우,우리,누리,성주 (이합철)
                            if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("성주포장"))
                            {
                                //설정에서 이합철 체크시 통수값 * 2 
                                pCnt = SettingJsonManager.twoSum ? (production.pCnt * 2) : 1;
                            }

                            //아두이노 카운터값 있으면
                            if (str != 0)
                            {
                                //옵션에서 상자 수 체크시 
                                if (SettingJsonManager.boxCheck)
                                {
                                    if (production != null)
                                    {
                                        production.sumPworkCount = Convert.ToInt32(str);
                                    }
                                }
                                else
                                {
                                    production.sumPworkCount = Convert.ToInt32(Math.Floor(str * pCnt)); //카운터값 * 통수
                                }

                                production.presultCount = production.sumPworkCount;
                                production.presultCount1 = production.sumPworkCount;

                                //인풋창 팝업시
                                if (win_Input != null)
                                {
                                    win_Input.inputModel.count = production.presultCount;
                                }

                                CountManager.SendMsg(SettingJsonManager.mercName, production.presultCount.ToString()); //현황판으로 카운터 전송
                                Console.WriteLine("현황판으로 카운터(통수) 전송 값(Main) : " + SettingJsonManager.mercName + ", " + production.presultCount.ToString()); //실시간 값 확인용

                                //LogManager.Info("출력값 (통수) : " + production.presultCount.ToString() + SettingJsonManager.boxCheck, MethodBase.GetCurrentMethod().DeclaringType);

                                //실시간 값 확인용
                                //if (SettingJsonManager.boxCheck)
                                //{
                                //    Console.WriteLine("카운터(통수,상자수 체크) 전송 값(Main) : " + SettingJsonManager.mercName + ", " + Convert.ToInt32(production.presultCount)); 
                                //}
                                //else
                                //{
                                //    Console.WriteLine("카운터(통수,상자수 미체크) 전송 값(Main) : " + SettingJsonManager.mercName + ", " + Convert.ToInt32(production.presultCount));
                                //}

                                if (SettingJsonManager.corp_name.Equals("광성수출포장") && SettingJsonManager.dbUse)
                                {
                                    //RawData 저장(업체코드,설비명,데이터,오더키,수주키,제품ID)
                                    new LocalDBManager().SetRawData(SettingJsonManager.corp_id, SettingJsonManager.mercName, production.presultCount.ToString(), production.orderKey, production.prodID);
                                }
                            }
                        }
                    }));
                }
                else
                {
                    Console.WriteLine("production == null");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("아두이노 => Main 데이터 수신 오류 : " + ex.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private void DoBackgroundwork(object sender, DoWorkEventArgs e)
        {
            SelectWorkList(); // 생산 계획 리스트 조회
        }
        private void ProgressChangedBackgroundwork(object sender, ProgressChangedEventArgs e)
        {
            // TODO : ProgressBar를 사용할 경우 작업 진행 정도를 표시 해줄수 있다.

        }
        private void CompleteBackgroundwork(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundCheck = true;
            // TODO : 작업 완료
            bw.RunWorkerAsync();

        }
        // 초기화 부분에 BackgroundWorker 생성 후 이벤트 핸들러를 바인딩 해줍니다.



        //현황판으로 카운터 전송

       

        //자동 로그인 해제
        private void Serial_click(object sender, RoutedEventArgs e)
        {
        }

        //원격연결 버튼
        private void Remote_click(object sender, MouseButtonEventArgs e)
        {
            _ = Process.Start("http://2007.asw.co.kr/client/index.jsp?cpcode=icd&autostart=true");
        }

        //날짜 클릭
        private void TxtDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DateTimeManager.SetTodayDate(); //당일 날짜로 변경
            UpdateWorkPlanNResult(); // 계획/결과 조회
        }


        //설비명 클릭
        private void Mer_click(object sender, MouseButtonEventArgs e)
        {
            // UNLOCK 일때
            if (SettingJsonManager.checkMerc == false)
            {
                //현재 설비명 before에 담기
                string before = string.IsNullOrWhiteSpace(SettingJsonManager.mercName) ? "" : SettingJsonManager.mercName;
                MercList mercList = new MercList(); //리스트 가져오기
                _ = mercList.ShowDialog(); //설비 리스트 팝업
                tbMercName.Text = SettingJsonManager.mercName;

                if (before.Equals(SettingJsonManager.mercName) == false) //설비명이 변경되면
                {
                    //관리자 계정이 아니고 전 설비가 선택되어 있었던 경우만 실행 (혼선 방지)
                    if (SettingJsonManager.manager.Equals("1") == false && before.Equals("") == false)
                    {
                        CountManager.SendMsg(before, "B"); //변경전 설비명, 꺼짐신호
                        CountManager.SendMsg(SettingJsonManager.mercName, "A"); //변경된 설비명, 켜짐신호
                        SettingJsonManager.error = "";
                        SettingJsonManager.error1 = "";
                        SettingJsonManager.SaveJsonFile();
                    }

                    LogManager.Info("설비 변경됨 => " + SettingJsonManager.mercName, MethodBase.GetCurrentMethod().DeclaringType);
                }
                else //설비명 변경 안했으면
                {
                    //관리자 계정이 아닐때만 현황판으로 켜짐 신호 보냄 (혼선 방지)
                    if (!SettingJsonManager.manager.Equals("1"))
                    {
                        CountManager.SendMsg(SettingJsonManager.mercName, "A"); //기존 설비명, 켜짐신호
                    }
                }

                // 상단 바 계획 글자 옵션
                if (SettingJsonManager.CountType != 0) //통수일때 
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
            // 서버 인덱스와 DBName을 저장할 Dictionary 생성
            Dictionary<string, string> serverIndices = new Dictionary<string, string>();

            if (SettingJsonManager.companyIntegration
                && (SettingJsonManager.corp_name.Equals("하나포장") || SettingJsonManager.corp_name.Equals("하나포장테스트")))
            {
                if (SettingJsonManager.corp_name.Equals("하나포장테스트"))
                {
                    serverIndices.Add("hnbox", "icdtest3"); serverIndices.Add("opbox", "icdtest23");
                }
                else
                {
                    serverIndices.Add("hnbox", "A07E164641");
                    serverIndices.Add("opbox", "8E874414");

                }
            }
            else
            {
                    serverIndices.Add(SettingJsonManager.serverIndex, SettingJsonManager.corp_id);
            }

            // workList와 workList1을 초기화
            workList = new ObservableCollection<MainModel>(); // 메인 화면
            workList1 = new ObservableCollection<MainModel>(); // 순서변경 화면
            wdIGCOunt = 0; //원단입고값 초기화

            // 각 서버 인덱스와 DBName에 대해 데이터 조회 및 처리
            foreach (var server in serverIndices)
            {
                string ServerIndex = server.Key;
                string DBName = server.Value;

                Console.WriteLine("현재 서버 인덱스: " + ServerIndex + ", DBName: " + DBName);

                // 현재 서버에 맞게 서비스 모델 초기화 (DBName 추가)
                ServiceModel.ServiceModelClean(ServerIndex, DBName);

                // 현재 서버에서 데이터 조회
                ReturnModel returnData = ServiceModel.GetData(
                    ServerIndex,
                    "[PopWorkList]",
                    new string[] { "@pplanDate" },
                    new string[] { DateTimeManager.GetSelectedDateStr() }, // 날짜 변경용
                    0
                );


                if (returnData.결과 == 1)
                {
                    JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                    if (Data_Jarry != null)
                    {
                        Visibility newProd;
                        foreach (JObject result in Data_Jarry)
                        {


                            // pplanNo 처리
                            double pplanNo = double.Parse(result["pplanNo"].ToString()) == 0 ? double.MaxValue : double.Parse(result["pplanNo"].ToString());
                            newProd = result["newProd"].Equals("1") ? Visibility.Visible : Visibility.Hidden;

                            // MainModel 인스턴스 생성 및 기본 속성 설정
                            MainModel listitem = new MainModel
                            {
                                pCnt = float.Parse(result["pCnt"].ToString()), // 통수
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

                            switch (SettingJsonManager.corp_name) //업체명따른 처래처 품명 텍스트 변경
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
                            if (SettingJsonManager.meterialinfo.Equals(""))
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
                            //세팅에서 마닐라 정보 체크시
                            else
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
                            listitem.ProcessTypeName = result["ProcessTypeName"].ToString(); //공정명
                            listitem.pplanMemo = result["pplanMemo"].ToString(); //비고
                            listitem.SpecialMemo = result["SpecialMemo"].ToString(); //특별지시사항

                            if (result["orderDate"].ToString() != null) //수주일자
                            {
                                listitem.orderDate = "[" + result["orderDate"].ToString().Replace("-", "/").Substring(2, 8) + "]";
                            }

                            //원단입고 0:발주, 1:미발주, 2:대체
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
                                    catch (Exception)
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

                            listitem.pplanKey = int.Parse(result["pplanKey"].ToString()); //계획키
                            listitem.orderKey = result["orderKey"].ToString(); //수주키
                            listitem.ProcessType = int.Parse(result["ProcessType"].ToString()); //공정IC
                            listitem.pplanDate = "[" + result["pplanDate"].ToString().Replace("-", "/").Substring(2, 8) + "]"; //계획일자
                            listitem.prodName = result["prodName"].ToString(); //제품명
                            listitem.customSName = result["customSName"].ToString(); //거래처명
                            listitem.paperGor = result["paperGor"].ToString() + " 골"; //골
                            listitem.orderWorkNo = int.Parse(result["orderWorkNo"].ToString()); //수주 생산순번
                            listitem.mercName = result["mercName"].ToString(); //설비명
                            listitem.urgency = result["urgency"].ToString(); //긴급
                            listitem.aL = result["aL"].ToString(); //전장
                            listitem.aW = result["aW"].ToString(); //전폭
                            listitem.aLaW = result["WDmL"].ToString() + "*" + result["WDmW"].ToString(); //전장 * 전폭

                            listitem.ColorType = result["ColorType"].ToString().Replace("|", ""); //인쇄색도
                            listitem.ConnetingType = result["ConnetingType"].ToString(); //접합방법
                            listitem.PrintNo = result["PrintNo"].ToString(); //조판번호
                            listitem.Printposition = result["Printposition"].ToString(); //조판위치
                            listitem.PushingNo = result["PushingNo"].ToString(); //목형번호
                            listitem.Pushingposition = result["Pushingposition"].ToString(); //목형위치

                            if (result["corpName"] != null)
                            {
                                listitem.corpName = result["corpName"].ToString();
                            }
                            listitem.handChk = result["handChk"].ToString(); //손잡이사용
                            listitem.PackType = result["PackType"].ToString(); //포장방법
                            listitem.filmNo = result["filmNo"].ToString(); //필름번호
                            listitem.CoatingType = result["CoatingType"].ToString(); //코팅방법
                            listitem.PalletType = result["PalletType"].ToString(); //파렛트종류
                            listitem.PackCount = result["PackCount"].ToString(); //포장매수
                            listitem.pplanCount = int.Parse(result["pplanCount"].ToString()); //계획량

                           

                            //신우,우리,누리,혜성,성주
                            if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("혜성수출포장") || SettingJsonManager.corp_name.Equals("신우포장테스트") || SettingJsonManager.corp_name.Equals("성주포장"))
                            {
                                //매수
                                if (SettingJsonManager.CountType == 0)
                                {
                                    listitem.boxvisibility = Visibility.Collapsed;
                                }
                                else//통수
                                {
                                    listitem.pplanboxCount = (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                                }
                            }
                            else//기본
                            {
                                listitem.boxvisibility = Visibility.Collapsed;
                            }

                            //신우, 우리, 누리, 혜성, 성주
                            //옵션>상자수 사용시 계획수량
                            if (SettingJsonManager.boxCheck)
                            {
                                listitem.pplanCount = SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("혜성수출포장") || SettingJsonManager.corp_name.Equals("신우포장테스트") || SettingJsonManager.corp_name.Equals("성주포장")
                                    ? int.Parse(result["pplanCount"].ToString())
                                    : (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                            }

                            listitem.presultCount = float.Parse(result["presultCount"].ToString()); //결과량

                            //옵션>상자수 사용시 결과수량에 나누기 통수
                            if (SettingJsonManager.boxCheck)
                            {
                                listitem.presultCount /= listitem.pCnt;
                                listitem.presultCount1 /= listitem.pCnt;
                            }
                            else
                            {
                                //통수
                                if (SettingJsonManager.CountType == 1)
                                {
                                    //신우,우리,누리,성주
                                    //결과값에 통스 나누기
                                    if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("성주포장"))
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

                            listitem.pSize = result["pSize"].ToString(); //규격
                            listitem.orderCount = int.Parse(result["orderCount"].ToString()); //수주량

                            listitem.orderLast = result["orderLast"].ToString(); //납기일
                            if (result["orderLast"].ToString() != null)
                            {
                                listitem.orderLast = "[" + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8) + "]";
                            }

                            listitem.prodID = result["prodID"].ToString(); //제품ID
                            listitem.newProd = newProd;
                            listitem.eDatetime = "";
                            listitem.sDatetime = "E";
                            listitem.subOb = new List<SubModel>();
                            listitem.resultList = new ObservableCollection<InputModel>();
                            listitem.bMember = bool.Parse(result["bMember"].ToString()); //실수요자 포함확인
                            listitem.workMemo = result["workMemo"].ToString();


                            //오주, 대양
                            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트") ||
                                SettingJsonManager.corp_name.Equals("대양포장") || SettingJsonManager.corp_name.Equals("대양포장테스트"))
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

                                listitem.ServerIndex = ServerIndex;
                                listitem.DBName = DBName;
                            }
                            //하나포장 (프로그램 통합 시 주업체, 부업체 구분용)
                            else if (SettingJsonManager.companyIntegration && (SettingJsonManager.corp_name.Equals("하나포장") || SettingJsonManager.corp_name.Equals("하나포장테스트")))
                            {
                                if (ServerIndex == "hnbox")
                                {
                                    listitem.icback = "#AFE1FF";
                                    listitem.ServerIndex = ServerIndex;
                                    listitem.DBName = DBName;
                                }
                                else if (ServerIndex == "shbox")
                                {
                                    listitem.icback = "Whute";
                                    listitem.ServerIndex = ServerIndex;
                                    listitem.DBName = DBName;
                                }
                            }
                            //그 외 업체
                            else
                            {
                                listitem.icback = listitem.bMember ? "#AFE1FF" : "White"; //실수요자일때 하늘색 바탕으로 표시
                                listitem.pStateVisi = Visibility.Collapsed;
                                listitem.ServerIndex = ServerIndex;
                                listitem.DBName = DBName;
                            }

                            //상태표시
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

                            if (SettingJsonManager.corp_name.Equals("오주포장") && (SettingJsonManager.mercName.Equals("인라인2호기") || SettingJsonManager.mercName.Equals("인라인3호기")) ||
                                SettingJsonManager.corp_name.Equals("오주포장테스트") && (SettingJsonManager.mercName.Equals("인라인2호기") || SettingJsonManager.mercName.Equals("인라인3호기")))
                            {
                                if (listitem.ProcessType == 7 || listitem.ProcessType == 8) //후렉소(1), 1공장철(7), 베가(8), 인라인(13)
                                {
                                    //작업정보 설정값
                                    string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
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
                                                item = SettingJsonManager.color_check
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
                            else if (SettingJsonManager.corp_name.Equals("오주포장") && SettingJsonManager.mercName.Equals("풀접합") ||
                                     SettingJsonManager.corp_name.Equals("오주포장테스트") && SettingJsonManager.mercName.Equals("풀접합"))
                            {
                                if (listitem.ProcessType == 8 || listitem.ProcessType == 7 || listitem.ProcessType == 5) //7:철접합  8: 풀접합
                                {
                                    //작업정보 설정값
                                    string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
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
                                                item = SettingJsonManager.color_check
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
                            else if (SettingJsonManager.corp_name.Equals("오주포장") &&
                                    (SettingJsonManager.mercName.Equals("베가") ||
                                     SettingJsonManager.mercName.Equals("1공장1합철") ||
                                     SettingJsonManager.mercName.Equals("1공장2합철") ||
                                     SettingJsonManager.mercName.Equals("1공장수동철")))
                            {
                                if (listitem.ProcessType == 7 || listitem.ProcessType == 8) //풀접합
                                {
                                    string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
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
                                                item = SettingJsonManager.color_check
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
                            else if (SettingJsonManager.corp_name.Equals("오주포장") && SettingJsonManager.mercName.Equals("2공장인쇄기"))
                            {
                                if (listitem.ProcessType == 27) //2공장 인쇄기
                                {
                                    if (listitem.ProcessType == SettingJsonManager.processType)
                                    {
                                        //작업정보 설정값
                                        string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
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
                                                    item = SettingJsonManager.color_check ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ") : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
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
                            //기본업체
                            else
                            {
                                //공정명 없을때
                                if (listitem.mercName.Equals(""))
                                {
                                    //설비 번호로 구별
                                    if (listitem.ProcessType == SettingJsonManager.processType)
                                    {
                                        //작업정보 설정값
                                        string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
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
                                                    item = SettingJsonManager.color_check ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ") : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
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
                                                subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());       //계획키

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
                                    if (listitem.mercName.Equals(SettingJsonManager.mercName))
                                    {
                                        //작업정보 설정값
                                        string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
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
                        Console.WriteLine("계획리스트 수량: " + workList.Count);

                    }
                    else
                    {
                        //MessageBox_Dialog box = new MessageBox_Dialog("PopWorkList 조회 오류!!");
                        //_ = box.ShowDialog();
                    }
                }
                else
                {
                    Console.WriteLine("생산계획 조회 오류");
                }

                
            }
        }

        //생산결과 조회
        public void ResultSearchDate()
        {
            Console.WriteLine("[PopResultSearchDate] 조회");

            // 서버 인덱스를 저장할 리스트 생성
            Dictionary<string, string> serverIndices2 = new Dictionary<string, string>();


            if (SettingJsonManager.companyIntegration && (SettingJsonManager.corp_name.Equals("하나포장") || SettingJsonManager.corp_name.Equals("하나포장테스트")))
            {
                if (SettingJsonManager.corp_name.Equals("하나포장테스트"))
                {
                    serverIndices2.Add("hnbox", "icdtest3"); serverIndices2.Add("opbox", "icdtest23");
                }
                else
                {
                    serverIndices2.Add("hnbox", "A07E164641");
                    serverIndices2.Add("opbox", "8E874414");
                }
            }
            else
            {
                serverIndices2.Add(SettingJsonManager.serverIndex, SettingJsonManager.corp_id);
            }
            foreach (var server in serverIndices2)
            {
                string ServerIndex = server.Key;
                string DBName = server.Value;


                production = null;
                ServiceModel.ServiceModelClean(ServerIndex, DBName);
                ReturnModel returnData = ServiceModel.GetData(
                    ServerIndex,
                    "[PopResultSearchDate]",
                    new string[] { "@pworkDate" },
                    new string[] { DateTimeManager.GetSelectedDateStr() },
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
                            //결과값이 있는지에 따른 정보 관리.
                            if (Data_Jarry.Count == 0)
                            {
                                workItem.presultCount = 0;   //생산결과 수량
                                workItem.sumPworkCount += 0; //생산결과 총 수량
                                workItem.sDatetime = ""; //시작시간
                                workItem.perrorCount += 0;
                                workItem.eDatetime = "";
                            }
                            else
                            {
                                foreach (JObject result in Data_Jarry)
                                {
                                    _ = new InputModel();
                                    //해당 설비의 계획리스트에서 pplanKey일치
                                    if (workItem.pplanKey.ToString().Equals(result["pplanKey"].ToString()))
                                    {
                                        //현재POP 설정된 설비명과 일치
                                        if (result["mercName"].ToString().Equals(SettingJsonManager.mercName))
                                        {
                                            workItem.presultCount = float.Parse(result["pworkCount"].ToString());   //생산결과 수량
                                            workItem.sumPworkCount += float.Parse(result["pworkCount"].ToString()); //생산결과 총 수량
                                            workItem.sDatetime = result["sDatetime"].ToString(); //시작시간

                                            if (!result["perrorCount"].ToString().Equals("")) //불량수량
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
                                                //생산중이 아니면
                                                else
                                                {
                                                    production = workItem;

                                                    //카운터 집계용 로컬DB
                                                    if (SettingJsonManager.dbUse)
                                                    {
                                                        DataTable temp = new LocalDBManager().GetRawData("Last");

                                                        // 설비명 동일 && 제품 orderKey 동일 && 메세지 타입은 N
                                                        if (temp.Rows[0]["MercName"].ToString().Equals(SettingJsonManager.mercName)
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
                        }

                        //리스트 만큼 반복
                        for (int i = 0; i < workList.Count; i++)
                        {
                            //상자수 체크시 결과값에 통수 나누기
                            if (SettingJsonManager.boxCheck)
                            {
                                workList[i].sumPworkCount = workList[i].sumPworkCount / workList[i].pCnt;
                            }

                            //완료항목 제거 체크시
                            if (SettingJsonManager.completeValue_checked)
                            {
                                //종료시간 있으면 삭제
                                if (workList[i].eDatetime.Length > 5)
                                {
                                    workList.Remove(workList[i]);
                                }
                            }
                        }

                        //동시작업 설정 체크시
                        if (!SettingJsonManager.samework)
                        {
                            foreach (MainModel item in tempOb)
                            {
                                if (!SettingJsonManager.mercName.Equals(item.mercName))
                                {
                                    //작업중 버튼이 아니면 삭제
                                    if (!item.imagePath.Equals("/img/bn_01.png"))
                                    {
                                        _ = workList.Remove(item);
                                    }
                                }
                            }
                        }
                        txtNoPlan.Visibility = workList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    LogManager.Info("생산결과 리스트 조회 완료", MethodBase.GetCurrentMethod().DeclaringType);
                }
                else
                {
                    LogManager.Error("생산결과 리스트 조회 오류", MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
            SortWorkList();

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

                if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("대양포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
                {
                    oderLast_Sort = true; //수주일자 정렬
                }
            }
            else//일반 정렬(상/하단)
            {
                SortWorkList();
            }
        }

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
                bool sortCheck = SettingJsonManager.RadioChecked;

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
                    if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("대양포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
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
                        //하나포장, 통합관리 설정값이 true일 경우
                        if (SettingJsonManager.companyIntegration && (SettingJsonManager.corp_name.Equals("하나포장") || SettingJsonManager.corp_name.Equals("하나포장테스트")))
                        {
                            collectionView.SortDescriptions.Add(sort3); //긴급
                            collectionView.SortDescriptions.Add(sort1); //계획번호
                            collectionView.SortDescriptions.Add(sort4); //생산차수
                        }
                        else
                        {
                            collectionView.SortDescriptions.Add(sort3);
                            collectionView.SortDescriptions.Add(sort1);
                            collectionView.SortDescriptions.Add(sort2);
                            collectionView.SortDescriptions.Add(sort4);
                        }
                    }
                    collectionView.Refresh();
                }
                else//상단정렬
                {
                    collectionView.SortDescriptions.Add(sort);  //시작시간
                    collectionView.SortDescriptions.Add(sort8); //상태
                    if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("대양포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
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
                        //하나포장, 통합관리 설정값이 true일 경우
                        if (SettingJsonManager.companyIntegration && (SettingJsonManager.corp_name.Equals("하나포장") || SettingJsonManager.corp_name.Equals("하나포장테스트")))
                        {
                            collectionView.SortDescriptions.Add(sort3); //긴급
                            collectionView.SortDescriptions.Add(sort1); //계획번호
                            collectionView.SortDescriptions.Add(sort4); //생산차수
                        }
                        else
                        {
                            collectionView.SortDescriptions.Add(sort3); //긴급
                            collectionView.SortDescriptions.Add(sort1); //계획번호
                            collectionView.SortDescriptions.Add(sort2); //계획키
                            collectionView.SortDescriptions.Add(sort4); //생산차수
                        }
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
                //하늘색처리된 계획건 수량
                if (workList[i].icback.Equals("#AFE1FF"))
                {
                    count = count + 1;
                }
            }

            if (SettingJsonManager.corp_name.Equals("오주포장"))
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
            SettingJsonManager.consumersumcount = 0;
            SettingJsonManager.SaveJsonFile();

            //관리자 계정이 아닐때만 현황판으로 꺼짐 신호 보냄 
            if (!SettingJsonManager.manager.Equals("1"))
            {
                CountManager.SendMsg(SettingJsonManager.mercName, "B"); //설비명, 꺼짐신호
            }

            //인증코드 저장(로그인 정보)
            SettingJsonManager.SaveJsonFile();

            //프로그램 종료
            LogManager.Info("프로그램 종료", MethodBase.GetCurrentMethod().DeclaringType);
            Environment.Exit(0);
        }

        // DependencyObject의 부모를 찾는 재귀적 메서드
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // VisualTreeHelper를 사용하여 child의 부모 요소를 가져옴.
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // 부모 요소가 없으면 null을 반환.
            if (parentObject == null)
                return null;

            // 부모 요소가 형식 T와 일치하는지 확인합니다.
            T parent = parentObject as T;

            // 부모 요소가 형식 T와 일치하는 경우 해당 부모 요소를 반환합니다.
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // 다음 상위 레벨로 계속 검색
                return FindParent<T>(parentObject);
            }
        }

        // 상태 버튼 클릭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //두성포장은 상태버튼 클릭시 Detail창으로 진입
                if (SettingJsonManager.corp_name.Equals("두성포장") || SettingJsonManager.corp_name.Equals("두성포장테스트") || SettingJsonManager.detailPopup)
                {
                    // Canvas 객체를 생성하여 sender로 전달
                    Canvas canvas = FindParent<Canvas>((FrameworkElement)sender);
                    if (canvas != null)
                    {
                        ShowDetail(canvas);
                    }
                    return;
                }

                bool carryForwardButton = true; //이월 버튼 상태
                if (sender.GetType().Name.Equals("Image"))
                {
                    can.Visibility = Visibility.Visible; //화면 어둡게 처리

                    Image img = (Image)sender;
                    MainModel model = (MainModel)img.DataContext;

                    //실수요자 일때
                    if (model.bMember)
                    {
                        //대기 버튼 진입
                        if (model.imagePath.Equals("/img/bn_02.png"))
                        {
                            carryForwardButton = false; //이월 버튼 비활성화
                            model.sDatetime = DateTimeManager.GetSelectedDateStr(); //yyyy-MM-dd

                            //관리자 로그인 아닐때 키 생성
                            if (!SettingJsonManager.manager.Equals("1"))
                            {
                                //아두이노 연결 안되어있으면 무시
                                if (!SettingJsonManager.SerialName.Equals("") && production == null)
                                {
                                    arduSerial.Write("A");//아두이노 카운터 리셋
                                }

                                PopResultStart(model); //시작 SP 실행
                            }
                        }
                        //작업중 버튼 진입
                        else if (model.imagePath.Equals("/img/bn_01.png"))
                        {
                            carryForwardButton = false; //이월 버튼 비활성화
                        }
                        //완료 버튼 진입
                        else if (model.imagePath.Equals("/img/bn_03.png"))
                        {
                            carryForwardButton = true; //이월 버튼 활성화

                            //아두이노 연결 안되어있으면 무시
                            if (!SettingJsonManager.SerialName.Equals("") && production == null)
                            {
                                arduSerial.Write("A");//아두이노 카운터 리셋
                            }
                        }
                        var vvvv = dSConsumer;
                        e.Handled = false;
                        dSConsumer = new DSConsumer(model, carryForwardButton);
                        dSConsumer.ShowDialog(); //실수요자 리스트 창 팝업
                        dSConsumer = null;

                        CountManager.SendMsg(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
                        SelectWorkList();   //생산계획 조회 [PopWorkList]
                        ResultSearchDate(); //생산결과 조회 [PopResultSearchDate]
                    }
                    //일반생산
                    else
                    {
                        win_Input = new Win_input(model, workList, false);
                        _ = win_Input.ShowDialog(); //인풋창 팝업

                        //팝업창 종료 후
                        //
                        if (SettingJsonManager.presultcount != 0)
                        {
                            model.presultCount = SettingJsonManager.presultcount;
                        }

                        //대세, 구룡일때
                        if (SettingJsonManager.corp_name.Equals("대세물산") ||
                            SettingJsonManager.corp_name.Equals("대세물산테스트") ||
                            SettingJsonManager.corp_name.Equals("구룡산업제2공장") ||
                            SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트") ||
                            SettingJsonManager.corp_name.Equals("금화") ||
                            SettingJsonManager.corp_name.Equals("금화테스트"))
                        {
                            SelectWorkList();
                            ResultSearchDate();
                            LogManager.Info("대세,구룡 인풋창 닫은 후 계획, 결과건 갱신 완료", MethodBase.GetCurrentMethod().DeclaringType);
                        }

                        SettingJsonManager.presultcount = 0;
                        SettingJsonManager.SaveJsonFile();
                    }

                    //유창
                    if (SettingJsonManager.corp_name.Equals("유창포장") || SettingJsonManager.corp_name.Equals("유창포장테스트"))
                    {
                        //계획건수 20개 미만일때
                        if (workList.Count < 20)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                    }
                    win_Input = null;

                    //아두이노 현재값 요청
                    if (production != null)
                    {
                        //arduSerial.Write("B");
                    }

                    e.Handled = false;
                    can.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception e1)
            {
                if (e1.Message == "포트가 닫혀 있습니다.")
                {
                    SettingJsonManager.SerialName = "";
                }
                LogManager.Error("상태버튼 클릭 오류 : " + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);
                Console.WriteLine(e1.Message); e.Handled = false;
                can.Visibility = Visibility.Collapsed;
            }
        }

        //실수요자 생산 시작
        private int PopResultStart(MainModel model)
        {
            if (SettingJsonManager.corp_name.Contains("테스트") && model.bMember)
            {
                Console.WriteLine("---------------------------------------------------------------------------" + Environment.NewLine +
                                  "실수요자 팝업 : PopResultStart" + Environment.NewLine +
                                  "pworkDate:" + DateTimeManager.GetSelectedDateStr() + Environment.NewLine +
                                  "orderKey:" + model.orderKey + Environment.NewLine +
                                  "pplanKey:" + model.pplanKey + Environment.NewLine +
                                  "ProcessType:" + model.ProcessType + Environment.NewLine +
                                  "mercName:" + SettingJsonManager.mercName + Environment.NewLine +
                                  "UpUserID:" + MainWindow.userID +
                                  "UpUserName:" + MainWindow.userName + Environment.NewLine +
                                  "workerName:" + "" + Environment.NewLine +
                                  "workerID:" + "" + Environment.NewLine +
                                  "SCMKey:" + "");
            }


            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultStart]",
                new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey" },
                new object[] {
                DateTimeManager.GetSelectedDateStr(), //yyyy-MM-dd
                model.orderKey,
                model.pplanKey,
                model.ProcessType,
                SettingJsonManager.mercName,
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

                model.sDatetime = DateTimeManager.GetSelectedStr(); //yyyy-MM-dd HH:mm:ss
                CountManager.SendMsg(SettingJsonManager.mercName, "S");

                SettingJsonManager.pworkkey = returnData.리턴값;
                SettingJsonManager.count = 0;
                SettingJsonManager.saveDate = "";
                SettingJsonManager.pplanKey = 0;
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;
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

            // 클릭한 캔버스의 부모 요소인 Border의 BorderThickness 속성을 조정
            Border border = VisualTreeHelper.GetParent(canvas) as Border;
            if (border != null)
            {
                border.BorderThickness = new Thickness(0); // 클릭한 캔버스의 테두리를 두껍게 변경
            }

            // 현재 클릭한 캔버스를 이전에 클릭한 캔버스로 설정
            previousClickedCanvas = canvas;

            //듀얼상세화면 체크시
            if (SettingJsonManager.detail_check)
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


        //디테일창 팝업
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
                        Win_Detail win_detail = new Win_Detail(context, special, workList, SettingJsonManager.detail_check, false);
                        _ = win_detail.ShowDialog();

                        if (SettingJsonManager.corp_name.Equals("유창포장"))
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
                        LogManager.Error("디테일 창 팝업 실패 : " + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);
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
                LogManager.Error("디테일 창 팝업 실패 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private List<Win_Detail> activeDetails = new List<Win_Detail>();
        int Barcode_Scan = 0;


        private void TbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("바코드 리드");
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
                //can.Visibility = Visibility.Visible;
                string special = "";
                for (int i = 0; i < workList.Count; i++)
                {
                    if (tempItem.orderKey.Equals(workList[i].orderKey.ToString()))
                    {
                        special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                    }
                }

                //디테일창 팝업
                try
                {
                    //PLC 사용시 
                    CNS_Barcode_Send = SettingJsonManager.Plc ? true : false;

                    if ((SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트")) && SettingJsonManager.mercName.Equals("후렉소1호기"))
                    {
                        Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, SettingJsonManager.detail_check, CNS_Barcode_Send);
                        activeDetails.Add(win_Detail);
                        win_Detail.Show();
                    }
                    else
                    {
                        Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, SettingJsonManager.detail_check, CNS_Barcode_Send);
                        _ = win_Detail.ShowDialog();
                        win_Input = null;
                        win_Detail = null;
                    }
                }
                catch (Exception e1)
                {
                    LogManager.Error("디테일 창 팝업 실패 : " + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);
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
                //can.Visibility = Visibility.Collapsed;
            }
            e.Handled = false;
        }

        private System.Threading.Timer timers;
        private void StartTimer()
        {
            Console.WriteLine("StartTimer() 실행");
            // 타이머 시작, 1초마다 Callback 메소드 호출
            timers = new System.Threading.Timer(Callback, null, 0, 1000);
        }

        private void Callback(object state)
        {
            Console.WriteLine("Callback(object state) 실행");

            // Task를 사용하여 백그라운드 작업 수행
            Task.Run(() =>
            {
                // UI 컨트롤에 안전하게 접근하기 위해 Dispatcher 사용
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // TextBox에 포커스 주고 전체 선택
                    tbBarcode.Focus();
                    tbBarcode.SelectAll();
                });

                // 여기에 추가적인 백그라운드 로직 구현
                ProcessBarcode();
            });
        }

        private void ProcessBarcode()
        {
            Console.WriteLine("ProcessBarcode() 실행");

            Barcode_Scan++;

            MainModel tempItem = null;

            CNS_Barcode_Send = false;
            string code = "";

            Application.Current.Dispatcher.Invoke(() => {
                code = tbBarcode.Text;
            });
            _ = int.TryParse(code, out int pCode); //바코드 값  정수로 변환
            Console.WriteLine("pCode: " + code);


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
                //can.Visibility = Visibility.Visible;
                string special = "";
                for (int i = 0; i < workList.Count; i++)
                {
                    if (tempItem.orderKey.Equals(workList[i].orderKey.ToString()))
                    {
                        special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                    }
                }

                //디테일창 팝업
                try
                {
                    //PLC 사용시 
                    CNS_Barcode_Send = SettingJsonManager.Plc ? true : false;

                    if ((SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트")) && SettingJsonManager.mercName.Equals("후렉소1호기"))
                    {
                        Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, SettingJsonManager.detail_check, CNS_Barcode_Send);
                        activeDetails.Add(win_Detail);
                        win_Detail.Show();
                    }
                    else
                    {
                        Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, SettingJsonManager.detail_check, CNS_Barcode_Send);
                        _ = win_Detail.ShowDialog();
                        win_Input = null;
                        win_Detail = null;
                    }
                }
                catch (Exception e1)
                {
                    Console.WriteLine("예외");
                    LogManager.Error("디테일 창 팝업 실패 : " + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);
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
                        Console.WriteLine("예외");
                        string exception = ex.Message;
                    }

                }
                //can.Visibility = Visibility.Collapsed;
            }

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
        private void Logout_click(object sender, RoutedEventArgs e)
        {
            setLock();

        }

        //LOCK /UNLOCK
        private void setLock()
        {
            //lock일때
            if (SettingJsonManager.checkMerc)
            {
                AuthCode auth = new AuthCode();
                auth.ShowDialog();

                //비밀번호 일치시
                if (auth.PasswordCorrect == 1)
                {
                    unlock_img.Visibility = Visibility.Visible;
                    lock_img.Visibility = Visibility.Collapsed;
                    merc_lock.Text = "UNLOCK";

                    if (SettingJsonManager.corp_name.Equals("대세물산") ||
                        SettingJsonManager.corp_name.Equals("유창포장") ||
                        SettingJsonManager.corp_name.Equals("광성수출포장") ||
                        SettingJsonManager.corp_name.Contains("테스트") ||
                        Environment.MachineName.Equals("PRG-PJH-DESK"))
                    {
                        txtDate2.Visibility = Visibility.Visible; //날짜변경 버튼 표시
                        txtDate3.Visibility = Visibility.Visible; //날짜변경 버튼 표시
                    }

                    SettingJsonManager.checkMerc = false;
                    LogManager.Info("Lock => UnLock", MethodBase.GetCurrentMethod().DeclaringType);
                }
                else if (auth.PasswordCorrect == 2)//비밀번호 미일치
                {
                    MessageBox_Dialog box = new MessageBox_Dialog("비밀번호가 일치하지 않습니다. \n\n비밀번호 분실 시 국제전산에 문의해주세요.");
                    _ = box.ShowDialog();
                }
                else if (auth.PasswordCorrect == 0)
                {

                }
            }
            else//unlock일때
            {
                unlock_img.Visibility = Visibility.Collapsed;
                lock_img.Visibility = Visibility.Visible;
                merc_lock.Text = "LOCK";
                txtDate2.Visibility = Visibility.Collapsed; //날짜변경 버튼 숨김
                txtDate3.Visibility = Visibility.Collapsed; //날짜변경 버튼 숨김

                SettingJsonManager.checkMerc = true;
                LogManager.Info("UnLock => Lock", MethodBase.GetCurrentMethod().DeclaringType);

            }

            SettingJsonManager.SaveJsonFile();
        }

        //설정 버튼
        private void Setting(object sender, MouseButtonEventArgs e)
        {
            LogManager.Info("설정창 열기", MethodBase.GetCurrentMethod().DeclaringType);
            can.Visibility = Visibility.Visible;
            settingPopupCheck = true;
            Win_Setting win_Setting = new Win_Setting();
            _ = win_Setting.ShowDialog();
            string metrialitem = SettingJsonManager.meterialinfo;
            settingPopupCheck = false;

            SetSetting(metrialitem);
            //arduSerial.DataReceived -= ArduReReceive;

            ArduConn();//아두이노 통신연결(설정창 종료시)
            // ArduConn1();

            //스크롤바 사용 유/무
            sv.VerticalScrollBarVisibility = SettingJsonManager.scrollvarCheck ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            if (SettingJsonManager.scrollvarCheck)
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

            if (SettingJsonManager.searchCheck)
            {
                SelectWorkList(); // 생산 계획 리스트 조회
                ResultSearchDate();
            }

            SettingJsonManager.searchCheck = false;
            SettingJsonManager.SaveJsonFile();
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
                    info.Text = "마닐라정보";
                    break;

                case "20":
                    info.Text = "원단정보";
                    break;

                default:
                    arduSerial.Close();
                    timer.Stop();
                    MainWindow.main1 = null;

                    if (MainWindow.main == null)
                    {
                        MainWindow.main = new Main();
                    }
                    Content = MainWindow.main;
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
                //하나포장 (프로그램 통합 시 주업체, 부업체 구분용)
                if (SettingJsonManager.companyIntegration && (SettingJsonManager.corp_name.Equals("하나포장") || SettingJsonManager.corp_name.Equals("하나포장테스트")))
                {
                    item.icback = item.ServerIndex.Equals("hnbox") ? "#AFE1FF" : "White";
                }
                //그 외 업체
                else
                {
                    item.icback = "White";
                }
            }

            if (!SettingJsonManager.ChangeCheck)
            {
                UpdateWorkPlanNResult(); // 계획/결과 조회
            }

            SettingJsonManager.ChangeCheck = false;
            SettingJsonManager.index = 0;
            SettingJsonManager.SaveJsonFile();
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
                    OpenSearchList();
                    break;
                case "F2":
                    Ip_input ip_Input = new Ip_input();
                    ip_Input.ShowDialog();
                    break;
                case "F8":
                    if (SettingJsonManager.manager.Equals("0"))
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

        private void workList_search(object sender, MouseButtonEventArgs e)
        {
            //SearchList search = new SearchList(workList);
            //_ = search.ShowDialog();
            OpenSearchList();
            SelectWorkList();
            ResultSearchDate();
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
            DateTimeManager.SetPreviousDate();

            SelectWorkList(); //계획 조회
            ResultSearchDate(); //결과 조회
        }

        //날짜 앞으로 이동
        private void Date_Click2(object sender, MouseButtonEventArgs e)
        {
            DateTimeManager.SetNextDate();

            SelectWorkList(); //계획 조회
            ResultSearchDate(); //결과 조회
        }


        //설비점검 클릭
        private void Machine_Check(object sender, MouseButtonEventArgs e)
        {
            MachineCheck machineCheck = new MachineCheck();
            machineCheck.Show();
        }

        //상태버튼 클릭시 SignalR 재연결
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("SignalR 서버에 재연결 합니다.");
                dialog.ShowDialog();
                LogManager.Info("SignalR 재연결 요청", MethodBase.GetCurrentMethod().DeclaringType);

                SignalRAdministrator conn = new SignalRAdministrator();
                conn.Setconnection(false);
                conn.Setconnection(true);
            }
            catch (Exception ex)
            {
                LogManager.Error("SignalR 재연결 요청 실패: " + ex.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // 프로그램 종료 시 JSON 파일에 설정 정보 저장
            SettingJsonManager.SaveJsonFile();
        }

        //공지사항 버튼
        private void notice_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Notice_PopUp();
        }

        private void Notice_PopUp()
        {
            Notice notice = new Notice();
            _ = notice.ShowDialog();
        }


        //버전 클릭시 날짜조절 가능하도록 함
        private void versionTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isTextVisible)
            {
                // 텍스트 블록이 보이지 않는 경우, 보이도록 설정
                txtDate2.Visibility = Visibility.Visible;
                txtDate3.Visibility = Visibility.Visible;
                isTextVisible = true;
            }
            else
            {
                // 텍스트 블록이 보이는 경우, 숨기도록 설정
                txtDate2.Visibility = Visibility.Collapsed;
                txtDate3.Visibility = Visibility.Collapsed;
                isTextVisible = false;
            }
        }

        //검색창 호출
        private void OpenSearchList()
        {
            if (searchListWindow == null || !searchListWindow.IsVisible)
            {
                searchListWindow = new SearchList(workList);
                searchListWindow.ShowDialog();
            }
            else
            {
                searchListWindow.Activate();
            }
        }

        //검색창에서 클릭한 품목으로 스크롤 이동
        private void SearchList_Pointer()
        {
            for (int i = 0; i < workList.Count; i++)
            {
                //하늘색처리된 계획건 수량
                if (workList[i].icback.Equals("#6799FF"))
                {
                    sv.ScrollToVerticalOffset((77.8 * i) - (77.8 * 4));
                }
            }
        }
    }
    public class StrConverte : IValueConverter
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

    public class IndexConverte : IValueConverter
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


    public class IndexConverte1 : IValueConverter
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