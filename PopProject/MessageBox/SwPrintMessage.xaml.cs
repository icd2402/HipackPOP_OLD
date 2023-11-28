using Monitor.Util;
using Newtonsoft.Json.Linq;
using PopProject.Detail;
using PopProject.KeyBoard;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace PopProject.MessageBox
{
    /// <summary>
    /// SwPrintMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SwPrintMessage : Window
    {
        ClsUc_완제품명세서_Control4.ProductReport pr1 = new ClsUc_완제품명세서_Control4.ProductReport();
        ClsUc_완제품명세서_Control4 완제품명세서_Uc1 = new ClsUc_완제품명세서_Control4();
        ClsUc_명세서_Report2 명세서양식_Uc = new ClsUc_명세서_Report2();
        FixedPage Set_FixedPage;
        FixedDocument FixedDocument;

        ClsUc_완제품명세서_Control4.ProductReport pr11 = new ClsUc_완제품명세서_Control4.ProductReport();
        ClsUc_완제품명세서_Control4 완제품명세서_Uc11 = new ClsUc_완제품명세서_Control4();
        ClsUc_명세서_Report2 명세서양식_Uc1 = new ClsUc_명세서_Report2();
        FixedPage Set_FixedPage1;
        FixedDocument FixedDocument1;

        MainModel mainModel;
        DetailModel detailitem;
        PrintWindow reportWindow;

        private readonly int metrialBJKey;
        private readonly string scmServerIndex;
        private readonly string scmDBName;

        public SwPrintMessage(MainModel main, DetailModel detailitem)
        {
            InitializeComponent();
            mainModel = main;
            this.detailitem = detailitem;
            total_count.Text = mainModel.pplanCount.ToString(); //총수량
            box_Count.Text = mainModel.pplanCount.ToString(); //적재수량
            
            //거래처 공백 체크
            if (SettingJsonManager.accountCheck)
            {
                account_check.IsChecked = true;
            }

            //생산처 공백 체크
            if (SettingJsonManager.productionCheck)
            {
                production_check.IsChecked = true;
            }

            //부착수 콤보박스
            SetComboBox();
        }

        //부착수 콤보박스
        private void SetComboBox()
        {
            ObservableCollection<string> ob = new ObservableCollection<string>();
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        ob.Add("1");
                        break;
                    case 1:
                        ob.Add("2");
                        break;
                    case 2:
                        ob.Add("4");
                        break;
                }
            }
            combo.ItemsSource = ob;
            combo.SelectedIndex = 0;
        }

     
        //적재수량 클릭
        private void Pallet_count(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                //키보드 호출
                TextBox context = (TextBox)sender;
                Win_KeyBoard win_KeyBoard = new Win_KeyBoard(context)
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = 300
                };
                _ = win_KeyBoard.ShowDialog();

                float selectedItem = float.Parse(combo.SelectedItem.ToString());
                float copyCount = box_Count.Text.ToString().Equals("0")
                    ? selectedItem
                    : (float)Math.Ceiling(float.Parse(total_count.Text.ToString()) / float.Parse(box_Count.Text.ToString())) * selectedItem;
                
                //출력매수 
                setCopyCount(copyCount);
            }
        }

        //취소 버튼
        private void Messagebox_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //인쇄 확인버튼
        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {
            //인쇄 수량 가입고 DB에 저장 
            try
            {
                string destSCMServerIndex = "";
                string destSCMDBName = "";
                int palletCount = 0;

                if (string.IsNullOrEmpty(scmServerIndex))
                {
                    destSCMServerIndex = SettingJsonManager.serverIndex;
                }
                else
                {
                    destSCMServerIndex = scmServerIndex;
                    destSCMDBName = scmDBName;
                }

                ServiceModel.ServiceModelClean(destSCMServerIndex, destSCMDBName);
                ReturnModel returnData = ServiceModel.GetData(
                    destSCMServerIndex,
                    "[PopSubscriptionBJCountUpdate]",
                    new string[] { "@metrialBJKey", "@palletCount" },
                    new object[] { this.metrialBJKey, palletCount
                     }, 1
                  );

                if (returnData.결과 == 1)
                {
                }
                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("팔레트 수량 저장 실패 , 알림");
                    _ = msg.ShowDialog();
                }
            }
            catch (Exception)
            {
            }


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
                JArray SignCorpinfo_Base_JArray = null
                    /* TODO Change to default(_) if this is not a reference type */;
                if (returnData.결과 == 1)
                {
                    SignCorpinfo_Base_JArray = JArray.Parse(returnData.리턴값);
                }
                if (SignCorpinfo_Base_JArray != null && SignCorpinfo_Base_JArray.ToArray().Length > 0)
                {
                    JToken result = SignCorpinfo_Base_JArray.First;
                   

                    pr1.corpName = (bool)account_check.IsChecked ? "" : mainModel.corpName;
                    pr1.MycustomSName = (bool)production_check.IsChecked ? "" : SettingJsonManager.corp_name.Equals("대양포장") ? "    " : SettingJsonManager.corp_name;

                    FixedDocument = new FixedDocument();
                    double Dbl_HeightReportPageWidthSize = 8.2 * 96;
                    double Dbl_HeightReportPageHeightSize = 12 * 96;
                    // 양식 유저 컨트롤 페이지 넓이 사이즈
                    명세서양식_Uc.Width = Dbl_HeightReportPageWidthSize;
                    // 양식 유저 컨트롤 페이지 높이 사이즈
                    명세서양식_Uc.Height = Dbl_HeightReportPageHeightSize;
                    // 새로운 픽셀페이지 객체 생성
                    Set_FixedPage = new FixedPage
                    {
                        // 페이지 넓이 사이즈 
                        Width = Dbl_HeightReportPageWidthSize,
                        // 페이지 높이 사이즈 
                        Height = Dbl_HeightReportPageHeightSize
                    };

                    pr1.customSName = result["customSName"].ToString();

                    if (SettingJsonManager.corp_name.Equals("신우수출포장"))
                    {
                        //거래처 명
                        switch (result["customSName"].ToString().Trim())
                        {
                            case "우리포장산업":
                            case "누리팩":
                            case "유램":
                            case "지온":
                            case "채움팩":
                            case "(주)우리포장산업":
                            case "주식회사누리팩":
                            case "주식회사유램":
                            case "주식회사지온":
                            case "주식회사채움팩":
                                pr1.customSName = "";
                                pr1.MycustomSName = "";
                                pr1.corpName = "";
                                break;

                            default:
                                pr1.customSName = result["customSName"].ToString();
                                pr1.MycustomSName = SettingJsonManager.corp_name;
                                break;

                        }
                    }
                    else if (SettingJsonManager.corp_name.Equals("우리포장산업"))
                    {
                        //거래처 명
                        switch (result["customSName"].ToString().Trim())
                        {
                            case "신우수출포장":
                            case "누리팩":
                            case "유램":
                            case "지온":
                            case "채움팩":
                            case "(주)신우수출포장":
                            case "주식회사누리팩":
                            case "주식회사유램":
                            case "주식회사지온":
                            case "주식회사채움팩":
                                pr1.customSName = "";
                                pr1.MycustomSName = "";
                                pr1.corpName = "";
                                break;

                            default:
                                pr1.customSName = result["customSName"].ToString();
                                pr1.MycustomSName = SettingJsonManager.corp_name;
                                break;
                        }
                    }
                    else if (SettingJsonManager.corp_name.Equals("누리팩"))
                    {
                        //거래처 명
                        switch (result["customSName"].ToString().Trim())
                        {
                            case "우리포장산업":
                            case "신우수출포장":
                            case "누리팩":
                            case "유램":
                            case "지온":
                            case "채움팩":
                            case "(주)신우수출포장":
                            case "주식회사누리팩":
                            case "주식회사유램":
                            case "주식회사지온":
                            case "주식회사채움팩":
                                pr1.customSName = "";
                                pr1.MycustomSName = "";
                                pr1.corpName = "";
                                break;

                            default:
                                pr1.customSName = result["customSName"].ToString();
                                pr1.MycustomSName = SettingJsonManager.corp_name;
                                break;
                        }
                    }

                    pr1.prodName = result["prodName"].ToString();
                    pr1.orderLast = Convert.ToDateTime(result["orderLast"]);
                    pr1.pSize = result["pSize"].ToString();

                    string memo = getMemo(SettingJsonManager.printItem);
                    pr1.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                    pr1.pworkDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd");
                    pr1.resultCount = result["resultCount"].ToString();
                    pr1.pStockKey = result["pStockKey"].ToString();

                    int keyLenth = 0;
                    keyLenth = 10 - pr1.pStockKey.Length;

                    for (var index = 1; index <= keyLenth; index++)
                    {
                        pr1.pStockKey = "0" + pr1.pStockKey;
                    }

                    if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                    {
                        if (mainModel.sumPworkCount == 0)
                        {
                            pr1.resultCount = "";
                        }
                        else
                        {
                            pr1.resultCount = mainModel.sumPworkCount + "";
                        }
                    }

                    if (!SettingJsonManager.count_check)
                    {
                        pr1.resultCount = "";
                    }
                    pr1.corpName = (bool)account_check.IsChecked ? "" : mainModel.corpName;
                    pr1.MycustomSName = (bool)production_check.IsChecked ? "" : SettingJsonManager.corp_name.Equals("대양포장") ? "    " : SettingJsonManager.corp_name;

                    Print_(pr1.customSName,
                        result["prodName"].ToString(),
                        result["orderLast"].ToString(),
                        result["pSize"].ToString(),
                        result["pMemo"].ToString(),
                        DateTime.Today.ToString("yyyy/MM/dd"),
                        result["pStockKey"].ToString()
                        );

                }

            }
            catch (Exception m)
            {
            }
            Close();
        }

        private void Print_(string customSName, string prodName, string orderLast, string pMemo, string pSize, string DateTime, string pStockKey)
        {
            //출력 매수가 1장일떄
            if (int.Parse(copy_count.Text) == 1)
            {
                //적재수량 0일때
                if (box_Count.Text.ToString().Equals("0"))
                {
                    pr1.loadCount = "";
                }
                else
                {
                    pr1.loadCount = box_Count.Text.ToString();
                }
                
                완제품명세서_Uc1.ViewModel = pr1;
                완제품명세서_Uc1.Set_BarCode();
                Grid.SetRow(완제품명세서_Uc1, 0);
                명세서양식_Uc.ItemPanel.Children.Add(완제품명세서_Uc1);
                Set_FixedPage.Children.Add(명세서양식_Uc as UIElement);

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
                reportWindow.DirectPrint(int.Parse(copy_count.Text));
                reportWindow.Close();
            }
            //출력매수 - 부착수량이 0일때
            else if (int.Parse(copy_count.Text.ToString()) - int.Parse(combo.SelectedItem.ToString()) == 0)
            {
                int remainder = int.Parse(box_Count.Text.ToString());
                setPrint(remainder);
                reportWindow.DirectPrint(int.Parse(copy_count.Text.ToString()));
                reportWindow.Close();
                Focus();
                Close();
            }
            else
            {
                int remainder = int.Parse(box_Count.Text.ToString());
                setPrint(remainder);
                reportWindow.DirectPrint(int.Parse(copy_count.Text.ToString()) - int.Parse(combo.SelectedItem.ToString()));
                reportWindow.Close();
                Focus();
                Close();

                신우프린터(customSName,
                        prodName,
                        orderLast,
                        pSize,
                        getMemo(SettingJsonManager.printItem),
                        DateTime,
                        pStockKey
                        );
            }
        }

        private void setPrint(int remainder)
        {
            string memo = getMemo(SettingJsonManager.printItem);
            pr1.pMemo = memo;
            pr1.loadCount = remainder.ToString();

            완제품명세서_Uc1.ViewModel = pr1;
            완제품명세서_Uc1.Set_BarCode();
            Grid.SetRow(완제품명세서_Uc1, 0);
            명세서양식_Uc.ItemPanel.Children.Add(완제품명세서_Uc1);
            Set_FixedPage.Children.Add(명세서양식_Uc as UIElement);

            PageContent Set_PageContent = new PageContent();
            // 픽셀 페이지 페이지 콘텐츠 객체 자식 형태로 저장
            (Set_PageContent as IAddChild).AddChild(Set_FixedPage);

            // 픽셀 도큐먼트 페이지 콘텐츠 저장
            FixedDocument.Pages.Add(Set_PageContent);
            reportWindow = new PrintWindow();
            reportWindow.PrintViewer.Document = FixedDocument;
            reportWindow.가로세로 = 1;
            reportWindow.방향.IsEnabled = false;
            reportWindow.엑셀.IsEnabled = false;
            reportWindow.Opacity = 0;
            reportWindow.Show();
        }

        //출력매수 클릭
        private void Copy_count_click(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                //키보드 호출
                TextBox context = (TextBox)sender;
                Win_KeyBoard win_KeyBoard = new Win_KeyBoard(context)
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = 300
                };
                _ = win_KeyBoard.ShowDialog();
            }
        }

        //닫기 버튼
        private void Close_messagebox(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //비고란 정보집계
        private string getMemo(string printItem)
        {
            string memo = "";
            string[] print = printItem.Split(new string[] { "|" }, StringSplitOptions.None);
            for (int i = 0; i < print.Length; i++)
            {
                if (i != 0 && i % 4 == 0)
                {
                    memo += "\n";
                }

                switch (print[i])
                {
                    case "1":
                        memo += string.IsNullOrEmpty(detailitem.filmNo) ? "" : detailitem.filmNo + " / ";
                        break;
                    case "2":
                        memo += string.IsNullOrEmpty(detailitem.CoatingType) ? "" : detailitem.CoatingType + " / ";
                        break;
                    case "3":
                        memo += string.IsNullOrEmpty(detailitem.PrintNo) ? "" : detailitem.PrintNo + " / ";
                        break;
                    case "4":
                        memo += string.IsNullOrEmpty(detailitem.Printposition) ? "" : detailitem.Printposition + " / "; ;
                        break;
                    case "5":
                        memo += string.IsNullOrEmpty(detailitem.PushingNo) ? "" : detailitem.PushingNo + " / "; ;
                        break;
                    case "6":
                        memo += string.IsNullOrEmpty(detailitem.Pushingposition) ? "" : detailitem.Pushingposition + " / ";
                        break;
                    case "7":
                        memo += string.IsNullOrEmpty(detailitem.ColorType) ? "" : detailitem.ColorType + " / ";
                        break;
                    case "8":
                        memo += string.IsNullOrEmpty(detailitem.ConnetingType) ? "" : detailitem.ConnetingType + " / ";
                        break;
                    case "9":
                        memo += string.IsNullOrEmpty(detailitem.handChk) ? "" : detailitem.handChk + " / ";
                        break;
                    case "10":
                        memo += string.IsNullOrEmpty(detailitem.PackCount + "") ? "" : detailitem.PackCount + "매 / ";
                        break;
                    case "11":
                        memo += string.IsNullOrEmpty(detailitem.PackType) ? "" : detailitem.PackType + " / ";
                        break;
                    default:
                        break;
                }
            }
            return memo;
        }

        //부착수 콤보박스 클릭
        private void ComboSelected(object sender, SelectionChangedEventArgs e)
        {
            float selectedItem = float.Parse(combo.SelectedItem.ToString());
            float copyCount;

            //적재수량 0이면 부착수만큼 출력매수 지정
            if (box_Count.Text.ToString().Equals("0"))
            {
                copyCount = selectedItem;
            }
            //적재수량이 0이 아니면 총수량/적재수량만큼 출력매수 지정
            else
            {
                copyCount = (float)Math.Ceiling(float.Parse(total_count.Text.ToString()) / float.Parse(box_Count.Text.ToString())) * selectedItem;
            }
            setCopyCount(copyCount);
        }

        //콤보박스로 연산된만큼 출력매수 지정
        private void setCopyCount(float copyCount)
        {
            copy_count.Text = copyCount.ToString();
        }

        private void 신우프린터(string customSName, string prodName, string orderLast, string pSize, string pMemo, string pworkDate, string pStockKey)
        {

            int remainder = int.Parse(total_count.Text.ToString()) % int.Parse(box_Count.Text.ToString());
            double Dbl_HeightReportPageWidthSize = 8.2 * 96;
            double Dbl_HeightReportPageHeightSize = 12 * 96;

            // 양식 유저 컨트롤 페이지 넓이 사이즈
            명세서양식_Uc1.Width = Dbl_HeightReportPageWidthSize;
            // 양식 유저 컨트롤 페이지 높이 사이즈
            명세서양식_Uc1.Height = Dbl_HeightReportPageHeightSize;

            // 새로운 픽셀페이지 객체 생성
            Set_FixedPage1 = new FixedPage();

            // 페이지 넓이 사이즈 
            Set_FixedPage1.Width = Dbl_HeightReportPageWidthSize;

            // 페이지 높이 사이즈 
            Set_FixedPage1.Height = Dbl_HeightReportPageHeightSize;
            pr1.loadCount = remainder.ToString();
            
            if (SettingJsonManager.corp_name.Equals("신우수출포장"))
            {
                switch (mainModel.corpName.Trim())
                {
                    case "우리포장산업":
                    case "누리팩":
                    case "유램":
                    case "지온":
                    case "채움팩":
                    case "(주)우리포장산업":
                    case "주식회사누리팩":
                    case "주식회사유램":
                    case "주식회사지온":
                    case "주식회사채움팩":
                        pr1.customSName = "";
                        pr1.MycustomSName = "";
                        pr1.corpName = "";
                        break;
                    default:
                        pr1.corpName = mainModel.corpName;
                        pr1.MycustomSName = SettingJsonManager.corp_name;
                        break;
                }
            }
            else if (SettingJsonManager.corp_name.Equals("우리포장산업"))
            {
                switch (mainModel.corpName.Trim())
                {
                    case "신우수출포장":
                    case "누리팩":
                    case "유램":
                    case "지온":
                    case "채움팩":
                    case "(주)신우수출포장":
                    case "주식회사누리팩":
                    case "주식회사유램":
                    case "주식회사지온":
                    case "주식회사채움팩":
                        pr1.customSName = "";
                        pr1.MycustomSName = "";
                        pr1.corpName = "";
                        break;
                    default:
                        pr1.corpName = mainModel.corpName;
                        pr1.MycustomSName = SettingJsonManager.corp_name;
                        break;
                }
            }
            else if (SettingJsonManager.corp_name.Equals("누리팩"))
            {
                switch (mainModel.corpName.Trim())
                {
                    case "우리포장산업":
                    case "신우수출포장":
                    case "유램":
                    case "지온":
                    case "채움팩":
                    case "(주)신우수출포장":
                    case "주식회사누리팩":
                    case "주식회사유램":
                    case "주식회사지온":
                    case "주식회사채움팩":
                        pr1.customSName = "";
                        pr1.MycustomSName = "";
                        pr1.corpName = "";
                        break;
                    default:
                        pr1.customSName = mainModel.corpName;
                        pr1.MycustomSName = SettingJsonManager.corp_name;
                        break;
                }
            }

            pr1.corpName = (bool)account_check.IsChecked ? "" : mainModel.corpName;
            pr1.MycustomSName = (bool)production_check.IsChecked ? "" : SettingJsonManager.corp_name.Equals("대양포장") ? "    " : SettingJsonManager.corp_name;
            pr1.customSName = customSName;
            pr1.prodName = prodName;
            pr1.orderLast = Convert.ToDateTime(orderLast);
            pr1.pSize = pSize;

            string memo = getMemo(SettingJsonManager.printItem);
            pr1.pMemo = pMemo + (string.IsNullOrEmpty(pMemo) ? memo : Environment.NewLine + memo);
            pr1.pworkDate = Convert.ToDateTime(pworkDate).ToString("yyyy/MM/dd");
            pr1.pStockKey = pStockKey;

            int keyLenth = 0;
            keyLenth = 10 - pr1.pStockKey.Length;

            for (var index = 1; index <= keyLenth; index++)
            {
                pr1.pStockKey = "0" + pr1.pStockKey;
            }

            완제품명세서_Uc11.ViewModel = pr1;
            완제품명세서_Uc11.Set_BarCode();
            Grid.SetRow(완제품명세서_Uc11, 0);

            명세서양식_Uc1.ItemPanel.Children.Add(완제품명세서_Uc11);
            Set_FixedPage1.Children.Add(명세서양식_Uc1 as UIElement);

            PageContent Set_PageContent = new PageContent();
            // 픽셀 페이지 페이지 콘텐츠 객체 자식 형태로 저장
            (Set_PageContent as IAddChild).AddChild(Set_FixedPage1);

            // 픽셀 도큐먼트 페이지 콘텐츠 저장
            FixedDocument1 = new FixedDocument();
            FixedDocument1.Pages.Add(Set_PageContent);

            PrintWindow reportWindow = new PrintWindow();
            reportWindow.PrintViewer.Document = FixedDocument1;
            reportWindow.가로세로 = 1;
            reportWindow.방향.IsEnabled = false;
            reportWindow.엑셀.IsEnabled = false;
            reportWindow.Opacity = 0;
            reportWindow.Show();
            reportWindow.DirectPrint(int.Parse(combo.SelectedItem.ToString()));
            reportWindow.Close();
            this.Focus();
            this.Close();
        }
    }
}
