package compton_data

const (
	GogInfoSection          = "gog-info"
	GogOfferingsSection     = "gog-offerings"
	GogMediaSection         = "gog-media"
	GogNewsSection          = "gog-news"
	GogReceptionSection     = "gog-reception"
	GogCompatibilitySection = "gog-compatibility"
	GogInstallersSection    = "gog-installers"
)

var SectionTitles = map[string]string{
	GogInfoSection:          "Info",
	GogOfferingsSection:     "Offerings",
	GogMediaSection:         "Media",
	GogNewsSection:          "News",
	GogReceptionSection:     "Reception",
	GogCompatibilitySection: "Compatibility",
	GogInstallersSection:    "Files",
}

var SectionStyles = map[string]string{
	GogInfoSection:          "info.css",
	GogOfferingsSection:     "offerings.css",
	GogMediaSection:         "media.css",
	GogNewsSection:          "news.css",
	GogReceptionSection:     "reception.css",
	GogCompatibilitySection: "compatibility.css",
	GogInstallersSection:    "installers.css",
}

var SectionAccessKeys = map[string]string{
	GogInfoSection:          "i",
	GogOfferingsSection:     "o",
	GogMediaSection:         "m",
	GogNewsSection:          "n",
	GogReceptionSection:     "r",
	GogCompatibilitySection: "c",
	GogInstallersSection:    "f",
}
