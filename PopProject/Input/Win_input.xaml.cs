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

namespace PopProject.Input
{
    /// <summary>
    /// Win_input.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_input : Window
    {
        public static Socket mainSock;
        ObservableCollection<string> ob = new ObservableCollection<string>();
        List<string> ob1 = new List<string>();
        private ObservableCollection<string> errorOb;
        private readonly string metrialitem = Properties.Settings.Default.meterialinfo;
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

        public Win_input(MainModel mainModel, ObservableCollection<MainModel> workList, bool check)
        {
            InitializeComponent();
            if (Properties.Settings.Default.manager.Equals("1"))
            {
                Main.production = null;
                Main1.production = null;
            }
          
            this.workList = workList;
            inputModel = new InputModel();
            this.mainModel = mainModel;
            boxType.Text = "형 태 : " + mainModel.boxType;

            if (Properties.Settings.Default.corp_name.Equals("오주포장") || Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
            {
                SignalRAdministrator.허브.On<string, string>("returnCount", recive);
            }

            //for (int i = 0; i < ma inModel.subOb.Count; i++)
            //{
            //    if (Properties.Settings.Default.processType == mainModel.subOb[i].processType)
            //    {
            //        result = i != 0 ? mainModel.subOb[i - 1].presultCount : 0;
            //    }
            //}
            //    Pre_Count.Text = result.ToString();

            if (mainModel.presultCount > 0 && mainModel.sDatetime.Length > 3 && mainModel.eDatetime.Length > 3)
            {
                box1.Visibility = Visibility.Visible;
                Canvas.SetTop(box, 490);
                Canvas.SetTop(box1, 570);
                //Result_Update.Visibility = Visibility.Visible;
            }

            else
            {
                box1.Visibility = Visibility.Collapsed;
                Canvas.SetTop(box, 540);
            }

            ConvertModel();
            PopBeforeProcessItemCount();

            if (Properties.Settings.Default.keyinCheck)
            {
                tbResult.Focusable = true;
                _ = tbResult.Focus();
                _ = tbResult.Dispatcher.BeginInvoke(new Action(() => { tbResult.SelectAll(); }));
            }

            if (check)
            {
                if (Main.production == null && Main1.production == null)
                {
                    if (inputModel.sDatetime.Length < 3 && inputModel.eDatetime.Length < 3)
                    {
                        _ = PopResultStart();
                        PopResultSearch();
                    }
                }
            }
            if (Properties.Settings.Default.Plc)
            {
                Can_plc.Visibility = Visibility.Collapsed;
                plc_setting.Visibility = Visibility.Visible;
                //소켓 연결
                //ConnectSocket();

            }
            else
            {
                plc_setting.Visibility = Visibility.Collapsed;
                Can_plc.Visibility = Visibility.Visible;
            }
        }




        private void recive(string arg1, string arg2)  // 장비별 카운트값 수신
        {
            //Console.WriteLine(arg1 + " : " + arg2);
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
                                    if (inputModel.imagePath.Equals("/img/bn_06.png"))
                                    {
                                        if (arg1.Equals(Properties.Settings.Default.mercName))
                                        {
                                            inputModel.pworkCount = float.Parse(arg2);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    //MessageBox_Dialog msg = new MessageBox_Dialog(e.Message);
                                    //msg.Show();
                                }
                            }
                            break;

                    }
                }), DispatcherPriority.ApplicationIdle);
            }
            catch (Exception e)

            {
                //MessageBox_Dialog msg = new MessageBox_Dialog(e.Message);
                //msg.Show();
            }
        }


        private void ConnectSocket()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            if (!mainSock.Connected)
            {
                int port = int.Parse(Properties.Settings.Default.port);
                try
                {
                    IAsyncResult result = mainSock.BeginConnect(Properties.Settings.Default.ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(1500, true);
                    if (success)
                    {
                        mainSock.EndConnect(result);
                        AsyncObject obj = new AsyncObject(2000);
                        obj.WorkingSocket = mainSock;
                        obj.ClearBuffer();
                        mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                    }
                    else
                    {
                        mainSock.Close();
                        MessageBox_Dialog msg = new MessageBox_Dialog("Plc 연결 불가");
                        msg.ShowDialog();
                    }
                    // mainSock.Connect(Properties.Settings.Default.ip, port); //서버ip,포트번호

                }
                catch (Exception e)
                {
                    //MessageBox_Dialog msg = new MessageBox_Dialog(e.Message);
                    //msg.Show();
                }
            }
        }

        private void PopBeforeProcessItemCount()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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

            pCnt.Text = Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업")
                ? "(" + inputModel.pCnt + "구)"
                : "(" + inputModel.pCnt + "통)";

            //switch (Properties.Settings.Default.CountType)
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
                switch (metrialitem)
                {

                    case "12":
                    case "13":

                        if (Main1.production != null)
                        {
                            Main1.arduSerial.Write("B");
                        }

                        else
                        {
                            Main1.arduSerial.Write("A");
                            Main1.arduSerial.Write("B");
                        }

                        break;

                    default:

                        if (Main.production != null)
                        {
                            Main.arduSerial.Write("B");
                        }

                        else
                        {
                            Main.arduSerial.Write("A");
                            Main.arduSerial.Write("B");
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
                        case "12":
                        case "13":

                            if (Main1.production != null)
                            {
                                Main1.arduSerial.Write("B");
                            }
                            else
                            {
                                Main1.arduSerial.Write("A");
                                Main1.arduSerial.Write("B");
                            }
                            break;

                        default:
                            if (Main.production != null)
                            {
                                Main.arduSerial.Write("B");
                            }
                            else
                            {
                                Main.arduSerial.Write("A");
                                Main.arduSerial.Write("B");
                            }
                            break;
                    }
                }

                catch (Exception)
                {

                }
            }

            PopResultSearch();
            PopErrorSearch();
            TimerSet();

        }

        private void TimerSet()
        {

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(20)
            };
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _ = tbBarcode.Focus();
            tbBarcode.SelectAll();
        }

        // 불량 사유 리스트 조회
        private void PopErrorSearch()
        {
            _ = new LogData("불량사유조회");
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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

                    if (Properties.Settings.Default.error.Length == 0 && Properties.Settings.Default.error1.Length == 0)
                    {
                        combo.ItemsSource = ob;
                    }

                    else
                    {

                        string[] error = Properties.Settings.Default.error.Split(',');
                        string[] error1 = Properties.Settings.Default.error1.Split(',');

                        for (int i = 0; i < error.Length; i++)
                        {
                            if (Properties.Settings.Default.error.Length != 0)
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
                            if (Properties.Settings.Default.error1.Length != 0)
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
            }
        }

        private void PopResultSearch()
        {
            // Console.WriteLine("PopResultSearch 시작");
            totalPerrCount = 0;
            float tempCount = inputModel.count;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
                        if (result["mercName"].ToString().Equals(Properties.Settings.Default.mercName))
                        {
                            listitem.pworkDate = DateTime.Parse(result["pworkDate"].ToString());
                            listitem.pworkCount = float.Parse(result["pworkCount"].ToString());
                            listitem.pworkKey = int.Parse(result["pworkKey"].ToString());
                            listitem.sDatetime = result["sDatetime"].ToString();
                            listitem.eDatetime = !result["eDatetime"].ToString().Equals("") ? result["eDatetime"].ToString() : "";
                            listitem.orderKey = result["orderKey"].ToString();
                            listitem.pplanKey = int.Parse(result["pplanKey"].ToString());
                            listitem.ProcessType = int.Parse(result["ProcessType"].ToString());
                            listitem.perrCount = float.Parse(result["perrCount"].ToString());
                            listitem.pMemo = result["pMemo"].ToString();
                            listitem.errorModelOb = PopErrorListSearch(listitem);

                            if (!listitem.eDatetime.Equals(""))
                            {
                                errorModel item2 = new errorModel();
                                item2.pworkKey = listitem.pworkKey;
                                item2.memo = "작업완료";
                                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트"))
                                {
                                    if (Properties.Settings.Default.CountType == 1)
                                    {
                                        item2.perrCount = listitem.pworkCount * (1 / mainModel.pCnt);
                                    }
                                    else
                                    {
                                        item2.perrCount = listitem.pworkCount;
                                    }

                                }
                                else
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

                    if (ob.Count > 0)
                    {
                        InputModel tempItem = ob[ob.Count - 1];
                        if ((!tempItem.sDatetime.ToString().Equals("E")) && tempItem.eDatetime.ToString().Equals(""))
                        {
                            //Console.WriteLine("시작");
                            inputModel.imagePath = "/img/bn_06.png";
                            mainModel.imagePath = "/img/bn_01.png";
                            inputModel.pworkDate = tempItem.pworkDate;
                            inputModel.pworkKey = tempItem.pworkKey;
                            inputModel.orderKey = tempItem.orderKey;
                            inputModel.pplanKey = tempItem.pplanKey;
                            inputModel.ProcessType = tempItem.ProcessType;
                            inputModel.perrCount = tempItem.perrCount;
                            inputModel.count = tempCount;
                            inputModel.pMemo = tempItem.pMemo;
                            inputModel.sDatetime = tempItem.sDatetime;
                            inputModel.eDatetime = tempItem.eDatetime;
                        }

                        else
                        {
                            inputModel.imagePath = "/img/bn_05.png";
                            mainModel.imagePath = !tempItem.eDatetime.Equals("") ? "/img/bn_03.png" : "/img/bn_02.png";

                            inputModel.eDatetime = tempItem.eDatetime;
                            inputModel.pworkKey = tempItem.pworkKey;
                        }
                    }

                    if (Properties.Settings.Default.CountType == 1)
                    {
                        if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("우리포장산업"))
                        {
                            inputModel.sumPlanCount = (float)Math.Ceiling(inputModel.pplanCount / (float)inputModel.pCnt) - (inputModel.sumPworkCount / (float)inputModel.pCnt);

                        }
                        else
                        {
                            inputModel.sumPlanCount = inputModel.pplanCount - inputModel.sumPworkCount;

                        }

                    }
                    else
                    {
                        inputModel.sumPlanCount = inputModel.pplanCount - inputModel.sumPworkCount;
                    }

                    if (Properties.Settings.Default.boxCheck)
                    {
                        inputModel.sumPlanCount = inputModel.pplanCount - (inputModel.sumPworkCount / inputModel.pCnt);
                    }

                    //Console.WriteLine("ob.count : " + ob.Count.ToString());

                    if (ob.Count != 0)
                    {
                        try
                        {
                            // Console.WriteLine("ob.count sDatetime: " + ob[ob.Count - 1].sDatetime.ToString());
                            // Console.WriteLine("ob.count eDatetime: " + ob[ob.Count - 1].eDatetime.ToString());
                            // Console.WriteLine("ob.count imagePath: " + ob[ob.Count - 1].imagePath.ToString());
                        }
                        catch (Exception)
                        {

                        }
                    }

                    ic.ItemsSource = ob;

                }

                //inputModel.pworkCount = inputModel.sumPworkCount;
                txtTotalPerrCount.Text = totalPerrCount.ToString();
                perrcount.Text = totalPerrCount.ToString();
            }

            sv.ScrollToVerticalOffset(ic.ActualHeight);

        }

        private ObservableCollection<errorModel> PopErrorListSearch(InputModel inputModel)
        {
            ObservableCollection<errorModel> ob = null;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

                Properties.Settings.Default.serverIndex,
                "[PopErrorListSearch]",
                new string[] { "@pworkKey", "@ProcessType" },
                new object[] { inputModel.pworkKey, Properties.Settings.Default.processType }, 0
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
                            perrKey = result["perrKey"].ToString(),
                            memo = result["Memo"].ToString(),
                            perrCount = float.Parse(result["perrorCount"].ToString()),
                            updateTime = DateTime.Parse(result["updateTime"].ToString()),
                            fontColor = Brushes.Black
                        };
                        totalPerrCount += item.perrCount;
                        ob.Add(item);
                    }
                }
            }
            return ob;
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

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

        private void TbResult_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                if (context.Name.Equals("tbResult"))
                {
                    win_KeyBoard = new Win_KeyBoard(context, context.DataContext, inputModel)
                    {
                        Left = SystemParameters.WorkArea.Width - this.Width - 10,
                        Top = (SystemParameters.WorkArea.Height / 2) - this.Top
                    };
                    _ = win_KeyBoard.ShowDialog();
                    win_KeyBoard = null;
                }

                else
                {
                    if (Properties.Settings.Default.keyinCheck)
                    {
                        tbError.Focusable = true;
                        _ = tbError.Focus();
                        _ = tbError.Dispatcher.BeginInvoke(new Action(() => { tbError.SelectAll(); }));
                    }
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

                if (Properties.Settings.Default.CountType == 0)
                {
                    value = 1;
                }

                else
                {
                    value = Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩")
                        ? Properties.Settings.Default.twoSum ? (inputModel.pCnt * 2) : 1
                        : Properties.Settings.Default.twoSum ? (inputModel.pCnt * 2) : inputModel.pCnt;

                    if (Properties.Settings.Default.boxCheck)
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
                if (Properties.Settings.Default.CountType == 0)
                {
                    value = 1;
                }

                else
                {
                    if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩"))
                    {
                        value = Properties.Settings.Default.twoSum ? (inputModel.pCnt * 2) : 1;
                    }
                    else
                    {
                        value = Properties.Settings.Default.twoSum ? (inputModel.pCnt * 2) : inputModel.pCnt;

                    }

                    if (Properties.Settings.Default.boxCheck)
                    {
                        value = 1 * inputModel.pCnt;
                    }

                }

                inputModel.pworkCount += value;
                inputModel.count += value;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (e.ClickCount == 2)
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

                    else if (context.memo.Equals("작업완료"))
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

        private int PopInlineStart(SubModel subOb)
        {

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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

                Properties.Settings.Default.pworkkey = returnData.리턴값;
                Properties.Settings.Default.Save();
                //Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                InvokeCount(Properties.Settings.Default.mercName, "S");
                Properties.Settings.Default.count = 0;
                Properties.Settings.Default.saveDate = "";
                Properties.Settings.Default.pplanKey = 0;
                Properties.Settings.Default.Save();

                PopInlineEnd(subOb, int.Parse(returnData.리턴값));
                switch (metrialitem)
                {
                    case "12":
                    case "13":
                        Main1.production = mainModel;
                        try
                        {
                            if (Main1.production != null)
                            {
                                Main1.arduSerial.Write("A");
                                Main1.arduSerial.Write("B");
                            }
                            else
                            {
                                Main1.arduSerial.Write("A");
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
                                Main.arduSerial.Write("A");
                                Main.arduSerial.Write("B");
                            }

                            else
                            {
                                Main.arduSerial.Write("A");
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
            }
            return returnData.결과;

        }

        private void PopInlineEnd(SubModel subOb, int pWorkkey)
        {
            string 전체공정 = "0";
            if (Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (Properties.Settings.Default.mercName.Contains("인라인"))
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

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            string parameter = "[" + subOb.ProcessTypeName + "] " + this.inputModel.customSName + Environment.NewLine + inputModel.prodName + " " + inputModel.pworkCount.ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
            Properties.Settings.Default.consumersumcount = 0;
            Properties.Settings.Default.count = 0;
            Properties.Settings.Default.saveDate = "";
            Properties.Settings.Default.pplanKey = 0;
            Properties.Settings.Default.pworkkey = "";
            Properties.Settings.Default.Save();
        }

        //닫기 버튼 클릭
        private void Win_input_exit(object sender, MouseButtonEventArgs e)
        {
            //if (mainSock.Connected)
            //{
            //    mainSock.Close();
            //}
            Close();
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

            Win_Detail win_Detail = new Win_Detail(mainModel, special, workList, Properties.Settings.Default.detail_check);
            win_Detail.ShowDialog();
            //Consumer consumer = new Consumer();
            //consumer.ShowDialog();
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (inputModel.imagePath.Equals("/img/bn_05.png"))
            {
                string metrialitem = Properties.Settings.Default.meterialinfo;
                //Console.WriteLine("metrialitem -> " + metrialitem);
                switch (metrialitem)
                {
                    case "12":
                    case "13":
                        //if (!Properties.Settings.Default.corp_id.Equals("icdtest99"))
                        //{
                        if (Main1.production != null)
                        {
                            MessageBox_Dialog messageBox = new MessageBox_Dialog("작업중인 건이 있습니다.");
                            _ = messageBox.ShowDialog();
                        }
                        else
                        {
                            _ = new LogData("작업시작");
                            _ = PopResultStart();
                            PopResultSearch();
                        }
                        //}
                        //else
                        //{
                        //    LogData logData = new LogData("작업시작");
                        //    popResultStart();
                        //    popResultSearch();

                        //}

                        break;

                    default:
                        //if (!Properties.Settings.Default.corp_id.Equals("icdtest99"))
                        //{

                        if (Main.production != null)
                        {
                            MessageBox_Dialog messageBox = new MessageBox_Dialog("작업중인 건이 있습니다.");
                            _ = messageBox.ShowDialog();

                        }


                        else
                        {
                            _ = new LogData("작업시작");
                            _ = PopResultStart();
                            PopResultSearch();
                        }

                        //}
                        //else
                        //{
                        //    LogData logData = new LogData("작업시작");
                        //    popResultStart();
                        //    popResultSearch();

                        //}
                        break;
                }

            }

            else if (inputModel.imagePath.Equals("/img/bn_06.png"))
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("작업을 저장 하시겠습니까 ?", "작업 저장",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    Close();
                }

                else
                {
                    LogData logData = new LogData("작업종료");
                    PopResultEnd();
                    CNSDataRead();



                    inputModel.perrCount = 0;
                    inputModel.count = 0;
                    inputModel.pMemo = "";
                    inputModel.pworkCount = 0;
                    PopResultSearch();
                }
            }

        }


        bool cnsProcessing = false;

        private void CNSDataRead() //데이터 수신 요청
        {
            if (Properties.Settings.Default.Plc) //PLC 사용 체크일때
            {
                try
                {
                    ConnectSocket(); //통신 오픈
                    Ls_Fenet wd = new Ls_Fenet(); //헤더
                    byte[] bDts = wd.Word_continuous_Read("%DB7804", 71); //연속읽기 D3902 ~ D3972 (71개)

                    //송신한 데이터 값 보는용도
                    string Send_Data = string.Empty;
                    for (int i = 0; i < bDts.Length; i++)
                    {
                        Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    }
                    Console.WriteLine("PLC송신 데이터 : " + Send_Data); //송신값 확인
                    cnsProcessing = true;
                    mainSock.Send(bDts); //데이터 쓰기
                }
                catch (Exception)
                {
                }
            }
        }

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
                for (int i = 0; i < RD.Length; i++)
                {
                    string high_data = RD.Substring(0, 2); //앞 두자리 추출
                    string low_data = RD.Substring(2, 2); //뒤 두자리 추출

                    switch (i)
                    {
                        case 0:
                            itemModel.ModelNum = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 1:
                            itemModel.WorkPlan1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 2:
                            itemModel.WorkPlan2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 3:
                            itemModel.BoxType = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 4:
                            itemModel.Pock = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 5:
                            itemModel.Jang = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 6:
                            itemModel.Go = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 7:
                            itemModel.Gap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 8:
                            itemModel.Wing = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 9:
                            itemModel.Cutting = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 10:
                            itemModel.SaveDone = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 11:
                            itemModel.WorkSpeed = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 12:
                            itemModel.SaveState = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 13:
                            itemModel.spare = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 14:
                            itemModel.aRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 15:
                            itemModel.bRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 16:
                            itemModel.cRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 17:
                            itemModel.dRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 18:
                            itemModel.eRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 19:
                            itemModel.f1Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 20:
                            itemModel.f2Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 21:
                            itemModel.g2pRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 22:
                            itemModel.oRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 23:
                            itemModel.p1Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 24:
                            itemModel.p2pRecipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 25:
                            itemModel.p3Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 26:
                            itemModel.p4Recipe = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 27:
                            itemModel.spare2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 28:
                            itemModel.spare3 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 29:
                            itemModel.Slotter = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 30:
                            itemModel.DieCutter = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 31:
                            itemModel.PrinterPull1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 32:
                            itemModel.PrinterRoll1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 33:
                            itemModel.PrinterPull2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 34:
                            itemModel.PrinterRoll2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 35:
                            itemModel.PrinterPull3 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 36:
                            itemModel.PrinterRoll3 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 37:
                            itemModel.PrinterPull4 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 38:
                            itemModel.PrinterRoll4 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 39:
                            itemModel.spare4 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 40:
                            itemModel.spare5 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 41:
                            itemModel.spare6 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 42:
                            itemModel.spare7 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 43:
                            itemModel.FeederGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 44:
                            itemModel.FeederRoll = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 45:
                            itemModel.SlotterSubGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 46:
                            itemModel.SlotterMiddleGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 47:
                            itemModel.SlotterConnection = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 48:
                            itemModel.DieCutterRollGap = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 49:
                            itemModel.GluerbeltF1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 50:
                            itemModel.GluerbeltF2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 51:
                            itemModel.GluerPressA = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 52:
                            itemModel.GluerPressB = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 53:
                            itemModel.DieCutterRollA = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 54:
                            itemModel.DieCutterRollB = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 55:
                            itemModel.GluerPE = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 56:
                            itemModel.BRSub = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 57:
                            itemModel.BRMain = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 58:
                            itemModel.BRConnection = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 59:
                            itemModel.BRmiddle = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 60:
                            itemModel.spare8 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 61:
                            itemModel.spare9 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 62:
                            itemModel.spare10 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 63:
                            itemModel.spare11 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 64:
                            itemModel.spare12 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 65:
                            itemModel.spare13 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 66:
                            itemModel.spare14 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 67:
                            itemModel.TotalCount1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 68:
                            itemModel.TotalCount2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 69:
                            itemModel.Count1 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;
                        case 70:
                            itemModel.Count2 = Convert.ToInt32(low_data + high_data, 16).ToString();
                            break;

                        default:
                            break;
                    }
                    continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                    RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                }

                Console.WriteLine("PLC수신 데이터 : " + continuous_read); //수신값 확인


                if (cnsProcessing)
                {
                    ReturnModel returnData = ServiceModel.GetData(
                        Properties.Settings.Default.serverIndex,
                        "[PopCNSUpdate]",
                        new string[] { "@prodID", "@MachineName", "@ModelNum", "@WorkPlan1", "@WorkPlan2", "@BoxType", "@Pock", "@Jang", "@Go",
                                   "@Gap", "@Wing", "@Cutting", "@SaveDone", "@WorkSpeed", "@SaveState", "@spare", "@aRecipe", "@bRecipe",
                                   "@cRecipe", "@dRecipe", "@eRecipe", "@f1Recipe", "@f2Recipe", "@g2pRecipe", "@oRecipe", "@p1Recipe", "@p2pRecipe",
                                   "@p3Recipe", "@p4Recipe", "@spare2", "@spare3", "@Slotter", "@DieCutter", "@PrinterPull1", "@PrinterRoll1",
                                   "@PrinterPull2", "@PrinterRoll2", "@PrinterPull3", "@PrinterRoll3", "@PrinterPull4", "@PrinterRoll4", "@spare4",
                                   "@spare5", "@spare6", "@spare7", "@FeederGap", "@FeederRoll", "@SlotterSubGap", "@SlotterMiddleGap", "@SlotterConnection",
                                   "@DieCutterRollGap", "@GluerbeltF1", "@GluerbeltF2", "@GluerPressA", "@GluerPressB", "@DieCutterRollA", "@DieCutterRollB",
                                   "@GluerPE", "@BRSub", "@BRMain", "@BRConnection", "@BRmiddle", "@spare8", "@spare9", "@spare10", "@spare11", "@spare12",
                                   "@spare13", "@spare14", "@TotalCount1", "@TotalCount2", "@Count1", "@Count2" },
                        new object[] { this.mainModel.prodID, Properties.Settings.Default.mercName, itemModel.ModelNum, itemModel.WorkPlan1, itemModel.WorkPlan2, itemModel.BoxType,
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

                }

                obj.ClearBuffer();
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

        private int PopResultEnd()
        {
            string 전체공정 = "0";
            if (Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (Properties.Settings.Default.mercName.Contains("인라인"))
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
            inputModel.pworkCount = int.Parse(tbResult.Text.ToString());
            string arrChkAED = inputModel.pworkCount == 0 && inputModel.perrCount == 0 ? "3|" : "1|";
            if (Properties.Settings.Default.boxCheck)
            {
                inputModel.pworkCount = (int)(this.inputModel.pworkCount * this.inputModel.pCnt);
            }
            if (Properties.Settings.Default.CountType == 1)
            {
                if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("누리팩") || Properties.Settings.Default.corp_name.Equals("우리포장산업"))
                {
                    inputModel.pworkCount = (int)(this.inputModel.pworkCount * this.inputModel.pCnt);
                }

            }

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            string parameter = "[" + Properties.Settings.Default.mercName + "] " + this.inputModel.customSName + Environment.NewLine + inputModel.prodName + " " + inputModel.pworkCount.ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
                Properties.Settings.Default.mercName,
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
                inputModel.imagePath = "/img/bn_05.png";
                mainModel.imagePath = "/img/bn_02.png";
                mainModel.sumPworkCount = inputModel.pworkCount + inputModel.perrCount;

                mainModel.presultCount = inputModel.pworkCount + inputModel.perrCount;
                mainModel.presultCount1 = inputModel.pworkCount + inputModel.perrCount;

                mainModel.perrorCount = (int)inputModel.perrCount;
                for (int i = 0; i < mainModel.subOb.Count; i++)
                {
                    if (Properties.Settings.Default.processType == mainModel.subOb[i].processType)
                    {
                        mainModel.subOb[i].stateColor = Brushes.Black;
                        mainModel.subOb[i].stateFontColor = Brushes.White;
                    }
                }



                InvokeCount(Properties.Settings.Default.mercName, "E");

                //Console.WriteLine("metrialitem -> " + metrialitem);
                if (Properties.Settings.Default.corp_name.Equals("오주포장")  &&
                    (Properties.Settings.Default.processType == 7
                    || Properties.Settings.Default.processType == 5
                    || Properties.Settings.Default.processType == 8
                    || Properties.Settings.Default.processType == 13

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
                if (Properties.Settings.Default.corp_name.Equals("성주포장") && Properties.Settings.Default.mercName.Equals("후렉소") &&
                    (Properties.Settings.Default.processType == 1))
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
                if(Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
                {
                    //if (Properties.Settings.Default.mercName.Contains("인라인"))
                    //{
                    //    if (mainModel.subOb.Count == 2)
                    //    {
                    //        PopInlineStart(mainModel.subOb[mainModel.subOb.Count - 1]);
                    //    }
                       
                    //}
                }


                switch (metrialitem)
                {
                    case "12":
                    case "13":
                        Main1.production = null;

                        try
                        {
                            Main1.arduSerial.Write("A"); // 카운트값 초기화
                            Main1.arduSerial.Write("B"); // 
                        }
                        catch (Exception)
                        {

                        }
                        break;

                    default:
                        Main.production = null;
                        try
                        {
                            Main.arduSerial.Write("A");
                            Main.arduSerial.Write("B");
                        }

                        catch (Exception e)
                        {
                            //MessageBox_Dialog msg = new MessageBox_Dialog(e.Message);
                            //msg.Show();
                        }
                        break;
                }



                Properties.Settings.Default.consumersumcount = 0;
                Properties.Settings.Default.count = 0;
                Properties.Settings.Default.saveDate = "";
                Properties.Settings.Default.pplanKey = 0;
                Properties.Settings.Default.pworkkey = "";
                Properties.Settings.Default.Save();
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopResultEnd 저장실패 !!");
                msg.ShowDialog();
            }

            return returnData.결과;

        }

        private int PopResultStart()
        {

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopResultStart]",
                new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey" },
                new object[] {
                DateTime.Now.ToString("yyyy-MM-dd"),
                inputModel.orderKey,
                inputModel.pplanKey,
                inputModel.ProcessType,
                Properties.Settings.Default.mercName,
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
                Properties.Settings.Default.pworkkey = returnData.리턴값;
                Properties.Settings.Default.Save();
                //Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                InvokeCount(Properties.Settings.Default.mercName, "S");
                Properties.Settings.Default.count = 0;
                Properties.Settings.Default.saveDate = "";
                Properties.Settings.Default.pplanKey = 0;
                Properties.Settings.Default.Save();

                switch (metrialitem)
                {
                    case "12":
                    case "13":
                        Main1.production = mainModel;
                        try
                        {
                            if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트")) 
                            {
                                
                            }
                            else
                            {
                                if (Main1.production != null)
                                {
                                    Main1.arduSerial.Write("A");
                                    Main1.arduSerial.Write("B");
                                }

                                else
                                {
                                    Main1.arduSerial.Write("A");
                                }

                            }         
                        }
                        catch (Exception)
                        {
                            //MessageBox_Dialog msg = new MessageBox_Dialog("PopResultStart 오류발생 !!" + e.Message);
                            //msg.ShowDialog();
                        }
                        break;

                    default:

                        Main.production = mainModel;

                        try
                        {
                            if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트"))
                            {
                               
                            }
                            else
                            {
                                if (Main.production != null)
                                {
                                    Main.arduSerial.Write("A");
                                    Main.arduSerial.Write("B");
                                }

                                else
                                {
                                    Main.arduSerial.Write("A");
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

                if (Properties.Settings.Default.corp_name.Equals("대양포장") || Properties.Settings.Default.corp_name.Equals("오주포장")|| Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                {
                    try
                    {
                        string[] psize;
                        psize = mainModel.pSize.Split('*');
                        Properties.Settings.Default.mimi = 35;
                        Properties.Settings.Default.jang = int.Parse(psize[0].Trim());
                        Properties.Settings.Default.pok = int.Parse(psize[1].Trim());
                        Properties.Settings.Default.go = int.Parse(psize[2].Trim());
                        Properties.Settings.Default.setCount = (int)mainModel.pplanCount;
                        switch (mainModel.paperGor.Replace("골", "").Trim())
                        {
                            case "AB":
                            case "BA":
                                Properties.Settings.Default.Gap = 70;
                                break;

                            case "BB":
                                Properties.Settings.Default.Gap = 60;
                                break;

                            case "A":
                                Properties.Settings.Default.Gap = 50;
                                break;

                            case "EB":
                                Properties.Settings.Default.Gap = 40;
                                break;

                            case "B":
                                Properties.Settings.Default.Gap = 30;
                                break;

                            case "E":
                                Properties.Settings.Default.Gap = 20;
                                break;
                            default:
                                break;
                        }
                        //Properties.Settings.Default.setCount = (int)mainModel.pplanCount + (int)(mainModel.pplanCount * 0.1);
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception)
                    {
                        Properties.Settings.Default.mimi = 35;
                        Properties.Settings.Default.jang = 0;
                        Properties.Settings.Default.pok = 0;
                        Properties.Settings.Default.go = 0;
                        Properties.Settings.Default.setCount = 99999;
                        Properties.Settings.Default.Gap = 7;
                        Properties.Settings.Default.Save();
                    }
                    finally
                    {

                        if (Properties.Settings.Default.Plc)
                        {
                            try
                            {
                                ConnectSocket();
                                Ls_Fenet wd = new Ls_Fenet();
                                //byte[] bDts = wd.continuous_Read("%DB7800", 70);
                                List<int> items = new List<int>();
                                items.Add(99);  //설정 변경요구 (99로 요청하면 1로바뀜)
                                items.Add(0);   //설정 byte[] bDts = wd.continuous_Read("%DB7800", 70); 요구 (0=> 신규설정요구, 1=>데이터 설정 변경요구
                                items.Add(1); //작업 모델 번호
                                items.Add(10000); //작업지시 매수(하위)
                                items.Add(0); //작업지시 매수(상위)
                                items.Add(Properties.Settings.Default.boxType); //BOX TYPE (1=>1합, 3=>2합, 5=>인쇄물, 6=>H형, 7=>H-Cover형 
                                items.Add(Properties.Settings.Default.pok); //폭
                                items.Add(Properties.Settings.Default.jang); //장
                                items.Add(Properties.Settings.Default.go); //고
                                items.Add(Properties.Settings.Default.Gap); //갭
                                items.Add(0); //앞날개
                                items.Add(Properties.Settings.Default.mimi); //갭

                                //
                                int WriteCount = items.Count() * 2;
                                byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items);
                                //송신한 데이터 값 보는용도
                                string Send_Data = string.Empty;

                                for (int i = 0; i < bDts.Length; i++)
                                {
                                    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                                }

                                Console.WriteLine("송신 데이터 : " + Send_Data);
                                mainSock.Send(bDts); //데이터 쓰기

                            }

                            catch (Exception)
                            {

                            }
                            finally
                            {
                                mainSock.Close();
                            }

                        }


                    }
                }

            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;
        }

        private void InvokeCount(string mercName, string v)
        {
            //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
            try
            {
                SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);

            }
            catch (Exception e)
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
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            float value;
            switch (Properties.Settings.Default.processType)
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
                Properties.Settings.Default.serverIndex,
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
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,

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
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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

        private void 이월(object sender, MouseButtonEventArgs e)
        {


            var result = System.Windows.MessageBox.Show(mainModel.prodName + " 품목을 이월 하시겠습니까 ?", "이월",
                    MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                Close();
            }

            else
            {
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
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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

        private void Plc_button(object sender, MouseButtonEventArgs e)
        {
            Plc_Pop plc = new Plc_Pop(mainModel);
            //ConnectSocket();

            plc.ShowDialog();
            //if (Properties.Settings.Default.corp_name.Equals("대양포장"))
            //{
            //    if (Properties.Settings.Default.plcCheck)
            //    {
            //        try
            //        {
            //            ConnectSocket();
            //            Ls_Fenet wd = new Ls_Fenet();
            //            List<int> items = new List<int>();
            //            items.Add(99);  //설정 변경요구 (99로 요청하면 1로바뀜)
            //            items.Add(0);   //설정 저장 요구 (0=> 신규설정요구, 1=>데이터 설정 변경요구
            //            items.Add(1); //작업 모델 번호
            //            items.Add(Properties.Settings.Default.setCount); //작업지시 매수(하위)
            //            items.Add(0); //작업지시 매수(상위)
            //            items.Add(Properties.Settings.Default.boxType); //BOX TYPE (1=>1합, 3=>2합, 5=>인쇄물, 6=>H형, 7=>H-Cover형 
            //            items.Add(Properties.Settings.Default.pok); //폭
            //            items.Add(Properties.Settings.Default.jang); //장
            //            items.Add(Properties.Settings.Default.go); //고
            //            items.Add(Properties.Settings.Default.Gap); //갭

            //            int WriteCount = items.Count() * 2;
            //            byte[] bDts = wd.continuous_Write("%DB7600", WriteCount, items);
            //            //송신한 데이터 값 보는용도
            //            string Send_Data = string.Empty;

            //            for (int i = 0; i < bDts.Length; i++)
            //            {
            //                Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
            //            }

            //            Console.WriteLine("송신 데이터 : " + Send_Data);
            //            mainSock.Send(bDts); //데이터 쓰기

            //        }

            //        catch (Exception)
            //        {

            //        }
            //        finally
            //        {
            //            mainSock.Close();
            //        }

            //    }
            //}

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