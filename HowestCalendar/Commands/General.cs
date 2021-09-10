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
        ICSConverter converter = new();

        [Command("task")]
        public async Task TaskAsync()
        {
            var events = converter.GetCalendar().First();
            var embed = new EmbedBuilder()
            {
                Title = "Volgende les",
                Description =   $"Les: {events.Subject}\n" +
                                $"Dag: {events.Day:d}\n" +
                                $"Les start: {events.Time.StartTime:t}\n" +
                                $"Les eind: {events.Time.EndTime:t}"
            }.Build();
            await ReplyAsync(embed: embed);
        }
    }
}
