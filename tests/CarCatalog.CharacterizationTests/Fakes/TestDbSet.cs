using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CarCatalog.CharacterizationTests.Fakes
{
    /// <summary>
    /// In-memory <see cref="DbSet{TEntity}"/> so the production services can be exercised
    /// without SQL Server LocalDB.
    /// </summary>
    public class TestDbSet<TEntity> : DbSet<TEntity>, IQueryable, IEnumerable<TEntity>
        where TEntity : class
    {
        private readonly ObservableCollection<TEntity> data;
        private readonly IQueryable query;

        public TestDbSet(IEnumerable<TEntity> entities = null)
        {
            data = new ObservableCollection<TEntity>(entities ?? Enumerable.Empty<TEntity>());
            query = data.AsQueryable();
        }

        public override TEntity Add(TEntity entity)
        {
            data.Add(entity);
            return entity;
        }

        public override TEntity Remove(TEntity entity)
        {
            data.Remove(entity);
            return entity;
        }

        public override TEntity Attach(TEntity entity)
        {
            data.Add(entity);
            return entity;
        }

        public override TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local
        {
            get { return data; }
        }

        Type IQueryable.ElementType
        {
            get { return query.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return query.Provider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
