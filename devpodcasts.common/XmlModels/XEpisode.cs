using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace devpodcasts.common.XmlModels;

public class XEpisode
{
    [XmlAttribute("title")] public string Title { get; set; }

    [XmlAttribute("enclosure")] public object Enclosure { get; set; }

    [XmlAttribute("link")] public string Link { get; set; }
}
