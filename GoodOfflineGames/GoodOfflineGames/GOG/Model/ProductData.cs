using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class ProductData
    {
        // fields we won't be serializing

        // public int VotesCount;
        // public string DownloadSize;
        // public List<ProductData> Packs;

        [DataMember(Name = "genres")]
        public List<NamedEntry> Genres { get; set; }
        [DataMember(Name = "publisher")]
        public NamedEntry Publisher { get; set; }
        [DataMember(Name = "developer")]
        public NamedEntry Developer { get; set; }
        [DataMember(Name = "modes")]
        public List<NamedEntry> Modes { get; set; }
        [DataMember(Name = "backgroundImageSource")]
        public string BackgroundImageSource { get; set; }
        [DataMember(Name = "cardSeoKeywords ")]
        public string CardSEOKeywords { get; set; }
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "releaseDate")]
        public long? ReleaseDate { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "series")]
        public Series Series { get; set; }
        [DataMember(Name = "dlcs")]
        public List<ProductData> DLCs { get; set; }
        [DataMember(Name = "requiredProducts")]
        public List<ProductData> RequiredProducts { get; set; }
    }
}
