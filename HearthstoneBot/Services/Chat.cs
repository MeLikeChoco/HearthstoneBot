using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using MoreLinq;
using Discord;
using HearthstoneBot.Objects;

namespace HearthstoneBot.Services
{
    public class Chat
    {

        public const string Pattern = "<.+?>";

        public static async Task CardSearch(SocketMessage message)
        {

            var matches = Regex.Matches(message.Content, Pattern);

            if (matches.Count < 4 && matches.Count != 0)
            {

                using (message.Channel.EnterTypingState())
                {

                    bool isMinimal;

                    if (message.Channel is SocketGuildChannel)
                    {

                        var guild = (message.Channel as SocketGuildChannel).Guild;
                        isMinimal = Settings.GetMinimalSetting(guild.Id);

                    }
                    else
                        isMinimal = false;

                    foreach (var match in matches.OfType<Match>())
                    {

                        var card = match.Value;
                        card = card.Substring(1, card.Length - 2).ToLower();

                        var embed = Cache.EmbedsCache.FirstOrDefault(kv => kv.Key.Name.ToLower() == card).Value;

                        if (embed != null)
                        {

                            await message.Channel.SendMessageAsync("", embed: CleanEmbed(embed, isMinimal));
                            return;

                        }
                        else
                        {

                            if (!Cache.Cards.TryGetValue(card, out Card closestCard))
                            {

                                var cardArray = card.Split(' ');
                                closestCard = Cache.Cards.AsParallel().FirstOrDefault(kv => cardArray.All(str => kv.Key.Contains(str))).Value;

                                if (closestCard == null)
                                    closestCard = Cache.Cards.AsParallel().MinBy(kv => Compute(kv.Key, card)).Value;

                            }

                            await PrintToChat(closestCard, message.Channel, isMinimal);

                        }

                    }

                }

            }

        }

        public static async Task PrintToChat(Card card, ISocketMessageChannel channel, bool isMinimal)
        {

            var author = new EmbedAuthorBuilder()
                .WithIconUrl("https://cdn.iconverticons.com/files/png/e374004e6f5ac18b_256x256.png")
                .WithUrl("http://us.battle.net/hearthstone/en/")
                .WithName("Hearthstone");

            var footer = new EmbedFooterBuilder()
                .WithIconUrl("http://i.imgur.com/zz1Bcek.png")
                .WithText("Brought to you by The One and Only");

            var body = new EmbedBuilder
            {

                Author = author,
                Color = GetColor(card.Class),
                ImageUrl = card.GoldImage,
                Url = card.Url,
                ThumbnailUrl = card.RegularImage,
                Title = card.Name,
                Footer = footer,

            };

            body.Description = $"**Set:** {card.Set}\n" +
                $"**Type:** {card.Type}\n" +
                $"**Class:** {card.Class}\n" +
                $"**Rarity:** {card.Rarity}\n" +
                $"**Mana Cost:** {card.ManaCost}\n" +
                $"**Attack:** {card.Attack}\n" +
                $"**Health:** {card.Health}\n" +
                $"**Durability:** {card.Durability}";

            body.AddField("Description", card.Description ?? "N/A");
            body.AddField("Lore", card.Lore ?? "N/A");
            body.AddField("Abilities", string.Join(", ", card.Abilities ?? new string[] { "None" }));
            body.AddField("Tags", string.Join(", ", card.Tags ?? new string[] { "None" }));
            body.AddField("Artist", card.Artist ?? "None");

            Cache.AddToEmbedsCache(card, body);

            await channel.SendMessageAsync("", embed: CleanEmbed(body, isMinimal));

        }

        public static Color GetColor(string cardClass)
        {

            switch (cardClass)
            {

                case "Hunter":
                    return new Color(102, 102, 102);
                case "Priest":
                    return new Color(167, 174, 182);
                case "Warlock":
                    return new Color(78, 50, 88);
                case "Warrior":
                    return new Color(87, 97, 88);
                case "Paladin":
                    return new Color(183, 137, 60);
                case "Druid":
                    return new Color(104, 59, 40);
                case "Mage":
                    return new Color(107, 118, 163);
                case "Shaman":
                    return new Color(50, 53, 96);
                case "Rogue":
                    return new Color(55, 54, 60);
                default:
                    return new Color(107, 86, 75);


            }

        }

        public static EmbedBuilder CleanEmbed(EmbedBuilder embedToClean, bool isMinimal)
        {

            if (isMinimal && embedToClean.ImageUrl != null)
            {

                var image = embedToClean.ImageUrl;
                embedToClean.ImageUrl = null;

                if (embedToClean.ThumbnailUrl == null)
                    embedToClean.ThumbnailUrl = image;

            }

            return embedToClean;

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
