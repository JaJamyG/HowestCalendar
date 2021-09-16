using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowestCalendar.Entities
{
    public class ServerEvent
    {
        public ServerEvent()
        {
                
        }
        public string Guild { get; set; }
        public List<Event> Events { get; set; } = new();
    }
}
