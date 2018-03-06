using ESource.Base;
using System;

namespace ESource.AppointmentBook.Commands
{
    public class RequestAppointmentCommand : Command
    {
        public RequestAppointmentCommand(Guid day, int timeslot, string appointmentName)
        {
            Day = day;
            Timeslot = timeslot;
            AppointmentName = appointmentName;
        }

        public Guid Day { get; }
        public int Timeslot { get; }
        public string AppointmentName { get; }
    }
}
