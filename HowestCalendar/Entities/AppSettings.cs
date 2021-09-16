using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowestCalendar.Entities
{
    public class AppSettings
    {
        public string Token { get; set; }
        public List<Settings> Settings { get; set; }
    }
    public class Settings
    {
        public string Guild { get; set; }
        public string SetChannel { get; set; }
        public string TimeTable { get; set; }
    }
}
