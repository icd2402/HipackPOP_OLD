using Newtonsoft.Json.Linq;
using PopProject.input;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PopProject.Consumer_
{
    // <summary>
    // Consumer.xaml에 대한 상호 작용 논리
    // </summary>

    public partial class Consumer : Window
    {
        private readonly string metrialitem = Properties.Settings.Default.meterialinfo;
        public static ObservableCollection<ConsumerModel> consumerList;
        private readonly MainModel mainModel;
        public InputModel inputModel;
        public static ConsumerModel product;
        public static Consumer_input consumer_input;
        private float consumerSumCount = 0;
        private float consumerSumErrorCount = 0;
        private float str;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        private bool checkProcessing = false;

        public Consumer(MainModel mainModel)
        {

            InitializeComponent();

            if (metrialitem.Length == 0)
            {
                metrialitem = "";
            }

            //설정에서 세팅 정보
            switch (metrialitem)
            {
                case "12":
                case "13":
                    if (Main1.production != null)
                    {
                        try
                        {
                            Main1.arduSerial.Write("A");
                            Main1.arduSerial.Write("B");
                        }

                        catch (Exception e)
                        {
                            Console.Write(e.Message);
                        }

                    }
                    else
                    {
                        try
                        {
                            Main1.arduSerial.Write("A");
                        }

                        catch (Exception)
                        {

                        }
                    }

                    break;
                default:
                    if (Main.production != null)
                    {
                        try
                        {
                            Main.arduSerial.Write("A");
                            Main.arduSerial.Write("B");
                        }

                        catch (Exception)
                        {

                        }
                    }

                    else
                    {
                        try
                        {

                            Main.arduSerial.Write("A");

                        }

                        catch (Exception)
                        {

                        }

                    }

                    break;
            }

            PreviewKeyDown += new KeyEventHandler(HandleKeyDown);
            this.mainModel = mainModel;
            inputModel = new InputModel();
            PopWorkListSearch();
            switch (metrialitem)
            {
                case "12":
                case "13":
                case "14":

                    if (Main1.arduSerial.IsOpen)
                    {
                        Main1.arduSerial.DataReceived += ArduReReceive;
                    }

                    break;

                default:

                    if (Main.arduSerial.IsOpen)
                    {
                        Main.arduSerial.DataReceived += ArduReReceive;
                    }

                    break;

            }
            

        }

        

        private void ArduReReceive(object sender, SerialDataReceivedEventArgs e)
        {
            switch (metrialitem)
            {
                case "12":
                case "13":

                    str = Main1.str;
                    break;

                default:
                    str = Main.str;
                    break;
            }

            // Console.WriteLine("imageSource" + consumer_input.inputModel.imagePath);
            switch (Properties.Settings.Default.processType)
            {

                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                    if (consumer_input != null)
                    {
                        if (consumer_input.inputModel.imagePath.Equals("/img/bn_06.png"))
                        {
                            product.sumPworkCount = str;
                            product.presultCount = str - Properties.Settings.Default.consumersumcount;
                        }
                        // consumer_input.inputModel.count = str - Properties.Settings.Default.consumersumcount;
                        consumer_input.inputModel.count = str;
                        InvokeCount(str);
                        Console.WriteLine("consumer_input.inputModel.count : " + consumer_input.inputModel.count);
                    }
                    break;
                default:
                    if (consumer_input != null)
                    {
                        if (consumer_input.inputModel.imagePath.Equals("/img/bn_06.png"))
                        {
                            product.sumPworkCount = str * consumer_input.inputModel.pCnt;
                            product.presultCount = (str * consumer_input.inputModel.pCnt) - (Properties.Settings.Default.consumersumcount * consumer_input.inputModel.pCnt);
                        }

                        // consumer_input.inputModel.count = (str * consumer_input.inputModel.pCnt) - (Properties.Settings.Default.consumersumcount);
                        consumer_input.inputModel.count = str * consumer_input.inputModel.pCnt;
                        InvokeCount(str * float.Parse(consumer_input.inputModel.pCnt + ""));
                        Console.WriteLine("consumer_input.inputModel.count : " + consumer_input.inputModel.count);

                    }

                    break;
            }
        }

        public void InvokeCount(float str)
        {
            _ = SignalRAdministrator.허브.Invoke("invokeCount", Properties.Settings.Default.corp_id, Properties.Settings.Default.mercName, str + "");
        }

        private void PopWorkListSearch()
        {
            _ = new LogData("조회");
            //consumerSumCount = 0;
            checkProcessing = false;
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
                            orderCount = int.Parse(result["orderCount"].ToString())
                        };

                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.orderCount /= (int)mainModel.pCnt;
                        }

                        listitem.pWorkCount = float.Parse(result["pworkCount"].ToString());
                        consumerSumCount += Properties.Settings.Default.pworkkey == listitem.pworkKey.ToString() ? listitem.pWorkCount : 0;
                        Properties.Settings.Default.consumersumcount = Properties.Settings.Default.consumersumcount + consumerSumCount;
                        listitem.sDatetime = result["sDatetime"].ToString();
                        listitem.eDatetime = result["eDatetime"].ToString();
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());
                        consumerSumErrorCount += Properties.Settings.Default.pworkkey == listitem.pworkKey.ToString() ? listitem.perrCnt : 0;
                        listitem.Memo = result["Memo"].ToString();

                        if (checkProcessing == false)
                        {

                            if (listitem.sDatetime.Length > 1 && listitem.eDatetime.Length <= 1)
                            {
                                checkProcessing = true;
                            }
                            // checkProcessing = !string.IsNullOrEmpty(listitem.sDatetime) && string.IsNullOrEmpty(listitem.eDatetime);

                        }

                        consumerList.Add(listitem);
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

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Program_exit(object sender, MouseButtonEventArgs e)
        {

            switch (metrialitem)
            {
                case "12":
                case "13":
                    Main1.production = null;
                    Popresultend();
                    if (Main1.production != null)
                    {
                        Main1.arduSerial.Write("A");
                        Main1.arduSerial.Write("B");
                    }

                    else
                    {
                        try
                        {
                            Main1.arduSerial.Write("A");
                        }

                        catch (Exception)
                        {

                        }
                    }
                    break;

                default:

                    Main.production = null;
                    Popresultend();

                    if (Main.production != null)
                    {
                        Main.arduSerial.Write("A");
                        Main.arduSerial.Write("B");
                    }

                    else
                    {
                        try
                        {
                            Main.arduSerial.Write("A");
                        }
                        catch (Exception)
                        {

                        }
                    }
                    break;
            }


        }
        private void Popresultend()
        {
            _ = new LogData("작업완료");
            if (Properties.Settings.Default.pworkkey == "")
            {
                Close();

            }

            consumerSumCount = 0;
            consumerSumErrorCount = 0;
            PopWorkListSearch();

            if (checkProcessing)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("작업중인 건이 있습니다", "",
                    MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }


            }

            string arrChkAED;
            if (Properties.Settings.Default.consumersumcount == 0)
            {
                Close();
                return;
            }

            else
            {
                arrChkAED = "1|";
            }

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,

                "[PopResultEnd]",
                new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고"},
                new object[] {
                "1",
                DateTime.Now.ToString("yyyy-MM-dd"),
                mainModel.orderKey,
                arrChkAED,
                mainModel.ProcessType+ "|",
                mainModel.pplanKey+ "|",
                Properties.Settings.Default.consumpWorkKey+ "|",
                consumerSumCount + "|",
                consumerSumErrorCount+ "|",
                ""+"|",

                 }, 1

              );

            if (returnData.결과 == 1)
            {
                Properties.Settings.Default.pworkkey = "";
                Properties.Settings.Default.consumpWorkKey = 0;
                Properties.Settings.Default.consumersumcount = 0;
                Properties.Settings.Default.Save();

                switch (metrialitem)
                {
                    case "12":
                    case "13":
                        Main1.production = null;

                        try
                        {
                            Main1.arduSerial.Write("A");
                            Main1.arduSerial.Write("B");
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("에러 : " + e.Message);
                        }

                        InvokeCount(Properties.Settings.Default.mercName, "E");
                        Close();
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
                            Console.WriteLine("에러 : " + e.Message);
                        }

                        InvokeCount(Properties.Settings.Default.mercName, "E");
                        Close();
                        break;

                }




            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopResultEnd 저장실패 !!");
                _ = msg.ShowDialog();

            }

        }
        private void InvokeCount(string mercName, string v)
        {
            _ = SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);
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
                consumer_input = new Consumer_input(consumerModel, mainModel);
                _ = consumer_input.ShowDialog();
                PopWorkListSearch();
                consumer_input = null;
                e.Handled = false;

                switch (metrialitem)
                {
                    case "12":
                    case "13":

                        if (Main1.production != null)
                        {
                            try
                            {
                                Main1.arduSerial.Write("B");
                            }
                            catch(Exception)
                            {

                            }
                            
                        }
                        break;

                    default:
                        if (Main.production != null)
                        {
                            try { 
                                Main.arduSerial.Write("B");
                            }
                            catch(Exception)
                            {

                            }
                        }
                        break;
                }


                can.Visibility = Visibility.Collapsed;

            }

        }

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
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

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
