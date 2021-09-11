using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;
using HowestCalendar.Entities;
using System.Timers;
using System.Linq;

namespace HowestCalendar.Services
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly ICSHandler _icsHandler = new();
        private readonly Timer _timer;

        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _timer = new Timer { AutoReset = true, Interval = 5000 };

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            var prefix = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json")).Prefix;
            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
            Console.WriteLine($"{DateTime.Now} // {context.User} used command {context.Message}");
        }
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
                return;

            await context.Channel.SendMessageAsync($"error: {result}");
        }
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e) => await SendMessage();
        private async Task SendMessage()
        {
            var guilds = _discord.Guilds;
            var guild = _discord.GetGuild(_discord.Guilds.First().Id);
            var channel = guild.GetChannel(885595138238332948) as SocketTextChannel;
            await channel.SendMessageAsync("test test test");
        }
    }
}
