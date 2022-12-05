using FrameWork.Base;
using PopProject.input;
using PopProject.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace PopProject.Input.update
{
    /// <summary>
    /// Win_Update.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Update : Window
    {
        UserControlBase ucb = new UserControlBase();

        int beforePerrorCount = 0;
        public Win_Update(errorModel errorModel, List<String> errorList)
        {
            InitializeComponent();
            this.DataContext = errorModel;
            this.combo.Text = errorModel.memo;
            this.beforePerrorCount = errorModel.perrCount;
        }

        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if(popErrorUpdate() == 1)
            {
                this.Close();
            }
            else
            {
                MessageBox_Dialog messageBox_Dialog = new MessageBox_Dialog("저장실패");
                messageBox_Dialog.ShowDialog();
            }
            e.Handled = false;
        }

        private int popErrorUpdate()
        {
            errorModel context = (errorModel)this.DataContext;
            object[] objParams = new object[]
         {
              "perrKey",
              "perrorCount",
              "beforePerrorCount",
              "pworkKey",
              "Memo",
              "작업일자",
              "오더Key",
              "arr작업Key",
              "arr공정구분",
              "arr비고"


         };
            object[] objValues = new object[]
            {

                context.perrKey,
                context.perrCount,
                this.beforePerrorCount,
                context.InputModel.pworkKey,
                context.memo,
                DateTime.Now.ToString("yyyy-MM-dd"),
                context.InputModel.orderKey,
                context.InputModel.pplanKey,
                context.InputModel.ProcessType,
                context.InputModel.pMemo
            };
            object[] objInfo = new object[]
            {
                "[PopErrorUpdate]"
                ,objParams
                ,objValues

            };
            DataSet ds = ucb.GF_Get_DataSp_To_DataSet(objInfo);

            String xmlstr = ucb.GF_Get_DataSp_To_Xml(objInfo);
            Console.WriteLine("PopErrorUpdate" + xmlstr);

            if (ds.Tables[0].Rows.Count != 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image image = (Image)sender;
                errorModel context = (errorModel)image.DataContext;
                context.perrCount -= 1;
            }
           
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType().Name.Equals("Image"))
            {
                Image image = (Image)sender;
                errorModel context = (errorModel)image.DataContext;
                context.perrCount += 1;
            }
        }

        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
