using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using HearthstoneBot.Core;
using HearthstoneBot.Services;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HearthstoneBot.Modules
{
    public class Games : InteractiveModuleBase<SocketCommandContext>
    {

        protected override void BeforeExecute()
        {

            _channel = Context.Channel;

        }

        private ISocketMessageChannel _channel;
        private SocketUser _winner;
        private Timer _timer;
        private string _match;

        [Command("guess")]
        [Summary("Guess the card with only the full art given")]
        public async Task GuessCommand()
        {

            if (!(Context.Channel is SocketDMChannel) && !Settings.IsGuessChannel(Context.Channel as SocketGuildChannel))
                return;

            if (Cache.GuessGames.ContainsKey(_channel.Id))
            {

                await ReplyAsync("There is already a game in progress! Wait for it to end.");
                return;

            }
            else
                Cache.GuessGames[_channel.Id] = null;

            var kv = Cache.FullArts.RandomSubset(1).First();
            //var kv = Cache.FullArts.FirstOrDefault(keyvalue => keyvalue.Key == "Rallying Blade");
            _match = kv.Key;

            using (Context.Channel.EnterTypingState())
            {

                var stream = await Web.GetImage(kv.Value);

                try
                {

                    await Context.Channel.SendFileAsync(stream, "jpg", ":card_index: A guessing game has begun! What card is this? You have 60 seconds!");

                }
                catch
                {

                    await ReplyAsync("It seems you have disabled attach files for the bot! Please enable for picture uploading to work!");

                }

            }

            Context.Client.MessageReceived += CompareMessage;

            _timer = new Timer(OnTimeUp, null, 60000, Timeout.Infinite);

        }

        private async Task CompareMessage(SocketMessage message)
        {

            if (message.Channel.Id != _channel.Id)
                return;
            
            if (message.Content.ToLower() == _match.ToLower())
            {

                _winner = message.Author;
                Context.Client.MessageReceived -= CompareMessage;

                if (_channel is SocketTextChannel)
                {
                    
                    var user = _winner as SocketGuildUser;
                    await ReplyAsync($":trophy: The card was `{_match}`! **{user.Nickname ?? user.Username}** got the card!");

                }
                else
                    await ReplyAsync($":trophy: The card was `{_match}`! **{_winner.Username}** got the card!");

                Cache.GuessGames.TryRemove(_channel.Id, out var blah);
                _timer.Dispose();

            }

        }

        private void OnTimeUp(object state)
        {
            
            Context.Client.MessageReceived -= CompareMessage;
            ReplyAsync($":poop: There was no winner! The card was `{_match}`!");
            Cache.GuessGames.TryRemove(_channel.Id, out var blah);
            _timer.Dispose();

        }
    }
}
