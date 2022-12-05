using PopProject.Util;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using System.Collections.ObjectModel;
using PopProject.UserControl_;

namespace PopProject.MerList
{
    /// <summary>
    /// MercList.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MercList : Window
    {
        private ObservableCollection<MerModel> list;
        public MercList()
        {
            InitializeComponent();
            tbMercName.Text = Properties.Settings.Default.mercName;
            PopSearchMerc();
        }

        private void PopSearchMerc()
        {
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopSearchMerc]",
                new string [] {},
                new object [] {},
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    list = new ObservableCollection<MerModel>();
                    foreach (JObject result in Data_JArray)
                    {
                        MerModel model = new MerModel
                        {
                            mercName = result["mercName"].ToString(),
                            ProcessType = result["ProcessType"].ToString(),
                            CountType = result["CountType"].ToString(),
                            TypeIndex = result["TypeIndex"].ToString()
                        };
                        if (!model.mercName.Equals(""))
                        {
                            list.Add(model);
                        }
                    }
                    lb.ItemsSource = list;
                }
            }

            else
            {
                MessageBox_Dialog msg = new MessageBox_Dialog("PopSearchMerc 조회 오류 !!");
                _ = msg.ShowDialog();
            }
        }

        private void Lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.mercName = list[lb.SelectedIndex].mercName;
            Properties.Settings.Default.processType = int.Parse(list[lb.SelectedIndex].ProcessType);
            Properties.Settings.Default.TypeIndex = int.Parse(list[lb.SelectedIndex].TypeIndex);
            Properties.Settings.Default.CountType = int.Parse(list[lb.SelectedIndex].CountType);
            Properties.Settings.Default.Save();
            Main.production = null;
            Close();
        }
    }
}
