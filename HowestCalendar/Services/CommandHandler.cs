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
        private readonly Timer _timer = new() { AutoReset = true, Interval = 1_800_000, Enabled = true };
        private AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
        private DateTime? sended = null;
        private readonly SlashCommands _slashCommands = new();

        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.InteractionCreated += Client_InteractionCreated;
            _discord.JoinedGuild += Client_JoinGuild;
            _timer.Elapsed += Timer_Elapsed;
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
        public static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
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
                if(sended == null || sended.Value.Date != DateTime.Today.Date && DateTime.Now.TimeOfDay > TimeSpan.FromHours(6))
                {
                    Console.WriteLine($"{DateTime.Now} checking for events for tomorrow.");
                    appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
                    var settings = appSettings.Settings;
                    
                    foreach(var setting in settings)
                    {
                        var events = _icsHandler.TomorrowSchedule(setting.Guild);
                        if(events.Count != 0)
                        {
                            foreach(var calander in events)
                            {
                                var channel = _discord.GetChannel(ulong.Parse(setting.SetChannel)) as SocketTextChannel;
                                var embedbuild = new EmbedBuilder() { Title = "Todays schedule" };
                                foreach (var schedule in events)
                                {
                                    embedbuild.AddField(schedule.Subject, $"classroom: {schedule.ClassRoom}\n" +
                                                                        $"Hour: {schedule.Time.StartTime:t} - {schedule.Time.EndTime:t}\n" +
                                                                        $"Teacher(s): {schedule.Teacher}");
                                }
                                await channel.SendMessageAsync(embed: embedbuild.WithColor(Color.Blue).Build());
                                Console.WriteLine($"{DateTime.Now} Found one for server {channel}");
                            }
                        }
                        sended = DateTime.Now;
                        Console.WriteLine($"{DateTime.Now} Done!");
                    }
                }
            }
            catch
            {
                Console.WriteLine("No channel set");
            }
        }
        private Task Client_JoinGuild(SocketGuild socketGuild)
        {
            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
            Settings settings = new();
            settings.Guild = socketGuild.Id.ToString();
            appSettings.Settings.Add(settings);
            File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(appSettings));

            _icsHandler.Reset();
            return Task.CompletedTask;
        }
    }
}
