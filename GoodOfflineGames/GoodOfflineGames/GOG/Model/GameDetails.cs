using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    [KnownType(typeof(OperatingSystemsDownloads))]
    public class GameDetails: ProductCore
    {
        // fields we won't be serializing

        // public string backgroundImage;
        // public string combinedExtrasDownloaderUrl { get; set; }
        // public string forumLink;
        // public bool isPreOrder;
        // public string messages;`
        // public string textInformation;

        [DataMember(Name = "cdKey")]
        public string CDKey { get; set; }
        [DataMember(Name = "dlcs")]
        public List<GameDetails> DLCs { get; set; }
        [DataMember(Name = "downloads"), IgnoreDataMember()]
        public dynamic[][] DynamicDownloads { get; set; }
        [DataMember(Name = "languageDownloads")]
        public List<OperatingSystemsDownloads> LanguageDownloads { get; set; }
        [DataMember(Name = "extras")]
        public List<DownloadEntry> Extras { get; set; }
        [DataMember(Name = "changelog")]
        public string Changelog { get; set; }
        [DataMember(Name = "releaseTimestamp")]
        public long ReleaseTimestamp { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags;

    }
}
