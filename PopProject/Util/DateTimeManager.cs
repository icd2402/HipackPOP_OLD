using System;
using System.Windows.Threading;

namespace PopProject.Util
{
    public static class DateTimeManager
    {
        static private DispatcherTimer timer;
        static private DateTime nowDT = new DateTime();
        static private DateTime selectedDT = new DateTime();

        static DateTimeManager()
        {
            //초기 설정
            nowDT = DateTime.Now;
            selectedDT = DateTime.Now;
            StartTimer();
        }

        private static void StartTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        //시간만 실시간으로 갱신
        private static void Timer_Tick(object sender, EventArgs e)
        {
            nowDT = DateTime.Now;
            selectedDT = new DateTime(selectedDT.Year, selectedDT.Month, selectedDT.Day, nowDT.Hour, nowDT.Minute, nowDT.Second);
        }

        // 현재 날짜와 시간 반환
        public static DateTime GetNowDt()
        {
            return nowDT;
        }

        public static DateTime GetNowDate()
        {
            return nowDT.Date;
        }

        public static string GetNowDateStr()
        {
            return nowDT.ToString("yyyy-MM-dd");
        }

        public static string GetNowTimeStr()
        {
            return nowDT.ToString("HH:mm:ss");
        }

        public static string GetNowStr()
        {
            return nowDT.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime GetSelectedDT()
        {
            return selectedDT;
        }

        public static DateTime GetSelectedDate()
        {
            return selectedDT.Date;
        }

        public static string GetSelectedDateStr()
        {
            return selectedDT.ToString("yyyy-MM-dd");
        }

        public static string GetSelectedTimeStr()
        {
            return selectedDT.ToString("HH:mm:ss");
        }

        public static string GetSelectedStr()
        {
            return selectedDT.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static bool CompareDateTime()
        {
            return DateTime.Equals(nowDT, selectedDT);
        }

        public static bool CompareDate()
        {
            return DateTime.Equals(GetNowDate(), GetSelectedDate());
        }

        // 전날짜로
        public static void SetPreviousDate()
        {
            selectedDT = selectedDT.AddDays(-1);
        }

        // 다음날짜로
        public static void SetNextDate()
        {
            selectedDT = selectedDT.AddDays(1);
        }

        //현재 날짜로 변경
        public static void SetTodayDate()
        {
            selectedDT = nowDT;
        }
    }
}
