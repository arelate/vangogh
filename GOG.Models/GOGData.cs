using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class GOGData
    {
        // product_ids... - happy to support when GOG returns consistent result
        // perhaps I'd consider pre-cleaning to something consistent?

        [DataMember(Name = "mixes")]
        public Mix[] Mixes { get; set; }
        [DataMember(Name = "mixesPerPage")]
        public int MixesPerPage { get; set; }
        //[DataMember(Name = "proReviews")]
        //public string[] ProReviews { get; set; }
        //[DataMember(Name = "reviews")]
        //public ReviewsPages Reviews { get; set; }
        [DataMember(Name = "series_ids")]
        public int[] SeriesIds { get; set; }
        [DataMember(Name = "gameProductData")]
        public GameProductData GameProductData { get; set; }
        [DataMember(Name = "gameProductSeriesData")]
        public Product[] GameProductSeriesData { get; set; }
        [DataMember(Name = "recommendedProducts")]
        public Product[] RecommendedProducts { get; set; }
        [DataMember(Name = "requiredProductsData")]
        public Product[] RequiredProductsData { get; set; }
        [DataMember(Name = "children")]
        public Product[] Children { get; set; }
        [DataMember(Name = "parents")]
        public Product[] Parents { get; set; }
        [DataMember(Name = "dlcs")]
        public Product[] DLCs { get; set; }
        [DataMember(Name = "packs")]
        public Product[] Packs { get; set; }
        [DataMember(Name = "languages")]
        public Languages Languages { get; set; }
        [DataMember(Name = "brandRatings")]
        public BrandRatings BrandRatings { get; set; }
        [DataMember(Name = "rating")]
        public int Rating { get; set; }
        [DataMember(Name = "screenshotCount")]
        public int screenshotCount { get; set; }
        [DataMember(Name = "videoCount")]
        public int VideoCount { get; set; }
        [DataMember(Name = "anonymous_personalization")]
        public bool AnonymousPersonalization { get; set; }
        [DataMember(Name = "currentCurrency")]
        public Currency CurrentCurrency { get; set; }
        [DataMember(Name = "availableCurrencies")]
        public Currency[] AvailableCurrencies { get; set; }
        [DataMember(Name = "currentLanguage")]
        public string CurrentLanguage { get; set; }
        [DataMember(Name = "availableLanguages")]
        public Language[] AvailableLanguages { get; set; }
        [DataMember(Name = "dateFormats")]
        public DateFormat DateFormats { get; set; }
        [DataMember(Name = "now")]
        public long Now { get; set; }
        [DataMember(Name = "currentCountry")]
        public string CurrentCountry { get; set; }
        [DataMember(Name = "personalizationEndpointCacheTtl")]
        public int PersonalizationEndpointCacheTtl { get; set; }
    }
}