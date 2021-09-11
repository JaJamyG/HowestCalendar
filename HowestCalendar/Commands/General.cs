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

namespace HowestCalendar.Commands
{
    public class General : ModuleBase<SocketCommandContext>
    {
        ICSHandler icsHandler = new();

        //[Command("task")]
        //public async Task TaskAsync()
        //{
        //    //var events = converter.GetCalendar().First();
        //    //var embed = new EmbedBuilder()
        //    //{
        //    //    Title = "Volgende les",
        //    //    Description =   $"Les: {events.Subject}\n" +
        //    //                    $"Dag: {events.Day:d}\n" +
        //    //                    $"Les start: {events.Time.StartTime:t}\n" +
        //    //                    $"Les eind: {events.Time.EndTime:t}"
        //    //}.Build();
        //    //await ReplyAsync(embed: embed);
        //}
        [Command("today")]
        public async Task TodayAsync()
        {
            var events = icsHandler.TodaySchedule();
            if(events.Count == 0)
            {
                var NoTasks = new EmbedBuilder(){ Title = "No events today" }.Build();
                await ReplyAsync(embed: NoTasks);
            }
            else
            {
                var today = new EmbedBuilder() { Title = "Todays schedule" };
                foreach (var schedule in events)
                {
                    today.AddField(schedule.Subject,    $"classroom: {schedule.ClassRoom}\n" +
                                                        $"hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                        $"Teacher(s): {schedule.Teacher}");
                }
                
                await ReplyAsync(embed: today.Build());
            }

        }
    }
}
