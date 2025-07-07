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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace PopProject.Detail
{

    /// <summary>
    /// Robot_Setting4.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Robot_Setting4 : Window
    {
        bool PLC_Connection = false;
        System.Timers.Timer Read_Timer;
        ActUtlTypeLib.ActUtlType ActUtilType; //MX Component 라이브러리 선언
        

        string Robot_State = ""; //로봇 가동/정지 상태
        string Con_State = ""; //통신 상태
        string prodID;
        string pattern = ""; //패턴
        string orderKey;
        string gor_H;
        string customsName = ""; //작업명
        string prodName = ""; //품명
        int _merc1_cnt = 0;
        int _merc2_cnt = 0;

        public Robot_Setting4(MainModel mainModel)
        {
            try
            {
                customsName = mainModel.customSName;
                prodName = mainModel.prodName;
                orderKey = mainModel.orderKey;
                InitializeComponent();
                Event_Binding();

                //ReturnModel returnData = ServiceModel.GetData(
                //    mainModel.ServerIndex,
                //    "[spSel009metrialinfo_GorID]",
                //    new string[] { },
                //    new string[] { },
                //    0
                //);
                //
                //
                //if (returnData.결과 == 1)
                //{
                //    JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                //    string[] _whd = mainModel.mLmW.Split('*');
                //    gor_H = GorCalc(Data_Jarry, mainModel.paperGor);
                //    if (_whd.Length >= 2)
                //    {
                //        WriteData1.Text = (int.Parse(_whd[0].Trim()) + int.Parse(_whd[1].Trim())).ToString();
                //        WriteData2.Text = (int.Parse(_whd[1].Trim()) + int.Parse(_whd[2].Trim())).ToString();
                //        WriteData3.Text = gor_H;
                //    } // 밴딩매수 연산 추가
                //}
                //workName.Text = customsName + " -" + prodName; // 거래처 + 품명
                LogWrite("로봇작업");
                serchRobotData(); //DB에서 데이터 조회
                DataRead(); //실시간 데이터 조회
            }
            catch (Exception e)
            {
                LogWrite(e.Message);
                LogWrite(e.StackTrace);
            }

        }

        #region 골 높이 계산
        public string GorCalc(JArray GorArray, string _paperGor)
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

            box1.MouseLeftButtonDown += pat_select_MIT;
            box2.MouseLeftButtonDown += pat_select_MIT;
            box3.MouseLeftButtonDown += pat_select_MIT;
            box4.MouseLeftButtonDown += pat_select_MIT;
            box5.MouseLeftButtonDown += pat_select_MIT;
            box6.MouseLeftButtonDown += pat_select_MIT;

            Robot_Send.MouseLeftButtonDown += Data_Send;
            //sendButton.MouseLeftButtonDown += Data_Send;
            netSetting.MouseLeftButtonDown += IP_setting;
        }
        #endregion

        

        private void DataRead()//통신 오픈 / 읽기
        {
            MessageBox_Dialog msg;
            //int StationNumber = int.Parse(SettingJsonManager.robotPort);
            int StationNumber = 0;
            try
            {
                //통신 셋팅
                ActUtilType = new ActUtlTypeLib.ActUtlType();
                ActUtilType.ActLogicalStationNumber = StationNumber; //PLC 국번
                //미쯔비시 통신 오픈
                int nRtn = ActUtilType.Open();
                if (nRtn == 0) 
                {
                    PLC_Connection = true;
                    Read_Timer.Start();
                    State3.BorderBrush = Brushes.Green; //통신 상태

                    // 로봇 데이터 읽기 메모리 주소
                    int _D12000 = 0; // 자동운전
                    int _D12001 = 0; // 적재패턴
                    int _D12002 = 0; // 모델명
                    int _D12003 = 0; // 가로
                    int _D12004 = 0; // 세로
                    int _D12005 = 0; // 높이
                    int _D12006 = 0; // 단
                    int _D12007 = 0; // 간지공급
                    int _D12008 = 0; // 폭
                    ActUtilType.GetDevice("D12000", out _D12000);


                    if (_D12000 == 1)
                    {
                        // Base 작업조건 가져와서 업데이트
                        ActUtilType.GetDevice("D12001", out _D12001);
                        ActUtilType.GetDevice("D12002", out _D12002);
                        ActUtilType.GetDevice("D12003", out _D12003);
                        ActUtilType.GetDevice("D12004", out _D12004);
                        ActUtilType.GetDevice("D12005", out _D12005);
                        ActUtilType.GetDevice("D12006", out _D12006);
                        ActUtilType.GetDevice("D12007", out _D12007);
                        ActUtilType.GetDevice("D12008", out _D12008);

                        //업데이트 프로시저 호출

                    }

                    //1초마다 실시간 통신
                    Read_Timer = new System.Timers.Timer();
                    Read_Timer.Interval = 1000;
                    Read_Timer.Elapsed += Read_Timer_Tick;
                }
                //실패시
                else
                {
                    ActUtilType.Close();
                    Read_Timer.Stop();
                    Read_Timer.Dispose();

                    msg = new MessageBox_Dialog("로봇 통신 상태를 확인해 주세요.");
                    msg.ShowDialog();
                }
            }
            catch (Exception e)
            {
                LogManager.Error("로봇 통신 에러\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }

        }

        //실시간 로봇데이터 수신
        public void Read_Timer_Tick(object sender, System.Timers.ElapsedEventArgs e) 
        {
            //통신 연결되면
            if (PLC_Connection)
            {
                int _D12031 = 0;
                int _D12131 = 0;
                ActUtilType.GetDevice("D12031", out _D12031);
                ActUtilType.GetDevice("D12131", out _D12131);
                _merc1_cnt = _D12031;
                _merc2_cnt = _D12131;

                // recv_cnt 랑 날짜 업데이트


                //this.Dispatcher.Invoke(DispatcherPriority.Normal
                //, new Action(
                //    delegate
                //    {
                //        Robot_State = rda7.ToString();    //로봇상태
                //        Con_State = rda8.ToString();      //통신상태
                //
                //        State1.BorderBrush = rda7.Equals(1) ? Brushes.Green : Brushes.Red; //설비 가동상태
                //        State2.BorderBrush = rda8.Equals(1) ? Brushes.Green : Brushes.Red; //설비 통신상태
                //
                //        // 값이 0이 아닐때만 전송버튼 활성화
                //        Robot_Send.Background = (WriteData1.Text.Length > 1 & //가로
                //                                 WriteData2.Text.Length > 1 & //세로
                //                                 WriteData3.Text.Length > 1 & //높이
                //                                 WriteData4.Text.Length > 0 & //단수
                //                                 WriteData5.Text.Length > 0 & //간지
                //                                 WriteData6.Text.Length > 0 & //패턴
                //
                //        rda8.Equals(1)) ? Brushes.Green : Brushes.Red; //데이터 전송버튼 활성화
                //
                //        // 현재 로봇 패턴 표시
                //        //switch (rda6.ToString())
                //        //{
                //        //    case "11":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/1종.png", UriKind.Relative)); break;
                //        //    case "21":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/2종.png", UriKind.Relative)); break;
                //        //    case "31":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/3종.png", UriKind.Relative)); break;
                //        //    case "32":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/3-1종.png", UriKind.Relative)); break;
                //        //    case "41":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/4종.png", UriKind.Relative)); break;
                //        //    case "42":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/4-1종.png", UriKind.Relative)); break;
                //        //    case "51":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/5종.png", UriKind.Relative)); break;
                //        //    case "61":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/6종.png", UriKind.Relative)); break;
                //        //    case "62":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/6-1종.png", UriKind.Relative)); break;
                //        //    case "71":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/7종.png", UriKind.Relative)); break;
                //        //    case "81":
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/8종.png", UriKind.Relative)); break;
                //        //    default:
                //        //        top_boximage.Source = new BitmapImage(new Uri(@"/img/White_Back.png", UriKind.Relative)); //패턴값 없으면 공백
                //        //        break;
                //        //}
                //    }));
            }
            else
            {
            }
        }

        //DB에서 데이터 조회
        private void serchRobotData()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spSelIFrobotdata_new]",
               new string[] { "@machine" },
               new object[] { SettingJsonManager.mercName }, 0);

            if (returnData.결과 == 1)
            {

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                JObject Data_Jobject = Data_Jarry.First().ToObject<JObject>();
                ReadData1.Text = Data_Jobject["prodName"].ToString(); //제품명

                switch (Data_Jobject["Pattern"].ToString())
                {
                    case "1종.png"://
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/1종.png")); break;
                    case "2종.png"://
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/2종.png")); break;
                    case "3-1종.png"://
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/3-1종.png")); break;
                    case "4종구11자.png"://
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/4종구11자.png")); break;
                    case "5종.png"://
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/5종.png")); break;
                    case "6-1종.png"://
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/6-1종.png")); break;
                    default:
                        top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/White_Back.png")); break;
                }
            }
            else
            {
                ReadData1.Text = "";
                top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/White_Back.png"));
            }
        }

        private void Data_Send(object sender, RoutedEventArgs e) //데이터 전송 버튼 활성화
        {
            MessageBox_Dialog msg;
            if (Con_State.Equals("1")) //데이터 송신 가능상태 확인
            {
                try
                {
                    int wda1 = 0; //가로
                    int wda2 = 0; //세로
                    int wda3 = 0; //높이
                    int wda4 = 0; //폭
                    int wda5 = 0; //간지
                    int wda6 = 0; //단수

                    wda1 = Convert.ToInt32(WriteData1.Text); //가로
                    wda2 = Convert.ToInt32(WriteData2.Text); //세로
                    wda3 = Convert.ToInt32(WriteData3.Text); //높이
                    wda4 = Convert.ToInt32(WriteData4.Text); //폭
                    wda5 = Convert.ToInt32(WriteData5.Text); //간지
                    wda6 = Convert.ToInt32(WriteData6.Text); //단수

                    ActUtilType.SetDevice("D12000", wda1); //가로
                    ActUtilType.SetDevice("D12001", wda2); //세로
                    ActUtilType.SetDevice("D12002", wda3); //높이
                    ActUtilType.SetDevice("D12003", wda4); //폭
                    ActUtilType.SetDevice("D12004", wda5); //간지
                    ActUtilType.SetDevice("D12005", wda6); //단수

                    Save_robotdata(); //DB 저장 (로봇데이터)

                    msg = new MessageBox_Dialog("전송되었습니다.");
                    msg.Show();
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
        //데이터 값 키인(클릭)
        private void Input_data(object sender, MouseButtonEventArgs e) 
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

                    case "WriteData4": //폭
                        value = int.Parse(WriteData4.Text.ToString());
                        if (value < 1 || value > 25)
                        {
                            msg = new MessageBox_Dialog("폭 값은 1보다 작거나 25보다 클 수 없습니다. ");
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

                    case "WriteData6": //단수
                        value = int.Parse(WriteData5.Text.ToString());
                        if (value < 1 || value > 25)
                        {
                            msg = new MessageBox_Dialog("단수 값은 1보다 작거나 25보다 클 수 없습니다. ");
                            msg.Show();
                            WriteData6.Text = "0";
                        }
                        break;
                }
            }
        }

        //DB 저장 (로봇데이터)
        private void Save_robotdata() 
        {
            MessageBox_Dialog msg;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string[] _tmp = Bottom_Image.Source.ToString().Split('/');
            pattern = _tmp[_tmp.Length - 1].ToString();
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spAEDIFrobotdata_new]",
               new string[] { "@orderKey", "@Machine", "@Pattern", "@BOX_W", "@BOX_H", "@BOX_D", "@BOX_DANSU", "@BOX_BANDING", "@BOX_GANGI", "@PALLET_W", "@PALLET_H", "@PALLET_D",},
               new object[] {
                orderKey,
                SettingJsonManager.mercName,
                pattern, //패턴
                WriteData1.Text.ToString(), //가로
                WriteData2.Text.ToString(), //세로
                WriteData3.Text.ToString(), //높이
                WriteData4.Text.ToString(), //대세 : 폭// ??오주: 다단 
                WriteData5.Text.ToString(), //대세 : 간지// ??오주: 단수 
                WriteData6.Text.ToString() , // 단수 /??오주: 파레트 단수 
                WriteData7.Text.ToString() , // 대세 안씀 /??오주: 파레트 가로 
                WriteData8.Text.ToString() , // 대세 안씀 /??오주: 파레트 세로 
                WriteData9.Text.ToString() , // 대세 안씀 /??오주: 파레트 높이 
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


        private void pat_select_MIT(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Grid"))
            {
                Grid context = (Grid)sender;
                MessageBox_Dialog dialog;

                // 패턴 배경 리셋  
                box1.Background = Brushes.White;
                box2.Background = Brushes.White;
                box3.Background = Brushes.White;
                box4.Background = Brushes.White;
                box5.Background = Brushes.White;
                box6.Background = Brushes.White;
                //box7.Background = Brushes.White;
                //box8.Background = Brushes.White;
                //box9.Background = Brushes.White;
                //box10.Background = Brushes.White;
                //box11.Background = Brushes.White;

                context.Background = Brushes.Green;
                switch (context.Name)
                {
                    case "box1":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/1종.png"));
                        }
                        break;
                    case "box2":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/2종.png"));
                        }
                        break;
                    case "box3":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/3-1종.png"));
                        }
                        break;
                    case "box4":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/4종구11자.png"));
                        }
                        break;
                    case "box5":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/5종.png"));
                        }
                        break;
                    case "box6":
                        if (context.Background == Brushes.Green)
                        {
                            Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/6-1종.png"));
                        }
                        break;
                    //case "box7":
                    //    if (context.Background == Brushes.Green)
                    //    {
                    //        Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/5종.png"));
                    //        pattern = "51";
                    //    }
                    //    break;
                    //case "box8":
                    //    if (context.Background == Brushes.Green)
                    //    {
                    //        Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/6종.png"));
                    //        pattern = "61";
                    //    }
                    //    break;
                    //case "box9":
                    //    if (context.Background == Brushes.Green)
                    //    {
                    //        Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/6-1종.png"));
                    //        pattern = "62";
                    //    }
                    //    break;
                    //case "box10":
                    //    if (context.Background == Brushes.Green)
                    //    {
                    //        Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/7종.png"));
                    //        pattern = "71";
                    //    }
                    //    break;
                    //case "box11":
                    //    if (context.Background == Brushes.Green)
                    //    {
                    //        Bottom_Image.Source = new BitmapImage(new Uri("pack://application:,,,/img/8종.png"));
                    //        pattern = "81";
                    //    }
                    //    break;
                }
            }
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
        private void Exit_window(object sender, EventArgs e)// 창 닫히면
        {
            try
            {
                State3.BorderBrush = Brushes.Red;

                if (PLC_Connection)
                {
                    ActUtilType.Close();
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
                State3.BorderBrush = Brushes.Red;

                if (PLC_Connection)
                {
                    ActUtilType.Close();
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
    }
}
