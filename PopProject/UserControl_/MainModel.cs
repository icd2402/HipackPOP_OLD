using PopProject.input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace PopProject.UserControl_
{
    public class MainModel : ViewModelBase
    {
        private String _PackCount = "";
        public String PackCount
        {

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
        private String _item6 = "";
        public String item6
        {
            get
            {
                return _item6;
            }
            set
            {
                _item6 = value;
                onPropertyChanged("item6");
            }
        }

        private String _item5 = "";
        public String item5
        {
            get
            {
                return _item5;
            }
            set
            {
                _item5 = value;
                onPropertyChanged("item5");
            }
        }

        private String _item4 = "";
        public String item4
        {
            get
            {
                return _item4;
            }
            set
            {
                _item4 = value;
                onPropertyChanged("item4");
            }
        }


        private String _item1 = "";
        public String item1
        {
            get
            {
                return _item1;
            }
            set
            {
                _item1 = value;
                onPropertyChanged("item1");
            }
        }
        private System.Windows.Media.Brush _scbjcountColor;
        public System.Windows.Media.Brush scbjcountColor
        {
            get
            {
                return _scbjcountColor;
            }
            set
            {
                _scbjcountColor = value;
                onPropertyChanged("scbjcountColor");
            }
        }
        private String _item2 = "";
        public String item2
        {
            get
            {
                return _item2;
            }
            set
            {
                _item2 = value;
                onPropertyChanged("item2");
            }
        }
        private String _item3 = "";
        public String item3
        {
            get
            {
                return _item3;
            }
            set
            {
                _item3 = value;
                onPropertyChanged("item3");
            }
        }

        private String _CoatingType = "";
        public String CoatingType
        {
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

        private String _filmNo = "";
        public String filmNo
        {

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

        private String _PackType = "";
        public String PackType
        {

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

        private String _handChk = "";
        public String handChk
        {

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


        private String _Pushingposition = "";
        public String Pushingposition
        {

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

        private String _PushingNo = "";
        public String PushingNo
        {

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

        private String _Printposition = "";
        public String Printposition
        {

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


        private bool _check = false;
        public bool check
        {
            get
            {
                return _check;
            }
            set
            {
                _check = value;
                onPropertyChanged("check");

            }
        }

        private String _corpName = "";
        public String corpName
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

        private String _icback1;
        public String icback1
        {
            
            get
            {
                return _icback1;
            }

            set
            {
                _icback1 = value;
                onPropertyChanged("icback1");
            }
        }
        
        private String _pStateSColor;
        public String pStateSColor
        {
            get
            {
                return _pStateSColor;
            }
            set
            {
                _pStateSColor = value;

                onPropertyChanged("pStateSColor");

            }
        }
        private String _pStateString;
        public String pStateString
        {
            get
            {
                return _pStateString;
            }
            set
            {
                _pStateString = value;

                onPropertyChanged("pStateString");

            }
        }

        private Visibility _pStateVisi;
        public Visibility pStateVisi
        {
            get
            {
                return _pStateVisi;
            }
            set
            {
                _pStateVisi = value;

                onPropertyChanged("pStateVisi");

            }
        }


        private String _pState;
        public String pState
        {
            get
            {
                return _pState;
            }
            set
            {
                _pState = value;
                switch (pState)
                {
                    //정상
                    case "0":
                        pStateColor = icback.Equals("#AFE1FF") ? "#AFE1FF" : "White";

                        break;
                    //보류
                    case "1":
                        _pStateColor = "";
                        break;
                    //지급
                    case "2":
                        _pStateColor = "Green";
                        break;
                    //초지급
                    case "3":
                        _pStateColor = "Red";
                        break;
                    //상차
                    case "4":
                        _pStateColor = "Yellow";
                        break;
                }
                onPropertyChanged("pState");

            }
        }

        private String _pStateColor = "White";
        public String pStateColor
        {
            get
            {
                return _pStateColor;
            }
            set
            {
                _pStateColor = value;
                onPropertyChanged("pStateColor");

            }
        }


        private String _icback="White";
        public String icback
        {
            get
            {
                return _icback;
            }
            set
            {
                _icback = value;
                onPropertyChanged("icback");

            }
        }
        //workMemo
        private String _workMemo;
        public String workMemo
        {
            get
            {
                return _workMemo;
            }
            set
            {
                _workMemo = value;
                onPropertyChanged("workMemo");

            }
        }
        private Boolean _bMember;
        public Boolean bMember
        {
            get
            {
                return _bMember;
            }
            
            set
            {
                _bMember = value;
                onPropertyChanged("bMember");
            }
                
        }

        private float _pCnt;
        public float pCnt {
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
        private String _urgency;

        public String urgency {
            get
            {
                return _urgency;
            }
            set
            {
                _urgency = value;
                onPropertyChanged("urgency");
            }
        }

        private double _pplanNo;

        public double pplanNo {
            get
            {
                return _pplanNo;
            }
            set
            {
                _pplanNo = value;

            }
        }
        private int _perrorCount;
        public int perrorCount {
            get
            {
                return _perrorCount;
            }
            set
            {
                _perrorCount = value;
                onPropertyChanged("perrorCount");
            }
        }

        private String _mercName;
        public String mercName {
            get
            {
                return _mercName;
            }
            set
            {
                _mercName = value;
            }
        }

        private String _prodID;
        public String prodID {
            get
            {
                return _prodID;
            }
            set
            {
                _prodID = value;
                onPropertyChanged("prodId");
            }
        }
        private int _pplanKey;
        public int pplanKey {
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

        private String _SCBJCount;
        public String SCBJCount
        {
            get
            {
                return _SCBJCount;
            }
            set
            {
                _SCBJCount = value;
                onPropertyChanged("SCBJCount");
            }
        }
        private String _SCIGCount;
        public String SCIGCount
        {
            get
            {
                return _SCIGCount;
            }
            set
            {
                _SCIGCount = value;
                onPropertyChanged("SCIGCount");
            }
        }

        private String _WDBJCount;
        public String WDBJCount {
            get
            {
                return _WDBJCount;
            }
            set
            {
                _WDBJCount = value;
                onPropertyChanged("WDBJCount");
            }
        }

        private String _WDIGCount;
        public String WDIGCount {
            get
            {
                return _WDIGCount;
            }
            set
            {
                _WDIGCount = value;
                onPropertyChanged("WDIGCount");
            }
        
        }
        private String _WDIGCount1;
        public String WDIGCount1
        {
            get
            {
                return _WDIGCount1;
            }
            set
            {
                _WDIGCount1 = value;
                onPropertyChanged("WDIGCount1");
            }

        }

        private String _orderKey;
        public String orderKey {
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
        private String _ProcessTypeName;
        public String ProcessTypeName
        {
            get
            {
                return _ProcessTypeName;
            }
            set
            {
                _ProcessTypeName = value;
                onPropertyChanged("ProcessTypeName");
            }
        }
            
        private String _givSC = "";
        public String givSC
        {
            get
            {
                return _givSC;
            }
            set
            {
                _givSC = value;
                onPropertyChanged("givSC");
            }
        }
        private string _boxType = "";
        public string boxType
        {
            get
            {
                return _boxType;
            }
            set
            {
                _boxType = value;
                onPropertyChanged("boxType");
            }
        }

        private String _givWD = "";
        public String givWD
        {
            get
            {
                return _givWD;
            }
            set
            {
                _givWD = value;
                onPropertyChanged("givWD");
            }
        }

        private String _ProcessMemo;
        public String ProcessMemo
        {
            get
            {
                return _ProcessMemo;
            }
            set
            {
                _ProcessMemo = value;
                onPropertyChanged("ProcessMemo");
            }
        }
        private int _ProcessType;

        public int ProcessType {
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

        private String _pplanDate;
        public String pplanDate {
            get
            {
                return _pplanDate;
            }
            set
            {
                _pplanDate = value;
                onPropertyChanged("pplanDate");
            }
        }
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
        private string _prodName;
        public string prodName {
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
          
        private String _paperGor;
        public String paperGor {
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
        private String _SCmetrialName;
        public String SCmetrialName
        {
            get
            {
                return _SCmetrialName;
            }
            set
            {
                _SCmetrialName = value;
                onPropertyChanged("SCmetrialName");
            }
        }
        private String _aL;
        public String aL {
            get
            {
                return _aL;
            }
            set
            {
                _aL = value;
                onPropertyChanged("aL");
            }
        }
        private String _aW;
        public String aW {
            
            get
            {
                return _aW;
            }

            set
            {
                _aW = value;
                onPropertyChanged("aW");
            }
        }

        private String _mLmW;
        public String mLmW
        {
            get
            {
                return _mLmW;
            }
            set
            {
                _mLmW = value;
                onPropertyChanged("mLmW");
            }
        }

        private String _aLaW;
        public String aLaW {
            get
            {
                return _aLaW;
            }
            set
            {
                _aLaW = value;
                onPropertyChanged("aLaW");
            }
        }
        private String _ConnetingType;
        public String ConnetingType
        {
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

        private String _ColorType;
        public String ColorType {
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

        private String _PrintNo;
        public String PrintNo {
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

        private String _pAccount;
        public String pAccount
        {
            get
            {
                return _pAccount;
            }
            set
            {
                _pAccount = value;
                onPropertyChanged("pAccount");
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

        private float _pplanboxCount;
        public float pplanboxCount
        {
            get
            {
                return _pplanboxCount;
            }
            set
            {
                _pplanboxCount = value;
                onPropertyChanged("pplanboxCount");
            }
        }

        private float _presultCount1;
        public float presultCount1
        {
            get
            {
                return _presultCount1;
            }
            set
            {
                _presultCount1 = value;
                if (this.inputModel != null)
                {
                    this.inputModel.count = value;
                }
                onPropertyChanged("presultCount1");
            }
        }

        private float _presultCount;
        public float presultCount {
            get
            {
                return _presultCount;
            }
            set
            {
                _presultCount = value;
                if(this.inputModel != null)
                {
                    this.inputModel.count = value;
                }
                onPropertyChanged("presultCount");
            }
        }

        private String _pplanMemo;
        public String pplanMemo
        {
            get
            {
                return _pplanMemo;
            }
            set
            {
                _pplanMemo = value;
                onPropertyChanged("pplanMemo");
            }
        }


        private String _SpecialMemo;
        public String SpecialMemo
        {
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
        private String _pSize;
        public String pSize
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
        private int _orderWorkNo;
        public int orderWorkNo {
            get
            {
                return _orderWorkNo;
            }
            set
            {
                _orderWorkNo = value;

            }
        }

        private List<SubModel> _subOb;
        public List<SubModel> subOb {
            get
            {
                return _subOb;
            }
            set
            {
                _subOb = value;
                onPropertyChanged("subOb");
            }
        
        }

        private String _sDatetime="";
        public String sDatetime {

            get
            {
                return _sDatetime;
            }
            set
            {
                _sDatetime = value;
                onPropertyChanged("sDateTime");
            }
        }




        private String _eDatetime=""; 
        public String eDatetime {
            get
            {
                return _eDatetime;
            }
            set
            {
                _eDatetime = value;
                if (sDatetime.Equals(""))
                {
                    imagePath = "/img/bn_02.png";
                }
                else
                {
                    if (eDatetime.Equals(""))
                    {
                        imagePath = "/img/bn_01.png";
                    }
                    else
                    {
                        imagePath = "/img/bn_03.png";
                        
                    }
                }
                onPropertyChanged("eDateTime");
            }
        }

        private String _imagePath;
        public String imagePath {
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
        private int _orderCount;
        public int orderCount {
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
        private int _preCount;
        public int preCount
        {
            get
            {
                return _preCount;
            }
            set
            {
                _preCount = value;
                onPropertyChanged("preCount");
            }
        }

        private string _orderLast;
        public string orderLast {
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

        private ObservableCollection<InputModel> _resultList;
        public ObservableCollection<InputModel> resultList {
            get
            {
                return _resultList;
            }
            set
            {
                _resultList = value;

            }
        }



        private float _sumPworkCount;
        public float sumPworkCount {
            get { 
                return _sumPworkCount;
            }
            set
            {
                _sumPworkCount = value;
                onPropertyChanged("sumPworkCount");
            }

        }

        private Visibility _boxvisibility;

        public Visibility boxvisibility
        {
            get
            {
                return _boxvisibility;
            }
            set
            {
                _boxvisibility = value;
                onPropertyChanged("boxvisibility");
            }
        }


        private Visibility _visibility;

        public Visibility visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                onPropertyChanged("visibility");
            }
        }

        private Visibility _visibility1;

        public Visibility visibility1
        {
            get
            {
                return _visibility1;
            }
            set
            {
                _visibility1 = value;
                onPropertyChanged("visibility1");
            }
        }
        private int _left;

        public int left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
                onPropertyChanged("left");
            }
        }

        private Visibility _newProd;
        public Visibility newProd {
            get
            {
                return _newProd;
            }
            set
            {
                _newProd = value;
                onPropertyChanged("newProd");
            }
        }

        private InputModel _inputModel;
        public InputModel inputModel {
            get
            {
                return _inputModel;
            }
            set
            {
                _inputModel = value;
            }
        }
       
    }

    public class SubModel :ViewModelBase
    {
        private string _ProcessTypeName;
        public String ProcessTypeName {
            get
            {
                return _ProcessTypeName;
            }
            set
            {
                _ProcessTypeName = value;
            }
        }

        private string _ProcessSeq;
        public String ProcessSeq
        {
            get
            {
                return _ProcessSeq;
            }
            set
            {
                _ProcessSeq = value;
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

       

        private System.Windows.Media.Brush _stateColor;
        public System.Windows.Media.Brush stateColor {
            get
            {
                return _stateColor;
            }
            set
            {
                _stateColor = value;
                onPropertyChanged("stateColor");
            }
        }

        private System.Windows.Media.Brush _stateFontColor;
        public System.Windows.Media.Brush stateFontColor {
            get
            {
                return _stateFontColor;
            }
            set
            {
                _stateFontColor = value;
                onPropertyChanged("stateFontColor");
            }
        }

        private int _processType;
        public int processType {
            get
            {
                return _processType;
            }
            set
            {
                _processType = value;
            }
        }

        private int _presultCount;
        public int presultCount {
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

       
    }
}
