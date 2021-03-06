﻿using HearthstoneBot.Core;
using HearthstoneBot.Services;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//technically not an object, but put it here anyway

namespace HearthstoneBot.Objects
{
    public static class HStats
    {

        #region For Interlock.Increment

        private static int _cardCount = 0;
        private static int _collectibleCount = 0;
        private static int _uncollectibleCount = 0;

        //im not going to edit every variable to _lowerCase instead of _UpperCase
        private static int _DruidCardCount = 0;
        private static int _HunterCardCount = 0;
        private static int _MageCardCount = 0;
        private static int _PaladinCardCount = 0;
        private static int _PriestCardCount = 0;
        private static int _RogueCardCount = 0;
        private static int _ShamanCardCount = 0;
        private static int _WarlockCardCount = 0;
        private static int _WarriorCardCount = 0;
        private static int _NonClassCardCount = 0;

        private static int _BasicCardCount = 0;
        private static int _ClassicCardCount = 0;
        private static int _OldGodsCardCount = 0;
        private static int _KarzhanCardCount = 0;
        private static int _GadgetzanCardCount = 0;
        private static int _UnGoroCardCount = 0;
        private static int _HallOfFameCardCount = 0;
        private static int _NaxxramasCardCount = 0;
        private static int _GvGCardCount = 0;
        private static int _BlackrockCardCount = 0;
        private static int _TournamentCardCount = 0;
        private static int _ExplorersCardCount = 0;
        private static int _CheatCardCount = 0;

        private static int _nonRarityCount = 0;
        private static int _commonCount = 0;
        private static int _rareCount = 0;
        private static int _epicCount = 0;
        private static int _legendaryCount = 0;

        private static double _averageManaCost;
        private static double _averageAttack;
        private static double _averageHealth;
        private static double _averageDurability;

        private static ConcurrentDictionary<string, int> _artists = new ConcurrentDictionary<string, int>();
        private static ConcurrentDictionary<string, int> _abilities = new ConcurrentDictionary<string, int>();
        private static ConcurrentDictionary<string, int> _tags = new ConcurrentDictionary<string, int>();
        #endregion For Interlock.Increment

        public static int CardCount { get { return _cardCount; } }
        public static int CollectibleCount { get { return _collectibleCount; } }
        public static int UncollectibleCount { get { return _uncollectibleCount; } }

        public static int DruidCardCount { get { return _DruidCardCount; } }
        public static int HunterCardCount { get { return _HunterCardCount; } }
        public static int MageCardCount { get { return _MageCardCount; } }
        public static int PaladinCardCount { get { return _PaladinCardCount; } }
        public static int PriestCardCount { get { return _PriestCardCount; } }
        public static int RogueCardCount { get { return _RogueCardCount; } }
        public static int ShamanCardCount { get { return _ShamanCardCount; } }
        public static int WarlockCardCount { get { return _WarlockCardCount; } }
        public static int WarriorCardCount { get { return _WarriorCardCount; } }
        public static int NonClassCardCount { get { return _NonClassCardCount; } }

        public static int BasicCardCount { get { return _BasicCardCount; } }
        public static int ClassicCardCount { get { return _ClassicCardCount; } }
        public static int OldGodsCardCount { get { return _OldGodsCardCount; } }
        public static int KarzhanCardCount { get { return _KarzhanCardCount; } }
        public static int GadgetzanCardCount { get { return _GadgetzanCardCount; } }
        public static int UnGoroCardCount { get { return _UnGoroCardCount; } }
        public static int HallOfFameCardCount { get { return _HallOfFameCardCount; } }
        public static int NaxxramasCardCount { get { return _NaxxramasCardCount; } }
        public static int GvGCardCount { get { return _GvGCardCount; } }
        public static int BlackrockCardCount { get { return _BlackrockCardCount; } }
        public static int TournamentCardCount { get { return _TournamentCardCount; } }
        public static int ExplorersCardCount { get { return _ExplorersCardCount; } }
        public static int CheatCardCount { get { return _CheatCardCount; } }

        public static int NonRarityCount { get { return _nonRarityCount; } }
        public static int CommonCount { get { return _commonCount; } }
        public static int RareCount { get { return _rareCount; } }
        public static int EpicCount { get { return _epicCount; } }
        public static int LegendaryCount { get { return _legendaryCount; } }

        public static double AverageManaCost { get { return _averageManaCost; } }
        public static double AverageAttack { get { return _averageAttack; } }
        public static double AverageHealth { get { return _averageHealth; } }
        public static double AverageDurability { get { return _averageDurability; } }

        public static Dictionary<string, int> Artists { get { return _artists.ToDictionary(); } }
        public static Dictionary<string, int> Abilities { get { return _abilities.ToDictionary(); } }
        public static Dictionary<string, int> Tags { get { return _tags.ToDictionary(); } }

        public static void Initialize()
        {

            AltConsole.Print("Objects", "HStats", "Initializing HStats...");

            var cards = Cache.CardObjects;

            Parallel.ForEach(cards, card =>
            {                

                switch (card.Collectability)
                {

                    case CollectableStatus.Collectible:
                        Interlocked.Increment(ref _collectibleCount);
                        break;
                    case CollectableStatus.Uncollectible:
                        if (card.Set.ToLower() != "cheat")
                        {
                            Interlocked.Increment(ref _uncollectibleCount);
                        }
                        break;

                }

                foreach (var cardClass in card.Class)
                {

                    CountClass(cardClass);

                }


                if (card.Set.ToLower() != "cheat" && card.Abilities != null && card.Tags != null)
                {

                    CountAbilities(card.Abilities);
                    CountTags(card.Tags);

                }

                CountSet(card.Set);
                CountRarity(card.Rarity.ToString());
                CountArtists(card.Artist);

            });

            CalculateAverages();

            AltConsole.Print("Objects", "HStats", "HStats initialized.");

        }

