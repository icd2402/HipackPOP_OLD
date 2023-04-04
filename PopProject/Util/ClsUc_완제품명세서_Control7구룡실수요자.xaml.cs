using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace PopProject.Util
{
    /// <summary>
    /// ClsUc_완제품명세서_Control.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClsUc_완제품명세서_Control7구룡실수요자 : UserControl
    {
        public ProductReport ViewModel = new ProductReport();
        String printItem = Properties.Settings.Default.printItem;
        public ClsUc_완제품명세서_Control7구룡실수요자()
        {
            InitializeComponent();
            dock2.Visibility = System.Windows.Visibility.Collapsed;
            dock1.Visibility = System.Windows.Visibility.Visible;
            
        }

        public void Set_BarCode()
        {
            this.DataContext = ViewModel;

        }

        public class ProductReport
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

            protected virtual void onPropertyChanged(string propName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }

            private string _membName;
            public string membName
            {
                get
                {
                    return _membName;
                }
                set
                {
                    _membName = value;
                    onPropertyChanged("membName");
                }
            }

            private string _adress;
            public string adress
            {
                get
                {
                    return _adress;
                }
                set
                {
                    _adress = value;
                    onPropertyChanged("adress");
                }
            }
            private string _PhonNo;
            public string PhonNo
            {
                get
                {
                    return _PhonNo;
                }
                set
                {
                    _PhonNo = value;
                    onPropertyChanged("PhonNo");
                }
            }

            private string _LocationName;
            public string LocationName
            {
                get
                {
                    return _LocationName;
                }
                set
                {
                    _LocationName = value;
                    onPropertyChanged("LocationName");
                }
            }

            private string _vendorName;
            public string vendorName
            {
                get
                {
                    return _vendorName;
                }
                set
                {
                    _vendorName = value;
                    onPropertyChanged("vendorName");
                }
            }

            private string _customSName;
            public string customSName
            {
                get
                {
                    return _customSName;
                }
                set
                {
                    _customSName = value;
                    onPropertyChanged("customSName");
                }
            }
            private string _loadHeight;
            public string loadHeight
            {
                get
                {
                    return _loadHeight;
                }
                set
                {
                    _loadHeight = value;
                    onPropertyChanged("loadHeight");
                }
            }

            private string _loadType;
            public string loadType
            {
                get
                {
                    return _loadType;
                }
                set
                {
                    _loadType = value;
                    onPropertyChanged("loadType");
                }
            }
            private string _corpName;
            public string corpName
            {
                get
                {
                    return _corpName;
                }

                set
                {
                    _corpName = value;
                    onPropertyChanged("corpName");
                }
            
            }

            private string _prodName;
            public string prodName
            {
                get
                {
                    return _prodName;
                }
                set
                {
                    _prodName = value;
                    onPropertyChanged("prodName");
                }
            }

            private string _orderLast1;
            public string orderLast1
            {
                get
                {
                    return _orderLast1;
                }
                set
                {
                    _orderLast1 = value;
                    onPropertyChanged("orderLast1");
                }
            }


            private DateTime _orderLast;
            public DateTime orderLast
            {
                get
                {
                    return _orderLast;
                }
                set
                {
                    _orderLast = value;
                    onPropertyChanged("orderLast");
                }
            }

            private string _pSize;
            public string pSize
            {
                get
                {
                    return _pSize;
                }
                set
                {
                    _pSize = value;
                    onPropertyChanged("pSize");
                }
            }

            private string _pMemo;
            public string pMemo
            {
                get
                {
                    return _pMemo;
                }
                set
                {
                    _pMemo = value;
                    onPropertyChanged("pMemo");
                }
            }

            private string _pworkDate;
            public string pworkDate
            {
                get
                {
                    return _pworkDate;
                }
                set
                {
                    _pworkDate = value;
                    onPropertyChanged("pworkDate");
                }
            }

            private string _resultCount;
            public string resultCount
            {
                get
                {
                    return _resultCount;
                }
                set
                {
                    _resultCount = value;
                    onPropertyChanged("resultCount");
                }
            }
            

         private string _loadCount;
            public string loadCount
            {
                get
                {
                    return _loadCount;
                }
                set
                {
                    _loadCount = value;
                    onPropertyChanged("loadCount");
                }
            }
            private string _pStockKey;
            public string pStockKey
            {
                get
                {
                    return _pStockKey;
                }
                set
                {
                    _pStockKey = value;
                    onPropertyChanged("pStockKey");
                }
            }

            private string _MycustomSName;
            public string MycustomSName
            {
                
                get
                {
                    return _MycustomSName;
                }

                set
                {
                    _MycustomSName = value;
                    onPropertyChanged("MycustomSName");
                }
            }
        }
    }
}
