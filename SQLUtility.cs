using Microsoft.Data.SqlClient;
using System.Data;

namespace CPUFramework
{
    public class SQLUtility
    {
        public static string ConnectionString = "";

        public static DataTable GetDataTable(string sqlstatement)
        {
            DataTable dt = new();
            SqlConnection conn = new();
            conn.ConnectionString = ConnectionString;
            conn.Open();
            var cmd = new SqlCommand();
            cmd.CommandText = sqlstatement;
            cmd.Connection = conn;
            var dr = cmd.ExecuteReader();
            dt.Load(dr);
            return dt;
        }
    }
}
