using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data; 
using System.Collections;
using System.Data.SqlTypes;
using System.Reflection; 

namespace MKSS.APP.WinNBOnlinePrinterTest
{


    /// <summary>
    ///  数据库操作支持，对象化操作，数据库连接不单独开启，关闭
    /// 宋冠军
    /// </summary>  
    public class DatabaseObject : IDisposable
    {
        static DatabaseObject _main=null;  
        /// <summary>
        /// 中兴数据库
        /// </summary>
        public static DatabaseObject Main {
            get {
                if (_main == null) {
                    string conn = "server=192.168.111.17;Database=mkssdbbiaoding;Uid=root;Pwd=mkss2021;Port=3306;Allow User Variables=True;SslMode=None;";
                    conn = "server=117.160.239.252;Database=mkssdbbiaoding;Uid=root;Pwd=mkss2021;Port=20211;Allow User Variables=True;SslMode=None;";
                    _main = new DatabaseObject(conn);
                }
                return _main;
            }
        }
        public string ConnectionString { get; set;}
        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public DatabaseObject(string strPath) 
        {
            ConnectionString = strPath; 
        }

        #endregion 

        public bool exsitsRecord(string sql)
        {
            
            try
            {
                DataTable aTable = getDataTableFromSql(sql);
                if (aTable != null && aTable.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log(sql);
                throw;
            }
            
        }

        void Log(string st) { 
        
        }

        public int getRecordCount(string sql)
        {
            DataTable aTable = getDataTableFromSql(sql);
            return aTable.Rows.Count;
        }

        public int ExecuteSQL(string sqlstr)
        {
            SessionObject Session = null;
            Session = new SessionObject(this);
            try
            {
                Session.Reconnect();
                MySqlConnection conn = (MySqlConnection)Session.Connection;
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr;
                //Session.Transaction.Enlist(cmd);
                cmd.Prepare();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log(sqlstr);
                Error(ex);
                return -1;
            }
            finally
            {
                Session.Close();
            }
        }

        void Error(Exception ex) {
            Log(ex.Message);
            Log(ex.StackTrace); 
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable getDataTableFromSql(string sql)
        {
            SessionObject Session = null;
            Session = new SessionObject(this);
            MySqlDataReader reader = null;
            String connstr = Session.Connection.ToString();
            try
            {
                /**判断当前数据库为什么类型数据库**/
                if (connstr.IndexOf("Sql") >= 0)
                {

                    Session.Reconnect();
                    MySqlConnection conn = (MySqlConnection)Session.Connection;
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    DataTable dt = new DataTable();
                    //Session.Transaction.Enlist(cmd);
                    cmd.Prepare();
                    cmd.CommandTimeout = 1800;
                    reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    Hashtable table = new Hashtable();
                    //将日期格式化，在0002年以前认为是null
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.DataType == typeof(DateTime))
                        {
                            table.Add(col.ColumnName, col.ColumnName);
                        }
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (string cName in table.Keys)
                        {
                            if (dr[cName] != DBNull.Value)
                            {
                                DateTime d = ConvertDataToDT(dr[cName]);
                                if (d.Year < 2)
                                {
                                    dr[cName] = DBNull.Value;
                                }
                            }
                        }
                    }
                    return dt;
                }

            }
            catch (Exception ex) {
                Log(sql);
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Session.Close();
            }
            return null;

        }


        private DateTime ConvertDataToDT(object obj)
        {
            if (obj == null) return SqlDateTime.MinValue.Value;
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch(Exception ex)
            {
                Error(ex);
                return SqlDateTime.MinValue.Value;
            }

        }

        //public bool update(string sqlstr)
        //{
        //    SessionObject Session = null;
        //    Session = new SessionObject(this);
        //    String connstr = Session.Connection.ToString();
        //    try
        //    {
        //        Session.Reconnect();
        //        MySqlConnection conn = (MySqlConnection)Session.Connection;
        //        //OracleTransaction dbt = conn.BeginTransaction();
        //        MySqlCommand dbc = conn.CreateCommand();
        //        dbc.CommandText = sqlstr;
        //        dbc.CommandType = CommandType.Text;
        //        Session.Transaction.Enlist(dbc);
        //        dbc.Prepare();
        //        if (dbc.ExecuteNonQuery() > 0)
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log(sqlstr);
        //        Error(ex); 
        //    }
        //    finally
        //    {
        //        Session.Close();
        //    }
        //    return false;
        //}

