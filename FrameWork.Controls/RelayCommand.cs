using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrameWork.Controls
{
    /// <summary>
    /// 릴레이 명령
    /// </summary>
    public class RelayCommand : ICommand
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Event
        ////////////////////////////////////////////////////////////////////////////////////////// Public
        #region 실행 가능 여부 변경시 - CanExecuteChanged
        /// <summary>
        /// 실행 가능 여부 변경시
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private
        #region Field
        /// <summary>
        /// 실행 액션
        /// </summary>
        private readonly Action<object> executeAction;
        /// <summary>
        /// 실행 가능 여부 프레디킷
        /// </summary>
        private readonly Predicate<object> canExecutePredicate;
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public
        #region 생성자 - RelayCommand(executeAction, canExecutePredicate)
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="executeAction">실행 액션</param>
        /// <param name="canExecutePredicate">실행 가능 여부 프레디킷</param>
        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate)
        {
            if(executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }
            this.executeAction       = executeAction;
            this.canExecutePredicate = canExecutePredicate;
        }
        #endregion
        #region 생성자 - RelayCommand(executeAction)
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="executeAction">실행 액션</param>
        public RelayCommand(Action<object> executeAction) : this(executeAction, null)
        {
        }
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Public
        #region 실행하기 - Execute(parameter)
        /// <summary>
        /// 실행하기
        /// </summary>
        /// <param name="parameter">파라미터</param>
        public void Execute(object parameter)
        {
            this.executeAction(parameter);
        }
        #endregion
        #region 실행 가능 여부 구하기 - CanExecute(parameter)
        /// <summary>
        /// 실행 가능 여부 구하기
        /// </summary>
        /// <param name="parameter">파라미터</param>
        /// <returns>실행 가능 여부</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecutePredicate == null ? true : this.canExecutePredicate(parameter);
        }
        #endregion
    }
//출처: https://icodebroker.tistory.com/3142 [ICODEBROKER]
}
