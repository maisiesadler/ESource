using ESource.Aggregates;
using ESource.Base;
using ESource.Commands;
using System;

namespace ESource
{
    public class AddAppointmentCommandHandler : ICommandHandler<AddAppointmentCommand>
    {
        private IRepository<AppointmentAggregate> _repository;
        public AddAppointmentCommandHandler(IRepository<AppointmentAggregate> repository)
        {
            _repository = repository;
        }

        public void Handle(AddAppointmentCommand message)
        {
            var appointment = new AppointmentAggregate();
            appointment.Start(Guid.NewGuid());
            _repository.Save(appointment, 1);
        }
    }
}
