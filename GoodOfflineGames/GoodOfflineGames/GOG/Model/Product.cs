using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class Product
    {
        // fields we won't be serializing

        // public bool isGalaxyCompatible;
        // public Availability availability;
        // public bool buyable;
        // public List<Tag> tags;
        // public int rating;
        // public bool isGame;
        // public bool isMovie;
        // public string slug;
        // public int updates;
        // public bool IsHidden;

        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "worksOn")]
        public WorksOn WorksOn { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "isComingSoon")]
        public bool IsComingSoon { get; set; }
        [DataMember(Name = "isNew")]
        public bool IsNew { get; set; }
        [DataMember(Name = "dlcCount")]
        public int DlcCount { get; set; }
        [DataMember(Name = "releaseDate")]
        public TimezoneDate ReleaseDate { get; set; }

        // GoodOfflineGames data
        [DataMember(Name = "owned")]
        public bool Owned { get; set; }
        [DataMember(Name = "wishlisted")]
        public bool Wishlisted { get; set; }
        [DataMember(Name = "gameDetails")]
        public GameDetails GameDetails { get; set; }
        [DataMember(Name = "productData")]
        public ProductData ProductData { get; set; }
    }
}
