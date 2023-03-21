using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// DSConsumer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DSConsumer : Window
    {
        private MainModel mainModel;
        public static ObservableCollection<ConsumerModel> consumerList;
        private double hOff = 1;
        private double vOff = 1;
        private Point scrollMousePoint = new Point();
        private DsConsumer_input dsConsumer_Input = null;
        public static ConsumerModel product;
        private float sumPworkCount = 0;
        int beforeCount = 0;
        int consumbefore = 0;
        bool beforeCountCheck = true;
        string imgpath = "";
        private bool checkProcessing = false;
        private DispatcherTimer timer;
        private int second = 0;
        public DSConsumer(MainModel mainModel)

        {
            InitializeComponent();
            this.mainModel = mainModel;
            imgpath = mainModel.imagePath;
            sumPworkCount = mainModel.sumPworkCount;
            PopWorkListSearch();

            //PopResultStart();
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



        private void PopWorkListSearch()
        {
            checkProcessing = false;
            //consumerSumCount = 0;
            consumbefore = 0;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               Properties.Settings.Default.serverIndex,
              "[PopWorkListMember]",
               new string[] { "@orderKey", "@ProcessType", "@pplanDate" },
               new object[] { mainModel.orderKey, mainModel.ProcessType, DateTime.Now.ToString("yyyy-MM-dd") },

               0
               );

            if (returnData.결과 == 1)
            {

                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    consumerList = new ObservableCollection<ConsumerModel>();
                    foreach (JObject result in Data_Jarry)
                    {
                        ConsumerModel listitem = new ConsumerModel
                        {
                            orderKey = result["orderKey"].ToString(),
                            pworkKey = int.Parse(result["pworkKey"].ToString()),
                            orderDate = result["orderDate"].ToString(),
                            vendorID = result["vendorID"].ToString(),
                            vendorName = result["vendorName"].ToString(),
                            NoKey = result["NoKey"].ToString(),
                            membID = result["membID"].ToString(),
                            membName = result["membName"].ToString(),
                            adress = result["adress"].ToString(),
                            PhonNo = result["PhonNo"].ToString(),
                            orderCount = int.Parse(result["orderCount"].ToString()),
                            pworkDate = DateTime.Parse(result["pworkDate"].ToString())
                        };


                        listitem.pWorkCount = float.Parse(result["pworkCount"].ToString());
                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.orderCount /= (int)mainModel.pCnt;
                            listitem.pWorkCount /= (int)mainModel.pCnt;
                        }
                        //consumerSumCount += Properties.Settings.Default.pworkkey == listitem.pworkKey.ToString() ? listitem.pWorkCount : 0;
                        //Properties.Settings.Default.consumersumcount = Properties.Settings.Default.consumersumcount + consumerSumCount;
                        listitem.sDatetime = result["sDatetime"].ToString();
                        listitem.eDatetime = result["eDatetime"].ToString();
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());
                        //consumerSumErrorCount += Properties.Settings.Default.pworkkey == listitem.pworkKey.ToString() ? listitem.perrCnt : 0;
                        listitem.Memo = result["Memo"].ToString();


                        // 금일 실수요자 생산 리스트, 이월 실수요자 리스트만 보이도록 / 전날 생산 건은 삭제  
                        //if(listitem.pworkDate.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd")) || listitem.pworkDate.ToString("yyyyMMdd").Equals("19000101"))
                        //{
                            

                        //}
                        consumerList.Add(listitem);



                    }
                    for(int i = 0; i < consumerList.Count; i++)
                    {
                        if(consumerList[i].sDatetime.Length > 1 && consumerList[i].eDatetime.Length <= 1)
                        {
                            checkProcessing = true;
                            break;
                        }
                    }

                    for (int i = 0; i < consumerList.Count; i++)
                    {
                        consumbefore += (int)Math.Ceiling(float.Parse(consumerList[i].pWorkCount.ToString()));
                        if (consumerList[i].pworkKey != 0 && (imgpath.Equals("/img/bn_03.png") || imgpath.Equals("/img/bn_06.png")))
                        {
                            Properties.Settings.Default.pworkkey = consumerList[i].pworkKey.ToString();
                            Properties.Settings.Default.Save();

                        }
                    }




                    Console.WriteLine("consumerList.Count : " + consumerList.Count);
                    if (consumerList.Count > 0)
                    {
                        ic.ItemsSource = consumerList;
                        txtNoPlan.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtNoPlan.Visibility = Visibility.Visible;
                    }
                }
            }

            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("PopWorkListMember 조회 오류!!");
                _ = box.ShowDialog();
            }

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;

                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            }

            catch (Exception h)
            {
                string message = h.Message;
                Console.WriteLine("Exception message : " + message);
                // String message = h.Message();
            }

        }

        private void Program_exit(object sender, MouseButtonEventArgs e)
        {

            MessageBoxResult result = System.Windows.MessageBox.Show("작업을 저장 하시겠습니까 ?", "작업 저장",
                    MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                
            }
            else
            {
                if (checkProcessing)
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("생산 작업중인 건이 있습니다.");
                    msg.ShowDialog();
                }
                else
                {
                    //PopWorkListSearch();
                    int totalCount = getCount();
                    totalCount -= beforeCount;
                    int totalErrorCount = getErrorCount();
                    string arrChkAED = "";
                    if (Properties.Settings.Default.boxCheck)
                    {
                        totalCount *= int.Parse(mainModel.pCnt.ToString());


                    }
                    if (getCount() == 0)
                    {
                        arrChkAED = "3|";
                    }
                    else
                    {
                        if (mainModel.imagePath.Equals("/img/bn_02.png"))
                        {
                            arrChkAED = "1|";
                        }
                        else if (mainModel.imagePath.Equals("/img/bn_03.png"))
                        {
                            arrChkAED = "2|";
                        }
                        else if (mainModel.imagePath.Equals("/img/bn_01.png"))
                        {
                            if (getCount() == 0)
                            {
                                arrChkAED = "3|";
                            }
                            else
                            {
                                if (consumbefore > 0)
                                {
                                    arrChkAED = "2|";
                                }
                                else if (consumbefore ==0){
                                    arrChkAED = "1|";
                                }
                               
                            }
                        }

                    }
                    
                    if (PopresultEnd(totalCount, totalErrorCount, arrChkAED, Properties.Settings.Default.pworkkey) == 1)
                    {
                        if (Properties.Settings.Default.corp_name.Equals("구룡산업제2공장") || Properties.Settings.Default.corp_name.Equals("구룡산업제2공장 테스트"))
                        {
                            if (Properties.Settings.Default.mercName.Contains("인라인"))
                            {
                                if (mainModel.subOb.Count == 2)
                                {
                                    PopInlineStart(mainModel.subOb[mainModel.subOb.Count - 1]);
                                }
                            }
                        }

                            
                        Close();
                    }
                    //if ((mainModel.sumPworkCount == getCount()) && (mainModel.sumPworkCount > 0 || getCount() > 0))
                    //{
                    //    // Thread.Sleep(500);
                    //    arrChkAED = "2|";
                    //    if (PopresultEnd(totalCount, totalErrorCount, arrChkAED, Properties.Settings.Default.pworkkey) == 1)
                    //    {
                    //        Close();
                    //    };

                    //}
                    //else if ((int)Math.Ceiling(float.Parse(mainModel.sumPworkCount.ToString())) != 0 && getCount() > 0)
                    //{
                    //    // Thread.Sleep(500);
                    //    arrChkAED = "2|";
                    //    if (PopresultEnd(totalCount, totalErrorCount, arrChkAED, Properties.Settings.Default.pworkkey) == 1)
                    //    {
                    //        Close();
                    //    };
                    //    // Thread.Sleep(1000);

                    //}
                    //else if (getCount() == 0)
                    //{
                    //    //  Thread.Sleep(500);
                    //    arrChkAED = "3|";
                    //    if (PopresultEnd(0, 0, arrChkAED, Properties.Settings.Default.pworkkey) == 1)
                    //    {
                    //        Close();
                    //    };


                    //}
                    //else
                    //{
                    //    // Thread.Sleep(500);
                    //    arrChkAED = "1|";
                    //    if (PopresultEnd(totalCount, totalErrorCount, arrChkAED, Properties.Settings.Default.pworkkey) == 1)
                    //    {
                    //        Close();
                    //    };

                    //}
                    }



                // 
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
                "",
                "",
                ""},
                1

                );

            if (returnData.결과 == 1)
            {
                

                Properties.Settings.Default.pworkkey = returnData.리턴값;
                Properties.Settings.Default.Save();
                Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                InvokeCount(Properties.Settings.Default.mercName, "S");
                Properties.Settings.Default.count = 0;
                Properties.Settings.Default.saveDate = "";
                Properties.Settings.Default.pplanKey = 0;
                Properties.Settings.Default.Save();

                PopInlineEnd(subOb, int.Parse(returnData.리턴값));
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
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;

        }

        private void PopInlineEnd(SubModel subOb, int pWorkkey)
        {
            string arrChkAED = "1|";

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            string parameter = "[" + subOb.ProcessTypeName + "] " + mainModel.customSName + Environment.NewLine + mainModel.prodName + " " + getCount().ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopResultEnd]",
                new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명"},
                new object[] {
                "1",
                DateTime.Now.ToString("yyyy-MM-dd"),
                subOb.orderKey,
                arrChkAED,
                subOb.processType + "|",
                subOb.pplanKey + "|",
                pWorkkey + "|",
                getCount() + "|",
                getErrorCount() + "|",
                "" + "|",
                mainModel.sDatetime,
                subOb.ProcessTypeName
                }, 1, parameter
              );
            if (returnData.결과 == 1)
            {
                

            }

            Properties.Settings.Default.consumersumcount = 0;
            Properties.Settings.Default.count = 0;
            Properties.Settings.Default.saveDate = "";
            Properties.Settings.Default.pplanKey = 0;
            Properties.Settings.Default.pworkkey = "";
            Properties.Settings.Default.Save();
        }


        private int PopresultEnd(int totalCount, int totalErrorCount, string arrChkAED, string pWorkKey)
        {
            //Thread.Sleep(1000);
            string parameter = "[" + Properties.Settings.Default.mercName + "] " + this.mainModel.customSName + Environment.NewLine + mainModel.prodName + " " + totalCount.ToString("N0") + "개";
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,

               "[PopResultEnd]",
               new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명"},
               new object[] {
                "2",
                DateTime.Now.ToString("yyyy-MM-dd"),
                mainModel.orderKey,
                arrChkAED,
                mainModel.ProcessType+ "|",
                mainModel.pplanKey+ "|",
                pWorkKey+ "|",
                totalCount - beforeCount + "|",
                totalErrorCount+ "|",
                ""+"|",
                mainModel.sDatetime,
                Properties.Settings.Default.mercName

                }, 1, parameter

             );

            if (returnData.결과 == 1)
            {


                if ((imgpath.Equals("/img/bn_03.png") || imgpath.Equals("/img/bn_01.png")) && beforeCountCheck)
                {
                    beforeCountCheck = false;
                    PopresultEnd(0, 0, "3|", mainModel.ConsumWorkKey);

                }
            }
            Properties.Settings.Default.pplanKey = 0;
            Properties.Settings.Default.pworkkey = ""; 
            Properties.Settings.Default.Save();
            return returnData.결과;
        }
        private int getErrorCount()
        {
            int count = 0;

            for (int i = 0; i < consumerList.Count; i++)
            {
                count += int.Parse(consumerList[i].perrCnt.ToString());
            }

            return count;
        }

        private int getCount()
        {
            int count = 0;

            for (int i = 0; i < consumerList.Count; i++)
            {
                count += (int)Math.Ceiling(float.Parse(consumerList[i].pWorkCount.ToString()));
            }
            Console.WriteLine(count + "전체값");
            return count;
        }

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                can.Visibility = Visibility.Visible;
                Image img = (Image)sender;
                ConsumerModel consumerModel = (ConsumerModel)img.DataContext;
                product = consumerModel;
                dsConsumer_Input = new DsConsumer_input(consumerModel, mainModel);

                _ = dsConsumer_Input.ShowDialog();
                PopWorkListSearch();
                dsConsumer_Input = null;
                e.Handled = false;

                //switch (metrialitem)
                //{
                //    case "12":
                //    case "13":

                //        if (Main1.production != null)
                //        {
                //            try
                //            {
                //                Main1.arduSerial.Write("B");
                //            }
                //            catch (Exception)
                //            {

                //            }

                //        }
                //        break;

                //    default:
                //        if (Main.production != null)
                //        {
                //            try
                //            {
                //                Main.arduSerial.Write("B");
                //            }
                //            catch (Exception)
                //            {

                //            }
                //        }
                //        break;
                //}


                can.Visibility = Visibility.Collapsed;

            }

        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }
    }
}
