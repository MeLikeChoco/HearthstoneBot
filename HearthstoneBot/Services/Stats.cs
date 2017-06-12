using Discord.WebSocket;
using HearthstoneBot.Core;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HearthstoneBot.Services
{
    public static class Stats
    {

        public static int GuildCount { get; private set; }
        public static string LargestGuild { get; private set; }
        public static int LargestGuildCount { get; private set; }
        public static string MostRolesGuild { get; private set; }
        public static int MostRolesCount { get; private set; }
        public static string MostChannelsGuild { get; private set; }
        public static int MostChannelsCount { get; private set; }
        public static int TextChannels { get; private set; }
        public static int VoiceChannels { get; private set; }
        public static int RolesCount { get; private set; }
        public static int DMChannels { get; private set; }
        public static int UniqueUserCount { get; private set; }

        public static bool IsReady { get; private set; }

        private static DiscordSocketClient _client;
        private static Timer _calculateStats, _sendStats;

        public static void Initialize(DiscordSocketClient clientParams)
        {

            _client = clientParams;

        }

        public static Task ReadyStats()
        {

            _calculateStats = new Timer(GetStats, null, 10000, 3600000);
            _sendStats = new Timer(SendStats, null, 10000, 3600000);

            return Task.CompletedTask;

        }

        private static void GetStats(object state)
        {

            Log("Updating stats...");

            var guilds = _client.Guilds;
            var userSet = new HashSet<ulong>(guilds.SelectMany(guild => { guild.DownloadUsersAsync().GetAwaiter().GetResult(); return guild.Users.Where(user => !user.IsBot); }).Select(user => user.Id));
            int textChannels = 0, voiceChannels = 0, rolesCount = 0;
            var largestGuild = guilds.Where(guild => !guild.Name.Contains("Bot")).MaxBy(guild => guild.MemberCount);
            var mostRolesGuild = guilds.MaxBy(guild => guild.Roles.Count);
            var mostChannelsGuild = guilds.MaxBy(guild => guild.Channels.Count);

            GuildCount = guilds.Count;
            LargestGuild = largestGuild.Name;
            LargestGuildCount = largestGuild.MemberCount;
            MostRolesGuild = mostRolesGuild.Name;
            MostRolesCount = mostRolesGuild.Roles.Count;
            MostChannelsGuild = mostChannelsGuild.Name;
            MostChannelsCount = mostChannelsGuild.Channels.Count;
            UniqueUserCount = userSet.Count;

            Parallel.ForEach(guilds, guild =>
            {

                Interlocked.Add(ref textChannels, guild.TextChannels.Count);
                Interlocked.Add(ref voiceChannels, guild.VoiceChannels.Count);
                Interlocked.Add(ref rolesCount, guild.Roles.Count);

            });

            TextChannels = textChannels;
            VoiceChannels = voiceChannels;
            RolesCount = rolesCount;

            IsReady = true;

            Log("Stats updated.");

        }

        private static void SendStats(object state)
        {

            Web.SendStats(GuildCount).GetAwaiter().GetResult();

        }

        private static void Log(string message)
            => AltConsole.Print("Service", "Stats", message);

    }
}
