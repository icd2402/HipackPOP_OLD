using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject.Detail
{
    class WhareHouseModel : ViewModelBase

    {
        private string _WharehouseID = "";
        public string WharehouseID
        {

            get => _WharehouseID;
            set
            {
                _WharehouseID = value;
                onPropertyChanged("WharehouseID");
            }
        }

        private string _TypeName = "";
        public string TypeName
        {

            get => _TypeName;
            set
            {
                _TypeName = value;
                onPropertyChanged("TypeName");
            }
        }

        private string _StartNo = "";
        public string StartNo
        {

            get => _StartNo;
            set
            {
                _StartNo = value;
                onPropertyChanged("StartNo");
            }
        }

        private string _EndNo = "";
        public string EndNo
        {

            get => _EndNo;
            set
            {
                _EndNo = value;
                onPropertyChanged("EndNo");
            }
        }

        private string _UpUserID = "";
        public string UpUserID
        {

            get => _UpUserID;
            set
            {
                _UpUserID = value;
                onPropertyChanged("UpUserID");
            }
        }

        private string _UpUserName = "";
        public string UpUserName
        {

            get => _UpUserName;
            set
            {
                _UpUserName = value;
                onPropertyChanged("UpUserName");
            }
        }

        private string _UpDatetime = "";
        public string UpDatetime
        {

            get => _UpDatetime;
            set
            {
                _UpDatetime = value;
                onPropertyChanged("UpDatetime");
            }
        }

        private string _Memo = "";
        public string Memo
        {

            get => _Memo;
            set
            {
                _Memo = value;
                onPropertyChanged("Memo");
            }
        }
        private string _WharehouseType = "";
        public string WharehouseType
        {

            get => _WharehouseType;
            set
            {
                _WharehouseType = value;
                onPropertyChanged("WharehouseType");
            }
        }

        private List<Wharehouse_LocationModel> _적재구역_ItemList = new List<Wharehouse_LocationModel>();
        public List<Wharehouse_LocationModel> 적재구역_ItemList
        {

            get => _적재구역_ItemList;
            set
            {
                적재구역_ItemList = value;
                onPropertyChanged("적재구역_ItemList");
            }
        }
    }
}
