using System.Runtime.Serialization;

using Models.ProductCore;

namespace GOG.Models
{
    [DataContract]
    public class GameDetails: ProductCore
    {
        [DataMember(Name = "cdKey")]
        public string CDKey { get; set; }
        [DataMember(Name = "dlcs")]
        public GameDetails[] DLCs { get; set; }
        [DataMember(Name = "languageDownloads")]
        public OperatingSystemsDownloads[] LanguageDownloads { get; set; }
        [DataMember(Name = "extras")]
        public DownloadEntry[] Extras { get; set; }
        [DataMember(Name = "changelog")]
        public string Changelog { get; set; }
        [DataMember(Name = "releaseTimestamp")]
        public long ReleaseTimestamp { get; set; }
        [DataMember(Name = "tags")]
        public Tag[] Tags { get; set; }
        [DataMember(Name = "backgroundImage")]
        public string BackgroundImage { get; set; }
        [DataMember(Name = "textInformation")]
        public string TextInformation { get; set; }
        [DataMember(Name = "isPreOrder")]
        public bool IsPreOrder { get; set; }
        [DataMember(Name = "messages")]
        public string[] Messages { get; set; }
        [DataMember(Name = "forumLink")]
        public string ForumLink { get; set; }
        [DataMember(Name = "isBaseProductMissing")]
        public bool IsBaseProductMissing { get; set; }
        [DataMember(Name = "missingBaseProduct")]
        public ProductCore MissingBaseProduct { get; set; }

    }
}
