﻿using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Data;

namespace CPUFramework
{
    public class SQLUtility
    {
        public static string ConnectionString = "";

        public static DataTable GetDataTable(string sqlstatement)
        {
            Debug.Print(sqlstatement);
            DataTable dt = new();
            SqlConnection conn = new();
            conn.ConnectionString = ConnectionString;
            conn.Open();
            var cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlstatement;
            var dr = cmd.ExecuteReader();
            dt.Load(dr);

            SetAllColumnsAllowNull(dt);

            return dt;
        }

        public static void ExecuteSQL(string sqlstatement)
        {
            GetDataTable(sqlstatement);
        }

        private static void SetAllColumnsAllowNull(DataTable dt)
        {
            foreach(DataColumn c in dt.Columns)
            {
                c.AllowDBNull = true;
            }
        }

        public static void DebugPrintDataTable(DataTable dt) 
        {
            foreach(DataRow r in dt.Rows)
            {
                foreach(DataColumn c in dt.Columns)
                {
                    Debug.Print(c.ColumnName + " = " + r[c.ColumnName].ToString());
                }
            }
        }
    }
}
