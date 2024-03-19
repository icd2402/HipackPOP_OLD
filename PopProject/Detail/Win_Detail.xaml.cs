using Newtonsoft.Json.Linq;
using PopProject.Consumer_;
using PopProject.Input;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PopProject.KeyBoard;
using PopProject.input;

namespace PopProject.Detail
{
    /// <summary>
    /// Win_Detail.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Win_Detail : Window
    {
        Ls_Fenet wd = new Ls_Fenet();
        private ObservableCollection<string> uriPathOB = new ObservableCollection<string>();
        private ObservableCollection<string> localFilePath = new ObservableCollection<string>();
        private readonly MainModel mainModel;
        private int clickCount = 0;
        private readonly string metrialitem = SettingJsonManager.meterialinfo;
        public DetailModel Detailitem;
        private readonly ObservableCollection<MainModel> workList;
        private readonly bool monitorcheck = false;
        private  bool Barcode_Enter = false;
        public Socket mainSock;
        private string special;
        private bool detail_check;
        private bool consumcheck;
        private ConsumerModel consumerModel;
        private bool v;
        private bool Barcod;
        private Win_KeyBoard win_KeyBoard;

        private string 거래처;
        private string 품명;
        private string[] 규격pSize;
        private int 장;
        private int 폭;
        private int 고;
        private int 갭;
        private int 미미;
        private bool success = false; //CNS 통신연결 확인
        private bool CNS_Send = false;

        private int 작업지시;
        private bool consumer = false;
        private bool MemoLock = true;

        //private DateTimeManager dateTimeManager;

        //화면 더블클릭, 인풋창 에서 입장
        public Win_Detail(MainModel mainModel, string special, ObservableCollection<MainModel> workList, bool v, bool Barcod) //메인모델, 참고사항, 계획건, 듀얼모니터사용, 바코드 유무
        {
            InitializeComponent();
            
            this.mainModel = mainModel;
            this.workList = workList;
            monitorcheck = v;
            Barcode_Enter = Barcod;
            customSname.Text = mainModel.customSName;
            boxType.Text = "형  태 : " + mainModel.boxType;
            pplanCount.Text = "계획량 : " + mainModel.pplanCount.ToString("N0");
            prodName.Text = mainModel.prodName;
            subOb.ItemsSource = mainModel.subOb;
            tb공정별.Text = special.Replace("|", "\n");
            pSize.Text = mainModel.pSize;

            //dateTimeManager = DateTimeManager.Instance; // DateTimeManager 객체 생성

            Console.WriteLine("ProdID : " + mainModel.prodID);

            //실수요자
            consumcheck = mainModel.bMember;

            //오주포장 일때 옵션
            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
            {
                tb파렛트적재높이.Text = this.mainModel.PalletHeight;
                tb파렛트적재매수.Text = this.mainModel.PalletCount;
                파렛트적재매수.Visibility = Visibility.Visible; //파렛트 적재매수 표시
                파렛트적재높이.Visibility = Visibility.Visible; //파렛트 적재높이 표시
                Pass_detail.Visibility = Visibility.Visible;    //넘기기 버튼 표시
                wharehouse.Visibility = Visibility.Collapsed; //창고적재 버튼 숨김
                CNS_Save.Visibility = Visibility.Visible; //CNS 레시피 저장버튼 표시
            }
            else //그외 업체
            {
                Pass_detail.Visibility = Visibility.Collapsed; //넘기기 버튼 숨김
                CNS_Save.Visibility = Visibility.Collapsed; //CNS 레시피 저장버튼 숨김
            }

            //오주포장 로봇 체크시 로봇 제어창 팝업
            if (SettingJsonManager.corp_name.Equals("오주포장") && SettingJsonManager.robotchecked && Barcode_Enter)
            {
                serchRobotData();
            }

            //오주포장 PLC사용 체크되었으면 바코드 리드시 CNS로 자동 전송
            if (SettingJsonManager.corp_name.Equals("오주포장") && Barcode_Enter || 
                SettingJsonManager.corp_name.Equals("오주포장테스트") && Barcode_Enter)
            {
                //바코드로 디테일창 팝업시 동작
                if (SettingJsonManager.mercName.Equals("후렉소1호기") || SettingJsonManager.mercName.Equals("인라인2호기") || SettingJsonManager.mercName.Equals("인라인3호기"))
                {
                    LogManager.Info("CNS사용 체크 후 바코드로 상세화면 팝업 성공" + SettingJsonManager.mercName + "/" + this.mainModel.prodID, MethodBase.GetCurrentMethod().DeclaringType);

                    //DB에 데이터 있는거 확인용 변수
                    bool DB_DataOK = false; 

                    //DB 조회
                    ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                    ReturnModel returnDataCheck = ServiceModel.GetData(
                       SettingJsonManager.serverIndex,
                       "[PopCNSSearch]",
                       new string[] { "MachineName", "prodID" },
                       new object[] { SettingJsonManager.mercName, this.mainModel.prodID }, 0);

                    if (returnDataCheck.결과 == 1)
                    {
                        string machineName = "";
                        string ProdID = "";
                        int pock = 0;
                        int jang = 0;
                        int go = 0;

                        JArray Data_Jarry = JArray.Parse(returnDataCheck.리턴값);
                        foreach (JObject result in Data_Jarry)
                        {
                            machineName = result["MachineName"].ToString();
                            ProdID = result["prodID"].ToString();
                            pock = Convert.ToInt32(result["Pock"]);
                            jang = Convert.ToInt32(result["Jang"]);
                            go = Convert.ToInt32(result["Go"]);
                        }

                        //DB에 데이터가 있을경우
                        if (ProdID != "" && pock * jang != 0)
                        {
                            DB_DataOK = true;
                            CNS_DB_Send(); //DB 데이터 전송
                        }
                        //DB에 데이터가 없을경우
                        else
                        {
                            DB_DataOK = false;
                            CNS_defaultData_Send(); //신규 데이터 전송
                        }
                    }
                }
            }

            //로봇사용 체크시 로봇설정 버튼 표시
            robot.Visibility = SettingJsonManager.robotchecked ? Visibility.Visible : Visibility.Collapsed;

            //두성,혜성,지온 일때 적재창고 버튼 표시
            if (SettingJsonManager.corp_name.Equals("두성포장테스트") ||
                SettingJsonManager.corp_name.Equals("두성포장") ||
                SettingJsonManager.corp_name.Contains("혜성") ||
                SettingJsonManager.corp_name.Contains("광성수출포장") ||
                SettingJsonManager.corp_name.Contains("대세물산") ||
                SettingJsonManager.corp_name.Contains("지온"))
            {
                wharehouse.Visibility = Visibility.Visible;
            }
            else
            {
                wharehouse.Visibility = Visibility.Collapsed;
            }
            DetailSearch(); //세부정보 조회 (왼쪽화면)
            MetrialIG(); //원단입고,발주 같이조회 (자재상태창)
            PopSearchPhoto(); //인쇄사양 조회

            R_Value.Visibility = SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ? Visibility.Visible : Visibility.Collapsed;
            R_Value.Visibility = SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ? Visibility.Visible : Visibility.Collapsed;
            R_ValueTxt.Visibility = SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ? Visibility.Visible : Visibility.Collapsed;

            H_Value.Visibility = SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ? Visibility.Visible : Visibility.Collapsed;
            H_Value.Visibility = SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ? Visibility.Visible : Visibility.Collapsed;
            H_ValueTxt.Visibility = SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ? Visibility.Visible : Visibility.Collapsed;

            if (SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트"))
            {
                rValueSearch(); //금화 인쇄기 레지스타 조회
                holeValueSearch(); //금화 인쇄기 레지스타 조회
            }
        }

        public Win_Detail(MainModel mainModel, string special, bool detail_check)
        {
            InitializeComponent();

            this.mainModel = mainModel;
            this.special = special;
            this.detail_check = detail_check;

            customSname.Text = mainModel.customSName;
            boxType.Text = "형  태 : " + mainModel.boxType;
            pplanCount.Text = "계획량 : " + mainModel.pplanCount.ToString("N0");
            tb파렛트적재높이.Text = this.mainModel.PalletHeight;
            tb파렛트적재매수.Text = this.mainModel.PalletCount;
            prodName.Text = mainModel.prodName;
            subOb.ItemsSource = mainModel.subOb;
            tb공정별.Text = special.Replace("|", "\n");
            pSize.Text = mainModel.pSize;
            Pass_detail.Visibility = SettingJsonManager.corp_name.Equals("오주포장") ? Visibility.Visible : Visibility.Collapsed;
            R_Value.Visibility = SettingJsonManager.corp_name.Equals("금화") ? Visibility.Visible : Visibility.Collapsed;
            robot.Visibility = SettingJsonManager.robotchecked ? Visibility.Visible : Visibility.Collapsed;

            DetailSearch(); //세부정보 조회 (왼쪽화면)
            MetrialIG(); //원단입고,발주 같이조회 (자재상태창)
            PopSearchPhoto(); //인쇄사양
        }

        //실수요자 input에서 입장
        public Win_Detail(MainModel mainModel, string special, ConsumerModel consumerModel, bool detail_check, bool v)
        {
            InitializeComponent();

            this.mainModel = mainModel;
            this.special = special;
            this.consumerModel = consumerModel;
            this.detail_check = detail_check;
            this.v = v;

            customSname.Text = mainModel.customSName;
            boxType.Text = "형  태 : " + mainModel.boxType;
            pplanCount.Text = "계획량 : " + mainModel.pplanCount.ToString("N0");
            tb파렛트적재높이.Text = this.mainModel.PalletHeight;
            tb파렛트적재매수.Text = this.mainModel.PalletCount;
            prodName.Text = mainModel.prodName;
            subOb.ItemsSource = mainModel.subOb;
            tb공정별.Text = special.Replace("|", "\n");
            pSize.Text = mainModel.pSize;
            Pass_detail.Visibility = SettingJsonManager.corp_name.Equals("오주포장") ? Visibility.Visible : Visibility.Collapsed;
            R_Value.Visibility = SettingJsonManager.corp_name.Equals("금화") ? Visibility.Visible : Visibility.Collapsed;
            robot.Visibility = SettingJsonManager.robotchecked ? Visibility.Visible : Visibility.Collapsed;
            consumcheck = v;
            
            DetailSearch(); //세부정보 조회 (왼쪽화면)
            MetrialIG(); //원단입고,발주 같이조회 (자재상태창)
            PopSearchPhoto(); //인쇄사양

        }

        //DB데이터 CNS로 전송
        private void CNS_DB_Send()
        {
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
                int g2pRecipe = 0;
                int oRecipe = 0;
                int p1Recipe = 0;
                int p2pRecipe = 0;
                int p3Recipe = 0;
                int p4Recipe = 0;
                int spare2 = 0;
                int spare3 = 0;
                int slotter = 0;
                int dRagista = 0;
                int printerPull1 = 0;
                int printerRoll1 = 0;
                int printerPull2 = 0;
                int printerRoll2 = 0;
                int printerPull3 = 0;
                int printerRoll3 = 0;
                int printerPull4 = 0;
                int printerRoll4 = 0;
                int spare4 = 0;
                int spare5 = 0;
                int spare6 = 0;
                int spare7 = 0;
                int feederGap = 0;
                int feederRoll = 0;
                int slotterSubGap = 0;
                int slotterMiddleGap = 0;
                int slotterConnection = 0;
                int dieCutterRollGap = 0;
                int dieCutterConnectionGap = 0;
                int dummy1 = 0;
                int gluerbeltF1 = 0;
                int gluerbeltF2 = 0;
                int gluerPressA = 0;
                int gluerPressB = 0;
                int gluerPE = 0;
                int dummy2 = 0;
                int bRSub = 0;
                int bRMain = 0;
                int bRConnection = 0;
                int bRmiddle = 0;
                int spare8 = 0;
                int spare9 = 0;
                int spare10 = 0;
                int spare11 = 0;
                int spare12 = 0;
                int spare13 = 0;
                int spare14 = 0;
                int totalCount1 = 0;
                int totalCount2 = 0;
                int count1 = 0;
                int count2 = 0;


                try
                {
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

                        aRecipe = Convert.ToInt32(Convert.ToSingle(result["aRecipe"]) * 10);     //A
                        bRecipe = Convert.ToInt32(Convert.ToSingle(result["bRecipe"]) * 10);     //B
                        cRecipe = Convert.ToInt32(Convert.ToSingle(result["cRecipe"]) * 10);     //C
                        dRecipe = Convert.ToInt32(Convert.ToSingle(result["dRecipe"]) * 10);     //D
                        eRecipe = Convert.ToInt32(Convert.ToSingle(result["eRecipe"]) * 10);     //E
                        f1Recipe = Convert.ToInt32(Convert.ToSingle(result["f1Recipe"]) * 10);   //F1
                        f2Recipe = Convert.ToInt32(Convert.ToSingle(result["f2Recipe"]) * 10);   //F2
                        g2pRecipe = Convert.ToInt32(Convert.ToSingle(result["g2pRecipe"]) * 10); //G
                        oRecipe = Convert.ToInt32(Convert.ToSingle(result["oRecipe"]) * 10);     //O

                        p1Recipe = Convert.ToInt32(Convert.ToSingle(result["p1Recipe"]) * 10);   //P1
                        p2pRecipe = Convert.ToInt32(Convert.ToSingle(result["p2pRecipe"]) * 10); //P2
                        p3Recipe = Convert.ToInt32(Convert.ToSingle(result["p3Recipe"]) * 10);   //P3
                        p4Recipe = Convert.ToInt32(Convert.ToSingle(result["p4Recipe"]) * 10);   //P4

                        spare2 = Convert.ToInt32(Convert.ToSingle(result["spare2"]) * 10); //spare
                        spare3 = Convert.ToInt32(Convert.ToSingle(result["spare3"]) * 10); //spare

                        slotter = Convert.ToInt32(Convert.ToSingle(result["@Slotter"]) * 10);
                        dRagista = Convert.ToInt32(Convert.ToSingle(result["@DieCutter"]) * 10); //D레지스타

                        printerPull1 = Convert.ToInt32(Convert.ToSingle(result["PrinterPull1"]) * 10);  //1도 롤
                        printerRoll1 = Convert.ToInt32(Convert.ToSingle(result["PrinterRoll1"]) * 10);  //1도 
                        printerPull2 = Convert.ToInt32(Convert.ToSingle(result["PrinterPull2"]) * 10);  //2도 롤
                        printerRoll2 = Convert.ToInt32(Convert.ToSingle(result["PrinterRoll2"]) * 10);  //2도 
                        printerPull3 = Convert.ToInt32(Convert.ToSingle(result["PrinterPull3"]) * 10);  //3도 롤
                        printerRoll3 = Convert.ToInt32(Convert.ToSingle(result["PrinterRoll3"]) * 10);  //3도 
                        printerPull4 = Convert.ToInt32(Convert.ToSingle(result["PrinterPull4"]) * 10);  //4도 롤
                        printerRoll4 = Convert.ToInt32(Convert.ToSingle(result["PrinterRoll4"]) * 10);  //4도

                        spare4 = Convert.ToInt32(Convert.ToSingle(result["spare4"]) * 10); //spare
                        spare5 = Convert.ToInt32(Convert.ToSingle(result["spare5"]) * 10); //spare
                        spare6 = Convert.ToInt32(Convert.ToSingle(result["spare6"]) * 10); //spare
                        spare7 = Convert.ToInt32(Convert.ToSingle(result["spare7"]) * 10); //spare

                        feederGap = Convert.ToInt32(Convert.ToSingle(result["FeederGap"]) * 10);           //피다진입 갭
                        feederRoll = Convert.ToInt32(Convert.ToSingle(result["FeederRoll"]) * 10);         //피다진입 롤

                        slotterSubGap = Convert.ToInt32(Convert.ToSingle(result["SlotterSubGap"]) * 10);        //슬로다 보조 갭
                        slotterMiddleGap = Convert.ToInt32(Convert.ToSingle(result["SlotterMiddleGap"]) * 10);  //슬로다 본 갭
                        slotterConnection = Convert.ToInt32(Convert.ToSingle(result["SlotterConnection"]) * 10);//슬로다 연결갭

                        dieCutterRollGap = Convert.ToInt32(Convert.ToSingle(result["DieCutterRollGap"]) * 10); //다이캇타 롤 갭
                        dieCutterConnectionGap = Convert.ToInt32(Convert.ToSingle(result["GluerbeltF1"]) * 10); //다이캇타 연결 갭
                        dummy1 = Convert.ToInt32(Convert.ToSingle(result["GluerbeltF2"]) * 10);
                        gluerbeltF1 = Convert.ToInt32(Convert.ToSingle(result["GluerPressA"]) * 10);     //글루어벨트 진입 갭 F1
                        gluerbeltF2 = Convert.ToInt32(Convert.ToSingle(result["GluerPressB"]) * 10);     //글루어벨트 진입 갭 F2
                        gluerPressA = Convert.ToInt32(Convert.ToSingle(result["DieCutterRollA"]) * 10);  //글루어 프레스롤 A
                        gluerPressB = Convert.ToInt32(Convert.ToSingle(result["DieCutterRollB"]) * 10);  //글루어 프레스롤 B

                        gluerPE = Convert.ToInt32(Convert.ToSingle(result["GluerPE"]) * 10);             //글루어 PE
                        dummy2 = Convert.ToInt32(Convert.ToSingle(result["BRSub"]) * 10);
                        bRSub = Convert.ToInt32(Convert.ToSingle(result["BRMain"]) * 10);           //보조괘선 갭  
                        bRMain = Convert.ToInt32(Convert.ToSingle(result["BRConnection"]) * 10);    //본괘선 갭
                        bRConnection = Convert.ToInt32(Convert.ToSingle(result["BRmiddle"]) * 10);  //연결롤 갭
                        bRmiddle = Convert.ToInt32(Convert.ToSingle(result["spare8"]) * 10);        //중간롤 갭

                        spare9 = Convert.ToInt32(Convert.ToSingle(result["spare9"]) * 10); //spare
                        spare10 = Convert.ToInt32(Convert.ToSingle(result["spare10"]) * 10); //spare
                        spare11 = Convert.ToInt32(Convert.ToSingle(result["spare11"]) * 10); //spare
                        spare12 = Convert.ToInt32(Convert.ToSingle(result["spare12"]) * 10); //spare
                        spare13 = Convert.ToInt32(Convert.ToSingle(result["spare13"]) * 10); //spare
                        spare14 = Convert.ToInt32(Convert.ToSingle(result["spare14"]) * 10); //spare

                        totalCount1 = Convert.ToInt32(Convert.ToSingle(result["TotalCount1"]) * 1);
                        totalCount2 = Convert.ToInt32(Convert.ToSingle(result["TotalCount2"]) * 1);
                        count1 = Convert.ToInt32(Convert.ToSingle(result["Count1"]) * 1);
                        count2 = Convert.ToInt32(Convert.ToSingle(result["Count2"]) * 1);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                

                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ConnectSocket(); //CNS 통신연결
                        Console.WriteLine("CNS통신(Detail) : " + success);

                        if (success)//통신 연결 성공
                        {
                            Ls_Fenet wd = new Ls_Fenet();
                            List<int> items = new List<int>();

                            items.Add(99);  //설정 변경요구 (99로 요청하면 1로바뀜)
                            items.Add(1);   //설정 byte[] bDts = wd.continuous_Read("%DB7800", 70); 요구 (0=> 신규설정요구, 1=>데이터 설정 변경요구
                            items.Add(1); //작업 모델 번호
                            items.Add(10000); //작업지시 매수(하위)
                            items.Add(0); //작업지시 매수(상위)
                            items.Add(boxType); //BOX TYPE (1=>1합, 3=>2합, 5=>인쇄물, 6=>H형, 7=>H-Cover형 
                            items.Add(pock); //폭
                            items.Add(jang); //장
                            items.Add(go); //고
                            items.Add(gap); //갭
                            items.Add(wing); //앞날개
                            items.Add(cutting); //미미 컷팅
                            items.Add(0); //SaveDone
                            items.Add(0); //WorkSpeed
                            items.Add(0); //SaveState
                            items.Add(0); //Spare
                            items.Add(aRecipe);                //A
                            items.Add(bRecipe);                //B
                            items.Add(cRecipe);                //C
                            items.Add(dRecipe);                //D
                            items.Add(eRecipe);                //E
                            items.Add(f1Recipe);               //F1
                            items.Add(f2Recipe);               //F2
                            items.Add(g2pRecipe);              //G
                            items.Add(oRecipe);                //O
                            items.Add(p1Recipe);               //P1
                            items.Add(p2pRecipe);              //P2
                            items.Add(p3Recipe);               //P3
                            items.Add(p4Recipe);               //P4
                            items.Add(spare2);                 //Spare
                            items.Add(spare3);                 //Spare
                            items.Add(slotter);                //Spare
                            items.Add(dRagista);               //D레지스타
                            items.Add(printerPull1);           //1도 ?
                            items.Add(printerRoll1);           //1도 프레스
                            items.Add(printerPull2);           //2도 ?
                            items.Add(printerRoll2);           //2도 프레스
                            items.Add(printerPull3);           //3도 ?
                            items.Add(printerRoll3);           //3도 프레스
                            items.Add(printerPull4);           //4도 ?
                            items.Add(printerRoll4);           //4도 프레스
                            items.Add(spare4);                      //Spare
                            items.Add(spare5);                      //Spare
                            items.Add(spare6);                      //Spare
                            items.Add(spare7);                      //Spare
                            items.Add(feederGap);              //피다진입 갭
                            items.Add(feederRoll);             //피다진입 롤
                            items.Add(slotterSubGap);          //슬로다 보조 갭
                            items.Add(slotterMiddleGap);       //슬로다 본 갭
                            items.Add(slotterConnection);      //슬로다 연결갭
                            items.Add(dieCutterRollGap);       //다이캇타 롤 갭
                            items.Add(dieCutterConnectionGap); //다이캇타 연결 갭
                            items.Add(dummy1);                 //Spare
                            items.Add(gluerbeltF1);            //글루어벨트 진입 갭 F1
                            items.Add(gluerbeltF2);            //글루어벨트 진입 갭 F2
                            items.Add(gluerPressA);            //글루어 프레스롤 A
                            items.Add(gluerPressB);            //글루어 프레스롤 B
                            items.Add(gluerPE);                //글루어 PE
                            items.Add(dummy2);                 //Spare
                            items.Add(bRSub);                  //보조괘선 갭
                            items.Add(bRMain);                 //본괘선 갭
                            items.Add(bRConnection);           //연결롤 갭
                            items.Add(bRmiddle);               //중간롤 갭
                            items.Add(0);                      //spare9
                            items.Add(0);                      //spare10
                            items.Add(0);                      //spare11
                            items.Add(0);                      //spare12
                            items.Add(0);                      //spare13
                            items.Add(0);                      //spare14
                            items.Add(0);                      //TotalCount1
                            items.Add(0);                      //TotalCount2
                            items.Add(0);                      //Count1
                            items.Add(0);                      //Count2

                            int WriteCount = items.Count() * 2;
                            byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items); //연속쓰기 시작점(D3800), 데이터 개수, 값

                            //송신한 데이터 값 보는용도
                            string Send_Data = string.Empty;
                            for (int i = 0; i < bDts.Length; i++)
                            {
                                Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                            }
                            Console.WriteLine("CNS송신 데이터(Detail) : " + Send_Data);

                            //데이터 쓰기
                            mainSock.Send(bDts);
                        }
                    });
                }
                catch (Exception)
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("데이터 전송 실패!");
                    msg.ShowDialog();
                    mainSock.Close();
                }
                finally
                {
                    mainSock.Close();
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopCNSSearch 조회 오류 !!");
                msg.ShowDialog();
            }
        }


        //기본값만 CNS로 전송
        private void CNS_defaultData_Send()
        {
            try
            {
                string ProdID = mainModel.prodID;
                거래처 = mainModel.customSName; //거래처
                품명 = mainModel.prodName; //품명

                //생산매수
                작업지시 = 10000;

                //규격(장*폭*고)
                규격pSize = mainModel.pSize.Split('*');

                string s장 = 규격pSize[0] == null ? "" : 규격pSize[0].Trim();
                string s폭 = 규격pSize[1] == null ? "" : 규격pSize[1].Trim();
                string s고 = 규격pSize[2] == null ? "" : 규격pSize[2].Trim();
                장 = int.Parse(s장.ToString());
                폭 = int.Parse(s폭.ToString());
                고 = int.Parse(s고.ToString());

                if (SettingJsonManager.corp_name.Equals("오주포장"))
                {
                    if (SettingJsonManager.mercName.Equals("후렉소1호기"))
                    {
                        switch (mainModel.paperGor.Replace("골", "").Trim())
                        {
                            case "AB":
                            case "BA":
                                갭 = 70;
                                break;

                            case "BB":
                                갭 = 50;
                                break;

                            case "A":
                            case "EB":
                                갭 = 40;
                                break;

                            case "B":
                            case "E":
                                갭 = 20;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (SettingJsonManager.mercName.Equals("인라인2호기"))
                    {
                        switch (mainModel.paperGor.Replace("골", "").Trim())
                        {
                            case "AB":
                            case "BA":
                                갭 = 70;
                                break;

                            case "BB":
                                갭 = 50;
                                break;

                            case "A":
                            case "EB":
                                갭 = 40;
                                break;

                            case "B":
                            case "E":
                                갭 = 20;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (SettingJsonManager.mercName.Equals("인라인3호기"))
                    {
                        switch (mainModel.paperGor.Replace("골", "").Trim())
                        {
                            case "AB":
                            case "BA":
                                갭 = 70;
                                break;

                            case "BB":
                                갭 = 50;
                                break;

                            case "A":
                            case "EB":
                                갭 = 40;
                                break;

                            case "B":
                            case "E":
                                갭 = 20;
                                break;
                            default:
                                break;
                        }
                    }
                }

                //미미
                미미 = 30;

                Console.WriteLine("CNS 신규 데이터 전송값 => " + "ProdID:" + ProdID + ", 거래처:" + 거래처 + ", 품명:" + 품명 + ", 장:" + 장 + ", 폭:" + 폭 + ", 고:" + 고 + ", 갭:" + 갭 + ", 미미:" + 미미);
                LogManager.Info("CNS 신규 데이터 전송값 => " + "ProdID:" + ProdID + ", 거래처:" + 거래처 + ", 품명:" + 품명 + ", 장:" + 장 + ", 폭:" + 폭 + ", 고:" + 고 + ", 갭:" + 갭 + ", 미미:" + 미미, MethodBase.GetCurrentMethod().DeclaringType);
                
                ConnectSocket(); //CNS 통신연결
                Ls_Fenet wd = new Ls_Fenet();
                List<int> items = new List<int>();
                items.Add(99);  //설정 변경요구 (99로 요청하면 1로바뀜)
                items.Add(0);   //설정 byte[] bDts = wd.continuous_Read("%DB7800", 70); 요구 (0=> 신규설정요구, 1=>데이터 설정 변경요구
                items.Add(1); //작업 모델 번호
                items.Add(작업지시); //작업지시 매수(하위)
                items.Add(0); //작업지시 매수(상위)
                items.Add(1); //BOX TYPE (1=>1합, 3=>2합, 5=>인쇄물, 6=>H형, 7=>H-Cover형 3호기(1합철)
                items.Add(폭);   //폭
                items.Add(장);   //장
                items.Add(고);   //고
                items.Add(갭);   //갭
                items.Add(0);    //앞날개
                items.Add(미미); //미미

                int WriteCount = items.Count() * 2;
                byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items);

                //송신한 데이터 값 보는용도
                string Send_Data = string.Empty;
                for (int i = 0; i < bDts.Length; i++)
                {
                    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                }

                Console.WriteLine("CNS 송신데이터 : " + Send_Data);
                LogManager.Info("CNS 송신데이터 : " + Send_Data, MethodBase.GetCurrentMethod().DeclaringType);

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

        //CNS 소켓 오픈
        private void ConnectSocket()
        {
            LogManager.Info("소켓 오픈 요청", MethodBase.GetCurrentMethod().DeclaringType);

            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            if (!mainSock.Connected)
            {
                int port = int.Parse(SettingJsonManager.port); //포트 2004
                try
                {
                    IAsyncResult result = mainSock.BeginConnect(SettingJsonManager.ip, port, null, null);
                    success = result.AsyncWaitHandle.WaitOne(1500, true);

                    if (CNS_Send) // CNS -> PC(DB) 버튼 활성화
                    {
                    }
                    else //일반적인 연결
                    {
                        if (success)//연결 성공
                        {
                            LogManager.Info("소켓 연결 성공", MethodBase.GetCurrentMethod().DeclaringType);
                            mainSock.EndConnect(result);
                            AsyncObject obj = new AsyncObject(500);
                            obj.WorkingSocket = mainSock;
                            obj.ClearBuffer();
                            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                        }
                        else //연결 실패
                        {
                            LogManager.Error("소켓 연결 실패", MethodBase.GetCurrentMethod().DeclaringType);
                            mainSock.Close();
                            MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                            msg.ShowDialog();
                        }
                    }
                }
                catch (Exception e)
                {
                    LogManager.Error("소켓 연결 중 에러발생\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);

                    mainSock.Close();
                    MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                    msg.ShowDialog();
                }
            }
        }

        void DataReceived(IAsyncResult ar) // 데이터 수신
        {
            Resipe_Model itemModel = new Resipe_Model();

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

                //Word 연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                Console.WriteLine("PLC수신 데이터(Detail) : " + RD);

                for (int i = 0; i < RD.Length / 4; i++)
                {
                    //string high_data = RD.Substring(0, 2); //앞 두자리 추출
                    //string low_data = RD.Substring(2, 2); //뒤 두자리 추출

                    string data = RD.Substring(i * 4, 4);

                    switch (i)
                    {
                        case 0: //작업모델 번호
                            itemModel.ModelNum = ValueConvert(data, false);
                            break;
                        case 1: //작업지시 매수
                            itemModel.WorkPlan1 = ValueConvert(data, false);
                            break;
                        case 2: //작업지시 매수
                            itemModel.WorkPlan2 = ValueConvert(data, false);
                            break;
                        case 3: //박스타입
                            itemModel.BoxType = ValueConvert(data, false);
                            break;
                        case 4: //폭
                            itemModel.Pock = ValueConvert(data, false);
                            break;
                        case 5: //장
                            itemModel.Jang = ValueConvert(data, false);
                            break;
                        case 6: //고
                            itemModel.Go = ValueConvert(data, false);
                            break;
                        case 7: //갭
                            itemModel.Gap = ValueConvert(data, false);
                            break;
                        case 8: //앞날개
                            itemModel.Wing = ValueConvert(data, false);
                            break;
                        case 9: //미미
                            itemModel.Cutting = ValueConvert(data, false);
                            break;
                        case 10: //저장 완료
                            itemModel.SaveDone = ValueConvert(data, false);
                            break;
                        case 11: //생산속도
                            itemModel.WorkSpeed = ValueConvert(data, false);
                            break;
                        case 12: //저장값 쓰기 가능상태
                            itemModel.SaveState = ValueConvert(data, false);
                            break;
                        case 13: //spare
                            itemModel.spare = ValueConvert(data, false);
                            break;
                        case 14: //A
                            itemModel.aRecipe = ValueConvert(data, true);
                            break;
                        case 15: //B
                            itemModel.bRecipe = ValueConvert(data, true);
                            break;
                        case 16: //C
                            itemModel.cRecipe = ValueConvert(data, true);
                            break;
                        case 17: //D
                            itemModel.dRecipe = ValueConvert(data, true);
                            break;
                        case 18: //E
                            itemModel.eRecipe = ValueConvert(data, true);
                            break;
                        case 19: //F1
                            itemModel.f1Recipe = ValueConvert(data, true);
                            break;
                        case 20: //F2
                            itemModel.f2Recipe = ValueConvert(data, true);
                            break;
                        case 21: //G
                            itemModel.g2pRecipe = ValueConvert(data, true);
                            break;
                        case 22: //O
                            itemModel.oRecipe = ValueConvert(data, true);
                            break;
                        case 23: //P1
                            itemModel.p1Recipe = ValueConvert(data, true);
                            break;
                        case 24: //P2
                            itemModel.p2pRecipe = ValueConvert(data, true);
                            break;
                        case 25: //P3
                            itemModel.p3Recipe = ValueConvert(data, true);
                            break;
                        case 26: //P4
                            itemModel.p4Recipe = ValueConvert(data, true);
                            break;
                        case 27: //spare2
                            itemModel.spare2 = ValueConvert(data, true);
                            break;
                        case 28: //spare3
                            itemModel.spare3 = ValueConvert(data, true);
                            break;
                        case 29: //슬로타
                            itemModel.Slotter = ValueConvert(data, true);
                            break;
                        case 30: //타이캇타
                            itemModel.DieCutter = ValueConvert(data, true);
                            break;
                        case 31: //프린터1 풀 롤
                            itemModel.PrinterPull1 = ValueConvert(data, true);
                            break;
                        case 32: //프린터1 프레스 롤
                            itemModel.PrinterRoll1 = ValueConvert(data, true);
                            break;
                        case 33: //프린터2 풀 롤
                            itemModel.PrinterPull2 = ValueConvert(data, true);
                            break;
                        case 34: //프린터2 프레스 롤
                            itemModel.PrinterRoll2 = ValueConvert(data, true);
                            break;
                        case 35: //프린터3 풀 롤
                            itemModel.PrinterPull3 = ValueConvert(data, true);
                            break;
                        case 36: //프린터3 프레스 롤
                            itemModel.PrinterRoll3 = ValueConvert(data, true);
                            break;
                        case 37: //프린터4 풀 롤
                            itemModel.PrinterPull4 = ValueConvert(data, true);
                            break;
                        case 38: //프린터 프레스 롤
                            itemModel.PrinterRoll4 = ValueConvert(data, true);
                            break;
                        case 39: //spare4
                            itemModel.spare4 = ValueConvert(data, true);
                            break;
                        case 40: //spare5
                            itemModel.spare5 = ValueConvert(data, true);
                            break;
                        case 41: //spare6
                            itemModel.spare6 = ValueConvert(data, true);
                            break;
                        case 42: //spare
                            itemModel.spare7 = ValueConvert(data, true);
                            break;
                        case 43: //피다진입 갭
                            itemModel.FeederGap = ValueConvert(data, true);
                            break;
                        case 44: //피타진입 롤
                            itemModel.FeederRoll = ValueConvert(data, true);
                            break;
                        case 45: //슬로타 보조쾌선 갭
                            itemModel.SlotterSubGap = ValueConvert(data, true);
                            break;
                        case 46: //슬로타 본(중간) 갭
                            itemModel.SlotterMiddleGap = ValueConvert(data, true);
                            break;
                        case 47: //슬로타 연결(배출) 갭
                            itemModel.SlotterConnection = ValueConvert(data, true);
                            break;
                        case 48: //다이캇타 롤 갭
                            itemModel.DieCutterRollGap = ValueConvert(data, true);
                            break;
                        case 49: //글루어벨트 진앱 갭 F1
                            itemModel.GluerbeltF1 = ValueConvert(data, true);
                            break;
                        case 50: //글루어벨트 진앱 갭 F2
                            itemModel.GluerbeltF2 = ValueConvert(data, true);
                            break;
                        case 51: //글루어 프레스롤 갭 A
                            itemModel.GluerPressA = ValueConvert(data, true);
                            break;
                        case 52: //글루어 프레스롤 갭 B
                            itemModel.GluerPressB = ValueConvert(data, true);
                            break;
                        case 53: //다이캇타 롤 갭A
                            itemModel.DieCutterRollA = ValueConvert(data, true);
                            break;
                        case 54: //다이캇타 롤 갭B
                            itemModel.DieCutterRollB = ValueConvert(data, true);
                            break;
                        case 55: //글루어 PE 가이드
                            itemModel.GluerPE = ValueConvert(data, true);
                            break;
                        case 56: //R/B 보조괘선 진입 롤
                            itemModel.BRSub = ValueConvert(data, true);
                            break;
                        case 57: //R/B 본괘선 중간 롤
                            itemModel.BRMain = ValueConvert(data, true);
                            break;
                        case 58: //R/B 연결 배출 롤
                            itemModel.BRConnection = ValueConvert(data, true);
                            break;
                        case 59: //R/B 연결 중간 롤
                            itemModel.BRmiddle = ValueConvert(data, true);
                            break;
                        case 60: //spare
                            itemModel.spare8 = ValueConvert(data, true);
                            break;
                        case 61: //spare
                            itemModel.spare9 = ValueConvert(data, true);
                            break;
                        case 62: //spare
                            itemModel.spare10 = ValueConvert(data, true);
                            break;
                        case 63: //spare
                            itemModel.spare11 = ValueConvert(data, true);
                            break;
                        case 64: //spare
                            itemModel.spare12 = ValueConvert(data, true);
                            break;
                        case 65: //spare
                            itemModel.spare13 = ValueConvert(data, true);
                            break;
                        case 66: //spare
                            itemModel.spare14 = ValueConvert(data, true);
                            break;
                        case 67: //총생산량(하위) 
                            itemModel.TotalCount1 = ValueConvert(data, false);
                            break;
                        case 68: //총생산량(상위)
                            itemModel.TotalCount2 = ValueConvert(data, false);
                            break;
                        case 69: //현재생산량(하위)
                            itemModel.Count1 = ValueConvert(data, false);
                            break;
                        case 70: //현재생산량(상위)
                            itemModel.Count2 = ValueConvert(data, false);
                            break;

                        default:
                            break;
                    }
                    //continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                    //RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                }

                Console.WriteLine("CNS수신 데이터(input) : " + continuous_read); //수신값 확인용

                //CNS값 요청 성공시 DB 업데이트
                if (cnsProcessing)
                {
                    ReturnModel returnData = ServiceModel.GetData(
                        SettingJsonManager.serverIndex,
                        "[PopCNSUpdate]",
                        new string[] {
                            "@MachineName",
                            "@ModelNum",
                            "@prodID",
                            "@WorkPlan1",
                            "@WorkPlan2",
                            "@BoxType",
                            "@Pock",
                            "@Jang",
                            "@Go",
                            "Gap",
                            "@Wing",
                            "@Cutting",
                            "@SaveDone",
                            "@WorkSpeed",
                            "@SaveState",
                            "@spare",
                            "@aRecipe",
                            "@bRecipe",
                            "@cRecipe",
                            "@dRecipe",
                            "@eRecipe",
                            "@f1Recipe",
                            "@f2Recipe",
                            "@g2pRecipe",
                            "@oRecipe",
                            "@p1Recipe",
                            "@p2pRecipe",
                            "@p3Recipe",
                            "@p4Recipe",
                            "@spare2",
                            "@spare3",
                            "@Slotter",
                            "@DieCutter",
                            "@PrinterPull1",
                            "@PrinterRoll1",
                            "@PrinterPull2",
                            "@PrinterRoll2",
                            "@PrinterPull3",
                            "@PrinterRoll3",
                            "@PrinterPull4",
                            "@PrinterRoll4",
                            "@spare4",
                            "@spare5",
                            "@spare6",
                            "@spare7",
                            "@FeederGap",
                            "@FeederRoll",
                            "@SlotterSubGap",
                            "@SlotterMiddleGap",
                            "@SlotterConnection",
                            "@DieCutterRollGap",
                            "@GluerbeltF1",
                            "@GluerbeltF2",
                            "@GluerPressA",
                            "@GluerPressB",
                            "@DieCutterRollA",
                            "@DieCutterRollB",
                            "@GluerPE",
                            "@BRSub",
                            "@BRMain",
                            "@BRConnection",
                            "@BRmiddle",
                            "@spare8",
                            "@spare9",
                            "@spare10",
                            "@spare11",
                            "@spare12",
                            "@spare13",
                            "@spare14",
                            "@TotalCount1",
                            "@TotalCount2",
                            "@Count1",
                            "@Count2" },
                        new object[] { SettingJsonManager.mercName, itemModel.ModelNum, this.mainModel.prodID, itemModel.WorkPlan1, itemModel.WorkPlan2, itemModel.BoxType,
                                   itemModel.Pock, itemModel.Jang, itemModel.Go, itemModel.Gap, itemModel.Wing, itemModel.Cutting, itemModel.SaveDone, itemModel.WorkSpeed,
                                   itemModel.SaveState, itemModel.spare, itemModel.aRecipe, itemModel.bRecipe, itemModel.cRecipe, itemModel.dRecipe, itemModel.eRecipe,
                                   itemModel.f1Recipe, itemModel.f2Recipe, itemModel.g2pRecipe, itemModel.oRecipe, itemModel.p1Recipe, itemModel.p2pRecipe,
                                   itemModel.p3Recipe, itemModel.p4Recipe, itemModel.spare2, itemModel.spare3, itemModel.Slotter, itemModel.DieCutter, itemModel.PrinterPull1,
                                   itemModel.PrinterRoll1, itemModel.PrinterPull2, itemModel.PrinterRoll2, itemModel.PrinterPull3, itemModel.PrinterRoll3, itemModel.PrinterPull4,
                                   itemModel.PrinterRoll4, itemModel.spare4, itemModel.spare5, itemModel.spare6, itemModel.spare7, itemModel.FeederGap, itemModel.FeederRoll,
                                   itemModel.SlotterSubGap, itemModel.SlotterMiddleGap, itemModel.SlotterConnection, itemModel.DieCutterRollGap, itemModel.GluerbeltF1,
                                   itemModel.GluerbeltF2, itemModel.GluerPressA, itemModel.GluerPressB, itemModel.DieCutterRollA, itemModel.DieCutterRollB, itemModel.GluerPE,
                                   itemModel.BRSub, itemModel.BRMain, itemModel.BRConnection, itemModel.BRmiddle, itemModel.spare8, itemModel.spare9, itemModel.spare10,
                                   itemModel.spare11, itemModel.spare12, itemModel.spare13, itemModel.spare14, itemModel.TotalCount1, itemModel.TotalCount2, itemModel.Count1,
                                   itemModel.Count2 },
                        1
                        );

                    if (returnData.결과 == 1)
                    {
                    }
                    else
                    {
                    }

                    cnsProcessing = false;
                    MessageBoxResult result = System.Windows.MessageBox.Show("저장이 완료되었습니다", "CNS 데이터 저장");
                }

                obj.ClearBuffer();
            }
            catch (Exception)
            {
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

        //로봇데이터 DB조회
        private void serchRobotData()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[spSelIFrobotdata_info]",
               new string[] { "@prodID" },
               new object[] {
               mainModel.prodID
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
                    string UpDatetime = result["UpDatetime"].ToString();
                }

                //DB에 데이터가 있으면
                if (Data_Jarry.Count > 0)
                {
                    //로봇으로 데이터 전송
                    DataSend(BOX_W, BOX_H, BOX_D, BOX_DADAN, BOX_DANSU, BOX_PAT, PALLET_W, PALLET_H, PALLET_D);
                }
                else //저장된 데이터가 없으면 로봇설정 창 팝업
                {
                    Main.robot_Setting = new Robot_Setting(mainModel.prodID, mainModel.customSName, mainModel.prodName);
                    Main.robot_Setting.ShowDialog();
                    Main.robot_Setting = null;
                }
            }
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

                ///MessageBox_Dialog msg = new MessageBox_Dialog(continuous_read.ToString());
                // msg.ShowDialog();//수신 데이터값 보기
                //개별읽기
                //string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                //string high_data = RD.Substring(0, 2); //앞 두자리 추출
                //string low_data = RD.Substring(2, 2); //뒤 두자리 추출
                //int single_read = Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                //MessageBox.Show(single_read.ToString()); //수신 데이터값 보기
                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                Console.WriteLine(continuous_read.Substring(0, 36).ToString());
                obj.ClearBuffer();
            }
            catch (Exception)
            {
            }
            mainSock.Close();
        }

        private Boolean ConnectSocket(string flag)
        {
            if (true)
            {
                bool success = false;

                mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                if (!mainSock.Connected)
                {
                    int port = int.Parse(SettingJsonManager.robotPort);
                    IAsyncResult result = mainSock.BeginConnect(SettingJsonManager.robotIP, port, null, null);
                    //IAsyncResult result = mainSock.BeginConnect("192.168.7.7", port, null, null);

                    success = result.AsyncWaitHandle.WaitOne(2000, true);

                    if (success)
                    {
                        mainSock.EndConnect(result);
                        AsyncObject obj = new AsyncObject(150);
                        obj.WorkingSocket = mainSock;
                        obj.ClearBuffer();
                        switch (flag)
                        {
                            case "W":
                                mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived1, obj);
                                break;
                        }
                    }
                }
                return success;
            }
        }

        //오주 로봇 통신(쓰기)
        private void DataSend(string bOX_W, string bOX_H, string bOX_D, string bOX_DADAN, string bOX_DANSU, string bOX_PAT, string pALLET_W, string pALLET_H, string pALLET_D)
        {
            MessageBox_Dialog msg;
            if (ConnectSocket("W"))
            {
                try
                {
                    List<int> items = new List<int>();
                    items.Add(int.Parse(bOX_W));     //박스가로
                    items.Add(int.Parse(bOX_H));     //박스세로 
                    items.Add(int.Parse(bOX_D));     //박스높이
                    items.Add(int.Parse(bOX_DADAN)); //다단
                    items.Add(int.Parse(bOX_DANSU)); //단수
                    items.Add(int.Parse(bOX_PAT));   //패턴
                    items.Add(int.Parse(pALLET_W));  //파렛트가로
                    items.Add(int.Parse(pALLET_H));  //파렛트세로
                    items.Add(int.Parse(pALLET_D));  //파렛트높이
                    int WriteCount = items.Count() * 2;

                    //D600부터 데이터 전송
                    byte[] bDts = wd.continuous_Write("%DB1200", WriteCount, items);
                    // byte[] bDts = wd.Byte_continuous_Read("D", "312", 2);
                    string Send_Data = string.Empty;

                    // 전송 데이터 확인용
                    for (int i = 0; i < bDts.Length; i++)
                    {
                        Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    }
                    Console.WriteLine("오주 로봇 전송 데이터 : " + Send_Data);

                    mainSock.Send(bDts); //데이터 쓰기

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
                msg = new MessageBox_Dialog("로봇 연결 실패!!");
                msg.ShowDialog();
            }
        }

        string ftpUri = "";
        string ImagName = "";

        //인쇄사양 출력
        private void PopSearchPhoto()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
            SettingJsonManager.serverIndex,
            "[PopSearchPhoto]",
            new string[] { "@prodID" },
            new object[] { mainModel.prodID },
            0
            );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ObservableCollection<string> ob = new ObservableCollection<string>();
                    foreach (JObject result in Data_JArray)
                    {
                        string uri = SettingJsonManager.url + SettingJsonManager.corp_id + "/" + result["photoName"].ToString();
                        string imageName = result["photoName"].ToString();

                        //ftp용 uri, 파일명 저장
                        ftpUri = uri; 
                        ImagName = result["photoName"].ToString();

                        Console.WriteLine("인쇄사양 이미지 uri : " + uri);
                        ob.Add(uri);
                    }
                    uriPathOB = ob;

                    //FTP(가비아서버)
                    if (SettingJsonManager.corp_name.Equals("대양포장"))
                    {
                        //이미지 있을때
                        if (uriPathOB.Count > 0)
                        {
                            tbNoCount.Visibility = Visibility.Collapsed;
                            tbPicCount.Visibility = Visibility.Visible;
                            picCount.Visibility = Visibility.Visible;
                            tbPicCount.Text = "1/" + uriPathOB.Count;

                            string uri = ftpUri;
                            string id = "BOXWAS";
                            string pwd = "WAS2402!";
                            string imageName = ImagName;

                            //ftp 연결
                            ftpConnection(uri, id, pwd, imageName);
                        }
                        //이미지 없을때
                        else
                        {
                            tbNoCount.Visibility = Visibility.Visible;
                            tbPicCount.Visibility = Visibility.Collapsed;
                            picCount.Visibility = Visibility.Collapsed;
                        }
                    }
                    //HTTP(NAS)
                    else
                    {
                        //이미지 있을때
                        if (uriPathOB.Count > 0)
                        {
                            tbNoCount.Visibility = Visibility.Collapsed;
                            tbPicCount.Visibility = Visibility.Visible;
                            picCount.Visibility = Visibility.Visible;
                            tbPicCount.Text = "1/" + uriPathOB.Count;

                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(uriPathOB[0], UriKind.Absolute);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.DecodePixelWidth = 2000;
                            bitmap.EndInit();

                            pic.Source = bitmap;
                        }
                        //이미지 없을때
                        else
                        {
                            tbNoCount.Visibility = Visibility.Visible;
                            tbPicCount.Visibility = Visibility.Collapsed;
                            picCount.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                tbNoCount.Visibility = Visibility.Visible;
                tbPicCount.Visibility = Visibility.Collapsed;
                picCount.Visibility = Visibility.Collapsed;
                MessageBox_Dialog msg = new MessageBox_Dialog("PopSearchPhoto 조회 오류!!");
                _ = msg.ShowDialog();
            }
        }

        private void ftpConnection(string uri, string id, string pwd, string name)
        {
            string ftpUrl = uri;
            string ID = id;
            string Password = pwd;
            string localFilePath = "C:\\Downloads\\"+ name; // C드라이버 Downloads파일에 이미지파일 저장

            try
            {
                //Downloads 폴더없으면 생성
                string downloadFolderPath = @"C:\Downloads";
                if (!Directory.Exists(downloadFolderPath))
                {
                    Directory.CreateDirectory(downloadFolderPath);
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ID, Password);

                //FTP Request 결과
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream ftpStream = response.GetResponseStream())
                using (Stream fileStream = File.Create(localFilePath))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }

                // 파일 다운로드 후 BitmapImage 생성 및 할당
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(localFilePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 2000;
                bitmap.EndInit();

                // pic.Source에 할당
                pic.Source = bitmap;
            }
            catch (WebException ex)
            {
                LogManager.Error("ftpConnection 에러\n" + ex.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        
        //원단입고 내역, 원단발주 내역
        private void MetrialIG()
        {
            string scmServerIndex = ""; //업체 인덱스
            string scmDBName = ""; //접속정보(DB명)

            ObservableCollection<IgSmModel> ob = new ObservableCollection<IgSmModel>();

            //거래처로 ServerIndex,DB명(접속정보)임시로 변경
            if (Detailitem.SCMorderKey > 0)
            {
                switch (mainModel.customSName)
                {
                    case "신우수출포장":
                    case "신우포장":
                        scmServerIndex = "swbox";
                        scmDBName = "A07E67034";
                        break;
                    case "누리팩":
                        scmServerIndex = "nrbox";
                        scmDBName = "A07E265671";
                        break;
                    case "우리포장산업":
                    case "우리포장":
                        scmServerIndex = "wrbox";
                        scmDBName = "A07E144572";
                        break;
                    case "채움팩":
                        scmServerIndex = "cwbox";
                        scmDBName = "3ED73331";
                        break;
                    case "유램":
                        scmServerIndex = "erboxtest";
                        scmDBName = "1461E3757";
                        break;
                    case "지온":
                        scmServerIndex = "jobox";
                        scmDBName = "13C554403";
                        break;

                    default:
                        break;
                }

                //테스트
                if (SettingJsonManager.corp_id.ToUpper().Contains("ICDTEST"))
                {
                    switch (mainModel.customSName)
                    {
                        case "신우수출포장":
                        case "신우포장":
                            scmDBName = "icdtest9";
                            break;
                        case "누리팩":
                            scmDBName = "icdtest10";
                            break;
                        case "우리포장산업":
                        case "우리포장":
                            scmDBName = "icdtest11";
                            break;
                        case "채움팩":
                            scmDBName = "icdtest11";
                            break;
                        case "유램":
                            scmDBName = "icdtest10";
                            break;
                        case "지온":
                            scmDBName = "icdtest16";
                            break;

                        default:
                            break;
                    }
                }

                //////////////////////////////////////////////////////////////////SCM발주/////////////////////////////////////////////////////////////////////////
                ServiceModel.ServiceModelClean(scmServerIndex, scmDBName);
                ReturnModel returnDataSCM가입고 = ServiceModel.GetData(
                    scmServerIndex,
                    "[PopBJSearch]",
                    new string[] { "@orderKey" },
                    new object[] { Detailitem.SCMorderKey },
                    0
                    );
                if (returnDataSCM가입고.결과 == 1)
                {
                    JArray Data_JArray = JArray.Parse(returnDataSCM가입고.리턴값);
                    if (Data_JArray != null)
                    {
                        foreach (JObject result in Data_JArray)
                        {
                            IgSmModel listitem = new IgSmModel();
                            listitem.metrialBJKey = int.Parse(result["metrialBJKey"].ToString());                //원단발주키
                            listitem.wdjs = (result["Wdjs"] != null) ? int.Parse(result["Wdjs"].ToString()) : 1; //원단절수
                            listitem.SubscriptionBJCount = int.Parse(result["SubscriptionBJCount"].ToString());  //가입고수량

                            listitem.GigText = listitem.SubscriptionBJCount == 0 ? "가입고" : listitem.SubscriptionBJCount.ToString();  //가입고 버튼 텍스트
                            listitem.BtVisibility = "Visible"; //가입고 버튼 표시

                            listitem.SubscriptionBJCount *= listitem.wdjs; //가입고 수량에 절수 곱함
                            listitem.SCMServerIndex = scmServerIndex;
                            listitem.SCMDBName = scmDBName;

                            ob.Add(listitem);
                        }
                    }
                    else
                    {
                        MessageBox_Dialog msg = new MessageBox_Dialog("PopBJSearch 오류 발생 !!");
                        _ = msg.ShowDialog();
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////발주/////////////////////////////////////////////////////////////////////////
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData가입고 = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopBJSearch]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey },
                0
                );
            if (returnData가입고.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData가입고.리턴값);
                if (Data_JArray != null)
                {
                    foreach (JObject result in Data_JArray)
                    {
                        IgSmModel listitem = new IgSmModel();
                        listitem.metDate = DateTime.Parse(result["metBJDate"].ToString());                   //발주일자
                        listitem.customSName = result["customSName"].ToString();                             //거래처명
                        listitem.metrialName = result["metrialName"].ToString();                             //자재명
                        listitem.metBJCount = float.Parse(result["metBJCount"].ToString());                  //발주수량
                        listitem.wdjs = (result["Wdjs"] != null) ? int.Parse(result["Wdjs"].ToString()) : 1; //원단절수
                        listitem.metBJLast = DateTime.Parse(result["metBJLast"].ToString());                 //납기일
                        listitem.SubscriptionBJCount = int.Parse(result["SubscriptionBJCount"].ToString());  //가입고수량
                        listitem.metrialBJKey = int.Parse(result["metrialBJKey"].ToString());                //원단발주키

                        listitem.paperGor = (result["paperGor"] != null) ? result["paperGor"].ToString() : "";                      //골재질
                        listitem.metCount = (result["metBJCount"] != null) ? int.Parse(result["metBJCount"].ToString()) : 0;        //입고수량

                        listitem.GigText = listitem.SubscriptionBJCount == 0 ? "가입고" : listitem.SubscriptionBJCount.ToString();  //가입고 버튼 텍스트
                        listitem.BtVisibility = "Visible";             //가입고 버튼 표시
                        
                        listitem.metCount *= listitem.wdjs;            //발주 수량에 절수 곱함
                        listitem.SubscriptionBJCount *= listitem.wdjs; //가입고 수량에 절수 곱함
                        listitem.fontColor = Brushes.ForestGreen;
                        listitem.metCountString = listitem.metCount.ToString();

                        ob.Add(listitem);
                    }
                }
                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("PopBJSearch 오류 발생 !!");
                    _ = msg.ShowDialog();
                }
            }

            //////////////////////////////////////////////////////////////////////////입고/////////////////////////////////////////////////////////////////////
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopIGSearch]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    float wdCount = 0;
                    float scCount = 0;

                    foreach (JObject result in Data_JArray)
                    {
                        IgSmModel listitem = new IgSmModel
                        {
                            metDate = DateTime.Parse(result["metIGDate"].ToString()),   //발주일자
                            metType = int.Parse(result["metType"].ToString()),          //자재구분
                            metCount = float.Parse(result["metIGCount"].ToString()),    //입고수량
                            customSName = result["customSName"].ToString(),             //거래처명
                            metrialName = result["metrialName"].ToString(),             //자재명
                            wdjs = int.Parse(result["WdJS"].ToString()),                //원단절수
                            //metBJLast = DateTime.Parse(result["metBJLast"].ToString()), //납기일
                            fontColor = Brushes.ForestGreen,
                        };

                        listitem.metCount *= listitem.wdjs;
                        listitem.metCountString = listitem.metCount.ToString();
                        listitem.metPreCount *= listitem.wdjs;
                        listitem.metPreCountString = listitem.metPreCount.ToString();

                        if (listitem.metType == 0)
                        {
                            wdCount += listitem.metCount;
                        }
                        else
                        {
                            scCount += listitem.metCount;
                        }

                        //실입고 수량 확인 후 가입고 버튼 생성
                        if (listitem.metCount == 0)
                        {
                            listitem.BtVisibility = "Visible";
                            listitem.GigText = "가입고";
                        }
                        else //실입고 수량 있으면
                        {
                            listitem.BtVisibility = "Collapsed";
                        }

                        ob.Add(listitem);
                        
                        tbWdCount.Text = wdCount + "";
                        tbScCount.Text = scCount + "";
                    }

                    MetrialSM(ob); //소모내역 조회
                }
                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("PopIGSearch 오류 발생 !!");
                    _ = msg.ShowDialog();
                }
            }
        }

        //원단 소모내역 조회 (자재상태창)
        private void MetrialSM(ObservableCollection<IgSmModel> ob)
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopSMSearch]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    float wdCount = 0;
                    float scCount = 0;
                    foreach (JObject result in Data_JArray)
                    {
                        IgSmModel listitem = new IgSmModel
                        {
                            metDate = DateTime.Parse(result["metSMDate"].ToString()),    //발주일자
                            metBJLast = DateTime.Parse(result["metSMDate"].ToString()),  //납기일
                            metType = int.Parse(result["metType"].ToString()),           //자재구분
                            metCount = float.Parse(result["metSMCount"].ToString())      //소모수량
                        };
                        listitem.metCount = -listitem.metCount;
                        listitem.metCountString = "";
                        listitem.metPreCountString = listitem.metCount.ToString();
                        listitem.fontColor = Brushes.Red;
                        listitem.metrialName = result["metrialName"].ToString();
                        listitem.metCount = float.Parse(result["metSMCount"].ToString());

                        if (listitem.metType == 0)
                        {
                            wdCount += listitem.metCount;
                        }
                        else
                        {
                            scCount += listitem.metCount;
                        }

                        //실입고 수량 확인 후 가입고 버튼 생성
                        if (listitem.metCount == 0)
                        {
                            listitem.BtVisibility = "Visible";
                            listitem.GigText = "가입고";
                        }
                        else //실입고 수량 있으면
                        {
                            listitem.BtVisibility = "Collapsed";
                        }

                        ob.Add(listitem);
                    }

                    tbWdCount.Text = (int.Parse(tbWdCount.Text) + wdCount).ToString();
                    tbWdCount.Foreground = float.Parse(tbWdCount.Text) >= 0 ? Brushes.ForestGreen : Brushes.Red;
                    tbScCount.Text = (float.Parse(tbScCount.Text) + scCount).ToString();
                    tbScCount.Foreground = float.Parse(tbScCount.Text) >= 0 ? Brushes.ForestGreen : Brushes.Red;

                    //우리6사 가입고 버튼 사용
                    if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("지온") || SettingJsonManager.corp_name.Equals("유램"))
                    {
                        //가입고 버튼 있는 창
                        ic.ItemsSource = ob;
                        sv2.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        //가입고 버튼 없는 창
                        ic2.ItemsSource = ob;
                        sv.Visibility = Visibility.Collapsed;

                    }
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopSMSearch 조회 오류!!");
                _ = msg.ShowDialog();
            }
        }

        private string ExtractKorean(string beforestr)
        {

            //beforestr = "#ffffff";
            string str = Regex.Replace(beforestr, "#(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})", "");
            return str;
        }

        //세부정보 조회 (왼쪽화면)
        private void DetailSearch() 
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopWorkDetail]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey }, 0
                );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    _ = new ObservableCollection<MainModel>();
                    foreach (JObject result in Data_JArray)
                    {
                        Detailitem = new DetailModel
                        {
                            metrialName = result["metrialName"].ToString(),    //자재명
                            metType = int.Parse(result["metType"].ToString()), //자재구분
                            mL = int.Parse(result["mL"].ToString()), //장
                            mW = int.Parse(result["mW"].ToString()), //폭
                            orderKey = int.Parse(result["orderKey"].ToString()), //수주키
                            PrintNo = result["PrintNo"].ToString(), //조판번호
                            Printposition = result["Printposition"].ToString(), //조판위치
                            ColorType = ExtractKorean(result["ColorType"].ToString().Replace("|", "")), //인쇄색도
                            CoatingType = result["CoatingType"].ToString(), //코팅방법
                            PushingNo = result["PushingNo"].ToString(), //목형번호
                            Pushingposition = result["Pushingposition"].ToString(),//목형위치
                            ConnetingType = result["ConnetingType"].ToString(), //접합방법
                            PackCount = int.Parse(result["PackCount"].ToString()), //포장매수
                            PackType = result["PackType"].ToString(), //포장방법
                            loadposition = result["loadposition"].ToString(), //적재위치
                            filmNo = result["filmNo"].ToString(), //필름넘버
                            handChk = result["handChk"].ToString(), //손잡이사용
                            jobMemo = result["orderJobMemo"].ToString(), //
                            WDSize = result["WDSize"] != null ? result["WDSize"].ToString() : "", //원단규격
                            WDScore = result["WDScore"] != null ? result["WDScore"].ToString() : "", //스코아
                            DesignNO = result["DesignNO"] != null ? result["DesignNO"].ToString() : "", //디자인No
                            HandPosition = result["HandPosition"] != null ? result["HandPosition"].ToString() : "", //손잡이위치
                            LoadHeight = result["LoadHeight"] != null ? result["LoadHeight"].ToString() : "", //적재단수
                            LoadType = result["LoadType"] != null ? result["LoadType"].ToString() : "", //적재방법
                            PalletType = result["PalletType"] != null ? result["PalletType"].ToString() : "", //파렛트 종류
                            PalletSize = result["PalletSize"] != null ? result["PalletSize"].ToString() : "", //파렛트 규격
                            vendorName = result["vendorName"].ToString(), //납품처명
                            SCMorderKey = int.Parse(result["SCMorderKey"].ToString()) //SCM오더키

                        };

                        //오주
                        if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
                        {
                            //프린트넘버에 CNS 넘버가 있어서 가져옴
                            string CNS_No = result["PrintNo"].ToString(); //프린트 넘버

                            if (SettingJsonManager.mercName.Equals("인라인1호기"))
                            {
                                Match match = Regex.Match(CNS_No, @"1호기\s*:\s*(\d+)"); // 정규식 패턴을 사용하여 "1호기 : " 이후의 숫자 값을 추출

                                if (match.Success)
                                {
                                    string extractedValue = match.Groups[1].Value;
                                    int CSN레시피번호 = int.Parse(extractedValue);
                                }
                            }
                            else if (SettingJsonManager.mercName.Equals("인라인2호기"))
                            {
                                Match match = Regex.Match(CNS_No, @"1호기\s*:\s*(\d+)"); // 정규식 패턴을 사용하여 "2호기 : " 이후의 숫자 값을 추출

                                if (match.Success)
                                {
                                    string extractedValue = match.Groups[1].Value;
                                    int CSN레시피번호 = int.Parse(extractedValue);
                                }
                            }
                        }
                        else
                        {

                        }

                        //두성
                        if (SettingJsonManager.corp_name.Equals("두성포장") || SettingJsonManager.corp_name.Equals("두성포장테스트"))
                        {
                            JToken token = result["LocationName"];
                            if (token != null)
                            {
                                Detailitem.LocationName = result["LocationName"].ToString();
                                if (Detailitem.LocationName.Length > 2)
                                {
                                    wharehouselocation.Text = Detailitem.LocationName;
                                }
                            }
                            
                        }

                        //오주
                        if (SettingJsonManager.corp_name.Equals("오주포장"))
                        {
                            if (Detailitem.PrintNo.Contains("무인쇄") || Detailitem.PrintNo.Contains("X"))
                            {
                                tbNoCount.Text = "무 인 쇄";

                                tbNoCount.FontSize = 32;

                            }
                        }
                        Detailitem.ColorType = Detailitem.ColorType.Replace("|", ""); //인쇄색도
                        Detailitem.SpecialMemo = result["SpecialMemo"].ToString();


                        //메모
                        //우리6사는 메모 사용 안함 (박소연대리? 요청)
                        if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("우리포장") || SettingJsonManager.corp_name.Equals("유램"))
                        {
                            tb공정별.Text = tb공정별.Text.ToString();
                        }
                        else
                        {
                            //Console.WriteLine("prodMemo : " + result["prodMemo"]);
                            if (result["prodMemo"] != null && result["prodMemo"].ToString() != "")
                            {
                                if (MemoLock)
                                {
                                    tb공정별.Text = tb공정별.Text.ToString() + "\n" + "메  모 : " + result["prodMemo"].ToString();
                                }
                                MemoLock = false;
                            }
                        }
                        int count = 17;



                        //파렛트 사이즈
                        if (Detailitem.PalletSize.Equals(""))
                        {
                            //파렛트규격.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb파렛트규격.Text = Detailitem.PalletSize;
                        }

                        //파렛트 종류
                        if (Detailitem.PalletType.Equals(""))
                        {
                            // 파렛트종류.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb파렛트종류.Text = Detailitem.PalletType;
                        }

                        //적재방법
                        if (Detailitem.LoadType.Equals(""))
                        {
                            //적재방법.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb적재방법.Text = Detailitem.LoadType;
                            switch (Detailitem.LoadType.Trim())
                            {
                                case "1종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/1종구.png", UriKind.Relative));
                                    break;
                                case "2종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/2종구.png", UriKind.Relative));
                                    break;
                                case "3종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/3종구.png", UriKind.Relative));
                                    break;
                                case "4종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/4종구.png", UriKind.Relative));
                                    break;
                                case "5종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/5종구.png", UriKind.Relative));
                                    break;
                                case "6종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/6종구.png", UriKind.Relative));
                                    break;
                                case "7종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/7종구.png", UriKind.Relative));
                                    break;
                                case "8종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/8종구.png", UriKind.Relative));
                                    break;
                                case "9종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/9종구.png", UriKind.Relative));
                                    break;
                                case "11자4종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/11자4종구.png", UriKind.Relative));
                                    break;
                                case "8종구2":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/8종구2.png", UriKind.Relative));
                                    break;
                                case "1자3종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/1자3종구.png", UriKind.Relative));
                                    break;
                                case "1자4종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/1자4종구.png", UriKind.Relative));
                                    break;
                                case "1자5종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/1자5종구.png", UriKind.Relative));
                                    break;
                                case "1자6종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/1자6종구.png", UriKind.Relative));
                                    break;
                                case "5종구+6종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/5종구+6종구.png", UriKind.Relative));
                                    break;
                                case "4종구+11자4종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/4종구+11자4종구.png", UriKind.Relative));
                                    break;
                                default:
                                    jonggu.Visibility = Visibility.Collapsed;
                                    break;
                            }
                        }

                        //적재단수
                        if (Detailitem.LoadHeight.Equals(""))
                        {
                            //적재단수.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb적재단수.Text = Detailitem.LoadHeight;
                        }

                        //손잡이 위치
                        if (Detailitem.HandPosition.Equals(""))
                        {
                            //손잡이위치.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb손잡이위치.Text = Detailitem.HandPosition;
                        }

                        //디자인No.
                        if (Detailitem.DesignNO.Equals(""))
                        {
                            //디자인NO.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb디자인NO.Text = Detailitem.DesignNO;
                        }

                        //조판번호
                        if (Detailitem.PrintNo.Equals(""))
                        {
                            //조판번호.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb조판번호.Text = Detailitem.PrintNo;
                        }

                        //조판위치
                        if (Detailitem.Printposition.Equals(""))
                        {
                            //조판위치.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb조판위치.Text = Detailitem.Printposition;
                        }

                        //인쇄색도
                        if (Detailitem.ColorType.Equals(""))
                        {
                            //인쇄색도.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb인쇄색도.Text = Detailitem.ColorType;
                        }

                        //필름번호
                        if (Detailitem.filmNo.Equals(""))
                        {
                            // 필름번호.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb필름번호.Text = Detailitem.filmNo;
                        }

                        //코팅방법
                        if (Detailitem.CoatingType.Equals(""))
                        {
                            코팅.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb코팅.Text = Detailitem.CoatingType;
                        }

                        //목형번호
                        if (Detailitem.PushingNo.Equals(""))
                        {
                            목형번호.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb목형번호.Text = Detailitem.PushingNo;
                        }

                        //목형위치
                        if (Detailitem.Pushingposition.Equals(""))
                        {
                            목형위치.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb목형위치.Text = Detailitem.Pushingposition;
                        }

                        //손잡이
                        if (Detailitem.handChk.Equals(""))
                        {
                            손잡이.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb손잡이.Text = Detailitem.handChk;
                        }

                        //접합방법
                        if (Detailitem.ConnetingType.Equals(""))
                        {
                            접합방법.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb접합방법.Text = Detailitem.ConnetingType;
                        }

                        //밴딩매수
                        if (Detailitem.PackCount < 0)
                        {
                            밴딩매수.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb밴딩매수.Text = Detailitem.PackCount + "";
                        }

                        //밴딩방법
                        if (Detailitem.PackType.Equals(""))
                        {
                            밴딩방법.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }
                        else
                        {
                            tb밴딩방법.Text = Detailitem.PackType;
                        }

                        //작업지시
                        tb작업지시.Text = Detailitem.jobMemo;
                        
                        //특별지시
                        tb특별지시.Text = Detailitem.SpecialMemo;

                        //자재구분
                        if ( Detailitem.metType == 1) // 0:원단, 1:마닐라 2:외주, 3:부자재, 4:기타
                        {
                            if (SettingJsonManager.corp_name.Equals("누리팩"))
                            {
                                canSC.Visibility = Visibility.Visible;
                            }

                            txtSc.Text = Detailitem.metrialName;
                            txtScSize.Text = Detailitem.mL + " * " + Detailitem.mW;
                        }
                        //마닐라 정보삭제
                        else
                        {
                            canSC.Visibility = Visibility.Collapsed;
                            txtBh.Text = Detailitem.metrialName + "    (" + mainModel.paperGor + ")";
                            txtBhSize.Text = Detailitem.mL + " * " + Detailitem.mW;

                            string[] split = Detailitem.WDScore.Split('*');
                            if (split.Length > 0 && split.Length != 1)
                            {
                                if (!split[0].Equals(split[2]))
                                {
                                    WDScore.Foreground = Brushes.Red;
                                }
                            }
                            else
                            {
                                split = Detailitem.WDScore.Split('+');
                                if (split.Length == 4)
                                {
                                    WDScore.Foreground = Brushes.Red;
                                }
                            }

                            WDScore.Text = Detailitem.WDScore.Replace("*", " * ");
                            WDSize.Text = Detailitem.WDSize.Replace("*", " * ");

                            
                        }

                        if (count == 0)
                        {
                            imgLine.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("metrialBj 오류확인!!");
                _ = msg.ShowDialog();
            }
        }


        // 상세 화면 종료
        private void Detail_close(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Main.passCheck = true;
                pic.Source = null;
                Barcode_Enter = false;
                Close();
            }
            catch(Exception e1)
            {
                LogManager.Error("상세창 닫기 에러\n" + e1.Message, MethodBase.GetCurrentMethod().DeclaringType);
                LogManager.Error("상세창 닫기 에러\n" + e1.StackTrace, MethodBase.GetCurrentMethod().DeclaringType);
            }
            finally
            {
                this.Close();
            }
        }

        //최근작업 클릭(디테일 리스트)
        private void Detail_List(object sender, MouseButtonEventArgs e)
        {
            can.Visibility = Visibility.Visible;
            Win_WorkList workList = new Win_WorkList(mainModel);
            _ = workList.ShowDialog();
            can.Visibility = Visibility.Collapsed;
        }

        // 이미지 확대 축소 팝업
        private void Image_Detail(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                can.Visibility = Visibility.Visible;
                Image img = (Image)sender;
                Win_PictureView win_Picture = new Win_PictureView(img.Source);
                _ = win_Picture.ShowDialog();
                can.Visibility = Visibility.Collapsed;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //제품명세서 출력 요청 
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("지온"))
            {
                SwPrintMessage swPrintMessage = new SwPrintMessage(mainModel, Detailitem);
                swPrintMessage.ShowDialog();
            }
            else
            {
                if (consumcheck)
                {
                    PrintMessage printMessage = new PrintMessage(mainModel, Detailitem,consumerModel,true);
                    _ = printMessage.ShowDialog();
                }
                else
                {
                    PrintMessage printMessage = new PrintMessage(mainModel, Detailitem);
                    _ = printMessage.ShowDialog();
                }
            }
        }

        //시작 버튼
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            //Close();
            switch (metrialitem)
            {
                case "19": //마닐라 
                case "20": //원단
                    Close();

                    Win_input input = new Win_input(mainModel, workList, true);
                    Main1.win_Input = input;
                    _ = input.ShowDialog();
                    Main1.win_Input = null;
                    if (SettingJsonManager.presultcount != 0)
                    {
                        mainModel.presultCount = SettingJsonManager.presultcount;
                    }
                    SettingJsonManager.presultcount = 0;
                    SettingJsonManager.SaveJsonFile();
                    break;

                default: //일반
                    //실수요자 건 일때
                    if (consumcheck)
                    {
                        MessageBox_Dialog msg = new MessageBox_Dialog("정상적인 경로가 아닙니다.");
                        _ = msg.ShowDialog();
                    }
                    //일반 생산 건 일때
                    else
                    {
                        LogManager.Info(mainModel.customSName + " / " + mainModel.prodName + " [생산시작]", MethodBase.GetCurrentMethod().DeclaringType);

                        Close();

                        input = new Win_input(mainModel, workList, true);
                        Main.win_Input = input;
                        _ = input.ShowDialog();

                        
                        Main.win_Input = null;
                        if (SettingJsonManager.presultcount != 0)
                        {
                            mainModel.presultCount = SettingJsonManager.presultcount;
                        }
                        SettingJsonManager.presultcount = 0;
                        SettingJsonManager.SaveJsonFile();

                    }
                    break;
            }
            pic.Source = null;
        }

        // 인쇄사양 다음 이미지 
        private void Image_next(object sender, MouseButtonEventArgs e)
        {
            //ftp이미지
            if (SettingJsonManager.corp_name.Equals("대양포장"))
            {
                if (clickCount + 1 > uriPathOB.Count - 1)
                {
                    clickCount = 0;
                }
                else
                {
                    clickCount += 1;
                }
                tbPicCount.Text = clickCount + 1 + "/" + uriPathOB.Count;

            }
            //http이미지
            else
            {
                if (clickCount + 1 > uriPathOB.Count - 1)
                {
                    clickCount = 0;
                }
                else
                {
                    clickCount += 1;
                }
                tbPicCount.Text = clickCount + 1 + "/" + uriPathOB.Count;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(uriPathOB[clickCount], UriKind.Absolute);
                bitmap.EndInit();

                pic.Source = bitmap;
            }
        }

        private void Sv_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        //디테일 창 로드시 듀얼 모니터인지 확인
        private void Detail_Loaded(object sender, RoutedEventArgs e)
        {
            //듀얼 상세화면 사용시
            if (monitorcheck)
            {
                //듀얼모니터 이면
                if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
                {
                    System.Drawing.Rectangle rec;

                    if (SettingJsonManager.mercName.Equals("인라인2호기") || SettingJsonManager.mercName.Equals("인라인3호기"))
                    {
                        //메인화면 팝업
                        rec = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
                    }
                    else
                    {
                        //듀얼화면 팝업
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
        }

        //넘기기 버튼
        private void Pass_detailInfo(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D353D"));

            Main.passCheck = false;
            CountManager.SendMsg(SettingJsonManager.mercName, mainModel.orderKey.ToString() + "^@"); //넘기기 (설비명, 오더키+^@)
            Console.WriteLine("넘기기 정보 : " + SettingJsonManager.mercName + mainModel.orderKey.ToString() + "^@");
        }

        //오주 CNS전용 넘기기 기능   // 확인
        //private void InvokeCount(string mercName, string v)
        //{
        //    SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v); // EX) invokeCount, 3125247246, 인라인3호기, 오더키
        //}

        //로봇 세팅 버튼
        private void robot_setting(object sender, MouseButtonEventArgs e)
        {
            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
            {
                Main.robot_Setting = new Robot_Setting(mainModel.prodID, mainModel.customSName, mainModel.prodName);
                Main.robot_Setting.ShowDialog();
                Main.robot_Setting = null;
            }
            else if(SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
            {
                Main.robot_Setting2 = new Robot_Setting2(mainModel.prodID, mainModel.customSName, mainModel.prodName);
                Main.robot_Setting2.ShowDialog();
                Main.robot_Setting2 = null;
            }
        }

        //박스적재 창고
        private void Wharehouse(object sender, MouseButtonEventArgs e)
        {
            Wharehouse_info wharehouse = new Wharehouse_info(mainModel,Detailitem.LocationName);
            wharehouse.ShowDialog();
            DetailSearch();
        }

        //가입고 버튼 클릭
        private void GIGButton_Click(object sender, RoutedEventArgs e)
        {
            IgSmModel item = ((Button)sender).DataContext as IgSmModel;

            Win_GigUpdate win_GigUpdate = new Win_GigUpdate(item.metrialBJKey, item.SCMServerIndex, item.SCMDBName);
            _ = win_GigUpdate.ShowDialog();

            MetrialIG();
        }

        //넘기기 버튼
        private void Pass_detail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#485563"));
        }

        //CNS 레시피 저장 버튼
        private void CNS_Save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D353D"));

            SingleWrite(); //현재 설정을 읽기 위해서는 D3814번에 0이 아닌 수를 넣어줘야함
            CNSDataRead(); //CNS 데이터 읽기
        }

        //CNS 레시피 저장 버튼
        private void CNS_Save_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#485563"));
        }

        private void SingleWrite() //데이터를 읽기 위한 쓰기
        {
            CNS_Send = true;

            ConnectSocket(); //CNS 통신 연결
            Ls_Fenet wd = new Ls_Fenet(); //헤더
            byte[] bDts2 = wd.single_Write("%DW3814", 02, 2); //개별쓰기 (변수명, 데이터 타입, 쓸 데이터값) 
            mainSock.Send(bDts2); //데이터 쓰기
            mainSock.Close();

            CNS_Send = false;
        }


        //금화 R값 클릭 down
        private void R_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D353D"));
        }

        //금화 R값 클릭 up
        private void R_Value_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#485563"));

            Win_KeyBoard3 keyBoard3 = new Win_KeyBoard3(this.mainModel.prodID);
            keyBoard3.ShowDialog();

            rValueSearch(); //금화 인쇄기 R값 조회
        }

        //금화 타공 값 클릭 down
        private void H_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D353D"));
        }

        //금화 타공 값 클릭 up
        private void H_Value_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = (Border)sender;
            clickedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#485563"));

            Win_KeyBoard4 keyBoard4 = new Win_KeyBoard4(this.mainModel.prodID);
            keyBoard4.ShowDialog();

            holeValueSearch(); //금화 인쇄기 타공값 조회
        }

        //금화 인쇄기 레지스타 조회
        private void rValueSearch()
        {
           ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[rRegistaSearch]",
               new string[] { "prodID" },
               new object[] { this.mainModel.prodID }, 
               0
             );

            if (returnData.결과 == 1)
            {
                string RValue = "";

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach (JObject result in Data_Jarry)
                {
                    RValue = result["rValue"].ToString();
                }
                rValueTxt.Text = RValue;
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("rRegistaSearch 조회 오류!!");
                msg.ShowDialog();
                Console.WriteLine("rRegistaSearch 실패");
            }
        }

        //금화 인쇄기 타공값 조회
        private void holeValueSearch()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[holeRegistaSearch]",
               new string[] { "prodID" },
               new object[] { this.mainModel.prodID },
               0
             );

            if (returnData.결과 == 1)
            {
                string HValue = "";

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach (JObject result in Data_Jarry)
                {
                    HValue = result["hValue"].ToString();
                }
                hValueTxt.Text = HValue;
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("holeRegistaSearch 조회 오류!!");
                msg.ShowDialog();
                Console.WriteLine("holeRegistaSearch 실패");
            }
        }

        bool cnsProcessing = false;
        //데이터 수신 요청
        private void CNSDataRead() 
        {
            if (SettingJsonManager.Plc) //PLC 사용 체크일때
            {
                try
                {
                    ConnectSocket(); //CNS 통신 연결
                    Ls_Fenet wd = new Ls_Fenet(); //헤더

                    byte[] bDts = wd.Word_continuous_Read("%DB7804", 71); //연속읽기 D3902 ~ D3972 (71개)

                    //송신한 데이터 값 보는용도
                    string Send_Data = string.Empty;
                    for (int i = 0; i < bDts.Length; i++)
                    {
                        Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    }
                    Console.WriteLine("CNS송신 데이터(Detail) : " + Send_Data);

                    cnsProcessing = true;
                    mainSock.Send(bDts); //데이터 쓰기
                }
                catch (Exception)
                {
                }
            }
        }

        //CNS 테스트용
        private void hiddenBt(object sender, MouseButtonEventArgs e)
        {
            if (SettingJsonManager.corp_name.Equals("오주포장") && SettingJsonManager.Plc ||
                SettingJsonManager.corp_name.Equals("오주포장테스트") && SettingJsonManager.Plc)
            {
                //바코드로 디테일창 팝업시 동작
                if (SettingJsonManager.mercName.Equals("후렉소1호기") || SettingJsonManager.mercName.Equals("인라인2호기") || SettingJsonManager.mercName.Equals("인라인3호기"))
                {
                    LogManager.Info("CNS사용 체크 후 바코드로 상세화면 팝업 성공" + SettingJsonManager.mercName + "/" + this.mainModel.prodID, MethodBase.GetCurrentMethod().DeclaringType);

                    //DB에 데이터 있는거 확인용 변수
                    bool DB_DataOK = false;

                    //DB 조회
                    ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                    ReturnModel returnDataCheck = ServiceModel.GetData(
                       SettingJsonManager.serverIndex,
                       "[PopCNSSearch]",
                       new string[] { "MachineName", "prodID" },
                       new object[] { SettingJsonManager.mercName, this.mainModel.prodID }, 0);

                    if (returnDataCheck.결과 == 1)
                    {
                        string machineName = "";
                        string ProdID = "";
                        int pock = 0;
                        int jang = 0;
                        int go = 0;

                        JArray Data_Jarry = JArray.Parse(returnDataCheck.리턴값);
                        foreach (JObject result in Data_Jarry)
                        {
                            machineName = result["MachineName"].ToString();
                            ProdID = result["prodID"].ToString();
                            pock = Convert.ToInt32(result["Pock"]);
                            jang = Convert.ToInt32(result["Jang"]);
                            go = Convert.ToInt32(result["Go"]);
                        }

                        //DB에 데이터가 있을경우
                        if (ProdID != "" && pock * jang != 0)
                        {
                            DB_DataOK = true;
                            CNS_DB_Send(); //DB 데이터 전송
                        }
                        //DB에 데이터가 없을경우
                        else
                        {
                            DB_DataOK = false;
                            CNS_defaultData_Send(); //신규 데이터 전송
                        }
                    }
                }
            }

        }
    }
}
