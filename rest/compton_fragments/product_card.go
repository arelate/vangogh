package compton_fragments

import (
	_ "embed"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"slices"
	"strings"
)

func ProductCard(r compton.Registrar, id string, hydrated bool, rdx kevlar.ReadableRedux) compton.Element {

	pc := compton.Card(r, id)

	if viSrc, ok := rdx.GetLastVal(vangogh_local_data.VerticalImageProperty, id); ok {

		posterUrl := "/image?id=" + viSrc
		dhSrc, _ := rdx.GetLastVal(vangogh_local_data.DehydratedVerticalImageProperty, id)
		placeholderSrc := dhSrc
		pc.AppendPoster(placeholderSrc, posterUrl, hydrated)

		pc.WidthPixels(85.5)
		pc.HeightPixels(120.5)
	}

	if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
		pc.AppendTitle(title)
	}

	if labels := compton.Labels(r,
		FormatLabels(id, rdx, compton_data.LabelProperties...)...).
		FontSize(size.XSmall).
		ColumnGap(size.XXSmall).
		RowGap(size.XXSmall); labels != nil {
		pc.AppendLabels(labels)
	}

	osSymbols := make([]compton.Element, 0, 2)
	osOrder := []vangogh_local_data.OperatingSystem{
		vangogh_local_data.Windows,
		vangogh_local_data.MacOS,
		vangogh_local_data.Linux}
	if oses, ok := rdx.GetAllValues(vangogh_local_data.OperatingSystemsProperty, id); ok {
		pOses := vangogh_local_data.ParseManyOperatingSystems(oses)
		for _, os := range osOrder {
			if slices.Contains(pOses, os) {
				osSymbols = append(osSymbols, compton.SvgUse(r, compton_data.OperatingSystemSymbols[os]))
			}
		}
	}
	if len(osSymbols) > 0 {
		pc.AppendProperty(compton_data.PropertyTitles[vangogh_local_data.OperatingSystemsProperty], osSymbols...)
	}

	if developers, ok := rdx.GetAllValues(vangogh_local_data.DevelopersProperty, id); ok {
		pc.AppendProperty(compton_data.PropertyTitles[vangogh_local_data.DevelopersProperty], compton.Text(strings.Join(developers, ", ")))
	}

	if publishers, ok := rdx.GetAllValues(vangogh_local_data.PublishersProperty, id); ok {
		pc.AppendProperty(compton_data.PropertyTitles[vangogh_local_data.PublishersProperty], compton.Text(strings.Join(publishers, ", ")))
	}

	return pc

}
