using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using PopProject.Detail;
using PopProject.input;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace PopProject.Input
{
    /// <summary>
    /// Win_input.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_input : Window
    {
        public static Socket mainSock; //CNS PLC
        ObservableCollection<string> ob = new ObservableCollection<string>();
        List<string> ob1 = new List<string>();
        private ObservableCollection<string> errorOb;
        private readonly string metrialitem = SettingJsonManager.meterialinfo; //
        public bool checkInput = false;
        private Point scrollMousePoint = new Point();
        private Point scrollMousePoint1 = new Point();
        private bool checkChangeButton = false;
        private readonly MainModel mainModel;
        public InputModel inputModel;
        private float count = 0;
        private readonly ObservableCollection<MainModel> workList;
        //  ObservableCollection<String> error;
        private float totalPerrCount = 0;
        private Win_KeyBoard win_KeyBoard;
        private double hOff = 1;
        private double vOff = 1;
        
        private double hOff1 = 1;
        private double vOff1 = 1;
        private int result;
        private bool ErrorCheck = true;
        private bool MultiErrorCheck = true;
        private bool CNS_Send = false;
        string Url; //서비스 주소

        private bool success = false; //CNS 통신연결 확인

        public ILog log = LogManager.GetLogger(typeof(Win_input)); //로그


        public Win_input(MainModel mainModel, ObservableCollection<MainModel> workList, bool check) //check= 메인화면에서 들어오면 false, 디테일 화면에서 들어오면 true
        {
            InitializeComponent();
            
            this.workList = workList;
            inputModel = new InputModel();
            this.mainModel = mainModel;
            boxType.Text = "형 태 : " + mainModel.boxType;

            //Log4net
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));

            //오주포장 카운터 수신
            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
            {
                SignalRAdministrator.허브.On<string, string>("returnCount", recive); //아두이노 통신
            }

            //이월버튼 
            if (mainModel.presultCount > 0 && mainModel.sDatetime.Length > 3 && mainModel.eDatetime.Length > 3)
            {
                box1.Visibility = Visibility.Visible; //이월버튼 활성화
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);
            }
            else
            {
                box1.Visibility = Visibility.Collapsed; //이월버튼 비활성화
                Canvas.SetTop(box, 540);
            }

            ConvertModel();
            PopBeforeProcessItemCount();
            CNS_DB확인();

            //키인사용 체크
            if (SettingJsonManager.keyinCheck)
            {
                tbResult.Focusable = true;
                _ = tbResult.Focus(); //생산수량에 포커스
                _ = tbResult.Dispatcher.BeginInvoke(new Action(() => { tbResult.SelectAll(); }));
            }

            //디테일 화면에서 시작버튼을 누르면 실행
            if (check)
            {
                //생산중이지 않을시 시작
                if (Main.production == null && Main1.production == null)
                {
                    if (inputModel.sDatetime.Length < 3 && inputModel.eDatetime.Length < 3)
                    {
                        _ = PopResultStart();
                        PopResultSearch();
                    }
                }
            }

            //PLC사용 체크시 체팅 버튼 활성화
            if (SettingJsonManager.Plc)
            {
                Can_plc.Visibility = Visibility.Collapsed;
                plc_Send.Visibility = Visibility.Visible;
                plc_Save.Visibility = Visibility.Visible;
                plc_Search.Visibility = Visibility.Visible;
            }
            else
            {
                Can_plc.Visibility = Visibility.Visible;
                plc_Send.Visibility = Visibility.Collapsed;
                plc_Save.Visibility = Visibility.Collapsed;
                plc_Search.Visibility = Visibility.Collapsed;
            }

            Console.WriteLine("품명: "     + inputModel.prodName + ", " +
                              "orderkey: " + inputModel.orderKey + ", " +
                              "pplanKey: " + inputModel.pplanKey + ", " +
                              "prodID: "   + inputModel.prodID + ", " +
                              "시작시간: " + inputModel.sDatetime
                              );
        }

        //오주포장 장비별 카운터 수신(아두이노)
        private void recive(string arg1, string arg2)  //설비명, 카운터
        {
            Console.WriteLine("오주 아두이노 값(input) : " +  arg2);
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    switch (arg1)
                    {
                        case "인라인3호기":
                        case "후렉소":
                            {
                                try
                                {
                                    //종료버튼 활성화 일때
                                    if (inputModel.imagePath.Equals("/img/bn_06.png"))
                                    {
                                        if (arg1.Equals(SettingJsonManager.mercName))
                                        {
                                            inputModel.pworkCount = float.Parse(arg2);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                }
                            }
                            break;
                    }
                }), DispatcherPriority.ApplicationIdle);
            }
            catch (Exception e)
            {
                log.Error("오주포장 인라인3호기 전용 카운터 SignalR 수신 오류 : " + e.Message);
            }
        }

        

        private void PopBeforeProcessItemCount()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
               "[PopBeforeProcessItemCount]",
                new string[] { "@orderKey", "@ProcessType", "@MinusThisProcessItem" },
                new object[] { inputModel.orderKey, inputModel.ProcessType, "1" },
                0
                );

            if (returnData.결과 == 1)
            {
                string beforeCount = "";
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);

                foreach (JObject result in Data_Jarry)
                {
                    beforeCount = result["BeforeProcessCount"].ToString();
                }

                try
                {
                    int before = (int)inputModel.pplanCount - int.Parse(beforeCount);
                    Pre_Count.Text = int.Parse(beforeCount).ToString();
                    // Pre_Count.Text = before.ToString("N0");
                }
                catch (Exception)
                {
                    Pre_Count.Text = "0";
                }
            }
            else
            {
                log.Error("PopBeforeProcessItemCount 조회 실패");
            }
        }

        private void ConvertModel()
        {
            inputModel.ProcessType = mainModel.ProcessType;
            inputModel.pplanKey = mainModel.pplanKey;
            inputModel.orderKey = mainModel.orderKey;
            inputModel.BMember = mainModel.bMember;
            inputModel.pCnt = mainModel.pCnt;
            inputModel.orderWorkNo = mainModel.orderWorkNo;
            inputModel.orderLast = mainModel.orderLast;
            inputModel.orderCount = mainModel.orderCount;
            inputModel.pSize = mainModel.pSize;
            inputModel.prodName = mainModel.prodName;
            inputModel.customSName = mainModel.customSName;
            inputModel.pplanCount = mainModel.pplanCount;
            inputModel.errorModel = new errorModel();
            inputModel.pworkDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            inputModel.newProd = mainModel.newProd;
            inputModel.eDatetime = "";
            inputModel.perrCount = 0;
            inputModel.count = 0;
            inputModel.pMemo = "";
            inputModel.pworkCount = 0;
            inputModel.imagePath = "/img/bn_05.png";

            pCnt.Text = SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업")
                ? "(" + inputModel.pCnt + "구)"
                : "(" + inputModel.pCnt + "통)";

            //switch (SettingJsonManager.CountType)
            //{
            //    case 0:
            //        inputModel.pCntVisible = Visibility.Collapsed;
            //        break;
            //    case 1:
            //        inputModel.pCntVisible = Visibility.Visible;
            //        break;
            //    default:
            //        break;
            //}

            mainModel.inputModel = inputModel;
            DataContext = inputModel;

            try
            {
                //마닐라,원단 설정
                switch (metrialitem)
                {
                    //둘중 하나 선택하면
                    case "19":
                    case "20":
                        if (Main1.production != null)
                        {
                            Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청 //종료
                        }
                        else
                        {
                            Main1.arduSerial.Write("A"); //아두이노 카운터 리셋 //시작
                            Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청 //종료
                        }
                        break;

                    default: //기본
                        if (Main.production != null)
                        {
                            Main.arduSerial.Write("B"); //아두이노 현재카운터 요청 //종료
                        }
                        else
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋 //시작
                            Main.arduSerial.Write("B"); //아두이노 현재카운터 요청 //종료
                        }
                        break;
                }
            }
            catch (Exception)
            {
                // MessageBox_Dialog msg = new MessageBox_Dialog(ex.Message);
                // msg.ShowDialog();
            }
            finally
            {
                try
                {
                    switch (metrialitem)
                    {
                        //마닐라,원단 설정
                        case "19": //마닐라
                        case "20": //원단

                            if (Main1.production != null)
                            {
                                Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            else
                            {
                                Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                                Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            break;

                        default:
                            if (Main.production != null)
                            {
                                Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            else
                            {
                                Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                                Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }

            PopResultSearch(); //결과 조회
            PopErrorSearch(); //불량 조회
            TimerSet(); //바코드 타이머
        }

        //바코드 타이머
        private void TimerSet()
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(20)
            };
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        //바코드
        private void Timer_Tick(object sender, EventArgs e)
        {
            _ = tbBarcode.Focus();
            tbBarcode.SelectAll();
        }

        // 불량 사유 리스트 조회
        private void PopErrorSearch()
        {
            _ = new LogData("불량사유조회");
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
               "[PopErrorSearch]",
                new string[] { "@ProcessType" },
                new object[] { inputModel.ProcessType },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    ob = new ObservableCollection<string>();
                    ob1 = new List<string>();
                    foreach (JObject result in Data_Jarry)
                    {
                        string memo = result["Memo"].ToString();
                        ob.Add(memo);
                    }

                    inputModel.errorModel.memo = "";
                    errorOb = ob;

                    if (SettingJsonManager.error.Length == 0 && SettingJsonManager.error1.Length == 0)
                    {
                        combo.ItemsSource = ob;
                    }
                    else
                    {

                        string[] error = SettingJsonManager.error.Split(',');
                        string[] error1 = SettingJsonManager.error1.Split(',');

                        for (int i = 0; i < error.Length; i++)
                        {
                            if (SettingJsonManager.error.Length != 0)
                            {
                                ob1.Add(error[i]);
                            }
                            for (int j = 0; j < ob.Count; j++)
                            {
                                if (ob[j].Equals(error[i]))
                                {
                                    ob.Remove(error[i]);
                                }
                            }
                        }
                        for (int i = 0; i < error1.Length; i++)
                        {
                            if (SettingJsonManager.error1.Length != 0)
                            {
                                ob1.Add(error1[i]);
                            }

                            for (int j = 0; j < ob.Count; j++)
                            {
                                if (ob[j].Equals(error1[i]))
                                {
                                    ob.Remove(error1[i]);
                                }
                            }
                        }
                        ob1 = ob1.Distinct().ToList();
                        combo.ItemsSource = ob1;
                        combo1.ItemsSource = ob;
                    }
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopErrorSearch 조회 오류");
                _ = msg.ShowDialog();
                log.Error("불량사유 리스트 조회 실패");
            }
        }

        //결과리스트 조회
        private void PopResultSearch()
        {
            // Console.WriteLine("PopResultSearch 시작");
            totalPerrCount = 0;
            float tempCount = inputModel.count;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultSearch]",
                new string[] { "@pplankey" },
                new object[] { inputModel.pplanKey },
                0
                );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ObservableCollection<InputModel> ob = new ObservableCollection<InputModel>();
                    inputModel.sumPworkCount = 0;
                    tbEmpty.Visibility = Data_JArray.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    foreach (JObject result in Data_JArray)
                    {
                        InputModel listitem = new InputModel();
                        if (result["mercName"].ToString().Equals(SettingJsonManager.mercName))
                        {
                            listitem.pworkDate = DateTime.Parse(result["pworkDate"].ToString()); //작업일자
                            listitem.pworkCount = float.Parse(result["pworkCount"].ToString());  //작업량
                            listitem.pworkKey = int.Parse(result["pworkKey"].ToString());        //공정작업키
                            listitem.sDatetime = result["sDatetime"].ToString();                 //시작시간
                            listitem.eDatetime = !result["eDatetime"].ToString().Equals("") ? result["eDatetime"].ToString() : ""; //완료시간
                            listitem.orderKey = result["orderKey"].ToString();                   //오더키
                            listitem.pplanKey = int.Parse(result["pplanKey"].ToString());        //계획수량
                            listitem.ProcessType = int.Parse(result["ProcessType"].ToString());  //공정구분
                            listitem.perrCount = float.Parse(result["perrCount"].ToString());    //불량수량
                            listitem.pMemo = result["pMemo"].ToString();                         //비고
                            listitem.errorModelOb = PopErrorListSearch(listitem);               

                            //완료시간 없으면
                            if (!listitem.eDatetime.Equals(""))
                            {
                                errorModel item2 = new errorModel();
                                item2.pworkKey = listitem.pworkKey;
                                item2.memo = "작업완료";

                                //신우, 우리, 누리, 대세
                                if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
                                {
                                    //통수
                                    if (SettingJsonManager.CountType == 1)
                                    {
                                        item2.perrCount = listitem.pworkCount * (1 / mainModel.pCnt);
                                    }
                                    else //매수
                                    {
                                        item2.perrCount = listitem.pworkCount;
                                    }
                                }
                                else //타 업체
                                {
                                    item2.perrCount = listitem.pworkCount;
                                }
                                item2.fontColor = Brushes.Red;
                                item2.updateTime = DateTime.Parse(listitem.eDatetime);

                                listitem.errorModelOb.Add(item2);
                            }

                            //if (listitem.errorModelOb != null)
                            //{
                            //    foreach (errorModel item in listitem.errorModelOb)
                            //    {
                            //        listitem.perrCount += item.perrCount;
                            //    }
                            //}

                            inputModel.sumPworkCount += listitem.pworkCount;
                            ob.Add(listitem);
                        }
                    }

                    //결과값 있으면
                    if (ob.Count > 0)
                    {
                        InputModel tempItem = ob[ob.Count - 1];
                        //시작시간이 "E"가 아니고 종료시간 없을때 (작업중)
                        if ((!tempItem.sDatetime.ToString().Equals("E")) && tempItem.eDatetime.ToString().Equals(""))
                        {
                            //Console.WriteLine("시작");
                            inputModel.imagePath = "/img/bn_06.png";       //input화면 버튼 [종료]
                            mainModel.imagePath = "/img/bn_01.png";        //메인화면 버튼 [작업중]
                            inputModel.pworkDate = tempItem.pworkDate;     //작업일자
                            inputModel.pworkKey = tempItem.pworkKey;       //작업키
                            inputModel.orderKey = tempItem.orderKey;       //오더키
                            inputModel.pplanKey = tempItem.pplanKey;       //계획키
                            inputModel.ProcessType = tempItem.ProcessType; //공정구분
                            inputModel.perrCount = tempItem.perrCount;     //불량수량
                            inputModel.count = tempCount;                  //카운터
                            inputModel.pMemo = tempItem.pMemo;             //비고
                            inputModel.sDatetime = tempItem.sDatetime;     //시작시간
                            inputModel.eDatetime = tempItem.eDatetime;     //종료시간
                        }
                        else //작업 대기중
                        {
                            inputModel.imagePath = "/img/bn_05.png"; //input화면 버튼[시작]
                            mainModel.imagePath = !tempItem.eDatetime.Equals("") ? "/img/bn_03.png" : "/img/bn_02.png"; //메인화면 종료시간 있으면 [완료] , 없으면[대기]
                            inputModel.eDatetime = tempItem.eDatetime; //완료시간
                            inputModel.pworkKey = tempItem.pworkKey;   //작업키
                        }
                    }

                    //통수
                    if (SettingJsonManager.CountType == 1)
                    {
                        //신우, 누리, 우리
                        if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("우리포장산업"))
                        {
                            inputModel.sumPlanCount = (float)Math.Ceiling(inputModel.pplanCount / (float)inputModel.pCnt) - (inputModel.sumPworkCount / (float)inputModel.pCnt);
                        }
                        else //타 업체
                        {
                            inputModel.sumPlanCount = inputModel.pplanCount - inputModel.sumPworkCount;
                        }

                    }
                    else //매수
                    {
                        inputModel.sumPlanCount = inputModel.pplanCount - inputModel.sumPworkCount;
                    }

                    //상자수 사용 체크
                    if (SettingJsonManager.boxCheck)
                    {
                        inputModel.sumPlanCount = inputModel.pplanCount - (inputModel.sumPworkCount / inputModel.pCnt);
                    }

                    ic.ItemsSource = ob;
                }

                //inputModel.pworkCount = inputModel.sumPworkCount;
                txtTotalPerrCount.Text = totalPerrCount.ToString(); //총불량 수량 텍스트
                perrcount.Text = totalPerrCount.ToString(); //불량 수량 텍스트(중앙)
            }
            else
            {
                log.Error("생산결과 조회 실패");

            }
            sv.ScrollToVerticalOffset(ic.ActualHeight);
        }

        //불량 리스트 조회
        private ObservableCollection<errorModel> PopErrorListSearch(InputModel inputModel)
        {
            ObservableCollection<errorModel> ob = null;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

                SettingJsonManager.serverIndex,
                "[PopErrorListSearch]",
                new string[] { "@pworkKey", "@ProcessType" },
                new object[] { inputModel.pworkKey, SettingJsonManager.processType }, 0
                );

            if (returnData.결과 == 1)
            {
                ob = new ObservableCollection<errorModel>();
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    errorModel startItem = new errorModel
                    {
                        updateTime = DateTime.Parse(inputModel.sDatetime),
                        memo = "작업시작",
                        fontColor = Brushes.Green,
                        fontVisible = Visibility.Collapsed
                    };

                    ob.Add(startItem);
                    foreach (JObject result in Data_JArray)
                    {
                        errorModel item = new errorModel
                        {
                            InputModel = inputModel,
                            perrKey = result["perrKey"].ToString(), //불량키
                            memo = result["Memo"].ToString(), //비고
                            perrCount = float.Parse(result["perrorCount"].ToString()), //불량수량
                            updateTime = DateTime.Parse(result["updateTime"].ToString()), //업데이트 시간
                            fontColor = Brushes.Black
                        };
                        totalPerrCount += item.perrCount;
                        ob.Add(item);
                    }
                }
            }
            else
            {
                log.Error("불량리스트 조회 실패");
            }
            return ob;
        }

        // 화면 드러그
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //작업내역 창 열기 버튼 클릭
        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkChangeButton)
            {
                changeButtonIn.Visibility = Visibility.Collapsed;
                changeButtonOut.Visibility = Visibility.Visible;
                checkChangeButton = false;
                Width = 550;
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

        //생산 수량 버튼 클릭 (실시간/결과값)
        private void TbResult_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (sender.GetType().Name.Equals("TextBox")) //텍스트박스 클릭
            {
                TextBox context = (TextBox)sender;

                //결과 카운터 텍스트 클릭
                if (context.Name.Equals("tbResult")) 
                {
                    //Point clickPoint = e.GetPosition(null);

                    //// 클릭한 위치의 모니터 정보를 가져옵니다.
                    //foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                    //{
                    //    Rect bounds = new Rect(screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Width, screen.Bounds.Height);
                    //    if (bounds.Contains(clickPoint))
                    //    {
                    //        Win_KeyBoard win_KeyBoard = new Win_KeyBoard(context, context.DataContext, inputModel);

                    //        // 클릭한 모니터의 작업 영역 중앙 좌표를 찾습니다.
                    //        double centerX = screen.WorkingArea.Left + (screen.WorkingArea.Width / 2);
                    //        double centerY = screen.WorkingArea.Top + (screen.WorkingArea.Height / 2);

                    //        // 윈도우를 해당 모니터에 표시합니다.
                    //        win_KeyBoard.WindowStartupLocation = WindowStartupLocation.Manual;
                    //        win_KeyBoard.Topmost = true; // 윈도우를 최상위로 표시

                    //        win_KeyBoard.ShowDialog();
                    //        win_KeyBoard = null;
                    //    }
                    //}

                    win_KeyBoard = new Win_KeyBoard(context, context.DataContext, inputModel)
                    {
                        Left = SystemParameters.WorkArea.Width - this.Width - 10,
                        Top = (SystemParameters.WorkArea.Height / 2) - this.Top
                    };
                    _ = win_KeyBoard.ShowDialog(); //키보드창 팝업
                    win_KeyBoard = null;
                }
                else
                {
                    //설정에서 키인사용 체크시
                    if (SettingJsonManager.keyinCheck)
                    {
                        tbError.Focusable = true;
                        _ = tbError.Focus();
                        _ = tbError.Dispatcher.BeginInvoke(new Action(() => { tbError.SelectAll(); }));
                    }
                    //미체크
                    else 
                    {
                        InputModel inputmodel = (InputModel)context.DataContext;
                        win_KeyBoard = new Win_KeyBoard(context, inputmodel.errorModel, inputmodel)
                        {

                            Left = SystemParameters.WorkArea.Width - Width,
                            Top = (SystemParameters.WorkArea.Height / 2) - Top
                        };
                        _ = win_KeyBoard.ShowDialog();
                        win_KeyBoard = null;
                    }
                }
            }
        }

        //생산수량 - 버튼
        private void Result_count_minus(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image img = (Image)sender;
                InputModel context = (InputModel)img.DataContext;
                float value;

                if (SettingJsonManager.CountType == 0) //매수
                {
                    value = 1;
                }
                else //통수
                {
                    value = SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩")
                        ? SettingJsonManager.twoSum ? (inputModel.pCnt * 2) : 1
                        : SettingJsonManager.twoSum ? (inputModel.pCnt * 2) : inputModel.pCnt;

                    if (SettingJsonManager.boxCheck)
                    {
                        value = 1 * inputModel.pCnt;
                    }
                }

                if (context.pworkCount - value >= 0)
                {
                    inputModel.pworkCount -= value;
                    inputModel.count -= value;
                }
            }
        }
        //생산 수량 + 버튼
        private void Result_count_plus(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {

                float value;
                if (SettingJsonManager.CountType == 0) //매수
                {
                    value = 1;
                }
                else //통수
                {
                    if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("우리포장산업") || SettingJsonManager.corp_name.Equals("누리팩"))
                    {
                        value = SettingJsonManager.twoSum ? (inputModel.pCnt * 2) : 1;
                    }
                    else
                    {
                        value = SettingJsonManager.twoSum ? (inputModel.pCnt * 2) : inputModel.pCnt;
                    }

                    if (SettingJsonManager.boxCheck)
                    {
                        value = 1 * inputModel.pCnt;
                    }
                }
                inputModel.pworkCount += value;
                inputModel.count += value;
            }
        }

        //작업내역 상세창 리스트 클릭
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (e.ClickCount == 2) //더블클릭
            {
                if (sender.GetType().Name.Equals("Border"))
                {
                    Border border = (Border)sender;
                    errorModel context = (errorModel)border.DataContext;
                    //Console.WriteLine("메모" + context.memo);

                    if (!context.memo.Equals("작업시작") && !context.memo.Equals("작업완료")) 
                    {
                        can.Visibility = Visibility.Visible;
                        Win_Update win_Update = new Win_Update(context, errorOb);
                        _ = win_Update.ShowDialog();
                        PopResultSearch();
                        can.Visibility = Visibility.Collapsed;
                        checkInput = true;
                    }
                    else if (context.memo.Equals("작업완료")) //작업완료칸 클릭
                    {

                        can.Visibility = Visibility.Visible;
                        Win_resultUpdate win_resultUpdate = new Win_resultUpdate(context, inputModel);
                        _ = win_resultUpdate.ShowDialog();
                        PopResultSearch();
                        can.Visibility = Visibility.Collapsed;
                        checkInput = true;

                    }
                }
            }
        }

        //인라인설비 시작
        private int PopInlineStart(SubModel subOb)
        {

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultStart]",
                new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey" },
                new object[] {
                DateTime.Now.ToString("yyyy-MM-dd"),
                subOb.orderKey,
                subOb.pplanKey,
                subOb.processType,
                subOb.ProcessTypeName,
                MainWindow.userID,
                MainWindow.userName,
                inputModel.workerName,
                inputModel.workerId,
                inputModel.SCMKey},
                1

                );

            if (returnData.결과 == 1)
            {
                checkInput = true;

                SettingJsonManager.pworkkey = returnData.리턴값;
                SettingJsonManager.SaveJsonFile();
                //Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                InvokeCount(SettingJsonManager.mercName, "S");
                InvokeCount(SettingJsonManager.mercName, "0");
                SettingJsonManager.count = 0;
                SettingJsonManager.saveDate = "";
                SettingJsonManager.pplanKey = 0;
                SettingJsonManager.SaveJsonFile();

                PopInlineEnd(subOb, int.Parse(returnData.리턴값));
                switch (metrialitem)
                {
                    case "19":
                    case "20":
                        Main1.production = mainModel;
                        try
                        {
                            if (Main1.production != null)
                            {
                                Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                                Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            else
                            {
                                Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                            msg.ShowDialog();
                        }
                        break;
                    default:
                        Main.production = mainModel;

                        try
                        {
                            if (Main.production != null)
                            {
                                Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                                Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }

                            else
                            {
                                Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                            }
                        }

                        catch (Exception e)
                        {
                            MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                            // msg.ShowDialog();
                        }

                        break;
                }

            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
                log.Error("불량리스트 조회 실패");
            }
            return returnData.결과;

        }

        private void PopInlineEnd(SubModel subOb, int pWorkkey)
        {
            string 전체공정 = "0";
            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (SettingJsonManager.mercName.Contains("인라인"))
                {
                    //if (mainModel.subOb.Count == 2)
                    //{
                    //    전체공정 = "1";
                    //}
                    //else
                    //{
                    //    전체공정 = "0";
                    //}
                    전체공정 = "1";

                }
            }
            string arrChkAED = "1|";

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string parameter = "[" + subOb.ProcessTypeName + "] " + this.inputModel.customSName + Environment.NewLine + inputModel.prodName + " " + inputModel.pworkCount.ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEnd]",
                new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명","@전체공정"},
                new object[] {
                "1",
                DateTime.Now.ToString("yyyy-MM-dd"),
                subOb.orderKey,
                arrChkAED,
                subOb.processType + "|",
                subOb.pplanKey + "|",
                pWorkkey + "|",
                inputModel.pworkCount + "|",
                inputModel.perrCount + "|",
                inputModel.pMemo + "|",
                inputModel.sDatetime,
                subOb.ProcessTypeName,
                전체공정
                }, 1, parameter
              );
            if (returnData.결과 == 1)
            {
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);

                count = inputModel.pworkCount;
                //Result_Update.Visibility = Visibility.Visible;
                mainModel.presultCount = inputModel.pworkCount + inputModel.perrCount;
                mainModel.presultCount1 = inputModel.pworkCount + inputModel.perrCount;
                mainModel.perrorCount = (int)inputModel.perrCount;

                if (count > 0)
                {
                    box1.Visibility = Visibility.Visible;
                    Canvas.SetTop(box, 490);
                    Canvas.SetTop(box1, 570);
                }

                else
                {
                    box1.Visibility = Visibility.Collapsed;
                    Canvas.SetTop(box, 540);
                }
                Main.production = null;
            }
            Main.production = null;
            SettingJsonManager.consumersumcount = 0;
            SettingJsonManager.count = 0;
            SettingJsonManager.saveDate = "";
            SettingJsonManager.pplanKey = 0;
            SettingJsonManager.pworkkey = "";
            SettingJsonManager.SaveJsonFile();
        }

        //닫기 버튼 클릭
        private void Win_input_exit(object sender, MouseButtonEventArgs e)
        {
            //if (mainSock.Connected)
            //{
            //    mainSock.Close();
            //}
            Close();
            log.Info("인풋창 닫기");
        }

        //상세 내역 화면 호출
        private void Detail_item(object sender, MouseButtonEventArgs e)
        {
            Close();
            string special = "";

            for (int i = 0; i < workList.Count; i++)
            {
                if (mainModel.orderKey.Equals(workList[i].orderKey.ToString()))
                {
                    special += workList[i].ProcessTypeName + " 참고 사항 : " + workList[i].ProcessMemo + "|";
                }
            }

            Win_Detail win_Detail = new Win_Detail(mainModel, special, workList, SettingJsonManager.detail_check, CNS_Send);
            win_Detail.ShowDialog();
        }

        //시작 & 종료 버튼
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //시작 버튼 활성화 일때
            if (inputModel.imagePath.Equals("/img/bn_05.png")) 
            {
                Console.WriteLine("생산 시작한 prodID : " + this.mainModel.prodID);
                string metrialitem = SettingJsonManager.meterialinfo;
                string sCustomSName = "";
                string sProdName = "";

                switch (metrialitem)
                {
                    //마닐라
                    case "19":
                    case "20":
                        if (Main1.production != null)
                        {
                            //생상중이 거래서/품명 표현
                            sCustomSName = Main.production.customSName;
                            sProdName = Main.production.prodName;
                            MessageBox_Dialog messageBox = new MessageBox_Dialog("거래처: " + sCustomSName + "\n" +
                                                                                 "품명: " + sProdName + "\n" +
                                                                                 "해당 건이 작업중입니다.");
                            _ = messageBox.ShowDialog();
                        }
                        else
                        {
                            _ = new LogData("작업시작");
                            _ = PopResultStart();
                            PopResultSearch();
                            log.Info(Main.production.customSName + " / " + Main.production.prodName + " [생산시작]");
                        }

                        break;

                    //일반
                    default:
                        if (Main.production != null)
                        {
                            //생상중이 거래서/품명 표현
                            sCustomSName = Main.production.customSName;
                            sProdName= Main.production.prodName;

                            MessageBox_Dialog messageBox = new MessageBox_Dialog("거래처: " + sCustomSName + "\n" +
                                                                                 "품명: " + sProdName + "\n" + 
                                                                                 "해당 건이 작업중입니다.");
                            _ = messageBox.ShowDialog();
                        }
                        else
                        {
                            _ = new LogData("작업시작");
                            _ = PopResultStart();
                            PopResultSearch();
                            log.Info(Main.production.customSName + " / " + Main.production.prodName + " [생산시작]");
                        }
                        break;
                }

            }
            //종료버튼 활성화 일때
            else if (inputModel.imagePath.Equals("/img/bn_06.png")) 
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("작업을 저장 하시겠습니까 ?", "작업 저장", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    Close();
                }
                else
                {
                    LogData logData = new LogData("작업종료");
                    PopResultEnd(); //결과값 조회

                    if (SettingJsonManager.Plc) //PLC 사용 체크일때
                    {
                        //CNSDataRead(); //CNS 데이터 읽기
                    }

                    inputModel.perrCount = 0;
                    inputModel.count = 0;
                    inputModel.pMemo = "";
                    inputModel.pworkCount = 0;
                    PopResultSearch();

                    log.Info(inputModel.customSName + " / " + inputModel.prodName + " [생산종료] " + inputModel.sumPworkCount + "개");
                }
            }

        }




        bool cnsProcessing = false;

        private void CNSDataRead() //데이터 수신 요청
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
                Console.WriteLine("CNS송신 데이터(input) : " + Send_Data);

                cnsProcessing = true;
                mainSock.Send(bDts); //데이터 쓰기
            }
            catch (Exception)
            {
            }
        }

        //CNS PLC 통신연결
        private void ConnectSocket()
        {
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
                            mainSock.EndConnect(result);
                            AsyncObject obj = new AsyncObject(500);
                            obj.WorkingSocket = mainSock;
                            obj.ClearBuffer();
                            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                        }
                        else //연결 실패
                        {
                            mainSock.Close();
                            MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                            msg.ShowDialog();
                        }
                    }
                }
                catch (Exception e)
                {
                    mainSock.Close();
                    MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                    msg.ShowDialog();
                }
            }
        }

        //CNS데이터 DB에 저장
        public void DataReceived(IAsyncResult ar) // 데이터 수신
        {
            Resipe_Model itemModel = new Resipe_Model();

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
                Console.WriteLine("CNS수신 데이터(input) : " + RD);

                for (int i = 0; i < RD.Length; i++)
                {
                    string high_data = RD.Substring(0, 2); //앞 두자리 추출
                    string low_data = RD.Substring(2, 2); //뒤 두자리 추출

                    switch (i)
                    {
                        case 0: //작업모델 번호
                            itemModel.ModelNum = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 1: //작업지시 매수
                            itemModel.WorkPlan1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 2: //작업지시 매수
                            itemModel.WorkPlan2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 3: //박스타입
                            itemModel.BoxType = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 4: //폭
                            itemModel.Pock = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 5: //장
                            itemModel.Jang = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 6: //고
                            itemModel.Go = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 7: //갭
                            itemModel.Gap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 8: //앞날개
                            itemModel.Wing = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 9: //미미
                            itemModel.Cutting = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 10: //저장 완료
                            itemModel.SaveDone = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 11: //생산속도
                            itemModel.WorkSpeed = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 12: //저장값 쓰기 가능상태
                            itemModel.SaveState = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 13: //spare
                            itemModel.spare = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 14: //A
                            itemModel.aRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 15: //B
                            itemModel.bRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 16: //C
                            itemModel.cRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 17: //D
                            itemModel.dRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 18: //E
                            itemModel.eRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 19: //F1
                            itemModel.f1Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 20: //F2
                            itemModel.f2Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 21: //G
                            itemModel.g2pRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 22: //O
                            itemModel.oRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 23: //P1
                            itemModel.p1Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 24: //P2
                            itemModel.p2pRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 25: //P3
                            itemModel.p3Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 26: //P4
                            itemModel.p4Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 27: //spare
                            itemModel.spare2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 28: //spare
                            itemModel.spare3 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 29: //슬로타
                            itemModel.Slotter = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 30: //타이캇타
                            itemModel.DieCutter = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 31: //프린터1 풀 롤
                            itemModel.PrinterPull1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 32: //프린터1 프레스 롤
                            itemModel.PrinterRoll1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 33: //프린터2 풀 롤
                            itemModel.PrinterPull2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 34: //프린터2 프레스 롤
                            itemModel.PrinterRoll2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 35: //프린터3 풀 롤
                            itemModel.PrinterPull3 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 36: //프린터3 프레스 롤
                            itemModel.PrinterRoll3 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 37: //프린터4 풀 롤
                            itemModel.PrinterPull4 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 38: //프린터 프레스 롤
                            itemModel.PrinterRoll4 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 39: //spare
                            itemModel.spare4 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 40: //spare
                            itemModel.spare5 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 41: //spare
                            itemModel.spare6 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 42: //spare
                            itemModel.spare7 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 43: //피다진입 갭
                            itemModel.FeederGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 44: //피타진입 롤
                            itemModel.FeederRoll = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 45: //슬로타 보조쾌선 갭
                            itemModel.SlotterSubGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 46: //슬로타 본(중간) 갭
                            itemModel.SlotterMiddleGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 47: //슬로타 연결(배출) 갭
                            itemModel.SlotterConnection = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 48: //다이캇타 롤 갭
                            itemModel.DieCutterRollGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 49: //글루어벨트 진앱 갭 F1
                            itemModel.GluerbeltF1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 50: //글루어벨트 진앱 갭 F2
                            itemModel.GluerbeltF2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 51: //글루어 프레스롤 갭 A
                            itemModel.GluerPressA = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 52: //글루어 프레스롤 갭 B
                            itemModel.GluerPressB = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 53: //다이캇타 롤 갭A
                            itemModel.DieCutterRollA = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 54: //다이캇타 롤 갭B
                            itemModel.DieCutterRollB = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 55: //글루어 PE 가이드
                            itemModel.GluerPE = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 56: //R/B 보조괘선 진입 롤
                            itemModel.BRSub = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 57: //R/B 본괘선 중간 롤
                            itemModel.BRMain = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 58: //R/B 연결 배출 롤
                            itemModel.BRConnection = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 59: //R/B 연결 중간 롤
                            itemModel.BRmiddle = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 60: //spare
                            itemModel.spare8 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 61: //spare
                            itemModel.spare9 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 62: //spare
                            itemModel.spare10 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 63: //spare
                            itemModel.spare11 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 64: //spare
                            itemModel.spare12 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 65: //spare
                            itemModel.spare13 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 66: //spare
                            itemModel.spare14 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 67: //총생산량(하위) 
                            itemModel.TotalCount1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 68: //총생산량(상위)
                            itemModel.TotalCount2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 69: //현재생산량(하위)
                            itemModel.Count1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 70: //현재생산량(상위)
                            itemModel.Count2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;

                        default:
                            break;
                    }
                    continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                    RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                }

                Console.WriteLine("CNS수신 데이터(input) : " + continuous_read); //수신값 확인용


                if (cnsProcessing)
                {
                    ReturnModel returnData = ServiceModel.GetData(
                        SettingJsonManager.serverIndex,
                        "[PopCNSUpdate]",
                        new string[] { 
                            "@prodID",
                            "@MachineName",
                            "@ModelNum",
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
                        new object[] { this.mainModel.prodID, SettingJsonManager.mercName, itemModel.ModelNum, itemModel.WorkPlan1, itemModel.WorkPlan2, itemModel.BoxType,
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
                        checkInput = true;
                    }
                    else
                    {
                    }

                    cnsProcessing = false;
                    MessageBoxResult result = System.Windows.MessageBox.Show("저장이 완료되었습니다", "CNS 데이터 저장");
                }

                obj.ClearBuffer();
                Console.WriteLine("CNS => DB 저장완료 prodID :  " + this.mainModel.prodID);
            }
            catch (Exception)
            {
                MessageBox_Dialog messageBox = new MessageBox_Dialog("레시피 저장 실패");
                _ = messageBox.ShowDialog();
                mainSock.Close();
            }
            finally
            {
                mainSock.Close();
            }
        }

        //생산 종료
        private int PopResultEnd()
        {
            string 전체공정 = "0";
            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (SettingJsonManager.mercName.Contains("인라인"))
                {
                    전체공정 = "1";
                }
            }

            //결과수량
            inputModel.pworkCount = int.Parse(tbResult.Text.ToString());

            //생산,불량수량 0이면 3| 경과수량 있으면 1|
            string arrChkAED = inputModel.pworkCount == 0 && inputModel.perrCount == 0 ? "3|" : "1|";

            //상자 수 사용 체크시 통수 곱해줌
            if (SettingJsonManager.boxCheck)
            {
                inputModel.pworkCount = (int)(this.inputModel.pworkCount * this.inputModel.pCnt);
            }

            //통수일때
            if (SettingJsonManager.CountType == 1)
            {
                //신우, 누리, 우리
                if (SettingJsonManager.corp_name.Equals("신우수출포장") || SettingJsonManager.corp_name.Equals("누리팩") || SettingJsonManager.corp_name.Equals("우리포장산업"))
                {
                    inputModel.pworkCount = (int)(this.inputModel.pworkCount * this.inputModel.pCnt);
                }       
            }

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string parameter = "[" + SettingJsonManager.mercName + "] " + this.inputModel.customSName + Environment.NewLine + inputModel.prodName + " " + inputModel.pworkCount.ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEnd]",
                new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명", "@전체공정"},
                new object[] {
                "1",
                DateTime.Now.ToString("yyyy-MM-dd"),
                inputModel.orderKey,
                arrChkAED,
                inputModel.ProcessType + "|",
                inputModel.pplanKey + "|",
                inputModel.pworkKey + "|",
                Math.Ceiling(inputModel.pworkCount) + "|",
                inputModel.perrCount + "|",
                inputModel.pMemo + "|",
                inputModel.sDatetime,
                SettingJsonManager.mercName,
                전체공정
                }, 1, parameter
              );

            if (returnData.결과 == 1)
            {

                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);

                count = inputModel.pworkCount;
                //Result_Update.Visibility = Visibility.Visible;

                if (count > 0)
                {
                    box1.Visibility = Visibility.Visible;
                    Canvas.SetTop(box, 490);
                    Canvas.SetTop(box1, 570);
                }

                else
                {
                    box1.Visibility = Visibility.Collapsed;
                    Canvas.SetTop(box, 540);
                }

                checkInput = true;
                inputModel.imagePath = "/img/bn_05.png"; //시작
                mainModel.imagePath = "/img/bn_02.png"; //대기
                mainModel.sumPworkCount = inputModel.pworkCount + inputModel.perrCount;

                mainModel.presultCount = inputModel.pworkCount + inputModel.perrCount;
                mainModel.presultCount1 = inputModel.pworkCount + inputModel.perrCount;

                mainModel.perrorCount = (int)inputModel.perrCount;
                for (int i = 0; i < mainModel.subOb.Count; i++)
                {
                    if (SettingJsonManager.processType == mainModel.subOb[i].processType)
                    {
                        mainModel.subOb[i].stateColor = Brushes.Black;
                        mainModel.subOb[i].stateFontColor = Brushes.White;
                    }
                }

                InvokeCount(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
                InvokeCount(SettingJsonManager.mercName, "0"); //

                //오주포장, 철접합(7),베가(8),인라인2,3호기(13), 5는 없음
                if (SettingJsonManager.corp_name.Equals("오주포장") && (SettingJsonManager.processType == 7
                    || SettingJsonManager.processType == 5
                    || SettingJsonManager.processType == 8
                    || SettingJsonManager.processType == 13
                    ))
                {
                    for (int i = 0; i < mainModel.subOb.Count; i++)
                    {
                        if (mainModel.subOb[i].ProcessTypeName.Equals("인쇄"))
                        {
                            if (mainModel.subOb[i].presultCount < 1)
                            {
                                PopInlineStart(mainModel.subOb[i]);
                            }
                        }
                    }
                }

                //성주포장 후렉소
                if (SettingJsonManager.corp_name.Equals("성주포장") && SettingJsonManager.mercName.Equals("후렉소") && (SettingJsonManager.processType == 1))
                {
                    for (int i = 0; i < mainModel.subOb.Count; i++)
                    {

                        if (mainModel.subOb[i].ProcessTypeName.Equals("인쇄"))
                        {
                            if (mainModel.subOb[i].presultCount < 1)
                            {
                                PopInlineStart(mainModel.subOb[i + 1]);
                            }
                        }
                    }
                }

                //구룡산업
                if(SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
                {
                    //if (SettingJsonManager.mercName.Contains("인라인"))
                    //{
                    //    if (mainModel.subOb.Count == 2)
                    //    {
                    //        PopInlineStart(mainModel.subOb[mainModel.subOb.Count - 1]);
                    //    }
                       
                    //}
                }

                //아두이노 리셋
                switch (metrialitem)
                {
                    case "19":
                    case "20":
                        Main1.production = null;

                        try
                        {
                            Main1.arduSerial.Write("A"); //아두이노 카운터 리셋 
                            Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청 // 
                        }
                        catch (Exception)
                        {

                        }
                        break;

                    default:
                        Main.production = null;
                        try
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                            Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                        }

                        catch (Exception e)
                        {
                            //MessageBox_Dialog msg = new MessageBox_Dialog(e.Message);
                            //msg.Show();
                        }
                        break;
                }

                SettingJsonManager.consumersumcount = 0;
                SettingJsonManager.count = 0;
                SettingJsonManager.saveDate = "";
                SettingJsonManager.pplanKey = 0;
                SettingJsonManager.pworkkey = "";
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopResultEnd 저장실패 !!");
                msg.ShowDialog();
            }

            return returnData.결과;
        }

        //시작
        private int PopResultStart()
        {

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultStart]",
                new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey" },
                new object[] {
                DateTime.Now.ToString("yyyy-MM-dd"),
                inputModel.orderKey,
                inputModel.pplanKey,
                inputModel.ProcessType,
                SettingJsonManager.mercName,
                MainWindow.userID,
                MainWindow.userName,
                inputModel.workerName,
                inputModel.workerId,
                inputModel.SCMKey},
                1

                );

            if (returnData.결과 == 1)
            {
                checkInput = true;
                inputModel.pworkKey = int.Parse(returnData.리턴값);
                SettingJsonManager.pworkkey = returnData.리턴값;
                SettingJsonManager.SaveJsonFile();
                //Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                InvokeCount(SettingJsonManager.mercName, "S");//현황판으로 시작신호 송신 (설비명,S)
                InvokeCount(SettingJsonManager.mercName, "0"); //현황판으로 생산종료 송신
                SettingJsonManager.count = 0;
                SettingJsonManager.saveDate = "";
                SettingJsonManager.pplanKey = 0;
                SettingJsonManager.SaveJsonFile();

                switch (metrialitem)
                {
                    case "19": //마닐라
                    case "20": //원단
                        Main1.production = mainModel;
                        try
                        {
                            if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트")) 
                            {
                                
                            }
                            else
                            {
                                if (Main1.production != null)
                                {
                                    Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                                    Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                                }

                                else
                                {
                                    Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                                }

                            }         
                        }
                        catch (Exception)
                        {
                            //MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                            //msg.ShowDialog();
                        }
                        break;

                    default: //일반

                        Main.production = mainModel;

                        try
                        {
                            if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
                            {
                            }
                            else
                            {
                                if (Main.production != null)
                                {
                                    Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                                    Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                                }
                                else
                                {
                                    Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                            // msg.ShowDialog();
                        }

                        break;
                }
                //if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("대양포장"))
                //{
                //    if (SettingJsonManager.Plc)
                //    {
                //        try
                //        {
                //            string[] psize;
                //            psize = mainModel.pSize.Split('*');
                //            SettingJsonManager.mimi = 35;
                //            SettingJsonManager.jang = int.Parse(psize[0].Trim());
                //            SettingJsonManager.pok = int.Parse(psize[1].Trim());
                //            SettingJsonManager.go = int.Parse(psize[2].Trim());
                //            SettingJsonManager.setCount = (int)mainModel.pplanCount;
                //            switch (mainModel.paperGor.Replace("골", "").Trim())
                //            {
                //                case "AB":
                //                case "BA":
                //                    SettingJsonManager.Gap = 70;
                //                    break;

                //                case "BB":
                //                    SettingJsonManager.Gap = 60;
                //                    break;

                //                case "A":
                //                    SettingJsonManager.Gap = 50;
                //                    break;

                //                case "EB":
                //                    SettingJsonManager.Gap = 40;
                //                    break;

                //                case "B":
                //                    SettingJsonManager.Gap = 30;
                //                    break;

                //                case "E":
                //                    SettingJsonManager.Gap = 20;
                //                    break;
                //                default:
                //                    break;
                //            }
                //            SettingJsonManager.SaveJsonFile();
                //        }
                //        catch (Exception)
                //        {
                //            SettingJsonManager.mimi = 35;
                //            SettingJsonManager.jang = 0;
                //            SettingJsonManager.pok = 0;
                //            SettingJsonManager.go = 0;
                //            SettingJsonManager.setCount = 99999;
                //            SettingJsonManager.Gap = 7;
                //            SettingJsonManager.SaveJsonFile();
                //        }
                //        finally
                //        {
                //            if (SettingJsonManager.Plc)
                //            {
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;
        }

        //현황판 전송 POP 상태값(S,E,0)
        private void InvokeCount(string mercName, string mercState)
        {
            //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
            try
            {
                if (SettingJsonManager.corp_name.Equals("광성수출포장") ||
                    SettingJsonManager.corp_name.Equals("광성수출포장테스트") ||
                    SettingJsonManager.corp_name.Equals("대세물산") ||
                    SettingJsonManager.corp_name.Equals("대세물산테스트") ||
                    SettingJsonManager.corp_name.Equals("유창포장") ||
                    SettingJsonManager.corp_name.Equals("유창포장테스트"))
                {
                    inputPopService(mercName, mercState);
                }
                else
                {
                    SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, mercState);
                }
            }
            catch (Exception e)
            {
            }
        }

        //신규 서비스 Http 셋팅
        private static HttpClient client = new HttpClient();
        async void inputPopService(string mercName, string str)
        {
            //주소
            if (SettingJsonManager.corp_name.Equals("광성수출포장"))
            {
                Url = "http://ksbox.iptime.org:18888/pop/gathering"; //광성
            }
            else if (SettingJsonManager.corp_name.Equals("대세물산"))
            {
                Url = "http://dsbox.iptime.org:18888/pop/gathering"; //대세
            }
            else if (SettingJsonManager.corp_name.Equals("유창포장"))
            {
                Url = "http://211.253.37.232:18888/pop/gathering"; //유창
            }
            else if (SettingJsonManager.corp_name.Equals("광성수출포장테스트") ||
                    SettingJsonManager.corp_name.Equals("대세물산테스트") ||
                    SettingJsonManager.corp_name.Equals("유창포장테스트"))
            {
                Url = "http://localhost:18888/pop/gathering"; //테스트용 주소(개인 DeskTop)
            }

            string CompanyCode = SettingJsonManager.corp_id; //업체코드
            string MercName = mercName; //설비명
            string Data = str.ToString(); //데이터

            await inputWorkCnt(Url, CompanyCode, MercName, Data);
        }

        ////신규 서비스 Http Get
        async Task inputWorkCnt(string url, string companyCode, string mercName, string data)
        {
            try
            {
                // 파라미터를 포함한 URL 생성
                string urlWithParameters = $"{url}/{companyCode}/{mercName}/{data}";
                // HTTP GET 요청 보내기
                await client.GetAsync(urlWithParameters).ContinueWith((t) =>
                {
                    //HttpResponseMessage response;

                    if (t.IsFaulted)
                    {
                        log.Error("서비스 전송 후 대기 중 문제가 발생했습니다.");
                    }
                    else
                    {
                        //response = t.Result;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                //HttpResponseMessage response = await client.GetAsync(urlWithParameters);

                Console.WriteLine(urlWithParameters);
            }
            catch (Exception)
            {
            }
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image img = (Image)sender;
                InputModel context = (InputModel)img.DataContext;
                if (!tbError.Text.Equals("0") && !context.errorModel.memo.Equals(""))
                {
                    if (inputModel.pworkKey != 0)
                    {
                        if (popErrorInsert() == 1)
                        {
                            context.errorModel.perrCount = 0;
                            PopResultSearch();
                        }
                        else
                        {
                            MessageBox_Dialog msg = new MessageBox_Dialog("불량 데이터 저장 오류 !!");
                            msg.ShowDialog();
                        }
                    }
                }
            }
        }

        private int popErrorInsert()
        {
            _ = new LogData("불량입력");
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            float value;
            switch (SettingJsonManager.processType)
            {
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                    value = 1;
                    break;
                default:
                    value = (float)inputModel.pCnt;
                    break;

            }

            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopErrorInsert]",
                new string[] { "@pworkKey", "@perrorCount", "@Memo" },
                new object[] { inputModel.pworkKey, int.Parse(tbError.Text) * value, inputModel.errorModel.memo },
                1
                );

            if (returnData.결과 == 1)
            {
                checkInput = true;

                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void result_badCount_minus(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            InputModel context = (InputModel)img.DataContext;
            int value = 1;

            if (context.errorModel.perrCount - value >= 0)
            {
                context.errorModel.perrCount -= value;
                // tbError.Text = inputModel.perrCount + "";
            }
        }

        private void Result_badCount_plus(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            InputModel context = (InputModel)img.DataContext;
            int value = 1;
            context.errorModel.perrCount += value;
            //tbError.Text = inputModel.perrCount + "";
        }

        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }

        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {

            string code = tbBarcode.Text;
            tbBarcode.Text = "";
            if (PopSearchWorkerName(code) != true)
            {
                PopScmCheck(code);
            }

        }

        private void PopScmCheck(string SCMKey)
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,

                "[PopScmCheck]",
                new string[] { "@SCMKey" },
                new object[] { SCMKey },
                0

                );

            if (returnData.결과 == 1)
            {
                JArray Data_JArry = JArray.Parse(returnData.리턴값);
                if (Data_JArry.Count > 0)
                {
                    foreach (JObject result in Data_JArry)
                    {
                        if (inputModel.orderKey.Equals(result["orderKey"].ToString()))
                        {
                            inputModel.SCMKey = SCMKey;
                            tbCheck.Text = "원단확인";
                        }
                        else
                        {
                            inputModel.sDatetime = "";
                        }
                    }
                }
                else
                {
                    inputModel.sDatetime = "";
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopScmCheck 조회 오류!!");
                msg.ShowDialog();
            }
        }

        private bool PopSearchWorkerName(string workerID)
        {
            bool bool_return = false;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopSearchWorkerName]",
                new string[] { "@workerID" },
                new object[] { workerID },
                0

                );

            if (returnData.결과 == 1)
            {
                JArray Data_JArry = JArray.Parse(returnData.리턴값);
                if (Data_JArry.Count > 0)
                {
                    foreach (JObject result in Data_JArry)
                    {
                        inputModel.workerName = result["UserName"].ToString();
                        inputModel.workerId = workerID;
                        bool_return = true;
                    }
                    return bool_return;
                }
                else
                {
                    inputModel.workerId = "";
                    inputModel.workerName = "";
                    bool_return = false;
                    return bool_return;

                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopSearchWorkerName 조회 오류!!");
                msg.ShowDialog();
                bool_return = false;
                return bool_return;
            }
        }
        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv1);
            hOff = sv1.HorizontalOffset;
            vOff = sv1.VerticalOffset;
            sv1.CaptureMouse();
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        //이월 버튼
        private void 이월(object sender, MouseButtonEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(mainModel.prodName + " 품목을 이월 하시겠습니까 ?", "이월", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                Close();
            }
            else
            {
                // 결과값 있을때만
                if ((mainModel.presultCount > 0 || count > 0) && inputModel.sumPlanCount > 0)
                {
                    spAdd410processinfo_workplan();
                }
                else
                {
                    MessageBox_Dialog dialog = new MessageBox_Dialog("이월 작업을 할 수 없습니다.");
                    dialog.ShowDialog();
                }
            }
            // Console.WriteLine(mainModel.presultCount + "");
        }

        private void spAdd410processinfo_workplan()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[spAdd410processinfo_workplan]",
                new string[] { "@오더Key", "@계획Key" },
                new object[] { inputModel.orderKey, inputModel.pplanKey },
                1
                );
            if (returnData.결과 == 1)
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("이월 완료");
                msg.ShowDialog();
                box1.Visibility = Visibility.Collapsed;
                Canvas.SetTop(box, 540);
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("이월 실패");
                msg.ShowDialog();
            }
        }

        //특이사항 버튼 클릭
        private void Unique(object sender, MouseButtonEventArgs e)
        {
            Unique_input unique_Input = new Unique_input(inputModel);
            unique_Input.ShowDialog();
        }

        private void Result_modi(object sender, MouseButtonEventArgs e)
        {

        }

        private void modi(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Border txt = (Border)sender;
            errorModel ob = (errorModel)txt.DataContext;
            // Console.WriteLine(ob.memo);
        }

        private void Result_update(object sender, MouseButtonEventArgs e)
        {
            Win_resultUpdate win_resultUpdate = new Win_resultUpdate(inputModel);
            win_resultUpdate.ShowDialog();
            PopResultSearch();
        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv1.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv1.IsMouseCaptured)
            {
                sv1.ScrollToHorizontalOffset(hOff1 + (scrollMousePoint1.X - e.GetPosition(sv1).X));
                sv1.ScrollToVerticalOffset(vOff1 + (scrollMousePoint1.Y - e.GetPosition(sv1).Y));
            }
        }

        private void Sv_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint1 = e.GetPosition(sv1);
            hOff1 = sv1.HorizontalOffset;
            vOff1 = sv1.VerticalOffset;
            _ = sv1.CaptureMouse();
        }
        private void Error_mouseleftbuttonDown(object sender, MouseButtonEventArgs e)
        {
            if (ErrorCheck)
            {
                ErrorCheck = false;
                error_check_border.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorCheck = true;
                error_check_border.Visibility = Visibility.Collapsed;
            }
        }

        private void Error_multibuttondown(object sender, MouseButtonEventArgs e)
        {
            if (MultiErrorCheck)
            {
                //Canvas.SetTop(work, 1);
                //Canvas.SetTop(box, 20);
                MultiErrorCheck = false;
                error_multi.Visibility = Visibility.Visible;
                Canvas.SetLeft(error_check_border, 12);
            }
            else
            {
                MultiErrorCheck = true;
                error_multi.Visibility = Visibility.Collapsed;
                Canvas.SetLeft(error_check_border, 153);
            }
        }
        private void Error_multibuttondownexit(object sender, MouseButtonEventArgs e)
        {
            error_multi.Visibility = Visibility.Collapsed;
            MultiErrorCheck = true;
            error_multi.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(error_check_border, 153);
        }
        private void PreCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            string value = canvas.DataContext.ToString();
            textbox_error.Text = value;
            inputModel.errorModel.memo = value;
            MultiErrorCheck = true;
            error_multi.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(error_check_border, 153);
            ErrorCheck = true;
            error_check_border.Visibility = Visibility.Collapsed;
        }


        private void CNS_DB확인()
        {

        }



        // DB -> CNS 전송 버튼
        private void Plc_ButtonMouseDown1(object sender, MouseButtonEventArgs e)
        {
            log.Info("CNS로 데이터 전송 버튼 클릭(DB => CNS)");
            bool DB_DataOK = false; //DB에 데이터 있는거 확인

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnDataCheck = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopCNSSearch]",
               new string[] { "prodID", "MachineName" },
               new object[] { this.mainModel.prodID, SettingJsonManager.mercName }, 0);

            if (returnDataCheck.결과 == 1)
            {
                string ProdID = "";
                string machineName = "";
                int pock = 0;
                int jang = 0;
                int go = 0;

                JArray Data_Jarry = JArray.Parse(returnDataCheck.리턴값);
                foreach (JObject result in Data_Jarry)
                {
                    ProdID = result["prodID"].ToString();
                    machineName = result["MachineName"].ToString();
                    pock = Convert.ToInt32(result["Pock"]);
                    jang = Convert.ToInt32(result["Jang"]);
                    go = Convert.ToInt32(result["Go"]);
                }

                if (ProdID != "" ||  pock * jang != 0)
                {
                    MessageBoxResult result2 = System.Windows.MessageBox.Show("PC데이터를 CNS로 전송 하시겠습니까 ?", "CNS로 데이터 전송", MessageBoxButton.YesNo);

                    if (result2 == MessageBoxResult.Yes) //yes
                    {
                        DB_DataOK = true;
                    }
                    else //No
                    {
                        DB_DataOK = false;
                    }
                }
                else
                {
                    log.Error("DB에 데이터가 없어 전송할 수 없음");
                    MessageBoxResult result3 = System.Windows.MessageBox.Show(" 저장된 데이터가 없습니다.\n 먼저 CNS세팅 후 CNS데이터 저장을 눌러주세요.", "PC 데이터 없음");
                }
            }

            //DB에 데이터 있는거 확인되면
            if (DB_DataOK) 
            {
                CNS_Send = true; //데이터 쓰기

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
                        g2pRecipe = Convert.ToInt32(result["g2pRecipe"]); //G
                        oRecipe = Convert.ToInt32(result["oRecipe"]);     //O

                        p1Recipe = Convert.ToInt32(result["p1Recipe"]);   //P1
                        p2pRecipe = Convert.ToInt32(result["p2pRecipe"]); //P2
                        p3Recipe = Convert.ToInt32(result["p3Recipe"]);   //P3
                        p4Recipe = Convert.ToInt32(result["p4Recipe"]);   //P4
                        
                        dRagista = Convert.ToInt32(result["@DieCutter"]); //D레지스타
                       
                        printerPull1 = Convert.ToInt32(result["PrinterPull1"]);  //1도 롤
                        printerRoll1 = Convert.ToInt32(result["PrinterRoll1"]);  //1도 
                        printerPull2 = Convert.ToInt32(result["PrinterPull2"]);  //2도 롤
                        printerRoll2 = Convert.ToInt32(result["PrinterRoll2"]);  //2도 
                        printerPull3 = Convert.ToInt32(result["PrinterPull3"]);  //3도 롤
                        printerRoll3 = Convert.ToInt32(result["PrinterRoll3"]);  //3도 
                        printerPull4 = Convert.ToInt32(result["PrinterPull4"]);  //4도 롤
                        printerRoll4 = Convert.ToInt32(result["PrinterRoll4"]);  //4도

                        feederGap = Convert.ToInt32(result["FeederGap"]);           //피다진입 갭
                        feederRoll = Convert.ToInt32(result["FeederRoll"]);         //피다진입 롤

                        slotterSubGap = Convert.ToInt32(result["SlotterSubGap"]);        //슬로다 보조 갭
                        slotterMiddleGap = Convert.ToInt32(result["SlotterMiddleGap"]);  //슬로다 본 갭
                        slotterConnection = Convert.ToInt32(result["SlotterConnection"]);//슬로다 연결갭

                        dieCutterRollGap = Convert.ToInt32(result["DieCutterRollGap"]); //다이캇타 롤 갭
                        dieCutterConnectionGap = Convert.ToInt32(result["GluerbeltF"]); //다이캇타 연결 갭

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

                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ConnectSocket(); //CNS 통신연결

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
                                items.Add(0);                      //Spare
                                items.Add(0);                      //Spare
                                items.Add(0);                      //Spare
                                items.Add(dRagista);               //D레지스타
                                items.Add(printerPull1);           //1도 ?
                                items.Add(printerRoll1);           //1도 프레스
                                items.Add(printerPull2);           //2도 ?
                                items.Add(printerRoll2);           //2도 프레스
                                items.Add(printerPull3);           //3도 ?
                                items.Add(printerRoll3);           //3도 프레스
                                items.Add(printerPull4);           //4도 ?
                                items.Add(printerRoll4);           //4도 프레스
                                items.Add(0);                      //Spare
                                items.Add(0);                      //Spare
                                items.Add(0);                      //Spare
                                items.Add(0);                      //Spare
                                items.Add(feederGap);              //피다진입 갭
                                items.Add(feederRoll);             //피다진입 롤
                                items.Add(slotterSubGap);          //슬로다 보조 갭
                                items.Add(slotterMiddleGap);       //슬로다 본 갭
                                items.Add(slotterConnection);      //슬로다 연결갭
                                items.Add(dieCutterRollGap);       //다이캇타 롤 갭
                                items.Add(dieCutterConnectionGap); //다이캇타 연결 갭
                                items.Add(0);                      //Spare
                                items.Add(gluerbeltF1);            //글루어벨트 진입 갭 F1
                                items.Add(gluerbeltF2);            //글루어벨트 진입 갭 F2
                                items.Add(gluerPressA);            //글루어 프레스롤 A
                                items.Add(gluerPressB);            //글루어 프레스롤 B
                                items.Add(gluerPE);                //글루어 PE
                                items.Add(0);                      //Spare
                                items.Add(bRSub);                  //보조괘선 갭
                                items.Add(bRMain);                 //본괘선 갭
                                items.Add(bRConnection);           //연결롤 갭
                                items.Add(bRmiddle);               //중간롤 갭

                                int WriteCount = items.Count() * 2;
                                byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items); //연속쓰기 시작점, 데이터 개수, 값

                                //송신한 데이터 값 보는용도
                                string Send_Data = string.Empty;
                                for (int i = 0; i < bDts.Length; i++)
                                {
                                    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                                }
                                Console.WriteLine("CNS송신 데이터(input) : " + Send_Data);

                                //데이터 쓰기
                                mainSock.Send(bDts);
                            }
                        });

                        log.Info("데이터 전송 완료 (DB => CNS)");
                        MessageBoxResult result = System.Windows.MessageBox.Show("데이터 전송 완료", "CNS 데이터 전송");
                    }
                    catch (Exception)
                    {
                        log.Error("데이터 전송 실패 (DB => CNS)");
                        MessageBox_Dialog msg = new MessageBox_Dialog("데이터 전송 실패!");
                        msg.ShowDialog();
                        mainSock.Close();
                    }
                    finally
                    {
                        mainSock.Close();
                        CNS_Send = false;
                    }
                }
                else 
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("PopCNSSearch 조회 오류 !!");
                    msg.ShowDialog();
                    CNS_Send = false;
                }
            }
            else//DB데이터 없음
            {
                CNS_Send = false;
            }
        }

        //CNS 데이터 저장 버튼
        //CNS 데이터를 읽고 DB에 저장함
        private void Plc_ButtonMouseDown2(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("데이터를 PC에저장 하시겠습니까 ?", "CNS 데이터 저장", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) //확인
            {
                log.Info("CNS 데이터 저장 버튼 클릭 (CNS => DB)");

                if (SettingJsonManager.Plc) //PLC 사용 체크일때
                {
                    Console.WriteLine("CNS => DB 저장 prodID : " + this.mainModel.prodID);
                    SingleWrite(); //현재 설정을 저장하기 위해서는 D3814번에 0이 아닌 수를 넣어줘야함
                    CNSDataRead(); //CNS 데이터 읽기
                }
                else
                {
                    log.Error("CNS 데이터 저장 실패 (CNS사용 미체크)");
                    MessageBox_Dialog msg = new MessageBox_Dialog("설정에서 CNS사용을 체크해주세요.");
                }
            }
            else //취소
            {
            }
        }

        private void SingleWrite() //데이터를 읽기 위한 쓰기
        {
            CNS_Send = true;

            ConnectSocket(); //CNS 통신 연결
            Ls_Fenet wd = new Ls_Fenet(); //헤더
            byte[] bDts2 = wd.single_Write("%DW3814", 02, 2); //개별쓰기 (변수명, 데이터 타입, 쓸 데이터값) 
            mainSock.Send(bDts2); //데이터 쓰기
            Console.WriteLine("데이터를 읽기위한 데이터 전송 : " + bDts2);
            mainSock.Close();

            CNS_Send = false;
        }

        //CNS 데이터 상세창 버튼
        private void Plc_ButtonMouseDown3(object sender, MouseButtonEventArgs e)
        {
            log.Info("CNS 데이터 보기 버튼 클릭");

            Plc_Pop plc = new Plc_Pop(mainModel);
            plc.ShowDialog();
        }

        //키인 사용시 포커스가 생산수량으로 맞춰지면서 바코드 리드시 바코드값이 생산수량에 입력되는 현상을 보완
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (SettingJsonManager.keyinCheck)
            {
                if (e.Key == Key.Enter)
                {
                    tbResult.Text = "0";
                }
            }
        }
    }

    public class MinusConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? float.Parse(value.ToString()) < 0 ? "추가생산 " + (float.Parse(value.ToString()) * -1) : value : value;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
