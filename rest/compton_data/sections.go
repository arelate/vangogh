package compton_data

const (
	InfoSection          = "info"
	OfferingsSection     = "offerings"
	MediaSection         = "media"
	NewsSection          = "news"
	ReceptionSection     = "reception"
	CompatibilitySection = "compatibility"
	InstallersSection    = "installers"
)

var SectionTitles = map[string]string{
	InfoSection:          "Info",
	OfferingsSection:     "Offerings",
	MediaSection:         "Media",
	NewsSection:          "News",
	ReceptionSection:     "Reception",
	CompatibilitySection: "Compatibility",
	InstallersSection:    "Files",
}

var SectionStyles = map[string]string{
	InfoSection:          "info.css",
	OfferingsSection:     "offerings.css",
	MediaSection:         "media.css",
	NewsSection:          "news.css",
	ReceptionSection:     "reception.css",
	CompatibilitySection: "compatibility.css",
	InstallersSection:    "installers.css",
}

var SectionAccessKeys = map[string]string{
	InfoSection:          "i",
	OfferingsSection:     "o",
	MediaSection:         "m",
	NewsSection:          "n",
	ReceptionSection:     "r",
	CompatibilitySection: "c",
	InstallersSection:    "f",
}