        /**
         * 将IDataReader转化为dataset
         * */
        private DataSet ConvertDataReaderToDataSet(IDataReader reader)
        {
             
            DataSet dataSet = new DataSet();
            do
            {
                bool isReadTable = false;
                isReadTable = reader.Read();
                if (isReadTable)
                {
                    DataTable schemaTable = reader.GetSchemaTable();
                    DataTable dataTable = new DataTable();

                    if (schemaTable != null)
                    {
                        for (int i = 0; i < schemaTable.Rows.Count; i++)
                        {
                            DataRow dataRow = schemaTable.Rows[i];
                            string columnName = (string)dataRow["ColumnName"];
                            DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                            dataTable.Columns.Add(column);
                        }

                        dataSet.Tables.Add(dataTable);

                        while (isReadTable)
                        {
                            DataRow dataRow = dataTable.NewRow();
                            for (int j = 0; j < reader.FieldCount; j++)
                                dataRow[j] = reader.GetValue(j);

                            dataTable.Rows.Add(dataRow);
                            isReadTable = reader.Read();
                        }

                    }
                    else
                    {
                        DataColumn column = new DataColumn("RowsAffected");
                        dataTable.Columns.Add(column);
                        dataSet.Tables.Add(dataTable);
                        DataRow dataRow = dataTable.NewRow();
                        dataRow[0] = reader.RecordsAffected;
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            while (reader.NextResult());
            return dataSet;
        }



        public Object[] splitData(String sqlstr, int pagesize, int curpage)
        {
            return this.splitData(sqlstr, "", pagesize, curpage);
        }


        public Object[] splitData(String sqlstr, string attachConditional, int pagesize, int curpage)
        {
            SessionObject Session = null;
            Session = new SessionObject(this);
            //Hashtable ht = new Hashtable();
            Object[] objs = null;
            String connstr = Session.Connection.ToString();
            try
            {
                Session.Reconnect();
                MySqlConnection conn = (MySqlConnection)Session.Connection;
                objs = splitForSQL(sqlstr, attachConditional, pagesize, curpage, conn);
            }
            finally
            {
                Session.Close();
            }
            return objs;
        }

        /**进行Oracle数据库存储过程操作**/
        private Object[] splitForSQL(String sqlstr, string attachConditional, int pagesize, int curpage, MySqlConnection dbconn)
        {
            //Hashtable ht = new Hashtable();
            Object[] objs = new object[2];

            MySqlCommand comm = new MySqlCommand("SplitData", dbconn);
            comm.CommandType = CommandType.StoredProcedure;

            comm.Parameters.Add(new MySqlParameter("@sqlstr", MySqlDbType.VarChar, 4000));
            comm.Parameters["@sqlstr"].Value = sqlstr + attachConditional;

            comm.Parameters.Add(new MySqlParameter("@currentpage", MySqlDbType.Int32));
            comm.Parameters["@currentpage"].Value = curpage;

            comm.Parameters.Add(new MySqlParameter("@pagesize", MySqlDbType.Int32));
            comm.Parameters["@pagesize"].Value = pagesize;

            comm.Parameters.Add(new MySqlParameter("@PageCount", MySqlDbType.Int32));
            comm.Parameters["@PageCount"].Direction = ParameterDirection.Output;


            comm.Prepare();
            MySqlDataReader dr = comm.ExecuteReader();
            DataSet ds = ConvertDataReaderToDataSet(dr);

            int maxpage = int.Parse(comm.Parameters["@PageCount"].Value.ToString());

            objs[0] = maxpage;
            objs[1] = ds;


            return objs;
        }

        DataTable ReturnDataTable(string sql)
        {
            return getDataTableFromSql(sql);
        }
        public DateTime Now()
        {
            DataTable dt = getDataTableFromSql("select getdate()");
            return Convert.ToDateTime(dt.Rows[0][0]);
        }

        public int ReturnInt32(string TableName, string IdName)
        {
            DataTable dt = ReturnDataTable("select " + IdName + " from " + TableName + "");
            if (dt.Rows.Count > 0) return dt.Rows[0][0] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            return 0;
        }
        public int ReturnInt32(string sql)
        {
            DataTable dt = ReturnDataTable(sql);
            if (dt.Rows.Count > 0) return dt.Rows[0][0] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            return 0;
        }
        public List<T> QueryList<T>(string sql)
        {
            DataTable dt = ReturnDataTable(sql);
            List<T> ret = new List<T>();
            foreach (DataRow dr in dt.Rows) ret.Add(ConvertEntity<T>(dr));
            return ret;
        }

        public T Query<T>(string sql)
        {
            DataTable dt = ReturnDataTable(sql);
            if (dt.Rows.Count == 0) return default(T);
            return ConvertEntity<T>(dt.Rows[0]);
        }

        public T QueryByKey<T>(object id_value)
        {
            Type type = typeof(T);
            object[] attrs = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attrs.Length == 0) return default(T);
            TableAttribute t_attr = attrs[0] as TableAttribute;
            if (string.IsNullOrEmpty(t_attr.PrimaryKey)) return default(T);
            string idv = "";

            if (id_value.GetType() == typeof(string)) idv = "'" + id_value + "'";
            else idv = "" + id_value + "";
            string sqlWhere = t_attr.PrimaryKey + "=" + idv;
            return Query<T>("select * from " + type.Name + " where " + sqlWhere);
        }

        public T ConvertEntity<T>(DataRow dr)
        {
            Type type = typeof(T);
            object ret = type.Assembly.CreateInstance(type.FullName);
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.CanRead && p.CanWrite)
                {
                    object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (attrs.Length > 0)
                    {
                        try
                        {
                            object dr_value = dr[p.Name];
                            if (dr_value == DBNull.Value) continue;
                            object property_value = Convert.ChangeType(dr_value, p.PropertyType);
                            p.SetValue(ret, property_value, null);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            return (T)ret;
        }

        public int Insert<T>(T t)
        {
            List<T> list = new List<T>();
            list.Add(t);
            return Insert<T>(list);
        }
        public int Insert<T>(List<T> list)
        {

            Type type = typeof(T);
            PropertyInfo FIELD_PK = null;
            object[] attrs_table = type.GetCustomAttributes(typeof(TableAttribute), false);
            TableAttribute t_attr = attrs_table[0] as TableAttribute;

            DataTable dt = new DataTable();
            IList<string> maplist = new List<string>();
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.CanRead && p.CanWrite)
                {
                    object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (attrs.Length > 0)
                    {
                        maplist.Add(p.Name);
                        dt.Columns.Add(new DataColumn() { ColumnName = p.Name, DataType = p.PropertyType });
                        if (p.Name == t_attr.PrimaryKey) FIELD_PK = p;
                    }
                }
            }

            foreach (T t in list)
            {

                DataRow dr = dt.NewRow();
                foreach (PropertyInfo p in type.GetProperties())
                {
                    if (p.CanRead && p.CanWrite)
                    {
                        object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false);
                        if (attrs.Length > 0)
                        {
                            object value = p.GetValue(t, null);
                            ColumnAttribute colt_attr = attrs[0] as ColumnAttribute;

                            if (FIELD_PK != null && p.Name == FIELD_PK.Name && p.PropertyType == typeof(string) && string.IsNullOrEmpty((string)value))
                            {
                                //主键为空字符串，自动生成
                                if (colt_attr.Size >= 32) p.SetValue(t, Guid.NewGuid().ToString("N"), null);
                                value = p.GetValue(t, null);
                            }

                            if (
                                    (p.PropertyType == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
                                    ||
                                    (p.PropertyType == typeof(long) && (long)value == long.MinValue)
                                    ||
                                    (p.PropertyType == typeof(int) && (int)value == int.MinValue)
                                    ||
                                    (p.PropertyType == typeof(double) && (double)value == double.MinValue)
                                    ||
                                    (p.PropertyType == typeof(decimal) && (decimal)value == decimal.MinValue)
                                    )
                            {
                                dr[p.Name] = DBNull.Value;
                            }
                            else
                            {
                                dr[p.Name] = value;
                            }
                        }
                    }
                }
                dt.Rows.Add(dr);
            }

            bool sucess = false;// SqlBulkCopyImport(maplist, type.Name, dt);
            return sucess ? dt.Rows.Count : 0;
        }


        public bool Delete<T>(T t)
        {

            Type type = typeof(T);
            object[] attrs = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attrs.Length == 0) return false;
            TableAttribute t_attr = attrs[0] as TableAttribute;
            if (string.IsNullOrEmpty(t_attr.PrimaryKey)) return false;
            string idv = "";

            PropertyInfo id_property = type.GetProperty(t_attr.PrimaryKey);
            object id_value = id_property.GetValue(t, null);
            if (id_value.GetType() == typeof(string)) idv = "'" + id_value + "'";
            else idv = "" + id_value + "";
            string sqlWhere = t_attr.PrimaryKey + "=" + idv;
            return this.ExecuteSQL("delete from " + type.Name + " where " + sqlWhere) > 0;

        }


        //public bool Update<T>(T t)
        //{

        //    Type type = typeof(T);
        //    object[] attrs = type.GetCustomAttributes(typeof(TableAttribute), false);
        //    if (attrs.Length == 0) return false;
        //    TableAttribute t_attr = attrs[0] as TableAttribute;
        //    if (string.IsNullOrEmpty(t_attr.PrimaryKey)) return false;
        //    string idv = "";

        //    PropertyInfo id_property = type.GetProperty(t_attr.PrimaryKey);
        //    object id_value = id_property.GetValue(t, null);
        //    if (id_value.GetType() == typeof(string)) idv = "'" + id_value + "'";
        //    else idv = "" + id_value + "";
        //    string sqlWhere = t_attr.PrimaryKey + "=" + idv;
        //    SessionObject Session = null;
        //    Session = new SessionObject(this);
        //    MySqlConnection conn = (MySqlConnection)Session.Connection;
        //    Session.Reconnect();
        //    using (SqlBulkCopy bulkCopy = new MySqlBulkCopy(conn))
        //    {
        //        DataSet ds = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter("select * from " + type.Name + " where " + sqlWhere, conn as MySqlConnection);
        //        MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
        //        //if (!session.InTransaction)
        //        //{
        //        //    session.BeginTransaction();
        //        //}
        //        Session.Transaction.Enlist(adapter.SelectCommand);
        //        adapter.Fill(ds);

        //        DataTable dt = ds.Tables[0];
        //        dt.AcceptChanges();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            foreach (PropertyInfo p in type.GetProperties())
        //            {
        //                if (p.CanRead && p.CanWrite && dt.Columns.Contains(p.Name) && p.Name != t_attr.PrimaryKey)
        //                {
        //                    object value = p.GetValue(t, null);
        //                    if (
        //                            (p.PropertyType == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
        //                            ||
        //                            (p.PropertyType == typeof(long) && (long)value == long.MinValue)
        //                            ||
        //                            (p.PropertyType == typeof(int) && (int)value == int.MinValue)
        //                            ||
        //                            (p.PropertyType == typeof(double) && (double)value == double.MinValue)
        //                            ||
        //                            (p.PropertyType == typeof(decimal) && (decimal)value == decimal.MinValue)
        //                            )
        //                    {
        //                        dr[p.Name] = DBNull.Value;
        //                    }
        //                    else
        //                    {
        //                        dr[p.Name] = value;
        //                    }
        //                }
        //            }

        //            //dr.SetModified();
        //        }
        //        int ret = adapter.Update(dt);

        //        //adapter.SelectCommand.Transaction.Commit();
        //        return ret > 0;


        //    }
        //    Session.Close();

        //}

        //public bool SqlBulkCopyImport(IList<string> maplist, string tableName, DataTable dt)
        //{
        //    SessionObject Session = null;
        //    Session = new SessionObject(this);
        //    try
        //    {

        //        MySqlConnection conn = (MySqlConnection)Session.Connection;
        //        Session.Reconnect();
        //        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
        //        {

        //            SqlDataAdapter adapter = new SqlDataAdapter("select * from " + tableName + "  where 1=0 ", conn as MySqlConnection);
        //            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
        //            //if (!session.InTransaction) {
        //            //    session.BeginTransaction();
        //            //}
        //            Session.Transaction.Enlist(adapter.SelectCommand);
        //            int rowcount = dt.Rows.Count;
        //            dt.AcceptChanges();
        //            for (int n = 0; n < rowcount; n++)
        //            {
        //                dt.Rows[n].SetAdded();
        //            }
        //            //dt.AcceptChanges();
        //            //adapter.UpdateBatchSize = 1000;
        //            int ret = adapter.Update(dt);
        //            //dt.AcceptChanges();  
        //            //session.Transaction.Commit();
        //            return ret > 0;


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Error(ex);
        //        return false;
        //    } 
        //    finally {
        //        Session.Close();
        //    }

        //}




        public void Dispose()
        {

        }

    }

