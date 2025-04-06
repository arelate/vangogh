package compton_pages

import (
	"fmt"
	"github.com/arelate/southern_light"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/gogdb_integration"
	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/igdb_integration"
	"github.com/arelate/southern_light/ign_integration"
	"github.com/arelate/southern_light/mobygames_integration"
	"github.com/arelate/southern_light/opencritic_integration"
	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/protondb_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/strategywiki_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/vndb_integration"
	"github.com/arelate/southern_light/wikipedia_integration"
	"github.com/arelate/southern_light/winehq_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/redux"
	"net/url"
	"strings"
)

func Links(id string, rdx redux.Readable) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.LinksSection)

	stack := compton.FlexItems(s, direction.Column)
	s.Append(stack)

	extLinks := externalLinks(id, rdx)

	for _, linkProperty := range compton_data.ProductExternalLinksProperties {
		if links, ok := extLinks[linkProperty]; ok && len(links) > 0 {

			if eltv := linksTitleValues(s, linkProperty, links); eltv != nil {
				stack.Append(eltv)
			}

		}
	}

	return s
}

func gogLink(p string) string {
	u := url.URL{
		Scheme: southern_light.HttpsScheme,
		Host:   gog_integration.WwwGogHost,
		Path:   p,
	}
	return u.String()
}

func externalLinks(id string, rdx redux.Readable) map[string][]string {

	links := make(map[string][]string)

	for _, p := range []string{
		vangogh_integration.StoreUrlProperty,
		vangogh_integration.ForumUrlProperty,
		vangogh_integration.SupportUrlProperty} {
		if val, ok := rdx.GetLastVal(p, id); ok {
			links[compton_data.GauginGOGLinksProperty] = append(links[compton_data.GauginGOGLinksProperty],
				fmt.Sprintf("%s=%s", p, gogLink(val)))
		}
	}

	if appId, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, id); ok {
		links[compton_data.GauginSteamLinksProperty] =
			append(links[compton_data.GauginSteamLinksProperty],
				fmt.Sprintf("%s=%s", compton_data.GauginSteamCommunityUrlProperty, steam_integration.SteamCommunityUrl(appId)))
		links[compton_data.GauginSteamLinksProperty] =
			append(links[compton_data.GauginSteamLinksProperty],
				fmt.Sprintf("%s=%s", compton_data.GauginSteamGuidesUrlProperty, steam_integration.SteamGuidesUrl(appId)))
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty],
				fmt.Sprintf("%s=%s", compton_data.GauginProtonDBUrlProperty, protondb_integration.ProtonDBUrl(appId)))
	}

	links[compton_data.GauginOtherLinksProperty] = append(links[compton_data.GauginOtherLinksProperty],
		fmt.Sprintf("%s=%s", compton_data.GauginGOGDBUrlProperty, gogdb_integration.GOGDBUrl(id)))

	if website, ok := rdx.GetLastVal(vangogh_integration.WebsiteProperty, id); ok && website != "" {
		links[compton_data.GauginOtherLinksProperty] = append(links[compton_data.GauginOtherLinksProperty],
			fmt.Sprintf("%s=%s", compton_data.GauginWebsiteUrlProperty, website))
	}

	if pcgwWikiUrl := otherLink(id,
		vangogh_integration.PcgwPageIdProperty,
		compton_data.GauginPCGamingWikiUrlProperty,
		pcgw_integration.WikiUrl, rdx); pcgwWikiUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], pcgwWikiUrl)
	}
	if hltbUrl := otherLink(id,
		vangogh_integration.HltbIdProperty,
		compton_data.GauginHltbUrlProperty,
		hltb_integration.GameUrl, rdx); hltbUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], hltbUrl)
	}
	if igdbUrl := otherLink(id,
		vangogh_integration.IgdbIdProperty,
		compton_data.GauginIGDBUrlProperty,
		igdb_integration.GameUrl, rdx); igdbUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], igdbUrl)
	}
	if strategyWikiUrl := otherLink(id,
		vangogh_integration.StrategyWikiIdProperty,
		compton_data.GauginStrategyWikiUrlProperty,
		strategywiki_integration.WikiUrl, rdx); strategyWikiUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], strategyWikiUrl)
	}
	if mobyGamesUrl := otherLink(id,
		vangogh_integration.MobyGamesIdProperty,
		compton_data.GauginMobyGamesUrlProperty,
		mobygames_integration.GameUrl, rdx); mobyGamesUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], mobyGamesUrl)
	}
	if wikipediaUrl := otherLink(id,
		vangogh_integration.WikipediaIdProperty,
		compton_data.GauginWikipediaUrlProperty,
		wikipedia_integration.WikiUrl, rdx); wikipediaUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], wikipediaUrl)
	}
	if wineHqUrl := otherLink(id,
		vangogh_integration.WineHQIdProperty,
		compton_data.GauginWineHQUrlProperty,
		winehq_integration.WineHQUrl, rdx); wineHqUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], wineHqUrl)
	}
	if vndbUrl := otherLink(id,
		vangogh_integration.VndbIdProperty,
		compton_data.GauginVNDBUrlProperty,
		vndb_integration.ItemUrl, rdx); vndbUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], vndbUrl)
	}
	if ignWikiUrl := otherLink(id,
		vangogh_integration.IGNWikiSlugProperty,
		compton_data.GauginIGNWikiUrlProperty,
		ign_integration.WikiUrl, rdx); ignWikiUrl != "" {
		links[compton_data.GauginOtherLinksProperty] =
			append(links[compton_data.GauginOtherLinksProperty], ignWikiUrl)
	}

	if openCriticId, ok := rdx.GetLastVal(vangogh_integration.OpenCriticIdProperty, id); ok {
		if openCriticSlug, sure := rdx.GetLastVal(vangogh_integration.OpenCriticSlugProperty, id); sure {
			openCriticUrl := fmt.Sprintf("%s=%s", compton_data.GauginOpenCriticUrlProperty, opencritic_integration.GameUrl(openCriticId, openCriticSlug))
			links[compton_data.GauginOtherLinksProperty] =
				append(links[compton_data.GauginOtherLinksProperty], openCriticUrl)
		}
	}

	return links
}

func otherLink(id, property, urlProperty string, urlFunc func(string) *url.URL, rdx redux.Readable) string {
	if value, ok := rdx.GetLastVal(property, id); ok && value != "" {
		return fmt.Sprintf("%s=%s", urlProperty, urlFunc(value))
	}
	return ""
}

func linksTitleValues(r compton.Registrar, property string, links []string) compton.Element {
	linksHrefs := make(map[string]string)
	for _, link := range links {
		if linkProperty, value, ok := strings.Cut(link, "="); ok {
			linkPropertyTitle := compton_data.PropertyTitles[linkProperty]
			linksHrefs[linkPropertyTitle] = value
		}
	}
	propertyTitle := compton_data.PropertyTitles[property]
	tv := compton.TitleValues(r, propertyTitle).
		ForegroundColor(color.Cyan).
		TitleForegroundColor(color.Foreground).
		SetLinksTarget(compton.LinkTargetTop).
		AppendLinkValues(linksHrefs)
	return tv
}
