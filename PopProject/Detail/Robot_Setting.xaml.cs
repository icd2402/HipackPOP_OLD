using Newtonsoft.Json.Linq;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PopProject.Detail
{
    /// <summary>
    /// Robot_Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Robot_Setting : Window
    {
        Ls_Fenet wd = new Ls_Fenet();
        public Socket mainSock;
        public Socket patSock;
        Boolean startFlag = false;
        string prodID;
        string[] arry;
        DispatcherTimer timer = null;
        public Robot_Setting(String prodID, string customsName, string prodName)
        {
            try
            {
                InitializeComponent();
                LogManager.Info("로봇작업", MethodBase.GetCurrentMethod().DeclaringType);
                this.prodID = prodID;
                workName.Text = customsName + " -" + prodName;
                serchRobotData();
                DataRead();
                DataStartRead();

                //1초 타이머 실행
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
            }
            catch (Exception e)
            {
                LogManager.Error("로봇 설정 에러\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
                LogManager.Error("로봇 설정 에러\n" + e.StackTrace, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        //1초마다 실행 타이머
        private void Timer_Tick(object sender, EventArgs e)
        {
            DataRead(); //데이터 수신 요청
            DataStartRead();
        }


        private void DataStartRead()
        {
            //ConnectSocket에 "S" 호출하여 소켓 오픈시
            if (ConnectSocket("S"))
            {
                byte[] bDts = wd.Byte_continuous_Read("D", "312", 2); //D312 

                patSock.Send(bDts); //읽기 dycjd
            }
        }

        //데이터 수신 요청
        private void DataRead()
        {
            if (ConnectSocket("R"))
            {
                byte[] bDts = wd.Word_continuous_Read("%DB600", 18); //D300 ~ D308 까지

                mainSock.Send(bDts); //읽기 요청
            }
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
                        case 4:
                            ReadData5.Text = arry[i].ToString();
                            break;
                        case 5:
                            ReadData6.Text = arry[i].ToString();
                            break;
                        case 6:
                            ReadData7.Text = arry[i].ToString();
                            break;
                        case 7:
                            ReadData8.Text = arry[i].ToString();
                            break;
                        case 8:
                            ReadData9.Text = arry[i].ToString();
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
                    sendButton.Background = Brushes.Green;
                    startButton.Content = "시작";
                    startFlag = true;
                }
                else
                {
                    startButton.Background = Brushes.Red;
                    sendButton.Background = Brushes.Red;
                    startButton.Content = "시작 대기중";
                    startFlag = false;
                }

                if (patdata.Equals("1"))
                {
                    pat_grid1.Visibility = Visibility.Visible;
                    pat_grid2.Visibility = Visibility.Visible;
                    pat_grid3.Visibility = Visibility.Visible;
                    pat_span.Visibility = Visibility.Collapsed;
                }
                else
                {
                    pat_grid1.Visibility = Visibility.Collapsed;
                    pat_grid2.Visibility = Visibility.Collapsed;
                    pat_grid3.Visibility = Visibility.Collapsed;
                    pat_span.Visibility = Visibility.Visible;
                }

                if (!WriteData6.Equals("") || !WriteData6.Equals("0"))
                {
                    switch (WriteData6.Text)
                    {
                        case "1":
                            boximage.Source = new BitmapImage(new Uri(@"/img/box1.png", UriKind.Relative)); break;
                        case "2"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box2.png", UriKind.Relative)); break;
                        case "3"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box3.png", UriKind.Relative)); break;
                        case "4"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box4.png", UriKind.Relative)); break;
                        case "5"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box5.png", UriKind.Relative)); break;
                        case "6"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box6.png", UriKind.Relative)); break;
                        case "7"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box7.png", UriKind.Relative)); break;
                        case "8"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box8.png", UriKind.Relative)); break;
                        case "9"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box9.png", UriKind.Relative)); break;
                        case "10"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box10.png", UriKind.Relative)); break;
                        case "11"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box11.png", UriKind.Relative)); break;
                        case "12"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box12.png", UriKind.Relative)); break;
                        case "13"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box13.png", UriKind.Relative)); break;
                        case "14"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box14.png", UriKind.Relative)); break;
                        case "15"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box15.png", UriKind.Relative)); break;
                        case "16"://
                            boximage.Source = new BitmapImage(new Uri(@"/img/box16.png", UriKind.Relative)); break;
                        default:
                            boximage.Visibility = Visibility.Collapsed;
                            break;
                    }
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

                        case 6:
                            data = pat_d.Substring(i, 1);
                            box7.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 7:
                            data = pat_d.Substring(i, 1);
                            box8.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 8:
                            data = pat_d.Substring(i, 1);
                            box9.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 9:
                            data = pat_d.Substring(i, 1);
                            box10.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 10:
                            data = pat_d.Substring(i, 1);
                            box11.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 11:
                            data = pat_d.Substring(i, 1);
                            box12.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 12:
                            data = pat_d.Substring(i, 1);
                            box13.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 13:
                            data = pat_d.Substring(i, 1);
                            box14.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 14:
                            data = pat_d.Substring(i, 1);
                            box15.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
                            break;

                        case 15:
                            data = pat_d.Substring(i, 1);
                            box16.Background = data.Equals("1") ? Brushes.Green : Brushes.White;
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
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spSelIFrobotdata_info]",
               new string[] { "@prodID" },
               new object[] {
               prodID
               }, 0
             );
            if (returnData.결과 == 1)
            {
                string BOX_W = "";
                string BOX_H = "";
                string BOX_D = "";
                string BOX_DADAN = "";
                string BOX_DANSU = "";
                string BOX_PAT = "";
                string PALLET_W = "";
                string PALLET_H = "";
                string PALLET_D = "";
                string UpDatetime = "";

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach (JObject result in Data_Jarry)
                {
                    BOX_W = result["BOX_W"].ToString();
                    BOX_H = result["BOX_H"].ToString();
                    BOX_D = result["BOX_D"].ToString();
                    BOX_DADAN = result["BOX_DADAN"].ToString();
                    BOX_DANSU = result["BOX_DANSU"].ToString();
                    BOX_PAT = result["BOX_PAT"].ToString();
                    PALLET_W = result["PALLET_W"].ToString();
                    PALLET_H = result["PALLET_H"].ToString();
                    PALLET_D = result["PALLET_D"].ToString();
                    UpDatetime = result["UpDatetime"].ToString();
                }
                WriteData1.Text = BOX_W;
                WriteData2.Text = BOX_H;
                WriteData3.Text = BOX_D;
                WriteData4.Text = BOX_DADAN;
                WriteData5.Text = BOX_DANSU;
                WriteData6.Text = BOX_PAT;
                WriteData7.Text = PALLET_W;
                WriteData8.Text = PALLET_H;
                WriteData9.Text = PALLET_D;
            }

            if (!WriteData6.Equals("") || !WriteData6.Equals("0"))
            {
                switch (WriteData6.Text)
                {
                    case "1"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box1.png", UriKind.Relative)); break;
                    case "2"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box2.png", UriKind.Relative)); break;
                    case "3"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box3.png", UriKind.Relative)); break;
                    case "4"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box4.png", UriKind.Relative)); break;
                    case "5"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box5.png", UriKind.Relative)); break;
                    case "6"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box6.png", UriKind.Relative)); break;
                    case "7"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box7.png", UriKind.Relative)); break;
                    case "8"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box8.png", UriKind.Relative)); break;
                    case "9"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box9.png", UriKind.Relative)); break;
                    case "10"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box10.png", UriKind.Relative)); break;
                    case "11"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box11.png", UriKind.Relative)); break;
                    case "12"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box12.png", UriKind.Relative)); break;
                    case "13"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box13.png", UriKind.Relative)); break;
                    case "14"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box14.png", UriKind.Relative)); break;
                    case "15"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box15.png", UriKind.Relative)); break;
                    case "16"://
                        boximage.Source = new BitmapImage(new Uri(@"/img/box16.png", UriKind.Relative)); break;
                    default:
                        boximage.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        //닫기
        private void Exit_window(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (timer != null)
                    timer.Stop();
            }
            catch (Exception e1)
            {
                LogManager.Error("로봇 종료 에러\n" + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);

            }
            Close();
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
                    case "ReadData1":
                        value = int.Parse(WriteData1.Text.ToString());
                        if (value < 350 || value > 1400)
                        {
                            msg = new MessageBox_Dialog("가로 값은 350보다 작거나 1400보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData1.Text = "0";
                        }
                        break;

                    case "WriteData2":
                    case "ReadData2":
                        value = int.Parse(WriteData2.Text.ToString());
                        if (value < 250 || value > 1200)
                        {
                            msg = new MessageBox_Dialog("세로 값은 250보다 작거나 1200보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData2.Text = "0";
                        }
                        break;

                    case "WriteData3":
                    case "ReadData3":
                        value = int.Parse(WriteData3.Text.ToString());
                        if (value < 50 || value > 350)
                        {
                            msg = new MessageBox_Dialog("높이 값은 50보다 작거나 350보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData3.Text = "0";
                        }
                        break;

                    case "WriteData4":
                    case "ReadData4":
                        value = int.Parse(WriteData4.Text.ToString());
                        if (value < 1 || value > 2)
                        {
                            msg = new MessageBox_Dialog("다단 값은 1보다 작거나 2보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData4.Text = "0";
                        }
                        break;

                    case "WriteData5":
                    case "ReadData5":
                        value = int.Parse(WriteData5.Text.ToString());
                        if (value < 1 || value > 25)
                        {
                            msg = new MessageBox_Dialog("단수 값은 1보다 작거나 2보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData5.Text = "0";
                        }
                        break;

                    case "WriteData7":
                    case "ReadData7":
                        value = int.Parse(WriteData7.Text.ToString());
                        if (value < 1000 || value > 1200)
                        {
                            msg = new MessageBox_Dialog("파레트 가로 값은 1000보다 작거나 1200보다 클 수 없습니다.");
                            msg.Show();
                            WriteData7.Text = "0";
                        }
                        break;

                    case "WriteData8":
                    case "ReadData8":

                        value = int.Parse(WriteData8.Text.ToString());
                        if (value < 700 || value > 1500)
                        {
                            msg = new MessageBox_Dialog("파레트 세로 값은 700보다 작거나 1500보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData8.Text = "0";
                        }
                        break;

                    case "WriteData9":
                    case "ReadData9":
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
        private void Save_robotdata(object sender, RoutedEventArgs e)
        {
            MessageBox_Dialog msg;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spAEDIFrobotdata_info]",
               new string[] { "@prodID", "@BOX_W", "@BOX_H", "@BOX_D", "@BOX_DADAN", "@BOX_DANSU",
                "@BOX_PAT", "@PALLET_W", "@PALLET_H", "@PALLET_D"},
               new object[] {
                prodID,
                ReadData1.Text.ToString(),
                ReadData2.Text.ToString(),
                ReadData3.Text.ToString(),
                ReadData4.Text.ToString(),
                ReadData5.Text.ToString(),
                ReadData6.Text.ToString(),
                ReadData7.Text.ToString(),
                ReadData8.Text.ToString(),
                ReadData9.Text.ToString()

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
                    items.Add(int.Parse(WriteData4.Text)); //다단
                    items.Add(int.Parse(WriteData5.Text)); //단수
                    items.Add(int.Parse(WriteData6.Text)); //패턴
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
                    value3 >= 50 && value3 <= 350 &&
                    value4 >= 1 && value4 <= 2 &&
                    value5 >= 1 && value5 <= 25 &&
                    value7 >= 700 && value7 <= 1200 &&
                    value8 >= 700 && value8 <= 1500 &&
                    value9 >= 90 && value9 <= 150 &&
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

                msg = new MessageBox_Dialog("전송 되었습니다.");
                msg.Show();
            }
            else
            {
                msg = new MessageBox_Dialog("로봇 세팅을 대기 중입니다.. ");
                msg.Show();
            }
        }

        private void pat_select(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Grid"))
            {
                Grid context = (Grid)sender;
                //context.Background = Brushes.Blue;
                MessageBox_Dialog dialog;
                switch (context.Name)
                {
                    case "box1":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 1);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //WriteData6.Text = "1";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box2":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 2);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();

                            }
                            //WriteData6.Text = "2";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box3":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 3);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //WriteData6.Text = "3";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box4":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 4);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                            WriteData6.Text = "4";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box5":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 5);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            // WriteData6.Text = "5";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box6":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 6);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //   WriteData6.Text = "6";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box7":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 7);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                            WriteData6.Text = "7";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box8":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 8);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //WriteData6.Text = "8";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box9":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 9);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //WriteData6.Text = "9";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box10":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 10);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //WriteData6.Text = "10";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box11":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 11);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                            WriteData6.Text = "11";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box12":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 12);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                            WriteData6.Text = "12";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box13":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 13);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                            WriteData6.Text = "13";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box14":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 14);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                            WriteData6.Text = "14";
                        }
                        else
                        {

                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box15":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 15);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //                           WriteData6.Text = "15";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                    case "box16":
                        if (context.Background == Brushes.Green)
                        {
                            if (ConnectSocket("P"))
                            {
                                byte[] bDts = wd.Word_single_Write("%DW305", 02, 16);
                                patSock.Send(bDts);
                                timer.Stop();
                                Close();
                            }
                            //WriteData6.Text = "16";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("활성 패턴이 아닙니다.");
                            dialog.Show();
                        }
                        break;
                }
            }
        }

        //예정데이터를 현재데이터로 변경
        private void Data_Move(object sender, RoutedEventArgs e)
        {
            MessageBox_Dialog msg;
            if (startFlag)
            {
                if (ConnectSocket("W"))
                {
                    byte[] bDts = wd.Word_single_Write("%DW613", 02, 2);
                    mainSock.Send(bDts);
                }

                Thread.Sleep(1000);
                if (ConnectSocket("W"))
                {
                    byte[] bDts = wd.Word_single_Write("%DW613", 02, 0);
                    mainSock.Send(bDts);
                }

                msg = new MessageBox_Dialog("전송 되었습니다.");
                msg.Show();
            }
            else
            {
                msg = new MessageBox_Dialog("로봇 세팅을 대기 중입니다.. ");
                msg.Show();
            }
        }

        private void robot_Loaded(object sender, RoutedEventArgs e)
        {

            if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
            {
                System.Drawing.Rectangle rec;
                if (SettingJsonManager.mercName.Equals("인라인3호기") || SettingJsonManager.mercName.Equals("인라인2호기"))
                {
                    rec = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
                }
                else
                {
                    rec = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;
                }

                Left = rec.Left + 40;
                Top = rec.Top + 40;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WindowState = WindowState.Normal;
            }

            else
            {
                WindowState = WindowState.Normal;
            }

        }

        private void robot_closed(object sender, EventArgs e)
        {
            if (timer != null) { timer.Stop(); }



        }
    }
}
