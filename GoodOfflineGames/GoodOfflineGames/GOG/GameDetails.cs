using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    public class DownloadEntry
    {
        [DataMember(Name = "date")]
        public string Date { get; set; }
        [DataMember(Name = "downloaderUrl")]
        public string DownloaderUrl { get; set; }
        [DataMember(Name = "manualUrl")]
        public string ManualUrl { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "size")]
        public string Size { get; set; }
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }

    [DataContract]
    public class OperatingSystemsDownloads
    {
        [DataMember(Name = "linux")]
        public List<DownloadEntry> Linux { get; set; }
        [DataMember(Name = "mac")]
        public List<DownloadEntry> Mac { get; set; }
        [DataMember(Name = "windows")]
        public List<DownloadEntry> Windows { get; set; }
    }

    [DataContract]
    public class LanguageDownloads
    {
        [DataMember(Name = "English")]
        public OperatingSystemsDownloads English { get; set; }
    }

    [DataContract]
    public class GameDetails
    {
        [DataMember(Name = "backgroundImage")]
        public string BackgroundImage { get; set; }
        [DataMember(Name = "cdKey")]
        public string CDKey { get; set; }
        [DataMember(Name = "changelog")]
        public string Changelog { get; set; }
        [DataMember(Name = "combinedExtrasDownloaderUrl")]
        public string CombinedExtrasDownloaderUrl { get; set; }
        [DataMember(Name = "dlcs")]
        public List<GameDetails> DLCs { get; set; }
        [DataMember(Name = "downloads")]
        public LanguageDownloads Downloads { get; set; }
        [DataMember(Name = "extras")]
        public string Extras { get; set; }
        [DataMember(Name = "forumLink")]
        public string ForumLink { get; set; }
        [DataMember(Name = "isPreOrder")]
        public bool IsPreOrder { get; set; }
        [DataMember(Name = "messages")]
        public string Messages { get; set; }
        [DataMember(Name = "releaseTimestamp")]
        public long ReleaseTimestamp { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
        [DataMember(Name = "textInformation")]
        public string TextInformation { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; } 
    }
}
