using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        int todayTotalCount = 0;
#pragma warning disable CS0414 // 'DSConsumer.beforeCountCheck' 필드가 할당되었지만 사용되지 않았습니다.
        bool beforeCountCheck = true;
#pragma warning restore CS0414 // 'DSConsumer.beforeCountCheck' 필드가 할당되었지만 사용되지 않았습니다.
        string imgpath = "";
#pragma warning disable CS0414 // 'DSConsumer.checkProcessing' 필드가 할당되었지만 사용되지 않았습니다.
        private bool checkProcessing = false; //작업 진행건이 있는지 확인
#pragma warning restore CS0414 // 'DSConsumer.checkProcessing' 필드가 할당되었지만 사용되지 않았습니다.
#pragma warning disable CS0169 // 'DSConsumer.timer' 필드가 사용되지 않았습니다.
        private DispatcherTimer timer;
#pragma warning restore CS0169 // 'DSConsumer.timer' 필드가 사용되지 않았습니다.
#pragma warning disable CS0414 // 'DSConsumer.second' 필드가 할당되었지만 사용되지 않았습니다.
        private int second = 0;
#pragma warning restore CS0414 // 'DSConsumer.second' 필드가 할당되었지만 사용되지 않았습니다.
#pragma warning disable CS0414 // 'DSConsumer.totalCount' 필드가 할당되었지만 사용되지 않았습니다.
        int totalCount = 0;
#pragma warning restore CS0414 // 'DSConsumer.totalCount' 필드가 할당되었지만 사용되지 않았습니다.
#pragma warning disable CS0414 // 'DSConsumer.totalErrorCount' 필드가 할당되었지만 사용되지 않았습니다.
        int totalErrorCount = 0;
#pragma warning restore CS0414 // 'DSConsumer.totalErrorCount' 필드가 할당되었지만 사용되지 않았습니다.
        string arrChkAED = "";
#pragma warning disable CS0414 // 'DSConsumer.이월체크' 필드가 할당되었지만 사용되지 않았습니다.
        bool 이월체크 = false;
