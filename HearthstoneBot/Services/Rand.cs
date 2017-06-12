using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Services
{
    public static class Rand
    {

        private static Random _rand = new Random();

        public static int NextInt(int inclusiveMin, int exclusiveMax)
        {

            return _rand.Next(inclusiveMin, exclusiveMax);

        }

        public static Color GetRandomColor()
        {

            return new Color(NextInt(0, 256), NextInt(0, 256), NextInt(0, 256));

        }

    }
}
