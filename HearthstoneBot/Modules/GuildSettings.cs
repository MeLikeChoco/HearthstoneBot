using Discord.Addons.InteractiveCommands;
using Discord.Commands;
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

        [Command("prefix")]
        [Summary("Set prefix")]
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

    }

}
