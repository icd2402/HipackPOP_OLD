using PopProject.Detail;
using PopProject.input;
using PopProject.KeyBoard;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PopProject.Input
{
    /// <summary>
    /// Plc_Pop.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Plc_Pop : Window
    {
        string[] psize; //규격 배열
        MainModel mainModel;
        public InputModel inputModel;
        Socket mainSock;
        Win_KeyBoard win_KeyBoard;
        private bool checkChangeButton = false;




        public Plc_Pop(MainModel mainModel)
        {
            InitializeComponent();
            this.mainModel = mainModel;
            SetPlcData();
        }

        //초기화면 세팅
        private void SetPlcData()
        {
            ip.Text = Properties.Settings.Default.ip + " / " + Properties.Settings.Default.port; //IP
            거래처.Text = mainModel.customSName; //거래처
            품명.Text = mainModel.prodName; //품명
            psize = mainModel.pSize.Split('*'); //규격
            장.Text = psize[0] == null ? "" : psize[0].Trim();
            폭.Text = psize[1] == null ? "" : psize[1].Trim();
            고.Text = psize[2] == null ? "" : psize[2].Trim();
            갭.Text = Properties.Settings.Default.Gap.ToString();
            작업지시.Text = Properties.Settings.Default.setCount.ToString();
            미미.Text = Properties.Settings.Default.mimi.ToString();

        }

        //PLC IP 세팅
        private void ip_input(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Ip_input ip_Input = new Ip_input();
            ip_Input.ShowDialog();
            ip.Text = Properties.Settings.Default.ip + " / " + Properties.Settings.Default.port;
        }

        private void PLC_Read(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.Plc) //PLC 사용 체크일때
            {
                try
                {
                    ConnectSocket(); //CNS 통신 연결
                    Ls_Fenet wd = new Ls_Fenet(); //헤더
                    byte[] bDts = wd.Word_continuous_Read("%DB7804", 71); //연속읽기 D3902 ~ D3972 (71개)

                    //송신한 데이터 값 보는용도
                    string Send_Data = string.Empty;
                    for (int i = 0; i < bDts.Length; i++)
                    {
                        Send_Data += bDts[i].ToString("x2"); //Byte값을 16진수로 변환
                    }
                    Console.WriteLine("PLC송신 데이터(Plc_Pop) : " + Send_Data);

                    mainSock.Send(bDts); //데이터 쓰기
                }
                catch (Exception)
                {
                }
            }
        }

        private void ConnectSocket()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            if (!mainSock.Connected)
            {
                int port = int.Parse(Properties.Settings.Default.port); //포트 2004
                try
                {
                    IAsyncResult result = mainSock.BeginConnect(Properties.Settings.Default.ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(1500, true);
                    if (success)//연결 성공
                    {
                        mainSock.EndConnect(result);
                        AsyncObject obj = new AsyncObject(1000);
                        obj.WorkingSocket = mainSock;
                        obj.ClearBuffer();
                        mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                    }
                    else //연결 실패
                    {
                        mainSock.Close();
                        //MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                        //msg.ShowDialog();
                    }
                }
                catch (Exception e)
                {
                    mainSock.Close();
                    //MessageBox_Dialog msg = new MessageBox_Dialog("CNS PLC 연결 실패");
                    //msg.ShowDialog();
                }
            }
        }

        public void DataReceived(IAsyncResult ar) // 데이터 수신
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 수신한 데이터 16진수로 변환
                string Receive_Data = string.Empty;
                for (int i = 0; i < obj.Buffer.Length; i++)
                {
                    Receive_Data += obj.Buffer[i].ToString("x2");
                }

                //Word 연속읽기
                string continuous_read = string.Empty;
                string RD = Receive_Data.Substring(64); //앞에 데이터 지우고 데이터 정보부터 읽음
                Console.WriteLine("PLC수신 데이터(Plc_Pop) : " + RD);

                string dummyData = "";

                this.Dispatcher.Invoke(() =>
                {
                    for (int j = 0; j < RD.Length; j++)
                    {
                        string high_data = RD.Substring(0, 2); //앞 두자리 추출
                        string low_data = RD.Substring(2, 2); //뒤 두자리 추출


                        switch (j)
                        {
                            case 0: //작업모델 번호
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 1: //작업지시 매수
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 2: //작업지시 매수
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 3: //박스타입
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 4: //폭
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 5: //장
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 6: //고
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 7: //갭
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 8: //앞날개
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 9: //미미
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 10: //저장 완료
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 11: //생산속도
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 12: //저장값 쓰기 가능상태
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 13: //spare
                                dummyData = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 14: //A
                                double result1 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb1.Text = result1.ToString("N1");
                                break;
                            case 15: //B
                                double result2 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb2.Text = result2.ToString("N1");
                                break;
                            case 16: //C
                                double result3 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb3.Text = result3.ToString("N1");
                                break;
                            case 17: //D
                                double result4 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb4.Text = result4.ToString("N1");
                                break;
                            case 18: //E
                                double result5 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb5.Text = result5.ToString("N1");
                                break;
                            case 19: //F1
                                double result6 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb6.Text = result6.ToString("N1");
                                break;
                            case 20: //F2
                                double result7 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb7.Text = result7.ToString("N1");
                                break;
                            case 21: //G
                                double result8 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb8.Text = result8.ToString("N1");
                                break;
                            case 22: //O
                                double result9 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb9.Text = result9.ToString("N1");
                                break;
                            case 23: //P1
                                double result10 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb10.Text = result10.ToString("N1");
                                break;
                            case 24: //P2
                                double result11 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb11.Text = result11.ToString("N1");
                                break;
                            case 25: //P3
                                double result12 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb12.Text = result12.ToString("N1");
                                break;
                            case 26: //P4
                                double result13 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb13.Text = result13.ToString("N1");
                                break;
                            case 27: //spare
                                double result14 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb14.Text = result14.ToString("N1");
                                break;
                            case 28: //슬로타
                                double result15 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb15.Text = result15.ToString("N1");
                                break;
                            case 29: //다이캇타
                                double result16 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb16.Text = result16.ToString("N1");
                                break;
                            case 30: //프린터1 풀 롤
                                double result17 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb17.Text = result17.ToString("N1");
                                break;
                            case 31: //프린터1 프레스 롤
                                double result18 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb18.Text = result18.ToString("N1");
                                break;
                            case 32: //프린터2 풀 롤
                                double result19 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb19.Text = result19.ToString("N1");
                                break;
                            case 33: //프린터2 프레스 롤
                                double result20 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb20.Text = result20.ToString("N1");
                                break;
                            case 34: //프린터3 풀 롤
                                double result21 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb21.Text = result21.ToString("N1");
                                break;
                            case 35: //프린터3 풀 롤
                                double result22 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb22.Text = result22.ToString("N1");
                                break;
                            case 36: //프린터4 풀 롤
                                double result23 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb23.Text = result23.ToString("N1");
                                break;
                            case 37: //프린터 프레스 롤
                                double result24 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb24.Text = result24.ToString("N1");
                                break;
                            case 38: //spare
                                double result25 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb25.Text = result25.ToString("N1");
                                break;
                            case 39: //spare
                                double result26 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb26.Text = result26.ToString("N1");
                                break;
                            case 40: //spare
                                double result27 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb27.Text = result27.ToString("N1");
                                break;
                            case 41: //spare
                                double result28 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb28.Text = result28.ToString("N1");
                                break;
                            case 42: //피다진입 갭
                                double result29 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb29.Text = result29.ToString("N1");
                                break;
                            case 43: //피타진입 롤
                                double result30 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb30.Text = result30.ToString("N1");
                                break;
                            case 44: //로타 보조쾌선 갭
                                double result31 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb31.Text = result31.ToString("N1");
                                break;
                            case 45: //슬로타 본(중간) 갭
                                double result32 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb32.Text = result32.ToString("N1");
                                break;
                            case 46: //슬로타 연결(배출) 갭
                                double result33 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb33.Text = result33.ToString("N1");
                                break;
                            case 47: //다이캇타 롤 갭
                                double result34 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb34.Text = result34.ToString("N1");
                                break;
                            case 48: //글루어벨트 진앱 갭 F1
                                double result35 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb35.Text = result35.ToString("N1");
                                break;
                            case 49: //글루어벨트 진앱 갭 F2
                                double result36 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb36.Text = result36.ToString("N1");
                                break;
                            case 50: //글루어 프레스롤 갭 A
                                double result37 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb37.Text = result37.ToString("N1");
                                break;
                            case 51: //글루어 프레스롤 갭 B
                                double result38 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb38.Text = result38.ToString("N1");
                                break;
                            case 52: //다이캇타 롤 갭A
                                double result39 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb39.Text = result39.ToString("N1");
                                break;
                            case 53: //다이캇타 롤 갭B
                                double result40 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb40.Text = result40.ToString("N1");
                                break;
                            case 54: //글루어 PE 가이드
                                double result41 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb41.Text = result41.ToString("N1");
                                break;
                            case 55: //R/B 보조괘선 진입 롤
                                double result42 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb42.Text = result42.ToString("N1");
                                break;
                            case 56: //R/B 본괘선 중간 롤
                                double result43 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb43.Text = result43.ToString("N1");
                                break;
                            case 57: //R/B 연결 배출 롤
                                double result44 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb44.Text = result44.ToString("N1");
                                break;
                            case 58: //R/B 연결 중간 롤
                                double result45 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb45.Text = result45.ToString("N1");
                                break;
                            case 59: //R/B 연결 중간 롤
                                double result46 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb46.Text = result46.ToString("N1");
                                break;
                            case 60: //R/B 연결 중간 롤
                                double result47 = Convert.ToInt32(low_data + high_data, 16) / 10.0;
                                tb47.Text = result47.ToString("N1");
                                break;


                            default:
                                break;
                        }
                        continuous_read += Convert.ToInt32(low_data + high_data, 16) + "/"; //앞 뒤 자리바꾸고 10진수 변환
                        RD = RD.Substring(4); //변환 끝낸 4자리 데이터 자르기
                    }
                });

                Console.WriteLine("CNS PLC수신 데이터 확인(Plc_Pop) : " + continuous_read); //수신값 확인용
            }
            catch (Exception)
            {
                //MessageBox_Dialog messageBox = new MessageBox_Dialog("레시피 읽기 실패");
                //_ = messageBox.ShowDialog();
                mainSock.Close();
            }
            finally
            {
                mainSock.Close();
            }

        }



        //저장 버튼
        private void Done(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //박스타입 선택
            int a = combo.SelectedIndex;
            int boxType = 1;
            switch (a)
            {
                case 0:
                    boxType = 1; //1합
                    break;
                case 1:
                    boxType = 3; //2합
                    break;
                case 2:
                    boxType = 5; //인쇄물
                    break;
                case 3:
                    boxType = 6; //H형
                    break;
                case 4:
                    boxType = 7; //H-Cover형
                    break;
            }

            Properties.Settings.Default.Gap = int.Parse(갭.Text.ToString());
            Properties.Settings.Default.mimi = int.Parse(미미.Text.ToString());
            Properties.Settings.Default.jang = int.Parse(장.Text.ToString());
            Properties.Settings.Default.pok = int.Parse(폭.Text.ToString());
            Properties.Settings.Default.go = int.Parse(고.Text.ToString());
            Properties.Settings.Default.setCount = int.Parse(작업지시.Text.ToString());
            Properties.Settings.Default.boxType = int.Parse(boxType.ToString());
            Properties.Settings.Default.plcCheck = true;
            Properties.Settings.Default.Save();
            Close();
        }

        //값 넣기 이벤트
        private void Input_value(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 10,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }

        //PLC세팅 단축키 F1
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "F1":
                    Ip_input ip_Input = new Ip_input();
                    ip_Input.ShowDialog();
                    ip.Text = Properties.Settings.Default.ip + " / " + Properties.Settings.Default.port;
                    break;
                default:
                    break;
            }
        }

        //확장창 값 넣기 이벤트
        private void Input_value2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("TextBox"))
            {
                TextBox context = (TextBox)sender;
                win_KeyBoard = new Win_KeyBoard(context)
                {
                    Left = SystemParameters.WorkArea.Width - Width - 10,
                    Top = (SystemParameters.WorkArea.Height / 2) - Top
                };
                _ = win_KeyBoard.Opacity = 1;
                _ = win_KeyBoard.ShowDialog();
                win_KeyBoard = null;
            }
        }

        //화장 창 버튼
        private void ChangeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkChangeButton)
            {
                changeButtonIn.Visibility = Visibility.Collapsed;
                changeButtonOut.Visibility = Visibility.Visible;
                checkChangeButton = false;
                Width = 320;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
            else
            {
                changeButtonIn.Visibility = Visibility.Visible;
                changeButtonOut.Visibility = Visibility.Collapsed;
                checkChangeButton = true;
                Width = 1105;
                Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        //닫기 버튼
        private void exit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Properties.Settings.Default.plcCheck = false;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
