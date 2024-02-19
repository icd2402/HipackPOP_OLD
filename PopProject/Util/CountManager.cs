using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using PopProject.Util;

namespace PopProject.Util
{
    public static class CountManager
    {
        static bool useWorkCnt = false;
        static string corp_name;
        static string url;
        static HttpClient workCntClient = new HttpClient();

        // Thread 동기화용 락(세마포어)
        static object lockWorkCnt;
        private static string jsonString;

        static CountManager()
        {
            lockWorkCnt = new object();

            // 업체정보가 없으면 CountManager 종료
            if (SettingJsonManager.corp_name.Equals(""))
            {
                LogManager.Error("업체정보가 없습니다.", MethodBase.GetCurrentMethod().DeclaringType);
                return;
            }
            
            // workCnt 사용 업체 리스트
            Dictionary<string, string> dicCorpsUsingWorkCnt = new Dictionary<string, string>()
            {
                {"하나포장", "http://hnbox.iptime.org:18888/pop"},
                {"광성수출포장", "http://ksbox.iptime.org:18888/pop"},
                {"선호칼라팩", "http://shbox.iptime.org:18888/pop"},
                {"유창포장", "http://211.253.37.232:18888/pop"},
                {"광무포장", "http://kmbox.iptime.org:18888/pop"},
                {"대양포장", "http://dybox.iptime.org:18888/pop"},
                {"신우수출포장", "http://swbox.iptime.org:18888/pop"},
                {"누리팩", "http://erboxtest.iptime.org:18888/pop"},
                {"우리포장", "http://wrbox.iptime.org:18888/pop"},
                {"혜성수출포장", "http://hsbox.iptime.org:18888/pop"},
                {"㈜혜성수출포장", "http://hssbox.iptime.org:18888/pop"},
                {"성주포장", "http://sjbox.iptime.org:18888/pop"},
                {"지온", "http://jobox.iptime.org:18888/pop"},
                {"두성포장", "http://dubox.iptime.org:18888/pop"},
                {"대세물산", "http://dsbox.iptime.org:18888/pop"},
                {"구룡산업제2공장", "http://grbox.iptime.org:18888/pop"},
                {"금화", "http://khbox.iptime.org:18888/pop"},
                //{"오주포장", "http://ojbox.iptime.org:18888/pop"},
                //{"미래포장", "http://miraebox.float-zone.com:18888/pop"},

                {"하나포장테스트", "http://hnbox.iptime.org:18888/pop"},
                {"광성수출포장테스트", "http://ksbox.iptime.org:18888/pop"},
                {"선호칼라팩테스트", "http://shbox.iptime.org:18888/pop"},
                {"유창포장테스트", "http://211.253.37.232:18888/pop"},
                {"광무포장테스트", "http://kmbox.iptime.org:18888/pop"},
                {"대양포장테스트", "http://dybox.iptime.org:18888/pop"},
                {"신우수출포장테스트", "http://swbox.iptime.org:18888/pop"},
                {"누리팩테스트", "http://erboxtest.iptime.org:18888/pop"},
                {"우리포장테스트", "http://wrbox.iptime.org:18888/pop"},
                {"혜성수출포장테스트", "http://hsbox.iptime.org:18888/pop"},
                {"㈜혜성수출포장테스트", "http://hssbox.iptime.org:18888/pop"},
                {"성주포장테스트", "http://sjbox.iptime.org:18888/pop"},
                {"지온테스트", "http://jobox.iptime.org:18888/pop"},
                {"두성포장테스트", "http://dubox.iptime.org:18888/pop"},
                {"대세물산테스트", "http://dsbox.iptime.org:18888/pop"},
                {"구룡산업제2공장테스트", "http://grbox.iptime.org:18888/pop"},
                {"금화테스트", "http://khbox.iptime.org:18888/pop"},
                //{"오주포장테스트", "http://ojbox.iptime.org:18888/pop"},
                //{"미래포장테스트", "http://miraebox.float-zone.com:18888/pop"},

                {"국제포장테스트", "http://localhost:18888/pop"},
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

        // 데이터 쓰기
        public static void SendMsg(string mercName, string v)
        {
            if (useWorkCnt)
            {
                // Thread 동기화용 락(세마포어)
                lock (lockWorkCnt)
                {
                    _ = PopService(mercName, v);
                }
            }
            else
            {
                SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);
            }
        }

        //데이터 읽기
        public static async Task<string> ReadMsg()
        {
            if (useWorkCnt)
            {
                return await PopService2();
            }
            else
            {
                return null; // 또는 예외를 throw할 수도 있습니다.
            }
        }

        static async Task PopService(string mercName, string data)
        {
            try
            {
                // 파라미터를 포함한 URL 생성
                string urlWithParameters = $"{url}/gathering/{SettingJsonManager.corp_id}/{mercName}/{data}";
                // HTTP GET 요청 보내기
                HttpResponseMessage response = await workCntClient.GetAsync(urlWithParameters);

                if (response.IsSuccessStatusCode == false)
                {
                    LogManager.Error("서비스로 데이터 전송 실패", MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
            catch(Exception e)
            {
                LogManager.Error("CountManager.SendMsg 전송 실패 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        static async Task<string> PopService2()
        {
            try
            {
                // 파라미터를 포함한 URL 생성
                string urlWithParameters = $"{url}/monitoring/{SettingJsonManager.corp_id}";
                // HTTP GET 요청 보내기
                HttpResponseMessage response = await workCntClient.GetAsync(urlWithParameters);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    return jsonString;
                }
                else
                {
                    LogManager.Error("서비스로 데이터 전송 실패2", MethodBase.GetCurrentMethod().DeclaringType);
                    return null; 
                }
            }
            catch (Exception e)
            {
                LogManager.Error("CountManager.SendMsg 전송 실패2 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
                return null; 
            }
        }
    }
}
