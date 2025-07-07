namespace PopProject.UserControl_
{
    class UserModel : ViewModelBase
    {


        private string _login_id = "";
        public string login_id
        {

            get
            {
                return _login_id;
            }
            set
            {
                _login_id = value;
                onPropertyChanged("login_id");
            }
        }

        private string _login_password = "";
        public string login_password
        {

            get
            {
                return _login_password;
            }
            set
            {
                _login_password = value;
                onPropertyChanged("login_password");
            }
        }

        private string _corp_name = "";
        public string corp_name
        {

            get
            {
                return _corp_name;
            }
            set
            {
                _corp_name = value;
                onPropertyChanged("corp_name");
            }
        }

        private string _corp_id = "";
        public string corp_id
        {

            get
            {
                return _corp_id;
            }
            set
            {
                _corp_id = value;
                onPropertyChanged("corp_id");
            }
        }

        private string _url = "";
        public string url
        {

            get
            {
                return _url;
            }
            set
            {
                _url = value;
                onPropertyChanged("url");
            }
        }

        private string _signalR = "";
        public string signalR
        {

            get
            {
                return _signalR;
            }
            set
            {
                _signalR = value;
                onPropertyChanged("signalR");
            }
        }

        private string _serverIndex = "";
        public string serverIndex
        {

            get
            {
                return _serverIndex;
            }
            set
            {
                _serverIndex = value;
                onPropertyChanged("serverIndex");
            }
        }

        private string _manager = "";
        public string manager
        {

            get
            {
                return _manager;
            }
            set
            {
                _manager = value;
                onPropertyChanged("manager");
            }
        }

        private string _corp_num = "";
        public string corp_num
        {

            get
            {
                return _corp_num;
            }
            set
            {
                _corp_num = value;
                onPropertyChanged("corp_num");
            }
        }


        private string _user_name = "";
        public string user_name
        {

            get
            {
                return _user_name;
            }
            set
            {
                _user_name = value;
                onPropertyChanged("user_name");
            }
        }
        private string _login_time = "";
        public string login_time
        {

            get
            {
                return _login_time;
            }
            set
            {
                _login_time = value;
                onPropertyChanged("login_time");
            }
        }
    }


}
