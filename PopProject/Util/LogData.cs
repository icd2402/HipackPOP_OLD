using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Json;
using System.Text;
using System.Web;

namespace PopProject.Util
{
    public class LogData
    {
        /// <summary>
        /// 스마트공장 사업 로그 API 인증키 관리
        /// https://www.smart-factory.kr/
        /// </summary>

        private string crtfcKey = "";
        private string logDt = "";
        private string useSe = "";
        private string sysUser = "";
        private string conectIp = "";
        private int dataUsgqty = 0;

        public LogData(string useSe)
        {
            this.useSe = useSe; //로그 내용
            crtfcKey = CrtfcKey(SettingJsonManager.corp_id); //업체코드
            logDt = GetTime(); //시간
            sysUser = SettingJsonManager.mercName; //설비명
            conectIp = GetIP(); //접속 IP
            dataUsgqty = 0;

            JsonObjectCollection json = new JsonObjectCollection
            {
                new JsonStringValue("crtfcKey", crtfcKey),
                new JsonStringValue("logDt", logDt),
                new JsonStringValue("useSe", useSe),
                new JsonStringValue("sysUser", sysUser),
                new JsonStringValue("conectIp", conectIp),
                new JsonNumericValue("dataUsgqty", dataUsgqty)
            };

            HttpClient_util httpClient_Util = new HttpClient_util
                ("https://log.smart-factory.kr/apisvc/sendLogDataJSON.do?" + "logData=" + HttpUtility.UrlEncode(json.ToString(), Encoding.UTF8), "");

        }

        //시간
        public string GetTime()
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return time;
        }


        //접속 IP
        public string GetIP()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            string ServerIP = "0.0.0.0";

            if (addr.Length > 0)
            {
                ServerIP = addr[addr.Length - 1].ToString();
            }

            return ServerIP;

        }

        //업체코드로 API 인증키 분류
        public string CrtfcKey(string corp_id)
        {
            var crtfcKeys = new Dictionary<string, string> //업체코드, 인증키
            {
                { "75BD170261", "$5$API$9pgZWUx7Qgy0z/asRpn4l6VS.a8A0E.mfo2Yjb0BLjD" }, // 정일
                { "7C2564433", "$5$API$gswLJkqbSGriKu8Gt4Nh3ycrrEeQbxJl6tSHDE.JRw7" }, // 거성
                { "3125247246", "$5$API$vdZaR4Lyk12Z/NhS3bXUUKYZsP9gLgnx20GckLkfPk3" }, // 오주
                { "C4E5132036", "$5$API$1gtA7rTAzERz8hh18pTixIkIp6yDhq0vYA5gi6TkEQ1" }, // 미래
                { "A07E164641", "$5$API$tVvR3cwTa7CTErv3wui/zXIh8pUUxhD73TjckvwqjN7" }, // 하나
                { "3464250413", "$5$API$Srsh8f7I/QgnjX2QC3IIYF0bfChx6YmERjK5n/Q2OQA" }, // 광성
                { "75BB212362", "$5$API$Lqb0ylQQhtW2lYmGTA8oBnVnBiFMA5JGiu8XpA539.7" }, // 선호
                { "C8B516524", "$5$API$dOlFU/fI7Muxm1h40PiUJj5VSmdxXLOIxArRMV0sS.4" }, // 유창
                { "D85A2100", "$5$API$7OQpaXYXv0jbVOfwzdtQ3Gek7rT/MXdSyibahaAS3W/" }, // 광무
                { "C53155632", "$5$API$ZTlTdHPNO9nN4FFPYdBqSqOoKZKsy50/3cO9NP.qZG4" }, // 대양
                { "A07E67034", "$5$API$XiMBhWIUg.rz39Q15GxRuYHGYnNAOPC8p.LcQlmBkH8" }, // 신우
                { "A07E265671", "$5$API$BQJWitsO4EMnhC8Va/qEdE.ud2Uj4l9CdJ3kUVfiSb5" }, // 누리
                { "A07E144572", "$5$API$htyE4jLhKRKANA7EJqLa/n7XuH1wiY33NlCbDVWxKx1" }, // 우리
                { "F2A6214640", "$5$API$oeowtfLWrN5dLzEkTmVXaIGamdOJsRHy.t9PZ08R5ED" }, // 혜성
                { "D2E31043", "$5$API$e.xyAf/XO.s.39o56dy5CwbieTCATkYKu/5oJLJo/33" }, // 주)혜성
                { "5579217615", "$5$API$sQrmHHmidufXn9kMU21tR97uywZVIPWNFo9R7CZWr8B" }, // 성주
                { "13C554403", "$5$API$.q11WFEWM3minNDNHFXie3LeUFtUSC5aAEpI9EA6x88" }, // 지온
                { "C919111143", "$5$API$Ji3O4bFySk437RPI7sALQ1dyynUlWmt3SyqSP/h9bN2" }, // 두성
                { "C91950615", "$5$API$.NliYTwsduENTDEMjTcjtI9hppOfaLFwDCxdb7g6TU5" }, // 대세
                { "766562426", "$5$API$vHQrr9yDymkTOgbP4uUPfMfh6KRn/qmbJ9LfwRRRUl9" }, // 구룡
                { "9DBD134371", "$5$API$osZ29gtnQgI8k4cCxXgKLm1f7CL6MR9X5dMryhMJdTB" }  // 금화
            };
            return crtfcKeys.TryGetValue(corp_id, out string crtfcKey) ? crtfcKey : "";
        }
    }
}
