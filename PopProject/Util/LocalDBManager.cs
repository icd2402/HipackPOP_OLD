using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PopProject.Util
{
    public class LocalDBManager
    {
        string connectString;

        public LocalDBManager()
        {
            connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};",
                                                 "192.168.123.213,1433,", "POP_RawData", "Packcom", "Icd2402");
        }

        public void SetRawData(string dbCode, string mercName, string msg, string orderKey, string prodID)
        {
            //DB로 SET
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Add_RawData]", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter iDbCode    = new SqlParameter("@DBCode", SqlDbType.VarChar);
                SqlParameter iMercName  = new SqlParameter("@MercName", SqlDbType.NVarChar);
                SqlParameter iMsg       = new SqlParameter("@Msg", SqlDbType.NVarChar);
                SqlParameter iOrderKey  = new SqlParameter("@OrderKey", SqlDbType.Int);
                SqlParameter iProdID    = new SqlParameter("@ProdID", SqlDbType.Int);

                iDbCode.Value = dbCode;
                iMercName.Value = mercName;
                iMsg.Value = msg;
                iOrderKey.Value = orderKey;
                iProdID.Value = prodID;
                
                cmd.Parameters.Add(iDbCode);
                cmd.Parameters.Add(iMercName);
                cmd.Parameters.Add(iMsg);
                cmd.Parameters.Add(iOrderKey);
                cmd.Parameters.Add(iProdID);

                //프로시저실행
                cmd.ExecuteNonQuery();
            }
        }

        //DB로 Get
        public DataTable GetRawData(string msgType = null)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("[dbo].[Sel_RawData]", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (msgType != null)
                {
                    SqlParameter inputMsgType = new SqlParameter("@msgType", SqlDbType.VarChar);
                    inputMsgType.Value = msgType;
                    cmd.Parameters.Add(inputMsgType);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            return ds.Tables[0];
        }
    }
}