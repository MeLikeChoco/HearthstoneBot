﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using MoreLinq;
using Discord;
using HearthstoneBot.Objects;
using Force.DeepCloner;
using HearthstoneBot.Core;

namespace HearthstoneBot.Services
{
    public class Chat
    {

        public const string InlinePattern = "{{.+?}}", Check = "N/A";

        public static async Task CardSearch(SocketMessage message)
        {

            if (message.Author.IsBot || message.Content.Contains("eval"))
                return;

            var matches = Regex.Matches(message.Content, InlinePattern);

            if (matches.Count < 4 && matches.Count != 0)
            {

                using (message.Channel.EnterTypingState())
                {

                    bool isMinimal;
                    string guildName = null;

                    if (message.Channel is SocketGuildChannel)
                    {

                        var guild = (message.Channel as SocketGuildChannel).Guild;
                        guildName = guild.Name;
                        isMinimal = Settings.GetMinimalSetting(guild.Id);

                    }
                    else
                        isMinimal = false;

                    foreach (var match in matches.OfType<Match>())
                    {

                        var search = match.Value;
                        search = search.Substring(2, search.Length - 4).ToLower();

                        AltConsole.Print($"{message.Author.Username} from {guildName ?? "DM Channel"} searched for {search}");

                        var array = search.Split(' ');

                        var embed = Cache.Embeds.FirstOrDefault(kv => kv.Key == search).Value
                            ?? Cache.Embeds.FirstOrDefault(kv => array.All(str => kv.Key.Contains(str))).Value
                            //?? Cache.Embeds.FirstOrDefault(kv => kv.Key.Split(' ').Any(str => array.Contains(str))).Value
                            ?? Cache.Embeds.MinBy(kv => Compute(kv.Key, search)).Value;

                        //var test = Cache.Embeds.Where(kv => array.Any(str => kv.Key.Contains(str)));

                        //if(embed == null)
                        //{

                        //    var array = search.Split(' ');
                        //    embed = Cache.Embeds.FirstOrDefault(kv => array.All(str => kv.Key.ToLower().Contains(str))).Value;

                        //    if(embed == null)
                        //        embed = Cache.Embeds.MinBy(kv => Compute(kv.Key, search)).Value;

                        //}

                        await message.Channel.SendMessageAsync("", embed: CleanEmbed(embed, isMinimal));

                    }

                }

            }

        }

        public static EmbedBuilder CleanEmbed(EmbedBuilder embedToClean, bool isMinimal)
        {

            if (!isMinimal)
                return embedToClean;

            var clone = embedToClean.DeepClone();

            if (isMinimal)
            {

                clone.ImageUrl = null;
                clone.Fields.RemoveAll(field => field.Name == "Abilities" || field.Name == "Tags");

            }

            return clone;

        }

        /// <summary>
        /// Levenshtein distance
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns>int</returns>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {

                for (int j = 1; j <= m; j++)
                {

                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);

                }

            }

            return d[n, m];
        }

    }
}
