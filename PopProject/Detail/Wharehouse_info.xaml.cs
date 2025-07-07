using Newtonsoft.Json.Linq;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PopProject.Detail
{
    /// <summary>
    /// Wharehouse_info.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Wharehouse_info : Window
    {
        MainModel mainModel;
        ObservableCollection<WhareHouseModel> WhareHouselist;
        ObservableCollection<Wharehouse_LocationModel> wharehouse_LocationsList;
        public Wharehouse_info(MainModel mainModel, string locationName)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            this.mainModel = mainModel;
            customName.Text = mainModel.customSName;
            prodName.Text = mainModel.prodName;
            pworkCount.Text = mainModel.sumPworkCount.ToString();

            LocationName.Text = locationName;
            Get_DataInformation();

        }

        private void Get_DataInformation()
        {
            ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(

               SettingJsonManager.serverIndex,
              "[spSel021Wharehouse_Base]",
               new string[] { "@창고구분" },
               new object[] { "9" },

               0
               );

            if (returnData.결과 == 1)
            {
                JArray Data_Jarry = JArray.Parse(returnData.리턴값);
                WhareHouseModel model;
                WhareHouselist = new ObservableCollection<WhareHouseModel>();
                if (Data_Jarry != null)
                {
                    foreach (JObject item in Data_Jarry)
                    {
                        model = new WhareHouseModel
                        {
                            WharehouseID = item["WharehouseID"].ToString(),
                            TypeName = item["TypeName"].ToString(),
                            StartNo = item["StartNo"].ToString(),
                            EndNo = item["EndNo"].ToString(),
                            UpUserID = item["UpUserID"].ToString(),
                            UpUserName = item["UpUserName"].ToString(),
                            UpDatetime = item["UpDatetime"].ToString(),
                            Memo = item["Memo"].ToString(),
                            WharehouseType = item["WharehouseType"].ToString()
                        };
                        WhareHouselist.Add(model);

                    }
                }

                ServiceModel.ServiceModelClean(SettingJsonManager.serverIndex);
                returnData = ServiceModel.GetData(

                   SettingJsonManager.serverIndex,
                  "[spSel570Wharehouseinfo]",
                   new string[] { "@창고구분" },
                   new object[] { "9" },

                   0
                   );

                if (returnData.결과 == 1)
                {
                    Data_Jarry = JArray.Parse(returnData.리턴값);
                    wharehouse_LocationsList = new ObservableCollection<Wharehouse_LocationModel>();
                    if (Data_Jarry != null)
                    {
                        foreach (JObject item in Data_Jarry)
                        {
                            Wharehouse_LocationModel lmodel = new Wharehouse_LocationModel()
                            {
                                customID = item["customID"].ToString(),
                                customSName = item["customSName"].ToString(),
                                prodID = item["prodID"].ToString(),
                                prodName = item["prodName"].ToString(),
                                JGCount = item["JGCount"].ToString(),
                                LocationName = item["LocationName"].ToString()


                            };
                            wharehouse_LocationsList.Add(lmodel);


                        }

                    }

                    foreach (WhareHouseModel tempItem in WhareHouselist)
                    {
                        for (int i = int.Parse(tempItem.StartNo); i <= int.Parse(tempItem.EndNo); i++)
                        {
                            List<Wharehouse_LocationModel> temp2Item = wharehouse_LocationsList.Where(x => x.LocationName.Equals(String.Format("{0}-{1}", tempItem.TypeName, i))).ToList();

                            Wharehouse_LocationModel setedItem;
                            if (temp2Item.Count == 0)
                            {
                                setedItem = new Wharehouse_LocationModel
                                {
                                    LocationType = 0,
                                    LocationName = string.Format("{0}-{1}", tempItem.TypeName, i),
                                    IsSelectedStyle = 0

                                };
                            }
                            else
                            {
                                setedItem = new Wharehouse_LocationModel
                                {
                                    LocationType = 1,
                                    customID = temp2Item.FirstOrDefault().customID,
                                    customSName = temp2Item.FirstOrDefault().customSName,
                                    prodID = temp2Item.FirstOrDefault().prodID,
                                    prodName = temp2Item.FirstOrDefault().prodName,
                                    JGCount = temp2Item.FirstOrDefault().JGCount,
                                    LocationName = temp2Item.FirstOrDefault().LocationName,
                                    IsSelectedStyle = 0
                                };
                            }

                            if (setedItem.LocationName.Length - setedItem.LocationName.Replace("-", "").Length > 1)
                            {
                                setedItem.LocationNameDisplay = setedItem.LocationName.Substring(0, setedItem.LocationName.LastIndexOf("-"));
                                setedItem.LocationNameDisplay += Environment.NewLine + i.ToString("D2");
                            }
                            else
                            {
                                setedItem.LocationNameDisplay = setedItem.LocationName;
                            }

                            tempItem.적재구역_ItemList.Add(setedItem);
                        }

                    }
                    적재창고_ItemList.ItemsSource = WhareHouselist;
                }
            }

        }



        private void exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void drag_move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void 구역Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {


        }

        private void mouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            Border bd = (Border)sender;
            if (sender.GetType().Name.Equals("Border"))
            {
                Wharehouse_LocationModel model = (Wharehouse_LocationModel)bd.DataContext;

                if (model.LocationType == 0)
                {
                    WharehouseMessage whmsg = new WharehouseMessage(mainModel, model);
                    whmsg.ShowDialog();
                }
                else
                {
                    MessageBox_Dialog msg = new MessageBox_Dialog("적재 불가 구역 입니다.");
                    msg.ShowDialog();
                }
                Get_DataInformation();
                //Tooltip_Visible(model);
            }
        }

        private void Tooltip_Visible(Wharehouse_LocationModel model)
        {
            if (model.LocationType == 1)
            {
                model.IsSelectedStyle = 1;
            }
        }
    }
}
