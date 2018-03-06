using ESource.AppointmentBook.Commands;
using ESource.Base;

namespace ESource.AppointmentBook.CommandHandler
{
    public class RequestAppointmentCommandHandler : ICommandHandler<RequestAppointmentCommand>
    {
        private IRepository<CalendarDayAggregate> _repository;

        public RequestAppointmentCommandHandler(IRepository<CalendarDayAggregate> repository)
        {
            _repository = repository;
        }
        
        public void Handle(RequestAppointmentCommand message)
        {
            var calendarDay = _repository.GetById(message.Day);
            calendarDay.RequestAppointment(message.Timeslot, message.AppointmentName);
            _repository.Save(calendarDay, calendarDay.Version);
        }
    }
}
