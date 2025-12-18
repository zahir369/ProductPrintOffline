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

            if (FormMain.VersionOnline) {
                SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "server=192.168.111.17;Database=mkssdbbiaoding;Uid=root;Pwd=mkss2021;Port=3306;Allow User Variables=True;",
                    DbType = SqlSugar.DbType.MySql,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                });
                db.Ado.IsEnableLogEvent = false;
                Db = db;
            }
            else
            {
                SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "Data Source=" + DbNameof(DateTime.Now.ToString("yyyyMMddHHmmss")) + ";Version=3",
                    DbType = SqlSugar.DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                });
                db.Ado.IsEnableLogEvent = false;
                Db = db;
            }
           
            return Db;
        }


        public static string DbNameof(string time)
        {
      
            string dir = (new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)).FullName + "\\Data";
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            string target = string.Format(string.Format(dir + @"\MKSS.APP.WinNBTester.{0}.db", time));
            if (!System.IO.File.Exists(target)) System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"MKSS.APP.WinNBTester.sqlite", target);
            return target;
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

         
        public static bool CreateDevice( string imei, string imsi, string icccid,ref Device dev) {
            SqlSugarClient _db = MysqlDB.GetDB(); 
            imei = imei.Trim();
            icccid = icccid.Trim();

            if (FormMain.VersionOnline)
            {
                dev = new Device()
                {
                    F_Id = imei,
                    F_IMEI = imei,
                    F_IMSI = imsi,
                    F_ICCID = icccid,
                    F_ProductId = FormMain.Instance.SelectProduct.ProductId,
                    F_ProductName = FormMain.Instance.SelectProduct.ProductName,
                    F_NBProductId = FormMain.Instance.SelectNBProduct.ProductId,
                    F_NBProductName = FormMain.Instance.SelectNBProduct.ProductName,
                    F_Status = (int)DeviceStatus.None,
                    F_CreateDate = DateTime.Now
                };
            }
            else {
                dev = new Device()
                {
                    F_Id = imei,
                    F_IMEI = imei,
                    F_IMSI = imsi,
                    F_ICCID = icccid,
                    //F_ProductId = FormMain.Instance.SelectProduct.ProductId,
                    //F_ProductName = FormMain.Instance.SelectProduct.ProductName,
                    //F_NBProductId = FormMain.Instance.SelectNBProduct.ProductId,
                    //F_NBProductName = FormMain.Instance.SelectNBProduct.ProductName,
                    F_Status = (int)DeviceStatus.None,
                    F_CreateDate = DateTime.Now
                };
            }
    
            var _lstClass = _db.Queryable<Device>().Where(w => w.F_IMEI== imei).ToList();
            if (_lstClass.Count == 0) {
                return _db.Insertable<Device>(dev).ExecuteCommand()>0;
            }
            else {
                return true;
            }
            return false;
        }

        public static bool QueryBaseThirdPartyApp(string imei, string icccid)
        {
            SqlSugarClient _db = MysqlDB.GetDB();
            imei = imei.Trim();
            icccid = icccid.Trim();
            Device dev = new Device()
            {
                F_Id = imei,
                F_IMEI = imei,
                F_ICCID = icccid,
                F_ProductId = FormMain.Instance.SelectProduct.ProductId,
                F_ProductName = FormMain.Instance.SelectProduct.ProductName,
                F_NBProductId = FormMain.Instance.SelectNBProduct.ProductId,
                F_NBProductName = FormMain.Instance.SelectNBProduct.ProductName,
                F_Status = (int)DeviceStatus.None,
                F_CreateDate = DateTime.Now
            };
            var _lstClass = _db.Queryable<Device>().Where(w => w.F_IMEI == imei).ToList();
            if (_lstClass.Count == 0)
            {
                return _db.Insertable<Device>(dev).ExecuteCommand() > 0;
            }
            else
            {
                return true;
            }
            return false;
        }
    }
     

}