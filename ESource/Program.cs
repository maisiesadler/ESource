using ESource.Aggregates;
using ESource.CommandHandlers;
using ESource.Commands;
using System;

namespace ESource
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var publisher = new EventPublisher();

            var read = new Read(publisher.Read());

            var eventStore = new EventStore(publisher);

            read.PrintState();

            var repo = new EventSourcedRepository<AppointmentAggregate>(eventStore);
            var sender = new CommandSender();

            sender.RegisterHandler(new AddAppointmentCommandHandler(repo));
            sender.RegisterHandler(new ActivateAppointmentCommandHandler(repo));

          //  sender.Send(new AddAppointmentCommand());

            var onlyId = read.GetOnlyAppointmentId();
            Console.WriteLine("got id: " + onlyId);

          //  sender.Send(new ActivateAppointmentCommand(onlyId));


            Console.ReadLine();
        }
    }
}
