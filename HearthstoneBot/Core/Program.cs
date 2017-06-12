using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HearthstoneBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS4NetCore;

namespace HearthstoneBot.Core
{
    public class Program
    {

        static void Main(string[] args)
            => new Program().Run(args).GetAwaiter().GetResult();

        private string[] _args;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private bool _isTestRun;

        public async Task Run(string[] args)
        {

            AltConsole.Print("Hearthstone Bot has started up...");

            _args = args;

            ParseArgs();            
            Config.Initialize();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {

                LogLevel = LogSeverity.Verbose,
                WebSocketProvider = WS4NetProvider.Instance,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true,

            });
            _commands = new CommandService(new CommandServiceConfig
            {
                                
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose,           

            });

            InitializeLogging();
            InitializeServices();
            Events.Initialize(_client, _commands);

            await LoginAndConnect();

            await Task.Delay(-1);

        }

        public void InitializeLogging()
        {

            _client.Log += Events.Log;
            _commands.Log += Events.Log;

        }

        public void InitializeServices()
        {

            _client.Connected += Events.StartServices;
            _client.MessageReceived += Events.CommandHandler;
            _client.MessageReceived += Chat.CardSearch;
            _client.Ready += Events.SetGame;
            _client.Ready += Stats.ReadyStats;
            _client.JoinedGuild += Settings.CreateSettings;

        }

        public async Task LoginAndConnect()
        {

            string token = _isTestRun ? Config.TestToken : Config.BotToken;

            AltConsole.Print("Client", "Gateway", "Logging in...");
            await _client.LoginAsync(TokenType.Bot, token);
            AltConsole.Print("Client", "Gateway", "Logged in.");
            AltConsole.Print("Client", "Gateway", "Starting up...");
            await _client.StartAsync();
            AltConsole.Print("Client", "Gateway", "Finished starting up...");

        }

        public void ParseArgs()
            => _isTestRun = bool.Parse(_args.FirstOrDefault() ?? "False");

    }
}
