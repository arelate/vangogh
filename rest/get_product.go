package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"net/http"
)

var (
	propertiesSections = map[string]string{
		vangogh_integration.DescriptionOverviewProperty: compton_data.DescriptionSection,
		vangogh_integration.ChangelogProperty:           compton_data.ChangelogSection,
		vangogh_integration.ScreenshotsProperty:         compton_data.ScreenshotsSection,
		vangogh_integration.VideoIdProperty:             compton_data.VideosSection,
	}
	propertiesSectionsOrder = []string{
		vangogh_integration.DescriptionOverviewProperty,
		vangogh_integration.ChangelogProperty,
		vangogh_integration.ScreenshotsProperty,
		vangogh_integration.VideoIdProperty,
	}

	dataTypesSections = map[vangogh_integration.ProductType]string{
		vangogh_integration.SteamAppNews:                 compton_data.SteamNewsSection,
		vangogh_integration.SteamAppReviews:              compton_data.SteamReviewsSection,
		vangogh_integration.SteamDeckCompatibilityReport: compton_data.SteamDeckSection,
		//vangogh_integration.Details:                      compton_data.InstallersSection,
	}

	dataTypesSectionsOrder = []vangogh_integration.ProductType{
		vangogh_integration.SteamAppNews,
		vangogh_integration.SteamAppReviews,
		vangogh_integration.SteamDeckCompatibilityReport,
		//vangogh_integration.Details,
	}
)

func GetProduct(w http.ResponseWriter, r *http.Request) {

	// GET /product?slug -> /product?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if r.URL.Query().Has(vangogh_integration.SlugProperty) {
		if ids := rdx.Match(r.URL.Query(), redux.FullMatch); ids != nil {
			for id := range ids {
				http.Redirect(w, r, "/product?id="+id, http.StatusPermanentRedirect)
				return
			}
		} else {
			http.Error(w, nod.ErrorStr("unknown slug"), http.StatusInternalServerError)
			return
		}
	}

	id := r.URL.Query().Get(vangogh_integration.IdProperty)

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
			if rdx.HasValue(vangogh_integration.TypesProperty, id, dt.String()) {
				hasSections = append(hasSections, section)
			}
		}
	}

	if val, ok := rdx.GetLastVal(vangogh_integration.LicencesProperty, id); ok && val == vangogh_integration.TrueValue {
		hasSections = append(hasSections, compton_data.InstallersSection)
	}

	if productPage := compton_pages.Product(id, rdx, hasSections); productPage != nil {
		if err := productPage.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	} else {
		http.NotFound(w, r)
	}
}
