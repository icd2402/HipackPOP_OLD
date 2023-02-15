using PopProject.input;
using PopProject.KeyBoard;
using PopProject.Util;
using System;

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
            if (Properties.Settings.Default.keyinCheck)
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

            if (Properties.Settings.Default.keyinCheck)
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

            if (Properties.Settings.Default.keyinCheck)
            {
                pworkCount.Focusable = true;
                _ = pworkCount.Focus();
                _ = pworkCount.Dispatcher.BeginInvoke(new Action(() => { pworkCount.SelectAll(); }));
            }
        }



        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //수정 저장
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            PopresultUpdate();
        }

        private void PopresultUpdate()
        {
            string arrChkAED = "";
            string workCount = "";
            if (consumerCheck)
            {
                arrChkAED = pworkCount.Text.ToString().Equals("0") ? "3" : "2";
                if(Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트"))
                {
                    workCount = (int.Parse(pworkCount.Text) * inputModel.pCnt).ToString();
                }
                else
                {
                    workCount = pworkCount.Text;

                }

                ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
                ReturnModel returnData = ServiceModel.GetData(
                    Properties.Settings.Default.serverIndex,
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

                    Close();
                }
            }
            else
            {
                if (Properties.Settings.Default.corp_name.Equals("대세물산") || Properties.Settings.Default.corp_name.Equals("대세물산테스트"))
                {
                    workCount = (int.Parse(pworkCount.Text) * inputModel.pCnt).ToString();
                }
                else
                {
                    workCount = pworkCount.Text;

                }
                if (pworkCount.Text.ToString().Equals("0"))
                {
                    arrChkAED = "3|";
                }
                else
                {
                    arrChkAED = "2|";
                }
                ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
                string parameter = "[" + Properties.Settings.Default.mercName + "] " + inputModel.customSName + Environment.NewLine + inputModel.prodName + " " + inputModel.pworkCount.ToString("N0") + "개";
                ReturnModel returnData = ServiceModel.GetData(
                    Properties.Settings.Default.serverIndex,
                    "[PopResultEnd]",
                    new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고"},
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
                inputModel.pMemo + "|"
                     }, 1, parameter
                  );

                if (returnData.결과 == 1)
                {
                    Properties.Settings.Default.presultcount = float.Parse(pworkCount.Text);
                    Properties.Settings.Default.Save();
                    Close();
                }

            }

            
            

           

          

        }

        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void pworkCount_buttondown(object sender, MouseButtonEventArgs e)
        {

            TextBox context = (TextBox)sender;
            Win_KeyBoard win_KeyBoard = new Win_KeyBoard(context, inputModel,inputModel)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = 300
            };

            _ = win_KeyBoard.ShowDialog();
            win_KeyBoard = null;

        }
    }
}
