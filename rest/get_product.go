package rest

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

var (
	propertiesSections = map[string]string{
		vangogh_local_data.DescriptionOverviewProperty: compton_data.DescriptionSection,
		vangogh_local_data.ChangelogProperty:           compton_data.ChangelogSection,
		vangogh_local_data.ScreenshotsProperty:         compton_data.ScreenshotsSection,
		vangogh_local_data.VideoIdProperty:             compton_data.VideosSection,
	}
	propertiesSectionsOrder = []string{
		vangogh_local_data.DescriptionOverviewProperty,
		vangogh_local_data.ChangelogProperty,
		vangogh_local_data.ScreenshotsProperty,
		vangogh_local_data.VideoIdProperty,
	}

	dataTypesSections = map[vangogh_local_data.ProductType]string{
		vangogh_local_data.SteamAppNews:                 compton_data.SteamNewsSection,
		vangogh_local_data.SteamReviews:                 compton_data.SteamReviewsSection,
		vangogh_local_data.SteamDeckCompatibilityReport: compton_data.SteamDeckSection,
		//vangogh_local_data.Details:                      compton_data.DownloadsSection,
	}

	dataTypesSectionsOrder = []vangogh_local_data.ProductType{
		vangogh_local_data.SteamAppNews,
		vangogh_local_data.SteamReviews,
		vangogh_local_data.SteamDeckCompatibilityReport,
		//vangogh_local_data.Details,
	}
)

func GetProduct(w http.ResponseWriter, r *http.Request) {

	// GET /product?slug -> /product?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if r.URL.Query().Has(vangogh_local_data.SlugProperty) {
		if ids := rdx.Match(r.URL.Query(), kevlar.FullMatch); len(ids) > 0 {
			for _, id := range ids {
				http.Redirect(w, r, "/product?id="+id, http.StatusPermanentRedirect)
				return
			}
		} else {
			http.Error(w, nod.ErrorStr("unknown slug"), http.StatusInternalServerError)
			return
		}
	}

	id := r.URL.Query().Get(vangogh_local_data.IdProperty)

	hasSections := make([]string, 0)
	// every product is expected to have at least those sections
	hasSections = append(hasSections, compton_data.PropertiesSection, compton_data.ExternalLinksSection)

	for _, property := range propertiesSectionsOrder {
		if section, ok := propertiesSections[property]; ok {
			if rdx.HasKey(property, id) {
				hasSections = append(hasSections, section)
			}
		}
	}

	for _, dt := range dataTypesSectionsOrder {
		if section, ok := dataTypesSections[dt]; ok {
			if rdx.HasValue(vangogh_local_data.TypesProperty, id, dt.String()) {
				hasSections = append(hasSections, section)
			}
		}
	}

	if val, ok := rdx.GetLastVal(vangogh_local_data.OwnedProperty, id); ok && val == vangogh_local_data.TrueValue {
		hasSections = append(hasSections, compton_data.DownloadsSection)
	}

	if productPage := compton_pages.Product(id, rdx, hasSections); productPage != nil {
		if err := productPage.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	} else {
		http.NotFound(w, r)
	}
}
