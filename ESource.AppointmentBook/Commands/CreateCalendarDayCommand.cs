using ESource.Base;
using System;

namespace ESource.AppointmentBook.Commands
{
    public class CreateCalendarDayCommand : Command
    {
        public CreateCalendarDayCommand(DayOfWeek day)
        {
            Day = day;
        }

        public DayOfWeek Day { get; }
    }
}
