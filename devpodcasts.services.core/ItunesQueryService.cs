using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace DevPodcast.Services.Core
{
    public static class QueryService
    {
        private const string BASE_LOOKUP_URL = "https://itunes.apple.com/lookup/";

        public static async Task<JArray> QueryItunesId(string itunesId)
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
                    var request = (HttpWebRequest) WebRequest.Create(url);

                    var response = await request.GetResponseAsync().ConfigureAwait(false);

                    if (response == null) return new JArray();

                    if (response is HttpWebResponse wResponse && wResponse.StatusCode == HttpStatusCode.OK)
                        using (var stream = wResponse.GetResponseStream())
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

                    return new JArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (remainingTries > 0);

            return new JArray();
        }


        public static async Task<IReadOnlyCollection<XElement>> QueryFeedUrl(string url)
        {
            var maxTries = 3;
            var remainingTries = maxTries;
            do
            {
                --remainingTries;
                try
                {
                    Thread.Sleep(2000);
                    if (!(WebRequest.Create(url) is HttpWebRequest request)) return new List<XElement>();
                    if (!(await request.GetResponseAsync().ConfigureAwait(false) is HttpWebResponse webResponse))
                        return new List<XElement>();
                    if (webResponse.StatusCode != HttpStatusCode.OK) return new List<XElement>();
                    using (var stream = webResponse.GetResponseStream())
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (remainingTries > 0);
            return new List<XElement>();
        }
    }
}