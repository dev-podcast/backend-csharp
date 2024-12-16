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
      
        private readonly ILogger<ItunesHttpClient> _logger;
        private readonly IHttpClientFactory _clientFactory;
        public ItunesHttpClient(ILogger<ItunesHttpClient> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<JArray> QueryItunesId(string itunesId)
        {

            var client = _clientFactory.CreateClient();
            var maxTries = 3;
            var remainingTries = maxTries;
            do
            {
              //  Thread.Sleep(5000);
                --remainingTries;
                try
                {
                    itunesId = string.Join(string.Empty, itunesId.Skip(2));
                    var lookupUrl = $"https://itunes.apple.com/lookup?id={itunesId}";

                    Console.WriteLine(lookupUrl);
                  
                    var response = await client.GetAsync(lookupUrl);

                    if (!response.IsSuccessStatusCode) return new JArray();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(data);
                        if (json != null)
                        {
                            var resultCount = json["resultCount"];

                            if (resultCount != null)
                            {
                                if (resultCount.ToString() != "0")
                                {
                                    var results = json["results"];
                                    if (results != null)
                                    {
                                        return JArray.Parse(results.ToString());
                                    }
                                } else
                                {
                                    return new JArray();
                                }
                            }
                        }
                    }
                    if (remainingTries == 0)
                    {
                        return new JArray();
                    }
                    
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
            var client = _clientFactory.CreateClient();
            var maxTries = 3;
            var remainingTries = maxTries;
            do
            {
                --remainingTries;
                try
                {
                    Thread.Sleep(2000);

                    var response = await client.GetAsync(url).ConfigureAwait(false);

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
