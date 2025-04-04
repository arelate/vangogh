package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"slices"
)

func Debug(gogId string) (compton.PageElement, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	reduxProperties := make([]string, 0)

	for _, property := range vangogh_integration.ReduxProperties() {
		if slices.Contains(reduxProperties, property) {
			continue
		}
		reduxProperties = append(reduxProperties, property)
	}
	slices.Sort(reduxProperties)

	rdx, err := redux.NewReader(reduxDir, reduxProperties...)
	if err != nil {
		return nil, err
	}

	var productTitle string
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, gogId); ok && title != "" {
		productTitle = title
	} else {
		productTitle = "[GOG Id " + gogId + " title not found]"
	}

	var productTypes []vangogh_integration.ProductType
	if pts, ok := rdx.GetAllValues(vangogh_integration.TypesProperty, gogId); ok && len(pts) > 0 {
		for _, pt := range pts {
			productTypes = append(productTypes, vangogh_integration.ParseProductType(pt))
		}
	}

	p := compton.Page(productTitle)
	p.RegisterStyles(compton_styles.Styles, "debug.css")

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(pageStack)

	pageStack.Append(compton.FICenter(p, compton.H1Text(productTitle)))

	idsFrow := compton.Frow(p)
	pageStack.Append(compton.FICenter(p, idsFrow))

	idsFrow.Heading("Platform Ids")
	idsFrow.PropVal("GOG", gogId)

	var steamAppId string
	if sai, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok {
		steamAppId = sai
		idsFrow.PropVal("Steam", steamAppId)
	}

	var pcgwPageId string
	if pid, ok := rdx.GetLastVal(vangogh_integration.PcgwPageIdProperty, gogId); ok {
		pcgwPageId = pid
		idsFrow.PropVal("PCGW", pcgwPageId)
	}

	var hltbId string
	if hid, ok := rdx.GetLastVal(vangogh_integration.HltbIdProperty, gogId); ok {
		hltbId = hid
		idsFrow.PropVal("HLTB", hltbId)
	}

	var openCriticId string
	if ocid, ok := rdx.GetLastVal(vangogh_integration.OpenCriticIdProperty, gogId); ok && ocid != "" {
		openCriticId = ocid
		idsFrow.PropVal("OpenCritic", openCriticId)
	}

	// various other platform ids
	if igdbId, ok := rdx.GetLastVal(vangogh_integration.IgdbIdProperty, gogId); ok && igdbId != "" {
		idsFrow.PropVal("IGDB", igdbId)
	}
	if mobyGamesId, ok := rdx.GetLastVal(vangogh_integration.MobyGamesIdProperty, gogId); ok && mobyGamesId != "" {
		idsFrow.PropVal("MobyGames", mobyGamesId)
	}
	if vndbId, ok := rdx.GetLastVal(vangogh_integration.VndbIdProperty, gogId); ok && vndbId != "" {
		idsFrow.PropVal("VNDB", vndbId)
	}
	if wikipediaId, ok := rdx.GetLastVal(vangogh_integration.WikipediaIdProperty, gogId); ok && wikipediaId != "" {
		idsFrow.PropVal("Wikipedia", wikipediaId)
	}
	if strategyWikiId, ok := rdx.GetLastVal(vangogh_integration.StrategyWikiIdProperty, gogId); ok && strategyWikiId != "" {
		idsFrow.PropVal("StrategyWiki", strategyWikiId)
	}

	propertyProductType := map[string]vangogh_integration.ProductType{
		vangogh_integration.CatalogPageProductsProperty: vangogh_integration.CatalogPage,
		vangogh_integration.AccountPageProductsProperty: vangogh_integration.AccountPage,
		vangogh_integration.OrderPageProductsProperty:   vangogh_integration.OrderPage,
	}

	for property, pt := range propertyProductType {

		if !slices.Contains(productTypes, pt) {
			continue
		}

		if page, ok := rdx.GetLastVal(property, gogId); ok && page != "" {
			if ds := productTypeSection(p, page, pt); ds != nil {
				pageStack.Append(ds)
			}
		}

	}

	gogProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.ApiProducts,
		vangogh_integration.Details,
		vangogh_integration.GamesDbGogProducts,
		vangogh_integration.PcgwGogPageId,
	}

	for _, pt := range gogProductTypes {
		if !slices.Contains(productTypes, pt) {
			continue
		}

		if ds := productTypeSection(p, gogId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	steamProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.SteamAppDetails,
		vangogh_integration.SteamAppNews,
		vangogh_integration.SteamAppReviews,
		vangogh_integration.SteamDeckCompatibilityReport,
		vangogh_integration.PcgwSteamPageId,
	}

	for _, pt := range steamProductTypes {
		if !slices.Contains(productTypes, pt) {
			continue
		}

		if ds := productTypeSection(p, steamAppId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	pcgwProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.PcgwEngine,
		vangogh_integration.PcgwExternalLinks,
	}

	for _, pt := range pcgwProductTypes {
		if !slices.Contains(productTypes, pt) {
			continue
		}

		if ds := productTypeSection(p, pcgwPageId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	hltbProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.HltbData,
	}

	for _, pt := range hltbProductTypes {
		if !slices.Contains(productTypes, pt) {
			continue
		}

		if ds := productTypeSection(p, hltbId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	protonDbProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.ProtonDbSummary,
	}

	for _, pt := range protonDbProductTypes {
		if !slices.Contains(productTypes, pt) {
			continue
		}

		if ds := productTypeSection(p, steamAppId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	openCriticProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.OpenCriticApiGame,
	}

	for _, pt := range openCriticProductTypes {
		if !slices.Contains(productTypes, pt) {
			continue
		}

		if ds := productTypeSection(p, openCriticId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	reduxDs := compton.DSLarge(p, "Redux", false).BackgroundColor(color.Highlight)
	pageStack.Append(reduxDs)

	reduxStack := compton.FlexItems(p, direction.Column)
	reduxStack.AddClass("redux-data")
	reduxDs.Append(compton.FICenter(p, reduxStack))

	closedProperties := []string{
		vangogh_integration.DescriptionOverviewProperty,
		vangogh_integration.DescriptionFeaturesProperty,
		vangogh_integration.AdditionalRequirementsProperty,
		vangogh_integration.CopyrightsProperty,
		vangogh_integration.DehydratedImageProperty,
		vangogh_integration.StoreTagsProperty,
		vangogh_integration.ScreenshotsProperty,
		vangogh_integration.TypesProperty,
		vangogh_integration.HltbPlatformsProperty,
		vangogh_integration.ShortDescriptionProperty,
		vangogh_integration.SteamCategoriesProperty,
		vangogh_integration.ThemesProperty,
		vangogh_integration.ChangelogProperty,
	}

	propertySources := make(map[string][]string)
	for pt, properties := range vangogh_integration.ProductTypeProperties {
		for _, property := range properties {
			propertySources[property] = append(propertySources[property], compton_data.TypesTitles[pt.String()])
		}
	}

	for _, property := range reduxProperties {
		if values, ok := rdx.GetAllValues(property, gogId); ok {

			open := !slices.Contains(closedProperties, property)

			ds := compton.DSSmall(p, property, open)

			dsStack := compton.FlexItems(p, direction.Column).AlignItems(align.Start)
			ds.Append(dsStack)

			if len(propertySources[property]) > 0 {
				sourcesFrow := compton.Frow(p).FontSize(size.Small)
				sourcesFrow.PropVal("Sources", propertySources[property]...)
				dsStack.Append(sourcesFrow)
			}

			ul := compton.Ul()
			for _, value := range values {
				ul.Append(compton.ListItemText(value))
			}

			dsStack.Append(ul)

			reduxStack.Append(ds)
		}
	}

	// get-data per-type errors

	ds := compton.DSLarge(p, "Get Data Errors", false).BackgroundColor(color.Highlight)
	iframe := compton.IframeExpandHost(p, "get-data-errors", "/debug-get-data-errors?id="+gogId)
	ds.Append(iframe)
	pageStack.Append(ds)

	return p, nil
}

func productTypeSection(r compton.Registrar, id string, pt vangogh_integration.ProductType) compton.Element {
	ds := compton.DSLarge(r, compton_data.TypesTitles[pt.String()], false).BackgroundColor(color.Highlight)

	iframe := compton.IframeExpandHost(r, pt.String(), "/debug-data?id="+id+"&product-type="+pt.String())
	ds.Append(iframe)

	return ds
}
