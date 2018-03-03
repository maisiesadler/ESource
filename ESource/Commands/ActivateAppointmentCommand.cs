using ESource.Base;
using System;

namespace ESource.Commands
{
    public class ActivateAppointmentCommand : Command
    {
        public Guid Id { get; }

        public ActivateAppointmentCommand(Guid id)
        {
            Id = id;
        }
    }
}
