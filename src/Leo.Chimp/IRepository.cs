
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace Leo.Chimp
{
    /// <summary>
    /// This interface is implemented by all repositories to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {

        #region Select/Get/Query
        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>Entity</returns>
        TEntity GetById(object id);

        /// <summary>
        /// GetByIdAsync
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>Entity</returns>
        ValueTask<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Table Tracking
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// TableNoTracking
        /// </summary>
        IQueryable<TEntity> TableNoTracking { get; }

        #endregion

        #region Insert
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Inserts
        /// </summary>
        /// <param name="entities">Inserted entity</param>
        void Insert(IEnumerable<TEntity> entities);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity);

        /// <summary>
        /// InsertAsync
        /// </summary>
        /// <param name="entities">Inserted entity</param>
        Task InsertAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Update
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="properties">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(TEntity entity);


        /// <summary>
        /// Deletes
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<TEntity> entities);


        /// <summary>
        /// Delete by lambda
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);
        #endregion

    }
}
