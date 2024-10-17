package compton_data

import "github.com/arelate/vangogh/rest/compton_styles"

var SectionStyles = map[string][]byte{
	PropertiesSection:    nil,
	ExternalLinksSection: nil,
	DescriptionSection:   compton_styles.DescriptionStyle,
	ScreenshotsSection:   compton_styles.ScreenshotsStyle,
	VideosSection:        compton_styles.VideosStyle,
	ChangelogSection:     compton_styles.ChangelogStyle,
	SteamNewsSection:     compton_styles.SteamNewsStyle,
	SteamReviewsSection:  compton_styles.SteamReviewsStyle,
	SteamDeckSection:     compton_styles.SteamDeckStyle,
	DownloadsSection:     compton_styles.DownloadsStyle,
}
