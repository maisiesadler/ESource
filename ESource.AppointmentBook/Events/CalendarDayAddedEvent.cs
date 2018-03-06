using ESource.Base;
using System;

namespace ESource.AppointmentBook.Events
{
    public class CalendarDayAddedEvent : Event
    {
        public Day Day { get; set; }

        public CalendarDayAddedEvent(Guid id, Day day)
        {
            AggregateId = id;
            Day = day;
        }

        public CalendarDayAddedEvent()
        {

        }
    }
}
