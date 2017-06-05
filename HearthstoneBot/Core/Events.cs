using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HearthstoneBot.Objects;
using HearthstoneBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Core
{
    public static class Events
    {

        private static DiscordSocketClient _client;
        private static CommandService _commands;
        private static IServiceProvider _services;

        public static void Initialize(DiscordSocketClient clientParams, CommandService commandParams)
        {

            _client = clientParams;
            _commands = commandParams;

        }

        public static Task Log(LogMessage message)
            => Task.Run(() => AltConsole.Print(message.Severity, message.Source, message.Message, message.Exception));

        public static async Task CommandHandler(SocketMessage possibleCmd)
        {

            var message = possibleCmd as SocketUserMessage;

            if (message == null || message.Author.IsBot)
                return;

            var argPos = 0;
            string prefix;

            if (message.Channel is SocketDMChannel)
                prefix = "h^";
            else
                prefix = Settings.GetPrefix((message.Channel as SocketGuildChannel).Guild.Id);

            //if message is a command and doesnt just contain the prefix
            if ((message.HasStringPrefix(prefix, ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) && 
                message.Content.Trim() != prefix)
            {

                var context = new SocketCommandContext(_client, message);

                if(message.Channel is SocketDMChannel)
                {

                    AltConsole.Print("Command", "DM", $"{(message.Channel as SocketDMChannel).Recipient.Username}");
                    AltConsole.Print("Command", "DM", $"{message.Content}");

                }
                else
                {

                    AltConsole.Print("Command", "Guild", $"{(message.Channel as SocketGuildChannel).Guild.Name}");
                    AltConsole.Print("Command", "Guild", $"{message.Content}");

                }

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {

                    if (result.ErrorReason.ToLower().Contains("unknown command"))
                        return;

                    await context.Channel.SendMessageAsync("There was an error in the command.");
                    //await context.Channel.SendMessageAsync("https://goo.gl/JieFJM");

                    AltConsole.Print("Error", "Error", result.ErrorReason);
                    //debug purposes
                    //await context.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");

                }

            }

        }

        public static async Task StartServices()
        {

            _services = new ServiceCollection()
                .BuildServiceProvider();

            Cache.InitializeCache();
            await Settings.InitializeSettings();
            HStats.Initialize();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

        }

        public static async Task SetGame()
        {

            AltConsole.Print("Client", "Game", "Setting game message...");
            await _client.SetGameAsync(Config.Game);
            AltConsole.Print("Client", "Game", "Game has been set.");

        }
    }
}
