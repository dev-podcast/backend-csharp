
using System.Xml.Serialization;

namespace devpodcasts.common.XmlModels;

public class XEnclosure
{
    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("url")]
    public string Url { get; set; }
}
