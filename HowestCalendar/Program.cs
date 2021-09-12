using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using HowestCalendar.Services;
using Discord.Net;
using Newtonsoft.Json;
using System.IO;
using HowestCalendar.Entities;

namespace HowestCalendar
{
    class Program
    {
        private DiscordSocketClient _client;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.WriteLine("╔═════════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Discord Bot: Howest Calendar                            ║");
            Console.WriteLine("╠═════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ By JaJamy                                               ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            using (var services = ConfigureServices())
            {
                _client = services.GetRequiredService<DiscordSocketClient>();

                _client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                var token = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json")).Token;
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                //_client.Ready += Client_Ready;

                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                await Task.Delay(Timeout.Infinite);
            }
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString().Trim());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
        public async Task Client_Ready()
        {
            var infoCommand = new SlashCommandBuilder().WithName("info").WithDescription("Gives info about the bot.");
            var helpCommand = new SlashCommandBuilder().WithName("help").WithDescription("Gives all the commands with description.");
            var todayCommand = new SlashCommandBuilder().WithName("today").WithDescription("Gives the events from today.");
            var tomorrowCommand = new SlashCommandBuilder().WithName("tomorrow").WithDescription("Gives the events from tomorrow.");
            var upcomingCommand = new SlashCommandBuilder().WithName("upcoming").WithDescription("Gives the first day with events.");
            var weekCommand = new SlashCommandBuilder().WithName("week").WithDescription("Gives all the events from this week.");
            var nextweekCommand = new SlashCommandBuilder().WithName("nextweek").WithDescription("Gives all the events from next week.");
            var setChannelCommand = new SlashCommandBuilder().WithName("setchannel").WithDescription("Sets the notification channel.");
            Console.WriteLine($"{DateTime.Now} Creating commands");
            try
            {
                await _client.Rest.CreateGlobalCommand(weekCommand.Build());
                Console.WriteLine($"{DateTime.Now} Creating weekCommand");
                await Task.Delay(1000);
                await _client.Rest.CreateGlobalCommand(nextweekCommand.Build());
                Console.WriteLine($"{DateTime.Now} Creating nextweekCommand");
                await Task.Delay(1000);
                await _client.Rest.CreateGlobalCommand(setChannelCommand.Build());
                Console.WriteLine($"{DateTime.Now} Creating setChannelCommand");
                Console.WriteLine($"{DateTime.Now} Finished");

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
