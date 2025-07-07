namespace PopProject.Input
{
    class Error_Model : ViewModelBase

    {

        private bool _errorCheck;
        public bool errorCheck
        {

            get
            {
                return _errorCheck;
            }

            set
            {
                _errorCheck = value;
                onPropertyChanged("errorCheck");
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

        private string _Memo;
        public string Memo
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

        private string _ProcessTypeName;
        public string ProcessTypeName
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
    }
}
