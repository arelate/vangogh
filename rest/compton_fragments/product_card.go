package compton_fragments

import (
	_ "embed"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
	"slices"
	"strings"
)

func SummarizeProductProperties(id string, rdx redux.Readable) ([]string, map[string][]string) {
	properties := make([]string, 0)
	values := make(map[string][]string)

	if oses, ok := rdx.GetAllValues(vangogh_integration.OperatingSystemsProperty, id); ok {
		properties = append(properties, vangogh_integration.OperatingSystemsProperty)
		values[vangogh_integration.OperatingSystemsProperty] = oses
	}

	if developers, ok := rdx.GetAllValues(vangogh_integration.DevelopersProperty, id); ok {
		properties = append(properties, vangogh_integration.DevelopersProperty)
		values[vangogh_integration.DevelopersProperty] = developers
	}

	if publishers, ok := rdx.GetAllValues(vangogh_integration.PublishersProperty, id); ok {
		properties = append(properties, vangogh_integration.PublishersProperty)
		values[vangogh_integration.PublishersProperty] = publishers
	}

	return properties, values
}

func ProductCard(r compton.Registrar, id string, hydrated bool, rdx redux.Readable) compton.Element {

	pc := compton.Card(r, id)

	if repColor, ok := rdx.GetLastVal(vangogh_integration.RepImageColorProperty, id); ok && repColor != issa.NeutralRepColor {
		compton.SetTint(pc, repColor)
	}

	if viSrc, ok := rdx.GetLastVal(vangogh_integration.VerticalImageProperty, id); ok {

		posterUrl := "/image?id=" + viSrc
		dhSrc, _ := rdx.GetLastVal(vangogh_integration.DehydratedVerticalImageProperty, id)
		placeholderSrc := dhSrc
		repColor, _ := rdx.GetLastVal(vangogh_integration.RepVerticalImageColorProperty, id)
		pc.AppendPoster(repColor, placeholderSrc, posterUrl, hydrated)

		pc.WidthPixels(85.5)
		pc.HeightPixels(120.5)
	}

	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		pc.AppendTitle(title)
	} else {
		return nil
	}

	if labels := compton.Labels(r, FormatLabels(id, rdx)...).
		FontSize(size.XSmall).
		ColumnGap(size.XXSmall).
		RowGap(size.XXSmall); labels != nil {
		pc.AppendLabels(labels)
	}

	properties, values := SummarizeProductProperties(id, rdx)
	osSymbols := make([]compton.Element, 0, 2)

	for _, p := range properties {
		switch p {
		case vangogh_integration.OperatingSystemsProperty:
			osValues := vangogh_integration.ParseManyOperatingSystems(values[p])
			for _, os := range compton_data.OSOrder {
				if slices.Contains(osValues, os) {
					osSymbols = append(osSymbols, compton.SvgUse(r, compton_data.OperatingSystemSymbols[os]))
				}
			}
			pc.AppendProperty(compton_data.PropertyTitles[vangogh_integration.OperatingSystemsProperty], osSymbols...)
		default:
			pc.AppendProperty(compton_data.PropertyTitles[p], compton.Text(strings.Join(values[p], ", ")))
		}
	}

	return pc

}
