using PopProject.MessageBox;
using PopProject.UserControl_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PopProject.Util
{
    class PLC_LS_Manager
    {
        public ActUtlTypeLib.ActUtlType ActUtilType; //MX Component 라이브러리 선언
        public bool PLC_Connection = false; //PLC 통신 연결 상태
        short[] sInt = new short[100];
        private Object thisLock = new Object();
        public void PLC_Conn() //통신 오픈
        {
            MessageBox_Dialog msg;
            int StationNumber = 0;
            try
            {
                //통신 셋팅
                ActUtilType = new ActUtlTypeLib.ActUtlType();
                ActUtilType.ActLogicalStationNumber = StationNumber; //PLC 국번
                //미쯔비시 통신 오픈
                int nRtn = ActUtilType.Open();
                if (nRtn == 0)
                {
                    PLC_Connection = true; //통신 연결 성공
                }
                //실패시
                else
                {
                    msg = new MessageBox_Dialog("로봇 통신 상태를 확인해 주세요.");
                    msg.ShowDialog();
                }
            }
            catch (Exception e)
            {
                LogManager.Error("로봇 통신 에러\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public void SetDevice(string _Memory, int _Value) //로봇 데이터 쓰기
        {
            try
            {
                ActUtilType.SetDevice(_Memory, _Value); //적재 패턴
            }
            catch (Exception e)
            {
                LogManager.Error("로봇 통신 에러\n" + e.Message, MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        #region WriteDeviceBlock 블럭단위 쓰기
        public bool WriteDeviceBlock(string _sAdd, string _str)
        {


            if (_str == "")
            {
                sInt = new short[10];
                Array.Clear(sInt, 0, sInt.Length);
                bool bCheck1 = WriteDeviceBlock2(_sAdd, sInt.Length, ref sInt[0]);
                return bCheck1;
            }
            //if(!_str.Contains(Option.Save_Path)) return false ;
            //
            ////
            ////string sStr = @"D:\DWAMSaveImage\2020Y\12M\17D\131511_K30" ;
            //int    iStt = _str.IndexOf(Option.Save_Path) ;
            //int    iLen = Option.Save_Path.Length;
            //string sStr = _str.Substring(iStt + iLen,20);

            string str = _str;
            string[] str_temp;

            if (str.Length % 2 == 0)
            {
                str_temp = new string[str.Length / 2];
                sInt = new short[str_temp.Length];

                for (int i = 0; i < str.Length / 2; i++)
                {
                    str_temp[i] = str.Substring(i * 2, 2);
                }

                for (int i = 0; i < str_temp.Length; i++)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str_temp[i]);
                    short sh = BitConverter.ToInt16(bytes, 0);
                    sInt[i] = sh;
                }
            }
            else
            {
                str_temp = new string[(str.Length / 2) + 1];
                sInt = new short[str_temp.Length];

                for (int i = 0; i < str.Length / 2 + 1; i++)
                {
                    if (i < (str.Length - 1) / 2)
                        str_temp[i] = str.Substring(i * 2, 2);
                    else
                        str_temp[i] = str.Substring(i * 2, 1);
                }

                for (int i = 0; i < str_temp.Length; i++)
                {
                    if (i < str_temp.Length - 1)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(str_temp[i]);
                        short sh = BitConverter.ToInt16(bytes, 0);
                        sInt[i] = sh;
                    }
                    else
                    {
                        char data = Convert.ToChar(str_temp[i].Substring(0, 1));
                        sInt[i] = (short)data;
                    }
                }
            }

            bool bCheck2 = WriteDeviceBlock2(_sAdd, sInt.Length, ref sInt[0]);
            return bCheck2;

        }
        public bool WriteDeviceBlock2(string szDevice, int iSize, ref short iData)
        {
            if (!PLC_Connection) return false;

            int iRst = -1;

            lock (thisLock)
            {
                try
                {
                    iRst = ActUtilType.WriteDeviceBlock2(szDevice, iSize, ref iData);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
            if (iRst == 0) return true;

            return false;
        }

        #endregion

        #region ReadDeviceBlock 블럭단위 읽기
        public bool ReadDeviceBlock(string _sAdd, out string _str)
        {
            sInt = new short[10];
            Array.Clear(sInt, 0, sInt.Length);
            bool bCheck = ReadDeviceBlock2(_sAdd, sInt.Length, out sInt[0]);

            _str = "";
            if (bCheck)
            {
                for (int i = 0; i < sInt.Length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(sInt[i]);
                    _str += Encoding.Default.GetString(bytes);
                }
            }

            return bCheck;

            //_str = "" ;
            //Array.Clear(sInt, 0, sInt.Length);
            //bool bCheck = ReadDeviceBlock2(_sAdd, sInt.Length,out sInt[0]);
            //for (int i = 0; i < sInt.Length; i++)
            //{
            //    if(sInt[i] != 0) _str += Convert.ToChar(sInt[i]);
            //}
            //return bCheck ;
        }
        public bool ReadDeviceBlock2(string szDevice, int iSize, out short lplData)
        {
            lplData = 0;

            if (!PLC_Connection) return false;

            int iRst = -1;

            lock (thisLock)
            {
                try
                {
                    iRst = ActUtilType.ReadDeviceBlock2(szDevice, iSize, out lplData);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
            if (iRst == 0) return true;

            return false;
        }
        #endregion
    }
}
