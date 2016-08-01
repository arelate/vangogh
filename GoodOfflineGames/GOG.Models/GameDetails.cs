using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(GameDetails))]
    [KnownType(typeof(OperatingSystemsDownloads))]
    [KnownType(typeof(DownloadEntry))]
    [KnownType(typeof(Tag))]
    [KnownType(typeof(ProductCore))]
    public class GameDetails: ProductCore, IGameDetails
    {
        [DataMember(Name = "cdKey")]
        public string CDKey { get; set; }
        [DataMember(Name = "dlcs")]
        public IGameDetails[] DLCs { get; set; }
        [DataMember(Name = "languageDownloads")]
        public IOperatingSystemsDownloads[] LanguageDownloads { get; set; }
        [DataMember(Name = "extras")]
        public IDownloadEntry[] Extras { get; set; }
        [DataMember(Name = "changelog")]
        public string Changelog { get; set; }
        [DataMember(Name = "releaseTimestamp")]
        public long ReleaseTimestamp { get; set; }
        [DataMember(Name = "tags")]
        public ITag[] Tags { get; set; }
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
        public IProductCore MissingBaseProduct { get; set; }

    }
}
