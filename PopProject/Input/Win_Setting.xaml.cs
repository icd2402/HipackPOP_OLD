using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace PopProject.Input
{

    public partial class Win_Setting : Window
    {
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;
        private Point scrollMousePoint1 = new Point();
        private double hOff1 = 1;
        private double vOff1 = 1;

        public Win_Setting()
        {
            InitializeComponent();
            PopErrorSearch();
            Win_Set();
            
        }

        private void Win_Set()
        {
            count_check.IsChecked = Properties.Settings.Default.count_check;
            twoSum.IsChecked = Properties.Settings.Default.twoSum;
            samework.IsChecked = Properties.Settings.Default.samework;
            option1.IsChecked = Properties.Settings.Default.option1;
            plc.IsChecked = Properties.Settings.Default.Plc;
            color_check.IsChecked = Properties.Settings.Default.color_check;
            Setting_mercName.Text = Properties.Settings.Default.mercName;
            completeValue.IsChecked = Properties.Settings.Default.completeValue_checked;
            robotcheck.IsChecked = Properties.Settings.Default.robotchecked;
            if (Properties.Settings.Default.RadioChecked)
            {
                radio2.IsChecked = true;
            }

            else
            {
                radio1.IsChecked = true;
            }

            scrollvar_Check.IsChecked = Properties.Settings.Default.scrollvarCheck;
            production_check.IsChecked = Properties.Settings.Default.productionCheck;
            account_check.IsChecked = Properties.Settings.Default.accountCheck;
            boxCheck.IsChecked = Properties.Settings.Default.boxCheck;
            keyinCheck.IsChecked = Properties.Settings.Default.keyinCheck;
            detail_check.IsChecked = Properties.Settings.Default.detail_check;


            string[] port = SerialPort.GetPortNames();
            cb_1.ItemsSource = port;
            cb_12.ItemsSource = port;
            cb_1.Text = Properties.Settings.Default.SerialName;
            cb_12.Text = Properties.Settings.Default.SerialName1;
            int[] baudrate = new int[1] { 9600 };
            cb_2.ItemsSource = baudrate;
            cb_2.Text = Properties.Settings.Default.baudrate.ToString();
            string[] SensorRate = new string[7] { "없음", "0.1초", "0.2초", "0.3초", "0.4초", "0.5초", "1초" };
            cb_3.ItemsSource = SensorRate;
            cb_3.Text = GetSenserRate(Properties.Settings.Default.SensorRate);

            string settingItem = Properties.Settings.Default.settingitem;
            string metrialinfo = Properties.Settings.Default.meterialinfo;
            string printItem = Properties.Settings.Default.printItem;

            string[] setting = settingItem.Split(new string[] { "|" }, StringSplitOptions.None);
            string[] metrial = metrialinfo.Split(new string[] { "|" }, StringSplitOptions.None);
            string[] print = printItem.Split(new string[] { "|" }, StringSplitOptions.None);

            for (int i = 0; i < print.Length; i++)
            {
                switch (print[i])
                {
                    case "1":
                        _cb1.IsChecked = true;
                        break;

                    case "2":
                        _cb2.IsChecked = true;
                        break;

                    case "3":
                        _cb3.IsChecked = true;
                        break;

                    case "4":
                        _cb4.IsChecked = true;
                        break;

                    case "5":
                        _cb5.IsChecked = true;
                        break;

                    case "6":
                        _cb6.IsChecked = true;
                        break;

                    case "7":
                        _cb7.IsChecked = true;
                        break;

                    case "8":
                        _cb8.IsChecked = true;
                        break;

                    case "9":
                        _cb9.IsChecked = true;
                        break;

                    case "10":
                        _cb10.IsChecked = true;
                        break;

                    case "11":
                        _cb11.IsChecked = true;
                        break;
                    case "18":
                        _cb18.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < metrial.Length; i++)
            {
                switch (metrial[i])
                {
                    case "12":
                        scinfo.IsChecked = true;
                        break;

                    case "13":
                        wdinfo.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < setting.Length; i++)
            {
                switch (setting[i])
                {
                    case "1":
                        cb1.IsChecked = true;
                        break;

                    case "2":
                        cb2.IsChecked = true;
                        break;

                    case "3":
                        cb3.IsChecked = true;
                        break;

                    case "4":
                        cb4.IsChecked = true;
                        break;
                    case "5":
                        cb5.IsChecked = true;
                        break;
                    case "6":
                        cb6.IsChecked = true;
                        break;
                    case "7":
                        cb7.IsChecked = true;
                        break;
                    case "8":
                        cb8.IsChecked = true;
                        break;
                    case "9":
                        cb9.IsChecked = true;
                        break;
                    case "10":
                        cb10.IsChecked = true;
                        break;
                    case "11":
                        cb11.IsChecked = true;
                        break;
                    case "14":
                        cb12.IsChecked = true;
                        break;
                    case "15":
                        cb13.IsChecked = true;
                        break;
                    case "17":
                        cb15.IsChecked = true;
                        break;
                    case "18":
                        cb18.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }

            if (Properties.Settings.Default.corp_name.Equals("오주포장")
                || Properties.Settings.Default.corp_name.Equals("대양포장"))
            {
                plcText.Visibility = Visibility.Visible;
                plc.Visibility = Visibility.Visible;
            }
            else
            {
                plcText.Visibility = Visibility.Collapsed;
                plc.Visibility = Visibility.Collapsed;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // 작업 정보 설정
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            List<string> list = new List<string>();
            int count = 0;

            foreach (CheckBox cbx in str.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    count++;
                    if (count < 4)
                    {
                        list.Add(cbx.Content.ToString());
                    }
                }

                else
                {
                    _ = list.Remove(cbx.Content.ToString());
                }

            }
            foreach (CheckBox cbx in str1.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    count++;
                    if (count < 4)
                    {
                        list.Add(cbx.Content.ToString());
                    }
                }

                else
                {
                    _ = list.Remove(cbx.Content.ToString());
                }
            }

            if (count > 3)
            {
                CheckBox chb = (CheckBox)sender;
                chb.IsChecked = false;
            }
            string settingValue = GetSettingValue(list);
            if (settingValue.Length != 0)
            {
                settingValue = settingValue.Substring(0, settingValue.Length - 1);
            }
            Console.WriteLine("체크함 ->" + count);
            Console.WriteLine("SettingValue : " + settingValue);
            Properties.Settings.Default.searchCheck = !settingValue.Equals(Properties.Settings.Default.settingitem);
            Properties.Settings.Default.settingitem = settingValue;
            Properties.Settings.Default.Save();
        }
        //자재정보 설정
        private void CheckBox_Checked1(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            foreach (CheckBox cbx in str2.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            string settingValue = GetSettingValue(list);
            if (settingValue.Length != 0)
            {
                settingValue = settingValue.Substring(0, settingValue.Length - 1);
                Console.WriteLine("SettingValue : " + settingValue);
            }
            else
            {
                settingValue = "";
            }
            Properties.Settings.Default.searchCheck = !settingValue.Equals(Properties.Settings.Default.meterialinfo);
            Properties.Settings.Default.meterialinfo = settingValue;
            Properties.Settings.Default.Save();
        }
        // 출력 정보 설정
        private void Setting_print(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            foreach (CheckBox cbx in str3.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            foreach (CheckBox cbx in str4.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            string printItem = GetPringItem(list);

            if (printItem.Length != 0)
            {
                printItem = printItem.Substring(0, printItem.Length - 1);
            }

            Properties.Settings.Default.printItem = printItem;
            Properties.Settings.Default.Save();
        }

        private string GetPringItem(List<string> list)
        {
            string settindValue = "";
            for (int i = 0; i < list.Count; i++)
            {
                int setting = 0;
                switch (list[i])
                {
                    case "필름 NO":
                        setting = 1;
                        break;

                    case "코팅":
                        setting = 2;
                        break;

                    case "조판 NO":
                        setting = 3;
                        break;

                    case "조판 위치":
                        setting = 4;
                        break;

                    case "목형 NO":
                        setting = 5;
                        break;

                    case "목형 위치":
                        setting = 6;
                        break;

                    case "인쇄 색도":
                        setting = 7;
                        break;

                    case "접합 방법":
                        setting = 8;
                        break;

                    case "손잡이":
                        setting = 9;
                        break;

                    case "포장 매수":
                        setting = 10;
                        break;
                    case "포장 방법":
                        setting = 11;
                        break;
                    case "파렛트 종류":
                        setting = 18;
                        break;
                    default:
                        break;
                }

                settindValue = settindValue + setting + "|";
                Console.WriteLine("printlist" + i + " :" + list[i].ToString());
            }
            return settindValue;
        }


        // 작업정보 설정
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            List<string> list = new List<string>();

            foreach (CheckBox cbx in str.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }

                else
                {
                    _ = list.Remove(cbx.Content.ToString());
                }
            }

            foreach (CheckBox cbx in str1.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
                else
                {
                    _ = list.Remove(cbx.Content.ToString());
                }
            }
            string settingValue = GetSettingValue(list);
            settingValue = settingValue.Length > 1 ? settingValue.Substring(0, settingValue.Length - 1) : "";
            Properties.Settings.Default.searchCheck = !settingValue.Equals(Properties.Settings.Default.settingitem);
            Properties.Settings.Default.settingitem = settingValue;
            Properties.Settings.Default.Save();
        }

        private string GetSettingValue(List<string> list)
        {
            string settindValue = "";
            for (int i = 0; i < list.Count; i++)
            {
                int setting = 0;
                switch (list[i])
                {
                    case "필름 NO":
                        setting = 1;
                        break;

                    case "코팅":
                        setting = 2;
                        break;

                    case "조판 NO":
                        setting = 3;
                        break;

                    case "조판 위치":
                        setting = 4;
                        break;

                    case "목형 NO":
                        setting = 5;
                        break;

                    case "목형 위치":
                        setting = 6;
                        break;

                    case "인쇄 색도":
                        setting = 7;
                        break;

                    case "접합 방법":
                        setting = 8;
                        break;

                    case "손잡이":
                        setting = 9;
                        break;

                    case "포장 매수":
                        setting = 10;
                        break;

                    case "포장 방법":
                        setting = 11;
                        break;

                    case "마닐라 정보":
                        setting = 12;
                        break;

                    case "원단 정보":
                        setting = 13;
                        break;
                    case "참고 사항":
                        setting = 14;
                        break;

                    case "공정별참고사항":
                        setting = 15;
                        break;
                    case "납기일":
                        setting = 17;
                        break;
                    case "파렛트 종류":
                        setting = 18;
                        break;
                    default:
                        break;
                }

                settindValue = settindValue + setting + "|";
                Console.WriteLine("list" + i + " :" + list[i].ToString());
            }
            return settindValue;

        }

        private void Setting_done(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private string GetSenserRate(string senser)
        {
            string str = "";
            switch (senser)
            {

                case "0":
                    str = "없음";
                    break;
                case "1":
                    str = "0.1초";
                    break;
                case "2":
                    str = "0.2초";
                    break;
                case "3":
                    str = "0.3초";
                    break;
                case "4":
                    str = "0.4초";
                    break;
                case "5": //
                    str = "0.5초";
                    break;
                case "9":
                    str = "1초";
                    break;
                default:
                    break;
            }

            return str;
        }


        private void Window_Closed(object sender, EventArgs e)
        {

            if (cb_1.SelectedItem != null)
            {
                Properties.Settings.Default.SerialName = cb_1.SelectedItem.ToString();
                Properties.Settings.Default.Save();

            }

            if (cb_12.SelectedItem != null)
            {
                Properties.Settings.Default.SerialName1 = cb_12.SelectedItem.ToString();
                Properties.Settings.Default.Save();

            }

            if (cb_2.SelectedItem != null)
            {
                Properties.Settings.Default.baudrate = int.Parse(cb_2.SelectedItem.ToString());
                Properties.Settings.Default.Save();
            }


            if (cb_3.SelectedItem != null)//
            {
                switch (cb_3.Text)
                {
                    case "없음":
                        Properties.Settings.Default.SensorRate = "0";
                        break;
                    case "0.1초":
                        Properties.Settings.Default.SensorRate = "1";
                        break;
                    case "0.2초":
                        Properties.Settings.Default.SensorRate = "2";
                        break;
                    case "0.3초":
                        Properties.Settings.Default.SensorRate = "3";
                        break;
                    case "0.4초":
                        Properties.Settings.Default.SensorRate = "4";
                        break;
                    case "0.5초":
                        Properties.Settings.Default.SensorRate = "5";
                        break;
                    case "1초":
                        Properties.Settings.Default.SensorRate = "9";
                        break;
                    default:
                        break;
                }

                Properties.Settings.Default.Save();

            }


        }

        private void Cb_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RadioChecked = !radio1.IsChecked.Value;
            Console.WriteLine("radio1.IsChecked.Value" + radio1.IsChecked.Value);
            Properties.Settings.Default.Save();
        }

        private void SameCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.samework = samework.IsChecked.Value;
            Properties.Settings.Default.searchCheck = Properties.Settings.Default.samework != samework.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void ColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.color_check = color_check.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void CountCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.count_check = count_check.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void BoxCountChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.boxCheck = boxCheck.IsChecked.Value;
            Properties.Settings.Default.Save();

        }

        private void KeyinChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.keyinCheck = keyinCheck.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void Detail_checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.detail_check = detail_check.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void Optionchecked(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default.option1 = option1.IsChecked.Value;
            Properties.Settings.Default.Save();

        }

        private void TwoSumCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.twoSum = twoSum.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        // 자재정보 설정
        private void PopErrorSearch()
        {

            string[] error = Properties.Settings.Default.error.Split(',');
            string[] error1 = Properties.Settings.Default.error1.Split(',');
            List<string> errorlist = new List<string>();
            List<string> errorlist1 = new List<string>();

            for (int i = 0; i < error.Length; i++)
            {
                errorlist.Add(error[i]);
            }

            for (int i = 0; i < error1.Length; i++)
            {
                errorlist1.Add(error1[i]);
            }

            _ = new LogData("불량사유조회");
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
               "[PopErrorSearch]",
                new string[] { "@ProcessType" },
                new object[] { Properties.Settings.Default.processType },
                0
                );

            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                if (Data_Jarry != null)
                {
                    ObservableCollection<Error_Model> ob = new ObservableCollection<Error_Model>();
                    ObservableCollection<Error_Model> ob1 = new ObservableCollection<Error_Model>();
                    foreach (JObject result in Data_Jarry)
                    {
                        if (Properties.Settings.Default.processType != int.Parse(result["ProcessType"].ToString()))
                        {
                            Error_Model model = new Error_Model()
                            {
                                ProcessType = int.Parse(result["ProcessType"].ToString()),
                                Memo = result["Memo"].ToString(),
                                ProcessTypeName = result["ProcessTypeName"].ToString(),
                                errorCheck = false
                            };
                            ob.Add(model);
                        }
                        else
                        {
                            Error_Model model1 = new Error_Model()
                            {
                                ProcessType = int.Parse(result["ProcessType"].ToString()),
                                Memo = result["Memo"].ToString(),
                                ProcessTypeName = result["ProcessTypeName"].ToString(),
                                errorCheck = false
                            };
                            ob1.Add(model1);
                        }

                        for(int i = 0; i < ob.Count; i++)
                        {
                            for(int j = 0; j < errorlist.Count; j++)
                            {
                                if (ob[i].Memo.Equals(errorlist[j]))
                                {
                                    ob[i].errorCheck = true;
                                    break;
                                }
                            }
                        }

                        for (int i = 0; i < ob1.Count; i++)
                        {
                            for (int j = 0; j < errorlist1.Count; j++)
                            {
                                if (ob1[i].Memo.Equals(errorlist1[j]))
                                {
                                    ob1[i].errorCheck = true;
                                    break;
                                }
                            }
                        }

                        ic1.ItemsSource = ob1;
                        ic.ItemsSource = ob;

                    }
                    // inputModel.errorModel.memo = "";
                    // errorOb = ob;
                    // combo.ItemsSource = ob;
                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopErrorSearch 조회 오류");
                _ = msg.ShowDialog();
            }
        }

        private void ScrollViewer_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        private void ScrollViewer_PreviewMouseButtonDown1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint1 = e.GetPosition(sv);
            hOff1 = sv.HorizontalOffset;
            vOff1 = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        private void Sv_PreviewMouseLeftButtonUp1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv1.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove1(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv1.IsMouseCaptured)
            {
                sv1.ScrollToHorizontalOffset(hOff1 + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv1.ScrollToVerticalOffset(vOff1 + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }



        private void AccountCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default.accountCheck = account_check.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void ProductionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.productionCheck = production_check.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void scrollvar_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.scrollvarCheck = scrollvar_Check.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void Checked_checkbox(object sender, MouseButtonEventArgs e)
        {
            if((sender as CheckBox).IsChecked == true)
            {
                string errorValue = "";
                string[] error = Properties.Settings.Default.error.Split(',');
                List<string> errorlist = new List<string>();
                for(int i = 0; i < error.Length; i++)
                {
                    errorlist.Add(error[i]);
                        
                    if(error[i].Equals((sender as CheckBox).Content))
                    {
                        errorlist.Remove(errorlist[i]);
                    }

                    else
                    {
                        errorValue = "";

                        for (int j = 0; j < errorlist.Count; j++)
                        {
                            if (j == 0)
                            {
                                errorValue += errorlist[j];
                            }

                            else
                            {
                                errorValue += "," + errorlist[j];
                            }

                        }
                    }

                }
                (sender as CheckBox).IsChecked = false;
                Properties.Settings.Default.error = "";
                Properties.Settings.Default.error = errorValue;
                Properties.Settings.Default.Save();
                Console.WriteLine("체크해제 :" + Properties.Settings.Default.error);
            }

            else
            {

                (sender as CheckBox).IsChecked = true;
                if (Properties.Settings.Default.error.Length == 0)
                {
                    Properties.Settings.Default.error = (sender as CheckBox).Content.ToString();
                }

                else
                {
                    Properties.Settings.Default.error += "," + (sender as CheckBox).Content.ToString();
                }
                Properties.Settings.Default.Save();
                Console.WriteLine("체크 :" + Properties.Settings.Default.error);
            }
            
        }

        private void Checked_checkbox1(object sender, MouseButtonEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                string errorValue = "";
                string[] error = Properties.Settings.Default.error1.Split(',');
                List<string> errorlist = new List<string>();
                for (int i = 0; i < error.Length; i++)
                {
                    errorlist.Add(error[i]);
                    if (error[i].Equals((sender as CheckBox).Content))
                    {
                        errorlist.Remove(errorlist[i]);
                       
                    }
                    else
                    {
                        errorValue = "";
                        for (int j = 0; j < errorlist.Count; j++)
                        {
                            if (j == 0)
                            {
                                errorValue += errorlist[j];
                            }
                            else
                            {
                                errorValue += "," + errorlist[j];
                            }

                        }
                    }

                }
                (sender as CheckBox).IsChecked = false;
                Properties.Settings.Default.error1 = "";
                Properties.Settings.Default.error1 = errorValue;
                Properties.Settings.Default.Save();
                Console.WriteLine("체크해제 :" + Properties.Settings.Default.error1);
            }
            else
            {

                (sender as CheckBox).IsChecked = true;
                if (Properties.Settings.Default.error1.Length == 0)
                {
                    Properties.Settings.Default.error1 = (sender as CheckBox).Content.ToString();
                }
                else
                {
                    Properties.Settings.Default.error1 += "," + (sender as CheckBox).Content.ToString();
                }
                Properties.Settings.Default.Save();
                Console.WriteLine("체크 :" + Properties.Settings.Default.error1);
            }
        }
        private void PlcCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Plc = plc.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void completeValue_checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.completeValue_checked = completeValue.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void upload_setting(object sender, MouseButtonEventArgs e)
        {
            string macAddress = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[Pop_machinfo_update]",
                new string[] { "@설비명", "@작업정보", "@상자수", "@동시작업", "@이합설비", "@상자수계산", "@맥주소" },
                new object[] {
                    Properties.Settings.Default.mercName,
                    Properties.Settings.Default.settingitem,
                    Properties.Settings.Default.boxCheck,
                    Properties.Settings.Default.samework,
                    Properties.Settings.Default.twoSum,
                    Properties.Settings.Default.option1,
                    macAddress
                },
                1

                );
            if (returnData.결과 == 1)
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("업로드 성공");
                msg.ShowDialog();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("저장 실패");
                msg.ShowDialog();
            }
        }

        private void load_setting(object sender, MouseButtonEventArgs e)
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[Pop_sel_settingValue]",
                new string[] { "@설비명"},
                new object[] {
                    Properties.Settings.Default.mercName,
                },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach(JObject result in Data_Jarry)
                {
                   Properties.Settings.Default.settingitem = result["작업정보"].ToString();
                   Properties.Settings.Default.boxCheck = bool.Parse(result["상자수사용"].ToString());
                   Properties.Settings.Default.samework = bool.Parse(result["동시작업"].ToString());
                   Properties.Settings.Default.twoSum = bool.Parse(result["이합설비"].ToString());
                   Properties.Settings.Default.option1 = bool.Parse(result["상자수자동계산"].ToString());
                   Properties.Settings.Default.Save();
                }

                Win_Set();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("로드 실패");
                msg.ShowDialog();
            }
        }

        private void robotChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.robotchecked = robotcheck.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }

}
