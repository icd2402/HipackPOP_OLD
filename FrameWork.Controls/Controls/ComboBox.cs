using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork.Controls
{
    public class ComboBox : System.Windows.Controls.ComboBox
    {
        /// <summary>
        /// ComboBox 의 Oid 키값
        /// </summary>
        public string Oid
        {
            get
            {
                string retValue = string.Empty;
                foreach (var cmbi in this.Items.Cast<ComboBoxItem>())
                {
                    if (cmbi.IsSelected == true)
                    {
                        retValue = cmbi.Oid;
                    }
                }

                return retValue;
            }
            set
            {
                foreach (var cmbi in this.Items.Cast<ComboBoxItem>().Where(cmbi => cmbi.Oid == value))
                {
                    cmbi.IsSelected = true;
                }
            }
        }
        /// <summary>
        /// ComboBox 의 Code Oid와 다를수있음
        /// </summary>
        public string Code
        {
            get
            {
                string retValue = string.Empty;
                foreach (var cmbi in this.Items.Cast<ComboBoxItem>())
                {
                    if (cmbi.IsSelected == true)
                    {
                        retValue = cmbi.Code;
                    }
                }

                return retValue;
            }
            set
            {
                foreach (var cmbi in this.Items.Cast<ComboBoxItem>().Where(cmbi => cmbi.Code == value))
                {
                    cmbi.IsSelected = true;
                }
            }
        }
        /// <summary>
        /// ComboBox 의 Code_Name 보여주는것
        /// </summary>
        public string Code_Name
        {
            get
            {
                string retValue = string.Empty;
                foreach (var cmbi in this.Items.Cast<ComboBoxItem>())
                {
                    if (cmbi.IsSelected == true)
                    {
                        retValue = cmbi.Code_Name;
                    }
                }

                return retValue;
            }
            set
            {
                foreach (var cmbi in this.Items.Cast<ComboBoxItem>().Where(cmbi => cmbi.Code_Name == value))
                {
                    cmbi.IsSelected = true;
                }
            }
        }
    }
}
