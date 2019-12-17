using System.Runtime.Serialization;

using Models.ProductTypes;

namespace GOG.Models
{
    [DataContract]
    public class GameProductData: ProductCore
    {
        [DataMember(Name = "votesCount")]
        public int VotesCount { get; set; }
        [DataMember(Name = "systemRequirements")]
        public SystemRequirements SystemRequirements { get; set; }
        [DataMember(Name = "osRequirements")]
        public OSRequirements OSRequirements { get; set; }
        [DataMember(Name = "languages")]
        public string Languages { get; set; }
        [DataMember(Name = "copyrights")]
        public string Copyrights { get; set; }
        [DataMember(Name = "genres")]
        public NamedEntry[] Genres { get; set; }
        [DataMember(Name = "publisher")]
        public NamedEntry Publisher { get; set; }
        [DataMember(Name = "developer")]
        public NamedEntry Developer { get; set; }
        [DataMember(Name = "downloadSize")]
        public string downloadSize { get; set; }
        [DataMember(Name = "recommendations")]
        public Recommendations Recommendations { get; set; }
        [DataMember(Name = "features")]
        public TitledEntry[] Features { get; set; }
        [DataMember(Name = "series")]
        public Series Series { get; set; }
        [DataMember(Name = "brandRatings")]
        public BrandRatings BrandRatings { get; set; }
        [DataMember(Name = "dlcs")]
        public Product[] DLCs { get; set; }
        [DataMember(Name = "packs")]
        public Product[] Packs { get; set; }
        [DataMember(Name = "requiredProducts")]
        public Product[] RequiredProducts { get; set; }
        [DataMember(Name = "whatsCoolAboutIt")]
        public string WhatsCoolAboutIt { get; set; }
        [DataMember(Name = "backgroundImage")]
        public string BackgroundImage { get; set; }
        [DataMember(Name = "backgroundImageSource")]
        public string BackgroundImageSource { get; set; }
        [DataMember(Name = "description")]
        public Description Description { get; set; }
        [DataMember(Name = "bonusContent")]
        public BonusContent BonusContent { get; set; }
        [DataMember(Name = "children")]
        public Product Children { get; set; }
        [DataMember(Name = "parents")]
        public Product Parents { get; set; }
        [DataMember(Name = "imageLogoFacebook")]
        public string ImageLogoFacebook { get; set; }
        [DataMember(Name = "cardSeoDescription")]
        public string CardSeoDescription { get; set; }
        [DataMember(Name = "cardSeoKeywords")]
        public string CardSeoKeywords { get; set; }
        [DataMember(Name = "extraRequirements")]
        public string ExtraRequirements { get; set; }
        [DataMember(Name = "canBeReviewed")]
        public bool CanBeReviewed { get; set; }
        // notification
        // screenshots
        [DataMember(Name = "price")]
        public Price Price { get; set; }
        [DataMember(Name = "isDiscounted")]
        public bool IsDiscounted { get; set; }
        [DataMember(Name = "isInDevelopment")]
        public bool IsInDevelopment { get; set; }
        [DataMember(Name = "releaseDate")]
        public long? ReleaseDate { get; set; }
        [DataMember(Name = "availability")]
        public Availability Availability { get; set; }
        [DataMember(Name = "salesVisibility")]
        public SalesVisibility SalesVisibility { get; set; }
        [DataMember(Name = "buyable")]
        public bool Buyable { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "supportUrl")]
        public string SupportUrl { get; set; }
        [DataMember(Name = "forumUrl")]
        public string ForumUrl { get; set; }
        [DataMember(Name = "worksOn")]
        public WorksOn WorksOn { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "originalCategory")]
        public string OriginalCategory { get; set; }
        [DataMember(Name = "rating")]
        public int Rating { get; set; }
        [DataMember(Name = "type")]
        public int Type { get; set; }
        [DataMember(Name = "isComingSoon")]
        public bool IsComingSoon { get; set; }
        [DataMember(Name = "isPriceVisible")]
        public bool IsPriceVisible { get; set; }
        [DataMember(Name = "isMovie")]
        public bool IsMovie { get; set; }
        [DataMember(Name = "isGame")]
        public bool IsGame { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
        //[DataMember(Name = "reviews")]
        //public ReviewsPages Reviews { get; set; }
        // media
    }
}
