using ESource.Base;
using System;

namespace ESource
{
    public class EventSourcedRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStore _storage;

        public EventSourcedRepository(IEventStore storage)
        {
            _storage = storage;
        }

        public T GetById(Guid id)
        {
            var t = new T();
            t.LoadsFromHistory(_storage.GetEventsForAggregate(id));
            return t;
        }

        public void Save(T aggregate, int expectedVersion)
        {
            _storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
        }
    }
}
