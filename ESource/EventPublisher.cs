using ESource.Base;
using System.Reactive.Subjects;

namespace ESource
{
    public interface IEventPublisher
    {
        void Publish(Event @event);
    }

    public class EventPublisher : IEventPublisher
    {
        ReplaySubject<Event> subject;
        public EventPublisher()
        {
            this.subject = new ReplaySubject<Event>();
        }

        public void Publish(Event @event)
        {
            subject.OnNext(@event);
        }

        public ReplaySubject<Event> Read()
        {
            return subject;
        }
    }
}
