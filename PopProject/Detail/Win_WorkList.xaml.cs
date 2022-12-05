using Newtonsoft.Json.Linq;
using PopProject.input;
using PopProject.MessageBox;
using PopProject.UserControl_;
using PopProject.Util;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PopProject.Detail
{
    /// <summary>
    /// Win_WorkList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_WorkList : Window
    {
        private Point scrollMousePoint = new Point();
        private MainModel mainModel;
        private double hOff = 1;
        private double v0ff = 1;
        private float sumPworkCount = 0;
        private float sumPerrCount = 0;
        private double percent = 0.0;
        ObservableCollection<InputModel> ob ;
        public Win_WorkList(MainModel mainModel)
        {
            InitializeComponent();
            this.mainModel = mainModel;
            PopResultSearch();
        }

        //작업 결과 리스트 조회
        private void PopResultSearch()
        {
            sumPerrCount = 0;
            sumPworkCount = 0;
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
            Properties.Settings.Default.serverIndex,
            "[PopResultSearchProdID]",
            new string[] { "@prodID" },
            new object[] { mainModel.prodID },
            0
            );

            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    ob = new ObservableCollection<InputModel>();

                    tbEmpty.Visibility = Data_JArray.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                    foreach (JObject result in Data_JArray)
                    {
                        if(int.Parse(result["ProcessType"].ToString()) == Properties.Settings.Default.processType)
                        {
                            InputModel listitem = new InputModel();
                            listitem.pworkDate = DateTime.Parse(result["pworkDate"].ToString());
                            listitem.pworkCount = float.Parse(result["pworkCount"].ToString());
                            listitem.pworkKey = int.Parse(result["pworkKey"].ToString());
                            listitem.sDatetime = result["sDatetime"].ToString();

                            if (!result["eDatetime"].ToString().Equals(""))
                            {
                                listitem.eDatetime = result["eDatetime"].ToString();
                            }

                            listitem.prodID = result["prodID"].ToString();
                            listitem.orderKey = result["orderKey"].ToString();
                            listitem.pplanKey = int.Parse(result["pplanKey"].ToString());
                            listitem.ProcessType = int.Parse(result["ProcessType"].ToString());
                            listitem.perrCount = float.Parse(result["perrCount"].ToString());
                            listitem.pMemo = result["pMemo"].ToString();
                            listitem.errorModelOb = popErrorListSearch(listitem);

                            if (!listitem.eDatetime.Equals(""))
                            {
                                errorModel item2 = new errorModel
                                {
                                    memo = "작업완료",
                                    perrCount = listitem.pworkCount,
                                    updateTime = DateTime.Parse(listitem.eDatetime),
                                    fontColor = Brushes.Red
                                };
                                listitem.errorModelOb.Add(item2);
                            }

                            if (listitem.errorModelOb != null)
                            {
                                foreach(errorModel item in listitem.errorModelOb)
                                {
                                    listitem.perrCount += item.perrCount;
                                }
                            }

                            sumPworkCount += listitem.pworkCount;
                            ob.Add(listitem);

                        }
                    }


                    tbTotalWCount.Text = sumPworkCount.ToString();
                    tbTotalPCount.Text = sumPerrCount.ToString();

                    if (sumPerrCount > 0)
                    {
                        percent = sumPerrCount / (sumPworkCount + sumPerrCount) * 100;
                        tbPercent.Text = String.Format("{0,3:N2}", percent) + "%";
                    }

                    else
                    {
                        tbPercent.Text = "0%";
                    }
                    
                    ic.ItemsSource = ob;
                    sortList();
                }
            }

            else
            {
                MessageBox_Dialog dialog = new MessageBox_Dialog("PopResultSearchProdID 조회 오류!!");
                _ = dialog.ShowDialog();
            }
        }

        private void sortList()
        {
            SortDescription sort = new SortDescription("sDatetime", ListSortDirection.Descending);
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(ob);
            collectionView.Filter = null;
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(sort);
            collectionView.Refresh();

        }

        private ObservableCollection<errorModel> popErrorListSearch(InputModel inputModel)
        {
            ObservableCollection<errorModel> ob = new ObservableCollection<errorModel>();
            ServiceModel.ServiceModelClean(Properties.Settings.Default.serverIndex);
            ReturnModel returnData = ServiceModel.GetData(
                Properties.Settings.Default.serverIndex,
                "[PopErrorListSearch]",
                new string[] { "@pworkKey", "@ProcessType" },
                new object[] { inputModel.pworkKey, Properties.Settings.Default.processType },
                0
                );
            if (returnData.결과 == 1)
            {
                JArray Data_JArray = JArray.Parse(returnData.리턴값);
                if (Data_JArray != null)
                {
                    errorModel startitem = new errorModel();
                    startitem.updateTime = DateTime.Parse(inputModel.sDatetime);
                    startitem.memo = "작업시작";
                    startitem.fontColor = Brushes.Green;
                    startitem.fontVisible = Visibility.Collapsed;
                    ob.Add(startitem);

                    foreach (JObject result in Data_JArray)
                    {
                        SubData(inputModel, ob, result);
                    }
                }

                else
                {
                    return null;
                }
            }

            return ob;
        }

        private void SubData(InputModel inputModel, ObservableCollection<errorModel> ob, JObject result)
        {
            errorModel item = new errorModel();
            item.InputModel = inputModel;
            item.perrKey = result["perrKey"].ToString();
            item.memo = result["Memo"].ToString();
            item.perrCount = int.Parse(result["perrorCount"].ToString());
            item.updateTime = DateTime.Parse(result["updateTime"].ToString());
            item.fontColor = Brushes.Black;
            sumPerrCount += item.perrCount;
            ob.Add(item);
        }

        private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            sv.ReleaseMouseCapture();
        }

        private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(v0ff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }

        private void Sv_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            v0ff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }

        // 최근작업 화면 종료
        private void Win_close(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // 화면 드래그
        private void Win_Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
