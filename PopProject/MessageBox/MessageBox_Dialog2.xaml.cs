using PopProject.input;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PopProject.MessageBox
{
    /// <summary>
    /// MessageBox_Dialog2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBox_Dialog2 : Window
    {
        private String Message;
        private int flag = 0;
        private InputModel inputModel;
        private MainModel mainModel;
        private bool state = false;

        public MessageBox_Dialog2(InputModel inputModel, MainModel mainModel, String Message, bool State)
        {
            InitializeComponent();
            this.Message = Message;
            message_content.Text = Message;
            this.inputModel = inputModel;
            this.mainModel = mainModel;
            state = State;
        }

        public MessageBox_Dialog2(String Message, int flag)
        {

            InitializeComponent();
            this.Message = Message;
            message_content.Text = Message;
            this.flag = flag;

        }

        private void Close_messagebox(object sender, MouseEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {

            switch (flag)
            {
                case 0:
                    Window.GetWindow(this).Close();
                    break;

                case 1:
                    System.Diagnostics.Process.Start("explorer.exe", "http://packcom.net/Setup/POP_NEW/publish.htm");
                    Window.GetWindow(this).Close();
                    break;
            }
        }

        private void Cancel_button(object sender, MouseButtonEventArgs e)
        {
            //일반 생산건 강제종료
            if (state)
            {
                PopResultEnd();
            }
            else//실수요자 생산건 강제종료
            {
                PopResultEndMember();
            }

            Window.GetWindow(this).Close();
        }

        private int PopResultEnd()
        {
            string 전체공정 = "0";

            if (SettingJsonManager.corp_name.Equals("구룡산업제2공장") || SettingJsonManager.corp_name.Equals("구룡산업제2공장 테스트"))
            {
                if (SettingJsonManager.mercName.Contains("인라인"))
                {
                    전체공정 = "1";
                }
            }

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            string parameter = "[" + SettingJsonManager.mercName + "] " + this.inputModel.customSName + Environment.NewLine + this.inputModel.prodName + " " + this.inputModel.pworkCount.ToString("N0") + "개";
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEnd]",
                new string[] { "@AllChkAED", "@작업일자", "@오더Key", "@arrChkAED", "@arr공정구분", "@arr계획Key",
                "@arr작업Key", "@arr작업량", "@arr불량", "@arr비고","@시작시간","@설비명", "@전체공정"},
                new object[] {
                "1",
                DateTimeManager.GetSelectedDateStr(),
                this.inputModel.orderKey,
                "3|",
                this.inputModel.ProcessType + "|",
                this.inputModel.pplanKey + "|",
                this.inputModel.pworkKey + "|",
                0 + "|",
                0 + "|",
                this.inputModel.pMemo + "|",
                this.inputModel.sDatetime,
                SettingJsonManager.mercName,
                전체공정
                }, 1, parameter
              );

            if (returnData.결과 == 1)
            {
                Console.WriteLine();



                this.inputModel.imagePath = "/img/bn_05.png"; //input창 시작버튼으로 변경
                this.mainModel.imagePath = "/img/bn_02.png"; //main창 대기버튼으로 변경

                //공정 상태 검정색으로 변경
                for (int i = 0; i < mainModel.subOb.Count; i++)
                {
                    if (SettingJsonManager.processType == this.mainModel.subOb[i].processType)
                    {
                        this.mainModel.subOb[i].stateColor = Brushes.Black;
                        this.mainModel.subOb[i].stateFontColor = Brushes.White;
                    }
                }

                CountManager.SendMsg(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
                CountManager.SendMsg(SettingJsonManager.mercName, "0"); //





                SettingJsonManager.consumersumcount = 0;
                SettingJsonManager.count = 0;
                SettingJsonManager.saveDate = "";
                SettingJsonManager.pplanKey = 0;
                SettingJsonManager.pworkkey = "";
                SettingJsonManager.SaveJsonFile();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopResultEnd 저장실패 !!");
                msg.ShowDialog();
            }

            return returnData.결과;
        }


        private int PopResultEndMember()
        {
            int ChkAED = 3;
            int workcount = 0; //생산수량 담기

            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
                "[PopResultEndMember]",
                new string[] { "@작업일자", "@오더Key", "@ChkAED", "@실수요자생산Key", "@작업량", "@불량" },
                new object[] {
                DateTimeManager.GetSelectedDateStr(),      // 실수요자 전일 작업량 수정시 삭제건 해결을 위한 수정: DateTime.Now -> selectedDate
                this.inputModel.orderKey,
                ChkAED,
                this.inputModel.workMemberKey,
                workcount+"",
                0,
                 }, 1
              );

            if (returnData.결과 == 1)
            {
                CountManager.SendMsg(SettingJsonManager.mercName, "E"); //현황판으로 생산종료 송신
            }
            return returnData.결과;
        }
    }
}
