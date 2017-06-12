﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Objects
{
    public class Card
    {

        public string Name { get; set; }
        public string Set { get; set; } = "N/A";
        [JsonConverter(typeof(StringEnumConverter))]
        public Type Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Race Race { get; set; }
        public string Class { get; set; } = "N/A";
        [JsonConverter(typeof(StringEnumConverter))]
        public Rarity Rarity { get; set; }

        public string ManaCost { get; set; }
        public string Attack { get; set; }
        public string Health { get; set; }
        public string Durability { get; set; }
        //using a set cause wynaut
        public string[] Abilities { get; set; }
        public string[] Tags { get; set; }

        public string Description { get; set; }
        public string Lore { get; set; }
        public string Aquisition { get; set; }
        public string Bosses { get; set; }

        public string RegularImage { get; set; }
        public string GoldImage { get; set; }
        public string FullArt { get; set; }

        public string Artist { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CollectableStatus Collectability { get; set; }

        public string Url { get; set; }

    }

    public enum CollectableStatus
    {

        Collectible,
        Uncollectible

    }

    public enum Type
    {

        Unknown,
        Spell,
        Weapon,
        Minion,
        Enchantment

    }

    public enum Race
    {

        General,
        Beast,
        Demon,
        Dragon,
        Elemental,
        Mech,
        Pirate,
        Totem

    }

    public enum Rarity
    {

        Basic,
        Common,
        Rare,
        Epic,
        Legendary

    }
}
