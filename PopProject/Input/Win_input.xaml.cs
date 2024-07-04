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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.Http;
using System.Threading.Tasks;

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
        private bool startPBLock = false;
        private int inLineDone = 0;

        private DateTime selectedDate = new DateTime();


        public Win_input(MainModel mainModel, ObservableCollection<MainModel> workList, bool check) //check= 메인화면에서 들어오면 false, 디테일 화면에서 들어오면 true
        {
            InitializeComponent();
        
            this.workList = workList;
            inputModel = new InputModel();
            this.mainModel = mainModel;
            boxType.Text = "형 태 : " + mainModel.boxType;

            //현재 카운터값 송신
            if (this.mainModel.presultCount > 0)
            {
                CountManager.SendMsg(SettingJsonManager.mercName, this.mainModel.presultCount.ToString());
            }

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
                              "prodID: "   + inputModel.prodID + ", " 
                              );
        }

        //오주포장 장비별 카운터 수신(아두이노)
        private void recive(string arg1, string arg2)  //설비명, 카운터
        {
            //Console.WriteLine("오주 아두이노 값(input) : " +  arg2);
            //try
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        switch (arg1)
            //        {
            //            case "인라인3호기":
            //            case "후렉소":
            //                {
            //                    try
            //                    {
            //                        //종료버튼 활성화 일때
            //                        if (inputModel.imagePath.Equals("/img/bn_06.png"))
            //                        {
            //                            if (arg1.Equals(SettingJsonManager.mercName))
            //                            {
            //                                inputModel.pworkCount = float.Parse(arg2);
            //                            }
            //                        }
            //                    }
            //                    catch (Exception e)
            //                    {
            //                        LogManager.Error("후렉소 아두이노 값 표시 에러\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            //                    }
            //                }
            //                break;
            //        }
            //    }), DispatcherPriority.ApplicationIdle);
            //}
            //catch (Exception e)
            //{
            //    LogManager.Error("오주포장 인라인3호기 전용 카운터 SignalR 수신 오류 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            //}
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
                LogManager.Error("PopBeforeProcessItemCount 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
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
            inputModel.pworkDate = DateTime.Parse(DateTimeManager.GetSelectedDateStr()); //yyyy-MM-dd
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
                LogManager.Error("불량사유 리스트 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
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
                LogManager.Error("생산결과 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);

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

                    DateTime parsedDate;

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
                LogManager.Error("불량리스트 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
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

            if (e.ClickCount == 1) //더블클릭
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
                DateTimeManager.GetSelectedDateStr(),
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
                //Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                SettingJsonManager.SaveJsonFile();
                mainModel.sDatetime = DateTimeManager.GetSelectedStr(); //yyyy-MM-dd HH:mm:ss
                if (!SettingJsonManager.corp_name.Equals("오주포장") && !SettingJsonManager.corp_name.Equals("오주포장테스트"))
                {
                    CountManager.SendMsg(SettingJsonManager.mercName, "S");
                    CountManager.SendMsg(SettingJsonManager.mercName, "0");
                }
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
                LogManager.Error("불량리스트 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
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
                DateTimeManager.GetSelectedDateStr(),
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
            LogManager.Info("인풋창 닫기", MethodBase.GetCurrentMethod().DeclaringType);
        }

        //취소 버튼 클릭
        private void Win_input_cancel(object sender, MouseButtonEventArgs e)
        {
            if (!SettingJsonManager.manager.Equals("1"))
            {
                //종료버튼 활성화 일때
                if (inputModel.imagePath.Equals("/img/bn_06.png"))
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("작업을 취소하면 결과가 저장되지 않습니다." + Environment.NewLine + "정말로 취소 하시겠습니까?", "작업 취소", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        //Close();
                    }
                    else
                    {
                        PopResultEnd("cancel"); //결과값 삽입

                        inputModel.perrCount = 0;
                        inputModel.count = 0;
                        inputModel.pMemo = "";
                        inputModel.pworkCount = 0;
                        PopResultSearch();

                        LogManager.Info(inputModel.customSName + " / " + inputModel.prodName + " [생산취소] " + inputModel.sumPworkCount + "개", MethodBase.GetCurrentMethod().DeclaringType);

                        if (SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트"))
                        {
                            Close();
                        }
                    }
                }
                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("작업 중일 때만 취소할 수 있습니다.", "취소 불가", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("관리자 로그인 시 조작할 수 없습니다.");
                _ = box.ShowDialog();
            }
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
            //관리자 로그인이 아닐때
            if (!SettingJsonManager.manager.Equals("1"))
            {
                //시작 버튼 활성화 일때
                if (inputModel.imagePath.Equals("/img/bn_05.png") && startPBLock.Equals(false))
                {
                    startPBLock = true;

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
                                LogManager.Info(Main.production.customSName + " / " + Main.production.prodName + " [생산시작]", MethodBase.GetCurrentMethod().DeclaringType);
                            }

                            break;

                        //일반
                        default:
                            if (Main.production != null)
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
                                LogManager.Info(Main.production.customSName + " / " + Main.production.prodName + " [생산시작]", MethodBase.GetCurrentMethod().DeclaringType);
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
                        startPBLock = false;
                        inLineDone = 0;

                        PopResultEnd("end"); //결과값 삽입

                        //불량수량 자동 입력
                        if (tbError.Text != "0" && tbError.Text != "" && textbox_error.Text != "")
                        {
                            if (inputModel.pworkKey != 0)
                            {
                                if (popErrorInsert() == 1)
                                {
                                    tbError.Text = "0";
                                }
                                else
                                {
                                    MessageBox_Dialog msg = new MessageBox_Dialog("불량 데이터 저장 오류 !!");
                                    msg.ShowDialog();
                                }
                            }
                        }

                        inputModel.perrCount = 0;
                        inputModel.count = 0;
                        inputModel.pMemo = "";
                        inputModel.pworkCount = 0;
                        PopResultSearch();

                        LogManager.Info(inputModel.customSName + " / " + inputModel.prodName + " [생산종료] " + inputModel.sumPworkCount + "개", MethodBase.GetCurrentMethod().DeclaringType);

                        if (SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트"))
                        {
                            Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("관리자 로그인 시 조작할 수 없습니다.");
                _ = box.ShowDialog();
            }
        }


        bool cnsProcessing = false;

        private void CNSDataRead() //데이터 수신 요청
        {
            try
            {
                ConnectSocket(); //CNS 통신 연결
                Ls_Fenet wd = new Ls_Fenet(); //헤더

                byte[] bDts = wd.Word_continuous_Read("%DB7804", 74); //연속읽기 D3902 ~ D3975 (74개)

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
                    LogManager.Error("CNS PLC 연결 실패\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
                        mainSock.Close();
                    MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                    msg.ShowDialog();
                }
            }
        }

        //CNS데이터 DB에 저장
        public void DataReceived(IAsyncResult ar) // 데이터 수신
        {
            Resipe_Model2 itemModel = new Resipe_Model2();
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

                for (int i = 0; i < RD.Length / 4; i++)
                {
                    //string high_data = RD.Substring(0, 2); //앞 두자리 추출
                    //string low_data = RD.Substring(2, 2); //뒤 두자리 추출

                    string data = RD.Substring(i * 4, 4);

                    switch (i)
                    {
                        case 0: //작업모델 번호
                            itemModel.ModelNum = ValueConvert(data, false); break; //true: 소숫점으로 저장, false: 자연수 저장
                        case 1: //작업지시 매수
                            itemModel.WorkPlan1 = ValueConvert(data, false); break;
                        case 2: //작업지시 매수
                            itemModel.WorkPlan2 = ValueConvert(data, false); break;
                        case 3: //박스타입
                            itemModel.BoxType = ValueConvert(data, false); break;
                        case 4: //폭
                            itemModel.Pock = ValueConvert(data, false); break;
                        case 5: //장
                            itemModel.Jang = ValueConvert(data, false); break;
                        case 6: //고
                            itemModel.Go = ValueConvert(data, false); break;
                        case 7: //갭
                            itemModel.Gap = ValueConvert(data, false); break;
                        case 8: //앞날개
                            itemModel.Wing = ValueConvert(data, false); break;
                        case 9: //미미
                            itemModel.Mimi = ValueConvert(data, false); break;
                        case 10: //원단
                            itemModel.WonDan = ValueConvert(data, false); break;
                        case 11: //
                            itemModel.WorkSpeed = ValueConvert(data, false); break;
                        case 12: //                    
                            itemModel.SaveState = ValueConvert(data, false); break;
                        case 13: //           
                            itemModel.Spare1 = ValueConvert(data, false); break;
                        case 14: //
                            itemModel.Recipe1 = ValueConvert(data, false); break;
                        case 15: //
                            itemModel.Recipe2 = ValueConvert(data, false); break;
                        case 16: //                               
                            itemModel.Recipe3 = ValueConvert(data, false); break;
                        case 17: //                         
                            itemModel.Recipe4 = ValueConvert(data, false); break;
                        case 18: //                               
                            itemModel.Recipe5 = ValueConvert(data, false); break;
                        case 19: //                               
                            itemModel.Recipe6 = ValueConvert(data, false); break;
                        case 20: //                            
                            itemModel.Recipe7 = ValueConvert(data, false); break;
                        case 21: //
                            itemModel.Recipe8 = ValueConvert(data, false); break;
                        case 22: //
                            itemModel.Recipe9 = ValueConvert(data, false); break;
                        case 23: //
                            itemModel.Recipe10 = ValueConvert(data, false); break;
                        case 24: //
                            itemModel.Recipe11 = ValueConvert(data, false); break;
                        case 25: //
                            itemModel.Recipe12 = ValueConvert(data, false); break;
                        case 26: //
                            itemModel.Recipe13 = ValueConvert(data, false); break;
                        case 27: //
                            itemModel.Recipe14 = ValueConvert(data, false); break;
                        case 28: //
                            itemModel.Recipe15 = ValueConvert(data, false); break;
                        case 29: //
                            itemModel.Recipe16 = ValueConvert(data, false); break;
                        case 30: //
                            itemModel.Recipe17 = ValueConvert(data, false); break;
                        case 31: //
                            itemModel.Recipe18 = ValueConvert(data, false); break;
                        case 32: //
                            itemModel.Recipe19 = ValueConvert(data, false); break;
                        case 33: //
                            itemModel.Recipe20 = ValueConvert(data, false); break;
                        case 34: //
                            itemModel.Recipe21 = ValueConvert(data, false); break;
                        case 35: //
                            itemModel.Recipe22 = ValueConvert(data, false); break;
                        case 36: //
                            itemModel.Recipe23 = ValueConvert(data, false); break;
                        case 37: //
                            itemModel.Recipe24 = ValueConvert(data, false); break;
                        case 38: //
                            itemModel.Recipe25 = ValueConvert(data, false); break;
                        case 39: //
                            itemModel.Recipe26 = ValueConvert(data, false); break;
                        case 40: //
                            itemModel.Recipe27 = ValueConvert(data, false); break;
                        case 41: //
                            itemModel.Recipe28 = ValueConvert(data, false); break;
                        case 42: //
                            itemModel.Recipe29 = ValueConvert(data, false); break;
                        case 43: //
                            itemModel.Recipe30 = ValueConvert(data, false); break;
                        case 44: //
                            itemModel.Recipe31 = ValueConvert(data, false); break;
                        case 45: //
                            itemModel.Recipe32 = ValueConvert(data, false); break;
                        case 46: //
                            itemModel.Recipe33 = ValueConvert(data, false); break;
                        case 47: //
                            itemModel.Recipe34 = ValueConvert(data, false); break;
                        case 48: //
                            itemModel.Recipe35 = ValueConvert(data, false); break;
                        case 49: //
                            itemModel.Recipe36 = ValueConvert(data, false); break;
                        case 50: //
                            itemModel.Recipe37 = ValueConvert(data, false); break;
                        case 51: //
                            itemModel.Recipe38 = ValueConvert(data, false); break;
                        case 52: //
                            itemModel.Recipe39 = ValueConvert(data, false); break;
                        case 53: //
                            itemModel.Recipe40 = ValueConvert(data, false); break;
                        case 54: //
                            itemModel.Recipe41 = ValueConvert(data, false); break;
                        case 55: //
                            itemModel.Recipe42 = ValueConvert(data, false); break;
                        case 56: //
                            itemModel.Recipe43 = ValueConvert(data, false); break;
                        case 57: //    
                            itemModel.Recipe44 = ValueConvert(data, false); break;
                        case 58: //    
                            itemModel.Recipe45 = ValueConvert(data, false); break;
                        case 59: //
                            itemModel.Recipe46 = ValueConvert(data, false); break;
                        case 60: //
                            itemModel.Recipe47 = ValueConvert(data, false); break;
                        case 61: //
                            itemModel.Recipe48 = ValueConvert(data, false); break;
                        case 62: //
                            itemModel.Recipe49 = ValueConvert(data, false); break;
                        case 63: //
                            itemModel.Recipe50 = ValueConvert(data, false); break;
                        case 64: //                           
                            itemModel.Recipe51 = ValueConvert(data, false); break;
                        case 65: //                          
                            itemModel.Recipe52 = ValueConvert(data, false); break;
                        case 66: //                        
                            itemModel.Recipe53 = ValueConvert(data, false); break;
                        case 67: //                          
                            itemModel.Recipe54 = ValueConvert(data, false); break;
                        case 68: // 
                            itemModel.Recipe55 = ValueConvert(data, false); break;
                        case 69: //                   
                            itemModel.Recipe56 = ValueConvert(data, false); break;
                        case 70: //    
                            itemModel.Recipe57 = ValueConvert(data, false); break;
                        case 71: //             
                            itemModel.Recipe58 = ValueConvert(data, false); break;
                        case 72: //             
                            itemModel.Recipe59 = ValueConvert(data, false); break;
                        case 73: //             
                            itemModel.Recipe60 = ValueConvert(data, false); break;

                        default:
                            break;
                    }
                    //continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                    //RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                }

                //Console.WriteLine("CNS수신 데이터(input) : " + continuous_read); //수신값 확인용


                if (cnsProcessing)
                {
                    ReturnModel returnData = ServiceModel.GetData(
                        SettingJsonManager.serverIndex,
                        "[PopCNSUpdate2]",
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
                            "@Gap",
                            "@Wing",
                            "@Mimi",
                            "@WonDan",
                            "@WorkSpeed",
                            "@SaveState",
                            "@Spare1",
                            "@Recipe1",
                            "@Recipe2",
                            "@Recipe3",
                            "@Recipe4",
                            "@Recipe5",
                            "@Recipe6",
                            "@Recipe7",
                            "@Recipe8",
                            "@Recipe9",
                            "@Recipe10",
                            "@Recipe11",
                            "@Recipe12",
                            "@Recipe13",
                            "@Recipe14",
                            "@Recipe15",
                            "@Recipe16",
                            "@Recipe17",
                            "@Recipe18",
                            "@Recipe19",
                            "@Recipe20",
                            "@Recipe21",
                            "@Recipe22",
                            "@Recipe23",
                            "@Recipe24",
                            "@Recipe25",
                            "@Recipe26",
                            "@Recipe27",
                            "@Recipe28",
                            "@Recipe29",
                            "@Recipe30",
                            "@Recipe31",
                            "@Recipe32",
                            "@Recipe33",
                            "@Recipe34",
                            "@Recipe35",
                            "@Recipe36",
                            "@Recipe37",
                            "@Recipe38",
                            "@Recipe39",
                            "@Recipe40",
                            "@Recipe41",
                            "@Recipe42",
                            "@Recipe43",
                            "@Recipe44",
                            "@Recipe45",
                            "@Recipe46",
                            "@Recipe47",
                            "@Recipe48",
                            "@Recipe49",
                            "@Recipe50",
                            "@Recipe51",
                            "@Recipe52",
                            "@Recipe53",
                            "@Recipe54",
                            "@Recipe55",
                            "@Recipe56",
                            "@Recipe57",
                            "@Recipe58",
                            "@Recipe59",
                            "@Recipe60",
                            "@UpDatetime"},
                        new object[] { this.mainModel.prodID, SettingJsonManager.mercName, 
                                       itemModel.ModelNum, itemModel.WorkPlan1, itemModel.WorkPlan2, itemModel.BoxType, itemModel.Pock, itemModel.Jang, itemModel.Go, itemModel.Gap, itemModel.Wing, itemModel.Mimi, itemModel.WonDan, itemModel.WorkSpeed,itemModel.SaveState, itemModel.Spare1,
                                       itemModel.Recipe1, itemModel.Recipe2, itemModel.Recipe3, itemModel.Recipe4, itemModel.Recipe5, itemModel.Recipe6, itemModel.Recipe7, itemModel.Recipe8, itemModel.Recipe9, itemModel.Recipe10,
                                       itemModel.Recipe11,itemModel.Recipe12,itemModel.Recipe13,itemModel.Recipe14,itemModel.Recipe15,itemModel.Recipe16,itemModel.Recipe17,itemModel.Recipe18,itemModel.Recipe19,itemModel.Recipe20,
                                       itemModel.Recipe21,itemModel.Recipe22,itemModel.Recipe23,itemModel.Recipe24,itemModel.Recipe25,itemModel.Recipe26,itemModel.Recipe27,itemModel.Recipe28,itemModel.Recipe29,itemModel.Recipe30,
                                       itemModel.Recipe31,itemModel.Recipe32,itemModel.Recipe33,itemModel.Recipe34,itemModel.Recipe35,itemModel.Recipe36,itemModel.Recipe37,itemModel.Recipe38,itemModel.Recipe39,itemModel.Recipe40,
                                       itemModel.Recipe41,itemModel.Recipe42,itemModel.Recipe43,itemModel.Recipe44,itemModel.Recipe45,itemModel.Recipe46,itemModel.Recipe47,itemModel.Recipe48,itemModel.Recipe49,itemModel.Recipe50,
                                       itemModel.Recipe51,itemModel.Recipe52,itemModel.Recipe53,itemModel.Recipe54,itemModel.Recipe55,itemModel.Recipe56,itemModel.Recipe57,itemModel.Recipe58,itemModel.Recipe59,itemModel.Recipe60,
                                       DateTimeManager.GetSelectedStr()
                        },
                        1
                        );

                    if (returnData.결과 == 1)
                    {
                        checkInput = true;
                        MessageBoxResult result = System.Windows.MessageBox.Show("저장 완료.", "CNS 데이터 저장");
                    }
                    else
                    {
                        MessageBoxResult resultf = System.Windows.MessageBox.Show("저장 실패.", "CNS 데이터 저장");
                    }
                    cnsProcessing = false;
                    
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

        //생산 종료
        private int PopResultEnd(string request)
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
            if (request == "end")
            {
                inputModel.pworkCount = int.Parse(tbResult.Text.ToString());
            }
            else
            {
                inputModel.pworkCount = 0;
            }

            //생산,불량수량 0이면 3| 결과수량 있으면 1|
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

            if (SettingJsonManager.corp_name.Contains("테스트") || SettingJsonManager.corp_name.Contains("금"))
            {
                Console.WriteLine("---------------------------------------------------------------------------" + Environment.NewLine +
                                  "일반 생산 종료 [PopResultEnd] : " + Environment.NewLine +
                                  "AllChkAED:" + "1" + Environment.NewLine +
                                  ", 작업일자:" + DateTimeManager.GetSelectedDateStr() + Environment.NewLine +
                                  ", 오더Key:" + inputModel.orderKey + Environment.NewLine +
                                  ", arrChkAED:" + arrChkAED + Environment.NewLine +
                                  ", arr공정구분:" + inputModel.ProcessType + Environment.NewLine +
                                  ", arr계획Key:" + inputModel.pplanKey + Environment.NewLine +
                                  ", arr작업Key:" + inputModel.pworkKey + Environment.NewLine +
                                  ", arr작업량:" + Math.Ceiling(inputModel.pworkCount) + Environment.NewLine +
                                  ", arr불량:" + inputModel.perrCount + Environment.NewLine +
                                  ". arr비고:" + inputModel.pMemo + Environment.NewLine +
                                  ", 시작시간:" + inputModel.sDatetime + Environment.NewLine +
                                  ", 설비명:" + SettingJsonManager.mercName + Environment.NewLine +
                                  ", 전체공정" + 전체공정 + Environment.NewLine +
                                  "---------------------------------------------------------------------------");
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
                DateTimeManager.GetSelectedDateStr(),
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
                Console.WriteLine();


                Canvas.SetTop(box, 490); //세부내역 버튼
                Canvas.SetTop(box1, 570); //이월 버튼

                //생산수량 있을때만 이월버튼 보이도록 함
                count = inputModel.pworkCount;
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
                inputModel.imagePath = "/img/bn_05.png"; //input창 시작버튼으로 변경
                mainModel.imagePath = "/img/bn_02.png"; //main창 대기버튼으로 변경
                mainModel.sumPworkCount = inputModel.pworkCount + inputModel.perrCount;
                mainModel.presultCount = inputModel.pworkCount + inputModel.perrCount;
                mainModel.presultCount1 = inputModel.pworkCount + inputModel.perrCount;
                mainModel.perrorCount = (int)inputModel.perrCount;
                
                //공정 상태 검정색으로 변경
                for (int i = 0; i < mainModel.subOb.Count; i++)
                {
                    if (SettingJsonManager.processType == mainModel.subOb[i].processType)
                    {
                        mainModel.subOb[i].stateColor = Brushes.Black;
                        mainModel.subOb[i].stateFontColor = Brushes.White;
                    }
                }

                CountManager.SendMsg(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
                CountManager.SendMsg(SettingJsonManager.mercName, "0"); //

                //오주포장 인라인기 온제품 처리 (철접합(7),베가(8),인라인2,3호기(13))
                if (SettingJsonManager.corp_name.Equals("오주포장") && (SettingJsonManager.processType == 7 || SettingJsonManager.processType == 8 || SettingJsonManager.processType == 13) ||
                    SettingJsonManager.corp_name.Equals("오주포장테스트") && (SettingJsonManager.processType == 7 || SettingJsonManager.processType == 8 || SettingJsonManager.processType == 13))
                {
                    for (int i = 0; i < mainModel.subOb.Count; i++)
                    {
                        if (mainModel.subOb[i].ProcessTypeName.Equals("인쇄"))
                        {
                            if (mainModel.subOb[i].presultCount < 1)
                            {
                                Console.WriteLine(mainModel.subOb[i].stateColor); 
                                PopInlineStart(mainModel.subOb[i]);
                                Console.WriteLine(mainModel.subOb[i].stateColor); 
                            }
                        }
                    }
                }

                //금화 인라인기 완제품 처리
                if (SettingJsonManager.corp_name.Equals("금화")  && SettingJsonManager.mercName.Equals("인라인") && SettingJsonManager.processType == 1 ||
                    SettingJsonManager.corp_name.Equals("금화테스트") && SettingJsonManager.mercName.Equals("인라인") && SettingJsonManager.processType == 1)
                {
                    for (int i = 0; i < mainModel.subOb.Count; i++)
                    {
                        if (mainModel.subOb[i].ProcessTypeName.Equals("접합") || mainModel.subOb[i].ProcessTypeName.Equals("톰슨"))
                        {
                            if (mainModel.subOb[i].presultCount < 1)
                            {
                                PopInlineStart(mainModel.subOb[i]);
                            }
                        }
                    }
                }

                //성주포장 인라인기(후렉소) 완제품 처리
                if (SettingJsonManager.corp_name.Equals("성주포장") && SettingJsonManager.mercName.Equals("후렉소") && (SettingJsonManager.processType == 1) || 
                    SettingJsonManager.corp_name.Equals("성주포장테스트") && SettingJsonManager.mercName.Equals("후렉소") && (SettingJsonManager.processType == 1))
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
                        catch (Exception ex)
                        {
                            LogManager.Error("아두이노 리셋 에러\n" + ex.Message, MethodBase.GetCurrentMethod().DeclaringType);
                        }
                        break;

                    default:
                        Main.production = null;
                        try
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                            Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                        }

                        catch (Exception ex)
                        {
                            LogManager.Error("아두이노 리셋 에러\n" + ex.Message, MethodBase.GetCurrentMethod().DeclaringType);
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
                DateTimeManager.GetSelectedDateStr(),
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
                mainModel.sDatetime = DateTimeManager.GetSelectedStr();
                CountManager.SendMsg(SettingJsonManager.mercName, "S");//현황판으로 시작신호 송신 (설비명,S)
                CountManager.SendMsg(SettingJsonManager.mercName, "0"); //현황판으로 생산종료 송신
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
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;
        }

        //불량수량 적용 버튼
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
                    else
                    {
                        MessageBox_Dialog msg = new MessageBox_Dialog(" 생산결과 건이 있거나 \n 시작버튼을 누른 상태에서 저장해주세요.");
                        msg.ShowDialog();
                    }
                }
                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("불량 수량과 불량사유를 확인해주세요.");
                    msg.ShowDialog();
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
            }
        }

        private void Result_badCount_plus(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            InputModel context = (InputModel)img.DataContext;
            int value = 1;
            context.errorModel.perrCount += value;
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
                new object[] {  inputModel.orderKey, inputModel.pplanKey },
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
            LogManager.Info("CNS로 데이터 전송 버튼 클릭(DB => CNS)", MethodBase.GetCurrentMethod().DeclaringType);
            bool DB_to_CNS = false; 

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnDataCheck = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopCNSSearch2]",
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
                        DB_to_CNS = true;
                    }
                    else //No
                    {
                        DB_to_CNS = false;
                    }
                }
                else
                {
                    LogManager.Error("DB에 데이터가 없어 전송할 수 없음", MethodBase.GetCurrentMethod().DeclaringType);
                    MessageBoxResult result3 = System.Windows.MessageBox.Show(" 저장된 데이터가 없습니다.\n 먼저 CNS세팅 후 CNS데이터 저장을 눌러주세요.", "PC 데이터 없음");
                }
            }

            //DB => CNS 데이터 전송 확인클릭
            if (DB_to_CNS) 
            {
                CNS_Send = true; //데이터 쓰기

                ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                ReturnModel returnData = ServiceModel.GetData(
                   SettingJsonManager.serverIndex,
                   "[PopCNSSearch2]",
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
                    int mimi = 0;
                    int wonDan = 0;
                    int workSpeed = 0;
                    int saveState = 0;
                    int spare1 = 0;
                    int recipe1  = 0;
                    int recipe2  = 0;
                    int recipe3  = 0;
                    int recipe4  = 0;
                    int recipe5  = 0;
                    int recipe6  = 0;
                    int recipe7  = 0;
                    int recipe8  = 0;
                    int recipe9  = 0;
                    int recipe10 = 0;
                    int recipe11 = 0;
                    int recipe12 = 0;
                    int recipe13 = 0;
                    int recipe14 = 0;
                    int recipe15 = 0;
                    int recipe16 = 0;
                    int recipe17 = 0;
                    int recipe18 = 0;
                    int recipe19 = 0;
                    int recipe20 = 0;
                    int recipe21 = 0;
                    int recipe22 = 0;
                    int recipe23 = 0;
                    int recipe24 = 0;
                    int recipe25 = 0;
                    int recipe26 = 0;
                    int recipe27 = 0;
                    int recipe28 = 0;
                    int recipe29 = 0;
                    int recipe30 = 0;
                    int recipe31 = 0;
                    int recipe32 = 0;
                    int recipe33 = 0;
                    int recipe34 = 0;
                    int recipe35 = 0;
                    int recipe36 = 0;
                    int recipe37 = 0;
                    int recipe38 = 0;
                    int recipe39 = 0;
                    int recipe40 = 0;
                    int recipe41 = 0;
                    int recipe42 = 0;
                    int recipe43 = 0;
                    int recipe44 = 0;
                    int recipe45 = 0;
                    int recipe46 = 0;
                    int recipe47 = 0;
                    int recipe48 = 0;
                    int recipe49 = 0;
                    int recipe50 = 0;
                    int recipe51 = 0;
                    int recipe52 = 0;
                    int recipe53 = 0;
                    int recipe54 = 0;
                    int recipe55 = 0;
                    int recipe56 = 0;
                    int recipe57 = 0;
                    int recipe58 = 0;
                    int recipe59 = 0;
                    int recipe60 = 0;

                    JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                    foreach (JObject result in Data_Jarry)
                    {
                        ProdID = result["prodID"].ToString();
                        machineName = result["MachineName"].ToString();
                        modelNum = Convert.ToInt32(result["ModelNum"]);//D3802
                        workPlan1 = Convert.ToInt32(result["WorkPlan1"]);//D3803
                        workPlan2 = Convert.ToInt32(result["WorkPlan2"]);//D3804
                        boxType = Convert.ToInt32(result["BoxType"]);//D3805
                        jang = Convert.ToInt32(result["Jang"]);//D3806
                        pock = Convert.ToInt32(result["Pock"]);//D3807
                        go = Convert.ToInt32(result["Go"]);//D3808
                        gap = Convert.ToInt32(result["Gap"]);//D3809
                        wing = Convert.ToInt32(result["Wing"]);//D3810
                        mimi = Convert.ToInt32(result["Mimi"]);//D3811
                        wonDan = Convert.ToInt32(result["WonDan"]);//D3812
                        workSpeed = Convert.ToInt32(result["WorkSpeed"]);//D3813
                        saveState = Convert.ToInt32(result["SaveState"]);//D3814
                        spare1 = Convert.ToInt32(result["Spare1"]);//D3815

                        //D3816~
                        recipe1  = Convert.ToInt32(Convert.ToSingle(result["Recipe1"])); //A
                        recipe2  = Convert.ToInt32(Convert.ToSingle(result["Recipe2"])); //B
                        recipe3  = Convert.ToInt32(Convert.ToSingle(result["Recipe3"])); //C
                        recipe4  = Convert.ToInt32(Convert.ToSingle(result["Recipe4"])); //D
                        recipe5  = Convert.ToInt32(Convert.ToSingle(result["Recipe5"])); //E
                        recipe6  = Convert.ToInt32(Convert.ToSingle(result["Recipe6"])); //F1
                        recipe7  = Convert.ToInt32(Convert.ToSingle(result["Recipe7"])); //F2
                        recipe8  = Convert.ToInt32(Convert.ToSingle(result["Recipe8"])); //G
                        recipe9  = Convert.ToInt32(Convert.ToSingle(result["Recipe9"])); //O
                        recipe10 = Convert.ToInt32(Convert.ToSingle(result["Recipe10"])); //P1
                        recipe11 = Convert.ToInt32(Convert.ToSingle(result["Recipe11"])); //P1
                        recipe12 = Convert.ToInt32(Convert.ToSingle(result["Recipe12"])); //P2
                        recipe13 = Convert.ToInt32(Convert.ToSingle(result["Recipe13"])); //P3
                        recipe14 = Convert.ToInt32(Convert.ToSingle(result["Recipe14"])); //P4
                        recipe15 = Convert.ToInt32(Convert.ToSingle(result["Recipe15"])); //
                        recipe16 = Convert.ToInt32(Convert.ToSingle(result["Recipe16"])); //
                        recipe17 = Convert.ToInt32(Convert.ToSingle(result["Recipe17"])); //
                        recipe18 = Convert.ToInt32(Convert.ToSingle(result["Recipe18"])); //
                        recipe19 = Convert.ToInt32(Convert.ToSingle(result["Recipe19"])); //
                        recipe20 = Convert.ToInt32(Convert.ToSingle(result["Recipe20"])); //
                        recipe21 = Convert.ToInt32(Convert.ToSingle(result["Recipe21"])); //
                        recipe22 = Convert.ToInt32(Convert.ToSingle(result["Recipe22"])); //
                        recipe23 = Convert.ToInt32(Convert.ToSingle(result["Recipe23"])); //
                        recipe24 = Convert.ToInt32(Convert.ToSingle(result["Recipe24"])); //
                        recipe25 = Convert.ToInt32(Convert.ToSingle(result["Recipe25"])); //
                        recipe26 = Convert.ToInt32(Convert.ToSingle(result["Recipe26"])); //
                        recipe27 = Convert.ToInt32(Convert.ToSingle(result["Recipe27"])); //
                        recipe28 = Convert.ToInt32(Convert.ToSingle(result["Recipe28"])); //
                        recipe29 = Convert.ToInt32(Convert.ToSingle(result["Recipe29"])); //
                        recipe30 = Convert.ToInt32(Convert.ToSingle(result["Recipe30"])); //
                        recipe31 = Convert.ToInt32(Convert.ToSingle(result["Recipe31"])); //
                        recipe32 = Convert.ToInt32(Convert.ToSingle(result["Recipe32"])); //
                        recipe33 = Convert.ToInt32(Convert.ToSingle(result["Recipe33"])); //
                        recipe34 = Convert.ToInt32(Convert.ToSingle(result["Recipe34"])); //
                        recipe35 = Convert.ToInt32(Convert.ToSingle(result["Recipe35"])); //
                        recipe36 = Convert.ToInt32(Convert.ToSingle(result["Recipe36"])); //
                        recipe37 = Convert.ToInt32(Convert.ToSingle(result["Recipe37"])); //
                        recipe38 = Convert.ToInt32(Convert.ToSingle(result["Recipe38"])); //
                        recipe39 = Convert.ToInt32(Convert.ToSingle(result["Recipe39"])); //
                        recipe40 = Convert.ToInt32(Convert.ToSingle(result["Recipe40"])); //
                        recipe41 = Convert.ToInt32(Convert.ToSingle(result["Recipe41"])); //
                        recipe42 = Convert.ToInt32(Convert.ToSingle(result["Recipe42"])); //
                        recipe43 = Convert.ToInt32(Convert.ToSingle(result["Recipe43"])); //
                        recipe44 = Convert.ToInt32(Convert.ToSingle(result["Recipe44"])); //
                        recipe45 = Convert.ToInt32(Convert.ToSingle(result["Recipe45"])); //
                        recipe46 = Convert.ToInt32(Convert.ToSingle(result["Recipe46"])); //
                        recipe47 = Convert.ToInt32(Convert.ToSingle(result["Recipe47"])); //
                        recipe48 = Convert.ToInt32(Convert.ToSingle(result["Recipe48"])); //
                        recipe49 = Convert.ToInt32(Convert.ToSingle(result["Recipe49"])); //
                        recipe50 = Convert.ToInt32(Convert.ToSingle(result["Recipe50"])); //
                        recipe51 = Convert.ToInt32(Convert.ToSingle(result["Recipe51"])); //
                        recipe52 = Convert.ToInt32(Convert.ToSingle(result["Recipe52"])); //
                        recipe53 = Convert.ToInt32(Convert.ToSingle(result["Recipe53"])); //
                        recipe54 = Convert.ToInt32(Convert.ToSingle(result["Recipe54"])); //
                        recipe55 = Convert.ToInt32(Convert.ToSingle(result["Recipe55"])); //
                        recipe56 = Convert.ToInt32(Convert.ToSingle(result["Recipe56"])); //
                        recipe57 = Convert.ToInt32(Convert.ToSingle(result["Recipe57"])); //
                        recipe58 = Convert.ToInt32(Convert.ToSingle(result["Recipe58"])); //
                        recipe59 = Convert.ToInt32(Convert.ToSingle(result["Recipe59"])); //
                        recipe60 = Convert.ToInt32(Convert.ToSingle(result["Recipe60"])); //
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

                                items.Add(99);      //D3800 설정 변경요구 (99로 요청하면 1로바뀜)
                                items.Add(1);       //D3801 지령모드 (0=> 신규설정요구, 1=>데이터 설정 변경요구
                                items.Add(1);       //D3802 작업 모델 번호 
                                items.Add(10000);   //D3803 작업지시 매수(하위)
                                items.Add(0);       //D3804 작업지시 매수(상위)
                                items.Add(boxType); //D3805 BOX TYPE 
                                items.Add(pock);    //D3806 폭
                                items.Add(jang);    //D3807 장
                                items.Add(go);      //D3808 고
                                items.Add(gap);     //D3809 갭
                                items.Add(wing);    //D3810 앞날개
                                items.Add(mimi);    //D3811 미미
                                items.Add(wonDan);  //D3812 원단
                                items.Add(0);       //D3813 
                                items.Add(0);       //D3814 
                                items.Add(0);       //D3815 

                                items.Add(ValueConvert2(recipe1));//D3816
                                items.Add(ValueConvert2(recipe2));//D3817 
                                items.Add(ValueConvert2(recipe3));//D3818 
                                items.Add(ValueConvert2(recipe4));//D3819 
                                items.Add(ValueConvert2(recipe5));//D3820 
                                items.Add(ValueConvert2(recipe6));//D3821 
                                items.Add(ValueConvert2(recipe7));//D3822 
                                items.Add(ValueConvert2(recipe8));//D3823 
                                items.Add(ValueConvert2(recipe9));//D3824 
                                items.Add(ValueConvert2(recipe10));//D3825
                                items.Add(ValueConvert2(recipe11));//D3826
                                items.Add(ValueConvert2(recipe12));//D3827
                                items.Add(ValueConvert2(recipe13));//D3828
                                items.Add(ValueConvert2(recipe14));//D3829 2호기:뒷날개
                                items.Add(ValueConvert2(recipe15));//D3830
                                items.Add(ValueConvert2(recipe16));//D3831
                                items.Add(ValueConvert2(recipe17));//D3832
                                items.Add(ValueConvert2(recipe18));//D3833
                                items.Add(ValueConvert2(recipe19));//D3834
                                items.Add(ValueConvert2(recipe20));//D3835
                                items.Add(ValueConvert2(recipe21));//D3836
                                items.Add(ValueConvert2(recipe22));//D3837
                                items.Add(ValueConvert2(recipe23));//D3838
                                items.Add(ValueConvert2(recipe24));//D3839
                                items.Add(ValueConvert2(recipe25));//D3840
                                items.Add(ValueConvert2(recipe26));//D3841
                                items.Add(ValueConvert2(recipe27));//D3842
                                items.Add(ValueConvert2(recipe28));//D3843
                                items.Add(ValueConvert2(recipe29));//D3844
                                items.Add(ValueConvert2(recipe30));//D3845
                                items.Add(ValueConvert2(recipe31));//D3846
                                items.Add(ValueConvert2(recipe32));//D3847
                                items.Add(ValueConvert2(recipe33));//D3848
                                items.Add(ValueConvert2(recipe34));//D3849
                                items.Add(ValueConvert2(recipe35));//D3850
                                items.Add(ValueConvert2(recipe36));//D3851
                                items.Add(ValueConvert2(recipe37));//D3852
                                items.Add(ValueConvert2(recipe38));//D3853
                                items.Add(ValueConvert2(recipe39));//D3854
                                items.Add(ValueConvert2(recipe40));//D3855
                                items.Add(ValueConvert2(recipe41));//D3856
                                items.Add(ValueConvert2(recipe42));//D3857
                                items.Add(ValueConvert2(recipe43));//D3858
                                items.Add(ValueConvert2(recipe44));//D3859
                                items.Add(ValueConvert2(recipe45));//D3860
                                items.Add(ValueConvert2(recipe46));//D3861
                                items.Add(ValueConvert2(recipe47));//D3862
                                items.Add(ValueConvert2(recipe48));//D3863
                                items.Add(ValueConvert2(recipe49));//D3864
                                items.Add(ValueConvert2(recipe50));//D3865
                                items.Add(ValueConvert2(recipe51));//D3866
                                items.Add(ValueConvert2(recipe52));//D3867
                                items.Add(ValueConvert2(recipe53));//D3868
                                items.Add(ValueConvert2(recipe54));//D3869
                                items.Add(ValueConvert2(recipe55));//D3870
                                items.Add(ValueConvert2(recipe56));//D3871
                                items.Add(ValueConvert2(recipe57));//D3872
                                items.Add(ValueConvert2(recipe58));//D3873
                                items.Add(ValueConvert2(recipe59));//D3874
                                items.Add(ValueConvert2(recipe60));//D3875

                                int WriteCount = items.Count() * 2;
                                byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items); //연속쓰기 시작점, 데이터 개수, 값

                                //송신한 데이터 값 보는용도
                                string Send_Data = string.Empty;
                                for (int i = 0; i < bDts.Length; i++)
                                {
                                    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                                }

                                Console.WriteLine("CNS송신 데이터(input) : " + Send_Data);

                                Send_Data.Substring(76);

                                //데이터 쓰기
                                mainSock.Send(bDts);
                            }
                        });

                        LogManager.Info("데이터 전송 완료 (DB => CNS)", MethodBase.GetCurrentMethod().DeclaringType);
                        MessageBoxResult result = System.Windows.MessageBox.Show("데이터 전송 완료", "CNS 데이터 전송");
                    }
                    catch (Exception)
                    {
                        LogManager.Error("데이터 전송 실패 (DB => CNS)", MethodBase.GetCurrentMethod().DeclaringType);
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

        private int ValueConvert2(int data)
        {
            //앞뒤 두자리 묶음 자리변환
            int value = data;

            //값이 양수일때
            if (value >= 0)
            {
                return value;
            }
            //값이 음수일때
            else
            {
                int positiveValue = 65536 + value;
                return positiveValue;
            }
        }



        //CNS 데이터 저장 버튼
        //CNS 데이터를 읽고 DB에 저장함
        private void Plc_ButtonMouseDown2(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("데이터를 PC에저장 하시겠습니까 ?", "CNS 데이터 저장", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) //확인
            {
                LogManager.Info("CNS 데이터 저장 버튼 클릭 (CNS => DB)", MethodBase.GetCurrentMethod().DeclaringType);

                if (SettingJsonManager.Plc) //PLC 사용 체크일때
                {
                    Console.WriteLine("CNS => DB 저장 prodID : " + this.mainModel.prodID);
                    SingleWrite(); //현재 설정을 저장하기 위해서는 D3814번에 0이 아닌 수를 넣어줘야함
                    CNSDataRead(); //CNS 데이터 읽기
                }
                else
                {
                    LogManager.Error("CNS 데이터 저장 실패 (CNS사용 미체크)", MethodBase.GetCurrentMethod().DeclaringType);
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
            LogManager.Info("CNS 데이터 보기 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

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
            if (SettingJsonManager.corp_name.Equals("금화") || SettingJsonManager.corp_name.Equals("금화테스트") ||
                SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
            {
                return value;
            }
            else
            {
                // 주어진 값이 null이 아닌 경우
                if (value != null)
                {
                    float floatValue = float.Parse(value.ToString());
                    if (floatValue < 0)
                    {
                        // 주어진 값이 0보다 작은 경우, "추가생산 " 문자열과 주어진 값의 절댓값을 곱한 결과를 반환
                        return "추가생산 " + (floatValue * -1);
                    }
                    else
                    {
                        // 그렇지 않으면 주어진 값을 그대로 반환
                        return value;
                    }
                }
                else
                {
                    return value;
                }
            }
            //return value != null ? float.Parse(value.ToString()) < 0 ? "추가생산 " + (float.Parse(value.ToString()) * -1) : value : value;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
