using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PopProject.Util
{
    class DateTimeManager
    {
        private static DateTimeManager instance;
        private DispatcherTimer timer;
        private string currentDateTime; // 날짜,시간
        private string currentDate; // 날짜
        private string currentTime; // 시간

        public static DateTimeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DateTimeManager();
                }
                return instance;
            }
        }

        public DateTimeManager()
        {
            //초기 설정
            currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            currentDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            StartTimer();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        //시간만 실시간으로 갱신
        private void Timer_Tick(object sender, EventArgs e)
        {
            currentTime = DateTime.Now.ToString("hh:mm:ss");
        }

        // 현재 날짜와 시간 반환
        public string GetCurrentDateTime()
        {
            return currentDate + " " + currentTime;
        }

        // 날짜만 반환
        public string GetCurrentDate()
        {
            return currentDate;
        }

        // 시간만 반환
        public string GetCurrentTime()
        {
            return currentTime;
        }

        // 전날짜로
        public void SetDreviousDate()
        {
            DateTime current = DateTime.Parse(currentDate); 
            current = current.AddDays(-1); 
            currentDate = current.ToString("yyyy-MM-dd"); 
        }

        // 다음날짜로
        public void SetNextDate()
        {
            DateTime current = DateTime.Parse(currentDate); 
            current = current.AddDays(1); 
            currentDate = current.ToString("yyyy-MM-dd");
        }

        //현재 날짜로 변경
        public void SetTodayDate()
        {
            currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
