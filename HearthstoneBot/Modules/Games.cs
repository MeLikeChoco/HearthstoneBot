using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
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

        protected override void AfterExecute()
        {

            _timer.Dispose();

        }

        private ISocketMessageChannel _channel;
        private SocketUser _winner;
        private bool _didGameEnd = false;
        private Timer _timer;
        private string _match;

        [Command("guess")]
        [Summary("Guess the card with only the full art given")]
        public async Task GuessCommand()
        {

            if (Cache.GuessGames.TryGetValue(_channel.Id, out var gameInProgress))
            {

                await ReplyAsync("There is already a game in progress! Wait for it to end.");
                return;

            }
            else
            {

                gameInProgress = true;
                Cache.GuessGames[_channel.Id] = true;

            }

            var kv = Cache.FullArts.RandomSubset(1).First();
            _match = kv.Key.ToLower();

            using (Context.Channel.EnterTypingState())
            {

                var stream = await Web.GetImage(kv.Value);

                await Context.Channel.SendFileAsync(stream, "jpg", ":card_index: A guessing game has begun! What card is this? You have 60 seconds!");

            }

            Context.Client.MessageReceived += CompareMessage;

            _timer = new Timer(OnTimeUp, null, 60000, Timeout.Infinite);

            while (!_didGameEnd) { }

            Context.Client.MessageReceived -= CompareMessage;

            using (Context.Channel.EnterTypingState())
            {

                if (_winner == null)
                {

                    await ReplyAsync($":poop: There was no winner! The card was `{kv.Key}`!");
                    Cache.GuessGames.TryRemove(_channel.Id, out var blah);

                }
                else
                {

                    if (_channel is SocketTextChannel)
                    {

                        var user = _winner as SocketGuildUser;
                        await ReplyAsync($":trophy: The card was `{kv.Key}`! {user.Nickname ?? user.Username}");

                    }
                    else
                        await ReplyAsync($":trophy: The card was `{kv.Key}`! {_winner.Username}");


                }

            }

        }

        private Task CompareMessage(SocketMessage message)
        {

            if (message.Channel.Id != _channel.Id)
                return Task.CompletedTask;

            if (message.Content.ToLower() == _match)
            {

                _winner = message.Author;
                _didGameEnd = true;

            }

            return Task.CompletedTask;

        }

        private void OnTimeUp(object state)
        {

            _didGameEnd = true;
            Context.Client.MessageReceived -= CompareMessage;

        }
    }
}
