using Microsoft.AspNet.SignalR.Client;
using PopProject.MessageBox;
using System;

namespace PopProject.Util
{
    class SignalRAdministrator 
    {
        public static HubConnection 연결;
        public static IHubProxy 허브;
        public static string 주소 = Properties.Settings.Default.signalR.ToString();
            
        public SignalRAdministrator()
        {
            Console.WriteLine("Signalr 주소 : " + Properties.Settings.Default.signalR.ToString());
        }

        public void Setconnection(bool Request)
        {
            Connect(Request);
        }

        //연결
        private void Connect(bool request)
        {
            //연결 요청
            if (request)
            {
                연결 = new HubConnection(주소);
                허브 = 연결.CreateHubProxy("broadcasting");
                연결.StateChanged += Connection_StateChanged;
                try
                {
                    연결.Start().Wait();
                    SignalRAdministrator.허브.Invoke("JoinCompany", Properties.Settings.Default.corp_id);
                    Console.WriteLine("연결 => " + Properties.Settings.Default.corp_id);
                    Console.WriteLine("연결상태 => " + 연결.State);
                    Console.WriteLine("연결상태 => " + ConnectionState.Connected);
                    연결.Closed += 연결_Closed;
                }
                catch (Exception)
                {
                    if (연결.State == ConnectionState.Disconnected)
                    {
                        MessageBox_Dialog dialog = new MessageBox_Dialog("SignalR 서버 접속 실패!");
                        dialog.ShowDialog();
                    }
                }
            }
            //닫기 요청
            else
            {
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

            if (연결.State == ConnectionState.Disconnected)
            {
                Connect(true);
            }
        }

        //연결실패시 다시 시도
        private void 연결_Closed()
        {
            Connect(true);
            MessageBox_Dialog dialog = new MessageBox_Dialog("SignalR 서버에 재연결 합니다.");
            dialog.ShowDialog();
        }
    }
}
