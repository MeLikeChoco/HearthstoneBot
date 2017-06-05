﻿using Discord;
using HearthstoneBot.Core;
using HearthstoneBot.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Services
{
    public static class Cache
    {

        private const string DbPath = "Database/Database.json";

        public static ConcurrentDictionary<string, Card> Cards { get; private set; }
        public static ConcurrentDictionary<Card, EmbedBuilder> Embeds { get; set; }

        public static void InitializeCache()
        {

            AltConsole.Print("Caching all cards in Database.json...");

            var json = File.ReadAllText(DbPath);
            var cards = JsonConvert.DeserializeObject<List<Card>>(json);
            Cards = new ConcurrentDictionary<string, Card>();
            Embeds = new ConcurrentDictionary<Card, EmbedBuilder>();

            Parallel.ForEach(cards, card =>
            {

                var name = card.Name.ToLower();
                Cards.TryAdd(name, card);

            });

            AltConsole.Print("Finished caching all cards in Database.json");

        }

        public static void AddToEmbedsCache(Card card, EmbedBuilder embed)
            => Embeds.TryAdd(card, embed);

    }
}
