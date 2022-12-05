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

        public void Setconnection()
        {
            Connect();
        }

        private void Connect()
        {
            연결 = new HubConnection(주소);
            허브 = 연결.CreateHubProxy("broadcasting");
            try
            {
                연결.Start().Wait();
                SignalRAdministrator.허브.Invoke("JoinCompany", Properties.Settings.Default.corp_id);
                //SignalRAdministrator.허브.Invoke("JoinCompany", "C4E5132036");
                Console.WriteLine("연결 => " + Properties.Settings.Default.corp_id);
                Console.WriteLine("연결상태 => " + 연결.State);
                Console.WriteLine("연결상태 => " + ConnectionState.Connected);
                연결.Closed += 연결_Closed;
            }

            catch (Exception)
            {
                
                //MessageBox_Dialog dialog = new MessageBox_Dialog("SignalR 서버 접속 불가");
                //dialog.ShowDialog();
            }
            //C4E5132036

        }

        private void 연결_Closed()
        {
            Connect();
        }
    }
}
