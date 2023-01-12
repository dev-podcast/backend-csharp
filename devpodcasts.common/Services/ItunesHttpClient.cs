using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace devpodcasts.common.Services
{
    public interface IItunesHttpClient
    {
        Task<JArray> QueryItunesId(string itunesId);
        Task<IReadOnlyCollection<XElement>> QueryFeedUrl(string url);
    }

    public class ItunesHttpClient : IItunesHttpClient
    {
        private const string BASE_LOOKUP_URL = "https://itunes.apple.com/lookup/";

        private readonly ILogger<ItunesHttpClient> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient _client;
        public ItunesHttpClient(ILogger<ItunesHttpClient> logger, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _logger = logger;

            _client = _clientFactory.CreateClient();
        }

        public async Task<JArray> QueryItunesId(string itunesId)
        {
            var maxTries = 3;
            var remainingTries = maxTries;
            do
            {
                --remainingTries;
                try
                {
                    Thread.Sleep(2000);
                    var url = BASE_LOOKUP_URL + itunesId;

                    var response = await _client.GetAsync(url).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode) return new JArray();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.Content.ReadAsStream())
                        {
                            using (var raw = new StreamReader(stream, Encoding.UTF8))
                            {
                                var json = JObject.Parse(raw.ReadToEnd());
                                if (json != null)
                                {
                                    var resultCount = json["resultCount"].ToString();
                                    if (resultCount != "0") return JArray.Parse(json["results"].ToString());
                                }
                            }
                        }
                    }
                    return new JArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (remainingTries > 0);

            return new JArray();
        }

        public async Task<IReadOnlyCollection<XElement>> QueryFeedUrl(string url)
        {
            var maxTries = 3;
            var remainingTries = maxTries;
            do
            {
                --remainingTries;
                try
                {
                    Thread.Sleep(2000);

                    var response = await _client.GetAsync(url).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode) return new List<XElement>();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.Content.ReadAsStream())
                        {
                            if (stream != null)
                                using (var raw = new StreamReader(stream, Encoding.UTF8))
                                {
                                    var xmlData = XDocument.Parse(raw.ReadToEnd());

                                    var rss = xmlData.Descendants("rss");
                                    var channel = rss.Descendants("channel");
                                    if (channel != null) return channel.Descendants("item").ToImmutableList();
                                }
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (remainingTries > 0);
            return new List<XElement>();
        }


    }


}
