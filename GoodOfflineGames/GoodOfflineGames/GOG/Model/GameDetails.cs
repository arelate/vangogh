using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class GameDetails
    {
        // fields we won't be serializing

        // public string backgroundImage;
        // public string changelog;
        // public string combinedExtrasDownloaderUrl { get; set; }
        // public string forumLink;
        // public bool isPreOrder;
        // public string messages;
        // public long releaseTimestamp;
        // public string textInformation;
        // public List<Tag> tags;

        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "cdKey")]
        public string CDKey { get; set; }
        [DataMember(Name = "dlcs")]
        public List<GameDetails> DLCs { get; set; }
        [DataMember(Name = "downloads")]
        public LanguageDownloads Downloads { get; set; }
        [DataMember(Name = "extras")]
        public List<DownloadEntry> Extras { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; } 
    }
}
