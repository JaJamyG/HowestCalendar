using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowestCalendar.Entities
{
    public class ResetSlash
    {
        public async Task Reset(SocketSlashCommand command, DiscordSocketClient client)
        {
            Console.WriteLine($"{DateTime.Now} Deleting commands");
            await client.Rest.DeleteAllGlobalCommandsAsync();

            var infoCommand = new SlashCommandBuilder().WithName("info").WithDescription("Gives info about the bot.");
            var helpCommand = new SlashCommandBuilder().WithName("help").WithDescription("Gives all the commands with description.");
            var todayCommand = new SlashCommandBuilder().WithName("today").WithDescription("Gives the events from today.");
            var tomorrowCommand = new SlashCommandBuilder().WithName("tomorrow").WithDescription("Gives the events from tomorrow.");
            var upcomingCommand = new SlashCommandBuilder().WithName("upcoming").WithDescription("Gives the first day with events.");
            var weekCommand = new SlashCommandBuilder().WithName("week").WithDescription("Gives all the events from this week.");
            var nextweekCommand = new SlashCommandBuilder().WithName("nextweek").WithDescription("Gives all the events from next week.");
            var setChannelCommand = new SlashCommandBuilder().WithName("setchannel").WithDescription("Sets the notification channel.");
            var oneshot = new SlashCommandBuilder().WithName("oneshot").WithDescription("Resets the commands.");
            Console.WriteLine($"{DateTime.Now} Creating commands");
            try
            {
                await client.Rest.CreateGlobalCommand(infoCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(helpCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(todayCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(tomorrowCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(upcomingCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(weekCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(nextweekCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(setChannelCommand.Build());
                await Task.Delay(1000);
                await client.Rest.CreateGlobalCommand(oneshot.Build());
                Console.WriteLine($"{DateTime.Now} Finished");

                await command.RespondAsync("Reloaded the commands");
            }
            catch (ApplicationCommandException exception)
            {
                Console.WriteLine("Error");
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                Console.WriteLine(json);
            }
        }
    }
}
