package compton_data

const (
	PropertiesSection    = "properties"
	ExternalLinksSection = "external-links"
	DescriptionSection   = "description"
	ChangelogSection     = "changelog"
	ScreenshotsSection   = "screenshots"
	VideosSection        = "videos"
	SteamNewsSection     = "steam-news"
	SteamReviewsSection  = "steam-reviews"
	SteamDeckSection     = "steam-deck"
	DownloadsSection     = "downloads"
)

var SectionTitles = map[string]string{
	ChangelogSection:     "Changelog",
	DescriptionSection:   "Description",
	DownloadsSection:     "Downloads",
	ExternalLinksSection: "External Links",
	PropertiesSection:    "Properties",
	ScreenshotsSection:   "Screenshots",
	SteamNewsSection:     "Steam News",
	SteamReviewsSection:  "Steam Reviews",
	SteamDeckSection:     "Steam Deck",
	VideosSection:        "Videos",
}
