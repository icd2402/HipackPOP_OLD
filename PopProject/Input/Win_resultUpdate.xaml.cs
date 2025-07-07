using PopProject.input;
using PopProject.KeyBoard;
using PopProject.Util;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PopProject.Input
{
    /// <summary>
    /// Win_resultUpdate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_resultUpdate : Window
    {
        private InputModel inputModel;
        private errorModel errorModel;
        private bool consumerCheck = false;

        public Win_resultUpdate(errorModel errorModel, InputModel inputModel)
        {
            InitializeComponent();

            pworkCount.Text = errorModel.perrCount.ToString();
            this.inputModel = inputModel;
            this.errorModel = errorModel;

            //키인 체크시
            if (SettingJsonManager.keyinCheck)
            {
                pworkCount.Focusable = true;
                _ = pworkCount.Focus();
                _ = pworkCount.Dispatcher.BeginInvoke(new Action(() => { pworkCount.SelectAll(); }));
            }
        }

        public Win_resultUpdate(errorModel errorModel, InputModel inputModel, bool v)
        {
            InitializeComponent();

            consumerCheck = v;
            pworkCount.Text = errorModel.perrCount.ToString();
            this.inputModel = inputModel;
            this.errorModel = errorModel;

            if (SettingJsonManager.keyinCheck)
            {
                pworkCount.Focusable = true;
                _ = pworkCount.Focus();
                _ = pworkCount.Dispatcher.BeginInvoke(new Action(() => { pworkCount.SelectAll(); }));
            }
        }


        public Win_resultUpdate(InputModel inputModel)
        {
            InitializeComponent();
            this.inputModel = inputModel;
            pworkCount.Text = inputModel.sumPworkCount.ToString();

            if (SettingJsonManager.keyinCheck)
            {
                pworkCount.Focusable = true;
                _ = pworkCount.Focus();
                _ = pworkCount.Dispatcher.BeginInvoke(new Action(() => { pworkCount.SelectAll(); }));
            }
        }


        // 드래그
        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //저장 버튼
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            PopresultUpdate();
        }

        //저장 버튼
        private void PopresultUpdate()
        {

            string 전체공정 = "0";
            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (SettingJsonManager.mercName.Contains("인라인"))
                {
                    전체공정 = "1";

                }
                else
                {
                    전체공정 = "0";
                }
            }

            string arrChkAED = "";
            string workCount = "";

            //실수요자 intput에서 실행시
            if (consumerCheck)
            {
                arrChkAED = pworkCount.Text.ToString().Equals("0") ? "3" : "2";

                if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
                {
                    workCount = (int.Parse(pworkCount.Text) * inputModel.pCnt).ToString();
                }

                else
                {
                    workCount = pworkCount.Text;

                }

                ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                ReturnModel returnData = ServiceModel.GetData(
                    SettingJsonManager.serverIndex,
                    "[PopResultEndMember]",
                    new string[] { "@작업일자", "@오더Key", "@ChkAED", "@실수요자생산Key", "@작업량", "@불량" },
                    new object[] {
                DateTime.Now.ToString("yyyy-MM-dd"),
                inputModel.orderKey,
                arrChkAED,
                errorModel.workMemberKey,
                workCount,
                inputModel.perrCount,

                     }, 1

                  );

                if (returnData.결과 == 1)
                {
                    LogManager.Info("실수요자 생산결과 수정 완료" + workCount + "개", MethodBase.GetCurrentMethod().DeclaringType);

                    Close();
                }
            }
            //일반 intput에서 실행시
            else
            {
                //대세물산은 생산수량 수정시 통수를 곱해줌
                if (SettingJsonManager.corp_name.Equals("대세물산") || SettingJsonManager.corp_name.Equals("대세물산테스트"))
                {
                    workCount = (int.Parse(pworkCount.Text) * inputModel.pCnt).ToString();
                }
                else
                {
                    workCount = pworkCount.Text;
                }

                if (pworkCount.Text.ToString().Equals("0"))
                {
                    arrChkAED = "3|"; //삭제
                }
                else
                {
                    arrChkAED = "2|"; //수정
                }

                ServiceModel.ServiceModelClean(inputModel.ServerIndex, inputModel.DBName);
                string parameter = "[" + SettingJsonManager.mercName + "] " + inputModel.customSName + Environment.NewLine + inputModel.prodName + " " + inputModel.pworkCount.ToString("N0") + "개";
                ReturnModel returnData = ServiceModel.GetData(
                    inputModel.ServerIndex,
                    "[PopResultEnd]",
                    new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@전체공정"},
                    new object[] {
                "1",
                DateTime.Now.ToString("yyyy-MM-dd"),
                inputModel.orderKey,
                arrChkAED,
                inputModel.ProcessType + "|",
                inputModel.pplanKey + "|",
                errorModel.pworkKey + "|",
                workCount + "|",
                inputModel.perrCount + "|",
                inputModel.pMemo + "|",
                전체공정
                     }, 1, parameter
                  );

                if (returnData.결과 == 1)
                {
                    LogManager.Info("일반 생산결과 수정 완료" + workCount + "개", MethodBase.GetCurrentMethod().DeclaringType);
                    SettingJsonManager.presultcount = float.Parse(pworkCount.Text);
                    SettingJsonManager.SaveJsonFile();
                    Close();
                }
            }
        }

        //닫기
        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //수량 칸 클릭시 키보드 팝업
        private void pworkCount_buttondown(object sender, MouseButtonEventArgs e)
        {

            TextBox context = (TextBox)sender;
            Win_KeyBoard win_KeyBoard = new Win_KeyBoard(context, inputModel, inputModel)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = 300
            };

            _ = win_KeyBoard.ShowDialog();
            win_KeyBoard = null;
        }
    }
}
