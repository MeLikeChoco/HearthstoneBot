using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Core
{
    public static class AltConsole
    {

        /// <summary>Print to console colorfully.</summary>
        /// <param name="firstBracket">The service from where the message comes from.</param>
        /// <param name="secondBracket">The source of message.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The possible exception.</param>
        public static void Print(string firstBracket, string secondBracket, string message, Exception exception = null)
        {

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{DateTime.Now.ToString()} ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[{firstBracket}]");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"[{secondBracket}] ");
            Console.ForegroundColor = ConsoleColor.White;

            if (exception == null)
            {

                Console.WriteLine($"{message}");

            }
            else
            {

                Console.WriteLine($"{message}\t\t{exception}");

            }

        }

        /// <summary>Print to console colorfully.</summary>
        /// <param name="firstBracket">The severity.</param>
        /// <param name="secondBracket">The source of message.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The possible exception.</param>
        public static void Print(LogSeverity firstBracket, string secondBracket, string message, Exception exception = null)
        {

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{DateTime.Now.ToString()} ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[{firstBracket}]");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"[{secondBracket}] ");
            Console.ForegroundColor = ConsoleColor.White;

            if (exception == null)
            {

                Console.WriteLine($"{message}");

            }
            else
            {

                Console.WriteLine($"{message}\t\t{exception}");

            }

        }

        /// <summary>Print to console colorfully.</summary>
        /// <param name="line">The text to print.</param>
        public static void Print(string line)
        {

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{DateTime.Now.ToString()} ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[AltConsole]");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"[Print] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{line}");

        }

    }
}
