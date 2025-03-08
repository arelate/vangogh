package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"slices"
)

func Debug(gogId string) (compton.PageElement, error) {

	localProducts := vangogh_integration.AllProductTypes()
	sortedLocalProducts := slices.Sorted(localProducts)

	kvs := make(map[vangogh_integration.ProductType]kevlar.KeyValues, len(sortedLocalProducts))

	for _, pt := range sortedLocalProducts {
		if pt == vangogh_integration.UnknownProductType {
			continue
		}
		if absPtDir, err := vangogh_integration.AbsProductTypeDir(pt); err != nil {
			return nil, err
		} else {
			if kvs[pt], err = kevlar.New(absPtDir, kevlar.JsonExt); err != nil {
				return nil, err
			}
		}
	}

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
	}

	if steamAppId != "" {
		idsFrow.PropVal("Steam", steamAppId)
	}

	var pcgwPageId string
	if pid, ok := rdx.GetLastVal(vangogh_integration.PcgwPageIdProperty, gogId); ok {
		pcgwPageId = pid
	}

	if pcgwPageId != "" {
		idsFrow.PropVal("PCGW", pcgwPageId)
	}

	var hltbId string
	if hid, ok := rdx.GetLastVal(vangogh_integration.HltbIdProperty, gogId); ok {
		hltbId = hid
	}

	if hltbId != "" {
		idsFrow.PropVal("HLTB", hltbId)
	}

	gogProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.ApiProducts,
		vangogh_integration.Details,
		vangogh_integration.GamesDbGogProducts,
		vangogh_integration.PcgwGogPageId,
	}

	for _, pt := range gogProductTypes {
		if !kvs[pt].Has(gogId) {
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
		if !kvs[pt].Has(steamAppId) {
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
		if !kvs[pt].Has(pcgwPageId) {
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
		if !kvs[pt].Has(hltbId) {
			continue
		}

		if ds := productTypeSection(p, hltbId, pt); ds != nil {
			pageStack.Append(ds)
		}
	}

	reduxDs := compton.DSLarge(p, "Redux", false).BackgroundColor(color.Highlight)
	pageStack.Append(reduxDs)

	reduxStack := compton.FlexItems(p, direction.Column)
	reduxStack.AddClass("redux-data")
	reduxDs.Append(compton.FICenter(p, reduxStack))

	for _, property := range reduxProperties {
		if values, ok := rdx.GetAllValues(property, gogId); ok {

			open := !slices.Contains(vangogh_integration.LongTextProperties(), property) &&
				!slices.Contains(vangogh_integration.DehydratedImagesProperties(), property)

			ds := compton.DSSmall(p, property, open)

			ul := compton.Ul()
			for _, value := range values {
				ul.Append(compton.ListItemText(value))
			}

			ds.Append(ul)

			reduxStack.Append(ds)
		}
	}

	return p, nil
}

func productTypeSection(r compton.Registrar, id string, pt vangogh_integration.ProductType) compton.Element {
	ds := compton.DSLarge(r, compton_data.TypesTitles[pt.String()], false).BackgroundColor(color.Highlight)

	ds.SetLabelText(pt.String())

	iframe := compton.IframeExpandHost(r, pt.String(), "/debug-data?id="+id+"&product-type="+pt.String())
	ds.Append(iframe)

	return ds
}
