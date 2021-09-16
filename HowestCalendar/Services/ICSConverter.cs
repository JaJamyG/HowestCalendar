using HowestCalendar.Entities;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Newtonsoft.Json;
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
        private readonly List<ServerEvent> serverEvents = new();
        readonly AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
        public ICSConverter()
        {

        }
        public List<ServerEvent> GetCalendar()
        {
            foreach(var settings in appSettings.Settings)
            {
                try
                {
                    if (settings.Guild != "" && settings.TimeTable != "")
                    {
                        ServerEvent serverEvent = new(); 
                        List<Event> events = new(); 
                        Calendar calendar = Calendar.Load(File.ReadAllText(Directory.GetCurrentDirectory() + "/" + settings.TimeTable));
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
                        serverEvent.Guild = settings.Guild;
                        serverEvent.Events = events;
                        serverEvents.Add(serverEvent);
                    }
                }
                catch
                {
                }
            }
            return serverEvents;
        }
        private static DateTime GetDateZeroTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        private static string GetTeacher(string haystack)
        {
            string[] ret = haystack.Replace("\n\n", "\n").Split("\n");
            if (ret.Length < 4) return "";
            return ret[3].Replace("Staff member(s): ", "");
        }
    }
}