#pragma warning restore CS0414 // 'DSConsumer.이월체크' 필드가 할당되었지만 사용되지 않았습니다.
        bool checkbefore = true;
        bool CarryForwardButtonState = false;
       
        public DSConsumer(MainModel mainModel,bool carryForwardButtonState) //이월버튼 사용 유무
        {
            InitializeComponent();
            this.mainModel = mainModel;
            CarryForwardButtonState = carryForwardButtonState;
            LogManager.Info("실수요자 리스트 팝업: " + mainModel.corpName + " / " + mainModel.prodName, MethodBase.GetCurrentMethod().DeclaringType);
            string metrialitem = SettingJsonManager.meterialinfo;

            // 작업중 버튼으로 창을 열면 이월버튼 숨김 (이월버튼은 대기/완료일때만 가능)
            if (!CarryForwardButtonState)
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
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                sv.PreviewMouseLeftButtonDown += ScrollViewer_PreviewMouseLeftButtonDown;
                sv.PreviewMouseLeftButtonUp += Sv_PreviewMouseLeftButtonUp;
                sv.PreviewMouseMove += Sv_PreviewMouseMove;
            }

        }

        //실수요자 생산계획 조회
        private void PopWorkListSearch()
        {
            checkProcessing = false;
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               SettingJsonManager.serverIndex,
               "[PopWorkListMemberPlanKey]",
                new string[] { "@orderKey", "@ProcessType", "@pplanDate", "@pplanKey" },
                new object[] { mainModel.orderKey, mainModel.ProcessType, DateTimeManager.GetSelectedDateStr(), mainModel.pplanKey },
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
                            pworkDate = DateTime.Parse(result["pworkDate"].ToString()),     //작업일자
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
                            workCount = int.Parse(result["workCount"].ToString()),          //이전 생산량
                            pplanKey = int.Parse(result["pplanKey"].ToString()),            //계획키
                        };

                        listitem.pWorkCount = float.Parse(result["pworkCount"].ToString()); //금일 결과량
                        listitem.sDatetime = result["sDatetime"].ToString();                //시작시간
                        listitem.eDatetime = result["eDatetime"].ToString();                //종료시간
                        listitem.perrCnt = float.Parse(result["perrCnt"].ToString());       //불량수량
                        listitem.Memo = result["Memo"].ToString();                          //비고

                        //상자수 사용 체크
                        if (SettingJsonManager.boxCheck)
                        {
                            listitem.orderCount /= (int)mainModel.pCnt;  //주문량 ÷통수
                            listitem.workCount /= (int)mainModel.pCnt;   //이전 생산량 ÷ 통수
                            listitem.pWorkCount /= (int)mainModel.pCnt;  //금일 작업량 ÷ 통수
                        }

                        listitem.totalWorkCount = listitem.workCount + Convert.ToInt32(listitem.pWorkCount);

                        //리스트에서 금일결과량이 있으면 완료버튼으로 표시
                        if (listitem.pWorkCount > 0)
                        {
                            listitem.imgSource = "/img/bn_03.png"; //진행중 버튼으로 변경
                        }

                        //남은 생산량 30개 초과일때만 리스트 담기
                        if (listitem.pplanKey > 0 || listitem.orderCount - listitem.workCount > 30)
                        {
                            consumerList.Add(listitem);
                        }
                        
                        Console.WriteLine("실수요자명 : " + listitem.membName + ", pworkKey : " + listitem.pworkKey + ", pplanKey : " + listitem.pplanKey);
                    }

                    //실수요자 리스트 만큼 반복
                    for (int i = 0; i < consumerList.Count; i++)
                    {
                        //실수요자 창 켰을시 한번만 실행
                        if (checkbefore)
                        {
                            todayTotalCount += (int)Math.Ceiling(float.Parse(consumerList[i].pWorkCount.ToString())); //금일 작업량 합산
                        }

                        //리스트를 조회해서 시작시간 값이 있고 종료시간 값이 없으면 
                        if (consumerList[i].sDatetime.Length > 1 && consumerList[i].eDatetime.Length <= 1)
                        {
                            checkProcessing = true;
                            //consumerList[i].imgSource = "/img/bn_01.png"; //진행중 버튼으로 변경
                            break;
                        }

                        //시작,종료 시간이 둘 다 있을 경우 => 한번 이라도 생산 완료하면
                        if (consumerList[i].sDatetime.Length > 1 && consumerList[i].eDatetime.Length > 1)
                        {
                            //string sTime = consumerList[i].sDatetime;
                            //string eTime = consumerList[i].eDatetime;
                            //DateTime startTime = DateTime.ParseExact(sTime, "yyyy/MM/dd HH:mm:ss", null);
                            //DateTime endTime = DateTime.ParseExact(eTime, "yyyy/MM/dd HH:mm:ss", null);

                            DateTime startTime = Convert.ToDateTime(consumerList[i].sDatetime);
                            DateTime endTime = Convert.ToDateTime(consumerList[i].eDatetime);

                            if (startTime > endTime)
                            {
                                consumerList[i].imgSource = "/img/bn_01.png"; //진행중 버튼으로 변경
                                break;
                            }
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
                Console.WriteLine("------------------------------------------------------");
                LogManager.Info("PopWorkListMemberPlanKey 실수요자 리스트 조회 완료", MethodBase.GetCurrentMethod().DeclaringType);

            }
            else
            {
                LogManager.Error("PopWorkListMemberPlanKey 실수요자 리스트 조회 오류", MethodBase.GetCurrentMethod().DeclaringType);
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
        //private int PopInlineStart(SubModel subOb)
        //{
        //    Console.WriteLine("인라인시작");
        //    ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
        //    ReturnModel returnData = ServiceModel.GetData(
        //        SettingJsonManager.serverIndex,
        //        "[PopResultStart]",
        //        new string[] { "@pworkDate", "@orderKey", "@pplanKey", "@ProcessType", "@mercName", "@UpUserID", "@UpUserName", "@workerName", "@workerID", "@SCMKey" },
        //        new object[] {
        //        DateTimeManager.GetSelectedDateStr(),
        //        subOb.orderKey,
        //        subOb.pplanKey,
        //        subOb.processType,
        //        subOb.ProcessTypeName,
        //        MainWindow.userID,
        //        MainWindow.userName,
        //        "",
        //        "",
        //        ""},
        //        1
        //        );

        //    if (returnData.결과 == 1)
        //    {
                
        //        //리턴값 저장,확인
        //        Console.WriteLine("pworkKey리턴값" + returnData.리턴값);
        //        SettingJsonManager.pworkkey = returnData.리턴값;
        //        SettingJsonManager.SaveJsonFile();

        //        //시작시간 저장
        //        mainModel.sDatetime = (DateTime.Now.ToString("yyyy-MM-dd") == DateTimeManager.GetSelectedDateStr()) ?     
        //                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTimeManager.GetSelectedStr();// 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate

        //        //현황판으로 시작신호 보냄
        //        CountManager.SendMsg(SettingJsonManager.mercName, "S");

        //        //값 리셋
        //        SettingJsonManager.count = 0;
        //        SettingJsonManager.saveDate = "";
        //        SettingJsonManager.pplanKey = 0;
        //        SettingJsonManager.SaveJsonFile();

        //        //온라인 설비 종료 실행 
        //        PopInlineEnd(subOb, int.Parse(returnData.리턴값));
        //    }
        //    else
        //    {
        //        MessageBox_Dialog msg = new MessageBox_Dialog("저장실패");
        //        msg.ShowDialog();
        //    }
        //    return returnData.결과;

        //}

        ////인라인 설비 종료
        //private void PopInlineEnd(SubModel subOb, int pWorkkey)
        //{

        //    string 전체공정 = "0";
        //    if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
        //    {
        //        if (SettingJsonManager.mercName.Contains("인라인"))
        //        {
        //            if (mainModel.subOb.Count == 2)
        //            {
        //                전체공정 = "1";
        //            }
        //            else
        //            {
        //                전체공정 = "0";
        //            }
        //        }
        //    }
        //    string arrChkAED = "1|";

        //    ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
        //    string parameter = "[" + subOb.ProcessTypeName + "] " + mainModel.customSName + Environment.NewLine + mainModel.prodName + " " + SettingJsonManager.consumCount.ToString("N0") + "개";
        //    ReturnModel returnData = ServiceModel.GetData(
        //        SettingJsonManager.serverIndex,
        //        "[PopResultEnd]",
        //        new string[] { "@AllChkAED","@작업일자","@오더Key","@arrChkAED","@arr공정구분","@arr계획Key","@arr작업Key","@arr작업량","@arr불량","@arr비고","@시작시간","@설비명","@전체공정"},
        //        new object[] {
        //        "1",
        //        DateTimeManager.GetSelectedDateStr(),
        //        subOb.orderKey,
        //        arrChkAED,
        //        subOb.processType + "|",
        //        subOb.pplanKey + "|",
        //        pWorkkey + "|",
        //        SettingJsonManager.consumCount + "|",
        //        getErrorCount() + "|",
        //        "" + "|",
        //        mainModel.sDatetime,
        //        subOb.ProcessTypeName,
        //        전체공정
        //        }, 1, parameter
        //      );
        //    if (returnData.결과 == 1)
        //    {
        //        Console.WriteLine("인라인END");

        //    }

        //    //값 초기화
        //    SettingJsonManager.consumersumcount = 0;
        //    SettingJsonManager.count = 0;
        //    SettingJsonManager.saveDate = "";
        //    SettingJsonManager.pplanKey = 0;
        //    SettingJsonManager.pworkkey = "";
        //    SettingJsonManager.SaveJsonFile();
        //}


        //생산 종료
        private int PopresultEnd(int totalCount, int totalErrorCount, string arrChkAED, string pWorkKey)
        {
            string 전체공정 = "0";

            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (SettingJsonManager.mercName.Contains("인라인"))
                {
                    전체공정 = "1";
                }
            }

            if (SettingJsonManager.corp_name.Contains("테스트"))
            {
                Console.WriteLine("---------------------------------------------------------------------------" + Environment.NewLine +
                                  "실수요자 리스트 닫기 : PopResultEnd" + Environment.NewLine +
                                  "AllChkAED:" + "2" + Environment.NewLine +
                                  "작업일자:" + DateTimeManager.GetSelectedDateStr() + Environment.NewLine +
                                  "오더Key:" + mainModel.orderKey + Environment.NewLine +
                                  "arrChkAED:" + arrChkAED + Environment.NewLine +
                                  "arr공정구분:" + mainModel.ProcessType + "|" + Environment.NewLine +
                                  "arr계획키:" + mainModel.pplanKey + "|" + Environment.NewLine +
                                  "arr작업키:" + pWorkKey + "|" + Environment.NewLine +
                                  "arr작업량:" + totalCount + "|" + Environment.NewLine +
                                  "arr불량:" + totalErrorCount + "|" + Environment.NewLine +
                                  "arr비고:" + "" + "|" + Environment.NewLine +
                                  "시작시간:" + mainModel.sDatetime + Environment.NewLine +
                                  "설비명:" + SettingJsonManager.mercName + Environment.NewLine +
                                  "전체공정" + 전체공정 + Environment.NewLine +
                                 "---------------------------------------------------------------------------");
            }

            string parameter = "[" + SettingJsonManager.mercName + "] " + this.mainModel.customSName + Environment.NewLine + mainModel.prodName + " " + totalCount.ToString("N0") + "개";
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               SettingJsonManager.serverIndex,

               "[PopResultEnd]",
               new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명","@전체공정"},
               new object[] {
                "2",
                DateTimeManager.GetSelectedDateStr(),
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
                LogManager.Info("PopResultEnd 실행 완료" + ", arrChkAED: " + arrChkAED, MethodBase.GetCurrentMethod().DeclaringType);
                if (SettingJsonManager.meterialinfo.Equals("19"))
                {
                    Main1.production = null;

                }
                else
                {
                    Main.production = null;
                }
            }
            else
            {
                LogManager.Error("PopResultEnd 실행 실패", MethodBase.GetCurrentMethod().DeclaringType);
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
                
                
                //대기 버튼
                if (consumerModel.imgSource.Equals("/img/bn_02.png"))
                {
                    if (product.membID.Equals(consumerModel.membID))
                    {
                        Console.WriteLine("일치하는 실수요자임!!");
                    }
                    LogManager.Info("실수요자 대기버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);
                }
                //작업중 버튼
                else if (consumerModel.imgSource.Equals("/img/bn_01.png"))
                {
                    LogManager.Info("실수요자 작업중버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

                }
                //완료 버튼
                else if (consumerModel.imgSource.Equals("/img/bn_03.png"))
                {
                    LogManager.Info("실수요자 완료버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

                }

                //ds인풋화면 호출
                dsConsumer_Input = new DsConsumer_input(consumerModel, mainModel);
                _ = dsConsumer_Input.ShowDialog();

                PopWorkListSearch(); //계획리스트 조회
                dsConsumer_Input = null;
                e.Handled = false;
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
            LogManager.Info("실수요자 이월버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            이월체크 = true;
            var result = System.Windows.MessageBox.Show(mainModel.prodName + " 품목을 이월 하시겠습니까 ?", "이월", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
            }
            else //yes
            {
                //결과값이 있으면 이월
                if (mainModel.presultCount > 0)
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
                LogManager.Info("spAdd410processinfo_workplan 실수요자 이월 완료", MethodBase.GetCurrentMethod().DeclaringType);
                MessageBox_Dialog msg = new MessageBox_Dialog("이월 완료");
                msg.ShowDialog();
            }
            else
            {
                LogManager.Error("spAdd410processinfo_workplan 실행 이월 실패", MethodBase.GetCurrentMethod().DeclaringType);
                MessageBox_Dialog msg = new MessageBox_Dialog("이월 실패");
                msg.ShowDialog();
            }
        }

        //창 종료 버튼
        private void Program_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //실수요자 창 종료시
        private void consumClosed(object sender, EventArgs e)
        {
            LogManager.Info("실수요자 창 닫기 클릭", MethodBase.GetCurrentMethod().DeclaringType);

            Console.WriteLine("실수요자 리스트 닫기 = " + "pworkkey : " + SettingJsonManager.pworkkey);
            float TempTotalCount = 0;
            float TempTotalErrorCount = 0;

            //관리자 로그인이 아닐때
            if (!SettingJsonManager.manager.Equals("1"))
            {
                //pworkKey가 같으면
                if (consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey).Count() > 0)
                {
                    //총 생산 합산
                    TempTotalCount = consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey).Sum((x) => x.pWorkCount);
                    //총 불량 합산
                    TempTotalErrorCount = consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey).Sum((x) => x.perrCnt);

                    //상자수 체크시 매수 표현을 위해 통수 곱해줌
                    if (SettingJsonManager.boxCheck)
                    {
                        TempTotalCount *= int.Parse(mainModel.pCnt.ToString());
                        TempTotalErrorCount *= int.Parse(mainModel.pCnt.ToString());
                    }


                    if (consumerList.Where((x) => x.pworkKey.ToString() == SettingJsonManager.pworkkey && (!string.IsNullOrEmpty(x.sDatetime.ToString()) && (string.IsNullOrEmpty(x.eDatetime.ToString())))).Count() > 0)
                    {
                        //실수요자 작업 진행 중일 때는 아무 작업 하지 않음
                    }
                    else
                    {
                        //생산 결과량 또는 불량수량 있을때
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
                else
                {
                    arrChkAED = "3|"; //삭제
                }

                if (arrChkAED != "")
                {
                    string refPworkKey = string.Format("{0}", SettingJsonManager.pworkkey);
                    PopresultEnd(Convert.ToInt32(TempTotalCount), Convert.ToInt32(TempTotalErrorCount), arrChkAED, SettingJsonManager.pworkkey);

                    //수정이면 생산종료 시간 기록
                    if (arrChkAED == "2|")
                    {
                        PopResultEndTimeSetting(refPworkKey);
                    }
                }
            }
            else
            {

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
                DateTimeManager.GetSelectedStr()
                },
                1

                );

            if (returnData.결과 == 1)
            {
                LogManager.Info("spUdt431processinfo_workTimeChange 실행 완료", MethodBase.GetCurrentMethod().DeclaringType);
            }
            else
            {
                LogManager.Info("spUdt431processinfo_workTimeChange 실행 실패", MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        //원격연결 버튼
        private void Remote_click(object sender, MouseButtonEventArgs e)
        {
            LogManager.Info("원격연결 버튼 클릭", MethodBase.GetCurrentMethod().DeclaringType);
            _ = Process.Start("http://2007.asw.co.kr/client/index.jsp?cpcode=icd&autostart=true");
        }
    }
}
