using Microsoft.AspNet.SignalR.Client;
using System;
using System.Reflection;


namespace PopProject.Util
{
    class SignalRAdministrator
    {
        public static HubConnection 연결;
        public static IHubProxy 허브;
        public static string 주소 = SettingJsonManager.signalR.ToString();


        public SignalRAdministrator()
        {
            //Console.WriteLine("Signalr 주소 : " + SettingJsonManager.signalR.ToString());
        }

        //SignalR 연결 요청
        public void Setconnection(bool Request)
        {
            Connect(Request);
        }

        //메인화면에서 통신상태 확인
        public ConnectionState GetState()
        {
            return 연결.State;
        }

        //연결
        private void Connect(bool request)
        {

            //연결 요청
            if (request)
            {
                연결 = new HubConnection(주소); //연결 주소 설정C
                허브 = 연결.CreateHubProxy("broadcasting"); //broadcasting이라는 허브프록시 생성
                연결.StateChanged += Connection_StateChanged; //연결 상태가 변경될때 실행할 이벤트
                연결.Closed += 연결_Closed; //연결이 닫힐때 실행할 이벤트

                try
                {
                    연결.Start().Wait(); //연결 시작
                    SignalRAdministrator.허브.Invoke("JoinCompany", SettingJsonManager.corp_id); //SignalR서버의 JoinCompany 메서드 호출

                    LogManager.Info("SignalR 연결상태: " + 연결.State, MethodBase.GetCurrentMethod().DeclaringType);
                }
                catch (Exception e)
                {
                    LogManager.Error("SignalR 연결 실패 : " + e.Message, MethodBase.GetCurrentMethod().DeclaringType);

                    if (연결.State == ConnectionState.Disconnected)
                    {
                    }
                }
            }
            //닫기 요청
            else
            {
                LogManager.Info("SignalR 닫기", MethodBase.GetCurrentMethod().DeclaringType);

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
            //if (!SettingJsonManager.corp_name.Equals("오주포장") || !SettingJsonManager.corp_name.Equals("유창포장"))
            //{
            //    Connect(true);
            //}
        }
    }
}
