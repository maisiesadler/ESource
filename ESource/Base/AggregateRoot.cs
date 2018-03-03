using System;
using System.Collections.Generic;

namespace ESource.Base
{
    public abstract class AggregateRoot
    {
        private List<Event> _uncommittedEvents = new List<Event>();

        public Guid Id { get; protected set; }
        public int Version { get; internal set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _uncommittedEvents;
        }

        public void MarkChangesAsCommitted()
        {
            _uncommittedEvents.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        private void ApplyChange(Event @event, bool isNew)
        {
            Apply(@event);
            if (isNew) _uncommittedEvents.Add(@event);
        }

        protected void Publish(Event e)
        {
            ApplyChange(e);
        }

        protected abstract void Apply(Event @event);
    }
}
