using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class ProductData: ProductCore
    {
        // fields we won't be serializing

        // public int votesCount;
        // public string downloadSize;
        // public List<ProductData> packs;
        // public List<NamedEntry> modes;
        // public long? releaseDate;
        // public string backgroundImageSource;
        // public string cardSEOKeywords;

        [DataMember(Name = "genres")]
        public List<NamedEntry> Genres { get; set; }
        [DataMember(Name = "publisher")]
        public NamedEntry Publisher { get; set; }
        [DataMember(Name = "developer")]
        public NamedEntry Developer { get; set; }
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
