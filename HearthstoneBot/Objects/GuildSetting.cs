using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Objects
{

    [Table("GuildSettings")]
    public class GuildSetting
    {

        [ExplicitKey]
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public bool IsMinimal { get; set; }        
        public string GuessChannelsString { get; set; }

        [Computed, Write(false)]
        public List<ulong> GuessChannels
        {
            get
            {
                return GuessChannelsString.Split('/').Where(str => str != " ").Select(str => ulong.Parse(str)).ToList();
            }
        }

        public void AddGuessChannel(ulong id)
        {

            if (GuessChannelsString == "0")
                GuessChannelsString = GuessChannelsString.Replace("0", id.ToString());
            else
                GuessChannelsString += $"/{id}";

        }

        public void RemoveGuessChannel(ulong id)
        {

            GuessChannelsString = GuessChannelsString.Replace($"{id}", string.Empty).Replace("//", "/");

            if (string.IsNullOrEmpty(GuessChannelsString))
                GuessChannelsString = "0";

        }

    }

}
