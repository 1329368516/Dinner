using Dinner.Base.Model;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dinner.Base
{
    public class DataBaseConfig<T> where T : class, new()
    {
        #region SqlServer链接配置

        private static string DefaultSqlConnectionString = AppSettings.AdminConfig.ConnstringSql;
        private static string DefaultRedisString = AppSettings.AdminConfig.ConnectionRedisString;
        private static ConnectionMultiplexer redis;
        public SqlSugarClient Db;
        public ConnectionMultiplexer Redis;
        public DataBaseConfig()
        {
            Db = GetSqlConnection();
            Redis = GetRedis(); 
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };

        }




        private SqlSugarClient GetSqlConnection(string sqlConnectionString = null)
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = DefaultSqlConnectionString,//必填, 数据库连接字符串
                DbType = SqlSugar.DbType.SqlServer,         //必填, 数据库类型
                IsAutoCloseConnection = true,      //默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = InitKeyType.SystemTable    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
            });
        }


        #endregion

        #region Redis链接配置

        private ConnectionMultiplexer GetRedis(string redisString = null)
        {
            if (string.IsNullOrWhiteSpace(redisString))
            {
                redisString = DefaultRedisString;
            }
            if (redis == null || redis.IsConnected)
            {
                redis = ConnectionMultiplexer.Connect(redisString);
            }
            return redis;
        }

        #endregion
    }
}
