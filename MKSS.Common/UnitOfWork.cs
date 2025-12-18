using Microsoft.Extensions.Logging;
using MKSS.ICommon;
using MKSS.Util;
using SqlSugar;
using System;

namespace MKSS.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<UnitOfWork> _logger;      

        //public UnitOfWork(ISqlSugarClient sqlSugarClient, ILogger<UnitOfWork> logger)
        //{
        //    _sqlSugarClient = sqlSugarClient;
        //    _logger = logger;
        //}

        public UnitOfWork()
        {
            //if (_sqlSugarClient == null)
            //{
            //    var baseDB = BaseDBConfig.GetMainConnectionDb();
            //    var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            //    {
            //        ConnectionString = baseDB.Connection,//必填, 数据库连接字符串
            //        DbType = (DbType)baseDB.DbType,//必填, 数据库类型
            //        IsAutoCloseConnection = true,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
            //        IsShardSameThread = true,//共享线程
            //        InitKeyType = InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
            //    });

            //    _sqlSugarClient = sqlSugarClient;
            //}
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetDbClient()
        {
            //var baseDB = BaseDBConfig.GetMainConnectionDb();
            //var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            //{
            //    ConnectionString = baseDB.Connection,//必填, 数据库连接字符串
            //    DbType = (DbType)baseDB.DbType,//必填, 数据库类型
            //    IsAutoCloseConnection = true,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
            //    IsShardSameThread = true,//共享线程
            //    InitKeyType = InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
            //});
            //return sqlSugarClient;

            // 必须要as，后边会用到切换数据库操作
            // return _sqlSugarClient as SqlSugarClient;

            return MainDb.GetDbClient();
        }

        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran(); 
            }
            catch (Exception ex)
            {
                GetDbClient().RollbackTran();
                _logger.LogError($"{ex.Message}\r\n{ex.InnerException}");
            }
        }

        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }

    }

}
