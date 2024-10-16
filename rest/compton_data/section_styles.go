package compton_data

import "github.com/arelate/vangogh/rest/gaugin_styles"

var SectionStyles = map[string][]byte{
	PropertiesSection:    nil,
	ExternalLinksSection: nil,
	DescriptionSection:   gaugin_styles.DescriptionStyle,
	ScreenshotsSection:   gaugin_styles.ScreenshotsStyle,
	VideosSection:        gaugin_styles.VideosStyle,
	ChangelogSection:     gaugin_styles.ChangelogStyle,
	SteamNewsSection:     gaugin_styles.SteamNewsStyle,
	SteamReviewsSection:  gaugin_styles.SteamReviewsStyle,
	SteamDeckSection:     gaugin_styles.SteamDeckStyle,
	DownloadsSection:     gaugin_styles.DownloadsStyle,
}
