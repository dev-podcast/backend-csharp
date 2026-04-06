using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace devpodcasts.common.JsonObjects;

internal abstract class BasePodcastJsonObject
{
    public string Title { get; set; }
    public string PodcastSite { get; set; }
    public string Description { get; set; }
    public string ItunesSubscriberUrl { get; set; }
    public string EpisodesJohnWasOn { get; set; }
    public string ItunesId { get; set; }
}

internal class RootJsonObject
{
    [JsonProperty("General Interest")]
    public IEnumerable<GeneralInterest> GeneralInterest { get; set; }
    [JsonProperty(".NET and Microsoft Related")]
    public IEnumerable<NETAndMicrosoftRelated> NETAndMicrosoftRelated { get; set; }
    [JsonProperty("Mobile Development")]
    public IEnumerable<MobileDevelopment> MobileDevelopment { get; set; }
    [JsonProperty("Javascript and Web")]
    public IEnumerable<JavascriptAndWeb> JavascriptAndWeb { get; set; }
    [JsonProperty("Game Development")]
    public IEnumerable<GameDevelopment> GameDevelopment { get; set; }
    [JsonProperty("Freelancing")]
    public IEnumerable<Freelancing> FreeLancing { get; set; }
    [JsonProperty("Entrepreneurial")]
    public IEnumerable<Entrepreneurial> Entrepreneurial { get; set; }
    [JsonProperty("Python")]
    public IEnumerable<Python> Python { get; set; }
    [JsonProperty("PHP")]
    public IEnumerable<PHP> PHP { get; set; }
    [JsonProperty("C++")]
    public IEnumerable<CPlusPlus> CPlusPlus { get; set; }
    [JsonProperty("Java and JVM Languages")]
    public IEnumerable<JavaAndJVMLanguages> JavaAndJvmLanguages { get; set; }
    [JsonProperty("Ruby")]
    public IEnumerable<Ruby> Ruby { get; set; }
    [JsonProperty("Miscellaneous Languages")]
    public IEnumerable<MiscellaneousLanguages> MiscellaneousLanguages { get; set; }
    [JsonProperty("Data and Machine Learning")]
    public IEnumerable<DataAndMachineLearning> DataAndMachineLearning { get; set; }
    [JsonProperty("Agile and Scrum")]
    public IEnumerable<AgileAndScrum> AgileAndScrum { get; set; }
    [JsonProperty("DevOps")]
    public IEnumerable<DevOps> DevOps { get; set; }
    [JsonProperty("Cloud")]
    public IEnumerable<Cloud> Cloud { get; set; }
    [JsonProperty("SQL and Databases")]
    public IEnumerable<SqlAndDatabases> SqlAndDatabases { get; set; }
}

internal class GeneralInterest : BasePodcastJsonObject {}
internal class NETAndMicrosoftRelated : BasePodcastJsonObject{}
internal class MobileDevelopment : BasePodcastJsonObject {}
internal class JavascriptAndWeb: BasePodcastJsonObject { }
internal class GameDevelopment: BasePodcastJsonObject { }
internal class Freelancing: BasePodcastJsonObject { }
internal class Entrepreneurial : BasePodcastJsonObject { }
internal class Python : BasePodcastJsonObject { }
internal class PHP: BasePodcastJsonObject { }
[JsonObject("C++")]
internal class CPlusPlus : BasePodcastJsonObject {}
internal class JavaAndJVMLanguages : BasePodcastJsonObject { }
internal class Ruby: BasePodcastJsonObject { }
internal class MiscellaneousLanguages : BasePodcastJsonObject { }
internal class DataAndMachineLearning : BasePodcastJsonObject { }
internal class AgileAndScrum : BasePodcastJsonObject { }
internal class DevOps  : BasePodcastJsonObject { }
internal class Cloud : BasePodcastJsonObject { }
internal class SqlAndDatabases : BasePodcastJsonObject { }