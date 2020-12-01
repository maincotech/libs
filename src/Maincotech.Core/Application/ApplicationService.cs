using Maincotech.Domain;
using Maincotech.Domain.Repositories;
using System;
using System.Collections.Generic;

namespace Maincotech.Application
{
    /// <summary>
    /// 表示应用层服务的抽象类。
    /// </summary>
    public abstract class ApplicationService :  DisposableObject, IApplicationService
    {
        #region Private Fields

        private readonly IRepositoryContext context;

        #endregion Private Fields

        #region Ctor

        /// <summary>
        /// 初始化一个<c>ApplicationService</c>类型的实例。
        /// </summary>
        /// <param name="context">用来初始化<c>ApplicationService</c>类型的仓储上下文实例。</param>
        public ApplicationService(IRepositoryContext context)
        {
            this.context = context;
        }

        #endregion Ctor

        #region Protected Properties

        /// <summary>
        /// 获取当前应用层服务所使用的仓储上下文实例。
        /// </summary>
        protected IRepositoryContext Context
        {
            get { return context; }
        }

        #endregion Protected Properties

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// 判断指定的<see cref="string"/>值是否表示一个<see cref="Guid"/>类型的空值。
        /// </summary>
        /// <param name="s"><see cref="String"/>值</param>
        /// <returns>如果该值表示一个<see cref="Guid"/>类型的空值，则返回true，否则返回false。</returns>
        protected bool IsEmptyGuidString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return true;
            var guid = new Guid(s);
            return guid == Guid.Empty;
        }

        /// <summary>
        /// 处理简单的聚合创建逻辑。
        /// </summary>
        /// <typeparam name="TDataObjectList">包含数据传输对象的列表类型，比如<see cref="UserDataObjectList"/>等。</typeparam>
        /// <typeparam name="TDataObject">数据传输对象类型，比如<see cref="UserDataObject"/>等。</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根类型。</typeparam>
        /// <param name="dataTransferObjects">包含了一系列数据传输对象的列表实例。</param>
        /// <param name="repository">用于特定聚合根类型的仓储实例。</param>
        /// <param name="processDto">指定用于处理Data Transferring Object类型的函数。</param>
        /// <param name="processAggregateRoot">指定用于处理聚合根类型的函数。</param>
        /// <returns>包含了已创建的聚合的数据的列表。</returns>
        protected TDataObjectList PerformCreateObjects<TDataObjectList, TDataObject, TAggregateRoot>(TDataObjectList dataTransferObjects,
            IRepository<TAggregateRoot> repository,
            Action<TDataObject> processDto = null,
            Action<TAggregateRoot> processAggregateRoot = null)
            where TDataObject : class, new()
            where TDataObjectList : List<TDataObject>, new()
            where TAggregateRoot : class, IEntity, new()
        {
            if (dataTransferObjects == null)
                throw new ArgumentNullException("dataTransferObjects");
            if (repository == null)
                throw new ArgumentNullException("repository");
            TDataObjectList result = null;
            var mapper = AppRuntimeContext.Current.TypeAdapterFactory.Create();
            if (dataTransferObjects.Count > 0)
            {
                var ars = new List<TAggregateRoot>();
                result = new TDataObjectList();
                foreach (var dto in dataTransferObjects)
                {
                    if (processDto != null)
                        processDto(dto);

                    var ar = mapper.Adapt<TDataObject, TAggregateRoot>(dto);
                    if (processAggregateRoot != null)
                        processAggregateRoot(ar);
                    ars.Add(ar);
                    repository.Add(ar);
                }
                repository.Context.Commit();
                ars.ForEach(ar => result.Add(mapper.Adapt<TAggregateRoot, TDataObject>(ar)));
            }
            return result;
        }

        /// <summary>
        /// 处理简单的删除聚合根的操作。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要删除的聚合根的类型。</typeparam>
        /// <param name="ids">需要删除的聚合根的ID值列表。</param>
        /// <param name="repository">应用于指定聚合根类型的仓储实例。</param>
        /// <param name="preDelete">在指定聚合根被删除前，对所需删除的聚合根的ID值进行处理的回调函数。</param>
        /// <param name="postDelete">在指定聚合根被删除后，对所需删除的聚合根的ID值进行处理的回调函数。</param>
        protected void PerformDeleteObjects<TAggregateRoot>(IList<string> ids, IRepository<TAggregateRoot> repository, Action<Guid> preDelete = null, Action<Guid> postDelete = null)
            where TAggregateRoot : class, IEntity
        {
            if (ids == null)
                throw new ArgumentNullException("ids");
            if (repository == null)
                throw new ArgumentNullException("repository");
            foreach (var id in ids)
            {
                var guid = new Guid(id);
                if (preDelete != null)
                    preDelete(guid);
                var ar = repository.GetByKey(guid);
                repository.Remove(ar);
                if (postDelete != null)
                    postDelete(guid);
            }
            repository.Context.Commit();
        }

        /// <summary>
        /// 处理简单的聚合更新操作。
        /// </summary>
        /// <typeparam name="TDataObjectList">包含数据传输对象的列表类型，比如<see cref="UserDataObjectList"/>等。</typeparam>
        /// <typeparam name="TDataObject">数据传输对象类型，比如<see cref="UserDataObject"/>等。</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根类型。</typeparam>
        /// <param name="dataTransferObjects">包含了一系列数据传输对象的列表实例。</param>
        /// <param name="repository">用于特定聚合根类型的仓储实例。</param>
        /// <param name="idFieldFunc">用于获取数据传输对象唯一标识值的回调函数。</param>
        /// <param name="fieldUpdateAction">用于执行聚合更新的回调函数。</param>
        /// <returns>包含了已更新的聚合的数据的列表。</returns>
        protected TDataObjectList PerformUpdateObjects<TDataObjectList, TDataObject, TAggregateRoot>(TDataObjectList dataTransferObjects,
            IRepository<TAggregateRoot> repository,
            Func<TDataObject, string> idFieldFunc,
            Action<TAggregateRoot, TDataObject> fieldUpdateAction)
            where TDataObject : class, new()
            where TDataObjectList : List<TDataObject>, new()
            where TAggregateRoot : class, IEntity
        {
            if (dataTransferObjects == null)
                throw new ArgumentNullException("dataTransferObjects");
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (idFieldFunc == null)
                throw new ArgumentNullException("idFieldFunc");
            if (fieldUpdateAction == null)
                throw new ArgumentNullException("fieldAssignmentAction");
            TDataObjectList result = null;
            var mapper = AppRuntimeContext.Current.TypeAdapterFactory.Create();

            if (dataTransferObjects.Count > 0)
            {
                result = new TDataObjectList();
                foreach (var dto in dataTransferObjects)
                {
                    if (IsEmptyGuidString(idFieldFunc(dto)))
                        throw new ArgumentNullException("ID");
                    var id = new Guid(idFieldFunc(dto));
                    var ar = repository.GetByKey(id);
                    fieldUpdateAction(ar, dto);
                    repository.Update(ar);
                    result.Add(mapper.Adapt<TAggregateRoot, TDataObject>(ar));
                }
                repository.Context.Commit();
            }
            return result;
        }

        #endregion Protected Methods
    }
}