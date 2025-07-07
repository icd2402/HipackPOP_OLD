using Newtonsoft.Json.Linq;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VagabondK.Protocols.Channels;
using VagabondK.Protocols.LSElectric;
using VagabondK.Protocols.LSElectric.FEnet;

namespace PopProject.Detail
{
    /// <summary>
    /// Robot_Setting3.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Robot_Setting3 : Window
    {
        Ls_Fenet wd = new Ls_Fenet();
        public Socket mainSock;
        public Socket patSock;
        Boolean startFlag = false;
        string orderKey;
        string customsName;
        string prodName;
        string gor_H;
        string[] arry;
        DispatcherTimer timer= null;
        List<Image> _images = null;
        string _Pattern;
        FEnetClient client;

        public Robot_Setting3(MainModel mainModel)
        {
            try
            {
                customsName = mainModel.customSName; //거래처명
                prodName = mainModel.prodName; //품명
                orderKey = mainModel.orderKey; //작업지시번호
                InitializeComponent();
                Event_Binding();


                ReturnModel returnData = ServiceModel.GetData(
                    mainModel.ServerIndex,
                    "[spSel009metrialinfo_GorID]",
                    new string[] {  },
                    new string[] {  },
                    0
                );


                if (returnData.결과 == 1)
                {
                    JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                    string[] _whd = mainModel.mLmW.Split('*');
                    gor_H = GorCalc(Data_Jarry,mainModel.paperGor);
                    if (_whd.Length >= 2)
                    {
                        WriteData1.Text = (int.Parse(_whd[0].Trim()) + int.Parse(_whd[1].Trim())).ToString();
                        WriteData2.Text = (int.Parse(_whd[1].Trim()) + int.Parse(_whd[2].Trim())).ToString();
                        WriteData3.Text = gor_H;
                    }
                }

                Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/White_Back.png"));
                //1초 타이머 준비
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += new EventHandler(Timer_Tick);


                LogManager.Info("로봇작업", MethodBase.GetCurrentMethod().DeclaringType);
                workName.Text = customsName + " -" + prodName; // 거래처 + 품명
                serchRobotData(); // 로봇 마지막 데이터 조회


                client = new FEnetClient(new TcpChannel(SettingJsonManager.robotIP, int.Parse(SettingJsonManager.robotPort)));

                if (SettingJsonManager.ip != "" && SettingJsonManager.port != "")
                { 
                    DataRead();
                    timer.Start();
                }
                //DataStartRead();

                //DataContext = this;
            }
            catch(Exception e)
            {
                LogManager.Error("로봇 설정 에러\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
                LogManager.Error("로봇 설정 에러\n" + e.StackTrace, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        #region 골 높이 계산
        public string GorCalc(JArray GorArray,string _paperGor)
        {
            string _gor_H = string.Empty;
            string[] _gor = _paperGor.Split(' ');
            if (_gor[0].Length == 2)
            {
                string _first = _gor[0].Substring(0, 1);
                string _second = _gor[0].Substring(1, 1);

                _first = rtnGor_D(GorArray, _first);
                _second = rtnGor_D(GorArray, _second);
                _gor_H = (float.Parse(_first) + float.Parse(_second)).ToString();
            }
            else
            {
                _gor_H = rtnGor_D(GorArray, _gor[0]);
            }

            return _gor_H;
        }
        public string rtnGor_D(JArray GorArray, string _paperGor)
        {
            string _gor_H = string.Empty;
            foreach (JObject result in GorArray)
            {
                if (result["metrialGor"].ToString() == _paperGor)
                {
                    _gor_H = result["metrialH"].ToString();
                    break;
                }
            }
            return _gor_H;
        }
        #endregion

        #region 이벤트 바인딩
        private void Event_Binding()
        {
            _Exit.MouseLeftButtonDown += Exit_window;
            WriteData1.MouseLeftButtonDown += Input_data;
            WriteData2.MouseLeftButtonDown += Input_data;
            WriteData3.MouseLeftButtonDown += Input_data;
            WriteData4.MouseLeftButtonDown += Input_data;
            WriteData5.MouseLeftButtonDown += Input_data;
            WriteData6.MouseLeftButtonDown += Input_data;
            WriteData7.MouseLeftButtonDown += Input_data;
            WriteData8.MouseLeftButtonDown += Input_data;
            WriteData9.MouseLeftButtonDown += Input_data;

            box1.MouseLeftButtonDown += pat_select_LS;
            box2.MouseLeftButtonDown += pat_select_LS;
            box3.MouseLeftButtonDown += pat_select_LS;
            box4.MouseLeftButtonDown += pat_select_LS;
            box5.MouseLeftButtonDown += pat_select_LS;
            box6.MouseLeftButtonDown += pat_select_LS;

            //startButton.MouseLeftButtonDown += Data_Send;
            //sendButton.MouseLeftButtonDown += Data_Send;
            netSetting.MouseLeftButtonDown += IP_setting;
        }
        #endregion
        //1초마다 실행 타이머
        private void Timer_Tick(object sender, EventArgs e)
        {
            DataRead(); //데이터 수신 요청
            //DataStartRead();
        }

        private void DataRead()
        {

            // FenetClient를 사용하여 로봇 데이터 읽기
            client.Read(DeviceType.M, 100, 10).ToArray(); // M100부터 10개 읽기
            foreach(var item in client.Read(DeviceType.M, 100, 10))
            {
                Console.WriteLine(item);
                Save_robot_work_data(Convert.ToInt16(item));
            }
            


            // 아래는 기존 작성되어있던 방식으로 헤더부터 쭉 작성하여 사용
            // 헤더,영역 등 정보를 설정하는 방식
            if (ConnectSocket("R"))
            {
                byte[] bDts = wd.Word_continuous_Read("%DB600", 18); // DB600 읽기

                mainSock.Send(bDts); //읽기 요청
            }

            var bytes = client.Read(DeviceType.M, 200, 10).ToArray();
        }

        private Boolean ConnectSocket(string flag)
        {
            bool success = false;

            if (flag.Equals("P") || flag.Equals("S"))
            {
                patSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                if (!patSock.Connected)
                {
                    int port = int.Parse(SettingJsonManager.robotPort);
                    IAsyncResult result = patSock.BeginConnect(SettingJsonManager.robotIP, port, null, null);
                    success = result.AsyncWaitHandle.WaitOne(500, true);

                    if (success)
                    {
                        patSock.EndConnect(result);
                        AsyncObject obj = new AsyncObject(150);
                        obj.WorkingSocket = mainSock;
                        obj.ClearBuffer();

                        switch (flag)
                        {
                            case "S":
                                patSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceivedS, obj);
                                break;

                            case "P":
                                patSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceivedP, obj);
                                break;
                        }
                    }
                }
            }
            else
            {
                mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                if (!mainSock.Connected)
                {
                    int port = int.Parse(SettingJsonManager.robotPort);
                    IAsyncResult result = mainSock.BeginConnect(SettingJsonManager.robotIP, port, null, null);
                   // IAsyncResult result = mainSock.BeginConnect("192.168.7.7", port, null, null);
                    success = result.AsyncWaitHandle.WaitOne(2000, true);

                    if (success)
                    {
                        mainSock.EndConnect(result);
                        AsyncObject obj = new AsyncObject(150);
                        obj.WorkingSocket = mainSock;
                        obj.ClearBuffer();
                        switch (flag)
                        {
                            case "R":
                                mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                                break;
                            case "W":
                                mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived1, obj);
                                break;

                        }

                    }

                }
            }

            return success;
        }

        public void DataReceived(IAsyncResult ar) // 데이터 수신
        {
            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            // 수신한 데이터 16진수로 변환
            string Receive_Data = string.Empty;
            for (int i = 0; i < obj.Buffer.Length; i++)
            {
                Receive_Data += obj.Buffer[i].ToString("x2");
            }
            //MessageBox.Show(Receive_Data.ToString()); //수신 데이터값 보기(기본)

            //연속읽기
            string continuous_read = string.Empty;
            string RD = Receive_Data.Substring(64);
            Console.WriteLine("수신데이터RD : " + RD.ToString());

            //앞에 데이터 지우고 데이터 정보부터 읽음
            for (int i = 0; i < RD.Length; i++)
            {
                string high_data = RD.Substring(0, 2); // 앞 두자리 추출
                string low_data = RD.Substring(2, 2); // 뒤 두자리 
                //continuous_read += Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                continuous_read += low_data + high_data; //앞 뒤 자리바꾸고 10진수 변환
                RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
            }


            Console.WriteLine("응답" + continuous_read.Substring(0, 36).ToString());
            continuous_read = continuous_read.Substring(0, 36).ToString();
            int indexStart = 0;
            int indexEnd = 0;
            int split = 5;
            int totalLength = continuous_read.Length;
            int forCount = totalLength / split;
            arry = new string[forCount + 2];
            for (int i = 0; i <= forCount + 1; i++)
            {
                if (totalLength < indexStart + split)
                {
                    indexEnd = totalLength - indexStart;
                }
                else
                {
                    indexEnd = continuous_read.Substring(indexStart, split).LastIndexOf("");
                }
                arry[i] = Convert.ToInt64(continuous_read.Substring(indexStart, indexEnd), 16).ToString();

                indexStart += indexEnd;
            }
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {

                for (int i = 0; i < arry.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ReadData1.Text = arry[i].ToString();
                            break;
                        case 1:
                            ReadData2.Text = arry[i].ToString();
                            break;
                        case 2:
                            ReadData3.Text = arry[i].ToString();
                            break;
                        case 3:
                            ReadData4.Text = arry[i].ToString();
                            break;
                    
                    }
                }
            }));


