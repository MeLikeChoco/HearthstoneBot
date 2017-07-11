using Discord;
using Discord.WebSocket;
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
        public static ConcurrentDictionary<ulong, object> GuessGames { get; set; }

        public static void InitializeCache()
        {

            AltConsole.Print("Caching all cards in Database.json...");

            GuessGames = new ConcurrentDictionary<ulong, object>();
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

                if (!string.IsNullOrEmpty(card.FullArt)
                && card.Set.ToLower() != "credits"
                && card.Set.ToLower() != "tavern brawl"
                && card.Set.ToLower() != "cheat"
                && !card.Name.Contains("("))
                {

                    if (card.Source == null)
                        tempFullArts[card.Name] = card.FullArt;
                    else if (card.Source.Type != SourceType.Generated
                    && card.Source.Type != SourceType.Chosen)
                        tempFullArts[card.Name] = card.FullArt;

                }


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

        }

        private static EmbedBuilder GenerateEmbed(Card card)
        {

            var author = new EmbedAuthorBuilder()
                .WithIconUrl(new Uri(GetAuthorIcon(card.Class.FirstOrDefault())))
                .WithUrl(new Uri(card.Url))
                .WithName(card.Name);

            var footer = new EmbedFooterBuilder()
                .WithIconUrl(new Uri("http://i.imgur.com/zz1Bcek.png"))
                .WithText(card.Collectability.ToString());

            var fullArtUri = string.IsNullOrEmpty(card.FullArt) ? null : new Uri(card.FullArt);
            Uri thumbnailUri;

            if (string.IsNullOrEmpty(card.GoldImage ?? card.RegularImage)) //stupid little trick
                thumbnailUri = null;
            else if (!string.IsNullOrEmpty(card.GoldImage))
                thumbnailUri = new Uri(card.GoldImage);
            else
                thumbnailUri = new Uri(card.RegularImage);

            var body = new EmbedBuilder
            {

                Author = author,
                Color = GetColor(card.Class.FirstOrDefault()),
                ImageUrl = fullArtUri,
                ThumbnailUrl = thumbnailUri,
                Footer = footer,

            };

            body.Description = GenerateDescription(card);

            if (!string.IsNullOrEmpty(card.Description))
                body.AddField("Description", card.Description);

            if (!string.IsNullOrEmpty(card.Lore))
                body.AddField("Lore", card.Lore);

            if (!string.IsNullOrEmpty(card.Aquisition))
            {

                var aquisition = card.Aquisition.Length > 1024 ? card.Aquisition.Substring(0, 1024) : card.Aquisition;

                body.AddField("Aquisition", aquisition);

            }

            if (card.Source != null)
            {

                switch (card.Source.Type)
                {

                    case SourceType.Generated:
                        body.AddField("Generated by", string.Join(" / ", card.Source.Sources));
                        break;
                    case SourceType.Summoned:
                        body.AddField("Summoned by", string.Join(" / ", card.Source.Sources));
                        break;
                    case SourceType.Transformed:
                        body.AddField("Transformed by", string.Join(" / ", card.Source.Sources));
                        break;
                    case SourceType.Chosen:
                        body.AddField("Chosen from", string.Join(" / ", card.Source.Sources));
                        break;

                }

            }

            if (card.Bosses != null)
                body.AddField(card.Bosses.Length == 1 ? "Boss" : "Bosses", string.Join(" / ", card.Bosses));

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

            if (!card.Class.Contains(Class.None))
                builder.AppendLine($"**Class:** {string.Join(" / ", card.Class.Select(c => c.ToString()))}");

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

        //TODO THINK ABOUT CHANGING COLOR TO REFLECT ON TYPE
        private static Color GetColor(Class cardClass)
        {

            switch (cardClass)
            {

                case Class.Hunter:
                    return new Color(102, 102, 102);
                case Class.Priest:
                    return new Color(167, 174, 182);
                case Class.Warlock:
                    return new Color(78, 50, 88);
                case Class.Warrior:
                    return new Color(87, 97, 88);
                case Class.Paladin:
                    return new Color(183, 137, 60);
                case Class.Druid:
                    return new Color(104, 59, 40);
                case Class.Mage:
                    return new Color(107, 118, 163);
                case Class.Shaman:
                    return new Color(50, 53, 96);
                case Class.Rogue:
                    return new Color(55, 54, 60);
                default:
                    return new Color(107, 86, 75);

            }

        }

        private static string GetAuthorIcon(Class cardClass)
        {

            switch (cardClass)
            {

                case Class.Druid:
                    return "https://www.burning-crusade.com/wp-content/uploads/2014/05/druid_1.png";
                case Class.Hunter:
                    return "http://media-hearth.cursecdn.com/attachments/0/150/hunter_4.png";
                case Class.Mage:
                    return "http://media-hearth.cursecdn.com/attachments/0/151/mage_13.png";
                case Class.Paladin:
                    return "http://www.deckselect.eu/img/Paladin.png";
                case Class.Priest:
                    return "http://media-hearth.cursecdn.com/attachments/0/153/priest_12.png";
                case Class.Rogue:
                    return "http://media-hearth.cursecdn.com/attachments/0/154/rogue_8.png";
                case Class.Shaman:
                    return "http://media-hearth.cursecdn.com/attachments/0/155/shaman_5.png";
                case Class.Warlock:
                    return "http://www.deckselect.eu/img/Warlock.png";
                case Class.Warrior:
                    return "http://www.deckselect.eu/img/Warrior.png";
                default:
                    return "https://cdn.iconverticons.com/files/png/e374004e6f5ac18b_256x256.png";

            }

        }

        private static void Log(string message)
            => AltConsole.InLinePrint("Service", "Cache", message);

    }
}
