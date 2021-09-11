using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowestCalendar.Entities
{
    public class Checker
    {
        public static Task DidIMessageToday(DiscordSocketClient client)
        {
            var channelid = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json")).SetChannel;
            var channel = client.GetGuild(client.Guilds.First().Id).GetChannel(ulong.Parse(channelid)) as SocketTextChannel;
            var messages = channel.GetMessagesAsync();
            
            return Task.CompletedTask;
        }
    }
}
