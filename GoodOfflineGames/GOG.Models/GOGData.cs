using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(Mix))]
    [KnownType(typeof(ReviewsPages))]
    [KnownType(typeof(GameProductData))]
    [KnownType(typeof(Product))]
    [KnownType(typeof(Languages))]
    [KnownType(typeof(BrandRatings))]
    [KnownType(typeof(Currency))]
    [KnownType(typeof(Language))]
    [KnownType(typeof(DateFormat))]
    public class GOGData: IGOGData
    {
        // product_ids... - happy to support when GOG returns consistent result
        // perhaps I'd consider pre-cleaning to something consistent?

        [DataMember(Name = "mixes")]
        public IMix[] Mixes { get; set; }
        [DataMember(Name = "mixesPerPage")]
        public int MixesPerPage { get; set; }
        [DataMember(Name = "proReviews")]
        public string[] ProReviews { get; set; }
        [DataMember(Name = "reviews")]
        public IReviewsPages Reviews { get; set; }
        [DataMember(Name = "series_ids")]
        public int[] SeriesIds { get; set; }
        [DataMember(Name = "gameProductData")]
        public IGameProductData GameProductData { get; set; }
        [DataMember(Name = "gameProductSeriesData")]
        public IProduct[] GameProductSeriesData { get; set; }
        [DataMember(Name = "recommendedProducts")]
        public IProduct[] RecommendedProducts { get; set; }
        [DataMember(Name = "requiredProductsData")]
        public IProduct[] RequiredProductsData { get; set; }
        [DataMember(Name = "children")]
        public IProduct[] Children { get; set; }
        [DataMember(Name = "parents")]
        public IProduct[] Parents { get; set; }
        [DataMember(Name = "dlcs")]
        public IProduct[] DLCs { get; set; }
        [DataMember(Name = "packs")]
        public string[] Packs { get; set; }
        [DataMember(Name = "languages")]
        public ILanguages Languages { get; set; }
        [DataMember(Name = "brandRatings")]
        public IBrandRatings BrandRatings { get; set; }
        [DataMember(Name = "rating")]
        public int Rating { get; set; }
        [DataMember(Name = "screenshotCount")]
        public int screenshotCount { get; set; }
        [DataMember(Name = "videoCount")]
        public int VideoCount { get; set; }
        [DataMember(Name = "anonymous_personalization")]
        public bool AnonymousPersonalization { get; set; }
        [DataMember(Name = "currentCurrency")]
        public ICurrency CurrentCurrency { get; set; }
        [DataMember(Name = "availableCurrencies")]
        public ICurrency[] AvailableCurrencies { get; set; }
        [DataMember(Name = "currentLanguage")]
        public string CurrentLanguage { get; set; }
        [DataMember(Name = "availableLanguages")]
        public ILanguage[] AvailableLanguages { get; set; }
        [DataMember(Name = "dateFormats")]
        public IDateFormat DateFormats { get; set; }
        [DataMember(Name = "now")]
        public long Now { get; set; }
        [DataMember(Name = "currentCountry")]
        public string CurrentCountry { get; set; }
        [DataMember(Name = "personalizationEndpointCacheTtl")]
        public int PersonalizationEndpointCacheTtl { get; set; }
    }
}