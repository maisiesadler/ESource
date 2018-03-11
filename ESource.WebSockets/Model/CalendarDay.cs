using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESource.WebSockets.Model
{
    public class CalendarDay
    {
        [JsonProperty("Appointments")]
        private List<AppointmentModel> _appointments = new List<AppointmentModel>();

        public CalendarDay(Guid id, DayOfWeek day)
        {
            Id = id;
            Day = day;
        }

        public void Add(string appointmentName, int hour)
        {
            _appointments.Add(new AppointmentModel(appointmentName, hour));
        }

        public Guid Id { get; }
        public DayOfWeek Day { get; }

        public override string ToString()
        {
            return $"Day: {Day} - {string.Join(',', _appointments.Select(a => a.Name + " at + " + a.Hour))}";
        }
    }
}
