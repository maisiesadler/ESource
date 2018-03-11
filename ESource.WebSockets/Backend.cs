using ESource.AppointmentBook;
using ESource.AppointmentBook.Commands;
using ESource.AppointmentBook.Events;
using ESource.Base;
using ESource.WebSockets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ESource.WebSockets
{
    public class Backend
    {
        private CalendarSystem _system;
        private Dictionary<Guid, CalendarDay> _days = new Dictionary<Guid, CalendarDay>();
        private ReplaySubject<List<CalendarDay>> _latestDays = new ReplaySubject<List<CalendarDay>>(1);

        public Backend()
        {
            _system = new CalendarSystem();
            FillAppointments();
        }

        private ReplaySubject<Event> ReadEvents()
        {
            return _system.Read();
        }

        private void FillAppointments()
        {
            ReadEvents().Subscribe(@event =>
            {
                switch (@event)
                {
                    case CalendarDayAddedEvent e:
                        _days.Add(e.AggregateId, new CalendarDay(e.AggregateId, e.Day));
                        break;
                    case AppointmentRequestAcceptedEvent e:
                        var gotDay = _days.TryGetValue(e.AggregateId, out var day);
                        if (gotDay)
                            day.Add(e.AppointmentName, e.Timeslot);
                        break;
                }
                _latestDays.OnNext(_days.Values.ToList());
            });
        }

        public IObservable<string> Read()
        {
            return _latestDays
                .Select(s => JsonConvert.SerializeObject(s));
        }

        public void Process(JObject request)
        {
            if (request.TryGetValue("commandName", out var commandName))
            {
                var command = commandName.ToString();
                switch (command)
                {
                    case "RequestAppointmentCommand":
                        if (request.TryGetValue("data", out var data))
                        {
                            var jData = data as JObject;
                            jData.TryGetValue("day", out var day1);
                            if (jData.TryGetValue("day", out var day)
                                && jData.TryGetValue("timeslot", out var timeslot)
                                && jData.TryGetValue("appointmentName", out var appointmentName))
                            {
                                Guid id = new Guid(day.ToString());
                                int timeslotInt = int.Parse(timeslot.ToString());
                                string appt = appointmentName.ToString();
                                var cmd = new RequestAppointmentCommand(id, timeslotInt, appt);
                                Send(cmd);
                            }
                        }
                        break;
                }
            }
        }

        public void Send<T>(T command) where T : Command
        {
            _system.Send(command);
        }
    }
}
