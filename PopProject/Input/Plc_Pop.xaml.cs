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
                                tb1.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 15: //B
                                tb2.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 16: //C
                                tb3.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 17: //D
                                tb4.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 18: //E
                                tb5.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 19: //F1
                                tb6.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 20: //F2
                                tb7.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 21: //G
                                tb8.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 22: //O
                                tb9.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 23: //P1
                                tb10.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 24: //P2
                                tb11.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 25: //P3
                                tb12.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 26: //P4
                                tb13.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 27: //spare
                                tb14.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 28: //슬로타
                                tb15.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 29: //다이캇타
                                tb16.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 30: //프린터1 풀 롤
                                tb17.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 31: //프린터1 프레스 롤
                                tb18.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 32: //프린터2 풀 롤
                                tb19.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 33: //프린터2 프레스 롤
                                tb20.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 34: //프린터3 풀 롤
                                tb21.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 35: //프린터3 풀 롤
                                tb22.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 36: //프린터4 풀 롤
                                tb23.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 37: //프린터 프레스 롤
                                tb24.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 38: //spare
                                tb25.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 39: //spare
                                tb26.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 40: //spare
                                tb27.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 41: //spare
                                tb28.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 42: //피다진입 갭
                                tb29.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 43: //피타진입 롤
                                tb30.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 44: //로타 보조쾌선 갭
                                tb31.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 45: //슬로타 본(중간) 갭
                                tb32.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 46: //슬로타 연결(배출) 갭
                                tb33.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 47: //다이캇타 롤 갭
                                tb34.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 48: //글루어벨트 진앱 갭 F1
                                tb35.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 49: //글루어벨트 진앱 갭 F2
                                tb36.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 50: //글루어 프레스롤 갭 A
                                tb37.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 51: //글루어 프레스롤 갭 B
                                tb38.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 52: //다이캇타 롤 갭A
                                tb39.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 53: //다이캇타 롤 갭B
                                tb40.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 54: //글루어 PE 가이드
                                tb41.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 55: //R/B 보조괘선 진입 롤
                                tb42.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 56: //R/B 본괘선 중간 롤
                                tb43.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 57: //R/B 연결 배출 롤
                                tb44.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
                                break;
                            case 58: //R/B 연결 중간 롤
                                tb45.Text = Convert.ToInt32(low_data + high_data, 16).ToString();
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
