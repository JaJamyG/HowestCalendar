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

        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.WriteLine("╔═════════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Discord Bot: Howest Calendar                            ║");
            Console.WriteLine("╠═════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ By JaJamy                                               ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            using var services = ConfigureServices();
            _client = services.GetRequiredService<DiscordSocketClient>();

            _client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            var token = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json")).Token;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await services.GetRequiredService<CommandHandler>().InitializeAsync();
            await Task.Delay(Timeout.Infinite);
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString().Trim());
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
       
    }
}
