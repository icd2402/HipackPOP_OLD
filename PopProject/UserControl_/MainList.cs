using PopProject.input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace PopProject.UserControl_
{
    class MainList : ViewModelBase
    {

        

        private String _ConsumWorkKey = "";
        public String ConsumWorkKey
        {

            get
            {
                return _ConsumWorkKey;
            }
            set
            {
                _ConsumWorkKey = value;
                onPropertyChanged("ConsumWorkKey");
            }
        }



        //옵션 > 작업정보

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

        private String _PrintNo;
        public String PrintNo
        {
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

        private String _ColorType;
        public String ColorType
        {
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

        private String _PalletType;
        public String PalletType
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

        // 작업정보 텍스트 변수(계획순서 변경창)
        private String _item8 = "";
        public String item8
        {
            get
            {
                return _item8;
            }
            set
            {
                _item8 = value;
                onPropertyChanged("item8");
            }
        }

        private String _item7 = "";
        public String item7
        {
            get
            {
                return _item7;
            }
            set
            {
                _item7 = value;
                onPropertyChanged("item7");
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

        // 작업정보 텍스트 변수
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

        //실수요자
        private String _icback = "White";
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
        
        //실수요자
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

        //통수
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

        


        

        //불량
        private int _perrorCount;
        public int perrorCount
        {
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

        //설비명
        private String _mercName;
        public String mercName
        {
            get
            {
                return _mercName;
            }
            set
            {
                _mercName = value;
            }
        }

        //생산ID
        private String _prodID;
        public String prodID
        {
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

        //계획번호(정렬)
        private double _pplanNo;
        public double pplanNo
        {
            get
            {
                return _pplanNo;
            }
            set
            {
                _pplanNo = value;

            }
        }

        //계획키(정렬)
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

        //수주일자(정렬)
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

        //긴급(정렬)
        private String _urgency;
        public String urgency
        {
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

        //종료시간(정렬)
        private String _eDatetime = "";
        public String eDatetime
        {
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

        //생산차수(정렬)
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


        //스코아
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

        //마닐라 입고수량
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

        //원단 발주수량
        private String _WDBJCount;
        public String WDBJCount
        {
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

        //원단 입고수량
        private String _WDIGCount;
        public String WDIGCount
        {
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

        //원단 입고수량(계획순서 변경창)
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

        //오더키
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

        //설비구분
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

        //마닐라 발주
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

        //원단 발주
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

        //공정 비고
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

        //공정 구분
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

        //거래처명
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

        //품명1
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

        //품명2
        private string _prodName2;
        public string prodName2
        {
            get
            {
                return _prodName2;
            }
            set
            {
                _prodName2 = value;
                onPropertyChanged("prodName2");
            }
        }

        //품명 Cnavas
        private string _Canvas;
        public string Canvas
        {
            get
            {
                return _Canvas;
            }
            set
            {
                _Canvas = value;
                onPropertyChanged("Canvas");
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
        
        //마닐라 규격
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


        //원단 규격
        private String _aLaW;
        public String aLaW
        {
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

        //계획수량
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

        //계획 박스수량
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

        //결과수량 (상자수)
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

        //결과수량
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
                if (this.inputModel != null)
                {
                    this.inputModel.count = value;
                }
                onPropertyChanged("presultCount");
            }
        }

        //원단 사이즈
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

        //작업번호
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

            }
        }

        //공정칸 처리
        private List<SubModel> _subOb;
        public List<SubModel> subOb
        {
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

        //시작시간
        private String _sDatetime = "";
        public String sDatetime
        {

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

        //상태버튼 이미지
        private String _imagePath;
        public String imagePath
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
        

        //inpuModel 결과리스트
        private ObservableCollection<InputModel> _resultList;
        public ObservableCollection<InputModel> resultList
        {
            get
            {
                return _resultList;
            }
            set
            {
                _resultList = value;

            }
        }


        //총 결과수량
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

        //박스수
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

        //수주일자
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

        //납품일자
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

        //품명 텍스트용
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

        //뭔지 모르겠는데 사용중
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
                onPropertyChanged("newProd");
            }
        }

        private InputModel _inputModel;
        public InputModel inputModel
        {
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
}
