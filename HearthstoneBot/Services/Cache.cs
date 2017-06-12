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
        public static Dictionary<string, Card> CardObjectDict { get; private set; }
        public static HashSet<string> CardNames { get; private set; }
        public static HashSet<string> LowerCardNames { get; private set; }
        public static HashSet<Card> CardObjects { get; private set; }
        public static Dictionary<string, string> FullArts { get; private set; }
        public static ConcurrentDictionary<ulong, bool> GuessGames { get; set; }

        public static void InitializeCache()
        {

            AltConsole.Print("Caching all cards in Database.json...");

            var json = File.ReadAllText(DbPath);
            CardObjects = JsonConvert.DeserializeObject<HashSet<Card>>(json);
            var tempFullArts = new ConcurrentDictionary<string, string>();
            var tempDict = new ConcurrentDictionary<string, EmbedBuilder>();
            var tempList = new ConcurrentBag<string>();
            var lowerTempList = new ConcurrentBag<string>();
            var counter = 0;
            var totalAmount = CardObjects.Count;

            Parallel.ForEach(CardObjects, card =>
            {

                var embed = GenerateEmbed(card);
                var cardName = card.Name.ToLower();

                if (!string.IsNullOrEmpty(card.FullArt) && !card.Name.Contains("("))
                    tempFullArts[card.Name] = card.FullArt;

                tempDict[cardName] = embed;
                tempList.Add(card.Name);
                lowerTempList.Add(cardName);

                Log($"Progress: {Interlocked.Increment(ref counter)}/{totalAmount}");

            });
                        
            CardNames = new HashSet<string>(tempList);
            LowerCardNames = new HashSet<string>(lowerTempList);
            Embeds = tempDict.ToDictionary();
            CardObjectDict = CardObjects.ToDictionary(card => card.Name.ToLower(), card => card);
            FullArts = new Dictionary<string, string>(tempFullArts);

            AltConsole.Print("Finished caching all cards in Database.json");

            GuessGames = new ConcurrentDictionary<ulong, bool>();

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
                ImageUrl = card.FullArt,
                ThumbnailUrl = card.GoldImage ?? card.RegularImage,
                Url = card.Url,
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

            if (!string.IsNullOrEmpty(card.Artist))
                body.AddField("Artist", card.Artist);

            return body;

        }

        private static string GenerateDescription(Card card)
        {

            var builder = new StringBuilder($"**Set:** {card.Set}\n" +
                $"**Type:** {card.Type}\n");

            if (card.Type == Objects.Type.Minion)
                builder.AppendLine($"**Race:** {card.Race}");

            if (card.Class != Check)
                builder.AppendLine($"**Class:** {card.Class}");

            if (card.Rarity != Rarity.Basic)
                builder.AppendLine($"**Rarity:** {card.Rarity}");

            if (!string.IsNullOrEmpty(card.ManaCost))
                builder.AppendLine($"**Mana Cost:** {card.ManaCost}");

            if (!string.IsNullOrEmpty(card.Attack))
                builder.AppendLine($"**Attack:** {card.Attack}");

            if (!string.IsNullOrEmpty(card.Health))
                builder.AppendLine($"**Health:** {card.Health}");

            if (!string.IsNullOrEmpty(card.Durability))
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
