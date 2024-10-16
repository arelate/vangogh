package rest

import (
	"fmt"
	"github.com/arelate/southern_light"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/gogdb_integration"
	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/igdb_integration"
	"github.com/arelate/southern_light/ign_integration"
	"github.com/arelate/southern_light/mobygames_integration"
	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/protondb_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/strategywiki_integration"
	"github.com/arelate/southern_light/vndb_integration"
	"github.com/arelate/southern_light/wikipedia_integration"
	"github.com/arelate/southern_light/winehq_integration"
	"github.com/arelate/vangogh/data"
	"github.com/arelate/vangogh/paths"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
	"strconv"
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
		vangogh_local_data.Details:                      compton_data.DownloadsSection,
	}

	dataTypesSectionsOrder = []vangogh_local_data.ProductType{
		vangogh_local_data.SteamAppNews,
		vangogh_local_data.SteamReviews,
		vangogh_local_data.SteamDeckCompatibilityReport,
		vangogh_local_data.Details,
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
				http.Redirect(w, r, paths.ProductId(id), http.StatusPermanentRedirect)
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

	if productPage := compton_pages.Product(id, rdx, hasSections, externalLinks(id, rdx)); productPage != nil {
		if err := productPage.WriteContent(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	} else {
		http.NotFound(w, r)
	}
}

func gogLink(p string) string {
	u := url.URL{
		Scheme: southern_light.HttpsScheme,
		Host:   gog_integration.WwwGogHost,
		Path:   p,
	}
	return u.String()
}

func externalLinks(id string, rdx kevlar.ReadableRedux) map[string][]string {

	links := make(map[string][]string)

	for _, p := range []string{
		vangogh_local_data.StoreUrlProperty,
		vangogh_local_data.ForumUrlProperty,
		vangogh_local_data.SupportUrlProperty} {
		if val, ok := rdx.GetLastVal(p, id); ok {
			links[data.GauginGOGLinksProperty] = append(links[data.GauginGOGLinksProperty],
				fmt.Sprintf("%s=%s", p, gogLink(val)))
		}
	}

	if steamAppId, ok := rdx.GetLastVal(vangogh_local_data.SteamAppIdProperty, id); ok {
		if appId, err := strconv.ParseUint(steamAppId, 10, 32); err == nil {
			uAppId := uint32(appId)
			links[data.GauginSteamLinksProperty] =
				append(links[data.GauginSteamLinksProperty],
					fmt.Sprintf("%s=%s", data.GauginSteamCommunityUrlProperty, steam_integration.SteamCommunityUrl(uAppId)))
			links[data.GauginOtherLinksProperty] =
				append(links[data.GauginOtherLinksProperty],
					fmt.Sprintf("%s=%s", data.GauginProtonDBUrlProperty, protondb_integration.ProtonDBUrl(uAppId)))
		}
	}

	links[data.GauginOtherLinksProperty] = append(links[data.GauginOtherLinksProperty],
		fmt.Sprintf("%s=%s", data.GauginGOGDBUrlProperty, gogdb_integration.GOGDBUrl(id)))

	otherLink(links,
		vangogh_local_data.PCGWPageIdProperty,
		data.GauginPCGamingWikiUrlProperty,
		pcgw_integration.WikiUrl)
	otherLink(links,
		vangogh_local_data.HLTBIdProperty,
		data.GauginHLTBUrlProperty,
		hltb_integration.GameUrl)
	otherLink(links,
		vangogh_local_data.IGDBIdProperty,
		data.GauginIGDBUrlProperty,
		igdb_integration.GameUrl)
	otherLink(links,
		vangogh_local_data.StrategyWikiIdProperty,
		data.GauginStrategyWikiUrlProperty,
		strategywiki_integration.WikiUrl)
	otherLink(links,
		vangogh_local_data.MobyGamesIdProperty,
		data.GauginMobyGamesUrlProperty,
		mobygames_integration.GameUrl)
	otherLink(links,
		vangogh_local_data.WikipediaIdProperty,
		data.GauginWikipediaUrlProperty,
		wikipedia_integration.WikiUrl)
	otherLink(links,
		vangogh_local_data.WineHQIdProperty,
		data.GauginWineHQUrlProperty,
		winehq_integration.WineHQUrl)
	otherLink(links,
		vangogh_local_data.VNDBIdProperty,
		data.GauginVNDBUrlProperty,
		vndb_integration.ItemUrl)
	otherLink(links,
		vangogh_local_data.IGNWikiSlugProperty,
		data.GauginIGNWikiUrlProperty,
		ign_integration.WikiUrl)

	return links
}

func otherLink(rdx map[string][]string, p string, up string, uf func(string) *url.URL) {
	if len(rdx[p]) > 0 {
		id := rdx[p][0]
		rdx[data.GauginOtherLinksProperty] = append(rdx[data.GauginOtherLinksProperty],
			fmt.Sprintf("%s=%s", up, uf(id)))
	}
}
