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

        private const string DbPath = "Database/Settings.db";
        private const string DefaultPrefix = "h^";
        private static ConcurrentDictionary<ulong, GuildSetting> _guildSettings = new ConcurrentDictionary<ulong, GuildSetting>();

        public static async Task StorePrefix(ulong id, string prefix)
        {

            

        }

        public static string GetPrefix(ulong id)
        {

            _guildSettings.TryGetValue(id, out GuildSetting setting);
            return setting.Prefix;

        }

        public static async Task CreateSettings(SocketGuild guild)
        {

            using (var connection = new SqliteConnection(DbPath))
            {

                await connection.OpenAsync();

                var possibleEntry = await connection.ExecuteScalarAsync<ulong?>("select Id from GuildSettings where Id = @Id", new { Id = guild.Id });

                if (possibleEntry == null)
                    return;

                var setting = new GuildSetting
                {

                    Id = guild.Id,
                    Prefix = DefaultPrefix,
                    IsMinimal = false,

                };

                await connection.InsertAsync(setting);

                connection.Close();

            }

        }
    }
}
