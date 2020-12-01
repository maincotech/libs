using Maincotech.Data;
using Maincotech.Domain;
using Maincotech.Domain.Repositories;
using Maincotech.Domain.Specifications;
using Maincotech.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Maincotech.EF
{
    /// <summary>
    ///     Represents the Entity Framework repository.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    public class EntityFrameworkRepository<TAggregateRoot> : Repository<TAggregateRoot>
        where TAggregateRoot : class, IEntity
    {
        #region Ctor

        /// <summary>
        ///     Initializes a new instance of <c>EntityFrameworkRepository</c> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public EntityFrameworkRepository(IRepositoryContext context)
            : base(context)
        {
            if (context is IEntityFrameworkRepositoryContext)
                EFContext = context as IEntityFrameworkRepositoryContext;
        }

        #endregion Ctor

        #region Protected Properties

        /// <summary>
        ///     Gets the instance of the <see cref="IEntityFrameworkRepositoryContext" />.
        /// </summary>
        protected IEntityFrameworkRepositoryContext EFContext { get; }

        #endregion Protected Properties

        #region Private Fields

        #endregion Private Fields

        #region Private Methods

        private string GetEagerLoadingPath(Expression<Func<TAggregateRoot, dynamic>> eagerLoadingProperty)
        {
            var memberExpression = GetMemberInfo(eagerLoadingProperty);
            var parameterName = eagerLoadingProperty.Parameters.First().Name;
            var memberExpressionStr = memberExpression.ToString();
            var path = memberExpressionStr.Replace(parameterName + ".", "");
            return path;
        }

        private static MemberExpression GetMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        ///     Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        protected override void DoAdd(TAggregateRoot aggregateRoot)
        {
            EFContext.RegisterNew(aggregateRoot);
        }

        /// <summary>
        ///     Checks whether the aggregate root, which matches the given specification, exists in the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>True if the aggregate root exists, otherwise false.</returns>
        protected override bool DoExists(ISpecification<TAggregateRoot> specification)
        {
            var count = EFContext.Context.Set<TAggregateRoot>().Count(specification.IsSatisfiedBy);
            return count != 0;
        }

        protected override bool DoExists(FilterCondition filterCondition)
        {
            IQueryable<TAggregateRoot> query = EFContext.Context.Set<TAggregateRoot>();

            query = query.Filter(filterCondition);

            return query.Any();
        }

        /// <summary>
        ///     Finds a single aggregate root from the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected override TAggregateRoot DoFind(ISpecification<TAggregateRoot> specification)
        {
            return EFContext.Context.Set<TAggregateRoot>().Where(specification.IsSatisfiedBy).FirstOrDefault();
        }

        /// <summary>
        ///     Finds a single aggregate root from the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        protected override TAggregateRoot DoFind(ISpecification<TAggregateRoot> specification,
            params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            var dbset = EFContext.Context.Set<TAggregateRoot>();
            if (eagerLoadingProperties != null &&
                eagerLoadingProperties.Length > 0)
            {
                var eagerLoadingProperty = eagerLoadingProperties[0];
                var eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                var dbquery = dbset.Include(eagerLoadingPath);
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    eagerLoadingProperty = eagerLoadingProperties[i];
                    eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                    dbquery = dbquery.Include(eagerLoadingPath);
                }
                return dbquery.Where(specification.GetExpression()).FirstOrDefault();
            }
            return dbset.Where(specification.GetExpression()).FirstOrDefault();
        }

        /// <summary>
        ///     Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">
        ///     The <see cref="SRSoft.Infrastructure.Data.SortOrder" /> enumeration which specifies the sort
        ///     order.
        /// </param>
        /// <returns>The aggregate roots.</returns>
        protected override IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            var query = EFContext.Context.Set<TAggregateRoot>()
                .Where(specification.GetExpression());
            if (sortPredicate != null)
            {
                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        return query.SortBy(sortPredicate).ToList();

                    case SortOrder.Descending:
                        return query.SortByDescending(sortPredicate).ToList();

                    default:
                        break;
                }
            }
            return query.ToList();
        }

        /// <summary>
        ///     Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">
        ///     The <see cref="SRSoft.Infrastructure.Data.SortOrder" /> enumeration which specifies the sort
        ///     order.
        /// </param>
        /// <param name="pageNumber">The number of objects per page.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>The aggregate roots.</returns>
        protected override PagedResult<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber,
                    "The pageNumber is one-based and should be larger than zero.");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize,
                    "The pageSize is one-based and should be larger than zero.");
            if (sortPredicate == null)
                throw new ArgumentNullException("sortPredicate");

            var query = EFContext.Context.Set<TAggregateRoot>()
                .Where(specification.GetExpression());
            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;

            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    var pagedGroupAscending =
                        query.SortBy(sortPredicate)
                            .Skip(skip)
                            .Take(take)
                            .GroupBy(p => new { Total = query.Count() })
                            .FirstOrDefault();
                    if (pagedGroupAscending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupAscending.Key.Total, pageSize, pageNumber,
                        pagedGroupAscending.Select(p => p).ToList());

                case SortOrder.Descending:
                    var pagedGroupDescending =
                        query.SortByDescending(sortPredicate)
                            .Skip(skip)
                            .Take(take)
                            .GroupBy(p => new { Total = query.Count() })
                            .FirstOrDefault();
                    if (pagedGroupDescending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupDescending.Key.Total, pageSize, pageNumber,
                        pagedGroupDescending.Select(p => p).ToList());

                default:
                    break;
            }

            return null;
        }

        /// <summary>
        ///     Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">
        ///     The <see cref="SRSoft.Infrastructure.Data.SortOrder" /> enumeration which specifies the sort
        ///     order.
        /// </param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        protected override IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder,
            params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            var dbset = EFContext.Context.Set<TAggregateRoot>();
            IQueryable<TAggregateRoot> queryable = null;
            if (eagerLoadingProperties != null &&
                eagerLoadingProperties.Length > 0)
            {
                var eagerLoadingProperty = eagerLoadingProperties[0];
                var eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                var dbquery = dbset.Include(eagerLoadingPath);
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    eagerLoadingProperty = eagerLoadingProperties[i];
                    eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                    dbquery = dbquery.Include(eagerLoadingPath);
                }
                queryable = dbquery.Where(specification.GetExpression());
            }
            else
                queryable = dbset.Where(specification.GetExpression());

            if (sortPredicate != null)
            {
                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        return queryable.SortBy(sortPredicate).ToList();

                    case SortOrder.Descending:
                        return queryable.SortByDescending(sortPredicate).ToList();

                    default:
                        break;
                }
            }
            return queryable.ToList();
        }

        /// <summary>
        ///     Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">
        ///     The <see cref="SRSoft.Infrastructure.Data.SortOrder" /> enumeration which specifies the sort
        ///     order.
        /// </param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        protected override PagedResult<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize,
            params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber,
                    "The pageNumber is one-based and should be larger than zero.");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize,
                    "The pageSize is one-based and should be larger than zero.");
            if (sortPredicate == null)
                throw new ArgumentNullException("sortPredicate");

            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;

            var dbset = EFContext.Context.Set<TAggregateRoot>();
            IQueryable<TAggregateRoot> queryable = null;
            if (eagerLoadingProperties != null &&
                eagerLoadingProperties.Length > 0)
            {
                var eagerLoadingProperty = eagerLoadingProperties[0];
                var eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                var dbquery = dbset.Include(eagerLoadingPath);
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    eagerLoadingProperty = eagerLoadingProperties[i];
                    eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                    dbquery = dbquery.Include(eagerLoadingPath);
                }
                queryable = dbquery.Where(specification.GetExpression());
            }
            else
                queryable = dbset.Where(specification.GetExpression());

            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    var pagedGroupAscending =
                        queryable.SortBy(sortPredicate)
                            .Skip(skip)
                            .Take(take)
                            .GroupBy(p => new { Total = queryable.Count() })
                            .FirstOrDefault();
                    if (pagedGroupAscending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupAscending.Key.Total, pageSize, pageNumber,
                        pagedGroupAscending.Select(p => p).ToList());

                case SortOrder.Descending:
                    var pagedGroupDescending =
                        queryable.SortByDescending(sortPredicate)
                            .Skip(skip)
                            .Take(take)
                            .GroupBy(p => new { Total = queryable.Count() })
                            .FirstOrDefault();
                    if (pagedGroupDescending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupDescending.Key.Total, pageSize, pageNumber,
                        pagedGroupDescending.Select(p => p).ToList());

                default:
                    break;
            }

            return null;
        }

        /// <summary>
        ///     Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected override TAggregateRoot DoGetByKey(Guid key)
        {
            return EFContext.Context.Set<TAggregateRoot>().First(p => p.Id == key);
        }

        protected override IQueryable<TAggregateRoot> DoGetAll(SortGroup sortGroup = null,
            FilterCondition filterCondition = null,
            Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties = null)
        {
            IQueryable<TAggregateRoot> query = EFContext.Context.Set<TAggregateRoot>();
            if (eagerLoadingProperties != null)
            {
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    var eagerLoadingProperty = eagerLoadingProperties[i];
                    var eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                    query = query.Include(eagerLoadingPath);
                }
            }
            if (filterCondition != null)
            {
                query = query.Filter(filterCondition);
            }
            if (sortGroup != null)
            {
                query = query.Sort(sortGroup);
            }
            return query;
        }

        protected override PagedResult<TAggregateRoot> DoGetPagedResult(PagingQuery pagingQuery,
            Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties = null)
        {
            ParameterChecker.ArgumentNotNull(pagingQuery.SortGroup, "sortGroup");
            IQueryable<TAggregateRoot> query = EFContext.Context.Set<TAggregateRoot>();
            if (eagerLoadingProperties != null)
            {
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    var eagerLoadingProperty = eagerLoadingProperties[i];
                    var eagerLoadingPath = GetEagerLoadingPath(eagerLoadingProperty);
                    query = query.Include(eagerLoadingPath);
                }
            }
            if (pagingQuery.FilterCondition != null)
            {
                query = query.Filter(pagingQuery.FilterCondition);
            }
            if (pagingQuery.SortGroup != null)
            {
                query = query.Sort(pagingQuery.SortGroup);
            }
            if (pagingQuery.Pagination.Total.HasValue == false)
            {
                pagingQuery.Pagination.Total = query.Count();
            }
            var skip = pagingQuery.Pagination.Start;
            var take = pagingQuery.Pagination.PageSize;
            query = query.Skip(skip).Take(take);
            var result = new PagedResult<TAggregateRoot>
                (
                pagingQuery.Pagination.Total.Value,
                pagingQuery.Pagination.PageSize,
                pagingQuery.Pagination.PageNumber,
                query.ToList()
                );
            return result;
        }

        /// <summary>
        ///     Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        protected override void DoRemove(TAggregateRoot aggregateRoot)
        {
            EFContext.RegisterDeleted(aggregateRoot);
        }

        /// <summary>
        ///     Removes the aggregate roots from current repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        protected override void DoRemoveAll(ISpecification<TAggregateRoot> specification)
        {
            IQueryable<TAggregateRoot> query = EFContext.Context.Set<TAggregateRoot>().Where(specification.GetExpression());
            EFContext.Context.Set<TAggregateRoot>().RemoveRange(query);
            EFContext.Context.SaveChanges();
        }

        protected override void DoRemoveAll(FilterCondition filterCondition)
        {
            IQueryable<TAggregateRoot> query = EFContext.Context.Set<TAggregateRoot>();
            query = query.Filter(filterCondition);
            //query.d();
            EFContext.Context.Set<TAggregateRoot>().RemoveRange(query);
            EFContext.Context.SaveChanges();
        }

        /// <summary>
        ///     Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        protected override void DoUpdate(TAggregateRoot aggregateRoot)
        {
            EFContext.RegisterModified(aggregateRoot);
        }

        #endregion Protected Methods
    }
}
