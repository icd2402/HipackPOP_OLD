using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject.MerList
{
    class MerModel : ViewModelBase
    {
        private String _mercName;
        public String mercName {
            get
            {
                return _mercName;
            }
            set
            {
                _mercName = value;
                onPropertyChanged("mercName");
            }
        }
        private String _CountType;
        public String CountType
        {
            get
            {
                return _CountType;
            }
            set
            {
                _CountType = value;
                onPropertyChanged("CountType");
            }
        }
        private String _TypeIndex;
        public String TypeIndex
        {
            get
            {
                return _TypeIndex;
            }
            set
            {
                _TypeIndex = value;
                onPropertyChanged("TypeIndex");
            }
        }
        private String _ProcessType;
        public String ProcessType {
            get
            {
                return _ProcessType;
            }
            set
            {
                _ProcessType = value;
                onPropertyChanged("processType");
            }

        }

      
    }
}
