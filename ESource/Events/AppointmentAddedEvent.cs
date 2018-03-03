using ESource.Base;
using System;

namespace ESource.Events
{

    public class AppointmentAddedEvent : Event
    {
        public AppointmentAddedEvent(Guid id)
        {
            AggregateId = id;
        }

        public AppointmentAddedEvent()
        {

        }
    }
}
