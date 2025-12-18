using System;
using System.IO;
using MKSS.APP.WinNBTester;
using SqlSugar;

namespace MKSS.Model
{
    public partial class MysqlDB
    {
        public static SqlSugarClient Db { get; set; }
        public static SqlSugarClient GetDB()
        {
            if (Db != null) return Db;

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "server=192.168.111.17;Database=mkssdbbiaoding;Uid=root;Pwd=mkss2021;Port=3306;Allow User Variables=True;",
                DbType = SqlSugar.DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
            db.Ado.IsEnableLogEvent = false;
            Db = db;

            return Db;
        }
         
        public static SqlSugarClient DBProduct { get; set; }
        public static SqlSugarClient GetDBProduct()
        {
            if (DBProduct != null) return DBProduct;
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "server=192.168.111.17;Database=mkss_product_infomation;Uid=root;Pwd=mkss2021;Port=3306;Allow User Variables=True;",
                DbType = SqlSugar.DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
            db.Ado.IsEnableLogEvent = false;
            DBProduct = db;
            return DBProduct;
        }

        public static bool QueryDevice(string serialNO, ref Device dev,ref string msg)
        {
            SqlSugarClient _db = MysqlDB.GetDB();

            var _lstClass = _db.Queryable<Device>().Where(w => w.F_SerialNO == serialNO).ToList();
            if (_lstClass.Count == 0)
            {
                msg = "串号未找到！";
                return false;
            }
            else if (_lstClass.Count > 1)
            {
                msg = "串号重复！";
                return false;
            }
            else
            {
                dev = _lstClass[0];
                msg = "已上线！";
                return true;
            }
            return false;
        }
         
         
    }
     

}