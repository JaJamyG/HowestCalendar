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
        public List<Event> Lessons { get; private set; }
        private readonly ICSConverter icsConverter = new();
        public ICSHandler()
        {
            Lessons = new();
            Lessons = icsConverter.GetCalendar();
        }
        public List<Event> TodaySchedule()
        {
            DateTime today = DateTime.Now;
            List<Event> lessons = new();

            foreach (Event lesson in Lessons)
            {
                if(lesson.Day.ToShortDateString() == today.ToShortDateString())
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> TomorrowSchedule()
        {
            DateTime today = DateTime.Now;
            List<Event> lessons = new();

            foreach (Event lesson in Lessons)
            {
                if (lesson.Day.ToShortDateString() == today.AddDays(1).ToShortDateString())
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> WeekSchedule()
        {
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            DateTime today = DateTime.Now;
            List<Event> lessons = new();
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            foreach (Event lesson in Lessons)
            {
                if (myCal.GetWeekOfYear(lesson.Day, myCWR, myFirstDOW) == myCal.GetWeekOfYear(today, myCWR, myFirstDOW))
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
        public List<Event> NextWeekSchedule()
        {
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            DateTime today = DateTime.Now;
            List<Event> lessons = new();
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            foreach (Event lesson in Lessons)
            {
                if (myCal.GetWeekOfYear(lesson.Day, myCWR, myFirstDOW) == (1 + myCal.GetWeekOfYear(today, myCWR, myFirstDOW)))
                {
                    lessons.Add(lesson);
                }
            }
            return lessons;
        }
    }
}
