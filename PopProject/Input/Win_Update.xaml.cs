
using PopProject.input;
using PopProject.MessageBox;
using PopProject.Util;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PopProject.Input
{
    /// <summary>
    /// Win_Update.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Update : Window
    {
        // private errorModel errorModel;
        private readonly float beforePerrorCount = 0;
        public Win_Update(errorModel errorModel, ObservableCollection<string> errorOb)
        {
            InitializeComponent();
            DataContext = errorModel;
            combo.ItemsSource = errorOb;
            combo.Text = errorModel.memo;
            beforePerrorCount = errorModel.perrCount;
        }

        //드래그
        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //불량 수량 저장
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("combo" + combo.SelectedItem);
            errorModel context = (errorModel)DataContext;
            try
            {
                context.memo = combo.SelectedItem == null ? "" : combo.SelectedItem.ToString();
            }
            catch (Exception)
            {
                context.memo = "";
            }

            if (PopErrorUpdate() == 1)
            {
                Close();
            }
            else
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("불량 수정 실패");
                _ = dialog.ShowDialog();
            }
            e.Handled = false;
        }

        //불량 수량 저장
        private int PopErrorUpdate()
        {
            errorModel context = (errorModel)DataContext;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopErrorUpdate]",
                new string[] {
                "@perrKey",
                "@perrorCount",
                "@beforePerrorCount",
                "@pworkKey",
                "@Memo",
                "@작업일자",
                "@오더Key",
                "@arr작업Key",
                "@arr공정구분",
                "@arr비고",
                },

                new object[] {
                context.perrKey,
                context.perrCount,
                beforePerrorCount,
                context.InputModel.pworkKey,
                context.memo,
                DateTime.Now.ToString("yyyy-MM-dd"),
                context.InputModel.orderKey,
                context.InputModel.pplanKey,
                context.InputModel.ProcessType,
                context.InputModel.pMemo,
                },
                1
                );

            return returnData.결과 == 1 ? 1 : 0;
        }

        //닫기
        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // - 버튼
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image img = (Image)sender;
                errorModel context = (errorModel)img.DataContext;
                context.perrCount -= 1;
            }
        }

        // + 버튼
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image img = (Image)sender;
                errorModel context = (errorModel)img.DataContext;
                context.perrCount += 1;
            }
        }
    }
}
