using Newtonsoft.Json.Linq;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
    /// Robot_Setting2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Robot_Setting2 : Window
    {
        bool PLC_Connection = false;
        System.Timers.Timer Read_Timer;
        ActUtlTypeLib.ActUtlType ActUtlType; //MX Component 라이브러리 선언

        string Robot_State = ""; //로봇 가동/정지 상태
        string Con_State = ""; //통신 상태
        string prodID;

        public Robot_Setting2(String prodID, string customsName, string prodName)
        {
            try
            {
                InitializeComponent();
                LogWrite("로봇작업");
                this.prodID = prodID;
                serchRobotData(); //DB에서 데이터 조회
                DataRead(); //실시간 데이터 조회
                workName.Text = customsName + " -" + prodName;
            }
            catch (Exception e)
            {
                LogWrite(e.Message);
                LogWrite(e.StackTrace);
            }

        }

        private void LogWrite(string str) //에러사항 로그
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

        private void DataRead()//통신 오픈 / 읽기
        {
            MessageBox_Dialog msg;
            int StationNumber = int.Parse(Properties.Settings.Default.robotPort);
            try
            {
                ActUtlType = new ActUtlTypeLib.ActUtlType();
                ActUtlType.ActLogicalStationNumber = StationNumber; //PLC 국번

                Read_Timer = new System.Timers.Timer();
                Read_Timer.Interval = 1000;
                Read_Timer.Elapsed += Read_Timer_Tick;


                if (ActUtlType.Open().Equals(0)) //미쯔비시 통신 오픈 요청
                {
                    PLC_Connection = true;
                    Read_Timer.Start();
                }
                else //실패시
                {
                    ActUtlType.Close();
                    Read_Timer.Stop();
                    Read_Timer.Dispose();
                }
            }
            catch (Exception e)
            {
            }

        }

        public void Read_Timer_Tick(object sender, System.Timers.ElapsedEventArgs e) //실시간 로봇데이터 수신
        {
            if (PLC_Connection)
            {
                int rda1 = 0;
                int rda2 = 0;
                int rda3 = 0;
                int rda4 = 0;
                int rda5 = 0;
                int rda6 = 0;
                int rda7 = 0;
                int rda8 = 0;

                ActUtlType.GetDevice("D12000", out rda1); //가로
                ActUtlType.GetDevice("D12001", out rda2); //세로
                ActUtlType.GetDevice("D12002", out rda3); //높이
                ActUtlType.GetDevice("D12003", out rda4); //단수
                ActUtlType.GetDevice("D12004", out rda5); //간지
                ActUtlType.GetDevice("D12005", out rda6); //패턴
                ActUtlType.GetDevice("D12006", out rda7); //로봇상태 0:정지, 1:가동
                ActUtlType.GetDevice("D12007", out rda8); //통신상태 0:불가능, 1:가능


                this.Dispatcher.Invoke(DispatcherPriority.Normal
                , new Action(
                    delegate
                    {
                        ReadData1.Text = rda1.ToString(); //가로
                        ReadData2.Text = rda2.ToString(); //세로
                        ReadData3.Text = rda3.ToString(); //높이
                        ReadData4.Text = rda4.ToString(); //단수
                        ReadData5.Text = rda5.ToString(); //간지
                        ReadData6.Text = rda6.ToString(); //패턴
                        Robot_State = rda7.ToString(); //로봇상태
                        Con_State = rda8.ToString(); //통신상태

                        State1.BorderBrush = rda7.Equals(1) ? Brushes.Green : Brushes.Red; //설비 가동상태
                        State2.BorderBrush = rda8.Equals(1) ? Brushes.Green : Brushes.Red; //설비 통신상태

                        // 값이 0이 아닐때만 전송버튼 활성화
                        Robot_Send.Background = (WriteData1.Text.Length > 1 & //가로
                                                 WriteData2.Text.Length > 1 & //세로
                                                 WriteData3.Text.Length > 1 & //높이
                                                 WriteData4.Text.Length > 0 & //단수
                                                 WriteData5.Text.Length > 0 & //간지
                                                 WriteData6.Text.Length > 0 & //패턴
                        rda8.Equals(1)) ? Brushes.Green : Brushes.Red; //데이터 전송버튼 활성화


                        // 현재 로봇 패턴 표시
                        switch (ReadData6.Text)
                        {
                            case "1"://
                                boximage.Source = new BitmapImage(new Uri(@"/img/box1.png", UriKind.Relative));
                                break;
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
                                boximage.Visibility = Visibility.Collapsed; //패턴값 없으면 공백
                                break;
                        }
                    }));
            }
        }

        private void serchRobotData() //DB에서 데이터 조회
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[spSelIFrobotdata_info]",
               new string[] { "@prodID" },
               new object[] { prodID }, 0);

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
                    BOX_W = result["BOX_W"].ToString();           //가로
                    BOX_H = result["BOX_H"].ToString();           //세로
                    BOX_D = result["BOX_D"].ToString();           //높이
                    BOX_DADAN = result["BOX_DADAN"].ToString();   //단수
                    BOX_DANSU = result["BOX_DANSU"].ToString();   //간지
                    BOX_PAT = result["BOX_PAT"].ToString();       //패턴
                    PALLET_W = result["PALLET_W"].ToString();     //팔렛 가로 (안씀)
                    PALLET_H = result["PALLET_H"].ToString();     //팔렛 세로 (안씀)
                    PALLET_D = result["PALLET_D"].ToString();     //팔렛 높이 (안씀)
                    UpDatetime = result["UpDatetime"].ToString(); //업데이트 시간 (안씀)
                }
                WriteData1.Text = BOX_W;     //가로
                WriteData2.Text = BOX_H;     //세로
                WriteData3.Text = BOX_D;     //높이
                WriteData4.Text = BOX_DADAN; //단수
                WriteData5.Text = BOX_DANSU; //간지
                WriteData6.Text = BOX_PAT;   //패턴
            }
            else
            {
            }

            // 패턴 표시
            string data = "";
            switch (Convert.ToInt32(WriteData6.Text))
            {
                case 1:
                    data = WriteData6.Text;
                    if (data.Equals("1"))
                    {
                        box1.Background = Brushes.Green;
                    }
                    else
                    {
                        box1.Background = Brushes.White;
                    }

                    break;

                case 2:
                    data = WriteData6.Text;
                    if (data.Equals("2"))
                    {
                        box2.Background = Brushes.Green;
                    }
                    else
                    {
                        box2.Background = Brushes.White;
                    }
                    break;

                case 3:
                    data = WriteData6.Text;
                    if (data.Equals("3"))
                    {
                        box3.Background = Brushes.Green;
                    }
                    else
                    {
                        box3.Background = Brushes.White;
                    }
                    break;

                case 4:
                    data = WriteData6.Text;
                    if (data.Equals("4"))
                    {
                        box4.Background = Brushes.Green;
                    }
                    else
                    {
                        box4.Background = Brushes.White;
                    }
                    break;

                case 5:
                    data = WriteData6.Text;
                    if (data.Equals("5"))
                    {
                        box5.Background = Brushes.Green;
                    }
                    else
                    {
                        box5.Background = Brushes.White;
                    }
                    break;

                case 6:
                    data = WriteData6.Text;
                    if (data.Equals("6"))
                    {
                        box6.Background = Brushes.Green;
                    }
                    else
                    {
                        box6.Background = Brushes.White;
                    }
                    break;

                case 7:
                    data = WriteData6.Text;
                    if (data.Equals("7"))
                    {
                        box7.Background = Brushes.Green;
                    }
                    else
                    {
                        box7.Background = Brushes.White;
                    }
                    break;

                case 8:
                    data = WriteData6.Text;
                    if (data.Equals("8"))
                    {
                        box8.Background = Brushes.Green;
                    }
                    else
                    {
                        box8.Background = Brushes.White;
                    }
                    break;

                case 9:
                    data = WriteData6.Text;
                    if (data.Equals("9"))
                    {
                        box9.Background = Brushes.Green;
                    }
                    else
                    {
                        box9.Background = Brushes.White;
                    }
                    break;

                case 10:
                    data = WriteData6.Text;
                    if (data.Equals("10"))
                    {
                        box10.Background = Brushes.Green;
                    }
                    else
                    {
                        box10.Background = Brushes.White;
                    }
                    break;

                case 11:
                    data = WriteData6.Text;
                    if (data.Equals("11"))
                    {
                        box11.Background = Brushes.Green;
                    }
                    else
                    {
                        box11.Background = Brushes.White;
                    }
                    break;

                case 12:
                    data = WriteData6.ToString();
                    if (data.Equals("12"))
                    {
                        box12.Background = Brushes.Green;
                    }
                    else
                    {
                        box12.Background = Brushes.White;
                    }
                    break;

                case 13:
                    data = WriteData6.Text;
                    if (data.Equals("13"))
                    {
                        box13.Background = Brushes.Green;
                    }
                    else
                    {
                        box13.Background = Brushes.White;
                    }
                    break;

                case 14:
                    data = WriteData6.Text;
                    if (data.Equals("14"))
                    {
                        box14.Background = Brushes.Green;
                    }
                    else
                    {
                        box14.Background = Brushes.White;
                    }
                    break;

                case 15:
                    data = WriteData6.Text;
                    if (data.Equals("15"))
                    {
                        box15.Background = Brushes.Green;
                    }
                    else
                    {
                        box15.Background = Brushes.White;
                    }
                    break;

                case 16:
                    data = WriteData6.Text;
                    if (data.Equals("16"))
                    {
                        box16.Background = Brushes.Green;
                    }
                    else
                    {
                        box16.Background = Brushes.White;
                    }
                    break;
            }


        }

        private void pat_select(object sender, MouseButtonEventArgs e) //활성화된 패턴 선택
        {
            box1.Background = Brushes.White;
            box2.Background = Brushes.White;
            box3.Background = Brushes.White;
            box4.Background = Brushes.White;
            box5.Background = Brushes.White;
            box6.Background = Brushes.White;
            box7.Background = Brushes.White;
            box8.Background = Brushes.White;
            box9.Background = Brushes.White;
            box10.Background = Brushes.White;
            box11.Background = Brushes.White;
            box12.Background = Brushes.White;
            box13.Background = Brushes.White;
            box14.Background = Brushes.White;
            box15.Background = Brushes.White;
            box16.Background = Brushes.White;

            if (sender.GetType().Name.Equals("Grid"))
            {
                Grid context = (Grid)sender;
                MessageBox_Dialog dialog;
                switch (context.Name)
                {
                    case "box1":

                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "1";
                        }
                        else
                        {
                            dialog = new MessageBox_Dialog("이미 선택 되었습니다.");
                            dialog.Show();
                        }
                        break;

                    case "box2":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "2";
                        }
                        else
                        {
                        }
                        break;

                    case "box3":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "3";
                        }
                        else
                        {
                        }
                        break;

                    case "box4":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "4";
                        }
                        else
                        {
                        }
                        break;

                    case "box5":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "5";
                        }
                        else
                        {
                        }
                        break;

                    case "box6":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "6";
                        }
                        else
                        {
                        }
                        break;

                    case "box7":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "7";
                        }
                        else
                        {
                        }
                        break;

                    case "box8":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "8";
                        }
                        else
                        {
                        }
                        break;

                    case "box9":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "9";
                        }
                        else
                        {
                        }
                        break;

                    case "box10":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "10";
                        }
                        else
                        {
                        }
                        break;

                    case "box11":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "11";
                        }
                        else
                        {
                        }
                        break;

                    case "box12":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "12";
                        }
                        else
                        {
                        }
                        break;

                    case "box13":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "13";
                        }
                        else
                        {
                        }
                        break;

                    case "box14":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "14";
                        }
                        else
                        {
                        }
                        break;

                    case "box15":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "15";
                        }
                        else
                        {
                        }
                        break;

                    case "box16":
                        if (context.Background == Brushes.White)
                        {
                            context.Background = Brushes.Green;
                            WriteData6.Text = "16";
                        }
                        else
                        {
                        }
                        break;
                }
            }
        }

        private void Data_Send(object sender, RoutedEventArgs e) //데이터 전송 버튼 활성화
        {
            MessageBox_Dialog msg;
            if (dataCheck() && Con_State.Equals("1")) //통신연결 상태 확인
            {
                try
                {
                    int wda1 = 0; //가로
                    int wda2 = 0; //세로
                    int wda3 = 0; //높이
                    int wda4 = 0; //단수
                    int wda5 = 0; //간지
                    int wda6 = 0; //패턴

                    wda1 = Convert.ToInt32(WriteData1.Text); //가로
                    wda2 = Convert.ToInt32(WriteData2.Text); //세로
                    wda3 = Convert.ToInt32(WriteData3.Text); //높이
                    wda4 = Convert.ToInt32(WriteData4.Text); //단수
                    wda5 = Convert.ToInt32(WriteData5.Text); //간지
                    wda6 = Convert.ToInt32(WriteData6.Text); //패턴

                    if (dataCheck()) //전송전 범위값 재확인 후 정상이면
                    {
                        ActUtlType.SetDevice("D12000", wda1); //가로
                        ActUtlType.SetDevice("D12001", wda2); //세로
                        ActUtlType.SetDevice("D12002", wda3); //높이
                        ActUtlType.SetDevice("D12003", wda4); //단수
                        ActUtlType.SetDevice("D12004", wda5); //간지
                        ActUtlType.SetDevice("D12005", wda6); //패턴

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
            }
            else
            {
                msg = new MessageBox_Dialog("로봇이 준비 중입니다.");
                msg.ShowDialog();
            }


        }

        private bool dataCheck() //전송전 범위값 재확인
        {
            bool check = false;
            int value1 = int.Parse(WriteData1.Text.ToString()); //가로
            int value2 = int.Parse(WriteData2.Text.ToString()); //세로
            int value3 = int.Parse(WriteData3.Text.ToString()); //높이
            int value4 = int.Parse(WriteData4.Text.ToString()); //단수
            int value5 = int.Parse(WriteData5.Text.ToString()); //간지
            int value6 = int.Parse(WriteData6.Text.ToString()); //패턴
            check = value1 >= 350 && value1 <= 1400 &&
                    value2 >= 250 && value2 <= 1200 &&
                    value3 >= 50 && value3 <= 350 &&
                    value4 >= 1 && value4 <= 25 &&
                    value5 >= 1 && value5 <= 25;

            return check;
        }

        private void Input_data(object sender, MouseButtonEventArgs e) //데이터 값 키인(클릭)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;

                Win_KeyBoard keyBoard = new Win_KeyBoard(context) //키보드 팝업 위치
                {
                    Left = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width + 950,
                    Top = (SystemParameters.WorkArea.Height / 2) - this.Top
                };
                keyBoard.ShowDialog();
                int value;
                MessageBox_Dialog msg;
                switch (((TextBox)sender).Name)
                {
                    case "WriteData1": //가로
                        value = int.Parse(WriteData1.Text.ToString());
                        if (value < 350 || value > 1400)
                        {
                            msg = new MessageBox_Dialog("가로 값은 350보다 작거나 1400보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData1.Text = "0";
                        }
                        break;

                    case "WriteData2": //세로
                        value = int.Parse(WriteData2.Text.ToString());
                        if (value < 250 || value > 1200)
                        {
                            msg = new MessageBox_Dialog("세로 값은 250보다 작거나 1200보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData2.Text = "0";
                        }
                        break;

                    case "WriteData3": //높이
                        value = int.Parse(WriteData3.Text.ToString());
                        if (value < 50 || value > 350)
                        {
                            msg = new MessageBox_Dialog("높이 값은 50보다 작거나 350보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData3.Text = "0";
                        }
                        break;

                    case "WriteData4": //단수
                        value = int.Parse(WriteData4.Text.ToString());
                        if (value < 1 || value > 25)
                        {
                            msg = new MessageBox_Dialog("단수 값은 1보다 작거나 25보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData4.Text = "0";
                        }

                        break;

                    case "WriteData5": //간지
                        value = int.Parse(WriteData5.Text.ToString());
                        if (value < 1 || value > 25)
                        {
                            msg = new MessageBox_Dialog("간지 값은 1보다 작거나 25보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData5.Text = "0";
                        }
                        break;

                    case "WriteData6": //패턴
                        value = int.Parse(WriteData5.Text.ToString());
                        if (value < 1 || value > 16)
                        {
                            msg = new MessageBox_Dialog("패턴 값은 1보다 작거나 16보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData6.Text = "0";
                        }
                        break;
                }
            }

        }

        private void Save_robotdata1(object sender, RoutedEventArgs e) //DB 저장 (로봇데이터)
        {
            MessageBox_Dialog msg;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[spAEDIFrobotdata_info]",
               new string[] { "@prodID", "@BOX_W", "@BOX_H", "@BOX_D", "@BOX_DADAN", "@BOX_DANSU",
                "@BOX_PAT", "@PALLET_W", "@PALLET_H", "@PALLET_D"},
               new object[] {
                prodID,
                ReadData1.Text.ToString(), //가로
                ReadData2.Text.ToString(), //세로
                ReadData3.Text.ToString(), //높이
                ReadData4.Text.ToString(), //오주: 다단 / 대세 : 단수
                ReadData5.Text.ToString(), //오주: 단수 / 대세 : 간지
                ReadData6.Text.ToString(), //패턴
                "0", //오주: 파레트 가로 / 대세 안씀
                "0", //오주: 파레트 세로 / 대세 안씀
                "0"  //오주: 파레트 높이 / 대세 안씀

               }, 1
             );
            if (returnData.결과 == 1)
            {
                msg = new MessageBox_Dialog("로봇 데이터 저장 완료");
                msg.Show();
            }
            else
            {
                msg = new MessageBox_Dialog("저장 실패");
                msg.Show();
            }
        }

        private void Save_robotdata2(object sender, RoutedEventArgs e) //DB 저장 (전송데이터)
        {
            MessageBox_Dialog msg;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[spAEDIFrobotdata_info]",
               new string[] { "@prodID", "@BOX_W", "@BOX_H", "@BOX_D", "@BOX_DADAN", "@BOX_DANSU",
                "@BOX_PAT", "@PALLET_W", "@PALLET_H", "@PALLET_D"},
               new object[] {
                prodID,
                WriteData1.Text.ToString(), //가로
                WriteData2.Text.ToString(), //세로
                WriteData3.Text.ToString(), //높이
                WriteData4.Text.ToString(), //오주: 다단 / 대세 : 단수
                WriteData5.Text.ToString(), //오주: 단수 / 대세 : 간지
                WriteData6.Text.ToString(), //패턴
                "0", //오주: 파레트 가로 / 대세 안씀
                "0", //오주: 파레트 세로 / 대세 안씀
                "0"  //오주: 파레트 높이 / 대세 안씀

               }, 1
             );
            if (returnData.결과 == 1)
            {
                msg = new MessageBox_Dialog("전송예정 데이터 저장 완료");
                msg.Show();
            }
            else
            {
                msg = new MessageBox_Dialog("저장 실패");
                msg.Show();
            }
        }

        private void Exit_window(object sender, EventArgs e)// 창 닫히면
        {
            try
            {
                if (PLC_Connection)
                {
                    ActUtlType.Close();
                }

                if (Read_Timer != null)
                {
                    Read_Timer.Stop();
                    Read_Timer.Dispose();
                }
                PLC_Connection = false;
            }
            catch (Exception)
            {
            }
            Close();
        }

        private void robot_closed(object sender, EventArgs e)// 창 닫히면
        {
            try
            {
                if (PLC_Connection)
                {
                    ActUtlType.Close();
                }

                if (Read_Timer != null)
                {
                    Read_Timer.Stop();
                    Read_Timer.Dispose();
                }
                PLC_Connection = false;
            }
            catch (Exception)
            {
            }
            Close();
        }

        private void IP_setting(object sender, MouseButtonEventArgs e)
        {
            IP_Setting ip = new IP_Setting();
            ip.Show();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void kk_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
