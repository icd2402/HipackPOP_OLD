using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject.Detail
{
    public class Wharehouse_LocationModel :ViewModelBase
    {
        private string _LocationName = "";
        public string LocationName
        {

            get => _LocationName;
            set
            {
                _LocationName = value;
                onPropertyChanged("LocationName");
            }
        }

        private int _LocationType ;
        public int LocationType
        {

            get => _LocationType;
            set
            {
                _LocationType = value;
                onPropertyChanged("LocationType");
            }
        }

        private int _IsSelectedStyle=0;
        public int IsSelectedStyle
        {

            get => _IsSelectedStyle;
            set
            {
                _IsSelectedStyle = value;
                onPropertyChanged("IsSelectedStyle");
            }
        }

        private string _customID = "";
        public string customID
        {

            get => _customID;
            set
            {
                _customID = value;
                onPropertyChanged("customID");
            }
        }

        private string _customSName = "";
        public string customSName
        {

            get => _customSName;
            set
            {
                _customSName = value;
                onPropertyChanged("customSName");
            }
        }

        private string _prodID = "";
        public string prodID
        {

            get => _prodID;
            set
            {
                _prodID = value;
                onPropertyChanged("prodID");
            }
        }

        private string _prodName = "";
        public string prodName
        {

            get => _prodName;
            set
            {
                _prodName = value;
                onPropertyChanged("prodName");
            }
        }

        private string _JGCount = "";
        public string JGCount
        {

            get => _JGCount;
            set
            {
                _JGCount = value;
                onPropertyChanged("JGCount");
            }
        }
    }
}
