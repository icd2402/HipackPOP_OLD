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
        private string crtfcKey = "";
        private string logDt = "";
        private string useSe = "";
        private string sysUser = "";
        private string conectIp = "";
        private int dataUsgqty = 0;

        public LogData(string useSe)
        {
            this.useSe = useSe;
            crtfcKey = CrtfcKey(Properties.Settings.Default.corp_id);
            logDt = GetTime();
            sysUser = Properties.Settings.Default.mercName;

            conectIp = GetIP();
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

        public string GetTime()
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return time;
        }


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

        public string CrtfcKey(string corp_id)
        {
            string crtfcKey;
            switch (corp_id)
            {
                case "F2A6214640":
                    {
                        // 혜성: $5$API$WFRMJMEi/JIWzHjSbEFNqO54g5ewV94Q4ppXtiM5ou2
                        crtfcKey = "$5$API$oeowtfLWrN5dLzEkTmVXaIGamdOJsRHy.t9PZ08R5ED";
                        break;
                    }

                case "75BD170261":
                    {
                        // 정일: $5$API$9pgZWUx7Qgy0z/asRpn4l6VS.a8A0E.mfo2Yjb0BLjD
                        crtfcKey = "$5$API$9pgZWUx7Qgy0z/asRpn4l6VS.a8A0E.mfo2Yjb0BLjD";
                        break;
                    }

                case "7C2564433":
                    {
                        // 거성: $5$API$gswLJkqbSGriKu8Gt4Nh3ycrrEeQbxJl6tSHDE.JRw7
                        crtfcKey = "$5$API$gswLJkqbSGriKu8Gt4Nh3ycrrEeQbxJl6tSHDE.JRw7";
                        break;
                    }

                case "C4E5132036":
                    {
                        // 미래: $5$API$1gtA7rTAzERz8hh18pTixIkIp6yDhq0vYA5gi6TkEQ1
                        crtfcKey = "$5$API$1gtA7rTAzERz8hh18pTixIkIp6yDhq0vYA5gi6TkEQ1";
                        break;
                    }

                case "3464250413":
                    {
                        // 광성: $5$API$Srsh8f7I/QgnjX2QC3IIYF0bfChx6YmERjK5n/Q2OQA
                        crtfcKey = "$5$API$Srsh8f7I/QgnjX2QC3IIYF0bfChx6YmERjK5n/Q2OQA";
                        break;
                    }

                case "A07E164641":
                    {
                        // 하나: $5$API$tVvR3cwTa7CTErv3wui/zXIh8pUUxhD73TjckvwqjN7
                        crtfcKey = "$5$API$tVvR3cwTa7CTErv3wui/zXIh8pUUxhD73TjckvwqjN7";
                        break;
                    }

                case "75BB212362":
                    {
                        // 선호: $5$API$Lqb0ylQQhtW2lYmGTA8oBnVnBiFMA5JGiu8XpA539.7
                        crtfcKey = "$5$API$Lqb0ylQQhtW2lYmGTA8oBnVnBiFMA5JGiu8XpA539.7";
                        break;
                    }

                case "C8B516524":
                    {
                        // 유창:
                        crtfcKey = "$5$API$dOlFU/fI7Muxm1h40PiUJj5VSmdxXLOIxArRMV0sS.4";
                        break;
                    }

                case "D85A2100":
                    {
                        // 광무: $5$API$7OQpaXYXv0jbVOfwzdtQ3Gek7rT/MXdSyibahaAS3W/
                        crtfcKey = "$5$API$7OQpaXYXv0jbVOfwzdtQ3Gek7rT/MXdSyibahaAS3W/";
                        break;
                    }
                case "A07E67034":
                    {
                        // 신우: $5$API$XiMBhWIUg.rz39Q15GxRuYHGYnNAOPC8p.LcQlmBkH8
                        crtfcKey = "$5$API$XiMBhWIUg.rz39Q15GxRuYHGYnNAOPC8p.LcQlmBkH8";
                        break;
                    }

                case "A07E265671":
                    {
                        // 누리: $5$API$BQJWitsO4EMnhC8Va/qEdE.ud2Uj4l9CdJ3kUVfiSb5
                        crtfcKey = "$5$API$BQJWitsO4EMnhC8Va/qEdE.ud2Uj4l9CdJ3kUVfiSb5";
                        break;
                    }

                case "A07E144572":
                    //우리:   $5$API$3ZoVWHfWxIlw1bvMuIy1VONQGALcFNv4MmsqCpLC1n4
                    crtfcKey = "$5$API$3ZoVWHfWxIlw1bvMuIy1VONQGALcFNv4MmsqCpLC1n4";
                    break;

                case "3125247246":
                    //오주:   $5$API$88YVlKU1Ieysybu0wsbdrxgZ9XIk2M8t5NC3ex/FHs9
                    crtfcKey = "$5$API$88YVlKU1Ieysybu0wsbdrxgZ9XIk2M8t5NC3ex/FHs9";
                    break;

                case "C53155632":
                    //대양 : $5$API$ZTlTdHPNO9nN4FFPYdBqSqOoKZKsy50/3cO9NP.qZG4
                    crtfcKey = "$5$API$ZTlTdHPNO9nN4FFPYdBqSqOoKZKsy50/3cO9NP.qZG4";
                    break;

                default:
                    {
                        crtfcKey = "";
                        break;
                    }

            }
            return crtfcKey;
        }
    }
}
