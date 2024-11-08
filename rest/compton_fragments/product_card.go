package compton_fragments

import (
	_ "embed"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"golang.org/x/exp/slices"
	"strings"
)

func SummarizeProductProperties(id string, rdx kevlar.ReadableRedux) ([]string, map[string][]string) {
	properties := make([]string, 0)
	values := make(map[string][]string)

	if oses, ok := rdx.GetAllValues(vangogh_local_data.OperatingSystemsProperty, id); ok {
		properties = append(properties, vangogh_local_data.OperatingSystemsProperty)
		values[vangogh_local_data.OperatingSystemsProperty] = oses
	}

	if developers, ok := rdx.GetAllValues(vangogh_local_data.DevelopersProperty, id); ok {
		properties = append(properties, vangogh_local_data.DevelopersProperty)
		values[vangogh_local_data.DevelopersProperty] = developers
	}

	if publishers, ok := rdx.GetAllValues(vangogh_local_data.PublishersProperty, id); ok {
		properties = append(properties, vangogh_local_data.PublishersProperty)
		values[vangogh_local_data.PublishersProperty] = publishers
	}

	return properties, values
}

func ProductCard(r compton.Registrar, id string, hydrated bool, rdx kevlar.ReadableRedux) compton.Element {

	pc := compton.Card(r, id)

	if rc, ok := rdx.GetLastVal(vangogh_local_data.RepVerticalImageColorProperty, id); ok {
		pc.SetAttribute("style", "background-color:color-mix(in display-p3,"+rc+" var(--cma), var(--c-background))")

	}

	if viSrc, ok := rdx.GetLastVal(vangogh_local_data.VerticalImageProperty, id); ok {

		posterUrl := "/image?id=" + viSrc
		dhSrc, _ := rdx.GetLastVal(vangogh_local_data.DehydratedVerticalImageProperty, id)
		placeholderSrc := dhSrc
		repColor, _ := rdx.GetLastVal(vangogh_local_data.RepVerticalImageColorProperty, id)
		pc.AppendPoster(repColor, placeholderSrc, posterUrl, hydrated)

		pc.WidthPixels(85.5)
		pc.HeightPixels(120.5)
	}

	if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
		pc.AppendTitle(title)
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
		case vangogh_local_data.OperatingSystemsProperty:
			osValues := vangogh_local_data.ParseManyOperatingSystems(values[p])
			for _, os := range compton_data.OSOrder {
				if slices.Contains(osValues, os) {
					osSymbols = append(osSymbols, compton.SvgUse(r, compton_data.OperatingSystemSymbols[os]))
				}
			}
			pc.AppendProperty(compton_data.PropertyTitles[vangogh_local_data.OperatingSystemsProperty], osSymbols...)
		default:
			pc.AppendProperty(compton_data.PropertyTitles[p], compton.Text(strings.Join(values[p], ", ")))
		}
	}

	return pc

}
