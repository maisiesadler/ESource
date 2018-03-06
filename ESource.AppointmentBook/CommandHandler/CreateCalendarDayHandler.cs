using ESource.AppointmentBook.Commands;
using ESource.Base;
using System;

namespace ESource.AppointmentBook.CommandHandler
{
    public class CreateCalendarDayHandler : ICommandHandler<CreateCalendarDayCommand>
    {
        private IRepository<CalendarDayAggregate> _repository;
        public CreateCalendarDayHandler(IRepository<CalendarDayAggregate> repository)
        {
            _repository = repository;
        }
        
        public void Handle(CreateCalendarDayCommand message)
        {
            var calendarDayAggregate = new CalendarDayAggregate();
            calendarDayAggregate.Initialise(Guid.NewGuid(), message.Day);
            _repository.Save(calendarDayAggregate, 1);
        }
    }
}
