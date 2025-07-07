using System;

namespace PopProject.Input
{
    public class UpdateInfor_Model : ViewModelBase
    {
        private int _Notice_No;  // 기본키
        public int Notice_No
        {
            get
            {
                return _Notice_No;
            }
            set
            {
                _Notice_No = value;
                onPropertyChanged("Notice_No");
            }
        }

        private int _Program_ID;
        public int Program_ID
        {
            get
            {
                return _Program_ID;
            }
            set
            {
                _Program_ID = value;
                onPropertyChanged("Program_ID");
            }
        }

        private string _Version_Number = "";
        public string Version_Number
        {
            get
            {
                return _Version_Number;
            }
            set
            {
                _Version_Number = value;
                onPropertyChanged("Version_Number");
            }
        }

        private DateTime _Update_Date = DateTime.Now;
        public DateTime Update_Date
        {
            get
            {
                return _Update_Date;
            }
            set
            {
                _Update_Date = value;
                onPropertyChanged("Update_Date");
            }
        }

        private string _Notice = "";
        public string Notice
        {
            get
            {
                return _Notice;
            }
            set
            {
                _Notice = value;
                onPropertyChanged("Notice");
            }
        }

        private string _Apply_Server = "";
        public string Apply_Server
        {
            get
            {
                return _Apply_Server;
            }
            set
            {
                _Apply_Server = value;
                onPropertyChanged("Apply_Server");
            }
        }

        private int _ChkAED = 1;
        public int ChkAED
        {
            get
            {
                return _ChkAED;
            }
            set
            {
                _ChkAED = value;
                onPropertyChanged("ChkAED");
            }
        }

        public void Set_Clean()
        {
            {
                var withBlock = this;
                withBlock.ChkAED = 1;
                withBlock.Notice_No = 0;
                withBlock.Program_ID = 0;
                withBlock.Update_Date = DateTime.Now;
                withBlock.Version_Number = "";
                withBlock.Apply_Server = "";
                withBlock.Notice = "";
            }
        }

    }
}
