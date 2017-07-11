using HearthstoneBot.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;
using Dapper.Contrib.Extensions;
using Dapper;

namespace HearthstoneBot.Services
{
    public static class Settings
    {

        private const string DbPath = "Data Source = Database/Settings.db";
        private const string DefaultPrefix = "h^";
        private static ConcurrentDictionary<ulong, GuildSetting> _guildSettings;

        public static async Task SetPrefix(ulong id, string prefix)
        {

            var settings = _guildSettings[id];

            settings.Prefix = prefix;
            _guildSettings[id] = settings;

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                await connection.UpdateAsync(settings);

                connection.Close();

            }

        }

        public static string GetPrefix(ulong id)
        {

            return _guildSettings[id].Prefix;

        }

        public static async Task SetMinimalSetting(ulong id, bool isMinimal)
        {

            var settings = _guildSettings[id];

            settings.IsMinimal = isMinimal;
            _guildSettings[id] = settings;

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                await connection.UpdateAsync(settings);

                connection.Close();

            }

        }

        public static bool GetMinimalSetting(ulong id)
        {

            return _guildSettings[id].IsMinimal;

        }

        public static async Task SetGuessChannel(SocketGuildChannel channel)
        {

            var setting = _guildSettings[channel.Guild.Id];

            if (setting.GuessChannels.Contains(channel.Id))
                return;

            setting.AddGuessChannel(channel.Id);

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                await connection.UpdateAsync(setting);

                connection.Close();

            }

        }

        public static async Task RemoveGuessChannel(SocketGuildChannel channel)
        {

            var setting = _guildSettings[channel.Guild.Id];

            if (!setting.GuessChannels.Contains(channel.Id))
                return;

            setting.RemoveGuessChannel(channel.Id);

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                await connection.UpdateAsync(setting);

                connection.Close();

            }

        }

        public static bool IsGuessChannel(SocketGuildChannel channel)
        {

            var setting = _guildSettings[channel.Guild.Id];

            if (setting.GuessChannels.Contains(channel.Id))
                return true;
            else
                return false;

        }

        public static async Task SendSqlStatement(string statement)
        {

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                await connection.ExecuteAsync(statement);

                connection.Close();

            }

        }

        public static async Task CreateSettings(SocketGuild guild)
        {

            try
            {

                using (var connection = new SqliteConnection(DbPath))
                {

                    await connection.OpenAsync();

                    var possibleEntry = await connection.ExecuteScalarAsync<ulong>("select Id from GuildSettings where Id = @Id", new { Id = guild.Id });

                    if (possibleEntry == 0)
                    {

                        var setting = new GuildSetting
                        {

                            Id = guild.Id,
                            Prefix = DefaultPrefix,
                            IsMinimal = false,
                            GuessChannelsString = "0",

                        };

                        await connection.InsertAsync(setting);
                        _guildSettings[guild.Id] = setting;

                    }

                    connection.Close();

                }

            }catch(Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }

        }

        public static async Task InitializeSettings(DiscordSocketClient client)
        {

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                var settings = await connection.GetAllAsync<GuildSetting>();
                _guildSettings = new ConcurrentDictionary<ulong, GuildSetting>(settings.ToDictionary(setting => setting.Id, setting => setting));
                var uninitializedGuilds = client.Guilds.Where(guild => !_guildSettings.ContainsKey(guild.Id)).ToList();

                foreach (var guild in uninitializedGuilds)
                {

                    await CreateSettings(guild);

                }

                connection.Close();

            }

        }
    }
}
