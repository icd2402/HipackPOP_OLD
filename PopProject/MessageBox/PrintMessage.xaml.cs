using Monitor.Util;
using Newtonsoft.Json.Linq;
using PopProject.Consumer_;
using PopProject.Detail;
using PopProject.Input;
using PopProject.KeyBoard;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace PopProject.MessageBox
{
    /// <summary>
    /// PrintMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PrintMessage : Window
    {
        MainModel mainModel;
        DetailModel detailitem;
        private Win_KeyBoard win_KeyBoard;
        private ConsumerModel consumerModel;
        bool consumCheck;

        public PrintMessage(MainModel main, DetailModel detailitem)
        {
            InitializeComponent();
            copy_count.Text = "1";
            mainModel = main;
            this.detailitem = detailitem;

            if (SettingJsonManager.keyinCheck)
            {
                copy_count.Focusable = true;
                _ = copy_count.Focus();
                _ = copy_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));

                box_count.Focusable = true;
                _ = box_count.Focus();
                _ = box_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));
            }

            //오주, 대세, 구룡
            if (SettingJsonManager.corp_id.Equals("3125247246") || SettingJsonManager.corp_id.Equals("C91950615") || SettingJsonManager.corp_id.Equals("766562426")) // 오주포장
            {
                //팔레트당 적재수량 표기 
                pallet.Visibility = Visibility.Visible;

                //각종 버튼 위치 이동
                //print_stack.Margin = new Thickness(0, 0, 0, 0);
                //set_count.Margin = new Thickness(0, 20, 0, 0);
            }
            else//그외 업체
            {
                //각종 버튼 위치 이동
                print_stack.Margin = new Thickness(0, 0, 0, 0);
                set_count.Margin = new Thickness(0, 20, 0, 0);
            }

        }

        public PrintMessage(MainModel main, DetailModel detailitem, ConsumerModel consumerModel,bool consumCheck) 
        {
            InitializeComponent();
            this.consumCheck = consumCheck;
            copy_count.Text = "1";
            mainModel = main;
            this.detailitem = detailitem;

            //설정에서 키인사용 체크시
            if (SettingJsonManager.keyinCheck)
            {
                copy_count.Focusable = true;
                _ = copy_count.Focus();
                _ = copy_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));

                box_count.Focusable = true;
                _ = box_count.Focus();
                _ = box_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));
            }

            //오주, 대세, 구룡
            if (SettingJsonManager.corp_id.Equals("3125247246") || SettingJsonManager.corp_id.Equals("C91950615") || SettingJsonManager.corp_id.Equals("766562426")) // 오주포장
            {
                //팔레트당 적재수량 표기 
                pallet.Visibility = Visibility.Visible;

                //각종 버튼 위치 이동
                //print_stack.Margin = new Thickness(0, 0, 0, 0);
                //set_count.Margin = new Thickness(0, 20, 0, 0);
            }
            else//그외 업체
            {
                //각종 버튼 위치 이동
                print_stack.Margin = new Thickness(0, 0, 0, 0);
                set_count.Margin = new Thickness(0, 20, 0, 0);
            }

            this.consumerModel = consumerModel;
        }

        //인쇄수랑 - 버튼
        private void Result_count_minus(object sender, MouseButtonEventArgs e)
        {
            int value = 1;
            if (int.Parse(copy_count.Text) - value >= 1)
            {
                copy_count.Text = int.Parse(copy_count.Text) - value + "";
            }
        }

        //인쇄수량 +버튼
        private void Result_count_plus(object sender, MouseButtonEventArgs e)
        {
            int value = 1;
            copy_count.Text = int.Parse(copy_count.Text) + value + "";
        }

        //닫기 버튼
        private void Messagebox_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //비고란 입력 버튼
        private void Memo_button(object sender, MouseButtonEventArgs e)
        {
            PrintMemoBox printMemoBox = new PrintMemoBox(mainModel, this.detailitem);
            printMemoBox.ShowDialog();
        }

        //인쇄 버튼
        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                ReturnModel returnData = ServiceModel.GetData(
                    SettingJsonManager.serverIndex,
                     "[sp200PrintReport_Product]",
                      new string[] { "@생산계획키" },
                      new object[] { mainModel.pplanKey },
                      0
                      );

                JArray SignCorpinfo_Base_JArray = null/* TODO Change to default(_) if this is not a reference type */;
                if (returnData.결과 == 1)
                {
                    SignCorpinfo_Base_JArray = JArray.Parse(returnData.리턴값);
                }

                if (SignCorpinfo_Base_JArray != null && SignCorpinfo_Base_JArray.ToArray().Length > 0)
                {
                    ClsUc_명세서_Report 명세서양식_Uc = new ClsUc_명세서_Report();
                    ClsUc_명세서_Report2 명세서양식_Uc2 = new ClsUc_명세서_Report2();
                    ClsUc_명세서_Report3 명세서양식_Uc3 = new ClsUc_명세서_Report3();
                    ClsUc_명세서_Report5두성 명세서양식_uc5두성 = new ClsUc_명세서_Report5두성();
                    ClsUc_명세서_Report6구룡 명세서양식_uc6구룡 = new ClsUc_명세서_Report6구룡();
                    ClsUc_명세서_Report7구룡실수요자 명세서양식_uc7구룡실수요자 = new ClsUc_명세서_Report7구룡실수요자();
                    ClsUc_명세서_Report8대세 명세서양식_uc8대세 = new ClsUc_명세서_Report8대세();

                    ClsUc_완제품명세서_Control1 완제품명세서_Uc1 = new ClsUc_완제품명세서_Control1();
                    ClsUc_완제품명세서_Control2 완제품명세서_Uc2 = new ClsUc_완제품명세서_Control2();
                    ClsUc_완제품명세서_Control3 완제품명세서_Uc3 = new ClsUc_완제품명세서_Control3();
                    ClsUc_완제품명세서_Control5두성 완제품명세서_두성 = new ClsUc_완제품명세서_Control5두성();
                    ClsUc_완제품명세서_Control6구룡 완제품명세서_구룡 = new ClsUc_완제품명세서_Control6구룡();
                    ClsUc_완제품명세서_Control7구룡실수요자 완제품명세서_구룡실수요자 = new ClsUc_완제품명세서_Control7구룡실수요자();
                    ClsUc_완제품명세서_Control8대세 완제품명세서_대세 = new ClsUc_완제품명세서_Control8대세();

                    ClsUc_완제품명세서_Control1.ProductReport pr1 = new ClsUc_완제품명세서_Control1.ProductReport();
                    ClsUc_완제품명세서_Control2.ProductReport pr2 = new ClsUc_완제품명세서_Control2.ProductReport();
                    ClsUc_완제품명세서_Control3.ProductReport pr3 = new ClsUc_완제품명세서_Control3.ProductReport();
                    ClsUc_완제품명세서_Control5두성.ProductReport pr5 = new ClsUc_완제품명세서_Control5두성.ProductReport();
                    ClsUc_완제품명세서_Control6구룡.ProductReport pr6 = new ClsUc_완제품명세서_Control6구룡.ProductReport();
                    ClsUc_완제품명세서_Control7구룡실수요자.ProductReport pr7 = new ClsUc_완제품명세서_Control7구룡실수요자.ProductReport();
                    ClsUc_완제품명세서_Control8대세.ProductReport pr8 = new ClsUc_완제품명세서_Control8대세.ProductReport();

                    JToken result = SignCorpinfo_Base_JArray.First;

                    //선호칼라팩
                    //if (SettingJsonManager.corp_id.Equals("75BB212362"))
                    if (SettingJsonManager.corp_name.Equals("선호칼라팩") || SettingJsonManager.corp_name.Equals("선호칼라팩테스트"))
                    {
                        pr2.MycustomSName = "";                                       //업체명
                        pr2.corpName = "";                                            //거래처 상호
                        pr2.customSName = result["customSName"].ToString();           //거래처명
                        pr2.prodName = result["prodName"].ToString();                 //품명
                        pr2.orderLast = Convert.ToDateTime(result["orderLast"]);      //납기일
                        pr2.pSize = result["pSize"].ToString();                       //규격
                        string memo = getMemo(SettingJsonManager.printItem); //출력정보
                        pr2.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo); //비고 + 출력정보 
                        pr2.pworkDate = DateTime.Now; //작성일자
                        pr2.resultCount = result["resultCount"].ToString(); //총수량
                        pr2.pStockKey = result["pStockKey"].ToString();     //재고키
                        int keyLenth = 0;                                   //재고키 변수

                        keyLenth = 10 - pr2.pStockKey.Length;               // 10 - 재고키 길이
                        for (int index = 1; index <= keyLenth; index++)
                        {
                            pr2.pStockKey = "0" + pr2.pStockKey;
                        }

                        //총수량 1이상
                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr2.resultCount = mainModel.sumPworkCount + "";
                        }

                        //총수량 표현 미체크시 값 제거
                        if (!SettingJsonManager.count_check)
                        {
                            pr2.resultCount = "";
                        }
                    }
                    else //선호 제외 업체
                    {
                        //거래처 표현 체크시 표현x
                        pr1.corpName = SettingJsonManager.accountCheck ? "" : mainModel.corpName;

                        //생산처 표현 미체시 표현x
                        //생산처 표현 미체크시 우측 최하단에 업체명 기입
                        //대양포장은 생산처 미체크라도 무조건 안보이게
                        pr1.MycustomSName = SettingJsonManager.productionCheck ? "" : SettingJsonManager.corp_name.Equals("대양포장") ? "    " : SettingJsonManager.corp_name;
                    }

                    

                    


                    FixedDocument FixedDocument = new FixedDocument();
                    double Dbl_HeightReportPageWidthSize = 8.2 * 96;
                    double Dbl_HeightReportPageHeightSize = 12 * 96;

                    // 양식 유저 컨트롤 페이지 넓이 사이즈
                    명세서양식_Uc.Width = Dbl_HeightReportPageWidthSize;
                    // 양식 유저 컨트롤 페이지 높이 사이즈
                    명세서양식_Uc.Height = Dbl_HeightReportPageHeightSize;

                    // 새로운 픽셀페이지 객체 생성
                    FixedPage Set_FixedPage = new FixedPage
                    {
                        // 페이지 넓이 사이즈 
                        Width = Dbl_HeightReportPageWidthSize,

                        // 페이지 높이 사이즈 
                        Height = Dbl_HeightReportPageHeightSize
                    };

                    //유창포장
                    if (SettingJsonManager.corp_name.Equals("유창포장") || SettingJsonManager.corp_name.Equals("유창포장테스트"))
                    {
                        pr2.corpName = SettingJsonManager.accountCheck ? "" : mainModel.corpName; //거래처 상호
                        pr2.MycustomSName = SettingJsonManager.productionCheck ? "" : SettingJsonManager.corp_name; //생산처
                        pr2.customSName = result["customSName"].ToString();      //거래처명
                        pr2.prodName = result["prodName"].ToString();            //품명
                        pr2.orderLast = Convert.ToDateTime(result["orderLast"]); //납기일
                        pr2.pSize = result["pSize"].ToString();                  //규격

                        string memo = getMemo(SettingJsonManager.printItem); //프린트 출력정보
                        pr2.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo); //비고 + 출력정보
                        pr2.pworkDate = Convert.ToDateTime(result["pworkDate"]); //작업일자
                        pr2.pworkDate = DateTime.Now;                            //작성일자
                        pr2.resultCount = result["resultCount"].ToString();      //총수량
                        pr2.pStockKey = result["pStockKey"].ToString();          //재고키

                        int keyLenth = 0;                     //재고키 변수
                        keyLenth = 10 - pr2.pStockKey.Length; // 10 - 재고키 길이

                        for (int index = 1; index <= keyLenth; index++)
                        {
                            pr2.pStockKey = "0" + pr2.pStockKey;
                        }

                        //총수량 1이상
                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr2.resultCount = mainModel.sumPworkCount + "";
                        }

                        //총수량 표현 미체크
                        if (!SettingJsonManager.count_check)
                        {
                            pr2.resultCount = "";
                        }

                        완제품명세서_Uc2.ViewModel = pr2;
                        완제품명세서_Uc2.Set_BarCode();
                        Grid.SetRow(완제품명세서_Uc2, 0);

                        명세서양식_Uc2.ItemPanel.Children.Add(완제품명세서_Uc2);
                        Set_FixedPage.Children.Add(명세서양식_Uc2 as UIElement);
                    }

                    //오주포장
                    else if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
                    {

                        pr3.corpName = SettingJsonManager.accountCheck ? "" : mainModel.corpName; //거래처 상호
                        pr3.MycustomSName = SettingJsonManager.productionCheck ? "" : SettingJsonManager.corp_name; //생산처
                        pr3.customSName = result["customSName"].ToString();           //거래처명
                        pr3.prodName = result["prodName"].ToString();                 //품명
                        pr3.orderLast = Convert.ToDateTime(result["orderLast"]);      //납기일
                        pr3.pSize = result["pSize"].ToString();                       //규격
                        pr3.pGol = mainModel.paperGor;                                //골
                        pr3.PalletType = detailitem.PalletType;                       //파렛트 종류
                        string memo = getMemo(SettingJsonManager.printItem); //프린트 출력정보
                        pr3.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo); //비고 + 출력정보
                        //pr3.pworkDate = Convert.ToDateTime(result["pworkDate"]).ToString("yyyy/MM/dd"); //작업일자
                        pr3.pworkDate = DateTime.Today.ToString("yyyy/MM/dd"); //작성일자
                        pr3.pStockKey = result["pStockKey"].ToString();        //재고키
                  
                        int keyLenth = 0;                     //재고키 변수
                        keyLenth = 10 - pr3.pStockKey.Length; // 10 - 재고키 길이

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr3.pStockKey = "0" + pr3.pStockKey;
                        }

                        //팔레트당 적재수량 기입시
                        if (box_count.Text.Length == 0)
                        {
                            pr3.resultCount = "";
                        }
                        else
                        {
                            pr3.resultCount = box_count.Text;
                        }

                        완제품명세서_Uc3.ViewModel = pr3;
                        완제품명세서_Uc3.Set_BarCode();
                        Grid.SetRow(완제품명세서_Uc3, 1);

                        명세서양식_Uc3.ItemPanel.Children.Add(완제품명세서_Uc3);
                        Set_FixedPage.Children.Add(명세서양식_Uc3 as UIElement);
                    }

                    //두성포장, 혜성수출포장
                    else if(SettingJsonManager.corp_name.Equals("두성포장") || SettingJsonManager.corp_name.Equals("두성포장테스트") || SettingJsonManager.corp_name.Contains("혜성"))
                    {

                        pr5.customSName = result["customSName"].ToString();      //거래처명
                        pr5.prodName = result["prodName"].ToString();            //품명
                        pr5.orderLast = Convert.ToDateTime(result["orderLast"]); //납기일
                        pr5.pSize = result["pSize"].ToString();                  //규격
                        
                        string memo = "";
                        memo = getMemo(SettingJsonManager.printItem);   //프린트 출력정보  

                        pr5.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo); //비고 + 출력정보
                        pr5.pworkDate = DateTime.Today.ToString("yyyy/MM/dd"); //작성일자
                        pr5.resultCount = result["resultCount"].ToString();    //총수량
                        pr5.pStockKey = result["pStockKey"].ToString();        //재고키
                        pr5.loadCount = "";                                    //파렛트당 적재수량
                        pr5.LocationName = detailitem.LocationName;            //적재위치이름
                        pr5.LoadHeight = detailitem.LoadHeight;                //적재단수
                        pr5.LoadType = detailitem.LoadType;                    //적재방법

                        //파렛트당 적재수량
                        if (box_count.Text.ToString().Length > 0)
                        {
                            pr5.loadCount = box_count.Text;
                        }

                        int keyLenth = 0;                     //재고키 변수
                        keyLenth = 10 - pr5.pStockKey.Length; // 10 - 재고키 길이

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr5.pStockKey = "0" + pr5.pStockKey;
                        }

                        //총수량 1이상
                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr5.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                        }

                        //총수량 표현 미체크
                        if (!SettingJsonManager.count_check)
                        {
                            pr5.resultCount = "";
                        }
                        완제품명세서_두성.ViewModel = pr5;
                        완제품명세서_두성.Set_BarCode();
                        Grid.SetRow(완제품명세서_두성, 1);
                        명세서양식_uc5두성.ItemPanel.Children.Add(완제품명세서_두성);
                        Set_FixedPage.Children.Add(명세서양식_uc5두성 as UIElement);
                    }

                    //구룡산업
                    else if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
                    {
                        //실수요자
                        if (consumCheck)
                        {
                            pr7.customSName = result["customSName"].ToString();      //거래처명
                            pr7.vendorName = detailitem.vendorName;                  //납품처명
                            pr7.membName = consumerModel.membName;                   //이름
                            pr7.adress = consumerModel.adress;                       //주소
                            pr7.PhonNo = consumerModel.PhonNo;                       //연락처
                            pr7.prodName = result["prodName"].ToString();            //제품명
                            pr7.orderLast = Convert.ToDateTime(result["orderLast"]); //납기일
                            pr7.orderLast1 = pr6.orderLast.ToString("yyyy.MM.dd");   //납기일
                            pr7.pSize = result["pSize"].ToString();                  //규격
                            
                            string memo = "";
                            memo = getMemo(SettingJsonManager.printItem);   //프린트 출력정보
                            pr7.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                            pr7.pMemo = pr7.pMemo + Environment.NewLine + consumerModel.Memo;

                            pr7.pworkDate = DateTime.Today.ToString("yyyy/MM/dd"); //작성일자
                            pr7.LocationName = detailitem.LocationName;            //적재위치 
                            pr7.loadHeight = detailitem.LoadHeight;                //적재단수
                            pr7.loadType = detailitem.LoadType;                    //적재방법
                            pr7.loadCount = "";                                    //파렛트당 적재수량
                            if (box_count.Text.ToString().Length > 0)
                            {
                                pr7.loadCount = box_count.Text; 
                            }

                            pr7.pStockKey = result["pStockKey"].ToString();        //재고키
                            int keyLenth = 0;
                            keyLenth = 10 - pr7.pStockKey.Length;

                            for (var index = 1; index <= keyLenth; index++)
                            {
                                pr7.pStockKey = "0" + pr6.pStockKey;
                            }

                            pr6.pplanCount = int.Parse(mainModel.pplanCount.ToString());//계획량
                            pr7.resultCount = consumerModel.orderCount.ToString();//총수량
                            if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                            {
                                pr7.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                            }
                            //총수량 표현 미체크
                            if (!SettingJsonManager.count_check)
                            {
                                pr7.resultCount = "";
                            }
                            완제품명세서_구룡실수요자.ViewModel = pr7;
                            완제품명세서_구룡실수요자.Set_BarCode(mainModel.orderKey.ToString());
                            Grid.SetRow(완제품명세서_구룡실수요자, 1);
                            명세서양식_uc7구룡실수요자.ItemPanel.Children.Add(완제품명세서_구룡실수요자);
                            Set_FixedPage.Children.Add(명세서양식_uc7구룡실수요자 as UIElement);


                        }
                        else //일반생산
                        {
                            pr6.customSName = result["customSName"].ToString();      //거래처명
                            pr6.vendorName = detailitem.vendorName;                  //납품처명
                            pr6.prodName = result["prodName"].ToString();            //품명
                            pr6.orderLast = Convert.ToDateTime(result["orderLast"]); //납기일
                            pr6.orderLast1 = pr6.orderLast.ToString("yyyy.MM.dd");   //납기일
                            pr6.pSize = result["pSize"].ToString();                  //규격
                           
                            Console.WriteLine("비고란 정보 : " + SettingJsonManager.printItem);
                            string memo = "";                                        
                            memo = getMemo(SettingJsonManager.printItem);   //프린트 출력정보
                            pr6.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                           
                            pr6.pworkDate = DateTime.Today.ToString("yyyy/MM/dd"); //작성일자
                            pr6.pStockKey = result["pStockKey"].ToString();        //재고키
                            pr6.loadCount = "";                                    //파렛트당 적재수량
                            pr6.LocationName = detailitem.LocationName;            //적재위치
                            pr6.loadHeight = detailitem.LoadHeight;                //적재단수
                           
                            pr6.loadType = detailitem.LoadType;                    //적재방법
                            if (box_count.Text.ToString().Length > 0)
                            {
                                pr6.loadCount = box_count.Text;
                            }
                           
                           
                            int keyLenth = 0;                                      //재고키
                            keyLenth = 10 - pr6.pStockKey.Length;   
                           
                            for (var index = 1; index <= keyLenth; index++)
                            {
                                pr6.pStockKey = "0" + pr6.pStockKey;
                            }
                           
                            pr6.pplanCount = int.Parse(mainModel.pplanCount.ToString());//계획량
                            pr6.resultCount = result["resultCount"].ToString();    //총수량
                            if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                            {
                                pr6.resultCount = mainModel.sumPworkCount == 0 ? "" : Convert.ToInt32(mainModel.sumPworkCount).ToString("N0");
                            }
                            //총수량 표현 미체크
                            if (!SettingJsonManager.count_check)
                            {
                                pr6.resultCount = "";
                            }
                           
                            완제품명세서_구룡.ViewModel = pr6;
                            완제품명세서_구룡.Set_BarCode(mainModel.orderKey.ToString());
                            Grid.SetRow(완제품명세서_구룡, 1);
                            명세서양식_uc6구룡.ItemPanel.Children.Add(완제품명세서_구룡);
                            Set_FixedPage.Children.Add(명세서양식_uc6구룡 as UIElement);
                        }
                    }

                    //대세물산
                    else if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
                    {
                        pr8.customSName = result["customSName"].ToString();      //거래처명
                        pr8.prodName = result["prodName"].ToString();            //품명
                        //if (result["prodName"].ToString().Length > 5)
                        //{
                        //    pr8.prodName = result["prodName"].ToString().Insert(5, " &#x0a; ");

                        //}
                        //else
                        //{
                        //    pr8.prodName = result["prodName"].ToString();
                        //}

                        pr8.orderLast = Convert.ToDateTime(result["orderLast"]); //납기일
                        pr8.pSize = result["pSize"].ToString();                  //규격
                        //pr8.MycustomSName = SettingJsonManager.corp_name; //생산처
                        pr8.MycustomSName = SettingJsonManager.productionCheck ? "" : SettingJsonManager.corp_name; //생산처 상호
                        pr8.corpName = SettingJsonManager.accountCheck ? "" : mainModel.customSName; //거래처 상호


                        string memo = "";
                        memo = getMemo(SettingJsonManager.printItem);

                        pr8.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);

                        pr8.pworkDate = DateTime.Today.ToString("yyyy/MM/dd"); //작성일자
                        pr8.loadCount = "";                                    //파레트당 적재수량
                        if (box_count.Text.ToString().Length > 0)
                        {
                            pr8.loadCount = box_count.Text;
                        }


                        pr8.pStockKey = result["pStockKey"].ToString();        //재고키
                        int keyLenth = 0;
                        keyLenth = 10 - pr8.pStockKey.Length;

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr8.pStockKey = "0" + pr8.pStockKey;
                        }

                        pr8.resultCount = result["resultCount"].ToString();    //총수량
                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr8.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                        }
                        //총수량 표현 미체크
                        if (!SettingJsonManager.count_check)
                        {
                            pr8.resultCount = "";
                        }

                        완제품명세서_대세.ViewModel = pr8;
                        완제품명세서_대세.Set_BarCode(mainModel.orderKey.ToString());
                        Grid.SetRow(완제품명세서_대세, 1);
                        명세서양식_uc8대세.ItemPanel.Children.Add(완제품명세서_대세);
                        Set_FixedPage.Children.Add(명세서양식_uc8대세 as UIElement);
                    }
                    else //(선호),유창,오주,두성,혜성,구룡 제외
                    {
                        pr1.customSName = result["customSName"].ToString();      //거래처명
                        pr1.prodName = result["prodName"].ToString();            //품명
                        pr1.orderLast = Convert.ToDateTime(result["orderLast"]); //납기일
                        pr1.pSize = result["pSize"].ToString();                  //규격
                        
                        string memo = ""; 
                        //광무
                        if (SettingJsonManager.corp_name.Equals("광무포장"))
                        {
                            memo = detailitem.SpecialMemo + Environment.NewLine;
                            memo+= getMemo(SettingJsonManager.printItem);
                        }
                        else //그외 EX)성주,우리, 신우 등등
                        {
                            memo = getMemo(SettingJsonManager.printItem);
                        }
                        pr1.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        
                        pr1.pworkDate = DateTime.Today.ToString("yyyy/MM/dd"); //작성일자
                        pr1.loadCount = "";                                    //파레트당 적재수량
                        if (box_count.Text.ToString().Length > 0)
                        {
                            pr1.loadCount = box_count.Text;
                        }
                       
                        
                        pr1.pStockKey = result["pStockKey"].ToString();        //재고키
                        int keyLenth = 0; 
                        keyLenth = 10 - pr1.pStockKey.Length;

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr1.pStockKey = "0" + pr1.pStockKey;
                        }

                        pr1.resultCount = result["resultCount"].ToString();    //총수량
                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr1.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                        }
                        //총수량 표현 미체크
                        if (!SettingJsonManager.count_check)
                        {
                            pr1.resultCount = "";
                        }

                        완제품명세서_Uc1.ViewModel = pr1;
                        완제품명세서_Uc1.Set_BarCode();
                        Grid.SetRow(완제품명세서_Uc1, 1);
                        명세서양식_Uc.ItemPanel.Children.Add(완제품명세서_Uc1);
                        Set_FixedPage.Children.Add(명세서양식_Uc as UIElement);
                    }

                    // 양식 유저 컨트롤 픽셀 페이지 자식 형태로 저장
                    // 페이지 콘텐츠 객체 생성
                    PageContent Set_PageContent = new PageContent();

                    // 픽셀 페이지 페이지 콘텐츠 객체 자식 형태로 저장
                    (Set_PageContent as IAddChild).AddChild(Set_FixedPage);

                    // 픽셀 도큐먼트 페이지 콘텐츠 저장
                    FixedDocument.Pages.Add(Set_PageContent);

                    PrintWindow reportWindow = new PrintWindow();
                    reportWindow.PrintViewer.Document = FixedDocument;

                    if (SettingJsonManager.corp_name.Equals("대세물산"))
                    {
                        reportWindow.가로세로 = 0; //가로 인쇄
                    }
                    else
                    {
                        reportWindow.가로세로 = 1; //세로 인쇄
                    }
                    reportWindow.방향.IsEnabled = false;
                    reportWindow.엑셀.IsEnabled = false;
                    reportWindow.Opacity = 0;
                    reportWindow.Show();

                    //오주포장
                    if (SettingJsonManager.corp_id.Equals("3125247246"))
                    {
                        //출력매수 1일때
                        if (int.Parse(copy_count.Text) == 1)
                        {
                            reportWindow.DirectPrint(int.Parse(copy_count.Text));
                            reportWindow.Close();
                        }
                        else //여러개 일때
                        {
                            reportWindow.DirectPrint(int.Parse(copy_count.Text) - 1);
                            reportWindow.Close();
                            Focus();
                            Close();
                            오주프린터(result["customSName"].ToString(),
                                result["prodName"].ToString(),
                                result["orderLast"].ToString(),
                                result["pSize"].ToString(),
                                result["pMemo"].ToString(),
                                DateTime.Today.ToString("yyyy/MM/dd"),
                                result["pStockKey"].ToString()
                                );
                        }
                    }
                    else//오주포장 제외
                    {
                        reportWindow.DirectPrint(int.Parse(copy_count.Text));
                        reportWindow.Close();
                        Focus();
                        Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void 오주프린터(string customSName, string prodName, string orderLast, string pSize,string pMemo, string pworkDate, string pStockKey)
        {
            ClsUc_완제품명세서_Control3 완제품명세서_Uc3 = new ClsUc_완제품명세서_Control3();
            ClsUc_명세서_Report3 명세서양식_Uc3 = new ClsUc_명세서_Report3();
            ClsUc_완제품명세서_Control3.ProductReport pr3 = new ClsUc_완제품명세서_Control3.ProductReport();
            FixedDocument FixedDocument = new FixedDocument();
            double Dbl_HeightReportPageWidthSize = 8.2 * 96;
            double Dbl_HeightReportPageHeightSize = 12 * 96;

            // 양식 유저 컨트롤 페이지 넓이 사이즈
            명세서양식_Uc3.Width = Dbl_HeightReportPageWidthSize;
            // 양식 유저 컨트롤 페이지 높이 사이즈
            명세서양식_Uc3.Height = Dbl_HeightReportPageHeightSize;

            // 새로운 픽셀페이지 객체 생성
            FixedPage Set_FixedPage = new FixedPage();

            // 페이지 넓이 사이즈 
            Set_FixedPage.Width = Dbl_HeightReportPageWidthSize;

            // 페이지 높이 사이즈 
            Set_FixedPage.Height = Dbl_HeightReportPageHeightSize;

            pr3.corpName = mainModel.corpName;
            pr3.MycustomSName = SettingJsonManager.productionCheck ? "" : SettingJsonManager.corp_name;
            pr3.customSName = customSName;
            pr3.prodName = prodName;
            pr3.orderLast = Convert.ToDateTime(orderLast);
            pr3.pSize = pSize;
            pr3.pGol = mainModel.paperGor;
            string memo = getMemo(SettingJsonManager.printItem);
            pr3.pMemo = pMemo + (string.IsNullOrEmpty(pMemo) ? memo : Environment.NewLine + memo);
            pr3.pworkDate = Convert.ToDateTime(pworkDate).ToString("yyyy/MM/dd");

            pr3.pStockKey = pStockKey;

            int keyLenth = 0;
            keyLenth = 10 - pr3.pStockKey.Length;

            for (var index = 1; index <= keyLenth; index++)
            {
                pr3.pStockKey = "0" + pr3.pStockKey;
            }

            pr3.resultCount = "";

            완제품명세서_Uc3.ViewModel = pr3;
            완제품명세서_Uc3.Set_BarCode();
            Grid.SetRow(완제품명세서_Uc3, 1);

            명세서양식_Uc3.ItemPanel.Children.Add(완제품명세서_Uc3);
            Set_FixedPage.Children.Add(명세서양식_Uc3 as UIElement);

            PageContent Set_PageContent = new PageContent();

            // 픽셀 페이지 페이지 콘텐츠 객체 자식 형태로 저장
            (Set_PageContent as IAddChild).AddChild(Set_FixedPage);

            // 픽셀 도큐먼트 페이지 콘텐츠 저장
            FixedDocument.Pages.Add(Set_PageContent);

            PrintWindow reportWindow = new PrintWindow();
            reportWindow.PrintViewer.Document = FixedDocument;
            reportWindow.가로세로 = 1;
            reportWindow.방향.IsEnabled = false;
            reportWindow.엑셀.IsEnabled = false;
            reportWindow.Opacity = 0;
            reportWindow.Show();
            reportWindow.DirectPrint(1);
            reportWindow.Close();
            Focus();
            Close();

        }

        //닫기버튼
        private void Close_messagebox(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //비고란에 자재명세서 출력정보 순차 입력
        private string getMemo(string printItem)
        {
            string memo = "";
            string[] print = printItem.Split(new string[] { "|" }, StringSplitOptions.None); //1|6|8|12

            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                //순서대로 정렬
                string[] numbers = printItem.Split('|');
                int[] intArray = Array.ConvertAll(numbers, int.Parse);
                Array.Sort(intArray);
                string sortedPrintItem = string.Join("|", intArray);
                print = sortedPrintItem.Split(new string[] { "|" }, StringSplitOptions.None);
            }

            for (int i = 0; i < print.Length; i++)
            {
                if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
                {
                    switch (print[i])
                    {

                        case "1": //접합방법
                            memo += string.IsNullOrEmpty(detailitem.ConnetingType) ? "" : detailitem.ConnetingType + " / ";
                            break;
                        case "2": //코팅
                            memo += string.IsNullOrEmpty(detailitem.CoatingType) ? "" : detailitem.CoatingType + " / ";
                            break;
                        case "3": //밴딩매수
                            memo += string.IsNullOrEmpty(detailitem.PackCount + "") ? "" : detailitem.PackCount + "매 / ";
                            break;
                        case "4": //적재방법
                            memo += string.IsNullOrEmpty(detailitem.LoadType) ? "" : detailitem.LoadType + " / ";
                            break;
                        case "5": //적재단수
                            memo += string.IsNullOrEmpty(detailitem.LoadHeight) ? "" : detailitem.LoadHeight + " / ";
                            break;
                        case "6": //파렛트 종류
                            memo += string.IsNullOrEmpty(detailitem.PalletType) ? "" : detailitem.PalletType + " / ";
                            break;
                        case "7": //손잡이
                            memo += string.IsNullOrEmpty(detailitem.handChk) ? "" : detailitem.handChk + " / ";
                            break;
                        case "8": //밴딩방법
                            memo += string.IsNullOrEmpty(detailitem.PackType) ? "" : detailitem.PackType + " / ";
                            break;
                        case "9": //목형No
                            memo += string.IsNullOrEmpty(detailitem.PushingNo) ? "" : detailitem.PushingNo + " / "; ;
                            break;
                        
                    }
                }
                else
                {
                    //3번째 마다 줄바꾸기(Enter)
                    if (i != 0 && i % 3 == 0)
                    {
                        memo += "\n";
                    }

                    switch (print[i])
                    {
                        case "1": //필름 No
                            memo += string.IsNullOrEmpty(detailitem.filmNo) ? "" : detailitem.filmNo + " / ";
                            break;
                        case "2": //코팅
                            memo += string.IsNullOrEmpty(detailitem.CoatingType) ? "" : detailitem.CoatingType + " / ";
                            break;
                        case "3": //조판No
                            memo += string.IsNullOrEmpty(detailitem.PrintNo) ? "" : detailitem.PrintNo + " / ";
                            break;
                        case "4": //조판위치
                            memo += string.IsNullOrEmpty(detailitem.Printposition) ? "" : detailitem.Printposition + " / "; ;
                            break;
                        case "5": //목형No
                            memo += string.IsNullOrEmpty(detailitem.PushingNo) ? "" : detailitem.PushingNo + " / "; ;
                            break;
                        case "6": //목형위치
                            memo += string.IsNullOrEmpty(detailitem.Pushingposition) ? "" : detailitem.Pushingposition + " / ";
                            break;
                        case "7": //인쇄랙도
                            memo += string.IsNullOrEmpty(detailitem.ColorType) ? "" : detailitem.ColorType + " / ";
                            break;
                        case "8": //접합방법
                            memo += string.IsNullOrEmpty(detailitem.ConnetingType) ? "" : detailitem.ConnetingType + " / ";
                            break;
                        case "9": //손잡이
                            memo += string.IsNullOrEmpty(detailitem.handChk) ? "" : detailitem.handChk + " / ";
                            break;
                        case "10": //포장매수
                            memo += string.IsNullOrEmpty(detailitem.PackCount + "") ? "" : detailitem.PackCount + "매 / ";
                            break;
                        case "11": //포장방법
                            memo += string.IsNullOrEmpty(detailitem.PackType) ? "" : detailitem.PackType + " / ";
                            break;
                        case "12": //파렛트 종류
                            memo += string.IsNullOrEmpty(detailitem.PalletType) ? "" : detailitem.PalletType + " / ";
                            break;
                        case "13": //적재단수
                            memo += string.IsNullOrEmpty(detailitem.LoadHeight) ? "" : detailitem.LoadHeight + " / ";
                            break;
                        case "14": //적재방법
                            memo += string.IsNullOrEmpty(detailitem.LoadType) ? "" : detailitem.LoadType + " / ";
                            break;
                        case "15": //메모
                            memo += string.IsNullOrEmpty(detailitem.UserMemo) ? "" : detailitem.LoadType + " / ";
                            break;
                    }
                }
            }

            return memo;
        }


        //파레트당 적재 수량클릭시 키보드 팝업
        private void pallet_count(object sender, MouseButtonEventArgs e)
        {
            //키인사용 체크시
            if (SettingJsonManager.keyinCheck)
            {
                box_count.Focusable = true;
                _ = box_count.Focus();
                _ = box_count.Dispatcher.BeginInvoke(new Action(() => { box_count.SelectAll(); }));
            }
            else //미체크
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context) {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = 650
                };
                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }

        //출력매수 수량 클릭시 키보드 팝업
        private void Copy_count_click(object sender, MouseButtonEventArgs e)
        {
            //키인사용 체크시
            if (SettingJsonManager.keyinCheck)
            {
                copy_count.Focusable = true;
                _ = copy_count.Focus();
                _ = copy_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));
            }
            else //미체크
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context)
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = 650
                };

                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }
    }
}
