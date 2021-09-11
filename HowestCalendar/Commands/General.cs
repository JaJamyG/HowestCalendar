using Discord;
using Discord.WebSocket;
using Discord.Commands;
using HowestCalendar.Entities;
using HowestCalendar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace HowestCalendar.Commands
{
    public class General : ModuleBase<SocketCommandContext>
    {
        private ICSHandler _icsHandler = new();
        [Command("today")]
        public async Task TodayAsync()
        {
            var events = _icsHandler.TodaySchedule();
            if(events.Count == 0)
            {
                var NoTasks = new EmbedBuilder(){ Title = "No events today" }.WithColor(Color.Blue).Build();
                await ReplyAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "Todays schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject,    $"classroom: {schedule.ClassRoom}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }
                
                await ReplyAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }
        [Command("tomorrow")]
        public async Task TomorrowAsync()
        {
            var events = _icsHandler.TomorrowSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events tomorrow" }.WithColor(Color.Blue).Build();
                await ReplyAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "Tomorrows schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"classroom: {schedule.ClassRoom}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await ReplyAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }
        [Command("nextupcomingday")]
        public async Task UpcommingDayAsync()
        {
            var events = _icsHandler.FirstUpcomingSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events up coming lessons" }.WithColor(Color.Blue).Build();
                await ReplyAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "Next upcoming schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"classroom: {schedule.ClassRoom}\n" +
                                                        $"day: {schedule.Day:d}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await ReplyAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }
        [Command("thisweek")]
        public async Task ThisWeekAsync()
        {
            var events = _icsHandler.WeekSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events this week" }.WithColor(Color.Blue).Build();
                await ReplyAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "This weeks schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"classroom: {schedule.ClassRoom}\n" +
                                                        $"Day: {schedule.Day:d}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await ReplyAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }
        [Command("nextweek")]
        public async Task NextWeekAsync()
        {
            var events = _icsHandler.NextWeekSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events this next week" }.WithColor(Color.Blue).Build();
                await ReplyAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "This next weeks schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"classroom: {schedule.ClassRoom}\n" +
                                                        $"Day: {schedule.Day:d}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await ReplyAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }
        [Command("setchannel")]
        public async Task SetChannelAsync()
        {
            var channel = Context.Channel;
            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
            appSettings.SetChannel = channel.Id.ToString();
            File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(appSettings));
        }
    }
}
