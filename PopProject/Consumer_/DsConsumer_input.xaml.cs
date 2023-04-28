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
using System.Linq;
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
    
        public DsConsumer_input(ConsumerModel consumerModel, MainModel mainModel)
        {
            InitializeComponent();
            this.consumerModel = consumerModel;
            this.mainModel = mainModel;
            inputModel = new InputModel();

            if (consumerModel.pWorkCount > 0 && consumerModel.sDatetime.Length > 3 && consumerModel.eDatetime.Length > 3)
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
            ConvertModel();
            PopBeforeProcessItemCount();
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
            inputModel.prodName = mainModel.prodName + " \n(" + consumerModel.vendorName + "/" + consumerModel.membName + ")";


            mainModel.inputModel = inputModel;
            DataContext = inputModel;

            PopResultSearchMember();
            PopErrorSearch();
            TimerSet();

        }
        private void TimerSet()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            second++;
            if(second == 10)
            {
                mainModel.inputModel = inputModel;
                Main.production = mainModel;
                second = 0;
            }
           
        }

        private void PopErrorSearch()
        {
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

        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkChangeButton)
            {
                changeButtonIn.Visibility = Visibility.Collapsed;
                changeButtonOut.Visibility = Visibility.Visible;
                checkChangeButton = false;
                Width = 550;
                Left =
                    (SystemParameters.WorkArea.Width / 2) - (Width / 2);
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

        private void TbBarcode_KeyDown(object sender, KeyEventArgs e)
        {

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

        private void Result_count_plus(object sender, MouseButtonEventArgs e)
        {
            float value = 1;
            inputModel.count += value;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 시작 버튼 일 경우
            if (inputModel.imagePath.Equals("/img/bn_05.png"))
            {
                // 실수요자 작업 건 이 있을 경우
                if (CheckConsumerWork())
                {
                    MessageBox_Dialog messageBox = new MessageBox_Dialog("실수요자 작업중인 건이 있습니다.");
                    _ = messageBox.ShowDialog();
                }
                else
                {
                    try
                    {
                        Main.arduSerial.Write("B");
                    }catch(Exception e1)
                    {

                    }
                   
                    PopResultStartMember();
                    PopResultSearchMember();
                }
            }
            // 종료 버튼 일 경우
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
                    string metrialitem = Properties.Settings.Default.meterialinfo;
                    Console.WriteLine("end_count" + inputModel.count);
                    if (PopResultEndMember() == 1)
                    {
                        Properties.Settings.Default.consumersumcount = Properties.Settings.Default.consumersumcount + inputModel.pworkCount;
                        Properties.Settings.Default.consumersumerrorcount = Properties.Settings.Default.consumersumerrorcount + inputModel.perrCount;
                        Console.WriteLine("consumerCount : " + Properties.Settings.Default.consumersumcount);
                        Properties.Settings.Default.Save();

                        Main.production = null;
                        inputModel.perrCount = 0;
                        inputModel.count = 0;
                        inputModel.pMemo = "";
                        inputModel.pworkCount = 0;
                        PopResultSearchMember();
                        DSConsumer.product.imgSource = "/img/bn_03.png";
                        try
                        {
                            Main.arduSerial.Write("A");
                        }catch(Exception ardu)
                        {
//                            MessageBox_Dialog msg = new MessageBox_Dialog("아두이노 전송 에러")
                        }

                        if(Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
                        {
                            if (Properties.Settings.Default.mercName.Contains("인라인"))
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

        private int PopResultEndMember()
        {
            int count = 0;
            int ChkAED;
            if (inputModel.pworkCount == 0 && inputModel.perrCount == 0)
            {
                ChkAED = 3;
            }
            else
            {
                ChkAED = 1;
            }
            inlineCount = int.Parse(tbResult.Text.ToString());
            int workcount = int.Parse(tbResult.Text.ToString());
            if (Properties.Settings.Default.boxCheck)
            {
                workcount *= (int)mainModel.pCnt;
            }

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
                Properties.Settings.Default.consumCount += workcount;
                Properties.Settings.Default.Save();
                InvokeCount(Properties.Settings.Default.mercName, "E");
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


        private int PopResultInlineEndMember()
        {
            int count = 0;
            int ChkAED;
            if (inlineCount == 0 && inputModel.perrCount == 0)
            {
                ChkAED = 3;
            }
            else
            {
                ChkAED = 1;
            }

            int workcount = int.Parse(tbResult.Text.ToString());
            if (Properties.Settings.Default.boxCheck)
            {
                workcount *= (int)mainModel.pCnt;
            }

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopResultEndMember]",
                new string[] { "@작업일자", "@오더Key", "@ChkAED", "@실수요자생산Key", "@작업량", "@불량" },
                new object[] {
                DateTime.Now.ToString("yyyy-MM-dd"),
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
                InvokeCount(Properties.Settings.Default.mercName, "E");
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
        private void PopResultInlineStartMember(SubModel subOb)
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
              Properties.Settings.Default.serverIndex,
              "[PopResultStartMember]",
              new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey", "@pworkKey", "@vendorID", "@membID" },
              new object[] {
                  DateTime.Now.ToString("yyyy-MM-dd"),
                  consumerModel.orderKey,
                  inputModel.pplanKey,
                  subOb.processType,
                  subOb.ProcessTypeName,
                  MainWindow.userID,
                  MainWindow.userName,
                  inputModel.workerName,
                  inputModel.workerId,
                  inputModel.SCMKey,
                  Properties.Settings.Default.pworkkey,
                  consumerModel.vendorID,
                  consumerModel.membID
              }, 1

            );

            if (returnData.결과 == 1)
            {
                string[] result = returnData.리턴값.Split(new string[] { "|" }, StringSplitOptions.None);
                Properties.Settings.Default.consumpWorkKey = int.Parse(result[0]);
                //Properties.Settings.Default.pworkkey = result[0];
                Properties.Settings.Default.Save();
                inputModel.pworkKey = int.Parse(result[0]);
                inputModel.workMemberKey = int.Parse(result[1]);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Main.production = mainModel;

                Console.WriteLine("result[0]" + result[0]);
                Console.WriteLine("result[0]" + result[1]);
                PopResultInlineEndMember();
                //InvokeCount(Properties.Settings.Default.mercName, "S");

                //if (Main.production != null)
                //{
                //    Main.arduSerial.Write("A");
                //    Main.arduSerial.Write("B");
                //}

                //else
                //{
                //    Main.arduSerial.Write("A");
                //}



            }
        }

        private void PopResultStartMember()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
              Properties.Settings.Default.serverIndex,
              "[PopResultStartMember]",
              new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey", "@pworkKey", "@vendorID", "@membID" },
              new object[] {
                  DateTime.Now.ToString("yyyy-MM-dd"),
                  consumerModel.orderKey,
                  inputModel.pplanKey,
                  Properties.Settings.Default.processType,
                  Properties.Settings.Default.mercName,
                  MainWindow.userID,
                  MainWindow.userName,
                  inputModel.workerName,
                  inputModel.workerId,
                  inputModel.SCMKey,
                  Properties.Settings.Default.pworkkey,
                  consumerModel.vendorID,
                  consumerModel.membID
              }, 1

            );

            if (returnData.결과 == 1)
            {
                string[] result = returnData.리턴값.Split(new string[] { "|" }, StringSplitOptions.None);
                Properties.Settings.Default.consumpWorkKey = int.Parse(result[0]);
                //Properties.Settings.Default.pworkkey = result[0];
                Properties.Settings.Default.Save();
                inputModel.pworkKey = int.Parse(result[0]);
                inputModel.workMemberKey = int.Parse(result[1]);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Main.production = mainModel;

                Console.WriteLine("result[0]" + result[0]);
                Console.WriteLine("result[0]" + result[1]);

                //InvokeCount(Properties.Settings.Default.mercName, "S");

                //if (Main.production != null)
                //{
                //    Main.arduSerial.Write("A");
                //    Main.arduSerial.Write("B");
                //}

                //else
                //{
                //    Main.arduSerial.Write("A");
                //}



            }
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
        private ObservableCollection<errorModel> PopErrorListSearchMember(ConsumerModel listitem)
        {
            ObservableCollection<errorModel> ob = null;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               Properties.Settings.Default.serverIndex,
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
        private void PopResultSearchMember()
        {
            float tempCount = inputModel.count;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
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
                        if (consumerModel.membID.Equals(result["membID"].ToString()) && Properties.Settings.Default.processType.Equals(listitem.ProcessType))
                        {
                            listitem.errorModelOb = PopErrorListSearchMember(listitem);
                        }
                        

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
                            if (Properties.Settings.Default.boxCheck)
                            {
                                item2.perrCount /= mainModel.pCnt;
                            }
                            if (consumerModel.membID.Equals(result["membID"].ToString()) && Properties.Settings.Default.processType.Equals(listitem.ProcessType))
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
                        if (consumerModel.membID.Equals(result["membID"].ToString()) && Properties.Settings.Default.processType.Equals(listitem.ProcessType))
                        {
                            ob.Add(listitem);
                        }

                       

                    }

                    if (ob.Count > 0)
                    {

                        ConsumerModel tempItem = ob[ob.Count - 1];

                        if (tempItem.sDatetime.ToString().Length > 3 && tempItem.eDatetime.ToString().Length <= 1)
                        {
                            inputModel.imagePath = "/img/bn_06.png";
                            DSConsumer.product.imgSource = "/img/bn_01.png";
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

                                DSConsumer.product.imgSource = "/img/bn_03.png";
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

        private bool CheckConsumerWork()
        {
            bool check = false;
            for (int i = 0; i < DSConsumer.consumerList.Count; i++)
            {
                if (DSConsumer.consumerList[i].sDatetime.Length > 2 && DSConsumer.consumerList[i].eDatetime.Length <= 1)
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

        private void Win_input_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Detail_item(object sender, MouseButtonEventArgs e)
        {
            string special = "";

            

            Win_Detail win_Detail = new Win_Detail(mainModel, special,consumerModel, Properties.Settings.Default.detail_check,true);
            win_Detail.ShowDialog();
        }

        private void 이월(object sender, MouseButtonEventArgs e)
        {

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
                        PopResultSearchMember();
                        can.Visibility = Visibility.Collapsed;


                    }
                }
            }
        }
    }
}
