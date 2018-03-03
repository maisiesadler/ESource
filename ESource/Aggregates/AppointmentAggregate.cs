using ESource.Base;
using ESource.Events;
using System;

namespace ESource.Aggregates
{
    public class AppointmentAggregate : AggregateRoot
    {
        private bool isActive = false;
        public AppointmentAggregate()
        {
        }

        protected override void Apply(Event @event)
        {
            Version++;
            switch (@event)
            {
                case AppointmentAddedEvent aae:
                    Apply(aae);
                    break;
                case AppointmentActivatedEvent aae:
                    Apply(aae);
                    break;
            }
        }

        private void Apply(AppointmentAddedEvent aae)
        {
            this.Id = aae.AggregateId;
        }

        private void Apply(AppointmentActivatedEvent aae)
        {
            isActive = true;
        }

        public void Start(Guid id)
        {
            Publish(new AppointmentAddedEvent(id));
        }

        public void Activate()
        {
            Publish(new AppointmentActivatedEvent(Id));
        }
    }
}
