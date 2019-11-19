using System.Xml.Serialization;

namespace DevPodcast.Services.Core
{
    public class XEpisode
    {
        [XmlAttribute("title")] public string Title { get; set; }

        [XmlAttribute("enclosure")] public object Enclosure { get; set; }
        
        [XmlAttribute("link")] public string Link { get; set; }
        
        
        
    }

    public class XEnclosure
    {
        [XmlAttribute("type")]
        public string Type { get; set;}
        
        [XmlAttribute("url")]
        public string Url { get; set; }
    }
   
    
       
}