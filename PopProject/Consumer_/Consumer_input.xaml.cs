using Newtonsoft.Json.Linq;
using PopProject.input;
using PopProject.Input;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PopProject.Consumer_
{
    /// <summary>
    /// Consumer_input.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Consumer_input : Window
    {
        private readonly string metrialitem = SettingJsonManager.meterialinfo; //작업정보 설정값 저장

        private readonly ConsumerModel consumerModel;
        private readonly MainModel mainModel;
        public readonly InputModel inputModel;
        public Win_KeyBoard win_KeyBoard;

        private bool checkChangeButton = false; //확장창 버튼 활성유뮤 
        private float totalPerrCount = 0; //인풋 확장창 총불량 수량
        public static bool check = false; //생산중??
        private int count = 0; //생산수량


        public Consumer_input(ConsumerModel consumerModel, MainModel mainModel)
        {
            InitializeComponent();
            this.consumerModel = consumerModel;
            this.mainModel = mainModel;
            inputModel = new InputModel();
            ConvertModel();

            //생산결과값 있을때만 이월버튼 활성화
            if (consumerModel.pWorkCount > 0 && consumerModel.sDatetime.Length > 3 && consumerModel.eDatetime.Length > 3)
            {
                box1.Visibility = Visibility.Visible; //이월 버튼 활성화
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);
            }
            else
            {
                box1.Visibility = Visibility.Collapsed; //이월버튼 비활성
                Canvas.SetTop(box, 540);
            }
        }

        //작업정보 설정
        public Consumer_input(string metrialitem)
        {
            this.metrialitem = metrialitem;
        }


        private void ConvertModel()
        {
            inputModel.pCnt = mainModel.pCnt;
            inputModel.orderWorkNo = mainModel.orderWorkNo;
            inputModel.orderLast = mainModel.orderLast;
            inputModel.orderCount = consumerModel.orderCount;
            inputModel.pplanCount = consumerModel.orderCount;
            inputModel.pSize = mainModel.pSize;
            inputModel.customSName = mainModel.customSName;
            inputModel.sumPlanCount = consumerModel.orderCount;
            inputModel.errorModel = new errorModel();
            inputModel.pplanKey = mainModel.pplanKey;
            inputModel.orderKey = mainModel.orderKey;
            inputModel.ProcessType = mainModel.ProcessType;
            inputModel.newProd = mainModel.newProd;
            inputModel.sDatetime = consumerModel.sDatetime;
            inputModel.eDatetime = "";
            inputModel.perrCount = 0;
            inputModel.count = 0;
            inputModel.pMemo = "";
            inputModel.pworkCount = consumerModel.pWorkCount;
            inputModel.imagePath = inputModel.sDatetime.Length > 3 && inputModel.eDatetime.Length <= 1 ? "/img/bn_06.png" : "/img/bn_05.png";
            inputModel.prodName = mainModel.prodName + " (" + consumerModel.vendorName + "/" + consumerModel.membName + ")";
            switch (SettingJsonManager.processType)
            {
                case 5:
                    inputModel.pCntVisible = Visibility.Collapsed;
                    break;
                case 8:
                    inputModel.pCntVisible = Visibility.Collapsed;
                    break;
                case 9:
                    inputModel.pCntVisible = Visibility.Collapsed;
                    break;
                case 10:
                    inputModel.pCntVisible = Visibility.Collapsed;
                    break;
                case 11:
                    inputModel.pCntVisible = Visibility.Collapsed;
                    break;

                default:
                    inputModel.pCntVisible = Visibility.Visible;
                    break;
            }

            mainModel.inputModel = inputModel;
            DataContext = inputModel;

            PopResultSearchMember(); //실수요자 결과조회
            PopErrorSearch();        //불량 조회

        }

        //실수요자 결과조회
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
                    tbEmpty.Visibility = Data_JArray.Count > 0 ? Visibility.Collapsed : Visibility.Visible; //확장창에 결과값에따른 표기

                    foreach (JObject result in Data_JArray)
                    {
                        ConsumerModel listitem = new ConsumerModel
                        {
                            workMemberKey = result["workMemberKey"].ToString(),
                            pworkDate = DateTime.Parse(result["pworkDate"].ToString()),
                            pWorkCount = float.Parse(result["pworkCount"].ToString()),
                            pworkKey = int.Parse(result["pworkKey"].ToString()),
                            sDatetime = result["sDatetime"].ToString()
                        };
                        listitem.eDatetime = result["sDatetime"].ToString().Length > 2 ? result["eDatetime"].ToString() : "";
                        listitem.orderKey = result["orderKey"].ToString();
                        listitem.pplanKey = int.Parse(result["pplanKey"].ToString());
                        listitem.ProcessType = int.Parse(result["ProcessType"].ToString());
                        listitem.pworkKey = int.Parse(result["pworkKey"].ToString());
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());
                        //listitem.pMemo = result["pMemo"].ToString();
                        listitem.errorModelOb = PopErrorListSearchMember(listitem);

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
                            listitem.errorModelOb.Add(item2);

                        }

                        // if (listitem.errorModelOb != null)
                        // {
                        //    foreach (errorModel item in listitem.errorModelOb)
                        //    {
                        //        listitem.perrCnt += item.perrCount;
                        //    }
                        //}
                        inputModel.sumPworkCount += listitem.pWorkCount;
                        if (consumerModel.membID.Equals(result["membID"].ToString()))
                        {

                        }

                        ob.Add(listitem);
                    }

                    if (ob.Count > 0)
                    {

                        ConsumerModel tempItem = ob[ob.Count - 1];

                        if (tempItem.sDatetime.ToString().Length > 3 && tempItem.eDatetime.ToString().Length <= 1)
                        {
                            inputModel.imagePath = "/img/bn_06.png";
                            Consumer.product.imgSource = "/img/bn_01.png";
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
                        else
                        {
                            inputModel.imagePath = "/img/bn_05.png";

                            if (inputModel.eDatetime.Length > 3)
                            {
                                Consumer.product.imgSource = "/img/bn_03.png";
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

        //실수요자 불량 조회
        private ObservableCollection<errorModel> PopErrorListSearchMember(ConsumerModel listitem)
        {
            ObservableCollection<errorModel> ob = null;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               SettingJsonManager.serverIndex,
               "[PopErrorListSearchMember]",
               new string[] { "@workMemberKey" },
               new object[] { listitem.workMemberKey }, 0

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


        //불량조회
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
                //MessageBox_Dialog msg = new MessageBox_Dialog("PopErrorSearch 조회 오류");
                //_ = msg.ShowDialog();
                LogManager.Error("불량사유 리스트 조회 실패", MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        //확장창 리스트 더블클릭
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (e.ClickCount == 2)
            {
                if (sender.GetType().Name.Equals("Border"))
                {
                    Border border = (Border)sender;
                    errorModel context = (errorModel)border.DataContext;

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
                        can.Visibility = Visibility.Visible; //화면 회색처리
                        Win_resultUpdate win_resultUpdate = new Win_resultUpdate(context, inputModel, true);
                        _ = win_resultUpdate.ShowDialog(); //결과값 변경 창 팝업
                        PopResultSearchMember(); //실수요자 결과 조회
                        can.Visibility = Visibility.Collapsed; //화면 회색처리 해제
                    }
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //확장 버튼 클릭
        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (checkChangeButton)
            {
                changeButtonOut.Visibility = Visibility.Visible;  // -> 버튼 활성화 
                changeButtonIn.Visibility = Visibility.Collapsed; // <- 버튼 비활성화
                checkChangeButton = false;
                Width = 550;
                Left =
                    (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
            else
            {
                changeButtonOut.Visibility = Visibility.Collapsed; // -> 버튼 ql활성화 
                changeButtonIn.Visibility = Visibility.Visible;    // <- 버튼 활성화
                checkChangeButton = true;
                Width = 1105;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }

        }

        private void Detail_item(object sender, MouseButtonEventArgs e)
        {
        }

        //작업 시작 버튼
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("Main.production.orderKey : " + Main.production.orderKey);
            Console.WriteLine("Consumer.orderkey : " + consumerModel.orderKey);

            if (inputModel.imagePath.Equals("/img/bn_05.png")) //시작버튼 확성화
            {
                if (CheckConsumerWork())
                {
                    MessageBox_Dialog messageBox = new MessageBox_Dialog("실수요자 작업중인 건이 있습니다.");
                    _ = messageBox.ShowDialog();
                }
                else
                {
                    _ = new LogData("작업시작");
                    PopResultStartMember();
                    PopResultSearchMember();
                }
            }
            else if (inputModel.imagePath.Equals("/img/bn_06.png")) //종료버튼 활성화
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("작업을 저장 하시겠습니까 ?", "작업 저장", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) //취소
                {
                    Close();
                }
                else //확인
                {
                    string metrialitem = SettingJsonManager.meterialinfo;
                    Console.WriteLine("end_count" + inputModel.count);
                    if (PopResultEndMember() == 1)
                    {
                        _ = new LogData("작업종료");
                        switch (metrialitem)
                        {
                            case "19": //마닐라
                            case "20": //원단
                                Main1.production = null;
                                try
                                {
                                    if (Main1.production != null)
                                    {
                                        Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                                        Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                                    }
                                    else
                                    {
                                        Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                break;

                            default:
                                Main.production = null;
                                try
                                {
                                    if (Main.production != null)
                                    {
                                        Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                                        Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                                    }
                                    else
                                    {
                                        Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                break;
                        }

                        SettingJsonManager.consumersumcount = SettingJsonManager.consumersumcount + inputModel.pworkCount;
                        SettingJsonManager.consumersumerrorcount = SettingJsonManager.consumersumerrorcount + inputModel.perrCount;
                        Console.WriteLine("consumerCount : " + SettingJsonManager.consumersumcount);
                        SettingJsonManager.SaveJsonFile();

                        Main.production = null;
                        inputModel.perrCount = 0;
                        inputModel.count = 0;
                        inputModel.pMemo = "";
                        inputModel.pworkCount = 0;
                        PopResultSearchMember();
                        Consumer.product.imgSource = "/img/bn_03.png";
                    }
                    else
                    {
                        MessageBox_Dialog box = new MessageBox_Dialog("저장 오류!!");
                        _ = box.ShowDialog();
                    }
                }
            }
        }

        //실수요자 결과 조회
        private int PopResultEndMember()
        {
            int ChkAED;
            if (inputModel.pworkCount == 0 && inputModel.perrCount == 0)
            {
                ChkAED = 3;
            }
            else
            {
                ChkAED = 1;
            }

            int workcount = int.Parse(tbResult.Text.ToString());
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
                DateTime.Now.ToString("yyyy-MM-dd"),
                inputModel.orderKey,
                ChkAED,
                inputModel.workMemberKey,
                workcount+"",
                inputModel.perrCount,

                 }, 1

              );

            if (returnData.결과 == 1)
            {
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);
                count = int.Parse(tbResult.Text.ToString());

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



        private void PopResultStartMember()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
              SettingJsonManager.serverIndex,
              "[PopResultStartMember]",
              new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey", "@pworkKey", "@vendorID", "@membID" },
              new object[] {
                  DateTime.Now.ToString("yyyy-MM-dd"),
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
                SettingJsonManager.pworkkey = result[0];
                SettingJsonManager.SaveJsonFile();
                inputModel.pworkKey = int.Parse(result[0]);
                inputModel.workMemberKey = int.Parse(result[1]);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Main.production = mainModel;

                Console.WriteLine("result[0]" + result[0]);
                Console.WriteLine("result[0]" + result[1]);

                switch (metrialitem)
                {

                    case "19": //마닐라
                    case "20": //원단
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
                            _ = msg.ShowDialog();
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
                            _ = msg.ShowDialog();
                        }
                        break;
                }
            }
        }

        private bool CheckConsumerWork()
        {
            bool check = true;
            for (int i = 0; i < Consumer.consumerList.Count; i++)
            {
                if (Consumer.consumerList[i].sDatetime.Length > 2 && Consumer.consumerList[i].eDatetime.Length <= 1)
                {
                    check = true;
                    break;
                }
                else
                {
                    check = false;
                }
            }
            return check;
        }

        //불량수량 저장버튼 클릭
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image img = (Image)sender;
                InputModel context = (InputModel)img.DataContext;

                if (context.errorModel.perrCount > 0 && !context.errorModel.memo.Equals(""))
                {
                    if (inputModel.pworkKey != 0)
                    {
                        if (PopErrorInsertMember() == 1)
                        {
                            context.errorModel.perrCount = 0;
                            PopResultSearchMember();
                        }
                        else
                        {
                            MessageBox_Dialog msg = new MessageBox_Dialog("불량 데이터 저장 오류 !!");
                            _ = msg.ShowDialog();
                        }
                    }
                }
            }
        }

        private int PopErrorInsertMember()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            int value;
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
                    value = (int)inputModel.pCnt;
                    break;

            }

            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopErrorInsertMember]",
                new string[] { "@workMemberKey", "@perrorCount", "@Memo" },
                new object[] { inputModel.workMemberKey, inputModel.errorModel.perrCount * value, inputModel.errorModel.memo },
                1
                );

            return returnData.결과 == 1 ? 1 : 0;
        }

        //불량수량 - 버튼
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

        //불량수량 + 버튼
        private void Result_badCount_plus(object sender, MouseButtonEventArgs e)
        {

            Image img = (Image)sender;
            InputModel context = (InputModel)img.DataContext;
            int value = 1;
            context.errorModel.perrCount += value;

        }

        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        //결과수량 - 버튼
        private void Result_count_minus(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image img = (Image)sender;
                InputModel context = (InputModel)img.DataContext;
                float value;
                if (SettingJsonManager.CountType == 0)
                {
                    value = 1;
                }
                else
                {
                    value = inputModel.pCnt;
                    if (SettingJsonManager.boxCheck)
                    {
                        value = 1;
                    }
                }
                if (context.pworkCount - value >= 0)
                {
                    inputModel.count -= value;
                }
            }
        }

        //결과수량 + 버튼
        private void Result_count_plus(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                float value;
                if (SettingJsonManager.CountType == 0)
                {
                    value = 1;
                }
                else
                {
                    value = inputModel.pCnt;
                    if (SettingJsonManager.boxCheck)
                    {
                        value = 1;
                    }
                }
                inputModel.count += value;
            }
        }

        //바코드
        private void TbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
        }

        //실시간 생산수량 텍스트 클릭
        private void TbResult_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        private void Win_input_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        //이월버튼 클릭
        private void 이월(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show(mainModel.prodName + " 품목을 이월 하시겠습니까 ?", "이월", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                Close();
            }
            else
            {
                if (consumerModel.pWorkCount > 0 || count > 0)
                {
                    SpAdd410processinfo_workplan();
                }
                else
                {
                    MessageBox_Dialog dialog = new MessageBox_Dialog("이월 작업을 할 수 없습니다.");
                    _ = dialog.ShowDialog();
                }
            }
            Console.WriteLine(mainModel.presultCount + "");
        }

        //이월계획 DB저장
        private void SpAdd410processinfo_workplan()
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
                _ = msg.ShowDialog();
                box1.Visibility = Visibility.Collapsed;
                Canvas.SetTop(box, 540);
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("spAdd410processinfo_workplan 조회 오류!!");
                _ = msg.ShowDialog();
            }
        }
    }
}
