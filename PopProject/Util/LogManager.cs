using log4net;
using System;

namespace PopProject.Util
{
    public static class LogManager
    {
        private static ILog log; //로그
        private static string folderPath;

        private static object lockInfo;
        private static object lockError;
        private static object lockWarn;
        private static object lockFatal;
        private static object lockDebug;

        static LogManager()
        {
            folderPath = @"C:\ICD\POP\logs\";

            // app.config 사용하지 않고 cs 로 옵션 설정. Activate 시 폴더/파일 자동 생성
            SetLogOptions();

            lockInfo = new object();
            lockError = new object();
            lockWarn = new object();
            lockFatal = new object();
            lockDebug = new object();

            //CheckFolderExits(folderPath);
        }

        public static void Info(string msg, Type classType)
        {
            lock (lockInfo)
            {
                string _msg = msg;
                string _classType = classType.ToString();

                log = log4net.LogManager.GetLogger(_classType);
                log.Info(_msg);
            }
        }

        public static void Error(string msg, Type classType)
        {
            lock (lockError)
            {
                string _msg = msg;
                string _classType = classType.ToString();

                log = log4net.LogManager.GetLogger(_classType);
                log.Error(_msg);
            }
        }

        public static void Warn(string msg, Type classType)
        {
            lock (lockWarn)
            {
                string _msg = msg;
                string _classType = classType.ToString();

                log = log4net.LogManager.GetLogger(_classType);
                log.Warn(_msg);
            }
        }

        public static void Fatal(string msg, Type classType)
        {
            lock (lockFatal)
            {
                string _msg = msg;
                string _classType = classType.ToString();

                log = log4net.LogManager.GetLogger(_classType);
                log.Fatal(_msg);
            }
        }

        public static void Debug(string msg, Type classType)
        {
            lock (lockDebug)
            {
                string _msg = msg;
                string _classType = classType.ToString();

                log = log4net.LogManager.GetLogger(_classType);
                log.Debug(_msg);
            }
        }

        private static void SetLogOptions()
        {
            log4net.Repository.Hierarchy.Hierarchy hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            hierarchy.Configured = true;

            log4net.Appender.RollingFileAppender rollingAppender = new log4net.Appender.RollingFileAppender();
            rollingAppender.File = folderPath;
            rollingAppender.AppendToFile = true;
            rollingAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rollingAppender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
            rollingAppender.DatePattern = "yyyy-MM-dd\".log\""; // 날짜가 지나간 경우 이전 로그에 붙을 이름 구성c
            rollingAppender.StaticLogFileName = false;
            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("[%date] [%level] [%thread] [%logger] %message%newline");
            rollingAppender.Layout = layout;

            hierarchy.Root.AddAppender(rollingAppender);

            // Activate 시 폴더/파일 없으면 자동으로 생성
            rollingAppender.ActivateOptions();

            hierarchy.Root.Level = log4net.Core.Level.All;
        }
    }
}