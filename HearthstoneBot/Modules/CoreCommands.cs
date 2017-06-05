using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using HearthstoneBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace HearthstoneBot.Modules
{
    public class CoreCommands : InteractiveModuleBase<SocketCommandContext>
    {

        [Command("search"), Alias("s")]
        [Summary("Search for cards")]
        public async Task SearchCommand([Remainder]string search)
        {

            var aSearch = search.ToLower();
            var cards = Cache.Cards.Where(kv => kv.Key.Contains(aSearch)).Select(kv => kv.Value);
            var results = GenerateSearch(cards.Select(c => c.Name));

            await ReplyAsync(results);

            var selection = await WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(60));

            if (int.TryParse(selection.Content, out var number))
            {

                var card = cards.ElementAtOrDefault(number);

                if (card != null)
                {

                    var isMinimal = Settings.GetMinimalSetting(Context.Guild.Id);

                    if (Cache.Embeds.TryGetValue(card, out var embed))
                        await ReplyAsync("", embed: Chat.CleanEmbed(embed, isMinimal));
                    else
                        await Chat.PrintCardToChat(card, Context.Channel, isMinimal);

                }

            }

        }

        [Command("card"), Alias("c")]
        [Summary("Search for a card")]
        public async Task CardCommand([Remainder]string search)
        {

            var aSearch = search.ToLower();
            var card = Cache.Cards.FirstOrDefault(kv => kv.Key == aSearch).Value;
            var isMinimal = Settings.GetMinimalSetting(Context.Guild.Id);

            if (card != null)
            {

                if (Cache.Embeds.TryGetValue(card, out var embed))
                    await ReplyAsync("", embed: Chat.CleanEmbed(embed, isMinimal));
                else
                    await Chat.PrintCardToChat(card, Context.Channel, isMinimal);

            }
            else
                await ReplyAsync($"There was no card found with the search of `{search}`");

        }

        [Command("random"), Alias("r")]
        [Summary("Get a random card")]
        public async Task RandomCommand()
        {

            var card = Cache.Cards.RandomSubset(3).First(kv => kv.Value.Set.ToLower() != "cheat").Value;
            var isMinimal = Settings.GetMinimalSetting(Context.Guild.Id);

            await Chat.PrintCardToChat(card, Context.Channel, isMinimal);

        }

        private string GenerateSearch(IEnumerable<string> cardNames)
        {

            var builder = new StringBuilder("```The result of your search:\n");

            for (int i = 1; i <= cardNames.Count(); i++)
            {

                builder.AppendLine($"{i}. {cardNames.ElementAt(i)}");

            }

            builder.AppendLine();
            builder.Append("Hit a number to see that result!```");

            return builder.ToString();

        }

    }
}
