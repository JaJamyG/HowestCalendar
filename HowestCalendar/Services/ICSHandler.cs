using HowestCalendar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace HowestCalendar.Services
{
    public class ICSHandler
    {
        public List<ServerEvent> Lessons { get; private set; }
        private readonly ICSConverter icsConverter = new();
        public ICSHandler()
        {
            Lessons = new();
            Lessons = icsConverter.GetCalendar();
        }
        public void Reset()
        {
            Lessons = new();
            Lessons = icsConverter.GetCalendar();
        }
        public List<Event> TodaySchedule(string guild)
        {
            DateTime today = DateTime.Now;
            List<Event> lessons = new();
            foreach (Event lesson in Lessons.Where(x => x.Guild == guild).First().Events)
            {
                if(lesson.Day.ToShortDateString() == today.ToShortDateString())
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> TomorrowSchedule(string guild)
        {
            DateTime today = DateTime.Now;
            List<Event> lessons = new();
            foreach (Event lesson in Lessons.Where(x => x.Guild == guild).First().Events)
            {
                if (lesson.Day.ToShortDateString() == today.AddDays(1).ToShortDateString())
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> WeekSchedule(string guild)
        {
            CultureInfo myCI = new("en-US");
            Calendar myCal = myCI.Calendar;
            DateTime today = DateTime.Now;
            List<Event> lessons = new();
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            foreach (Event lesson in Lessons.Where(x => x.Guild == guild).First().Events)
            {
                if (myCal.GetWeekOfYear(lesson.Day, myCWR, myFirstDOW) == myCal.GetWeekOfYear(today, myCWR, myFirstDOW))
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> NextWeekSchedule(string guild)
        {
            CultureInfo myCI = new("en-US");
            Calendar myCal = myCI.Calendar;
            DateTime today = DateTime.Now;
            List<Event> lessons = new();
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            foreach (Event lesson in Lessons.Where(x => x.Guild == guild).First().Events)
            {
                if (myCal.GetWeekOfYear(lesson.Day, myCWR, myFirstDOW) == (1 + myCal.GetWeekOfYear(today, myCWR, myFirstDOW)))
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> FirstUpcomingSchedule(string guild)
        {
            DateTime today = DateTime.Now;
            DateTime? firstUpcomingDay = null;
            List<Event> lessons = new();

            foreach (Event lesson in Lessons.Where(x => x.Guild == guild).First().Events)
            {
                if (lesson.Day > today)
                {
                    if(firstUpcomingDay == null)
                        firstUpcomingDay = lesson.Day;

                    if(firstUpcomingDay == lesson.Day)
                        lessons.Add(lesson);
                }
            }
            return lessons;
        }
    }
}
