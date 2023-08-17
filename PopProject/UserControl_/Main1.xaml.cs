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

namespace PopProject.UserControl_
{
    /// <summary>
    /// Main.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Main1 : UserControl
    {
        // int pageCheck = 0;
        private DispatcherTimer timer;
        public bool backgroundCheck = true;
        public static SerialPort arduSerial = new SerialPort();
        public static Win_input win_Input;
        public ObservableCollection<MainModel> workList = null;
        public ObservableCollection<MainModel> workList1 = null;

        // 새로운 BackgroundWorker 생성
        private readonly BackgroundWorker bw = new BackgroundWorker();
        // 중간 취소를 지원할 경우 설정값

        public static Consumer consumer_;
        public static MainModel production = null;
        private int minuite = 0;
        public static int str;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        private bool CNS_Barcode_Send = false; //true이면 바코드 읽으면 CNS로 데이터 전송

        public Main1()
        {
            InitializeComponent();
            production = null;
            setLock();
            string metrialitem = Properties.Settings.Default.meterialinfo;
            SetSetting(metrialitem);
            string merName = Properties.Settings.Default.mercName;
            tbMercName.Text = merName;

            try
            {
                SignalRAdministrator conn = new SignalRAdministrator();

                conn.Setconnection(true);
            }
            catch (Exception)
            {

            }
            Properties.Settings.Default.consumersumcount = 0;
            Properties.Settings.Default.Save();

            corp_name.Text = Properties.Settings.Default.corp_name;
            ArduConn();
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
            //setTbPintNo();
            bool connected = NetworkInterface.GetIsNetworkAvailable();
            if (connected)
            {
                SelectWorkList();
                ResultSearchDate();

                TimerSet();
                //  BackGround();
            }


            else
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("인터넷 상태를 확인해 주세요");
                _ = dialog.ShowDialog();
            }

            //testSearch();

        }

        private void ArduConn()
        {
            try
            {
                if (arduSerial.IsOpen)
                {
                    arduSerial.Close();
                }

                arduSerial.PortName = Properties.Settings.Default.SerialName;
                arduSerial.BaudRate = Properties.Settings.Default.baudrate;
                arduSerial.DataReceived += ArduReReceive;
                arduSerial.Open();
                arduSerial.Write(Properties.Settings.Default.SensorRate);
                btrSerial.Background = Brushes.Green;

            }

            catch (Exception e)
            {

                btrSerial.Background = Brushes.Red;
                Console.WriteLine(e.Message);
                //MessageBox_Dialog msgbox = new MessageBox_Dialog(e.Message);
                //msgbox.ShowDialog();
            }
        }

