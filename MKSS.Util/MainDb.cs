using SqlSugar;

namespace MKSS.Util
{
    public static class MainDb
    {
        public static string CurrentDbConnId = "1";

        public static ConnectionConfig MainConnectionReplace { get; set; }
        public static SqlSugarClient GetDbClient()
        {
            if (MainConnectionReplace != null) {
                return new SqlSugarClient(MainConnectionReplace);
            }
            var baseDB = BaseDBConfig.GetMainConnectionDb();

            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = baseDB.Connection,//必填, 数据库连接字符串
                DbType = (DbType)baseDB.DbType,//必填, 数据库类型
                IsAutoCloseConnection = true,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
                IsShardSameThread = true,//共享线程
                InitKeyType = InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
            });

            return sqlSugarClient;
        }

        public static SqlSugarClient GetDbClientAttribute()
        {
            var baseDB = BaseDBConfig.GetMainConnectionDb();

            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = baseDB.Connection,//必填, 数据库连接字符串
                DbType = (DbType)baseDB.DbType,//必填, 数据库类型
                IsAutoCloseConnection = true,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
                IsShardSameThread = true,//共享线程
                InitKeyType = InitKeyType.Attribute//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
            });

            return sqlSugarClient;
        }

    }

}
