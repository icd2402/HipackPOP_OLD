using System;

namespace PopProject.Input
{
    class UserBaseModel : ViewModelBase
    {
        private string _UpUserID;
        public string UpUserID
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

        private string _UpUserName;
        public string UpUserName
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

        private DateTime _UpDatetime;
        public DateTime UpDatetime
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

        private bool _SearchPrintCheck;
        public bool SearchPrintCheck
        {
            get
            {
                return _SearchPrintCheck;
            }
            set
            {
                _SearchPrintCheck = value;
                onPropertyChanged("SearchPrintCheck");
            }
        }
    }
}
