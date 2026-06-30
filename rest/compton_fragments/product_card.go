package compton_fragments

import (
	"slices"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func SummarizeGogProductProperties(id string, rdx redux.Readable) ([]string, map[string][]string) {
	properties := []string{
		vangogh_integration.GogOperatingSystemsProperty,
		vangogh_integration.GogDevelopersProperty,
		vangogh_integration.GogPublishersProperty,
	}
	values := make(map[string][]string)

	var oses []string
	if osp, ok := rdx.GetAllValues(vangogh_integration.GogOperatingSystemsProperty, id); ok {
		oses = osp
	}
	values[vangogh_integration.GogOperatingSystemsProperty] = oses

	var developers []string
	if dp, ok := rdx.GetAllValues(vangogh_integration.GogDevelopersProperty, id); ok {
		developers = dp
	}
	values[vangogh_integration.GogDevelopersProperty] = developers

	var publishers []string
	if pp, ok := rdx.GetAllValues(vangogh_integration.GogPublishersProperty, id); ok {
		publishers = pp
	}
	values[vangogh_integration.GogPublishersProperty] = publishers

	return properties, values
}

func GogProductCard(r compton.Registrar, id string, rdx redux.Readable, permissions ...author.Permission) compton.Element {

	pc := compton.Card(r, id)

	var imageUrl string

	if verticalImageId, ok := rdx.GetLastVal(vangogh_integration.GogVerticalImageProperty, id); ok && verticalImageId != "" {
		imageUrl = "/image?id=" + verticalImageId
	}

	pc.AppendImage(imageUrl, 85.5, 120.5)

	if title, ok := rdx.GetLastVal(vangogh_integration.GogTitleProperty, id); ok {
		pc.AppendTitle(title)
	} else {
		return nil
	}

	fmtBadges := FormatBadges(id, rdx, compton_data.InformationBadgeProperties, permissions...)

	pc.AppendBadges(compton.Badges(r, fmtBadges...))

	properties, values := SummarizeGogProductProperties(id, rdx)
	osSymbols := make([]compton.Element, 0, 2)

	for _, p := range properties {
		switch p {
		case vangogh_integration.GogOperatingSystemsProperty:
			osValues := vangogh_integration.ParseManyOperatingSystems(values[p])
			for _, os := range compton_data.OSOrder {
				if slices.Contains(osValues, os) {
					osSymbols = append(osSymbols, compton.SvgUse(r, compton_data.OperatingSystemSymbols[os]))
				}
			}
			pc.AppendProperty(compton_data.PropertyTitles[p], osSymbols...)
		default:
			pc.AppendProperty(compton_data.ShortPropertyTitles[p], compton.Text(strings.Join(values[p], ", ")))
		}
	}

	return pc

}
