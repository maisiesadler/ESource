using ESource.Base;
using System;

namespace ESource.Events
{
    public class AppointmentActivatedEvent : Event
    {
        public AppointmentActivatedEvent(Guid id)
        {
            AggregateId = id;
        }

        public AppointmentActivatedEvent()
        {

        }
    }
}
