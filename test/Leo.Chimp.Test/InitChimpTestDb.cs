using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.Test
{
    class InitChimpTestDb
    {
        public static void Start(ServiceCollection services,DbType dbType)
        {
            switch (dbType)
            {
                case DbType.MSSQL:
                    services.AddChimp(opt => { opt.UseSqlServer("Server=10.0.0.99;Database=chimp;Uid=sa;Pwd=Fuluerp123"); });
                    break;
                case DbType.MYSQL:
                    services.AddChimp(opt => { opt.UseMySql("Server=192.168.5.5;Database=Chimp;Uid=root;Pwd='luodaoyi';SslMode=none"); ; });
                    //services.AddChimp(opt => { opt.UseMySql("server=10.0.0.146;database=chimp;uid=root;password=123456;"); });
                    break;
                case DbType.SQLITE:
                    services.AddChimp(opt => { opt.UseSqlite($"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\chimp.db;"); });
                    // Sqlite and Guid column
                    Dapper.SqlMapper.AddTypeHandler(new GuidTypeHandler());
                    Dapper.SqlMapper.RemoveTypeMap(typeof(Guid));
                    Dapper.SqlMapper.RemoveTypeMap(typeof(Guid?));
                    break;
                default:
                    break;
            }
        }
    }
}
