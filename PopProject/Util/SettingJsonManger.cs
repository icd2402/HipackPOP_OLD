using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using log4net;
using log4net.Config;
using PopProject.UserControl_;

namespace PopProject.Util
{
    public static class SettingJsonManager
    {
        public static string login_id { get; set; }
        public static string login_password { get; set; }
        public static string corp_name { get; set; }
        public static string corp_id { get; set; }
        public static bool login_check { get; set; }
        public static string mercName { get; set; }
        public static string SerialName { get; set; }
        public static int processType { get; set; }
        public static int hour { get; set; }
        public static int minute { get; set; }
        public static string url { get; set; }
        public static string saveDate { get; set; }
        public static int pplanKey { get; set; }
        public static int count { get; set; }
        public static string UserID { get; set; }
        public static string UserName { get; set; }
        public static string signalR { get; set; }
        public static string serverIndex { get; set; }
        public static string pworkkey { get; set; }
        public static float consumersumcount { get; set; }
        public static int consumpWorkKey { get; set; }
        public static float consumersumerrorcount { get; set; }
        public static int TypeIndex { get; set; }
        public static int CountType { get; set; }
        public static string settingitem { get; set; }
        public static string meterialinfo { get; set; }
        public static int setting_check { get; set; }
        public static int baudrate { get; set; }
        public static string SensorRate { get; set; }
        public static bool RadioChecked { get; set; }
        public static string printItem { get; set; }
        public static bool samework { get; set; }
        public static bool color_check { get; set; }
        public static bool count_check { get; set; }
        public static bool boxCheck { get; set; }
        public static bool searchCheck { get; set; }
        public static bool keyinCheck { get; set; }
        public static bool detail_check { get; set; }
        public static bool option1 { get; set; }
        public static float presultcount { get; set; }
        public static bool twoSum { get; set; }
        public static bool accountCheck { get; set; }
        public static bool productionCheck { get; set; }
        public static bool scrollvarCheck { get; set; }
        public static int index { get; set; }
        public static string error { get; set; }
        public static string error1 { get; set; }
        public static bool Plc { get; set; }
        public static bool ChangeCheck { get; set; }
        public static string ip { get; set; }
        public static string port { get; set; }
        public static int Gap { get; set; }
        public static int mimi { get; set; }
        public static int jang { get; set; }
        public static int pok { get; set; }
        public static int go { get; set; }
        public static int setCount { get; set; }
        public static int boxType { get; set; }
        public static bool plcCheck { get; set; }
        public static bool completeValue_checked { get; set; }
        public static bool checkMerc { get; set; }
        public static bool robotchecked { get; set; }
        public static string robotIP { get; set; }
        public static string robotPort { get; set; }
        public static string manager { get; set; }
        public static string authKey { get; set; }
        public static string SerialName1 { get; set; }
        public static int consumCount { get; set; }
        public static bool dbUse { get; set; }


        // JSON 파일 경로
        public static string filePath { get; set; }

        // JSON 파일 내 정보 string 변수
        private static string loadedSettings;

        // log4net
        private static ILog log; //로그

