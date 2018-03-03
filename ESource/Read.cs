using ESource.Base;
using ESource.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESource
{
    public class Read
    {
        public Read(IObservable<Event> eventStream)
        {
            eventStream.Subscribe(@event =>
            {
                switch (@event)
                {
                    case AppointmentAddedEvent e:
                        Handle(e);
                        break;
                    case AppointmentActivatedEvent e:
                        Handle(e);
                        break;
                }
            });
        }

        private List<Appointment> _appointments = new List<Appointment>();

        private void Handle(AppointmentAddedEvent t)
        {
            _appointments.Add(new Appointment(t.AggregateId));
        }

        private void Handle(AppointmentActivatedEvent t)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == t.AggregateId);
            appointment?.Activate();
        }

        public Guid GetOnlyAppointmentId()
        {
            return _appointments[0].Id;
        }

        public void PrintState()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_appointments));
        }
    }

    public class Appointment
    {
        public Guid Id { get; }
        public AppointmentStatus Status { get; private set; }

        public Appointment(Guid id)
        {
            Id = id;
            Status = AppointmentStatus.Created;
        }

        public void Activate()
        {
            Status = AppointmentStatus.Active;
        }
    }

    public enum AppointmentStatus
    {
        Created, Active, Retired
    }
}
