using Discord;
using HearthstoneBot.Core;
using HearthstoneBot.Objects;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HearthstoneBot.Services
{
    public static class Cache
    {

        private const string DbPath = "Database/Database.json";
        private const string Check = "N/A";

        public static Dictionary<string, EmbedBuilder> Embeds { get; private set; }
        public static HashSet<string> CardNames { get; private set; }
        public static List<Card> CardObjects { get; private set; }

        public static void InitializeCache()
        {

            AltConsole.Print("Caching all cards in Database.json...");

            var json = File.ReadAllText(DbPath);
            var cards = JsonConvert.DeserializeObject<List<Card>>(json);
            var tempDict = new ConcurrentDictionary<string, EmbedBuilder>();
            var tempList = new ConcurrentBag<string>();
            var tempObjects = new ConcurrentBag<Card>();
            var counter = 0;
            var totalAmount = cards.Count;

            Parallel.ForEach(cards, card =>
            {

                var embed = GenerateEmbed(card);
                var cardName = card.Name.ToLower();

                tempDict[cardName] = embed;
                tempList.Add(cardName);
                tempObjects.Add(card);

                Log($"Progress: {Interlocked.Increment(ref counter)}/{totalAmount}");

            });

            CardNames = new HashSet<string>(tempList);
            CardObjects = new List<Card>(tempObjects);
            Embeds = tempDict.ToDictionary();

            AltConsole.Print("Finished caching all cards in Database.json");

        }

        private static EmbedBuilder GenerateEmbed(Card card)
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

            body.Description = GenerateDescription(card);

            if (!string.IsNullOrEmpty(card.Description))
                body.AddField("Description", card.Description);

            if (!string.IsNullOrEmpty(card.Lore))
                body.AddField("Lore", card.Lore);

            if (!string.IsNullOrEmpty(card.Aquisition))
                body.AddField("Aquisition", card.Aquisition);

            if (!string.IsNullOrEmpty(card.Bosses))
                body.AddField(card.Bosses.Split(',').Length == 1 ? "Boss" : "Bosses", card.Bosses);

            if (card.Abilities != null)
                body.AddField("Abilities", string.Join(", ", card.Abilities));

            if (card.Tags != null)
                body.AddField("Tags", string.Join(", ", card.Tags));

            if (card.Artist != null)
                body.AddField("Artist", card.Artist);

            return body;

        }

        private static string GenerateDescription(Card card)
        {

            var builder = new StringBuilder($"**Set:** {card.Set}\n" +
                $"**Type:** {card.Type}\n");

            if (card.Class != Check)
                builder.AppendLine($"**Class:** {card.Class}");

            if (card.Rarity != Check)
                builder.AppendLine($"**Rarity:** {card.Rarity}");

            if (card.ManaCost != Check)
                builder.AppendLine($"**Mana Cost:** {card.ManaCost}");

            if (card.Attack != Check)
                builder.AppendLine($"**Attack:** {card.Attack}");

            if (card.Health != Check)
                builder.AppendLine($"**Health:** {card.Health}");

            if (card.Durability != Check)
                builder.AppendLine($"**Durability:** {card.Durability}");

            return builder.ToString();

        }

        private static Color GetColor(string cardClass)
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

        private static void Log(string message)
            => AltConsole.InLinePrint("Service", "Cache", message);

    }
}