        private static void CountTags(string[] tags)
        {

            foreach (var tag in tags)
            {

                if (_tags.ContainsKey(tag))
                    _tags[tag]++;
                else
                    _tags[tag] = 1;

            }

        }

        private static void CountAbilities(string[] abilities)
        {

            foreach (var ability in abilities)
            {

                if (_abilities.ContainsKey(ability))
                    _abilities[ability]++;
                else
                    _abilities[ability] = 1;

            }

        }

        private static void CalculateAverages()
        {

            var cards = Cache.CardObjects;
            int aManaCost, aAttack, aHealth, aDurability, manaCounter, attackCounter, healthCounter, durabilityCounter;
            aManaCost = aAttack = aHealth = aDurability = manaCounter = attackCounter = healthCounter = durabilityCounter = 0;

            Parallel.ForEach(cards, card =>
            {

                if(card.Set.ToLower() != "cheat")
                {

                    if (!string.IsNullOrEmpty(card.ManaCost))
                    {

                        Interlocked.Add(ref aManaCost, int.Parse(card.ManaCost));
                        Interlocked.Increment(ref manaCounter);

                    }

                    if (!string.IsNullOrEmpty(card.Attack))
                    {

                        Interlocked.Add(ref aAttack, int.Parse(card.Attack));
                        Interlocked.Increment(ref attackCounter);

                    }

                    if (!string.IsNullOrEmpty(card.Health) && !card.Health.Contains("Heroic"))
                    {

                        Interlocked.Add(ref aHealth, int.Parse(card.Health));
                        Interlocked.Increment(ref healthCounter);

                    }

                    if (!string.IsNullOrEmpty(card.Durability))
                    {

                        Interlocked.Add(ref aDurability, int.Parse(card.Durability));
                        Interlocked.Increment(ref durabilityCounter);

                    }

                }

            });

            _averageManaCost = aManaCost / manaCounter;
            _averageAttack = aAttack / attackCounter;
            _averageHealth = aHealth / healthCounter;
            _averageDurability = aDurability / durabilityCounter;

        }

        private static void CountRarity(string rarity)
        {

            switch (rarity)
            {

                case "Common":
                    Interlocked.Increment(ref _commonCount);
                    break;
                case "Rare":
                    Interlocked.Increment(ref _rareCount);
                    break;
                case "Epic":
                    Interlocked.Increment(ref _epicCount);
                    break;
                case "Legendary":
                    Interlocked.Increment(ref _legendaryCount);
                    break;
                default:
                    Interlocked.Increment(ref _nonRarityCount);
                    break;

            }

        }

        private static void CountArtists(string artist)
        {

            if (!string.IsNullOrEmpty(artist))
            {

                if (_artists.ContainsKey(artist))
                    _artists[artist]++;
                else
                    _artists[artist] = 1;

            }

        }

        private static void CountClass(Class cardClass)
        {

            switch (cardClass)
            {

                case Class.Druid:
                    Interlocked.Increment(ref _DruidCardCount);
                    break;
                case Class.Hunter:
                    Interlocked.Increment(ref _HunterCardCount);
                    break;
                case Class.Mage:
                    Interlocked.Increment(ref _MageCardCount);
                    break;
                case Class.Paladin:
                    Interlocked.Increment(ref _PaladinCardCount);
                    break;
                case Class.Priest:
                    Interlocked.Increment(ref _PriestCardCount);
                    break;
                case Class.Rogue:
                    Interlocked.Increment(ref _RogueCardCount);
                    break;
                case Class.Shaman:
                    Interlocked.Increment(ref _ShamanCardCount);
                    break;
                case Class.Warrior:
                    Interlocked.Increment(ref _WarriorCardCount);
                    break;
                case Class.Warlock:
                    Interlocked.Increment(ref _WarlockCardCount);
                    break;
                default:
                    Interlocked.Increment(ref _NonClassCardCount);
                    break;

            }

        }

        private static void CountSet(string set)
        {

            switch (set)
            {

                case "Basic":
                    Interlocked.Increment(ref _BasicCardCount);
                    break;
                case "Classic":
                    Interlocked.Increment(ref _ClassicCardCount);
                    break;
                case "Whispers of the Old Gods":
                    Interlocked.Increment(ref _OldGodsCardCount);
                    break;
                case "One Night in Karazhan":
                    Interlocked.Increment(ref _KarzhanCardCount);
                    break;
                case "Mean Streets of Gadgetzan":
                    Interlocked.Increment(ref _GadgetzanCardCount);
                    break;
                case "Journey to Un'Goro":
                    Interlocked.Increment(ref _UnGoroCardCount);
                    break;
                case "Hall of Fame":
                    Interlocked.Increment(ref _HallOfFameCardCount);
                    break;
                case "Naxxramas":
                    Interlocked.Increment(ref _NaxxramasCardCount);
                    break;
                case "Goblins vs Gnomes":
                    Interlocked.Increment(ref _GvGCardCount);
                    break;
                case "Blackrock Mountain":
                    Interlocked.Increment(ref _BlackrockCardCount);
                    break;
                case "The Grand Tournament":
                    Interlocked.Increment(ref _TournamentCardCount);
                    break;
                case "League of Explorers":
                    Interlocked.Increment(ref _ExplorersCardCount);
                    break;
                case "Cheat":
                    Interlocked.Increment(ref _CheatCardCount);
                    break;
                default:
                    break;

            }

        }
    }
}
