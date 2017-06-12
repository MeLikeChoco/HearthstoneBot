﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HearthstoneBot.Services;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Modules
{
    public class MiscCommands : ModuleBase<SocketCommandContext>
    {

        protected override void BeforeExecute()
        {

            _program = Process.GetCurrentProcess();

        }

        private Process _program;
        private CommandService _cmdService;

        public MiscCommands(CommandService serviceParams)
        {

            _cmdService = serviceParams;

        }

        [Command("invite")]
        [Summary("Get invite to bot")]
        public async Task InviteCommand()
            => await ReplyAsync("<https://discordapp.com/oauth2/authorize?client_id=312429302283108353&scope=bot&permissions=19457>");

        [Command("ping")]
        [Summary("Get the latency between the server and bot")]
        public async Task PingCommand()
            => await ReplyAsync($"The latency between the bot and {Context.Guild.VoiceRegionId} is **{Context.Client.Latency}ms**.");

        [Command("uptime")]
        [Summary("Get the uptime of the bot")]
        public async Task UptimeCommand()
            => await ReplyAsync(GetUptime());

        [Command("stats")]
        [Summary("Get stats on the bot")]
        public async Task StatsCommand()
        {

            if (Stats.IsReady)
            {

                using (Context.Channel.EnterTypingState())
                {

                    var appinfo = await Context.Client.GetApplicationInfoAsync();

                    var author = new EmbedAuthorBuilder()
                        .WithIconUrl(appinfo.IconUrl)
                        .WithName(Context.Client.CurrentUser.Username);

                    var footer = new EmbedFooterBuilder()
                        .WithText("Created by " + appinfo.Owner.Username);

                    var body = new EmbedBuilder()
                        .WithAuthor(author)
                        .WithFooter(footer)
                        .WithColor(Rand.GetRandomColor())
                        .WithTitle("Bot Statistics")
                        .WithDescription($"{Stats.UniqueUserCount} unique users\n" +
                        $"{Stats.GuildCount} guilds\n" +
                        $"{Stats.TextChannels} text channels\n" +
                        $"{Stats.VoiceChannels} voice channels\n" +
                        $"{Stats.RolesCount} roles");

                    body.AddInlineField("Largest Guild", $"{Stats.LargestGuild}\n{Stats.LargestGuildCount} users");
                    body.AddInlineField("Most Roles Guild", $"{Stats.MostRolesGuild}\n{Stats.MostRolesCount} roles");
                    body.AddInlineField($"Most Channels Guild", $"{Stats.MostChannelsGuild}\n{Stats.MostChannelsCount} channels");

                    await ReplyAsync("", embed: body);

                }

            }
            else
                await ReplyAsync("Bot statistics has not been calculated yet.");

        }

        [Command("info")]
        [Summary("Return info on the bot")]
        public async Task InfoCommand()
        {

            using (Context.Channel.EnterTypingState())
            {

                var ramUsage = ((double)_program.PrivateMemorySize64 / 1024 / 1024).ToString("#.##");
                var appInfo = await Context.Client.GetApplicationInfoAsync();
                var owner = appInfo.Owner;
                var creationDate = appInfo.CreatedAt;

                var author = new EmbedAuthorBuilder()
                    .WithName(appInfo.Name)
                    .WithUrl("https://github.com/MeLikeChoco/HearthstoneBot");

                var footer = new EmbedFooterBuilder()
                    .WithText($"Made by {owner.Username}");

                var body = new EmbedBuilder()
                    .WithAuthor(author)
                    .WithFooter(footer)
                    .WithColor(Rand.GetRandomColor())
                    .WithDescription($"**CPU Cores:** {Environment.ProcessorCount}" +
                    $"\n**Ram Usage:** {ramUsage} Megabytes" +
                    $"\n**Creation Date:** {creationDate.Month}/{creationDate.Day}/{creationDate.Year} {creationDate.Hour}:{creationDate.Second}" +
                    $"\n**Framework:** {RuntimeInformation.FrameworkDescription}" +
                    $"\n**Library:** Discord.NET {DiscordConfig.Version}" +
                    $"\n**Uptime:** {GetUptime()}" +
                    $"\n**Latency:** {Context.Client.Latency}ms")
                    .WithThumbnailUrl(appInfo.IconUrl);

                await ReplyAsync("", embed: body);

            }

        }

        [Command("feedback")]
        [Summary("Direct your feedback here")]
        public async Task FeedbackCommand([Remainder]string feedback)
        {

            var channel = Context.Client.GetChannel(296117398132752384) as SocketTextChannel;
            var sender = Context.User;
            var guild = Context.Guild;

            var author = new EmbedAuthorBuilder()
                .WithName(sender.Username)
                .WithIconUrl(sender.GetAvatarUrl());

            var footer = new EmbedFooterBuilder()
                .WithText($"{guild.Name} | {guild.Id}")
                .WithIconUrl(guild.IconUrl);

            var body = new EmbedBuilder()
                .WithColor(Rand.GetRandomColor())
                .WithAuthor(author)
                .WithFooter(footer)
                .WithDescription(feedback);

            await channel.SendMessageAsync("", embed: body);

        }

        [Command("help")]
        [Summary("Defacto help command")]
        public async Task HelpCommand()
        {

            bool isAdmin;

            if (Context.Channel is SocketGuildChannel)
            {

                var user = Context.User as SocketGuildUser;
                isAdmin = user.GuildPermissions.Administrator;

            }

            var modules = Context.User.Id == Context.Client.GetApplicationInfoAsync().Result.Owner.Id && Context.Guild.Id == 171432768767524864
                ? _cmdService.Modules : _cmdService.Modules.Where(m => m.Name != "Utility");

            var builder = new StringBuilder("```css\n" +
                "The Defacto Help Menu" +
                $"\n{"-".PadRight(50, '-')}\n");

            foreach (var module in modules)
            {

                foreach (var command in module.Commands)
                {

                    var preconditionResult = await command.CheckPreconditionsAsync(Context);

                    if (preconditionResult.ErrorReason == null || !preconditionResult.ErrorReason.Contains("permission Administrator"))
                    {

                        var commandInfo = command.Name;

                        foreach (var param in command.Parameters)
                        {

                            commandInfo += $" <{param}>";

                        }

                        builder.Append(commandInfo.PadRight(20, ' '));
                        builder.Append(" | ");
                        builder.Append(command.Summary);
                        builder.AppendLine();

                    }

                }

            }

            builder.AppendLine("The bot also supports inline card searches! Ex. \"I like {Armor Plating} and {Stalagg} in Hearthstone!\" " +
                "will search out Armor Plating and Stalagg! That's right, it can do multiple card searches at once!");
            builder.Append($"{"-".PadRight(50, '-')}```");

            await ReplyAsync(builder.ToString());

        }

        private string GetUptime()
        {

            var timespan = DateTime.Now - _program.StartTime;
            var builder = new StringBuilder("The bot has been up for ");

            if (timespan.Days != 0)
                builder.Append($"**{timespan.Days} days**, ");

            if (timespan.Hours != 0)
                builder.Append($"**{timespan.Hours} hours**, ");

            if (timespan.Minutes != 0)
                builder.Append($"**{timespan.Minutes} minutes**, ");

            if (timespan.Seconds != 0 && timespan.Minutes != 0)
                builder.Append($"and **{timespan.Seconds} seconds**.");
            else
                builder.Append($"**{timespan.Seconds} seconds**.");

            return builder.ToString();

        }

    }
}