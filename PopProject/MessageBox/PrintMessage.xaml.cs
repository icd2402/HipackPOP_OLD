using Monitor.Util;
using Newtonsoft.Json.Linq;
using PopProject.Detail;
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

        public PrintMessage(MainModel main, DetailModel detailitem)
        {
            InitializeComponent();
            copy_count.Text = "1";
            mainModel = main;
            this.detailitem = detailitem;

            if (Properties.Settings.Default.keyinCheck)
            {
                copy_count.Focusable = true;
                _ = copy_count.Focus();
                _ = copy_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));

                box_count.Focusable = true;
                _ = box_count.Focus();
                _ = box_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));
            }

            if (Properties.Settings.Default.corp_id.Equals("3125247246") || Properties.Settings.Default.corp_id.Equals("C91950615") || Properties.Settings.Default.corp_id.Equals("766562426")) // 오주포장
            {
                pallet.Visibility = Visibility.Visible;
                //print_stack.Margin = new Thickness(0, 0, 0, 0);
                //set_count.Margin = new Thickness(0, 20, 0, 0);
            }
            else
            {
                print_stack.Margin = new Thickness(0, 0, 0, 0);
                set_count.Margin = new Thickness(0, 20, 0, 0);
            }

        }

        private void Result_count_minus(object sender, MouseButtonEventArgs e)
        {
            int value = 1;
            if (int.Parse(copy_count.Text) - value >= 1)
            {
                copy_count.Text = int.Parse(copy_count.Text) - value + "";
            }
        }

        private void Result_count_plus(object sender, MouseButtonEventArgs e)
        {
            int value = 1;
            copy_count.Text = int.Parse(copy_count.Text) + value + "";
        }

        private void Messagebox_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
                ReturnModel returnData = ServiceModel.GetData(
                    Properties.Settings.Default.serverIndex,
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
                    ClsUc_완제품명세서_Control1.ProductReport pr1 = new ClsUc_완제품명세서_Control1.ProductReport();
                    ClsUc_완제품명세서_Control2.ProductReport pr2 = new ClsUc_완제품명세서_Control2.ProductReport();
                    ClsUc_완제품명세서_Control3.ProductReport pr3 = new ClsUc_완제품명세서_Control3.ProductReport();
                    ClsUc_완제품명세서_Control5두성.ProductReport pr5 = new ClsUc_완제품명세서_Control5두성.ProductReport();
                    ClsUc_완제품명세서_Control6구룡.ProductReport pr6 = new ClsUc_완제품명세서_Control6구룡.ProductReport();
                    JToken result = SignCorpinfo_Base_JArray.First;

                    if (Properties.Settings.Default.corp_id.Equals("75BB212362"))
                    {
                        pr2.MycustomSName = "";
                        pr2.corpName = "";
                        pr2.customSName = result["customSName"].ToString();
                        pr2.prodName = result["prodName"].ToString();
                        pr2.orderLast = Convert.ToDateTime(result["orderLast"]);
                        pr2.pSize = result["pSize"].ToString();
                        string memo = getMemo(Properties.Settings.Default.printItem);
                        pr2.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        pr2.pworkDate = DateTime.Now;
                        pr2.resultCount = result["resultCount"].ToString();
                        pr2.pStockKey = result["pStockKey"].ToString();
                        int keyLenth = 0;
                        keyLenth = 10 - pr2.pStockKey.Length;

                        for (int index = 1; index <= keyLenth; index++)
                        {
                            pr2.pStockKey = "0" + pr2.pStockKey;
                        }

                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr2.resultCount = mainModel.sumPworkCount + "";
                        }

                        if (!Properties.Settings.Default.count_check)
                        {
                            pr2.resultCount = "";
                        }
                    }

                    else
                    {
                        pr1.corpName = Properties.Settings.Default.accountCheck ? "" : mainModel.corpName;

                        pr1.MycustomSName = Properties.Settings.Default.productionCheck ? "" : Properties.Settings.Default.corp_name.Equals("대양포장") ? "    " : Properties.Settings.Default.corp_name;
                        
                    }

                    ClsUc_완제품명세서_Control1 완제품명세서_Uc1 = new ClsUc_완제품명세서_Control1();
                    ClsUc_완제품명세서_Control2 완제품명세서_Uc2 = new ClsUc_완제품명세서_Control2();
                    ClsUc_완제품명세서_Control3 완제품명세서_Uc3 = new ClsUc_완제품명세서_Control3();
                    ClsUc_완제품명세서_Control5두성 완제품명세서_두성 = new ClsUc_완제품명세서_Control5두성();
                    ClsUc_완제품명세서_Control6구룡 완제품명세서_구룡 = new ClsUc_완제품명세서_Control6구룡();
                    ClsUc_명세서_Report 명세서양식_Uc = new ClsUc_명세서_Report();
                    ClsUc_명세서_Report2 명세서양식_Uc2 = new ClsUc_명세서_Report2();
                    ClsUc_명세서_Report3 명세서양식_Uc3 = new ClsUc_명세서_Report3();
                    ClsUc_명세서_Report5두성 명세서양식_uc5두성 = new ClsUc_명세서_Report5두성();
                    ClsUc_명세서_Report6구룡 명세서양식_uc6구룡 = new ClsUc_명세서_Report6구룡();

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

                    if (Properties.Settings.Default.corp_name.Equals("유창포장"))
                    {
                        pr2.corpName = Properties.Settings.Default.accountCheck ? "" : mainModel.corpName;
                        pr2.MycustomSName = Properties.Settings.Default.productionCheck ? "" : Properties.Settings.Default.corp_name;
                        pr2.customSName = result["customSName"].ToString();
                        pr2.prodName = result["prodName"].ToString();
                        pr2.orderLast = Convert.ToDateTime(result["orderLast"]);
                        pr2.pSize = result["pSize"].ToString();

                        string memo = getMemo(Properties.Settings.Default.printItem);
                        pr2.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        pr2.pworkDate = Convert.ToDateTime(result["pworkDate"]);
                        pr2.pworkDate = DateTime.Now;
                        pr2.resultCount = result["resultCount"].ToString();
                        pr2.pStockKey = result["pStockKey"].ToString();

                        int keyLenth = 0;
                        keyLenth = 10 - pr2.pStockKey.Length;
                        
                        for (int index = 1; index <= keyLenth; index++)
                        {
                            pr2.pStockKey = "0" + pr2.pStockKey;
                        }

                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr2.resultCount = mainModel.sumPworkCount + "";
                        }
                        if (!Properties.Settings.Default.count_check)
                        {
                            pr2.resultCount = "";
                        }
                        완제품명세서_Uc2.ViewModel = pr2;
                        완제품명세서_Uc2.Set_BarCode();
                        Grid.SetRow(완제품명세서_Uc2, 0);

                        명세서양식_Uc2.ItemPanel.Children.Add(완제품명세서_Uc2);
                        Set_FixedPage.Children.Add(명세서양식_Uc2 as UIElement);
                    }

                    else if (Properties.Settings.Default.corp_name.Equals("오주포장") || Properties.Settings.Default.corp_name.Equals("오주포장테스트"))
                    {

                        pr3.corpName = Properties.Settings.Default.accountCheck ? "" : mainModel.corpName;
                        pr3.MycustomSName = Properties.Settings.Default.productionCheck ? "" : Properties.Settings.Default.corp_name;
                        pr3.customSName = result["customSName"].ToString();
                        pr3.prodName = result["prodName"].ToString();

                        pr3.orderLast = Convert.ToDateTime(result["orderLast"]);
                        pr3.pSize = result["pSize"].ToString();
                        pr3.pGol = mainModel.paperGor;
                        pr3.PalletType = detailitem.PalletType;
                        string memo = getMemo(Properties.Settings.Default.printItem);
                        pr3.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        //pr3.pworkDate = Convert.ToDateTime(result["pworkDate"]).ToString("yyyy/MM/dd");
                        pr3.pworkDate = DateTime.Today.ToString("yyyy/MM/dd");
                        pr3.pStockKey = result["pStockKey"].ToString();
                  
                        int keyLenth = 0;
                        keyLenth = 10 - pr3.pStockKey.Length;

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr3.pStockKey = "0" + pr3.pStockKey;
                        }

                        pr3.resultCount = box_count.Text.Length == 0 ? "" : box_count.Text;

                        완제품명세서_Uc3.ViewModel = pr3;
                        완제품명세서_Uc3.Set_BarCode();
                        Grid.SetRow(완제품명세서_Uc3, 1);

                        명세서양식_Uc3.ItemPanel.Children.Add(완제품명세서_Uc3);
                        Set_FixedPage.Children.Add(명세서양식_Uc3 as UIElement);
                    }

                    else if(Properties.Settings.Default.corp_name.Equals("두성포장") || Properties.Settings.Default.corp_name.Equals("두성포장테스트") || Properties.Settings.Default.corp_name.Contains("혜성"))
                    {

                        pr5.customSName = result["customSName"].ToString();
                        pr5.prodName = result["prodName"].ToString();
                        pr5.orderLast = Convert.ToDateTime(result["orderLast"]);
                        pr5.pSize = result["pSize"].ToString();
                        string memo = "";

                        if (Properties.Settings.Default.corp_name.Equals("광무포장"))
                        {
                            memo = detailitem.SpecialMemo + Environment.NewLine;
                            memo += getMemo(Properties.Settings.Default.printItem);
                        }

                        else
                        {
                            memo = getMemo(Properties.Settings.Default.printItem);
                        }

                        // memo = getMemo(Properties.Settings.Default.printItem);
                        pr5.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        pr5.pworkDate = DateTime.Today.ToString("yyyy/MM/dd");
                        pr5.resultCount = result["resultCount"].ToString();
                        pr5.pStockKey = result["pStockKey"].ToString();
                        pr5.loadCount = "";
                        pr5.LocationName = detailitem.LocationName;
                        pr5.LoadHeight = detailitem.LoadHeight;
                        pr5.LoadType = detailitem.LoadType;


                        if (box_count.Text.ToString().Length > 0)
                        {
                            pr5.loadCount = box_count.Text;
                        }


                        int keyLenth = 0;
                        keyLenth = 10 - pr5.pStockKey.Length;

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr5.pStockKey = "0" + pr5.pStockKey;
                        }

                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr5.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                        }

                        if (!Properties.Settings.Default.count_check)
                        {
                            pr5.resultCount = "";
                        }
                        완제품명세서_두성.ViewModel = pr5;
                        완제품명세서_두성.Set_BarCode();
                        Grid.SetRow(완제품명세서_두성, 1);
                        명세서양식_uc5두성.ItemPanel.Children.Add(완제품명세서_두성);
                        Set_FixedPage.Children.Add(명세서양식_uc5두성 as UIElement);
                    }
                    else if (Properties.Settings.Default.corp_name.Equals("구룡산업제2공장"))
                    {
                        
                        pr6.customSName = result["customSName"].ToString();
                        pr6.vendorName = detailitem.vendorName;


                        pr6.prodName = result["prodName"].ToString();
                        pr6.orderLast = Convert.ToDateTime(result["orderLast"]);
                        pr6.pSize = result["pSize"].ToString();
                        string memo = "";

                        if (Properties.Settings.Default.corp_name.Equals("광무포장"))
                        {
                            memo = detailitem.SpecialMemo + Environment.NewLine;
                            memo += getMemo(Properties.Settings.Default.printItem);
                        }

                        else
                        {
                            memo = getMemo(Properties.Settings.Default.printItem);
                        }

                        // memo = getMemo(Properties.Settings.Default.printItem);
                        pr6.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        pr6.pworkDate = DateTime.Today.ToString("yyyy/MM/dd");
                        pr6.resultCount = result["resultCount"].ToString();
                        pr6.pStockKey = result["pStockKey"].ToString();
                        pr6.loadCount = "";
                        pr6.LocationName = detailitem.LocationName;
                        pr6.loadHeight = detailitem.LoadHeight;
                        pr6.loadType = detailitem.LoadType;
                        if (box_count.Text.ToString().Length > 0)
                        {
                            pr6.loadCount = box_count.Text;
                        }


                        int keyLenth = 0;
                        keyLenth = 10 - pr6.pStockKey.Length;

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr6.pStockKey = "0" + pr6.pStockKey;
                        }

                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr6.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                        }

                        if (!Properties.Settings.Default.count_check)
                        {
                            pr6.resultCount = "";
                        }
                        완제품명세서_구룡.ViewModel = pr6;
                        완제품명세서_구룡.Set_BarCode();
                        Grid.SetRow(완제품명세서_구룡, 1);
                        명세서양식_uc6구룡.ItemPanel.Children.Add(완제품명세서_구룡);
                        Set_FixedPage.Children.Add(명세서양식_uc6구룡 as UIElement);
                    }

                    else
                    {
                        pr1.customSName = result["customSName"].ToString();
                        pr1.prodName = result["prodName"].ToString();
                        pr1.orderLast = Convert.ToDateTime(result["orderLast"]);
                        pr1.pSize = result["pSize"].ToString();
                        string memo = "";
                    
                        if (Properties.Settings.Default.corp_name.Equals("광무포장"))
                        {
                            memo = detailitem.SpecialMemo + Environment.NewLine;
                            memo+= getMemo(Properties.Settings.Default.printItem);
                        }

                        else
                        {
                            memo = getMemo(Properties.Settings.Default.printItem);
                        }

                        // memo = getMemo(Properties.Settings.Default.printItem);
                        pr1.pMemo = result["pMemo"].ToString() + (string.IsNullOrEmpty(result["pMemo"].ToString()) ? memo : Environment.NewLine + memo);
                        pr1.pworkDate = DateTime.Today.ToString("yyyy/MM/dd");
                        pr1.resultCount = result["resultCount"].ToString();
                        pr1.pStockKey = result["pStockKey"].ToString();
                        pr1.loadCount = "";
                        if (box_count.Text.ToString().Length > 0)
                        {
                            pr1.loadCount = box_count.Text;
                        }
                       
                        
                        int keyLenth = 0;
                        keyLenth = 10 - pr1.pStockKey.Length;

                        for (var index = 1; index <= keyLenth; index++)
                        {
                            pr1.pStockKey = "0" + pr1.pStockKey;
                        }

                        if (Convert.ToInt32(mainModel.sumPworkCount) > 0)
                        {
                            pr1.resultCount = mainModel.sumPworkCount == 0 ? "" : mainModel.sumPworkCount + "";
                        }

                        if (!Properties.Settings.Default.count_check)
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

                    reportWindow.가로세로 = 1;

                    reportWindow.방향.IsEnabled = false;
                    reportWindow.엑셀.IsEnabled = false;
                    reportWindow.Opacity = 0;
                    reportWindow.Show();
                    if (Properties.Settings.Default.corp_id.Equals("3125247246"))
                    {
                        if (int.Parse(copy_count.Text) == 1)
                        {
                            reportWindow.DirectPrint(int.Parse(copy_count.Text));
                            reportWindow.Close();
                        }

                        else
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
                    else
                    {
                        reportWindow.DirectPrint(int.Parse(copy_count.Text));
                       // reportWindow.Close();
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
            pr3.MycustomSName = Properties.Settings.Default.productionCheck ? "" : Properties.Settings.Default.corp_name;
            pr3.customSName = customSName;
            pr3.prodName = prodName;
            pr3.orderLast = Convert.ToDateTime(orderLast);
            pr3.pSize = pSize;
            pr3.pGol = mainModel.paperGor;
            string memo = getMemo(Properties.Settings.Default.printItem);
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

        private void Close_messagebox(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private string getMemo(string printItem)
        {
            string memo = "";
            string[] print = printItem.Split(new string[] { "|" }, StringSplitOptions.None);
            for (int i = 0; i < print.Length; i++)

            {

                if(i!=0 && i%3 == 0)
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
                    case "18":
                        memo += string.IsNullOrEmpty(detailitem.PalletType) ? "" : detailitem.PalletType + " / ";
                        break;
                }
            }

            return memo;
        }

        private void pallet_count(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.keyinCheck)
            {
                box_count.Focusable = true;
                _ = box_count.Focus();
                _ = box_count.Dispatcher.BeginInvoke(new Action(() => { box_count.SelectAll(); }));
            }
            else
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

        private void Copy_count_click(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.keyinCheck)
            {
                copy_count.Focusable = true;
                _ = copy_count.Focus();
                _ = copy_count.Dispatcher.BeginInvoke(new Action(() => { copy_count.SelectAll(); }));
            }
            else
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
