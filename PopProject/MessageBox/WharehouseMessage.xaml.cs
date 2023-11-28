using PopProject.Detail;
using PopProject.UserControl_;
using PopProject.Util;
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

namespace PopProject.MessageBox
{
    /// <summary>
    /// WharehouseMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WharehouseMessage : Window
    {
        MainModel mainModel;
        Wharehouse_LocationModel model;
        public WharehouseMessage(MainModel mainModel, Wharehouse_LocationModel model)
        {
            InitializeComponent();
            this.mainModel = mainModel;
            this.model = model;

            location.Text = model.LocationName;


        }

        private void Messagebox_buttonClose(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Messagebox_button(object sender, MouseButtonEventArgs e)
        {
            if (mainModel.sumPworkCount == 0)
            {
                MessageBox_Dialog di = new MessageBox_Dialog("생산 되지 않았습니다.");
                di.ShowDialog();

                this.Close();
            }
            else
            {
                ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                ReturnModel returnData = ServiceModel.GetData(

                   SettingJsonManager.serverIndex,
                  "[PopStockLocation]",
                   new string[] { "@orderKey", "@LocationName" },
                   new object[] { mainModel.orderKey, model.LocationName },

                   1
                   );

                if (returnData.결과 == 1)
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("등록 성공");
                    msg.ShowDialog();

                    this.Close();
                }
                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("등록 실패");
                    msg.ShowDialog();

                    this.Close();
                }
            }
           
        }
    }
}
