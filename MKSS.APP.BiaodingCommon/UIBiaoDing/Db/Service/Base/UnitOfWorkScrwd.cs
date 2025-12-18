
using MKSS.ICommon;
using MKSS.Service.UIBiaoDing;
using MKSS.Util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace MKSS.Common
{
    public class UnitOfWorkScrwd : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        

        public UnitOfWorkScrwd()
        {
             
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetDbClient()
        {
            //var baseDB = BaseDBConfig.GetMainConnectionDb();

            // 获取加密的链接字符串，然后解密
            DecryptAndEncryptionHelper helper = new DecryptAndEncryptionHelper(ConfigInformation.Key, ConfigInformation.Vector);
            string xxx1 = helper.Encrypto("server=117.160.239.252;Database=mkssdbbiaodinghis;Uid=root;Pwd=mkss2021;Port=20211;Allow User Variables=True;SslMode=None;");
            string xxx2 = helper.Encrypto("server=117.160.239.252;Database=mkssdbbiaoding;Uid=root;Pwd=mkss2021;Port=20211;Allow User Variables=True;SslMode=None;");

            // 明文
            var configStr1 = ConfigurationManager.AppSettings["Connection"];
            if((ConfigurationManager.AppSettings["ConnectionEncrypt"]+"").ToLower()=="true") configStr1 = helper.Decrypto(configStr1);
            var configStr2 = ConfigurationManager.AppSettings["ScrwdConnection"];
            if ((ConfigurationManager.AppSettings["ConnectionEncrypt"] + "").ToLower() == "true") configStr2 = helper.Decrypto(configStr2);


            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = configStr2,//必填, 数据库连接字符串
                DbType = DbType.MySql,//必填, 数据库类型
                IsAutoCloseConnection = true,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
                IsShardSameThread = true,//共享线程
                InitKeyType = InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
            });

            // 连接字符串
            var listConfig = new List<ConnectionConfig>();
            // 从库
            var listConfig_Slave = new List<SlaveConnectionConfig>();

            string connstr = configStr2;// "Data Source=" + BiaoDingSaver.DbNameof(null) + ";Version=3";
            listConfig.Add(new ConnectionConfig()
            {
                ConfigId = "mainn",
                ConnectionString = connstr,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                // Check out more information: https://github.com/anjoy8/Blog.Core/issues/122
                IsShardSameThread = false,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        {
                            Parallel.For(0, 1, e =>
                            {
                                //MiniProfiler.Current.CustomTiming("SQL：", GetParas(p) + "【SQL语句】：" + sql);
                                //LogLock.OutSql2Log("SqlLog", new string[] { GetParas(p), "【SQL语句】：" + sql });
                            });
                        }
                    }
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                },
                // 从库
                SlaveConnectionConfigs = listConfig_Slave,
                // 自定义特性
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {
                        if (column.IsPrimarykey && property.PropertyType == typeof(int))
                        {
                            column.IsIdentity = true;
                        }
                    }
                },
                InitKeyType = InitKeyType.Attribute
            }
               );

            return new SqlSugarClient(listConfig);
            //return sqlSugarClient;
             

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
                Console.WriteLine($"{ex.Message}\r\n{ex.InnerException}");
            }
        }

        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }


        private static string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value}\n";
            }

            return key;
        }

    }

}
