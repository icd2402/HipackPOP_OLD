using Newtonsoft.Json.Linq;
using PopProject.Detail;
using PopProject.input;
using PopProject.Input;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PopProject.Consumer_
{
    /// <summary>
    /// DsConsumer_input.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DsConsumer_input : Window
    {
        public readonly InputModel inputModel;
        public int inlineCount = 0;
        private ConsumerModel consumerModel;
        private MainModel mainModel;
        private float totalPerrCount = 0;
        private bool checkChangeButton = false;
        public Win_KeyBoard win_KeyBoard;
        private DispatcherTimer timer;
        int pworkkey;
        int workmemberkey, second =0;
        private bool startPBLock = false;
        private DateTime selectedDate = new DateTime();
    
        public DsConsumer_input(ConsumerModel consumerModel, MainModel mainModel)
        {
            InitializeComponent();
            this.consumerModel = consumerModel;
            this.mainModel = mainModel;
            inputModel = new InputModel();

            LogManager.Info("실수요자 input창 팝업: " + consumerModel.membName + " / " + consumerModel.vendorName, MethodBase.GetCurrentMethod().DeclaringType);

            //이월버튼 비활성화
            box1.Visibility = Visibility.Collapsed; //이월버튼 비활성화
            Canvas.SetTop(box, 540);

            ConvertModel();
            PopBeforeProcessItemCount();
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
        }
        private void ConvertModel()
        {
            inputModel.pCnt = mainModel.pCnt;                   //통수
            inputModel.orderWorkNo = mainModel.orderWorkNo;     //
            inputModel.orderLast = mainModel.orderLast;         //납기일
            inputModel.orderCount = consumerModel.orderCount;   //수주량
            inputModel.pplanCount = consumerModel.orderCount;   //계획량
            inputModel.pSize = mainModel.pSize;                 //규격
            inputModel.customSName = mainModel.customSName;     //거래처명
            inputModel.sumPlanCount = consumerModel.orderCount; //총계획수량
            inputModel.errorModel = new errorModel();           //불량건
            inputModel.pplanKey = mainModel.pplanKey;           //계획키
            inputModel.orderKey = mainModel.orderKey;           //오더키
            inputModel.ProcessType = mainModel.ProcessType;     //설비번호
            inputModel.newProd = mainModel.newProd;             //
            inputModel.sDatetime = consumerModel.sDatetime;     //시작시간
            inputModel.eDatetime = "";                          //종료시간
            inputModel.perrCount = 0;                           //불량수량
            inputModel.count = 0;                               //생산수량
            inputModel.pMemo = "";                              //비고
            inputModel.pworkCount = consumerModel.pWorkCount;   //작업량
            inputModel.prodName = mainModel.prodName + " \n(" + consumerModel.vendorName + "/" + consumerModel.membName + ")"; //품명

            //시작시간이 있고 완료시간 없으면
            if (inputModel.sDatetime.Length > 3 && inputModel.eDatetime.Length <= 1)
            {
                inputModel.imagePath = "/img/bn_06.png";//종료
            }
            else
            {
                inputModel.imagePath = "/img/bn_05.png";//시작
            }

            mainModel.inputModel = inputModel;
            DataContext = inputModel;

            PopResultSearchMember();//생산 결과 조회
            PopErrorSearch();//불량수량 조회
            TimerSet();

        }

        //1초 타이머
        private void TimerSet()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

        }

        //타이머 실행
        private void Timer_Tick(object sender, EventArgs e)
        {
            second++;

            //10초마다 실행
            if(second == 10)
            {
                mainModel.inputModel = inputModel;
                Main.production = mainModel;
                second = 0;
            }
           
        }

        //불량건 조회
        private void PopErrorSearch()
        {
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
                    ObservableCollection<string> ob = new ObservableCollection<string>();
                    foreach (JObject result in Data_Jarry)
                    {
                        string memo = result["Memo"].ToString();
                        ob.Add(memo);
                    }
                    inputModel.errorModel.memo = "";
                    combo.ItemsSource = ob;
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopErrorSearch 조회 오류");
                _ = msg.ShowDialog();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        //확장 창 클릭
        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (checkChangeButton)
            {
                LogManager.Info("실수요자 input창 축소", MethodBase.GetCurrentMethod().DeclaringType);

                changeButtonIn.Visibility = Visibility.Collapsed;
                changeButtonOut.Visibility = Visibility.Visible;
                checkChangeButton = false;
                Width = 550;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
            else
            {
                LogManager.Info("실수요자 input창 확장", MethodBase.GetCurrentMethod().DeclaringType);

                changeButtonIn.Visibility = Visibility.Visible;
                changeButtonOut.Visibility = Visibility.Collapsed;
                checkChangeButton = true;
                Width = 1105;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
        }

        //바코드
        private void TbBarcode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        //생산수량 텍스트 클릭
        private void TbResult_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LogManager.Info("실수요자 input창 생산수량 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                if (context.Name.Equals("tbResult"))
                {
                    win_KeyBoard = new Win_KeyBoard(context, context.DataContext, inputModel)
                    {
                        WindowStartupLocation = WindowStartupLocation.Manual,
                        Left = 300
                    };

                    _ = win_KeyBoard.ShowDialog();
                    win_KeyBoard = null;
                }
                else
                {
                    InputModel inputmodel = (InputModel)context.DataContext;
                    win_KeyBoard = new Win_KeyBoard(context, inputmodel.errorModel, inputmodel);
                    _ = win_KeyBoard.ShowDialog();
                    win_KeyBoard = null;
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
                float value = 1;
                if (context.count - value >= 0)
                {
                    inputModel.count -= value;

                }
            }
        }

        //생산수량 + 버튼
        private void Result_count_plus(object sender, MouseButtonEventArgs e)
        {
            float value = 1;
            inputModel.count += value;
        }

        //시작,종료 버튼 클릭
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //관리자 로그인이 아닐때
            if (!SettingJsonManager.manager.Equals("1"))
            {
                // 시작 버튼 클릭
                if (inputModel.imagePath.Equals("/img/bn_05.png") && startPBLock.Equals(false))
                {
                    LogManager.Info("실수요자 input창 시작버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

                    startPBLock = true;

                    if (Main.production != null)
                    {
                        MessageBox_Dialog messageBox = new MessageBox_Dialog(inputModel.prodName + "\n" + "해당 건이 작업중입니다.");
                        _ = messageBox.ShowDialog();

                        //MessageBox_Dialog2 messageBox2 = new MessageBox_Dialog2(inputModel, mainModel, inputModel.prodName + "\n" + "해당 건이 작업중입니다.", true);
                        //_ = messageBox2.ShowDialog();
                        startPBLock = false;

                    }
                    else
                    {
                        // 실수요자 작업중인건이 있을 경우
                        if (CheckConsumerWork())
                        {
                            MessageBox_Dialog messageBox = new MessageBox_Dialog("실수요자 작업중인 건이 있습니다.");
                            _ = messageBox.ShowDialog();
                            startPBLock = false;
                        }
                        else //작업 중인건 없음
                        {
                            try
                            {
                                Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            catch
                            {
                            }

                            PopResultStartMember(); //생산시작 [PopResultStartMember]
                            PopResultSearchMember(); //생산결과 건 조회 [PopResultSearchMember]
                            CountManager.SendMsg(SettingJsonManager.mercName, "S"); //현황판으로 생산종료 송신

                        }
                    }
                }
                // 종료 버튼 클릭
                else if (inputModel.imagePath.Equals("/img/bn_06.png"))
                {
                    LogManager.Info("실수요자 input창 종료버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);
                    startPBLock = false;

                    MessageBoxResult result = System.Windows.MessageBox.Show("작업을 저장 하시겠습니까 ?", "작업 저장", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                    }
                    else //yes
                    {
                        string metrialitem = SettingJsonManager.meterialinfo; //작업정보 설정 저장
                        Console.WriteLine("실수요자 결과수량 : " + inputModel.count);

                        if (PopResultEndMember() == 1)
                        {
                            SettingJsonManager.consumersumcount = SettingJsonManager.consumersumcount + inputModel.pworkCount; //생산
                            SettingJsonManager.consumersumerrorcount = SettingJsonManager.consumersumerrorcount + inputModel.perrCount; //불량
                            Console.WriteLine("consumerCount : " + SettingJsonManager.consumersumcount);
                            SettingJsonManager.SaveJsonFile();

                            Main.production = null; //생산정보 리셋
                            inputModel.perrCount = 0; //불량수량 리셋
                            inputModel.count = 0; //생산수량 리셋
                            inputModel.pMemo = ""; //비고 리셋
                            inputModel.pworkCount = 0; //방금 생산량 리셋
                            PopResultSearchMember(); //생산 결과 조회
                            DSConsumer.product.imgSource = "/img/bn_03.png"; //완료버튼으로 변경

                            //아두이노 카운터 리셋
                            try
                            {
                                Main.arduSerial.Write("A");
                            }
                            catch (Exception ardu)
                            {
                                MessageBox_Dialog msg = new MessageBox_Dialog("아두이노 연결 오류!!");
                            }

                            //구룡포장 
                            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
                            {
                                if (SettingJsonManager.mercName.Contains("인라인"))
                                {
                                    if (mainModel.subOb.Count == 2)
                                    {
                                        //PopResultInlineStartMember(mainModel.subOb[mainModel.subOb.Count - 1]);
                                    }
                                }
                            }

                        }
                        else
                        {
                            MessageBox_Dialog box = new MessageBox_Dialog("저장 오류!!");
                            _ = box.ShowDialog();
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

        //생산 결과 저장
        private int PopResultEndMember()
        {
            int count = 0;
            int ChkAED; //삭제상태

            //생산수량,불량수량 없으면
            if (inputModel.pworkCount == 0 && inputModel.perrCount == 0)
            {
                ChkAED = 3; //삭제
            }
            else //작업건 있으면
            {
                ChkAED = 1; //입력
            }

            inlineCount = int.Parse(tbResult.Text.ToString());   //생산수량 담기
            int workcount = int.Parse(tbResult.Text.ToString()); //생산수량 담기

            //상자수 사용시 생산수량 * 통수
            if (SettingJsonManager.boxCheck)
            {
                workcount *= (int)mainModel.pCnt;
            }

            if (SettingJsonManager.corp_name.Contains("테스트"))
            {
                LogManager.Debug("실수요자 input창 결과저장 : [PopResultEndMember] : " + Environment.NewLine +
                                 "작업일자: " + DateTimeManager.GetSelectedDateStr() + Environment.NewLine +      // 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate
                                 "오더Key: " + inputModel.orderKey + Environment.NewLine +
                                 "ChkAED: " + ChkAED + Environment.NewLine +
                                 "실수요자생산Key: " + inputModel.workMemberKey + Environment.NewLine +
                                 "작업량: " + workcount + Environment.NewLine +
                                 "불량: " + inputModel.perrCount, MethodBase.GetCurrentMethod().DeclaringType);

                Console.WriteLine("---------------------------------------------------------------------------" + Environment.NewLine +
                                 "실수요자 input창 결과저장 : [PopResultEndMember] : " + Environment.NewLine +
                                 "작업일자: " + DateTimeManager.GetSelectedDateStr() + Environment.NewLine +      // 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate
                                 "오더Key: " + inputModel.orderKey + Environment.NewLine +
                                 "ChkAED: " + ChkAED + Environment.NewLine +
                                 "실수요자생산Key: " + inputModel.workMemberKey + Environment.NewLine +
                                 "작업량: " + workcount + Environment.NewLine +
                                 "불량: " + inputModel.perrCount + Environment.NewLine +
                                 "---------------------------------------------------------------------------");
            }

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEndMember]",
                new string[] { "@작업일자", "@오더Key", "@ChkAED", "@실수요자생산Key", "@작업량", "@불량" },
                new object[] {
                DateTimeManager.GetSelectedDateStr(),      // 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate
                inputModel.orderKey,
                ChkAED,
                inputModel.workMemberKey,
                workcount+"",
                inputModel.perrCount,
                 }, 1
              );

            if (returnData.결과 == 1)
            {
                SettingJsonManager.consumCount += workcount; //생산수량 담기
                SettingJsonManager.SaveJsonFile();
                CountManager.SendMsg(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);
               
                count = int.Parse(tbResult.Text.ToString()); //생산수량 담기

                //생산수량 있으면 이월버튼 활성화
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
            }
            return returnData.결과;
        }

        //인라인설비 결과 저장
        private int PopResultInlineEndMember()
        {
            int count = 0;
            int ChkAED; //삭제상태

            //작업건 없으면
            if (inlineCount == 0 && inputModel.perrCount == 0)
            {
                ChkAED = 3; //삭제상태 3
            }
            else
            {
                ChkAED = 1; //삭제상태1
            }

            int workcount = int.Parse(tbResult.Text.ToString()); //생산수량 담기
            //상자수 사용시 생산수량 * 통수
            if (SettingJsonManager.boxCheck)
            {
                workcount *= (int)mainModel.pCnt;
            }

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEndMember]",
                new string[] { "@작업일자", "@오더Key", "@ChkAED", "@실수요자생산Key", "@작업량", "@불량" },
                new object[] {
                DateTimeManager.GetSelectedDateStr(),      // 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate
                inputModel.orderKey,
                ChkAED,
                inputModel.workMemberKey,
                inlineCount+"",
                inputModel.perrCount,
                 }, 1
              );

            if (returnData.결과 == 1)
            {
                inlineCount = 0;
                CountManager.SendMsg(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);
                count = int.Parse(tbResult.Text.ToString());

                //생산수량 있으면 이월버튼 활성화
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
            }
            return returnData.결과;
        }


        //인라인설비 작업 시작
        private void PopResultInlineStartMember(SubModel subOb)
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
              SettingJsonManager.serverIndex,
              "[PopResultStartMember]",
              new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey", "@pworkKey", "@vendorID", "@membID" },
              new object[] {
                  DateTimeManager.GetSelectedDateStr(),      // 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate
                  consumerModel.orderKey,
                  inputModel.pplanKey,
                  subOb.processType,
                  subOb.ProcessTypeName,
                  MainWindow.userID,
                  MainWindow.userName,
                  inputModel.workerName,
                  inputModel.workerId,
                  inputModel.SCMKey,
                  SettingJsonManager.pworkkey,
                  consumerModel.vendorID,
                  consumerModel.membID
              }, 1

            );

            if (returnData.결과 == 1)
            {
                string[] result = returnData.리턴값.Split(new string[] { "|" }, StringSplitOptions.None);
                SettingJsonManager.consumpWorkKey = int.Parse(result[0]);
                //SettingJsonManager.pworkkey = result[0];
                SettingJsonManager.SaveJsonFile();
                inputModel.pworkKey = int.Parse(result[0]);
                inputModel.workMemberKey = int.Parse(result[1]);

                mainModel.sDatetime = (DateTime.Now.ToString("yyyy-MM-dd") == DateTimeManager.GetSelectedDateStr()) ?     
                                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTimeManager.GetSelectedStr();// 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate

                Main.production = mainModel;

                Console.WriteLine("result[0]" + result[0]);
                Console.WriteLine("result[0]" + result[1]);
                PopResultInlineEndMember();
            }
        }

        //생산 시작
        private void PopResultStartMember()
        {

            if (SettingJsonManager.corp_name.Contains("테스트"))
            {
                Console.WriteLine("---------------------------------------------------------------------------" + Environment.NewLine +
                                  "실수요자 시작버튼 [PopResultStartMember] : " + Environment.NewLine +
                                  "pworkDate:" + DateTimeManager.GetSelectedDateStr() + Environment.NewLine +
                                  "orderKey:" + consumerModel.orderKey + Environment.NewLine +
                                  "pplanKey:" + inputModel.pplanKey + Environment.NewLine +
                                  "ProcessType:" + SettingJsonManager.processType + Environment.NewLine +
                                  "mercName:" + SettingJsonManager.mercName + Environment.NewLine +
                                  "UpUserID:" + MainWindow.userID + Environment.NewLine +
                                  "UpUserName:" + MainWindow.userName + Environment.NewLine +
                                  "workerName:" + inputModel.workerName + Environment.NewLine +
                                  "workerID:" + inputModel.workerId + Environment.NewLine +
                                  "SCMKey:" + inputModel.SCMKey + Environment.NewLine +
                                  "pworkKey:" + SettingJsonManager.pworkkey + Environment.NewLine +
                                  "vendorID:" + consumerModel.vendorID + Environment.NewLine +
                                  "membID" + consumerModel.membID + Environment.NewLine +
                                  "---------------------------------------------------------------------------");
            }

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
              SettingJsonManager.serverIndex,
              "[PopResultStartMember]",
              new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName",
                             "@workerID", "@SCMKey", "@pworkKey", "@vendorID", "@membID" },
              new object[] {
                  DateTimeManager.GetSelectedDateStr(),      
                  consumerModel.orderKey,
                  inputModel.pplanKey,
                  SettingJsonManager.processType,
                  SettingJsonManager.mercName,
                  MainWindow.userID,
                  MainWindow.userName,
                  inputModel.workerName,
                  inputModel.workerId,
                  inputModel.SCMKey,
                  SettingJsonManager.pworkkey,
                  consumerModel.vendorID,
                  consumerModel.membID
              }, 1
            );

            if (returnData.결과 == 1)
            {
                string[] result = returnData.리턴값.Split(new string[] { "|" }, StringSplitOptions.None);
                SettingJsonManager.consumpWorkKey = int.Parse(result[0]);
                //SettingJsonManager.pworkkey = result[0];
                SettingJsonManager.SaveJsonFile();
                inputModel.pworkKey = int.Parse(result[0]);
                inputModel.workMemberKey = int.Parse(result[1]);
                mainModel.sDatetime = (DateTime.Now.ToString("yyyy-MM-dd") == DateTimeManager.GetSelectedDateStr()) ?
                                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTimeManager.GetSelectedStr();
                Main.production = mainModel;

                Console.WriteLine("result[0]" + result[0]);
                Console.WriteLine("result[0]" + result[1]);

                PopResultEndTimeSetting();
            }

        }

        private void PopResultEndTimeSetting()
        {

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[spUdt431processinfo_workTimeChange]",
                new string[] { "@생산키", "@생산시작일시", "@생산종료일시" },
                new object[] {
                SettingJsonManager.pworkkey,
                "",
                ""},
                1
                );

            if (returnData.결과 == 1)
            {
            }
            else
            {
            }
        }

        //불량건 조회
        private ObservableCollection<errorModel> PopErrorListSearchMember(ConsumerModel listitem)
        {
            ObservableCollection<errorModel> ob = null;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,
               "[PopErrorListSearchMember]",
               new string[] { "@workMemberKey" },
               new object[] { listitem.workMemberKey },
               0
               );

            Console.WriteLine("WorkMemberKey -> " + listitem.workMemberKey);
            if (returnData.결과 == 1)
            {

                ob = new ObservableCollection<errorModel>();
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    errorModel startItem = new errorModel
                    {

                        updateTime = DateTime.Parse(listitem.sDatetime),
                        memo = "작업시작",
                        fontColor = Brushes.Green,
                        fontVisible = Visibility.Collapsed
                    };
                    ob.Add(startItem);

                    foreach (JObject result in Data_JArray)
                    {
                        errorModel item = new errorModel
                        {
                            workMemberKey = listitem.workMemberKey,
                            ConsumerModel = listitem,
                            perrKey = result["workErrorMemberKey"].ToString(),
                            memo = result["Memo"].ToString(),
                            perrCount = float.Parse(result["perrCnt"].ToString()),
                            updateTime = DateTime.Parse(result["UpDatetime"].ToString()),
                            fontColor = Brushes.Black
                        };

                        totalPerrCount += item.perrCount;
                        ob.Add(item);

                    }
                }
            }
            return ob;
        }

        //생산 결과 조회
        private void PopResultSearchMember()
        {
            float tempCount = inputModel.count;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultSearchMember]",
                new string[] { "@orderKey" },
                new object[] { consumerModel.orderKey },
                0
                );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ObservableCollection<ConsumerModel> ob = new ObservableCollection<ConsumerModel>();
                    inputModel.sumPworkCount = 0;
                    tbEmpty.Visibility = Data_JArray.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                    foreach (JObject result in Data_JArray)
                    {
                        ConsumerModel listitem = new ConsumerModel
                        {

                            workMemberKey = result["workMemberKey"].ToString(),         //실수요자 작업키
                            pworkDate = DateTime.Parse(result["pworkDate"].ToString()), //작업일자
                            pWorkCount = float.Parse(result["pworkCount"].ToString()),  //작업량
                            pworkKey = int.Parse(result["pworkKey"].ToString()),        //작업키
                            sDatetime = result["sDatetime"].ToString()                  //시작시간
                        };
                        listitem.eDatetime = result["sDatetime"].ToString().Length > 2 ? result["eDatetime"].ToString() : ""; //종료시간
                        listitem.orderKey = result["orderKey"].ToString();                  //오더키
                        listitem.pplanKey = int.Parse(result["pplanKey"].ToString());       //계획키
                        listitem.ProcessType = int.Parse(result["ProcessType"].ToString()); //설비번호
                        listitem.pworkKey = int.Parse(result["pworkKey"].ToString());       //작업
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());       //불량수량
                        //listitem.pMemo = result["pMemo"].ToString();                        //비고
                        if (consumerModel.membID.Equals(result["membID"].ToString()) && SettingJsonManager.processType.Equals(listitem.ProcessType))
                        {
                            listitem.errorModelOb = PopErrorListSearchMember(listitem); //불량 리스트 조회
                        }

                        //종료시간이 있으면
                        if (listitem.eDatetime.Length > 2)
                        {
                            errorModel item2 = new errorModel
                            {
                                workMemberKey = result["workMemberKey"].ToString(),
                                memo = "작업완료",
                                perrCount = listitem.pWorkCount,
                                updateTime = DateTime.Parse(listitem.eDatetime),
                                fontColor = Brushes.Red
                            };

                            if (SettingJsonManager.boxCheck)
                            {
                                item2.perrCount /= mainModel.pCnt;
                            }

                            if (consumerModel.membID.Equals(result["membID"].ToString()) && SettingJsonManager.processType.Equals(listitem.ProcessType))
                            {
                                listitem.errorModelOb.Add(item2);
                            }
                        }
                        // if (listitem.errorModelOb != null)
                        // {
                        //    foreach (errorModel item in listitem.errorModelOb)
                        //    {
                        //        listitem.perrCnt += item.perrCount;
                        //    }
                        //}
                        inputModel.sumPworkCount += listitem.pWorkCount;

                        if (consumerModel.membID.Equals(result["membID"].ToString()) && SettingJsonManager.processType.Equals(listitem.ProcessType))
                        {
                            ob.Add(listitem);
                        }
                    }

                    if (ob.Count > 0)
                    {

                        ConsumerModel tempItem = ob[ob.Count - 1];

                        //시작 시간은 있는데 종료 시간이 없으면 (생산중이면)
                        if (tempItem.sDatetime.ToString().Length > 3 && tempItem.eDatetime.ToString().Length <= 1)
                        {
                            inputModel.imagePath = "/img/bn_06.png"; //종료 버튼
                            DSConsumer.product.imgSource = "/img/bn_01.png";//작업중 버튼
                            inputModel.workMemberKey = int.Parse(tempItem.workMemberKey);
                            inputModel.pworkDate = tempItem.pworkDate;
                            inputModel.pworkDate = tempItem.pworkDate;
                            inputModel.pworkKey = tempItem.pworkKey;
                            inputModel.orderKey = tempItem.orderKey;
                            inputModel.pplanKey = tempItem.pplanKey;
                            inputModel.ProcessType = tempItem.ProcessType;
                            inputModel.perrCount = tempItem.perrCnt;
                            inputModel.count = tempCount;
                            inputModel.pMemo = tempItem.Memo;
                            inputModel.sDatetime = tempItem.sDatetime;
                            inputModel.eDatetime = tempItem.eDatetime;
                        }   
                        else //생산중이 아니면
                        {
                            inputModel.imagePath = "/img/bn_05.png"; //시작버튼

                            //종료시간이 있으면
                            if (inputModel.eDatetime.Length > 3)
                            {
                                DSConsumer.product.imgSource = "/img/bn_03.png"; //완료 버튼
                                //mainModel.imagePath = "/img/bn_03.png";
                            }
                            else
                            {
                                //  Consumer.product.imgSource = "/img/bn_02.png";
                                //  mainModel.imagePath = "/img/bn_02.png";
                            }

                            inputModel.eDatetime = tempItem.eDatetime;
                            inputModel.pworkKey = tempItem.pworkKey;
                        }
                    }

                    inputModel.sumPlanCount = inputModel.pplanCount;
                    Console.WriteLine("ob.count : " + ob.Count.ToString());
                    if (ob.Count != 0)
                    {
                        try
                        {
                            Console.WriteLine("ob.count sDatetime: " + ob[ob.Count - 1].sDatetime.ToString());
                            Console.WriteLine("ob.count eDatetime: " + ob[ob.Count - 1].eDatetime.ToString());
                            Console.WriteLine("ob.count imagePath: " + ob[ob.Count - 1].imgSource.ToString());
                        }
                        catch (Exception)
                        {

                        }
                    }
                    ic.ItemsSource = ob;
                }

                txtTotalPerrCount.Text = totalPerrCount.ToString();
            }
            sv.ScrollToVerticalOffset(ic.ActualHeight);
        }

        //실수요자 작업중인건이 있는지 확인
        private bool CheckConsumerWork()
        {
            bool check = false;
            for (int i = 0; i < DSConsumer.consumerList.Count; i++)
            {
                //시작시간 있고 완료시간 없으면
                if (DSConsumer.consumerList[i].sDatetime.Length > 2 && DSConsumer.consumerList[i].eDatetime.Length <= 1)
                {
                    check = true;
                    break;
                }
                else//작업중인 건 없으면
                {
                    check = false;
                }
            }
            return check;
        }

        //특이사항 버튼 클릭
        private void Unique(object sender, MouseButtonEventArgs e)
        {
            Unique_input unique_Input = new Unique_input(inputModel);
            unique_Input.ShowDialog();
        }

        private void Win_input_exit(object sender, MouseButtonEventArgs e)
        {
            LogManager.Info("실수요자 input창 닫기 클릭", MethodBase.GetCurrentMethod().DeclaringType);
            Close();
        }

        private void Detail_item(object sender, MouseButtonEventArgs e)
        {
            Close();
            string special = "";

            

            Win_Detail win_Detail = new Win_Detail(mainModel, special,consumerModel, SettingJsonManager.detail_check, true);
            win_Detail.ShowDialog();
        }

        private void 이월(object sender, MouseButtonEventArgs e)
        {
            MessageBox_Dialog msg = new MessageBox_Dialog("계획화면에서 이월 해주세요.");
            msg.ShowDialog();
        }

        private void Result_badCount_minus(object sender, MouseButtonEventArgs e)
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

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void consum_closed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        //생산 완료수량 수정
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!SettingJsonManager.manager.Equals("1"))
            {
                e.Handled = true;

                if (e.ClickCount == 2)
                {
                    LogManager.Info("실수요자 input창 생산결과 수량 수정", MethodBase.GetCurrentMethod().DeclaringType);

                    if (sender.GetType().Name.Equals("Border"))
                    {
                        Border border = (Border)sender;
                        errorModel context = (errorModel)border.DataContext;

                        //Console.WriteLine("메모" + context.memo);

                        if (!context.memo.Equals("작업시작") && !context.memo.Equals("작업완료"))
                        {
                            //can.Visibility = Visibility.Visible;
                            //Win_Update win_Update = new Win_Update(context,inputModel);
                            //_ = win_Update.ShowDialog();

                            //PopResultSearchMember();
                            //can.Visibility = Visibility.Collapsed;
                        }
                        else if (context.memo.Equals("작업완료"))
                        {
                            can.Visibility = Visibility.Visible;
                            Win_resultUpdate win_resultUpdate = new Win_resultUpdate(context, inputModel, true);
                            _ = win_resultUpdate.ShowDialog();
                            PopResultSearchMember(); //생산 결과 조회
                            can.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }


            
        }
    }
}
