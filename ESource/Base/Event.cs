using System;

namespace ESource.Base
{
    public class Event
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
    }
}
