using Microsoft.AspNet.SignalR.Client;
using PopProject.MessageBox;
using System;
using log4net;
using log4net.Config;
using System.Windows;
using System.IO;


namespace PopProject.Util
{
    class SignalRAdministrator 
    {
        public static HubConnection 연결;
        public static IHubProxy 허브;
        public static string 주소 = Properties.Settings.Default.signalR.ToString();
        public ILog log;


        public SignalRAdministrator()
        {
            Console.WriteLine("Signalr 주소 : " + Properties.Settings.Default.signalR.ToString());
        }

        //SignalR 연결 요청
        public void Setconnection(bool Request)
        {
            Connect(Request);
        }

        //연결
        private void Connect(bool request)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));
            log = LogManager.GetLogger(typeof(SignalRAdministrator));

            //연결 요청
            if (request)
            {
                연결 = new HubConnection(주소); //연결 주소 설정
                허브 = 연결.CreateHubProxy("broadcasting"); //broadcasting이라는 허브프록시 생성
                연결.StateChanged += Connection_StateChanged; //연결 상태가 변경될때 실행할 이벤트
                연결.Closed += 연결_Closed; //연결이 닫힐때 실행할 이벤트

                try
                {
                    연결.Start().Wait(); //연결 시작
                    SignalRAdministrator.허브.Invoke("JoinCompany", Properties.Settings.Default.corp_id); //SignalR서버의 JoinCompany 메서드 호출

                    log.Info("SignalR 연결상태: " + 연결.State);
                }
                catch (Exception e)
                {
                    log.Error(e.Message);

                    if (연결.State == ConnectionState.Disconnected)
                    {
                        if (!Properties.Settings.Default.corp_name.Equals("오주포장") || !Properties.Settings.Default.corp_name.Equals("광성수출포장"))
                        {
                            MessageBox_Dialog dialog = new MessageBox_Dialog("SignalR 서버 접속 실패!");
                            dialog.ShowDialog();
                        }
                    }
                }
            }
            //닫기 요청
            else
            {
                log.Info("SignalR 닫기");

                if (연결.State == ConnectionState.Connected)
                {
                    연결.Stop();
                }
            }
        }

        //연결상태 조회
        private void Connection_StateChanged(StateChange stateChange)
        {
            // 연결 상태 변화 감지 시 실행될 코드
            Console.WriteLine("연결 상태 변화: 이전 상태 - " + stateChange.OldState + " / 새로운 상태 - " + stateChange.NewState);
        }

        //연결 실패시 다시 시도
        private void 연결_Closed()
        {
            //if (!Properties.Settings.Default.corp_name.Equals("오주포장") || !Properties.Settings.Default.corp_name.Equals("유창포장"))
            //{
            //    Connect(true);
            //}
        }
    }
}
