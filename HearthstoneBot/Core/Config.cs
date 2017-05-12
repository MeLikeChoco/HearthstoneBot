using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthstoneBot.Properties;

namespace HearthstoneBot.Core
{
    public static class Config
    {

        public static string BotToken { get; private set; }
        public static string TestToken { get; private set; }
        public static string Game { get; private set; }

        public static void Initialize()
        {

            AltConsole.Print("Config", "Config", "Initializing configuration...");

            var manager = Resources.ResourceManager;

            BotToken = manager.GetString("BotToken");
            TestToken = manager.GetString("TestToken");
            Game = manager.GetString("Game");

            AltConsole.Print("Config", "Config", "Configuration initialized.");

        }

    }
}
