using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using PopProject.Util;
using log4net;
using log4net.Config;

namespace PopProject.Util
{
    public static class CountManager
    {
        public static ILog log = LogManager.GetLogger(typeof(CountManager)); //로그

        static bool useWorkCnt = false;
        static string corp_name;
        static string url;
        static HttpClient workCntClient = new HttpClient();

        static CountManager()
        {
            // log setting
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));

            // 업체정보가 없으면 CountManager 종료
            if (SettingJsonManager.corp_name.Equals(""))
            {
                log.Error("업체정보가 없습니다.");
                return;
            }
            
            // workCnt 사용 업체 리스트
            Dictionary<string, string> dicCorpsUsingWorkCnt = new Dictionary<string, string>()
            {
                {"광성수출포장", "http://ksbox.iptime.org:18888/pop/gathering"},
                {"대세물산", "http://dsbox.iptime.org:18888/pop/gathering"},
                {"유창포장", "http://211.253.37.232:18888/pop/gathering"},
                {"금화", "http://khbox.iptime.org:18888/pop/gathering"},

                {"광성수출포장테스트", "http://localhost:18888/pop/gathering"},
                {"대세물산테스트", "http://localhost:18888/pop/gathering"},
                {"유창포장테스트", "http://localhost:18888/pop/gathering"},
                {"금화테스트", "http://localhost:18888/pop/gathering"}
            };

            // workCnt 사용 업체에 포함되어 있는지 확인
            useWorkCnt = dicCorpsUsingWorkCnt.ContainsKey(SettingJsonManager.corp_name);
            //useWorkCnt = Array.Exists(corpsUsingWorkCnt, x => x == SettingJsonManager.corp_name);

            if (useWorkCnt)
            {
                // 업체명 저장
                corp_name = SettingJsonManager.corp_name;

                // 업체명 사용 url 저장
                url = dicCorpsUsingWorkCnt[corp_name];
            }
        }

        public static void SendMsg(string mercName, string v)
        {
            if (useWorkCnt)
            {
                object lockO = new object();
                lock (lockO)
                {
                    _ = PopService(mercName, v);
                }
            }
            else
            {
                SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);
            }
        }

        static async Task PopService(string mercName, string data)
        {
            string companyCode = SettingJsonManager.corp_id; //업체코드
            try
            {
                // 파라미터를 포함한 URL 생성
                string urlWithParameters = $"{url}/{companyCode}/{mercName}/{data}";
                // HTTP GET 요청 보내기
                HttpResponseMessage response = await workCntClient.GetAsync(urlWithParameters);

                if (response.IsSuccessStatusCode == false)
                {
                    log.Error("서비스로 데이터 전송 실패");
                }
                else
                {
                }
            }
            catch(Exception e)
            {
                log.Error("CountManager.SendMsg 전송 실패 : " + e.Message);
            }
        }
    }
}
