using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using HearthstoneBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using HearthstoneBot.Objects;
using Discord;

namespace HearthstoneBot.Modules
{
    public class CoreCommands : InteractiveModuleBase<SocketCommandContext>
    {

        private bool _isMinimal;

        protected override void BeforeExecute()
            => _isMinimal = Settings.GetMinimalSetting(Context.Guild.Id);

        [Command("search"), Alias("s")]
        [Summary("Search for cards")]
        public async Task SearchCommand([Remainder]string search)
        {

            var aSearch = search.ToLower();
            var cards = Cache.CardObjects.Where(card => card.Name.ToLower().Contains(aSearch));
            var length = cards.Count();
            IUserMessage selection;
            string content;

            if (length > 31)
            {

                await ReplyAsync("Your search returned too many results! Try narrowing it down.");
                return;

            }
            else if (length == 0)
            {

                await ReplyAsync($"Nothing was found with the search term of {search}!");
                return;

            }
            else if (length != 1)
                content = "1";
            else
            {

                var results = GenerateSearch(cards.Select(c => c.Name));

                await ReplyAsync(results);

                selection = await WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(60));
                content = selection.Content;

            }      

            if (int.TryParse(content, out var number))
            {

                var card = cards.ElementAtOrDefault(number - 1);

                if (card != null)
                {

                    var embed = Cache.Embeds[card.Name.ToLower()];
                    await ReplyAsync("", embed: Chat.CleanEmbed(embed, _isMinimal));

                }

            }

        }

        [Command("lsearch"), Alias("ls")]
        [Summary("Lazily search for cards")]
        public async Task LazySearchCommand([Remainder]string search)
        {

            var array = search.ToLower().Split(' ');
            var cards = Cache.CardObjects.Where(card => array.All(str => card.Name.ToLower().Contains(str)));
            var length = cards.Count();
            IUserMessage selection;
            string content;

            if (length > 31)
            {

                await ReplyAsync("Your search returned too many results! Try narrowing it down.");
                return;

            }
            else if (length == 0)
            {

                await ReplyAsync($"Nothing was found with the search term of {search}!");
                return;

            }
            else if (length != 1)
                content = "1";
            else
            {

                var results = GenerateSearch(cards.Select(c => c.Name));

                await ReplyAsync(results);

                selection = await WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(60));
                content = selection.Content;

            }

            if (int.TryParse(content, out var number))
            {

                var card = cards.ElementAtOrDefault(number - 1);

                if (card != null)
                {

                    var embed = Cache.Embeds[card.Name.ToLower()];
                    await ReplyAsync("", embed: Chat.CleanEmbed(embed, _isMinimal));

                }

            }

        }

        [Command("card"), Alias("c")]
        [Summary("Search for a card")]
        public async Task CardCommand([Remainder]string search)
        {

            var aSearch = search.ToLower();
            var embed = Cache.Embeds.FirstOrDefault(kv => kv.Key == aSearch).Value;

            if (embed != null)
                await ReplyAsync("", embed: Chat.CleanEmbed(embed, _isMinimal));
            else
                await ReplyAsync($"There was no card found with the search of `{search}`");

        }

        [Command("lcard"), Alias("lc")]
        [Summary("Lazily search for a card")]
        public async Task LazyCardCommand([Remainder]string search)
        {

            var array = search.ToLower().Split(' ');
            var embed = Cache.Embeds.FirstOrDefault(kv => kv.Key.Split(' ').All(str => array.Contains(str))).Value;

            if (embed != null)
                await ReplyAsync("", embed: Chat.CleanEmbed(embed, _isMinimal));
            else
                await ReplyAsync($"There was no card found with the search of `{search}`");

        }

        [Command("random"), Alias("r")]
        [Summary("Get a random card")]
        public async Task RandomCommand()
        {

            string cardName;

            do
            {

                cardName = Cache.CardObjects.RandomSubset(3).FirstOrDefault(c => c.Set.ToLower() != "cheat")?.Name.ToLower();

            } while (cardName == null);

            var embed = Cache.Embeds[cardName];

            await ReplyAsync("", embed: Chat.CleanEmbed(embed, _isMinimal));

        }

        private string GenerateSearch(IEnumerable<string> cardNames)
        {

            var builder = new StringBuilder("```The result of your search:");

            builder.AppendLine();
            builder.AppendLine();

            for (int i = 1; i <= cardNames.Count(); i++)
            {

                builder.AppendLine($"{i}. {cardNames.ElementAt(i - 1)}");

            }

            builder.AppendLine();
            builder.Append("Hit a number to see that result!```");

            return builder.ToString();

        }

    }
}
