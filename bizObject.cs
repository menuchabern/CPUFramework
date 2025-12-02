using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CPUFramework
{
    public class bizObject : INotifyPropertyChanged
    {
        string _tablename = ""; string _getsproc = ""; string _updatesproc = ""; string _deletesproc = ""; string _primarykeyname = ""; string _primarykeyparamname = "";
        DataTable _datatable = new();
        List<PropertyInfo> properties = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public bizObject()
        {
            Type t = this.GetType();
            _tablename = this.GetType().Name;
            if (_tablename.ToLower().StartsWith("biz"))
            {
                _tablename = _tablename.Substring(3);
            }
            _getsproc = _tablename + "Get";
            _updatesproc = _tablename + "Update";
            _deletesproc = _tablename + "Delete";
            _primarykeyname = _tablename + "Id";
            _primarykeyparamname = "@" + _primarykeyname;
            properties = t.GetProperties().ToList<PropertyInfo>();
        }

        public DataTable Load(int primarykeyvalue)
        {
            DataTable dt = new();
            SqlCommand cmd = SQLUtility.GetSQLCommand(_getsproc);
            SQLUtility.SetParamValue(cmd, _primarykeyparamname, primarykeyvalue);
            dt = SQLUtility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                LoadProps(dt.Rows[0]);
            }
            _datatable = dt;
            return dt;
        }

        private void LoadProps(DataRow dr)
        {
            foreach (DataColumn col in dr.Table.Columns)
            {
                string colname = col.ColumnName.ToLower();
                PropertyInfo? prop = properties.FirstOrDefault(p => p.Name.ToLower() == colname && p.CanWrite == true);
                if (prop != null)
                {
                    prop.SetValue(this, dr[colname]);
                }
            }
        }

        public void Delete(DataTable datatable)
        {
            int id = (int)datatable.Rows[0][_primarykeyname];
            SqlCommand cmd = SQLUtility.GetSQLCommand(_deletesproc);
            SQLUtility.SetParamValue(cmd, _primarykeyparamname, id);
            SQLUtility.ExecuteSQL(cmd);
        }

        public void Save()
        {
            SqlCommand cmd = SQLUtility.GetSQLCommand(_updatesproc);
            foreach (SqlParameter param in cmd.Parameters)
            {
                string colname = param.ParameterName.ToLower().Substring(1);
                PropertyInfo? prop = properties.FirstOrDefault(p => p.Name.ToLower() == colname && p.CanRead == true);
                if (prop != null)
                {
                    param.Value = prop.GetValue(this);
                }
            }
            SQLUtility.ExecuteSQL(cmd);
            foreach (SqlParameter param in cmd.Parameters)
            {
                if (param.Direction == ParameterDirection.InputOutput)
                {
                    string colname = param.ParameterName.ToLower().Substring(1);
                    PropertyInfo? prop = properties.FirstOrDefault(p => p.Name.ToLower() == colname && p.CanRead == true);
                    if (prop != null)
                    {
                        prop.SetValue(this, param.Value);
                    }
                }
            }
        }

        public void Save(DataTable datatable)
        {
            if (datatable.Rows.Count == 0)
            {
                throw new Exception($"Cannot call {_tablename} save method because there are no rows in the table");
            }

            DataRow r = datatable.Rows[0];
            SQLUtility.SaveDataRow(r, _updatesproc);
        }
        protected void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}