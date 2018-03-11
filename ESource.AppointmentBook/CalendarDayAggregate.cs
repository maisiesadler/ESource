using ESource.AppointmentBook.Events;
using ESource.Base;
using System;

namespace ESource.AppointmentBook
{
    public class CalendarDayAggregate : AggregateRoot
    {
        private bool[] _slots;

        protected override void Apply(Event @event)
        {
            Version++;
            switch (@event)
            {
                case CalendarDayAddedEvent e:
                    Apply(e);
                    break;
                case AppointmentRequestAcceptedEvent e:
                    Apply(e);
                    break;
            }
        }

        private void Apply(CalendarDayAddedEvent @event)
        {
            Id = @event.AggregateId;
            _slots = new bool[24];
        }

        private void Apply(AppointmentRequestAcceptedEvent @event)
        {
            _slots[@event.Timeslot] = true;
        }

        public void Initialise(Guid id, DayOfWeek day)
        {
            Publish(new CalendarDayAddedEvent(id, day));
        }

        public void RequestAppointment(int timeslot, string appointmentName)
        {
            bool RequestIsValid()
            {
                if (timeslot < 0 || timeslot >= _slots.Length)
                    return false;

                var slotTaken = _slots[timeslot];
                if (slotTaken)
                    return false;
                else
                    return true;
            }
            
            if (RequestIsValid())
                Publish(new AppointmentRequestAcceptedEvent(Id, timeslot, appointmentName));
            else
                Publish(new AppointmentRequestRejectedEvent(Id, timeslot, appointmentName));
        }
    }
}
