
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using log4net;
using log4net.Config;

namespace PopProject
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Main main;
        public static Main1 main1;
        private Mutex mutex = null;
        public static string userID = "0";
        public static string userName = "0";

        public MainWindow()
        {

            InitializeComponent();

            try
            {
                //Properties.Settings.Default.Reset();
                if (Properties.Settings.Default.baudrate == 0)
                {
                    Properties.Settings.Default.baudrate = 9600;
                }
                if (Properties.Settings.Default.SensorRate.Length == 0)
                {
                    Properties.Settings.Default.SensorRate = "0";
                }
                if (Properties.Settings.Default.meterialinfo.Length <= 0)
                {
                    Properties.Settings.Default.meterialinfo = "";
                }

                Properties.Settings.Default.Save();
                Duplicate_execution("PopProject");

            }
            catch (Exception e)
            {
                LogWrite(e.Message);

            }

        }

        private void Duplicate_execution(string mutexName)
        {
            //InitializeComponent();
            //MainCP.Content = new Login();
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));
            ILog log = LogManager.GetLogger(typeof(SignalRAdministrator));

            try
            {
                mutex = new Mutex(false, mutexName);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                _ = System.Windows.MessageBox.Show("프로그램이 이미 실행 중입니다!", "알림1", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }

            if (mutex.WaitOne(0, false))
            {
                MainCP.Content = new Login();
            }
            else
            {
                log.Error("오류2");
                _ = System.Windows.MessageBox.Show("프로그램이 이미 실행 중입니다!", "알림2", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }

            
        }

        private void MainCP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            }
            catch (Exception h)
            {
                string message = h.Message;
                Console.WriteLine("Exception message : " + message);
            }
        }

        private void invokeMercName(string mercName, string v)
        {
            try
            {
                SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);
            }
            catch (Exception )
            {
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "Escape":
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }

        private void Win_closed(object sender, EventArgs e)
        {
            Console.WriteLine("종료됨");
            //Properties.Settings.Default.pworkkey = "";
            //Properties.Settings.Default.Save();

            //관리자 계정이 아닐때만 현황판으로 꺼짐 신호 보냄 
            if (!Properties.Settings.Default.manager.Equals("1"))
            {
                invokeMercName(Properties.Settings.Default.mercName, "B");
            }
        }

        private void LogWrite(string str)
        {
            string DirPath = Environment.CurrentDirectory + @"\Log";
            string FilePath = DirPath + "\\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
