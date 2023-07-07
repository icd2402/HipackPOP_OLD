using PopProject.input;
using System;
using System.Collections.ObjectModel;

namespace PopProject.Consumer_
{
    public class ConsumerModel : ViewModelBase
    {
        private string _memo = "";
        public string memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = value;
                onPropertyChanged("memo");
            }
        }

        private bool _workCheck = false;

        public bool workCheck
        {
            get
            {
                return _workCheck;
            }
            set
            {
                _workCheck = value;
                onPropertyChanged("workCheck");
            }
        }

        private float _presultCount;
        public float presultCount
        {
            get
            {
                return _presultCount;
            }
            set
            {
                _presultCount = value;
                onPropertyChanged("presultCount");
            }
        }
  
        

        private float _sumPworkCount;
        public float sumPworkCount
        {
            get
            {
                
                return _sumPworkCount;
            
            }
            
            set
            {
                _sumPworkCount = value;
                onPropertyChanged("sumPworkCount");
            }
        }

        private ObservableCollection<errorModel> _errorModelOb;
        public ObservableCollection<errorModel> errorModelOb
        {
            get
            {
                    
                return _errorModelOb;
            
            }
            set
            {
                _errorModelOb = value;
                onPropertyChanged("errorModelOb");
            }

        }
        private String _workMemberKey;
        public String workMemberKey
        {
            get
            {
                return _workMemberKey;
            }
            set
            {
                _workMemberKey = value;
                onPropertyChanged("workMemberKey");
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

        private int _pplanKey;
        public int pplanKey
        {
            get
            {
                return _pplanKey;
            }
            set
            {
                _pplanKey = value;
                onPropertyChanged("pplanKey");
            }
        }

        private int _pworkKey;
        public int pworkKey
        {
            get
            {
                return _pworkKey;
            }
            set
            {
                _pworkKey = value;
                onPropertyChanged("pworkKey");
            }
        }

        private int _ProcessType;
        public int ProcessType
        {
            get
            {
                return _ProcessType;
            }
            set
            {
                _ProcessType = value;
                onPropertyChanged("ProcessType");
            }
        }

        private String _UpUserID;
        public String UpUserID
        {
            get
            {
                
                return _UpUserID;
            }
            set
            {
                _UpUserID = value;
                onPropertyChanged("UpUserID");
            }
        }

        private String _UpUserName;
        public String UpUserName
        {
            get
            {
                return _UpUserName;
            }

            set
            {
                _UpUserName = value;
                onPropertyChanged("UpUserName");
            }
            
        }

        private String _UpDatetime;
        public String UpDatetime
        {
            get
            {
                return _UpDatetime;
            }
            set
            {
                _UpDatetime = value;
                onPropertyChanged("UpDatetime");
            }
        }
        private String _imgSource;

        public String imgSource
        {
            get
            {
                return _imgSource;
            }
            set
            {
                _imgSource = value;
                onPropertyChanged("imgSource");
            }
        }
        private String _orderKey;
        public String orderKey
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

        private String _orderDate;
        public String orderDate
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

        private String _vendorID;
        public String vendorID
        {
            get
            {
                return _vendorID;
            }
            set
            {
                _vendorID = value;
                onPropertyChanged("vendorID");
            }
        }
        private string _vendorName;
        public String vendorName
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
        private String _NoKey;
        public String NoKey
        {
            get
            {
                return _NoKey;
            }
            set
            {
                _NoKey = value;
                onPropertyChanged("NoKey");
            }
        }
        private String _membID;
        public String membID
        {
            get
            {
                return _membID;
            }
            set
            {
                _membID = value;
                onPropertyChanged("membID");
            }
        }
        private String _membName;
        public String membName
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
        private String _adress;
        public String adress
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
        private String _PhonNo;
        public String PhonNo
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
        private int _orderCount;
        public int orderCount
        {
            get
            {
                return _orderCount;
            }
            set
            {
                _orderCount = value;
                onPropertyChanged("orderCount");
            }
        }
        private float _pWorkCount;
        public float pWorkCount
        {
            get
            {
                return _pWorkCount;
            }

            set
            {
                _pWorkCount = value;
                onPropertyChanged("pWorkCount");
            }

        }
        private String _eDatetime;
        public String eDatetime
        {
            get
            {
                return _eDatetime;
            }
            set
            {
                _eDatetime = value;
                if (sDatetime.Equals("") || sDatetime.Equals("0"))
                {
                    imgSource = "/img/bn_02.png";
                }
                else
                {
                    if(eDatetime.Equals("") || eDatetime.Equals("0"))
                    {
                        imgSource = "/img/bn_01.png";
                    }
                    else
                    {
                        imgSource = "/img/bn_03.png";
                    }
                }
              
                onPropertyChanged("eDatetime");
            }

        }
        private String _sDatetime;
        public String sDatetime
        {
            get
            {
                return _sDatetime;
            }

            set
            {
                _sDatetime = value;
                onPropertyChanged("sDatetime");
            }

        }
        private float _perrCnt;
        public float perrCnt
        {
            get
            {
                return _perrCnt;
            }

            set
            {
                _perrCnt = value;
                onPropertyChanged("perrCnt");
            }

        }

        private errorModel _errorModel;
        public errorModel errorModel
        {
            get
            {
                return _errorModel;
            }
            set
            {
                _errorModel = value;
                onPropertyChanged("errorModel");
            }

        }

        private String _Memo;
        public String Memo
        {
            get
            {
                return _Memo;
            }
            set
            {   
                _Memo = value;
                onPropertyChanged("Memo");
            }
        }


        private int _workCount;
        public int workCount
        {
            get
            {
                return _workCount;
            }
            set
            {
                _workCount = value;
                onPropertyChanged("workCount");
            }
        }

    }
}
