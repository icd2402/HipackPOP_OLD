using Newtonsoft.Json.Linq;
using PopProject.Detail;
using PopProject.input;
using PopProject.KeyBoard;
using PopProject.KeyBoard2;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using log4net;
using log4net.Config;

namespace PopProject.Input
{
    /// <summary>
    /// Plc_Pop.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Plc_Pop : Window
    {
        string[] psize; //규격 배열
        MainModel mainModel;
        public InputModel inputModel;
        Socket mainSock;
        Win_KeyBoard win_KeyBoard; //일반 키보드
        Win_KeyBoard2 win_KeyBoard2; //소숫점 키보드
        private bool checkChangeButton = false;
        private bool CNS_Send = false;
        public ILog log = LogManager.GetLogger(typeof(Plc_Pop)); //로그



        public Plc_Pop(MainModel mainModel)
        {
            InitializeComponent();
            //Log4net
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));
            this.mainModel = mainModel;
            SetPlcData();


            //if (SettingJsonManager.corp_name.Equals("오주포장") && SettingJsonManager.mercName.Equals("인라인2호기"))
            //{
            //    tb1_.Text = "A 위치";
            //    tb2_.Text = "B 위치";
            //    tb3_.Text = "C 위치";
            //    tb4_.Text = "D 위치";
            //    tb5_.Text = "E 위치";
            //    tb6_.Text = "F1 위치";
            //    tb7_.Text = "F2 위치";
            //    tb8_.Text = "G 위치";
            //    tb9_.Text = "O 위치";
            //    tb10_.Text = "P1";
            //    tb11_.Text = "P2";
            //    tb12_.Text = "P3";

            //    tb13_.Text = "P4";
            //    tb14_.Text = "SPARE";
            //    tb15_.Text = "SPARE";
            //    tb16_.Text = "슬로타";
            //    tb17_.Text = "다이캇타";
            //    tb18_.Text = "예약";
            //    tb19_.Text = "1도 프레스";
            //    tb20_.Text = "예약";
            //    tb21_.Text = "2도 프레스";
            //    tb22_.Text = "예약";
            //    tb23_.Text = "3도프레스";
            //    tb24_.Text = "예약";
                            
            //    tb25_.Text = "4도 프레스";
            //    tb26_.Text = "";
            //    tb27_.Text = "";
            //    tb28_.Text = "";
            //    tb29_.Text = "";
            //    tb30_.Text = "";
            //    tb31_.Text = "";
            //    tb32_.Text = "";
            //    tb33_.Text = "";
            //    tb34_.Text = "";
            //    tb35_.Text = "";
            //    tb36_.Text = "";
                            
