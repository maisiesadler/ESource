using ESource.Base;

namespace ESource.AppointmentBook.Commands
{
    public class CreateCalendarDayCommand : Command
    {
        public CreateCalendarDayCommand(Day day)
        {
            Day = day;
        }

        public Day Day { get; }
    }
}