        public void ArduReReceive(object sender, SerialDataReceivedEventArgs e)
        {


            try
            {
                str = int.Parse(arduSerial.ReadLine().Trim());
            }

            catch (Exception)
            {

            }

            if (production != null)
            {
                float pCnt;


                pCnt = Properties.Settings.Default.twoSum ? production.pCnt * 2 : production.pCnt;
                if (!Properties.Settings.Default.saveDate.Equals("") && !Properties.Settings.Default.saveDate.Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    if (production.pplanKey == Properties.Settings.Default.pplanKey)
                    {
                        str = Properties.Settings.Default.count + str;
                    }
                }

                switch (Properties.Settings.Default.processType)
                {

                    /*
                     case 5:
                     case 8:
                     case 9:
                     case 10:
                     case 11:

                         production.sumPworkCount = str;
                         production.presultCount = str;

                         if (win_Input != null)
                         {
                             win_Input.inputModel.count = str;
                             Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                         }

                         invokeCount(str);
                         break;
                    */

                    default:
                        if (Properties.Settings.Default.CountType == 0)
                        {
                            production.sumPworkCount = str;
                            production.presultCount = str;

                            if (win_Input != null)
                            {

                                win_Input.inputModel.count = str;
                                Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);

                            }

                            InvokeCount(str);
                        }

                        else
                        {

                            production.sumPworkCount = Convert.ToInt32(Math.Floor(str * pCnt));
                            if (Properties.Settings.Default.boxCheck)
                            {
                                if (pCnt >= 1)
                                {
                                    production.sumPworkCount /= (float)pCnt;
                                }
                            }

                            production.presultCount = production.sumPworkCount;

                            if (win_Input != null)
                            {
                                win_Input.inputModel.count = production.presultCount;
                                Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                            }

                            InvokeCount(Convert.ToInt32(production.presultCount));
                        }
                        break;
                }
                // Console.WriteLine("sumPworkCount : " + (str * production.pCnt));
                //  Console.WriteLine("presultCount : " + (str * production.pCnt));
                //  int lastcount = str;
                //  double 분당생산수 = str - lastcount;

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

        private void TimerSet()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            DateTime dt = DateTime.Now;
            string datePart = DateTime.Today.ToString("D");
            string datetime = string.Format("{0:D2}시 {1:D2}분 {2:D2}초",
                              dt.Hour, dt.Minute, dt.Second);
            txtDate.Text = datePart;
            txtClock.Text = datetime;

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

            _ = tbBarcode.Focus();
            tbBarcode.SelectAll();
          
            if (minuite == 300)
            {
                bool connected = NetworkInterface.GetIsNetworkAvailable();
                Console.WriteLine("네트워크 연결 체크 : " + connected);
                if (connected)
                {
                    if (production != null) { InvokeCount(Convert.ToInt32(production.presultCount)); }
                    if (win_Input == null)
                    {

                        if (!Properties.Settings.Default.corp_name.Equals("유창포장"))
                        {

                            SelectWorkList();
                            ResultSearchDate();

                        }

                    }

                    if (!arduSerial.IsOpen)
                    {
                        ArduConn();
                    }

                    minuite = 0;
                }

            }

            if (Properties.Settings.Default.corp_name.Equals("유창포장"))
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
                }

            }

            minuite += 1;

            //  Console.WriteLine("minuite - " + minuite);

        }

        // 현황판으로 카운터 전송

        public void InvokeCount(int str)
        {
            try
            {
                _ = SignalRAdministrator.허브.Invoke("invokeCount", Properties.Settings.Default.corp_id, Properties.Settings.Default.mercName, str + "");
            }
            catch (Exception)
            {
            }
        }

        private void Serial_click(object sender, RoutedEventArgs e)
        {
        }

        private void Remote_click(object sender, MouseButtonEventArgs e)
        {
            _ = Process.Start("http://onefax.co.kr/w_onefax/remotebox_view.aspx");
        }

        private void TxtDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Time_pick.Time_pick time_Pick = new Time_pick.Time_pick();
            _ = time_Pick.ShowDialog();
        }

        private void Mer_click(object sender, MouseButtonEventArgs e)
        {

            if (!Properties.Settings.Default.checkMerc)
            {
                string before = Properties.Settings.Default.mercName;
                MercList mercList = new MercList();
                _ = mercList.ShowDialog();
                tbMercName.Text = Properties.Settings.Default.mercName;
                if (!before.Equals(Properties.Settings.Default.mercName))
                {
                    //관리자 계정이 아닐때만 현황판으로 켜짐 신호 보냄 
                    if (!Properties.Settings.Default.manager.Equals("1"))
                    {
                        invokeMercName(before, "B");
                        Properties.Settings.Default.error = "";
                        Properties.Settings.Default.error1 = "";
                        Properties.Settings.Default.Save();
                    }
                }
                else
                {
                    //관리자 계정이 아닐때만 현황판으로 켜짐 신호 보냄 
                    if (!Properties.Settings.Default.manager.Equals("1"))
                    {
                        invokeMercName(Properties.Settings.Default.mercName, "A");
                    }
                }
                //setTbPintNo();
                SelectWorkList();
                ResultSearchDate();
            }
           

        }
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
        }
        public void SelectWorkList()
        {
            LogData logData = new LogData("조회");
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[PopWorkList]",
               new string[] { "@pplanDate" },
               new string[] { DateTime.Now.ToString("yyyy-MM-dd") },
               0
               );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    Visibility newProd;
                    ObservableCollection<MainModel> workList = new ObservableCollection<MainModel>();
                    foreach (JObject result in Data_Jarry)
                    {
                        double pplanNo = double.Parse(result["pplanNo"].ToString()) == 0 ? double.MaxValue : double.Parse(result["pplanNo"].ToString());
                        newProd = result["newProd"].Equals("1") ? Visibility.Visible : Visibility.Hidden;
                        MainModel listitem = new MainModel
                        {
                            pCnt = float.Parse(result["pCnt"].ToString()),
                            pplanNo = pplanNo
                        };
                        if (result["givWD"].ToString() != null)
                        {
                            listitem.givWD = result["givWD"].ToString();
                        }
                        if (result["givSC"].ToString() != null)
                        {
                            listitem.givSC = result["givSC"].ToString();
                        }
                        // Console.WriteLine("meterialinfo --> " + Properties.Settings.Default.meterialinfo);
                        switch (Properties.Settings.Default.corp_name)
                        {

                            case "오주포장":
                                div.Text = "[수주날짜][납품일자] 거래처/품명";
                                Canvas.SetLeft(div, 113);
                                listitem.visibility = Visibility.Visible;
                                listitem.left = 160;
                                break;

                            case "신우수출포장":
                                div.Text = "[수주날짜] 거래처/품명";
                                Canvas.SetLeft(div, 133);
                                listitem.visibility = Visibility.Visible;
                                listitem.left = 160;
                                break;
                            default:
                                listitem.visibility = Visibility.Collapsed;
                                listitem.left = 70;
                                div.Text = "거래처/품명";
                                Canvas.SetLeft(div, 160);
                                break;

                        }

                        if (Properties.Settings.Default.meterialinfo.Equals(""))
                        {
                            info.Text = "장 * 폭 * 고";
                            listitem.SCBJCount = result["WDScore"].ToString();
                            listitem.SCIGCount = "";
                            listitem.SCmetrialName = "";
                            listitem.mLmW = result["pSize"].ToString();
                        }

                        else if (Properties.Settings.Default.meterialinfo.Equals("19"))
                        {
                            info.Text = "마닐라정보";
                            listitem.aLaW = result["SCmW"].ToString() + "*" + result["SCmL"].ToString();

                            if (listitem.givSC.Equals("2"))
                            {
                                listitem.WDBJCount = "대체량   " + string.Format("{0:n0}", result["SCBJCount"]);
                                listitem.WDIGCount = "";
                            }

                            else
                            {
                                listitem.WDBJCount = "발주   " + string.Format("{0:n0}", result["SCBJCount"]);
                                listitem.WDIGCount = "입고   " + string.Format("{0:n0}", result["SCIGCount"]);
                            }
                            listitem.paperGor = result["SCmetrialName"].ToString();

                        }
                        else
                        {
                            info.Text = "원단정보";
                            listitem.aLaW = result["WDmL"].ToString() + "*" + result["WDmW"].ToString();

                            if (listitem.givWD.Equals("2"))
                            {
                                listitem.WDBJCount = "대체량   " + string.Format("{0:n0}", result["WDBJCount"]);
                                listitem.WDIGCount = "";
                            }

                            else
                            {
                                listitem.WDBJCount = "발주   " + string.Format("{0:n0}", result["WDBJCount"]);
                                listitem.WDIGCount = "입고   " + string.Format("{0:n0}", result["WDIGCount"]);
                            }
                            listitem.paperGor = result["paperGor"].ToString() + " 골";
                        }

                        listitem.pAccount = !result["pAccount"].ToString().Equals("0") ? "(" + result["pAccount"].ToString() + "차)" : "";

                        listitem.ProcessMemo = result["ProcessMemo"].ToString();
                        listitem.ProcessTypeName = result["ProcessTypeName"].ToString();
                        listitem.pplanMemo = result["pplanMemo"].ToString();
                        listitem.SpecialMemo = result["SpecialMemo"].ToString();
                        listitem.pplanMemo = result["pplanMemo"].ToString();
                        listitem.SpecialMemo = result["SpecialMemo"].ToString();
                        if (result["orderDate"].ToString() != null)
                        {
                            listitem.orderDate = "[" + result["orderDate"].ToString().Replace("-", "/").Substring(2, 8) + "]";
                        }

                        listitem.pplanKey = int.Parse(result["pplanKey"].ToString());
                        listitem.orderKey = result["orderKey"].ToString();
                        listitem.ProcessType = int.Parse(result["ProcessType"].ToString());
                        listitem.pplanDate = "[" + result["pplanDate"].ToString().Replace("-", "/").Substring(2, 8) + "]";


                        listitem.prodName = result["prodName"].ToString();
                        listitem.customSName = result["customSName"].ToString();


                        listitem.orderWorkNo = int.Parse(result["orderWorkNo"].ToString());
                        listitem.mercName = result["mercName"].ToString();
                        listitem.urgency = result["urgency"].ToString();
                        listitem.aL = result["aL"].ToString();
                        listitem.aW = result["aW"].ToString();


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
                        listitem.PackCount = result["PackCount"].ToString();
                        listitem.pplanCount = int.Parse(result["pplanCount"].ToString());

                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.pplanCount = Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장")
                             ? int.Parse(result["pplanCount"].ToString())
                             : (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                        }

                        listitem.presultCount = float.Parse(result["presultCount"].ToString());
                        listitem.presultCount1 = float.Parse(result["presultCount"].ToString());
                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.presultCount /= listitem.pCnt;
                            listitem.presultCount1 /= listitem.pCnt;
                        }

                        listitem.pSize = result["pSize"].ToString();
                        listitem.orderCount = int.Parse(result["orderCount"].ToString());
                        listitem.orderLast = result["orderLast"].ToString();
                        listitem.prodID = result["prodID"].ToString();
                        listitem.newProd = newProd;
                        listitem.eDatetime = "";
                        listitem.sDatetime = "E";
                        listitem.subOb = new List<SubModel>();
                        listitem.resultList = new ObservableCollection<InputModel>();
                        listitem.bMember = bool.Parse(result["bMember"].ToString());

                        if (!Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            listitem.icback = listitem.bMember ? "#E2FEFF" : "White";
                        }
                        else
                        {
                            listitem.icback = result["WDIGCount"].ToString().Equals("0") || result["WDIGCount"].ToString().Equals("0.0") ? "White" : "#E2FEFF";
                            if (listitem.givWD.Equals("2"))
                            {

                                listitem.icback = "#E2FEFF";
                            }
                        }


                        if (Properties.Settings.Default.mercName.Equals("인라인2호기") || Properties.Settings.Default.mercName.Equals("인라인3호기") || Properties.Settings.Default.mercName.Equals("풀접합"))
                        {
                            if (listitem.ProcessType == 8 || listitem.ProcessType == 5)
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
                                    if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                    {
                                        subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();
                                        subItem.processType = int.Parse(resultSub["ProcessType"].ToString());
                                        subItem.orderKey = resultSub["orderKey"].ToString();
                                        subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());
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
                                        else
                                        {
                                            subItem.stateColor = Brushes.White;
                                            subItem.stateFontColor = Brushes.Black;
                                        }
                                        listitem.subOb.Add(subItem);
                                    }
                                }
                                listitem.subOb = listitem.subOb.OrderBy(x => x.processType).ToList();
                                workList.Add(listitem);
                            }
                        }
                        else
                        {
                            if (listitem.mercName.Equals(""))
                            {
                                if (listitem.ProcessType == Properties.Settings.Default.processType)
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

                                        if (i == 0)
                                        {
                                            listitem.item1 = item.Replace("\r\n", "");
                                        }
                                        else if (i == 1)
                                        {
                                            listitem.item2 = item.Replace("\r\n", "");
                                        }
                                        else if (i == 2)
                                        {
                                            listitem.item3 = item.Replace("\r\n", "");
                                        }
                                        else if (i == 3)
                                        {
                                            listitem.item4 = item.Replace("\r\n", "");
                                        }
                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {
                                        SubModel subItem = new SubModel();
                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {

                                            subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();
                                            subItem.processType = int.Parse(resultSub["ProcessType"].ToString());
                                            subItem.orderKey = resultSub["orderKey"].ToString();
                                            subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());
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
                                            else
                                            {
                                                subItem.stateColor = Brushes.White;
                                                subItem.stateFontColor = Brushes.Black;
                                            }

                                            listitem.subOb.Add(subItem);
                                        }
                                    }
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.processType).ToList();
                                    workList.Add(listitem);
                                }
                            }
                            else
                            {
                                if (listitem.mercName.Equals(Properties.Settings.Default.mercName))
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

                                        if (i == 0)
                                        {
                                            listitem.item1 = item.Replace("\r\n", "");
                                        }
                                        else if (i == 1)
                                        {
                                            listitem.item2 = item.Replace("\r\n", "");
                                        }
                                        else if (i == 2)
                                        {
                                            listitem.item3 = item.Replace("\r\n", "");
                                        }
                                        else if (i == 3)
                                        {
                                            listitem.item4 = item.Replace("\r\n", "");
                                        }
                                    }
                                    foreach (JObject resultSub in Data_Jarry)
                                    {

                                        SubModel subitem = new SubModel();
                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subitem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();
                                            subitem.orderKey = resultSub["orderKey"].ToString();
                                            subitem.processType = int.Parse(resultSub["ProcessType"].ToString());
                                            subitem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());
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
                                            else
                                            {
                                                subitem.stateColor = Brushes.White;
                                                subitem.stateFontColor = Brushes.Black;
                                            }
                                            listitem.subOb.Add(subitem);
                                        }
                                    }
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.processType).ToList();
                                    workList.Add(listitem);
                                }
                            }

                        }
                    }
                    workList1 = workList;
                    this.workList = workList;
                    ic.ItemsSource = workList;
                }
                else
                {
                    //MessageBox_Dialog box = new MessageBox_Dialog("PopWorkList 조회 오류!!");
                    //_ = box.ShowDialog();
                }
            }
        }

        public void ResultSearchDate()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //production = null;

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopResultSearchDate]",
                new string[] { "@pworkDate" },
                new string[] { DateTime.Now.ToString("yyyy-MM-dd") },
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

                                    workItem.sumPworkCount += float.Parse(result["pworkCount"].ToString());
                                    workItem.sDatetime = result["sDatetime"].ToString();

                                    if (!result["perrorCount"].ToString().Equals(""))
                                    {

                                        workItem.perrorCount += int.Parse(result["perrorCount"].ToString());

                                    }

                                    workItem.imagePath = "/img/bn_01.png";
                                    if (!result["eDatetime"].ToString().Equals(""))
                                    {
                                        workItem.eDatetime = result["eDatetime"].ToString();
                                    }
                                    else
                                    {
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

                    if (!Properties.Settings.Default.samework)
                    {
                        foreach (MainModel item in tempOb)
                        {
                            _ = workList.Remove(item);
                        }
                    }

                    tbNo.Text = workList.Count.ToString();

                    txtNoPlan.Visibility = workList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    //sw.Stop();
                    //Console.WriteLine(sw.ElapsedMilliseconds.ToString() + "ms");
                    SortWorkList();
                }
            }
            else
            {
                //MessageBox_Dialog box = new MessageBox_Dialog("PopResultSearchDate 조회 오류!!");
                //_ = box.ShowDialog();

            }


        }

        private void SortWorkList()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SortDescription sort = new SortDescription("sDatetime", ListSortDirection.Ascending);
            SortDescription sort1 = new SortDescription("pplanNo", ListSortDirection.Ascending);
            SortDescription sort2 = new SortDescription("pplanKey", ListSortDirection.Ascending);
            SortDescription sort3 = new SortDescription("urgency", ListSortDirection.Descending);
            SortDescription sort4 = new SortDescription("pAccount", ListSortDirection.Ascending);
            SortDescription sort5 = new SortDescription("WDIGCount", ListSortDirection.Descending);
            SortDescription sort6 = new SortDescription("icback", ListSortDirection.Ascending);
            bool sortCheck = Properties.Settings.Default.RadioChecked;
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(workList);

            if (sortCheck)
            {

                sort = new SortDescription("eDatetime", ListSortDirection.Ascending);
                collectionView.SortDescriptions.Add(sort);
                sort = new SortDescription("sDatetime", ListSortDirection.Ascending);

                collectionView.SortDescriptions.Add(sort);
                if (Properties.Settings.Default.corp_name.Equals("오주포장"))
                {
                    collectionView.SortDescriptions.Add(sort6);
                    collectionView.SortDescriptions.Add(sort5);

                }
                collectionView.SortDescriptions.Add(sort3);
                collectionView.SortDescriptions.Add(sort1);
                collectionView.SortDescriptions.Add(sort2);
                collectionView.SortDescriptions.Add(sort4);

                collectionView.Refresh();
            }

            else
            {
                collectionView.SortDescriptions.Add(sort);
                if (Properties.Settings.Default.corp_name.Equals("오주포장"))
                {
                    collectionView.SortDescriptions.Add(sort6);
                    collectionView.SortDescriptions.Add(sort5);

                }
                collectionView.SortDescriptions.Add(sort3);
                collectionView.SortDescriptions.Add(sort1);
                collectionView.SortDescriptions.Add(sort2);
                collectionView.SortDescriptions.Add(sort4);
                collectionView.Refresh();
            }

            List<MainModel> lists = collectionView.Cast<MainModel>().ToList();

            for (int i = 0; i < lists.Count; i++)
            {

                workList1[i] = lists[i];

            }
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds.ToString() + "ms");
        }
        private string ExtractKorean(string beforestr)
        {
            string str = Regex.Replace(beforestr, "#(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})", "");
            return str;
        }
        // 프로그램 종료 버튼 클릭
        private void Program_exit(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.consumersumcount = 0;
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업"))
            {
                //관리자 계정이 아닐때만 현황판으로 꺼짐 신호 보냄 
                if (!Properties.Settings.Default.manager.Equals("1"))
                {
                    invokeMercName(Properties.Settings.Default.mercName, "B");
                }
            }

            Environment.Exit(0);
        }

        private void invokeMercName(string mercName, string v)
        {

            //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
            SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);

        }
        // 상태 버튼 클릭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                can.Visibility = Visibility.Visible;
                Image img = (Image)sender;
                MainModel model = (MainModel)img.DataContext;
                Console.WriteLine("Image_MouseDown : " + model.bMember);

                if (model.bMember)
                {
                    Consumer consumer = new Consumer(model);
                    _ = consumer.ShowDialog();

                }

                else

                {
                    win_Input = new Win_input(model, workList, false);
                    _ = win_Input.ShowDialog();
                    if (Properties.Settings.Default.presultcount != 0)
                    {
                        model.presultCount = Properties.Settings.Default.presultcount;
                    }
                    Properties.Settings.Default.presultcount = 0;
                    Properties.Settings.Default.Save();
                }

                e.Handled = false;
                can.Visibility = Visibility.Collapsed;
                if (Properties.Settings.Default.corp_name.Equals("유창포장"))
                {
                    if (workList.Count < 50)
                    {
                        SelectWorkList();
                        ResultSearchDate();
                    }
                }



                win_Input = null;
                if (production != null)
                {
                    try
                    {
                        arduSerial.Write("B");

                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        Console.WriteLine("아두이노 에러->" + message);
                    }
                }
            }
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender.GetType().Name.Equals("Canvas"))
                {
                    Canvas canvas = (Canvas)sender;
                    MainModel context = (MainModel)canvas.DataContext;
                    can.Visibility = Visibility.Visible;
                    /* Consumer consumer = new Consumer(context);
                     consumer_ = consumer;
                     consumer.ShowDialog();
                     consumer_ = null;
                    */
                    string special = "";

                    for (int i = 0; i < workList.Count; i++)
                    {
                        if (context.orderKey.Equals(workList[i].orderKey))
                        {
                            special += workList[i].ProcessTypeName + " 참고 사항 :" + workList[i].ProcessMemo + "|";
                        }
                    }
                    Win_Detail win_detail = new Win_Detail(context, special, workList, Properties.Settings.Default.detail_check, CNS_Barcode_Send);
                    _ = win_detail.ShowDialog();
                    if (Properties.Settings.Default.corp_name.Equals("유창포장"))
                    {
                        if (workList.Count < 50)
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }
                    }
                    // selectWorkList();
                    // resultSearchDate();
                    win_Input = null;

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

        private void TbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            MainModel tempItem = null;

            string code = tbBarcode.Text;
            _ = int.TryParse(code, out int pCode);

            if (pCode > 0)
            {
                if (workList.Count(x => x.orderKey == pCode.ToString()) > 0)
                {
                    tempItem = workList.FirstOrDefault(x => x.orderKey == pCode.ToString());
                }
            }

            //foreach (MainModel item in workList)
            //{
            //    String tempStr = "";
            //    for (int i = 0; i < 4 - item.orderWorkNo.ToString().Length; i++)
            //    {
            //        tempStr += "0";
            //    }

            //    tempStr += item.orderWorkNo;

            //    String tempOrderKey = "";
            //    for (int i = 0; i < 10 - item.orderKey.ToString().Length; i++)
            //    {
            //        tempOrderKey += "0";
            //    }

            //    tempOrderKey += item.orderKey;

            //    if ((tempOrderKey + tempStr).Equals(code))
            //    {
            //        tempItem = item;
            //        break;
            //    }
            //    else if ((tempOrderKey + "0000").Equals(code))
            //    {
            //        tempItem = item;
            //        break;
            //    }
            //}
            if (tempItem != null)
            {
                string special = "";
                for (int i = 0; i < workList.Count; i++)
                {
                    if (tempItem.orderKey.Equals(workList[i].orderKey.ToString()))
                    {
                        special = special + workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                    }
                }
                tbBarcode.Text = "";
                can.Visibility = Visibility.Visible;
                Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, Properties.Settings.Default.detail_check, CNS_Barcode_Send);
                _ = win_Detail.ShowDialog();
                SelectWorkList();
                ResultSearchDate();


                win_Input = null;

                if (production != null)
                {
                    try
                    {
                        arduSerial.Write("B");
                    }

                    catch (Exception ex)
                    {
                        string exception = ex.Message;
                    }
                }
                e.Handled = false;
                can.Visibility = Visibility.Collapsed;
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
            //Properties.Settings.Default.Save();
            //Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Save();
            //_ = Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
        }

        private void Setting(object sender, MouseButtonEventArgs e)
        {
            can.Visibility = Visibility.Visible;
            Win_Setting win_Setting = new Win_Setting();
            _ = win_Setting.ShowDialog();
            _ = Properties.Settings.Default.settingitem;
            string metrialitem = Properties.Settings.Default.meterialinfo;
            Console.WriteLine("metrialitem -> " + metrialitem);
            SetSetting(metrialitem);
            ArduConn();

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

        private void SetSetting(string metrialitem)
        {
            switch (metrialitem)
            {
                case "19":
                    info.Text = "마닐라정보";
                    // div.Text = "자재종류";
                    //Canvas.SetLeft(info, 446);
                    break;

                case "20":
                    info.Text = "원단정보";
                    // Canvas.SetLeft(info, 446);
                    break;

                default:
                    //   Properties.Settings.Default.setting_check = 1;
                    // Properties.Settings.Default.Save();
                    // Application.Current.Shutdown();
                    //Process.GetCurrentProcess().Kill();
                    //var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
                    //Process.Start(currentExecutablePath);
                    arduSerial.Close();
                    timer.Stop();
                    MainWindow.main1 = null;

                    if (MainWindow.main == null)
                    {
                        MainWindow.main = new Main();
                    }
                    Content = MainWindow.main;
                    // System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    break;
            }
        }

        private void Change_workList(object sender, MouseButtonEventArgs e)
        {

            ChangeWorkList ch = new ChangeWorkList(workList1);
            _ = ch.ShowDialog();
            if (!Properties.Settings.Default.ChangeCheck)
            {
                SelectWorkList(); // 생산 계획 리스트 조회
                ResultSearchDate();
            }
            foreach (MainModel item in workList)
            {
                item.check = false;
                item.icback = "White";
            }
            Properties.Settings.Default.ChangeCheck = false;
            Properties.Settings.Default.index = 0;
            Properties.Settings.Default.Save();
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







