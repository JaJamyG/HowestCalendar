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
using HowestCalendar.Commands;

namespace HowestCalendar.Services
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly ICSHandler _icsHandler = new();
        private readonly Timer _timer;
        private AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
        private DateTime? sended = null;
        private readonly SlashCommands _slashCommands = new();

        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _timer = new Timer { AutoReset = true, Interval = 3_600_000 };

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.InteractionCreated += Client_InteractionCreated;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        private async Task Client_InteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
                await _slashCommands.Command(command, _discord);
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
            try
            {
                if(sended == null || sended.Value.Date != DateTime.Today.Date)
                {
                    Console.WriteLine($"{DateTime.Now.Date} checking for events for tomorrow.");
                    appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
                    var channel = _discord.GetChannel(ulong.Parse(appSettings.SetChannel)) as SocketTextChannel;
                    var events = _icsHandler.TomorrowSchedule();
                    if(events.Count() != 0)
                    {
                        Console.WriteLine($"{DateTime.Now.Date} Found one.");
                        var embedbuild = new EmbedBuilder() { Title = "Todays schedule" };
                        foreach (var schedule in events)
                        {
                            embedbuild.AddField(schedule.Subject, $"classroom: {schedule.ClassRoom}\n" +
                                                                $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                                $"Teacher(s): {schedule.Teacher}");
                        }
                        await channel.SendMessageAsync(embed: embedbuild.WithColor(Color.Blue).Build());
                    }
                    sended = DateTime.Now; 
                    Console.WriteLine($"{DateTime.Now.Date} Done!");
                }
            }
            catch
            {
                Console.WriteLine("No channel set");
            }

        }
    }
}
