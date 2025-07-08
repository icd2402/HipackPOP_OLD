using ActUtlTypeLib;
using log4net.Core;
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
using System.Text;
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
    /// 
    /*
            0xF0000001 라이선스 없음 에러 PC 에 라이선스가 부여되지 않았다 . • 라이선스 키 FD 에서 PC 에 라이선스를 부여하십시오 .
            0xF0000002 설정 데이터 읽기 에러 논리 국번의 설정 데이터 읽기에 실패하였다 . • 올바른 논리 국번을 지정하십시오 . • 통신 설정 유틸리티에서 논리 국번을 설정하십시오 .
            0xF0000003 오픈 완료 에러 오픈 상태에서 Open 메소드를 실행하였다 . • 통신 대상 CPU를 변경하는 경우, Close 후 Open 메소드를 실행 하십시오 .
            0xF0000004 미오픈 에러 Open 메소드를 실행하고 있지 않다 . • Open 메소드 실행 후 해당 메소드를 실행하십시오 .                
            0xF0000005 초기화 에러 MX Component 내부 유지 오브젝트의 초기화에 실 패하였다 . • 프로그램을 종료하고 나서 PC 를 재기동하십시오 . • MX Component 를 재인스톨하십시오 .
            0xF0000006 메모리 확보 에러 MX Component 내부 메모리의 확보에 실패하였 다 . • 프로그램을 종료하고 나서 PC 를 재기동하십시오 . • 다른 프로그램을 종료하여 사용 가능 메모리를 확보하십시오 . 
            0x01809001 GX Simulator2 미기동 에러 GX Simulator2 가 기동되어 있지 않다 • GX Simulator2 를 기동하십시오 .
    */

    public partial class Robot_Setting4 : Window
    {
        System.Timers.Timer Read_Timer;
        //bool PLC_Connection = false;
        //ActUtlTypeLib.ActUtlType ActUtilType; //MX Component 라이브러리 선언
        PopProject.Util.PLC_LS_Manager _PLC_LS_Manager; //PLC 통신 매니저

        string Robot_State = ""; //로봇 가동/정지 상태
        string prodID;
        string pattern = ""; //패턴
        string orderKey;
        string gor_H;
        string customsName = ""; //작업명
        string prodName = ""; //품명
        int _merc1_cnt = 0;
        int _merc2_cnt = 0;
        int patton = 0; //패턴 번호
        MainModel mainModel = null; //메인 모델
        

        public Robot_Setting4(MainModel mainModel)
        {
            try
            {
                this.mainModel = mainModel;
                customsName = mainModel.customSName;
                prodName = mainModel.prodName;
                orderKey = mainModel.orderKey;
                InitializeComponent();
                Event_Binding();

                LogWrite("로봇작업");
                // 1.DB에서 마지막 진행한 작업 데이터 조회
                serchRobotData();
                // 2. 로봇 통신 연결
                _PLC_LS_Manager = new PopProject.Util.PLC_LS_Manager();
                _PLC_LS_Manager.PLC_Conn(); //PLC 통신 연결
                // 3.기준정보 바인딩
                serchProdWHD();
                //DataWrite(); //로봇 데이터 쓰기(데이터 전송 터치 시)
                //DataRead(); //실시간 데이터 조회
            }
            catch (Exception e)
            {
                LogWrite(e.Message);
                LogWrite(e.StackTrace);
            }

        }



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
            netSetting.MouseLeftButtonDown += IP_setting;
        }
        #endregion

        private void DataRead()
        {
            MessageBox_Dialog msg;
            try
            {
                //if(PLC_Connection == true)
                //{
                //    Read_Timer.Start();
                //
                //    //1초마다 실시간 통신
                //    Read_Timer = new System.Timers.Timer();
                //    Read_Timer.Interval = 1000;
                //    Read_Timer.Elapsed += Read_Timer_Tick;
                //}
                //    
                //
                //    // 로봇 데이터 읽기 메모리 주소
                //    int _D12000 = 0; // 자동운전모드 체크
                //    int _D12001 = 0; // 적재패턴
                //    int _D12002 = 0; // 모델명
                //    int _D12003 = 0; // 가로
                //    int _D12004 = 0; // 세로
                //    int _D12005 = 0; // 높이
                //    int _D12006 = 0; // 단
                //    int _D12007 = 0; // 간지공급
                //    int _D12008 = 0; // 폭
                //    ActUtilType.GetDevice("D12000", out _D12000);
                //
                //
                //    if (_D12000 == 1)
                //    {
                //        // Base 작업조건 가져와서 업데이트
                //        ActUtilType.GetDevice("D12001", out _D12001);
                //        ActUtilType.GetDevice("D12002", out _D12002);
                //        ActUtilType.GetDevice("D12003", out _D12003);
                //        ActUtilType.GetDevice("D12004", out _D12004);
                //        ActUtilType.GetDevice("D12005", out _D12005);
                //        ActUtilType.GetDevice("D12006", out _D12006);
                //        ActUtilType.GetDevice("D12007", out _D12007);
                //        ActUtilType.GetDevice("D12008", out _D12008);
                //
                //        //업데이트 프로시저 호출
                //
                //    }
                ////실패시
                //else
                //{
                //    ActUtilType.Close();
                //    Read_Timer.Stop();
                //    Read_Timer.Dispose();
                //
                //    msg = new MessageBox_Dialog("로봇 통신 상태를 확인해 주세요.");
                //    msg.ShowDialog();
                //}
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
            if (_PLC_LS_Manager.PLC_Connection)
            {
                int _D12031 = 0;
                int _D12131 = 0;
                _PLC_LS_Manager.ActUtilType.GetDevice("D12031", out _D12031);
                _PLC_LS_Manager.ActUtilType.GetDevice("D12131", out _D12131);
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
        //DB에서 이전 작업 데이터 조회
        private void serchRobotData()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[Select_robotdata_Load_History]",
               new string[] { "@orderkey" },
               new object[] { "" }, 0);

            if (returnData.결과 == 1)
            {

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                JObject Data_Jobject = Data_Jarry.First().ToObject<JObject>();
                workName.Text = Data_Jobject["prodName"].ToString(); //제품명

                switch (Data_Jobject["BOX_PATTON"].ToString())
                {
                    // 여기 case 명칭들 HMI에 저장되는 패턴의 숫자 Key로 변경 필요
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

        private void serchProdWHD()
        {
            // 작업할 제품에 대한 기준정보 조회 및 바인딩
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[Select_robotdata_Load_History]",
               new string[] { "@orderkey" },
               new object[] { SettingJsonManager.pplanKey },
               0);

            if (returnData.결과 == 1)
            {

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                JObject Data_Jobject = Data_Jarry.First().ToObject<JObject>();
                workName.Text = Data_Jobject["PRODNAME"].ToString(); //제품명
                WriteData1.Text = Data_Jobject["BOX_L"].ToString(); //가로
                WriteData2.Text = Data_Jobject["BOX_W"].ToString(); //세로
                WriteData3.Text = Data_Jobject["BOX_H"].ToString(); //높이
                WriteData4.Text = Data_Jobject["BOX_Packing"].ToString(); //폭
                WriteData5.Text = Data_Jobject["BOX_GANGI"].ToString(); //간지
                WriteData6.Text = Data_Jobject["BOX_DANSU"].ToString(); //단수
                WriteData7.Text = Data_Jobject["PALLET_L"].ToString(); //파레트 가로
                WriteData8.Text = Data_Jobject["PALLET_W"].ToString(); //파레트 세로
                WriteData9.Text = Data_Jobject["PALLET_H"].ToString(); //파레트 높이
                // Data_Jobject["PALLET_PATTON"].ToString(); //패턴

            }
            // 이전에 작업한 적이 없는 신규
            else
            {
                ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                returnData = ServiceModel.GetData(
                   SettingJsonManager.serverIndex,
                   "[robotdata_Load_calculate]",
                   new string[] { "@prodID" },
                   new object[] { mainModel.prodID },
                   0);

                if (returnData.결과 == 1)
                {
                    JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                    JObject Data_Jobject = Data_Jarry.First().ToObject<JObject>();
                    workName.Text = Data_Jobject["PRODNAME"].ToString(); //제품명
                    WriteData1.Text = Data_Jobject["LOADW"].ToString(); //가로
                    WriteData2.Text = Data_Jobject["LPADH"].ToString(); //세로
                    WriteData3.Text = Data_Jobject["LOADL"].ToString(); //높이
                }
                else
                {
                    workName.Text = "";
                    top_boximage.Source = new BitmapImage(new Uri("pack://application:,,,/img/White_Back.png"));
                }
            }
        }

        private void Data_Send(object sender, RoutedEventArgs e) //데이터 전송 버튼 활성화
        {
            MessageBox_Dialog msg;
            try
            {
                if (_PLC_LS_Manager.PLC_Connection == false)
                {
                    _PLC_LS_Manager.PLC_Conn(); //PLC 통신 연결
                }
                else
                {
                    if (mainModel.mercName.Equals("톰슨1호기"))
                    {
                        _PLC_LS_Manager.SetDevice("D0001000", 1); //적재 패턴
                        _PLC_LS_Manager.WriteDeviceBlock("D001001", workName.Text); // 모델명
                        _PLC_LS_Manager.SetDevice("D0001010", Convert.ToInt32(WriteData1.Text)); //가로
                        _PLC_LS_Manager.SetDevice("D0001011", Convert.ToInt32(WriteData2.Text)); //세로
                        _PLC_LS_Manager.SetDevice("D0001012", Convert.ToInt32(WriteData3.Text)); //높이
                        _PLC_LS_Manager.SetDevice("D0001013", Convert.ToInt32(WriteData6.Text)); //단
                        _PLC_LS_Manager.SetDevice("D0001014", Convert.ToInt32(WriteData5.Text)); //간지
                        _PLC_LS_Manager.SetDevice("D0001015", Convert.ToInt32(WriteData4.Text)); //폭
                    }
                    else if (mainModel.mercName.Equals("톰슨2호기"))
                    {
                        _PLC_LS_Manager.SetDevice("D0001050", 1); //적재 패턴
                        _PLC_LS_Manager.WriteDeviceBlock("D0001051", workName.Text); // 모델명
                        _PLC_LS_Manager.SetDevice("D0001060", Convert.ToInt32(WriteData1.Text)); //가로
                        _PLC_LS_Manager.SetDevice("D0001061", Convert.ToInt32(WriteData2.Text)); //세로
                        _PLC_LS_Manager.SetDevice("D0001062", Convert.ToInt32(WriteData3.Text)); //높이
                        _PLC_LS_Manager.SetDevice("D0001063", Convert.ToInt32(WriteData6.Text)); //단
                        _PLC_LS_Manager.SetDevice("D0001064", Convert.ToInt32(WriteData5.Text)); //간지
                        _PLC_LS_Manager.SetDevice("D0001065", Convert.ToInt32(WriteData4.Text)); //폭
                    }
                }
                msg = new MessageBox_Dialog("전송되었습니다.");
                msg.Show();
            }
            catch (Exception ex)
            {
                msg = new MessageBox_Dialog("재시도 해주세요.");
                msg.Show();
            }
            Save_robotdata(); //DB 저장 (로봇데이터)
            //여기서 베이스로 저장해야함
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
            switch (Bottom_Image.Name)
            {
                // 여기 case 명칭들 HMI에 저장되는 패턴의 숫자 Key로 변경 필요
                case "1종.png"://
                    patton = 10; break;
                case "2종.png"://
                    patton = 11; break;
                case "3-1종.png"://
                    patton = 12; break;
                case "4종구11자.png"://
                    patton = 13; break;
                case "5종.png"://
                    patton = 14; break;
                case "6-1종.png"://
                    patton = 15; break;
                default:
                    patton = 16; break;
            }
            MessageBox_Dialog msg;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[Insert_robotdata_Load_Info]",
               new string[] { "@prodID", "@mercName", "@BOX_L", "@BOX_W", "@BOX_H", "@BOX_Packing",
                   "@BOX_DANSU", "@BOX_PATTON", "@BOX_GANGI", "@PALLET_L", "@PALLET_W", "@PALLET_H",},
               new object[] {
                mainModel.prodID,
                SettingJsonManager.mercName,
                WriteData1.Text.ToString(), //가로
                WriteData2.Text.ToString(), //세로
                WriteData3.Text.ToString(), //높이
                WriteData4.Text.ToString(), //폭
                WriteData5.Text.ToString(), //단수
                patton, //패턴
                WriteData6.Text.ToString() , //간지
                WriteData7.Text.ToString() , //파렛트 가로
                WriteData8.Text.ToString() , //파렛트 세로
                WriteData9.Text.ToString() , //파렛트 높이
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

                if (_PLC_LS_Manager.PLC_Connection)
                {
                    _PLC_LS_Manager.ActUtilType.Close();
                }

                if (Read_Timer != null)
                {
                    Read_Timer.Stop();
                    Read_Timer.Dispose();
                }
                _PLC_LS_Manager.PLC_Connection = false;
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

                if (_PLC_LS_Manager.PLC_Connection)
                {
                    _PLC_LS_Manager.ActUtilType.Close();
                }

                if (Read_Timer != null)
                {
                    Read_Timer.Stop();
                    Read_Timer.Dispose();
                }
                _PLC_LS_Manager.PLC_Connection = false;
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
