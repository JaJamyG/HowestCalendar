using Discord.WebSocket;
using HowestCalendar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowestCalendar.Commands
{
    public class SlashCommands
    {
        private readonly General _general = new();
        public async Task Command(SocketSlashCommand command, DiscordSocketClient client)
        {
            switch (command.Data.Name)
            {
                case "info":
                    await General.InfoAsync(command);
                    break;
                case "help":
                    await General.HelpAsync(command);
                    break;
                case "today":
                    await _general.TodayAsync(command);
                    break;
                case "tomorrow":
                    await _general.TomorrowAsync(command);
                    break;
                case "upcoming":
                    await _general.UpcommingDayAsync(command);
                    break;
                case "week":
                    await _general.ThisWeekAsync(command);
                    break;
                case "nextweek":
                    await _general.NextWeekAsync(command);
                    break;
                case "setchannel":
                    await General.SetChannelAsync(command);
                    break;
                case "oneshot":
                    if (command.User.Id == 272800434639470592)
                        await ResetSlash.Reset(command, client);
                    else
                        await command.RespondAsync("You are not authorised");
                    break;
            }
        }
    }
}
