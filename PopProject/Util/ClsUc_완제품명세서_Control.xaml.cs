using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PopProject.Util
{
    /// <summary>
    /// ClsUc_완제품명세서_Control.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClsUc_완제품명세서_Control : UserControl
    {
        public ProductReport ViewModel = new ProductReport();
        String printItem = Properties.Settings.Default.printItem;

        public ClsUc_완제품명세서_Control()
        {
            
            InitializeComponent();
            barcodeImage.Visibility = System.Windows.Visibility.Hidden;
            
        }

        public void Set_BarCode()
        {
                
            this.DataContext = ViewModel;
            if (Convert.ToInt32(ViewModel.pStockKey) > 0)
            {

                barcodeImage.Visibility = System.Windows.Visibility.Visible;
               // BarCode barcodeFont = new BarCode();
               // barcodeFont.SymbologyType = SymbologyType.Code128c;
              //  barcodeFont.X = 3;
               // barcodeFont.BarCodeWidth = 450;
              //  barcodeFont.BarCodeHeight = 150;
              //  barcodeFont.CodeText = ViewModel.pStockKey;
              //  System.IO.Stream stream = new System.IO.MemoryStream();
              //  barcodeFont.drawBarcode(stream);
                
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                //imageSource.StreamSource = stream;
                imageSource.EndInit();
                barcodeImage.Source = imageSource;
                 
            }
        }

        public class ProductReport
        {

            public event PropertyChangedEventHandler PropertyChanged;
            public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

            protected virtual void onPropertyChanged(string propName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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

            private DateTime _pworkDate;
            public DateTime pworkDate
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

            private int _resultCount;
            public int resultCount
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
