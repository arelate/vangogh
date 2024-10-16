package compton_data

const (
	ChangelogSection     = "changelog"
	DescriptionSection   = "description"
	DownloadsSection     = "downloads"
	ExternalLinksSection = "links"
	PropertiesSection    = "properties"
	ScreenshotsSection   = "screenshots"
	SteamNewsSection     = "steam-news"
	SteamReviewsSection  = "steam-reviews"
	SteamDeckSection     = "steam-deck"
	VideosSection        = "videos"
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

var SectionsTitlesOrder = []string{
	SectionTitles[PropertiesSection],
	SectionTitles[ExternalLinksSection],
	SectionTitles[DescriptionSection],
	SectionTitles[ChangelogSection],
	SectionTitles[ScreenshotsSection],
	SectionTitles[VideosSection],
	SectionTitles[SteamNewsSection],
	SectionTitles[SteamReviewsSection],
	SectionTitles[SteamDeckSection],
	SectionTitles[DownloadsSection],
}
