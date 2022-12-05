using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace FrameWork.Controls
{
    public class TabItemEx : System.Windows.Controls.TabItem
    {
        public RelayCommand TabCloseCommand { get; set; }

        public delegate void ClosedEventHandler(string p_str);    //이벤트처리를위한델리게이트정의
        public event ClosedEventHandler ClosedEvent;    //이벤트 명 선언

        public TabItemEx()
        {
            TabCloseCommand = new RelayCommand(TabClose);

            this.DataContext = this;//바인딩 해주기 위해 필요
        }
        #region RelayCommand 선택 변경시 처리하기 - OnSelectionChanged(parameter)
        /// <summary>
        /// 선택 변경시 처리하기
        /// </summary>
        /// <param name="parameter">파라미터</param>
        public void TabClose(object parameter)
        {
            ClosedEvent(this.Content.ToString());
        }
        #endregion
        

    }
}