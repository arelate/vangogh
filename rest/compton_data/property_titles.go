package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
)

var PropertyTitles = map[string]string{
	vangogh_integration.GogTitleProperty:               "Title",
	vangogh_integration.GogTagIdProperty:               "Account Tags",
	vangogh_integration.VangoghLocalTagsProperty:       "Local Tags",
	vangogh_integration.GogOperatingSystemsProperty:    "OS",
	vangogh_integration.GogDevelopersProperty:          "Developers",
	vangogh_integration.GogPublishersProperty:          "Publishers",
	vangogh_integration.PcgwEnginesProperty:            "Engine",
	vangogh_integration.PcgwEnginesBuildsProperty:      "Engine Build",
	vangogh_integration.GogSeriesProperty:              "Series",
	vangogh_integration.GogGenresProperty:              "Genres",
	vangogh_integration.GogThemesProperty:              "Themes",
	vangogh_integration.GogStoreTagsProperty:           "Store Tags",
	vangogh_integration.SteamCategoriesProperty:        "Steam Categories",
	vangogh_integration.GogFeaturesProperty:            "Features",
	vangogh_integration.GogGameModesProperty:           "Game Modes",
	vangogh_integration.GogLanguageCodeProperty:        "Language",
	vangogh_integration.GogIsAccountProductProperty:    "Account Product",
	vangogh_integration.GogAccountProductOrderProperty: "Account Page Order",

	vangogh_integration.GogIsIncludedByGamesProperty: "Editions",
	vangogh_integration.GogIsRequiredByGamesProperty: "DLCs",
	vangogh_integration.GogIsModifiedByGamesProperty: "Mods",
	vangogh_integration.GogIncludesGamesProperty:     "Includes",
	vangogh_integration.GogRequiresGamesProperty:     "Requires",
	vangogh_integration.GogModifiesGamesProperty:     "Modifies",

	vangogh_integration.GogProductTypeProperty: "Product Type",

	vangogh_integration.GogUserWishlistProperty: "Wishlisted",
	vangogh_integration.GogOwnedProperty:        "Owned",

	vangogh_integration.GogIsFreeProperty:                            "Free",
	vangogh_integration.GogIsDemoProperty:                            "Demo",
	vangogh_integration.GogIsModProperty:                             "Mod",
	vangogh_integration.GogIsDiscountedProperty:                      "On Sale",
	vangogh_integration.GogPreOrderProperty:                          "Pre-order",
	vangogh_integration.GogComingSoonProperty:                        "Coming Soon",
	vangogh_integration.GogInDevelopmentProperty:                     "In Development",
	vangogh_integration.SteamReviewScoreDescProperty:                 "Steam Reviews",
	vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:    "Steam Deck",
	vangogh_integration.SteamMachineCompatibilityCategoryProperty:    "Steam Machine",
	vangogh_integration.SteamFrameCompatibilityCategoryProperty:      "Steam Frame",
	vangogh_integration.SteamSteamOsAppCompatibilityCategoryProperty: "SteamOS",
	vangogh_integration.ProtonDbTierProperty:                         "ProtonDB Tier",
	vangogh_integration.ProtonDbConfidenceProperty:                   "ProtonDB Confidence",
	vangogh_integration.UrlSortParameter:                             "Sort",
	vangogh_integration.UrlDescendingParameter:                       "Desc",
	vangogh_integration.GogGlobalReleaseDateProperty:                 "Global Release",
	vangogh_integration.GogReleaseDateProperty:                       "GOG.com Release",
	vangogh_integration.GogOrderDateProperty:                         "GOG.com Order",
	vangogh_integration.GogProductValidationResultProperty:           "Validation Result",
	vangogh_integration.GogProductGeneratedChecksumProperty:          "Self-Validated",
	vangogh_integration.GogRatingProperty:                            "GOG.com Rating",
	vangogh_integration.SteamReviewScoreProperty:                     "Steam Rating",
	vangogh_integration.MetacriticScoreProperty:                      "Metacritic Rating",
	vangogh_integration.GogPriceProperty:                             "Price",
	vangogh_integration.GogBasePriceProperty:                         "Base Price",
	vangogh_integration.GogDiscountPercentageProperty:                "Discount",

	vangogh_integration.HltbHoursToCompleteMainProperty: "HLTB Main Story",
	vangogh_integration.HltbHoursToCompletePlusProperty: "HLTB Story + Extras",
	vangogh_integration.HltbHoursToComplete100Property:  "HLTB Completionist",
	vangogh_integration.HltbGenresProperty:              "HLTB Genres",
	vangogh_integration.HltbPlatformsProperty:           "HLTB Platforms",
	vangogh_integration.HltbReviewScoreProperty:         "HLTB Rating",

	vangogh_integration.OpenCriticMedianScoreProperty: "OpenCritic Rating",
	vangogh_integration.OpenCriticTierProperty:        "OpenCritic Tier",
	vangogh_integration.OpenCriticPercentileProperty:  "OpenCritic Percentile",

	GOGLinksProperty:   "GOG.com Links",
	OtherLinksProperty: "Other Links",
	SteamLinksProperty: "Steam Links",

	vangogh_integration.GogForumUrlProperty:   "Forum",
	vangogh_integration.GogStoreUrlProperty:   "Store",
	vangogh_integration.GogSupportUrlProperty: "Support",

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

	vangogh_integration.WikipediaCreatorsProperty:    "Creators",
	vangogh_integration.WikipediaDirectorsProperty:   "Directors",
	vangogh_integration.WikipediaProducersProperty:   "Producers",
	vangogh_integration.WikipediaDesignersProperty:   "Designers",
	vangogh_integration.WikipediaProgrammersProperty: "Programmers",
	vangogh_integration.WikipediaArtistsProperty:     "Artists",
	vangogh_integration.WikipediaWritersProperty:     "Writers",
	vangogh_integration.WikipediaComposersProperty:   "Composers",

	vangogh_integration.VangoghDownloadCompletedProperty: "Download Completed",
}

var ShortPropertyTitles = map[string]string{
	vangogh_integration.GogDevelopersProperty: "Dev.",
	vangogh_integration.GogPublishersProperty: "Pub.",
}

var PropertySymbols = map[string]compton.Symbol{
	vangogh_integration.GogIsIncludedByGamesProperty: compton.ItemsPack,
	vangogh_integration.GogIsRequiredByGamesProperty: compton.ItemPlus,
	vangogh_integration.GogIsModifiedByGamesProperty: compton.PuzzlePiece,
	vangogh_integration.GogIncludesGamesProperty:     compton.ItemsPack,
	vangogh_integration.GogRequiresGamesProperty:     compton.CircleCompactDisk,
	vangogh_integration.GogModifiesGamesProperty:     compton.PuzzlePiece,
}
