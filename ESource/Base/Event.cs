using System;

namespace ESource.Base
{
    internal interface IEvent
    {
        Guid AggregateId { get; set; }
        int Version { get; set; }
    }
    public class Event
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
    }
}
