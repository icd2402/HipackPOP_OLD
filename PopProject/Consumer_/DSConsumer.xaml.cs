using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        bool beforeCountCheck = true;
        string imgpath = "";
        private bool checkProcessing = false; //작업 진행건이 있는지 확인
        private DispatcherTimer timer;
        private int second = 0;
        int totalCount = 0;
        int totalErrorCount = 0;
        string arrChkAED = "";
        bool 이월체크 = false;
        bool checkbefore = true;
        bool statCheck = false;
       
        public DSConsumer(MainModel mainModel,bool check) //check= 대기버튼,완료버튼으로 들어오면 true / 작업중 버튼으로 들어오면 false

        {
            InitializeComponent();
            this.mainModel = mainModel;
            statCheck = check;

            // 작업중 버튼으로 창을 열면 이월버튼 숨김 (이월버튼은 대기/완료일때만 가능)
            if (!statCheck)
            {
                box1.Visibility = Visibility.Collapsed;
            }

            imgpath = mainModel.imagePath; //버튼 상태 확인
            sumPworkCount = mainModel.sumPworkCount; //생산 합계
            PopWorkListSearch(); //실수요자 생산계획 조회
            SettingJsonManager.consumCount = 0;
            SettingJsonManager.SaveJsonFile();

            //스크롤바 사용으로 고정(대세요청)
            if (true)
            {
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                sv.PreviewMouseLeftButtonDown -= ScrollViewer_PreviewMouseLeftButtonDown;
                sv.PreviewMouseLeftButtonUp -= Sv_PreviewMouseLeftButtonUp;
                sv.PreviewMouseMove -= Sv_PreviewMouseMove;
            }
            else
            {
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                sv.PreviewMouseLeftButtonDown += ScrollViewer_PreviewMouseLeftButtonDown;
                sv.PreviewMouseLeftButtonUp += Sv_PreviewMouseLeftButtonUp;
                sv.PreviewMouseMove += Sv_PreviewMouseMove;
            }

        }

        //현황판으로 시작신호 보냄 // 확인
        //private void InvokeCount(string mercName, string v)
        //{
        //    //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
        //    try
        //    {
        //        SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}

        //실수요자 생산계획 조회
        private void PopWorkListSearch()
        {
            checkProcessing = false;
            //consumerSumCount = 0;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               SettingJsonManager.serverIndex,
              //"[PopWorkListMember]",
              // new string[] { "@orderKey", "@ProcessType", "@pplanDate" },
              // new object[] { mainModel.orderKey, mainModel.ProcessType, DateTime.Now.ToString("yyyy-MM-dd") },
               "[PopWorkListMemberPlanKey]",
                new string[] { "@orderKey", "@ProcessType", "@pplanDate", "@pplanKey" },
                new object[] { mainModel.orderKey, mainModel.ProcessType, DateTime.Now.ToString("yyyy-MM-dd"), mainModel.pplanKey },
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
                            orderKey = result["orderKey"].ToString(),                       //오더키
                            pworkKey = int.Parse(result["pworkKey"].ToString()),            //작업키
                            orderDate = result["orderDate"].ToString(),                     //수주일자
                            vendorID = result["vendorID"].ToString(),                       //납춤처ID
                            vendorName = result["vendorName"].ToString(),                   //납품처명
                            NoKey = result["NoKey"].ToString(),                             //순번키
                            membID = result["membID"].ToString(),                           //실수요자ID
                            membName = result["membName"].ToString(),                       //이름
                            adress = result["adress"].ToString(),                           //주소
                            PhonNo = result["PhonNo"].ToString(),                           //연락처
                            orderCount = int.Parse(result["orderCount"].ToString()),        //수주량
                            workCount = int.Parse(result["workCount"].ToString()),        //수주량
                            pworkDate = DateTime.Parse(result["pworkDate"].ToString()),     //작업일자
                            pplanKey = int.Parse(result["pplanKey"].ToString()),            //계획키
                        };

                        listitem.pWorkCount = float.Parse(result["pworkCount"].ToString()); //작업량
                        listitem.sDatetime = result["sDatetime"].ToString();                //시작시간
                        listitem.eDatetime = result["eDatetime"].ToString();                //종료시간
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());       //불량수량
                        listitem.Memo = result["Memo"].ToString();                          //비고

                        //상자수 사용 체크
                        if (SettingJsonManager.boxCheck)
                        {
                            listitem.orderCount /= (int)mainModel.pCnt;  //수주량/통수
                            listitem.pWorkCount /= (int)mainModel.pCnt;  //작업량/통수
                        }

                        ////금일 실수요자 생산리스트 또는 수주량 - 작업량이 30개 초과인경우(작업이 남은경우) 리스트 등록
                        //if (listitem.pworkDate.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd")) || listitem.orderCount - listitem.pWorkCount - listitem.workCount > 30)
                        //{
                        //    consumerList.Add(listitem);
                        //}

                        //계획키 있거나 계획량 - 결과량 - 생산량 > 30
                        if (listitem.pplanKey > 0 || listitem.orderCount - listitem.pWorkCount - listitem.workCount > 30)
                        {
                            consumerList.Add(listitem);
                        }

                        //if (SettingJsonManager.corp_name.Equals("선호칼라팩") || SettingJsonManager.corp_name.Equals("선호칼라팩테스트"))
                        //{
                        //    if (listitem.pworkDate.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd")) || listitem.orderCount - listitem.pWorkCount - listitem.workCount > 30)
                        //    {
                        //        consumerList.Add(listitem);
                        //    }
                        //}
                        //else
                        //{
                        //    if (listitem.pplanKey > 0 || listitem.orderCount - listitem.pWorkCount - listitem.workCount > 30)
                        //    {
                        //        consumerList.Add(listitem);
                        //    }
                        //}
                    }
                    for (int i = 0; i < consumerList.Count; i++)
                    {
                        //실수요자 창 켰을시 한번만 실행
                        if (checkbefore)
                        {
                            beforeCount += (int)Math.Ceiling(float.Parse(consumerList[i].pWorkCount.ToString())); //이미 완료된 생산량 더하기
                        }

                        //리스트를 조회해서 작업중인 건이 있으면 
                        if (consumerList[i].sDatetime.Length > 1 && consumerList[i].eDatetime.Length <= 1)
                        {
                            checkProcessing = true;
                            break;
                        }


                    }
                    Console.WriteLine("DSConsumerList.Count : " + consumerList.Count);
                    
                    //실수요자 리스트가 있으면
                    if (consumerList.Count > 0)
                    {
                        if (consumerList.Where((x) => x.pplanKey == mainModel.pplanKey).Count() > 0)
                        {
                            SettingJsonManager.pworkkey = consumerList.Where((x) => x.pplanKey == mainModel.pplanKey).FirstOrDefault().pworkKey.ToString();
                            SettingJsonManager.SaveJsonFile();
                        }

                        ic.ItemsSource = consumerList; //리스트 등록
                        txtNoPlan.Visibility = Visibility.Collapsed; //데이터 없음 글자 숨김
                    }
                    else
                    {
                        txtNoPlan.Visibility = Visibility.Visible; //데이터 없음 글자 팝업
                    }
                }
            }
            else
            {
                MessageBox_Dialog box = new MessageBox_Dialog("PopWorkListMember 조회 오류!!");
                _ = box.ShowDialog();
            }
            checkbefore = false;
        }

        //드래그
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
            }

        }

        

        //인라인 설비 DB저장
        private int PopInlineStart(SubModel subOb)
        {
            Console.WriteLine("인라인시작");
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
                "",
                "",
                ""},
                1
                );

            if (returnData.결과 == 1)
            {
                
                //리턴값 저장,확인
                Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
                SettingJsonManager.pworkkey = returnData.리턴값;
                SettingJsonManager.SaveJsonFile();

                //시작시간 저장
                mainModel.sDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //현황판으로 시작신호 보냄
                CountManager.SendMsg(SettingJsonManager.mercName, "S");

                //값 리셋
                SettingJsonManager.count = 0;
                SettingJsonManager.saveDate = "";
                SettingJsonManager.pplanKey = 0;
                SettingJsonManager.SaveJsonFile();

                //온라인 설비 종료 실행 
                PopInlineEnd(subOb, int.Parse(returnData.리턴값));
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
                msg.ShowDialog();
            }
            return returnData.결과;

        }

        //인라인 설비 종료
        private void PopInlineEnd(SubModel subOb, int pWorkkey)
        {
            string 전체공정 = "0";
            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (SettingJsonManager.mercName.Contains("인라인"))
                {
                    if (mainModel.subOb.Count == 2)
                    {
                        전체공정 = "1";
                    }
                    else
                    {
                        전체공정 = "0";
                    }

                }
            }
            string arrChkAED = "1|";

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string parameter = "[" + subOb.ProcessTypeName + "] " + mainModel.customSName + Environment.NewLine + mainModel.prodName + " " + SettingJsonManager.consumCount.ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEnd]",
                new string[] { "@AllChkAED","@작업일자","@오더Key","@arrChkAED","@arr공정구분","@arr계획Key","@arr작업Key","@arr작업량","@arr불량","@arr비고","@시작시간","@설비명","@전체공정"},
                new object[] {
                "1",
                DateTime.Now.ToString("yyyy-MM-dd"),
                subOb.orderKey,
                arrChkAED,
                subOb.processType + "|",
                subOb.pplanKey + "|",
                pWorkkey + "|",
                SettingJsonManager.consumCount + "|",
                getErrorCount() + "|",
                "" + "|",
                mainModel.sDatetime,
                subOb.ProcessTypeName,
                전체공정
                }, 1, parameter
              );
            if (returnData.결과 == 1)
            {
                Console.WriteLine("인라인END");

            }

            //값 초기화
            SettingJsonManager.consumersumcount = 0;
            SettingJsonManager.count = 0;
            SettingJsonManager.saveDate = "";
            SettingJsonManager.pplanKey = 0;
            SettingJsonManager.pworkkey = "";
            SettingJsonManager.SaveJsonFile();
        }


        //생산 종료
        private int PopresultEnd(int totalCount, int totalErrorCount, string arrChkAED, string pWorkKey)
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
            //Thread.Sleep(1000);
            string parameter = "[" + SettingJsonManager.mercName + "] " + this.mainModel.customSName + Environment.NewLine + mainModel.prodName + " " + totalCount.ToString("N0") + "개";
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,

               "[PopResultEnd]",
               new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명","@전체공정"},
               new object[] {
                "2",
                DateTime.Now.ToString("yyyy-MM-dd"),
                mainModel.orderKey,
                arrChkAED,
                mainModel.ProcessType+ "|",
                mainModel.pplanKey+ "|",
                pWorkKey+ "|",
                totalCount + "|",
                totalErrorCount+ "|",
                ""+"|",
                mainModel.sDatetime,
                SettingJsonManager.mercName,
                전체공정

                }, 1, parameter

             );

            if (returnData.결과 == 1)
            {
                Main.production = null;

                //작업중 또는 완료 버튼으로 들어왔고 beforeCountCheck true면
                //if ((imgpath.Equals("/img/bn_03.png") || imgpath.Equals("/img/bn_01.png")) && beforeCountCheck)
                //{
                //    beforeCountCheck = false;
                //    PopresultEnd(0, 0, "3|", mainModel.ConsumWorkKey);
                //}
            }
            SettingJsonManager.pplanKey = 0;
            SettingJsonManager.pworkkey = ""; 
            SettingJsonManager.SaveJsonFile();
            return returnData.결과;
        }

        //전체 불량수량 집계
        private int getErrorCount()
        {
            int count = 0;

            for (int i = 0; i < consumerList.Count; i++)
            {
                count += int.Parse(consumerList[i].perrCnt.ToString());
            }

            return count;
        }

        //완료 생산수량 집계
        private int getCount()
        {
            int count = 0;

            for (int i = 0; i < consumerList.Count; i++)
            {
                //완료건만 집계
                if (consumerList[i].imgSource.Equals("/img/bn_03.png"))
                {
                    count += (int)Math.Ceiling(float.Parse(consumerList[i].pWorkCount.ToString()));
                }
                
            }
            Console.WriteLine("총 생산수량" + count);
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

        //상태 버튼 클릭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                can.Visibility = Visibility.Visible; //화면 어둡게 처리
                Image img = (Image)sender;
                ConsumerModel consumerModel = (ConsumerModel)img.DataContext;
                product = consumerModel;

                //ds인풋화면 호출
                dsConsumer_Input = new DsConsumer_input(consumerModel, mainModel);
                _ = dsConsumer_Input.ShowDialog();

                PopWorkListSearch(); //계획리스트 조회
                dsConsumer_Input = null;
                e.Handled = false;

                //인풋화면 호출과 동시에 아두이노 카운팅 시작
                //switch (metrialitem)
                //{
                //    case "19":
                //    case "20":
                //        if (Main1.production != null)
                //        {
                //            try
                //            {
                //                Main1.arduSerial.Write("B"); //아두이노 현재카운터 요청
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
                //                Main.arduSerial.Write("B"); //아두이노 현재카운터 요청
                //            }
                //            catch (Exception)
                //            {
                //            }
                //        }
                //        break;
                //}
                can.Visibility = Visibility.Collapsed; //화면 어둡게 처리 끄기
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

        //이월 버튼
        private void 이월(object sender, MouseButtonEventArgs e)
        {
            이월체크 = true;
            var result = System.Windows.MessageBox.Show(mainModel.prodName + " 품목을 이월 하시겠습니까 ?", "이월", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
            }
            else //yes
            {
                //결과값이 있으면 이월
                if (mainModel.presultCount > 0 )
                {
                    spAdd410processinfo_workplan();
                    Close();
                }
                else //결과값 없으면
                {
                    MessageBox_Dialog dialog = new MessageBox_Dialog("이월 작업을 할 수 없습니다.");
                    dialog.ShowDialog();
                }
            }
        }

        //생산계획 이월
        private void spAdd410processinfo_workplan()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[spAdd410processinfo_workplan]",
                new string[] { "@오더Key", "@계획Key" },
                new object[] { mainModel.orderKey, mainModel.pplanKey },
                1
                );

            if (returnData.결과 == 1)
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("이월 완료");
                msg.ShowDialog();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("이월 실패");
                msg.ShowDialog();
            }
        }

        //창 종료 버튼
        private void Program_exit(object sender, MouseButtonEventArgs e)
        {
            Close();

            //MessageBoxResult result = System.Windows.MessageBox.Show("작업을 저장 하시겠습니까 ?", "작업 저장", MessageBoxButton.YesNo);

            //if (result == MessageBoxResult.No)
            //{
            //}
            //else
            //{
            //    //작업중인 건 있으면
            //    if (checkProcessing)
            //    {
            //        MessageBox_Dialog msg = new MessageBox_Dialog("생산 작업중인 건이 있습니다.");
            //        msg.ShowDialog();
            //    }
            //    else//작업중인 건 없으면
            //    {
            //        Close();
            //    }
            //}
        }

        //실수요자 창 종료시
        private void consumClosed(object sender, EventArgs e)
        {
            float TempTotalCount = 0;
            float TempTotalErrorCount = 0;

            //pworkKey가 같으면
            if (consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey).Count() > 0)
            {
                //총 생산
                TempTotalCount = consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey).Sum((x) => x.pWorkCount);
                //총 불량
                TempTotalErrorCount = consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey).Sum((x) => x.perrCnt);

                //상자수 체크시
                if (SettingJsonManager.boxCheck)
                {
                    TempTotalCount *= int.Parse(mainModel.pCnt.ToString());
                    TempTotalErrorCount *= int.Parse(mainModel.pCnt.ToString());
                }

                // 관리자 로그인이 아닐때
                if (!SettingJsonManager.manager.Equals("1"))
                {
                    if (consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey && (!string.IsNullOrEmpty(x.sDatetime.ToString()) && (string.IsNullOrEmpty(x.eDatetime.ToString())))).Count() > 0)
                    {
                        //실수요자 작업 진행 중일 때는 아무 작업 하지 않음
                    }
                    else
                    {
                        //생산 결과량 있을때
                        if (TempTotalCount > 0 || TempTotalErrorCount > 0)
                        {
                            //대기 버튼으로 들어왔었으면
                            if (mainModel.imagePath.Equals("/img/bn_02.png"))
                            {
                                arrChkAED = "1|"; //입력
                            }
                            //작업중 버튼으로 들어왔었으면
                            else if (mainModel.imagePath.Equals("/img/bn_01.png"))
                            {
                                //input창 완료수량 있을때
                                if (string.IsNullOrEmpty(mainModel.eDatetime.ToString()))
                                {
                                    arrChkAED = "1|"; //입력
                                }
                                else
                                {
                                    arrChkAED = "2|"; //수정
                                }
                            }
                            //완료 버튼으로 들어왔었으면
                            else if (mainModel.imagePath.Equals("/img/bn_03.png"))
                            {
                                arrChkAED = "2|"; //수정
                            }
                        }
                        //생산량 없을때
                        else
                        {
                            arrChkAED = "3|"; //삭제
                        }
                    }
                }
            }
            else
            {
                arrChkAED = "3|"; //삭제
            }

            if (arrChkAED != "")
            {
                string refPworkKey = string.Format("{0}", SettingJsonManager.pworkkey);
                PopresultEnd(Convert.ToInt32(TempTotalCount), Convert.ToInt32(TempTotalErrorCount), arrChkAED, SettingJsonManager.pworkkey);

                if (arrChkAED == "2|")
                {
                    PopResultEndTimeSetting(refPworkKey);
                }
            }
        }

        private void PopResultEndTimeSetting(string pworkkey)
        {

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[spUdt431processinfo_workTimeChange]",
                new string[] { "@생산키", "@생산시작일시", "@생산종료일시" },
                new object[] {
                pworkkey,
                "",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                },
                1

                );

            if (returnData.결과 == 1)
            {
            }
            else
            {
            }
        }

        //원격연결 버튼
        private void Remote_click(object sender, MouseButtonEventArgs e)
        {
            _ = Process.Start("http://onefax.co.kr/w_onefax/remotebox_view.aspx");
        }
    }
}
