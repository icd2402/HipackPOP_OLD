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
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;

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
        public static Consumer consumer_;
        public static MainModel production = null;
        private int minuite = 0;
        private int minuite1 = 0;
        public static int str;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        public static bool passCheck = true;
        public Main()
        {
            try
            {
                InitializeComponent();
                production = null;
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
                //upload_setting();
                string merName = Properties.Settings.Default.mercName;
                tbMercName.Text = merName;
                try
                {
                    SignalRAdministrator conn = new SignalRAdministrator();
                    conn.Setconnection();
                }

                catch (Exception)
                {

                }

                SignalRAdministrator.허브.On<string, string>("returnCount", recive);

               
                    InvokeMercName(Properties.Settings.Default.mercName, "A");
                

                Properties.Settings.Default.consumersumcount = 0;
                Properties.Settings.Default.Save();
                corp_name.Text = Properties.Settings.Default.corp_name;
                string metrialitem = Properties.Settings.Default.meterialinfo;
                SetSetting(metrialitem);
                ArduConn();
                //ArduConn1();
                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                {
                    if (Properties.Settings.Default.CountType != 0)
                    {
                        work.Text = "박스수";
                        work.FontSize = 16;
                        box.Visibility = Visibility.Visible;
                        Canvas.SetTop(work, 1);
                        Canvas.SetTop(box, 20);
                    }

                }
                //setTbPintNo();
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
                bool connected = NetworkInterface.GetIsNetworkAvailable();
                if (connected)
                {
                    if (win_Input == null || robot_Setting == null)
                    {
                        SelectWorkList(); // 생산 계획 리스트 조회
                    }

                    ResultSearchDate(); // 생산 결과 리스트 조회
                    TimerSet();
                }

                else

                {
                    MessageBox_Dialog dialog = new MessageBox_Dialog("인터넷 상태를 확인해 주세요");
                    _ = dialog.ShowDialog();
                }
                //testSearch();
            }
            catch (Exception e)
            {
                LogWrite(e.Message);
                //MessageBox_Dialog msg = new MessageBox_Dialog(e.Message);
                //msg.Show();
            }
        }

        private void recive(string arg1, string arg2)
        {
            MainModel model = new MainModel();
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    switch (arg1)
                    {
                        case "인라인3호기":
                        case "인라인2호기":
                        case "후렉소1호기":

                            Console.WriteLine(arg1 + " : " + arg2);
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
                                    /* Consumer consumer = new Consumer(context);
                                     consumer_ = consumer;
                                     consumer.ShowDialog();
                                     consumer_ = null;
                                    */
                                    if (passCheck)
                                    {
                                        try
                                        {
                                            can.Visibility = Visibility.Visible;
                                            Win_Detail win_detail = new Win_Detail(model, special, workList, Properties.Settings.Default.detail_check);
                                            win_detail.ShowDialog();

                                            // selectWorkList();
                                            // resultSearchDate();
                                            win_Input = null;
                                            win_detail = null;
                                        }
                                        catch (Exception e1)
                                        {
                                            LogWrite(e1.StackTrace);
                                            LogWrite(e1.Message);
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



        private void serialDisConnect()
        {
            try
            {
                arduSerial.DataReceived -= ArduReReceive;
                arduSerial.DtrEnable = false;
                arduSerial.RtsEnable = false;
                arduSerial.ReadExisting(); // 시리얼 읽기, 비우기
                arduSerial.DiscardInBuffer();
                arduSerial.DiscardOutBuffer();
                arduSerial.Close(); // 시리얼 포트 닫기
            }
            catch { }
            finally{
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


        public  void ArduConn()
        {
            try
            {
                if (arduSerial.IsOpen)
                {
                    Thread serialClose;
                    serialClose = new System.Threading.Thread(new System.Threading.ThreadStart(serialDisConnect));
                    serialClose.IsBackground = true;
                    serialClose.Start(); // 스레드 시작

                    //serialDisConnect();
                }
                else
                {
                    arduSerial.Encoding = Encoding.GetEncoding(65001);
                    arduSerial.PortName = Properties.Settings.Default.SerialName;
                    arduSerial.BaudRate = Properties.Settings.Default.baudrate;
                    arduSerial.DataReceived += ArduReReceive;

                    arduSerial.Open();
                    arduSerial.Write(Properties.Settings.Default.SensorRate);
                    btrSerial.Background = Brushes.Green;
                }

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
                LogWrite("아두이노 연결 오류 : " + e.Message);
                //MessageBox_Dialog msg = new MessageBox_Dialog("아두이노 연결 오류 : " + e.Message);
                //msg.Show();
                btrSerial.Background = Brushes.Red;
                
            }
        }

        private void ArduConn1()
        {

            try
            {
                if (arduSerial1.IsOpen)
                {
                    Thread serialClose;
                    serialClose = new System.Threading.Thread(new System.Threading.ThreadStart(serialDisConnect));
                    serialClose.IsBackground = true;
                    serialClose.Start(); // 스레드 시작
                    //serialDisConnect();
                }
                arduSerial1.Encoding = Encoding.GetEncoding(65001);
                arduSerial1.PortName = Properties.Settings.Default.SerialName1;
                arduSerial1.BaudRate = Properties.Settings.Default.baudrate;
                arduSerial1.DataReceived += ArduReReceive1;

                arduSerial1.Open();
                arduSerial1.Write(Properties.Settings.Default.SensorRate);
                // btrSerial.Background = Brushes.Green;

            }
            catch (Exception e)
            {
                LogWrite("아두이노 연결 오류 : " + e.Message);
                //MessageBox_Dialog msg = new MessageBox_Dialog("아두이노 연결 오류 : " + e.Message);
                //msg.Show();
                btrSerial.Background = Brushes.Red;
            }
        }


        private void InvokeMercName(string mercName, string v)
        {
            //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
            try
            {
                SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);
            }
            catch (Exception)
            {

            }

        }
        private void LogWrite(string str)
        {
            string DirPath = Environment.CurrentDirectory + @"\Log";
            string FilePath = DirPath + "\\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
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
        private void ArduReReceive1(object sender, SerialDataReceivedEventArgs e)
        {
            float pCnt;
            string ArdumercName;

            switch (Properties.Settings.Default.corp_id)
            {
                case "icdtest2":
                    string[] arry = arduSerial1.ReadLine().Split('^');
                    ArdumercName = arry[0].ToString().Trim();

                    if (arry.Length == 2)
                    {
                        str = int.Parse(arry[1].Trim());
                        LogWrite(str.ToString());
                    }
                    else
                    {
                        return;
                    }
                    if (production != null)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            pCnt = production != null ? Properties.Settings.Default.twoSum ? production.pCnt * 2 : production.pCnt : 1;


                            if (!Properties.Settings.Default.saveDate.Equals("") && !Properties.Settings.Default.saveDate.Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                if (production.pplanKey == Properties.Settings.Default.pplanKey)
                                {
                                    str = Properties.Settings.Default.count + str;
                                }
                            }

                            if (Properties.Settings.Default.CountType == 0)
                            {
                                production.sumPworkCount = str;
                                production.presultCount = str;
                                production.presultCount1 = str;
                                if (win_Input != null)
                                {
                                    win_Input.inputModel.count = str;
                                        // Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                                    }
                                InvokeCount(str, ArdumercName);
                            }

                            else
                            {
                                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                                {
                                    pCnt = Properties.Settings.Default.twoSum ? (production.pCnt * 2) : 1;
                                }


                                production.sumPworkCount = Convert.ToInt32(Math.Floor(str * pCnt));
                                if (Properties.Settings.Default.boxCheck)
                                {
                                    if (pCnt >= 1)
                                    {
                                        production.sumPworkCount /= (float)pCnt;
                                    }
                                }
                                production.presultCount = production.sumPworkCount;
                                production.presultCount1 = production.sumPworkCount;
                                if (win_Input != null)
                                {
                                    win_Input.inputModel.count = production.presultCount;
                                        // Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                                    }
                                InvokeCount(Convert.ToInt32(production.presultCount), ArdumercName);
                            }
                        }));


                        //  int lastcount = str;
                        //  double 분당생산수 = str - lastcount;
                    }


                    break;

            }

            //Console.WriteLine("aa" + arduSerial.ReadLine().Trim());
            // Console.WriteLine(arduSerial.ReadLine().Trim());


        }

        public void ArduReReceive(object sender, SerialDataReceivedEventArgs e)
        {

            //Console.WriteLine(arduSerial.ReadLine().Trim());
            //str = int.Parse(arduSerial.ReadLine().Trim());
            float pCnt;
            int bytesize;
            string ArdumercName;
           
                switch (Properties.Settings.Default.corp_id)
                {
                    case "icdtest2":
                        string[] arry = arduSerial.ReadLine().Split('^');
                        ArdumercName = arry[0].ToString().Trim();
                        if (arry.Length == 2)
                        {
                            str = int.Parse(arry[1].Trim());
                            LogWrite(str.ToString());

                        }
                        else
                        {
                            return;
                        }

                        if (production != null)
                        {

                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                if (production != null)
                                {
                                    pCnt = Properties.Settings.Default.twoSum ? production.pCnt * 2 : production.pCnt;

                                }
                                else
                                {
                                    pCnt = 1;
                                }

                                if (!Properties.Settings.Default.saveDate.Equals("") && !Properties.Settings.Default.saveDate.Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                                {
                                    if (production.pplanKey == Properties.Settings.Default.pplanKey)
                                    {
                                        str = Properties.Settings.Default.count + str;
                                    }
                                }

                                if (Properties.Settings.Default.CountType == 0)
                                {
                                    production.sumPworkCount = str;
                                    production.presultCount = str;
                                    production.presultCount1 = str;
                                    if (win_Input != null)
                                    {
                                        win_Input.inputModel.count = str;
                                        // Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                                    }
                                    InvokeCount(str, ArdumercName);
                                }
                                else
                                {
                                    if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("성주포장") || Properties.Settings.Default.corp_name.Equals("대세물산"))
                                    {
                                        pCnt = Properties.Settings.Default.twoSum ? (production.pCnt * 2) : 1;
                                    }

                                    production.sumPworkCount = Convert.ToInt32(Math.Floor(str * pCnt));
                                    if (Properties.Settings.Default.boxCheck)
                                    {
                                        if (pCnt >= 1)
                                        {
                                            production.sumPworkCount /= (float)pCnt;
                                        }
                                    }
                                    production.presultCount = production.sumPworkCount;
                                    production.presultCount1 = production.sumPworkCount;
                                    if (win_Input != null)
                                    {
                                        win_Input.inputModel.count = production.presultCount;
                                        // Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                                    }
                                    InvokeCount(Convert.ToInt32(production.presultCount), ArdumercName);
                                }
                            }));



                            //  int lastcount = str;
                            //  double 분당생산수 = str - lastcount;
                        }


                        break;

                    default:
                        try
                        {

                        //bytesize = arduSerial.BytesToRead;

                        //if (bytesize == -1) return;
                        //byte[] data = new byte[bytesize];
                        //arduSerial.Read(data, 0, bytesize);
                        //string msg = Encoding.Default.GetString(data);
                        //str = int.Parse(msg);
                        //LogWrite(str.ToString());
                        //Console.WriteLine("값 : " + str);
                        str = int.Parse(arduSerial.ReadLine().Trim());
                        Console.WriteLine("값 : " + str);
                        //LogWrite(str.ToString());
                        if (production != null)
                            {
                                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                                {
                                    if (production != null)
                                    {
                                        pCnt = Properties.Settings.Default.twoSum ? production.pCnt * 2 : production.pCnt;

                                    }
                                    else
                                    {
                                        pCnt = 1;
                                    }

                                    if (!Properties.Settings.Default.saveDate.Equals("") && !Properties.Settings.Default.saveDate.Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                                    {
                                        if (production.pplanKey == Properties.Settings.Default.pplanKey)
                                        {
                                            str = Properties.Settings.Default.count + str;
                                        }
                                    }

                                    if (Properties.Settings.Default.CountType == 0)
                                    {
                                        production.sumPworkCount = str;
                                        production.presultCount = str;
                                        production.presultCount1 = str;
                                        if (win_Input != null)
                                        {
                                            win_Input.inputModel.count = str;
                                            // Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                                        }
                                        InvokeCount(str, Properties.Settings.Default.mercName);
                                    }

                                    else
                                    {
                                        if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                                        {
                                            pCnt = Properties.Settings.Default.twoSum ? (production.pCnt * 2) : 1;
                                        }


                                        if(str != 0)
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
                                        production.presultCount1 = production.sumPworkCount;
                                        if (win_Input != null)
                                        {
                                            win_Input.inputModel.count = production.presultCount;
                                            // Console.WriteLine("win_Input.inputModel.count : " + win_Input.inputModel.count);
                                        }
                                        InvokeCount(Convert.ToInt32(production.presultCount), Properties.Settings.Default.mercName);
                                        }
                                    }
                                }));



                                //  int lastcount = str;
                                //  double 분당생산수 = str - lastcount;
                            }
                        }


                        catch (Exception e1)
                        {
                            //MessageBoxResult result = System.Windows.MessageBox.Show(e1.Message, "작업 저장",
                            //     MessageBoxButton.YesNo);
                            //MessageBox_Dialog msg = new MessageBox_Dialog("아두이노 리시브 오류 : " + e1.Message);
                            //msg.Show();
                        }

                        break;
                }
           
            //Console.WriteLine("aa" + arduSerial.ReadLine().Trim());
            // Console.WriteLine(arduSerial.ReadLine().Trim());



        }

        private void CountReflash(string v)
        {
            throw new NotImplementedException();
        }

        // 현황판으로 카운터 전송

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
            //           Console.Write(dt.Minute);

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
            if (minuite1 == 60)
            {
               // ArduConn();
                bool connected = NetworkInterface.GetIsNetworkAvailable();
                if (connected)
                {
                   
                        InvokeMercName(Properties.Settings.Default.mercName, "A");
                    
                }
                minuite1 = 0;
            }

            if (minuite == 300)
            {
                bool connected = NetworkInterface.GetIsNetworkAvailable();
                // Console.WriteLine("네트워크 연결 체크 : " + connected);
                if (!arduSerial.IsOpen)
                {
                    arduSerial.DataReceived -= ArduReReceive;

                    ArduConn();


                }
                //if (!arduSerial1.IsOpen)
                //{
                //    //arduSerial1.DataReceived -= ArduReReceive1;

                //    //ArduConn1();


                //}
                if (connected)
                {


                    if (production != null)
                    {
                        InvokeCount(Convert.ToInt32(production.presultCount), Properties.Settings.Default.mercName);
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
        // 현황판으로 카운터 전송
        public void InvokeCount(int str, string mercName)
        {
            try
            {
                _ = SignalRAdministrator.허브.Invoke("invokeCount", Properties.Settings.Default.corp_id, mercName, str + "");
            }

            catch (Exception)
            {

            }
        }

        private void Serial_click(object sender, RoutedEventArgs e)
        {

            /*
            e.Handled = true;
            string[] portName = SerialPort.GetPortNames();
            SerialList.SerialList serial = new SerialList.SerialList(portName);
            serial.ShowDialog();
            ArduConn();
            can.Visibility = Visibility.Collapsed; 
            */

        }

        private void Remote_click(object sender, MouseButtonEventArgs e)
        {
            //Process.Start("C:\\Windows\\notepad.exe");
            _ = Process.Start("http://onefax.co.kr/w_onefax/remotebox_view.aspx");
        }

        private void TxtDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

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
                        InvokeMercName(before, "B");
                        InvokeMercName(Properties.Settings.Default.mercName, "A");
                        Properties.Settings.Default.error = "";
                        Properties.Settings.Default.error1 = "";
                        Properties.Settings.Default.Save();
                    }

                    else
                    {
                        InvokeMercName(Properties.Settings.Default.mercName, "A");
                    }
                
                //setTbPintNo();
               
                    if (Properties.Settings.Default.CountType != 0)
                    {
                        work.Text = "박스수";
                        work.FontSize = 16;
                        box.Visibility = Visibility.Visible;
                        Canvas.SetTop(work, 1);
                        Canvas.SetTop(box, 20);
                    }
                    else
                    {
                        work.Text = "계획수";
                        box.Visibility = Visibility.Collapsed;
                        Canvas.SetTop(work, 10);
                    }

                
                SelectWorkList();
                ResultSearchDate();

            }

        }

        public void SelectWorkList()
        {
            wdIGCOunt = 0;
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
                    workList = new ObservableCollection<MainModel>();
                    workList1 = new ObservableCollection<MainModel>();
                    foreach (JObject result in Data_Jarry)
                    {
                        double pplanNo = double.Parse(result["pplanNo"].ToString()) == 0 ? double.MaxValue : double.Parse(result["pplanNo"].ToString());
                        newProd = result["newProd"].Equals("1") ? Visibility.Visible : Visibility.Hidden;
                        MainModel listitem = new MainModel
                        {
                            pCnt = float.Parse(result["pCnt"].ToString()),
                            pplanNo = pplanNo
                        };
                        if (result["boxType"].ToString() != null)
                        {
                            listitem.boxType = result["boxType"].ToString();
                        }

                        if (result["PalletHeight"].ToString() != null)
                        {
                            listitem.PalletHeight = result["PalletHeight"].ToString();
                        }

                        if (result["PalletCount"].ToString() != null)
                        {
                            listitem.PalletCount = result["PalletCount"].ToString();
                        }

                        if (result["givWD"].ToString() != null)
                        {
                            listitem.givWD = result["givWD"].ToString();
                        }

                        if (result["givSC"].ToString() != null)
                        {
                            listitem.givSC = result["givSC"].ToString();
                        }

                        switch (Properties.Settings.Default.corp_name)
                        {
                            case "오주포장":
                            case "대양포장":
                                div.Text = "[수주날짜][납품일자] 거래처/품명";
                                Canvas.SetLeft(div, 106);
                                listitem.visibility = Visibility.Visible;
                                listitem.visibility1 = Visibility.Visible;
                                listitem.left = 250;
                                break;

                            case "신우수출포장":
                            case "신우포장테스트":
                                div.Text = "[수주날짜] 거래처/품명";
                                Canvas.SetLeft(div, 133);
                                listitem.visibility = Visibility.Visible;
                                listitem.visibility1 = Visibility.Collapsed;
                                listitem.left = 160;
                                break;

                            default:
                                listitem.visibility = Visibility.Collapsed;
                                listitem.visibility1 = Visibility.Collapsed;
                                listitem.left = 70;
                                div.Text = "거래처/품명";
                                Canvas.SetLeft(div, 160);
                                break;
                        }

                        // Console.WriteLine("meterialinfo --> " + Properties.Settings.Default.meterialinfo);
                        if (Properties.Settings.Default.meterialinfo.Equals(""))
                        {
                            info.Text = "장 * 폭 * 고";
                            listitem.SCBJCount = result["WDScore"].ToString();
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
                            listitem.SCIGCount = "";
                            listitem.SCmetrialName = "";
                            listitem.mLmW = result["pSize"].ToString();
                        }

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
                        if (listitem.givWD.Equals("2"))
                        {
                            listitem.WDBJCount = "대체량   " + string.Format("{0:n0}", result["WDBJCount"]);
                            listitem.WDIGCount = "";
                        }

                        else
                        {
                            listitem.WDBJCount = "발주   " + string.Format("{0:n0}", result["WDBJCount"]);
                            //int a = int.Parse(result["WDIGCount"].ToString());
                            //if (a == 0) {

                            //    listitem.WDIGCount = result["metCustomSName"].ToString();
                            //}
                            //else
                            //{
                            //    listitem.WDIGCount = "입고   " + string.Format("{0:n0}", result["WDIGCount"]);
                            //}
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
                        listitem.PackCount = result["PackCount"].ToString();
                        listitem.pplanCount = int.Parse(result["pplanCount"].ToString());

                        if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("우리포장산업테스트") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("누리팩테스트"))
                        {
                            ss.Visibility = Visibility.Visible; //집계창 표시
                        }
                        else
                        {
                            Grid.SetRowSpan(workListBorder, 2); //WorkList Grid.Row3까지 확장 
                            ss.Visibility = Visibility.Collapsed; //집계창 표시해제
                        }
                         
                        if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                        {
                            if (Properties.Settings.Default.CountType == 0)
                            {
                                listitem.boxvisibility = Visibility.Collapsed;
                            }
                            else
                            {
                                listitem.pplanboxCount = (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                            }
                        }

                        else
                        {
                            listitem.boxvisibility = Visibility.Collapsed;
                        }

                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.pplanCount = Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("혜성수출포장") || Properties.Settings.Default.corp_name.Equals("신우포장테스트") || Properties.Settings.Default.corp_name.Equals("성주포장")
                                ? int.Parse(result["pplanCount"].ToString())
                                : (float)Math.Ceiling(listitem.pplanCount / (float)listitem.pCnt);
                        }


                        listitem.presultCount = float.Parse(result["presultCount"].ToString());
                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.presultCount /= listitem.pCnt;
                            listitem.presultCount1 /= listitem.pCnt;
                        }
                        else
                        {
                            if (Properties.Settings.Default.CountType == 1)
                            {
                                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("성주포장"))
                                {
                                    listitem.presultCount = float.Parse(result["presultCount"].ToString()) / listitem.pCnt;
                                    listitem.presultCount1 = float.Parse(result["presultCount"].ToString()) / listitem.pCnt;
                                }
                                else
                                {
                                    listitem.presultCount = float.Parse(result["presultCount"].ToString());
                                    listitem.presultCount1 = float.Parse(result["presultCount"].ToString());
                                }

                            }
                            else
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


                        if (!Properties.Settings.Default.corp_name.Equals("오주포장") && !Properties.Settings.Default.corp_name.Equals("대양포장") && !Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                        {
                            listitem.icback = listitem.bMember ? "#AFE1FF" : "White";
                            listitem.pStateVisi = Visibility.Collapsed;
                        }

                        else
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
                        if (Properties.Settings.Default.mercName.Equals("인라인2호기") || Properties.Settings.Default.mercName.Equals("인라인3호기"))
                        {
                            if (listitem.ProcessType == 8)
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
                                        case "14":
                                            item = result["workMemo"].ToString();
                                            break;
                                        case "15":
                                            item = result["pplanMemo"].ToString();
                                            break;
                                        case "16":
                                            item = result["ProcessMemo"].ToString();
                                            break;
                                        case "17":
                                            item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                                ? "-"
                                                : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                            break;
                                        case "18":
                                            item = result["PalletType"].ToString();
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
                                }

                                foreach (JObject resultSub in Data_Jarry)
                                {
                                    SubModel subItem = new SubModel();

                                    if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                    {
                                        if (resultSub["ProcessTypeName"].ToString().Equals("톰슨"))
                                        {
                                            continue;
                                        }
                                        subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();
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
                                listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                if (listitem.subOb.Count == 2)
                                {

                                    workList.Add(listitem);
                                }

                            }
                        }

                        else if (Properties.Settings.Default.mercName.Equals("풀접합") && Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (listitem.ProcessType == 8 || listitem.ProcessType == 7 || listitem.ProcessType == 5)
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

                                        case "14":
                                            item = result["workMemo"].ToString();
                                            break;

                                        case "15":
                                            item = result["pplanMemo"].ToString();
                                            break;
                                        case "16":
                                            item = result["ProcessMemo"].ToString();
                                            break;
                                        case "17":
                                            item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                               ? "-"
                                               : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                            break;
                                        case "18":
                                            item = result["PalletType"].ToString();
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

                                    if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                    {
                                        subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();
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
                                listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                workList.Add(listitem);
                            }
                        }
                        else if (Properties.Settings.Default.mercName.Equals("베가") && Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (listitem.ProcessType == 8)
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
                                        case "14":
                                            item = result["workMemo"].ToString();
                                            break;
                                        case "15":
                                            item = result["pplanMemo"].ToString();
                                            break;
                                        case "16":
                                            item = result["ProcessMemo"].ToString();
                                            break;
                                        case "17":
                                            item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                               ? "-"
                                               : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                            break;
                                        case "18":
                                            item = result["PalletType"].ToString();
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

                                    if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                    {
                                        subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();
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

                                listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                workList.Add(listitem);
                                //if (listitem.subOb.Count == 2)
                                //{
                                //    workList.Add(listitem);


                                //}

                            }

                        }
                        //else if (Properties.Settings.Default.mercName.Equals("1호기(오프라인)") && Properties.Settings.Default.corp_name.Equals("구룡산업제2공장"))
                        //{
                        //    if (listitem.ProcessType == 15)
                        //    {
                        //        string[] setting = Properties.Settings.Default.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                        //        for (int i = 0; i < setting.Length; i++)
                        //        {
                        //            string item = "";
                        //            switch (setting[i])
                        //            {
                        //                case "1":
                        //                    item = result["filmNo"].ToString();
                        //                    break;
                        //                case "2":
                        //                    item = result["CoatingType"].ToString();
                        //                    break;
                        //                case "3":
                        //                    item = result["PrintNo"].ToString();
                        //                    break;

                        //                case "4":
                        //                    item = result["Printposition"].ToString();
                        //                    break;
                        //                case "5":
                        //                    item = result["PushingNo"].ToString();
                        //                    break;
                        //                case "6":
                        //                    item = result["Pushingposition"].ToString();
                        //                    break;
                        //                case "7":
                        //                    item = Properties.Settings.Default.color_check ? result["ColorType"].ToString().Replace("|", "").Replace("색", "색 ") : ExtractKorean(result["ColorType"].ToString().Replace("|", "")).Replace("색", "색 ");
                        //                    break;
                        //                case "8":
                        //                    item = result["ConnetingType"].ToString();
                        //                    break;

                        //                case "9":
                        //                    item = result["handChk"].ToString();
                        //                    break;

                        //                case "10":
                        //                    item = result["PackCount"].ToString();
                        //                    break;

                        //                case "11":
                        //                    item = result["PackType"].ToString();
                        //                    break;
                        //                case "14":
                        //                    item = result["workMemo"].ToString();
                        //                    break;
                        //                case "15":
                        //                    item = result["pplanMemo"].ToString();
                        //                    break;
                        //                case "16":
                        //                    item = result["ProcessMemo"].ToString();
                        //                    break;
                        //                case "17":
                        //                    item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                        //                       ? "-"
                        //                       : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                        //                    break;
                        //                case "18":
                        //                    item = result["PalletType"].ToString();
                        //                    break;

                        //                default:
                        //                    break;
                        //            }

                        //            if (i == 0)
                        //            {
                        //                listitem.item1 = item.Replace("\r\n", "");
                        //            }

                        //            else if (i == 1)
                        //            {
                        //                listitem.item2 = item.Replace("\r\n", "");
                        //            }

                        //            else if (i == 2)
                        //            {
                        //                listitem.item3 = item.Replace("\r\n", "");
                        //            }
                        //        }
                        //        foreach (JObject resultSub in Data_Jarry)
                        //        {
                        //            SubModel subItem = new SubModel();
                        //            if (listitem.orderKey.Equals(resultSub["orderKey"].ToString()))
                        //            {
                        //                if (listitem.pplanKey.Equals(resultSub["pplanKey"].ToString()))
                        //                {

                        //                }
                        //            }

                        //            if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                        //            {
                        //                subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();
                        //                subItem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();
                        //                subItem.processType = int.Parse(resultSub["ProcessType"].ToString());
                        //                subItem.orderKey = resultSub["orderKey"].ToString();
                        //                subItem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());
                        //                if (int.Parse(resultSub["presultCount"].ToString()) > 0)
                        //                {
                        //                    subItem.stateColor = Brushes.Black;
                        //                    subItem.stateFontColor = Brushes.White;
                        //                    subItem.presultCount = int.Parse(resultSub["presultCount"].ToString());

                        //                    if (!resultSub["perrCount"].ToString().Equals(""))
                        //                    {
                        //                        subItem.presultCount -= int.Parse(resultSub["perrCount"].ToString());
                        //                    }

                        //                }

                        //                else
                        //                {
                        //                    subItem.stateColor = Brushes.White;
                        //                    subItem.stateFontColor = Brushes.Black;
                        //                }
                        //                listitem.subOb.Add(subItem);
                        //            }
                        //        }

                        //        listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                        //        workList.Add(listitem);
                        //    }
                        //}
                        else if (Properties.Settings.Default.mercName.Equals("2공장인쇄기") && Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (listitem.mercName.Equals("2공장인쇄기"))
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
                                            case "14":
                                                item = result["workMemo"].ToString();
                                                break;
                                            case "15":
                                                item = result["pplanMemo"].ToString();
                                                break;
                                            case "16":
                                                item = result["ProcessMemo"].ToString();
                                                break;
                                            case "17":
                                                item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                                ? "-"
                                                : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                                break;
                                            case "18":
                                                item = result["PalletType"].ToString();
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
                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {
                                        SubModel subItem = new SubModel();
                                        //Console.WriteLine(resultSub["pplanKey"].ToString());
                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();
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
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                    workList.Add(listitem);
                                }
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
                                            case "14":
                                                item = result["workMemo"].ToString();
                                                break;
                                            case "15":
                                                item = result["pplanMemo"].ToString();
                                                break;
                                            case "16":
                                                item = result["ProcessMemo"].ToString();
                                                break;
                                            case "17":
                                                item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                                ? "-"
                                                : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                                break;
                                            case "18":
                                                item = result["PalletType"].ToString();
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
                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {
                                        SubModel subItem = new SubModel();
                                        //Console.WriteLine(resultSub["pplanKey"].ToString());
                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subItem.ProcessSeq = resultSub["ProcessSeq"].ToString();
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
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
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
                                            case "14":
                                                item = result["workMemo"].ToString();
                                                break;

                                            case "15":
                                                item = result["pplanMemo"].ToString();
                                                break;
                                            case "16":
                                                item = result["ProcessMemo"].ToString();
                                                break;
                                            case "17":
                                                item = result["orderLast"].ToString().Substring(0, 4).Equals("9999")
                                               ? "-"
                                               : "납기 : " + result["orderLast"].ToString().Replace("-", "/").Substring(2, 8);
                                                break;
                                            case "18":
                                                item = result["PalletType"].ToString();
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
                                    }

                                    foreach (JObject resultSub in Data_Jarry)
                                    {

                                        SubModel subitem = new SubModel();

                                        if ((listitem.orderKey + listitem.orderWorkNo).Equals(resultSub["orderKey"].ToString() + resultSub["orderWorkNo"].ToString()))
                                        {
                                            subitem.ProcessSeq = resultSub["ProcessSeq"].ToString();
                                            subitem.ProcessTypeName = resultSub["ProcessTypeName"].ToString();
                                            subitem.processType = int.Parse(resultSub["ProcessType"].ToString());
                                            subitem.pplanKey = int.Parse(resultSub["pplanKey"].ToString());
                                            subitem.orderKey = resultSub["orderKey"].ToString();

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
                                    listitem.subOb = listitem.subOb.OrderBy(x => x.ProcessSeq).ToList();
                                    workList.Add(listitem);
                                }
                            }

                        }
                    }
                    workList1 = workList;
                    //this.workList = workList;


                    ic.ItemsSource = workList;
                }
                else
                {
                    //MessageBox_Dialog box = new MessageBox_Dialog("PopWorkList 조회 오류!!");
                    //_ = box.ShowDialog();
                }
            }

            실적현황조회();
        }

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
        }

        public void ResultSearchDate()
        {
            production = null;
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
                                    workItem.presultCount = float.Parse(result["pworkCount"].ToString());
                                    workItem.sumPworkCount += float.Parse(result["pworkCount"].ToString());
                                    //if (Properties.Settings.Default.boxCheck)
                                    //{
                                    //    workItem.sumPworkCount = workItem.sumPworkCount / workItem.pCnt;
                                    //}
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

                    if (!Properties.Settings.Default.samework)
                    {
                        foreach (MainModel item in tempOb)
                        {
                            _ = workList.Remove(item);

                        }
                    }

                    txtNoPlan.Visibility = workList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    SortWorkList();

                }
            }

            else
            {
                //MessageBox_Dialog box = new MessageBox_Dialog("PopResultSearchDate 조회 오류!!");
                //_ = box.ShowDialog();
            }

            실적현황조회();
        }

        private void SortedList()
        {
            SortDescription sort = new SortDescription("customSName", ListSortDirection.Ascending);
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(workList);

            if (!sortChecked)
            {
                collectionView.Filter = null;
                collectionView.SortDescriptions.Clear();
                collectionView.SortDescriptions.Add(sort);
                collectionView.Refresh();
            }
            else
            {
                SortWorkList();
            }
        }

        private void SortWorkList()
        {

            if (sortChecked)
            {


                SortDescription sort = new SortDescription("sDatetime", ListSortDirection.Ascending);
                SortDescription sort1 = new SortDescription("pplanNo", ListSortDirection.Ascending);
                SortDescription sort2 = new SortDescription("pplanKey", ListSortDirection.Ascending);
                SortDescription sort3 = new SortDescription("urgency", ListSortDirection.Descending);
                SortDescription sort4 = new SortDescription("pAccount", ListSortDirection.Ascending);
                _ = new SortDescription("WDIGCount", ListSortDirection.Descending);
                SortDescription sort6 = new SortDescription("icback", ListSortDirection.Ascending);
                SortDescription sort7 = new SortDescription("orderDate", ListSortDirection.Ascending);
                SortDescription sort8 = new SortDescription("pState", ListSortDirection.Descending);
                bool sortCheck = Properties.Settings.Default.RadioChecked;

                ICollectionView collectionView = CollectionViewSource.GetDefaultView(workList);

                collectionView.Filter = null;
                collectionView.SortDescriptions.Clear();

                if (sortCheck)
                {

                    sort = new SortDescription("eDatetime", ListSortDirection.Ascending);
                    collectionView.SortDescriptions.Add(sort);
                    sort = new SortDescription("sDatetime", ListSortDirection.Ascending);
                    collectionView.SortDescriptions.Add(sort);
                    collectionView.SortDescriptions.Add(sort8);
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

                    else
                    {
                        collectionView.SortDescriptions.Add(sort3);
                        collectionView.SortDescriptions.Add(sort1);
                        collectionView.SortDescriptions.Add(sort2);
                        collectionView.SortDescriptions.Add(sort4);
                    }
                    collectionView.Refresh();

                }

                else
                {

                    collectionView.SortDescriptions.Add(sort);
                    collectionView.SortDescriptions.Add(sort8);
                    if (Properties.Settings.Default.corp_name.Equals("오주포장") || Properties.Settings.Default.corp_name.Equals("대양포장") || Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                    {

                        collectionView.SortDescriptions.Add(sort3);
                        collectionView.SortDescriptions.Add(sort6);

                        collectionView.SortDescriptions.Add(sort1);
                        collectionView.SortDescriptions.Add(sort7);
                        collectionView.SortDescriptions.Add(sort2);
                        collectionView.SortDescriptions.Add(sort4);

                    }

                    else
                    {
                        collectionView.SortDescriptions.Add(sort3);
                        collectionView.SortDescriptions.Add(sort1);
                        collectionView.SortDescriptions.Add(sort2);
                        collectionView.SortDescriptions.Add(sort4);
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
           
             InvokeMercName(Properties.Settings.Default.mercName, "B");
            
            Environment.Exit(0);




        }

        // 상태 버튼 클릭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {


                if (sender.GetType().Name.Equals("Image"))
                {
                    can.Visibility = Visibility.Visible;
                    Image img = (Image)sender;
                    MainModel model = (MainModel)img.DataContext;

                    if (model.bMember)
                    {
                        if(Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
                        {

                            if (model.imagePath.Equals("/img/bn_02.png"))
                            {
                                model.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                PopResultStart(model);

                            }
                            else if (model.imagePath.Equals("/img/bn_03.png"))
                            {
                                model.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                PopResultStart(model);
                            }else if (model.imagePath.Equals("/img/bn_01.png"))
                            {
                                model.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            e.Handled = false;
                            DSConsumer dSConsumer = new DSConsumer(model);
                            dSConsumer.ShowDialog();
                            InvokeCount(Properties.Settings.Default.mercName, "E");
                            SelectWorkList();
                            ResultSearchDate();
                        }
                        else
                        {
                            Consumer consumer = new Consumer(model);
                            _ = consumer.ShowDialog();
                            SelectWorkList();
                            ResultSearchDate();

                        }

                    }

                    else
                    {
                        win_Input = new Win_input(model, workList, false);
                        _ = win_Input.ShowDialog();
                        if (Properties.Settings.Default.presultcount != 0)
                        {
                            model.presultCount = Properties.Settings.Default.presultcount;
                        }
                        if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
                        {
                            SelectWorkList();
                            ResultSearchDate();
                        }


                        Properties.Settings.Default.presultcount = 0;
                        Properties.Settings.Default.Save();
                        // model.presultCount = 11;
                        // model.presultCount = 
                        // ResultSearchDate();

                    }

                    // ArduConn();
                    if (Properties.Settings.Default.corp_name.Equals("유창포장") || Properties.Settings.Default.corp_name.Equals("유창포장테스트"))
                    {
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
                //MessageBox_Dialog msg = new MessageBox_Dialog(e1.Message);
                //msg.ShowDialog();
            }
        }
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
                if (model.imagePath.Equals("/img/bn_03.png"))
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

                //switch (metrialitem)
                //{
                //    case "12":
                //    case "13":
                //        Main1.production = mainModel;
                //        try
                //        {

                //            if (Main1.production != null)
                //            {
                //                Main1.arduSerial.Write("A");
                //                Main1.arduSerial.Write("B");
                //            }

                //            else
                //            {
                //                Main1.arduSerial.Write("A");
                //            }
                //        }
                //        catch (Exception)
                //        {
                //            //MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                //            //msg.ShowDialog();
                //        }
                //        break;

                //    default:

                //        Main.production = mainModel;

                //        try
                //        {
                //            if (Main.production != null)
                //            {
                //                Main.arduSerial.Write("A");
                //                Main.arduSerial.Write("B");
                //            }

                //            else
                //            {
                //                Main.arduSerial.Write("A");
                //            }
                //        }

                //        catch (Exception)
                //        {
                //            // MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                //            // msg.ShowDialog();
                //        }

                //        break;

                //}


            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;
        }
        private void InvokeCount(string mercName, string v)
        {
            //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
            try
            {
                SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);

            }
            catch (Exception e)
            {

            }
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (Properties.Settings.Default.detail_check)
            {
                if (e.ClickCount == 2)
                {
                    ShowDetail(sender);
                }
            }
            else
            {
                if (e.ClickCount == 2)
                {
                    ShowDetail(sender);

                }
            }

        }

        private void ShowDetail(object sender)
        {
            try
            {

               
                if (!sender.GetType().Name.Equals("Image"))
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
                        if (context.orderKey.Equals(workList[i].orderKey.ToString()))
                        {
                            special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                        }
                    }
                    try
                    {
                        Win_Detail win_detail = new Win_Detail(context, special, workList, Properties.Settings.Default.detail_check);
                        _ = win_detail.ShowDialog();
                        if (Properties.Settings.Default.corp_name.Equals("유창포장"))
                        {
                            if (workList.Count < 30)
                            {
                                SelectWorkList();
                                ResultSearchDate();
                            }
                        }
                        // selectWorkList();
                        // resultSearchDate();
                        win_Input = null;
                        win_detail = null;
                    }
                    catch (Exception e1)
                    {
                        LogWrite(e1.Message);
                        LogWrite(e1.StackTrace);
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
                LogWrite(e.Message);
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

            //    foreach (MainModel item in workList)
            //    {
            //        St0000064751
            //        ring tempStr = "";
            //        for (int i = 0; i < 4 - item.orderWorkNo.ToString().Length; i++)
            //        {
            //            tempStr += "0";

            //        }

            //        tempStr += item.orderWorkNo;

            //        String tempOrderKey = "";
            //        for (int i = 0; i < 10 - item.orderKey.ToString().Length; i++)
            //        {
            //            tempOrderKey += "0";
            //        }


            //    tempOrderKey += item.orderKey;

            //        if ((tempOrderKey + tempStr).Equals(code))
            //        {
            //            tempItem = item;
            //            break;
            //        }

            //        else if ((tempOrderKey + "0000").Equals(code))
            //        {
            //            tempItem = item;
            //            break;
            //        }

            //}

            if (tempItem != null)
            {
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
                    Win_Detail win_Detail = new Win_Detail(tempItem, special, workList, Properties.Settings.Default.detail_check);
                    _ = win_Detail.ShowDialog();
                    //  selectWorkList();
                    //  resultSearchDate();
                    win_Input = null;
                    win_Detail = null;
                }
                catch (Exception e1)
                {
                    LogWrite(e1.Message);
                    LogWrite(e1.StackTrace);
                }


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

        private void Logout_click(object sender, RoutedEventArgs e)
        {

            setLock();

            //Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Save();
            //Application.Current.Shutdown();
            //_ = Process.Start(Application.ResourceAssembly.Location);
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

        private void Setting(object sender, MouseButtonEventArgs e)
        {
            can.Visibility = Visibility.Visible;
            Win_Setting win_Setting = new Win_Setting();
            _ = win_Setting.ShowDialog();
            string metrialitem = Properties.Settings.Default.meterialinfo;

            SetSetting(metrialitem);
            arduSerial.DataReceived -= ArduReReceive;

            ArduConn();
           // ArduConn1();

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

                case "13":
                case "12":
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

        private void sort(object sender, MouseButtonEventArgs e)
        {
            if (sortChecked)
            {
                sortChecked = false;
            }
            else
            {
                sortChecked = true;
            }
            SortedList();
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







