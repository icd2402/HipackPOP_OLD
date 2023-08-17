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
                case "19": //마닐라
                case "20": //원단
                    if (Main1.production != null)
                    {
                        try
                        {
                            Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                            Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
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
                            Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;

                default: //기본
                    if (Main.production != null)
                    {
                        try
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                            Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
            }

            PreviewKeyDown += new KeyEventHandler(HandleKeyDown); //클릭시 메서드 호출
            this.mainModel = mainModel;
            inputModel = new InputModel();
            PopWorkListSearch(); //생산계획 리스트 조회
            switch (metrialitem)
            {
                case "14":
                case "19": //마닐라
                case "20": //원단
                    if (Main1.arduSerial.IsOpen)
                    {
                        Main1.arduSerial.DataReceived += ArduReReceive;
                    }
                    break;

                default://일반
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
                case "19": //마닐라
                case "20": //원단
                    str = Main1.str;
                    break;

                default: //일반
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
                        //생산중일때
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

        //생산계획 조회
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
                            orderKey = result["orderKey"].ToString(),               //오더키
                            pworkKey = int.Parse(result["pworkKey"].ToString()),    //작업키
                            orderDate = result["orderDate"].ToString(),             //수주일자
                            vendorID = result["vendorID"].ToString(),               //납춤처ID
                            vendorName = result["vendorName"].ToString(),           //납품처명
                            NoKey = result["NoKey"].ToString(),                     //순번키
                            membID = result["membID"].ToString(),                   //실수요자ID
                            membName = result["membName"].ToString(),               //이름
                            adress = result["adress"].ToString(),                   //주소
                            PhonNo = result["PhonNo"].ToString(),                   //연락처
                            orderCount = int.Parse(result["orderCount"].ToString()) //수주량
                        };

                        //상자수 사용 체크
                        if (Properties.Settings.Default.boxCheck)
                        {
                            listitem.orderCount /= (int)mainModel.pCnt;
                        }

                        listitem.pWorkCount = float.Parse(result["pworkCount"].ToString()); //생산량
                        consumerSumCount += Properties.Settings.Default.pworkkey == listitem.pworkKey.ToString() ? listitem.pWorkCount : 0;  //각각의 생산량 합산
                        Properties.Settings.Default.consumersumcount = Properties.Settings.Default.consumersumcount + consumerSumCount;      //하반값 저장
                        listitem.sDatetime = result["sDatetime"].ToString();                //시작시간
                        listitem.eDatetime = result["eDatetime"].ToString();                //종료시간
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());       //불량수량
                        consumerSumErrorCount += Properties.Settings.Default.pworkkey == listitem.pworkKey.ToString() ? listitem.perrCnt : 0; //각각의 불량수량 합산
                        listitem.Memo = result["Memo"].ToString();                          //비고

                        //작업중인건이 없을때
                        if (checkProcessing == false)
                        {
                            //시작시간 있고 종료시간 있으면 (작업중이 아니면)
                            if (listitem.sDatetime.Length > 1 && listitem.eDatetime.Length <= 1)
                            {
                                checkProcessing = true;
                            }
                            // checkProcessing = !string.IsNullOrEmpty(listitem.sDatetime) && string.IsNullOrEmpty(listitem.eDatetime);
                        }
                        consumerList.Add(listitem);
                    }

                    Console.WriteLine("consumerList.Count : " + consumerList.Count);

                    //실수요자 계획리스트 있을때
                    if (consumerList.Count > 0)
                    {
                        ic.ItemsSource = consumerList;
                        txtNoPlan.Visibility = Visibility.Collapsed;
                    }
                    else //없을때
                    {
                        txtNoPlan.Visibility = Visibility.Visible; //실수요자 데이터가 없습니다
                    }
                }
            }
            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("PopWorkListMember 조회 오류!!");
                _ = box.ShowDialog();
            }
        }

        //화면 클릭시
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

        //ESC 버튼 
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        //닫기 버튼
        private void Program_exit(object sender, MouseButtonEventArgs e)
        {
            switch (metrialitem)
            {
                case "19": //마닐라
                case "20": //원단
                    Main1.production = null;
                    Popresultend();
                    if (Main1.production != null)
                    {
                        Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                        Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                    }
                    else //일반
                    {
                        try
                        {
                            Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;

                default:

                    Main.production = null;
                    Popresultend();

                    //생산중일때
                    if (Main.production != null)
                    {
                        Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                        Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                    }
                    else //미생산중
                    {
                        try
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
            }
        }

        //생산 종료
        private void Popresultend()
        {
            _ = new LogData("작업완료");
            if (Properties.Settings.Default.pworkkey == "")
            {
                Close();
            }

            consumerSumCount = 0; //생산결과
            consumerSumErrorCount = 0; //불량수량
            PopWorkListSearch();//계획리스트 조회

            //작업중인 건 있으면
            if (checkProcessing)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("작업중인 건이 있습니다", "",
                    MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            //생산결과가 없으면
            string arrChkAED;
            if (Properties.Settings.Default.consumersumcount == 0)
            {
                Close();
                return;
            }
            else //생산결과 있으면
            {
                arrChkAED = "1|";
            }

            //DB에 쓰기
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,

                "[PopResultEnd]",
                new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고"},
                new object[] {
                "1",                                             //AllChkAED
                DateTime.Now.ToString("yyyy-MM-dd"),             //작업일자
                mainModel.orderKey,                              //오더키
                arrChkAED,                                       //arrChkAED
                mainModel.ProcessType+ "|",                      //arr공정구분
                mainModel.pplanKey+ "|",                         //arr계획Key
                Properties.Settings.Default.consumpWorkKey+ "|", //arr작업Key
                consumerSumCount + "|",                          //arr작업량
                consumerSumErrorCount+ "|",                      //arr불량
                ""+"|",                                          //arr비고
                 }, 1
              );

            if (returnData.결과 == 1)
            {
                //저장값 리셋
                Properties.Settings.Default.pworkkey = "";
                Properties.Settings.Default.consumpWorkKey = 0;
                Properties.Settings.Default.consumersumcount = 0;
                Properties.Settings.Default.Save();

                switch (metrialitem)
                {
                    case "19": //마닐라
                    case "20": //원단

                        Main1.production = null;
                        try
                        {
                            Main1.arduSerial.Write("A"); //아두이노 카운터 리셋
                            Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("에러 : " + e.Message);
                        }

                        InvokeCount(Properties.Settings.Default.mercName, "E"); //현황판으로 생산종료 송신 //현황판으로 생산종료 송신
                        Close();
                        break;

                    default: //일반
                        Main.production = null;
                        try
                        {
                            Main.arduSerial.Write("A"); //아두이노 카운터 리셋
                            Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("에러 : " + e.Message);
                        }

                        InvokeCount(Properties.Settings.Default.mercName, "E"); //현황판으로 생산종료 송신 //현황판으로 생산종료 송신
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

        //대기버튼 클릭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                can.Visibility = Visibility.Visible; //화면 회색처리
                Image img = (Image)sender;
                ConsumerModel consumerModel = (ConsumerModel)img.DataContext;
                product = consumerModel;
                consumer_input = new Consumer_input(consumerModel, mainModel);
                _ = consumer_input.ShowDialog();
                PopWorkListSearch(); //생산계획 조회
                consumer_input = null;
                e.Handled = false;

                switch (metrialitem)
                {
                    case "19": //마니라
                    case "20": //원단
                        //생산중일때
                        if (Main1.production != null)
                        {
                            try
                            {
                                Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            catch(Exception)
                            {
                            }
                        }
                        break;

                    default: //일반
                        //생산중일때
                        if (Main.production != null)
                        {
                            try 
                            { 
                                Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                            }
                            catch(Exception)
                            {
                            }
                        }
                        break;
                }
                can.Visibility = Visibility.Collapsed; //화면 회색처리 없앰
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
