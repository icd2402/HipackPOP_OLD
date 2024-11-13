using PopProject.Util;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using System.Collections.ObjectModel;
using PopProject.UserControl_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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
            tbMercName.Text = SettingJsonManager.mercName;
            PopSearchMerc();
        }

        private void PopSearchMerc()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                SettingJsonManager.serverIndex,
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
                            //Console.WriteLine("설비명 : " + model.mercName);
                            list.Add(model);
                        }
                    }

                    if (SettingJsonManager.corp_name.Equals("오주포장") || SettingJsonManager.corp_name.Equals("오주포장테스트"))
                    {
                        // 해당 순서로 정렬
                        List<string> customOrder = new List<string>
                        {
                            "후렉소1호기",
                            "인라인2호기",
                            "인라인3호기",
                            "베가",
                            "1공장수동철",
                            "1공장1합철",
                            "1공장2합철",
                            "2공장인쇄기",
                            "2공장수동철",
                            "2공장톰슨기",
                            "2공장(세미)풀접합",
                            "2공장로터리",
                            "2공장재단기",
                            "2공장합지기",
                            "2공장탈지",
                            "2공장밴딩",
                            "2공장원단",
                            "2공장(손)조립",
                            "2공장랩핑",
                            "2공장P교체",
                            "원단 P교체 명세서"
                        };

                        list = new ObservableCollection<MerModel>(list.OrderBy(item => customOrder.IndexOf(item.mercName)).ToList());

                        lb.ItemsSource = list;

                    }
                    else
                    {
                        lb.ItemsSource = list;
                    }

                    //lb.ItemsSource = list;

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
            SettingJsonManager.mercName = list[lb.SelectedIndex].mercName;
            SettingJsonManager.processType = int.Parse(list[lb.SelectedIndex].ProcessType);
            SettingJsonManager.TypeIndex = int.Parse(list[lb.SelectedIndex].TypeIndex);
            SettingJsonManager.CountType = int.Parse(list[lb.SelectedIndex].CountType);
            SettingJsonManager.SaveJsonFile();
            Main.production = null;
            Close();
        }
    }
}
