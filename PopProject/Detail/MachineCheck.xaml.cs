using Newtonsoft.Json.Linq;
using PopProject.input;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Net.Http;
using System.Threading.Tasks;

namespace PopProject.Detail
{
    /// <summary>
    /// MachineCheck.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MachineCheck : Window
    {
        private bool 구분Check = true;
        private bool 점검명Check = true;
        private bool 점검중 = false;
        string 점검내용 = "";
        string s구분 = "";
        string s점검명 = "";
        string s시간 = "";
        string ServerIndex = ""; //업체 인덱스
        string DBName = ""; //접속정보(DB명)

        public MachineCheck()
        {
            InitializeComponent();
            BringWindowToTop();


            //SignalR 연결
            try
            {
                SignalRAdministrator conn = new SignalRAdministrator();
                //conn.Setconnection();
            }
            catch (Exception)
            {
            }

            //DB접속정보 변경
            switch (SettingJsonManager.corp_name)
            {
                case "대세물산":
                case "대세물산테스트":
                    ServerIndex = "dsbox";
                    DBName = "FrameWork_MES";
                    break;
                case "광성수출포장":
                case "광성수출포장테스트":
                    ServerIndex = "ksbox";
                    DBName = "FrameWork_MES";
                    break;

                default:
                    break;
            }
        }

        //창 맨위로 고정
        private void BringWindowToTop()
        {
            this.Topmost = true;
        }


        //구분 선택시 고장명 조회
        private void 구분ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string ServerIndex = ""; //업체 인덱스
            //string DBName = ""; //접속정보(DB명)

            
            string columnName = "";
            int Opt_Num = 0;

            //선택된 구분에 따라 조회정보 변경
            if ((구분combo.SelectedItem as ComboBoxItem)?.Content.ToString() == "고장")
            {
                Opt_Num = 1;
                columnName = "ErrName";
            }
            else if ((구분combo.SelectedItem as ComboBoxItem)?.Content.ToString() == "점검")
            {
                Opt_Num = 2;
                columnName = "ChkName";
            }
            else if ((구분combo.SelectedItem as ComboBoxItem)?.Content.ToString() == "부품교체")
            {
                Opt_Num = 3;
                columnName = "RplcName";
            }

            //DB 조회
            ServiceModel.ServiceModelClean(ServerIndex, DBName);
            ReturnModel returnData = ServiceModel.GetData(
               ServerIndex,
               "[TBL_SYS].[spSel641_MainDG]",
               new string[] { "@Opt" },
               new object[] { Opt_Num },
               0);

            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    점검명combo.Items.Clear();

                    foreach (JObject result in Data_Jarry)
                    {
                        string inspectName = (result[columnName] != null) ? result[columnName].ToString() : "";  
                        점검명combo.Items.Add(inspectName);
                    }
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("점검코드 조회 오류");
                _ = msg.ShowDialog();
            }
        }

        private void 점검명ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("선택된 점검명 : " + 점검명combo.Text);
        }

        //시작, 종료 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string ServerIndex = ""; //업체 인덱스
            //string DBName = ""; //접속정보(DB명)

            ////MES DB 설정
            //switch (SettingJsonManager.corp_name)
            //{
            //    case "대세물산":
            //        ServerIndex = "dsbox";
            //        DBName = "FrameWork_MES";
            //        break;
            //    case "광성수출포장":
            //        ServerIndex = "ksbox";
            //        DBName = "FrameWork_MES";
            //        break;

            //    default:
            //        break;
            //}

            int merchinID = 0;
            string inspectCode = string.Empty;

            Button clickedButton = sender as Button;
            
            if (clickedButton != null && clickedButton.Content is string content)
            {
                if (content == "시   작") // 버튼의 Content가 "시작"인 경우
                {
                    //구분,점검명 선택시 동작
                    if (!string.IsNullOrEmpty(구분combo.Text) && !string.IsNullOrEmpty(점검명combo.Text))
                    {
                        //관리자 계정이 아닐때만 (현황판 전송)
                        if (!SettingJsonManager.manager.Equals("1"))
                        {
                            s구분 = 구분combo.Text;
                            s점검명 = 점검명combo.Text;
                            string s시간 = DateTime.Now.ToString("HH:mm");

                            if (SettingJsonManager.corp_name.Equals("광성수출포장") || SettingJsonManager.corp_name.Equals("광성수출포장테스트"))
                            {
                                점검내용 = "C" + "," + s구분 + "," + s점검명 + "," + s시간;

                            }
                            else if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
                            {
                                점검내용 = "C" + "/" + s구분 + "/" + s점검명 + "/" + s시간;

                            }
                        }

                        // MercID 조회
                        ServiceModel.ServiceModelClean(ServerIndex, DBName);
                        ReturnModel returnData = ServiceModel.GetData(
                           ServerIndex,
                           "[TBL_MERC_MGT].[spSel001_MercProc]",
                           new string[] { "@Opt", "@mercName" },
                           new object[] { 5, SettingJsonManager.mercName },
                           0);

                        if (returnData.결과 == 1)
                        {
                            JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                            if (Data_Jarry != null)
                            {
                                foreach (JObject result in Data_Jarry)
                                {
                                    merchinID = (result["merchinID"] != null) ? Convert.ToInt32(result["merchinID"]) : 0;
                                }
                            }
                        }

                        // Inspection Code 조회
                        ServiceModel.ServiceModelClean(ServerIndex, DBName);
                        returnData = ServiceModel.GetData(
                           ServerIndex,
                           "[TBL_SYS].[spSel641_MainDG]",
                           new string[] { "@Opt", "@inspectType", "@CodeName" },
                           new object[] { 5, s구분, s점검명 },
                           0);

                        if (returnData.결과 == 1)
                        {
                            string strTmp = s구분.Equals("고장") ? "ErrCode" : s구분.Equals("점검") ? "ChkCode" : "RplcCode";
                            JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                            if (Data_Jarry != null)
                            {
                                foreach (JObject result in Data_Jarry)
                                {
                                    inspectCode = (result[strTmp] != null) ? result[strTmp].ToString() : "";
                                }
                            }
                        }

                        //DB 저장(시작)
                        ServiceModel.ServiceModelClean(ServerIndex, DBName);
                        returnData = ServiceModel.GetData(
                            ServerIndex,
                            "[TBL_MERC_MGT].[spAdd523_Data2]",
                            new string[]
                            {
                                "@type",
                                "@StartDt",
                                "@EndDt",
                                "@MercID",
                                "@MercName",
                                "@Code",
                                "@Name",
                                "@Reason",
                                "@Action",
                                "@Duration",
                                "@Memo",
                                "@User"
                            },
                            new object[]
                            {
                                s구분,
                                DateTime.Now,
                                "",
                                merchinID,
                                SettingJsonManager.mercName,
                                inspectCode,
                                s점검명,
                                "",
                                "",
                                "",
                                "",
                                MainWindow.userName
                            },
                            1
                        );

                        if (returnData.결과 == 1)
                        {
                        }
                        else
                        {
                            //System.Windows.MessageBox.Show("DB저장 실패" + returnData.결과.ToString());
                        }

                        tb1.Visibility = Visibility.Collapsed; //설비 점검을 시작하시겠습니까?? 숨기기
                        tb2.Visibility = Visibility.Visible; //설비 점검 중 표기
                        구분combo.IsHitTestVisible = false;
                        점검명combo.IsHitTestVisible = false;
                        clickedButton.Content = "종   료";
                        점검중 = true;

                        //관리자 계정이 아닐때만 (현황판 전송)
                        if (!SettingJsonManager.manager.Equals("1"))
                        {
                            InvokeMercName(SettingJsonManager.mercName, 점검내용); //설비명, 상태, 내용
                            Console.WriteLine("SignalR : " + SettingJsonManager.mercName + ", " + 점검내용);

                        }
                    }
                    else
                    {
                        MessageBox_Dialog msg = new MessageBox_Dialog("점검내용을 입력하세요.");
                        _ = msg.ShowDialog();
                    }
                }
                else if(content == "종   료")
                {
                    종료();

                    tb1.Visibility = Visibility.Visible; //설비 점검을 시작하시겠습니까?? 표기
                    tb2.Visibility = Visibility.Collapsed; //설비 점검 중 숨기기
                    구분combo.IsHitTestVisible = true;
                    점검명combo.IsHitTestVisible = true;
                    clickedButton.Content = "시   작";

                    //관리자 계정이 아닐때만 (현황판 전송)
                    if (!SettingJsonManager.manager.Equals("1"))
                    {
                        InvokeMercName(SettingJsonManager.mercName, "D"); //설비명, 상태, 내용
                    }

                }
            }
        }

        private void 종료()
        {
            JObject joData = new JObject();

            //점검 데이터 조회
            ServiceModel.ServiceModelClean(ServerIndex, DBName);
            ReturnModel returnData = ServiceModel.GetData(
               ServerIndex,
               "[TBL_MERC_MGT].[spSel521_MainDG]",
               new string[] { "@type", "@startDate", "@endDate", "@mercName" },
               new object[] { s구분, DateTime.Now.AddDays(-7), DateTime.Now, SettingJsonManager.mercName },
               0);

            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    joData = (JObject)Data_Jarry.Last;
                }
            }

            //DB 저장(종료)
            ServiceModel.ServiceModelClean(ServerIndex, DBName);
            returnData = ServiceModel.GetData(
                ServerIndex,
                "[TBL_MERC_MGT].[spUdt524_Data2]",
                new string[]
                {
                    "@type",
                    "@No",
                    "@EndDt",
                    "@Duration",
                    "@User"
                },
                new object[]
                {
                    s구분,
                    Convert.ToInt32(joData["No"]),
                    DateTime.Now,
                    Convert.ToInt32((DateTime.Now - Convert.ToDateTime(joData["StartDtDt"])).TotalMinutes),
                    MainWindow.userName
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

        // 설비 점검시 현황판으로 전송
        private void InvokeMercName(string mercName, string 상태)
        {
            try
            {
                if (SettingJsonManager.corp_name.Equals("광성수출포장") || SettingJsonManager.corp_name.Equals("광성수출포장테스트"))
                {
                    popService(mercName, 상태);
                }
                else
                {
                    _ = SignalRAdministrator.허브.Invoke("invokeCount", SettingJsonManager.corp_id, mercName, 상태); //DB명, 설비명, 카운터
                }
            }
            catch (Exception)
            {
            }
        }

        //신규 서비스 Http 셋팅
        private static HttpClient client = new HttpClient();
        async Task popService(string mercName, string str)
        {
            string Url = "http://localhost:18888/pop/gathering"; //주소
            //string Url = SettingJsonManager.signalR; //주소
            string CompanyCode = SettingJsonManager.corp_id; //업체코드
            string MercName = mercName; //설비명
            string Data = str.ToString(); //데이터

            await workCnt(Url, CompanyCode, MercName, Data);
        }

        ////신규 서비스 Http Get
        async Task workCnt(string url, string companyCode, string mercName, string data)
        {
            try
            {
                // 파라미터를 포함한 URL 생성
                string urlWithParameters = $"{url}/{companyCode}/{mercName}/{data}";
                // HTTP GET 요청 보내기
                HttpResponseMessage response = await client.GetAsync(urlWithParameters);
                Console.WriteLine(urlWithParameters);
            }
            catch (Exception)
            {
            }
        }

        //드래그
        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //닫기
        private void Exit_window(object sender, EventArgs e)
        {
            Close();
        }

        //창 닫히면
        private void closed(object sender, EventArgs e)
        {
            if (점검중)
            {
                종료();

                tb1.Visibility = Visibility.Visible; //설비 점검을 시작하시겠습니까?? 표기
                tb2.Visibility = Visibility.Collapsed; //설비 점검 중 숨기기

                //관리자 계정이 아닐때만 (현황판 전송)
                if (!SettingJsonManager.manager.Equals("1"))
                {
                    InvokeMercName(SettingJsonManager.mercName, "D"); //설비명, 상태, 내용
                }
            }
            //비정상 종료
            else
            {
                tb1.Visibility = Visibility.Visible; //설비 점검을 시작하시겠습니까?? 표기
                tb2.Visibility = Visibility.Collapsed; //설비 점검 중 숨기기

                //관리자 계정이 아닐때만 (현황판 전송)
                if (!SettingJsonManager.manager.Equals("1"))
                {
                    InvokeMercName(SettingJsonManager.mercName, "D"); //설비명, 상태, 내용
                }
            }
        }
    }
}
