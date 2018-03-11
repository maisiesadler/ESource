using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESource.WebSockets.Model
{
    public class AppointmentModel
    {
        public AppointmentModel(string name, int hour)
        {
            Name = name;
            Hour = hour;
        }

        public string Name { get; }
        public int Hour { get; }
    }
}
