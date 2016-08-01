namespace GOG.Interfaces.Models
{
    public interface IProductCore
    {
        long Id { get; set; }
        string Title { get; set; }
    }

    public interface IWorksOn
    {
        bool Linux { get; set; }
        bool Mac { get; set; }
        bool Windows { get; set; }
    }

    public interface IUser
    {
        string Name { get; set; }
        string AvatarPath { get; set; }
        long Id { get; set; }
    }

    public interface IMix
    {
        string Slug { get; set; }
        int Votes { get; set; }
        string Title { get; set; }
        IUser User { get; set; }
    }

    public interface ITag
    {
        long Id { get; set; }
        string Name { get; set; }
        long ProductCount { get; set; }
    }

    public interface IDownloadEntry
    {
        string Date { get; set; }
        string ManualUrl { get; set; }
        string DownloaderUrl { get; set; }
        string Name { get; set; }
        string Size { get; set; }
        string Type { get; set; }
        int Info { get; set; }
        string Version { get; set; }
    }

    public interface INamedEntry
    {
        string Name { get; set; }
        string Slug { get; set; }
    }

    public interface ITitledEntry
    {
        string Title { get; set; }
        string Slug { get; set; }
    }

    public interface IReview
    {
        long Id { get; set; }
        string Title { get; set; }
        string Teaser { get; set; }
        string Description { get; set; }
        IUser Author { get; set; }
        int HelpfulVotes { get; set; }
        int TotalVotes { get; set; }
        int Rating { get; set; }
        long Added { get; set; }
        long Edited { get; set; }
    }

    public interface IReviewsPages
    {
        IReview[][] Pages { get; set; }
        string TotalResults { get; set; }
        int TotalPages { get; set; }
    }

    public interface IDateFormat
    {
        string Tiny { get; set; }
    }

    public interface ILanguage
    {
        string Code { get; set; }
        string Name { get; set; }
    }

    public interface ICurrency
    {
        string Code { get; set; }
        string Symbol { get; set; }
    }

    public interface IRatingData
    {
        string Brand { get; set; }
        string BrandString { get; set; }
        string With { get; set; }
    }

    public interface IRatingAge
    {
        string AgeString { get; set; }
        int Age { get; set; }
    }

    public interface IRating
    {
        IRatingData Data { get; set; }
        IRatingAge Age { get; set; }
    }

    public interface IBrandRatings
    {
        IRating ESRB { get; set; }
        IRating PEGI { get; set; }
        IRating USK { get; set; }
    }

    public interface ILanguages
    {
        string Cn { get; set; }
        string Cz { get; set; }
        string De { get; set; }
        string En { get; set; }
        string Es { get; set; }
        string Fr { get; set; }
        string Hu { get; set; }
        string It { get; set; }
        string Jp { get; set; }
        string Ko { get; set; }
        string Pl { get; set; }
        string Pt { get; set; }
        string Ru { get; set; }
        string Tr { get; set; }
    }

    public interface IPrice
    {
        string Amount { get; set; }
        string BaseAmount { get; set; }
        string FinalAmount { get; set; }
        bool IsDiscounted { get; set; }
        double DiscountPercentage { get; set; }
        string DiscountDifference { get; set; }
        string Symbol { get; set; }
        bool IsFree { get; set; }
        double Discount { get; set; }
        bool IsBonusStoreCreditIncluded { get; set; }
        string BonusStoreCreditAmount { get; set; }
    }

    public interface IDiscountedPrice
    {
        string Amount { get; set; }
        string Symbol { get; set; }
        string Code { get; set; }
        bool IsZero { get; set; }
        double RawAmount { get; set; }
        string FormattedAmount { get; set; }
        string Full { get; set; }
        string ForEmail { get; set; }
    }

    public interface ISeries
    {
        long Id { get; set; }
        string Name { get; set; }
        IPrice Price { get; set; }
        IDiscountedPrice DiscountedPrice { get; set; }
        IProduct[] Products { get; set; }
        double DiscountPercentage { get; set; }
        string GiftUrl { get; set; }
    }

    public interface IDescription
    {
        string Full { get; set; }
        string Lead { get; set; }
    }

    public interface IBonusContentItem
    {
        string Name { get; set; }
        string IconClass { get; set; }
        int Count { get; set; }
        long Id { get; set; }
        string IconTitle { get; set; }
    }

    public interface IBonusContent
    {
        IBonusContent[] Visible { get; set; }
        IBonusContent[] Hidden { get; set; }
    }

    public interface IAvailability
    {
        bool IsAvailable { get; set; }
        bool IsAvailableInAccount { get; set; }
    }

    public interface ITimezoneDate
    {
        string Date { get; set; }
        string Timezone { get; set; }
        int TimezoneType { get; set; }
    }

    public interface ISalesVisibility
    {
        bool IsActive { get; set; }
        ITimezoneDate FromObject { get; set; }
        long From { get; set; }
        ITimezoneDate ToObject { get; set; }
        long To { get; set; }
    }

    public interface IOperatingSystemsDownloads
    {
        string Language { get; set; }
        IDownloadEntry[] Linux { get; set; }
        IDownloadEntry[] Mac { get; set; }
        IDownloadEntry[] Windows { get; set; }
    }

    public interface IProduct :
        IProductCore
    {
        string[] CustomAttributes { get; set; }
        IPrice Price { get; set; }
        bool IsDiscounted { get; set; }
        bool IsInDevelopment { get; set; }
        long ReleaseDate { get; set; }
        IAvailability Availability { get; set; }
        ISalesVisibility SalesVisibility { get; set; }
        bool Buyable { get; set; }
        string SupportUrl { get; set; }
        string ForumUrl { get; set; }
        string Category { get; set; }
        string OriginalCategory { get; set; }
        int Rating { get; set; }
        int Type { get; set; }
        bool IsComingSoon { get; set; }
        bool IsPriceVisible { get; set; }
        bool IsMovie { get; set; }
        bool IsGame { get; set; }
        string Image { get; set; }
        string Url { get; set; }
        IWorksOn WorksOn { get; set; }
        string Slug { get; set; }
    }

    public interface IAccountProduct
    {
        bool IsGalaxyCompatible { get; set; }
        ITag[] Tags { get; set; }
        IAvailability Availability { get; set; }
        string Image { get; set; }
        string Url { get; set; }
        IWorksOn WorksOn { get; set; }
        string Category { get; set; }
        int Rating { get; set; }
        bool IsComingSoon { get; set; }
        bool IsMovie { get; set; }
        bool IsGame { get; set; }
        string Slug { get; set; }
        int Updates { get; set; }
        bool IsNew { get; set; }
        int DLCCount { get; set; }
        ITimezoneDate ReleaseDate { get; set; }
        bool IsBaseProductMissing { get; set; }
        bool IsHidingDisabled { get; set; }
        bool IsInDevelopment { get; set; }
        bool IsHidden { get; set; }
    }

    public interface ISystemRequirement
    {
        string Minimum { get; set; }
        string Recommended { get; set; }
    }

    public interface ISystemRequirements
    {
        ISystemRequirement Windows { get; set; }
        ISystemRequirement Mac { get; set; }
        ISystemRequirement Linux { get; set; }
    }

    public interface IOSRequirements
    {
        string[] Windows { get; set; }
        string[] Mac { get; set; }
        string[] Linux { get; set; }
    }

    public interface IProductFile
    {
        long Id { get; set; }
        string OperatingSystem { get; set; }
        string Language { get; set; }
        string Url { get; set; }
        string Name { get; set; }
        string Size { get; set; }
        string Version { get; set; }
        string Folder { get; set; }
        string File { get; set; }
        bool Extra { get; set; }
        string ResolvedUrl { get; set; }
        bool Validated { get; set; }
    }

    public interface IRecommendations
    {
        IProduct[] FirstPage { get; set; }
        IProduct[] All { get; set; }
    }

    public interface IGameDetails :
        IProductCore
    {
        string CDKey { get; set; }
        IGameDetails[] DLCs { get; set; }
        IOperatingSystemsDownloads[] LanguageDownloads { get; set; }
        IDownloadEntry[] Extras { get; set; }
        string Changelog { get; set; }
        long ReleaseTimestamp { get; set; }
        ITag[] Tags { get; set; }
        string BackgroundImage { get; set; }
        string TextInformation { get; set; }
        bool IsPreOrder { get; set; }
        string[] Messages { get; set; }
        string ForumLink { get; set; }
        bool IsBaseProductMissing { get; set; }
        IProductCore MissingBaseProduct { get; set; }
    }

    public interface IGOGData
    {
        IMix[] Mixes { get; set; }
        int MixesPerPage { get; set; }
        string[] ProReviews { get; set; }
        IReviewsPages Reviews { get; set; }
        int[] SeriesIds { get; set; }
        IGameProductData GameProductData { get; set; }
        IProduct[] GameProductSeriesData { get; set; }
        IProduct[] RecommendedProducts { get; set; }
        IProduct[] RequiredProductsData { get; set; }
        IProduct[] Children { get; set; }
        IProduct[] Parents { get; set; }
        IProduct[] DLCs { get; set; }
        string[] Packs { get; set; }
        ILanguages Languages { get; set; }
        IBrandRatings BrandRatings { get; set; }
        int Rating { get; set; }
        int screenshotCount { get; set; }
        int VideoCount { get; set; }
        bool AnonymousPersonalization { get; set; }
        ICurrency CurrentCurrency { get; set; }
        ICurrency[] AvailableCurrencies { get; set; }
        string CurrentLanguage { get; set; }
        ILanguage[] AvailableLanguages { get; set; }
        IDateFormat DateFormats { get; set; }
        long Now { get; set; }
        string CurrentCountry { get; set; }
        int PersonalizationEndpointCacheTtl { get; set; }
    }

    public interface IGameProductData
    {
        int VotesCount { get; set; }
        ISystemRequirements SystemRequirements { get; set; }
        IOSRequirements OSRequirements { get; set; }
        string Languages { get; set; }
        string Copyrights { get; set; }
        INamedEntry[] Genres { get; set; }
        INamedEntry Publisher { get; set; }
        INamedEntry Developer { get; set; }
        string downloadSize { get; set; }
        IRecommendations Recommendations { get; set; }
        ITitledEntry[] Features { get; set; }
        ISeries Series { get; set; }
        IBrandRatings BrandRatings { get; set; }
        IProduct[] DLCs { get; set; }
        IProduct[] Packs { get; set; }
        IProduct[] RequiredProducts { get; set; }
        string WhatsCoolAboutIt { get; set; }
        string BackgroundImage { get; set; }
        string BackgroundImageSource { get; set; }
        IDescription Description { get; set; }
        IBonusContent BonusContent { get; set; }
        IProduct Children { get; set; }
        IProduct Parents { get; set; }
        string ImageLogoFacebook { get; set; }
        string CardSeoDescription { get; set; }
        string CardSeoKeywords { get; set; }
        string ExtraRequirements { get; set; }
        bool CanBeReviewed { get; set; }
        IPrice Price { get; set; }
        bool IsDiscounted { get; set; }
        bool IsInDevelopment { get; set; }
        long ReleaseDate { get; set; }
        IAvailability Availability { get; set; }
        ISalesVisibility SalesVisibility { get; set; }
        bool Buyable { get; set; }
        string Image { get; set; }
        string url { get; set; }
        string SupportUrl { get; set; }
        string ForumUrl { get; set; }
        IWorksOn WorksOn { get; set; }
        string Category { get; set; }
        string OriginalCategory { get; set; }
        int Rating { get; set; }
        int Type { get; set; }
        bool IsComingSoon { get; set; }
        bool IsPriceVisible { get; set; }
        bool IsMovie { get; set; }
        bool IsGame { get; set; }
        string Slug { get; set; }
        IReviewsPages Reviews { get; set; }
    }
}
