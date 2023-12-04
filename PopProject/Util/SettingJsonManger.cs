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

        public static bool LoadJsonFile()
        {
            log.Info("json → 셋팅정보(Class) Load 시작");

            try
            {
                JObject jObject = JObject.Parse(loadedSettings);

                // login_id 없는 경우 ""
                login_id = jObject.ContainsKey("login_id") ? jObject["login_id"].ToString() : "";
                // login_password 없는 경우 ""
                login_password = jObject.ContainsKey("login_password") ? jObject["login_password"].ToString() : "";
                // corp_name 없는 경우 ""
                corp_name = jObject.ContainsKey("corp_name") ? jObject["corp_name"].ToString() : "";
                // corp_id 없는 경우 ""
                corp_id = jObject.ContainsKey("corp_id") ? jObject["corp_id"].ToString() : "";
                // login_check 없는 경우 false
                login_check = jObject.ContainsKey("login_check") ? bool.Parse(jObject["login_check"].ToString()) : false;
                // mercName 없는 경우 ""
                mercName = jObject.ContainsKey("mercName") ? jObject["mercName"].ToString() : "";
                // SerialName 없는 경우 ""
                SerialName = jObject.ContainsKey("SerialName") ? jObject["SerialName"].ToString() : "";
                // processType 없는 경우 0
                processType = jObject.ContainsKey("processType") ? Convert.ToInt32(jObject["processType"]) : 0;
                // hour 없는 경우 0
                hour = jObject.ContainsKey("hour") ? Convert.ToInt32(jObject["hour"]) : 0;
                // minute 없는 경우 0
                minute = jObject.ContainsKey("minute") ? Convert.ToInt32(jObject["minute"]) : 0;
                // url 없는 경우 ""
                url = jObject.ContainsKey("url") ? jObject["url"].ToString() : "";
                // saveDate 없는 경우 ""
                saveDate = jObject.ContainsKey("saveDate") ? jObject["saveDate"].ToString() : "";
                // pplanKey 없는 경우 0
                pplanKey = jObject.ContainsKey("pplanKey") ? Convert.ToInt32(jObject["pplanKey"]) : 0;
                // count 없는 경우 0
                count = jObject.ContainsKey("count") ? Convert.ToInt32(jObject["count"]) : 0;
                // UserID 없는 경우 ""
                UserID = jObject.ContainsKey("UserID") ? jObject["UserID"].ToString() : "";
                // UserName 없는 경우 ""
                UserName = jObject.ContainsKey("UserName") ? jObject["UserName"].ToString() : "";
                // signalR 없는 경우 ""
                signalR = jObject.ContainsKey("signalR") ? jObject["signalR"].ToString() : "";
                // serverIndex 없는 경우 ""
                serverIndex = jObject.ContainsKey("serverIndex") ? jObject["serverIndex"].ToString() : "";
                // pworkkey 없는 경우 ""
                pworkkey = jObject.ContainsKey("pworkkey") ? jObject["pworkkey"].ToString() : "";
                // consumersumcount 없는 경우 0
                consumersumcount = jObject.ContainsKey("consumersumcount") ? float.Parse(jObject["consumersumcount"].ToString()) : 0;
                // consumpWorkKey 없는 경우 0
                consumpWorkKey = jObject.ContainsKey("consumpWorkKey") ? Convert.ToInt32(jObject["consumpWorkKey"]) : 0;
                // consumersumerrorcount 없는 경우 0
                consumersumerrorcount = jObject.ContainsKey("consumersumerrorcount") ? float.Parse(jObject["consumersumerrorcount"].ToString()) : 0;
                // TypeIndex 없는 경우 0
                TypeIndex = jObject.ContainsKey("TypeIndex") ? Convert.ToInt32(jObject["TypeIndex"]) : 0;
                // CountType 없는 경우 0
                CountType = jObject.ContainsKey("CountType") ? Convert.ToInt32(jObject["CountType"]) : 0;
                // settingitem 없는 경우 ""
                settingitem = jObject.ContainsKey("settingitem") ? jObject["settingitem"].ToString() : "";
                // meterialinfo 없는 경우 ""
                meterialinfo = jObject.ContainsKey("meterialinfo") ? jObject["meterialinfo"].ToString() : "";
                // setting_check 없는 경우 0
                setting_check = jObject.ContainsKey("setting_check") ? Convert.ToInt32(jObject["setting_check"]) : 0;
                // baudrate 없는 경우 0
                baudrate = jObject.ContainsKey("baudrate") ? Convert.ToInt32(jObject["baudrate"]) : 0;
                // SensorRate 없는 경우 ""
                SensorRate = jObject.ContainsKey("SensorRate") ? jObject["SensorRate"].ToString() : "";
                // RadioChecked 없는 경우 false
                RadioChecked = jObject.ContainsKey("RadioChecked") ? bool.Parse(jObject["RadioChecked"].ToString()) : false;
                // printItem 없는 경우 ""
                printItem = jObject.ContainsKey("printItem") ? jObject["printItem"].ToString() : "";
                // samework 없는 경우 false
                samework = jObject.ContainsKey("samework") ? bool.Parse(jObject["samework"].ToString()) : false;
                // color_check 없는 경우 false
                color_check = jObject.ContainsKey("color_check") ? bool.Parse(jObject["color_check"].ToString()) : false;
                // count_check 없는 경우 false
                count_check = jObject.ContainsKey("count_check") ? bool.Parse(jObject["count_check"].ToString()) : false;
                // boxCheck 없는 경우 false
                boxCheck = jObject.ContainsKey("boxCheck") ? bool.Parse(jObject["boxCheck"].ToString()) : false;
                // searchCheck 없는 경우 false
                searchCheck = jObject.ContainsKey("searchCheck") ? bool.Parse(jObject["searchCheck"].ToString()) : false;
                // keyinCheck 없는 경우 false
                keyinCheck = jObject.ContainsKey("keyinCheck") ? bool.Parse(jObject["keyinCheck"].ToString()) : false;
                // detail_check 없는 경우 false
                detail_check = jObject.ContainsKey("detail_check") ? bool.Parse(jObject["detail_check"].ToString()) : false;
                // option1 없는 경우 false
                option1 = jObject.ContainsKey("option1") ? bool.Parse(jObject["option1"].ToString()) : false;
                // presultcount 없는 경우 0
                presultcount = jObject.ContainsKey("presultcount") ? float.Parse(jObject["presultcount"].ToString()) : 0;
                // twoSum 없는 경우 false
                twoSum = jObject.ContainsKey("twoSum") ? bool.Parse(jObject["twoSum"].ToString()) : false;
                // accountCheck 없는 경우 false
                accountCheck = jObject.ContainsKey("accountCheck") ? bool.Parse(jObject["accountCheck"].ToString()) : false;
                // productionCheck 없는 경우 false
                productionCheck = jObject.ContainsKey("productionCheck") ? bool.Parse(jObject["productionCheck"].ToString()) : false;
                // scrollvarCheck 없는 경우 false
                scrollvarCheck = jObject.ContainsKey("scrollvarCheck") ? bool.Parse(jObject["scrollvarCheck"].ToString()) : false;
                // index 없는 경우 0
                index = jObject.ContainsKey("index") ? Convert.ToInt32(jObject["index"]) : 0;
                // error 없는 경우 ""
                error = jObject.ContainsKey("error") ? jObject["error"].ToString() : "";
                // error1 없는 경우 ""
                error1 = jObject.ContainsKey("error1") ? jObject["error1"].ToString() : "";
                // Plc 없는 경우 false
                Plc = jObject.ContainsKey("Plc") ? bool.Parse(jObject["Plc"].ToString()) : false;
                // ChangeCheck 없는 경우 false
                ChangeCheck = jObject.ContainsKey("ChangeCheck") ? bool.Parse(jObject["ChangeCheck"].ToString()) : false;
                // ip 없는 경우 ""
                ip = jObject.ContainsKey("ip") ? jObject["ip"].ToString() : "";
                // port 없는 경우 ""
                port = jObject.ContainsKey("port") ? jObject["port"].ToString() : "";
                // Gap 없는 경우 0
                Gap = jObject.ContainsKey("Gap") ? Convert.ToInt32(jObject["Gap"]) : 0;
                // mimi 없는 경우 0
                mimi = jObject.ContainsKey("mimi") ? Convert.ToInt32(jObject["mimi"]) : 0;
                // jang 없는 경우 0
                jang = jObject.ContainsKey("jang") ? Convert.ToInt32(jObject["jang"]) : 0;
                // pok 없는 경우 0
                pok = jObject.ContainsKey("pok") ? Convert.ToInt32(jObject["pok"]) : 0;
                // go 없는 경우 0
                go = jObject.ContainsKey("go") ? Convert.ToInt32(jObject["go"]) : 0;
                // setCount 없는 경우 0
                setCount = jObject.ContainsKey("setCount") ? Convert.ToInt32(jObject["setCount"]) : 0;
                // boxType 없는 경우 0
                boxType = jObject.ContainsKey("boxType") ? Convert.ToInt32(jObject["boxType"]) : 0;
                // plcCheck 없는 경우 false
                plcCheck = jObject.ContainsKey("plcCheck") ? bool.Parse(jObject["plcCheck"].ToString()) : false;
                // completeValue_checked 없는 경우 false
                completeValue_checked = jObject.ContainsKey("completeValue_checked") ? bool.Parse(jObject["completeValue_checked"].ToString()) : false;
                // checkMerc 없는 경우 false
                checkMerc = jObject.ContainsKey("checkMerc") ? bool.Parse(jObject["checkMerc"].ToString()) : false;
                // robotchecked 없는 경우 false
                robotchecked = jObject.ContainsKey("robotchecked") ? bool.Parse(jObject["robotchecked"].ToString()) : false;
                // robotIP 없는 경우 ""
                robotIP = jObject.ContainsKey("robotIP") ? jObject["robotIP"].ToString() : "";
                // robotPort 없는 경우 ""
                robotPort = jObject.ContainsKey("robotPort") ? jObject["robotPort"].ToString() : "";
                // manager 없는 경우 ""
                manager = jObject.ContainsKey("manager") ? jObject["manager"].ToString() : "";
                // authKey 없는 경우 ""
                authKey = jObject.ContainsKey("authKey") ? jObject["authKey"].ToString() : "";
                // SerialName1 없는 경우 ""
                SerialName1 = jObject.ContainsKey("SerialName1") ? jObject["SerialName1"].ToString() : "";
                // consumCount 없는 경우 0
                consumCount = jObject.ContainsKey("consumCount") ? Convert.ToInt32(jObject["consumCount"]) : 0;
                // dbUse 없는 경우 false
                dbUse = jObject.ContainsKey("dbUse") ? bool.Parse(jObject["dbUse"].ToString()) : false;

                log.Info("json → 셋팅정보(Class) Load 완료");

                // 인증 정보가 없는 경우(Settings.Settings 또한 입력)
                if (string.IsNullOrWhiteSpace(login_id) && string.IsNullOrWhiteSpace(login_password))
                {
                    // 로그인 정보가 없으므로 login_check 는 false
                    login_check = false;
                    SaveJsonFile();
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("json → 셋팅정보(Class) Load 실패: " + ex.Message);
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

                log.Info("셋팅정보(Class) → json Save 시작");

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

                log.Info("셋팅정보(Class) → json Save 완료");
            }
            catch (Exception ex)
            {
                log.Error("셋팅정보(Class) → json Save 실패: " + ex.Message);
                return false;
            }
            return true;
        }

        // Settings.Settings (user.config 파일)에 정보 저장
        public static bool SaveConfigFile()
        {
            log.Info("셋팅정보(Class) → 셋팅(user.config) 입력 시작");

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

                log.Info("셋팅정보(Class) → 셋팅(user.config) 정보 입력 완료");
            }
            catch (Exception ex)
            {
                log.Error("셋팅정보(Class) → 셋팅(user.config) 정보 입력 실패: " + ex.Message);
                return false;
            }
            return true;
        }

        // Settings.Settings (user.config 파일)에 정보 저장
        public static bool LoadConfigFile()
        {
            log.Info("셋팅(user.config) → 셋팅정보(Class) Save 시작");

            try
            {
                login_id = Properties.Settings.Default.login_id;
                login_password = Properties.Settings.Default.login_password;
                corp_name = Properties.Settings.Default.corp_name;           
                corp_id = Properties.Settings.Default.corp_id;             
                login_check = Properties.Settings.Default.login_check;         
                mercName = Properties.Settings.Default.mercName;            
                SerialName = Properties.Settings.Default.SerialName;          
                processType = Properties.Settings.Default.processType;         
                hour = Properties.Settings.Default.hour;
                minute = Properties.Settings.Default.minute;
                url = Properties.Settings.Default.url;
                saveDate = Properties.Settings.Default.saveDate;
                pplanKey = Properties.Settings.Default.pplanKey;
                count = Properties.Settings.Default.count;
                UserID = Properties.Settings.Default.UserID;
                UserName = Properties.Settings.Default.UserName;
                signalR = Properties.Settings.Default.signalR;
                serverIndex = Properties.Settings.Default.serverIndex;
                pworkkey = Properties.Settings.Default.pworkkey;
                consumersumcount = Properties.Settings.Default.consumersumcount;
                consumpWorkKey = Properties.Settings.Default.consumpWorkKey;
                consumersumerrorcount = Properties.Settings.Default.consumersumerrorcount;
                TypeIndex = Properties.Settings.Default.TypeIndex;
                CountType = Properties.Settings.Default.CountType;
                settingitem = Properties.Settings.Default.settingitem;
                meterialinfo = Properties.Settings.Default.meterialinfo;
                setting_check = Properties.Settings.Default.setting_check;
                baudrate = Properties.Settings.Default.baudrate;
                SensorRate = Properties.Settings.Default.SensorRate;
                RadioChecked = Properties.Settings.Default.RadioChecked;
                printItem = Properties.Settings.Default.printItem;
                samework = Properties.Settings.Default.samework;
                color_check = Properties.Settings.Default.color_check;
                count_check = Properties.Settings.Default.count_check;
                boxCheck = Properties.Settings.Default.boxCheck;
                searchCheck = Properties.Settings.Default.searchCheck;
                keyinCheck = Properties.Settings.Default.keyinCheck;
                detail_check = Properties.Settings.Default.detail_check;
                option1 = Properties.Settings.Default.option1;
                presultcount = Properties.Settings.Default.presultcount;
                twoSum = Properties.Settings.Default.twoSum;
                accountCheck = Properties.Settings.Default.accountCheck;
                productionCheck = Properties.Settings.Default.productionCheck;
                scrollvarCheck = Properties.Settings.Default.scrollvarCheck;
                index = Properties.Settings.Default.index;
                error = Properties.Settings.Default.error;
                error1 = Properties.Settings.Default.error1;
                Plc = Properties.Settings.Default.Plc;
                ChangeCheck = Properties.Settings.Default.ChangeCheck;
                ip = Properties.Settings.Default.ip;
                port = Properties.Settings.Default.port;
                Gap = Properties.Settings.Default.Gap;
                mimi = Properties.Settings.Default.mimi;
                jang = Properties.Settings.Default.jang;
                pok = Properties.Settings.Default.pok;
                go = Properties.Settings.Default.go;
                setCount = Properties.Settings.Default.setCount;
                boxType = Properties.Settings.Default.boxType;
                plcCheck = Properties.Settings.Default.plcCheck;
                completeValue_checked = Properties.Settings.Default.completeValue_checked;
                checkMerc = Properties.Settings.Default.checkMerc;
                robotchecked = Properties.Settings.Default.robotchecked;
                robotIP = Properties.Settings.Default.robotIP;
                robotPort = Properties.Settings.Default.robotPort;
                manager = Properties.Settings.Default.manager;
                authKey = Properties.Settings.Default.authKey;
                SerialName1 = Properties.Settings.Default.SerialName1;
                consumCount = Properties.Settings.Default.consumCount;
                dbUse = Properties.Settings.Default.dbUse;

                log.Info("셋팅(user.config) → 셋팅정보(Class) Save 완료");
            }
            catch (Exception ex)
            {
                log.Error("셋팅(user.config) → 셋팅정보(Class) Save 실패: " + ex.Message);
                return false;
            }
            return true;
        }
    }
}