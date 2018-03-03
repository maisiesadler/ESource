using ESource.Base;
using ESource.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESource
{
    public class EventStore : IEventStore
    {
        private const string _filePath = "../events/";
        private const string _eventExtension = ".event";

        List<Event> eventsOnLoad;

        public IEventPublisher Publisher { get; }
        private struct EventDescriptor
        {

            public readonly Event EventData;
            public readonly Guid Id;
            public readonly int Version;

            public EventDescriptor(Guid id, Event eventData, int version)
            {
                EventData = eventData;
                Version = version;
                Id = id;
            }
        }

        private readonly Dictionary<Guid, List<EventDescriptor>> _current = new Dictionary<Guid, List<EventDescriptor>>();

        public EventStore(IEventPublisher publisher)
        {
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

            Publisher = publisher;
            Load();
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            var eventDescriptors = GetOrCreateEventDescriptors(aggregateId, expectedVersion);

            var i = expectedVersion;

            // iterate through current aggregate events increasing version with each processed event
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;

                // push event to the event descriptors list for current aggregate
                eventDescriptors.Add(new EventDescriptor(@event.AggregateId, @event, i));
                Publisher.Publish(@event);
                SaveEvent(@event);
            }
        }

        private List<EventDescriptor> GetOrCreateEventDescriptors(Guid aggregateId, int expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;

            // try to get event descriptors list for given aggregate id
            // otherwise -> create empty dictionary
            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _current.Add(aggregateId, eventDescriptors);
            }
            // check whether latest event version matches current aggregate version
            // otherwise -> throw exception
            else if (eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion && expectedVersion != -1)
            {
                throw new ConcurrencyException();
            }
            return eventDescriptors;
        }

        public List<Event> GetEventsForAggregate(Guid aggregateId)
        {
            List<EventDescriptor> eventDescriptors;

            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }

        private void Load()
        {
            eventsOnLoad = new List<Event>();
            var files = Directory.GetFiles(_filePath)
                                 .Where(file => file.EndsWith(".event", StringComparison.Ordinal))
                                 .OrderBy(file => file);

            var events = new List<Event>();
            foreach (var file in files)
            {
                var e = File.ReadAllText(file);

                var deserialised = JsonConvert.DeserializeObject(e, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                }) as Event;

                var expectedVersion = deserialised.Version - 1; // last version
                var eventDescriptors = GetOrCreateEventDescriptors(deserialised.AggregateId, expectedVersion);
                eventDescriptors.Add(new EventDescriptor(deserialised.AggregateId, deserialised, deserialised.Version));
                Publisher.Publish(deserialised);
            }
        }

        private void SaveEvent(Event @event)
        {
            var json = JsonConvert.SerializeObject(@event,
                                                     Formatting.Indented,
                                                     new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            File.WriteAllText(GetNextFullFilePath(), json);
        }

        private string GetNextFullFilePath()
        {
            return _filePath + DateTime.UtcNow.Ticks + _eventExtension;
        }
    }
}
