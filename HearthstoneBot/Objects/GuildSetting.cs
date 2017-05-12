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

        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public bool IsMinimal { get; set; }

    }

}
