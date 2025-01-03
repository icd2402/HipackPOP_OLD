using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject
{
    class Cls_Variable
    {
        private static bool _sharedValue;

        // 설비 선택 마우스 클릭시, 화면 move에 대한 방지를 위한 변수.
        public static bool SharedValue
        {
            get { return _sharedValue; }
            set { _sharedValue = value; }
        }
    }
}
