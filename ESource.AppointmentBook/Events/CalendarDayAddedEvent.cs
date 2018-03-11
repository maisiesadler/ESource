using ESource.Base;
using System;

namespace ESource.AppointmentBook.Events
{
    public class CalendarDayAddedEvent : Event
    {
        public DayOfWeek Day { get; set; }

        public CalendarDayAddedEvent(Guid id, DayOfWeek day)
        {
            AggregateId = id;
            Day = day;
        }

        public CalendarDayAddedEvent()
        {

        }
    }
}
