
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Leo.Chimp.DapperAdapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace Leo.Chimp
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IUnitOfWork"/> 
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the 
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(DbContext context)
        {
            _context = context;
        }



        public int SaveChanges()
        {
            return _context.SaveChanges();
        }


        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }


        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param = null, IDbContextTransaction trans = null) where TEntity : class
        {
            var conn = GetConnection();
            return conn.QueryAsync<TEntity>(sql, param, trans?.GetDbTransaction());

        }


        public async Task<int> ExecuteAsync(string sql, object param, IDbContextTransaction trans = null)
        {
            var conn = GetConnection();
            return await conn.ExecuteAsync(sql, param, trans?.GetDbTransaction());

        }



        public async Task<PagedList<TEntity>> QueryPagedListAsync<TEntity>(int pageIndex, int pageSize, string pageSql, object pageSqlArgs = null) where TEntity : class
        {
            if (pageSize < 1 || pageSize > 5000)
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            var partedSql = PagingUtil.SplitSql(pageSql);
            ISqlAdapter sqlAdapter = null;
            if (_context.Database.IsMySql())
                sqlAdapter = new MysqlAdapter();
            else if(_context.Database.IsSqlServer())
                sqlAdapter = new SqlServerAdapter();
            else if(_context.Database.IsSqlite())
                sqlAdapter = new SqliteAdapter();
            else
                throw new Exception("Unsupported database type");
            pageSql = sqlAdapter.PagingBuild(ref partedSql, pageSqlArgs, (pageIndex - 1) * pageSize, pageSize);
            var sqlCount = PagingUtil.GetCountSql(partedSql);
            var conn = GetConnection();
            var totalCount = await conn.ExecuteScalarAsync<int>(sqlCount, pageSqlArgs);
            var items = await conn.QueryAsync<TEntity>(pageSql, pageSqlArgs);
            var pagedList = new PagedList<TEntity>(items.ToList(), pageIndex - 1, pageSize, totalCount);
            return pagedList;
        }


        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }



        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public IDbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }
    }


}
