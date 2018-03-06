using System;
using System.Collections.Generic;
using System.Linq;

namespace ESource.AppointmentBook
{
    public class AppointmentBook<T>
    {
        private Dictionary<int, T> _slots;
        private int _maxSlot;

        public AppointmentBook(Guid id, int timeSlots)
        {
            Id = id;
            _maxSlot = timeSlots;
            _slots = new Dictionary<int, T>();
        }

        public Guid Id { get; }

        public void Add(int slot, T value)
        {
            if (slot > _maxSlot)
                throw new InvalidSlotException();

            if (_slots.ContainsKey(slot))
                throw new TimeSlotNotAvailableException();

            _slots[slot] = value;
        }

        public List<(int, T)> GetAppointments()
        {
            return _slots.Select(s => (s.Key, s.Value)).ToList();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _slots.Select(s => s.Key + "-" + s.Value));
        }
    }

    public class InvalidSlotException : Exception { }
    public class TimeSlotNotAvailableException : Exception { }
}
