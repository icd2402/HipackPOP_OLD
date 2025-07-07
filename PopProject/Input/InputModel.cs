using PopProject.Consumer_;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PopProject.input
{
    public class InputModel : ViewModelBase
    {
        private String _ServerIndex;
        public String ServerIndex
        {
            get
            {
                return _ServerIndex;
            }
            set
            {
                _ServerIndex = value;
                onPropertyChanged("ServerIndex");
            }
        }

        private String _DBName;
        public String DBName
        {
            get
            {
                return _DBName;
            }
            set
            {
                _DBName = value;
                onPropertyChanged("DBName");
            }
        }


        private bool _bMember;
        public bool BMember
        {
            get => _bMember;

            set
            {
                _bMember = value;
                onPropertyChanged("bMember");
            }

        }

        private float _pCnt;
        public float pCnt
        {
            get
            {
                return _pCnt;
            }


            set
            {
                _pCnt = value;
                onPropertyChanged("pCnt");
            }
        }
        private float _perrCount;
        public float perrCount
        {


            get
            {
                return _perrCount;
            }

            set
            {
                _perrCount = value;
                onPropertyChanged("perrCount");
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
            }
        }

        private float _consumer_count;
        public float consumer_count
        {
            get
            {
                return _consumer_count;
            }

            set
            {

                consumer_pworkCount = consumer_count - perrCount;
                _consumer_count = value;
                onPropertyChanged("consumer_count");

            }
        }

        private float _count;
        public float count
        {
            get
            {
                return _count;
            }
            set
            {
                if (count == 0)
                {
                    pworkCount = pworkCount - perrCount;
                }
                else
                {
                    pworkCount = count - perrCount;
                }
                _count = value;
                onPropertyChanged("count");
            }
        }

        //consumer_pworkCount
        private float _consumer_pworkCount;
        public float consumer_pworkCount
        {

            get
            {
                return _consumer_pworkCount;
            }

            set
            {
                _consumer_pworkCount = value;
                onPropertyChanged("consumer_pworkCount");
            }
        }

        private float _pworkCount;

        public float pworkCount
        {

            get
            {
                return _pworkCount;
            }

            set
            {
                _pworkCount = value;
                onPropertyChanged("pworkCount");
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
        private int _workMemberKey;
        public int workMemberKey
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
        private int _pplanNo;
        public int pplanNo
        {
            get
            {
                return _pplanNo;
            }
            set
            {
                _pplanNo = value;
                onPropertyChanged("pplanNo");
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

        private string _sDatetime = "";
        public string sDatetime
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

        private string _eDatetime = "";
        public string eDatetime
        {
            get
            {
                return _eDatetime;
            }
            set
            {
                _eDatetime = value;
                onPropertyChanged("eDatetime");
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

        private int _orderWorkNo;
        public int orderWorkNo
        {

            get
            {
                return _orderWorkNo;
            }

            set

            {
                _orderWorkNo = value;
                onPropertyChanged("orderWorkNo");
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

        private string _imagePath;
        public string imagePath
        {
            get
            {
                return _imagePath;

            }
            set
            {
                _imagePath = value;
                onPropertyChanged("imagePath");
            }
        }
        private float _sumPlanCount;
        public float sumPlanCount
        {
            get
            {
                return _sumPlanCount;
            }

            set
            {
                _sumPlanCount = value;
                onPropertyChanged("sumPlanCount");
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

        private string _workerName;
        public string workerName
        {
            get
            {
                return _workerName;
            }
            set
            {
                _workerName = value;
                onPropertyChanged("workerName");
            }

        }

        private string _SCMKey;
        public string SCMKey
        {

            get
            {
                return _SCMKey;
            }
            set
            {
                _SCMKey = value;
                onPropertyChanged("SCMKey");
            }

        }

        private string _workerId;
        public string workerId
        {
            get
            {
                return _workerId;
            }
            set
            {
                _workerId = value;
                onPropertyChanged("workerId");
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

        private Visibility _newProd;
        public Visibility newProd
        {
            get
            {
                return _newProd;
            }
            set
            {
                _newProd = value;

            }
        }

        private Visibility _pCntVisible;
        public Visibility pCntVisible
        {
            get
            {
                return _pCntVisible;
            }
            set
            {
                _pCntVisible = value;
                onPropertyChanged("pCntVisible");
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
    }
    public class errorModel : ViewModelBase
    {

        private string _workMemberKey;
        public string workMemberKey
        {
            get
            {
                return _workMemberKey;
            }
            set
            {
                _workMemberKey = value;
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
            }

        }

        private string _perrKey;

        public string perrKey
        {
            get
            {
                return _perrKey;
            }
            set
            {
                _perrKey = value;
            }

        }
        private string _memo;

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

        private float _perrCount;
        public float perrCount
        {
            get
            {
                return _perrCount;
            }
            set
            {
                _perrCount = value;
                onPropertyChanged("perrCount");
            }
        }

        private DateTime _updateTime;
        public DateTime updateTime
        {

            get
            {
                return _updateTime;
            }
            set
            {
                _updateTime = value;
                onPropertyChanged("updateTime");
            }
        }

        private System.Windows.Media.Brush _fontColor;
        public System.Windows.Media.Brush fontColor
        {
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

        private Visibility _fontVisible;
        public Visibility fontVisible
        {
            get
            {
                return _fontVisible;
            }
            set
            {
                _fontVisible = value;
                onPropertyChanged("fontVisible");
            }
        }

        private InputModel _InputModel;
        public InputModel InputModel
        {
            get
            {
                return _InputModel;
            }
            set
            {
                _InputModel = value;
            }

        }
        private ConsumerModel _ConsumerModel;
        public ConsumerModel ConsumerModel
        {
            get
            {
                return _ConsumerModel;
            }
            set
            {
                _ConsumerModel = value;
            }

        }


    }
}