        static SettingJsonManager()
        {
            filePath = @"C:\ICD\settings.json";

            loadedSettings = File.ReadAllText(filePath);

            //Log4net
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));
            log = LogManager.GetLogger(typeof(Login)); //로그
        }
        
        public static bool Init()
        {
            log.Info("셋팅정보(json) Load 시작");

            try
            {
                JObject jObject = JObject.Parse(loadedSettings);

                login_id = jObject["login_id"].ToString();
                login_password = jObject["login_password"].ToString();
                corp_name = jObject["corp_name"].ToString();
                corp_id = jObject["corp_id"].ToString();
                login_check = bool.Parse(jObject["login_check"].ToString());
                mercName = jObject["mercName"].ToString();
                SerialName = jObject["SerialName"].ToString();
                processType = Convert.ToInt32(jObject["processType"]);
                hour = Convert.ToInt32(jObject["hour"]);
                minute = Convert.ToInt32(jObject["minute"]);
                url = jObject["url"].ToString();
                saveDate = jObject["saveDate"].ToString();
                pplanKey = Convert.ToInt32(jObject["pplanKey"]);
                count = Convert.ToInt32(jObject["count"]);
                UserID = jObject["UserID"].ToString();
                UserName = jObject["UserName"].ToString();
                signalR = jObject["signalR"].ToString();
                serverIndex = jObject["serverIndex"].ToString();
                pworkkey = jObject["pworkkey"].ToString();
                consumersumcount = float.Parse(jObject["consumersumcount"].ToString());
                consumpWorkKey = Convert.ToInt32(jObject["consumpWorkKey"]);
                consumersumerrorcount = float.Parse(jObject["consumersumerrorcount"].ToString());
                TypeIndex = Convert.ToInt32(jObject["TypeIndex"]);
                CountType = Convert.ToInt32(jObject["CountType"]);
                settingitem = jObject["settingitem"].ToString();
                meterialinfo = jObject["meterialinfo"].ToString();
                setting_check = Convert.ToInt32(jObject["setting_check"]);
                baudrate = Convert.ToInt32(jObject["baudrate"]);
                SensorRate = jObject["SensorRate"].ToString();
                RadioChecked = bool.Parse(jObject["RadioChecked"].ToString());
                printItem = jObject["printItem"].ToString();
                samework = bool.Parse(jObject["samework"].ToString());
                color_check = bool.Parse(jObject["color_check"].ToString());
                count_check = bool.Parse(jObject["count_check"].ToString());
                boxCheck = bool.Parse(jObject["boxCheck"].ToString());
                searchCheck = bool.Parse(jObject["searchCheck"].ToString());
                keyinCheck = bool.Parse(jObject["keyinCheck"].ToString());
                detail_check = bool.Parse(jObject["detail_check"].ToString());
                option1 = bool.Parse(jObject["option1"].ToString());
                presultcount = float.Parse(jObject["presultcount"].ToString());
                twoSum = bool.Parse(jObject["twoSum"].ToString());
                accountCheck = bool.Parse(jObject["accountCheck"].ToString());
                productionCheck = bool.Parse(jObject["productionCheck"].ToString());
                scrollvarCheck = bool.Parse(jObject["scrollvarCheck"].ToString());
                index = Convert.ToInt32(jObject["index"]);
                error = jObject["error"].ToString();
                error1 = jObject["error1"].ToString();
                Plc = bool.Parse(jObject["Plc"].ToString());
                ChangeCheck = bool.Parse(jObject["ChangeCheck"].ToString());
                ip = jObject["ip"].ToString();
                port = jObject["port"].ToString();
                Gap = Convert.ToInt32(jObject["Gap"]);
                mimi = Convert.ToInt32(jObject["mimi"]);
                jang = Convert.ToInt32(jObject["jang"]);
                pok = Convert.ToInt32(jObject["pok"]);
                go = Convert.ToInt32(jObject["go"]);
                setCount = Convert.ToInt32(jObject["setCount"]);
                boxType = Convert.ToInt32(jObject["boxType"]);
                plcCheck = bool.Parse(jObject["plcCheck"].ToString());
                completeValue_checked = bool.Parse(jObject["completeValue_checked"].ToString());
                checkMerc = bool.Parse(jObject["checkMerc"].ToString());
                robotchecked = bool.Parse(jObject["robotchecked"].ToString());
                robotIP = jObject["robotIP"].ToString();
                robotPort = jObject["robotPort"].ToString();
                manager = jObject["manager"].ToString();
                authKey = jObject["authKey"].ToString();
                SerialName1 = jObject["SerialName1"].ToString();
                consumCount = Convert.ToInt32(jObject["consumCount"]);
                // dbUse 없는 경우 확인
                dbUse = jObject.ContainsKey("dbUse") && bool.Parse(jObject["dbUse"].ToString()) ? true : false;

                log.Info("셋팅정보(json) Load 완료");

                // 인증 정보가 없는 경우(Settings.Settings 또한 입력)
                if (!string.IsNullOrWhiteSpace(login_id) && !string.IsNullOrWhiteSpace(login_password))
                {
                    SaveConfigFile();
                }
            }
            catch (Exception ex)
            {
                log.Error("셋팅정보(json) Load 실패: " + ex.Message);
                return false;
            }
            return true;
        }

        // Settings.Settings (user.config 파일)에 정보 저장
        public static bool SaveConfigFile()
        {
            log.Info("인증정보 없음. 셋팅(json) → 셋팅(user.config) 입력 시작");

            try {
                Properties.Settings.Default.login_id = login_id;
                Properties.Settings.Default.login_password = login_password;
                Properties.Settings.Default.corp_name = corp_name;
                Properties.Settings.Default.corp_id = corp_id;
                Properties.Settings.Default.login_check = login_check;
                Properties.Settings.Default.mercName = mercName;
                Properties.Settings.Default.SerialName = SerialName;
                Properties.Settings.Default.processType = processType;
                Properties.Settings.Default.hour = hour;
                Properties.Settings.Default.minute = minute;
                Properties.Settings.Default.url = url;
                Properties.Settings.Default.saveDate = saveDate;
                Properties.Settings.Default.pplanKey = pplanKey;
                Properties.Settings.Default.count = count;
                Properties.Settings.Default.UserID = UserID;
                Properties.Settings.Default.UserName = UserName;
                Properties.Settings.Default.signalR = signalR;
                Properties.Settings.Default.serverIndex = serverIndex;
                Properties.Settings.Default.pworkkey = pworkkey;
                Properties.Settings.Default.consumersumcount = consumersumcount;
                Properties.Settings.Default.consumpWorkKey = consumpWorkKey;
                Properties.Settings.Default.consumersumerrorcount = consumersumerrorcount;
                Properties.Settings.Default.TypeIndex = TypeIndex;
                Properties.Settings.Default.CountType = CountType;
                Properties.Settings.Default.settingitem = settingitem;
                Properties.Settings.Default.meterialinfo = meterialinfo;
                Properties.Settings.Default.setting_check = setting_check;
                Properties.Settings.Default.baudrate = baudrate;
                Properties.Settings.Default.SensorRate = SensorRate;
                Properties.Settings.Default.RadioChecked = RadioChecked;
                Properties.Settings.Default.printItem = printItem;
                Properties.Settings.Default.samework = samework;
                Properties.Settings.Default.color_check = color_check;
                Properties.Settings.Default.count_check = count_check;
                Properties.Settings.Default.boxCheck = boxCheck;
                Properties.Settings.Default.searchCheck = searchCheck;
                Properties.Settings.Default.keyinCheck = keyinCheck;
                Properties.Settings.Default.detail_check = detail_check;
                Properties.Settings.Default.option1 = option1;
                Properties.Settings.Default.presultcount = presultcount;
                Properties.Settings.Default.twoSum = twoSum;
                Properties.Settings.Default.accountCheck = accountCheck;
                Properties.Settings.Default.productionCheck = productionCheck;
                Properties.Settings.Default.scrollvarCheck = scrollvarCheck;
                Properties.Settings.Default.index = index;
                Properties.Settings.Default.error = error;
                Properties.Settings.Default.error1 = error1;
                Properties.Settings.Default.Plc = Plc;
                Properties.Settings.Default.ChangeCheck = ChangeCheck;
                Properties.Settings.Default.ip = ip;
                Properties.Settings.Default.port = port;
                Properties.Settings.Default.Gap = Gap;
                Properties.Settings.Default.mimi = mimi;
                Properties.Settings.Default.jang = jang;
                Properties.Settings.Default.pok = pok;
                Properties.Settings.Default.go = go;
                Properties.Settings.Default.setCount = setCount;
                Properties.Settings.Default.boxType = boxType;
                Properties.Settings.Default.plcCheck = plcCheck;
                Properties.Settings.Default.completeValue_checked = completeValue_checked;
                Properties.Settings.Default.checkMerc = checkMerc;
                Properties.Settings.Default.robotchecked = robotchecked;
                Properties.Settings.Default.robotIP = robotIP;
                Properties.Settings.Default.robotPort = robotPort;
                Properties.Settings.Default.manager = manager;
                Properties.Settings.Default.authKey = authKey;
                Properties.Settings.Default.SerialName1 = SerialName1;
                Properties.Settings.Default.consumCount = consumCount;
                Properties.Settings.Default.dbUse = dbUse;

                log.Info("셋팅(json) → 셋팅(user.config) 정보 입력 완료");
            }
            catch (Exception ex)
            {
                log.Error("셋팅(json) → 셋팅(user.config) 정보 입력 실패: " + ex.Message);
                return false;
            }
            return true;
        }

        // JSON 설정 (settings.json) 파일에 저장
        public static bool SaveJsonFile()
        {
            try
            {
                //셋팅파일 경로가 존재하는지 확인 후 경로 생성
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                log.Info("JSON 파일 저장 시작");

                //파일 내용
                var settings = new Dictionary<string, string>
                            {
                                {"login_id", login_id},
                                {"login_password", login_password},
                                {"corp_name", corp_name},
                                {"corp_id", corp_id},
                                {"login_check", login_check.ToString()},
                                {"mercName", mercName},
                                {"SerialName", SerialName},
                                {"processType", processType.ToString()},
                                {"hour", hour.ToString()},
                                {"minute", minute.ToString()},
                                {"url", url},
                                {"saveDate", saveDate},
                                {"pplanKey", pplanKey.ToString()},
                                {"count", count.ToString()},
                                {"UserID", UserID},
                                {"UserName", UserName},
                                {"signalR", signalR},
                                {"serverIndex", serverIndex},
                                {"pworkkey", pworkkey},
                                {"consumersumcount", consumersumcount.ToString()},
                                {"consumpWorkKey", consumpWorkKey.ToString()},
                                {"consumersumerrorcount", consumersumerrorcount.ToString()},
                                {"TypeIndex", TypeIndex.ToString()},
                                {"CountType", CountType.ToString()},
                                {"settingitem", settingitem},
                                {"meterialinfo", meterialinfo},
                                {"setting_check", setting_check.ToString()},
                                {"baudrate", baudrate.ToString()},
                                {"SensorRate", SensorRate},
                                {"RadioChecked", RadioChecked.ToString()},
                                {"printItem", printItem},
                                {"samework", samework.ToString()},
                                {"color_check", color_check.ToString()},
                                {"count_check", count_check.ToString()},
                                {"boxCheck", boxCheck.ToString()},
                                {"searchCheck", searchCheck.ToString()},
                                {"keyinCheck", keyinCheck.ToString()},
                                {"detail_check", detail_check.ToString()},
                                {"option1", option1.ToString()},
                                {"presultcount", presultcount.ToString()},
                                {"twoSum", twoSum.ToString()},
                                {"accountCheck", accountCheck.ToString()},
                                {"productionCheck", productionCheck.ToString()},
                                {"scrollvarCheck", scrollvarCheck.ToString()},
                                {"index", index.ToString()},
                                {"error", error},
                                {"error1", error1},
                                {"Plc", Plc.ToString()},
                                {"ChangeCheck", ChangeCheck.ToString()},
                                {"ip", ip},
                                {"port", port},
                                {"Gap", Gap.ToString()},
                                {"mimi", mimi.ToString()},
                                {"jang", jang.ToString()},
                                {"pok", pok.ToString()},
                                {"go", go.ToString()},
                                {"setCount", setCount.ToString()},
                                {"boxType", boxType.ToString()},
                                {"plcCheck", plcCheck.ToString()},
                                {"completeValue_checked", completeValue_checked.ToString()},
                                {"checkMerc", checkMerc.ToString()},
                                {"robotchecked", robotchecked.ToString()},
                                {"robotIP", robotIP},
                                {"robotPort", robotPort},
                                {"manager", manager},
                                {"authKey", authKey},
                                {"SerialName1", SerialName1},
                                {"consumCount", consumCount.ToString()},
                                {"dbUse", dbUse.ToString() }
                            };

                //인증코드 파일 작성
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

                //인증코드 파일이 있는지 확인 후 해당 파일을 숨김 처리 (숨김 이후 수정되지 않음)
                File.WriteAllText(filePath, json);

                log.Info("JSON 파일 저장 완료");
            }
            catch (Exception ex)
            {
                log.Error("JSON 파일 저장 실패: " + ex.Message);
                return false;
            }
            return true;
        }
    }
}