    /// <summary>
    /// 表名: 实体层
    /// 描述: 
    /// 作者: songguanjun
    /// 最后修改时间:2015/7/14 20:31:56
    /// </summary>
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 表名: 实例化 
        /// </summary>
        public ColumnAttribute()
        { }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public int Size { get; set; }
        public bool AllowDBNull { get; set; }
        public string DataType { get; set; }
        public string SystemType { get; set; }
        public string DateCreated { get; set; }
        public string Table { get; set; }
        public bool DeepLoad { get; set; }
        public string FullName { get; set; }
        public string SortName { get; set; }
        public bool IsUnique { get; set; }
        public bool IsForeignKeyMember { get; set; }
        public bool IsPrimaryKeyMember { get; set; }
    }

    [Serializable]
    /// <summary>
    /// 表名: 实体层
    /// 描述: 
    /// 作者: songguanjun
    /// 最后修改时间:2015/7/14 20:31:56
    /// </summary>
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 表名: 实例化 
        /// </summary>
        public TableAttribute()
        { }
        public string Name { get; set; }
        public bool HasPrimaryKey { get; set; }
        public string PrimaryKey { get; set; }
        public string Description { get; set; }
        public string FullName { get; set; }
        public string DateCreated { get; set; }
        public bool DeepLoad { get; set; }
        public string SortName { get; set; }
        public string DatabaseName { get; set; }

    }

     
    public class SessionObject
    { 
        string ConnectionString { get; set; }
        public DatabaseObject DbOperate { get; private set; }
        public  MySqlConnection Connection { get; set; }
        public MySqlTransaction Transaction { get; private set; }
        public bool InTransaction { get { return this.Transaction != null ; } }
        public SessionObject(DatabaseObject _DataGetClassObject)
        {
            ConnectionString = _DataGetClassObject.ConnectionString;
            Connection = new MySqlConnection(ConnectionString);
            Connection.ConnectionString = ConnectionString;
            DbOperate = _DataGetClassObject;
             
        }
        public void Reconnect()
        {
            if (Connection.State != ConnectionState.Open) {
                try
                {
                    Connection.Open(); 
                }
                catch (Exception ex)
                {
                    Log("连接数据库出错:"+ ex.Message);
                    throw ex;
                }
            } 
        }
        void Log(string tr) { 
        
        }
        public MySqlTransaction BeginTransaction()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                if (!this.InTransaction) Transaction = this.Connection.BeginTransaction();
                return Transaction;
            }
            return null;
        }
        public void Disconnect()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                if (this.InTransaction) this.Transaction.Rollback();
            }
        }
        public void Close()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                if (this.InTransaction) this.Transaction.Rollback();
                try
                {
                    Connection.Close();
                }
                catch { }
            }
        }
        ~SessionObject()
        {
            Close();
        }
    }

    public class ITransaction
    {
        public MySqlTransaction Tran { get; set; }
        public bool InTransaction { get; set; }
        public  MySqlConnection Connection { get; set; }
        public ITransaction( MySqlConnection conn)
        {
            Connection = conn;
        }
        public void BeginTransaction()
        {
            Tran = Connection.BeginTransaction();
            InTransaction = true;
        }
        public void Commit()
        {
            if (Tran != null)
            {
                Tran.Commit();
                InTransaction = false;
            }
        }

        public void Rollback()
        {
            if (Tran != null && InTransaction)
            {
                Tran.Rollback();
                InTransaction = false;
            }
        }

        public void Enlist(MySqlCommand cmd)
        {
            if (InTransaction) cmd.Transaction = this.Tran;
        }

    }
}
