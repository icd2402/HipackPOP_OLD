using Newtonsoft.Json.Linq;
using PopProject.Input;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PopProject.Detail
{
    /// <summary>
    /// Win_Detail.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Win_Detail : Window
    {
        Ls_Fenet wd = new Ls_Fenet();
        private ObservableCollection<string> uriPathOB = new ObservableCollection<string>();
        private readonly MainModel mainModel;
        private int clickCount = 0;
        private readonly string metrialitem = Properties.Settings.Default.meterialinfo;
        public DetailModel Detailitem;
        private readonly ObservableCollection<MainModel> workList;
        private readonly bool monitorcheck = false;
        public Socket mainSock;
        public Win_Detail(MainModel mainModel, string special, ObservableCollection<MainModel> workList, bool v)
        {
            InitializeComponent();
            this.mainModel = mainModel;
            monitorcheck = v;
            this.workList = workList;
            customSname.Text = mainModel.customSName;
            boxType.Text = "형  태 : " + mainModel.boxType;
            pplanCount.Text = "계획량 : " + mainModel.pplanCount.ToString("N0");
            prodName.Text = mainModel.prodName;
            subOb.ItemsSource = mainModel.subOb;
            tb공정별.Text = special.Replace("|", "\n");
            pSize.Text = mainModel.pSize;
            Pass_detail.Visibility = Properties.Settings.Default.corp_name.Equals("오주포장") ? Visibility.Visible : Visibility.Collapsed;
            robot.Visibility = Properties.Settings.Default.robotchecked ? Visibility.Visible : Visibility.Collapsed;

            MetrialBj();
            MetrialIG();
            PopSearchPhoto();

            if (Properties.Settings.Default.robotchecked && Properties.Settings.Default.corp_name.Equals("오주포장"))
            {
                serchRobotData();
            }
        }

        private void serchRobotData()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
               Properties.Settings.Default.serverIndex,
               "[spSelIFrobotdata_info]",
               new string[] { "@prodID" },
               new object[] {
               mainModel.prodID
               }, 0
             );
            if (returnData.결과 == 1)
            {
                string BOX_W = "";
                string BOX_H = "";
                string BOX_D = "";
                string BOX_DADAN = "";
                string BOX_DANSU = "";
                string BOX_PAT = "";
                string PALLET_W = "";
                string PALLET_H = "";
                string PALLET_D = "";
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);

                foreach (JObject result in Data_Jarry)
                {
                    BOX_W = result["BOX_W"].ToString();
                    BOX_H = result["BOX_H"].ToString();
                    BOX_D = result["BOX_D"].ToString();
                    BOX_DADAN = result["BOX_DADAN"].ToString();
                    BOX_DANSU = result["BOX_DANSU"].ToString();
                    BOX_PAT = result["BOX_PAT"].ToString();
                    PALLET_W = result["PALLET_W"].ToString();
                    PALLET_H = result["PALLET_H"].ToString();
                    PALLET_D = result["PALLET_D"].ToString();
                    string UpDatetime = result["UpDatetime"].ToString();
                }

                if (Data_Jarry.Count > 0)
                {
                    DataSend(BOX_W, BOX_H, BOX_D, BOX_DADAN, BOX_DANSU, BOX_PAT, PALLET_W, PALLET_H, PALLET_D);
                }
                else
                {
                    Main.robot_Setting = new Robot_Setting(mainModel.prodID, mainModel.customSName, mainModel.prodName);
                    Main.robot_Setting.ShowDialog();
                    Main.robot_Setting = null;
                }
            }
        }
        public void DataReceived1(IAsyncResult ar) // 데이터 수신
        {
            try
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                AsyncObject obj = (AsyncObject)ar.AsyncState;
                // 수신한 데이터 16진수로 변환
                string Receive_Data = string.Empty;
                for (int i = 0; i < obj.Buffer.Length; i++)
                {
                    Receive_Data += obj.Buffer[i].ToString("x2");
                }
                //MessageBox.Show(Receive_Data.ToString()); //수신 데이터값 보기(기본)
                //연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64);
                //Console.WriteLine("수신데이터RD : " + RD.ToString());
                //앞에 데이터 지우고 데이터 정보부터 읽음

                for (int i = 0; i < RD.Length; i++)
                {
                    string high_data = RD.Substring(0, 2); // 앞 두자리 추출
                    string low_data = RD.Substring(2, 2); // 뒤 두자리 
                                                          //continuous_read += Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                    continuous_read += low_data + high_data; //앞 뒤 자리바꾸고 10진수 변환
                    RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                }

                ///MessageBox_Dialog msg = new MessageBox_Dialog(continuous_read.ToString());
                // msg.ShowDialog();//수신 데이터값 보기

                //개별읽기
                //string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                //string high_data = RD.Substring(0, 2); //앞 두자리 추출
                //string low_data = RD.Substring(2, 2); //뒤 두자리 추출
                //int single_read = Convert.ToInt32(low_data + high_data, 16); //앞 뒤 자리바꾸고 10진수 변환
                //MessageBox.Show(single_read.ToString()); //수신 데이터값 보기
                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.

                Console.WriteLine(continuous_read.Substring(0, 36).ToString());
                obj.ClearBuffer();

            }
            catch (Exception)
            {

            }
            // 수신 대기
            // obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 2000, 0, DataReceived, obj);
            mainSock.Close();
        }
        private Boolean ConnectSocket(string flag)
        {
            bool success = false;

            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            if (!mainSock.Connected)
            {
                int port = int.Parse(Properties.Settings.Default.robotPort);
                IAsyncResult result = mainSock.BeginConnect(Properties.Settings.Default.robotIP, port, null, null);
                //IAsyncResult result = mainSock.BeginConnect("192.168.7.7", port, null, null);

                success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (success)
                {
                    mainSock.EndConnect(result);
                    AsyncObject obj = new AsyncObject(150);
                    obj.WorkingSocket = mainSock;
                    obj.ClearBuffer();
                    switch (flag)
                    {
                        case "W":
                            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived1, obj);
                            break;
                    }

                }


            }

            return success;
        }
        private void DataSend(string bOX_W, string bOX_H, string bOX_D, string bOX_DADAN, string bOX_DANSU, string bOX_PAT, string pALLET_W, string pALLET_H, string pALLET_D)
        {
            MessageBox_Dialog msg;
            if (ConnectSocket("W"))
            {
                try
                {
                    List<int> items = new List<int>();
                    items.Add(int.Parse(bOX_W));//박스가로
                    items.Add(int.Parse(bOX_H));//박스세로 
                    items.Add(int.Parse(bOX_D));// 박스높이
                    items.Add(int.Parse(bOX_DADAN));// 다단
                    items.Add(int.Parse(bOX_DANSU));// 단수
                    items.Add(int.Parse(bOX_PAT));//패턴
                    items.Add(int.Parse(pALLET_W));//파렛트가로
                    items.Add(int.Parse(pALLET_H));//파렛트세로
                    items.Add(int.Parse(pALLET_D));//파렛트높이

                    int WriteCount = items.Count() * 2;
                    byte[] bDts = wd.continuous_Write("%DB1200", WriteCount, items);
                    // byte[] bDts = wd.Byte_continuous_Read("D", "312", 2);
                    string Send_Data = string.Empty;
                    //for (int i = 0; i < bDts.Length; i++)
                    //{
                    //    Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    //}
                    //Console.WriteLine("전송 데이터 : " + Send_Data);
                    mainSock.Send(bDts); //데이터 쓰기
                   // msg = new MessageBox_Dialog("전송되었습니다.");
                   // msg.Show();
                }
                catch (Exception)
                {
                    msg = new MessageBox_Dialog("재시도 해주세요.");

                    msg.Show();
                }
                finally
                {
                    mainSock.Close();



                }
                //if (WriteData6.Text.ToString().Equals("0"))
                //{
                //    Thread.Sleep(1000);
                //    if (ConnectSocket("P"))
                //    {
                //        byte[] bDts = wd.Byte_continuous_Read("D", "312", 2);
                //        string Send_Data = string.Empty;
                //        for (int i = 0; i < bDts.Length; i++)
                //        {
                //            Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                //        }
                //        Console.WriteLine(Send_Data);

                //        patSock.Send(bDts);
                //    }

                //}
            }
            else
            {
                mainSock.Close();
                msg = new MessageBox_Dialog("Plc 연결 불가");
                msg.ShowDialog();
            }

        }

        //인쇄사양 출력
        private void PopSearchPhoto()
        {

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
            Properties.Settings.Default.serverIndex,
            "[PopSearchPhoto]",
            new string[] { "@prodID" },
            new object[] { mainModel.prodID
            },
            0
            );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ObservableCollection<string> ob = new ObservableCollection<string>();
                    foreach (JObject result in Data_JArray)
                    {
                        string uri = Properties.Settings.Default.url + Properties.Settings.Default.corp_id + "/" + result["photoName"].ToString();
                        Console.WriteLine("imageuri : " + uri);
                        ob.Add(uri);
                    }
                    uriPathOB = ob;
                    if (uriPathOB.Count > 0)
                    {
                        tbNoCount.Visibility = Visibility.Collapsed;
                        tbPicCount.Visibility = Visibility.Visible;
                        picCount.Visibility = Visibility.Visible;
                        tbPicCount.Text = "1/" + uriPathOB.Count;
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(uriPathOB[0], UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.DecodePixelWidth = 2000;
                        bitmap.EndInit();
                        pic.Source = bitmap;
                    }

                    else
                    {
                        tbNoCount.Visibility = Visibility.Visible;
                        tbPicCount.Visibility = Visibility.Collapsed;
                        picCount.Visibility = Visibility.Collapsed;
                    }
                }
            }

            else
            {
                tbNoCount.Visibility = Visibility.Visible;
                tbPicCount.Visibility = Visibility.Collapsed;
                picCount.Visibility = Visibility.Collapsed;
                MessageBox_Dialog msg = new MessageBox_Dialog("PopSearchPhoto 조회 오류!!");
                _ = msg.ShowDialog();
            }
        }

        //원단 입고 내역 출력
        private void MetrialIG()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopIGSearch]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ObservableCollection<IgSmModel> ob = new ObservableCollection<IgSmModel>();
                    float wdCount = 0;
                    float scCount = 0;

                    foreach (JObject result in Data_JArray)
                    {
                        IgSmModel listitem = new IgSmModel
                        {
                            metDate = DateTime.Parse(result["metIGDate"].ToString()),
                            metType = int.Parse(result["metType"].ToString()),
                            metCount = float.Parse(result["metIGCount"].ToString()),
                            customSName = result["customSName"].ToString(),
                            metrialName = result["metrialName"].ToString(),
                            //metBJLast = DateTime.Parse(result["metBJLast"].ToString()),
                            fontColor = Brushes.ForestGreen,
                            wdjs = int.Parse(result["WdJS"].ToString()),
                            //metPreCount = float.Parse(result["metIGCount"].ToString()),

                        };

                        listitem.metCount *= listitem.wdjs;
                        listitem.metCountString = listitem.metCount.ToString();
                        listitem.metPreCount *= listitem.wdjs;
                        listitem.metPreCountString = listitem.metPreCount.ToString();
                        
                        if (listitem.metType == 0)
                        {
                            wdCount += listitem.metCount;
                        }

                        else
                        {
                            scCount += listitem.metCount;
                        }

                        ob.Add(listitem);
                        tbWdCount.Text = wdCount + "";
                        tbScCount.Text = scCount + "";
                    }
                    //if (ob.Count != 0)
                    //{
                    MetrialSM(ob);
                    //}
                    //else
                    //{
                    //    //PopBJSearch();
                    //}

                }

                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("PopIGSearch 오류 발생 !!");
                    _ = msg.ShowDialog();
                }
            }
        }

        private void PopBJSearch()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopBJSearch]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ObservableCollection<IgSmModel> ob = new ObservableCollection<IgSmModel>();
                    float wdCount = 0;
                    float scCount = 0;
                    IgSmModel listitem = new IgSmModel();
                    foreach (JObject result in Data_JArray)
                    {

                        listitem.customSName = result["customSName"].ToString();
                        listitem.paperGor = result["customSName"].ToString();
                        listitem.metCount = float.Parse(result["metBJCount"].ToString());
                        listitem.metDate = DateTime.Parse(result["metBJDate"].ToString());
                        // listitem.metBJLast = DateTime.Parse(result["metBJLast"].ToString());
                        // listitem.metPreCount = float.Parse(result["metPreCount"].ToString());
                        listitem.metrialName = result["metrialName"].ToString();
                        if (result["WdJS"] != null)
                        {
                            listitem.wdjs = int.Parse(result["WdJS"].ToString());
                        }
                        else
                        {
                            listitem.wdjs = 1;
                        }

                        listitem.metCount *= listitem.wdjs;
                        listitem.metPreCount *= listitem.wdjs;
                        listitem.fontColor = Brushes.ForestGreen;
                    }
                    listitem.metCountString = listitem.metCount.ToString();
                    if (listitem.metPreCount == 0)
                    {
                        // listitem.metPreCountString = "[가입고]";
                        listitem.metPreCountString = "0";
                    }
                    else
                    {
                        listitem.metPreCountString = "[" + listitem.metPreCount + "]";
                    }
                    ob.Add(listitem);
                    ic.ItemsSource = ob;
                }

                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("PopIGSearch 오류 발생 !!");
                    _ = msg.ShowDialog();
                }
            }
        }

        //원단 소모내역 조회
        private void MetrialSM(ObservableCollection<IgSmModel> ob)
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopSMSearch]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    float wdCount = 0;
                    float scCount = 0;
                    foreach (JObject result in Data_JArray)
                    {
                        IgSmModel listitem = new IgSmModel
                        {
                            metDate = DateTime.Parse(result["metSMDate"].ToString()),
                            metBJLast = DateTime.Parse(result["metSMDate"].ToString()),
                            metType = int.Parse(result["metType"].ToString()),
                            metCount = float.Parse(result["metSMCount"].ToString())
                        };
                        listitem.metCount = -listitem.metCount;
                        listitem.metCountString = "";
                        listitem.metPreCountString = listitem.metCount.ToString();
                        listitem.fontColor = Brushes.Red;
                        listitem.metrialName = result["metrialName"].ToString();
                        listitem.metCount = float.Parse(result["metSMCount"].ToString());
                        if (listitem.metType == 0)
                        {
                            wdCount += listitem.metCount;
                        }
                        else
                        {
                            scCount += listitem.metCount;
                        }
                        ob.Add(listitem);
                    }

                    tbWdCount.Text = (int.Parse(tbWdCount.Text) + wdCount).ToString();
                    tbWdCount.Foreground = float.Parse(tbWdCount.Text) >= 0 ? Brushes.ForestGreen : Brushes.Red;
                    tbScCount.Text = (float.Parse(tbScCount.Text) + scCount).ToString();
                    tbScCount.Foreground = float.Parse(tbScCount.Text) >= 0 ? Brushes.ForestGreen : Brushes.Red;
                    ic.ItemsSource = ob;

                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopSMSearch 조회 오류!!");
                _ = msg.ShowDialog();
            }
        }

        private string ExtractKorean(string beforestr)
        {

            //beforestr = "#ffffff";
            string str = Regex.Replace(beforestr, "#(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})", "");
            return str;
        }

        //원단 발주리스트 출력
        private void MetrialBj()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopWorkDetail]",
                new string[] { "@orderKey" },
                new object[] { mainModel.orderKey }, 0
                );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {

                    _ = new ObservableCollection<MainModel>();
                    foreach (JObject result in Data_JArray)
                    {
                        Detailitem = new DetailModel
                        {
                            metrialName = result["metrialName"].ToString(),
                            metType = int.Parse(result["metType"].ToString()),
                            mL = int.Parse(result["mL"].ToString()),
                            mW = int.Parse(result["mW"].ToString()),
                            orderKey = int.Parse(result["orderKey"].ToString()),
                            PrintNo = result["PrintNo"].ToString(),
                            Printposition = result["Printposition"].ToString(),
                            ColorType = ExtractKorean(result["ColorType"].ToString().Replace("|", "")),
                            CoatingType = result["CoatingType"].ToString(),
                            PushingNo = result["PushingNo"].ToString(),
                            Pushingposition = result["Pushingposition"].ToString(),
                            ConnetingType = result["ConnetingType"].ToString(),
                            PackCount = int.Parse(result["PackCount"].ToString()),
                            PackType = result["PackType"].ToString(),
                            loadposition = result["loadposition"].ToString(),
                            filmNo = result["filmNo"].ToString(),
                            handChk = result["handChk"].ToString(),
                            jobMemo = result["orderJobMemo"].ToString(),
                            WDSize = result["WDSize"] != null ? result["WDSize"].ToString() : "",
                            WDScore = result["WDScore"] != null ? result["WDScore"].ToString() : "",
                            DesignNO = result["DesignNO"] != null ? result["DesignNO"].ToString() : "",
                            HandPosition = result["HandPosition"] != null ? result["HandPosition"].ToString() : "",
                            LoadHeight = result["LoadHeight"] != null ? result["LoadHeight"].ToString() : "",
                            LoadType = result["LoadType"] != null ? result["LoadType"].ToString() : "",
                            PalletType = result["PalletType"] != null ? result["PalletType"].ToString() : "",
                            PalletSize = result["PalletSize"] != null ? result["PalletSize"].ToString() : ""
                        };
                        if (Properties.Settings.Default.corp_name.Equals("오주포장"))
                        {
                            if (Detailitem.PrintNo.Contains("무인쇄") || Detailitem.PrintNo.Contains("X"))
                            {
                                tbNoCount.Text = "무 인 쇄";

                                tbNoCount.FontSize = 32;

                            }
                        }
                        Detailitem.ColorType = Detailitem.ColorType.Replace("|", "");
                        Detailitem.SpecialMemo = result["SpecialMemo"].ToString();

                        if (result["prodMemo"] != null)
                        {
                            tb공정별.Text = tb공정별.Text.ToString() + "\n" + "메  모 : " + result["prodMemo"].ToString();
                        }

                        int count = 17;

                        if (Detailitem.PalletSize.Equals(""))
                        {
                            //파렛트규격.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb파렛트규격.Text = Detailitem.PalletSize;
                        }

                        if (Detailitem.PalletType.Equals(""))
                        {
                            // 파렛트종류.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb파렛트종류.Text = Detailitem.PalletType;
                        }

                        if (Detailitem.LoadType.Equals(""))
                        {
                            //적재방법.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb적재방법.Text = Detailitem.LoadType;
                            switch (Detailitem.LoadType.Trim())
                            {
                                case "1종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/1종구.png", UriKind.Relative));
                                    break;
                                case "2종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/2종구.png", UriKind.Relative));
                                    break;
                                case "3종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/3종구.png", UriKind.Relative));
                                    break;
                                case "4종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/4종구.png", UriKind.Relative));
                                    break;
                                case "5종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/5종구.png", UriKind.Relative));
                                    break;
                                case "6종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/6종구.png", UriKind.Relative));
                                    break;
                                case "7종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/7종구.png", UriKind.Relative));
                                    break;
                                case "8종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/8종구.png", UriKind.Relative));
                                    break;
                                case "9종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/9종구.png", UriKind.Relative));
                                    break;
                                case "11자4종구":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/11자4종구.png", UriKind.Relative));
                                    break;
                                case "8종구2":
                                    jonggu.Source = new BitmapImage(new Uri(@"/img/8종구2.png", UriKind.Relative));
                                    break;
                                default:
                                    jonggu.Visibility = Visibility.Collapsed;
                                    break;
                            }
                        }

                        if (Detailitem.LoadHeight.Equals(""))
                        {
                            //적재단수.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb적재단수.Text = Detailitem.LoadHeight;
                        }

                        if (Detailitem.HandPosition.Equals(""))
                        {
                            //손잡이위치.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb손잡이위치.Text = Detailitem.HandPosition;
                        }

                        if (Detailitem.DesignNO.Equals(""))
                        {
                            //디자인NO.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb디자인NO.Text = Detailitem.DesignNO;
                        }


                        if (Detailitem.PrintNo.Equals(""))
                        {
                            //조판번호.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb조판번호.Text = Detailitem.PrintNo;
                        }

                        if (Detailitem.Printposition.Equals(""))
                        {
                            //조판위치.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb조판위치.Text = Detailitem.Printposition;
                        }

                        if (Detailitem.ColorType.Equals(""))
                        {
                            //인쇄색도.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb인쇄색도.Text = Detailitem.ColorType;
                        }

                        if (Detailitem.filmNo.Equals(""))
                        {
                            // 필름번호.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }


                        else
                        {
                            tb필름번호.Text = Detailitem.filmNo;
                        }


                        if (Detailitem.CoatingType.Equals(""))
                        {
                            코팅.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb코팅.Text = Detailitem.CoatingType;
                        }

                        if (Detailitem.PushingNo.Equals(""))
                        {
                            목형번호.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb목형번호.Text = Detailitem.PushingNo;
                        }

                        if (Detailitem.Pushingposition.Equals(""))
                        {
                            목형위치.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb목형위치.Text = Detailitem.Pushingposition;
                        }

                        if (Detailitem.handChk.Equals(""))
                        {
                            손잡이.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb손잡이.Text = Detailitem.handChk;
                        }

                        if (Detailitem.ConnetingType.Equals(""))
                        {
                            접합방법.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb접합방법.Text = Detailitem.ConnetingType;
                        }

                        if (Detailitem.PackCount < 0)
                        {
                            밴딩매수.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb밴딩매수.Text = Detailitem.PackCount + "";
                        }

                        if (Detailitem.PackType.Equals(""))
                        {
                            밴딩방법.Visibility = Visibility.Collapsed;
                            count -= 1;
                        }

                        else
                        {
                            tb밴딩방법.Text = Detailitem.PackType;
                        }

                        tb작업지시.Text = Detailitem.jobMemo;
                        tb특별지시.Text = Detailitem.SpecialMemo;

                        if (Detailitem.metType == 1)
                        {
                            txtSc.Text = Detailitem.metrialName;
                            txtScSize.Text = Detailitem.mL + " * " + Detailitem.mW;
                        }

                        else
                        {
                            canSC.Visibility = Visibility.Collapsed;
                            txtBh.Text = Detailitem.metrialName + "    (" + mainModel.paperGor + ")";
                            txtBhSize.Text = Detailitem.mL + " * " + Detailitem.mW;
                            //Detailitem.WDScore = "123*456*789";
                            string[] split = Detailitem.WDScore.Split('*');
                            if (split.Length > 0 && split.Length != 1)
                            {
                                if (!split[0].Equals(split[2]))
                                {
                                    WDScore.Foreground = Brushes.Red;
                                }
                            }
                            else
                            {
                                split = Detailitem.WDScore.Split('+');
                                if (split.Length == 4)
                                {
                                    WDScore.Foreground = Brushes.Red;
                                }
                            }

                            WDScore.Text = Detailitem.WDScore.Replace("*", " * ");
                            WDSize.Text = Detailitem.WDSize.Replace("*", " * ");
                        }

                        if (count == 0)
                        {
                            imgLine.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("metrialBj 오류확인!!");
                _ = msg.ShowDialog();
            }
        }




        // 상세 화면 종료
        private void Detail_close(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Main.passCheck = true;
                pic.Source = null;
                Close();
            }
            catch(Exception e1)
            {
                LogWrite(e1.Message);
                LogWrite(e1.StackTrace);
            }
            finally
            {
                this.Close();
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
        private void Detail_List(object sender, MouseButtonEventArgs e)
        {
            can.Visibility = Visibility.Visible;
            Win_WorkList workList = new Win_WorkList(mainModel);
            _ = workList.ShowDialog();
            can.Visibility = Visibility.Collapsed;
        }

        // 이미지 확대 축소 팝업
        private void Image_Detail(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {

                can.Visibility = Visibility.Visible;
                Image img = (Image)sender;
                Win_PictureView win_Picture = new Win_PictureView(img.Source);
                _ = win_Picture.ShowDialog();
                can.Visibility = Visibility.Collapsed;

            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //제품명세서 출력 요청 
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업") || Properties.Settings.Default.corp_name.Equals("누리팩"))
            {
                SwPrintMessage swPrintMessage = new SwPrintMessage(mainModel, Detailitem);
                swPrintMessage.ShowDialog();
            }

            else
            {
                PrintMessage printMessage = new PrintMessage(mainModel, Detailitem);
                _ = printMessage.ShowDialog();
            }

        }

        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Close();
            switch (metrialitem)
            {
                case "12":
                case "13":
                    Win_input input = new Win_input(mainModel, workList, true);
                    Main1.win_Input = input;
                    _ = input.ShowDialog();
                    Main1.win_Input = null;
                    if (Properties.Settings.Default.presultcount != 0)
                    {
                        mainModel.presultCount = Properties.Settings.Default.presultcount;
                    }
                    Properties.Settings.Default.presultcount = 0;
                    Properties.Settings.Default.Save();
                    break;

                default:
                    input = new Win_input(mainModel, workList, true);
                    Main.win_Input = input;
                    _ = input.ShowDialog();

                    Main.win_Input = null;
                    if (Properties.Settings.Default.presultcount != 0)
                    {
                        mainModel.presultCount = Properties.Settings.Default.presultcount;
                    }
                    Properties.Settings.Default.presultcount = 0;
                    Properties.Settings.Default.Save();
                    break;
            }
            pic.Source = null;
        }

        // 인쇄사양 다음 이미지 
        private void Image_next(object sender, MouseButtonEventArgs e)
        {

            if (clickCount + 1 > uriPathOB.Count - 1)
            {
                clickCount = 0;
            }

            else
            {
                clickCount += 1;
            }
            tbPicCount.Text = clickCount + 1 + "/" + uriPathOB.Count;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uriPathOB[clickCount], UriKind.Absolute);
            bitmap.EndInit();
            pic.Source = bitmap;
        }

        private void Sv_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void Detail_Loaded(object sender, RoutedEventArgs e)
        {
            if (monitorcheck)
            {
                if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
                {
                    System.Drawing.Rectangle rec;
                    if (Properties.Settings.Default.mercName.Equals("인라인3호기") || Properties.Settings.Default.mercName.Equals("인라인2호기"))
                    {
                        rec = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
                    }
                    else
                    {
                        rec = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;
                    }

                    Left = rec.Left + 40;
                    Top = rec.Top + 40;
                    WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    WindowState = WindowState.Normal;
                }

                else
                {
                    WindowState = WindowState.Normal;
                }
            }

        }

        private void Pass_detailInfo(object sender, MouseButtonEventArgs e)
        {
            Main.passCheck = false;
            InvokeCount(Properties.Settings.Default.mercName, mainModel.orderKey.ToString() + "^@");

        }

        private void InvokeCount(string mercName, string v)
        {

            //Console.WriteLine("ServiceModel.DB_Handle_Model.접속정보[0]" + ServiceModel.접속정보[0]);
            SignalRAdministrator.허브.Invoke("invokeCount", ServiceModel.접속정보[0], mercName, v);


        }

        private void Detail_closed(object sender, EventArgs e)
        {
            Main.passCheck = true;
            pic.Source = null;
        }

        private void robot_setting(object sender, MouseButtonEventArgs e)
        {
            Main.robot_Setting = new Robot_Setting(mainModel.prodID, mainModel.customSName, mainModel.prodName);
            Main.robot_Setting.ShowDialog();
            Main.robot_Setting = null;
        }
    }
}
