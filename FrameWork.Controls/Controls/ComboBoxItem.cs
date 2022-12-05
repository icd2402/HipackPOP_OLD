using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork.Controls
{
    public class ComboBoxItem : System.Windows.Controls.ComboBoxItem
    {
        /// <summary>
        /// ComboBoxItem의 Oid 키값
        /// </summary>
        public string Oid
        {
            get;
            set;
        }
        /// <summary>
        /// ComboBoxItem의 Code
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// ComboBoxItem의 보이는 Code_Name
        /// </summary>
        public string Code_Name
        {
            get;
            set;
        }
    }
}
