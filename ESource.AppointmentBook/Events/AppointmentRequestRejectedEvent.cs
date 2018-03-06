using ESource.Base;
using System;

namespace ESource.AppointmentBook.Events
{
    public class AppointmentRequestRejectedEvent : Event
    {
        public int Timeslot { get; set; }
        public string AppointmentName { get; set; }

        public AppointmentRequestRejectedEvent(Guid id, int timeslot, string appointmentName)
        {
            AggregateId = id;
            Timeslot = timeslot;
            AppointmentName = appointmentName;
        }

        public AppointmentRequestRejectedEvent()
        {

        }
    }
}
