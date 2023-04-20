using PopProject.Consumer_;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PopProject.input
{
    class Resipe_Model : ViewModelBase

    {
        private int _prodID;

        public int prodID
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

        private string _MachineName;

        public string MachineName
        {
            get
            {
                return _MachineName;
            }
            set
            {
                _MachineName = value;
                onPropertyChanged("MachineName");
            }
        }

        private string _ModelNum;

        public string ModelNum
        {
            get
            {
                return _ModelNum;
            }
            set
            {
                _ModelNum = value;
                onPropertyChanged("ModelNum");
            }
        }

        private string _WorkPlan1;

        public string WorkPlan1
        {
            get
            {
                return _WorkPlan1;
            }
            set
            {
                _WorkPlan1 = value;
                onPropertyChanged("WorkPlan1");
            }
        }

        private string _WorkPlan2;

        public string WorkPlan2
        {
            get
            {
                return _WorkPlan2;
            }
            set
            {
                _WorkPlan2 = value;
                onPropertyChanged("WorkPlan2");
            }
        }

        private string _BoxType;

        public string BoxType
        {
            get
            {
                return _BoxType;
            }
            set
            {
                _BoxType = value;
                onPropertyChanged("BoxType");
            }
        }

        private string _Pock;

        public string Pock
        {
            get
            {
                return _Pock;
            }
            set
            {
                _Pock = value;
                onPropertyChanged("Pock");
            }
        }

        private string _Jang;

        public string Jang
        {
            get
            {
                return _Jang;
            }
            set
            {
                _Jang = value;
                onPropertyChanged("Jang");
            }
        }

        private string _Go;

        public string Go
        {
            get
            {
                return _Go;
            }
            set
            {
                _Go = value;
                onPropertyChanged("Go");
            }
        }

        private string _Gap;

        public string Gap
        {
            get
            {
                return _Gap;
            }
            set
            {
                _Gap = value;
                onPropertyChanged("Gap");
            }
        }

        private string _Wing;

        public string Wing
        {
            get
            {
                return _Wing;
            }
            set
            {
                _Wing = value;
                onPropertyChanged("Wing");
            }
        }

        private string _Cutting;

        public string Cutting
        {
            get
            {
                return _Cutting;
            }
            set
            {
                _Cutting = value;
                onPropertyChanged("Cutting");
            }
        }

        private string _SaveDone;

        public string SaveDone
        {
            get
            {
                return _SaveDone;
            }
            set
            {
                _SaveDone = value;
                onPropertyChanged("SaveDone");
            }
        }

        private string _WorkSpeed;

        public string WorkSpeed
        {
            get
            {
                return _WorkSpeed;
            }
            set
            {
                _WorkSpeed = value;
                onPropertyChanged("WorkSpeed");
            }
        }

        private string _SaveState;

        public string SaveState
        {
            get
            {
                return _SaveState;
            }
            set
            {
                _SaveState = value;
                onPropertyChanged("SaveState");
            }
        }

        private string _spare;

        public string spare
        {
            get
            {
                return _spare;
            }
            set
            {
                _spare = value;
                onPropertyChanged("spare");
            }
        }

        private string _aRecipe;

        public string aRecipe
        {
            get
            {
                return _aRecipe;
            }
            set
            {
                _aRecipe = value;
                onPropertyChanged("aRecipe");
            }
        }

        private string _bRecipe;

        public string bRecipe
        {
            get
            {
                return _bRecipe;
            }
            set
            {
                _bRecipe = value;
                onPropertyChanged("bRecipe");
            }
        }

        private string _cRecipe;

        public string cRecipe
        {
            get
            {
                return _cRecipe;
            }
            set
            {
                _cRecipe = value;
                onPropertyChanged("cRecipe");
            }
        }

        private string _dRecipe;

        public string dRecipe
        {
            get
            {
                return _dRecipe;
            }
            set
            {
                _dRecipe = value;
                onPropertyChanged("dRecipe");
            }
        }

        private string _eRecipe;

        public string eRecipe
        {
            get
            {
                return _eRecipe;
            }
            set
            {
                _eRecipe = value;
                onPropertyChanged("eRecipe");
            }
        }

        private string _f1Recipe;

        public string f1Recipe
        {
            get
            {
                return _f1Recipe;
            }
            set
            {
                _f1Recipe = value;
                onPropertyChanged("f1Recipe");
            }
        }

        private string _f2Recipe;

        public string f2Recipe
        {
            get
            {
                return _f2Recipe;
            }
            set
            {
                _f2Recipe = value;
                onPropertyChanged("f2Recipe");
            }
        }

        private string _g2pRecipe;

        public string g2pRecipe
        {
            get
            {
                return _g2pRecipe;
            }
            set
            {
                _g2pRecipe = value;
                onPropertyChanged("g2pRecipe");
            }
        }

        private string _oRecipe;

        public string oRecipe
        {
            get
            {
                return _oRecipe;
            }
            set
            {
                _oRecipe = value;
                onPropertyChanged("oRecipe");
            }
        }

        private string _p1Recipe;

        public string p1Recipe
        {
            get
            {
                return _p1Recipe;
            }
            set
            {
                _p1Recipe = value;
                onPropertyChanged("p1Recipe");
            }
        }

        private string _p2pRecipe;

        public string p2pRecipe
        {
            get
            {
                return _p2pRecipe;
            }
            set
            {
                _p2pRecipe = value;
                onPropertyChanged("p2pRecipe");
            }
        }

        private string _p3Recipe;

        public string p3Recipe
        {
            get
            {
                return _p3Recipe;
            }
            set
            {
                _p3Recipe = value;
                onPropertyChanged("p3Recipe");
            }
        }

        private string _p4Recipe;

        public string p4Recipe
        {
            get
            {
                return _p4Recipe;
            }
            set
            {
                _p4Recipe = value;
                onPropertyChanged("p4Recipe");
            }
        }

        private string _spare2;

        public string spare2
        {
            get
            {
                return _spare2;
            }
            set
            {
                _spare2 = value;
                onPropertyChanged("spare2");
            }
        }

        private string _spare3;

        public string spare3
        {
            get
            {
                return _spare3;
            }
            set
            {
                _spare3 = value;
                onPropertyChanged("spare3");
            }
        }

        private string _Slotter;

        public string Slotter
        {
            get
            {
                return _Slotter;
            }
            set
            {
                _Slotter = value;
                onPropertyChanged("Slotter");
            }
        }

        private string _DieCutter;

        public string DieCutter
        {
            get
            {
                return _DieCutter;
            }
            set
            {
                _DieCutter = value;
                onPropertyChanged("DieCutter");
            }
        }

        private string _PrinterPull1;

        public string PrinterPull1
        {
            get
            {
                return _PrinterPull1;
            }
            set
            {
                _PrinterPull1 = value;
                onPropertyChanged("PrinterPull1");
            }
        }

        private string _PrinterRoll1;

        public string PrinterRoll1
        {
            get
            {
                return _PrinterRoll1;
            }
            set
            {
                _PrinterRoll1 = value;
                onPropertyChanged("PrinterRoll1");
            }
        }

        private string _PrinterPull2;

        public string PrinterPull2
        {
            get
            {
                return _PrinterPull2;
            }
            set
            {
                _PrinterPull2 = value;
                onPropertyChanged("PrinterPull2");
            }
        }

        private string _PrinterRoll2;

        public string PrinterRoll2
        {
            get
            {
                return _PrinterRoll2;
            }
            set
            {
                _PrinterRoll2 = value;
                onPropertyChanged("PrinterRoll2");
            }
        }

        private string _PrinterPull3;

        public string PrinterPull3
        {
            get
            {
                return _PrinterPull3;
            }
            set
            {
                _PrinterPull3 = value;
                onPropertyChanged("PrinterPull3");
            }
        }

        private string _PrinterRoll3;

        public string PrinterRoll3
        {
            get
            {
                return _PrinterRoll3;
            }
            set
            {
                _PrinterRoll3 = value;
                onPropertyChanged("PrinterRoll3");
            }
        }

        private string _PrinterPull4;

        public string PrinterPull4
        {
            get
            {
                return _PrinterPull4;
            }
            set
            {
                _PrinterPull4 = value;
                onPropertyChanged("PrinterPull4");
            }
        }

        private string _PrinterRoll4;

        public string PrinterRoll4
        {
            get
            {
                return _PrinterRoll4;
            }
            set
            {
                _PrinterRoll4 = value;
                onPropertyChanged("PrinterRoll4");
            }
        }

        private string _spare4;

        public string spare4
        {
            get
            {
                return _spare4;
            }
            set
            {
                _spare4 = value;
                onPropertyChanged("spare4");
            }
        }

        private string _spare5;

        public string spare5
        {
            get
            {
                return _spare5;
            }
            set
            {
                _spare5 = value;
                onPropertyChanged("spare5");
            }
        }

        private string _spare6;

        public string spare6
        {
            get
            {
                return _spare6;
            }
            set
            {
                _spare6 = value;
                onPropertyChanged("spare6");
            }
        }

        private string _spare7;

        public string spare7
        {
            get
            {
                return _spare7;
            }
            set
            {
                _spare7 = value;
                onPropertyChanged("spare7");
            }
        }

        private string _FeederGap;

        public string FeederGap
        {
            get
            {
                return _FeederGap;
            }
            set
            {
                _FeederGap = value;
                onPropertyChanged("FeederGap");
            }
        }

        private string _FeederRoll;

        public string FeederRoll
        {
            get
            {
                return _FeederRoll;
            }
            set
            {
                _FeederRoll = value;
                onPropertyChanged("FeederRoll");
            }
        }

        private string _SlotterSubGap;

        public string SlotterSubGap
        {
            get
            {
                return _SlotterSubGap;
            }
            set
            {
                _SlotterSubGap = value;
                onPropertyChanged("SlotterSubGap");
            }
        }

        private string _SlotterMiddleGap;

        public string SlotterMiddleGap
        {
            get
            {
                return _SlotterMiddleGap;
            }
            set
            {
                _SlotterMiddleGap = value;
                onPropertyChanged("SlotterMiddleGap");
            }
        }

        private string _SlotterConnection;

        public string SlotterConnection
        {
            get
            {
                return _SlotterConnection;
            }
            set
            {
                _SlotterConnection = value;
                onPropertyChanged("SlotterConnection");
            }
        }

        private string _DieCutterRollGap;

        public string DieCutterRollGap
        {
            get
            {
                return _DieCutterRollGap;
            }
            set
            {
                _DieCutterRollGap = value;
                onPropertyChanged("DieCutterRollGap");
            }
        }

        private string _GluerbeltF1;

        public string GluerbeltF1
        {
            get
            {
                return _GluerbeltF1;
            }
            set
            {
                _GluerbeltF1 = value;
                onPropertyChanged("GluerbeltF1");
            }
        }

        private string _GluerbeltF2;

        public string GluerbeltF2
        {
            get
            {
                return _GluerbeltF2;
            }
            set
            {
                _GluerbeltF2 = value;
                onPropertyChanged("GluerbeltF2");
            }
        }

        private string _GluerPressA;

        public string GluerPressA
        {
            get
            {
                return _GluerPressA;
            }
            set
            {
                _GluerPressA = value;
                onPropertyChanged("GluerPressA");
            }
        }

        private string _GluerPressB;

        public string GluerPressB
        {
            get
            {
                return _GluerPressB;
            }
            set
            {
                _GluerPressB = value;
                onPropertyChanged("GluerPressB");
            }
        }

        private string _DieCutterRollA;

        public string DieCutterRollA
        {
            get
            {
                return _DieCutterRollA;
            }
            set
            {
                _DieCutterRollA = value;
                onPropertyChanged("DieCutterRollA");
            }
        }

        private string _DieCutterRollB;

        public string DieCutterRollB
        {
            get
            {
                return _DieCutterRollB;
            }
            set
            {
                _DieCutterRollB = value;
                onPropertyChanged("DieCutterRollB");
            }
        }

        private string _GluerPE;

        public string GluerPE
        {
            get
            {
                return _GluerPE;
            }
            set
            {
                _GluerPE = value;
                onPropertyChanged("GluerPE");
            }
        }

        private string _BRSub;

        public string BRSub
        {
            get
            {
                return _BRSub;
            }
            set
            {
                _BRSub = value;
                onPropertyChanged("BRSub");
            }
        }

        private string _BRMain;

        public string BRMain
        {
            get
            {
                return _BRMain;
            }
            set
            {
                _BRMain = value;
                onPropertyChanged("BRMain");
            }
        }

        private string _BRConnection;

        public string BRConnection
        {
            get
            {
                return _BRConnection;
            }
            set
            {
                _BRConnection = value;
                onPropertyChanged("BRConnection");
            }
        }

        private string _BRmiddle;

        public string BRmiddle
        {
            get
            {
                return _BRmiddle;
            }
            set
            {
                _BRmiddle = value;
                onPropertyChanged("BRmiddle");
            }
        }

        private string _spare8;

        public string spare8
        {
            get
            {
                return _spare8;
            }
            set
            {
                _spare8 = value;
                onPropertyChanged("spare8");
            }
        }

        private string _spare9;

        public string spare9
        {
            get
            {
                return _spare9;
            }
            set
            {
                _spare9 = value;
                onPropertyChanged("spare9");
            }
        }

        private string _spare10;

        public string spare10
        {
            get
            {
                return _spare10;
            }
            set
            {
                _spare10 = value;
                onPropertyChanged("spare10");
            }
        }

        private string _spare11;

        public string spare11
        {
            get
            {
                return _spare11;
            }
            set
            {
                _spare11 = value;
                onPropertyChanged("spare11");
            }
        }

        private string _spare12;

        public string spare12
        {
            get
            {
                return _spare12;
            }
            set
            {
                _spare12 = value;
                onPropertyChanged("spare12");
            }
        }

        private string _spare13;

        public string spare13
        {
            get
            {
                return _spare13;
            }
            set
            {
                _spare13 = value;
                onPropertyChanged("spare13");
            }
        }

        private string _spare14;

        public string spare14
        {
            get
            {
                return _spare14;
            }
            set
            {
                _spare14 = value;
                onPropertyChanged("spare14");
            }
        }

        private string _TotalCount1;

        public string TotalCount1
        {
            get
            {
                return _TotalCount1;
            }
            set
            {
                _TotalCount1 = value;
                onPropertyChanged("TotalCount1");
            }
        }

        private string _TotalCount2;

        public string TotalCount2
        {
            get
            {
                return _TotalCount2;
            }
            set
            {
                _TotalCount2 = value;
                onPropertyChanged("TotalCount2");
            }
        }

        private string _Count1;

        public string Count1
        {
            get
            {
                return _Count1;
            }
            set
            {
                _Count1 = value;
                onPropertyChanged("Count1");
            }
        }

        private string _Count2;

        public string Count2
        {
            get
            {
                return _Count2;
            }
            set
            {
                _Count2 = value;
                onPropertyChanged("Count2");
            }
        }
    }
}
