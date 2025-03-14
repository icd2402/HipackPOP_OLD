using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject.Detail
{
   public class DetailModel : ViewModelBase
    {
        //WDScore WDSize DesignNO handChk HandPosition PackCount PackType LoadHeight LoadType PalletType PalletSize   LocationName

        private string _LocationName="";
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

        private string _PalletSize;
        public string PalletSize
        {
            get
            {
                return _PalletSize;
            }
            set
            {
                _PalletSize = value;
                onPropertyChanged("PalletSize");
            }
        }

        private string _UserMemo;
        public string UserMemo
        {
            get
            {
                return _UserMemo;
            }
            set
            {
                _UserMemo = value;
                onPropertyChanged("UserMemo");
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

        private string _PalletType;
        public string PalletType
        {
            get
            {
                return _PalletType;
            }
            set
            {
                _PalletType = value;
                onPropertyChanged("PalletType");
            }
        }
        private string _prodID;
        public string prodID
        {
            get
            {
                return _prodID;
            }
            set
            {
                _prodID = value;
                onPropertyChanged("prodID");
            }
        }

        private string _LoadType;
        public string LoadType
        {
            get
            {
                return _LoadType;
            }
            set
            {
                _LoadType = value;
                onPropertyChanged("LoadType");
            }
        }

        private string _LoadHeight;
        public string LoadHeight
        {
            get
            {
                return _LoadHeight;
            }
            set
            {
                _LoadHeight = value;
                onPropertyChanged("LoadHeight");
            }
        }

        private string _HandPosition;
        public string HandPosition
        {
            get
            {
                return _HandPosition;
            }
            set
            {
                _HandPosition = value;
                onPropertyChanged("HandPosition");
            }
        }

        private string _DesignNO;
        public string DesignNO
        {
            get
            {
                return _DesignNO;
            }
            set
            {
                _DesignNO = value;
                onPropertyChanged("DesignNO");
            }
        }

        private string _WDSize;
        public string WDSize
        {
            get
            {
                return _WDSize;
            }
            set
            {
                _WDSize = value;
                onPropertyChanged("WDSize");
            }
        }

        private string _WDScore;
        public string WDScore
        {
            get
            {
                return _WDScore;
            }
            set
            {
                _WDScore = value;
                onPropertyChanged("WDScore");
            }
        }

        private string _metrialName;
        public string metrialName {
            get
            {
                return _metrialName;
            }
            set
            {
                _metrialName = value;
                onPropertyChanged("metrialName");
            }
        }

        private int _metType;
        public int metType {
            get
            {
                return _metType;
            }

            set
            {
                _metType = value;
                onPropertyChanged("metType");
            }
        }
        private int _mL;
        public int mL {
            get
            {
                return _mL;
            }
            set
            {
                _mL = value;
                onPropertyChanged("mL");
            }
        }
        private int _mW;
        public int mW {
            get
            {
                return _mW;
            }
            set
            {
                _mW = value;
                onPropertyChanged("mW");
            }
        }

        private int _orderKey;
        public int orderKey {
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

        private int _SCMorderKey;
        public int SCMorderKey
        {
            get
            {
                return _SCMorderKey;
            }

            set
            {
                _SCMorderKey = value;
                onPropertyChanged("SCMorderKey");
            }
        }

        private string _PrintNo;
        public string PrintNo {
            get
            {
                return _PrintNo;
            }
            set
            {
                _PrintNo = value;
                onPropertyChanged("PrintNo");
            }
        }
        private string _Printposition;
        public string Printposition {
            get
            {
                return _Printposition;
            }
            set
            {
                _Printposition = value;
                onPropertyChanged("Printposition");
            }
        }

        private string _ColorType;
        public string ColorType {
            get
            {
                return _ColorType;
            }
            set
            {
                _ColorType = value;
                onPropertyChanged("ColorType");
            }
        }
        private string _CoatingType;
        public string CoatingType {
            get
            {
                return _CoatingType;
            }
            set
            {
                _CoatingType = value;
                onPropertyChanged("CoatingType");
            }
        }
        private string _PushingNo;
        public string PushingNo {
            get
            {
                return _PushingNo;
            }
            set
            {
                _PushingNo = value;
                onPropertyChanged("PushingNo");
            }
        }

        private string _Pushingposition;
        public string Pushingposition {
            get
            {
                return _Pushingposition;
            }

            set
            {
                _Pushingposition = value;
                onPropertyChanged("Pushingposition");
            }
        }

        private string _ConnetingType;
        public string ConnetingType {
            get
            {
                return _ConnetingType;
            }
            set
            {
                _ConnetingType = value;
                onPropertyChanged("ConnetingType");
            }
        }
        private int _PackCount;
        public int PackCount {
            get
            {
                return _PackCount;
            }
            set
            {
                _PackCount = value;
                onPropertyChanged("PackCount");
            }
        }
        private string _PackType;

        public string PackType {

            get
            {
                return _PackType;
            }
            
            set
            {
                _PackType = value;
                onPropertyChanged("PackType");
            }
        }
        private string _loadposition;
        public string loadposition {
            get
            {
                return _loadposition;
            }

            set
            {
                _loadposition = value;
                onPropertyChanged("loadposition");
            }
        
        }

        private string _filmNo;
        public string filmNo {
            get
            {
                return _filmNo;
            }
            set
            {
                _filmNo = value;
                onPropertyChanged("filmNo");
            }
        }
        private string _handChk;
        public string handChk {
            get
            {
                return _handChk;
            }
            set
            {
                _handChk = value;
                onPropertyChanged("handChk");
            }
        }

        private string _jobMemo;
        public string jobMemo {
            get
            {
                return _jobMemo;
            }
            set
            {
                _jobMemo = value;
                onPropertyChanged("jobMemo");
            }
        }

        private string _SpecialMemo;
        public string SpecialMemo {
            get
            {
                return _SpecialMemo;
            }
            set
            {
                _SpecialMemo = value;
                onPropertyChanged("SpecialMemo");
            }
        
        }
    }

    public class IgSmModel: ViewModelBase
    {
        private float _metBJCount;
        public float metBJCount
        {
            get
            {
                return _metBJCount;
            }
            set
            {
                _metBJCount = value;
                onPropertyChanged("metBJCount");
            }
        }


        private float _metPreCount;
        public float metPreCount
        {
            get
            {
                return _metPreCount;
            }
            set
            {
                _metPreCount = value;
                onPropertyChanged("metPreCount");
            }
        }

        private string _paperGor;
        public string paperGor
        {
            get
            {
                return _paperGor;
            }
            set
            {
                _paperGor = value;
                onPropertyChanged("paperGor");
            }
        }

        private string _metBJLastString;
        public string metBJLastString
        {
            get
            {
                return _metBJLastString;
            }
            set
            {
                _metBJLastString = value;
                onPropertyChanged("_metBJLastString");
            }
        }
        private DateTime _metBJLast;
        public DateTime metBJLast
        {
            get
            {
                return _metBJLast;
            }
            set
            {
                _metBJLast = value;
                onPropertyChanged("metBJLast");
            }
        }

        private string _metBJDate;
        public string metBJDate
        {
            get
            {
                return _metBJDate;
            }
            set
            {
                _metBJDate = value;
                onPropertyChanged("metBJDate");
            }
        }

        private DateTime _metDate;
        public DateTime metDate {
            get
            {
                return _metDate;
            }
            set
            {
                _metDate = value;
                onPropertyChanged("metDate");
            }
        }

        private int _metType;
        public int metType {
            get
            {
                return _metType;
            }
            set
            {
                _metType = value;
                onPropertyChanged("metType");
            }
        }
        private string _metCountString;
        public string metCountString
        {
            get
            {
                return _metCountString;
            }
            set
            {
                _metCountString = value;
                onPropertyChanged("metCountString");
            }
        }


        private float _metCount;
        public float metCount {
            get
            {
                return _metCount;
            }
            set
            {
                _metCount = value;
                onPropertyChanged("metCount");
            }
        }
        //metPreCountString

        private float _SubscriptionBJCount;
        public float SubscriptionBJCount
        {
            get
            {
                return _SubscriptionBJCount;
            }
            set
            {
                _SubscriptionBJCount = value;
                onPropertyChanged("SubscriptionBJCount");
            }
        }

        private string _SubscriptionBJCountString;
        public string SubscriptionBJCountString
        {
            get
            {
                return _SubscriptionBJCountString;
            }
            set
            {
                _SubscriptionBJCountString = value;
                onPropertyChanged("SubscriptionBJCountString");
            }
        }

        private string _GigText;
        public string GigText
        {
            get
            {
                return _GigText;
            }
            set
            {
                _GigText = value;
                onPropertyChanged("GigText");
            }
        }

        private int _metrialBJKey;
        public int metrialBJKey
        {
            get
            {
                return _metrialBJKey;
            }
            set
            {
                _metrialBJKey = value;
                onPropertyChanged("metrialBJKey");
            }
        }

        private string _metPreCountString;
        public string metPreCountString
        {

            get
            {
                return _metPreCountString;
            }

            set
            {
                _metPreCountString = value;
                onPropertyChanged("metPreCountString");
            }
        }

        private string _BtVisibility;
        public string BtVisibility
        {

            get
            {
                return _BtVisibility;
            }

            set
            {
                _BtVisibility = value;
                onPropertyChanged("BtVisibility");
            }
        }

        private string _customSName;
        public string customSName {

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

        private string _metrialName;
        public string metrialName {
            get
            {
                return _metrialName;
            }

            set
            {
                _metrialName = value;
                onPropertyChanged("metrialName");
            }
        }
        private System.Windows.Media.Brush _fontColor;
        public System.Windows.Media.Brush fontColor {
            get
            {
                return _fontColor;
            }
            set
            {
                _fontColor = value;
                onPropertyChanged("fontColor");
            }

        }

        private int _wdjs;
        public int wdjs {

            get
            {
                return _wdjs;
            }

            set
            {
                _wdjs = value;
                onPropertyChanged("wdjs");
            }
        }


        private string _SCMServerIndex;
        public string SCMServerIndex
        {
            get
            {
                return _SCMServerIndex;
            }

            set
            {
                _SCMServerIndex = value;
                onPropertyChanged("SCMServerIndex");
            }
        }


        private string _SCMDBName;
        public string SCMDBName
        {
            get
            {
                return _SCMDBName;
            }

            set
            {
                _SCMDBName = value;
                onPropertyChanged("SCMDBName");
            }
        }

        private double _MetCanvas;
        public double MetCanvas
        {
            get
            {
                return _MetCanvas;
            }

            set
            {
                _MetCanvas = value;
                onPropertyChanged("MetCanvas");
            }
        }
    }
}
