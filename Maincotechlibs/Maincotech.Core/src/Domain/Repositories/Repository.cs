using Maincotech.Data;
using Maincotech.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Maincotech.Domain.Repositories
{
    /// <summary>
    /// Represents the base class for repositories.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root on which the repository operations
    /// should be performed.</typeparam>
    public abstract class Repository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : class, IEntity
    {
        #region Private Fields

        private readonly IRepositoryContext _context;

        #endregion Private Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of <c>Repository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        /// <param name="context">The repository context being used by the repository.</param>
        public Repository(IRepositoryContext context)
        {
            this._context = context;
        }

        #endregion Ctor

        #region Protected Methods

        /// <summary>
        /// Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        protected abstract void DoAdd(TAggregateRoot aggregateRoot);

        /// <summary>
        /// Checks whether the aggregate root, which matches the given specification, exists in the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>True if the aggregate root exists, otherwise false.</returns>
        protected abstract bool DoExists(ISpecification<TAggregateRoot> specification);

        protected abstract bool DoExists(FilterCondition filterCondition);

        /// <summary>
        /// Finds a single aggregate root that matches the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected abstract TAggregateRoot DoFind(ISpecification<TAggregateRoot> specification);

        /// <summary>
        /// Finds a single aggregate root that matches the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected abstract TAggregateRoot DoFind(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties);

        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <returns>All the aggregate roots got from the repository.</returns>
        protected virtual IEnumerable<TAggregateRoot> DoFindAll()
        {
            return DoFindAll(new AnySpecification<TAggregateRoot>(), null, SortOrder.None);
        }

        /// <summary>
        /// Finds all the aggregate roots from repository, sorting by using the provided sort predicate
        /// and the specified sort order.
        /// </summary>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>All the aggregate roots got from the repository, with the aggregate roots being sorted by
        /// using the provided sort predicate and the sort order.</returns>
        protected virtual IEnumerable<TAggregateRoot> DoFindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return DoFindAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder);
        }

        /// <summary>
        /// Finds all the aggregate roots from repository with paging enabled, sorting by using the provided sort predicate
        /// and the specified sort order.
        /// </summary>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots got from the repository, with the aggregate roots being sorted by
        /// using the provided sort predicate and the sort order.</returns>
        protected virtual PagedResult<TAggregateRoot> DoFindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return DoFindAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder, pageNumber, pageSize);
        }

        /// <summary>
        /// Finds all the aggregate roots that match the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <returns>All the aggregate roots that match the given specification.</returns>
        protected virtual IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification)
        {
            return DoFindAll(specification, null, SortOrder.None);
        }

        /// <summary>
        /// Gets all the aggregate roots from repository.
        /// </summary>
        /// <returns>All the aggregate roots got from the repository.</returns>
        protected virtual IEnumerable<TAggregateRoot> DoFindAll(params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(new AnySpecification<TAggregateRoot>(), null, SortOrder.None, eagerLoadingProperties);
        }

        /// <summary>
        /// Gets all the aggregate roots from repository, sorting by using the provided sort predicate
        /// and the specified sort order.
        /// </summary>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>All the aggregate roots got from the repository, with the aggregate roots being sorted by
        /// using the provided sort predicate and the sort order.</returns>
        protected virtual IEnumerable<TAggregateRoot> DoFindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder, eagerLoadingProperties);
        }

        /// <summary>
        /// Gets all the aggregate roots from repository with paging enabled, sorting by using the provided sort predicate
        /// and the specified sort order.
        /// </summary>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots got from the repository for the specified page, with the aggregate roots being sorted by
        /// using the provided sort predicate and the sort order.</returns>
        protected virtual PagedResult<TAggregateRoot> DoFindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder, pageNumber, pageSize, eagerLoadingProperties);
        }

        /// <summary>
        /// Gets all the aggregate roots that match the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <returns>All the aggregate roots that match the given specification.</returns>
        protected virtual IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(specification, null, SortOrder.None, eagerLoadingProperties);
        }

        /// <summary>
        /// Finds all the aggregate roots that match the given specification, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        protected abstract IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder);

        /// <summary>
        /// Finds all the aggregate roots that match the given specification with paging enabled, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The number of objects per page.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        protected abstract PagedResult<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);

        /// <summary>
        /// Gets all the aggregate roots that match the given specification, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        protected abstract IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties);

        /// <summary>
        /// Gets all the aggregate roots that match the given specification with paging enabled, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        protected abstract PagedResult<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties);

        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected abstract TAggregateRoot DoGetByKey(Guid key);

        protected abstract IQueryable<TAggregateRoot> DoGetAll(SortGroup sortGroup = null, FilterCondition filterCondition = null, Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties = null);

        protected abstract PagedResult<TAggregateRoot> DoGetPagedResult(PagingQuery pagingQuery, Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties = null);

        /// <summary>
        /// Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        protected abstract void DoRemove(TAggregateRoot aggregateRoot);

        protected abstract void DoRemoveAll(ISpecification<TAggregateRoot> specification);

        protected abstract void DoRemoveAll(FilterCondition filterCondition);

        /// <summary>
        /// Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        protected abstract void DoUpdate(TAggregateRoot aggregateRoot);

        #endregion Protected Methods

        #region IRepository<TAggregateRoot> Members

        /// <summary>
        /// Gets the <see cref="Repositories.IRepositoryContext"/> instance.
        /// </summary>
        public IRepositoryContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        public void Add(TAggregateRoot aggregateRoot)
        {
            DoAdd(aggregateRoot);
        }

        /// <summary>
        /// Checkes whether the aggregate root, which matches the given specification, exists in the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>True if the aggregate root exists, otherwise false.</returns>
        public bool Exists(ISpecification<TAggregateRoot> specification)
        {
            return DoExists(specification);
        }

        /// <summary>
        /// Finds a single aggregate root that matches the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>The instance of the aggregate root.</returns>
        public TAggregateRoot Find(ISpecification<TAggregateRoot> specification)
        {
            return DoFind(specification);
        }

        public TAggregateRoot Find(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFind(specification, eagerLoadingProperties);
        }

        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <returns>All the aggregate roots got from the repository.</returns>
        public IEnumerable<TAggregateRoot> FindAll()
        {
            return DoFindAll();
        }

        /// <summary>
        /// Finds all the aggregate roots from repository, sorting by using the provided sort predicate
        /// and the specified sort order.
        /// </summary>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>All the aggregate roots got from the repository, with the aggregate roots being sorted by
        /// using the provided sort predicate and the sort order.</returns>
        public IEnumerable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return DoFindAll(sortPredicate, sortOrder);
        }

        /// <summary>
        /// Finds all the aggregate roots that match the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <returns>All the aggregate roots that match the given specification.</returns>
        public IEnumerable<TAggregateRoot> FindAll(ISpecification<TAggregateRoot> specification)
        {
            return DoFindAll(specification);
        }

        /// <summary>
        /// Finds all the aggregate roots that match the given specification, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        public IEnumerable<TAggregateRoot> FindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return DoFindAll(specification, sortPredicate, sortOrder);
        }

        /// <summary>
        /// Finds all the aggregate roots from repository with paging enabled, sorting by using the provided sort predicate
        /// and the specified sort order.
        /// </summary>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots got from the repository, with the aggregate roots being sorted by
        /// using the provided sort predicate and the sort order.</returns>
        public PagedResult<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return DoFindAll(sortPredicate, sortOrder, pageNumber, pageSize);
        }

        /// <summary>
        /// Finds all the aggregate roots that match the given specification with paging enabled, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The number of objects per page.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        public PagedResult<TAggregateRoot> FindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return DoFindAll(specification, sortPredicate, sortOrder, pageNumber, pageSize);
        }

        public PagedResult<TAggregateRoot> FindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(specification, sortPredicate, sortOrder, pageNumber, pageSize, eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> FindAll(params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(sortPredicate, sortOrder, eagerLoadingProperties);
        }

        public PagedResult<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(sortPredicate, sortOrder, pageNumber, pageSize, eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> FindAll(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(specification, eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> FindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return DoFindAll(specification, sortPredicate, sortOrder, eagerLoadingProperties);
        }

        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        public TAggregateRoot GetByKey(Guid key)
        {
            return DoGetByKey(key);
        }

        /// <summary>
        /// Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        public void Remove(TAggregateRoot aggregateRoot)
        {
            DoRemove(aggregateRoot);
        }

        public void RemoveAll(ISpecification<TAggregateRoot> specification)
        {
            DoRemoveAll(specification);
        }

        public void RemoveAll(FilterCondition filterCondition)
        {
            DoRemoveAll(filterCondition);
        }

        /// <summary>
        /// Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        public void Update(TAggregateRoot aggregateRoot)
        {
            DoUpdate(aggregateRoot);
        }

        public IQueryable<TAggregateRoot> GetAll(SortGroup sortGroup = null, FilterCondition filterCondition = null, Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties = null)
        {
            return DoGetAll(sortGroup, filterCondition, eagerLoadingProperties);
        }

        public bool Exists(FilterCondition filterCondition)
        {
            return DoExists(filterCondition);
        }

        public PagedResult<TAggregateRoot> GetPagedResult(PagingQuery pagingQuery, Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties = null)
        {
            return DoGetPagedResult(pagingQuery, eagerLoadingProperties);
        }

        public void AddOrUpdate(TAggregateRoot aggregateRoot)
        {
            var exists = Exists(DomainObjectSpecifications.Id<TAggregateRoot>(aggregateRoot.Id));
            if (exists)
            {
                Update(aggregateRoot);
            }
            else
            {
                Add(aggregateRoot);
            }
        }

        #endregion IRepository<TAggregateRoot> Members
    }
}