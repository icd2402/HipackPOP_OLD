using System;
using System.IO;
using System.Net;
using System.Net.Http;

using System.Text;

namespace PopProject.Util
{

    class HttpClient_util
    {
        private string url = "http://hipackcom.co.kr";
        private string commend = string.Empty;
        private HttpClient httpClient;

        public HttpClient_util(string commend)
        {
            httpClient = new HttpClient();
            this.commend = commend;
            url += "/" + commend;
        }
        public HttpClient_util(string url, string jsonData)
        {

            this.url = url;
            SendLog(jsonData);
        }

        public string SendHttp(string sendMessage)
        {
            string result = string.Empty;
            try
            {

                Uri uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = 5000;

                byte[] data = Encoding.UTF8.GetBytes(sendMessage);
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = data.Length;

                // 전송 처리 
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                //응답 처리
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();

            }

            catch (Exception e)

            {
                Console.WriteLine(e.Message);
                return result;
            }

            //Console.WriteLine("전송 값 : " + url + sendMessage);
            //Console.WriteLine("응답 값 : " + result);
            return result;
        }

        public string SendLog(string sendMessage)
        {
            string result = string.Empty;

            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = 5000;
                request.ContentType = "application/json; charset=utf-8";


                // 전송 처리 
                Stream dataStream = request.GetRequestStream();
                dataStream.Close();

                //응답 처리
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();

            }

            catch (Exception e)

            {
                Console.WriteLine(e.Message);
                return result;
            }

            //Console.WriteLine("전송 값 : " + url + sendMessage);
            //Console.WriteLine("응답 값 : " + result);

            return result;
        }
    }
}
