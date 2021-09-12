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
    public class General
    {
        private ICSHandler _icsHandler = new();
        public async Task InfoAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var embedInfo = new EmbedBuilder() 
            { 
                Title = "Info about this discord bot.", 
                Description="This bot will post a message once a day about tomorrows events on the calander." 
            }.WithColor(Color.Blue).Build();
            await command.RespondAsync(embed: embedInfo);
        }

        public async Task HelpAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var embedHelp = new EmbedBuilder()
            {
                Title = "All the commands:",
                Description =   "All the commands starts with /\n" +
                                "/today to see the todays events\n" +
                                "/tomorrow to see all the events from tomorrow\n" +
                                "/upcoming the first upcoming event\n" +
                                "/thisweek all the events from this week\n" +
                                "/nextweek all the events from next week"
            }.WithColor(Color.Blue).Build();
            await command.RespondAsync(embed: embedHelp);
        }

        public async Task TodayAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var events = _icsHandler.TodaySchedule();
            if(events.Count == 0)
            {
                var NoTasks = new EmbedBuilder(){ Title = "No events today" }.WithColor(Color.Blue).Build();
                await command.RespondAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "Today's schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject,    $"Classroom: {schedule.ClassRoom}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }
                
                await command.RespondAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }

        public async Task TomorrowAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var events = _icsHandler.TomorrowSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events tomorrow" }.WithColor(Color.Blue).Build();
                await command.RespondAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "Tomorrows schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"Classroom: {schedule.ClassRoom}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await command.RespondAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }
 
        public async Task UpcommingDayAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var events = _icsHandler.FirstUpcomingSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events up coming lessons" }.WithColor(Color.Blue).Build();
                await command.RespondAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "Next upcoming schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"Classroom: {schedule.ClassRoom}\n" +
                                                        $"Day: {schedule.Day:dd/MM/yyyy}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await command.RespondAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }

        public async Task ThisWeekAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var events = _icsHandler.WeekSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events this week" }.WithColor(Color.Blue).Build();
                await command.RespondAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "This weeks schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"Classroom: {schedule.ClassRoom}\n" +
                                                        $"Day: {schedule.Day:dd/MM/yyyy}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await command.RespondAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }

        public async Task NextWeekAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var events = _icsHandler.NextWeekSchedule();
            if (events.Count == 0)
            {
                var NoTasks = new EmbedBuilder() { Title = "No events this next week" }.WithColor(Color.Blue).Build();
                await command.RespondAsync(embed: NoTasks);
            }
            else
            {
                var embedbuild = new EmbedBuilder() { Title = "This next weeks schedule" };
                foreach (var schedule in events)
                {
                    embedbuild.AddField(schedule.Subject, $"Classroom: {schedule.ClassRoom}\n" +
                                                        $"Day: {schedule.Day:dd/MM/yyyy}\n" +
                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }

                await command.RespondAsync(embed: embedbuild.WithColor(Color.Blue).Build());
            }
        }

        public async Task SetChannelAsync(SocketSlashCommand command)
        {
            Console.WriteLine($"{DateTime.Now} user {command.User.Username} uses {command.Data.Name}");
            var channel = command.Channel;
            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
            appSettings.SetChannel = channel.Id.ToString();
            File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(appSettings));
            await command.RespondAsync($"Notications set to channel: {channel.Name}");
        }
    }
}
