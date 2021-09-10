using HowestCalendar.Entities;
using Ical.Net;
using Ical.Net.CalendarComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowestCalendar.Services
{
    public class ICSConverter
    {
        private List<Event> events;
        public List<Event> GetCalendar()
        {
            events = new();
            Calendar calendar = Calendar.Load(File.ReadAllText(Directory.GetCurrentDirectory() + "/TimeTable.ics"));
            foreach (CalendarEvent lessonEvents in calendar.Events)
            {
                events.Add(new Event
                {
                    ClassRoom = lessonEvents.Location,
                    Day = GetDateZeroTime(lessonEvents.DtStart.Value),
                    Subject = lessonEvents.Summary,
                    Teacher = GetTeacher(lessonEvents.Description),
                    Time = new Time
                    {
                        StartTime = lessonEvents.DtStart.Value,
                        EndTime = lessonEvents.DtEnd.Value
                    }
                });
            }
            return events;
        }
        private DateTime GetDateZeroTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        private string GetTeacher(string haystack)
        {
            string[] ret = haystack.Replace("\n\n", "\n").Split("\n");
            if (ret.Length < 4) return "";
            return ret[3].Replace("Lector(en): ", "");
        }
    }
}
