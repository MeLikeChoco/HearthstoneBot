using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using MoreLinq;
using Discord;

namespace HearthstoneBot.Services
{
    public class Chat
    {

        public const string Pattern = "<.+?>";

        public static Task CardSearch(SocketMessage message)
        {

            var matches = Regex.Matches(message.Content, Pattern);

            if (matches.Count < 4 && matches.Count != 0)
            {

                foreach (var match in matches.OfType<Match>())
                {

                    var card = match.Value;
                    card = card.Substring(1, card.Length - 2).ToLower();

                    if (Cache.EmbedsCache.TryGetValue(card, out EmbedBuilder embed))
                    {

                        

                    }

                }

            }

            return Task.CompletedTask;

        }

        /// <summary>
        /// Levenshtein distance
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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
