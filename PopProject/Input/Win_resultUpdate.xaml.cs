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
            if (pworkCount.Text.ToString().Equals("0"))
            {
                arrChkAED = "3|";
            }
            else
            {
                arrChkAED = "2|";
            }

            if (Properties.Settings.Default.boxCheck)
            {
                pworkCount.Text = ((int)(inputModel.pworkCount * inputModel.pCnt)).ToString();
            }
            else
            {
                if (Properties.Settings.Default.CountType == 1)
                {
                    if (Properties.Settings.Default.corp_name.Equals("신우수출포장") || Properties.Settings.Default.corp_name.Equals("우리포장산업"))
                    {
                        pworkCount.Text = ((int)(inputModel.pworkCount * inputModel.pCnt)).ToString();
                    }
                   
                }


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
                pworkCount.Text + "|",
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
