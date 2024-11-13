using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
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
            string crtfcKey;
            switch (corp_id)
            {
                case "75BD170261": //정일
                    {
                        crtfcKey = "$5$API$9pgZWUx7Qgy0z/asRpn4l6VS.a8A0E.mfo2Yjb0BLjD";
                        break;
                    }

                case "7C2564433": //거성
                    {
                        crtfcKey = "$5$API$gswLJkqbSGriKu8Gt4Nh3ycrrEeQbxJl6tSHDE.JRw7";
                        break;
                    }

                case "3125247246": //오주
                    {
                        crtfcKey = "$5$API$vdZaR4Lyk12Z/NhS3bXUUKYZsP9gLgnx20GckLkfPk3";
                        break;
                    }

                case "C4E5132036": //미래
                    {
                        crtfcKey = "$5$API$1gtA7rTAzERz8hh18pTixIkIp6yDhq0vYA5gi6TkEQ1";
                        break;
                    }

                case "A07E164641": //하나
                    {
                        crtfcKey = "$5$API$tVvR3cwTa7CTErv3wui/zXIh8pUUxhD73TjckvwqjN7";
                        break;
                    }

                case "3464250413": //광성
                    {
                        crtfcKey = "$5$API$Srsh8f7I/QgnjX2QC3IIYF0bfChx6YmERjK5n/Q2OQA";
                        break;
                    }
                
                case "75BB212362": //선호
                    {
                        crtfcKey = "$5$API$Lqb0ylQQhtW2lYmGTA8oBnVnBiFMA5JGiu8XpA539.7";
                        break;
                    }

                case "C8B516524": //유창
                    {
                        crtfcKey = "$5$API$dOlFU/fI7Muxm1h40PiUJj5VSmdxXLOIxArRMV0sS.4";
                        break;
                    }

                case "D85A2100": //광무
                    {
                        crtfcKey = "$5$API$7OQpaXYXv0jbVOfwzdtQ3Gek7rT/MXdSyibahaAS3W/";
                        break;
                    }

                case "C53155632": //대양
                    {
                        crtfcKey = "$5$API$ZTlTdHPNO9nN4FFPYdBqSqOoKZKsy50/3cO9NP.qZG4";
                        break;
                    }

                case "A07E67034": //신우
                    {
                        crtfcKey = "$5$API$XiMBhWIUg.rz39Q15GxRuYHGYnNAOPC8p.LcQlmBkH8";
                        break;
                    }

                case "A07E265671": //누리
                    {
                        crtfcKey = "$5$API$BQJWitsO4EMnhC8Va/qEdE.ud2Uj4l9CdJ3kUVfiSb5";
                        break;
                    }

                case "A07E144572": //우리
                    {
                        crtfcKey = "$5$API$htyE4jLhKRKANA7EJqLa/n7XuH1wiY33NlCbDVWxKx1";
                        break;
                    }

                case "F2A6214640": //혜성
                    {
                        crtfcKey = "$5$API$oeowtfLWrN5dLzEkTmVXaIGamdOJsRHy.t9PZ08R5ED";
                        break;
                    }

                case "D2E31043": //주)혜성
                    {
                        crtfcKey = "$5$API$e.xyAf/XO.s.39o56dy5CwbieTCATkYKu/5oJLJo/33";
                        break;
                    }

                case "5579217615": //성주
                    {
                        crtfcKey = "$5$API$sQrmHHmidufXn9kMU21tR97uywZVIPWNFo9R7CZWr8B";
                        break;
                    }

                case "13C554403": //지온
                    {
                        crtfcKey = "$5$API$.q11WFEWM3minNDNHFXie3LeUFtUSC5aAEpI9EA6x88";
                        break;
                    }

                case "C919111143": //두성
                    {
                        crtfcKey = "$5$API$Ji3O4bFySk437RPI7sALQ1dyynUlWmt3SyqSP/h9bN2";
                        break;
                    }

                case "C91950615": //대세
                    {
                        crtfcKey = "$5$API$.NliYTwsduENTDEMjTcjtI9hppOfaLFwDCxdb7g6TU5";
                        break;
                    }

                case "766562426": //구룡
                    {
                        crtfcKey = "$5$API$vHQrr9yDymkTOgbP4uUPfMfh6KRn/qmbJ9LfwRRRUl9";
                        break;
                    }
               
                case "9DBD134371": //금화
                    {
                        crtfcKey = "$5$API$osZ29gtnQgI8k4cCxXgKLm1f7CL6MR9X5dMryhMJdTB";
                        break;
                    }

                default:
                    {
                        crtfcKey = "";
                        break;
                    }
            }
            return crtfcKey; //인증키 반환
        }
    }
}
