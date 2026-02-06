package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
)

var PropertyTitles = map[string]string{
	vangogh_integration.TitleProperty:               "Title",
	vangogh_integration.DescriptionOverviewProperty: "Description",
	vangogh_integration.TagIdProperty:               "Account Tags",
	vangogh_integration.LocalTagsProperty:           "Local Tags",
	vangogh_integration.OperatingSystemsProperty:    "OS",
	vangogh_integration.DevelopersProperty:          "Developers",
	vangogh_integration.PublishersProperty:          "Publishers",
	vangogh_integration.EnginesProperty:             "Engine",
	vangogh_integration.EnginesBuildsProperty:       "Engine Build",
	vangogh_integration.SeriesProperty:              "Series",
	vangogh_integration.GenresProperty:              "Genres",
	vangogh_integration.ThemesProperty:              "Themes",
	vangogh_integration.StoreTagsProperty:           "Store Tags",
	vangogh_integration.SteamCategoriesProperty:     "Steam Categories",
	vangogh_integration.FeaturesProperty:            "Features",
	vangogh_integration.GameModesProperty:           "Game Modes",
	vangogh_integration.LanguageCodeProperty:        "Language",

	vangogh_integration.IsIncludedByGamesProperty: "Editions",
	vangogh_integration.IsRequiredByGamesProperty: "DLCs",
	vangogh_integration.IsModifiedByGamesProperty: "Mods",
	vangogh_integration.IncludesGamesProperty:     "Includes",
	vangogh_integration.RequiresGamesProperty:     "Requires",
	vangogh_integration.ModifiesGamesProperty:     "Modifies",

	vangogh_integration.ProductTypeProperty: "Product Type",

	vangogh_integration.UserWishlistProperty: "Wishlisted",
	vangogh_integration.OwnedProperty:        "Owned",

	vangogh_integration.IsFreeProperty:                            "Free",
	vangogh_integration.IsDemoProperty:                            "Demo",
	vangogh_integration.IsModProperty:                             "Mod",
	vangogh_integration.IsDiscountedProperty:                      "On Sale",
	vangogh_integration.PreOrderProperty:                          "Pre-order",
	vangogh_integration.ComingSoonProperty:                        "Coming Soon",
	vangogh_integration.InDevelopmentProperty:                     "In Development",
	vangogh_integration.TypesProperty:                             "Data Type",
	vangogh_integration.SteamReviewScoreDescProperty:              "Steam Reviews",
	vangogh_integration.SteamDeckAppCompatibilityCategoryProperty: "Steam Deck",
	vangogh_integration.SteamOsAppCompatibilityCategoryProperty:   "SteamOS",
	vangogh_integration.ProtonDBTierProperty:                      "ProtonDB Tier",
	vangogh_integration.ProtonDBConfidenceProperty:                "ProtonDB Confidence",
	vangogh_integration.SortProperty:                              "Sort",
	vangogh_integration.DescendingProperty:                        "Desc",
	vangogh_integration.GlobalReleaseDateProperty:                 "Global Release",
	vangogh_integration.GOGReleaseDateProperty:                    "GOG.com Release",
	vangogh_integration.GOGOrderDateProperty:                      "GOG.com Order",
	vangogh_integration.ProductValidationResultProperty:           "Validation Result",
	vangogh_integration.ProductGeneratedChecksumProperty:          "Generated Checksum",
	vangogh_integration.RatingProperty:                            "GOG.com Rating",
	vangogh_integration.SteamReviewScoreProperty:                  "Steam Rating",
	vangogh_integration.MetacriticScoreProperty:                   "Metacritic Rating",
	vangogh_integration.PriceProperty:                             "Price",
	vangogh_integration.BasePriceProperty:                         "Base Price",
	vangogh_integration.DiscountPercentageProperty:                "Discount",

	vangogh_integration.HltbHoursToCompleteMainProperty: "HLTB Main Story",
	vangogh_integration.HltbHoursToCompletePlusProperty: "HLTB Story + Extras",
	vangogh_integration.HltbHoursToComplete100Property:  "HLTB Completionist",
	vangogh_integration.HltbGenresProperty:              "HLTB Genres",
	vangogh_integration.HltbPlatformsProperty:           "HLTB Platforms",
	vangogh_integration.HltbReviewScoreProperty:         "HLTB Rating",

	vangogh_integration.SummaryRatingProperty:  "Summary Rating",
	vangogh_integration.SummaryReviewsProperty: "Summary Reviews",

	vangogh_integration.OpenCriticMedianScoreProperty: "OpenCritic Rating",
	vangogh_integration.OpenCriticTierProperty:        "OpenCritic Tier",
	vangogh_integration.TopPercentProperty:            "OpenCritic Top",

	GOGLinksProperty:   "GOG.com Links",
	OtherLinksProperty: "Other Links",
	SteamLinksProperty: "Steam Links",

	vangogh_integration.ForumUrlProperty:   "Forum",
	vangogh_integration.StoreUrlProperty:   "Store",
	vangogh_integration.SupportUrlProperty: "Support",

	SteamCommunityUrlProperty: "Community",
	SteamGuidesUrlProperty:    "Guides",

	GOGDBUrlProperty:        "GOGDB",
	IGDBUrlProperty:         "IGDB",
	HltbUrlProperty:         "HLTB",
	MobyGamesUrlProperty:    "MobyGames",
	PCGamingWikiUrlProperty: "PCGamingWiki",
	ProtonDBUrlProperty:     "ProtonDB",
	StrategyWikiUrlProperty: "StrategyWiki",
	WikipediaUrlProperty:    "Wikipedia",
	WineHQUrlProperty:       "WineHQ",
	VNDBUrlProperty:         "VNDB",
	IGNWikiUrlProperty:      "IGN Wiki",
	OpenCriticUrlProperty:   "OpenCritic",
	MetacriticUrlProperty:   "Metacritic",
	WebsiteUrlProperty:      "Website",

	vangogh_integration.CreditsProperty:     "Credits",
	vangogh_integration.CreatorsProperty:    "Creators",
	vangogh_integration.DirectorsProperty:   "Directors",
	vangogh_integration.ProducersProperty:   "Producers",
	vangogh_integration.DesignersProperty:   "Designers",
	vangogh_integration.ProgrammersProperty: "Programmers",
	vangogh_integration.ArtistsProperty:     "Artists",
	vangogh_integration.WritersProperty:     "Writers",
	vangogh_integration.ComposersProperty:   "Composers",
}

var ShortPropertyTitles = map[string]string{
	vangogh_integration.DevelopersProperty: "Dev.",
	vangogh_integration.PublishersProperty: "Pub.",
}

var PropertySymbols = map[string]compton.Symbol{
	vangogh_integration.IsIncludedByGamesProperty: compton.ItemsPack,
	vangogh_integration.IsRequiredByGamesProperty: compton.ItemPlus,
	vangogh_integration.IsModifiedByGamesProperty: compton.PuzzlePiece,
	vangogh_integration.IncludesGamesProperty:     compton.ItemsPack,
	vangogh_integration.RequiresGamesProperty:     compton.CircleCompactDisk,
	vangogh_integration.ModifiesGamesProperty:     compton.PuzzlePiece,
}
