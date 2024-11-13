using PopProject.Consumer_;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data;

namespace PopProject.input
{
    static class Data_GeumSeongPrint1

    {
        static public List<string> columns = new List<string>();


        static private Dictionary<string, string> dicDatas = new Dictionary<string, string>();

        static public void SetDic(DataTable _dtDatas)
        {
            dicDatas.Clear();

            for(int i= 0; i < columns.Count; i++)
            {
                dicDatas.Add(columns[i].ToString(), _dtDatas.Rows[0][i].ToString());
            }
        }

        static public void SetColumns(DataTable _dtDatas)
        {
            for (int i = 0; i < _dtDatas.Columns.Count; i++)
            {
                columns.Add(_dtDatas.Columns[i].ColumnName);
            }
        }
    }
}
