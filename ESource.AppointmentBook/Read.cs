using ESource.AppointmentBook.Events;
using ESource.Base;
using ESource.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESource.AppointmentBook
{
    public class Read2
    {
        public Read2(IObservable<Event> eventStream)
        {
            eventStream.Subscribe(@event =>
            {
                WriteToConsole(@event);
                switch (@event)
                {
                    case CalendarDayAddedEvent e:
                        Handle(e);
                        break;
                    case AppointmentRequestAcceptedEvent e:
                        Handle(e);
                        break;
                }
            });
        }

        private void WriteToConsole(Event @event)
        {
            Console.WriteLine("1-" + @event.GetType().ToString() + "-" + JsonConvert.SerializeObject(@event));
        }

        private Dictionary<Guid, AppointmentBook<string>> _appointmentBooks = new Dictionary<Guid, AppointmentBook<string>>();

        private void Handle(CalendarDayAddedEvent t)
        {
            _appointmentBooks.Add(t.AggregateId, new AppointmentBook<string>(t.AggregateId, 24));
        }

        private void Handle(AppointmentRequestAcceptedEvent t)
        {
            var appointmentBook = _appointmentBooks[t.AggregateId];
            appointmentBook.Add(t.Timeslot, t.AppointmentName);
        }

        public Guid GetFirstCalendarDayId()
        {
            return _appointmentBooks.Keys.First();
        }

        public void PrintFirstAppointmentBook(Guid id)
        {
            Console.WriteLine(_appointmentBooks[id]);
        }

        public void PrintState()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_appointmentBooks));
        }
    }

    public enum AppointmentStatus
    {
        Created, Active, Retired
    }
}
