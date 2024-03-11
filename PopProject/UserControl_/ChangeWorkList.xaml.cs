using PopProject.Input;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PopProject.UserControl_
{
    /// <summary>
    /// ChangeWorkList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChangeWorkList : Window
    {
        public static ObservableCollection<MainModel> workList;
        public static int index;
        public static bool indexCheck = false;
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;

        private DateTime selectedDate = new DateTime();

        public ChangeWorkList(ObservableCollection<MainModel> list, DateTime _selectedDate)
        {
            InitializeComponent();

            selectedDate = _selectedDate;
            string merName = SettingJsonManager.mercName;
            tbMercName.Text = merName;
            workList = new ObservableCollection<MainModel>();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].WDIGCount1 = list[i].WDIGCount;
                if (list[i].givWD.Equals("2"))
                {
                    list[i].WDIGCount1 =  string.Format("{0:n0}", list[i].WDBJCount);
                }
                string[] setting = SettingJsonManager.settingitem.Split(new string[] { "|" }, StringSplitOptions.None);
                for (int j = 0; j < setting.Length; j++)
                {
                    string item = "";
                    switch (setting[j])
                    {
                        case "1":
                            item = list[i].filmNo;
                            break;
                        case "2":
                            item = list[i].CoatingType;
                            break;
                        case "3":
                            item = list[i].PrintNo;
                            break;
                        case "4":
                            item = list[i].Printposition;
                            break;
                        case "5":
                            item = list[i].PushingNo;
                            break;
                        case "6":
                            item = list[i].Pushingposition;
                            break;
                        case "7":
                            item = SettingJsonManager.color_check ? list[i].ColorType.Replace("|", "").Replace("색", "색 ") : ExtractKorean(list[i].ColorType.Replace("|", "")).Replace("색", "색 ");
                            break;
                        case "8":
                            item = list[i].ConnetingType;
                            break;
                        case "9":
                            item = list[i].handChk;
                            break;
                        case "10":
                            item = list[i].PackCount;
                            break;
                        case "11":
                            item = list[i].PackType;
                            break;
                        case "12":
                            item = list[i].workMemo;
                            break;
                        case "13":
                            item = list[i].pplanMemo;
                            break;
                        case "14":
                            item = list[i].orderLast.Substring(0, 4).Equals("9999")
                            ? "-"
                            : "납기 : " + list[i].orderLast.Replace("[", "").Replace("]", "").Replace("-", "/");
                            break;
                        case "15":
                            item = list[i].PalletType;
                            break;
                        case "16":
                            item = list[i].SpecialMemo;
                            break;
                        default:
                            break;
                    }

                    if (j == 0)
                    {
                        if (item != null)
                            list[i].item5 = item.Replace("\r\n", "");
                    }
                    else if (j == 1)
                    {
                        if(item!=null)
                            list[i].item6 = item.Replace("\r\n", "");
                    }
                    else if (j == 2)
                    {
                        if(item != null)
                            list[i].item7 = item.Replace("\r\n", "");
                    }
                    else if (j == 3)
                    {
                        if(item != null)
                            list[i].item8 = item.Replace("\r\n", "");
                    }
                }
                workList.Add(list[i]);
            }




            Console.WriteLine("workList" + workList.Count);
            txtNoPlan.Visibility = workList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ic3.ItemsSource = workList;
            tbNo.Text = workList.Count + "";
            // selectWorkList(); // 생산 계획 리스트 조회
            // resultSearchDate();
        }
        private string ExtractKorean(string beforestr)
        {
            //beforestr = "#ffffff";
            string str = Regex.Replace(beforestr, "#(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})", "");
            return str;
        }
        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(icsv);
            hOff = icsv.HorizontalOffset;
            vOff = icsv.VerticalOffset;
            _ = icsv.CaptureMouse();
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (sender.GetType().Name.Equals("Canvas"))
                {
                    Canvas canvas = (Canvas)sender;
                    index = ic3.Items.IndexOf(canvas.DataContext);
                    Console.WriteLine("i = > " + index);

                    if (workList[index].check)
                    {
                        workList[index].check = false;
                        workList[index].icback = "White";
                        indexCheck = false;
                    }

                    else
                    {
                        indexCheck = true;
                        workList[index].check = true;
                        workList[index].icback = "#6799FF";
                    }

                    for (int j = 0; j < workList.Count; j++)
                    {
                        if (j != index)
                        {
                            if (workList[j].icback.Equals("#6799FF"))
                            {
                                workList[j].check = false;
                                workList[j].icback = "White";
                            }
                        }
                    }
                }
            }
        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            icsv.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (icsv.IsMouseCaptured)
            {
                icsv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(icsv).X));
                icsv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(icsv).Y));
            }
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            // this.DragMove();
        }

        private void Up_mouse(object sender, MouseButtonEventArgs e)
        {
            if (indexCheck)
            {
                if (index > 0)
                {
                    workList.Move(index, index - 1);
                    index--;
                    icsv.ScrollToVerticalOffset((index * 77.8) - 270);
                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("항목을 선택 하세요");
                _ = msg.ShowDialog();
            }
        }

        private void Down_mouse(object sender, MouseButtonEventArgs e)
        {
            if (indexCheck)
            {
                if (index < workList.Count - 1)
                {
                    workList.Move(index, index + 1);
                    index++;
                    icsv.ScrollToVerticalOffset((index * 77.8) - 270);
                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("항목을 선택 하세요");
                _ = msg.ShowDialog();
            }
        }
        private void Complete_mouse(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("순서 변경 하시겠습니까 ?", "순서 변경",
                    MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {

            }

            else
            {
                ChangeWorkListFinished();
                Close();
            }
        }

        private void ChangeWorkListFinished()
        {
            string orderkey = "";
            string orderWorkNo = "";
            string customSName = "";

            for (int i = 0; i < workList.Count; i++)
            {
                if (SettingJsonManager.processType.Equals(workList[i].ProcessType))
                {
                    orderkey = orderkey + workList[i].orderKey + "|";
                    orderWorkNo = orderWorkNo + workList[i].orderWorkNo + "|";
                    customSName = customSName + workList[i].customSName + "|";
                }
            }

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            _ = ServiceModel.GetData(

               SettingJsonManager.serverIndex,
               "[spUdt410processinfo_workSeq]",
               new string[] { "@공정구분", "@arr오더키", "@arr생산계획번호" },
               new string[] { SettingJsonManager.processType + "", orderkey, orderWorkNo },
               1
               );

        }

        private void Exit_mouse(object sender, MouseButtonEventArgs e)
        {
            Close();
            SettingJsonManager.ChangeCheck = true;
            SettingJsonManager.SaveJsonFile();
        }

        private void TopUp_mouse(object sender, MouseButtonEventArgs e)
        {
            if (indexCheck)
            {
                if (index > 0)
                {
                    workList.Move(index, 0);
                    index = 0;
                    icsv.ScrollToVerticalOffset(0);
                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("항목을 선택 하세요");
                _ = msg.ShowDialog();
            }
        }

        private void TopDown_mouse(object sender, MouseButtonEventArgs e)
        {
            if (indexCheck)
            {
                if (index < workList.Count - 1)
                {
                    workList.Move(index, workList.Count - 1);
                    index = workList.Count - 1;
                    icsv.ScrollToVerticalOffset(index * 77.8);
                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("항목을 선택 하세요");
                _ = msg.ShowDialog();
            }
        }

        private void Select_prod(object sender, MouseButtonEventArgs e)
        {
            SearchList searchList = new SearchList(workList, true, selectedDate);
            searchList.ShowDialog();
            foreach (MainModel item in workList)
            {
                item.check = false;
                item.icback = "White";
            }
            workList[SettingJsonManager.index].check = true;
            workList[SettingJsonManager.index].icback = "#6799FF";
            indexCheck = true;
            icsv.ScrollToVerticalOffset((SettingJsonManager.index * 77.8) - 171);
            index = SettingJsonManager.index;


        }
    }

    public class indexConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
