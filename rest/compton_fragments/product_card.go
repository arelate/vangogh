package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
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

	if verticalImageId, ok := rdx.GetLastVal(vangogh_integration.VerticalImageProperty, id); ok {

		imageUrl := "/image?id=" + verticalImageId
		dehydratedImage, _ := rdx.GetLastVal(vangogh_integration.DehydratedImageProperty, verticalImageId)

		var repColor = issa.NeutralRepColor

		if rp, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, verticalImageId); sure && rp != issa.NeutralRepColor {
			repColor = rp
		}

		pc.SetAttribute("style", "--c-rep:"+repColor)
		pc.AppendPoster(repColor, dehydratedImage, imageUrl, hydrated)

		pc.WidthPixels(85.5)
		pc.HeightPixels(120.5)
	}

	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		pc.AppendTitle(title)
	} else {
		return nil
	}

	productBadges := compton.FlexItems(r, direction.Row).
		RowGap(size.XXSmall).
		ColumnGap(size.XXSmall).
		JustifyContent(align.Start).
		Width(size.FullWidth)
	//productBadges.SetAttribute("style", "view-transition-name:product-badges-"+id)

	for _, fmtBadge := range FormatBadges(id, rdx) {
		var badge *compton.FspanElement
		if fmtBadge.Title != "" && fmtBadge.Icon == compton.NoSymbol {
			badge = compton.SmallBadge(r, fmtBadge.Title, fmtBadge.Background, fmtBadge.Foreground)
		} else if fmtBadge.Icon != compton.NoSymbol {
			badge = compton.SmallBadgeIcon(r, fmtBadge.Icon, fmtBadge.Background, fmtBadge.Foreground)
		}
		if badge != nil {
			badge.AddClass(fmtBadge.Class)
			productBadges.Append(badge)
		}
	}

	pc.AppendBadges(productBadges)

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
			pc.AppendProperty(compton_data.PropertyTitles[p], osSymbols...).
				SetAttribute("style", "view-transition-name:"+p+id)
		default:
			pc.AppendProperty(compton_data.ShortPropertyTitles[p], compton.Text(strings.Join(values[p], ", "))).
				SetAttribute("style", "view-transition-name:"+p+id)
		}
	}

	return pc

}
