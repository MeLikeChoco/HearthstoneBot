using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using HearthstoneBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Modules
{

    public class GuildSettings : ModuleBase<SocketCommandContext>
    {

        [Command("minimal")]
        [Summary("Gets the minimal setting")]
        public async Task MinimalSettingCommand()
            => await ReplyAsync($"The minimal setting is: {Settings.GetMinimalSetting(Context.Guild.Id)}");

        [Command("prefix")]
        [Summary("Set prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PrefixCommand(string prefix = null)
        {

            if (string.IsNullOrEmpty(prefix))
            {

                await Settings.SetPrefix(Context.Guild.Id, "h^");
                await ReplyAsync("Prefix has been reset to: h^");

            }
            else
            {

                await Settings.SetPrefix(Context.Guild.Id, prefix);
                await ReplyAsync("Prefix has been set to: " + prefix);

            }

        }

        [Command("minimal")]
        [Summary("Sets minimal settings")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MinimalCommand(bool setting)
        {

            await Settings.SetMinimalSetting(Context.Guild.Id, setting);
            await ReplyAsync("Minimal setting has been set to: " + setting);

        }

        [Command("guesschannel")]
        [Summary("Marks a channel for guess games")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GuessChannelCommand(bool setting)
        {

            if (Settings.IsGuessChannel(Context.Channel as SocketGuildChannel))
            {

                await ReplyAsync("Channel is already a guess channel!");
                return;

            }

            await Settings.SetGuessChannel(Context.Channel as SocketGuildChannel);
            await ReplyAsync("Channel set!");

        }

        [Command("prefix")]
        [Summary("Set prefix")]
        [RequireOwner]
        public async Task PrefixCommandOwner(string prefix = null)
        {

            if (string.IsNullOrEmpty(prefix))
            {

                await Settings.SetPrefix(Context.Guild.Id, "h^");
                await ReplyAsync("Prefix has been reset to: h^");

            }
            else
            {

                await Settings.SetPrefix(Context.Guild.Id, prefix);
                await ReplyAsync("Prefix has been set to: " + prefix);

            }

        }

        [Command("minimal")]
        [Summary("Sets minimal settings")]
        [RequireOwner]
        public async Task MinimalCommandOwner(bool setting)
        {

            await Settings.SetMinimalSetting(Context.Guild.Id, setting);
            await ReplyAsync("Minimal setting has been set to: " + setting);

        }

    }

}
