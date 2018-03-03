using ESource.Aggregates;
using ESource.Base;
using ESource.Commands;

namespace ESource.CommandHandlers
{
    public class ActivateAppointmentCommandHandler : ICommandHandler<ActivateAppointmentCommand>
    {
        private readonly IRepository<AppointmentAggregate> _repository;
        public ActivateAppointmentCommandHandler(IRepository<AppointmentAggregate> repository)
        {
            _repository = repository;
        }

        public void Handle(ActivateAppointmentCommand message)
        {
            var appointment = _repository.GetById(message.Id);
            appointment.Activate();
            _repository.Save(appointment, appointment.Version);
        }
    }
}
