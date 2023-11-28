using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PopProject.input;
using PopProject.KeyBoard;
using PopProject.Util;
using PopProject.Detail;
using PopProject.MessageBox;
using PopProject.UserControl_;

namespace PopProject.Detail
{
    /// <summary>
    /// win_GigUpdate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_GigUpdate : Window
    {
        private readonly int metrialBJKey;
        private readonly string scmServerIndex;
        private readonly string scmDBName;


        public Win_GigUpdate()
        {
            InitializeComponent();
        }

        public Win_GigUpdate(int bjKey, string serverIndex, string dbname)
        {
            InitializeComponent();

            this.metrialBJKey = bjKey;
            this.scmServerIndex = serverIndex;
            this.scmDBName = dbname;
        }


        //저장 버튼
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            string count = "";
            count = Count.Text; //입력수량


            string destSCMServerIndex = "";
            string destSCMDBName = "";

            if (string.IsNullOrEmpty(scmServerIndex))
            {
                destSCMServerIndex = SettingJsonManager.serverIndex;
            }
            else
            {
                destSCMServerIndex = scmServerIndex;
                destSCMDBName = scmDBName;
            }


            ServiceModel.ServiceModelClean(destSCMServerIndex, destSCMDBName);
            ReturnModel returnData = ServiceModel.GetData(
                destSCMServerIndex,
                "[PopSubscriptionBJCountUpdate]",
                new string[] { "@metrialBJKey", "@SubscriptionBJCount" },
                new object[] { this.metrialBJKey, count

                 }, 1
              );

            if (returnData.결과 == 1)
            {
                Close();
            }
            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("가입고 수량 입력 오류 , 알림");
                _ = msg.ShowDialog();
            }
        }

        //수량 입력
        private void Count_buttondown(object sender, MouseButtonEventArgs e)
        {

            TextBox context = (TextBox)sender;

            Win_KeyBoard win_KeyBoard = new Win_KeyBoard(context)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = 300
            };

            _ = win_KeyBoard.ShowDialog(); //키보드창 팝업
            win_KeyBoard = null;
        }

        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //닫기 버튼
        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

    }
}
