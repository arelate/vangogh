using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(SystemRequirements))]
    [KnownType(typeof(OSRequirements))]
    [KnownType(typeof(NamedEntry))]
    [KnownType(typeof(Recommendations))]
    [KnownType(typeof(TitledEntry))]
    [KnownType(typeof(Series))]
    [KnownType(typeof(BrandRatings))]
    [KnownType(typeof(Product))]
    [KnownType(typeof(Description))]
    [KnownType(typeof(BonusContent))]
    [KnownType(typeof(Product))]
    [KnownType(typeof(Price))]
    [KnownType(typeof(Availability))]
    [KnownType(typeof(SalesVisibility))]
    [KnownType(typeof(WorksOn))]
    [KnownType(typeof(ReviewsPages))]
    public class GameProductData: ProductCore, IGameProductData
    {
        [DataMember(Name = "votesCount")]
        public int VotesCount { get; set; }
        [DataMember(Name = "systemRequirements")]
        public ISystemRequirements SystemRequirements { get; set; }
        [DataMember(Name = "osRequirements")]
        public IOSRequirements OSRequirements { get; set; }
        [DataMember(Name = "languages")]
        public string Languages { get; set; }
        [DataMember(Name = "copyrights")]
        public string Copyrights { get; set; }
        [DataMember(Name = "genres")]
        public INamedEntry[] Genres { get; set; }
        [DataMember(Name = "publisher")]
        public INamedEntry Publisher { get; set; }
        [DataMember(Name = "developer")]
        public INamedEntry Developer { get; set; }
        [DataMember(Name = "downloadSize")]
        public string downloadSize { get; set; }
        [DataMember(Name = "recommendations")]
        public IRecommendations Recommendations { get; set; }
        [DataMember(Name = "features")]
        public ITitledEntry[] Features { get; set; }
        [DataMember(Name = "series")]
        public ISeries Series { get; set; }
        [DataMember(Name = "brandRatings")]
        public IBrandRatings BrandRatings { get; set; }
        [DataMember(Name = "dlcs")]
        public IProduct[] DLCs { get; set; }
        [DataMember(Name = "packs")]
        public IProduct[] Packs { get; set; }
        [DataMember(Name = "requiredProducts")]
        public IProduct[] RequiredProducts { get; set; }
        [DataMember(Name = "whatsCoolAboutIt")]
        public string WhatsCoolAboutIt { get; set; }
        [DataMember(Name = "backgroundImage")]
        public string BackgroundImage { get; set; }
        [DataMember(Name = "backgroundImageSource")]
        public string BackgroundImageSource { get; set; }
        [DataMember(Name = "description")]
        public IDescription Description { get; set; }
        [DataMember(Name = "bonusContent")]
        public IBonusContent BonusContent { get; set; }
        [DataMember(Name = "children")]
        public IProduct Children { get; set; }
        [DataMember(Name = "parents")]
        public IProduct Parents { get; set; }
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
        public IPrice Price { get; set; }
        [DataMember(Name = "isDiscounted")]
        public bool IsDiscounted { get; set; }
        [DataMember(Name = "isInDevelopment")]
        public bool IsInDevelopment { get; set; }
        [DataMember(Name = "releaseDate")]
        public long ReleaseDate { get; set; }
        [DataMember(Name = "availability")]
        public IAvailability Availability { get; set; }
        [DataMember(Name = "salesVisibility")]
        public ISalesVisibility SalesVisibility { get; set; }
        [DataMember(Name = "buyable")]
        public bool Buyable { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "url")]
        public string url { get; set; }
        [DataMember(Name = "supportUrl")]
        public string SupportUrl { get; set; }
        [DataMember(Name = "forumUrl")]
        public string ForumUrl { get; set; }
        [DataMember(Name = "worksOn")]
        public IWorksOn WorksOn { get; set; }
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
        [DataMember(Name = "reviews")]
        public IReviewsPages Reviews { get; set; }
        // media
    }
}
