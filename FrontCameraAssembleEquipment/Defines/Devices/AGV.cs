//using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class AGV
    {
        public bool CallAGV(bool bCall)
        {
            return true;
            //try
            //{
            //    string sqlConnection = @"Data Source=107.124.46.147;Initial Catalog=SW;User ID=sa;Password=123";
            //    using (var con = new SqlConnection(sqlConnection))
            //    {
            //        con.Open();
            //        var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            //        string pan_id = "09";
            //        string address = "2";
            //        var input = bCall;
            //        var strQuery = $"SELECT * FROM S_WIP_CALL WHERE PAN_ID ={pan_id} AND ADDRESS={address}";
            //        using (var cmd = new SqlCommand(strQuery, con))
            //        {
            //            if (cmd.ExecuteScalar() == null)
            //                strQuery = $"INSERT INTO S_WIP_CALL (TIME, PAN_ID, ADDRESS, INPUT1) VALUES ({time}, {pan_id}, {address}, {input})";
            //            else
            //                strQuery = $"UPDATE S_WIP_CALL SET TIME={time}, INPUT1={input} WHERE PAN_ID={pan_id} AND ADDRESS ={address}";
            //            //MSystem.MyMessagerBottom(strQuery);
            //            using (var cmd1 = new SqlCommand(strQuery, con))
            //            {
            //                return cmd1.ExecuteNonQuery() > 0;
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //    return false;
            //}
        }
    }
}
