using System;

namespace ESource.AppointmentBook
{
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
}
