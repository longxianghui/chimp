using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp
{
    public class KingOptions
    {
        /// <summary>
        /// 数据库类型，目前只支持mssql和mysql
        /// </summary>
        public DbType DbType { get; set; }
        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }
    }

    public enum DbType
    {
        MSSQL,
        MYSQL
    }

}
