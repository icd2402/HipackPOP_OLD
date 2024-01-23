using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Printing;
using System.Collections;
using PopProject.Util;

namespace Monitor.Util
{
    public partial class PrintWindow : Window
    {
        private ArrayList TempArray;
        private int Type;
        public int 가로세로;
        private bool 확대Type;
        public string 리포트Type;

        private int 수주서Type;

        public MainViewModel ViewModel = new MainViewModel();

        // 출력 다이알로그 정의
        public PrintDialog Prt_Dialog;
        // 출력 페이지 정의 
        private FixedDocument 가로FixedDocument;
        private FixedDocument 세로FixedDocument;

        // 스레드 정지 값 
        private int Invock_Value = 100;

        public delegate void PrintSendEventHanlder(int type);
        public event PrintSendEventHanlder PrintSendEvent;


        public PrintWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            DataContext = ViewModel;
        }

        public static readonly RoutedEvent PrintDataSaveEvent = EventManager.RegisterRoutedEvent("PrintDataSave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PrintWindow));

        public event RoutedEventHandler Tap
        {
            add
            {
                this.AddHandler(PrintDataSaveEvent, value);
            }

            remove
            {
                this.RemoveHandler(PrintDataSaveEvent, value);
            }
        }

        void OnTap(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void RaisePrintDataSaveEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PrintWindow.PrintDataSaveEvent);
            RaiseEvent(newEventArgs);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as System.Windows.Controls.Button;
            if (bt != null)
            {
                switch (bt.Name)
                {
                    case "출력":
                        {

                            // 프린트 새로운 객체 생성
                            Prt_Dialog = new PrintDialog
                            {
                                // 현재 프린터 대기열에 반영하여 출력물 전달 
                                PrintQueue = LocalPrintServer.GetDefaultPrintQueue()
                            };

                            // PC 기본프린터 설정 가져오기 
                            Prt_Dialog.PrintTicket = Prt_Dialog.PrintQueue.DefaultPrintTicket;

                            // 전체페이지 출력 
                            Prt_Dialog.PageRangeSelection = PageRangeSelection.AllPages;

                            // 사용자 지정 출력 설정 
                            Prt_Dialog.UserPageRangeEnabled = true;
                            Prt_Dialog.MinPage = 1;

                            Prt_Dialog.MaxPage = Convert.ToUInt16(PrintViewer.Document.DocumentPaginator.PageCount);
                            Prt_Dialog.PageRange = new PageRange(1, Convert.ToUInt16(Prt_Dialog.MaxPage));

                            // 부모 컨트롤 정의를 통한 프린트옵셜 상단 호출 
                            Application.Current.MainWindow = Window.GetWindow(this);

                            Prt_Dialog.PrintTicket.PageOrientation = 가로세로 == 0 ? PageOrientation.Landscape : (PageOrientation?)System.Printing.PageOrientation.Portrait;

                            try
                            {
                                // 옵션 설정 확인 
                                if (Prt_Dialog.ShowDialog().GetValueOrDefault())
                                {

                                    // 리포트 도큐먼트페이지 저장 
                                    DocumentPaginator Set_DocumentPaginator = PrintViewer.Document.DocumentPaginator;
                                    // 페이지 사이즈 재정의 
                                    Set_DocumentPaginator.PageSize = new Size();
                                    // 선택 페이지 인새 설정시 실행 
                                    // If Prt_Dialog.PageRangeSelection = PageRangeSelection.UserPages Then
                                    // Set_DocumentPaginator = New Cls_PageRangDocumentPaginator(PrintViewer.Document.DocumentPaginator, Prt_Dialog.PageRange)
                                    // End If
                                    // 프린트 출력
                                    Prt_Dialog.PrintDocument(Set_DocumentPaginator, "Report");
                                    
                                    RaisePrintDataSaveEvent();

                                    PrintSendEvent?.Invoke(수주서Type);
                                }
                            }
                            catch (Exception )
                            {

                            }

                            break;
                        }

                    case "방향":
                        {
                            if (가로세로 == 0)
                            {
                                가로세로 = 1;
                                PrintViewer.Document = null;

                                this.Dispatcher.Invoke(() =>
                                {
                                    PrintViewer.Document = 세로FixedDocument;
                                });
                            }
                            else
                            {
                                가로세로 = 0;
                                PrintViewer.Document = null;

                                this.Dispatcher.Invoke(() =>
                                {
                                    PrintViewer.Document = 가로FixedDocument;
                                });
                            }

                            break;
                        }

                    case "확대":
                        {
                            if (확대Type == false)
                            {
                                
                                확대Type = true;
                                PrintViewer.Zoom = 150;
                            
                            }

                            else
                            {
                            
                                확대Type = false;
                                PrintViewer.FitToHeight();
                            
                            }

                            break;
                        }

                    case "이미지":
                        {
                            Thread.Sleep(Invock_Value);

                            // 저장 다이알 로그 객체 생성
                            SaveFileDialog Set_SaveFileDialog = new SaveFileDialog();

                            try
                            {
                                if (Set_SaveFileDialog.ShowDialog() == true)
                                {

                                    // 리포트 페이지 수량 만큼 반복
                                    for (var i = 0; i <= PrintViewer.Document.DocumentPaginator.PageCount - 1; i++)
                                    {
                                        // 현재 리포트 페이지 정보 저장
                                        DocumentPage Set_DocumentPage = PrintViewer.Document.DocumentPaginator.GetPage(i);
                                        // 현재 리포트 페이지 이미지 형태 변환
                                        RenderTargetBitmap Set_BitMap = new RenderTargetBitmap(Convert.ToInt32(Set_DocumentPage.Size.Width), Convert.ToInt32(Set_DocumentPage.Size.Height), 96, 96, System.Windows.Media.PixelFormats.Default);
                                        // 현재 페이지 이미지 변환처리 
                                        Set_BitMap.Render(Set_DocumentPage.Visual);
                                        // Png이미지 객체 생성
                                        PngBitmapEncoder Set_BitMapEncoder = new PngBitmapEncoder();
                                        // 변환된 이미지 페이지 생성
                                        Set_BitMapEncoder.Frames.Add(BitmapFrame.Create(Set_BitMap));
                                        // 변환된 이미지 저장
                                        FileStream Set_FileStream = new FileStream(Set_SaveFileDialog.FileName + "_00" + i + ".Png", FileMode.Create, FileAccess.Write);
                                        Set_BitMapEncoder.Save(Set_FileStream);
                                        Set_FileStream.Close();
                                    }
                                }
                            }
                            catch (Exception )
                            {
                            }

                            break;
                        }

                    case "엑셀":
                        {
                            switch (Type)
                            {
                                case 1:
                                    {
                                        break;
                                    }

                                case 2:
                                    {
                                        break;
                                    }

                                case 3:
                                    {
                                        break;
                                    }
                            }

                            break;
                        }

                    case "종료":
                        {
                            this.Close();
                            break;
                        }

                    case "닫기":
                        {
                            this.Close();
                            break;
                        }

                    case "최소화":
                        {
                            this.WindowState = WindowState.Minimized;
                            break;
                        }
                }
            }
        }

