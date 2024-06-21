using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
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
        private bool testLock = false;

        public Win_Setting()
        {
            InitializeComponent();
            PopErrorSearch();
            Win_Set();

            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                _cb1.Content = "접합 방법";   _cb2.Content = "코팅";
                _cb3.Content = "밴딩 매수";   _cb4.Content = "적재 방법";
                _cb5.Content = "적재 단수"; _cb6.Content = "파렛트 종류";
                _cb7.Content = "손잡이 종류";   _cb8.Content = "밴딩 방법";
                _cb9.Content = "목형 NO";        _cb10.Content = "";
                _cb11.Content = "";           _cb12.Content = "";
                _cb13.Content = "";           _cb14.Content = "";
                _cb15.Content = ""; 

                _cb10.Visibility = Visibility.Collapsed;
                _cb11.Visibility = Visibility.Collapsed;
                _cb12.Visibility = Visibility.Collapsed;
                _cb13.Visibility = Visibility.Collapsed;
                _cb14.Visibility = Visibility.Collapsed;
                _cb15.Visibility = Visibility.Collapsed;
            }
        }

        private void Win_Set()
        {
            //설비명
            Setting_mercName.Text = SettingJsonManager.mercName;

            //기본 설정2 화면
            if (SettingJsonManager.RadioChecked)//상단,하단 정렬
            {
                radio2.IsChecked = true; //상단
            }
            else
            {
                radio1.IsChecked = true; //하단
            }

            //기본 설정2 화면
            keyinCheck.IsChecked = SettingJsonManager.keyinCheck;//키인 사용
            robotcheck.IsChecked = SettingJsonManager.robotchecked; //로봇사용
            detail_check.IsChecked = SettingJsonManager.detail_check; //듀얼 모니터 상세화면 표현
            option1.IsChecked = SettingJsonManager.option1; //상자수 자동계산
            samework.IsChecked = SettingJsonManager.samework; //동시작업 설정
            color_check.IsChecked = SettingJsonManager.color_check; //인쇄 색도 표현
            twoSum.IsChecked = SettingJsonManager.twoSum; //이합설비
            scrollvar_Check.IsChecked = SettingJsonManager.scrollvarCheck; //스크롤바 사용
            plc.IsChecked = SettingJsonManager.Plc; // PLC사용
            boxCheck.IsChecked = SettingJsonManager.boxCheck; //상자수 사용
            completeValue.IsChecked = SettingJsonManager.completeValue_checked; //완료항목 제거
            detailPass.IsChecked = SettingJsonManager.detailPass; //상세창 넘기기 사용
            detailPopup.IsChecked = SettingJsonManager.detailPopup; //상세창 먼저 팝업 사용
            windowMax.IsChecked = SettingJsonManager.windowState; //윈도우 창 설정
            noticePopup.IsChecked = SettingJsonManager.noticePopup; //공지사항 자동 팝업


            //자재명세서 출력정보 화면
            count_check.IsChecked = SettingJsonManager.count_check; //총수량 표현
            account_check.IsChecked = SettingJsonManager.accountCheck; //거래처 표현
            production_check.IsChecked = SettingJsonManager.productionCheck; //생산처 표현

            

            //시리얼 포트
            string[] port = SerialPort.GetPortNames();
            
            //포트넘버
            cb_1.ItemsSource = port; 
            cb_12.ItemsSource = port; 
            cb_1.Text = SettingJsonManager.SerialName;
            cb_12.Text = SettingJsonManager.SerialName1; 
            
            //통신속도
            int[] baudrate = new int[1] { 9600 }; //통신속도
            cb_2.ItemsSource = baudrate;
            cb_2.Text = SettingJsonManager.baudrate.ToString();

            //센서 딜레이
            string[] SensorRate = new string[7] { "없음", "0.1초", "0.2초", "0.3초", "0.4초", "0.5초", "1초" };
            cb_3.ItemsSource = SensorRate;
            cb_3.Text = GetSenserRate(SettingJsonManager.SensorRate);

            string settingItem = SettingJsonManager.settingitem;  //작업정보
            string metrialinfo = SettingJsonManager.meterialinfo; //마닐라,원단정보
            string printItem = SettingJsonManager.printItem; //자재명세서 출정 정보

            string[] setting = settingItem.Split(new string[] { "|" }, StringSplitOptions.None); //작업정보 배열 생성
            string[] metrial = metrialinfo.Split(new string[] { "|" }, StringSplitOptions.None); //마닐라,원단정보 배열 생성
            string[] print = printItem.Split(new string[] { "|" }, StringSplitOptions.None); //저장된 자재명세서 출정 정보 배열 생성
            Console.WriteLine("프린터 셋팅정보 : " + print);

            //자재명세서 배열
            for (int i = 0; i < print.Length; i++)
            {
                if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
                {
                    switch (print[i])
                    {
                        case "1": //접합 방법
                            _cb1.IsChecked = true;
                            break;
                        case "2": //코팅
                            _cb2.IsChecked = true;
                            break;
                        case "3": //밴딩 매수
                            _cb3.IsChecked = true;
                            break;
                        case "4": //접재 방법
                            _cb4.IsChecked = true;
                            break;
                        case "5": //적재 단수
                            _cb5.IsChecked = true;
                            break;
                        case "6": //파렛트 종류
                            _cb6.IsChecked = true;
                            break;
                        case "7": //손잡이 종류
                            _cb7.IsChecked = true;
                            break;
                        case "8": //밴딩 방법
                            _cb8.IsChecked = true;
                            break;
                        case "9": //목형 NO
                            _cb9.IsChecked = true;
                            break;
                        
                        default:
                            break;
                    }
                }
                else
                {
                    switch (print[i])
                    {
                        case "1": //필름NO
                            _cb1.IsChecked = true;
                            break;
                        case "2": //코팅
                            _cb2.IsChecked = true;
                            break;
                        case "3": //조판NO
                            _cb3.IsChecked = true;
                            break;
                        case "4": //조판위치
                            _cb4.IsChecked = true;
                            break;
                        case "5": //목형NO
                            _cb5.IsChecked = true;
                            break;
                        case "6": //목형위치
                            _cb6.IsChecked = true;
                            break;
                        case "7": //인쇄색도
                            _cb7.IsChecked = true;
                            break;
                        case "8": //접합방법
                            _cb8.IsChecked = true;
                            break;
                        case "9": //손잡이
                            _cb9.IsChecked = true;
                            break;
                        case "10": //포장매수
                            _cb10.IsChecked = true;
                            break;
                        case "11": //포장방법
                            _cb11.IsChecked = true;
                            break;
                        case "12"://파렛트 종류
                            _cb12.IsChecked = true;
                            break;
                        case "13": //적재단수
                            _cb13.IsChecked = true;
                            break;
                        case "14": //적재방법
                            _cb14.IsChecked = true;
                            break;
                        case "15": //메모
                            _cb15.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                }
                
                
            }

            //마닐라,원단정보 배열
            for (int i = 0; i < metrial.Length; i++)
            {
                switch (metrial[i])
                {
                    case "19": //마닐라 정보
                        scinfo.IsChecked = true;
                        break;
                    case "20": //원단 정보
                        wdinfo.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }

            //작업정보 배열 생성
            for (int i = 0; i < setting.Length; i++)
            {
                switch (setting[i])
                {
                    case "1": //필름NO
                        cb1.IsChecked = true;
                        break;
                    case "2": //코팅
                        cb2.IsChecked = true;
                        break;
                    case "3": //조판NO
                        cb3.IsChecked = true;
                        break;
                    case "4": //조판위치
                        cb4.IsChecked = true;
                        break;
                    case "5": //목형NO
                        cb5.IsChecked = true;
                        break;
                    case "6": //목형위치
                        cb6.IsChecked = true;
                        break;
                    case "7": //인쇄색도
                        cb7.IsChecked = true;
                        break;
                    case "8": //접합방법
                        cb8.IsChecked = true;
                        break;
                    case "9": //손잡이
                        cb9.IsChecked = true;
                        break;
                    case "10": //포장매수
                        cb10.IsChecked = true;
                        break;
                    case "11": //포장방법
                        cb11.IsChecked = true;
                        break;
                    case "12": //참고사항
                        cb12.IsChecked = true;
                        break;
                    case "13": //공정별참고사항
                        cb13.IsChecked = true;
                        break;
                    case "14": //납기일
                        cb14.IsChecked = true;
                        break;
                    case "15": //파렛트종류
                        cb15.IsChecked = true;
                        break;
                    case "16": //특별지시사항
                        cb16.IsChecked = true;
                        break;
                    case "17": //spare
                        cb17.IsChecked = true;
                        break;
                    case "18": //spare2
                        cb18.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
        }

        //드래그
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //닫기
        private void Exit(object sender, MouseButtonEventArgs e)
        {
            구룡제품명세서저장();
            Close();
        }

         // 기본설정1 [작업 정보] 설정 체크 수량
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            List<string> list = new List<string>();
            int count = 0;

            //왼쪽줄 확인
            foreach (CheckBox cbx in str.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    count++;
                    if (count < 5)
                    {
                        list.Add(cbx.Content.ToString());
                    }
                }

                else
                {
                    _ = list.Remove(cbx.Content.ToString());
                }

            }

            //오른쪽줄 확인
            foreach (CheckBox cbx in str1.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    count++;
                    if (count < 5)
                    {
                        list.Add(cbx.Content.ToString());
                    }
                }
                else
                {
                    _ = list.Remove(cbx.Content.ToString());
                }
            }

            //체크수량 4개 넘으면 체크안되게함
            if (count > 4)
            {
                CheckBox chb = (CheckBox)sender;
                chb.IsChecked = false;
            }

            string settingValue = GetSettingValue(list);
            if (settingValue.Length != 0)
            {
                settingValue = settingValue.Substring(0, settingValue.Length - 1);
            }

            Console.WriteLine("작업정보 체크수량 ->" + count);
            Console.WriteLine("SettingValue(작업정보) : " + settingValue);
            SettingJsonManager.searchCheck = !settingValue.Equals(SettingJsonManager.settingitem);
            SettingJsonManager.settingitem = settingValue;
            //SettingJsonManager.SaveJsonFile();

            LogManager.Info("작업정보 설정 변경 : " + settingValue, MethodBase.GetCurrentMethod().DeclaringType);
        }

        //마닐라,원단정보 체크
        private void CheckBox_Checked1(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            //체크항목 확인
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

            Console.WriteLine("마닐라,원단 체크 : " + settingValue);
            SettingJsonManager.searchCheck = !settingValue.Equals(SettingJsonManager.meterialinfo); //저장값이 변경되어있으면
            SettingJsonManager.meterialinfo = settingValue;
            SettingJsonManager.SaveJsonFile();

            LogManager.Info("자재정보 설정 변경 : " + settingValue, MethodBase.GetCurrentMethod().DeclaringType);
        }

        // 자재명세서 출력 정보 설정 체크시
        private void Setting_print(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            //왼쪽
            foreach (CheckBox cbx in str3.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            //오른쪽
            foreach (CheckBox cbx in str4.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            string printItem = GetPrintItem(list);

            //명세서 출력정보 체크값 있을시 담고 저장
            if (printItem.Length != 0)
            {
                printItem = printItem.Substring(0, printItem.Length - 1);
            }

            Console.WriteLine("printItem(명세서) : " + printItem);
            SettingJsonManager.printItem = printItem;
            //SettingJsonManager.SaveJsonFile();

            LogManager.Info("작업정보 설정 변경 : " + printItem, MethodBase.GetCurrentMethod().DeclaringType);
        }

        //제품명세서 출력 정보 체크값 저장
        private string GetPrintItem(List<string> list)
        {
            string settindValue = "";

            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    int setting = 0;
                    switch (list[i])
                    {
                        case "접합 방법":
                            setting = 1;
                            break;
                        case "코팅":
                            setting = 2;
                            break;
                        case "밴딩 매수":
                            setting = 3;
                            break;
                        case "적재 방법":
                            setting = 4;
                            break;
                        case "적재 단수":
                            setting = 5;
                            break;
                        case "파렛트 종류":
                            setting = 6;
                            break;
                        case "손잡이 종류":
                            setting = 7;
                            break;
                        case "밴딩 방법":
                            setting = 8;
                            break;
                        case "목형 NO":
                            setting = 9;
                            break;
                       
                        default:
                            break;
                    }

                    settindValue = settindValue + setting + "|";
                    //Console.WriteLine("printlist : " + i + ", " + list[i].ToString());
                }
            }
            else
            {
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
                            setting = 12;
                            break;
                        case "적재 단수":
                            setting = 13;
                            break;
                        case "적재 방법":
                            setting = 14;
                            break;
                        case "메모":
                            setting = 15;
                            break;
                        default:
                            break;
                    }

                    settindValue = settindValue + setting + "|";
                    //Console.WriteLine("printlist : " + i + ", " + list[i].ToString());
                }
            }

            Console.WriteLine("printItem : " + settindValue);
            LogManager.Info("제품명세서 설정 변경 : " + settindValue, MethodBase.GetCurrentMethod().DeclaringType);

            return settindValue;
        }

       

        // 기본설정1 작업정보 설정 체크 해제
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
            SettingJsonManager.searchCheck = !settingValue.Equals(SettingJsonManager.settingitem);
            SettingJsonManager.settingitem = settingValue;
            SettingJsonManager.SaveJsonFile();
        }

        //작업정보 설정 list에 담기위한 번호 부여
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
                    case "참고 사항":
                        setting = 12;
                        break;
                    case "공정별참고사항":
                        setting = 13;
                        break;
                    case "납기일":
                        setting = 14;
                        break;
                    case "파렛트 종류":
                        setting = 15;
                        break;
                    case "특별지시사항":
                        setting = 16;
                        break;
                    case "spare":
                        setting = 17;
                        break;
                    case "spare2":
                        setting = 18;
                        break;
                    case "마닐라 정보":
                        setting = 19;
                        break;
                    case "원단 정보":
                        setting = 20;
                        break;
                    default:
                        break;
                }

                settindValue = settindValue + setting + "|";
                Console.WriteLine("list" + i + " :" + list[i].ToString()); //선택된 리스트 확인용
            }
            return settindValue;

        }

        private void Setting_done(object sender, MouseButtonEventArgs e)
        {
            구룡제품명세서저장();
            Close();
        }

        //센서 딜레이
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
                case "5": 
                    str = "0.5초";
                    break;
                case "9":
                    str = "1초";
                    break;
                default:
                    break;
            }
            LogManager.Info("게더링 센서딜레이 변경 : " + str, MethodBase.GetCurrentMethod().DeclaringType);
            return str;
        }


        //창 닫을때 저장
        private void Window_Closed(object sender, EventArgs e)
        {
            //게더링 COM넘버
            if (cb_1.SelectedItem != null)
            {
                SettingJsonManager.SerialName = cb_1.SelectedItem.ToString();
            }
            
            //게더링 COM넘버 두번째꺼
            if (cb_12.SelectedItem != null)
            {
                SettingJsonManager.SerialName1 = cb_12.SelectedItem.ToString();
            }

            //게더링 BaudRate
            if (cb_2.SelectedItem != null)
            {
                SettingJsonManager.baudrate = int.Parse(cb_2.SelectedItem.ToString());
            }

            //센서 딜레이
            if (cb_3.SelectedItem != null)
            {
                switch (cb_3.Text)
                {
                    case "없음":
                        SettingJsonManager.SensorRate = "0";
                        break;
                    case "0.1초":
                        SettingJsonManager.SensorRate = "1";
                        break;
                    case "0.2초":
                        SettingJsonManager.SensorRate = "2";
                        break;
                    case "0.3초":
                        SettingJsonManager.SensorRate = "3";
                        break;
                    case "0.4초":
                        SettingJsonManager.SensorRate = "4";
                        break;
                    case "0.5초":
                        SettingJsonManager.SensorRate = "5";
                        break;
                    case "1초":
                        SettingJsonManager.SensorRate = "9";
                        break;
                    default:
                        break;
                }
            }

            SettingJsonManager.SaveJsonFile();
            LogManager.Info("설정창 닫기", MethodBase.GetCurrentMethod().DeclaringType);
        }

        //센서 딜레이 설정
        private void Cb_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        //상단,하단 정렬 버튼
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.RadioChecked = !radio1.IsChecked.Value;
            Console.WriteLine("radio1.IsChecked.Value" + radio1.IsChecked.Value);
            //SettingJsonManager.SaveJsonFile();
        }

        //동시작업 
        private void SameCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.samework = samework.IsChecked.Value;
            SettingJsonManager.searchCheck = SettingJsonManager.samework != samework.IsChecked.Value;
            //SettingJsonManager.SaveJsonFile();
        }

        //인쇄색도 표현
        private void ColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.color_check = color_check.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //총수량 표현
        private void CountCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.count_check = count_check.IsChecked.Value;
            //SettingJsonManager.SaveJsonFile();
        }

        //상자수 사용
        private void BoxCountChecked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.boxCheck = boxCheck.IsChecked.Value;
            //SettingJsonManager.SaveJsonFile();

        }

        //키인 사용
        private void KeyinChecked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.keyinCheck = keyinCheck.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //듀얼 모니터 상세화면 표현
        private void Detail_checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.detail_check = detail_check.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //상자수 자동계산
        private void Optionchecked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.option1 = option1.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //이합설비
        private void TwoSumCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.twoSum = twoSum.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        // 불량사유 화면
        private void PopErrorSearch()
        {
            string[] error = SettingJsonManager.error.Split(','); //공통
            string[] error1 = SettingJsonManager.error1.Split(','); //설비
            List<string> errorlist = new List<string>();
            List<string> errorlist1 = new List<string>();

            //공통
            for (int i = 0; i < error.Length; i++)
            {
                errorlist.Add(error[i]);
            }

            //설비
            for (int i = 0; i < error1.Length; i++)
            {
                errorlist1.Add(error1[i]);
            }

            _ = new LogData("불량사유조회");
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
               "[PopErrorSearch]",
                new string[] { "@ProcessType" },
                new object[] { SettingJsonManager.processType },
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
                        if (SettingJsonManager.processType != int.Parse(result["ProcessType"].ToString())) //공통
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
                        else //설비
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

                        //공통
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

                        //설비
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

                        ic.ItemsSource = ob; //공통
                        ic1.ItemsSource = ob1; //설비
                    }
                }
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopErrorSearch 조회 오류");
                _ = msg.ShowDialog();
            }
        }

        //공통 스크롤뷰어 클릭
        private void ScrollViewer_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        //공통 스크롤뷰어 클릭
        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }


        //공통 스크롤뷰어 마우스 이동
        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        //설비 스크롤뷰어 클릭
        private void ScrollViewer_PreviewMouseButtonDown1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint1 = e.GetPosition(sv);
            hOff1 = sv.HorizontalOffset;
            vOff1 = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        //설비 스크롤뷰어 클릭
        private void Sv_PreviewMouseLeftButtonUp1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv1.ReleaseMouseCapture();
        }

        //설비 스크롤뷰어 마우스 이동
        private void Sv_PreviewMouseMove1(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv1.IsMouseCaptured)
            {
                sv1.ScrollToHorizontalOffset(hOff1 + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv1.ScrollToVerticalOffset(vOff1 + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        //자재명세서 거래처
        private void AccountCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            SettingJsonManager.accountCheck = account_check.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //자재명세서 생산처
        private void ProductionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.productionCheck = production_check.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //스크롤바 사용
        private void scrollvar_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.scrollvarCheck = scrollvar_Check.IsChecked.Value;
            //SettingJsonManager.SaveJsonFile();
        }

        //공통 불량사유 체크박스
        private void Checked_checkbox(object sender, MouseButtonEventArgs e)
        {
            if((sender as CheckBox).IsChecked == true) //체크해제 
            {
                string errorValue = "";
                string[] error = SettingJsonManager.error.Split(',');
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
                SettingJsonManager.error = "";
                SettingJsonManager.error = errorValue;
                SettingJsonManager.SaveJsonFile();
                Console.WriteLine("체크해제 :" + SettingJsonManager.error);
            }
            else //체크
            {
                (sender as CheckBox).IsChecked = true;
                if (SettingJsonManager.error.Length == 0)
                {
                    SettingJsonManager.error = (sender as CheckBox).Content.ToString();
                }
                else
                {
                    SettingJsonManager.error += "," + (sender as CheckBox).Content.ToString();
                }
                SettingJsonManager.SaveJsonFile();
                Console.WriteLine("체크 :" + SettingJsonManager.error);
                LogManager.Info("공통 불량사유 체크 : " + SettingJsonManager.error, MethodBase.GetCurrentMethod().DeclaringType);
            }
            
        }

        //설비 불량사유 체크박스
        private void Checked_checkbox1(object sender, MouseButtonEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true) //체크해제
            {
                string errorValue = "";
                string[] error = SettingJsonManager.error1.Split(',');
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
                SettingJsonManager.error1 = "";
                SettingJsonManager.error1 = errorValue;
                SettingJsonManager.SaveJsonFile();
                Console.WriteLine("체크해제 :" + SettingJsonManager.error1);
            }
            else //체크
            {

                (sender as CheckBox).IsChecked = true;
                if (SettingJsonManager.error1.Length == 0)
                {
                    SettingJsonManager.error1 = (sender as CheckBox).Content.ToString();
                }
                else
                {
                    SettingJsonManager.error1 += "," + (sender as CheckBox).Content.ToString();
                }
                SettingJsonManager.SaveJsonFile();
                Console.WriteLine("체크 :" + SettingJsonManager.error1);
                LogManager.Info("설비 불량사유 체크 : " + SettingJsonManager.error1, MethodBase.GetCurrentMethod().DeclaringType);

            }
        }

        //로봇사용 
        private void robotChecked(object sender, RoutedEventArgs e)
        {
            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트") || 
                SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
            {
                SettingJsonManager.robotchecked = robotcheck.IsChecked.Value;
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                robotcheck.IsChecked = false;
            }
        }

        //PLC사용
        private void PlcCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트") || 
                SettingJsonManager.corp_name.Equals("대양포장") || SettingJsonManager.corp_name.Equals("대양포장테스트") || 
                SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
            {
                SettingJsonManager.Plc = plc.IsChecked.Value;
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                plc.IsChecked = false;
            }
        }

        //상세화면 넘기기
        private void detailPass_Checked(object sender, RoutedEventArgs e)
        {
            if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
            {
                SettingJsonManager.detailPass = detailPass.IsChecked.Value;
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                detailPass.IsChecked = false;
            }
        }

        //상세화면 먼저 팝업
        private void detailPopup_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.detailPopup = detailPopup.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //윈도우 전체화면 설정
        private void windowMax_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.windowState = windowMax.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //공지사항 자동 팝업
        private void noticePopup_Checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.noticePopup = noticePopup.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }


        //완료항목 제거
        private void completeValue_checked(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.completeValue_checked = completeValue.IsChecked.Value;
            SettingJsonManager.SaveJsonFile();
        }

        //설정값 업로드 버튼 (DB저장)
        private void upload_setting(object sender, MouseButtonEventArgs e)
        {
            string macAddress = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[Pop_machinfo_update]",
                new string[] { "@설비명", "@작업정보", "@상자수", "@동시작업", "@이합설비", "@상자수계산", "@맥주소" },
                new object[] {
                    SettingJsonManager.mercName,    //설비명
                    SettingJsonManager.settingitem, //자재정보
                    SettingJsonManager.boxCheck,    //상자수 사용
                    SettingJsonManager.samework,    //동시작업
                    SettingJsonManager.twoSum,      //이합설비
                    SettingJsonManager.option1,     //상자수 자동계산
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

        //설정값 로드 버튼 (DB불러오기)
        private void load_setting(object sender, MouseButtonEventArgs e)
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[Pop_sel_settingValue]",
                new string[] { "@설비명"},
                new object[] {
                    SettingJsonManager.mercName,
                },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                foreach(JObject result in Data_Jarry)
                {
                    SettingJsonManager.settingitem = result["작업정보"].ToString();
                    SettingJsonManager.boxCheck = bool.Parse(result["상자수사용"].ToString());
                    SettingJsonManager.samework = bool.Parse(result["동시작업"].ToString());
                    SettingJsonManager.twoSum = bool.Parse(result["이합설비"].ToString());
                    SettingJsonManager.option1 = bool.Parse(result["상자수자동계산"].ToString());
                }

                SettingJsonManager.SaveJsonFile();
                Win_Set();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("로드 실패");
                msg.ShowDialog();
            }
        }

        

        private void 구룡제품명세서저장()
        {
            List<string> list = new List<string>();

            //왼쪽
            foreach (CheckBox cbx in str3.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            //오른쪽
            foreach (CheckBox cbx in str4.Children.OfType<CheckBox>())
            {
                if (cbx.IsChecked == true)
                {
                    list.Add(cbx.Content.ToString());
                }
            }

            string printItem = GetPrintItem2(list);

            //명세서 출력정보 체크값 있을시 담고 저장
            if (printItem.Length != 0)
            {
                printItem = printItem.Substring(0, printItem.Length - 1);
            }

            Console.WriteLine("printItem(명세서) : " + printItem);
            SettingJsonManager.printItem = printItem;
            SettingJsonManager.SaveJsonFile();

            LogManager.Info("작업정보 설정 변경 : " + printItem, MethodBase.GetCurrentMethod().DeclaringType);
        }

        private string GetPrintItem2(List<string> list)
        {
            string settindValue = "";

            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    int setting = 0;
                    switch (list[i])
                    {
                        case "접합 방법":
                            setting = 1;
                            break;
                        case "코팅":
                            setting = 2;
                            break;
                        case "밴딩 매수":
                            setting = 3;
                            break;
                        case "적재 방법":
                            setting = 4;
                            break;
                        case "적재 단수":
                            setting = 5;
                            break;
                        case "파렛트 종류":
                            setting = 6;
                            break;
                        case "손잡이 종류":
                            setting = 7;
                            break;
                        case "밴딩 방법":
                            setting = 8;
                            break;
                        case "목형 NO":
                            setting = 9;
                            break;

                        default:
                            break;
                    }

                    settindValue = settindValue + setting + "|";
                    //Console.WriteLine("printlist : " + i + ", " + list[i].ToString());
                }
            }
            else
            {
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
                            setting = 12;
                            break;
                        case "적재 단수":
                            setting = 13;
                            break;
                        case "적재 방법":
                            setting = 14;
                            break;
                        case "메모":
                            setting = 15;
                            break;
                        default:
                            break;
                    }

                    settindValue = settindValue + setting + "|";
                    //Console.WriteLine("printlist : " + i + ", " + list[i].ToString());
                }
            }

            Console.WriteLine("printItem : " + settindValue);
            LogManager.Info("제품명세서 설정 변경 : " + settindValue, MethodBase.GetCurrentMethod().DeclaringType);

            return settindValue;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //배치파일 내용
            string batContent = @"@echo off
                                  :: PopProject 프로그램 종료
                                  taskkill /IM PopProject.exe /F
                                  
                                  :: 사용자 폴더 삭제
                                  rmdir /S /Q ""C:\Users\%USERNAME%\AppData\Local\Apps\2.0""
                                  
                                  :: admin 폴더 삭제
                                  rmdir /S /Q ""C:\Users\admin\AppData\Local\Apps\2.0""
                                  
                                  :: Test 폴더 삭제
                                  rmdir /S /Q ""C:\Users\wlgns\AppData\Local\Apps\3.0""

                                  :: Test 폴더 삭제
                                  rmdir /S /Q ""C:\Users\SungPC\AppData\Local\Apps\2.0""

                                  :: 창 닫기
                                  exit";

            // 배치파일 생성 폴더 경로, 이름
            string folderPath = @"C:\ICD\POP";
            string filePath = Path.Combine(folderPath, "CleanPopProject.bat");

            try
            {
                // 폴더가 존재하지 않으면 생성
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 배치 파일 작성
                File.WriteAllText(filePath, batContent);

                // 배치파일 실행
                ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + filePath);
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                Process process = Process.Start(processInfo);
            }
            catch (Exception ex)
            {
            }

           



















            //string folderPath = @"C:\Users\admin\AppData\Local\Apps\2.0"; //
            //try
            //{
            //    // 폴더가 존재하는지 확인
            //    if (Directory.Exists(folderPath))
            //    {
            //        // 폴더와 하위 폴더 및 파일 삭제
            //        Directory.Delete(folderPath, true);
            //        Console.WriteLine("폴더가 성공적으로 삭제되었습니다.");
            //        MessageBox_Dialog dialog = new MessageBox_Dialog("프로그램을 재실행 해주세요.");
            //        dialog.ShowDialog();
            //    }
            //    else //폴더가 없으면 다른경로 삭제시도
            //    {
            //        folderPath = @"C:\Users\USER\AppData\Local\Apps\2.0";

            //        tryㄷ
            //        {
            //            if (Directory.Exists(folderPath))
            //            {
            //                Directory.Delete(folderPath, true);
            //                Console.WriteLine("대체 폴더가 성공적으로 삭제되었습니다.");
            //                MessageBox_Dialog dialog = new MessageBox_Dialog("프로그램을 재실행 해주세요.");
            //                dialog.ShowDialog();
            //            }
            //            else
            //            {
            //                MessageBox_Dialog dialog = new MessageBox_Dialog("업데이트 다시받기 실패.");
            //                dialog.ShowDialog();
            //            }
            //        }
            //        catch (Exception ex2)
            //        {

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
        }
    }

}
