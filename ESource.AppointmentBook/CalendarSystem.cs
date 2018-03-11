using ESource.AppointmentBook.CommandHandler;
using ESource.AppointmentBook.Commands;
using ESource.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Reactive.Subjects;

namespace ESource.AppointmentBook
{
    public class CalendarSystem
    {
        private EventPublisher _publisher;
        private CommandSender _sender;

        public CalendarSystem()
        {
            _publisher = new EventPublisher();
            var eventStore = new EventStore(_publisher);
           _sender = new CommandSender();

            var calendarDayRepo = new EventSourcedRepository<CalendarDayAggregate>(eventStore);

            _sender.RegisterHandler(new CreateCalendarDayHandler(calendarDayRepo));
            _sender.RegisterHandler(new RequestAppointmentCommandHandler(calendarDayRepo));
        }

        public ReplaySubject<Event> Read()
        {
            return _publisher.Read();
        }
        
        public void Send<T>(T command) where T: Command
        {
            _sender.Send(command);
        }
    }
}
