using ESource.AppointmentBook.CommandHandler;
using ESource.AppointmentBook.Commands;
using System;

namespace ESource.AppointmentBook
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new EventPublisher();
            var eventStore = new EventStore(publisher);
            var sender = new CommandSender();

            var calendarDayRepo = new EventSourcedRepository<CalendarDayAggregate>(eventStore);

            sender.RegisterHandler(new CreateCalendarDayHandler(calendarDayRepo));
            sender.RegisterHandler(new RequestAppointmentCommandHandler(calendarDayRepo));

            // sender.Send(new CreateCalendarDayCommand(Day.Thursday));

            //var read = new Read(publisher.Read());
            var read2 = new Read2(publisher.Read());
            var calendarDay = read2.GetFirstCalendarDayId();

            //sender.Send(new RequestAppointmentCommand(calendarDay, 1, "one"));
            sender.Send(new RequestAppointmentCommand(calendarDay, 23, "pass"));

            //sender.Send(new RequestAppointmentCommand(calendarDay, 24, "fail"));

            read2.PrintFirstAppointmentBook(calendarDay);

            Console.ReadLine();
        }
    }
}
