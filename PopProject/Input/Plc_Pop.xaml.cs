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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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

        public Plc_Pop(MainModel mainModel)
        {
            InitializeComponent();
            //Log4net
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
            LogManager.Info("CNS 상세창에서 PC조회 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            Border clickedBorder = (Border)sender;
            clickedBorder.Background = Brushes.Gray;

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopCNSSearch]",
               new string[] { "prodID", "MachineName" },
               new object[] { this.mainModel.prodID, SettingJsonManager.mercName }, 
               0);

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
                float aRecipe = 0;
                float bRecipe = 0;
                float cRecipe = 0;
                float dRecipe = 0;
                float eRecipe = 0;
                float f1Recipe = 0;
                float f2Recipe = 0;
                float gRecipe = 0;
                float oRecipe = 0;
                float p1Recipe = 0;
                float p2pRecipe = 0;
                float p3Recipe = 0;
                float p4Recipe = 0;
                float spare2 = 0;
                float spare3 = 0;
                float slotter = 0;
                float dRagista = 0;
                float printerPull1 = 0;
                float printerRoll1 = 0;
                float printerPull2 = 0;
                float printerRoll2 = 0;
                float printerPull3 = 0;
                float printerRoll3 = 0;
                float printerPull4 = 0;
                float printerRoll4 = 0;
                float spare4 = 0;
                float spare5 = 0;
                float spare6 = 0;
                float spare7 = 0;
                float feederGap = 0;
                float feederRoll = 0;
                float slotterSubGap = 0;
                float slotterMiddleGap = 0;
                float slotterConnection = 0;
                float dieCutterRollGap = 0;
                float dieCutterConnectionGap = 0;
                float dummy1 = 0;
                float gluerbeltF1 = 0;
                float gluerbeltF2 = 0;
                float gluerPressA = 0;
                float gluerPressB = 0;
                float gluerPE = 0;
                float dummy2 = 0;
                float bRSub = 0;
                float bRMain = 0;
                float bRConnection = 0;
                float bRmiddle = 0;

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

                    aRecipe = Convert.ToSingle(result["aRecipe"]);     //A
                    bRecipe = Convert.ToSingle(result["bRecipe"]);     //B
                    cRecipe = Convert.ToSingle(result["cRecipe"]);     //C
                    dRecipe = Convert.ToSingle(result["dRecipe"]);     //D
                    eRecipe = Convert.ToSingle(result["eRecipe"]);     //E
                    f1Recipe = Convert.ToSingle(result["f1Recipe"]);   //F1
                    f2Recipe = Convert.ToSingle(result["f2Recipe"]);   //F2
                    gRecipe = Convert.ToSingle(result["g2pRecipe"]);   //G
                    oRecipe = Convert.ToSingle(result["oRecipe"]);     //O

                    p1Recipe = Convert.ToSingle(result["p1Recipe"]);   //P1
                    p2pRecipe = Convert.ToSingle(result["p2pRecipe"]); //P2
                    p3Recipe = Convert.ToSingle(result["p3Recipe"]);   //P3
                    p4Recipe = Convert.ToSingle(result["p4Recipe"]);   //P4

                    spare2 = Convert.ToSingle(result["spare2"]);
                    spare3 = Convert.ToSingle(result["spare3"]);
                    slotter = Convert.ToSingle(result["Slotter"]);

                    dRagista = Convert.ToSingle(result["DieCutter"]);  //D레지스터

                    printerPull1 = Convert.ToSingle(result["PrinterPull1"]);  //1도 롤
                    printerRoll1 = Convert.ToSingle(result["PrinterRoll1"]);  //1도 
                    printerPull2 = Convert.ToSingle(result["PrinterPull2"]);  //2도 롤
                    printerRoll2 = Convert.ToSingle(result["PrinterRoll2"]);  //2도 
                    printerPull3 = Convert.ToSingle(result["PrinterPull3"]);  //3도 롤
                    printerRoll3 = Convert.ToSingle(result["PrinterRoll3"]);  //3도 
                    printerPull4 = Convert.ToSingle(result["PrinterPull4"]);  //4도 롤
                    printerRoll4 = Convert.ToSingle(result["PrinterRoll4"]);  //4도

                    spare4 = Convert.ToSingle(result["spare4"]);
                    spare5 = Convert.ToSingle(result["spare5"]); 
                    spare6 = Convert.ToSingle(result["spare6"]);
                    spare7 = Convert.ToSingle(result["spare7"]);

                    feederGap = Convert.ToSingle(result["FeederGap"]);        //급지부 진입 갭
                    feederRoll = Convert.ToSingle(result["FeederRoll"]);      //급지부 진입 롤갭

                    slotterSubGap = Convert.ToSingle(result["SlotterSubGap"]);         //슬로다 보조 갭
                    slotterMiddleGap = Convert.ToSingle(result["SlotterMiddleGap"]);   //슬로다 본 갭
                    slotterConnection = Convert.ToSingle(result["SlotterConnection"]); //슬로다 연결갭

                    dieCutterRollGap = Convert.ToSingle(result["DieCutterRollGap"]);   //다이캇타 롤 갭
                    dieCutterConnectionGap = Convert.ToSingle(result["GluerbeltF1"]);   //다이캇타 연결 갭
                    dummy1 = Convert.ToSingle(result["GluerbeltF2"]);             //

                    gluerbeltF1 = Convert.ToSingle(result["GluerPressA"]);     //글루어벨트 진입 갭 F1
                    gluerbeltF2 = Convert.ToSingle(result["GluerPressB"]);     //글루어벨트 진입 갭 F2
                    gluerPressA = Convert.ToSingle(result["DieCutterRollA"]);  //글루어 프레스롤 A
                    gluerPressB = Convert.ToSingle(result["DieCutterRollB"]);  //글루어 프레스롤 B
                    gluerPE = Convert.ToSingle(result["GluerPE"]);             //글루어 PE
                    dummy2 = Convert.ToSingle(result["BRSub"]);             //

                    bRSub = Convert.ToSingle(result["BRMain"]);           //보조괘선 갭  
                    bRMain = Convert.ToSingle(result["BRConnection"]);    //본괘선 갭
                    bRConnection = Convert.ToSingle(result["BRmiddle"]);  //연결롤 갭
                    bRmiddle = Convert.ToSingle(result["spare8"]);        //중간롤 갭
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

                tb1.Text = aRecipe.ToString();
                tb2.Text = bRecipe.ToString();
                tb3.Text = cRecipe.ToString();
                tb4.Text = dRecipe.ToString();
                tb5.Text = eRecipe.ToString();
                tb6.Text = f1Recipe.ToString();
                tb7.Text = f2Recipe.ToString();
                tb8.Text = gRecipe.ToString();
                tb9.Text = oRecipe.ToString();
                tb10.Text = p1Recipe.ToString();
                tb11.Text = p2pRecipe.ToString();
                tb11.Text = p2pRecipe.ToString();
                tb12.Text = p3Recipe.ToString();

                tb13.Text = p4Recipe.ToString();
                tb14.Text = spare2.ToString();
                tb15.Text = spare3.ToString(); 
                tb16.Text = slotter.ToString();
                tb17.Text = dRagista.ToString();
                tb18.Text = printerPull1.ToString();
                tb19.Text = printerRoll1.ToString();
                tb20.Text = printerPull2.ToString();
                tb21.Text = printerRoll2.ToString();
                tb22.Text = printerPull3.ToString();
                tb23.Text = printerRoll3.ToString();
                tb24.Text = printerPull4.ToString();

                tb25.Text = printerRoll4.ToString();
                tb26.Text = spare4.ToString();
                tb27.Text = spare5.ToString();
                tb28.Text = spare6.ToString();
                tb29.Text = spare7.ToString();
                tb30.Text = feederGap.ToString();
                tb31.Text = feederRoll.ToString();
                tb32.Text = slotterSubGap.ToString();
                tb33.Text = slotterMiddleGap.ToString();
                tb34.Text = slotterConnection.ToString();
                tb35.Text = dieCutterRollGap.ToString();
                tb36.Text = dieCutterConnectionGap.ToString();

                tb37.Text = dummy1.ToString();
                tb38.Text = gluerbeltF1.ToString();
                tb39.Text = gluerbeltF2.ToString();
                tb40.Text = gluerPressA.ToString();
                tb41.Text = gluerPressB.ToString();
                tb42.Text = gluerPE.ToString();
                tb43.Text = dummy2.ToString();
                tb44.Text = bRSub.ToString();
                tb45.Text = bRMain.ToString();
                tb46.Text = bRConnection.ToString();
                tb47.Text = bRmiddle.ToString();
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
            LogManager.Info("CNS 상세창에서 CNS데이터 조회 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

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

                mainSock.Send(bDts); //수신데이터 요청

                LogManager.Info("CNS 상세창에서 CNS데이터 조회 요청 내용 : " + Send_Data, MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch (Exception)
            {
                CNS_Send = false;
                LogManager.Info("CNS 상세창에서 CNS데이터 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
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

        // 데이터 수신
        public void DataReceived(IAsyncResult ar) 
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 수신한 데이터 16진수로 변환
                string Receive_Data = string.Empty;
                for (int i = 0; i < obj.Buffer.Length; i++)
                {
                    Receive_Data += obj.Buffer[i].ToString("x2"); //16진수
                }

                //Word 연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                Console.WriteLine("PLC수신 데이터(Plc_Pop) : " + RD);
                Console.WriteLine("테스트 값 : " + RD);

                string dummyData = "";

                this.Dispatcher.Invoke(() =>
                {
                    for (int j = 0; j < RD.Length / 4; j++)
                    {
                        string data = RD.Substring(j * 4, 4);

                        switch (j)
                        {
                            case 0: //작업모델 번호
                                dummyData = ValueConvert(data, false);
                                break;
                            case 1: //작업지시 매수
                                dummyData = ValueConvert(data, false);
                                break;
                            case 2: //작업지시 매수
                                dummyData = ValueConvert(data, false);
                                break;
                            case 3: //박스타입
                                int boxtype = Convert.ToInt32(ValueConvert(data, false));
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
                                폭.Text = ValueConvert(data, false);
                                break;
                            case 5: //장
                                장.Text = ValueConvert(data, false);
                                break;
                            case 6: //고
                                고.Text = ValueConvert(data, false);
                                break;
                            case 7: //갭
                                갭.Text = ValueConvert(data, false);
                                break;
                            case 8: //앞날개
                                dummyData = ValueConvert(data, false);
                                break;
                            case 9: //미미
                                미미.Text = ValueConvert(data, false);
                                break;
                            case 10: //저장 완료
                                dummyData = ValueConvert(data, false);
                                break;
                            case 11: //생산속도
                                dummyData = ValueConvert(data, false);
                                break;
                            case 12: //저장값 쓰기 가능상태
                                dummyData = ValueConvert(data, false);
                                break;
                            case 13: //spare
                                dummyData = ValueConvert(data, false);
                                break;
                            case 14: //A
                                tb1.Text = ValueConvert(data, true);
                                break;
                            case 15: //B
                                tb2.Text = ValueConvert(data, true);
                                break;
                            case 16: //C
                                tb3.Text = ValueConvert(data, true);
                                break;
                            case 17: //D
                                tb4.Text = ValueConvert(data, true);
                                break;
                            case 18: //E
                                tb5.Text = ValueConvert(data, true);
                                break;
                            case 19: //F1
                                tb6.Text = ValueConvert(data, true);
                                break;
                            case 20: //F2
                                tb7.Text = ValueConvert(data, true);
                                break;
                            case 21: //G
                                tb8.Text = ValueConvert(data, true);
                                break;
                            case 22: //O
                                tb9.Text = ValueConvert(data, true);
                                break;
                            case 23: //P1
                                tb10.Text = ValueConvert(data, true);
                                break;
                            case 24: //P2
                                tb11.Text = ValueConvert(data, true);
                                break;
                            case 25: //P3
                                tb12.Text = ValueConvert(data, true);
                                break;
                            case 26: //P4
                                tb13.Text = ValueConvert(data, true);
                                break;
                            case 27: //spare
                                tb14.Text = ValueConvert(data, true);
                                break;
                            case 28: //spare
                                tb15.Text = ValueConvert(data, true);
                                break;
                            case 29: //spare
                                tb16.Text = ValueConvert(data, true);
                                break;
                            case 30: //D레지스터
                                tb17.Text = ValueConvert(data, true);
                                break;
                            case 31: //프린터1 롤?
                                tb18.Text = ValueConvert(data, true);
                                break;
                            case 32: //1도 프레스
                                tb19.Text = ValueConvert(data, true);
                                break;
                            case 33: //프린터2 롤?
                                tb20.Text = ValueConvert(data, true);
                                break;
                            case 34: //2도 프레스
                                tb21.Text = ValueConvert(data, true);
                                break;
                            case 35: //프린터3 롤?
                                tb22.Text = ValueConvert(data, true);
                                break;
                            case 36: //3도 프레스
                                tb23.Text = ValueConvert(data, true);
                                break;
                            case 37: //프린터4 롤?
                                tb24.Text = ValueConvert(data, true);
                                break;
                            case 38: //4도 프레스
                                tb25.Text = ValueConvert(data, true);
                                break;
                            case 39: //spare
                                tb26.Text = ValueConvert(data, true);
                                break;
                            case 40: //spare
                                tb27.Text = ValueConvert(data, true);
                                break;
                            case 41: //spare
                                tb28.Text = ValueConvert(data, true);
                                break;
                            case 42: //spare
                                tb29.Text = ValueConvert(data, true);
                                break;
                            case 43: //피타진입 갭
                                tb30.Text = ValueConvert(data, true);
                                break;
                            case 44: //급지부 롤 갭
                                tb31.Text = ValueConvert(data, true);
                                break;
                            case 45: //슬로타 보조괘선 갭
                                tb32.Text = ValueConvert(data, true);
                                break;
                            case 46: //슬로타 본괘선 갭
                                tb33.Text = ValueConvert(data, true);
                                break;
                            case 47: //슬로타 연결 갭
                                tb34.Text = ValueConvert(data, true);
                                break;
                            case 48: //다이캇타 롤 갭
                                tb35.Text = ValueConvert(data, true);
                                break;
                            case 49: //다이캇타 연결 갭
                                tb36.Text = ValueConvert(data, true);
                                break;
                            case 50: //spare
                                tb37.Text = ValueConvert(data, true);
                                break;
                            case 51: //글루어벨트 진입갭 F1
                                tb38.Text = ValueConvert(data, true);
                                break;
                            case 52: //글루어벨트 진입갭 F2
                                tb39.Text = ValueConvert(data, true);
                                break;
                            case 53: //글루어 배출 갭 프레스 A
                                tb40.Text = ValueConvert(data, true);
                                break;
                            case 54: //글루어 배출 갭 프레스 B
                                tb41.Text = ValueConvert(data, true);
                                break;
                            case 55: //PE가이드
                                tb42.Text = ValueConvert(data, true);
                                break;
                            case 56: //spare
                                tb43.Text = ValueConvert(data, true);
                                break;
                            case 57: //R/B 보조괘선 갭
                                tb44.Text = ValueConvert(data, true);
                                break;
                            case 58: //R/B 본괘선 갭
                                tb45.Text = ValueConvert(data, true);
                                break;
                            case 59: //R/B 연결롤 갭
                                tb46.Text = ValueConvert(data, true);
                                break;
                            case 60: //R/B 중간연결 갭
                                tb47.Text = ValueConvert(data, true);
                                break;
                            default:
                                break;
                        }
                        //continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                    }
                });
                //Console.WriteLine("CNS PLC수신 데이터 확인(Plc_Pop) : " + continuous_read); //수신값 확인용
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

        private string ValueConvert(string data, bool intIsFalse)
        {
            //앞뒤 두자리 묶음 자리변환
            int value = Convert.ToInt32(data.Substring(2, 2) + data.Substring(0, 2), 16);

            //
            if (value > 32767)
            {
                value -= 65536;
            }

            //-값 표현
            if (intIsFalse)
            {
                return (value / 10.0).ToString("N1"); //소숫점 첫번째까지 표현
            }

            return value.ToString();
        }

        //신규 생산 버튼
        private void Done(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogManager.Info("CNS 상세창에서 신규생산 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

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
            LogManager.Info("CNS 상세창 종료", MethodBase.GetCurrentMethod().DeclaringType);

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

        private void IP_setting(object sender, MouseButtonEventArgs e)
        {
            Ip_input ip = new Ip_input();
            ip.Show();
        }
    }
}