        private void RefreshArray(ArrayList array)
        {
            TempArray = new ArrayList();

            for (var i = 1; i <= array.Count; i++)
            {
                string Column1 = string.Empty;
                string Column2 = string.Empty;

                if (array[i - 1].ToString().Split('|')[0].IndexOf("/") > 0)
                {
                    Column1 = array[i - 1].ToString().Split('|')[0].Split('/')[0] + "|";
                    Column2 = array[i - 1].ToString().Split('|')[0].Split('/')[1] + "|";
                }
                else
                {
                    Column1 = array[i - 1].ToString().Split('|')[0].Split('/')[0] + "|";
                }

                Column1 += array[i - 1].ToString().Split('|')[1] + "|";
                Column1 += array[i - 1].ToString().Split('|')[2] + "|";

                if (Column2 != "")
                {

                    Column2 += array[i - 1].ToString().Split('|')[1] + "|";
                    Column2 += array[i - 1].ToString().Split('|')[2] + "|";

                }

                if (array[i - 1].ToString().Split('|')[3].IndexOf("/") > 0)
                {
                    Column1 = array[i - 1].ToString().Split('|')[3].Split('/')[0] + "|";
                    Column2 = array[i - 1].ToString().Split('|')[3].Split('/')[1] + "|";
                }

                else
                {
                    Column1 += array[i - 1].ToString().Split('|')[3].Split('/')[0] + "|";
                }

                Column1 += array[i - 1].ToString().Split('|')[4] + "|";
                Column1 += array[i - 1].ToString().Split('|')[5];

                TempArray.Add(Column1);

                if (Column2 != "")
                {
                    Column2 += array[i - 1].ToString().Split('|')[4] + "|";
                    Column2 += array[i - 1].ToString().Split('|')[5];

                    TempArray.Add(Column2);
                }
            }
        }

