using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Models
{
    //class User
    //{
    //    public string Name { get; set; }
    //    public string AvatarPath { get; set; }
    //    public long Id { get; set; }
    //}

    //class Mix
    //{
    //    public string Slug { get; set; }
    //    public int Votes { get; set; }
    //    public string Title { get; set; }
    //    public User User { get; set; }

    //}

    [DataContract]
    public class NamedEntry
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        //public string Slug { get; set; }
    }
    
    [DataContract]
    public class Series
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }
    }

    [DataContract]
    public class ProductData
    {
        //public int VotesCount { get; set; }
        [DataMember(Name = "genres")]
        public List<NamedEntry> Genres { get; set; }
        [DataMember(Name = "publisher")]
        public NamedEntry Publisher { get; set; }
        [DataMember(Name = "developer")]
        public NamedEntry Developer { get; set; }
        //public string DownloadSize { get; set; }
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
        //public List<ProductData> Packs { get; set; }
        [DataMember(Name = "requiredProducts")]
        public List<ProductData> RequiredProducts { get; set; }
    }

    [DataContract]
    public class GOGData
    {
        //[DataMember(Name = "series_ids")]
        //public List<int> SeriesIds { get; set; }
        [DataMember(Name = "gameProductData")]
        public ProductData ProductData { get; set; }
    }
}
