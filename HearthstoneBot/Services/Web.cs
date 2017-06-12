using HearthstoneBot.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot.Services
{
    public static class Web
    {

        private static HttpClient _http = new HttpClient();
        private const string BlueDiscordBots = "https://discordbots.org";
        private const string BlackDiscordBots = "https://bots.discord.pw";
        private const string DiscordBotsEndpoint = "/api/bots/312429302283108353/stats";
        private const string Payload = "{{ \"server_count\": count }}";

        public static async Task<Stream> GetImage(string url)
        {

            var stream = await GetStream(url);
            stream.Position = 0;

            return stream;

        }

        public static async Task SendStats(int guildCount)
        {

            HttpResponseMessage response;

            //fuck you guys for not having different names, why you all have to be named "Discord Bots" or something along those lines...
            Log("Posting server count to Blue Discord Bots...");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Config.BlueDiscordBots);
            var payload = new StringContent(Payload.Replace("count", guildCount.ToString()), Encoding.UTF8, "application/json");

            try
            {

                response = await _http.PostAsync(BlackDiscordBots + DiscordBotsEndpoint, payload);
                Log($"The status was: {response.StatusCode.ToString()} ({(int)response.StatusCode})");

            }
            catch (HttpRequestException)
            {

                Log("Error in sending guild count. Sad face.");

            }

            Log("Posting server count to Black Discord Bots...");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Config.BlackDiscordBots);
            payload = new StringContent(Payload.Replace("count", guildCount.ToString()), Encoding.UTF8, "application/json");

            try
            {

                response = await _http.PostAsync(BlackDiscordBots + DiscordBotsEndpoint, payload);
                Log($"The status was: {response.StatusCode.ToString()} ({(int)response.StatusCode})");

            }
            catch (HttpRequestException)
            {

                Log("Error in sending guild count. Sad face.");

            }

            _http.DefaultRequestHeaders.Authorization = null;

        }

        private static async Task<string> GetString(string url)
        {

            HttpResponseMessage status;

            do
            {

                status = await _http.GetAsync(url);

            } while (status.StatusCode != HttpStatusCode.OK);

            return await status.Content.ReadAsStringAsync();

        }

        private static async Task<Stream> GetStream(string url)
        {

            HttpResponseMessage status;

            do
            {

                status = await _http.GetAsync(url);

            } while (status.StatusCode != HttpStatusCode.OK);

            return await status.Content.ReadAsStreamAsync();

        }

        private static void Log(string message)
            => AltConsole.Print("Service", "Web", message);

    }
}
