using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KeepDynamic.Barcode.Generator;

namespace PopProject.Util
{
    /// <summary>
    /// ClsUc_완제품명세서_Control내포.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClsUc_완제품명세서_Control내포 : UserControl
    {
        public ProductReport ViewModel = new ProductReport();
        //String printItem = SettingJsonManager.printItem;
        public ClsUc_완제품명세서_Control내포()
        {
            InitializeComponent();


        }
        public void Set_Context()
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
            //수주키
            private string _prodColor;
            public string prodColor
            {
                get
                {
                    return _prodColor;
                }
                set
                {
                    _prodColor = value;
                    onPropertyChanged("prodColor");
                }
            }
            //수주일
            private string _orderDate;
            public string orderDate
            {
                get
                {
                    return _orderDate;
                }
                set
                {
                    _orderDate = value;
                    onPropertyChanged("orderDate");
                }
            }
            //수주키
            private string _orderKey;
            public string orderKey
            {
                get
                {
                    return _orderKey;
                }
                set
                {
                    _orderKey = value;
                    onPropertyChanged("orderKey");
                }
            }
            //거래처
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

            //상호
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

            //품명
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

            //규격
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


            //생산일자
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

            //생산수량
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
            //납기
            private string _orderLast;
            public string orderLast
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

            //비고
            private string _deliverMemo;
            public string deliverMemo
            {
                get
                {
                    return _deliverMemo;
                }
                set
                {
                    _deliverMemo = value;
                    onPropertyChanged("deliverMemo");
                }
            }

        }

    }
}