        private void NewLineRefreshArray(ArrayList array)
        {
            TempArray = new ArrayList();

            for (var i = 1; i <= array.Count; i++)
            {
                string Column1 = string.Empty;
                string Column2 = string.Empty;
                //{"jj|j", "hhh"}
                //array[i - 1].ToString().Split('|')[0]
                if (array[i - 1].ToString().Split('|')[0].IndexOf(Environment.NewLine) > 0)
                {
                    Column1 = array[i - 1].ToString().Split('|')[0].Split('/')[0] + "|";
                    Column2 = array[i - 1].ToString().Split('|')[0].Split('/')[1] + "|";
                }
                else
                {
                    Column1 = array[i - 1].ToString().Split('|')[0].Split('/')[0] + "|";
                }

                Column1 += array[i - 1].ToString().Split('|')[1] + "|";
                Column1 += array[i - 1].ToString().Split('|')[2] + "|";

                if (Column2 != "")
                {
                    Column2 += array[i - 1].ToString().Split('|')[1] + "|";
                    Column2 += array[i - 1].ToString().Split('|')[2] + "|";
                }

                if (array[i - 1].ToString().Split('|')[3].IndexOf("/") > 0)
                {
                    Column1 = array[i - 1].ToString().Split('|')[3].Split('/')[0] + "|";
                    Column2 = array[i - 1].ToString().Split('|')[3].Split('/')[1] + "|";
                }
                else
                {
                    Column1 += array[i - 1].ToString().Split('|')[3].Split('/')[0] + "|";
                }

                Column1 += array[i - 1].ToString().Split('|')[4] + "|";
                Column1 += array[i - 1].ToString().Split('|')[5];

                TempArray.Add(Column1);

                if (Column2 != "")
                {
                    Column2 += array[i - 1].ToString().Split('|')[4] + "|";
                    Column2 += array[i - 1].ToString().Split('|')[5];

                    TempArray.Add(Column2);
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                PrintSendEvent?.Invoke(수주서Type);
                this.Close();
            }
        }

        private void Title_Panel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MainWindow2_Loaded(object sender, RoutedEventArgs e)
        {
            PrintViewer.FitToHeight();
        }

        public void DirectPrint(int count)
        {
            // 프린트 새로운 객체 생성
            Prt_Dialog = new PrintDialog();

            // 현재 프린터 대기열에 반영하여 출력물 전달 
            Prt_Dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();

            // PC 기본프린터 설정 가져오기 
            Prt_Dialog.PrintTicket = Prt_Dialog.PrintQueue.DefaultPrintTicket;
            Prt_Dialog.PrintTicket.CopyCount = count;

            // 전체페이지 출력 
            Prt_Dialog.PageRangeSelection = PageRangeSelection.AllPages;

            // 사용자 지정 출력 설정 
            Prt_Dialog.UserPageRangeEnabled = true;
            Prt_Dialog.MinPage = 1;
            Prt_Dialog.MaxPage = Convert.ToUInt16(PrintViewer.Document.DocumentPaginator.PageCount);
            Prt_Dialog.PageRange = new PageRange(1, Convert.ToUInt16(Prt_Dialog.MaxPage)); 
            
            if (가로세로 == 0)
            {
                Prt_Dialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            }
            else
            {
            }

            try
            {
                // 리포트 도큐먼트페이지 저장 
                DocumentPaginator Set_DocumentPaginator = PrintViewer.Document.DocumentPaginator;
                // 페이지 사이즈 재정의 
                Set_DocumentPaginator.PageSize = new Size();

                // 선택 페이지 인새 설정시 실행 
                // If Prt_Dialog.PageRangeSelection = PageRangeSelection.UserPages Then

                // Set_DocumentPaginator = New Cls_PageRangDocumentPaginator(PrintViewer.Document.DocumentPaginator, Prt_Dialog.PageRange)

                // End If

                LogManager.Debug("프린트 이름 : " + Prt_Dialog.PrintQueue.FullName, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                LogManager.Debug("프린트 범위 : " + Prt_Dialog.MinPage.ToString() + " ~ " + Prt_Dialog.MaxPage.ToString()
                                 + "\n출력수 : " + Prt_Dialog.PrintTicket.CopyCount.ToString()

                                 , System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                // 프린트 출력
                Prt_Dialog.PrintDocument(Set_DocumentPaginator, "Report");

                RaisePrintDataSaveEvent();

            }
            catch (Exception ex)
            {
                LogManager.Error("프린터 인쇄 오류1" + ex.StackTrace, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
    }

    public class MainViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        protected virtual void onPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public MainViewModel()
        {

            // 기본 3    1/2/ 3/ 4/ 5
            SliderValue = 3;
            // 0% 50% 100% 150% 200%
            Str_SliderValue = "100%";
        }

        // 확대/축소 현재 위치값 표현
        private string _Str_SliderValue;
        public string Str_SliderValue
        {
            get
            {
                return _Str_SliderValue;
            }
            set
            {
                _Str_SliderValue = value;
                onPropertyChanged("Str_SliderValue");
            }
        }

        // 확대/축소 값 
        private int _SliderValue;
        public int SliderValue
        {
            get
            {
                return _SliderValue;
            }
            set
            {
                _SliderValue = value;
                onPropertyChanged("SliderValue");
            }
        }
    }
}