            //    tb37_.Text = "";
            //    tb38_.Text = "";
            //    tb39_.Text = "";
            //    tb40_.Text = "";
            //    tb41_.Text = "";
            //    tb42_.Text = "";
            //    tb43_.Text = "";
            //    tb44_.Text = "";
            //    tb45_.Text = "";
            //    tb46_.Text = "";
            //    tb47_.Text = "";
            //}

        }

        //초기화면 세팅
        private void SetPlcData()
        {
            ip.Text = SettingJsonManager.ip + " / " + SettingJsonManager.port; //IP
            거래처.Text = mainModel.customSName; //거래처
            품명.Text = mainModel.prodName; //품명
            psize = mainModel.pSize.Split('*'); //규격
            작업지시.Text = SettingJsonManager.setCount.ToString();
            장.Text = psize[0] == null ? "" : psize[0].Trim();
            폭.Text = psize[1] == null ? "" : psize[1].Trim();
            고.Text = psize[2] == null ? "" : psize[2].Trim();
            
            미미.Text = SettingJsonManager.mimi.ToString();
            switch (mainModel.paperGor.Replace("골", "").Trim())
            {
                case "AB":
                case "BA":
                    SettingJsonManager.Gap = 70;
                    break;

                case "BB":
                    SettingJsonManager.Gap = 60;
                    break;

                case "A":
                    SettingJsonManager.Gap = 50;
                    break;

                case "EB":
                    SettingJsonManager.Gap = 40;
                    break;

                case "B":
                    SettingJsonManager.Gap = 30;
                    break;

                case "E":
                    SettingJsonManager.Gap = 20;
                    break;
                default:
                    break;
            }
            갭.Text = SettingJsonManager.Gap.ToString();

        }

        //PLC IP 세팅
        private void ip_input(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Ip_input ip_Input = new Ip_input();
            ip_Input.ShowDialog();
            ip.Text = SettingJsonManager.ip + " / " + SettingJsonManager.port;
        }

        //DB데이터 조회
        private void DB_Read(object sender, MouseButtonEventArgs e)
        {
            log.Info("CNS 상세창에서 PC조회 버튼 클릭");

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopCNSSearch]",
               new string[] { "prodID", "MachineName" },
               new object[] { this.mainModel.prodID, SettingJsonManager.mercName }, 0);

            if (returnData.결과 == 1)
            {
                string ProdID = "";
                string machineName = "";
                int modelNum = 0;
                int workPlan1 = 0;
                int workPlan2 = 0;
                int boxType = 0;
                int pock = 0;
                int jang = 0;
                int go = 0;
                int gap = 0;
                int wing = 0;
                int cutting = 0;
                int saveDone = 0;
                int workSpeed = 0;
                int saveState = 0;
                int spare = 0;
                int aRecipe = 0;
                int bRecipe = 0;
                int cRecipe = 0;
                int dRecipe = 0;
                int eRecipe = 0;
                int f1Recipe = 0;
                int f2Recipe = 0;
                int gRecipe = 0;
                int oRecipe = 0;
                int p1Recipe = 0;
                int p2pRecipe = 0;
                int p3Recipe = 0;
                int p4Recipe = 0;
                int spare2 = 0;
                int spare3 = 0;
                int spare4 = 0;
                int dRagista = 0;
                int printerPull1 = 0;
                int printerRoll1 = 0;
                int printerPull2 = 0;
                int printerRoll2 = 0;
                int printerPull3 = 0;
                int printerRoll3 = 0;
                int printerPull4 = 0;
                int printerRoll4 = 0;
                int spare6 = 0;
                int spare7 = 0;
                int spare8 = 0;
                int spare9 = 0;
                int feederGap = 0;
                int feederRoll = 0;
                int slotterSubGap = 0;
                int slotterMiddleGap = 0;
                int slotterConnection = 0;
                int dieCutterRollGap = 0;
                int dieCutterConnectionGap = 0;
                int spare10 = 0;
                int gluerbeltF1 = 0;
                int gluerbeltF2 = 0;
                int gluerPressA = 0;
                int gluerPressB = 0;
                int gluerPE = 0;
                int spare11 = 0;
                int bRSub = 0;
                int bRMain = 0;
                int bRConnection = 0;
                int bRmiddle = 0;

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach (JObject result in Data_Jarry)
                {
                    ProdID = result["prodID"].ToString();
                    machineName = result["MachineName"].ToString();

                    modelNum = Convert.ToInt32(result["ModelNum"]);
                    workPlan1 = Convert.ToInt32(result["WorkPlan1"]);
                    workPlan2 = Convert.ToInt32(result["WorkPlan2"]);
                    boxType = Convert.ToInt32(result["BoxType"]);
                    pock = Convert.ToInt32(result["Pock"]);
                    jang = Convert.ToInt32(result["Jang"]);
                    go = Convert.ToInt32(result["Go"]);
                    gap = Convert.ToInt32(result["Gap"]);
                    wing = Convert.ToInt32(result["Wing"]);
                    cutting = Convert.ToInt32(result["Cutting"]);
                    saveDone = Convert.ToInt32(result["SaveDone"]);
                    workSpeed = Convert.ToInt32(result["WorkSpeed"]);
                    saveState = Convert.ToInt32(result["SaveState"]);
                    spare = Convert.ToInt32(result["spare"]);

                    aRecipe = Convert.ToInt32(result["aRecipe"]);     //A
                    bRecipe = Convert.ToInt32(result["bRecipe"]);     //B
                    cRecipe = Convert.ToInt32(result["cRecipe"]);     //C
                    dRecipe = Convert.ToInt32(result["dRecipe"]);     //D
                    eRecipe = Convert.ToInt32(result["eRecipe"]);     //E
                    f1Recipe = Convert.ToInt32(result["f1Recipe"]);   //F1
                    f2Recipe = Convert.ToInt32(result["f2Recipe"]);   //F2
                    gRecipe = Convert.ToInt32(result["g2pRecipe"]);   //G
                    oRecipe = Convert.ToInt32(result["oRecipe"]);     //O

                    p1Recipe = Convert.ToInt32(result["p1Recipe"]);   //P1
                    p2pRecipe = Convert.ToInt32(result["p2pRecipe"]); //P2
                    p3Recipe = Convert.ToInt32(result["p3Recipe"]);   //P3
                    p4Recipe = Convert.ToInt32(result["p4Recipe"]);   //P4

                    dRagista = Convert.ToInt32(result["DieCutter"]);  //D레지스터

                    printerPull1 = Convert.ToInt32(result["PrinterRoll1"]);  //1도 롤
                    printerRoll1 = Convert.ToInt32(result["PrinterPull1"]);  //1도 
                    printerPull2 = Convert.ToInt32(result["PrinterRoll2"]);  //2도 롤
                    printerRoll2 = Convert.ToInt32(result["PrinterPull2"]);  //2도 
                    printerPull3 = Convert.ToInt32(result["PrinterRoll3"]);  //3도 롤
                    printerRoll3 = Convert.ToInt32(result["PrinterPull3"]);  //3도 
                    printerPull4 = Convert.ToInt32(result["PrinterRoll4"]);  //4도 롤
                    printerRoll4 = Convert.ToInt32(result["PrinterPull4"]);  //4도

                    feederGap = Convert.ToInt32(result["FeederGap"]);        //급지부 진입 갭
                    feederRoll = Convert.ToInt32(result["FeederRoll"]);      //급지부 진입 롤갭

                    slotterSubGap = Convert.ToInt32(result["SlotterSubGap"]);         //슬로다 보조 갭
                    slotterMiddleGap = Convert.ToInt32(result["SlotterMiddleGap"]);   //슬로다 본 갭
                    slotterConnection = Convert.ToInt32(result["SlotterConnection"]); //슬로다 연결갭

                    dieCutterRollGap = Convert.ToInt32(result["DieCutterRollGap"]);   //다이캇타 롤 갭
                    dieCutterConnectionGap = Convert.ToInt32(result["GluerbeltF"]);   //다이캇타 연결 갭

                    gluerbeltF1 = Convert.ToInt32(result["GluerPressA"]);     //글루어벨트 진입 갭 F1
                    gluerbeltF2 = Convert.ToInt32(result["GluerPressB"]);     //글루어벨트 진입 갭 F2
                    gluerPressA = Convert.ToInt32(result["DieCutterRollA"]);  //글루어 프레스롤 A
                    gluerPressB = Convert.ToInt32(result["DieCutterRollB"]);  //글루어 프레스롤 B
                    gluerPE = Convert.ToInt32(result["GluerPE"]);             //글루어 PE

                    bRSub = Convert.ToInt32(result["BRMain"]);           //보조괘선 갭  
                    bRMain = Convert.ToInt32(result["BRConnection"]);    //본괘선 갭
                    bRConnection = Convert.ToInt32(result["BRmiddle"]);  //연결롤 갭
                    bRmiddle = Convert.ToInt32(result["spare8"]);        //중간롤 갭
                }

                //박스타입 선택

                
                int a = boxType;
                switch (a)
                {
                    case 1:
                        combo.SelectedIndex = 0; //1합
                        break;
                    case 3:
                        combo.SelectedIndex = 1; //2합
                        break;
                    case 5:
                        combo.SelectedIndex = 2; //인쇄물
                        break;
                    case 6:
                        combo.SelectedIndex = 3; //H형
                        break;
                    case 7:
                        combo.SelectedIndex = 4; //H-Cover형
                        break;
                }

                장.Text = jang.ToString();
                폭.Text = pock.ToString();
                고.Text = go.ToString();
                갭.Text = gap.ToString();
                미미.Text = cutting.ToString();

                tb1.Text = (aRecipe / 10.0).ToString("N1");
                tb2.Text = (bRecipe / 10.0).ToString("N1");
                tb3.Text = (cRecipe / 10.0).ToString("N1");
                tb4.Text = (dRecipe / 10.0).ToString("N1");
                tb5.Text = (eRecipe / 10.0).ToString("N1");
                tb6.Text = (f1Recipe / 10.0).ToString("N1");
                tb7.Text = (f2Recipe / 10.0).ToString("N1");
                tb8.Text = (gRecipe / 10.0).ToString("N1");
                tb9.Text = (oRecipe / 10.0).ToString("N1");
                tb10.Text = (p1Recipe / 10.0).ToString("N1");
                tb11.Text = (p2pRecipe / 10.0).ToString("N1");
                tb12.Text = (p3Recipe / 10.0).ToString("N1");

                tb13.Text = (p4Recipe / 10.0).ToString("N1");
                tb14.Text = "0";
                tb15.Text = "0";
                tb16.Text = "0";
                tb17.Text = (dRagista / 10.0).ToString("N1");
                tb18.Text = (printerPull1 / 10.0).ToString("N1");
                tb19.Text = (printerRoll1 / 10.0).ToString("N1");
                tb20.Text = (printerPull2 / 10.0).ToString("N1");
                tb21.Text = (printerRoll2 / 10.0).ToString("N1");
                tb22.Text = (printerPull3 / 10.0).ToString("N1");
                tb23.Text = (printerRoll3 / 10.0).ToString("N1");
                tb24.Text = (printerPull4 / 10.0).ToString("N1");

                tb25.Text = (printerRoll4 / 10.0).ToString("N1");
                tb26.Text = "0";
                tb27.Text = "0";
                tb28.Text = "0";
                tb29.Text = "0";
                tb30.Text = (feederGap / 10.0).ToString("N1");
                tb31.Text = (feederRoll / 10.0).ToString("N1");
                tb32.Text = (slotterSubGap / 10.0).ToString("N1");
                tb33.Text = (slotterMiddleGap / 10.0).ToString("N1");
                tb34.Text = (slotterConnection / 10.0).ToString("N1");
                tb35.Text = (dieCutterRollGap / 10.0).ToString("N1");
                tb36.Text = (dieCutterConnectionGap / 10.0).ToString("N1");

                tb37.Text = "0";
                tb38.Text = (gluerbeltF1 / 10.0).ToString("N1");
                tb39.Text = (gluerbeltF2 / 10.0).ToString("N1");
                tb40.Text = (gluerPressA / 10.0).ToString("N1");
                tb41.Text = (gluerPressB / 10.0).ToString("N1");
                tb42.Text = (gluerPE / 10.0).ToString("N1");
                tb43.Text = "0";
                tb44.Text = (bRSub / 10.0).ToString("N1");
                tb45.Text = (bRMain / 10.0).ToString("N1");
                tb46.Text = (bRConnection / 10.0).ToString("N1");
                tb47.Text = (bRmiddle / 10.0).ToString("N1");
            }
            else
            {
            }
        }


        private void SingleWrite() //데이터를 읽기 위한 쓰기
        {
            CNS_Send = true;

            ConnectSocket(); //CNS 통신 연결
            Ls_Fenet wd = new Ls_Fenet(); //헤더
            byte[] bDts2 = wd.single_Write("%DW3814", 02, 1); //개별쓰기 (변수명, 데이터 타입, 쓸 데이터값) 
            mainSock.Send(bDts2); //데이터 쓰기
            mainSock.Close();

            CNS_Send = false;
        }


        //CNS 데이터 조회 버튼
        private void PLC_Read(object sender, MouseButtonEventArgs e)
        {
            log.Info("CNS 상세창에서 CNS데이터 조회 버튼 클릭");

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            try
            {
                SingleWrite(); //현재 설정을 읽기 위해서는 D3814번에 0이 아닌 수를 넣어줘야함
                ConnectSocket(); //CNS 통신 연결
                Ls_Fenet wd = new Ls_Fenet(); //헤더

                byte[] bDts = wd.Word_continuous_Read("%DB7804", 71); //연속읽기 D3902 ~ D3972 (71개)
                
                //송신한 데이터 값 보는용도
                string Send_Data = string.Empty;
                for (int i = 0; i < bDts.Length; i++)
                {
                    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                }
                Console.WriteLine("PLC송신 데이터(Plc_Pop) : " + Send_Data);

                mainSock.Send(bDts); //데이터 쓰기

                log.Info("CNS 상세창에서 CNS데이터 조회 요청 내용 : " + Send_Data);
            }
            catch (Exception)
            {
                CNS_Send = false;
                log.Info("CNS 상세창에서 CNS데이터 조회 실패");
            }
            CNS_Send = false;
        }



        private void ConnectSocket()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            if (!mainSock.Connected)
            {
                int port = int.Parse(SettingJsonManager.port); //포트 2004
                try
                {
                    IAsyncResult result = mainSock.BeginConnect(SettingJsonManager.ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(1500, true);

                    if (CNS_Send)
                    {
                    }
                    else

                    {
                        if (success)//연결 성공
                        {
                            mainSock.EndConnect(result);
                            AsyncObject obj = new AsyncObject(500);
                            obj.WorkingSocket = mainSock;
                            obj.ClearBuffer();
                            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                        }
                        else //연결 실패
                        {
                            mainSock.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    mainSock.Close();
                }
            }
        }

        public void DataReceived(IAsyncResult ar) // 데이터 수신
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 수신한 데이터 16진수로 변환
                string Receive_Data = string.Empty;
                for (int i = 0; i < obj.Buffer.Length; i++)
                {
                    Receive_Data += obj.Buffer[i].ToString("x2");
                }

                //Word 연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                Console.WriteLine("PLC수신 데이터(Plc_Pop) : " + RD);

                string dummyData = "";

                this.Dispatcher.Invoke(() =>
                {
                    for (int j = 0; j < RD.Length; j++)
                    {
                        string high_data = RD.Substring(0, 2); //앞 두자리 추출
                        string low_data = RD.Substring(2, 2); //뒤 두자리 추출

                        switch (j)
                        {
                            case 0: //작업모델 번호
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 1: //작업지시 매수
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 2: //작업지시 매수
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 3: //박스타입
                                int boxtype = Convert.ToInt32(low_data + high_data, 16);
                                if (boxtype == 1)
                                {
                                    combo.SelectedIndex = 0;
                                }
                                else if (boxtype == 3)
                                {
                                    combo.SelectedIndex = 1;
                                }
                                else if (boxtype == 5)
                                {
                                    combo.SelectedIndex = 2;
                                }
                                else if (boxtype == 6)
                                {
                                    combo.SelectedIndex = 3;
                                }
                                else if (boxtype == 7)
                                {
                                    combo.SelectedIndex = 4;
                                }
                                break;
                            case 4: //폭
                                폭.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 5: //장
                                장.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 6: //고
                                고.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 7: //갭
                                갭.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 8: //앞날개
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 9: //미미
                                미미.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 10: //저장 완료
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 11: //생산속도
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 12: //저장값 쓰기 가능상태
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 13: //spare
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 14: //A
                                double result1 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb1.Text = result1.ToString("N1");
                                break;
                            case 15: //B
                                double result2 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb2.Text = result2.ToString("N1");
                                break;
                            case 16: //C
                                double result3 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb3.Text = result3.ToString("N1");
                                break;
                            case 17: //D
                                double result4 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb4.Text = result4.ToString("N1");
                                break;
                            case 18: //E
                                double result5 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb5.Text = result5.ToString("N1");
                                break;
                            case 19: //F1
                                double result6 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb6.Text = result6.ToString("N1");
                                break;
                            case 20: //F2
                                double result7 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb7.Text = result7.ToString("N1");
                                break;
                            case 21: //G
                                double result8 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb8.Text = result8.ToString("N1");
                                break;
                            case 22: //O
                                double result9 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb9.Text = result9.ToString("N1");
                                break;
                            case 23: //P1
                                double result10 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb10.Text = result10.ToString("N1");
                                break;
                            case 24: //P2
                                double result11 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb11.Text = result11.ToString("N1");
                                break;
                            case 25: //P3
                                double result12 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb12.Text = result12.ToString("N1");
                                break;
                            case 26: //P4
                                double result13 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb13.Text = result13.ToString("N1");
                                break;
                            case 27: //spare
                                double result14 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb14.Text = result14.ToString("N1");
                                break;
                            case 28: //spare
                                double result15 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb15.Text = result15.ToString("N1");
                                break;
                            case 29: //spare
                                double result16 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb16.Text = result16.ToString("N1");
                                break;
                            case 30: //D레지스터
                                double result17 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb17.Text = result17.ToString("N1");
                                break;
                            case 31: //프린터1 롤?
                                double result18 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb18.Text = result18.ToString("N1");
                                break;
                            case 32: //1도 프레스
                                double result19 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb19.Text = result19.ToString("N1");
                                break;
                            case 33: //프린터2 롤?
                                double result20 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb20.Text = result20.ToString("N1");
                                break;
                            case 34: //2도 프레스
                                double result21 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb21.Text = result21.ToString("N1");
                                break;
                            case 35: //프린터3 롤?
                                double result22 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb22.Text = result22.ToString("N1");
                                break;
                            case 36: //3도 프레스
                                double result23 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb23.Text = result23.ToString("N1");
                                break;
                            case 37: //프린터4 롤?
                                double result24 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb24.Text = result24.ToString("N1");
                                break;
                            case 38: //4도 프레스
                                double result25 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb25.Text = result25.ToString("N1");
                                break;
                            case 39: //spare
                                double result26 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb26.Text = result26.ToString("N1");
                                break;
                            case 40: //spare
                                double result27 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb27.Text = result27.ToString("N1");
                                break;
                            case 41: //spare
                                double result28 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb28.Text = result28.ToString("N1");
                                break;
                            case 42: //spare
                                double result29 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb29.Text = result29.ToString("N1");
                                break;
                            case 43: //피타진입 갭
                                double result30 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb30.Text = result30.ToString("N1");
                                break;
                            case 44: //급지부 롤 갭
                                double result31 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb31.Text = result31.ToString("N1");
                                break;
                            case 45: //슬로타 보조괘선 갭
                                double result32 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb32.Text = result32.ToString("N1");
                                break;
                            case 46: //슬로타 본괘선 갭
                                double result33 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb33.Text = result33.ToString("N1");
                                break;
                            case 47: //슬로타 연결 갭
                                double result34 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb34.Text = result34.ToString("N1");
                                break;
                            case 48: //다이캇타 롤 갭
                                double result35 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb35.Text = result35.ToString("N1");
                                break;
                            case 49: //다이캇타 연결 갭
                                double result36 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb36.Text = result36.ToString("N1");
                                break;
                            case 50: //spare
                                double result37 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb37.Text = result37.ToString("N1");
                                break;
                            case 51: //글루어벨트 진입갭 F1
                                double result38 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb38.Text = result38.ToString("N1");
                                break;
                            case 52: //글루어벨트 진입갭 F2
                                double result39 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb39.Text = result39.ToString("N1");
                                break;
                            case 53: //글루어 배출 갭 프레스 A
                                double result40 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb40.Text = result40.ToString("N1");
                                break;
                            case 54: //글루어 배출 갭 프레스 B
                                double result41 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb41.Text = result41.ToString("N1");
                                break;
                            case 55: //PE가이드
                                double result42 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb42.Text = result42.ToString("N1");
                                break;
                            case 56: //spare
                                double result43 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb43.Text = result43.ToString("N1");
                                break;
                            case 57: //R/B 보조괘선 갭
                                double result44 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb44.Text = result44.ToString("N1");
                                break;
                            case 58: //R/B 본괘선 갭
                                double result45 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb45.Text = result45.ToString("N1");
                                break;
                            case 59: //R/B 연결롤 갭
                                double result46 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb46.Text = result46.ToString("N1");
                                break;
                            case 60: //R/B 중간연결 갭
                                double result47 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb47.Text = result47.ToString("N1");
                                break;


                            default:
                                break;
                        }
                        continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                        RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                    }
                });

                Console.WriteLine("CNS PLC수신 데이터 확인(Plc_Pop) : " + continuous_read); //수신값 확인용
            }
            catch (Exception)
            {
                //MessageBox_Dialog messageBox = new MessageBox_Dialog("레시피 읽기 실패");
                //_ = messageBox.ShowDialog();
                mainSock.Close();
            }
            finally
            {
                mainSock.Close();
            }

        }



        //신규 생산 버튼
        private void Done(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            log.Info("CNS 상세창에서 신규생산 버튼 클릭");

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            MessageBoxResult result = System.Windows.MessageBox.Show(" 신규 생산시 기본정보만 전송됩니다.\n ※장,폭,고,갭,미미 외 세팅값은 0으로 처리됩니다." , "CNS 데이터 저장", MessageBoxButton.YesNo);

            clickedBorder.Background = Brushes.White;

            if (result == MessageBoxResult.Yes) //확인
            {
                //박스타입 선택
                int a = combo.SelectedIndex;
                int boxType = 1;
                switch (a)
                {
                    case 0:
                        boxType = 1; //1합
                        break;
                    case 1:
                        boxType = 3; //2합
                        break;
                    case 2:
                        boxType = 5; //인쇄물
                        break;
                    case 3:
                        boxType = 6; //H형
                        break;
                    case 4:
                        boxType = 7; //H-Cover형
                        break;
                }

                SettingJsonManager.Gap = int.Parse(갭.Text.ToString());
                SettingJsonManager.mimi = int.Parse(미미.Text.ToString());
                SettingJsonManager.jang = int.Parse(장.Text.ToString());
                SettingJsonManager.pok = int.Parse(폭.Text.ToString());
                SettingJsonManager.go = int.Parse(고.Text.ToString());
                SettingJsonManager.setCount = int.Parse(작업지시.Text.ToString());
                SettingJsonManager.boxType = int.Parse(boxType.ToString());
                SettingJsonManager.plcCheck = true;
                SettingJsonManager.SaveJsonFile();



                try
                {
                    ConnectSocket(); //CNS 통신연결
                    Ls_Fenet wd = new Ls_Fenet();
                    List<int> items = new List<int>();
                    items.Add(99);  //설정 변경요구 (99로 요청하면 1로바뀜)
                    items.Add(0);   //설정 byte[] bDts = wd.continuous_Read("%DB7800", 70); 요구 (0=> 신규설정요구, 1=>데이터 설정 변경요구
                    items.Add(1); //작업 모델 번호
                    items.Add(10000); //작업지시 매수(하위)
                    items.Add(0); //작업지시 매수(상위)
                    items.Add(SettingJsonManager.boxType); //BOX TYPE (1=>1합, 3=>2합, 5=>인쇄물, 6=>H형, 7=>H-Cover형 
                    items.Add(SettingJsonManager.pok);  //폭
                    items.Add(SettingJsonManager.jang); //장
                    items.Add(SettingJsonManager.go);   //고
                    items.Add(SettingJsonManager.Gap);  //갭
                    items.Add(0);                                //앞날개
                    items.Add(SettingJsonManager.mimi); //미미

                    int WriteCount = items.Count() * 2;
                    byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items);
                   
                    //송신한 데이터 값 보는용도
                    string Send_Data = string.Empty;
                    for (int i = 0; i < bDts.Length; i++)
                    {
                        Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    }
                    Console.WriteLine("신규생산 송신데이터(Plc_Pop) : " + Send_Data);

                    //데이터 쓰기
                    mainSock.Send(bDts);
                }
                catch (Exception)
                {
                }
                finally
                {
                    mainSock.Close();
                }

            }
            else //취소
            {
            }

            Close();

        }

        //값 넣기 이벤트
        private void Input_value(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 10,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }

        //PLC세팅 단축키 F1
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "F1":
                    Ip_input ip_Input = new Ip_input();
                    ip_Input.ShowDialog();
                    ip.Text = SettingJsonManager.ip + " / " + SettingJsonManager.port;
                    break;
                default:
                    break;
            }
        }

        //확장창 값 넣기 이벤트
        private void Input_value2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard2 = new Win_KeyBoard2(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 400,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard2.Opacity = 1;
                _ = win_KeyBoard2.ShowDialog();
                win_KeyBoard2 = null;
            }
        }

        //화장 창 버튼
        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkChangeButton)
            {
                changeButtonIn.Visibility = Visibility.Collapsed;
                changeButtonOut.Visibility = Visibility.Visible;
                checkChangeButton = false;
                Width = 320;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
            else
            {
                changeButtonIn.Visibility = Visibility.Visible;
                changeButtonOut.Visibility = Visibility.Collapsed;
                checkChangeButton = true;
                Width = 1105;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.White;
        }

        //닫기 버튼
        private void exit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            log.Info("CNS 상세창 종료");

            SettingJsonManager.plcCheck = false;
            SettingJsonManager.SaveJsonFile();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            changeButtonIn.Visibility = Visibility.Visible;
            changeButtonOut.Visibility = Visibility.Collapsed;
            checkChangeButton = true;
            Width = 1105;
            Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
        }
    }
}
