using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KeepDynamic.Barcode.Generator;


namespace PopProject.Util
{
    /// <summary>
    /// ClsUc_완제품명세서_Control.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClsUc_완제품명세서_Control6구룡 : UserControl
    {
        public ProductReport ViewModel = new ProductReport();
        String printItem = SettingJsonManager.printItem;
        public ClsUc_완제품명세서_Control6구룡()
        {
            InitializeComponent();

            dock2.Visibility = System.Windows.Visibility.Collapsed;
            dock1.Visibility = System.Windows.Visibility.Visible;
        }

        public void Set_BarCode(string id)
        {
            this.DataContext = ViewModel;
            // 바코드 이미지를 표시할 Image 컨트롤을 보이도록 설정
            barcodeImage.Visibility = System.Windows.Visibility.Visible;
            // BarCode 객체 생성
            BarCode barcodeFont = new BarCode();
            // 바코드 심볼러지 타입 설정 (여기서는 CODE128C)
            barcodeFont.SymbologyType = SymbologyType.Code128c;
            // 바코드 X(너비 비율) 설정
            barcodeFont.X = 3;
            // 바코드 너비와 높이 설정
            barcodeFont.BarCodeWidth = 140;
            barcodeFont.BarCodeHeight = 35;
            // 바코드에 인코딩할 텍스트 설정
            barcodeFont.CodeText = id.PadLeft(10, '0');
            // 바코드 이미지를 그릴 스트림 생성
            System.IO.Stream stream = new System.IO.MemoryStream();
            // 바코드를 스트림에 그리기
            barcodeFont.drawBarcode(stream);
            // 스트림에서 이미지 소스 생성
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = stream;
            imageSource.EndInit();

            // 이미지 소스를 Image 컨트롤에 설정하여 바코드 이미지 표시
            barcodeImage.Source = imageSource;
        }

        public class ProductReport
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

            protected virtual void onPropertyChanged(string propName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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

            private float _pplanCount;
            public float pplanCount
            {
                get
                {
                    return _pplanCount;
                }
                set
                {
                    _pplanCount = value;
                    onPropertyChanged("pplanCount");
                }
            }
        }
    }
}
