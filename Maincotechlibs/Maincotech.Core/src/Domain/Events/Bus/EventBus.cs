using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Maincotech.Domain.Events.Bus
{
    /// <summary>
    ///
    /// </summary>
    public class EventBus : DisposableObject, IEventBus
    {
        private readonly IEventAggregator aggregator;
        private readonly Guid id = Guid.NewGuid();
        private readonly ThreadLocal<Queue<object>> messageQueue = new ThreadLocal<Queue<object>>(() => new Queue<object>());
        private readonly MethodInfo publishMethod;
        private ThreadLocal<bool> committed = new ThreadLocal<bool>(() => true);

        public EventBus(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
            publishMethod = (from m in aggregator.GetType().GetMethods()
                             let parameters = m.GetParameters()
                             let methodName = m.Name
                             where methodName == "Publish" &&
                             parameters != null &&
                             parameters.Length == 1
                             select m).First();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                messageQueue.Dispose();
                committed.Dispose();
            }
        }

        #region IBus Members

        public void Clear()
        {
            messageQueue.Value.Clear();
            committed.Value = true;
        }

        public void Publish<TMessage>(TMessage message)
                    where TMessage : class, IEvent
        {
            messageQueue.Value.Enqueue(message);
            committed.Value = false;
        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages)
            where TMessage : class, IEvent
        {
            foreach (var message in messages)
                Publish(message);
        }

        #endregion IBus Members

        #region IUnitOfWork Members

        public bool Committed
        {
            get { return committed.Value; }
        }

        public bool DistributedTransactionSupported
        {
            get { return false; }
        }

        public Guid ID
        {
            get { return id; }
        }

        public void Commit()
        {
            while (messageQueue.Value.Count > 0)
            {
                var evnt = messageQueue.Value.Dequeue();
                var evntType = evnt.GetType();
                var method = publishMethod.MakeGenericMethod(evntType);
                method.Invoke(aggregator, new[] { evnt });
            }
            committed.Value = true;
        }

        public void Rollback()
        {
            Clear();
        }

        #endregion IUnitOfWork Members
    }
}