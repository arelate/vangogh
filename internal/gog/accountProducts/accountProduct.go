package accountProducts

const (
	MediaTypeGame  = 1
	MediaTypeMovie = 2
)

type AccountProduct struct {
	IsGalaxyCompatible bool          `json:"isGalaxyCompatible"`
	Tags               []interface{} `json:"tags"`
	ID                 int           `json:"id"`
	Availability       struct {
		IsAvailable          bool `json:"isAvailable"`
		IsAvailableInAccount bool `json:"isAvailableInAccount"`
	} `json:"availability"`
	Title   string `json:"title"`
	Image   string `json:"image"`
	URL     string `json:"url"`
	WorksOn struct {
		Windows bool `json:"Windows"`
		Mac     bool `json:"Mac"`
		Linux   bool `json:"Linux"`
	} `json:"worksOn"`
	Category     string `json:"category"`
	Rating       int    `json:"rating"`
	IsComingSoon bool   `json:"isComingSoon"`
	IsMovie      bool   `json:"isMovie"`
	IsGame       bool   `json:"isGame"`
	Slug         string `json:"slug"`
	Updates      int    `json:"updates"`
	IsNew        bool   `json:"isNew"`
	DlcCount     int    `json:"dlcCount"`
	ReleaseDate  struct {
		Date         string `json:"date"`
		TimezoneType int    `json:"timezone_type"`
		Timezone     string `json:"timezone"`
	} `json:"releaseDate"`
	IsBaseProductMissing bool `json:"isBaseProductMissing"`
	IsHidingDisabled     bool `json:"isHidingDisabled"`
	IsInDevelopment      bool `json:"isInDevelopment"`
	IsHidden             bool `json:"isHidden"`
}