            obj.ClearBuffer();
            mainSock.Close();

            // 수신 대기
            // obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 2000, 0, DataReceived, obj);
        }


        public void DataReceivedS(IAsyncResult ar) // 데이터 수신
        {
            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            // 수신한 데이터 16진수로 변환
            string Receive_Data = string.Empty;
            for (int i = 0; i < obj.Buffer.Length; i++)
            {
                Receive_Data += obj.Buffer[i].ToString("x2");
            }
            //MessageBox.Show(Receive_Data.ToString()); //수신 데이터값 보기(기본)

            //연속읽기
            string continuous_read = string.Empty;
            string RD = Receive_Data.Substring(64);
            char pad = '0';
            //Console.WriteLine("수신데이터RD : " + RD.ToString());

            //앞에 데이터 지우고 데이터 정보부터 읽음
            for (int i = 0; i < RD.Length; i++)
            {
                string high_data = RD.Substring(0, 2); // 앞 두자리 추출
                string low_data = RD.Substring(2, 2); // 뒤 두자리 
                                                      // continuous_read += Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                continuous_read += new String(Convert.ToString(Convert.ToInt32(low_data + high_data, 16), 2).PadLeft(16, pad).Reverse().ToArray()); // 2진수 변환 후 빈공간0처리
                RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
            }
            Console.WriteLine("패턴데이터 : " + continuous_read.FirstOrDefault().ToString());

            string patdata = continuous_read.Substring(16, 1);
            string startpat = continuous_read.Substring(17, 1);
            string pat_d = continuous_read.Substring(0, 16);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                if (startpat.Equals("1"))
                {
                    startButton.Background = Brushes.Green;
                    //sendButton.Background = Brushes.Green;
                    startButton.Content = "시작";
                    startFlag = true;
                }
                else
                {
                    startButton.Background = Brushes.Red;
                    //sendButton.Background = Brushes.Red;
                    startButton.Content = "시작 대기중";
                    startFlag = false;
                }

                if (patdata.Equals("1"))
                {
                    pat_grid1.Visibility = Visibility.Visible;
                    pat_grid2.Visibility = Visibility.Visible;
                    //pat_grid3.Visibility = Visibility.Visible;
                    pat_span.Visibility = Visibility.Collapsed;
                }
                else
                {
                    pat_grid1.Visibility = Visibility.Collapsed;
                    pat_grid2.Visibility = Visibility.Collapsed;
                    //pat_grid3.Visibility = Visibility.Collapsed;
                    pat_span.Visibility = Visibility.Visible;
                }

                for (int i = 0; i < pat_d.Length; i++)
                {
                    string data = "";
                    switch (i)
                    {
                        case 0:
                            data = pat_d.Substring(i, 1);
                            box1.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 1:
                            data = pat_d.Substring(i, 1);
                            box2.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 2:
                            data = pat_d.Substring(i, 1);
                            box3.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 3:
                            data = pat_d.Substring(i, 1);
                            box4.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 4:
                            data = pat_d.Substring(i, 1);
                            box5.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 5:
                            data = pat_d.Substring(i, 1);
                            box6.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;
                    }
                }
            }));

            obj.ClearBuffer();
            patSock.Close();
        }

        public void DataReceived1(IAsyncResult ar) // 데이터 수신
        {
            try
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 수신한 데이터 16진수로 변환
                string Receive_Data = string.Empty;
                for (int i = 0; i < obj.Buffer.Length; i++)
                {
                    Receive_Data += obj.Buffer[i].ToString("x2");
                }
                //MessageBox.Show(Receive_Data.ToString()); //수신 데이터값 보기(기본)

                //연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64);
                //Console.WriteLine("수신데이터RD : " + RD.ToString());

                //앞에 데이터 지우고 데이터 정보부터 읽음
                for (int i = 0; i < RD.Length; i++)
                {
                    string high_data = RD.Substring(0, 2); // 앞 두자리 추출
                    string low_data = RD.Substring(2, 2); // 뒤 두자리 
                                                          //continuous_read += Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                    continuous_read += low_data + high_data; //앞 뒤 자리바꾸고 10진수 변환
                    RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                }

                Console.WriteLine(continuous_read.Substring(0, 36).ToString());
                obj.ClearBuffer();
            }
            catch (Exception)
            {

            }
            // 수신 대기
            // obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 2000, 0, DataReceived, obj);
            mainSock.Close();
        }

        // 패턴 데이터 수신
        public static void DataReceivedP(IAsyncResult ar) 
        {
            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            // 수신한 데이터 16진수로 변환
            string Receive_Data = string.Empty;
            for (int i = 0; i < obj.Buffer.Length; i++)
            {
                Receive_Data += obj.Buffer[i].ToString("x2");
            }
            //MessageBox.Show(Receive_Data.ToString()); //수신 데이터값 보기(기본)

            //연속읽기
            string continuous_read = string.Empty;
            string RD = Receive_Data.Substring(64);
            char pad = '0';
            //Console.WriteLine("수신데이터RD : " + RD.ToString());

            //앞에 데이터 지우고 데이터 정보부터 읽음
            for (int i = 0; i < RD.Length; i++)
            {
                string high_data = RD.Substring(0, 2); // 앞 두자리 추출
                string low_data = RD.Substring(2, 2); // 뒤 두자리 
                                                      // continuous_read += Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                continuous_read += new String(Convert.ToString(Convert.ToInt32(low_data + high_data, 16), 2).PadLeft(16, pad).Reverse().ToArray()); // 2진수 변환 후 빈공간0처리
                RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
            }

            Console.WriteLine("패턴데이터 : " + continuous_read.ToString());
            obj.ClearBuffer();

        }

        //DB 조회하여 화면에 표시
        private void serchRobotData()
        {
            ReadData1.Text = "";
            ReadData2.Text = "";
            ReadData3.Text = "";
            ReadData4.Text = "";
            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/White_Back.png"));

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spSelIFrobotdata_info]",
               new string[] { "@machine" },
               new object[] {
               SettingJsonManager.mercName,
               }, 0
             );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarray = JArray.Parse(returnData.리턴값);
                if(Data_Jarray.Count > 0) 
                {
                    JObject Data_Jobject = Data_Jarray.First().ToObject<JObject>();

                    ReadData1.Text = Data_Jobject["prodName"].ToString();
                    ReadData2.Text = Data_Jobject["BOX_W"].ToString();
                    ReadData3.Text = Data_Jobject["BOX_H"].ToString();
                    ReadData4.Text = Data_Jobject["BOX_D"].ToString();

                    switch (Data_Jobject["Pattern"].ToString())
                    {
                        case "2종.png"://
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/2종.png")); break;
                        case "4종.png"://
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/4종.png")); break;
                        case "5종.png"://
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/5종.png")); break;
                        case "6종.png"://
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/6종.png")); break;
                        case "7종.png"://
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/7종.png")); break;
                        case "4종구11자.png"://
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/4종구11자.png")); break;
                        default:
                            top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/White_Back.png")); break;
                    }
                }
            }
        }

        //닫기
        private void Exit_window(object sender, MouseButtonEventArgs e)
        {
            //try
            //{
            //    if(timer!=null)
            //    timer.Stop();
            //}
            //catch(Exception e1)
            //{
            //    LogManager.Error("로봇 종료 에러\n" + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);
            //    
            //}
            //Close();
            Hide();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //키인으로 예정데이터 입력시 발생 이벤트
        private void Input_data(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                
                TextBox context = (TextBox)sender;
                Win_KeyBoard keyBoard = new Win_KeyBoard(context)
                {
                    Left = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width + 950,
                    Top = (SystemParameters.WorkArea.Height / 2) - this.Top
                };
                keyBoard.ShowDialog();

                int value;
                MessageBox_Dialog msg;
                switch (((TextBox)sender).Name)
                {
                    case "WriteData1":
                        value = int.Parse(WriteData1.Text.ToString());
                        if (value < 350 || value > 1400)
                        {
                            msg = new MessageBox_Dialog("가로 값은 350보다 작거나 1400보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData1.Text = "0";
                        }
                        break;

                    case "WriteData2":
                        value = int.Parse(WriteData2.Text.ToString());
                        if (value < 250 || value > 1200)
                        {
                            msg = new MessageBox_Dialog("세로 값은 250보다 작거나 1200보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData2.Text = "0";
                        }
                        break;

                    case "WriteData3":
                        value = int.Parse(WriteData3.Text.ToString());
                        if (value < 0 || value > 15)
                        {
                            msg = new MessageBox_Dialog("높이 값은 0보다 작거나 15보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData3.Text = "0";
                        }
                        break;

                    case "WriteData4":
                        value = int.Parse(WriteData4.Text.ToString());
                        if (value < 1 || value > 25)
                        {
                            msg = new MessageBox_Dialog("단수 값은 1보다 작거나 2보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData5.Text = "0";
                        }
                        break;

                    case "WriteData7":
                        value = int.Parse(WriteData7.Text.ToString());
                        if (value < 1000 || value > 1200)
                        {
                            msg = new MessageBox_Dialog("파레트 가로 값은 1000보다 작거나 1200보다 클 수 없습니다.");
                            msg.Show();
                            WriteData7.Text = "0";
                        }
                        break;

                    case "WriteData8":

                        value = int.Parse(WriteData8.Text.ToString());
                        if (value < 700 || value > 1500)
                        {
                            msg = new MessageBox_Dialog("파레트 세로 값은 700보다 작거나 1500보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData8.Text = "0";
                        }
                        break;

                    case "WriteData9":
                        value = int.Parse(WriteData9.Text.ToString());
                        if (value < 90 || value > 150)
                        {
                            msg = new MessageBox_Dialog("파레트 높이 값은 90보다 작거나 150보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData9.Text = "0";
                        }
                        break;
                }
            }
        }

        //로봇 현재 테이터를 DB에 저장
        private void Save_robotdata()
        {
            MessageBox_Dialog msg;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string[] _tmp = Bottom_Image.Source.ToString().Split('/');
            _Pattern = _tmp[_tmp.Length-1].ToString();
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spAEDIFrobotdata_info]",
               new string[] { "@orderKey", "@Machine" ,"@Pattern" ,"@BOX_W", "@BOX_H", "@BOX_D", "@BOX_DANSU", "@BOX_BANDING",
                "@BOX_GANGI", "@PALLET_W", "@PALLET_H", "@PALLET_D"},
               new object[] {
                orderKey,
                SettingJsonManager.mercName,
                _Pattern,
                WriteData1.Text.ToString(),
                WriteData2.Text.ToString(),
                WriteData3.Text.ToString(),
                WriteData4.Text.ToString(),
                WriteData5.Text.ToString(),
                WriteData6.Text.ToString(),
                WriteData7.Text.ToString(),
                WriteData8.Text.ToString(),
                WriteData9.Text.ToString(),
               }, 1
             );
            if (returnData.결과 == 1)
            {
                msg = new MessageBox_Dialog("데이터 저장 완료");
                msg.Show();
            }
            else
            {
                msg = new MessageBox_Dialog("저장 실패");
                msg.Show();
            }
        }

        private void Save_robot_work_data(int _Cnt)
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string[] _tmp = Bottom_Image.Source.ToString().Split('/');
            _Pattern = _tmp[_tmp.Length - 1].ToString();
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spAEDIFrobotworkdata_info]",
               new string[] { "@orderKey", "@workCnt"},
               new object[] {
                orderKey,
                _Cnt,
               }, 1
             );
            if (returnData.결과 == 1)
            {
                LogManager.Info("Cnt저장완료", MethodBase.GetCurrentMethod().DeclaringType);
            }
            else
            {
                LogManager.Info("Cnt저장실패", MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        //IP 세팅 창 팝업
        private void IP_setting(object sender, MouseButtonEventArgs e)
        {
            IP_Setting ip = new IP_Setting();
            ip.Show();
        }

        //로봇으로 예정 데이터 전송
        private void Data_Send(object sender, RoutedEventArgs e)
        {
            MessageBox_Dialog msg;

            if (ConnectSocket("W"))
            {
                try
                {
                    List<int> items = new List<int>();
                    items.Add(int.Parse(WriteData1.Text)); //박스가로
                    items.Add(int.Parse(WriteData2.Text)); //박스세로 
                    items.Add(int.Parse(WriteData3.Text)); //박스높이
                    items.Add(int.Parse(WriteData4.Text)); //단수
                    items.Add(int.Parse(WriteData5.Text)); //밴딩매수
                    items.Add(int.Parse(WriteData6.Text)); //간지공급
                    items.Add(int.Parse(WriteData7.Text)); //파렛트가로
                    items.Add(int.Parse(WriteData8.Text)); //파렛트세로
                    items.Add(int.Parse(WriteData9.Text)); //파렛트높이
                    int WriteCount = items.Count() * 2;

                    byte[] bDts = wd.continuous_Write("%DB1200", WriteCount, items);
                    string Send_Data = string.Empty;

                    //데이터 리미트 체크
                    if (dataCheck())
                    {
                        mainSock.Send(bDts); //데이터 쓰기
                        Save_robotdata();
                        msg = new MessageBox_Dialog("전송되었습니다.");
                        msg.Show();
                    }
                    else
                    {
                        msg = new MessageBox_Dialog("전송 예정 데이터를 확인 해 주세요");
                        msg.Show();
                    }
                }
                catch (Exception)
                {
                    msg = new MessageBox_Dialog("재시도 해주세요.");

                    msg.Show();
                }
                finally
                {
                    mainSock.Close();
                }
            }
            else
            {
                mainSock.Close();
                msg = new MessageBox_Dialog("Plc 연결 불가");
                msg.ShowDialog();
            }
        }

        //데이터 리미트 체크
        private bool dataCheck()
        {
            bool check = false;
            int value1 = int.Parse(WriteData1.Text.ToString());
            int value2 = int.Parse(WriteData2.Text.ToString());
            int value3 = int.Parse(WriteData3.Text.ToString());
            int value4 = int.Parse(WriteData4.Text.ToString());
            int value5 = int.Parse(WriteData5.Text.ToString());
            int value6 = int.Parse(WriteData6.Text.ToString());
            int value7 = int.Parse(WriteData7.Text.ToString());
            int value8 = int.Parse(WriteData8.Text.ToString());
            int value9 = int.Parse(WriteData9.Text.ToString());
            check = value1 >= 350 && value1 <= 1400 &&
                    value2 >= 250 && value2 <= 1200 &&
                    value3 >= 50  && value3 <= 350  &&
                    value4 >= 1   && value4 <= 2    &&
                    value5 >= 1   && value5 <= 25   &&
                    value7 >= 700 && value7 <= 1200 &&
                    value8 >= 700 && value8 <= 1500 &&
                    value9 >= 90  && value9 <= 150  &&
                    value3 * value5 < 2200;

            return check;
        }

        //시작버튼
        private void start_button(object sender, RoutedEventArgs e)
        {
            
            MessageBox_Dialog msg;
            if (startFlag)
            {
                //D613.0에 Pulse
                if (ConnectSocket("W"))
                {
                    byte[] bDts = wd.Word_single_Write("%DW613", 02, 1);
                    mainSock.Send(bDts);
                }

                Thread.Sleep(1000); //1초 딜레이

                if (ConnectSocket("W"))
                {
                    byte[] bDts = wd.Word_single_Write("%DW613", 02, 0);
                    mainSock.Send(bDts);
                }
                Save_robotdata();
                msg = new MessageBox_Dialog("전송 되었습니다.");
                msg.Show();
            }
            else
            {
                msg = new MessageBox_Dialog("로봇 세팅을 대기 중입니다.. ");
                msg.Show();
            }
        }

        private void pat_select_LS(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Grid"))
            {
                Grid context = (Grid)sender;

                // 패턴 배경 리셋  
                box1.Background = Brushes.White;
                box2.Background = Brushes.White;
                box3.Background = Brushes.White;
                box4.Background = Brushes.White;
                box5.Background = Brushes.White;
                box6.Background = Brushes.White;

                context.Background = Brushes.Green;
                switch (context.Name)
                {
                    case "box1":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/2종.png"));
                        }
                        break;
                    case "box2":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/4종.png"));
                        }
                        break;
                    case "box3":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/5종.png"));
                        }
                        break;
                    case "box4":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/6종.png"));
                        }
                        break;
                    case "box5":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/7종.png"));
                        }
                        break;
                    case "box6":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/4종구11자.png"));
                        }
                        break;
                }
            }
        }
        


        private void robot_closed(object sender, EventArgs e)
        {
            if (timer != null) { timer.Stop(); }
               

           
        }
    }
}
